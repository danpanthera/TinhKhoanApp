using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Khoan.Api.Data;
using Khoan.Api.Models;
using Khoan.Api.Models.Configuration;
using Khoan.Api.Models.Entities;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    /// <summary>
    /// DirectImportService - Handles CSV direct import cho DP01, DPDA, LN03
    /// CSV-First: Import trực tiếp từ CSV vào database với validation
    /// </summary>
    public class DirectImportService : IDirectImportService
    {
        // Nested metrics + parse error interfaces for DI wiring (simplified reconstruction)
        public interface IImportMetrics
        {
            void RecordBatch(string dataType, int rows, int milliseconds);
            long TotalRows { get; }
            object Snapshot();
            object Raw();
        }

    private static readonly int ParseErrorSampleLimitPerFile = 5;
        private static readonly int GlobalParseErrorCapacity = 50;
        private static readonly object _parseLock = new();
        private static readonly Queue<(string Table, string Line, string Error, DateTime At)> _recentParseErrors = new();

        private readonly ApplicationDbContext _context;
        private readonly ILogger<DirectImportService> _logger;
        private readonly DirectImportSettings _settings;
        private readonly IImportMetrics? _metrics;

        private int _currentFileParseErrors;

    // Bulk pipeline threshold (fallback to EF below this size)
    private const int BulkThreshold = 5000;

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

        public static void ClearRuntimeParseErrors()
        {
            lock (_parseLock)
            {
                _recentParseErrors.Clear();
            }
        }

        private void RecordParseError(string table, string line, string error)
        {
            lock (_parseLock)
            {
                if (_currentFileParseErrors >= ParseErrorSampleLimitPerFile) return;
                if (_recentParseErrors.Count >= GlobalParseErrorCapacity) _recentParseErrors.Dequeue();
                _recentParseErrors.Enqueue((table, line, error, DateTime.UtcNow));
                _currentFileParseErrors++;
            }
        }

        public List<object> GetRecentParseErrors(string? table = null)
        {
            lock (_parseLock)
            {
                return _recentParseErrors
                    .Where(e => table == null || string.Equals(e.Table, table, StringComparison.OrdinalIgnoreCase))
                    .Select(e => new { e.Table, e.Line, e.Error, e.At })
                    .Cast<object>()
                    .ToList();
            }
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

                // Parse DP01 CSV
                var records = await ParseDP01CsvAsync(file);
                _logger.LogInformation("📊 [DP01] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Choose pipeline: SqlBulkCopy staging for large files; EF for smaller
                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyDP01ReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "DP01");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [DP01] Import thành công {Count} records", insertedCount);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy DP01 records hợp lệ trong CSV");
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
                    // Chọn pipeline: SqlBulkCopy staging khi file lớn; EF khi nhỏ
                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyDPDAReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "DPDA");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [DPDA] Import thành công {Count} records", insertedCount);
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

                // Parse EI01 CSV (DataTables.EI01)
                var records = await ParseEI01CsvAsync(file);
                _logger.LogInformation("📊 [EI01] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Set NGAY_DL for all records from filename (CSV-first rule)
                    if (DateTime.TryParse(result.NgayDL, out var ngayDlDate))
                    {
                        foreach (var r in records)
                        {
                            r.NGAY_DL = ngayDlDate;
                            r.FILE_NAME = file.FileName;
                            r.CREATED_DATE = DateTime.UtcNow;
                            r.UPDATED_DATE = DateTime.UtcNow;
                        }
                    }

                    // Choose pipeline: SqlBulkCopy staging for large files; EF for smaller
                    int insertedCount;
                    if (records.Count >= BulkThreshold)
                    {
                        insertedCount = await BulkCopyEI01ReplaceByDateAsync(records);
                    }
                    else
                    {
                        insertedCount = await BulkInsertGenericAsync(records, "EI01");
                    }
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [EI01] Import thành công {Count} records", insertedCount);
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

                // Parse LN01 CSV
                var records = await ParseLN01CsvAsync(file);
                _logger.LogInformation("📊 [LN01] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Set NGAY_DL for all records from filename (CSV-first rule)
                    if (DateTime.TryParse(result.NgayDL, out var ngayDlDate))
                    {
                        foreach (var record in records)
                        {
                            record.NGAY_DL = ngayDlDate;
                            record.FILE_NAME = file.FileName;
                            record.CreatedAt = DateTime.UtcNow;
                            record.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    // Choose pipeline: SqlBulkCopy staging for large files; EF for smaller
                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyLN01ReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "LN01");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [LN01] Import thành công {Count} records", insertedCount);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy records hợp lệ trong LN01 CSV file");
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
                var records = await ParseLN03EnhancedAsync(file, statementDate);
                _logger.LogInformation("📊 [LN03_ENHANCED] Đã parse {Count} records", records.Count);

                if (records.Any())
                {
                    // Set NGAY_DL cho tất cả records từ filename + audit fields
                    if (DateTime.TryParse(result.NgayDL, out var ngayDlDate))
                    {
                        foreach (var r in records)
                        {
                            r.NGAY_DL = ngayDlDate;
                            r.CREATED_DATE = DateTime.UtcNow;
                            r.FILE_ORIGIN = file.FileName;
                        }
                    }

                    // Choose pipeline: SqlBulkCopy staging for large files; EF for smaller
                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyLN03ReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "LN03");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [LN03_ENHANCED] Import thành công {Count} records", insertedCount);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy LN03 records hợp lệ trong CSV");
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

                // Parse GL02 CSV
                var records = await ParseGL02CsvAsync(file);
                _logger.LogInformation("📊 [GL02] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Choose pipeline: SqlBulkCopy staging for large files; EF for smaller
                    int insertedCount;
                    if (records.Count >= BulkThreshold)
                    {
                        insertedCount = await BulkCopyGL02ReplaceByDateAsync(records);
                    }
                    else
                    {
                        insertedCount = await BulkInsertGenericAsync(records, "GL02");
                    }
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GL02] Import thành công {Count} records", insertedCount);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy GL02 records hợp lệ trong CSV");
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
                    // Chọn pipeline: SqlBulkCopy staging khi file lớn; EF khi nhỏ
                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyGL41ReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "GL41");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GL41] Import thành công {Count} records", insertedCount);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy GL41 records hợp lệ trong CSV");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL41] Import error: {Error}", ex.Message);
                result.Success = false;
                result.Errors.Add($"GL41 import error: {ex.Message}");
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
        /// Import GL01 từ CSV - 27 business columns; NGAY_DL lấy từ TR_TIME
        /// </summary>
        public async Task<DirectImportResult> ImportGL01Async(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL01] Import GL01 from CSV: {FileName}", file.FileName);

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

                // Parse GL01 CSV
                var records = await ParseGL01CsvAsync(file);
                _logger.LogInformation("📊 [GL01] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Choose pipeline: SqlBulkCopy staging for large files; EF for smaller
                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyGL01ReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "GL01");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GL01] Import thành công {Count} records", insertedCount);
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("Không tìm thấy GL01 records hợp lệ trong CSV");
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
                HeaderValidated = null
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
                HeaderValidated = null
            });

            await foreach (var record in csv.GetRecordsAsync<DP01>())
            {
                // Set audit fields
                record.CreatedAt = DateTime.UtcNow;
                record.UpdatedAt = DateTime.UtcNow;
                record.ImportDateTime = DateTime.UtcNow;
                record.FILE_NAME = file.FileName;

                records.Add(record);
            }

            return records;
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
                    CREATED_DATE = DateTime.Now,
                    UPDATED_DATE = DateTime.Now,
                    FILE_NAME = fileName
                };

                records.Add(dpda);
            }

            return records;
        }

        /// <summary>
        /// Parse EI01 CSV với 24 business columns (DataTables.EI01)
        /// </summary>
        private async Task<List<EI01>> ParseEI01CsvAsync(IFormFile file)
        {
            var records = new List<EI01>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            // Ensure DateTime parsing uses dd/MM/yyyy as primary format
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy" };
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats;

            await foreach (var record in csv.GetRecordsAsync<EI01>())
            {
                // Set audit fields
                record.CREATED_DATE = DateTime.UtcNow;
                record.UPDATED_DATE = DateTime.UtcNow;

                // Normalize date fields to datetime2 (dd/MM/yyyy)
                // CsvHelper already maps to DateTime? when possible; extra normalization hooks can be added if needed

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse LN01 CSV với 79 business columns
        /// </summary>
        private async Task<List<LN01>> ParseLN01CsvAsync(IFormFile file)
        {
            var records = new List<LN01>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            // Ensure DateTime parsing uses dd/MM/yyyy as primary format
            var dtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy" };
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats;

            await foreach (var record in csv.GetRecordsAsync<LN01>())
            {
                // Set audit fields
                record.CreatedAt = DateTime.UtcNow;
                record.UpdatedAt = DateTime.UtcNow;

                // Normalize date fields to datetime2 (dd/MM/yyyy)
                // CsvHelper already maps to DateTime? when possible; extra normalization hooks can be added if needed

                records.Add(record);
            }

            return records;
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
        /// Parse GL02 CSV với 17 business columns, set NGAY_DL từ TRDATE (DataTables.GL02)
        /// </summary>
        private async Task<List<GL02>> ParseGL02CsvAsync(IFormFile file)
        {
            var records = new List<GL02>();

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
                var record = new GL02();

                // Declare variables first to avoid scope issues
                DateTime trdate = DateTime.MinValue;
                DateTime crtdtm = DateTime.MinValue;
                decimal dramount = 0;
                decimal cramount = 0;

                // Map 17 business columns từ CSV
                if (csvRecord.TRDATE != null && DateTime.TryParse(csvRecord.TRDATE.ToString(), out trdate))
                {
                    // DataTables.GL02 does not have TRDATE column; NGAY_DL derived from TRDATE
                    record.NGAY_DL = trdate.Date;
                }

                record.TRBRCD = csvRecord.TRBRCD?.ToString() ?? string.Empty;
                record.USERID = csvRecord.USERID?.ToString();
                record.JOURSEQ = csvRecord.JOURSEQ?.ToString();
                record.DYTRSEQ = csvRecord.DYTRSEQ?.ToString();
                record.LOCAC = csvRecord.LOCAC?.ToString() ?? string.Empty;
                record.CCY = csvRecord.CCY?.ToString() ?? string.Empty;
                record.BUSCD = csvRecord.BUSCD?.ToString();
                record.UNIT = csvRecord.UNIT?.ToString();
                record.TRCD = csvRecord.TRCD?.ToString();
                record.CUSTOMER = csvRecord.CUSTOMER?.ToString();
                record.TRTP = csvRecord.TRTP?.ToString();
                record.REFERENCE = csvRecord.REFERENCE?.ToString();
                record.REMARK = csvRecord.REMARK?.ToString();

                // Parse amounts
                if (decimal.TryParse(csvRecord.DRAMOUNT?.ToString(), out dramount))
                    record.DRAMOUNT = dramount;
                if (decimal.TryParse(csvRecord.CRAMOUNT?.ToString(), out cramount))
                    record.CRAMOUNT = cramount;

                // Parse CRTDTM
                if (csvRecord.CRTDTM != null && DateTime.TryParse(csvRecord.CRTDTM.ToString(), out crtdtm))
                    record.CRTDTM = crtdtm;

                // System columns
                record.CREATED_DATE = DateTime.UtcNow;
                record.UPDATED_DATE = DateTime.UtcNow;
                record.FILE_NAME = file.FileName;

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
            dtOptions.Formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "d/M/yyyy", "yyyyMMdd" };
            var ndtOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>();
            ndtOptions.Formats = dtOptions.Formats;

            await foreach (var record in csv.GetRecordsAsync<GL01>())
            {
                // Derive NGAY_DL from TR_TIME (date component only)
                if (!string.IsNullOrWhiteSpace(record.TR_TIME))
                {
                    // Try multiple patterns observed in source data
                    var raw = record.TR_TIME!.Trim();
                    DateTime parsedDateTime;
                    if (DateTime.TryParseExact(raw, new[] { "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "yyyyMMdd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                    {
                        record.NGAY_DL = parsedDateTime.Date;
                    }
                    else if (DateTime.TryParse(raw, out var fallbackDt))
                    {
                        record.NGAY_DL = fallbackDt.Date;
                    }
                    else
                    {
                        // fallback to today but log
                        _logger.LogWarning("⚠️ [GL01] Không parse được TR_TIME='{TR_TIME}', fallback Today", record.TR_TIME);
                        record.NGAY_DL = DateTime.Today;
                    }
                }

                // Normalize audit fields
                record.CREATED_DATE = DateTime.UtcNow;
                record.UPDATED_DATE = DateTime.UtcNow;
                record.FILE_NAME = file.FileName;

                // Normalize numeric from string TR_EX_RT if needed is handled at query level; keep CSV-first mapping

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse GL41 CSV: NGAY_DL từ filename; 13 business columns; temporal table
        /// </summary>
        private async Task<List<GL41>> ParseGL41CsvAsync(IFormFile file)
        {
            var records = new List<GL41>();

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

            await foreach (var record in csv.GetRecordsAsync<GL41>())
            {
                // Set NGAY_DL từ filename (không có trong CSV)
                record.NGAY_DL = ngayDlDateTime;

                // Normalize audit fields
                record.CREATED_DATE = DateTime.UtcNow;
                record.FILE_NAME = file.FileName;

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Bulk insert generic cho bất kỳ entity type nào
        /// </summary>
        private async Task<int> BulkInsertGenericAsync<T>(List<T> records, string tableName) where T : class
        {
            try
            {
                var sw = Stopwatch.StartNew();
                _context.Set<T>().AddRange(records);
                var insertedCount = await _context.SaveChangesAsync();
                sw.Stop();

                _logger.LogInformation("✅ [BULK_INSERT] Inserted {Count} records vào {Table}", insertedCount, tableName);
                // Record EF-path metrics to Prometheus as well
                _metrics?.RecordBatch($"{tableName}-EF", insertedCount, (int)sw.ElapsedMilliseconds);
                return insertedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [BULK_INSERT] Error inserting vào {Table}: {Error}", tableName, ex.Message);
                throw;
            }
        }

        // ==========================
        // High-throughput bulk paths
        // ==========================

        private async Task<int> BulkCopyDP01ReplaceByDateAsync(List<DP01> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "DP01_Stage",
                targetTable: "DP01",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    dt.Columns.Add("MA_CN", typeof(string));
                    dt.Columns.Add("TAI_KHOAN_HACH_TOAN", typeof(string));
                    dt.Columns.Add("MA_KH", typeof(string));
                    dt.Columns.Add("TEN_KH", typeof(string));
                    dt.Columns.Add("DP_TYPE_NAME", typeof(string));
                    dt.Columns.Add("CCY", typeof(string));
                    dt.Columns.Add("CURRENT_BALANCE", typeof(decimal));
                    dt.Columns.Add("RATE", typeof(decimal));
                    dt.Columns.Add("SO_TAI_KHOAN", typeof(string));
                    dt.Columns.Add("OPENING_DATE", typeof(DateTime));
                    dt.Columns.Add("MATURITY_DATE", typeof(DateTime));
                    dt.Columns.Add("ADDRESS", typeof(string));
                    dt.Columns.Add("NOTENO", typeof(string));
                    dt.Columns.Add("MONTH_TERM", typeof(string));
                    dt.Columns.Add("TERM_DP_NAME", typeof(string));
                    dt.Columns.Add("TIME_DP_NAME", typeof(string));
                    dt.Columns.Add("MA_PGD", typeof(string));
                    dt.Columns.Add("TEN_PGD", typeof(string));
                    dt.Columns.Add("DP_TYPE_CODE", typeof(string));
                    dt.Columns.Add("RENEW_DATE", typeof(DateTime));
                    dt.Columns.Add("CUST_TYPE", typeof(string));
                    dt.Columns.Add("CUST_TYPE_NAME", typeof(string));
                    dt.Columns.Add("CUST_TYPE_DETAIL", typeof(string));
                    dt.Columns.Add("CUST_DETAIL_NAME", typeof(string));
                    dt.Columns.Add("PREVIOUS_DP_CAP_DATE", typeof(DateTime));
                    dt.Columns.Add("NEXT_DP_CAP_DATE", typeof(DateTime));
                    dt.Columns.Add("ID_NUMBER", typeof(string));
                    dt.Columns.Add("ISSUED_BY", typeof(string));
                    dt.Columns.Add("ISSUE_DATE", typeof(DateTime));
                    dt.Columns.Add("SEX_TYPE", typeof(string));
                    dt.Columns.Add("BIRTH_DATE", typeof(DateTime));
                    dt.Columns.Add("TELEPHONE", typeof(string));
                    dt.Columns.Add("ACRUAL_AMOUNT", typeof(decimal));
                    dt.Columns.Add("ACRUAL_AMOUNT_END", typeof(decimal));
                    dt.Columns.Add("ACCOUNT_STATUS", typeof(string));
                    dt.Columns.Add("DRAMT", typeof(decimal));
                    dt.Columns.Add("CRAMT", typeof(decimal));
                    dt.Columns.Add("EMPLOYEE_NUMBER", typeof(string));
                    dt.Columns.Add("EMPLOYEE_NAME", typeof(string));
                    dt.Columns.Add("SPECIAL_RATE", typeof(decimal));
                    dt.Columns.Add("AUTO_RENEWAL", typeof(string));
                    dt.Columns.Add("CLOSE_DATE", typeof(DateTime));
                    dt.Columns.Add("LOCAL_PROVIN_NAME", typeof(string));
                    dt.Columns.Add("LOCAL_DISTRICT_NAME", typeof(string));
                    dt.Columns.Add("LOCAL_WARD_NAME", typeof(string));
                    dt.Columns.Add("TERM_DP_TYPE", typeof(string));
                    dt.Columns.Add("TIME_DP_TYPE", typeof(string));
                    dt.Columns.Add("STATES_CODE", typeof(string));
                    dt.Columns.Add("ZIP_CODE", typeof(string));
                    dt.Columns.Add("COUNTRY_CODE", typeof(string));
                    dt.Columns.Add("TAX_CODE_LOCATION", typeof(string));
                    dt.Columns.Add("MA_CAN_BO_PT", typeof(string));
                    dt.Columns.Add("TEN_CAN_BO_PT", typeof(string));
                    dt.Columns.Add("PHONG_CAN_BO_PT", typeof(string));
                    dt.Columns.Add("NGUOI_NUOC_NGOAI", typeof(string));
                    dt.Columns.Add("QUOC_TICH", typeof(string));
                    dt.Columns.Add("MA_CAN_BO_AGRIBANK", typeof(string));
                    dt.Columns.Add("NGUOI_GIOI_THIEU", typeof(string));
                    dt.Columns.Add("TEN_NGUOI_GIOI_THIEU", typeof(string));
                    dt.Columns.Add("CONTRACT_COUTS_DAY", typeof(string));
                    dt.Columns.Add("SO_KY_AD_LSDB", typeof(string));
                    dt.Columns.Add("UNTBUSCD", typeof(string));
                    dt.Columns.Add("TYGIA", typeof(decimal));
                    dt.Columns.Add("FILE_NAME", typeof(string));
                    dt.Columns.Add("DataSource", typeof(string));
                    dt.Columns.Add("ImportDateTime", typeof(DateTime));
                    dt.Columns.Add("CreatedAt", typeof(DateTime));
                    dt.Columns.Add("UpdatedAt", typeof(DateTime));
                    dt.Columns.Add("CreatedBy", typeof(string));
                    dt.Columns.Add("UpdatedBy", typeof(string));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            (object?)r.NGAY_DL ?? DBNull.Value,
                            r.MA_CN,
                            r.TAI_KHOAN_HACH_TOAN,
                            r.MA_KH,
                            r.TEN_KH,
                            r.DP_TYPE_NAME,
                            r.CCY,
                            (object?)r.CURRENT_BALANCE ?? DBNull.Value,
                            (object?)r.RATE ?? DBNull.Value,
                            r.SO_TAI_KHOAN,
                            (object?)r.OPENING_DATE ?? DBNull.Value,
                            (object?)r.MATURITY_DATE ?? DBNull.Value,
                            r.ADDRESS,
                            r.NOTENO,
                            r.MONTH_TERM,
                            r.TERM_DP_NAME,
                            r.TIME_DP_NAME,
                            r.MA_PGD,
                            r.TEN_PGD,
                            r.DP_TYPE_CODE,
                            (object?)r.RENEW_DATE ?? DBNull.Value,
                            r.CUST_TYPE,
                            r.CUST_TYPE_NAME,
                            r.CUST_TYPE_DETAIL,
                            r.CUST_DETAIL_NAME,
                            (object?)r.PREVIOUS_DP_CAP_DATE ?? DBNull.Value,
                            (object?)r.NEXT_DP_CAP_DATE ?? DBNull.Value,
                            r.ID_NUMBER,
                            r.ISSUED_BY,
                            (object?)r.ISSUE_DATE ?? DBNull.Value,
                            r.SEX_TYPE,
                            (object?)r.BIRTH_DATE ?? DBNull.Value,
                            r.TELEPHONE,
                            (object?)r.ACRUAL_AMOUNT ?? DBNull.Value,
                            (object?)r.ACRUAL_AMOUNT_END ?? DBNull.Value,
                            r.ACCOUNT_STATUS,
                            (object?)r.DRAMT ?? DBNull.Value,
                            (object?)r.CRAMT ?? DBNull.Value,
                            r.EMPLOYEE_NUMBER,
                            r.EMPLOYEE_NAME,
                            (object?)r.SPECIAL_RATE ?? DBNull.Value,
                            r.AUTO_RENEWAL,
                            (object?)r.CLOSE_DATE ?? DBNull.Value,
                            r.LOCAL_PROVIN_NAME,
                            r.LOCAL_DISTRICT_NAME,
                            r.LOCAL_WARD_NAME,
                            r.TERM_DP_TYPE,
                            r.TIME_DP_TYPE,
                            r.STATES_CODE,
                            r.ZIP_CODE,
                            r.COUNTRY_CODE,
                            r.TAX_CODE_LOCATION,
                            r.MA_CAN_BO_PT,
                            r.TEN_CAN_BO_PT,
                            r.PHONG_CAN_BO_PT,
                            r.NGUOI_NUOC_NGOAI,
                            r.QUOC_TICH,
                            r.MA_CAN_BO_AGRIBANK,
                            r.NGUOI_GIOI_THIEU,
                            r.TEN_NGUOI_GIOI_THIEU,
                            r.CONTRACT_COUTS_DAY,
                            r.SO_KY_AD_LSDB,
                            r.UNTBUSCD,
                            (object?)r.TYGIA ?? DBNull.Value,
                            r.FILE_NAME,
                            r.DataSource,
                            r.ImportDateTime,
                            r.CreatedAt,
                            r.UpdatedAt,
                            r.CreatedBy,
                            r.UpdatedBy
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("DP01-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "DP01")
            );
        }

        private async Task<int> BulkCopyLN01ReplaceByDateAsync(List<LN01> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "LN01_Stage",
                targetTable: "LN01",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    // Note: add only a subset crucial for perf; assume stage has compatible schema
                    // For safety, include widely used fields and system columns
                    dt.Columns.Add("BRCD", typeof(string));
                    dt.Columns.Add("CUSTSEQ", typeof(string));
                    dt.Columns.Add("CUSTNM", typeof(string));
                    dt.Columns.Add("TAI_KHOAN", typeof(string));
                    dt.Columns.Add("CCY", typeof(string));
                    dt.Columns.Add("DU_NO", typeof(decimal));
                    dt.Columns.Add("DSBSSEQ", typeof(string));
                    dt.Columns.Add("TRANSACTION_DATE", typeof(DateTime));
                    dt.Columns.Add("DISBURSEMENT_AMOUNT", typeof(decimal));
                    dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                    dt.Columns.Add("UPDATED_DATE", typeof(DateTime));
                    dt.Columns.Add("FILE_NAME", typeof(string));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            (object?)r.NGAY_DL ?? DBNull.Value,
                            r.BRCD,
                            r.CUSTSEQ,
                            r.CUSTNM,
                            r.TAI_KHOAN,
                            r.CCY,
                            (object?)r.DU_NO ?? DBNull.Value,
                            r.DSBSSEQ,
                            (object?)r.TRANSACTION_DATE ?? DBNull.Value,
                            (object?)r.DISBURSEMENT_AMOUNT ?? DBNull.Value,
                            r.CreatedAt,
                            r.UpdatedAt,
                            r.FILE_NAME
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("LN01-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "LN01")
            );
        }

        private async Task<int> BulkCopyLN03ReplaceByDateAsync(List<LN03> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "LN03_Stage",
                targetTable: "LN03",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    dt.Columns.Add("MACHINHANH", typeof(string));
                    dt.Columns.Add("TENCHINHANH", typeof(string));
                    dt.Columns.Add("MAKH", typeof(string));
                    dt.Columns.Add("TENKH", typeof(string));
                    dt.Columns.Add("SOHOPDONG", typeof(string));
                    dt.Columns.Add("SOTIENXLRR", typeof(decimal));
                    dt.Columns.Add("NGAYPHATSINHXL", typeof(DateTime));
                    dt.Columns.Add("THUNOSAUXL", typeof(decimal));
                    dt.Columns.Add("CONLAINGOAIBANG", typeof(decimal));
                    dt.Columns.Add("DUNONOIBANG", typeof(decimal));
                    dt.Columns.Add("NHOMNO", typeof(string));
                    dt.Columns.Add("MACBTD", typeof(string));
                    dt.Columns.Add("TENCBTD", typeof(string));
                    dt.Columns.Add("MAPGD", typeof(string));
                    dt.Columns.Add("TAIKHOANHACHTOAN", typeof(string));
                    dt.Columns.Add("REFNO", typeof(string));
                    dt.Columns.Add("LOAINGUONVON", typeof(string));
                    dt.Columns.Add("Column18", typeof(string));
                    dt.Columns.Add("Column19", typeof(string));
                    dt.Columns.Add("Column20", typeof(decimal));
                    dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                    dt.Columns.Add("FILE_ORIGIN", typeof(string));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            r.NGAY_DL,
                            r.MACHINHANH,
                            r.TENCHINHANH,
                            r.MAKH,
                            r.TENKH,
                            r.SOHOPDONG,
                            (object?)r.SOTIENXLRR ?? DBNull.Value,
                            (object?)r.NGAYPHATSINHXL ?? DBNull.Value,
                            (object?)r.THUNOSAUXL ?? DBNull.Value,
                            (object?)r.CONLAINGOAIBANG ?? DBNull.Value,
                            (object?)r.DUNONOIBANG ?? DBNull.Value,
                            r.NHOMNO,
                            r.MACBTD,
                            r.TENCBTD,
                            r.MAPGD,
                            r.TAIKHOANHACHTOAN,
                            r.REFNO,
                            r.LOAINGUONVON,
                            r.Column18,
                            r.Column19,
                            (object?)r.Column20 ?? DBNull.Value,
                            r.CREATED_DATE,
                            r.FILE_ORIGIN
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("LN03-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "LN03")
            );
        }

        private async Task<int> BulkCopyGL01ReplaceByDateAsync(List<GL01> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "GL01_Stage",
                targetTable: "GL01",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    dt.Columns.Add("STS", typeof(string));
                    dt.Columns.Add("NGAY_GD", typeof(DateTime));
                    dt.Columns.Add("NGUOI_TAO", typeof(string));
                    dt.Columns.Add("DYSEQ", typeof(string));
                    dt.Columns.Add("TR_TYPE", typeof(string));
                    dt.Columns.Add("DT_SEQ", typeof(string));
                    dt.Columns.Add("TAI_KHOAN", typeof(string));
                    dt.Columns.Add("TEN_TK", typeof(string));
                    dt.Columns.Add("SO_TIEN_GD", typeof(decimal));
                    dt.Columns.Add("POST_BR", typeof(string));
                    dt.Columns.Add("LOAI_TIEN", typeof(string));
                    dt.Columns.Add("DR_CR", typeof(string));
                    dt.Columns.Add("MA_KH", typeof(string));
                    dt.Columns.Add("TEN_KH", typeof(string));
                    dt.Columns.Add("CCA_USRID", typeof(string));
                    dt.Columns.Add("TR_EX_RT", typeof(string));
                    dt.Columns.Add("REMARK", typeof(string));
                    dt.Columns.Add("BUS_CODE", typeof(string));
                    dt.Columns.Add("UNIT_BUS_CODE", typeof(string));
                    dt.Columns.Add("TR_CODE", typeof(string));
                    dt.Columns.Add("TR_NAME", typeof(string));
                    dt.Columns.Add("REFERENCE", typeof(string));
                    dt.Columns.Add("VALUE_DATE", typeof(DateTime));
                    dt.Columns.Add("DEPT_CODE", typeof(string));
                    dt.Columns.Add("TR_TIME", typeof(string));
                    dt.Columns.Add("COMFIRM", typeof(string));
                    dt.Columns.Add("TRDT_TIME", typeof(string));
                    dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                    dt.Columns.Add("UPDATED_DATE", typeof(DateTime));
                    dt.Columns.Add("FILE_NAME", typeof(string));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            r.NGAY_DL,
                            r.STS,
                            (object?)r.NGAY_GD ?? DBNull.Value,
                            r.NGUOI_TAO,
                            r.DYSEQ,
                            r.TR_TYPE,
                            r.DT_SEQ,
                            r.TAI_KHOAN,
                            r.TEN_TK,
                            (object?)r.SO_TIEN_GD ?? DBNull.Value,
                            r.POST_BR,
                            r.LOAI_TIEN,
                            r.DR_CR,
                            r.MA_KH,
                            r.TEN_KH,
                            r.CCA_USRID,
                            r.TR_EX_RT,
                            r.REMARK,
                            r.BUS_CODE,
                            r.UNIT_BUS_CODE,
                            r.TR_CODE,
                            r.TR_NAME,
                            r.REFERENCE,
                            (object?)r.VALUE_DATE ?? DBNull.Value,
                            r.DEPT_CODE,
                            r.TR_TIME,
                            r.COMFIRM,
                            r.TRDT_TIME,
                            r.CREATED_DATE,
                            r.UPDATED_DATE,
                            r.FILE_NAME
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("GL01-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "GL01")
            );
        }

        private async Task<int> BulkCopyDPDAReplaceByDateAsync(List<DPDA> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "DPDA_Stage",
                targetTable: "DPDA",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    dt.Columns.Add("MA_CHI_NHANH", typeof(string));
                    dt.Columns.Add("MA_KHACH_HANG", typeof(string));
                    dt.Columns.Add("TEN_KHACH_HANG", typeof(string));
                    dt.Columns.Add("SO_TAI_KHOAN", typeof(string));
                    dt.Columns.Add("LOAI_THE", typeof(string));
                    dt.Columns.Add("SO_THE", typeof(string));
                    dt.Columns.Add("NGAY_NOP_DON", typeof(DateTime));
                    dt.Columns.Add("NGAY_PHAT_HANH", typeof(DateTime));
                    dt.Columns.Add("USER_PHAT_HANH", typeof(string));
                    dt.Columns.Add("TRANG_THAI", typeof(string));
                    dt.Columns.Add("PHAN_LOAI", typeof(string));
                    dt.Columns.Add("GIAO_THE", typeof(string));
                    dt.Columns.Add("LOAI_PHAT_HANH", typeof(string));
                    dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                    dt.Columns.Add("UPDATED_DATE", typeof(DateTime));
                    dt.Columns.Add("FILE_NAME", typeof(string));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            r.NGAY_DL,
                            r.MA_CHI_NHANH,
                            r.MA_KHACH_HANG,
                            r.TEN_KHACH_HANG,
                            r.SO_TAI_KHOAN,
                            r.LOAI_THE,
                            r.SO_THE,
                            (object?)r.NGAY_NOP_DON ?? DBNull.Value,
                            (object?)r.NGAY_PHAT_HANH ?? DBNull.Value,
                            r.USER_PHAT_HANH,
                            r.TRANG_THAI,
                            r.PHAN_LOAI,
                            r.GIAO_THE,
                            r.LOAI_PHAT_HANH,
                            r.CREATED_DATE,
                            r.UPDATED_DATE,
                            r.FILE_NAME
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("DPDA-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "DPDA")
            );
        }

        private async Task<int> BulkCopyGL41ReplaceByDateAsync(List<GL41> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "GL41_Stage",
                targetTable: "GL41",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    dt.Columns.Add("MA_CN", typeof(string));
                    dt.Columns.Add("LOAI_TIEN", typeof(string));
                    dt.Columns.Add("MA_TK", typeof(string));
                    dt.Columns.Add("TEN_TK", typeof(string));
                    dt.Columns.Add("LOAI_BT", typeof(string));
                    dt.Columns.Add("DN_DAUKY", typeof(decimal));
                    dt.Columns.Add("DC_DAUKY", typeof(decimal));
                    dt.Columns.Add("SBT_NO", typeof(decimal));
                    dt.Columns.Add("ST_GHINO", typeof(decimal));
                    dt.Columns.Add("SBT_CO", typeof(decimal));
                    dt.Columns.Add("ST_GHICO", typeof(decimal));
                    dt.Columns.Add("DN_CUOIKY", typeof(decimal));
                    dt.Columns.Add("DC_CUOIKY", typeof(decimal));
                    dt.Columns.Add("FILE_NAME", typeof(string));
                    dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                    dt.Columns.Add("BATCH_ID", typeof(string));
                    dt.Columns.Add("IMPORT_SESSION_ID", typeof(string));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            r.NGAY_DL,
                            r.MA_CN,
                            r.LOAI_TIEN,
                            r.MA_TK,
                            r.TEN_TK,
                            r.LOAI_BT,
                            (object?)r.DN_DAUKY ?? DBNull.Value,
                            (object?)r.DC_DAUKY ?? DBNull.Value,
                            (object?)r.SBT_NO ?? DBNull.Value,
                            (object?)r.ST_GHINO ?? DBNull.Value,
                            (object?)r.SBT_CO ?? DBNull.Value,
                            (object?)r.ST_GHICO ?? DBNull.Value,
                            (object?)r.DN_CUOIKY ?? DBNull.Value,
                            (object?)r.DC_CUOIKY ?? DBNull.Value,
                            r.FILE_NAME,
                            r.CREATED_DATE,
                            r.BATCH_ID,
                            r.IMPORT_SESSION_ID
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("GL41-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "GL41")
            );
        }

        private async Task<int> BulkCopyRR01ReplaceByDateAsync(List<RR01> records)
        {
            return await BulkCopyReplaceByDateAsync(
                stageTable: "RR01_Stage",
                targetTable: "RR01",
                dateColumn: "NGAY_DL",
                buildTable: () =>
                {
                    var dt = new DataTable();
                    dt.Columns.Add("NGAY_DL", typeof(DateTime));
                    dt.Columns.Add("CN_LOAI_I", typeof(string));
                    dt.Columns.Add("BRCD", typeof(string));
                    dt.Columns.Add("MA_KH", typeof(string));
                    dt.Columns.Add("TEN_KH", typeof(string));
                    dt.Columns.Add("SO_LDS", typeof(string));
                    dt.Columns.Add("CCY", typeof(string));
                    dt.Columns.Add("SO_LAV", typeof(string));
                    dt.Columns.Add("LOAI_KH", typeof(string));
                    dt.Columns.Add("NGAY_GIAI_NGAN", typeof(DateTime));
                    dt.Columns.Add("NGAY_DEN_HAN", typeof(DateTime));
                    dt.Columns.Add("VAMC_FLG", typeof(string));
                    dt.Columns.Add("NGAY_XLRR", typeof(DateTime));
                    dt.Columns.Add("DUNO_GOC_BAN_DAU", typeof(decimal));
                    dt.Columns.Add("DUNO_LAI_TICHLUY_BD", typeof(decimal));
                    dt.Columns.Add("DOC_DAUKY_DA_THU_HT", typeof(decimal));
                    dt.Columns.Add("DUNO_GOC_HIENTAI", typeof(decimal));
                    dt.Columns.Add("DUNO_LAI_HIENTAI", typeof(decimal));
                    dt.Columns.Add("DUNO_NGAN_HAN", typeof(decimal));
                    dt.Columns.Add("DUNO_TRUNG_HAN", typeof(decimal));
                    dt.Columns.Add("DUNO_DAI_HAN", typeof(decimal));
                    dt.Columns.Add("THU_GOC", typeof(decimal));
                    dt.Columns.Add("THU_LAI", typeof(decimal));
                    dt.Columns.Add("BDS", typeof(decimal));
                    dt.Columns.Add("DS", typeof(decimal));
                    dt.Columns.Add("TSK", typeof(decimal));
                    dt.Columns.Add("FILE_NAME", typeof(string));
                    dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                    dt.Columns.Add("UPDATED_DATE", typeof(DateTime));
                    return dt;
                },
                addRow: (dt) =>
                {
                    foreach (var r in records)
                    {
                        dt.Rows.Add(
                            r.NGAY_DL,
                            r.CN_LOAI_I,
                            r.BRCD,
                            r.MA_KH,
                            r.TEN_KH,
                            r.SO_LDS,
                            r.CCY,
                            r.SO_LAV,
                            r.LOAI_KH,
                            (object?)r.NGAY_GIAI_NGAN ?? DBNull.Value,
                            (object?)r.NGAY_DEN_HAN ?? DBNull.Value,
                            r.VAMC_FLG,
                            (object?)r.NGAY_XLRR ?? DBNull.Value,
                            (object?)r.DUNO_GOC_BAN_DAU ?? DBNull.Value,
                            (object?)r.DUNO_LAI_TICHLUY_BD ?? DBNull.Value,
                            (object?)r.DOC_DAUKY_DA_THU_HT ?? DBNull.Value,
                            (object?)r.DUNO_GOC_HIENTAI ?? DBNull.Value,
                            (object?)r.DUNO_LAI_HIENTAI ?? DBNull.Value,
                            (object?)r.DUNO_NGAN_HAN ?? DBNull.Value,
                            (object?)r.DUNO_TRUNG_HAN ?? DBNull.Value,
                            (object?)r.DUNO_DAI_HAN ?? DBNull.Value,
                            (object?)r.THU_GOC ?? DBNull.Value,
                            (object?)r.THU_LAI ?? DBNull.Value,
                            (object?)r.BDS ?? DBNull.Value,
                            (object?)r.DS ?? DBNull.Value,
                            (object?)r.TSK ?? DBNull.Value,
                            r.FILE_NAME,
                            r.CREATED_DATE,
                            (object?)r.UPDATED_DATE ?? DBNull.Value
                        );
                    }
                },
                onMetrics: (ms) => _metrics?.RecordBatch("RR01-BULK", records.Count, ms),
                fallback: () => BulkInsertGenericAsync(records, "RR01")
            );
        }

        // Generic helper: SqlBulkCopy into stage, then replace-by-date into target
        private async Task<int> BulkCopyReplaceByDateAsync(
            string stageTable,
            string targetTable,
            string dateColumn,
            Func<DataTable> buildTable,
            Action<DataTable> addRow,
            Action<int>? onMetrics,
            Func<Task<int>> fallback)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var conn = (SqlConnection)_context.Database.GetDbConnection();
                var shouldClose = false;
                if (conn.State != ConnectionState.Open) { await conn.OpenAsync(); shouldClose = true; }

                using var tx = await conn.BeginTransactionAsync();
                try
                {
                    var dt = buildTable();
                    addRow(dt);

                    // Bulk copy to stage
                    using (var bcp = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, (SqlTransaction)tx))
                    {
                        bcp.DestinationTableName = stageTable;
                        foreach (DataColumn c in dt.Columns)
                        {
                            bcp.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                        }
                        await bcp.WriteToServerAsync(dt);
                    }

                    // Collect dates and replace-by-date
                    var dates = new HashSet<DateTime>();
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r[dateColumn] is DateTime d)
                            dates.Add(d.Date);
                    }

                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = (SqlTransaction)tx;
                    var columnList = string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    cmd.CommandText =
                        $"DELETE FROM {targetTable} WHERE CAST({dateColumn} AS date) = @d;" +
                        $"INSERT INTO {targetTable} ({columnList}) " +
                        $"SELECT {columnList} FROM {stageTable} WHERE CAST({dateColumn} AS date) = @d;";
                    var p = cmd.CreateParameter(); p.ParameterName = "@d"; p.SqlDbType = SqlDbType.Date; cmd.Parameters.Add(p);
                    foreach (var d in dates)
                    {
                        p.Value = d;
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Optional clean stage
                    using var clean = conn.CreateCommand();
                    clean.Transaction = (SqlTransaction)tx;
                    clean.CommandText = $"DELETE FROM {stageTable} WHERE CAST({dateColumn} AS date) = @d";
                    var p2 = clean.CreateParameter(); p2.ParameterName = "@d"; p2.SqlDbType = SqlDbType.Date; clean.Parameters.Add(p2);
                    foreach (var d in dates) { p2.Value = d; await clean.ExecuteNonQueryAsync(); }

                    await tx.CommitAsync();
                    sw.Stop();
                    onMetrics?.Invoke((int)sw.ElapsedMilliseconds);
                    return dt.Rows.Count; // approximate inserted rows
                }
                catch (SqlException ex) when (ex.Number == 208)
                {
                    await tx.RollbackAsync();
                    _logger.LogWarning("⚠️ [{Target}] Staging table missing, fallback to EF: {Msg}", targetTable, ex.Message);
                    return await fallback();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
                finally
                {
                    if (shouldClose) await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [{Target}] Bulk pipeline failed, fallback to EF", targetTable);
                return await fallback();
            }
        }

        private async Task<int> BulkCopyGL02ReplaceByDateAsync(List<GL02> records)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                if (records.Count == 0) return 0;
                var conn = (SqlConnection)_context.Database.GetDbConnection();
                var shouldClose = false;
                if (conn.State != ConnectionState.Open) { await conn.OpenAsync(); shouldClose = true; }

                using var tx = await conn.BeginTransactionAsync();
                try
                {
                    // Build DataTable for staging
                    var table = new DataTable();
                    table.Columns.Add("NGAY_DL", typeof(DateTime));
                    table.Columns.Add("TRBRCD", typeof(string));
                    table.Columns.Add("USERID", typeof(string));
                    table.Columns.Add("JOURSEQ", typeof(string));
                    table.Columns.Add("DYTRSEQ", typeof(string));
                    table.Columns.Add("LOCAC", typeof(string));
                    table.Columns.Add("CCY", typeof(string));
                    table.Columns.Add("BUSCD", typeof(string));
                    table.Columns.Add("UNIT", typeof(string));
                    table.Columns.Add("TRCD", typeof(string));
                    table.Columns.Add("CUSTOMER", typeof(string));
                    table.Columns.Add("TRTP", typeof(string));
                    table.Columns.Add("REFERENCE", typeof(string));
                    table.Columns.Add("REMARK", typeof(string));
                    table.Columns.Add("DRAMOUNT", typeof(decimal));
                    table.Columns.Add("CRAMOUNT", typeof(decimal));
                    table.Columns.Add("CRTDTM", typeof(DateTime));
                    table.Columns.Add("CREATED_DATE", typeof(DateTime));
                    table.Columns.Add("UPDATED_DATE", typeof(DateTime));
                    table.Columns.Add("FILE_NAME", typeof(string));

                    foreach (var r in records)
                    {
                        table.Rows.Add(
                            r.NGAY_DL,
                            r.TRBRCD ?? string.Empty,
                            r.USERID,
                            r.JOURSEQ,
                            r.DYTRSEQ,
                            r.LOCAC ?? string.Empty,
                            r.CCY ?? string.Empty,
                            r.BUSCD,
                            r.UNIT,
                            r.TRCD,
                            r.CUSTOMER,
                            r.TRTP,
                            r.REFERENCE,
                            r.REMARK,
                            r.DRAMOUNT ?? 0m,
                            r.CRAMOUNT ?? 0m,
                            (object?)r.CRTDTM ?? DBNull.Value,
                            r.CREATED_DATE,
                            (object?)r.UPDATED_DATE ?? DBNull.Value,
                            r.FILE_NAME ?? string.Empty
                        );
                    }

                    // Bulk copy into staging table
                    using (var bcp = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, (SqlTransaction)tx))
                    {
                        bcp.DestinationTableName = "GL02_Stage";
                        foreach (DataColumn c in table.Columns)
                        {
                            bcp.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                        }
                        await bcp.WriteToServerAsync(table);
                    }

                    // Replace-by-date: delete and insert from stage
                    var dates = records.Select(r => r.NGAY_DL.Date).Distinct().ToList();
                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = (SqlTransaction)tx;
                    cmd.CommandText = @"
DELETE FROM GL02 WHERE CAST(NGAY_DL AS date) = @d;
INSERT INTO GL02 (NGAY_DL, TRBRCD, USERID, JOURSEQ, DYTRSEQ, LOCAC, CCY, BUSCD, UNIT, TRCD, CUSTOMER, TRTP, REFERENCE, REMARK, DRAMOUNT, CRAMOUNT, CRTDTM, CREATED_DATE, UPDATED_DATE, FILE_NAME)
SELECT NGAY_DL, TRBRCD, USERID, JOURSEQ, DYTRSEQ, LOCAC, CCY, BUSCD, UNIT, TRCD, CUSTOMER, TRTP, REFERENCE, REMARK, DRAMOUNT, CRAMOUNT, CRTDTM, CREATED_DATE, UPDATED_DATE, FILE_NAME
FROM GL02_Stage WHERE CAST(NGAY_DL AS date) = @d;";
                    var p = cmd.CreateParameter(); p.ParameterName = "@d"; p.SqlDbType = SqlDbType.Date; cmd.Parameters.Add(p);
                    var total = 0;
                    foreach (var d in dates)
                    {
                        p.Value = d;
                        total += await cmd.ExecuteNonQueryAsync();
                    }

                    // Optional: clean staged rows for processed dates
                    using var clean = conn.CreateCommand();
                    clean.Transaction = (SqlTransaction)tx;
                    clean.CommandText = "DELETE FROM GL02_Stage WHERE CAST(NGAY_DL AS date) = @d";
                    var p2 = clean.CreateParameter(); p2.ParameterName = "@d"; p2.SqlDbType = SqlDbType.Date; clean.Parameters.Add(p2);
                    foreach (var d in dates) { p2.Value = d; await clean.ExecuteNonQueryAsync(); }

                    await tx.CommitAsync();
                    sw.Stop();
                    _metrics?.RecordBatch("GL02-BULK", records.Count, (int)sw.ElapsedMilliseconds);
                    // total contains rows deleted+inserted per date; return inserted count approximate
                    return records.Count;
                }
                catch (SqlException ex) when (ex.Number == 208 /* invalid object name */)
                {
                    await tx.RollbackAsync();
                    _logger.LogWarning("⚠️ [GL02] Staging table missing, fallback to EF: {Msg}", ex.Message);
                    return await BulkInsertGenericAsync(records, "GL02");
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
                finally
                {
                    if (shouldClose) await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL02] Bulk pipeline failed, fallback to EF");
                return await BulkInsertGenericAsync(records, "GL02");
            }
        }

        private async Task<int> BulkCopyEI01ReplaceByDateAsync(List<EI01> records)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                if (records.Count == 0) return 0;
                var conn = (SqlConnection)_context.Database.GetDbConnection();
                var shouldClose = false;
                if (conn.State != ConnectionState.Open) { await conn.OpenAsync(); shouldClose = true; }

                using var tx = await conn.BeginTransactionAsync();
                try
                {
                    // Build DataTable for staging
                    var table = new DataTable();
                    table.Columns.Add("NGAY_DL", typeof(DateTime));
                    table.Columns.Add("MA_CN", typeof(string));
                    table.Columns.Add("MA_KH", typeof(string));
                    table.Columns.Add("TEN_KH", typeof(string));
                    table.Columns.Add("LOAI_KH", typeof(string));
                    table.Columns.Add("SDT_EMB", typeof(string));
                    table.Columns.Add("TRANG_THAI_EMB", typeof(string));
                    table.Columns.Add("NGAY_DK_EMB", typeof(DateTime));
                    table.Columns.Add("SDT_OTT", typeof(string));
                    table.Columns.Add("TRANG_THAI_OTT", typeof(string));
                    table.Columns.Add("NGAY_DK_OTT", typeof(DateTime));
                    table.Columns.Add("SDT_SMS", typeof(string));
                    table.Columns.Add("TRANG_THAI_SMS", typeof(string));
                    table.Columns.Add("NGAY_DK_SMS", typeof(DateTime));
                    table.Columns.Add("SDT_SAV", typeof(string));
                    table.Columns.Add("TRANG_THAI_SAV", typeof(string));
                    table.Columns.Add("NGAY_DK_SAV", typeof(DateTime));
                    table.Columns.Add("SDT_LN", typeof(string));
                    table.Columns.Add("TRANG_THAI_LN", typeof(string));
                    table.Columns.Add("NGAY_DK_LN", typeof(DateTime));
                    table.Columns.Add("USER_EMB", typeof(string));
                    table.Columns.Add("USER_OTT", typeof(string));
                    table.Columns.Add("USER_SMS", typeof(string));
                    table.Columns.Add("USER_SAV", typeof(string));
                    table.Columns.Add("USER_LN", typeof(string));
                    table.Columns.Add("CREATED_DATE", typeof(DateTime));
                    table.Columns.Add("UPDATED_DATE", typeof(DateTime));
                    table.Columns.Add("FILE_NAME", typeof(string));

                    foreach (var r in records)
                    {
                        table.Rows.Add(
                            r.NGAY_DL,
                            r.MA_CN,
                            r.MA_KH,
                            r.TEN_KH,
                            r.LOAI_KH,
                            r.SDT_EMB,
                            r.TRANG_THAI_EMB,
                            (object?)r.NGAY_DK_EMB ?? DBNull.Value,
                            r.SDT_OTT,
                            r.TRANG_THAI_OTT,
                            (object?)r.NGAY_DK_OTT ?? DBNull.Value,
                            r.SDT_SMS,
                            r.TRANG_THAI_SMS,
                            (object?)r.NGAY_DK_SMS ?? DBNull.Value,
                            r.SDT_SAV,
                            r.TRANG_THAI_SAV,
                            (object?)r.NGAY_DK_SAV ?? DBNull.Value,
                            r.SDT_LN,
                            r.TRANG_THAI_LN,
                            (object?)r.NGAY_DK_LN ?? DBNull.Value,
                            r.USER_EMB,
                            r.USER_OTT,
                            r.USER_SMS,
                            r.USER_SAV,
                            r.USER_LN,
                            r.CREATED_DATE,
                            (object?)r.UPDATED_DATE ?? DBNull.Value,
                            r.FILE_NAME
                        );
                    }

                    // Bulk copy into staging table
                    using (var bcp = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, (SqlTransaction)tx))
                    {
                        bcp.DestinationTableName = "EI01_Stage";
                        foreach (DataColumn c in table.Columns)
                        {
                            bcp.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                        }
                        await bcp.WriteToServerAsync(table);
                    }

                    // Replace-by-date: delete and insert from stage
                    var dates = records.Select(r => r.NGAY_DL.Date).Distinct().ToList();
                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = (SqlTransaction)tx;
                    cmd.CommandText = @"
DELETE FROM EI01 WHERE CAST(NGAY_DL AS date) = @d;
INSERT INTO EI01 (NGAY_DL, MA_CN, MA_KH, TEN_KH, LOAI_KH, SDT_EMB, TRANG_THAI_EMB, NGAY_DK_EMB, SDT_OTT, TRANG_THAI_OTT, NGAY_DK_OTT, SDT_SMS, TRANG_THAI_SMS, NGAY_DK_SMS, SDT_SAV, TRANG_THAI_SAV, NGAY_DK_SAV, SDT_LN, TRANG_THAI_LN, NGAY_DK_LN, USER_EMB, USER_OTT, USER_SMS, USER_SAV, USER_LN, CREATED_DATE, UPDATED_DATE, FILE_NAME)
SELECT NGAY_DL, MA_CN, MA_KH, TEN_KH, LOAI_KH, SDT_EMB, TRANG_THAI_EMB, NGAY_DK_EMB, SDT_OTT, TRANG_THAI_OTT, NGAY_DK_OTT, SDT_SMS, TRANG_THAI_SMS, NGAY_DK_SMS, SDT_SAV, TRANG_THAI_SAV, NGAY_DK_SAV, SDT_LN, TRANG_THAI_LN, NGAY_DK_LN, USER_EMB, USER_OTT, USER_SMS, USER_SAV, USER_LN, CREATED_DATE, UPDATED_DATE, FILE_NAME
FROM EI01_Stage WHERE CAST(NGAY_DL AS date) = @d;";
                    var p = cmd.CreateParameter(); p.ParameterName = "@d"; p.SqlDbType = SqlDbType.Date; cmd.Parameters.Add(p);
                    foreach (var d in dates)
                    {
                        p.Value = d;
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Optional: clean staged rows for processed dates
                    using var clean = conn.CreateCommand();
                    clean.Transaction = (SqlTransaction)tx;
                    clean.CommandText = "DELETE FROM EI01_Stage WHERE CAST(NGAY_DL AS date) = @d";
                    var p2 = clean.CreateParameter(); p2.ParameterName = "@d"; p2.SqlDbType = SqlDbType.Date; clean.Parameters.Add(p2);
                    foreach (var d in dates) { p2.Value = d; await clean.ExecuteNonQueryAsync(); }

                    await tx.CommitAsync();
                    sw.Stop();
                    _metrics?.RecordBatch("EI01-BULK", records.Count, (int)sw.ElapsedMilliseconds);
                    return records.Count;
                }
                catch (SqlException ex) when (ex.Number == 208)
                {
                    await tx.RollbackAsync();
                    _logger.LogWarning("⚠️ [EI01] Staging table missing, fallback to EF: {Msg}", ex.Message);
                    return await BulkInsertGenericAsync(records, "EI01");
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
                finally
                {
                    if (shouldClose) await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [EI01] Bulk pipeline failed, fallback to EF");
                return await BulkInsertGenericAsync(records, "EI01");
            }
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
                var records = await ParseRR01CsvAsync(file, ngayDlDate);
                _logger.LogInformation("📊 [RR01] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Set NGAY_DL và audit fields cho tất cả records trước khi insert
                    foreach (var record in records)
                    {
                        record.NGAY_DL = ngayDlDate;
                        record.FILE_NAME = file.FileName;
                        record.CREATED_DATE = DateTime.UtcNow;
                    }

                    var insertedCount = records.Count >= BulkThreshold
                        ? await BulkCopyRR01ReplaceByDateAsync(records)
                        : await BulkInsertGenericAsync(records, "RR01");

                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [RR01] Import thành công {Count} records for date {Date}", insertedCount, ngayDlDate.ToString("yyyy-MM-dd"));
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

        #endregion
    }
}
