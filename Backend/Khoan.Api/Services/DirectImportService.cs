using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Khoan.Api.Data;
using Khoan.Api.Models;
using Khoan.Api.Models.Configuration;
using Khoan.Api.Models.Entities;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Models.CsvModels;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    /// <summary>
    /// DirectImportService - Handles CSV direct import cho DP01, DPDA, LN03
    /// CSV-First: Import trực tiếp từ CSV vào database với validation
    /// </summary>
    public class DirectImportService : IDirectImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DirectImportService> _logger;
        private readonly DirectImportSettings _settings;
        // Cache property reflection để giảm overhead per-batch
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _propertyCache = new();
    private readonly IImportMetrics? _metrics; // optional metrics provider

        // Limit concurrent heavy import operations to protect SQL Server
        private static readonly SemaphoreSlim ImportSemaphore = new(2, 2); // allow up to 2 concurrent imports

        public DirectImportService(
            ApplicationDbContext context,
            ILogger<DirectImportService> logger,
            IOptions<DirectImportSettings> settings,
            IImportMetrics? metrics = null)
        {
            _context = context;
            _logger = logger;
            _settings = settings.Value;
            _metrics = metrics;
        }

        private int ResolveBatchSize(string table, int defaultValue)
        {
            try
            {
                // Cho phép override qua ENV: DirectImport__BatchSize__{TABLE}
                var envKey = $"DirectImport__BatchSize__{table.ToUpper()}";
                var envVal = Environment.GetEnvironmentVariable(envKey);
                if (!string.IsNullOrWhiteSpace(envVal) && int.TryParse(envVal, out var parsed) && parsed > 0) return parsed;
                // Cho phép override chung: DirectImport__BatchSize__Default
                var envDefault = Environment.GetEnvironmentVariable("DirectImport__BatchSize__Default");
                if (!string.IsNullOrWhiteSpace(envDefault) && int.TryParse(envDefault, out var parsedDefault) && parsedDefault > 0) return parsedDefault;
            }
            catch { /* ignore */ }
            return defaultValue;
        }

        private int ResolveProgressInterval()
        {
            try
            {
                var env = Environment.GetEnvironmentVariable("DirectImport__ProgressInterval");
                if (!string.IsNullOrWhiteSpace(env) && int.TryParse(env, out var parsed) && parsed > 0) return parsed;
            }
            catch { }
            return 50_000; // mặc định 50k dòng
        }

        private int ResolveAbortThreshold(string table, int defaultValue)
        {
            try
            {
                var envKey = $"DirectImport__AbortErrors__{table.ToUpper()}";
                var envVal = Environment.GetEnvironmentVariable(envKey);
                if (!string.IsNullOrWhiteSpace(envVal) && int.TryParse(envVal, out var parsed) && parsed > 0) return parsed;
                var envCommon = Environment.GetEnvironmentVariable("DirectImport__AbortErrors__Default");
                if (!string.IsNullOrWhiteSpace(envCommon) && int.TryParse(envCommon, out var parsedCommon) && parsedCommon > 0) return parsedCommon;
            }
            catch { }
            return defaultValue;
        }

        public interface IImportMetrics
        {
            void AddImported(string table, int count);
            void ObserveDuration(string table, double seconds);
        }

        #region Generic Import Methods

        /// <summary>
        /// Generic CSV import cho bất kỳ entity type nào
        /// </summary>
        public async Task<DirectImportResult> ImportFromCsvAsync<T>(IFormFile file, string dataType, string? statementDate = null) where T : class
        {
            _logger.LogInformation("🚀 [GENERIC] Import {DataType} from CSV: {FileName}", dataType, file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = dataType,
                TargetTable = typeof(T).Name.Replace("Entity", ""),
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Parse CSV generic
                var records = await ParseCsvGenericAsync<T>(file);
                _logger.LogInformation("📊 [GENERIC] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Bulk insert
                    var insertedCount = await BulkInsertGenericAsync(records, typeof(T).Name);
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GENERIC] Import thành công {Count} records vào {Table}", insertedCount, typeof(T).Name);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy records hợp lệ trong CSV file");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GENERIC] Import error cho {DataType}: {Error}", dataType, ex.Message);
                result.Success = false;
                result.Errors.Add($"Import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// Generic import method - route tới specific implementations
        /// </summary>
        public async Task<DirectImportResult> ImportGenericAsync(IFormFile file, string dataType, string? statementDate = null)
        {
            // Throttle server-wide import concurrency to avoid DB overload and connection resets
            await ImportSemaphore.WaitAsync();
            try
            {
                return dataType.ToUpper() switch
                {
                    "DP01" => await ImportDP01Async(file, statementDate),
                    "DPDA" => await ImportDPDAAsync(file, statementDate),
                    "EI01" => await ImportEI01Async(file, statementDate),
                    "LN01" => await ImportLN01Async(file, statementDate),
                    "LN03" => await ImportLN03EnhancedAsync(file, statementDate),
                    "GL01" => await ImportGL01Async(file, statementDate),
                    "GL02" => await ImportGL02Async(file, statementDate),
                    "GL41" => await ImportGL41Async(file, statementDate),
                    "RR01" => await ImportRR01Async(file, statementDate),
                    _ => throw new NotSupportedException($"DataType '{dataType}' chưa được hỗ trợ")
                };
            }
            finally
            {
                ImportSemaphore.Release();
            }
        }

        #endregion

        #region DP01 Import

        /// <summary>
        /// Import DP01 từ CSV - 63 business columns
        /// </summary>
        public async Task<DirectImportResult> ImportDP01Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [DP01] Import DP01 from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "DP01",
                TargetTable = "DP01",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // STREAMING PARSE + BATCH BULK INSERT (giảm memory cho file rất lớn)
                var BATCH_SIZE = ResolveBatchSize("DP01", 5000);
                var progressInterval = ResolveProgressInterval();
                var totalInserted = 0;
                var totalRead = 0;
                var batch = new List<DP01>(BATCH_SIZE);
                var sw = Stopwatch.StartNew();
                var ngayDlDate = DateTime.TryParse(result.NgayDL, out var _ng) ? _ng : DateTime.Today;

                var consecutiveErrors = 0;
                var maxConsecutiveErrors = 1000; // EARLY ABORT NGƯỠNG
                await foreach (var rec in StreamParseDP01Async(file))
                {
                    rec.NGAY_DL = ngayDlDate; // enforce từ filename
                    try
                    {
                        batch.Add(rec);
                        consecutiveErrors = 0; // reset on success add
                    }
                    catch
                    {
                        consecutiveErrors++;
                        if (consecutiveErrors >= maxConsecutiveErrors)
                        {
                            _logger.LogError("❌ [DP01] Early abort: {Errors} lỗi liên tiếp khi xử lý batch", consecutiveErrors);
                            break;
                        }
                        continue;
                    }
                    totalRead++;
                    if (totalRead % progressInterval == 0)
                    {
                        _logger.LogInformation("⏱️ [DP01] Đã đọc {Total} dòng (inserted {Inserted})", totalRead, totalInserted);
                    }
                    if (batch.Count >= BATCH_SIZE)
                    {
                        totalInserted += await BulkInsertGenericAsync(batch, "DP01");
                        batch.Clear();
                        if (totalInserted % progressInterval == 0)
                        {
                            _logger.LogInformation("💾 [DP01] Đã chèn {Inserted} dòng", totalInserted);
                        }
                    }
                }
                if (batch.Count > 0)
                {
                    totalInserted += await BulkInsertGenericAsync(batch, "DP01");
                    batch.Clear();
                }

                sw.Stop();
                var elapsedSec = Math.Max(0.001, sw.Elapsed.TotalSeconds);
                var rps = (int)(totalInserted / elapsedSec);
                _metrics?.ObserveDuration("DP01", elapsedSec);
                result.ProcessedRecords = totalInserted;
                result.Success = totalInserted > 0;
                if (result.Success)
                {
                    _logger.LogInformation("✅ [DP01] Streaming import thành công {Count} records trong {Seconds:F2}s (~{Rps}/sec)", totalInserted, sw.Elapsed.TotalSeconds, rps);
                    await LogImportMetadataAsync(result);
                    await PersistHistoryAsync("DP01", file.FileName, result.NgayDL, file.Length, totalInserted, elapsedSec, "SUCCESS", null, BATCH_SIZE, progressInterval, maxConsecutiveErrors);
                }
                else
                {
                    result.Errors.Add("Không tìm thấy DP01 records hợp lệ trong CSV (stream)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DP01] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"DP01 import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region DPDA Import

        /// <summary>
        /// Import DPDA từ CSV - 13 business columns
        /// </summary>
        public async Task<DirectImportResult> ImportDPDAAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [DPDA] Import DPDA from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "DPDA",
                TargetTable = "DPDA",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Parse DPDA CSV
                var records = await ParseDPDACsvAsync(file);
                _logger.LogInformation("📊 [DPDA] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Bulk insert DPDA
                    var insertedCount = await BulkInsertGenericAsync(records, "DPDA");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [DPDA] Import thành công {Count} records", insertedCount);
                    await LogImportMetadataAsync(result);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy DPDA records hợp lệ trong CSV");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DPDA] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"DPDA import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region EI01 Import

        /// <summary>
        /// Import EI01 từ CSV - 24 business columns
        /// </summary>
        public async Task<DirectImportResult> ImportEI01Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [EI01] Import EI01 from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "EI01",
                TargetTable = "EI01",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Strict: only allow filename containing "ei01"
                if (!file.FileName.ToLower().Contains("ei01"))
                {
                    throw new InvalidOperationException($"Filename '{file.FileName}' is not allowed. Only files containing 'ei01' are accepted.");
                }

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Parse EI01 CSV
                var records = await ParseEI01CsvAsync(file);
                _logger.LogInformation("📊 [EI01] Đã parse {Count} records từ CSV", records.Count);
                if (records.Count == 0)
                {
                    _logger.LogWarning("⚠️ [EI01] Parse trả về 0 record. Kiểm tra header CSV có khớp với entity không và định dạng ngày (yyyyMMdd/dd/MM/yyyy)");
                }

                if (records.Any())
                {
                    // Set NGAY_DL for all records from filename (CSV-first rule)
                    if (DateTime.TryParse(result.NgayDL, out var ngayDlDate))
                    {
                        foreach (var r in records)
                        {
                            r.NGAY_DL = ngayDlDate;
                        }
                    }

                    // Bulk insert EI01
                    var insertedCount = await BulkInsertGenericAsync(records, "EI01");
                    result.ProcessedRecords = insertedCount;
                    result.Success = insertedCount > 0;
                    if (insertedCount > 0)
                    {
                        _logger.LogInformation("✅ [EI01] Import thành công {Count} records", insertedCount);
                        await LogImportMetadataAsync(result);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ [EI01] Import không chèn được bản ghi nào");
                    }
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy EI01 records hợp lệ trong CSV");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [EI01] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"EI01 import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region LN01 Import

        /// <summary>
        /// Import LN01 từ CSV - 79 business columns (Loan Data)
        /// </summary>
        public async Task<DirectImportResult> ImportLN01Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [LN01] Import LN01 from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "LN01",
                TargetTable = "LN01",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Validate filename contains "ln01"
                if (!file.FileName.ToLower().Contains("ln01"))
                {
                    result.Success = false;
                    result.Errors.Add("File name phải chứa 'ln01' để import LN01 data");
                    return result;
                }

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // STREAMING LN01 (tối ưu bộ nhớ cho file lớn)
                var batchSize = ResolveBatchSize("LN01", 6000); // mặc định 6k
                var progressInterval = ResolveProgressInterval();
                var batch = new List<LN01>(batchSize);
                var totalInserted = 0;
                var totalRead = 0;
                var sw = Stopwatch.StartNew();
                var consecutiveErrors = 0;
                var maxConsecutiveErrors = ResolveAbortThreshold("LN01", 1500);
                var ngayDlDate = DateTime.TryParse(result.NgayDL, out var _ng) ? _ng : DateTime.Today;

                await foreach (var rec in StreamParseLN01Async(file))
                {
                    rec.NGAY_DL = ngayDlDate;
                    try
                    {
                        batch.Add(rec);
                        consecutiveErrors = 0;
                    }
                    catch
                    {
                        consecutiveErrors++;
                        if (consecutiveErrors >= maxConsecutiveErrors)
                        {
                            _logger.LogError("❌ [LN01] Early abort: {Errors} lỗi liên tiếp", consecutiveErrors);
                            break;
                        }
                        continue;
                    }
                    totalRead++;
                    if (totalRead % progressInterval == 0)
                        _logger.LogInformation("⏱️ [LN01] Đã đọc {Read} dòng (inserted {Ins})", totalRead, totalInserted);
                    if (batch.Count >= batchSize)
                    {
                        var inserted = await BulkInsertGenericAsync(batch, "LN01");
                        totalInserted += inserted;
                        _metrics?.AddImported("LN01", inserted); // metrics hook
                        batch.Clear();
                        if (totalInserted % progressInterval == 0)
                            _logger.LogInformation("💾 [LN01] Đã chèn {Ins} dòng", totalInserted);
                    }
                }
                if (batch.Count > 0)
                {
                    var inserted = await BulkInsertGenericAsync(batch, "LN01");
                    totalInserted += inserted;
                    _metrics?.AddImported("LN01", inserted);
                    batch.Clear();
                }
                sw.Stop();
                var elapsedSec = Math.Max(0.001, sw.Elapsed.TotalSeconds);
                var rps = (int)(totalInserted / elapsedSec);
                result.ProcessedRecords = totalInserted;
                result.Success = totalInserted > 0;
                if (result.Success)
                {
                    _logger.LogInformation("✅ [LN01] Streaming import thành công {Count} records trong {Sec:F2}s (~{Rps}/sec)", totalInserted, sw.Elapsed.TotalSeconds, rps);
                    await LogImportMetadataAsync(result);
                    await PersistHistoryAsync("LN01", file.FileName, result.NgayDL, file.Length, totalInserted, elapsedSec, "SUCCESS", null, batchSize, progressInterval, maxConsecutiveErrors);
                }
                else
                {
                    result.Errors.Add("Không tìm thấy LN01 records hợp lệ (stream)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [LN01] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"LN01 import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region LN03 Import

        /// <summary>
        /// Import LN03 Enhanced - từ disabled extension
        /// </summary>
    public async Task<DirectImportResult> ImportLN03EnhancedAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [LN03_ENHANCED] Import LN03 with enhanced processing");

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "LN03",
                TargetTable = "LN03",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Strict: only allow filename containing "ln03"
                if (!file.FileName.ToLower().Contains("ln03"))
                {
                    throw new InvalidOperationException($"Filename '{file.FileName}' is not allowed. Only files containing 'ln03' are accepted.");
                }

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Parse LN03 CSV với enhanced processing
                // Streaming import theo batch, không xóa dữ liệu cũ (theo yêu cầu)
                var insertedTotal = 0;
                const int BATCH_SIZE = 5000;
                var batch = new List<LN03>(BATCH_SIZE);

                // Parse stream
                var parsed = await ParseLN03EnhancedAsync(file, statementDate);
                if (parsed.Count == 0)
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy LN03 records hợp lệ trong CSV");
                    return result;
                }

                if (!DateTime.TryParse(result.NgayDL, out var ngayDlDate))
                    ngayDlDate = DateTime.Today;

                // Nạp theo batch
                foreach (var r in parsed)
                {
                    r.NGAY_DL = ngayDlDate;
                    r.CREATED_DATE = DateTime.UtcNow;
                    r.FILE_ORIGIN = file.FileName;
                    batch.Add(r);

                    if (batch.Count >= BATCH_SIZE)
                    {
                        // Bulk insert bằng SqlBulkCopy (qua BulkInsertGenericAsync)
                        insertedTotal += await BulkInsertGenericAsync(batch, "LN03");
                        batch.Clear();
                    }
                }
                if (batch.Count > 0)
                {
                    insertedTotal += await BulkInsertGenericAsync(batch, "LN03");
                    batch.Clear();
                }

                result.ProcessedRecords = insertedTotal;
                result.Success = insertedTotal > 0;
                if (insertedTotal > 0)
                {
                    _logger.LogInformation("✅ [LN03_ENHANCED] Import thành công {Count} records", insertedTotal);
                    await LogImportMetadataAsync(result);
                }
                else
                {
                    _logger.LogWarning("⚠️ [LN03_ENHANCED] Không chèn được bản ghi nào");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [LN03_ENHANCED] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"LN03 enhanced import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region GL02 Import

        /// <summary>
        /// Import GL02 từ CSV - 17 business columns; NGAY_DL lấy từ TRDATE; non-temporal
        /// </summary>
        public async Task<DirectImportResult> ImportGL02Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL02] Import GL02 from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "GL02",
                TargetTable = "GL02",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Strict: only allow filename containing "gl02"
                if (!file.FileName.ToLower().Contains("gl02"))
                {
                    throw new InvalidOperationException($"Filename '{file.FileName}' is not allowed. Only files containing 'gl02' are accepted.");
                }

                // Streaming + batch (tương tự GL01/LN03) – giảm memory cho file > vài trăm MB
                var BATCH_SIZE = ResolveBatchSize("GL02", 5000);
                var progressInterval = ResolveProgressInterval();
                var batch = new List<Models.DataTables.GL02>(BATCH_SIZE);
                var totalInserted = 0;
                var totalRead = 0;
                var sw = Stopwatch.StartNew();
                var consecutiveErrors = 0;
                var maxConsecutiveErrors = 800; // GL02 nhỏ hơn
                await foreach (var rec in StreamParseGL02Async(file))
                {
                    try
                    {
                        batch.Add(rec);
                        consecutiveErrors = 0;
                    }
                    catch
                    {
                        consecutiveErrors++;
                        if (consecutiveErrors >= maxConsecutiveErrors)
                        {
                            _logger.LogError("❌ [GL02] Early abort: {Errors} lỗi liên tiếp khi xử lý batch", consecutiveErrors);
                            break;
                        }
                        continue;
                    }
                    totalRead++;
                    if (totalRead % progressInterval == 0)
                        _logger.LogInformation("⏱️ [GL02] Đã đọc {Total} dòng (inserted {Inserted})", totalRead, totalInserted);
                    if (batch.Count >= BATCH_SIZE)
                    {
                        totalInserted += await BulkInsertGenericAsync(batch, "GL02");
                        batch.Clear();
                        if (totalInserted % progressInterval == 0)
                            _logger.LogInformation("💾 [GL02] Đã chèn {Inserted} dòng", totalInserted);
                    }
                }
                if (batch.Count > 0)
                {
                    totalInserted += await BulkInsertGenericAsync(batch, "GL02");
                    batch.Clear();
                }
                sw.Stop();
                var elapsedSec = Math.Max(0.001, sw.Elapsed.TotalSeconds);
                var rps = (int)(totalInserted / elapsedSec);
                _metrics?.ObserveDuration("GL02", elapsedSec);
                result.ProcessedRecords = totalInserted;
                result.Success = totalInserted > 0;
                if (result.Success)
                {
                    _logger.LogInformation("✅ [GL02] Streaming import thành công {Count} records trong {Seconds:F2}s (~{Rps}/sec)", totalInserted, sw.Elapsed.TotalSeconds, rps);
                    await LogImportMetadataAsync(result);
                    await PersistHistoryAsync("GL02", file.FileName, result.NgayDL, file.Length, totalInserted, elapsedSec, "SUCCESS", null, BATCH_SIZE, progressInterval, maxConsecutiveErrors);
                }
                else
                {
                    result.Errors.Add("Không tìm thấy GL02 records hợp lệ trong CSV (stream)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL02] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"GL02 import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region GL41 Import

        /// <summary>
        /// Import GL41 từ CSV - 13 business columns; NGAY_DL lấy từ filename; temporal table
        /// </summary>
        public async Task<DirectImportResult> ImportGL41Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL41] Import GL41 from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "GL41",
                TargetTable = "GL41",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Strict: only allow filename containing "gl41"
                if (!file.FileName.ToLower().Contains("gl41"))
                {
                    throw new InvalidOperationException($"Filename '{file.FileName}' is not allowed. Only files containing 'gl41' are accepted.");
                }

                // Parse GL41 CSV
                var records = await ParseGL41CsvAsync(file);
                _logger.LogInformation("📊 [GL41] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Bulk insert GL41
                    var insertedCount = await BulkInsertGenericAsync(records, "GL41");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GL41] Import thành công {Count} records", insertedCount);
                    // Extract NGAY_DL for metadata
                    result.NgayDL = ExtractNgayDLFromFileName(file.FileName);
                    await LogImportMetadataAsync(result);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy GL41 records hợp lệ trong CSV");
                }
            }
            catch (Exception ex)
            {
                var root = ex.GetBaseException();
                var details = $"{ex.Message}" + (root != null && root != ex ? $" | Root: {root.Message}" : string.Empty);
                _logger.LogError(ex, "❌ [GL41] Import error: {Error}", details);
                result.Success = false;
                result.Errors.Add($"GL41 import error: {details}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region GL01 Import

        /// <summary>
        /// Import GL01 từ CSV - 27 business columns - HEAVY FILE OPTIMIZED
        /// MaxFileSize 2GB, BulkInsert BatchSize 10,000, Upload timeout 15 minutes
        /// NGAY_DL từ TR_TIME column (column 25)
        /// </summary>
        public async Task<DirectImportResult> ImportGL01Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL01] Import GL01 from CSV (HEAVY FILE OPTIMIZED): {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "GL01",
                TargetTable = "GL01",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Strict: only allow filename containing "gl01"
                if (!file.FileName.ToLower().Contains("gl01"))
                {
                    throw new InvalidOperationException($"Filename '{file.FileName}' is not allowed. Only files containing 'gl01' are accepted.");
                }

                // Heavy file validation (MaxFileSize 2GB)
                if (file.Length > 2L * 1024 * 1024 * 1024) // 2GB
                {
                    throw new InvalidOperationException($"File size {file.Length / (1024 * 1024)}MB exceeds maximum allowed size of 2GB for GL01.");
                }

                _logger.LogInformation("📊 [GL01] Heavy file processing - Size: {Size}MB", file.Length / (1024 * 1024));

                // Full STREAMING thay vì load toàn bộ list vào RAM
                var BATCH_SIZE = ResolveBatchSize("GL01", 10000);
                var progressInterval = ResolveProgressInterval();
                var batch = new List<GL01>(BATCH_SIZE);
                var totalInserted = 0;
                var totalRead = 0;
                var sw = Stopwatch.StartNew();
                var consecutiveErrors = 0;
                var maxConsecutiveErrors = 1500; // GL01 có thể rất lớn
                await foreach (var rec in StreamParseGL01Async(file))
                {
                    try
                    {
                        batch.Add(rec);
                        consecutiveErrors = 0;
                    }
                    catch
                    {
                        consecutiveErrors++;
                        if (consecutiveErrors >= maxConsecutiveErrors)
                        {
                            _logger.LogError("❌ [GL01] Early abort: {Errors} lỗi liên tiếp khi xử lý batch", consecutiveErrors);
                            break;
                        }
                        continue;
                    }
                    totalRead++;
                    if (totalRead % progressInterval == 0)
                        _logger.LogInformation("⏱️ [GL01] Đã đọc {Total} dòng (inserted {Inserted})", totalRead, totalInserted);
                    if (batch.Count >= BATCH_SIZE)
                    {
                        totalInserted += await BulkInsertGenericAsync(batch, "GL01");
                        batch.Clear();
                        if (totalInserted % progressInterval == 0)
                            _logger.LogInformation("💾 [GL01] Đã chèn {Inserted} dòng", totalInserted);
                    }
                }
                if (batch.Count > 0)
                {
                    totalInserted += await BulkInsertGenericAsync(batch, "GL01");
                    batch.Clear();
                }
                sw.Stop();
                var elapsedSec = Math.Max(0.001, sw.Elapsed.TotalSeconds);
                var rps = (int)(totalInserted / elapsedSec);
                _metrics?.ObserveDuration("GL01", elapsedSec);
                result.ProcessedRecords = totalInserted;
                result.Success = totalInserted > 0;
                if (result.Success)
                {
                    _logger.LogInformation("✅ [GL01] Streaming import thành công {Count} records trong {Seconds:F2}s (~{Rps}/sec)", totalInserted, sw.Elapsed.TotalSeconds, rps);
                    await LogImportMetadataAsync(result);
                    await PersistHistoryAsync("GL01", file.FileName, result.NgayDL, file.Length, totalInserted, elapsedSec, "SUCCESS", null, BATCH_SIZE, progressInterval, maxConsecutiveErrors);
                }
                else
                {
                    result.Errors.Add("Không tìm thấy GL01 records hợp lệ trong CSV (stream)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL01] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"GL01 import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Extract NgayDL từ filename patterns như 7800_dp01_20241231.csv
        /// </summary>
        public string ExtractNgayDLFromFileName(string fileName)
        {
            try
            {
                // Pattern: 7800_[type]_YYYYMMDD.csv
                var regex = new Regex(@"7800_\w+_(\d{8})\.csv", RegexOptions.IgnoreCase);
                var match = regex.Match(fileName);

                if (match.Success)
                {
                    var dateString = match.Groups[1].Value;
                    if (DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        return date.ToString("yyyy-MM-dd");
                    }
                }

                _logger.LogWarning("⚠️ Cannot extract NgayDL from filename: {FileName}", fileName);
                return DateTime.Today.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error extracting NgayDL from filename: {FileName}", fileName);
                return DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        private async Task PersistHistoryAsync(string dataType, string fileName, string? ngayDl, long fileSize, int inserted, double durationSec, string status, string? error, int batchSize, int progressInterval, int? abortThreshold)
        {
            try
            {
                var entity = new Khoan.Api.Models.Importing.ImportHistory
                {
                    DataType = dataType,
                    FileName = fileName,
                    NgayDL = ngayDl,
                    FileSizeBytes = fileSize,
                    RecordsInserted = inserted,
                    DurationSeconds = durationSec,
                    StartedUtc = DateTime.UtcNow.AddSeconds(-durationSec),
                    CompletedUtc = DateTime.UtcNow,
                    Status = status,
                    ErrorMessage = error,
                    BatchSizeUsed = batchSize,
                    ProgressIntervalUsed = progressInterval,
                    AbortErrorThreshold = abortThreshold
                };
                _context.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ PersistHistoryAsync thất bại cho {DataType}: {Msg}", dataType, ex.Message);
            }
        }

        /// <summary>
        /// Validate CSV file format
        /// </summary>
        public async Task<bool> ValidateFileFormatAsync(IFormFile file, string expectedDataType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return false;

                // Check file extension
                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return false;

                // Check filename pattern
                var expectedPattern = $"7800_{expectedDataType.ToLower()}_\\d{{8}}\\.csv";
                var regex = new Regex(expectedPattern, RegexOptions.IgnoreCase);

                return regex.IsMatch(file.FileName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get import status by ID
        /// </summary>
        public async Task<DirectImportResult> GetImportStatusAsync(Guid importId)
        {
            // Placeholder - implement based on your tracking needs
            return new DirectImportResult
            {
                Success = true,
                ImportId = importId.ToString(),
                Status = "Completed"
            };
        }

        /// <summary>
        /// Lấy số lượng records trong tất cả các bảng dữ liệu
        /// </summary>
        public async Task<Dictionary<string, long>> GetTableRecordCountsAsync()
        {
            try
            {
                var counts = new Dictionary<string, long>();

                // Count records in each data table
                counts["DP01"] = await _context.DP01.CountAsync();
                counts["DPDA"] = await _context.DPDA.CountAsync();
                counts["EI01"] = await _context.EI01.CountAsync();
                counts["GL01"] = await _context.GL01.CountAsync();
                counts["GL02"] = await _context.GL02.CountAsync();
                counts["GL41"] = await _context.GL41.CountAsync();
                counts["LN01"] = await _context.LN01.CountAsync();
                counts["LN03"] = await _context.LN03.CountAsync();
                counts["RR01"] = await _context.RR01.CountAsync();

                _logger.LogInformation("✅ Successfully retrieved record counts for all tables");
                return counts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving table record counts");
                throw;
            }
        }

        #endregion

        #region Private CSV Parsing Methods

        /// <summary>
        /// Parse CSV generic cho bất kỳ entity type nào
        /// </summary>
        private async Task<List<T>> ParseCsvGenericAsync<T>(IFormFile file) where T : class
        {
            var records = new List<T>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                PrepareHeaderForMatch = args => args.Header?.Trim()
            });

            await foreach (var record in csv.GetRecordsAsync<T>())
            {
                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse DP01 CSV với 63 business columns
        /// </summary>
        private async Task<List<DP01>> ParseDP01CsvAsync(IFormFile file)
        {
            var records = new List<DP01>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });

            // Thiết lập định dạng thời gian
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd", "yyyyMMdd HH:mm:ss" };
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats;
            ndtOptions.NullValues.AddRange(new[] { string.Empty, " ", "  ", "\t", "\r", "\n", "\r\n" });

            // Lấy ngày DL từ tên file
            var fileName = Path.GetFileName(file.FileName);
            var ngayDlString = ExtractNgayDLFromFileName(fileName);
            var ngayDl = DateTime.TryParse(ngayDlString, out var date) ? date : DateTime.Today;
            var nowUtc = DateTime.UtcNow;

            try
            {
                var csvRecords = csv.GetRecords<dynamic>();
                foreach (IDictionary<string, object> csvRecord in csvRecords)
                {
                    var record = new DP01();

                    // Set NGAY_DL from filename (first column requirement)
                    record.NGAY_DL = ngayDl;

                    // Safely map all 63 business columns with proper conversion
                    record.MA_CN = SafeGetString(csvRecord, "MA_CN");
                    record.TAI_KHOAN_HACH_TOAN = SafeGetString(csvRecord, "TAI_KHOAN_HACH_TOAN");
                    record.MA_KH = SafeGetString(csvRecord, "MA_KH");
                    record.TEN_KH = SafeGetString(csvRecord, "TEN_KH");
                    record.DP_TYPE_NAME = SafeGetString(csvRecord, "DP_TYPE_NAME");
                    record.CCY = SafeGetString(csvRecord, "CCY");
                    record.CURRENT_BALANCE = SafeGetDecimal(csvRecord, "CURRENT_BALANCE");
                    record.RATE = SafeGetDecimal(csvRecord, "RATE");
                    record.SO_TAI_KHOAN = SafeGetString(csvRecord, "SO_TAI_KHOAN");
                    record.OPENING_DATE = SafeGetDateTime(csvRecord, "OPENING_DATE");
                    record.MATURITY_DATE = SafeGetDateTime(csvRecord, "MATURITY_DATE");
                    record.ADDRESS = SafeGetString(csvRecord, "ADDRESS");
                    record.NOTENO = SafeGetString(csvRecord, "NOTENO");
                    record.MONTH_TERM = SafeGetString(csvRecord, "MONTH_TERM");
                    record.TERM_DP_NAME = SafeGetString(csvRecord, "TERM_DP_NAME");
                    record.TIME_DP_NAME = SafeGetString(csvRecord, "TIME_DP_NAME");
                    record.MA_PGD = SafeGetString(csvRecord, "MA_PGD");
                    record.TEN_PGD = SafeGetString(csvRecord, "TEN_PGD");
                    record.DP_TYPE_CODE = SafeGetString(csvRecord, "DP_TYPE_CODE");
                    record.RENEW_DATE = SafeGetDateTime(csvRecord, "RENEW_DATE");
                    record.CUST_TYPE = SafeGetString(csvRecord, "CUST_TYPE");
                    record.CUST_TYPE_NAME = SafeGetString(csvRecord, "CUST_TYPE_NAME");
                    record.CUST_TYPE_DETAIL = SafeGetString(csvRecord, "CUST_TYPE_DETAIL");
                    record.CUST_DETAIL_NAME = SafeGetString(csvRecord, "CUST_DETAIL_NAME");
                    record.PREVIOUS_DP_CAP_DATE = SafeGetDateTime(csvRecord, "PREVIOUS_DP_CAP_DATE");
                    record.NEXT_DP_CAP_DATE = SafeGetDateTime(csvRecord, "NEXT_DP_CAP_DATE");
                    record.ID_NUMBER = SafeGetString(csvRecord, "ID_NUMBER");
                    record.ISSUED_BY = SafeGetString(csvRecord, "ISSUED_BY");
                    record.ISSUE_DATE = SafeGetDateTime(csvRecord, "ISSUE_DATE");
                    record.SEX_TYPE = SafeGetString(csvRecord, "SEX_TYPE");
                    record.BIRTH_DATE = SafeGetDateTime(csvRecord, "BIRTH_DATE");
                    record.TELEPHONE = SafeGetString(csvRecord, "TELEPHONE");
                    record.ACRUAL_AMOUNT = SafeGetDecimal(csvRecord, "ACRUAL_AMOUNT");
                    record.ACRUAL_AMOUNT_END = SafeGetDecimal(csvRecord, "ACRUAL_AMOUNT_END");
                    record.ACCOUNT_STATUS = SafeGetString(csvRecord, "ACCOUNT_STATUS");
                    record.DRAMT = SafeGetDecimal(csvRecord, "DRAMT");
                    record.CRAMT = SafeGetDecimal(csvRecord, "CRAMT");
                    record.EMPLOYEE_NUMBER = SafeGetString(csvRecord, "EMPLOYEE_NUMBER");
                    record.EMPLOYEE_NAME = SafeGetString(csvRecord, "EMPLOYEE_NAME");
                    record.SPECIAL_RATE = SafeGetDecimal(csvRecord, "SPECIAL_RATE");
                    record.AUTO_RENEWAL = SafeGetString(csvRecord, "AUTO_RENEWAL");
                    record.CLOSE_DATE = SafeGetDateTime(csvRecord, "CLOSE_DATE");
                    record.LOCAL_PROVIN_NAME = SafeGetString(csvRecord, "LOCAL_PROVIN_NAME");
                    record.LOCAL_DISTRICT_NAME = SafeGetString(csvRecord, "LOCAL_DISTRICT_NAME");
                    record.LOCAL_WARD_NAME = SafeGetString(csvRecord, "LOCAL_WARD_NAME");
                    record.TERM_DP_TYPE = SafeGetString(csvRecord, "TERM_DP_TYPE");
                    record.TIME_DP_TYPE = SafeGetString(csvRecord, "TIME_DP_TYPE");
                    record.STATES_CODE = SafeGetString(csvRecord, "STATES_CODE");
                    record.ZIP_CODE = SafeGetString(csvRecord, "ZIP_CODE");
                    record.COUNTRY_CODE = SafeGetString(csvRecord, "COUNTRY_CODE");
                    record.TAX_CODE_LOCATION = SafeGetString(csvRecord, "TAX_CODE_LOCATION");
                    record.MA_CAN_BO_PT = SafeGetString(csvRecord, "MA_CAN_BO_PT");
                    record.TEN_CAN_BO_PT = SafeGetString(csvRecord, "TEN_CAN_BO_PT");
                    record.PHONG_CAN_BO_PT = SafeGetString(csvRecord, "PHONG_CAN_BO_PT");
                    record.NGUOI_NUOC_NGOAI = SafeGetString(csvRecord, "NGUOI_NUOC_NGOAI");
                    record.QUOC_TICH = SafeGetString(csvRecord, "QUOC_TICH");
                    record.MA_CAN_BO_AGRIBANK = SafeGetString(csvRecord, "MA_CAN_BO_AGRIBANK");
                    record.NGUOI_GIOI_THIEU = SafeGetString(csvRecord, "NGUOI_GIOI_THIEU");
                    record.TEN_NGUOI_GIOI_THIEU = SafeGetString(csvRecord, "TEN_NGUOI_GIOI_THIEU");
                    record.CONTRACT_COUTS_DAY = SafeGetString(csvRecord, "CONTRACT_COUTS_DAY");
                    record.SO_KY_AD_LSDB = SafeGetString(csvRecord, "SO_KY_AD_LSDB");
                    record.UNTBUSCD = SafeGetString(csvRecord, "UNTBUSCD");
                    record.TYGIA = SafeGetDecimal(csvRecord, "TYGIA");

                    // Set ImportDateTime for this import batch
                    record.ImportDateTime = nowUtc;
                    // Ensure CreatedAt/UpdatedAt populated to satisfy NOT NULL constraints if present
                    record.CreatedAt = nowUtc;
                    record.UpdatedAt = nowUtc;

                    // No need to set audit fields for temporal tables - managed automatically
                    // record.FILE_NAME = file.FileName; // TODO: Add FILE_NAME column to DP01 table

                    records.Add(record);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DP01] CSV parsing error: {Error}", ex.Message);
                throw new Exception($"DP01 CSV parsing failed: {ex.Message}", ex);
            }

            return records;
        }

        // STREAMING PARSERS (yield return) =====================================
        private async IAsyncEnumerable<DP01> StreamParseDP01Async(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });
            var nowUtc = DateTime.UtcNow;
            var csvRecords = csv.GetRecords<dynamic>();
            foreach (IDictionary<string, object> csvRecord in csvRecords)
            {
                var record = new DP01
                {
                    MA_CN = SafeGetString(csvRecord, "MA_CN"),
                    TAI_KHOAN_HACH_TOAN = SafeGetString(csvRecord, "TAI_KHOAN_HACH_TOAN"),
                    MA_KH = SafeGetString(csvRecord, "MA_KH"),
                    TEN_KH = SafeGetString(csvRecord, "TEN_KH"),
                    DP_TYPE_NAME = SafeGetString(csvRecord, "DP_TYPE_NAME"),
                    CCY = SafeGetString(csvRecord, "CCY"),
                    CURRENT_BALANCE = SafeGetDecimal(csvRecord, "CURRENT_BALANCE"),
                    RATE = SafeGetDecimal(csvRecord, "RATE"),
                    SO_TAI_KHOAN = SafeGetString(csvRecord, "SO_TAI_KHOAN"),
                    OPENING_DATE = SafeGetDateTime(csvRecord, "OPENING_DATE"),
                    MATURITY_DATE = SafeGetDateTime(csvRecord, "MATURITY_DATE"),
                    ADDRESS = SafeGetString(csvRecord, "ADDRESS"),
                    NOTENO = SafeGetString(csvRecord, "NOTENO"),
                    MONTH_TERM = SafeGetString(csvRecord, "MONTH_TERM"),
                    TERM_DP_NAME = SafeGetString(csvRecord, "TERM_DP_NAME"),
                    TIME_DP_NAME = SafeGetString(csvRecord, "TIME_DP_NAME"),
                    MA_PGD = SafeGetString(csvRecord, "MA_PGD"),
                    TEN_PGD = SafeGetString(csvRecord, "TEN_PGD"),
                    DP_TYPE_CODE = SafeGetString(csvRecord, "DP_TYPE_CODE"),
                    RENEW_DATE = SafeGetDateTime(csvRecord, "RENEW_DATE"),
                    CUST_TYPE = SafeGetString(csvRecord, "CUST_TYPE"),
                    CUST_TYPE_NAME = SafeGetString(csvRecord, "CUST_TYPE_NAME"),
                    CUST_TYPE_DETAIL = SafeGetString(csvRecord, "CUST_TYPE_DETAIL"),
                    CUST_DETAIL_NAME = SafeGetString(csvRecord, "CUST_DETAIL_NAME"),
                    PREVIOUS_DP_CAP_DATE = SafeGetDateTime(csvRecord, "PREVIOUS_DP_CAP_DATE"),
                    NEXT_DP_CAP_DATE = SafeGetDateTime(csvRecord, "NEXT_DP_CAP_DATE"),
                    ID_NUMBER = SafeGetString(csvRecord, "ID_NUMBER"),
                    ISSUED_BY = SafeGetString(csvRecord, "ISSUED_BY"),
                    ISSUE_DATE = SafeGetDateTime(csvRecord, "ISSUE_DATE"),
                    SEX_TYPE = SafeGetString(csvRecord, "SEX_TYPE"),
                    BIRTH_DATE = SafeGetDateTime(csvRecord, "BIRTH_DATE"),
                    TELEPHONE = SafeGetString(csvRecord, "TELEPHONE"),
                    ACRUAL_AMOUNT = SafeGetDecimal(csvRecord, "ACRUAL_AMOUNT"),
                    ACRUAL_AMOUNT_END = SafeGetDecimal(csvRecord, "ACRUAL_AMOUNT_END"),
                    ACCOUNT_STATUS = SafeGetString(csvRecord, "ACCOUNT_STATUS"),
                    DRAMT = SafeGetDecimal(csvRecord, "DRAMT"),
                    CRAMT = SafeGetDecimal(csvRecord, "CRAMT"),
                    EMPLOYEE_NUMBER = SafeGetString(csvRecord, "EMPLOYEE_NUMBER"),
                    EMPLOYEE_NAME = SafeGetString(csvRecord, "EMPLOYEE_NAME"),
                    SPECIAL_RATE = SafeGetDecimal(csvRecord, "SPECIAL_RATE"),
                    AUTO_RENEWAL = SafeGetString(csvRecord, "AUTO_RENEWAL"),
                    CLOSE_DATE = SafeGetDateTime(csvRecord, "CLOSE_DATE"),
                    LOCAL_PROVIN_NAME = SafeGetString(csvRecord, "LOCAL_PROVIN_NAME"),
                    LOCAL_DISTRICT_NAME = SafeGetString(csvRecord, "LOCAL_DISTRICT_NAME"),
                    LOCAL_WARD_NAME = SafeGetString(csvRecord, "LOCAL_WARD_NAME"),
                    TERM_DP_TYPE = SafeGetString(csvRecord, "TERM_DP_TYPE"),
                    TIME_DP_TYPE = SafeGetString(csvRecord, "TIME_DP_TYPE"),
                    STATES_CODE = SafeGetString(csvRecord, "STATES_CODE"),
                    ZIP_CODE = SafeGetString(csvRecord, "ZIP_CODE"),
                    COUNTRY_CODE = SafeGetString(csvRecord, "COUNTRY_CODE"),
                    TAX_CODE_LOCATION = SafeGetString(csvRecord, "TAX_CODE_LOCATION"),
                    MA_CAN_BO_PT = SafeGetString(csvRecord, "MA_CAN_BO_PT"),
                    TEN_CAN_BO_PT = SafeGetString(csvRecord, "TEN_CAN_BO_PT"),
                    PHONG_CAN_BO_PT = SafeGetString(csvRecord, "PHONG_CAN_BO_PT"),
                    NGUOI_NUOC_NGOAI = SafeGetString(csvRecord, "NGUOI_NUOC_NGOAI"),
                    QUOC_TICH = SafeGetString(csvRecord, "QUOC_TICH"),
                    MA_CAN_BO_AGRIBANK = SafeGetString(csvRecord, "MA_CAN_BO_AGRIBANK"),
                    NGUOI_GIOI_THIEU = SafeGetString(csvRecord, "NGUOI_GIOI_THIEU"),
                    TEN_NGUOI_GIOI_THIEU = SafeGetString(csvRecord, "TEN_NGUOI_GIOI_THIEU"),
                    CONTRACT_COUTS_DAY = SafeGetString(csvRecord, "CONTRACT_COUTS_DAY"),
                    SO_KY_AD_LSDB = SafeGetString(csvRecord, "SO_KY_AD_LSDB"),
                    UNTBUSCD = SafeGetString(csvRecord, "UNTBUSCD"),
                    TYGIA = SafeGetDecimal(csvRecord, "TYGIA"),
                    ImportDateTime = nowUtc,
                    CreatedAt = nowUtc,
                    UpdatedAt = nowUtc
                };
                yield return record;
            }
        }

        private async IAsyncEnumerable<Models.DataTables.GL02> StreamParseGL02Async(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });
            await foreach (var csvRecord in csv.GetRecordsAsync<dynamic>())
            {
                var rec = new Models.DataTables.GL02();
                var dict = csvRecord as IDictionary<string, object>;
                string GetS(string key)
                {
                    if (dict != null && dict.TryGetValue(key, out var v) && v != null)
                        return v.ToString();
                    return string.Empty;
                }
                // TRDATE -> NGAY_DL
                if (dict != null && dict.TryGetValue("TRDATE", out var trdateObj))
                {
                    var trStr = trdateObj?.ToString();
                    var dateFormats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd" };
                    if (!string.IsNullOrWhiteSpace(trStr))
                    {
                        var trimmed = trStr.Trim('\'', '"', ' ');
                        if (DateTime.TryParseExact(trimmed, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var trdate))
                            rec.NGAY_DL = trdate.Date;
                    }
                }
                if (rec.NGAY_DL == default) rec.NGAY_DL = DateTime.Today;
                rec.TRBRCD = GetS("TRBRCD");
                rec.USERID = string.IsNullOrEmpty(GetS("USERID")) ? null : GetS("USERID");
                rec.JOURSEQ = string.IsNullOrEmpty(GetS("JOURSEQ")) ? null : GetS("JOURSEQ");
                rec.DYTRSEQ = string.IsNullOrEmpty(GetS("DYTRSEQ")) ? null : GetS("DYTRSEQ");
                rec.LOCAC = GetS("LOCAC");
                rec.CCY = GetS("CCY");
                rec.BUSCD = string.IsNullOrEmpty(GetS("BUSCD")) ? null : GetS("BUSCD");
                rec.UNIT = string.IsNullOrEmpty(GetS("UNIT")) ? null : GetS("UNIT");
                rec.TRCD = string.IsNullOrEmpty(GetS("TRCD")) ? null : GetS("TRCD");
                rec.CUSTOMER = string.IsNullOrEmpty(GetS("CUSTOMER")) ? null : GetS("CUSTOMER");
                rec.TRTP = string.IsNullOrEmpty(GetS("TRTP")) ? null : GetS("TRTP");
                rec.REFERENCE = string.IsNullOrEmpty(GetS("REFERENCE")) ? null : GetS("REFERENCE");
                rec.REMARK = string.IsNullOrEmpty(GetS("REMARK")) ? null : GetS("REMARK");
                if (decimal.TryParse(GetS("DRAMOUNT"), NumberStyles.Number, CultureInfo.InvariantCulture, out var d1)) rec.DRAMOUNT = d1;
                if (decimal.TryParse(GetS("CRAMOUNT"), NumberStyles.Number, CultureInfo.InvariantCulture, out var d2)) rec.CRAMOUNT = d2;
                rec.CREATED_DATE = DateTime.UtcNow;
                rec.UPDATED_DATE = DateTime.UtcNow;
                yield return rec;
            }
        }

        private async IAsyncEnumerable<GL01> StreamParseGL01Async(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });
            await foreach (var rec in csv.GetRecordsAsync<GL01>())
            {
                if (rec.TR_TIME.HasValue) rec.NGAY_DL = rec.TR_TIME.Value.Date; else rec.NGAY_DL = DateTime.Today;
                yield return rec;
            }
        }

        /// <summary>
        /// Parse DPDA CSV với 13 business columns
        /// </summary>
        private async Task<List<DPDA>> ParseDPDACsvAsync(IFormFile file)
        {
            var records = new List<DPDA>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            var fileName = Path.GetFileName(file.FileName);
            var ngayDlString = ExtractNgayDLFromFileName(fileName);
            var ngayDl = DateTime.TryParse(ngayDlString, out var date) ? date : DateTime.Today;

            await foreach (var record in csv.GetRecordsAsync<dynamic>())
            {
                var dpda = new DPDA
                {
                    NGAY_DL = ngayDl,
                    MA_CHI_NHANH = record.MA_CHI_NHANH ?? "",
                    MA_KHACH_HANG = record.MA_KHACH_HANG ?? "",
                    TEN_KHACH_HANG = record.TEN_KHACH_HANG ?? "",
                    SO_TAI_KHOAN = record.SO_TAI_KHOAN ?? "",
                    LOAI_THE = record.LOAI_THE ?? "",
                    SO_THE = record.SO_THE ?? "",
                    NGAY_NOP_DON = ParseDateTimeSafely(record.NGAY_NOP_DON),
                    NGAY_PHAT_HANH = ParseDateTimeSafely(record.NGAY_PHAT_HANH),
                    USER_PHAT_HANH = record.USER_PHAT_HANH ?? "",
                    TRANG_THAI = record.TRANG_THAI ?? "",
                    PHAN_LOAI = record.PHAN_LOAI ?? "",
                    GIAO_THE = record.GIAO_THE ?? "",
                    LOAI_PHAT_HANH = record.LOAI_PHAT_HANH ?? "",
                    CREATED_DATE = DateTime.UtcNow, // đảm bảo UTC
                    UPDATED_DATE = DateTime.UtcNow // đảm bảo UTC
                };

                records.Add(dpda);
            }

            return records;
        }

        /// <summary>
        /// Parse EI01 CSV với 24 business columns
        /// </summary>
        private async Task<List<Models.DataTables.EI01>> ParseEI01CsvAsync(IFormFile file)
        {
            var records = new List<Models.DataTables.EI01>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            // Ensure DateTime parsing uses dd/MM/yyyy as primary format
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd" };
            // Treat common blank placeholders as nulls in CSV exports
            dtOptions.NullValues.AddRange(new[] { string.Empty, " ", "  ", "\t", "\r", "\n", "        " });
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats;
            ndtOptions.NullValues.AddRange(dtOptions.NullValues);

            await foreach (var record in csv.GetRecordsAsync<Models.DataTables.EI01>())
            {
                // Set audit fields
                record.CREATED_DATE = DateTime.UtcNow;
                record.UPDATED_DATE = DateTime.UtcNow;

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse LN01 CSV với 79 business columns
        /// </summary>

        // STREAM PARSER LN01 mới (yield) – tái sử dụng logic mapping cũ
        private async IAsyncEnumerable<LN01> StreamParseLN01Async(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });
            csv.Read();
            csv.ReadHeader();

            // Thiết lập format dùng lại
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd", "ddMMyyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:ss" };
            dtOptions.NullValues.AddRange(new[] { string.Empty, " ", "\t" });
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats; ndtOptions.NullValues.AddRange(dtOptions.NullValues);

            while (await csv.ReadAsync())
            {
                LN01? r = null;
                try
                {
                    r = new LN01();
                    string Clean(string? s)
                    {
                        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
                        var t = s.Trim();
                        if (t.StartsWith("'")) t = t.TrimStart('\'');
                        t = System.Text.RegularExpressions.Regex.Replace(t, @"\s+", " ").Trim();
                        return t;
                    }
                    decimal? Dec(string? s)
                    {
                        if (string.IsNullOrWhiteSpace(s)) return null;
                        var t = s.Replace(",", "").Trim();
                        t = t.Replace(" ", "").Trim('\'');
                        if (System.Text.RegularExpressions.Regex.IsMatch(t, @"^0+$")) return 0m;
                        if (decimal.TryParse(t, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var d)) return d;
                        return null;
                    }
                    DateTime? Dt(string? s)
                    {
                        if (string.IsNullOrWhiteSpace(s)) return null;
                        var t = s.Trim('\'', ' ');
                        var fmts = new[]{"yyyyMMdd","ddMMyyyy","dd/MM/yyyy","yyyy-MM-dd","yyyy/MM/dd"};
                        if (DateTime.TryParseExact(t, fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) return dt;
                        if (DateTime.TryParse(t, out dt)) return dt;
                        return null;
                    }

                    // Map subset theo pattern (giống parser legacy)
                    r.BRCD = Clean(csv.GetField("BRCD"));
                    r.CUSTSEQ = Clean(csv.GetField("CUSTSEQ"));
                    r.CUSTNM = Clean(csv.GetField("CUSTNM"));
                    r.TAI_KHOAN = Clean(csv.GetField("TAI_KHOAN"));
                    r.CCY = Clean(csv.GetField("CCY"));
                    r.DU_NO = Dec(csv.GetField("DU_NO"));
                    r.DSBSSEQ = Clean(csv.GetField("DSBSSEQ"));
                    r.TRANSACTION_DATE = Dt(csv.GetField("TRANSACTION_DATE"));
                    r.DSBSDT = Dt(csv.GetField("DSBSDT"));
                    r.DISBUR_CCY = Clean(csv.GetField("DISBUR_CCY"));
                    r.DISBURSEMENT_AMOUNT = Dec(csv.GetField("DISBURSEMENT_AMOUNT"));
                    r.DSBSMATDT = Dt(csv.GetField("DSBSMATDT"));
                    r.BSRTCD = Clean(csv.GetField("BSRTCD"));
                    r.INTEREST_RATE = Dec(csv.GetField("INTEREST_RATE"));
                    r.APPRSEQ = Clean(csv.GetField("APPRSEQ"));
                    r.APPRDT = Dt(csv.GetField("APPRDT"));
                    r.APPR_CCY = Clean(csv.GetField("APPR_CCY"));
                    r.APPRAMT = Dec(csv.GetField("APPRAMT"));
                    r.APPRMATDT = Dt(csv.GetField("APPRMATDT"));
                    r.LOAN_TYPE = Clean(csv.GetField("LOAN_TYPE"));
                    r.FUND_RESOURCE_CODE = Clean(csv.GetField("FUND_RESOURCE_CODE"));
                    r.FUND_PURPOSE_CODE = Clean(csv.GetField("FUND_PURPOSE_CODE"));
                    r.REPAYMENT_AMOUNT = Dec(csv.GetField("REPAYMENT_AMOUNT"));
                    r.NEXT_REPAY_DATE = Dt(csv.GetField("NEXT_REPAY_DATE"));
                    r.NEXT_REPAY_AMOUNT = Dec(csv.GetField("NEXT_REPAY_AMOUNT"));
                    r.NEXT_INT_REPAY_DATE = Dt(csv.GetField("NEXT_INT_REPAY_DATE"));
                    r.OFFICER_ID = Clean(csv.GetField("OFFICER_ID"));
                    r.OFFICER_NAME = Clean(csv.GetField("OFFICER_NAME"));
                    r.INTEREST_AMOUNT = Dec(csv.GetField("INTEREST_AMOUNT"));
                    r.PASTDUE_INTEREST_AMOUNT = Dec(csv.GetField("PASTDUE_INTEREST_AMOUNT"));
                    r.TOTAL_INTEREST_REPAY_AMOUNT = Dec(csv.GetField("TOTAL_INTEREST_REPAY_AMOUNT"));
                    r.CUSTOMER_TYPE_CODE = Clean(csv.GetField("CUSTOMER_TYPE_CODE"));
                    r.CUSTOMER_TYPE_CODE_DETAIL = Clean(csv.GetField("CUSTOMER_TYPE_CODE_DETAIL"));
                    r.TRCTCD = Clean(csv.GetField("TRCTCD"));
                    r.TRCTNM = Clean(csv.GetField("TRCTNM"));
                    r.ADDR1 = Clean(csv.GetField("ADDR1"));
                    r.PROVINCE = Clean(csv.GetField("PROVINCE"));
                    r.LCLPROVINNM = Clean(csv.GetField("LCLPROVINNM"));
                    r.DISTRICT = Clean(csv.GetField("DISTRICT"));
                    r.LCLDISTNM = Clean(csv.GetField("LCLDISTNM"));
                    r.COMMCD = Clean(csv.GetField("COMMCD"));
                    r.LCLWARDNM = Clean(csv.GetField("LCLWARDNM"));
                    r.LAST_REPAY_DATE = Dt(csv.GetField("LAST_REPAY_DATE"));
                    r.SECURED_PERCENT = Clean(csv.GetField("SECURED_PERCENT"));
                    r.NHOM_NO = Clean(csv.GetField("NHOM_NO"));
                    r.LAST_INT_CHARGE_DATE = Dt(csv.GetField("LAST_INT_CHARGE_DATE"));
                    r.EXEMPTINT = Clean(csv.GetField("EXEMPTINT"));
                    r.EXEMPTINTTYPE = Clean(csv.GetField("EXEMPTINTTYPE"));
                    r.EXEMPTINTAMT = Dec(csv.GetField("EXEMPTINTAMT"));
                    r.GRPNO = Clean(csv.GetField("GRPNO"));
                    r.BUSCD = Clean(csv.GetField("BUSCD"));
                    r.BSNSSCLTPCD = Clean(csv.GetField("BSNSSCLTPCD"));
                    r.USRIDOP = Clean(csv.GetField("USRIDOP"));
                    r.ACCRUAL_AMOUNT = Dec(csv.GetField("ACCRUAL_AMOUNT"));
                    r.ACCRUAL_AMOUNT_END_OF_MONTH = Dec(csv.GetField("ACCRUAL_AMOUNT_END_OF_MONTH"));
                    r.INTCMTH = Clean(csv.GetField("INTCMTH"));
                    r.INTRPYMTH = Clean(csv.GetField("INTRPYMTH"));
                    r.INTTRMMTH = Clean(csv.GetField("INTTRMMTH"));
                    r.YRDAYS = Clean(csv.GetField("YRDAYS"));
                    r.REMARK = Clean(csv.GetField("REMARK"));
                    r.CHITIEU = Clean(csv.GetField("CHITIEU"));
                    r.CTCV = Clean(csv.GetField("CTCV"));
                    r.CREDIT_LINE_YPE = Clean(csv.GetField("CREDIT_LINE_YPE"));
                    r.INT_LUMPSUM_PARTIAL_TYPE = Clean(csv.GetField("INT_LUMPSUM_PARTIAL_TYPE"));
                    r.INT_PARTIAL_PAYMENT_TYPE = Clean(csv.GetField("INT_PARTIAL_PAYMENT_TYPE"));
                    r.INT_PAYMENT_INTERVAL = Clean(csv.GetField("INT_PAYMENT_INTERVAL"));
                    r.AN_HAN_LAI = Clean(csv.GetField("AN_HAN_LAI"));
                    r.PHUONG_THUC_GIAI_NGAN_1 = Clean(csv.GetField("PHUONG_THUC_GIAI_NGAN_1"));
                    r.TAI_KHOAN_GIAI_NGAN_1 = Clean(csv.GetField("TAI_KHOAN_GIAI_NGAN_1"));
                    r.SO_TIEN_GIAI_NGAN_1 = Dec(csv.GetField("SO_TIEN_GIAI_NGAN_1"));
                    r.PHUONG_THUC_GIAI_NGAN_2 = Clean(csv.GetField("PHUONG_THUC_GIAI_NGAN_2"));
                    r.TAI_KHOAN_GIAI_NGAN_2 = Clean(csv.GetField("TAI_KHOAN_GIAI_NGAN_2"));
                    r.SO_TIEN_GIAI_NGAN_2 = Dec(csv.GetField("SO_TIEN_GIAI_NGAN_2"));
                    r.CMT_HC = Clean(csv.GetField("CMT_HC"));
                    r.NGAY_SINH = Dt(csv.GetField("NGAY_SINH"));
                    r.MA_CB_AGRI = Clean(csv.GetField("MA_CB_AGRI"));
                    r.MA_NGANH_KT = Clean(csv.GetField("MA_NGANH_KT"));
                    r.TY_GIA = Dec(csv.GetField("TY_GIA"));
                    r.OFFICER_IPCAS = Clean(csv.GetField("OFFICER_IPCAS"));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("⚠️ [LN01] Row streaming parse error: {Err}", ex.Message);
                }
                if (r != null) yield return r;
            }
        }

        /// <summary>
        /// Parse LN03 CSV với enhanced processing
        /// </summary>
        private async Task<List<LN03>> ParseLN03EnhancedAsync(IFormFile file, string? statementDate)
        {
            var records = new List<LN03>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            using var csv = new CsvReader(reader, config);

            // Read header and check column count
            csv.Read();
            csv.ReadHeader();
            var headerCount = csv.HeaderRecord?.Length ?? 0;
            _logger.LogInformation("🔍 [LN03] CSV has {HeaderCount} headers", headerCount);

            while (csv.Read())
            {
                try
                {
                    var record = new LN03();

                    // Map the 17 header columns directly
                    record.MACHINHANH = csv.GetField("MACHINHANH");
                    record.TENCHINHANH = csv.GetField("TENCHINHANH");
                    record.MAKH = csv.GetField("MAKH");
                    record.TENKH = csv.GetField("TENKH");
                    record.SOHOPDONG = csv.GetField("SOHOPDONG");

                    // Parse decimal fields with robust error handling
                    record.SOTIENXLRR = ParseDecimalSafely(csv.GetField("SOTIENXLRR"));

                    // Parse date field
                    record.NGAYPHATSINHXL = ParseDateTimeSafely(csv.GetField("NGAYPHATSINHXL"));

                    record.THUNOSAUXL = ParseDecimalSafely(csv.GetField("THUNOSAUXL"));
                    record.CONLAINGOAIBANG = ParseDecimalSafely(csv.GetField("CONLAINGOAIBANG"));
                    record.DUNONOIBANG = ParseDecimalSafely(csv.GetField("DUNONOIBANG"));
                    record.NHOMNO = csv.GetField("NHOMNO");
                    record.MACBTD = csv.GetField("MACBTD");
                    record.TENCBTD = csv.GetField("TENCBTD");
                    record.MAPGD = csv.GetField("MAPGD");
                    record.TAIKHOANHACHTOAN = csv.GetField("TAIKHOANHACHTOAN");
                    record.REFNO = csv.GetField("REFNO");
                    record.LOAINGUONVON = csv.GetField("LOAINGUONVON");

                    // Map the 3 no-header columns by index (18, 19, 20)
                    record.Column18 = csv.GetField(17); // 0-based index for column 18
                    record.Column19 = csv.GetField(18); // 0-based index for column 19
                    record.Column20 = ParseDecimalSafely(csv.GetField(19)); // 0-based index for column 20

                    records.Add(record);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("⚠️ [LN03] Row parsing error: {Error}", ex.Message);
                    // Continue processing other rows
                }
            }

            return records;
        }

        /// <summary>
        /// Parse GL02 CSV: NGAY_DL từ TRDATE; datetime2 dd/MM/yyyy; decimals #,###.00
        /// </summary>
        /// <summary>
        /// Parse GL02 CSV với 17 business columns, set NGAY_DL từ TRDATE
        /// </summary>
        private async Task<List<Models.DataTables.GL02>> ParseGL02CsvAsync(IFormFile file)
        {
            var records = new List<Models.DataTables.GL02>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            // Date parsing preferences
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd" };
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss" };

            await foreach (var csvRecord in csv.GetRecordsAsync<dynamic>())
            {
                var record = new Models.DataTables.GL02();

                // Access dynamic row safely via dictionary
                var dict = csvRecord as IDictionary<string, object>;

                // TRDATE -> NGAY_DL (ngày sao kê)
                if (dict != null && dict.TryGetValue("TRDATE", out var trdateObj))
                {
                    var trStr = trdateObj?.ToString();
                    if (!string.IsNullOrWhiteSpace(trStr))
                    {
                        var dateFormats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd" };
                        if (DateTime.TryParseExact(trStr.Trim('\'', '"', ' '), dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var trdate))
                        {
                            record.NGAY_DL = trdate.Date;
                        }
                    }
                }
                if (record.NGAY_DL == default)
                {
                    // Fallback để không lỗi constraint
                    record.NGAY_DL = DateTime.Today;
                    _logger.LogWarning("⚠️ [GL02] Không đọc được TRDATE, set NGAY_DL=Today");
                }

                // Map chuỗi an toàn
                string GetS(string key)
                {
                    if (dict != null && dict.TryGetValue(key, out var v) && v != null)
                        return v.ToString();
                    return string.Empty;
                }

                record.TRBRCD = GetS("TRBRCD");
                record.USERID = string.IsNullOrEmpty(GetS("USERID")) ? null : GetS("USERID");
                record.JOURSEQ = string.IsNullOrEmpty(GetS("JOURSEQ")) ? null : GetS("JOURSEQ");
                record.DYTRSEQ = string.IsNullOrEmpty(GetS("DYTRSEQ")) ? null : GetS("DYTRSEQ");
                record.LOCAC = GetS("LOCAC");
                record.CCY = GetS("CCY");
                record.BUSCD = string.IsNullOrEmpty(GetS("BUSCD")) ? null : GetS("BUSCD");
                record.UNIT = string.IsNullOrEmpty(GetS("UNIT")) ? null : GetS("UNIT");
                record.TRCD = string.IsNullOrEmpty(GetS("TRCD")) ? null : GetS("TRCD");
                record.CUSTOMER = string.IsNullOrEmpty(GetS("CUSTOMER")) ? null : GetS("CUSTOMER");
                record.TRTP = string.IsNullOrEmpty(GetS("TRTP")) ? null : GetS("TRTP");
                record.REFERENCE = string.IsNullOrEmpty(GetS("REFERENCE")) ? null : GetS("REFERENCE");
                record.REMARK = string.IsNullOrEmpty(GetS("REMARK")) ? null : GetS("REMARK");

                // Amounts
                decimal tmpDec;
                var drRaw = GetS("DRAMOUNT");
                if (!string.IsNullOrWhiteSpace(drRaw) && decimal.TryParse(drRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out tmpDec))
                    record.DRAMOUNT = tmpDec;
                var crRaw = GetS("CRAMOUNT");
                if (!string.IsNullOrWhiteSpace(crRaw) && decimal.TryParse(crRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out tmpDec))
                    record.CRAMOUNT = tmpDec;

                // CRTDTM (có thể không có cột này trong một số file)
                var crtStr = GetS("CRTDTM");
                if (!string.IsNullOrWhiteSpace(crtStr))
                {
                    var dtFormats = new[] { "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd HH:mm:ss", "yyyyMMddHH:mm:ss", "yyyy-MM-ddTHH:mm:ss" };
                    if (DateTime.TryParseExact(crtStr.Trim('\'', '"', ' '), dtFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var crtdtm))
                        record.CRTDTM = crtdtm;
                    else if (DateTime.TryParse(crtStr, out var general))
                        record.CRTDTM = general;
                }

                // Audit/system columns
                record.CREATED_DATE = DateTime.UtcNow;
                record.UPDATED_DATE = DateTime.UtcNow;
                // DataTables.GL02 không có cột FILE_NAME trong DB model

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse GL01 CSV với 27 business columns, set NGAY_DL từ TR_TIME (date component)
        /// </summary>
        private async Task<List<GL01>> ParseGL01CsvAsync(IFormFile file)
        {
            var records = new List<GL01>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            // Date parsing preferences (dd/MM/yyyy common), and allow timestamp for TR_TIME
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] {
                // common numeric formats
                "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd",
                // alpha month formats seen in GL01 exports
                "dd-MMM-yy", "d-MMM-yy", "dd-MMM-yyyy", "d-MMM-yyyy",
                // with time components
                "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "dd-MMM-yy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss",
                // compact with time (observed in TRDT_TIME): 20241201 06:13:46
                "yyyyMMdd HH:mm:ss", "yyyyMMddHH:mm:ss", "yyyy-MM-ddTHH:mm:ss"
            };
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats;
            // Treat blanks and whitespace as nulls to avoid conversion errors on empty cells
            ndtOptions.NullValues.AddRange(new[] { string.Empty, " ", "  ", "\t", "\r", "\n", "\r\n" });

            await foreach (var record in csv.GetRecordsAsync<GL01>())
            {
                // Derive NGAY_DL from TR_TIME (date component only)
                if (record.TR_TIME.HasValue)
                {
                    // TR_TIME is already DateTime?, use date component
                    record.NGAY_DL = record.TR_TIME.Value.Date;
                }
                else
                {
                    // fallback to today but log
                    _logger.LogWarning("⚠️ [GL01] TR_TIME is null, fallback to Today");
                    record.NGAY_DL = DateTime.Today;
                }

                // Normalize audit fields
                // record.CREATED_DATE = DateTime.UtcNow; // Không có CreatedAt trong GL02Entity mới
                // record.UpdatedAt = DateTime.UtcNow; // Không có UpdatedAt trong GL02Entity mới
                // record.FileName = file.FileName; // TODO: Add FILE_NAME column to GL01 table
                // record.FileName = file.FileName; // GL01 doesn't have FileName property yet

                // Normalize numeric from string TR_EX_RT if needed is handled at query level; keep CSV-first mapping

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse GL41 CSV: NGAY_DL từ filename; 13 business columns; temporal table
        /// </summary>
        private async Task<List<GL41Entity>> ParseGL41CsvAsync(IFormFile file)
        {
            var records = new List<GL41Entity>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            // Date parsing preferences for decimal amounts
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd" };

            // Extract NGAY_DL from filename và convert to DateTime
            var ngayDlString = ExtractNgayDLFromFileName(file.FileName);
            var ngayDlDateTime = DateTime.ParseExact(ngayDlString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            await foreach (var record in csv.GetRecordsAsync<GL41CsvRow>())
            {
                var entity = new GL41Entity
                {
                    NGAY_DL = ngayDlDateTime,
                    MA_CN = record.MA_CN,
                    LOAI_TIEN = record.LOAI_TIEN,
                    MA_TK = record.MA_TK,
                    TEN_TK = record.TEN_TK,
                    LOAI_BT = record.LOAI_BT,
                    DN_DAUKY = ParseDecimalSafely(record.DN_DAUKY),
                    DC_DAUKY = ParseDecimalSafely(record.DC_DAUKY),
                    SBT_NO = ParseDecimalSafely(record.SBT_NO),
                    ST_GHINO = ParseDecimalSafely(record.ST_GHINO),
                    SBT_CO = ParseDecimalSafely(record.SBT_CO),
                    ST_GHICO = ParseDecimalSafely(record.ST_GHICO),
                    DN_CUOIKY = ParseDecimalSafely(record.DN_CUOIKY),
                    DC_CUOIKY = ParseDecimalSafely(record.DC_CUOIKY),
                    CREATED_DATE = DateTime.UtcNow,
                    FILE_NAME = file.FileName
                };

                records.Add(entity);
            }

            return records;
        }

        /// <summary>
        /// Bulk insert generic cho bất kỳ entity type nào
        /// Chọn engine tự động: SqlBulkCopy cho khối lượng lớn, EF AddRange cho khối lượng nhỏ hoặc khi không tương thích
        /// </summary>
        private async Task<int> BulkInsertGenericAsync<T>(List<T> records, string tableName) where T : class
        {
            try
            {
                // Ensure common audit columns are populated if present (defensive)
                var type = typeof(T);
                var now = DateTime.UtcNow;
                var createdAtProp = type.GetProperty("CreatedAt");
                var updatedAtProp = type.GetProperty("UpdatedAt");
                var createdDateProp = type.GetProperty("CREATED_DATE");
                var updatedDateProp = type.GetProperty("UPDATED_DATE");

                if (createdAtProp != null || updatedAtProp != null || createdDateProp != null || updatedDateProp != null)
                {
                    foreach (var r in records)
                    {
                        if (createdAtProp != null)
                        {
                            var v = createdAtProp.GetValue(r);
                            if (v == null || (v is DateTime dt && dt == default))
                                createdAtProp.SetValue(r, now);
                        }
                        if (updatedAtProp != null)
                        {
                            var v = updatedAtProp.GetValue(r);
                            if (v == null || (v is DateTime dt && dt == default))
                                updatedAtProp.SetValue(r, now);
                        }
                        if (createdDateProp != null)
                        {
                            var v = createdDateProp.GetValue(r);
                            if (v == null || (v is DateTime dt && dt == default))
                                createdDateProp.SetValue(r, now);
                        }
                        if (updatedDateProp != null)
                        {
                            var v = updatedDateProp.GetValue(r);
                            if (v == null || (v is DateTime dt && dt == default))
                                updatedDateProp.SetValue(r, now);
                        }
                    }
                }

                // LN01: Trực tiếp insert theo DataTables.LN01 để khớp schema vật lý trong DB
                // (tránh sai khác precision/column naming khi qua lớp Entities)
                if (typeof(T) == typeof(Models.DataTables.LN01))
                {
                    return await BulkInsertSqlBulkCopyAsync(records, tableName);
                }
                // Với LN03 CSV model -> map sang Entity trước khi insert
                if (typeof(T) == typeof(Models.DataTables.LN03))
                {
                    var list = records.Cast<Models.DataTables.LN03>().Select(MapLN03ToEntity).ToList();
                    return await BulkInsertSqlBulkCopyAsync(list, tableName);
                }

                // Mặc định dùng SqlBulkCopy cho tất cả các bảng dữ liệu trực tiếp
                return await BulkInsertSqlBulkCopyAsync(records, tableName);
            }
            catch (DbUpdateException dbEx)
            {
                var root = dbEx.GetBaseException();
                _logger.LogError(dbEx, "❌ [BULK_INSERT] DbUpdateException on {Table}: {Message}", tableName, dbEx.Message);
                if (root != null && root != dbEx)
                {
                    _logger.LogError("🔎 [BULK_INSERT] Root cause: {RootType}: {RootMessage}", root.GetType().Name, root.Message);
                }
#if DEBUG
                // Log detailed entries causing failure when available
                foreach (var entry in dbEx.Entries)
                {
                    try
                    {
                        var props = entry.CurrentValues.Properties.Select(p => $"{p.Name}={(entry.CurrentValues[p] ?? "<null>")}");
                        _logger.LogError("⚙️ [BULK_INSERT] Failed entity {EntityType}: {Values}", entry.Entity.GetType().Name, string.Join(", ", props));
                    }
                    catch { /* best-effort */ }
                }
#endif
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [BULK_INSERT] Error inserting vào {Table}: {Error}", tableName, ex.Message);
                throw;
            }
        }

        private static bool IsTransient(Exception ex)
        {
            // Consider common SQL/network transport/login issues as transient
            if (ex is SqlException)
                return true;
            if (ex is TimeoutException)
                return true;
            if (ex is System.Net.Sockets.SocketException)
                return true;
            if (ex is System.IO.IOException)
                return true;
            if (ex is System.ComponentModel.Win32Exception)
                return true;

            // Unwrap common wrapper exceptions
            if (ex is DbUpdateException dbu && dbu.InnerException != null)
                return IsTransient(dbu.InnerException);
            if (ex.InnerException != null)
                return IsTransient(ex.InnerException);
            return false;
        }

        /// <summary>
        /// Heavy file bulk insert for GL01 - BatchSize 10,000 records using SqlBulkCopy
        /// Optimized for large CSV files up to 2GB
        /// </summary>
        private async Task<int> BulkInsertGL01HeavyAsync(List<GL01> records)
        {
            const int BATCH_SIZE = 10000;
            int totalInserted = 0;

            try
            {
                _logger.LogInformation("🚀 [GL01_HEAVY] Starting heavy file bulk insert - {Count} records in batches of {BatchSize}",
                    records.Count, BATCH_SIZE);

                for (int i = 0; i < records.Count; i += BATCH_SIZE)
                {
                    var batch = records.Skip(i).Take(BATCH_SIZE).ToList();

                    _logger.LogInformation("📦 [GL01_HEAVY] Processing batch {BatchNumber}/{TotalBatches} ({BatchSize} records)",
                        (i / BATCH_SIZE) + 1, (int)Math.Ceiling((double)records.Count / BATCH_SIZE), batch.Count);

                    var batchInserted = await BulkInsertSqlBulkCopyAsync(batch, "GL01", BATCH_SIZE, timeoutSec: 900);
                    totalInserted += batchInserted;

                    _logger.LogInformation("✅ [GL01_HEAVY] Batch completed - {BatchInserted} records inserted (Total: {Total})",
                        batchInserted, totalInserted);
                }

                _logger.LogInformation("🎉 [GL01_HEAVY] Heavy file bulk insert completed - {Total} records inserted", totalInserted);
                return totalInserted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL01_HEAVY] Heavy file bulk insert error: {Error}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Engine: SqlBulkCopy for ultra-fast inserts. Tự động map cột bằng tên property hoặc [Column(Name)]
        /// - Tôn trọng transaction hiện tại của EF nếu có
        /// - Bỏ qua các property [NotMapped] và Identity (Id)
        /// </summary>
        private async Task<int> BulkInsertSqlBulkCopyAsync<T>(List<T> records, string tableName, int batchSize = 10000, int timeoutSec = 600) where T : class
        {
            if (records == null || records.Count == 0)
                return 0;

            // Chuẩn bị schema và dữ liệu DataTable
            var props = GetScaffoldableProperties(typeof(T));
            var (dataTable, mappings) = BuildDataTableAndMappings(records, props, tableName);

            // Dùng connection/transaction của EF nếu có để đảm bảo tính atomic với các thao tác khác
            var dbConn = _context.Database.GetDbConnection();
            var currentTx = (_context.Database.CurrentTransaction as IInfrastructure<DbTransaction>)?.Instance as SqlTransaction;

            // Helper tạo SqlBulkCopy phù hợp
            async Task<int> ExecuteWith(SqlConnection sqlConn, SqlTransaction? sqlTx)
            {
                using var bulk = sqlTx != null
                    ? new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints, sqlTx)
                    : new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints, null);

                bulk.DestinationTableName = $"[dbo].[{tableName}]";
                bulk.BatchSize = Math.Max(1000, batchSize);
                bulk.BulkCopyTimeout = Math.Max(60, timeoutSec);

                // Thiết lập column mappings
                foreach (var map in mappings)
                {
                    bulk.ColumnMappings.Add(map.Source, map.Destination);
                }

                _logger.LogInformation("⚡ [SqlBulkCopy] Inserting {Count} rows into {Table} (BatchSize={BatchSize}, Timeout={Timeout}s)",
                    dataTable.Rows.Count, tableName, bulk.BatchSize, bulk.BulkCopyTimeout);

                // Diagnostic: log column types for LN01 to verify TY_GIA handling
                if (string.Equals(tableName, "LN01", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var cols = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => $"{c.ColumnName}:{c.DataType.Name}"));
                        _logger.LogInformation("🧪 [LN01] DataTable columns: {Cols}", cols);
                    }
                    catch { }
                }

                var needOpen = sqlConn.State != ConnectionState.Open;
                if (needOpen) await sqlConn.OpenAsync();
                try
                {
                    await bulk.WriteToServerAsync(dataTable);
                }
                finally
                {
                    if (needOpen) await sqlConn.CloseAsync();
                }

                return records.Count;
            }

            // Nếu EF đang dùng SqlConnection, ưu tiên dùng nó để tận dụng transaction hiện tại
            if (dbConn is SqlConnection efSqlConn)
            {
                return await ExecuteWith(efSqlConn, currentTx);
            }
            else
            {
                // Fallback: Tạo connection mới bằng connection string từ DbContext
                var cs = _context.Database.GetConnectionString();
                using var sqlConn = new SqlConnection(cs);
                return await ExecuteWith(sqlConn, null);
            }
        }

        private static IEnumerable<PropertyInfo> GetScaffoldableProperties(Type t)
        {
            if (_propertyCache.TryGetValue(t, out var cached)) return cached;
            var list = new List<PropertyInfo>();
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead) continue;
                if (p.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                if (string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    var dbGen = p.GetCustomAttribute<DatabaseGeneratedAttribute>();
                    if (dbGen?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity) continue;
                }
                var pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (!IsScalarType(pt)) continue;
                list.Add(p);
            }
            _propertyCache[t] = list;
            return list;
        }

        private static bool IsScalarType(Type t)
        {
            return t.IsPrimitive
                   || t.IsEnum
                   || t == typeof(string)
                   || t == typeof(decimal)
                   || t == typeof(DateTime)
                   || t == typeof(Guid)
                   || t == typeof(byte[])
                   || t == typeof(TimeSpan)
                   || t == typeof(DateTimeOffset)
                   || t == typeof(bool)
                   || t == typeof(short)
                   || t == typeof(int)
                   || t == typeof(long)
                   || t == typeof(float)
                   || t == typeof(double);
        }

        private record ColumnMap(string Source, string Destination);

        private sealed class ColumnDescriptor
        {
            public PropertyInfo Property { get; init; } = default!;
            public string ColumnName { get; init; } = string.Empty;
            public Type ColumnType { get; init; } = typeof(object);
            public bool DateToNVarChar { get; init; } = false; // use yyyy-MM-dd
        }

    private static (DataTable Table, List<ColumnMap> Mappings) BuildDataTableAndMappings<T>(IEnumerable<T> records, IEnumerable<PropertyInfo> props, string tableName)
        {
            var dt = new DataTable();
            var mappings = new List<ColumnMap>();

            // Describe columns and conversions
            var propList = props.ToList();
            var descriptors = new List<ColumnDescriptor>(propList.Count);
            foreach (var p in propList)
            {
                var colAttr = p.GetCustomAttribute<ColumnAttribute>();
                var colName = string.IsNullOrWhiteSpace(colAttr?.Name) ? p.Name : colAttr!.Name;
                var pType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var dateToString = false;
                var forceString = false;
                // Special-cases:
                // 1) EF conversion DateTime <-> nvarchar(10) (e.g., GL41.NGAY_DL configured in OnModelCreating)
                if ((pType == typeof(DateTime) || pType == typeof(DateTime?)))
                {
                    // If ColumnAttribute explicitly says nvarchar, convert to string
                    if (!string.IsNullOrWhiteSpace(colAttr?.TypeName) && colAttr!.TypeName!.StartsWith("nvarchar", StringComparison.OrdinalIgnoreCase))
                    {
                        dateToString = true;
                    }
                    // If table is GL41 and property is NGAY_DL, DbContext maps it to nvarchar(10)
                    if (!dateToString && string.Equals(tableName, "GL41", StringComparison.OrdinalIgnoreCase) && string.Equals(p.Name, "NGAY_DL", StringComparison.OrdinalIgnoreCase))
                    {
                        dateToString = true;
                    }
                }

                // TY_GIA: đã chuẩn hóa decimal(18,6) => không ép kiểu string

                descriptors.Add(new ColumnDescriptor
                {
                    Property = p,
                    ColumnName = colName,
                    ColumnType = (dateToString || forceString) ? typeof(string) : pType,
                    DateToNVarChar = dateToString
                });

                dt.Columns.Add(colName, (dateToString || forceString) ? typeof(string) : pType);
                // IMPORTANT: SqlBulkCopy maps source columns by DataTable column names, not property names.
                // When [Column(Name=...)] is present, DataTable column is "colName" while property name may differ.
                // Therefore, set both Source and Destination to the actual column name in the DataTable/DB.
                mappings.Add(new ColumnMap(colName, colName));
            }

            // Đổ dữ liệu
            foreach (var item in records)
            {
                var row = dt.NewRow();
                foreach (var d in descriptors)
                {
                    var val = d.Property.GetValue(item);
                    if (val == null)
                    {
                        row[d.ColumnName] = DBNull.Value;
                    }
                    else if (d.DateToNVarChar)
                    {
                        var dtVal = (val is DateTimeOffset dto) ? dto.DateTime : (DateTime)val;
                        row[d.ColumnName] = dtVal.ToString("yyyy-MM-dd");
                    }
                    // TY_GIA: xử lý bình thường không heuristic
                    else if (d.ColumnType == typeof(string) && (val is decimal || val is decimal? || val is IFormattable))
                    {
                        // Normalize numeric/string values to invariant format without spaces/commas
                        var s = (val is IFormattable f) ? f.ToString(null, CultureInfo.InvariantCulture) : val.ToString();
                        if (s != null)
                        {
                            s = s.Trim().Replace(",", "").Replace(" ", "");
                        }
                        row[d.ColumnName] = s ?? string.Empty;
                    }
                    else
                    {
                        row[d.ColumnName] = val;
                    }
                }
                dt.Rows.Add(row);
            }

            return (dt, mappings);
        }

        #region Helper Methods for Parsing

        /// <summary>
        /// Parse decimal values safely with support for thousand separators and various formats
        /// </summary>
        private decimal? ParseDecimalSafely(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove quotes and trim whitespace
            value = value.Trim('"', ' ');
            // Remove internal padding spaces often present in fixed-width exports
            value = value.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Try parsing with various number formats
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
                return result;

            // Try with Vietnamese format (comma as thousands separator)
            if (decimal.TryParse(value.Replace(",", ""), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
                return result;

            // Log warning but don't throw
            _logger.LogWarning("⚠️ Cannot parse decimal value: '{Value}'", value);
            return null;
        }

        /// <summary>
        /// Map từ DataTables.LN01 (model CSV) sang Entities.LN01Entity (model DB)
        /// </summary>
        private static Models.Entities.LN01Entity MapLN01ToEntity(Models.DataTables.LN01 r)
        {
            var e = new Models.Entities.LN01Entity
            {
                // NGAY_DL bắt buộc
                NGAY_DL = r.NGAY_DL ?? DateTime.Today,

                // Map các cột chính; các cột còn lại giữ nguyên tên, chuyển kiểu nếu cần
                BRCD = r.BRCD ?? string.Empty,
                CUSTSEQ = r.CUSTSEQ ?? string.Empty,
                CUSTNM = r.CUSTNM,
                TAI_KHOAN = r.TAI_KHOAN ?? string.Empty,
                CCY = r.CCY,
                DU_NO = r.DU_NO,
                DSBSSEQ = r.DSBSSEQ,
                TRANSACTION_DATE = r.TRANSACTION_DATE,
                DSBSDT = r.DSBSDT,
                DISBUR_CCY = r.DISBUR_CCY,
                DISBURSEMENT_AMOUNT = r.DISBURSEMENT_AMOUNT,
                DSBSMATDT = r.DSBSMATDT,
                BSRTCD = r.BSRTCD,
                INTEREST_RATE = r.INTEREST_RATE,
                // APRSEQ column not present in DB; keep APPRSEQ only
                APPRSEQ = r.APPRSEQ,
                APPRDT = r.APPRDT,
                APPR_CCY = r.APPR_CCY,
                APPRAMT = r.APPRAMT,
                APPRMATDT = r.APPRMATDT,
                LOAN_TYPE = r.LOAN_TYPE,
                FUND_RESOURCE_CODE = r.FUND_RESOURCE_CODE,
                FUND_PURPOSE_CODE = r.FUND_PURPOSE_CODE,
                REPAYMENT_AMOUNT = r.REPAYMENT_AMOUNT,
                NEXT_REPAY_DATE = r.NEXT_REPAY_DATE,
                NEXT_REPAY_AMOUNT = r.NEXT_REPAY_AMOUNT,
                NEXT_INT_REPAY_DATE = r.NEXT_INT_REPAY_DATE,
                OFFICER_ID = r.OFFICER_ID,
                OFFICER_NAME = r.OFFICER_NAME,
                INTEREST_AMOUNT = r.INTEREST_AMOUNT,
                PASTDUE_INTEREST_AMOUNT = r.PASTDUE_INTEREST_AMOUNT,
                TOTAL_INTEREST_REPAY_AMOUNT = r.TOTAL_INTEREST_REPAY_AMOUNT,
                CUSTOMER_TYPE_CODE = r.CUSTOMER_TYPE_CODE,
                CUSTOMER_TYPE_CODE_DETAIL = r.CUSTOMER_TYPE_CODE_DETAIL,
                TRCTCD = r.TRCTCD,
                TRCTNM = r.TRCTNM,
                ADDR1 = r.ADDR1,
                PROVINCE = r.PROVINCE,
                LCLPROVINNM = r.LCLPROVINNM,
                DISTRICT = r.DISTRICT,
                LCLDISTNM = r.LCLDISTNM,
                COMMCD = r.COMMCD,
                LCLWARDNM = r.LCLWARDNM,
                LAST_REPAY_DATE = r.LAST_REPAY_DATE,
                // SECURED_PERCENT: DataTables là string?, Entities yêu cầu decimal? -> cố gắng parse nếu có
                SECURED_PERCENT = TryParseDecimal(r.SECURED_PERCENT),
                // NHOM_NO: DataTables là string?, Entities là int? -> parse nếu có
                NHOM_NO = TryParseInt(r.NHOM_NO),
                LAST_INT_CHARGE_DATE = r.LAST_INT_CHARGE_DATE,
                EXEMPTINT = TryParseDecimal(r.EXEMPTINT),
                EXEMPTINTTYPE = r.EXEMPTINTTYPE,
                EXEMPTINTAMT = r.EXEMPTINTAMT,
                GRPNO = TryParseInt(r.GRPNO),
                BUSCD = r.BUSCD,
                BSNSSCLTPCD = r.BSNSSCLTPCD,
                USRIDOP = r.USRIDOP,
                ACCRUAL_AMOUNT = r.ACCRUAL_AMOUNT,
                ACCRUAL_AMOUNT_END_OF_MONTH = r.ACCRUAL_AMOUNT_END_OF_MONTH,
                INTCMTH = r.INTCMTH,
                INTRPYMTH = r.INTRPYMTH,
                INTTRMMTH = TryParseInt(r.INTTRMMTH),
                YRDAYS = TryParseInt(r.YRDAYS),
                REMARK = r.REMARK,
                CHITIEU = r.CHITIEU,
                CTCV = r.CTCV,
                CREDIT_LINE_YPE = r.CREDIT_LINE_YPE,
                INT_LUMPSUM_PARTIAL_TYPE = r.INT_LUMPSUM_PARTIAL_TYPE,
                INT_PARTIAL_PAYMENT_TYPE = r.INT_PARTIAL_PAYMENT_TYPE,
                INT_PAYMENT_INTERVAL = TryParseInt(r.INT_PAYMENT_INTERVAL),
                AN_HAN_LAI = TryParseInt(r.AN_HAN_LAI),
                PHUONG_THUC_GIAI_NGAN_1 = r.PHUONG_THUC_GIAI_NGAN_1,
                TAI_KHOAN_GIAI_NGAN_1 = r.TAI_KHOAN_GIAI_NGAN_1,
                SO_TIEN_GIAI_NGAN_1 = r.SO_TIEN_GIAI_NGAN_1,
                PHUONG_THUC_GIAI_NGAN_2 = r.PHUONG_THUC_GIAI_NGAN_2,
                TAI_KHOAN_GIAI_NGAN_2 = r.TAI_KHOAN_GIAI_NGAN_2,
                SO_TIEN_GIAI_NGAN_2 = r.SO_TIEN_GIAI_NGAN_2,
                CMT_HC = r.CMT_HC,
                NGAY_SINH = r.NGAY_SINH,
                MA_CB_AGRI = r.MA_CB_AGRI,
                MA_NGANH_KT = r.MA_NGANH_KT,
                // TY_GIA now string in DataTables; Entities mapping not used for insert
                // If needed in future, parse to decimal here.
                OFFICER_IPCAS = r.OFFICER_IPCAS
            };

            return e;
        }

        private static decimal? TryParseDecimal(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var t = s.Trim().Trim('\'').Replace(",", "");
            if (decimal.TryParse(t, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var d)) return d;
            return null;
        }

        private static int? TryParseInt(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var t = s.Trim().Trim('\'');
            if (int.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
            return null;
        }

        /// <summary>
        /// Map từ DataTables.LN03 sang Entities.LN03Entity
        /// </summary>
        private static Models.Entities.LN03Entity MapLN03ToEntity(Models.DataTables.LN03 r)
        {
            return new Models.Entities.LN03Entity
            {
                NGAY_DL = r.NGAY_DL,
                MACHINHANH = r.MACHINHANH,
                TENCHINHANH = r.TENCHINHANH,
                MAKH = r.MAKH,
                TENKH = r.TENKH,
                SOHOPDONG = r.SOHOPDONG,
                SOTIENXLRR = r.SOTIENXLRR,
                NGAYPHATSINHXL = r.NGAYPHATSINHXL,
                THUNOSAUXL = r.THUNOSAUXL,
                CONLAINGOAIBANG = r.CONLAINGOAIBANG,
                DUNONOIBANG = r.DUNONOIBANG,
                NHOMNO = r.NHOMNO,
                MACBTD = r.MACBTD,
                TENCBTD = r.TENCBTD,
                MAPGD = r.MAPGD,
                TAIKHOANHACHTOAN = r.TAIKHOANHACHTOAN,
                REFNO = r.REFNO,
                LOAINGUONVON = r.LOAINGUONVON,
                Column18 = r.Column18,
                Column19 = r.Column19,
                Column20 = r.Column20,
                CREATED_DATE = r.CREATED_DATE == default ? DateTime.UtcNow : r.CREATED_DATE,
                FILE_ORIGIN = string.IsNullOrWhiteSpace(r.FILE_ORIGIN) ? null : r.FILE_ORIGIN
            };
        }

        /// <summary>
        /// Parse DateTime values safely with support for multiple formats
        /// </summary>
        private DateTime? ParseDateTimeSafely(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove quotes and trim
            value = value.Trim('"', ' ');

            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Try multiple date formats
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd", "dd/MM/yyyy HH:mm:ss" };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    return result;
            }

            // Try general parsing as last resort
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var generalResult))
                return generalResult;

            // Log warning but don't throw
            _logger.LogWarning("⚠️ Cannot parse DateTime value: '{Value}'", value);
            return null;
        }

        #endregion

        #region RR01 Import

        /// <summary>
        /// Import RR01 từ CSV - 25 business columns (Risk Report)
        /// NGAY_DL từ filename, strict filename validation containing "rr01"
        /// </summary>
    public async Task<DirectImportResult> ImportRR01Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [RR01] Import RR01 Risk Report from CSV: {FileName}", file.FileName);

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "RR01",
                TargetTable = "RR01",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Strict: only allow filename containing "rr01"
                if (!file.FileName.ToLower().Contains("rr01"))
                {
                    result.Success = false;
                    result.Errors.Add("❌ File name must contain 'rr01' for RR01 import");
                    return result;
                }

                // Extract NgayDL từ filename và convert sang DateTime
                var ngayDlString = ExtractNgayDLFromFileName(file.FileName);
                var ngayDlDate = DateTime.ParseExact(ngayDlString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                result.NgayDL = ngayDlString;

                // Parse RR01 CSV
                // Streaming theo batch, không xóa dữ liệu cũ
                const int BATCH_SIZE = 5000;
                var batch = new List<RR01>(BATCH_SIZE);
                var totalInserted = 0;

                using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                }))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        try
                        {
                            var rec = new RR01
                            {
                                NGAY_DL = ngayDlDate,
                                CN_LOAI_I = csv.GetField("CN_LOAI_I"),
                                BRCD = csv.GetField("BRCD"),
                                MA_KH = csv.GetField("MA_KH"),
                                TEN_KH = csv.GetField("TEN_KH"),
                                SO_LDS = csv.GetField("SO_LDS"),
                                CCY = csv.GetField("CCY"),
                                SO_LAV = csv.GetField("SO_LAV"),
                                LOAI_KH = csv.GetField("LOAI_KH"),
                                NGAY_GIAI_NGAN = ParseRR01DateSafely(csv.GetField("NGAY_GIAI_NGAN")),
                                NGAY_DEN_HAN = ParseRR01DateSafely(csv.GetField("NGAY_DEN_HAN")),
                                VAMC_FLG = csv.GetField("VAMC_FLG"),
                                NGAY_XLRR = ParseRR01DateSafely(csv.GetField("NGAY_XLRR")),
                                DUNO_GOC_BAN_DAU = ParseDecimalSafely(csv.GetField("DUNO_GOC_BAN_DAU")),
                                DUNO_LAI_TICHLUY_BD = ParseDecimalSafely(csv.GetField("DUNO_LAI_TICHLUY_BD")),
                                DOC_DAUKY_DA_THU_HT = ParseDecimalSafely(csv.GetField("DOC_DAUKY_DA_THU_HT")),
                                DUNO_GOC_HIENTAI = ParseDecimalSafely(csv.GetField("DUNO_GOC_HIENTAI")),
                                DUNO_LAI_HIENTAI = ParseDecimalSafely(csv.GetField("DUNO_LAI_HIENTAI")),
                                DUNO_NGAN_HAN = ParseDecimalSafely(csv.GetField("DUNO_NGAN_HAN")),
                                DUNO_TRUNG_HAN = ParseDecimalSafely(csv.GetField("DUNO_TRUNG_HAN")),
                                DUNO_DAI_HAN = ParseDecimalSafely(csv.GetField("DUNO_DAI_HAN")),
                                THU_GOC = ParseDecimalSafely(csv.GetField("THU_GOC")),
                                THU_LAI = ParseDecimalSafely(csv.GetField("THU_LAI")),
                                BDS = ParseDecimalSafely(csv.GetField("BDS")),
                                DS = ParseDecimalSafely(csv.GetField("DS")),
                                TSK = ParseDecimalSafely(csv.GetField("TSK"))
                            };

                            batch.Add(rec);
                            if (batch.Count >= BATCH_SIZE)
                            {
                                totalInserted += await BulkInsertGenericAsync(batch, "RR01");
                                batch.Clear();
                            }
                        }
                        catch (Exception exRow)
                        {
                            _logger.LogWarning("⚠️ [RR01] Row parsing error: {Error}", exRow.Message);
                        }
                    }
                }

                if (batch.Count > 0)
                {
                    totalInserted += await BulkInsertGenericAsync(batch, "RR01");
                    batch.Clear();
                }

                result.ProcessedRecords = totalInserted;
                result.Success = totalInserted > 0;
                if (totalInserted > 0)
                {
                    _logger.LogInformation("✅ [RR01] Import thành công {Count} records for date {Date}", totalInserted, ngayDlDate.ToString("yyyy-MM-dd"));
                    await LogImportMetadataAsync(result);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy RR01 records hợp lệ trong CSV");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [RR01] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"RR01 import error: {ex.Message}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.ProcessingTimeMs = (int)(result.EndTime - result.StartTime).TotalMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// Parse RR01 CSV với enhanced processing cho 25 business columns
        /// Hỗ trợ robust date/decimal parsing theo format riêng của RR01
        /// </summary>
        private async Task<List<RR01>> ParseRR01CsvAsync(IFormFile file, DateTime ngayDL)
        {
            var records = new List<RR01>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            using var csv = new CsvReader(reader, config);

            // Read header and check column count
            csv.Read();
            csv.ReadHeader();
            var headerCount = csv.HeaderRecord?.Length ?? 0;
            _logger.LogInformation("🔍 [RR01] CSV has {HeaderCount} headers", headerCount);

            while (csv.Read())
            {
                try
                {
                    var record = new RR01();

                    // Map the 25 business columns directly from CSV headers
                    record.CN_LOAI_I = csv.GetField("CN_LOAI_I");
                    record.BRCD = csv.GetField("BRCD");
                    record.MA_KH = csv.GetField("MA_KH");
                    record.TEN_KH = csv.GetField("TEN_KH");
                    record.SO_LDS = csv.GetField("SO_LDS");
                    record.CCY = csv.GetField("CCY");
                    record.SO_LAV = csv.GetField("SO_LAV");
                    record.LOAI_KH = csv.GetField("LOAI_KH");

                    // Parse date fields - RR01 uses YYYYMMDD format
                    record.NGAY_GIAI_NGAN = ParseRR01DateSafely(csv.GetField("NGAY_GIAI_NGAN"));
                    record.NGAY_DEN_HAN = ParseRR01DateSafely(csv.GetField("NGAY_DEN_HAN"));
                    record.NGAY_XLRR = ParseRR01DateSafely(csv.GetField("NGAY_XLRR"));

                    record.VAMC_FLG = csv.GetField("VAMC_FLG");

                    // Parse decimal fields with robust error handling
                    record.DUNO_GOC_BAN_DAU = ParseDecimalSafely(csv.GetField("DUNO_GOC_BAN_DAU"));
                    record.DUNO_LAI_TICHLUY_BD = ParseDecimalSafely(csv.GetField("DUNO_LAI_TICHLUY_BD"));
                    record.DOC_DAUKY_DA_THU_HT = ParseDecimalSafely(csv.GetField("DOC_DAUKY_DA_THU_HT"));
                    record.DUNO_GOC_HIENTAI = ParseDecimalSafely(csv.GetField("DUNO_GOC_HIENTAI"));
                    record.DUNO_LAI_HIENTAI = ParseDecimalSafely(csv.GetField("DUNO_LAI_HIENTAI"));
                    record.DUNO_NGAN_HAN = ParseDecimalSafely(csv.GetField("DUNO_NGAN_HAN"));
                    record.DUNO_TRUNG_HAN = ParseDecimalSafely(csv.GetField("DUNO_TRUNG_HAN"));
                    record.DUNO_DAI_HAN = ParseDecimalSafely(csv.GetField("DUNO_DAI_HAN"));
                    record.THU_GOC = ParseDecimalSafely(csv.GetField("THU_GOC"));
                    record.THU_LAI = ParseDecimalSafely(csv.GetField("THU_LAI"));
                    record.BDS = ParseDecimalSafely(csv.GetField("BDS"));
                    record.DS = ParseDecimalSafely(csv.GetField("DS"));
                    record.TSK = ParseDecimalSafely(csv.GetField("TSK"));

                    records.Add(record);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("⚠️ [RR01] Row parsing error: {Error}", ex.Message);
                    // Continue processing other rows
                }
            }

            return records;
        }

        /// <summary>
        /// Parse RR01 date fields - RR01 sử dụng format YYYYMMDD
        /// </summary>
        private DateTime? ParseRR01DateSafely(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove quotes and trim
            value = value.Trim('"', ' ');

            if (string.IsNullOrWhiteSpace(value))
                return null;

            // RR01 specific format: YYYYMMDD
            if (value.Length == 8 && DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            // Fallback to general date formats
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy" };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    return result;
            }

            // Log warning but don't throw
            _logger.LogWarning("⚠️ [RR01] Cannot parse date value: '{Value}'", value);
            return null;
        }

        #endregion

        /// <summary>
        /// Persist an import metadata record for UI history purposes.
        /// </summary>
        private async Task LogImportMetadataAsync(DirectImportResult result)
        {
            try
            {
                var meta = new ImportedDataRecord
                {
                    FileName = result.FileName,
                    FileType = "csv",
                    Category = result.DataType,
                    ImportDate = DateTime.UtcNow,
                    StatementDate = DateTime.TryParse(result.NgayDL, out var d) ? d : null,
                    ImportedBy = "system",
                    Status = result.Success ? "Completed" : "Failed",
                    RecordsCount = result.ProcessedRecords,
                    Notes = $"Imported into {result.TargetTable} in {result.ProcessingTimeMs} ms"
                };
                _context.ImportedDataRecords.Add(meta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log import metadata for {File}", result.FileName);
            }
        }

        #endregion

        #region Safe Data Conversion Helper Methods

        /// <summary>
        /// Safely get string value from CSV record dictionary
        /// </summary>
        private static string? SafeGetString(IDictionary<string, object> record, string columnName)
        {
            try
            {
                if (record.TryGetValue(columnName, out var value))
                {
                    return value?.ToString()?.Trim()?.Trim('"')?.Trim();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SafeGetString error for column {columnName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Safely get decimal value from CSV record dictionary
        /// </summary>
        private static decimal? SafeGetDecimal(IDictionary<string, object> record, string columnName)
        {
            try
            {
                if (record.TryGetValue(columnName, out var value))
                {
                    var str = value?.ToString()?.Trim()?.Trim('"')?.Trim();
                    if (string.IsNullOrWhiteSpace(str) || str == "0")
                        return 0;
                    
                    if (decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                        return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SafeGetDecimal error for column {columnName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Safely get DateTime value from CSV record dictionary
        /// </summary>
        private static DateTime? SafeGetDateTime(IDictionary<string, object> record, string columnName)
        {
            try
            {
                if (record.TryGetValue(columnName, out var value))
                {
                    var str = value?.ToString()?.Trim()?.Trim('"')?.Trim();
                    if (string.IsNullOrWhiteSpace(str))
                        return null;

                    // Try multiple date formats common in CSV files
                    string[] dateFormats = { 
                        "yyyyMMdd", "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy",
                        "yyyy/MM/dd", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd HH:mm:ss"
                    };

                    if (DateTime.TryParseExact(str, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        return result;
                    
                    if (DateTime.TryParse(str, out result))
                        return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
