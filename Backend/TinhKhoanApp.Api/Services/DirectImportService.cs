using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Configuration;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
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
                "LN03" => await ImportLN03EnhancedAsync(file, statementDate),
                "GL01" => await ImportGL01Async(file, statementDate),
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
                    // Bulk insert DP01
                    var insertedCount = await BulkInsertGenericAsync(records, "DP01");
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
                    // Bulk insert DPDA
                    var insertedCount = await BulkInsertGenericAsync(records, "DPDA");
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

                // Parse EI01 CSV
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
                        }
                    }

                    // Bulk insert EI01
                    var insertedCount = await BulkInsertGenericAsync(records, "EI01");
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
                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Parse LN03 CSV với enhanced processing
                var records = await ParseLN03EnhancedAsync(file, statementDate);
                _logger.LogInformation("📊 [LN03_ENHANCED] Đã parse {Count} records", records.Count);

                if (records.Any())
                {
                    // Bulk insert LN03
                    var insertedCount = await BulkInsertGenericAsync(records, "LN03");
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
                    // Bulk insert GL01
                    var insertedCount = await BulkInsertGenericAsync(records, "GL01");
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
        private async Task<List<DP01Entity>> ParseDP01CsvAsync(IFormFile file)
        {
            var records = new List<DP01Entity>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            await foreach (var record in csv.GetRecordsAsync<DP01Entity>())
            {
                // Set audit fields
                record.CreatedAt = DateTime.UtcNow;
                record.UpdatedAt = DateTime.UtcNow;

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse DPDA CSV với 13 business columns
        /// </summary>
        private async Task<List<DPDAEntity>> ParseDPDACsvAsync(IFormFile file)
        {
            var records = new List<DPDAEntity>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            await foreach (var record in csv.GetRecordsAsync<DPDAEntity>())
            {
                // Set audit fields
                record.CreatedAt = DateTime.UtcNow;
                record.UpdatedAt = DateTime.UtcNow;

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse EI01 CSV với 24 business columns
        /// </summary>
        private async Task<List<EI01Entity>> ParseEI01CsvAsync(IFormFile file)
        {
            var records = new List<EI01Entity>();

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

            await foreach (var record in csv.GetRecordsAsync<EI01Entity>())
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
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            await foreach (var record in csv.GetRecordsAsync<LN03>())
            {
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

        #endregion
    }
}
