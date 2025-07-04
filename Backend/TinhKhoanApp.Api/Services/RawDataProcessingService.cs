using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.RawData;
using TinhKhoanApp.Api.Models.DataTables;

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

                // Process based on category - Route to new separate data tables
                switch (category.ToUpper())
                {
                    case "LN01":
                        await ProcessLN01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "LN02":
                        await ProcessLN02ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "LN03":
                        await ProcessLN03ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "DB01":
                        await ProcessDB01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "DPDA":
                        await ProcessDPDAToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "DP01":
                        await ProcessDP01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "EI01":
                        await ProcessEI01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "GL01":
                        await ProcessGL01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "GL41":
                        await ProcessGL41ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "KH03":
                        await ProcessKH03ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "RR01":
                        await ProcessRR01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
                    case "7800_DT_KHKD1":
                    case "DT_KHKD1":
                        await ProcessDT_KHKD1ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName);
                        break;
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
            // Return all categories that we support processing for with new table structure
            return new List<string> {
                "LN01", "LN02", "LN03", "DB01", "DPDA", "DP01",
                "EI01", "GL01", "GL41", "KH03", "RR01", "7800_DT_KHKD1", "DT_KHKD1"
            };
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

                // For now, just validate that we have data items
                result.IsValid = true;
                result.ValidHeaders = new List<string> { "General data validation passed" };
                result.Message = "Data validation successful";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating imported data for category {Category}", category);
                result.Message = $"Validation failed: {ex.Message}";
                return result;
            }
        }

        #region New Processing Methods for Separate Data Tables

        private async Task ProcessLN01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "LN01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.LN01>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new LN01 entity with proper mapping
                    var ln01Entity = new TinhKhoanApp.Api.Models.DataTables.LN01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_KH = GetStringValue(rowData, "MA_KH"),
                        SO_HD_CHO_VAY = GetStringValue(rowData, "SO_HD_CHO_VAY"),
                        LOAI_HINH_CHO_VAY = GetStringValue(rowData, "LOAI_HINH_CHO_VAY"),
                        SO_TIEN_CHO_VAY = GetDecimalValue(rowData, "SO_TIEN_CHO_VAY"),
                        DU_NO_GOC = GetDecimalValue(rowData, "DU_NO_GOC"),
                        LAI_SUAT_CHO_VAY = GetDecimalValue(rowData, "LAI_SUAT_CHO_VAY"),
                        NGAY_GIAI_NGAN = GetDateTimeValue(rowData, "NGAY_GIAI_NGAN"),
                        NGAY_DEN_HAN = GetDateTimeValue(rowData, "NGAY_DEN_HAN"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(ln01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.LN01s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing LN01 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.LN01s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} LN01 records to database";

            _logger.LogInformation("✅ Processed {Count} LN01 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessLN02ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "LN02";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.LN02>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new LN02 entity with proper mapping
                    var ln02Entity = new TinhKhoanApp.Api.Models.DataTables.LN02
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_KH = GetStringValue(rowData, "MA_KH"),
                        SO_HD_CHO_VAY = GetStringValue(rowData, "SO_HD_CHO_VAY"),
                        KY_HAN_THANH_TOAN = GetIntValue(rowData, "KY_HAN_THANH_TOAN"),
                        SO_TIEN_GOC_PHAI_TRA = GetDecimalValue(rowData, "SO_TIEN_GOC_PHAI_TRA"),
                        SO_TIEN_LAI_PHAI_TRA = GetDecimalValue(rowData, "SO_TIEN_LAI_PHAI_TRA"),
                        NGAY_DEN_HAN_TRA = GetDateTimeValue(rowData, "NGAY_DEN_HAN_TRA"),
                        NGAY_TRA_THUC_TE = GetDateTimeValue(rowData, "NGAY_TRA_THUC_TE"),
                        SO_TIEN_DA_TRA = GetDecimalValue(rowData, "SO_TIEN_DA_TRA"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(ln02Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.LN02s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing LN02 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.LN02s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} LN02 records to database";

            _logger.LogInformation("✅ Processed {Count} LN02 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessDB01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DB01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.DB01>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new DB01 entity with proper mapping
                    var db01Entity = new TinhKhoanApp.Api.Models.DataTables.DB01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_KH = GetStringValue(rowData, "MA_KH"),
                        SO_HD_TSDB = GetStringValue(rowData, "SO_HD_TSDB"),
                        LOAI_TSDB = GetStringValue(rowData, "LOAI_TSDB"),
                        GIA_TRI_TSDB = GetDecimalValue(rowData, "GIA_TRI_TSDB"),
                        GIA_TRI_DINH_GIA = GetDecimalValue(rowData, "GIA_TRI_DINH_GIA"),
                        NGAY_DINH_GIA = GetDateTimeValue(rowData, "NGAY_DINH_GIA"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(db01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.DB01s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing DB01 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.DB01s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DB01 records to database";

            _logger.LogInformation("✅ Processed {Count} DB01 records with batch ID {BatchId}", processedCount, batchId);
        }

        // Complete mapping implementations for remaining table types
        private async Task ProcessLN03ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "LN03";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.LN03>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new LN03 entity with proper mapping
                    var ln03Entity = new TinhKhoanApp.Api.Models.DataTables.LN03
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_KH = GetStringValue(rowData, "MA_KH"),
                        SO_HD_CHO_VAY = GetStringValue(rowData, "SO_HD_CHO_VAY"),
                        NHOM_NO = GetStringValue(rowData, "NHOM_NO"),
                        DU_NO_GOC = GetDecimalValue(rowData, "DU_NO_GOC"),
                        DU_NO_LAI = GetDecimalValue(rowData, "DU_NO_LAI"),
                        SO_NGAY_QUA_HAN = GetIntValue(rowData, "SO_NGAY_QUA_HAN"),
                        TY_LE_DU_PHONG = GetDecimalValue(rowData, "TY_LE_DU_PHONG"),
                        SO_TIEN_DU_PHONG = GetDecimalValue(rowData, "SO_TIEN_DU_PHONG"),
                        NGAY_PHAN_LOAI_NO = GetDateTimeValue(rowData, "NGAY_PHAN_LOAI_NO"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(ln03Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.LN03s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing LN03 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.LN03s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} LN03 records to database";

            _logger.LogInformation("✅ Processed {Count} LN03 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessDPDAToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DPDA";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.DPDA>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new DPDA entity with proper mapping
                    var dpdaEntity = new TinhKhoanApp.Api.Models.DataTables.DPDA
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_KH = GetStringValue(rowData, "MA_KH"),
                        SO_TK_TIEN_GUI = GetStringValue(rowData, "SO_TK_TIEN_GUI"),
                        LOAI_TIEN_GUI = GetStringValue(rowData, "LOAI_TIEN_GUI"),
                        SO_DU_TIEN_GUI = GetDecimalValue(rowData, "SO_DU_TIEN_GUI"),
                        LAI_SUAT = GetDecimalValue(rowData, "LAI_SUAT"),
                        NGAY_MO_TK = GetDateTimeValue(rowData, "NGAY_MO_TK"),
                        NGAY_DEN_HAN = GetDateTimeValue(rowData, "NGAY_DEN_HAN"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(dpdaEntity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.DPDAs.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing DPDA row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.DPDAs.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DPDA records to database";

            _logger.LogInformation("✅ Processed {Count} DPDA records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessDP01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DP01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.DP01>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new DP01 entity with proper mapping
                    var dp01Entity = new TinhKhoanApp.Api.Models.DataTables.DP01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        TAI_KHOAN_HACH_TOAN = GetStringValue(rowData, "TAI_KHOAN_HACH_TOAN"),
                        CURRENT_BALANCE = GetDecimalValue(rowData, "CURRENT_BALANCE"),
                        SO_DU_DAU_KY = GetDecimalValue(rowData, "SO_DU_DAU_KY"),
                        SO_PHAT_SINH_NO = GetDecimalValue(rowData, "SO_PHAT_SINH_NO"),
                        SO_PHAT_SINH_CO = GetDecimalValue(rowData, "SO_PHAT_SINH_CO"),
                        SO_DU_CUOI_KY = GetDecimalValue(rowData, "SO_DU_CUOI_KY")
                    };

                    entitiesToAdd.Add(dp01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.DP01NewTables.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing DP01 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.DP01NewTables.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DP01 records to database";

            _logger.LogInformation("✅ Processed {Count} DP01 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessEI01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "EI01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.EI01>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new EI01 entity with proper mapping
                    var ei01Entity = new TinhKhoanApp.Api.Models.DataTables.EI01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_TK_HT = GetStringValue(rowData, "MA_TK_HT"),
                        TEN_TK_HT = GetStringValue(rowData, "TEN_TK_HT"),
                        LOAI_THU_NHAP = GetStringValue(rowData, "LOAI_THU_NHAP"),
                        SO_TIEN_THU_NHAP = GetDecimalValue(rowData, "SO_TIEN_THU_NHAP"),
                        NGAY_PHAT_SINH = GetDateTimeValue(rowData, "NGAY_PHAT_SINH"),
                        DIEN_GIAI = GetStringValue(rowData, "DIEN_GIAI")
                    };

                    entitiesToAdd.Add(ei01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.EI01s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing EI01 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.EI01s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} EI01 records to database";

            _logger.LogInformation("✅ Processed {Count} EI01 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessGL01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "GL01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.GL01>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new GL01 entity with proper mapping
                    var gl01Entity = new TinhKhoanApp.Api.Models.DataTables.GL01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_TK_HT = GetStringValue(rowData, "MA_TK_HT"),
                        TEN_TK_HT = GetStringValue(rowData, "TEN_TK_HT"),
                        SO_DU_DAU_KY = GetDecimalValue(rowData, "SO_DU_DAU_KY"),
                        PHAT_SINH_NO = GetDecimalValue(rowData, "PHAT_SINH_NO"),
                        PHAT_SINH_CO = GetDecimalValue(rowData, "PHAT_SINH_CO"),
                        SO_DU_CUOI_KY = GetDecimalValue(rowData, "SO_DU_CUOI_KY")
                    };

                    entitiesToAdd.Add(gl01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.GL01s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing GL01 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.GL01s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} GL01 records to database";

            _logger.LogInformation("✅ Processed {Count} GL01 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessGL41ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "GL41";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.GL41>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new GL41 entity with proper mapping
                    var gl41Entity = new TinhKhoanApp.Api.Models.DataTables.GL41
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        SO_TK = GetStringValue(rowData, "SO_TK"),
                        TEN_TK = GetStringValue(rowData, "TEN_TK"),
                        SO_DU = GetDecimalValue(rowData, "SO_DU"),
                        SO_DU_DAU_KY = GetDecimalValue(rowData, "SO_DU_DAU_KY"),
                        SO_PHAT_SINH_NO = GetDecimalValue(rowData, "SO_PHAT_SINH_NO"),
                        SO_PHAT_SINH_CO = GetDecimalValue(rowData, "SO_PHAT_SINH_CO"),
                        SO_DU_CUOI_KY = GetDecimalValue(rowData, "SO_DU_CUOI_KY"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(gl41Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.GL41s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing GL41 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.GL41s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} GL41 records to database";

            _logger.LogInformation("✅ Processed {Count} GL41 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessKH03ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "KH03";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.KH03>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new KH03 entity with proper mapping
                    var kh03Entity = new TinhKhoanApp.Api.Models.DataTables.KH03
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        MA_KH = GetStringValue(rowData, "MA_KH"),
                        TEN_KH = GetStringValue(rowData, "TEN_KH"),
                        LOAI_KH = GetStringValue(rowData, "LOAI_KH"),
                        SO_CMND = GetStringValue(rowData, "SO_CMND"),
                        DIA_CHI = GetStringValue(rowData, "DIA_CHI"),
                        SO_DIEN_THOAI = GetStringValue(rowData, "SO_DIEN_THOAI"),
                        EMAIL = GetStringValue(rowData, "EMAIL"),
                        NGAY_MO_TK = GetDateTimeValue(rowData, "NGAY_MO_TK"),
                        TRANG_THAI = GetStringValue(rowData, "TRANG_THAI")
                    };

                    entitiesToAdd.Add(kh03Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.KH03s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing KH03 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.KH03s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} KH03 records to database";

            _logger.LogInformation("✅ Processed {Count} KH03 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessRR01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "RR01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.RR01>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new RR01 entity with proper mapping
                    var rr01Entity = new TinhKhoanApp.Api.Models.DataTables.RR01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        LOAI_TY_LE = GetStringValue(rowData, "LOAI_TY_LE"),
                        TEN_CHI_TIEU = GetStringValue(rowData, "TEN_CHI_TIEU"),
                        GIA_TRI_TY_LE = GetDecimalValue(rowData, "GIA_TRI_TY_LE"),
                        DON_VI_TINH = GetStringValue(rowData, "DON_VI_TINH"),
                        NGAY_AP_DUNG = GetDateTimeValue(rowData, "NGAY_AP_DUNG"),
                        GHI_CHU = GetStringValue(rowData, "GHI_CHU")
                    };

                    entitiesToAdd.Add(rr01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.RR01s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing RR01 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.RR01s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} RR01 records to database";

            _logger.LogInformation("✅ Processed {Count} RR01 records with batch ID {BatchId}", processedCount, batchId);
        }

        private async Task ProcessDT_KHKD1ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DT_KHKD1";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.DT_KHKD1>();

            // Format statement date as dd/MM/yyyy for NgayDL field
            var ngayDL = statementDate.ToString("dd/MM/yyyy");

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Create new DT_KHKD1 entity with proper mapping
                    var dtKhkd1Entity = new TinhKhoanApp.Api.Models.DataTables.DT_KHKD1
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties
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
                        MONTH = GetIntValue(rowData, "MONTH")
                    };

                    entitiesToAdd.Add(dtKhkd1Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.DT_KHKD1s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("Error processing DT_KHKD1 row: {Error}", ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.DT_KHKD1s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DT_KHKD1 records to database";

            _logger.LogInformation("✅ Processed {Count} DT_KHKD1 records with batch ID {BatchId}", processedCount, batchId);
        }

        #endregion

        #region Helper Methods

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
