using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public DirectImportService(
            ApplicationDbContext context,
            ILogger<DirectImportService> logger,
            IOptions<DirectImportSettings> settings)
        {
            _context = context;
            _logger = logger;
            _settings = settings.Value;
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
                    // Ensure NGAY_DL for all records from filename
                    if (DateTime.TryParse(result.NgayDL, out var ngayDlDate))
                    {
                        foreach (var r in records)
                        {
                            r.NGAY_DL = ngayDlDate;
//                             r.FILE_NAME = file.FileName;
                        }
                    }
                    // Bulk insert DP01
                    var insertedCount = await BulkInsertGenericAsync(records, "DP01");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [DP01] Import thành công {Count} records", insertedCount);
                    // Log metadata for history
                    await LogImportMetadataAsync(result);
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
//                             r.FILE_NAME = file.FileName;
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
//                             record.FILE_NAME = file.FileName;
                            // record.CREATED_DATE = DateTime.UtcNow; // Không có CreatedAt trong GL02Entity mới
                            // record.UpdatedAt = DateTime.UtcNow; // Không có UpdatedAt trong GL02Entity mới
                        }
                    }

                    // Bulk insert LN01
                    var insertedCount = await BulkInsertGenericAsync(records, "LN01");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [LN01] Import thành công {Count} records", insertedCount);
                    await LogImportMetadataAsync(result);
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

                    // Upsert đơn giản: xóa dữ liệu cùng NGAY_DL trước khi insert (replace-by-date)
                    using var tx = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        if (DateTime.TryParse(result.NgayDL, out var ngayToDelete))
                        {
                            var affected = await _context.LN03.Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == ngayToDelete.Date).ExecuteDeleteAsync();
                            _logger.LogInformation("🧹 [LN03] Đã xoá {Count} bản ghi cũ cho ngày {Ngay}", affected, ngayToDelete.ToString("yyyy-MM-dd"));
                        }

                        var insertedCount = await BulkInsertGenericAsync(records, "LN03");
                        await tx.CommitAsync();
                        result.ProcessedRecords = insertedCount;
                        result.Success = true;
                        _logger.LogInformation("✅ [LN03_ENHANCED] Import thành công {Count} records", insertedCount);
                        await LogImportMetadataAsync(result);
                    }
                    catch
                    {
                        await tx.RollbackAsync();
                        throw;
                    }
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
                    // Bulk insert GL02
                    var insertedCount = await BulkInsertGenericAsync(records, "GL02");
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GL02] Import thành công {Count} records", insertedCount);
                    await LogImportMetadataAsync(result);
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

                // Parse GL01 CSV
                var records = await ParseGL01CsvAsync(file);
                _logger.LogInformation("📊 [GL01] Đã parse {Count} records từ CSV", records.Count);

                if (records.Any())
                {
                    // Heavy file bulk insert GL01 (BatchSize 10,000)
                    var insertedCount = await BulkInsertGL01HeavyAsync(records);
                    result.ProcessedRecords = insertedCount;
                    result.Success = true;
                    _logger.LogInformation("✅ [GL01] Heavy file import thành công {Count} records", insertedCount);
                    await LogImportMetadataAsync(result);
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
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null,
                IgnoreBlankLines = true
            });

            var fileName = Path.GetFileName(file.FileName);
            var ngayDlString = ExtractNgayDLFromFileName(fileName);
            var ngayDl = DateTime.TryParse(ngayDlString, out var date) ? date : DateTime.Today;

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

                    // Set audit fields
                    record.ImportDateTime = DateTime.UtcNow;
//                     // record.FILE_NAME = file.FileName; // TODO: Add FILE_NAME column to DP01 table

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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DataSource = fileName
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
        /// Parse GL02 CSV: NGAY_DL từ TRDATE; datetime2 dd/MM/yyyy; decimals #,###.00
        /// </summary>
        /// <summary>
        /// Parse GL02 CSV với 17 business columns, set NGAY_DL từ TRDATE
        /// </summary>
        private async Task<List<GL02Entity>> ParseGL02CsvAsync(IFormFile file)
        {
            var records = new List<GL02Entity>();

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
                var record = new GL02Entity();

                // Declare variables first to avoid scope issues
                DateTime trdate = DateTime.MinValue;
                DateTime crtdtm = DateTime.MinValue;
                decimal dramount = 0;
                decimal cramount = 0;

                // Map 17 business columns từ CSV
                if (csvRecord.TRDATE != null && DateTime.TryParse(csvRecord.TRDATE.ToString(), out trdate))
                {
                    // record.TRDATE = trdate; // TRDATE không còn trong GL02Entity mới
                    record.NGAY_DL = trdate.Date; // NGAY_DL derived from TRDATE
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
                // record.CREATED_DATE = DateTime.UtcNow; // Không có CreatedAt trong GL02Entity mới
                // record.UpdatedAt = DateTime.UtcNow; // Không có UpdatedAt trong GL02Entity mới
//                 // record.FILE_NAME = file.FileName; // Sử dụng FILE_NAME thay vì FileName
//                 record.FILE_NAME = file.FileName;

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
                // record.FileName = file.FileName; // Sử dụng FILE_NAME thay vì FileName
                record.FileName = file.FileName;

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
                    // FILE_NAME = file.FileName, // TODO: Add FILE_NAME column to database
                    CREATED_DATE = DateTime.UtcNow
                };

                records.Add(entity);
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
                _context.Set<T>().AddRange(records);
                var insertedCount = await _context.SaveChangesAsync();

                _logger.LogInformation("✅ [BULK_INSERT] Inserted {Count} records vào {Table}", insertedCount, tableName);
                return insertedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [BULK_INSERT] Error inserting vào {Table}: {Error}", tableName, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Heavy file bulk insert for GL01 - BatchSize 10,000 records
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

                    _context.Set<GL01>().AddRange(batch);
                    var batchInserted = await _context.SaveChangesAsync();
                    totalInserted += batchInserted;

                    // Clear the context to free up memory for heavy files
                    _context.ChangeTracker.Clear();

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
//                         record.FILE_NAME = file.FileName;
                        record.CREATED_DATE = DateTime.UtcNow;
                    }

                    // Replace-by-date upsert pattern: delete existing data for this date, then insert new
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Delete existing records cho ngày này
                        var existingRecords = _context.RR01.Where(r => r.NGAY_DL.Date == ngayDlDate.Date);
                        _context.RR01.RemoveRange(existingRecords);
                        await _context.SaveChangesAsync();

                        // Bulk insert new records
                        var insertedCount = await BulkInsertGenericAsync(records, "RR01");

                        await transaction.CommitAsync();

                        result.ProcessedRecords = insertedCount;
                        result.Success = true;
                        _logger.LogInformation("✅ [RR01] Import thành công {Count} records for date {Date}", insertedCount, ngayDlDate.ToString("yyyy-MM-dd"));
                        await LogImportMetadataAsync(result);
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
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
