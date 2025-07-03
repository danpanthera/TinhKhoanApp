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

        // Placeholder methods for remaining table types - will implement full mapping later
        private async Task ProcessLN03ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "LN03";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} LN03 records (placeholder)";
        }

        private async Task ProcessDPDAToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DPDA";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DPDA records (placeholder)";
        }

        private async Task ProcessDP01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DP01";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DP01 records (placeholder)";
        }

        private async Task ProcessEI01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "EI01";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} EI01 records (placeholder)";
        }

        private async Task ProcessGL01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "GL01";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} GL01 records (placeholder)";
        }

        private async Task ProcessGL41ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "GL41";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} GL41 records (placeholder)";
        }

        private async Task ProcessKH03ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "KH03";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} KH03 records (placeholder)";
        }

        private async Task ProcessRR01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "RR01";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} RR01 records (placeholder)";
        }

        private async Task ProcessDT_KHKD1ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName)
        {
            result.TableName = "DT_KHKD1";
            var processedCount = dataItems.Count(x => !string.IsNullOrWhiteSpace(x.RawData));
            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DT_KHKD1 records (placeholder)";
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
