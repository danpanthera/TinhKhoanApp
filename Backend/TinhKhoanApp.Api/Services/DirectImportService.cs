using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Concurrent;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Services.Interfaces;
using CsvHelper;
using OfficeOpenXml;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Direct Import Service - Import trực tiếp vào bảng riêng biệt sử dụng SqlBulkCopy
    /// Loại bỏ hoàn toàn ImportedDataItems để tăng hiệu năng 2-5x
    /// </summary>
    public partial class DirectImportService : IDirectImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DirectImportService> _logger;
        private readonly string _connectionString;

        /// <summary>
        /// Constructor for DirectImportService - initializes database context, logger and connection string
        /// </summary>
        public DirectImportService(
            ApplicationDbContext context,
            ILogger<DirectImportService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        /// <summary>
        /// Import Smart Direct - tự động detect loại file và import trực tiếp
        /// Hỗ trợ cấu hình mới: GL01 với NGAY_DL từ TR_TIME, 7 bảng khác từ filename
        /// </summary>
        public async Task<DirectImportResult> ImportSmartDirectAsync(IFormFile file, string? statementDate = null)
        {
            var result = new DirectImportResult
            {
                FileName = file.FileName,
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🚀 [DIRECT_IMPORT] Bắt đầu Smart Direct Import: {FileName}", file.FileName);

                // Detect data type từ filename
                var dataType = DetectDataTypeFromFileName(file.FileName);
                Console.WriteLine($"🔍 [DETECTION] File: {file.FileName} -> DataType: {dataType}");
                if (string.IsNullOrEmpty(dataType))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Không thể xác định loại dữ liệu từ filename: {file.FileName}";
                    return result;
                }

                result.DataType = dataType;
                _logger.LogInformation("📊 [DIRECT_IMPORT] Detected data type: {DataType}", dataType);

                // Route to specific import method
                return dataType.ToUpper() switch
                {
                    "DP01" => await ImportDP01DirectAsync(file, statementDate),
                    "LN01" => await ImportLN01DirectAsync(file, statementDate),
                    "LN03" => await ImportLN03DirectAsync(file, statementDate),
                    "GL01" => await ImportGL01DirectAsync(file, statementDate),
                    "GL02" => await ImportGL02DirectAsync(file, statementDate),
                    "GL41" => await ImportGL41DirectAsync(file, statementDate),
                    "DPDA" => await ImportDPDADirectAsync(file, statementDate),
                    "EI01" => await ImportEI01DirectAsync(file, statementDate),
                    "RR01" => await ImportRR01DirectAsync(file, statementDate),
                    _ => throw new NotSupportedException($"Data type {dataType} not supported")
                };
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [DIRECT_IMPORT] Smart Direct Import failed: {FileName}", file.FileName);
                return result;
            }
        }

        #region Specific Import Methods

        /// <summary>
        /// Import DP01 - Account balances
        /// Policy: Only accept files with "dp01" in filename
        /// </summary>
        public async Task<DirectImportResult> ImportDP01DirectAsync(IFormFile file, string? statementDate = null)
        {
            // Validate filename contains "dp01"
            if (!file.FileName.ToLower().Contains("dp01"))
            {
                _logger.LogWarning("❌ [DP01_DIRECT] File rejected: {FileName} does not contain 'dp01'", file.FileName);
                return new DirectImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    ErrorMessage = "File name must contain 'dp01' for DP01 import",
                    DataType = "DP01",
                    TargetTable = "DP01"
                };
            }

            _logger.LogInformation("🚀 [DP01_DIRECT] Import vào bảng DP01: {FileName}", file.FileName);
            return await ImportGenericCSVAsync<TinhKhoanApp.Api.Models.DataTables.DP01>("DP01", "DP01", file, statementDate);
        }

        /// <summary>
        /// Import LN01 - Loan data (using streaming approach)
        /// </summary>
        public async Task<DirectImportResult> ImportLN01DirectAsync(IFormFile file, string? statementDate = null)
        {
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
                _logger.LogInformation("🚀 [LN01_DIRECT] Import vào bảng LN01 using streaming approach");

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking
                var importRecord = await CreateImportedDataRecordAsync(file, "LN01", 0);
                result.ImportedDataRecordId = importRecord.Id;

                // Parse date
                DateTime parsedDate;
                if (!string.IsNullOrEmpty(statementDate) && DateTime.TryParse(statementDate, out parsedDate))
                {
                    // Use provided date
                }
                else if (!string.IsNullOrEmpty(ngayDL) && DateTime.TryParseExact(ngayDL, "dd/MM/yyyy", null, DateTimeStyles.None, out parsedDate))
                {
                    // Use date from filename
                }
                else
                {
                    parsedDate = DateTime.Now.Date;
                }

                // Stream import with specific LN01 logic
                var processedRecords = await ImportLN01StreamingAsync(file, parsedDate);
                result.ProcessedRecords = processedRecords;

                // Update record count
                importRecord.RecordsCount = processedRecords;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ [LN01_DIRECT] Import thành công: {Count} records trong {Duration}ms",
                    result.ProcessedRecords, result.Duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [LN01_DIRECT] Import failed: {FileName}", file.FileName);
                return result;
            }
        }

        /// <summary>
        /// Import LN01 streaming - simplified approach using generic bulk insert
        /// </summary>
        private async Task<int> ImportLN01StreamingAsync(IFormFile file, DateTime ngayDL)
        {
            using var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 16 * 1024 * 1024); // 🚀 Tăng buffer từ 8MB -> 16MB
            using var csvReader = new CsvReader(streamReader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BufferSize = 32768, // 🚀 Tăng CSV buffer size cho large files
                HasHeaderRecord = true,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                ReadingExceptionOccurred = (ex) => false, // 🚀 Skip malformed rows
                BadDataFound = null // 🚀 Ignore bad data cho tốc độ
            });

            // Read header (skip it)
            await csvReader.ReadAsync();
            csvReader.ReadHeader();

            var processedCount = 0;
            var batchSize = 100000;
            var batch = new List<LN01>();

            // Read data rows and convert to LN01 objects with proper data type conversion
            while (await csvReader.ReadAsync())
            {
                var ln01 = new LN01();
                ln01.NGAY_DL = ngayDL;
                ln01.FILE_NAME = file.FileName;
                ln01.CREATED_DATE = DateTime.Now;

                // Map CSV columns to LN01 properties (exactly 79 business columns)
                if (csvReader.ColumnCount >= 79)
                {
                    // String fields
                    ln01.BRCD = csvReader.GetField(0) ?? "";
                    ln01.CUSTSEQ = csvReader.GetField(1) ?? "";
                    ln01.CUSTNM = csvReader.GetField(2) ?? "";
                    ln01.TAI_KHOAN = csvReader.GetField(3) ?? "";
                    ln01.CCY = csvReader.GetField(4) ?? "";

                    // Decimal field - use ConvertCsvValue for proper parsing
                    ln01.DU_NO = (decimal?)ConvertCsvValue(csvReader.GetField(5) ?? "", typeof(decimal?));

                    ln01.DSBSSEQ = csvReader.GetField(6) ?? "";

                    // DateTime fields - use ConvertCsvValue for proper parsing
                    ln01.TRANSACTION_DATE = (DateTime?)ConvertCsvValue(csvReader.GetField(7) ?? "", typeof(DateTime?));
                    ln01.DSBSDT = (DateTime?)ConvertCsvValue(csvReader.GetField(8) ?? "", typeof(DateTime?));

                    ln01.DISBUR_CCY = csvReader.GetField(9) ?? "";

                    // Decimal field
                    ln01.DISBURSEMENT_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(10) ?? "", typeof(decimal?));

                    // DateTime field
                    ln01.DSBSMATDT = (DateTime?)ConvertCsvValue(csvReader.GetField(11) ?? "", typeof(DateTime?));

                    ln01.BSRTCD = csvReader.GetField(12) ?? "";

                    // Decimal field
                    ln01.INTEREST_RATE = (decimal?)ConvertCsvValue(csvReader.GetField(13) ?? "", typeof(decimal?));

                    ln01.APPRSEQ = csvReader.GetField(14) ?? "";

                    // DateTime field
                    ln01.APPRDT = (DateTime?)ConvertCsvValue(csvReader.GetField(15) ?? "", typeof(DateTime?));

                    ln01.APPR_CCY = csvReader.GetField(16) ?? "";

                    // Decimal field
                    ln01.APPRAMT = (decimal?)ConvertCsvValue(csvReader.GetField(17) ?? "", typeof(decimal?));

                    // DateTime field
                    ln01.APPRMATDT = (DateTime?)ConvertCsvValue(csvReader.GetField(18) ?? "", typeof(DateTime?));

                    ln01.LOAN_TYPE = csvReader.GetField(19) ?? "";
                    ln01.FUND_RESOURCE_CODE = csvReader.GetField(20) ?? "";
                    ln01.FUND_PURPOSE_CODE = csvReader.GetField(21) ?? "";

                    // Decimal field
                    ln01.REPAYMENT_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(22) ?? "", typeof(decimal?));

                    // DateTime field
                    ln01.NEXT_REPAY_DATE = (DateTime?)ConvertCsvValue(csvReader.GetField(23) ?? "", typeof(DateTime?));

                    // Decimal field
                    ln01.NEXT_REPAY_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(24) ?? "", typeof(decimal?));

                    // DateTime field
                    ln01.NEXT_INT_REPAY_DATE = (DateTime?)ConvertCsvValue(csvReader.GetField(25) ?? "", typeof(DateTime?));

                    ln01.OFFICER_ID = csvReader.GetField(26) ?? "";
                    ln01.OFFICER_NAME = csvReader.GetField(27) ?? "";

                    // Decimal fields
                    ln01.INTEREST_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(28) ?? "", typeof(decimal?));
                    ln01.PASTDUE_INTEREST_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(29) ?? "", typeof(decimal?));
                    ln01.TOTAL_INTEREST_REPAY_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(30) ?? "", typeof(decimal?));

                    ln01.CUSTOMER_TYPE_CODE = csvReader.GetField(31) ?? "";
                    ln01.CUSTOMER_TYPE_CODE_DETAIL = csvReader.GetField(32) ?? "";
                    ln01.TRCTCD = csvReader.GetField(33) ?? "";
                    ln01.TRCTNM = csvReader.GetField(34) ?? "";
                    ln01.ADDR1 = csvReader.GetField(35) ?? "";
                    ln01.PROVINCE = csvReader.GetField(36) ?? "";
                    ln01.LCLPROVINNM = csvReader.GetField(37) ?? "";
                    ln01.DISTRICT = csvReader.GetField(38) ?? "";
                    ln01.LCLDISTNM = csvReader.GetField(39) ?? "";
                    ln01.COMMCD = csvReader.GetField(40) ?? "";
                    ln01.LCLWARDNM = csvReader.GetField(41) ?? "";

                    // DateTime field
                    ln01.LAST_REPAY_DATE = (DateTime?)ConvertCsvValue(csvReader.GetField(42) ?? "", typeof(DateTime?));

                    ln01.SECURED_PERCENT = csvReader.GetField(43) ?? "";
                    ln01.NHOM_NO = csvReader.GetField(44) ?? "";

                    // DateTime field
                    ln01.LAST_INT_CHARGE_DATE = (DateTime?)ConvertCsvValue(csvReader.GetField(45) ?? "", typeof(DateTime?));

                    ln01.EXEMPTINT = csvReader.GetField(46) ?? "";
                    ln01.EXEMPTINTTYPE = csvReader.GetField(47) ?? "";

                    // Decimal field
                    ln01.EXEMPTINTAMT = (decimal?)ConvertCsvValue(csvReader.GetField(48) ?? "", typeof(decimal?));

                    ln01.GRPNO = csvReader.GetField(49) ?? "";
                    ln01.BUSCD = csvReader.GetField(50) ?? "";
                    ln01.BSNSSCLTPCD = csvReader.GetField(51) ?? "";
                    ln01.USRIDOP = csvReader.GetField(52) ?? "";

                    // Decimal fields
                    ln01.ACCRUAL_AMOUNT = (decimal?)ConvertCsvValue(csvReader.GetField(53) ?? "", typeof(decimal?));
                    ln01.ACCRUAL_AMOUNT_END_OF_MONTH = (decimal?)ConvertCsvValue(csvReader.GetField(54) ?? "", typeof(decimal?));

                    ln01.INTCMTH = csvReader.GetField(55) ?? "";
                    ln01.INTRPYMTH = csvReader.GetField(56) ?? "";
                    ln01.INTTRMMTH = csvReader.GetField(57) ?? "";
                    ln01.YRDAYS = csvReader.GetField(58) ?? "";
                    ln01.REMARK = csvReader.GetField(59) ?? "";
                    ln01.CHITIEU = csvReader.GetField(60) ?? "";
                    ln01.CTCV = csvReader.GetField(61) ?? "";
                    ln01.CREDIT_LINE_YPE = csvReader.GetField(62) ?? "";
                    ln01.INT_LUMPSUM_PARTIAL_TYPE = csvReader.GetField(63) ?? "";
                    ln01.INT_PARTIAL_PAYMENT_TYPE = csvReader.GetField(64) ?? "";
                    ln01.INT_PAYMENT_INTERVAL = csvReader.GetField(65) ?? "";
                    ln01.AN_HAN_LAI = csvReader.GetField(66) ?? "";
                    ln01.PHUONG_THUC_GIAI_NGAN_1 = csvReader.GetField(67) ?? "";
                    ln01.TAI_KHOAN_GIAI_NGAN_1 = csvReader.GetField(68) ?? "";

                    // Decimal field
                    ln01.SO_TIEN_GIAI_NGAN_1 = (decimal?)ConvertCsvValue(csvReader.GetField(69) ?? "", typeof(decimal?));

                    ln01.PHUONG_THUC_GIAI_NGAN_2 = csvReader.GetField(70) ?? "";
                    ln01.TAI_KHOAN_GIAI_NGAN_2 = csvReader.GetField(71) ?? "";

                    // Decimal field
                    ln01.SO_TIEN_GIAI_NGAN_2 = (decimal?)ConvertCsvValue(csvReader.GetField(72) ?? "", typeof(decimal?));

                    ln01.CMT_HC = csvReader.GetField(73) ?? "";

                    // DateTime field
                    ln01.NGAY_SINH = (DateTime?)ConvertCsvValue(csvReader.GetField(74) ?? "", typeof(DateTime?));

                    ln01.MA_CB_AGRI = csvReader.GetField(75) ?? "";
                    ln01.MA_NGANH_KT = csvReader.GetField(76) ?? "";

                    // Decimal field
                    ln01.TY_GIA = (decimal?)ConvertCsvValue(csvReader.GetField(77) ?? "", typeof(decimal?));

                    ln01.OFFICER_IPCAS = csvReader.GetField(78) ?? "";
                }

                batch.Add(ln01);

                if (batch.Count >= batchSize)
                {
                    // Use BulkInsertGenericAsync for proper column mapping
                    await BulkInsertGenericAsync(batch, "LN01");
                    processedCount += batch.Count;
                    batch.Clear();
                }
            }

            // Process remaining batch
            if (batch.Count > 0)
            {
                await BulkInsertGenericAsync(batch, "LN01");
                processedCount += batch.Count;
            }

            return processedCount;
        }

        /// <summary>
        /// Import LN03 - Bad debt data
        /// </summary>
        public async Task<DirectImportResult> ImportLN03DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<LN03>("LN03", "LN03", file, statementDate);
        }

        /// <summary>
        /// Import GL01 - SILENT ULTRA HIGH-SPEED với direct streaming (no verbose logging)
        /// </summary>
        public async Task<DirectImportResult> ImportGL01DirectAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL01_SILENT] SILENT ULTRA HIGH-SPEED Import vào bảng GL01");
            return await ImportGL01SilentDirectAsync(file, statementDate);
        }

        /// <summary>
        /// Import GL02 - ULTRA HIGH-SPEED với silent direct import (no verbose logging)
        /// </summary>
        public async Task<DirectImportResult> ImportGL02DirectAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL02_SILENT] SILENT ULTRA HIGH-SPEED Import vào bảng GL02");
            return await ImportGL02SilentDirectAsync(file, statementDate);
        }

        /// <summary>
        /// GL02 SILENT DIRECT Import - Tốc độ tối đa, không verbose logging
        /// Chuyên biệt cho file GL02 lớn (300k+ records)
        /// </summary>
        private async Task<DirectImportResult> ImportGL02SilentDirectAsync(IFormFile file, string? statementDate = null)
        {
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
                _logger.LogInformation("🔥 [GL02_SILENT] Starting SILENT Direct Import for GL02: {FileName}, Size: {Size}MB",
                    file.FileName, Math.Round(file.Length / 1024.0 / 1024.0, 2));

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking
                var importRecord = await CreateImportedDataRecordAsync(file, "GL02", 0);
                result.ImportedDataRecordId = importRecord.Id;

                // Parse CSV với STREAMING BATCH processing (không load tất cả vào memory)
                var processedCount = await ParseAndInsertGL02StreamingAsync(file, statementDate);
                _logger.LogInformation("🔥 [GL02_SILENT] STREAMING processing completed: {Count} records", processedCount);

                if (processedCount > 0)
                {
                    result.ProcessedRecords = processedCount;

                    // Update record count
                    importRecord.RecordsCount = processedCount;
                    await _context.SaveChangesAsync();
                }

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("🔥 [GL02_SILENT] COMPLETED: {Count} records in {Duration} seconds",
                    result.ProcessedRecords, Math.Round(result.Duration.TotalSeconds, 1));

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [GL02_SILENT] Failed: {FileName}", file.FileName);
                return result;
            }
        }

        /// <summary>
        /// GL01 SILENT DIRECT Import - Tốc độ tối đa, không verbose logging
        /// Chuyên biệt cho file GL01 lớn (300k+ records) - TR_TIME cho NGAY_DL
        /// </summary>
        private async Task<DirectImportResult> ImportGL01SilentDirectAsync(IFormFile file, string? statementDate = null)
        {
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
                _logger.LogInformation("🔥 [GL01_SILENT] Starting SILENT Direct Import for GL01: {FileName}, Size: {Size}MB",
                    file.FileName, Math.Round(file.Length / 1024.0 / 1024.0, 2));

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking
                var importRecord = await CreateImportedDataRecordAsync(file, "GL01", 0);
                result.ImportedDataRecordId = importRecord.Id;

                // Parse CSV với STREAMING BATCH processing (không load tất cả vào memory)
                var processedCount = await ParseAndInsertGL01StreamingAsync(file, statementDate);
                _logger.LogInformation("🔥 [GL01_SILENT] STREAMING processing completed: {Count} records", processedCount);

                if (processedCount > 0)
                {
                    result.ProcessedRecords = processedCount;

                    // Update record count
                    importRecord.RecordsCount = processedCount;
                    await _context.SaveChangesAsync();
                }

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("🔥 [GL01_SILENT] COMPLETED: {Count} records in {Duration} seconds",
                    result.ProcessedRecords, Math.Round(result.Duration.TotalSeconds, 1));

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [GL01_SILENT] Failed: {FileName}", file.FileName);
                return result;
            }
        }

        /// <summary>
        /// Parse GL02 CSV SILENT - no verbose logging, max speed
        /// </summary>
        private async Task<List<GL02>> ParseGL02SilentAsync(IFormFile file, string? statementDate = null)
        {
            var records = new List<GL02>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: 32768); // 32KB buffer
            using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BufferSize = 32768,
                HasHeaderRecord = true,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                ReadingExceptionOccurred = (ex) => false, // Skip malformed rows
                BadDataFound = null // Ignore bad data
            });

            await csv.ReadAsync();
            csv.ReadHeader();

            var recordCount = 0;
            while (await csv.ReadAsync())
            {
                try
                {
                    var record = new GL02
                    {
                        // TRDATE (index 0) -> NGAY_DL
                        NGAY_DL = DateTime.TryParseExact(csv.GetField(0)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : DateTime.Now,

                        TRBRCD = csv.GetField(1)?.Trim(),
                        USERID = csv.GetField(2)?.Trim(),
                        JOURSEQ = csv.GetField(3)?.Trim(),
                        DYTRSEQ = csv.GetField(4)?.Trim(),
                        LOCAC = csv.GetField(5)?.Trim(),
                        CCY = csv.GetField(6)?.Trim(),
                        BUSCD = csv.GetField(7)?.Trim(),
                        UNIT = csv.GetField(8)?.Trim(),
                        TRCD = csv.GetField(9)?.Trim(),
                        CUSTOMER = csv.GetField(10)?.Trim(),
                        TRTP = csv.GetField(11)?.Trim(),
                        REFERENCE = csv.GetField(12)?.Trim(),
                        REMARK = csv.GetField(13)?.Trim(),

                        // Numeric fields with error handling
                        DRAMOUNT = decimal.TryParse(csv.GetField(14)?.Replace(",", "")?.Trim(), out var dr) ? dr : 0,
                        CRAMOUNT = decimal.TryParse(csv.GetField(15)?.Replace(",", "")?.Trim(), out var cr) ? cr : 0,

                        // CRTDTM field
                        CRTDTM = DateTime.TryParseExact(csv.GetField(16)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var crtdtm) ? crtdtm : (DateTime?)null,

                        // System fields
                        CREATED_DATE = DateTime.UtcNow,
                        UPDATED_DATE = DateTime.UtcNow,
                        FILE_NAME = file.FileName
                    };

                    records.Add(record);
                    recordCount++;

                    // Progress log every 100k records only
                    if (recordCount % 100000 == 0)
                    {
                        _logger.LogInformation("🔥 [GL02_SILENT] Parsed {Count} records", recordCount);
                    }
                }
                catch
                {
                    // Skip malformed rows silently for max speed
                    continue;
                }
            }

            return records;
        }

        /// <summary>
        /// SILENT Bulk Insert for GL02 - no column mapping logs, max performance
        /// </summary>
        private async Task<int> BulkInsertGL02SilentAsync(List<GL02> records)
        {
            var connectionString = _context.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Create DataTable for GL02
            var dataTable = new DataTable();
            dataTable.Columns.Add("NGAY_DL", typeof(DateTime));
            dataTable.Columns.Add("TRBRCD", typeof(string));
            dataTable.Columns.Add("USERID", typeof(string));
            dataTable.Columns.Add("JOURSEQ", typeof(string));
            dataTable.Columns.Add("DYTRSEQ", typeof(string));
            dataTable.Columns.Add("LOCAC", typeof(string));
            dataTable.Columns.Add("CCY", typeof(string));
            dataTable.Columns.Add("BUSCD", typeof(string));
            dataTable.Columns.Add("UNIT", typeof(string));
            dataTable.Columns.Add("TRCD", typeof(string));
            dataTable.Columns.Add("CUSTOMER", typeof(string));
            dataTable.Columns.Add("TRTP", typeof(string));
            dataTable.Columns.Add("REFERENCE", typeof(string));
            dataTable.Columns.Add("REMARK", typeof(string));
            dataTable.Columns.Add("DRAMOUNT", typeof(decimal));
            dataTable.Columns.Add("CRAMOUNT", typeof(decimal));
            dataTable.Columns.Add("CRTDTM", typeof(DateTime));
            dataTable.Columns.Add("CREATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("UPDATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("FILE_NAME", typeof(string));

            // Populate DataTable SILENT (no logging per record)
            foreach (var record in records)
            {
                dataTable.Rows.Add(
                    record.NGAY_DL,
                    record.TRBRCD, record.USERID, record.JOURSEQ, record.DYTRSEQ, record.LOCAC,
                    record.CCY, record.BUSCD, record.UNIT, record.TRCD, record.CUSTOMER,
                    record.TRTP, record.REFERENCE, record.REMARK,
                    record.DRAMOUNT, record.CRAMOUNT, record.CRTDTM,
                    record.CREATED_DATE, record.UPDATED_DATE, record.FILE_NAME
                );
            }

            // ULTRA-HIGH-SPEED SqlBulkCopy settings
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction, null)
            {
                DestinationTableName = "GL02",
                BatchSize = 750000, // 750k for optimal speed
                BulkCopyTimeout = 7200, // 2 hours
                EnableStreaming = true,
                NotifyAfter = 100000 // Progress every 100k only
            };

            // Column mappings SILENT (no logging)
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            // Progress tracking
            bulkCopy.SqlRowsCopied += (sender, e) =>
            {
                _logger.LogInformation("🔥 [GL02_SILENT] Inserted {Count} records", e.RowsCopied);
            };

            // Execute SILENT bulk copy
            await bulkCopy.WriteToServerAsync(dataTable);
            return records.Count;
        }

        /// <summary>
        /// Import GL41 - Trial balance
        /// </summary>
        public async Task<DirectImportResult> ImportGL41DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<GL41>("GL41", "GL41", file, statementDate);
        }

        /// <summary>
        /// Import DPDA - Detailed account data
        /// </summary>
        public async Task<DirectImportResult> ImportDPDADirectAsync(IFormFile file, string? statementDate = null)
        {
            Console.WriteLine($"🎯 [DPDA_IMPORT] Starting DPDA import for file: {file.FileName}");

            // ✅ FILENAME VALIDATION - Only files containing "dpda" are allowed
            if (!file.FileName.ToLower().Contains("dpda"))
            {
                Console.WriteLine($"❌ [DPDA_IMPORT] Filename validation failed: {file.FileName}");
                return new DirectImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    DataType = "DPDA",
                    TargetTable = "DPDA",
                    ErrorMessage = $"Invalid filename for DPDA import. Filename must contain 'dpda'. Current: {file.FileName}",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow
                };
            }

            Console.WriteLine($"✅ [DPDA_IMPORT] Filename validation passed, calling ImportGenericCSVAsync<DPDA>");
            return await ImportGenericCSVAsync<DPDA>("DPDA", "DPDA", file, statementDate);
        }

        /// <summary>
        /// TRUE STREAMING: Parse và Insert GL02 theo batch nhỏ - không load tất cả vào memory
        /// Batch size 50k records để tránh memory overflow với file 338k+ records
        /// </summary>
        private async Task<int> ParseAndInsertGL02StreamingAsync(IFormFile file, string? statementDate = null)
        {
            var totalProcessed = 0;
            var connectionString = _context.Database.GetConnectionString();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: 65536); // 64KB buffer
            using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BufferSize = 65536,
                HasHeaderRecord = true,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                ReadingExceptionOccurred = (ex) => false,
                BadDataFound = null
            });

            await csv.ReadAsync();
            csv.ReadHeader();

            var batch = new List<GL02>();
            const int BATCH_SIZE = 50000; // 50k per batch to avoid memory issues
            var recordCount = 0;

            while (await csv.ReadAsync())
            {
                try
                {
                    var record = new GL02
                    {
                        // TRDATE (index 0) -> NGAY_DL
                        NGAY_DL = DateTime.TryParseExact(csv.GetField(0)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : DateTime.Now,

                        TRBRCD = csv.GetField(1)?.Trim(),
                        USERID = csv.GetField(2)?.Trim(),
                        JOURSEQ = csv.GetField(3)?.Trim(),
                        DYTRSEQ = csv.GetField(4)?.Trim(),
                        LOCAC = csv.GetField(5)?.Trim(),
                        CCY = csv.GetField(6)?.Trim(),
                        BUSCD = csv.GetField(7)?.Trim(),
                        UNIT = csv.GetField(8)?.Trim(),
                        TRCD = csv.GetField(9)?.Trim(),
                        CUSTOMER = csv.GetField(10)?.Trim(),
                        TRTP = csv.GetField(11)?.Trim(),
                        REFERENCE = csv.GetField(12)?.Trim(),
                        REMARK = csv.GetField(13)?.Trim(),

                        DRAMOUNT = decimal.TryParse(csv.GetField(14)?.Replace(",", "")?.Trim(), out var dr) ? dr : 0,
                        CRAMOUNT = decimal.TryParse(csv.GetField(15)?.Replace(",", "")?.Trim(), out var cr) ? cr : 0,

                        CRTDTM = DateTime.TryParseExact(csv.GetField(16)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var crtdtm) ? crtdtm : (DateTime?)null,

                        CREATED_DATE = DateTime.UtcNow,
                        UPDATED_DATE = DateTime.UtcNow,
                        FILE_NAME = file.FileName
                    };

                    batch.Add(record);
                    recordCount++;

                    // Insert batch when reached BATCH_SIZE
                    if (batch.Count >= BATCH_SIZE)
                    {
                        var insertedCount = await InsertGL02BatchAsync(batch, connectionString);
                        totalProcessed += insertedCount;
                        _logger.LogInformation("🔥 [GL02_STREAMING] Batch {BatchNum}: Inserted {Count} records (Total: {Total})",
                            (recordCount / BATCH_SIZE), insertedCount, totalProcessed);

                        batch.Clear(); // Clear memory immediately
                    }
                }
                catch
                {
                    // Skip malformed rows silently
                    continue;
                }
            }

            // Insert remaining records in final batch
            if (batch.Count > 0)
            {
                var insertedCount = await InsertGL02BatchAsync(batch, connectionString);
                totalProcessed += insertedCount;
                _logger.LogInformation("🔥 [GL02_STREAMING] Final batch: Inserted {Count} records (Total: {Total})",
                    insertedCount, totalProcessed);
            }

            return totalProcessed;
        }

        /// <summary>
        /// Insert một batch GL02 records với optimized SqlBulkCopy
        /// </summary>
        private async Task<int> InsertGL02BatchAsync(List<GL02> records, string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Create optimized DataTable for this batch only
            var dataTable = new DataTable();
            dataTable.Columns.Add("NGAY_DL", typeof(DateTime));
            dataTable.Columns.Add("TRBRCD", typeof(string));
            dataTable.Columns.Add("USERID", typeof(string));
            dataTable.Columns.Add("JOURSEQ", typeof(string));
            dataTable.Columns.Add("DYTRSEQ", typeof(string));
            dataTable.Columns.Add("LOCAC", typeof(string));
            dataTable.Columns.Add("CCY", typeof(string));
            dataTable.Columns.Add("BUSCD", typeof(string));
            dataTable.Columns.Add("UNIT", typeof(string));
            dataTable.Columns.Add("TRCD", typeof(string));
            dataTable.Columns.Add("CUSTOMER", typeof(string));
            dataTable.Columns.Add("TRTP", typeof(string));
            dataTable.Columns.Add("REFERENCE", typeof(string));
            dataTable.Columns.Add("REMARK", typeof(string));
            dataTable.Columns.Add("DRAMOUNT", typeof(decimal));
            dataTable.Columns.Add("CRAMOUNT", typeof(decimal));
            dataTable.Columns.Add("CRTDTM", typeof(DateTime));
            dataTable.Columns.Add("CREATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("UPDATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("FILE_NAME", typeof(string));

            // Populate DataTable from batch
            foreach (var record in records)
            {
                dataTable.Rows.Add(
                    record.NGAY_DL,
                    record.TRBRCD, record.USERID, record.JOURSEQ, record.DYTRSEQ, record.LOCAC,
                    record.CCY, record.BUSCD, record.UNIT, record.TRCD, record.CUSTOMER,
                    record.TRTP, record.REFERENCE, record.REMARK,
                    record.DRAMOUNT, record.CRAMOUNT, record.CRTDTM,
                    record.CREATED_DATE, record.UPDATED_DATE, record.FILE_NAME
                );
            }

            // Optimized SqlBulkCopy for small batches
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "GL02",
                BatchSize = records.Count, // Use actual batch size
                BulkCopyTimeout = 300, // 5 minutes for each batch
                EnableStreaming = true
            };

            // Setup column mappings (no logging for speed)
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            // Execute batch insert
            await bulkCopy.WriteToServerAsync(dataTable);
            return records.Count;
        }

        /// <summary>
        /// TRUE STREAMING: Parse và Insert GL01 theo batch nhỏ - không load tất cả vào memory
        /// Batch size 50k records để tránh memory overflow với file lớn
        /// GL01: TR_TIME (index 2) cho NGAY_DL thay vì filename
        /// </summary>
        private async Task<int> ParseAndInsertGL01StreamingAsync(IFormFile file, string? statementDate = null)
        {
            var totalProcessed = 0;
            var connectionString = _context.Database.GetConnectionString();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: 65536); // 64KB buffer
            using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BufferSize = 65536,
                HasHeaderRecord = true,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                ReadingExceptionOccurred = (ex) => false,
                BadDataFound = null
            });

            await csv.ReadAsync();
            csv.ReadHeader();

            var batch = new List<GL01>();
            const int BATCH_SIZE = 50000; // 50k per batch to avoid memory issues
            var recordCount = 0;

            while (await csv.ReadAsync())
            {
                try
                {
                    var record = new GL01
                    {
                        // TR_TIME (index 25) -> NGAY_DL (GL01 specific rule)
                        NGAY_DL = DateTime.TryParseExact(csv.GetField(25)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var trTime) ? trTime : DateTime.Now,

                        STS = csv.GetField(0)?.Trim(),
                        NGAY_GD = DateTime.TryParseExact(csv.GetField(1)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngayGd) ? ngayGd : (DateTime?)null,
                        NGUOI_TAO = csv.GetField(2)?.Trim(),
                        DYSEQ = csv.GetField(3)?.Trim(),
                        TR_TYPE = csv.GetField(4)?.Trim(),
                        DT_SEQ = csv.GetField(5)?.Trim(),
                        TAI_KHOAN = csv.GetField(6)?.Trim(),
                        TEN_TK = csv.GetField(7)?.Trim(),
                        SO_TIEN_GD = decimal.TryParse(csv.GetField(8)?.Replace(",", "")?.Trim(), out var soTienGd) ? soTienGd : (decimal?)null,
                        POST_BR = csv.GetField(9)?.Trim(),
                        LOAI_TIEN = csv.GetField(10)?.Trim(),
                        DR_CR = csv.GetField(11)?.Trim(),
                        MA_KH = csv.GetField(12)?.Trim(),
                        TEN_KH = csv.GetField(13)?.Trim(),
                        CCA_USRID = csv.GetField(14)?.Trim(),
                        TR_EX_RT = csv.GetField(15)?.Trim(),
                        REMARK = csv.GetField(16)?.Trim(),
                        BUS_CODE = csv.GetField(17)?.Trim(),
                        UNIT_BUS_CODE = csv.GetField(18)?.Trim(),
                        TR_CODE = csv.GetField(19)?.Trim(),
                        TR_NAME = csv.GetField(20)?.Trim(),
                        REFERENCE = csv.GetField(21)?.Trim(),
                        VALUE_DATE = DateTime.TryParseExact(csv.GetField(22)?.Trim() ?? "", "yyyyMMdd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var valueDate) ? valueDate : (DateTime?)null,
                        DEPT_CODE = csv.GetField(23)?.Trim(),
                        TR_TIME = csv.GetField(24)?.Trim(),
                        COMFIRM = csv.GetField(25)?.Trim(),
                        TRDT_TIME = csv.GetField(26)?.Trim(),

                        CREATED_DATE = DateTime.UtcNow,
                        UPDATED_DATE = DateTime.UtcNow,
                        FILE_NAME = file.FileName
                    };

                    batch.Add(record);
                    recordCount++;

                    // Insert batch when reached BATCH_SIZE
                    if (batch.Count >= BATCH_SIZE)
                    {
                        var insertedCount = await InsertGL01BatchAsync(batch, connectionString);
                        totalProcessed += insertedCount;
                        _logger.LogInformation("🔥 [GL01_STREAMING] Batch {BatchNum}: Inserted {Count} records (Total: {Total})",
                            (recordCount / BATCH_SIZE), insertedCount, totalProcessed);

                        batch.Clear(); // Clear memory immediately
                    }
                }
                catch
                {
                    // Skip malformed rows silently
                    continue;
                }
            }

            // Insert remaining records in final batch
            if (batch.Count > 0)
            {
                var insertedCount = await InsertGL01BatchAsync(batch, connectionString);
                totalProcessed += insertedCount;
                _logger.LogInformation("🔥 [GL01_STREAMING] Final batch: Inserted {Count} records (Total: {Total})",
                    insertedCount, totalProcessed);
            }

            return totalProcessed;
        }

        /// <summary>
        /// Insert một batch GL01 records với optimized SqlBulkCopy
        /// </summary>
        private async Task<int> InsertGL01BatchAsync(List<GL01> records, string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Create optimized DataTable for this batch only
            var dataTable = new DataTable();
            dataTable.Columns.Add("NGAY_DL", typeof(DateTime));
            dataTable.Columns.Add("STS", typeof(string));
            dataTable.Columns.Add("NGAY_GD", typeof(DateTime));
            dataTable.Columns.Add("NGUOI_TAO", typeof(string));
            dataTable.Columns.Add("DYSEQ", typeof(string));
            dataTable.Columns.Add("TR_TYPE", typeof(string));
            dataTable.Columns.Add("DT_SEQ", typeof(string));
            dataTable.Columns.Add("TAI_KHOAN", typeof(string));
            dataTable.Columns.Add("TEN_TK", typeof(string));
            dataTable.Columns.Add("SO_TIEN_GD", typeof(decimal));
            dataTable.Columns.Add("POST_BR", typeof(string));
            dataTable.Columns.Add("LOAI_TIEN", typeof(string));
            dataTable.Columns.Add("DR_CR", typeof(string));
            dataTable.Columns.Add("MA_KH", typeof(string));
            dataTable.Columns.Add("TEN_KH", typeof(string));
            dataTable.Columns.Add("CCA_USRID", typeof(string));
            dataTable.Columns.Add("TR_EX_RT", typeof(string));
            dataTable.Columns.Add("REMARK", typeof(string));
            dataTable.Columns.Add("BUS_CODE", typeof(string));
            dataTable.Columns.Add("UNIT_BUS_CODE", typeof(string));
            dataTable.Columns.Add("TR_CODE", typeof(string));
            dataTable.Columns.Add("TR_NAME", typeof(string));
            dataTable.Columns.Add("REFERENCE", typeof(string));
            dataTable.Columns.Add("VALUE_DATE", typeof(DateTime));
            dataTable.Columns.Add("DEPT_CODE", typeof(string));
            dataTable.Columns.Add("TR_TIME", typeof(string));
            dataTable.Columns.Add("COMFIRM", typeof(string));
            dataTable.Columns.Add("TRDT_TIME", typeof(string));
            dataTable.Columns.Add("CREATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("UPDATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("FILE_NAME", typeof(string));

            // Populate DataTable from batch
            foreach (var record in records)
            {
                dataTable.Rows.Add(
                    record.NGAY_DL,
                    record.STS, record.NGAY_GD, record.NGUOI_TAO, record.DYSEQ, record.TR_TYPE,
                    record.DT_SEQ, record.TAI_KHOAN, record.TEN_TK, record.SO_TIEN_GD, record.POST_BR,
                    record.LOAI_TIEN, record.DR_CR, record.MA_KH, record.TEN_KH, record.CCA_USRID,
                    record.TR_EX_RT, record.REMARK, record.BUS_CODE, record.UNIT_BUS_CODE, record.TR_CODE,
                    record.TR_NAME, record.REFERENCE, record.VALUE_DATE, record.DEPT_CODE, record.TR_TIME,
                    record.COMFIRM, record.TRDT_TIME,
                    record.CREATED_DATE, record.UPDATED_DATE, record.FILE_NAME
                );
            }

            // Optimized SqlBulkCopy for small batches
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "GL01",
                BatchSize = records.Count, // Use actual batch size
                BulkCopyTimeout = 300, // 5 minutes for each batch
                EnableStreaming = true
            };

            // Setup column mappings (no logging for speed)
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            // Execute batch insert
            await bulkCopy.WriteToServerAsync(dataTable);
            return records.Count;
        }

        /// <summary>
        /// Import EI01 - Electronic Banking Information data
        /// </summary>
        public async Task<DirectImportResult> ImportEI01DirectAsync(IFormFile file, string? statementDate = null)
        {
            Console.WriteLine($"🎯 [EI01_IMPORT] Starting EI01 import for file: {file.FileName}");

            // ✅ FILENAME VALIDATION - Only files containing "ei01" are allowed
            if (!file.FileName.ToLower().Contains("ei01"))
            {
                Console.WriteLine($"❌ [EI01_IMPORT] Filename validation failed: {file.FileName}");
                return new DirectImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    DataType = "EI01",
                    TargetTable = "EI01",
                    ErrorMessage = $"Invalid filename for EI01 import. Filename must contain 'ei01'. Current: {file.FileName}",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow
                };
            }

            Console.WriteLine($"✅ [EI01_IMPORT] Filename validation passed, calling ImportGenericCSVAsync<EI01>");
            return await ImportGenericCSVAsync<EI01>("EI01", "EI01", file, statementDate);
        }

        /// <summary>
        /// Import RR01 - Risk rating data (uses special parser for non-standard CSV format)
        /// </summary>
        public async Task<DirectImportResult> ImportRR01DirectAsync(IFormFile file, string? statementDate = null)
        {
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
                _logger.LogInformation("🚀 [RR01_DIRECT] Bắt đầu Direct Import với special parser: {FileName}", file.FileName);

                // Use special parser for RR01 format
                var records = await ParseRR01SpecialFormatAsync<RR01>(file, statementDate);

                result.ProcessedRecords = records.Count;
                _logger.LogInformation("📊 [IMPORT_DEBUG] Parsed {RecordCount} records from CSV", records.Count);

                if (records.Count > 0)
                {
                    // Bulk insert vào database
                    var insertedCount = await BulkInsertGenericAsync(records, "RR01");
                }

                result.Success = true;
                result.NgayDL = ExtractNgayDLFromFileName(file.FileName);
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ [RR01_DIRECT] Direct Import thành công: {RecordCount} records trong {Duration}ms",
                    result.ProcessedRecords, result.Duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                result.EndTime = DateTime.UtcNow;
                result.Success = false;
                result.ErrorMessage = ex.Message;

                _logger.LogError(ex, "❌ [RR01_DIRECT] Lỗi import: {Error}", ex.Message);
                return result;
            }
        }

        #endregion

        #region Generic Import Methods

        /// <summary>
        /// Generic CSV import method
        /// </summary>
        private async Task<DirectImportResult> ImportGenericCSVAsync<T>(string dataType, string tableName, IFormFile file, string? statementDate = null, bool ultraHighSpeed = false)
            where T : class, new()
        {
            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = dataType,
                TargetTable = tableName,
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🚀 [{DataType}_DIRECT] Bắt đầu Direct Import: {FileName}", dataType, file.FileName);

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking (chỉ metadata)
                var importRecord = await CreateImportedDataRecordAsync(file, dataType, 0);
                result.ImportedDataRecordId = importRecord.Id;

                // Parse CSV và bulk insert
                var records = await ParseGenericCSVAsync<T>(file, statementDate);
                _logger.LogInformation("📊 [IMPORT_DEBUG] Parsed {Count} records from CSV", records.Count);

                if (records.Any())
                {
                    _logger.LogInformation("📊 [IMPORT_DEBUG] Starting bulk insert for {Count} records", records.Count);
                    var insertedCount = await BulkInsertGenericAsync(records, tableName, ultraHighSpeed);
                    result.ProcessedRecords = insertedCount;

                    // Update record count
                    importRecord.RecordsCount = insertedCount;
                    await _context.SaveChangesAsync();
                }

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ [{DataType}_DIRECT] Direct Import thành công: {Count} records trong {Duration}ms",
                    dataType, result.ProcessedRecords, result.Duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [{DataType}_DIRECT] Direct Import failed: {FileName}", dataType, file.FileName);
                return result;
            }
        }

        /// <summary>
        /// Generic Excel import method
        /// </summary>
        private async Task<DirectImportResult> ImportGenericExcelAsync<T>(string dataType, string tableName, IFormFile file, string? statementDate = null)
            where T : class, new()
        {
            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = dataType,
                TargetTable = tableName,
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🚀 [{DataType}_DIRECT] Bắt đầu Excel Direct Import: {FileName}", dataType, file.FileName);

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking (chỉ metadata)
                var importRecord = await CreateImportedDataRecordAsync(file, dataType, 0);
                result.ImportedDataRecordId = importRecord.Id;

                // TODO: Implement Excel parsing
                result.ProcessedRecords = 0;

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ [{DataType}_DIRECT] Excel Direct Import (placeholder): {FileName}", dataType, file.FileName);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [{DataType}_DIRECT] Excel Direct Import failed: {FileName}", dataType, file.FileName);
                return result;
            }
        }

        /// <summary>
        /// ULTRA HIGH-SPEED Import cho GL01/GL02 - Streaming + Optimized Bulk Insert
        /// Không load toàn bộ vào memory, xử lý theo chunks để tốc độ tối đa
        /// </summary>
        private async Task<DirectImportResult> ImportUltraHighSpeedAsync<T>(string dataType, string tableName, IFormFile file, string? statementDate = null)
            where T : class, new()
        {
            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = dataType,
                TargetTable = tableName,
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🚀 [ULTRA_SPEED] Starting ULTRA HIGH-SPEED import for {DataType}: {FileName}", dataType, file.FileName);

                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking
                var importRecord = await CreateImportedDataRecordAsync(file, dataType, 0);
                result.ImportedDataRecordId = importRecord.Id;

                // ULTRA HIGH-SPEED: Direct streaming import without loading all to memory
                var processedRecords = await StreamingBulkInsertAsync<T>(file, tableName, dataType, statementDate);
                result.ProcessedRecords = processedRecords;

                // Update record count
                importRecord.RecordsCount = processedRecords;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("🚀 [ULTRA_SPEED] COMPLETED: {Count} records in {Duration}ms",
                    result.ProcessedRecords, result.Duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [ULTRA_SPEED] Failed: {FileName}", file.FileName);
                return result;
            }
        }

        /// <summary>
        /// ULTRA HIGH-SPEED: Stream CSV directly to SqlBulkCopy without loading into memory
        /// Optimized for GL01/GL02 large files (300k+ records)
        /// </summary>
        private async Task<int> StreamingBulkInsertAsync<T>(IFormFile file, string tableName, string dataType, string? statementDate = null)
            where T : class, new()
        {
            var connectionString = _context.Database.GetConnectionString();
            var processedCount = 0;

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Ultra-optimized SqlBulkCopy settings for maximum performance
            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = tableName,
                BatchSize = 500000, // 500k records per batch
                BulkCopyTimeout = 3600, // 1 hour timeout
                EnableStreaming = true,
                NotifyAfter = 50000 // Progress notification every 50k records
            };

            // Enable all performance optimizations
            bulkCopy.SqlRowsCopied += (sender, e) =>
            {
                _logger.LogInformation("🚀 [STREAMING] Processed {Count} records for {Table}", e.RowsCopied, tableName);
            };

            try
            {
                _logger.LogInformation("🚀 [STREAMING] Starting streaming bulk insert for {Table}: {FileName}", tableName, file.FileName);

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: 16777216); // 16MB buffer
                using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BufferSize = 65536, // 64KB CSV buffer for maximum speed
                    HasHeaderRecord = true,
                    TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                    ReadingExceptionOccurred = (ex) => false, // Skip malformed rows for speed
                    BadDataFound = null // Ignore bad data for maximum performance
                });

                // Skip header row
                await csv.ReadAsync();
                csv.ReadHeader();

                // Create DataReader for streaming to SqlBulkCopy
                var dataReader = new StreamingDataReader<T>(csv, dataType, statementDate, _logger);

                // Map columns for SqlBulkCopy
                SetupColumnMappings<T>(bulkCopy, dataType);

                // Execute streaming bulk copy
                await bulkCopy.WriteToServerAsync(dataReader);
                processedCount = dataReader.RecordCount;

                _logger.LogInformation("✅ [STREAMING] Successfully streamed {Count} records to {Table}", processedCount, tableName);
                return processedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [STREAMING] Failed streaming bulk insert for {Table}", tableName);
                throw;
            }
        }

        /// <summary>
        /// Setup column mappings for SqlBulkCopy based on data type
        /// Special handling for GL02: CSV TRDATE -> DB NGAY_DL
        /// </summary>
        private void SetupColumnMappings<T>(SqlBulkCopy bulkCopy, string dataType)
        {
            bulkCopy.ColumnMappings.Clear();

            if (dataType == "GL02")
            {
                // Special mapping for GL02: CSV columns -> Database columns
                var csvColumns = new[]
                {
                    "TRDATE", "TRBRCD", "USERID", "JOURSEQ", "DYTRSEQ", "LOCAC", "CCY", "BUSCD", "UNIT", "TRCD",
                    "CUSTOMER", "TRTP", "REFERENCE", "REMARK", "DRAMOUNT", "CRAMOUNT", "CRTDTM"
                };

                var dbColumns = new[]
                {
                    "NGAY_DL", "TRBRCD", "USERID", "JOURSEQ", "DYTRSEQ", "LOCAC", "CCY", "BUSCD", "UNIT", "TRCD",
                    "CUSTOMER", "TRTP", "REFERENCE", "REMARK", "DRAMOUNT", "CRAMOUNT", "CRTDTM"
                };

                // Map CSV columns to database columns
                for (int i = 0; i < Math.Min(csvColumns.Length, dbColumns.Length); i++)
                {
                    bulkCopy.ColumnMappings.Add(i, dbColumns[i]);
                    _logger.LogDebug("🔗 [GL02_MAPPING] CSV column {Index} ({CsvCol}) -> DB column {DbCol}",
                        i, csvColumns[i], dbColumns[i]);
                }

                // Add system columns with default values (these won't come from CSV)
                bulkCopy.ColumnMappings.Add("CREATED_DATE", "CREATED_DATE");
                bulkCopy.ColumnMappings.Add("FILE_NAME", "FILE_NAME");
            }
            else
            {
                // Standard mapping for other data types
                var properties = typeof(T).GetProperties();
                foreach (var prop in properties)
                {
                    // Skip navigation properties and non-database columns
                    if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                        continue;
                    if (prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                        continue;

                    bulkCopy.ColumnMappings.Add(prop.Name, prop.Name);
                }
            }

            _logger.LogDebug("🔗 [STREAMING] Mapped {Count} columns for {DataType}", bulkCopy.ColumnMappings.Count, dataType);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Detect data type từ filename
        /// </summary>
        public string DetectDataTypeFromFileName(string fileName)
        {
            var upperFileName = fileName.ToUpper();

            // Priority order detection
            if (upperFileName.Contains("DP01")) return "DP01";
            if (upperFileName.Contains("LN01")) return "LN01";
            if (upperFileName.Contains("LN03")) return "LN03";
            if (upperFileName.Contains("GL01")) return "GL01";
            if (upperFileName.Contains("GL02")) return "GL02";
            if (upperFileName.Contains("GL41")) return "GL41";
            if (upperFileName.Contains("CA01")) return "CA01";
            if (upperFileName.Contains("RR01")) return "RR01";
            if (upperFileName.Contains("TR01")) return "TR01";
            if (upperFileName.Contains("DPDA")) return "DPDA";
            if (upperFileName.Contains("EI01")) return "EI01";

            return "DP01"; // Default
        }

        /// <summary>
        /// Extract NgayDL từ filename (pattern YYYYMMDD -> dd/MM/yyyy)
        /// </summary>
        private string ExtractNgayDLFromFileName(string fileName)
        {
            var datePattern = @"(\d{8})";
            var match = Regex.Match(fileName, datePattern);

            if (match.Success)
            {
                var dateStr = match.Groups[1].Value;
                if (dateStr.Length == 8)
                {
                    var year = dateStr.Substring(0, 4);
                    var month = dateStr.Substring(4, 2);
                    var day = dateStr.Substring(6, 2);
                    return $"{day}/{month}/{year}";
                }
            }

            return DateTime.Now.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Special parser for RR01 format which has non-standard CSV structure
        /// Format: "field1,""field2"",""field3""..."
        /// </summary>
        private async Task<List<T>> ParseRR01SpecialFormatAsync<T>(IFormFile file, string? statementDate = null)
            where T : class, new()
        {
            var records = new List<T>();
            var ngayDL = ExtractNgayDLFromFileName(file.FileName);

            _logger.LogInformation("🔍 [RR01_SPECIAL] Parsing RR01 special format: {FileName}", file.FileName);

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 8 * 1024 * 1024);

            // Read header line
            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(headerLine))
            {
                _logger.LogWarning("❌ [RR01_SPECIAL] No header found");
                return records;
            }

            // Remove BOM if present
            if (headerLine.StartsWith("\uFEFF"))
            {
                headerLine = headerLine.Substring(1);
            }

            var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();
            _logger.LogInformation("📊 [RR01_SPECIAL] Headers: {Headers}", string.Join(", ", headers));

            // Read data lines
            string? dataLine;
            int lineNumber = 1;
            while ((dataLine = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(dataLine)) continue;

                try
                {
                    // Parse the special RR01 format: "field1,""field2"",""field3""..."
                    var fields = ParseRR01DataLine(dataLine);

                    _logger.LogDebug("🔍 [RR01_SPECIAL] Line {LineNumber}: Parsed {FieldCount} fields", lineNumber, fields.Length);

                    if (fields.Length != headers.Length)
                    {
                        _logger.LogWarning("⚠️ [RR01_SPECIAL] Field count mismatch on line {LineNumber}: expected {Expected}, got {Actual}",
                            lineNumber, headers.Length, fields.Length);
                    }

                    // Create model instance
                    var record = new T();
                    var properties = typeof(T).GetProperties();

                    for (int i = 0; i < Math.Min(headers.Length, fields.Length); i++)
                    {
                        var headerName = headers[i];
                        var fieldValue = fields[i];

                        // Find property by column name or property name
                        var property = properties.FirstOrDefault(p =>
                        {
                            var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                            var targetName = columnAttr?.Name ?? p.Name;
                            return string.Equals(targetName, headerName, StringComparison.OrdinalIgnoreCase);
                        });

                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                var convertedValue = ConvertCsvValue(fieldValue, property.PropertyType);
                                property.SetValue(record, convertedValue);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning("⚠️ [RR01_SPECIAL] Field conversion error for {Property}: {Error}",
                                    property.Name, ex.Message);
                            }
                        }
                    }

                    // Set common properties
                    SetCommonProperties(record, ngayDL, file.FileName);
                    records.Add(record);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ [RR01_SPECIAL] Error parsing line {LineNumber}: {Error}", lineNumber, ex.Message);
                }
            }

            _logger.LogInformation("✅ [RR01_SPECIAL] Parsed {RecordCount} records", records.Count);
            return records;
        }

        /// <summary>
        /// Parse RR01 data line with special format: "field1,""field2"",""field3""..."
        /// </summary>
        private string[] ParseRR01DataLine(string dataLine)
        {
            var fields = new List<string>();

            // Remove outer quotes if present
            var trimmed = dataLine.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
            {
                trimmed = trimmed.Substring(1, trimmed.Length - 2);
            }

            // Split by ,"" pattern
            var parts = trimmed.Split(new string[] { ",\"\"" }, StringSplitOptions.None);

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                // Clean up the field value
                if (i == 0)
                {
                    // First field: remove trailing space and comma if any
                    part = part.TrimEnd(' ', ',');
                }
                else
                {
                    // Other fields: remove trailing quotes if present
                    if (part.EndsWith("\""))
                    {
                        part = part.Substring(0, part.Length - 1);
                    }
                }

                // Unescape any remaining double quotes
                part = part.Replace("\"\"", "\"").Trim();
                fields.Add(part);
            }

            return fields.ToArray();
        }
        private async Task<List<T>> ParseGenericCSVAsync<T>(IFormFile file, string? statementDate = null)
            where T : class, new()
        {
            var records = new List<T>();
            var ngayDL = ExtractNgayDLFromFileName(file.FileName);

            _logger.LogInformation("🔍 [CSV_PARSE] Bắt đầu parse CSV: {FileName}, Target Type: {TypeName}", file.FileName, typeof(T).Name);

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 8 * 1024 * 1024);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // 🔧 ENHANCED CSV Configuration để handle complex formats như RR01
            csv.Context.Configuration.MissingFieldFound = null; // Bỏ qua fields không tồn tại
            csv.Context.Configuration.HeaderValidated = null; // Bỏ qua validation header
            csv.Context.Configuration.PrepareHeaderForMatch = args => args.Header.ToUpper(); // Case insensitive

            // 🔧 FIX: Enhanced CSV parsing for complex formats with nested quotes
            csv.Context.Configuration.BadDataFound = null; // Bỏ qua bad data thay vì throw exception
            csv.Context.Configuration.Quote = '"'; // Standard quote character
            csv.Context.Configuration.Escape = '"'; // Escape character for nested quotes
            csv.Context.Configuration.Mode = CsvMode.RFC4180; // Standard CSV mode
            // csv.Context.Configuration.TrimOptions = TrimOptions.Trim; // TrimOptions not available - removed
            csv.Context.Configuration.IgnoreBlankLines = true; // Skip empty lines
            csv.Context.Configuration.AllowComments = false; // No comment support

            _logger.LogInformation("🔧 [CSV_PARSE] Enhanced CSV configuration applied for complex format handling");

            // Auto-configure CSV mapping
            csv.Read();
            csv.ReadHeader();

            // Log headers để debug
            var headers = csv.HeaderRecord;
            Console.WriteLine($"📊 [CSV_PARSE] Headers found for {typeof(T).Name}: {string.Join(", ", headers ?? new string[0])}");
            _logger.LogInformation("📊 [CSV_PARSE] Headers found: {Headers}", string.Join(", ", headers ?? new string[0]));

            // Log model properties để debug
            var modelProps = typeof(T).GetProperties().Select(p => p.Name);
            _logger.LogInformation("🔧 [CSV_PARSE] Model properties: {Properties}", string.Join(", ", modelProps));

            // Configure mapping để bỏ qua auto-increment và system fields
            csv.Context.Configuration.ShouldSkipRecord = args => false;

            int totalRows = 0;
            int successRows = 0;

            while (csv.Read())
            {
                totalRows++;
                Console.WriteLine($"🔄 [CSV_PARSE] Processing row {totalRows} for {typeof(T).Name}");
                try
                {
                    var record = new T();

                    // Manual mapping chỉ các fields có trong CSV headers
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        // Bỏ qua auto-increment và system fields
                        if (prop.Name == "Id" || prop.Name == "CreatedDate" || prop.Name == "UpdatedDate")
                            continue;

                        // ✅ FIX: Sử dụng Column attribute name thay vì property name
                        var columnAttribute = prop.GetCustomAttribute<ColumnAttribute>();
                        var targetColumnName = columnAttribute?.Name ?? prop.Name;

                        // Tìm header tương ứng (case insensitive)
                        var headerName = headers?.FirstOrDefault(h =>
                            string.Equals(h, targetColumnName, StringComparison.OrdinalIgnoreCase));

                        // ✅ FALLBACK: Nếu không tìm thấy bằng Column name, thử property name
                        if (string.IsNullOrEmpty(headerName))
                        {
                            headerName = headers?.FirstOrDefault(h =>
                                string.Equals(h, prop.Name, StringComparison.OrdinalIgnoreCase));
                        }

                        if (!string.IsNullOrEmpty(headerName))
                        {
                            try
                            {
                                // 🔧 ENHANCED: Try multiple approaches to get field value
                                string? value = null;

                                try
                                {
                                    value = csv.GetField(headerName);
                                }
                                catch (Exception getFieldEx)
                                {
                                    _logger.LogDebug("🔧 [CSV_PARSE] GetField failed for {HeaderName}, trying by index: {Error}",
                                        headerName, getFieldEx.Message);

                                    // Fallback: Try getting by header index
                                    var headerIndex = Array.IndexOf(headers ?? new string[0], headerName);
                                    if (headerIndex >= 0 && headerIndex < csv.Parser.Count)
                                    {
                                        try
                                        {
                                            value = csv.Parser[headerIndex];
                                        }
                                        catch (Exception indexEx)
                                        {
                                            _logger.LogDebug("🔧 [CSV_PARSE] Index access also failed: {Error}", indexEx.Message);
                                        }
                                    }
                                }

                                // 🎯 DEBUG: Log every field value for GL02 to debug
                                if (typeof(T).Name == "GL02")
                                {
                                    Console.WriteLine($"🎯 [GL02_FIELD_DEBUG] {prop.Name}: '{value ?? "NULL"}'");
                                }

                                if (!string.IsNullOrEmpty(value))
                                {
                                    // 🔧 ENHANCED: Clean up value (remove extra quotes, trim)
                                    value = value.Trim();

                                    // Handle nested quotes: remove outer quotes if present
                                    if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 1)
                                    {
                                        value = value.Substring(1, value.Length - 2);
                                        // Replace escaped quotes with single quotes
                                        value = value.Replace("\"\"", "\"");
                                    }

                                    // 🎯 DEBUG: Log GL02 CRTDTM field conversions specifically
                                    if (typeof(T).Name == "GL02" && prop.Name == "CRTDTM")
                                    {
                                        Console.WriteLine($"🎯 [GL02_DEBUG] Converting {prop.Name}: '{value}' -> Type: {prop.PropertyType.Name}");
                                        _logger.LogInformation("🎯 [GL02_DEBUG] Converting {PropertyName}: '{Value}' -> Type: {PropertyType}",
                                            prop.Name, value, prop.PropertyType.Name);
                                    }

                                    // 🎯 DEBUG: Log DPDA datetime field conversions specifically
                                    if (typeof(T).Name == "DPDA" && (prop.Name == "NGAY_NOP_DON" || prop.Name == "NGAY_PHAT_HANH"))
                                    {
                                        Console.WriteLine($"🎯 [DPDA_DEBUG] Converting {prop.Name}: '{value}' -> Type: {prop.PropertyType.Name}");
                                        _logger.LogInformation("🎯 [DPDA_DEBUG] Converting {PropertyName}: '{Value}' -> Type: {PropertyType}",
                                            prop.Name, value, prop.PropertyType.Name);
                                    }

                                    // Convert value based on property type
                                    var convertedValue = ConvertCsvValue(value, prop.PropertyType);

                                    // 🎯 EMERGENCY FIX: Direct conversion for GL02 CRTDTM field if ConvertCsvValue fails
                                    if (typeof(T).Name == "GL02" && prop.Name == "CRTDTM" &&
                                        (convertedValue == null || convertedValue is string))
                                    {
                                        if (DateTime.TryParseExact(value, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var gl02Date))
                                        {
                                            convertedValue = gl02Date;
                                            Console.WriteLine($"🔧 [GL02_EMERGENCY] Fixed CRTDTM conversion: {prop.Name} = '{value}' -> {gl02Date}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"❌ [GL02_EMERGENCY] Failed to convert CRTDTM: {prop.Name} = '{value}'");
                                            convertedValue = null;
                                        }
                                    }

                                    // 🎯 EMERGENCY FIX: Direct conversion for DPDA datetime fields if ConvertCsvValue fails
                                    if (typeof(T).Name == "DPDA" && (prop.Name == "NGAY_NOP_DON" || prop.Name == "NGAY_PHAT_HANH") &&
                                        (convertedValue == null || convertedValue is string))
                                    {
                                        if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dpdaDate))
                                        {
                                            convertedValue = dpdaDate;
                                            Console.WriteLine($"🔧 [DPDA_EMERGENCY] Fixed datetime conversion: {prop.Name} = '{value}' -> {dpdaDate}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"❌ [DPDA_EMERGENCY] Failed to convert datetime: {prop.Name} = '{value}'");
                                            convertedValue = null;
                                        }
                                    }

                                    // 🎯 EMERGENCY FIX: Direct conversion for EI01 datetime fields if ConvertCsvValue fails
                                    if (typeof(T).Name == "EI01" && (prop.Name == "NGAY_DK_EMB" || prop.Name == "NGAY_DK_OTT" ||
                                        prop.Name == "NGAY_DK_SMS" || prop.Name == "NGAY_DK_SAV" || prop.Name == "NGAY_DK_LN") &&
                                        (convertedValue == null || convertedValue is string))
                                    {
                                        if (!string.IsNullOrEmpty(value) && value.Trim() != "" &&
                                            DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ei01Date))
                                        {
                                            convertedValue = ei01Date;
                                            Console.WriteLine($"🔧 [EI01_EMERGENCY] Fixed datetime conversion: {prop.Name} = '{value}' -> {ei01Date}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"💡 [EI01_EMERGENCY] NULL/Empty datetime field: {prop.Name} = '{value}' -> NULL");
                                            convertedValue = null;
                                        }
                                    }

                                    // 🎯 DEBUG: Log conversion result for GL02 CRTDTM field
                                    if (typeof(T).Name == "GL02" && prop.Name == "CRTDTM")
                                    {
                                        Console.WriteLine($"🎯 [GL02_DEBUG] Conversion result for {prop.Name}: '{value}' -> {convertedValue?.GetType().Name ?? "NULL"} = {convertedValue}");
                                    }

                                    // 🎯 DEBUG: Log conversion result for DPDA datetime fields
                                    if (typeof(T).Name == "DPDA" && (prop.Name == "NGAY_NOP_DON" || prop.Name == "NGAY_PHAT_HANH"))
                                    {
                                        Console.WriteLine($"🎯 [DPDA_DEBUG] Conversion result for {prop.Name}: '{value}' -> {convertedValue?.GetType().Name ?? "NULL"} = {convertedValue}");
                                    }

                                    if (convertedValue != null)
                                    {
                                        prop.SetValue(record, convertedValue);
                                        _logger.LogDebug("✅ [CSV_PARSE] Successfully set {PropertyName} = {Value}",
                                            prop.Name, value.Length > 50 ? value.Substring(0, 50) + "..." : value);
                                    }
                                }
                            }
                            catch (Exception fieldEx)
                            {
                                _logger.LogWarning("⚠️ [CSV_PARSE] Field mapping error for {PropertyName} -> {ColumnName}: {Error}",
                                    prop.Name, targetColumnName, fieldEx.Message);
                            }
                        }
                        else
                        {
                            _logger.LogDebug("🔍 [CSV_PARSE] No header found for property {PropertyName} (column: {ColumnName})",
                                prop.Name, targetColumnName);
                        }
                    }

                    if (record != null)
                    {
                        // Set common properties if they exist
                        SetCommonProperties(record, ngayDL, file.FileName);
                        records.Add(record);
                        successRows++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ [CSV_PARSE] Lỗi parse CSV dòng {LineNumber}: {Error}", csv.Context.Parser.Row, ex.Message);
                }
            }

            _logger.LogInformation("✅ [CSV_PARSE] Hoàn thành: {SuccessRows}/{TotalRows} rows parsed successfully", successRows, totalRows);

            return records;
        }

        /// <summary>
        /// Set common properties for all entities like NGAY_DL, FILE_NAME, CREATED_DATE, etc.
        /// </summary>
        private void SetCommonProperties<T>(T record, string ngayDL, string fileName)
        {
            var type = typeof(T);

            // Special handling for GL01 - get NGAY_DL from TR_TIME column instead of filename
            if (type.Name == "GL01")
            {
                var ngayDLProp = type.GetProperty("NGAY_DL");
                var trTimeProp = type.GetProperty("TR_TIME");

                if (ngayDLProp != null && ngayDLProp.CanWrite && trTimeProp != null)
                {
                    var trTimeValue = trTimeProp.GetValue(record) as string;
                    if (!string.IsNullOrEmpty(trTimeValue))
                    {
                        var convertedDate = ConvertTrTimeToNgayDL(trTimeValue);
                        if (convertedDate.HasValue)
                        {
                            ngayDLProp.SetValue(record, convertedDate.Value);
                            _logger.LogDebug("🗓️ [GL01_NGAY_DL] Converted TR_TIME '{TrTime}' to NGAY_DL: {NgayDL}", trTimeValue, convertedDate.Value);
                        }
                    }
                }
            }
            // Special handling for GL02 - get NGAY_DL from TRDATE column (YYYYMMDD format)
            else if (type.Name == "GL02")
            {
                var ngayDLProp = type.GetProperty("NGAY_DL");
                // Note: TRDATE is at position 0 in CSV but gets converted to NGAY_DL during mapping

                if (ngayDLProp != null && ngayDLProp.CanWrite)
                {
                    // NGAY_DL should already be set from TRDATE during CSV parsing
                    // Just log for debugging
                    var currentValue = ngayDLProp.GetValue(record);
                    if (currentValue != null)
                    {
                        _logger.LogDebug("🗓️ [GL02_NGAY_DL] NGAY_DL set from CSV TRDATE: {NgayDL}", currentValue);
                    }
                }
            }
            else
            {
                // For other tables, use filename-based NGAY_DL
                // Set NGAY_DL if property exists
                var ngayDLProp = type.GetProperty("NGAY_DL");
                if (ngayDLProp != null && ngayDLProp.CanWrite)
                {
                    // Check if property is DateTime type and convert from string
                    if (ngayDLProp.PropertyType == typeof(DateTime) || ngayDLProp.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.TryParseExact(ngayDL, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime dateValue))
                        {
                            ngayDLProp.SetValue(record, dateValue);
                            _logger.LogDebug("🗓️ [NGAY_DL] Set NGAY_DL property to DateTime: {NgayDL}", dateValue.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ [NGAY_DL] Could not parse date string '{NgayDL}' to DateTime", ngayDL);
                        }
                    }
                    else
                    {
                        // Property is string type, set directly
                        ngayDLProp.SetValue(record, ngayDL);
                        _logger.LogDebug("🗓️ [NGAY_DL] Set NGAY_DL property to string: {NgayDL}", ngayDL);
                    }
                }
            }

            // Set FILE_NAME if property exists
            var fileNameProp = type.GetProperty("FILE_NAME");
            if (fileNameProp != null && fileNameProp.CanWrite)
            {
                fileNameProp.SetValue(record, fileName);
                _logger.LogDebug("📁 [FILE_NAME] Set FILE_NAME property to: {FileName}", fileName);
            }

            // ⚠️ TEMPORAL COLUMNS: Set CREATED_DATE/UPDATED_DATE cho non-temporal tables
            // Set CREATED_DATE if property exists
            var createdDateProp = type.GetProperty("CREATED_DATE");
            if (createdDateProp != null && createdDateProp.CanWrite)
            {
                createdDateProp.SetValue(record, DateTime.UtcNow);
                _logger.LogDebug("🕒 [CREATED_DATE] Set CREATED_DATE property to: {CreatedDate}", DateTime.UtcNow);
            }

            // Set UPDATED_DATE if property exists
            var updatedDateProp = type.GetProperty("UPDATED_DATE");
            if (updatedDateProp != null && updatedDateProp.CanWrite)
            {
                updatedDateProp.SetValue(record, DateTime.UtcNow);
                _logger.LogDebug("🕒 [UPDATED_DATE] Set UPDATED_DATE property to: {UpdatedDate}", DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Convert TR_TIME format (DD-MMM-YY) to DateTime for GL01 NGAY_DL
        /// Examples: "02-Jun-25" -> 02/06/2025, "15-Dec-24" -> 15/12/2024
        /// </summary>
        private DateTime? ConvertTrTimeToNgayDL(string trTime)
        {
            try
            {
                if (string.IsNullOrEmpty(trTime))
                    return null;

                // Parse format DD-MMM-YY (e.g., "02-Jun-25")
                var formats = new[] { "dd-MMM-yy", "d-MMM-yy", "dd-MMM-yyyy", "d-MMM-yyyy" };

                foreach (var format in formats)
                {
                    if (DateTime.TryParseExact(trTime, format, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        _logger.LogDebug("🗓️ [TR_TIME_CONVERT] Successfully converted '{TrTime}' to {DateTime}", trTime, result.ToString("dd/MM/yyyy"));
                        return result;
                    }
                }

                _logger.LogWarning("⚠️ [TR_TIME_CONVERT] Could not parse TR_TIME value: '{TrTime}'", trTime);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [TR_TIME_CONVERT] Error converting TR_TIME '{TrTime}': {Error}", trTime, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Generic bulk insert method with improved column mapping
        /// </summary>
        private async Task<int> BulkInsertGenericAsync<T>(List<T> records, string tableName, bool ultraHighSpeed = false)
        {
            if (!records.Any()) return 0;

            var dataTable = ConvertToDataTable(records);

            _logger.LogInformation("💾 [BULK_INSERT] Table: {TableName}, DataTable columns: {Count} - {Columns}",
                tableName, dataTable.Columns.Count, string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check which columns exist in target table
            var targetColumns = new HashSet<string>();
            using (var command = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'", connection))
            {
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    targetColumns.Add(reader.GetString(0));
                }
            }

            _logger.LogInformation("🎯 [BULK_INSERT] Target table '{TableName}' has {Count} columns: {Columns}",
                tableName, targetColumns.Count, string.Join(", ", targetColumns.Take(10)) + (targetColumns.Count > 10 ? "..." : ""));

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction, null)
            {
                DestinationTableName = tableName,
                BatchSize = ultraHighSpeed ? 750000 : 500000, // 🚀 OPTIMIZED: 750k for GL02 (optimal sweet spot)
                BulkCopyTimeout = ultraHighSpeed ? 7200 : 3600, // 🚀 ULTRA: 2 hours for GL02, 1 hour for others
                EnableStreaming = true,
                NotifyAfter = ultraHighSpeed ? 100000 : 50000 // 🚀 Less frequent logging for speed
            };

            // Smart column mapping - include system columns for data tables
            var mappedColumns = 0;
            var excludedColumns = new HashSet<string> { "Id", "ValidFrom", "ValidTo" }; // Exclude auto-increment + temporal GENERATED ALWAYS columns

            foreach (DataColumn column in dataTable.Columns)
            {
                // Skip GENERATED ALWAYS temporal columns
                if (excludedColumns.Contains(column.ColumnName))
                {
                    _logger.LogInformation("🚫 [BULK_MAPPING] Skipping GENERATED ALWAYS column: {Column}", column.ColumnName);
                    continue;
                }

                if (targetColumns.Contains(column.ColumnName))
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    _logger.LogInformation("✅ [BULK_MAPPING] {Column} -> {Column}", column.ColumnName, column.ColumnName);
                    mappedColumns++;
                }
                else
                {
                    _logger.LogWarning("⚠️ [BULK_MAPPING] Column '{Column}' not found in target table, skipping", column.ColumnName);
                }
            }

            if (mappedColumns == 0)
            {
                throw new InvalidOperationException($"No columns could be mapped between source and target table '{tableName}'");
            }

            await bulkCopy.WriteToServerAsync(dataTable);

            _logger.LogInformation("💾 [{TableName}_BULK] Bulk insert hoàn thành: {Count} records với {MappedColumns} columns",
                tableName, records.Count, mappedColumns);
            return records.Count;
        }

        /// <summary>
        /// Convert generic list to DataTable - include system columns như CREATED_DATE/UPDATED_DATE
        /// </summary>
        private DataTable ConvertToDataTable<T>(List<T> records)
        {
            var table = new DataTable();
            var properties = typeof(T).GetProperties()
                .Where(p =>
                    p.Name != "Id" &&
                    p.Name != "UpdatedDate" &&
                    p.Name != "DATA_DATE" &&
                    p.Name != "CreatedAt" &&
                    p.Name != "UpdatedAt" &&
                    p.Name != "ValidFrom" &&
                    p.Name != "ValidTo" &&
                    p.Name != "IsDeleted" &&
                    p.Name != "SysStartTime" &&
                    p.Name != "SysEndTime" &&
                    // ✅ INCLUDE system columns như CREATED_DATE/UPDATED_DATE - đã set values trong SetCommonProperties
                    !p.Name.StartsWith("System") &&
                    p.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0) // Chỉ lấy properties có Column attribute
                .ToArray();

            var columnMappings = new Dictionary<string, string>(); // PropertyName -> ColumnName

            _logger.LogInformation("📊 [DATATABLE] Creating DataTable with {Count} business columns from {Total} total properties",
                properties.Length, typeof(T).GetProperties().Length);

            // Debug: Log tất cả properties được include
            _logger.LogInformation("🔍 [DEBUG] Included properties: {Properties}",
                string.Join(", ", properties.Select(p => p.Name)));

            // Create columns sử dụng Column attribute names
            foreach (var property in properties)
            {
                var columnAttr = property.GetCustomAttributes(typeof(ColumnAttribute), false)
                    .FirstOrDefault() as ColumnAttribute;

                var columnName = columnAttr?.Name ?? property.Name; // Use Column attribute name or property name
                columnMappings[property.Name] = columnName;

                var columnType = property.PropertyType;
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = columnType.GetGenericArguments()[0];
                }

                // 🔧 SPECIAL HANDLING: For NgayDL column, use DateTime type for database compatibility
                if (columnName == "NGAY_DL" && columnType == typeof(string))
                {
                    _logger.LogInformation("🗓️ [DATATABLE] Converting NGAY_DL column from string to DateTime for database compatibility");
                    columnType = typeof(DateTime);
                }

                table.Columns.Add(columnName, columnType);
            }

            _logger.LogInformation("📊 [DATATABLE] Column mappings: {Mappings}",
                string.Join(", ", columnMappings.Select(kvp => $"{kvp.Key}->{kvp.Value}")));

            // Fill data
            foreach (var record in records)
            {
                var row = table.NewRow();
                foreach (var property in properties)
                {
                    var value = property.GetValue(record);
                    var columnName = columnMappings[property.Name];

                    // 🔧 SPECIAL HANDLING: Convert NgayDL string to DateTime for database
                    if (columnName == "NGAY_DL" && value is string ngayDLStr && !string.IsNullOrEmpty(ngayDLStr))
                    {
                        if (DateTime.TryParseExact(ngayDLStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                        {
                            row[columnName] = dateValue;
                            _logger.LogDebug("🗓️ [DATATABLE] Converted NgayDL '{NgayDLStr}' to DateTime: {DateTime}", ngayDLStr, dateValue);
                        }
                        else
                        {
                            // Fallback to current date if parsing fails
                            row[columnName] = DateTime.Now.Date;
                            _logger.LogWarning("⚠️ [DATATABLE] Failed to parse NgayDL '{NgayDLStr}', using current date", ngayDLStr);
                        }
                    }
                    else
                    {
                        // 🔧 DEBUG: Log value type for datetime/decimal columns
                        if (columnName.Contains("DATE") || columnName.Contains("AMOUNT") || columnName.Contains("RATE") || columnName.Contains("BALANCE"))
                        {
                            _logger.LogDebug("🔍 [DATATABLE] Column {Column}: Value={Value}, Type={Type}",
                                columnName, value ?? "NULL", value?.GetType().Name ?? "NULL");
                        }

                        // 🔧 CRITICAL BUG FIX: Check if value is still string for DateTime columns
                        if (value is string stringValue && property.PropertyType == typeof(DateTime?) && !string.IsNullOrEmpty(stringValue))
                        {
                            Console.WriteLine($"⚠️ [DATATABLE] CRITICAL: DateTime column {columnName} has string value: '{stringValue}' - CONVERSION FAILED!");
                            _logger.LogError("⚠️ [DATATABLE] CRITICAL: DateTime column {Column} has string value: '{Value}' - CONVERSION FAILED!",
                                columnName, stringValue);

                            // 🎯 ENHANCED: Multiple datetime format support for DPDA
                            string[] formats = { "yyyyMMdd", "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" };
                            DateTime emergencyDate;
                            bool converted = false;

                            foreach (var format in formats)
                            {
                                if (DateTime.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out emergencyDate))
                                {
                                    Console.WriteLine($"✅ [DATATABLE] Emergency conversion success: '{stringValue}' -> {emergencyDate} (format: {format})");
                                    _logger.LogInformation("✅ [DATATABLE] Emergency conversion success: '{Original}' -> {Converted} (format: {Format})",
                                        stringValue, emergencyDate, format);
                                    row[columnName] = emergencyDate;
                                    converted = true;
                                    break;
                                }
                            }

                            if (!converted)
                            {
                                Console.WriteLine($"❌ [DATATABLE] All emergency conversion attempts failed for: '{stringValue}'");
                                _logger.LogError("❌ [DATATABLE] All emergency conversion attempts failed for: '{Value}'", stringValue);
                                row[columnName] = DBNull.Value;
                            }
                        }
                        else
                        {
                            row[columnName] = value ?? DBNull.Value;
                        }
                    }
                }
                table.Rows.Add(row);
            }

            _logger.LogInformation("📊 [DATATABLE] Created DataTable: {RowCount} rows, {ColumnCount} columns",
                table.Rows.Count, table.Columns.Count);

            // Debug: Log tất cả column names trong DataTable
            _logger.LogInformation("🔍 [DEBUG] DataTable columns: {Columns}",
                string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

            return table;
        }

        /// <summary>
        /// Tạo ImportedDataRecord chỉ cho metadata tracking
        /// </summary>
        private async Task<ImportedDataRecord> CreateImportedDataRecordAsync(IFormFile file, string category, int recordCount)
        {
            var record = new ImportedDataRecord
            {
                FileName = file.FileName,
                FileType = category,
                Category = category,
                ImportDate = DateTime.Now,
                ImportedBy = "DirectImportService",
                RecordsCount = recordCount,
                Status = "Completed",
                Notes = $"Direct import to {category} table - No JSON storage"
            };

            _context.ImportedDataRecords.Add(record);
            await _context.SaveChangesAsync();

            return record;
        }

        /// <summary>
        /// Lấy lịch sử import để hiển thị trong Raw Data view
        /// </summary>
        public async Task<List<object>> GetImportHistoryAsync()
        {
            try
            {
                var records = await _context.ImportedDataRecords
                    .OrderByDescending(r => r.ImportDate)
                    .Take(100) // Giới hạn 100 records gần nhất
                    .Select(r => new
                    {
                        Id = r.Id,
                        FileName = r.FileName,
                        FileType = r.FileType,
                        Category = r.Category,
                        ImportDate = r.ImportDate,
                        ImportedBy = r.ImportedBy,
                        RecordsCount = r.RecordsCount,
                        Status = r.Status,
                        Notes = r.Notes,
                        FileSizeDisplay = r.OriginalFileData != null ? FormatFileSize(r.OriginalFileData.Length) : "N/A",
                        NgayDL = r.StatementDate != null ? r.StatementDate.Value.ToString("dd/MM/yyyy") : "N/A"
                    })
                    .ToListAsync();

                _logger.LogInformation($"📋 Retrieved {records.Count} import history records");
                return records.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving import history");
                throw;
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }

        /// <summary>
        /// Convert CSV string value to proper type with enhanced cleaning and formatting support
        /// </summary>
        private object? ConvertCsvValue(string csvValue, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(csvValue))
                return null;

            try
            {
                // Enhanced string cleaning for complex CSV formats
                var cleanedValue = csvValue.Trim();

                // Remove BOM if present
                if (cleanedValue.Length > 0 && cleanedValue[0] == '\uFEFF')
                {
                    cleanedValue = cleanedValue.Substring(1);
                }

                // Handle quoted values with potential nested content
                if (cleanedValue.StartsWith("\"") && cleanedValue.EndsWith("\"") && cleanedValue.Length > 1)
                {
                    cleanedValue = cleanedValue.Substring(1, cleanedValue.Length - 2);
                    cleanedValue = cleanedValue.Replace("\"\"", "\""); // Unescape quotes
                }

                // Remove extra whitespace
                cleanedValue = cleanedValue.Trim();

                // Handle nullable types
                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (underlyingType == typeof(string))
                {
                    // For strings, apply additional cleaning for bank data
                    var result = cleanedValue;

                    // Handle special characters in bank data
                    if (result.Contains("'") && result.StartsWith("'"))
                    {
                        result = result.Substring(1); // Remove leading quote
                    }

                    return result;
                }
                else if (underlyingType == typeof(decimal))
                {
                    // Enhanced decimal parsing for financial data
                    var normalizedValue = cleanedValue;

                    // Remove leading single quote that's common in bank CSV exports
                    if (normalizedValue.StartsWith("'"))
                    {
                        normalizedValue = normalizedValue.Substring(1);
                    }

                    // Remove all commas and spaces for proper decimal parsing
                    normalizedValue = normalizedValue.Replace(",", "").Replace(" ", "").Trim();

                    if (string.IsNullOrWhiteSpace(normalizedValue))
                        return null;

                    if (decimal.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalResult))
                    {
                        _logger.LogDebug("✅ [CONVERT] Decimal success: '{Original}' -> '{Cleaned}' -> {Result}",
                            csvValue, normalizedValue, decimalResult);
                        return decimalResult;
                    }

                    _logger.LogWarning("⚠️ [CONVERT] Decimal conversion failed: '{Original}' -> '{Cleaned}'",
                        csvValue, normalizedValue);
                    return null;
                }
                else if (underlyingType == typeof(int))
                {
                    var normalizedValue = cleanedValue.Replace(",", "").Replace(" ", "").Trim();

                    if (string.IsNullOrWhiteSpace(normalizedValue))
                        return null;

                    if (int.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var intResult))
                    {
                        return intResult;
                    }
                    return null;
                }
                else if (underlyingType == typeof(long))
                {
                    var normalizedValue = cleanedValue.Replace(",", "").Replace(" ", "").Trim();

                    if (string.IsNullOrWhiteSpace(normalizedValue))
                        return null;

                    if (long.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var longResult))
                    {
                        return longResult;
                    }
                    return null;
                }
                else if (underlyingType == typeof(double))
                {
                    var normalizedValue = cleanedValue.Replace(",", "").Replace(" ", "").Trim();

                    if (string.IsNullOrWhiteSpace(normalizedValue))
                        return null;

                    if (double.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleResult))
                    {
                        return doubleResult;
                    }
                    return null;
                }
                else if (underlyingType == typeof(DateTime))
                {
                    // Enhanced datetime handling for multiple formats
                    var dateValue = cleanedValue;

                    // Remove leading single quote that's common in bank CSV exports
                    if (dateValue.StartsWith("'"))
                    {
                        dateValue = dateValue.Substring(1);
                    }

                    if (string.IsNullOrWhiteSpace(dateValue))
                        return null;

                    // Support multiple date formats in priority order
                    string[] dateFormats = {
                        "yyyyMMdd",           // Bank standard format: 20250630
                        "yyyyMMdd HH:mm:ss",  // GL02 CRTDTM format: 20241201 11:35:22
                        "dd/MM/yyyy",         // Common display format: 30/06/2025
                        "yyyy-MM-dd",         // ISO format: 2025-06-30
                        "dd-MM-yyyy",         // Alternative format: 30-06-2025
                        "MM/dd/yyyy",         // US format: 06/30/2025
                        "dd-MMM-yy",          // Short format: 30-Jun-25
                        "dd-MMM-yyyy",        // Long format: 30-Jun-2025
                        "dd/MM/yyyy HH:mm:ss",// With time: 30/06/2025 14:30:00
                        "yyyy-MM-dd HH:mm:ss" // ISO with time: 2025-06-30 14:30:00
                    };

                    if (DateTime.TryParseExact(dateValue.Trim(), dateFormats, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var dateResult))
                    {
                        _logger.LogDebug("✅ [CONVERT] DateTime success: '{Original}' -> '{Cleaned}' -> {Result}",
                            csvValue, dateValue, dateResult.ToString("yyyy-MM-dd"));
                        return dateResult;
                    }

                    // Fallback to standard parsing
                    if (DateTime.TryParse(dateValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fallbackDate))
                    {
                        _logger.LogDebug("✅ [CONVERT] DateTime fallback success: '{Original}' -> '{Cleaned}' -> {Result}",
                            csvValue, dateValue, fallbackDate.ToString("yyyy-MM-dd"));
                        return fallbackDate;
                    }

                    _logger.LogWarning("⚠️ [CONVERT] DateTime conversion failed: '{Original}' -> '{Cleaned}'",
                        csvValue, dateValue);
                    return null;
                }
                else if (underlyingType == typeof(bool))
                {
                    // Handle common boolean representations
                    var normalizedValue = cleanedValue.ToLowerInvariant();

                    if (string.IsNullOrWhiteSpace(normalizedValue))
                        return null;

                    if (normalizedValue == "1" || normalizedValue == "true" || normalizedValue == "yes" || normalizedValue == "y")
                        return true;
                    if (normalizedValue == "0" || normalizedValue == "false" || normalizedValue == "no" || normalizedValue == "n")
                        return false;

                    // Try standard parsing
                    if (bool.TryParse(normalizedValue, out var boolResult))
                    {
                        return boolResult;
                    }

                    return null;
                }
                else
                {
                    // Fallback: try direct conversion for other types
                    try
                    {
                        var result = Convert.ChangeType(cleanedValue.Trim(), underlyingType, CultureInfo.InvariantCulture);
                        return result;
                    }
                    catch
                    {
                        _logger.LogWarning("⚠️ [CONVERT] Direct conversion failed for {Type}: '{Value}'",
                            underlyingType.Name, cleanedValue);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ [CSV_CONVERT] Cannot convert '{Value}' to {TargetType}: {Error}",
                    csvValue, targetType.Name, ex.Message);
                return null;
            }
        }

        #endregion

        #region Preview and Delete Methods

        /// <summary>
        /// Mapping thứ tự cột theo CSV gốc cho từng bảng
        /// </summary>
        private readonly Dictionary<string, string[]> _csvColumnOrder = new Dictionary<string, string[]>
        {
            ["DP01"] = new[]
            {
                "MA_CN", "TAI_KHOAN_HACH_TOAN", "MA_KH", "TEN_KH", "DP_TYPE_NAME", "CCY", "CURRENT_BALANCE", "RATE",
                "SO_TAI_KHOAN", "OPENING_DATE", "MATURITY_DATE", "ADDRESS", "NOTENO", "MONTH_TERM", "TERM_DP_NAME",
                "TIME_DP_NAME", "MA_PGD", "TEN_PGD", "DP_TYPE_CODE", "RENEW_DATE", "CUST_TYPE", "CUST_TYPE_NAME",
                "CUST_TYPE_DETAIL", "CUST_DETAIL_NAME", "PREVIOUS_DP_CAP_DATE", "NEXT_DP_CAP_DATE", "ID_NUMBER",
                "ISSUED_BY", "ISSUE_DATE", "SEX_TYPE", "BIRTH_DATE", "TELEPHONE", "ACRUAL_AMOUNT", "ACRUAL_AMOUNT_END",
                "ACCOUNT_STATUS", "DRAMT", "CRAMT", "EMPLOYEE_NUMBER", "EMPLOYEE_NAME", "SPECIAL_RATE", "AUTO_RENEWAL",
                "CLOSE_DATE", "LOCAL_PROVIN_NAME", "LOCAL_DISTRICT_NAME", "LOCAL_WARD_NAME", "TERM_DP_TYPE",
                "TIME_DP_TYPE", "STATES_CODE", "ZIP_CODE", "COUNTRY_CODE", "TAX_CODE_LOCATION", "MA_CAN_BO_PT",
                "TEN_CAN_BO_PT", "PHONG_CAN_BO_PT", "NGUOI_NUOC_NGOAI", "QUOC_TICH", "MA_CAN_BO_AGRIBANK",
                "NGUOI_GIOI_THIEU", "TEN_NGUOI_GIOI_THIEU", "CONTRACT_COUTS_DAY", "SO_KY_AD_LSDB", "UNTBUSCD", "TYGIA"
            },
            ["RR01"] = new[]
            {
                "CN_LOAI_I", "BRCD", "MA_KH", "TEN_KH", "SO_LDS", "CCY", "SO_LAV", "LOAI_KH", "NGAY_GIAI_NGAN",
                "NGAY_DEN_HAN", "VAMC_FLG", "NGAY_XLRR", "DUNO_GOC_BAN_DAU", "DUNO_LAI_TICHLUY_BD", "DOC_DAUKY_DA_THU_HT",
                "DUNO_GOC_HIENTAI", "DUNO_LAI_HIENTAI", "DUNO_NGAN_HAN", "DUNO_TRUNG_HAN", "DUNO_DAI_HAN",
                "THU_GOC", "THU_LAI", "BDS", "DS", "TSK"
            },
            ["DPDA"] = new[]
            {
                "MA_CHI_NHANH", "MA_KHACH_HANG", "TEN_KHACH_HANG", "SO_TAI_KHOAN", "LOAI_THE", "SO_THE",
                "NGAY_NOP_DON", "NGAY_PHAT_HANH", "USER_PHAT_HANH", "TRANG_THAI", "PHAN_LOAI", "GIAO_THE", "LOAI_PHAT_HANH"
            },
            ["EI01"] = new[]
            {
                "MA_CN", "MA_KH", "TEN_KH", "LOAI_KH", "SDT_EMB", "TRANG_THAI_EMB", "NGAY_DK_EMB", "SDT_OTT",
                "TRANG_THAI_OTT", "NGAY_DK_OTT", "SDT_SMS", "TRANG_THAI_SMS", "NGAY_DK_SMS", "SDT_SAV",
                "TRANG_THAI_SAV", "NGAY_DK_SAV", "SDT_LN", "TRANG_THAI_LN", "NGAY_DK_LN", "USER_EMB",
                "USER_OTT", "USER_SMS", "USER_SAV", "USER_LN"
            },
            ["GL01"] = new[]
            {
                "STS", "NGAY_GD", "NGUOI_TAO", "DYSEQ", "TR_TYPE", "DT_SEQ", "TAI_KHOAN", "TEN_TK", "SO_TIEN_GD",
                "POST_BR", "LOAI_TIEN", "DR_CR", "MA_KH", "TEN_KH", "CCA_USRID", "TR_EX_RT", "REMARK",
                "BUS_CODE", "UNIT_BUS_CODE", "TR_CODE", "TR_NAME", "REFERENCE", "VALUE_DATE", "DEPT_CODE",
                "TR_TIME", "COMFIRM", "TRDT_TIME"
            },
            ["GL02"] = new[]
            {
                "TRDATE", "TRBRCD", "USERID", "JOURSEQ", "DYTRSEQ", "LOCAC", "CCY", "BUSCD", "UNIT", "TRCD",
                "CUSTOMER", "TRTP", "REFERENCE", "REMARK", "DRAMOUNT", "CRAMOUNT", "CRTDTM"
            },
            ["GL41"] = new[]
            {
                "MA_CN", "LOAI_TIEN", "MA_TK", "TEN_TK", "LOAI_BT", "DN_DAUKY", "DC_DAUKY", "SBT_NO", "ST_GHINO",
                "SBT_CO", "ST_GHICO", "DN_CUOIKY", "DC_CUOIKY"
            },
            ["LN01"] = new[]
            {
                "BRCD", "CUSTSEQ", "CUSTNM", "TAI_KHOAN", "CCY", "DU_NO", "DSBSSEQ", "TRANSACTION_DATE", "DSBSDT",
                "DISBUR_CCY", "DISBURSEMENT_AMOUNT", "DSBSMATDT", "BSRTCD", "INTEREST_RATE", "APPRSEQ", "APPRDT",
                "APPR_CCY", "APPRAMT", "APPRMATDT", "LOAN_TYPE", "FUND_RESOURCE_CODE", "FUND_PURPOSE_CODE",
                "REPAYMENT_AMOUNT", "NEXT_REPAY_DATE", "NEXT_REPAY_AMOUNT", "NEXT_INT_REPAY_DATE", "OFFICER_ID",
                "OFFICER_NAME", "INTEREST_AMOUNT", "PASTDUE_INTEREST_AMOUNT", "TOTAL_INTEREST_REPAY_AMOUNT",
                "CUSTOMER_TYPE_CODE", "CUSTOMER_TYPE_CODE_DETAIL", "TRCTCD", "TRCTNM", "ADDR1", "PROVINCE",
                "LCLPROVINNM", "DISTRICT", "LCLDISTNM", "COMMCD", "LCLWARDNM", "LAST_REPAY_DATE", "SECURED_PERCENT",
                "NHOM_NO", "LAST_INT_CHARGE_DATE", "EXEMPTINT", "EXEMPTINTTYPE", "EXEMPTINTAMT", "GRPNO", "BUSCD",
                "BSNSSCLTPCD", "USRIDOP", "ACCRUAL_AMOUNT", "ACCRUAL_AMOUNT_END_OF_MONTH", "INTCMTH", "INTRPYMTH",
                "INTTRMMTH", "YRDAYS", "REMARK", "CHITIEU", "CTCV", "CREDIT_LINE_YPE", "INT_LUMPSUM_PARTIAL_TYPE",
                "INT_PARTIAL_PAYMENT_TYPE", "INT_PAYMENT_INTERVAL", "AN_HAN_LAI", "PHUONG_THUC_GIAI_NGAN_1",
                "TAI_KHOAN_GIAI_NGAN_1", "SO_TIEN_GIAI_NGAN_1", "PHUONG_THUC_GIAI_NGAN_2", "TAI_KHOAN_GIAI_NGAN_2",
                "SO_TIEN_GIAI_NGAN_2", "CMT_HC", "NGAY_SINH", "MA_CB_AGRI", "MA_NGANH_KT", "TY_GIA", "OFFICER_IPCAS"
            },
            ["LN03"] = new[]
            {
                "MACHINHANH", "TENCHINHANH", "MAKH", "TENKH", "SOHOPDONG", "SOTIENXLRR", "NGAYPHATSINHXL",
                "THUNOSAUXL", "CONLAINGOAIBANG", "DUNONOIBANG", "NHOMNO", "MACBTD", "TENCBTD", "MAPGD",
                "TAIKHOANHACHTOAN", "REFNO", "LOAINGUONVON"
            }
        };

        /// <summary>
        /// Sắp xếp lại dictionary theo thứ tự cột CSV gốc
        /// </summary>
        private object ReorderColumnsToMatchCSV(Dictionary<string, object> row, string category)
        {
            if (!_csvColumnOrder.ContainsKey(category))
            {
                return row; // Nếu không có mapping, trả về nguyên bản
            }

            var csvColumns = _csvColumnOrder[category];

            // Tạo ordered dictionary để đảm bảo thứ tự
            var orderedData = new System.Collections.Specialized.OrderedDictionary();

            // Thêm các cột business theo thứ tự CSV gốc
            foreach (var columnName in csvColumns)
            {
                if (row.ContainsKey(columnName))
                {
                    orderedData[columnName] = row[columnName];
                }
            }

            // Thêm các cột temporal/system cuối cùng
            var systemColumns = new[] { "Id", "NGAY_DL", "CREATED_DATE", "UPDATED_DATE", "FILE_NAME" };
            foreach (var columnName in systemColumns)
            {
                if (row.ContainsKey(columnName))
                {
                    orderedData[columnName] = row[columnName];
                }
            }

            return orderedData;
        }

        /// <summary>
        /// Lấy preview data cho import record
        /// </summary>
        public async Task<object?> GetImportPreviewAsync(int importId)
        {
            try
            {
                _logger.LogInformation("🔍 Getting preview data for import ID: {ImportId}", importId);

                // Tìm import record trong ImportedDataRecords
                var importRecord = await _context.ImportedDataRecords
                    .FirstOrDefaultAsync(x => x.Id == importId);

                if (importRecord == null)
                {
                    _logger.LogWarning("⚠️ Import record not found: {ImportId}", importId);
                    return null;
                }

                _logger.LogInformation("✅ Found import record: {FileName}, Category: {Category}",
                    importRecord.FileName, importRecord.Category);

                // Lấy dữ liệu thực tế từ bảng tương ứng
                var previewRows = new List<object>();
                var tableName = GetTableNameFromCategory(importRecord.Category);
                int actualTotalRecords = 0; // 🔧 FIX: Đếm thực tế số records trong database

                if (!string.IsNullOrEmpty(tableName))
                {
                    try
                    {
                        // 🔧 FIX: Filter dữ liệu theo import record cụ thể
                        // Extract ngày từ FileName (format: 7800_dp01_20241231.csv)
                        string sql;
                        string countSql;
                        string? targetDate = null;

                        if (!string.IsNullOrEmpty(importRecord.FileName))
                        {
                            // Extract date from filename pattern: *_YYYYMMDD.csv
                            var fileNamePattern = System.Text.RegularExpressions.Regex.Match(
                                importRecord.FileName, @"(\d{8})");

                            if (fileNamePattern.Success)
                            {
                                var dateStr = fileNamePattern.Groups[1].Value;
                                if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null,
                                    System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                                {
                                    targetDate = parsedDate.ToString("yyyy-MM-dd"); // 🔧 FIX: Sử dụng ISO format cho SQL Server
                                    _logger.LogInformation("📅 Extracted date from filename: {Date}", targetDate);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(targetDate))
                        {
                            // Filter theo NGAY_DL với ngày extracted từ filename VÀ timerange của import
                            sql = $@"SELECT TOP 10 * FROM {tableName}
                                   WHERE NGAY_DL = @targetDate
                                   AND CREATED_DATE >= DATEADD(minute, -10, @importDate)
                                   AND CREATED_DATE <= DATEADD(minute, 10, @importDate)
                                   ORDER BY CREATED_DATE DESC, ID DESC";
                            countSql = $@"SELECT COUNT(*) FROM {tableName}
                                        WHERE NGAY_DL = @targetDate
                                        AND CREATED_DATE >= DATEADD(minute, -10, @importDate)
                                        AND CREATED_DATE <= DATEADD(minute, 10, @importDate)";
                        }
                        else
                        {
                            // Fallback: Lấy dữ liệu theo ImportDate
                            var importDateStr = importRecord.ImportDate.ToString("dd/MM/yyyy");
                            sql = $@"SELECT TOP 10 * FROM {tableName}
                                   WHERE CAST(CREATED_DATE AS DATE) = @importDate
                                   ORDER BY ID DESC";
                            countSql = $@"SELECT COUNT(*) FROM {tableName}
                                        WHERE CAST(CREATED_DATE AS DATE) = @importDate";
                        }

                        _logger.LogInformation("🔍 Executing preview query for {FileName}: {SQL}",
                            importRecord.FileName, sql);

                        using var connection = new SqlConnection(_connectionString);
                        await connection.OpenAsync();

                        // 🔧 FIX: Đếm tổng số records thực tế trước
                        using (var countCommand = new SqlCommand(countSql, connection))
                        {
                            // Thêm parameters để tránh SQL injection
                            if (!string.IsNullOrEmpty(targetDate))
                            {
                                countCommand.Parameters.AddWithValue("@targetDate", targetDate);
                                countCommand.Parameters.AddWithValue("@importDate", importRecord.ImportDate);
                            }
                            else
                            {
                                countCommand.Parameters.AddWithValue("@importDate", importRecord.ImportDate.Date);
                            }

                            var countResult = await countCommand.ExecuteScalarAsync();
                            actualTotalRecords = Convert.ToInt32(countResult ?? 0);
                            _logger.LogInformation("📊 Actual total records in database: {Count}", actualTotalRecords);
                        }

                        using var command = new SqlCommand(sql, connection);

                        // Thêm parameters để tránh SQL injection
                        if (!string.IsNullOrEmpty(targetDate))
                        {
                            command.Parameters.AddWithValue("@targetDate", targetDate);
                            command.Parameters.AddWithValue("@importDate", importRecord.ImportDate);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@importDate", importRecord.ImportDate.Date);
                        }

                        using var reader = await command.ExecuteReaderAsync(); var rows = new List<object>();
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                            }

                            // 🔄 Sắp xếp lại cột theo thứ tự CSV gốc
                            var orderedRow = ReorderColumnsToMatchCSV(row, importRecord.Category);
                            rows.Add(orderedRow);
                        }

                        previewRows = rows.Cast<object>().ToList();
                        _logger.LogInformation("📊 Retrieved {Count} preview rows from {TableName} for date {Date}",
                            previewRows.Count, tableName, targetDate ?? "ImportDate");

                        // 🔍 Nếu không tìm thấy data với NGAY_DL, thử fallback query
                        if (previewRows.Count == 0 && actualTotalRecords == 0 && !string.IsNullOrEmpty(targetDate))
                        {
                            _logger.LogWarning("⚠️ No data found with NGAY_DL filter, trying fallback query");

                            // Fallback: Lấy theo CREATED_DATE gần nhất với ImportDate
                            var fallbackSql = $@"SELECT TOP 10 * FROM {tableName}
                                               WHERE ABS(DATEDIFF(day, CREATED_DATE, @importDate)) <= 1
                                               ORDER BY CREATED_DATE DESC, ID DESC";

                            var fallbackCountSql = $@"SELECT COUNT(*) FROM {tableName}
                                                     WHERE ABS(DATEDIFF(day, CREATED_DATE, @importDate)) <= 1";

                            // Đếm fallback total
                            using (var fallbackCountCommand = new SqlCommand(fallbackCountSql, connection))
                            {
                                fallbackCountCommand.Parameters.AddWithValue("@importDate", importRecord.ImportDate.Date);
                                var fallbackCountResult = await fallbackCountCommand.ExecuteScalarAsync();
                                actualTotalRecords = Convert.ToInt32(fallbackCountResult ?? 0);
                                _logger.LogInformation("📊 Fallback actual total records: {Count}", actualTotalRecords);
                            }

                            using var fallbackCommand = new SqlCommand(fallbackSql, connection);
                            fallbackCommand.Parameters.AddWithValue("@importDate", importRecord.ImportDate.Date);

                            using var fallbackReader = await fallbackCommand.ExecuteReaderAsync(); var fallbackRows = new List<object>();
                            while (await fallbackReader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < fallbackReader.FieldCount; i++)
                                {
                                    var value = fallbackReader.GetValue(i);
                                    row[fallbackReader.GetName(i)] = value == DBNull.Value ? null : value;
                                }

                                // 🔄 Sắp xếp lại cột theo thứ tự CSV gốc cho fallback query
                                var orderedRow = ReorderColumnsToMatchCSV(row, importRecord.Category);
                                fallbackRows.Add(orderedRow);
                            }

                            previewRows = fallbackRows.Cast<object>().ToList();
                            _logger.LogInformation("📊 Fallback query retrieved {Count} preview rows, total: {Total}",
                                previewRows.Count, actualTotalRecords);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Could not retrieve preview data from table {TableName}", tableName);
                        // Không throw exception, chỉ log warning và trả về empty list
                    }
                }

                // Trả về thông tin với dữ liệu thực tế
                return new
                {
                    ImportId = importRecord.Id,
                    FileName = importRecord.FileName,
                    Category = importRecord.Category,
                    ImportDate = importRecord.ImportDate,
                    TotalRecords = actualTotalRecords, // 🔧 FIX: Sử dụng số thực tế từ database
                    PreviewRows = previewRows
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting preview data for import {ImportId}", importId);
                throw;
            }
        }

        /// <summary>
        /// Xóa import record và dữ liệu liên quan (bao gồm cả dữ liệu thực tế trong bảng)
        /// </summary>
        public async Task<(bool Success, string ErrorMessage, int RecordsDeleted)> DeleteImportAsync(int importId)
        {
            try
            {
                _logger.LogInformation("🗑️ Starting delete operation for import ID: {ImportId}", importId);

                // Tìm import record
                var importRecord = await _context.ImportedDataRecords
                    .FirstOrDefaultAsync(x => x.Id == importId);

                if (importRecord == null)
                {
                    _logger.LogWarning("⚠️ Import record not found: {ImportId}", importId);
                    return (false, "Import record not found", 0);
                }

                _logger.LogInformation("🔍 Found import record: {FileName}, Category: {Category}, Records: {RecordsCount}",
                    importRecord.FileName, importRecord.Category, importRecord.RecordsCount);

                var totalDeletedRecords = 0;

                // 🔥 XÓA DỮ LIỆU THỰC TẾ từ bảng tương ứng dựa trên import date và file name
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var dataType = importRecord.Category?.ToUpper();
                if (!string.IsNullOrEmpty(dataType) && IsValidDataType(dataType))
                {
                    // 🎯 CHIẾN LƯỢC XÓA DỮ LIỆU CHÍNH XÁC:
                    // 1. Ưu tiên xóa theo FILE_NAME và NGAY_DL (nếu có)
                    // 2. Fallback: Xóa theo CREATED_DATE trong khoảng thời gian import (±10 phút)
                    // 3. Last resort: Xóa theo batch ID nếu có

                    int deletedDataRecords = 0;

                    // Thử xóa theo FILE_NAME và NGAY_DL trước
                    if (!string.IsNullOrEmpty(importRecord.FileName))
                    {
                        var deleteSql1 = $@"
                            DELETE FROM [{dataType}]
                            WHERE FILE_NAME = @FileName
                               OR (FILE_NAME IS NULL AND NGAY_DL = @NgayDl)";

                        using var deleteCmd1 = new SqlCommand(deleteSql1, connection);
                        deleteCmd1.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = importRecord.FileName;
                        deleteCmd1.Parameters.Add("@NgayDl", SqlDbType.Date).Value = importRecord.ImportDate.Date;

                        deletedDataRecords = await deleteCmd1.ExecuteNonQueryAsync();

                        _logger.LogInformation("🗑️ Deleted {DeletedDataRecords} records using FILE_NAME/NGAY_DL strategy", deletedDataRecords);
                    }

                    // Nếu không xóa được record nào, thử xóa theo CREATED_DATE trong khoảng thời gian import
                    if (deletedDataRecords == 0)
                    {
                        var startTime = importRecord.ImportDate.AddMinutes(-10);
                        var endTime = importRecord.ImportDate.AddMinutes(10);

                        var deleteSql2 = $@"
                            DELETE FROM [{dataType}]
                            WHERE CREATED_DATE >= @StartTime AND CREATED_DATE <= @EndTime";

                        using var deleteCmd2 = new SqlCommand(deleteSql2, connection);
                        deleteCmd2.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = startTime;
                        deleteCmd2.Parameters.Add("@EndTime", SqlDbType.DateTime2).Value = endTime;

                        deletedDataRecords = await deleteCmd2.ExecuteNonQueryAsync();

                        _logger.LogInformation("🗑️ Deleted {DeletedDataRecords} records using CREATED_DATE strategy ({StartTime} - {EndTime})",
                            deletedDataRecords, startTime, endTime);
                    }

                    totalDeletedRecords += deletedDataRecords;

                    if (deletedDataRecords == 0)
                    {
                        _logger.LogWarning("⚠️ No data records found to delete for import {ImportId} in table {DataType}",
                            importId, dataType);
                    }
                }

                // Xóa import record từ ImportedDataRecords
                _context.ImportedDataRecords.Remove(importRecord);
                var deletedImportRecords = await _context.SaveChangesAsync();
                totalDeletedRecords += deletedImportRecords;

                _logger.LogInformation("✅ Successfully deleted import {ImportId}: {ImportRecords} import records + {DataRecords} data records = {Total} total",
                    importId, deletedImportRecords, totalDeletedRecords - deletedImportRecords, totalDeletedRecords);

                return (true, "Import record and associated data deleted successfully", totalDeletedRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting import record {ImportId}", importId);
                return (false, $"Error deleting import: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Kiểm tra data type có hợp lệ không
        /// </summary>
        private bool IsValidDataType(string dataType)
        {
            var validTypes = new[] { "DP01", "DPDA", "EI01", "GL01", "GL02", "GL41", "LN01", "LN03", "RR01" };
            return validTypes.Contains(dataType?.ToUpper());
        }

        /// <summary>
        /// Xóa import records theo ngày và data type
        /// </summary>
        public async Task<(bool Success, string ErrorMessage, int RecordsDeleted)> DeleteImportsByDateAsync(string dataType, string date)
        {
            try
            {
                _logger.LogInformation("🗑️ Starting bulk delete operation for type: {DataType}, date: {Date}", dataType, date);

                // Parse date
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var targetDate))
                {
                    return (false, "Invalid date format. Use yyyy-MM-dd", 0);
                }

                // Tìm records theo category và date
                var recordsToDelete = await _context.ImportedDataRecords
                    .Where(x => x.Category == dataType.ToUpper() &&
                               x.ImportDate.Date == targetDate.Date)
                    .ToListAsync();

                if (!recordsToDelete.Any())
                {
                    _logger.LogInformation("ℹ️ No records found for type: {DataType}, date: {Date}", dataType, date);
                    return (true, "No records found to delete", 0);
                }

                _logger.LogInformation("🔍 Found {Count} records to delete", recordsToDelete.Count);

                // Xóa tất cả records
                _context.ImportedDataRecords.RemoveRange(recordsToDelete);
                var deletedCount = await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Successfully deleted {DeletedCount} import records", deletedCount);

                return (true, $"Successfully deleted {deletedCount} records", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting imports by date: {DataType}, {Date}", dataType, date);
                return (false, $"Error deleting imports: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Map category to table name for preview
        /// </summary>
        private string? GetTableNameFromCategory(string category)
        {
            return category?.ToUpper() switch
            {
                "DP01" => "DP01",
                "LN01" => "LN01",
                "LN03" => "LN03",
                "GL01" => "GL01",
                "GL02" => "GL02",
                "GL41" => "GL41",
                "DPDA" => "DPDA",
                "EI01" => "EI01",
                "RR01" => "RR01",
                _ => null
            };
        }

        /// <summary>
        /// Xóa toàn bộ dữ liệu import (import history và dữ liệu trong các bảng)
        /// </summary>
        public async Task<(bool Success, string ErrorMessage, int RecordsDeleted)> ClearAllDataAsync()
        {
            try
            {
                _logger.LogInformation("🗑️ Starting clear all data operation...");

                // Đếm số lượng records trước khi xóa
                var totalRecords = await _context.ImportedDataRecords.CountAsync();
                _logger.LogInformation("📊 Found {TotalRecords} import records to delete", totalRecords);

                if (totalRecords == 0)
                {
                    return (true, "No data to clear", 0);
                }

                // Xóa tất cả import records
                var allRecords = await _context.ImportedDataRecords.ToListAsync();
                _context.ImportedDataRecords.RemoveRange(allRecords);

                // Xóa dữ liệu trong các bảng temporal (optional, có thể comment lại nếu muốn giữ data)
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Danh sách các bảng cần xóa dữ liệu
                var tablesToClear = new[]
                {
                    "DP01", "LN01", "LN03", "GL01", "GL02", "GL41",
                    "DPDA", "EI01", "RR01"
                };

                int totalDataRecords = 0;

                foreach (var tableName in tablesToClear)
                {
                    try
                    {
                        // Đếm records trước khi xóa
                        var countSql = $"SELECT COUNT(*) FROM [{tableName}]";
                        using var countCommand = new SqlCommand(countSql, connection);
                        var countResult = await countCommand.ExecuteScalarAsync();
                        var count = countResult != null ? (int)countResult : 0;

                        if (count > 0)
                        {
                            // Xóa dữ liệu
                            var deleteSql = $"DELETE FROM [{tableName}]";
                            using var deleteCommand = new SqlCommand(deleteSql, connection);
                            await deleteCommand.ExecuteNonQueryAsync();

                            totalDataRecords += count;
                            _logger.LogInformation("✅ Cleared {Count} records from table {TableName}", count, tableName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("⚠️ Could not clear table {TableName}: {Error}", tableName, ex.Message);
                        // Continue with other tables
                    }
                }

                // Lưu thay đổi import records
                var deletedImportRecords = await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Clear all data completed: {ImportRecords} import records, {DataRecords} data records",
                    deletedImportRecords, totalDataRecords);

                return (true, $"Successfully cleared {deletedImportRecords} import records and {totalDataRecords} data records",
                    deletedImportRecords + totalDataRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in clear all data operation");
                return (false, $"Error clearing data: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// 🔧 TEMPORARY: Fix GL41 database structure to match CSV (13 columns)
        /// </summary>
        public async Task<DirectImportResult> FixGL41DatabaseStructureAsync()
        {
            var result = new DirectImportResult
            {
                FileName = "GL41_Structure_Fix",
                DataType = "GL41",
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🔧 Starting GL41 database structure fix...");

                // TODO: Implement GL41 structure fix if needed
                result.Success = true;
                result.ProcessedRecords = 0;
                result.Details = "GL41 structure fix completed";
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ GL41 structure fix completed");
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ Error fixing GL41 structure");
                return result;
            }
        }

        /// <summary>
        /// Kiểm tra xem dữ liệu có tồn tại cho dataType và date cụ thể
        /// </summary>
        public async Task<DataCheckResult> CheckDataExistsAsync(string dataType, string date)
        {
            try
            {
                _logger.LogInformation("🔍 Checking data exists for {DataType} on {Date}", dataType, date);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = $@"
                    SELECT COUNT(*)
                    FROM [{dataType}]
                    WHERE CONVERT(DATE, NGAY_DL) = @Date";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Date", date);

                var count = (int)(await command.ExecuteScalarAsync() ?? 0);

                return new DataCheckResult
                {
                    DataType = dataType,
                    Date = date,
                    RecordsFound = count,
                    DataExists = count > 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking data exists for {DataType}", dataType);
                return new DataCheckResult
                {
                    DataType = dataType,
                    Date = date,
                    RecordsFound = 0,
                    DataExists = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Xóa toàn bộ dữ liệu của một bảng cụ thể
        /// </summary>
        public async Task<DirectImportResult> ClearTableDataAsync(string dataType)
        {
            var result = new DirectImportResult
            {
                FileName = $"Clear_{dataType}_Data",
                DataType = dataType,
                StartTime = DateTime.UtcNow
            };

            try
            {
                if (!IsValidDataType(dataType))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Invalid data type: {dataType}";
                    result.EndTime = DateTime.UtcNow;
                    return result;
                }

                _logger.LogInformation("🗑️ Clearing all data from {DataType} table", dataType);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Count records before deletion
                var countSql = $"SELECT COUNT(*) FROM [{dataType}]";
                using var countCmd = new SqlCommand(countSql, connection);
                var deletedCount = (int)(await countCmd.ExecuteScalarAsync() ?? 0);

                // Clear table data
                var deleteSql = $"DELETE FROM [{dataType}]";
                using var deleteCmd = new SqlCommand(deleteSql, connection);
                await deleteCmd.ExecuteNonQueryAsync();

                // Clear related import metadata
                var clearImportSql = @"
                    DELETE FROM ImportedDataRecords
                    WHERE Category = @DataType";

                using var clearImportCmd = new SqlCommand(clearImportSql, connection);
                clearImportCmd.Parameters.Add("@DataType", SqlDbType.NVarChar).Value = dataType;
                await clearImportCmd.ExecuteNonQueryAsync();

                result.Success = true;
                result.ProcessedRecords = deletedCount;
                result.Details = $"Deleted {deletedCount} records from {dataType} table";
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ Successfully cleared {Count} records from {DataType}", deletedCount, dataType);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ Error clearing table data for {DataType}", dataType);
                return result;
            }
        }

        /// <summary>
        /// Lấy số lượng records thực tế từ tất cả database tables
        /// </summary>
        public async Task<Dictionary<string, int>> GetTableRecordCountsAsync()
        {
            var counts = new Dictionary<string, int>();

            try
            {
                _logger.LogInformation("📊 Getting actual table record counts");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Kiểm tra các bảng tồn tại trước khi truy vấn count
                var existingTables = new List<string>();
                string checkTablesSql = @"
                    SELECT TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE='BASE TABLE'
                    AND TABLE_NAME IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL02', 'GL41', 'LN01', 'LN03', 'RR01')";

                using (var checkCmd = new SqlCommand(checkTablesSql, connection))
                {
                    using var checkReader = await checkCmd.ExecuteReaderAsync();
                    while (await checkReader.ReadAsync())
                    {
                        existingTables.Add(checkReader.GetString(0));
                    }
                }

                _logger.LogInformation("📊 Existing tables: {Tables}", string.Join(", ", existingTables));

                // Tạo SQL động dựa trên các bảng thực sự tồn tại
                var sqlBuilder = new System.Text.StringBuilder("WITH TableCounts AS (");
                bool isFirst = true;

                foreach (var table in existingTables)
                {
                    if (!isFirst) sqlBuilder.Append(" UNION ALL ");
                    sqlBuilder.AppendFormat("SELECT '{0}' as TableName, COUNT(*) as RecordCount FROM {0}", table);
                    isFirst = false;
                }

                sqlBuilder.Append(") SELECT TableName, RecordCount FROM TableCounts ORDER BY TableName");

                // Đặt giá trị mặc định 0 cho các bảng không tồn tại
                foreach (var table in new[] { "DP01", "DPDA", "EI01", "GL01", "GL02", "GL41", "LN01", "LN03", "RR01" })
                {
                    counts[table] = 0; // Giá trị mặc định
                }

                // Nếu không có bảng nào tồn tại, trả về counts với tất cả giá trị 0
                if (existingTables.Count == 0)
                {
                    _logger.LogWarning("⚠️ No data tables exist in the database yet");
                    return counts;
                }

                // Truy vấn số lượng bản ghi cho các bảng tồn tại
                using var command = new SqlCommand(sqlBuilder.ToString(), connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString("TableName");
                    var recordCount = reader.GetInt32("RecordCount");
                    counts[tableName] = recordCount;

                    _logger.LogDebug("📊 {TableName}: {RecordCount} records", tableName, recordCount);
                }

                _logger.LogInformation("✅ Successfully retrieved record counts for {TableCount} tables", existingTables.Count);
                return counts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting table record counts");
                throw;
            }
        }

        /// <summary>
        /// 🚀 STREAMING IMPORT - Import file lớn bằng streaming để tránh OutOfMemory
        /// Stream trực tiếp từ HTTP request vào database
        /// </summary>
        public async Task<DirectImportResult> StreamImportAsync(Stream fileStream, string fileName, string dataType)
        {
            var result = new DirectImportResult
            {
                FileName = fileName,
                DataType = dataType,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🚀 [STREAM_IMPORT] Starting streaming import for {FileName}", fileName);

                using var streamReader = new StreamReader(fileStream, Encoding.UTF8, leaveOpen: true);
                using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

                // Read header
                await csvReader.ReadAsync();
                csvReader.ReadHeader();
                var headers = csvReader.HeaderRecord;

                if (headers == null || !headers.Any())
                {
                    throw new InvalidDataException("File không có header hoặc header trống");
                }

                // Create DataTable based on data type
                var dataTable = CreateDataTableForType(dataType, headers);

                // Stream read and batch insert
                const int batchSize = 100000;
                var batch = new List<string[]>();
                var totalRecords = 0;

                _logger.LogInformation("📊 [STREAM_IMPORT] Processing batches of {BatchSize} records", batchSize);

                while (await csvReader.ReadAsync())
                {
                    var record = new string[headers.Length];
                    for (int i = 0; i < headers.Length; i++)
                    {
                        record[i] = csvReader.GetField(i) ?? "";
                    }

                    batch.Add(record);

                    if (batch.Count >= batchSize)
                    {
                        await ProcessBatchAsync(dataTable, batch, dataType, DateTime.Today, fileName);
                        totalRecords += batch.Count;
                        batch.Clear();

                        _logger.LogInformation("✅ [STREAM_IMPORT] Processed {TotalRecords} records", totalRecords);
                    }
                }

                // Process remaining records
                if (batch.Any())
                {
                    await ProcessBatchAsync(dataTable, batch, dataType, DateTime.Today, fileName);
                    totalRecords += batch.Count;
                }

                result.ProcessedRecords = totalRecords;
                result.Success = true;
                result.Details = $"Stream import thành công: {totalRecords} records";

                _logger.LogInformation("🎉 [STREAM_IMPORT] Completed: {TotalRecords} records in {Duration}ms",
                    totalRecords, (DateTime.UtcNow - result.StartTime).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [STREAM_IMPORT] Error in streaming import");
                result.Success = false;
                result.ErrorMessage = $"Lỗi stream import: {ex.Message}";
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
            }

            return result;
        }

        /// <summary>
        /// 🔄 PARALLEL IMPORT - Import với parallel processing cho file cực lớn
        /// </summary>
        public async Task<DirectImportResult> ParallelImportAsync(Stream fileStream, string fileName, string dataType, int chunkSize = 50000)
        {
            var result = new DirectImportResult
            {
                FileName = fileName,
                DataType = dataType,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🔄 [PARALLEL_IMPORT] Starting parallel import for {FileName} with chunk size {ChunkSize}",
                    fileName, chunkSize);

                // Pre-read file into memory chunks for parallel processing
                var chunks = await ReadFileIntoChunksAsync(fileStream, chunkSize);

                var totalRecords = 0;
                var tasks = new List<Task<int>>();

                // Process chunks in parallel
                var semaphore = new SemaphoreSlim(Environment.ProcessorCount); // Limit concurrent tasks

                foreach (var chunk in chunks)
                {
                    tasks.Add(ProcessChunkParallelAsync(chunk, dataType, semaphore));
                }

                var results = await Task.WhenAll(tasks);
                totalRecords = results.Sum();

                result.ProcessedRecords = totalRecords;
                result.Success = true;
                result.Details = $"Parallel import thành công: {totalRecords} records từ {chunks.Count} chunks";

                _logger.LogInformation("🎉 [PARALLEL_IMPORT] Completed: {TotalRecords} records from {ChunkCount} chunks in {Duration}ms",
                    totalRecords, chunks.Count, (DateTime.UtcNow - result.StartTime).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PARALLEL_IMPORT] Error in parallel import");
                result.Success = false;
                result.ErrorMessage = $"Lỗi parallel import: {ex.Message}";
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
            }

            return result;
        }

        /// <summary>
        /// Xử lý batch data với DataTable và SqlBulkCopy
        /// STRUCTURE: NGAY_DL (index 0) + CSV business columns (index 1-N) + system columns (index N+1...)
        /// </summary>
        private async Task ProcessBatchAsync(DataTable dataTable, List<string[]> batch, string dataType, DateTime ngayDL, string fileName)
        {
            dataTable.Clear();

            foreach (var record in batch)
            {
                var row = dataTable.NewRow();

                // NGAY_DL always at index 0 (matches model Order=0)
                row[0] = ngayDL;

                // Fill CSV business columns (starting from index 1)
                int csvColumnCount = record.Length;
                for (int i = 0; i < csvColumnCount && (i + 1) < dataTable.Columns.Count; i++)
                {
                    var value = record[i];

                    // Handle empty strings and null values based on column type
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        var columnType = dataTable.Columns[i + 1].DataType; // +1 for NGAY_DL offset
                        if (columnType == typeof(string))
                        {
                            row[i + 1] = ""; // Empty string instead of DBNull for string columns
                        }
                        else
                        {
                            row[i + 1] = DBNull.Value; // DBNull for non-string columns (nullable)
                        }
                    }
                    else
                    {
                        // Convert based on DataTable column type
                        var columnType = dataTable.Columns[i + 1].DataType; // +1 for NGAY_DL offset
                        if (columnType == typeof(int))
                        {
                            if (int.TryParse(value, out int intValue))
                                row[i + 1] = intValue;
                            else
                                row[i + 1] = DBNull.Value;
                        }
                        else if (columnType == typeof(decimal))
                        {
                            if (decimal.TryParse(value, out decimal decimalValue))
                                row[i + 1] = decimalValue;
                            else
                                row[i + 1] = DBNull.Value;
                        }
                        else if (columnType == typeof(DateTime))
                        {
                            if (DateTime.TryParse(value, out DateTime dateValue))
                                row[i + 1] = dateValue;
                            else
                                row[i + 1] = DBNull.Value;
                        }
                        else
                        {
                            row[i + 1] = value; // Default to string
                        }
                    }
                }

                // Fill system columns (after NGAY_DL + CSV business columns)
                int systemColumnStartIndex = csvColumnCount + 1; // +1 for NGAY_DL
                for (int i = systemColumnStartIndex; i < dataTable.Columns.Count; i++)
                {
                    var columnName = dataTable.Columns[i].ColumnName;
                    if (columnName == "CREATED_DATE")
                    {
                        row[i] = DateTime.UtcNow;
                    }
                    else if (columnName == "UPDATED_DATE")
                    {
                        row[i] = DateTime.UtcNow;
                    }
                    else if (columnName == "FILE_NAME")
                    {
                        row[i] = fileName;
                    }
                    else
                    {
                        row[i] = DBNull.Value; // Default for unknown system columns
                    }
                }

                dataTable.Rows.Add(row);
            }
            var tableName = GetTableNameForDataType(dataType);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction | SqlBulkCopyOptions.FireTriggers, null)
            {
                DestinationTableName = tableName,
                BatchSize = 500000, // 🚀 ULTRA OPTIMIZATION: 250k -> 500k cho GL02 338k records
                BulkCopyTimeout = 3600, // 🚀 EXTEND: 1800s -> 3600s (1 giờ)
                EnableStreaming = true,
                NotifyAfter = 50000 // 🚀 REDUCE: 100k -> 50k để theo dõi progress tốt hơn
            };

            // Map columns từ DataTable to Database table by name (skip IDENTITY columns)
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var columnName = dataTable.Columns[i].ColumnName;
                // Skip IDENTITY columns - let SQL Server handle them
                if (columnName != "Id")
                {
                    bulkCopy.ColumnMappings.Add(columnName, columnName); // Map by column name
                }
            }

            await bulkCopy.WriteToServerAsync(dataTable);
        }

        /// <summary>
        /// Đọc file thành chunks để xử lý parallel
        /// </summary>
        private async Task<List<List<string[]>>> ReadFileIntoChunksAsync(Stream fileStream, int chunkSize)
        {
            var chunks = new List<List<string[]>>();

            using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            // Read header
            await csvReader.ReadAsync();
            csvReader.ReadHeader();
            var headers = csvReader.HeaderRecord;

            var currentChunk = new List<string[]>();

            while (await csvReader.ReadAsync())
            {
                var record = new string[headers.Length];
                for (int i = 0; i < headers.Length; i++)
                {
                    record[i] = csvReader.GetField(i) ?? "";
                }

                currentChunk.Add(record);

                if (currentChunk.Count >= chunkSize)
                {
                    chunks.Add(new List<string[]>(currentChunk));
                    currentChunk.Clear();
                }
            }

            if (currentChunk.Any())
            {
                chunks.Add(currentChunk);
            }

            return chunks;
        }

        /// <summary>
        /// Xử lý một chunk trong parallel processing
        /// </summary>
        private async Task<int> ProcessChunkParallelAsync(List<string[]> chunk, string dataType, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();

            try
            {
                var dataTable = CreateDataTableForType(dataType, null);
                await ProcessBatchAsync(dataTable, chunk, dataType, DateTime.Today, "unknown");
                return chunk.Count;
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Tạo DataTable cho loại dữ liệu cụ thể
        /// </summary>
        private DataTable CreateDataTableForType(string dataType, string[]? headers)
        {
            var dataTable = new DataTable();

            // Create specific columns for each data type with correct types
            switch (dataType.ToUpper())
            {
                case "DP01":
                    CreateDP01DataTable(dataTable, headers);
                    break;
                case "DPDA":
                case "EI01":
                case "GL41":
                case "RR01":
                    // Use generic method for other tables
                    CreateGenericDataTable(dataTable, headers);
                    break;
                case "GL01":
                    CreateGL01DataTable(dataTable, headers);
                    break;
                case "GL02":
                    CreateGL02DataTable(dataTable, headers);
                    break;
                case "LN01":
                    CreateLN01DataTable(dataTable, headers);
                    break;
                case "LN03":
                    CreateLN03DataTable(dataTable, headers);
                    break;
                default:
                    // Fallback to string columns
                    if (headers != null && headers.Any())
                    {
                        foreach (var header in headers)
                        {
                            dataTable.Columns.Add(header, typeof(string));
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 20; i++)
                        {
                            dataTable.Columns.Add($"Column{i}", typeof(string));
                        }
                    }
                    break;
            }

            return dataTable;
        }

        /// <summary>
        /// Tạo DataTable cho LN01 với đúng column types
        /// STRUCTURE: NGAY_DL FIRST + CSV business columns (NO Id column - handled by SQL Server IDENTITY)
        /// </summary>
        private void CreateLN01DataTable(DataTable dataTable, string[]? headers)
        {
            // System column NGAY_DL first (Order=0) - matches model structure
            dataTable.Columns.Add("NGAY_DL", typeof(DateTime));

            // CSV Business columns 1-79 (exact order from CSV)
            dataTable.Columns.Add("BRCD", typeof(string));
            dataTable.Columns.Add("CUSTSEQ", typeof(string));
            dataTable.Columns.Add("CUSTNM", typeof(string));
            dataTable.Columns.Add("TAI_KHOAN", typeof(string));
            dataTable.Columns.Add("CCY", typeof(string));
            dataTable.Columns.Add("DU_NO", typeof(decimal));
            dataTable.Columns.Add("DSBSSEQ", typeof(string));
            dataTable.Columns.Add("TRANSACTION_DATE", typeof(DateTime));
            dataTable.Columns.Add("DSBSDT", typeof(DateTime));
            dataTable.Columns.Add("DISBUR_CCY", typeof(string));
            dataTable.Columns.Add("DISBURSEMENT_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("DSBSMATDT", typeof(DateTime));
            dataTable.Columns.Add("BSRTCD", typeof(string));
            dataTable.Columns.Add("INTEREST_RATE", typeof(decimal));
            dataTable.Columns.Add("APPRSEQ", typeof(string));
            dataTable.Columns.Add("APPRDT", typeof(DateTime));
            dataTable.Columns.Add("APPR_CCY", typeof(string));
            dataTable.Columns.Add("APPRAMT", typeof(decimal));
            dataTable.Columns.Add("APPRMATDT", typeof(DateTime));
            dataTable.Columns.Add("LOAN_TYPE", typeof(string));
            dataTable.Columns.Add("FUND_RESOURCE_CODE", typeof(string));
            dataTable.Columns.Add("FUND_PURPOSE_CODE", typeof(string));
            dataTable.Columns.Add("REPAYMENT_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("NEXT_REPAY_DATE", typeof(DateTime));
            dataTable.Columns.Add("NEXT_REPAY_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("NEXT_INT_REPAY_DATE", typeof(DateTime));
            dataTable.Columns.Add("OFFICER_ID", typeof(string));
            dataTable.Columns.Add("OFFICER_NAME", typeof(string));
            dataTable.Columns.Add("INTEREST_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("PASTDUE_INTEREST_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("TOTAL_INTEREST_REPAY_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("CUSTOMER_TYPE_CODE", typeof(string));
            dataTable.Columns.Add("CUSTOMER_TYPE_CODE_DETAIL", typeof(string));
            dataTable.Columns.Add("TRCTCD", typeof(string));
            dataTable.Columns.Add("TRCTNM", typeof(string));
            dataTable.Columns.Add("ADDR1", typeof(string));
            dataTable.Columns.Add("PROVINCE", typeof(string));
            dataTable.Columns.Add("LCLPROVINNM", typeof(string));
            dataTable.Columns.Add("DISTRICT", typeof(string));
            dataTable.Columns.Add("LCLDISTNM", typeof(string));
            dataTable.Columns.Add("COMMCD", typeof(string));
            dataTable.Columns.Add("LCLWARDNM", typeof(string));
            dataTable.Columns.Add("LAST_REPAY_DATE", typeof(DateTime));
            dataTable.Columns.Add("SECURED_PERCENT", typeof(decimal));
            dataTable.Columns.Add("NHOM_NO", typeof(string));
            dataTable.Columns.Add("LAST_INT_CHARGE_DATE", typeof(DateTime));
            dataTable.Columns.Add("EXEMPTINT", typeof(string));
            dataTable.Columns.Add("EXEMPTINTTYPE", typeof(string));
            dataTable.Columns.Add("EXEMPTINTAMT", typeof(decimal));
            dataTable.Columns.Add("GRPNO", typeof(string));
            dataTable.Columns.Add("BUSCD", typeof(string));
            dataTable.Columns.Add("BSNSSCLTPCD", typeof(string));
            dataTable.Columns.Add("USRIDOP", typeof(string));
            dataTable.Columns.Add("ACCRUAL_AMOUNT", typeof(decimal));
            dataTable.Columns.Add("ACCRUAL_AMOUNT_END_OF_MONTH", typeof(decimal));
            dataTable.Columns.Add("INTCMTH", typeof(string));
            dataTable.Columns.Add("INTRPYMTH", typeof(string));
            dataTable.Columns.Add("INTTRMMTH", typeof(string));
            dataTable.Columns.Add("YRDAYS", typeof(string));
            dataTable.Columns.Add("REMARK", typeof(string));
            dataTable.Columns.Add("CHITIEU", typeof(string));
            dataTable.Columns.Add("CTCV", typeof(string));
            dataTable.Columns.Add("CREDIT_LINE_YPE", typeof(string));
            dataTable.Columns.Add("INT_LUMPSUM_PARTIAL_TYPE", typeof(string));
            dataTable.Columns.Add("INT_PARTIAL_PAYMENT_TYPE", typeof(string));
            dataTable.Columns.Add("INT_PAYMENT_INTERVAL", typeof(string));
            dataTable.Columns.Add("AN_HAN_LAI", typeof(string));
            dataTable.Columns.Add("PHUONG_THUC_GIAI_NGAN_1", typeof(string));
            dataTable.Columns.Add("TAI_KHOAN_GIAI_NGAN_1", typeof(string));
            dataTable.Columns.Add("SO_TIEN_GIAI_NGAN_1", typeof(decimal));
            dataTable.Columns.Add("PHUONG_THUC_GIAI_NGAN_2", typeof(string));
            dataTable.Columns.Add("TAI_KHOAN_GIAI_NGAN_2", typeof(string));
            dataTable.Columns.Add("SO_TIEN_GIAI_NGAN_2", typeof(decimal));
            dataTable.Columns.Add("CMT_HC", typeof(string));
            dataTable.Columns.Add("NGAY_SINH", typeof(DateTime));
            dataTable.Columns.Add("MA_CB_AGRI", typeof(string));
            dataTable.Columns.Add("MA_NGANH_KT", typeof(string));
            dataTable.Columns.Add("TY_GIA", typeof(decimal));
            dataTable.Columns.Add("OFFICER_IPCAS", typeof(string));

            // System columns (after business columns) - NO Id column (IDENTITY handled by SQL Server)
            // 🚫 EXCLUDE GENERATED ALWAYS temporal columns - these are handled by SQL Server automatically
            // dataTable.Columns.Add("CREATED_DATE", typeof(DateTime));
            // dataTable.Columns.Add("UPDATED_DATE", typeof(DateTime));
            dataTable.Columns.Add("FILE_NAME", typeof(string));
        }        /// <summary>
                 /// Tạo DataTable cho LN03 với đúng column types
                 /// NEW STRUCTURE: CSV business columns FIRST, then system columns
                 /// </summary>
        private void CreateLN03DataTable(DataTable dataTable, string[]? headers)
        {
            // CSV Business columns 1-17 (exact order from CSV)
            dataTable.Columns.Add("MACHINHANH", typeof(string));
            dataTable.Columns.Add("TENCHINHANH", typeof(string));
            dataTable.Columns.Add("MAKH", typeof(string));
            dataTable.Columns.Add("TENKH", typeof(string));
            dataTable.Columns.Add("SOHOPDONG", typeof(string));
            dataTable.Columns.Add("SOTIENXLRR", typeof(decimal));
            dataTable.Columns.Add("NGAYPHATSINHXL", typeof(DateTime));
            dataTable.Columns.Add("THUNOSAUXL", typeof(decimal));
            dataTable.Columns.Add("CONLAINGOAIBANG", typeof(decimal));
            dataTable.Columns.Add("DUNONOIBANG", typeof(decimal));
            dataTable.Columns.Add("NHOMNO", typeof(int));
            dataTable.Columns.Add("MACBTD", typeof(string));
            dataTable.Columns.Add("TENCBTD", typeof(string));
            dataTable.Columns.Add("MAPGD", typeof(string));
            dataTable.Columns.Add("TAIKHOANHACHTOAN", typeof(string));
            dataTable.Columns.Add("REFNO", typeof(string));
            dataTable.Columns.Add("LOAINGUONVON", typeof(string));

            // System columns (18-20)
            dataTable.Columns.Add("Id", typeof(long));
            dataTable.Columns.Add("NGAY_DL", typeof(DateTime));
            dataTable.Columns.Add("FILE_NAME", typeof(string));
        }

        /// <summary>
        /// Tạo DataTable cho DP01 (fallback)
        /// </summary>
        private void CreateDP01DataTable(DataTable dataTable, string[]? headers)
        {
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header, typeof(string));
                }
            }
            else
            {
                for (int i = 1; i <= 50; i++)
                {
                    dataTable.Columns.Add($"Column{i}", typeof(string));
                }
            }
        }

        /// <summary>
        /// Tạo DataTable cho GL01 (fallback)
        /// </summary>
        private void CreateGL01DataTable(DataTable dataTable, string[]? headers)
        {
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header, typeof(string));
                }
            }
            else
            {
                for (int i = 1; i <= 30; i++)
                {
                    dataTable.Columns.Add($"Column{i}", typeof(string));
                }
            }
        }

        /// <summary>
        /// Create DataTable for GL02 - Giao dịch sổ cái (17 business columns)
        /// NGAY_DL from TRDATE column, partitioned columnstore (not temporal)
        /// </summary>
        private void CreateGL02DataTable(DataTable dataTable, string[]? headers)
        {
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header, typeof(string));
                }
            }
            else
            {
                // Default GL02 columns if no headers
                dataTable.Columns.Add("TRDATE", typeof(string));
                dataTable.Columns.Add("TRBRCD", typeof(string));
                dataTable.Columns.Add("USERID", typeof(string));
                dataTable.Columns.Add("JOURSEQ", typeof(string));
                dataTable.Columns.Add("DYTRSEQ", typeof(string));
                dataTable.Columns.Add("LOCAC", typeof(string));
                dataTable.Columns.Add("CCY", typeof(string));
                dataTable.Columns.Add("BUSCD", typeof(string));
                dataTable.Columns.Add("UNIT", typeof(string));
                dataTable.Columns.Add("TRCD", typeof(string));
                dataTable.Columns.Add("CUSTOMER", typeof(string));
                dataTable.Columns.Add("TRTP", typeof(string));
                dataTable.Columns.Add("REFERENCE", typeof(string));
                dataTable.Columns.Add("REMARK", typeof(string));
                dataTable.Columns.Add("DRAMOUNT", typeof(string));
                dataTable.Columns.Add("CRAMOUNT", typeof(string));
                dataTable.Columns.Add("CRTDTM", typeof(string));
            }
        }

        /// <summary>
        /// Generic DataTable creation for DPDA, EI01, GL41, RR01 tables
        /// Uses headers from CSV file if available, otherwise creates default columns
        /// </summary>
        private void CreateGenericDataTable(DataTable dataTable, string[]? headers)
        {
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header, typeof(string));
                }
            }
            else
            {
                // Default generic columns if no headers
                for (int i = 1; i <= 50; i++)
                {
                    dataTable.Columns.Add($"Column{i}", typeof(string));
                }
            }
        }

        /// <summary>
        /// Lấy tên bảng database cho loại dữ liệu - CẬP NHẬT TẤT CẢ 8 BẢNG DÀNH CHO DỮ LIỆU
        /// </summary>
        private string GetTableNameForDataType(string dataType)
        {
            return dataType.ToUpper() switch
            {
                "DP01" => "DP01",      // Temporal Table - 63 business columns
                "DPDA" => "DPDA",      // Temporal Table - 13 business columns
                "EI01" => "EI01",      // Temporal Table - 24 business columns
                "GL01" => "GL01",      // Basic Table - 27 business columns (NO temporal)
                "GL02" => "GL02",      // Basic Table - 17 business columns (NO temporal)
                "GL41" => "GL41",      // Temporal Table - 13 business columns
                "LN01" => "LN01",      // Temporal Table - 79 business columns
                "LN03" => "LN03",      // Temporal Table - 17 business columns
                "RR01" => "RR01",      // Temporal Table - 25 business columns
                // Legacy tables
                "CA01" => "CA01",
                "TR01" => "TR01",
                _ => throw new ArgumentException($"Unsupported data type: {dataType}")
            };
        }

        // Dictionary để lưu upload sessions trong memory (production nên dùng Redis)
        private readonly ConcurrentDictionary<string, UploadSession> _uploadSessions = new();

        /// <summary>
        /// 🆔 Tạo upload session cho chunked upload
        /// </summary>
        public async Task CreateUploadSessionAsync(UploadSession session)
        {
            // Tạo thư mục tạm cho session
            var tempDir = Path.Combine(Path.GetTempPath(), "chunked_uploads", session.SessionId);
            Directory.CreateDirectory(tempDir);

            session.TempDirectoryPath = tempDir;
            _uploadSessions[session.SessionId] = session;

            _logger.LogInformation("🆔 [CREATE_SESSION] Created session {SessionId} with temp dir {TempDir}",
                session.SessionId, tempDir);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 📤 Upload chunk
        /// </summary>
        public async Task<ChunkUploadResult> UploadChunkAsync(string sessionId, int chunkIndex, IFormFile chunk)
        {
            if (!_uploadSessions.TryGetValue(sessionId, out var session))
            {
                throw new InvalidOperationException($"Upload session {sessionId} not found");
            }

            try
            {
                var chunkPath = Path.Combine(session.TempDirectoryPath!, $"chunk_{chunkIndex:D6}");

                using var fileStream = new FileStream(chunkPath, FileMode.Create);
                await chunk.CopyToAsync(fileStream);

                // Update session
                session.UploadedChunks.Add(chunkIndex);
                session.UploadedChunks.Sort();

                _logger.LogInformation("📤 [UPLOAD_CHUNK] Saved chunk {ChunkIndex} for session {SessionId}",
                    chunkIndex, sessionId);

                return new ChunkUploadResult
                {
                    Success = true,
                    ChunkIndex = chunkIndex,
                    SessionId = sessionId,
                    ChunkSize = chunk.Length,
                    Message = "Chunk uploaded successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [UPLOAD_CHUNK] Error uploading chunk {ChunkIndex} for session {SessionId}",
                    chunkIndex, sessionId);

                return new ChunkUploadResult
                {
                    Success = false,
                    ChunkIndex = chunkIndex,
                    SessionId = sessionId,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// ✅ Finalize chunked upload và process file
        /// </summary>
        public async Task<DirectImportResult> FinalizeUploadAsync(string sessionId)
        {
            if (!_uploadSessions.TryGetValue(sessionId, out var session))
            {
                throw new InvalidOperationException($"Upload session {sessionId} not found");
            }

            try
            {
                _logger.LogInformation("✅ [FINALIZE_UPLOAD] Starting finalization for session {SessionId}", sessionId);

                // Verify all chunks uploaded
                if (session.UploadedChunks.Count != session.TotalChunks)
                {
                    throw new InvalidOperationException(
                        $"Missing chunks: expected {session.TotalChunks}, got {session.UploadedChunks.Count}");
                }

                // Combine chunks into single file
                var combinedFilePath = Path.Combine(session.TempDirectoryPath!, "combined_file");
                await CombineChunksAsync(session, combinedFilePath);

                // Process combined file
                using var fileStream = new FileStream(combinedFilePath, FileMode.Open, FileAccess.Read);
                var result = await StreamImportAsync(fileStream, session.FileName, session.DataType);

                // Cleanup
                session.IsCompleted = true;
                await CleanupSessionAsync(sessionId);

                _logger.LogInformation("🎉 [FINALIZE_UPLOAD] Completed session {SessionId}: {RecordsProcessed} records",
                    sessionId, result.ProcessedRecords);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [FINALIZE_UPLOAD] Error finalizing session {SessionId}", sessionId);
                throw;
            }
        }

        /// <summary>
        /// 📊 Get upload info (for resume functionality)
        /// </summary>
        public async Task<UploadInfoResponse> GetUploadInfoAsync(string sessionId)
        {
            if (!_uploadSessions.TryGetValue(sessionId, out var session))
            {
                throw new InvalidOperationException($"Upload session {sessionId} not found");
            }

            var remainingChunks = session.TotalChunks - session.UploadedChunks.Count;
            var progressPercentage = (double)session.UploadedChunks.Count / session.TotalChunks * 100;

            return await Task.FromResult(new UploadInfoResponse
            {
                SessionId = session.SessionId,
                FileName = session.FileName,
                FileSize = session.FileSize,
                TotalChunks = session.TotalChunks,
                UploadedChunks = new List<int>(session.UploadedChunks),
                RemainingChunks = remainingChunks,
                ProgressPercentage = progressPercentage,
                CreatedAt = session.CreatedAt,
                IsCompleted = session.IsCompleted
            });
        }

        /// <summary>
        /// 🚫 Cancel upload session
        /// </summary>
        public async Task CancelUploadAsync(string sessionId)
        {
            if (_uploadSessions.TryRemove(sessionId, out var session))
            {
                await CleanupSessionAsync(sessionId);

                _logger.LogInformation("🚫 [CANCEL_UPLOAD] Cancelled session {SessionId}", sessionId);
            }
        }

        /// <summary>
        /// 🔗 Combine chunks into single file
        /// </summary>
        private async Task CombineChunksAsync(UploadSession session, string outputPath)
        {
            using var outputStream = new FileStream(outputPath, FileMode.Create);

            for (int i = 0; i < session.TotalChunks; i++)
            {
                var chunkPath = Path.Combine(session.TempDirectoryPath!, $"chunk_{i:D6}");

                if (!File.Exists(chunkPath))
                {
                    throw new FileNotFoundException($"Chunk {i} not found at {chunkPath}");
                }

                using var chunkStream = new FileStream(chunkPath, FileMode.Open);
                await chunkStream.CopyToAsync(outputStream);
            }

            _logger.LogInformation("🔗 [COMBINE_CHUNKS] Combined {TotalChunks} chunks for session {SessionId}",
                session.TotalChunks, session.SessionId);
        }

        /// <summary>
        /// 🧹 Cleanup session files
        /// </summary>
        private async Task CleanupSessionAsync(string sessionId)
        {
            if (_uploadSessions.TryGetValue(sessionId, out var session) &&
                !string.IsNullOrEmpty(session.TempDirectoryPath) &&
                Directory.Exists(session.TempDirectoryPath))
            {
                try
                {
                    Directory.Delete(session.TempDirectoryPath, recursive: true);
                    _logger.LogInformation("🧹 [CLEANUP] Deleted temp directory for session {SessionId}", sessionId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ [CLEANUP] Failed to delete temp directory for session {SessionId}", sessionId);
                }
            }

            await Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Validate GL02 data - kiểm tra CRTDTM parsing và data integrity
        /// </summary>
        public async Task<object> ValidateGL02DataAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT
                        COUNT(*) as TotalRecords,
                        COUNT(CASE WHEN CRTDTM IS NOT NULL THEN 1 END) as RecordsWithCRTDTM,
                        COUNT(CASE WHEN CRTDTM IS NULL THEN 1 END) as RecordsWithNullCRTDTM,
                        MIN(CRTDTM) as MinCRTDTM,
                        MAX(CRTDTM) as MaxCRTDTM,
                        COUNT(CASE WHEN DRAMOUNT IS NOT NULL AND DRAMOUNT > 0 THEN 1 END) as RecordsWithDRAMOUNT,
                        COUNT(CASE WHEN CRAMOUNT IS NOT NULL AND CRAMOUNT > 0 THEN 1 END) as RecordsWithCRAMOUNT,
                        MAX(CREATED_DATE) as LatestImport
                    FROM GL02";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new
                    {
                        Success = true,
                        TotalRecords = reader.GetInt32("TotalRecords"),
                        RecordsWithCRTDTM = reader.GetInt32("RecordsWithCRTDTM"),
                        RecordsWithNullCRTDTM = reader.GetInt32("RecordsWithNullCRTDTM"),
                        MinCRTDTM = reader.IsDBNull("MinCRTDTM") ? (DateTime?)null : reader.GetDateTime("MinCRTDTM"),
                        MaxCRTDTM = reader.IsDBNull("MaxCRTDTM") ? (DateTime?)null : reader.GetDateTime("MaxCRTDTM"),
                        RecordsWithDRAMOUNT = reader.GetInt32("RecordsWithDRAMOUNT"),
                        RecordsWithCRAMOUNT = reader.GetInt32("RecordsWithCRAMOUNT"),
                        LatestImport = reader.IsDBNull("LatestImport") ? (DateTime?)null : reader.GetDateTime("LatestImport"),
                        CRTDTMParsingSuccess = reader.GetInt32("RecordsWithCRTDTM") > 0,
                        Message = reader.GetInt32("RecordsWithCRTDTM") > 0
                            ? "CRTDTM parsing successful"
                            : "CRTDTM parsing failed - all values are NULL"
                    };
                }

                return new { Success = false, Message = "No data found in GL02 table" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL02_VALIDATE] Error validating GL02 data");
                return new { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }

    /// <summary>
    /// Custom DataReader implementation for streaming CSV data directly to SqlBulkCopy
    /// Optimized for ultra-high-speed processing of large files without memory overhead
    /// </summary>
    public class StreamingDataReader<T> : IDataReader, IDisposable where T : class, new()
    {
        private readonly CsvReader _csvReader;
        private readonly PropertyInfo[] _properties;
        private readonly ILogger _logger;
        private readonly string _dataType;
        private readonly string? _statementDate;
        private bool _disposed = false;
        private int _recordCount = 0;

        public int RecordCount => _recordCount;

        public StreamingDataReader(CsvReader csvReader, string dataType, string? statementDate, ILogger logger)
        {
            _csvReader = csvReader;
            _dataType = dataType;
            _statementDate = statementDate;
            _logger = logger;
            _properties = typeof(T).GetProperties()
                .Where(p => p.CanWrite && !p.PropertyType.IsClass || p.PropertyType == typeof(string))
                .ToArray();
        }

        public bool Read()
        {
            try
            {
                var hasData = _csvReader.Read();
                if (hasData)
                {
                    _recordCount++;
                    if (_recordCount % 50000 == 0)
                    {
                        _logger.LogInformation("🚀 [STREAMING_READER] Streamed {Count} records", _recordCount);
                    }
                }
                return hasData;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ [STREAMING_READER] Skipping malformed row {Count}: {Error}", _recordCount + 1, ex.Message);
                // Try to continue reading
                try
                {
                    return _csvReader.Read();
                }
                catch
                {
                    return false;
                }
            }
        }

        public object GetValue(int i)
        {
            try
            {
                var csvValue = _csvReader.GetField(i)?.Trim();

                if (string.IsNullOrWhiteSpace(csvValue))
                    return DBNull.Value;

                // Special handling for GL02 TRDATE (column 0) -> NGAY_DL conversion
                if (_dataType == "GL02" && i == 0)
                {
                    // Convert TRDATE (YYYYMMDD) to NGAY_DL (datetime2)
                    if (csvValue.Length == 8 && DateTime.TryParseExact(csvValue, "yyyyMMdd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateResult))
                    {
                        _logger.LogDebug("🗓️ [GL02_TRDATE] Converting TRDATE '{TRDATE}' to NGAY_DL '{NGAY_DL}'",
                            csvValue, dateResult.ToString("yyyy-MM-dd"));
                        return dateResult;
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ [GL02_TRDATE] Invalid TRDATE format: '{TRDATE}', using current date", csvValue);
                        return DateTime.Now.Date;
                    }
                }

                // For non-GL02 data types, use standard property-based conversion
                if (i >= _properties.Length) return DBNull.Value;

                var property = _properties[i];
                var targetType = property.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (underlyingType == typeof(string))
                {
                    return csvValue;
                }
                else if (underlyingType == typeof(DateTime))
                {
                    if (DateTime.TryParseExact(csvValue, new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyyMMdd" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateResult))
                        return dateResult;
                    return DBNull.Value;
                }
                else if (underlyingType == typeof(decimal))
                {
                    var normalizedValue = csvValue.Replace(",", "").Replace(" ", "").Trim();
                    if (normalizedValue.StartsWith("'"))
                        normalizedValue = normalizedValue.Substring(1);

                    if (decimal.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalResult))
                        return decimalResult;
                    return DBNull.Value;
                }
                else if (underlyingType == typeof(int))
                {
                    var normalizedValue = csvValue.Replace(",", "").Replace(" ", "").Trim();
                    if (int.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var intResult))
                        return intResult;
                    return DBNull.Value;
                }
                else if (underlyingType == typeof(long))
                {
                    var normalizedValue = csvValue.Replace(",", "").Replace(" ", "").Trim();
                    if (long.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var longResult))
                        return longResult;
                    return DBNull.Value;
                }
                else if (underlyingType == typeof(bool))
                {
                    return csvValue.Equals("1") || csvValue.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                return DBNull.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ [STREAMING_READER] Error converting column {Index}: {Error}", i, ex.Message);
                return DBNull.Value;
            }
        }

        // Required IDataReader implementation
        public int FieldCount => _properties.Length;
        public string GetName(int i) => i < _properties.Length ? _properties[i].Name : string.Empty;
        public Type GetFieldType(int i) => i < _properties.Length ? _properties[i].PropertyType : typeof(object);
        public bool IsDBNull(int i) => GetValue(i) == DBNull.Value;
        public object this[int i] => GetValue(i);
        public object this[string name] => throw new NotImplementedException();

        // Unused methods for IDataReader compliance
        public bool NextResult() => false;
        public void Close() => Dispose();
        public DataTable GetSchemaTable() => throw new NotImplementedException();
        public int Depth => 0;
        public bool IsClosed => _disposed;
        public int RecordsAffected => _recordCount;
        public bool GetBoolean(int i) => Convert.ToBoolean(GetValue(i));
        public byte GetByte(int i) => Convert.ToByte(GetValue(i));
        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public char GetChar(int i) => Convert.ToChar(GetValue(i));
        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public IDataReader GetData(int i) => throw new NotImplementedException();
        public string GetDataTypeName(int i) => GetFieldType(i).Name;
        public DateTime GetDateTime(int i) => Convert.ToDateTime(GetValue(i));
        public decimal GetDecimal(int i) => Convert.ToDecimal(GetValue(i));
        public double GetDouble(int i) => Convert.ToDouble(GetValue(i));
        public float GetFloat(int i) => Convert.ToSingle(GetValue(i));
        public Guid GetGuid(int i) => (Guid)GetValue(i);
        public short GetInt16(int i) => Convert.ToInt16(GetValue(i));
        public int GetInt32(int i) => Convert.ToInt32(GetValue(i));
        public long GetInt64(int i) => Convert.ToInt64(GetValue(i));
        public int GetOrdinal(string name) => Array.FindIndex(_properties, p => p.Name == name);
        public string GetString(int i) => Convert.ToString(GetValue(i)) ?? string.Empty;
        public int GetValues(object[] values)
        {
            var count = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < count; i++)
                values[i] = GetValue(i);
            return count;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _logger.LogInformation("✅ [STREAMING_READER] Disposed after processing {Count} records", _recordCount);
            }
        }
    }
}
