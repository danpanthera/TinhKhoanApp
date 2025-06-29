using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.RawData;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service để xử lý import dữ liệu thô từ CSV vào các bảng History
    /// Đảm bảo giữ nguyên tên cột CSV gốc trong Model properties và database columns
    /// </summary>
    public interface IRawDataProcessingService
    {
        Task<ProcessingResult> ProcessImportedDataToHistoryAsync(int importedDataRecordId, string category);
        Task<ProcessingResult> ProcessImportedDataToHistoryAsync(int importedDataRecordId, string category, DateTime? statementDate);
        Task<List<string>> GetValidCategoriesAsync();
        Task<ProcessingValidationResult> ValidateImportedDataForCategoryAsync(int importedDataRecordId, string category);
    }

    public class ProcessingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BatchId { get; set; }
        public int ProcessedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? TableName { get; set; }
    }

    public class ProcessingValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> InvalidHeaders { get; set; } = new();
        public List<string> ValidHeaders { get; set; } = new();
        public int TotalRecords { get; set; }
        public string? Message { get; set; }
    }

    public class RawDataProcessingService : IRawDataProcessingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RawDataProcessingService> _logger;

        public RawDataProcessingService(ApplicationDbContext context, ILogger<RawDataProcessingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProcessingResult> ProcessImportedDataToHistoryAsync(int importedDataRecordId, string category)
        {
            return await ProcessImportedDataToHistoryAsync(importedDataRecordId, category, null);
        }

        public async Task<ProcessingResult> ProcessImportedDataToHistoryAsync(int importedDataRecordId, string category, DateTime? statementDate)
        {
            var result = new ProcessingResult();

            try
            {
                _logger.LogInformation("🔄 Starting processing imported data to history. RecordId: {RecordId}, Category: {Category}",
                    importedDataRecordId, category);

                // Validate parameters
                if (string.IsNullOrWhiteSpace(category))
                {
                    result.Message = "Category is required";
                    return result;
                }

                // Get imported data record
                var importedRecord = await _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .FirstOrDefaultAsync(r => r.Id == importedDataRecordId);

                if (importedRecord == null)
                {
                    result.Message = $"Imported data record with ID {importedDataRecordId} not found";
                    return result;
                }

                if (!importedRecord.ImportedDataItems.Any())
                {
                    result.Message = "No data items found in the imported record";
                    return result;
                }

                // Validate category and headers
                var validationResult = await ValidateImportedDataForCategoryAsync(importedDataRecordId, category);
                if (!validationResult.IsValid)
                {
                    result.Message = $"Data validation failed: {validationResult.Message}";
                    result.Errors.AddRange(validationResult.InvalidHeaders.Select(h => $"Invalid header: {h}"));
                    return result;
                }

                // Use statement date from parameter or from imported record
                var effectiveStatementDate = statementDate ?? importedRecord.StatementDate ?? DateTime.UtcNow.Date;

                // Generate batch ID for this processing operation
                var batchId = Guid.NewGuid().ToString();
                result.BatchId = batchId;

                // Process based on category
                switch (category.ToUpper())
                {
                    case "LN01":
                        await ProcessLN01DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    case "7800_DT_KHKD1":
                        await ProcessDT_KHKD1_DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    case "BC57":
                        await ProcessBC57DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    case "DPDA":
                        await ProcessDPDADataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    case "EI01":
                        await ProcessEI01DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    case "KH03":
                        await ProcessKH03DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    case "GLCB41":
                        await ProcessGLCB41DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                        break;
                    // GL01 sẽ được xử lý trong tương lai với model phù hợp
                    // case "GL01":
                    //     await ProcessGL01DataAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result);
                    //     break;
                    // Add more cases as needed for other categories
                    default:
                        result.Message = $"Category '{category}' is not supported yet";
                        return result;
                }

                if (result.Success)
                {
                    _logger.LogInformation("✅ Successfully processed {ProcessedRecords} records to {TableName}. BatchId: {BatchId}",
                        result.ProcessedRecords, result.TableName, batchId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing imported data to history. RecordId: {RecordId}, Category: {Category}",
                    importedDataRecordId, category);
                result.Message = $"Processing failed: {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<List<string>> GetValidCategoriesAsync()
        {
            // Return categories that we support processing for
            return new List<string> { "LN01", "7800_DT_KHKD1", "BC57", "DPDA", "EI01", "KH03" };
        }

        public async Task<ProcessingValidationResult> ValidateImportedDataForCategoryAsync(int importedDataRecordId, string category)
        {
            var result = new ProcessingValidationResult();

            try
            {
                var importedRecord = await _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .FirstOrDefaultAsync(r => r.Id == importedDataRecordId);

                if (importedRecord == null)
                {
                    result.Message = $"Imported data record with ID {importedDataRecordId} not found";
                    return result;
                }

                result.TotalRecords = importedRecord.ImportedDataItems.Count;

                if (!importedRecord.ImportedDataItems.Any())
                {
                    result.Message = "No data items found";
                    return result;
                }

                // Get headers from first item
                var firstItem = importedRecord.ImportedDataItems.First();
                if (string.IsNullOrWhiteSpace(firstItem.RawData))
                {
                    result.Message = "First data item has no raw data";
                    return result;
                }

                var firstItemData = JsonSerializer.Deserialize<Dictionary<string, object>>(firstItem.RawData);
                if (firstItemData == null || !firstItemData.Any())
                {
                    result.Message = "Unable to parse first data item";
                    return result;
                }

                var csvHeaders = firstItemData.Keys.ToList();

                // Validate headers against category mapping
                var (isValid, invalidHeaders, validHeaders) = CsvColumnMappingConfig.ValidateCsvHeaders(category, csvHeaders);

                result.IsValid = isValid;
                result.InvalidHeaders = invalidHeaders;
                result.ValidHeaders = validHeaders;

                if (!isValid)
                {
                    result.Message = $"Invalid headers found for category '{category}': {string.Join(", ", invalidHeaders)}";
                }
                else
                {
                    result.Message = $"All {validHeaders.Count} headers are valid for category '{category}'";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating imported data for category. RecordId: {RecordId}, Category: {Category}",
                    importedDataRecordId, category);
                result.Message = $"Validation error: {ex.Message}";
                return result;
            }
        }

        #region Private Processing Methods

        private async Task ProcessLN01DataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "LN01_CsvHistory";

            var historyRecords = new List<LN01_History>();

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create LN01_History record with original CSV column names
                    var historyRecord = new LN01_History
                    {
                        // SCD Type 2 Fields
                        BusinessKey = GenerateBusinessKey("LN01", rowData),
                        EffectiveDate = statementDate,
                        ExpiryDate = null,
                        IsCurrent = true,
                        RowVersion = 1,

                        // Metadata Fields
                        ImportId = batchId,
                        StatementDate = statementDate,
                        ProcessedDate = DateTime.UtcNow,
                        DataHash = GenerateDataHash(rowData),

                        // Business Data Fields (using original CSV column names)
                        BRCD = GetStringValue(rowData, "BRCD"),
                        CUSTSEQ = GetStringValue(rowData, "CUSTSEQ"),
                        CUSTNM = GetStringValue(rowData, "CUSTNM"),
                        TAI_KHOAN = GetStringValue(rowData, "TAI_KHOAN"),
                        CCY = GetStringValue(rowData, "CCY"),
                        DU_NO = GetDecimalValue(rowData, "DU_NO"),
                        DSBSSEQ = GetStringValue(rowData, "DSBSSEQ"),
                        TRANSACTION_DATE = GetDateTimeValue(rowData, "TRANSACTION_DATE"),
                        DSBSDT = GetDateTimeValue(rowData, "DSBSDT"),
                        DISBUR_CCY = GetStringValue(rowData, "DISBUR_CCY"),
                        DISBURSEMENT_AMOUNT = GetDecimalValue(rowData, "DISBURSEMENT_AMOUNT"),

                        // Store complete raw data for reference
                        RawDataJson = item.RawData
                    };

                    historyRecords.Add(historyRecord);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing LN01 data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            if (historyRecords.Any())
            {
                _context.LN01_History.AddRange(historyRecords);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.ProcessedRecords = historyRecords.Count;
                result.Message = $"Successfully processed {historyRecords.Count} LN01 records";
            }
            else
            {
                result.Message = "No valid LN01 records to process";
            }
        }

        private async Task ProcessDT_KHKD1_DataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "7800_DT_KHKD1_History";

            var historyRecords = new List<DT_KHKD1_History>();

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create DT_KHKD1_History record with original CSV column names
                    var historyRecord = new DT_KHKD1_History
                    {
                        // SCD Type 2 Fields
                        BusinessKey = GenerateBusinessKey("DT_KHKD1", rowData),
                        EffectiveDate = statementDate,
                        ExpiryDate = null,
                        IsCurrent = true,
                        RowVersion = 1,

                        // Metadata Fields
                        ImportId = batchId,
                        StatementDate = statementDate,
                        ProcessedDate = DateTime.UtcNow,
                        DataHash = GenerateDataHash(rowData),

                        // Business Data Fields (using original CSV column names)
                        BRCD = GetStringValue(rowData, "BRCD"),
                        BRANCH_NAME = GetStringValue(rowData, "BRANCH_NAME"),
                        INDICATOR_TYPE = GetStringValue(rowData, "INDICATOR_TYPE"),
                        INDICATOR_NAME = GetStringValue(rowData, "INDICATOR_NAME"),
                        PLAN_YEAR = GetDecimalValue(rowData, "PLAN_YEAR"),
                        PLAN_QUARTER = GetDecimalValue(rowData, "PLAN_QUARTER"),
                        PLAN_MONTH = GetDecimalValue(rowData, "PLAN_MONTH"),
                        ACTUAL_YEAR = GetDecimalValue(rowData, "ACTUAL_YEAR"),
                        ACTUAL_QUARTER = GetDecimalValue(rowData, "ACTUAL_QUARTER"),
                        ACTUAL_MONTH = GetDecimalValue(rowData, "ACTUAL_MONTH"),
                        ACHIEVEMENT_RATE = GetDecimalValue(rowData, "ACHIEVEMENT_RATE"),
                        YEAR = GetIntValue(rowData, "YEAR"),
                        QUARTER = GetIntValue(rowData, "QUARTER"),
                        MONTH = GetIntValue(rowData, "MONTH"),
                        CREATED_DATE = GetDateTimeValue(rowData, "CREATED_DATE"),
                        UPDATED_DATE = GetDateTimeValue(rowData, "UPDATED_DATE"),

                        // Store complete raw data for reference
                        RawDataJson = item.RawData
                    };

                    historyRecords.Add(historyRecord);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing DT_KHKD1 data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            if (historyRecords.Any())
            {
                _context.DT_KHKD1_History.AddRange(historyRecords);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.ProcessedRecords = historyRecords.Count;
                result.Message = $"Successfully processed {historyRecords.Count} DT_KHKD1 records";
            }
            else
            {
                result.Message = "No valid DT_KHKD1 records to process";
            }
        }

        private async Task ProcessBC57DataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "BC57_History";

            var historyRecords = new List<BC57History>();

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create BC57History record with original CSV column names
                    var historyRecord = new BC57History
                    {
                        // SCD Type 2 Fields
                        BusinessKey = GenerateBusinessKey("BC57", rowData),
                        EffectiveDate = statementDate,
                        ExpiryDate = null,
                        IsCurrent = true,
                        RowVersion = 1,

                        // Metadata Fields
                        ImportId = batchId,
                        StatementDate = statementDate,
                        ProcessedDate = DateTime.UtcNow,
                        DataHash = GenerateDataHash(rowData),

                        // Business Data Fields (using original CSV column names as available)
                        // Note: BC57History properties need to be mapped from CSV columns
                        MaKhachHang = GetStringValue(rowData, "MaKhachHang") ?? GetStringValue(rowData, "CUSTSEQ"),
                        TenKhachHang = GetStringValue(rowData, "TenKhachHang") ?? GetStringValue(rowData, "CUSTNM"),
                        SoTaiKhoan = GetStringValue(rowData, "SoTaiKhoan") ?? GetStringValue(rowData, "TAI_KHOAN"),
                        MaHopDong = GetStringValue(rowData, "MaHopDong"),
                        LoaiSanPham = GetStringValue(rowData, "LoaiSanPham"),
                        SoTienGoc = GetDecimalValue(rowData, "SoTienGoc"),
                        LaiSuat = GetDecimalValue(rowData, "LaiSuat"),
                        SoNgayTinhLai = GetIntValue(rowData, "SoNgayTinhLai"),
                        TienLaiDuThu = GetDecimalValue(rowData, "TienLaiDuThu"),
                        TienLaiQuaHan = GetDecimalValue(rowData, "TienLaiQuaHan"),
                        NgayBatDau = GetDateTimeValue(rowData, "NgayBatDau"),
                        NgayKetThuc = GetDateTimeValue(rowData, "NgayKetThuc"),
                        TrangThai = GetStringValue(rowData, "TrangThai"),
                        MaChiNhanh = GetStringValue(rowData, "MaChiNhanh") ?? GetStringValue(rowData, "BRCD"),
                        TenChiNhanh = GetStringValue(rowData, "TenChiNhanh"),
                        NgayTinhLai = GetDateTimeValue(rowData, "NgayTinhLai"),

                        // Store complete raw data for reference
                        AdditionalData = item.RawData
                    };

                    historyRecords.Add(historyRecord);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing BC57 data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            if (historyRecords.Any())
            {
                _context.BC57History.AddRange(historyRecords);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.ProcessedRecords = historyRecords.Count;
                result.Message = $"Successfully processed {historyRecords.Count} BC57 records";
            }
            else
            {
                result.Message = "No valid BC57 records to process";
            }
        }

        private async Task ProcessDPDADataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "DPDA_History";

            var historyRecords = new List<DPDAHistory>();

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create DPDAHistory record with original CSV column names
                    var historyRecord = new DPDAHistory
                    {
                        // SCD Type 2 Fields
                        BusinessKey = GenerateBusinessKey("DPDA", rowData),
                        EffectiveDate = statementDate,
                        ExpiryDate = null,
                        IsCurrent = true,
                        RowVersion = 1,

                        // Metadata Fields
                        ImportId = batchId,
                        StatementDate = statementDate,
                        ProcessedDate = DateTime.UtcNow,
                        DataHash = GenerateDataHash(rowData),

                        // Business Data Fields - map from CSV columns
                        MaKhachHang = GetStringValue(rowData, "MaKhachHang") ?? GetStringValue(rowData, "CUSTSEQ"),
                        TenKhachHang = GetStringValue(rowData, "TenKhachHang") ?? GetStringValue(rowData, "CUSTNM"),
                        SoThe = GetStringValue(rowData, "SoThe"),
                        LoaiThe = GetStringValue(rowData, "LoaiThe"),
                        TrangThaiThe = GetStringValue(rowData, "TrangThaiThe"),
                        NgayPhatHanh = GetDateTimeValue(rowData, "NgayPhatHanh")
                    };

                    historyRecords.Add(historyRecord);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing DPDA data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            if (historyRecords.Any())
            {
                _context.DPDAHistory.AddRange(historyRecords);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.ProcessedRecords = historyRecords.Count;
                result.Message = $"Successfully processed {historyRecords.Count} DPDA records";
            }
            else
            {
                result.Message = "No valid DPDA records to process";
            }
        }

        private async Task ProcessEI01DataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "EI01_History";

            // For EI01, we'll just count for now until model is fully mapped
            var processedCount = 0;

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing EI01 data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} EI01 records (raw data processing)";
        }

        private async Task ProcessKH03DataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "KH03_History";

            // For KH03, we'll just count for now until model is fully mapped
            var processedCount = 0;

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing KH03 data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} KH03 records (raw data processing)";
        }

        private async Task ProcessGLCB41DataAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result)
        {
            result.TableName = "GLCB41_History";
            var historyRecords = new List<GLCB41_History>();

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Tạo business key duy nhất cho GLCB41 record
                    var businessKey = GenerateBusinessKey("GLCB41", rowData);

                    var historyRecord = new GLCB41_History
                    {
                        BusinessKey = businessKey,
                        EffectiveDate = statementDate,
                        ProcessedDate = DateTime.UtcNow,
                        ImportId = batchId,
                        StatementDate = statementDate,
                        IsCurrent = true,
                        DataHash = GenerateDataHash(rowData),

                        // 🏦 GLCB41 specific fields - Mapping theo cột CSV gốc
                        JOURNAL_NO = GetStringValue(rowData, "JOURNAL_NO") ?? GetStringValue(rowData, "SO_CT"),
                        ACCOUNT_NO = GetStringValue(rowData, "ACCOUNT_NO") ?? GetStringValue(rowData, "SO_TK"),
                        ACCOUNT_NAME = GetStringValue(rowData, "ACCOUNT_NAME") ?? GetStringValue(rowData, "TEN_TK"),
                        CUSTOMER_ID = GetStringValue(rowData, "CUSTOMER_ID") ?? GetStringValue(rowData, "MA_KH"),
                        CUSTOMER_NAME = GetStringValue(rowData, "CUSTOMER_NAME") ?? GetStringValue(rowData, "TEN_KH"),
                        TRANSACTION_DATE = GetDateTimeValue(rowData, "TRANSACTION_DATE") ?? GetDateTimeValue(rowData, "NGAY_GD"),
                        POSTING_DATE = GetDateTimeValue(rowData, "POSTING_DATE") ?? GetDateTimeValue(rowData, "NGAY_HT"),
                        DESCRIPTION = GetStringValue(rowData, "DESCRIPTION") ?? GetStringValue(rowData, "DIEN_GIAI"),
                        DEBIT_AMOUNT = GetDecimalValue(rowData, "DEBIT_AMOUNT") ?? GetDecimalValue(rowData, "SO_NO"),
                        CREDIT_AMOUNT = GetDecimalValue(rowData, "CREDIT_AMOUNT") ?? GetDecimalValue(rowData, "SO_CO"),
                        DEBIT_BALANCE = GetDecimalValue(rowData, "DEBIT_BALANCE") ?? GetDecimalValue(rowData, "DU_NO"),
                        CREDIT_BALANCE = GetDecimalValue(rowData, "CREDIT_BALANCE") ?? GetDecimalValue(rowData, "DU_CO"),
                        BRCD = GetStringValue(rowData, "BRCD") ?? GetStringValue(rowData, "MA_CN"),
                        BRANCH_NAME = GetStringValue(rowData, "BRANCH_NAME") ?? GetStringValue(rowData, "TEN_CN"),
                        TRANSACTION_TYPE = GetStringValue(rowData, "TRANSACTION_TYPE") ?? GetStringValue(rowData, "LOAI_GD"),
                        ORIGINAL_TRANS_ID = GetStringValue(rowData, "ORIGINAL_TRANS_ID") ?? GetStringValue(rowData, "MA_GD_GOC"),
                        CREATED_DATE = GetDateTimeValue(rowData, "CREATED_DATE"),
                        UPDATED_DATE = GetDateTimeValue(rowData, "UPDATED_DATE"),

                        // Store complete raw data for reference
                        RawDataJson = item.RawData,
                        AdditionalData = item.RawData
                    };

                    historyRecords.Add(historyRecord);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ Error processing GLCB41 data item: {Error}", ex.Message);
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            if (historyRecords.Any())
            {
                _context.GLCB41_History.AddRange(historyRecords);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.ProcessedRecords = historyRecords.Count;
                result.Message = $"Successfully processed {historyRecords.Count} GLCB41 records to GLCB41_History";

                _logger.LogInformation("✅ GLCB41: Processed {Count} records to GLCB41_History", historyRecords.Count);
            }
            else
            {
                result.Message = "No valid GLCB41 records to process";
                _logger.LogWarning("⚠️ GLCB41: No valid records found to process");
            }
        }
        #endregion

        #region Helper Methods

        private string GenerateBusinessKey(string prefix, Dictionary<string, object> data)
        {
            // Create a business key based on primary identifier fields
            var keyFields = new List<string>();

            if (data.ContainsKey("BRCD")) keyFields.Add(GetStringValue(data, "BRCD") ?? "");
            if (data.ContainsKey("CUSTSEQ")) keyFields.Add(GetStringValue(data, "CUSTSEQ") ?? "");
            if (data.ContainsKey("TAI_KHOAN")) keyFields.Add(GetStringValue(data, "TAI_KHOAN") ?? "");

            var businessKey = $"{prefix}_{string.Join("_", keyFields.Where(k => !string.IsNullOrEmpty(k)))}";
            return businessKey.Length > 500 ? businessKey.Substring(0, 500) : businessKey;
        }

        private string GenerateDataHash(Dictionary<string, object> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = false });
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hashBytes);
        }

        private string? GetStringValue(Dictionary<string, object> data, string key)
        {
            if (data.TryGetValue(key, out var value))
            {
                return value?.ToString()?.Trim();
            }
            return null;
        }

        private decimal? GetDecimalValue(Dictionary<string, object> data, string key)
        {
            var stringValue = GetStringValue(data, key);
            if (string.IsNullOrWhiteSpace(stringValue)) return null;

            if (decimal.TryParse(stringValue, out var result))
                return result;

            return null;
        }

        private int? GetIntValue(Dictionary<string, object> data, string key)
        {
            var stringValue = GetStringValue(data, key);
            if (string.IsNullOrWhiteSpace(stringValue)) return null;

            if (int.TryParse(stringValue, out var result))
                return result;

            return null;
        }

        private DateTime? GetDateTimeValue(Dictionary<string, object> data, string key)
        {
            var stringValue = GetStringValue(data, key);
            if (string.IsNullOrWhiteSpace(stringValue)) return null;

            if (DateTime.TryParse(stringValue, out var result))
                return result;

            return null;
        }

        #endregion
    }
}
