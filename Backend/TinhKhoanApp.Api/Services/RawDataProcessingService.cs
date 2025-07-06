using System.Text.Json;
using System.Text.RegularExpressions;
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
        Task<ProcessingResult> ProcessImportedDataToHistoryAsync(int importedDataRecordId, string category, DateTime? statementDate, string? ngayDL);
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
            return await ProcessImportedDataToHistoryAsync(importedDataRecordId, category, statementDate, null);
        }

        public async Task<ProcessingResult> ProcessImportedDataToHistoryAsync(int importedDataRecordId, string category, DateTime? statementDate, string? ngayDL)
        {
            var result = new ProcessingResult();

            try
            {
                _logger.LogInformation("🔄 Starting processing imported data to history. RecordId: {RecordId}, Category: {Category}, NgayDL: {NgayDL}",
                    importedDataRecordId, category, ngayDL);

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

                // Use provided NgayDL or extract from filename as fallback
                var effectiveNgayDL = ngayDL ?? ExtractNgayDLFromFileName(importedRecord.FileName);
                _logger.LogInformation("📅 Using NgayDL: {NgayDL} for import processing", effectiveNgayDL);

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
                        await ProcessDP01ToNewTableAsync(importedRecord.ImportedDataItems, batchId, effectiveStatementDate, result, importedRecord.FileName, effectiveNgayDL);
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

            _logger.LogInformation("🔄 [LN01_IMPORT] Bắt đầu xử lý {Count} dòng dữ liệu, FileName: {FileName}", dataItems.Count, fileName);

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Ưu tiên lấy NGAY_DL từ file CSV, nếu không có thì dùng statementDate
                    string ngayDL = rowData.ContainsKey("NGAY_DL") && !string.IsNullOrWhiteSpace(rowData["NGAY_DL"]?.ToString())
                        ? rowData["NGAY_DL"]?.ToString() ?? statementDate.ToString("dd/MM/yyyy")
                        : statementDate.ToString("dd/MM/yyyy");

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

                    // Log chi tiết từng dòng để debug mapping
                    _logger.LogDebug("📊 [LN01_ROW_{RowNum}] NgayDL={NgayDL}, MA_CN={MaCN}, MA_KH={MaKH}, SO_HD={SoHD}, DU_NO={DuNo}",
                        processedCount + 1, ln01Entity.NgayDL, ln01Entity.MA_CN, ln01Entity.MA_KH,
                        ln01Entity.SO_HD_CHO_VAY, ln01Entity.DU_NO_GOC);

                    entitiesToAdd.Add(ln01Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.LN01s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("💾 [LN01_BATCH] Đã lưu {Count} bản ghi vào database", entitiesToAdd.Count);
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("⚠️ [LN01_ERROR] Lỗi xử lý dòng {RowNum}: {Error}", processedCount + 1, ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.LN01s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
                _logger.LogInformation("💾 [LN01_FINAL] Đã lưu {Count} bản ghi cuối cùng vào database", entitiesToAdd.Count);
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} LN01 records to database";

            _logger.LogInformation("✅ [LN01_COMPLETE] Đã xử lý xong {Count} bản ghi LN01 với BatchId: {BatchId}", processedCount, batchId);
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

        private async Task ProcessDP01ToNewTableAsync(ICollection<ImportedDataItem> dataItems, string batchId, DateTime statementDate, ProcessingResult result, string fileName, string ngayDL)
        {
            result.TableName = "DP01";
            var processedCount = 0;
            var entitiesToAdd = new List<TinhKhoanApp.Api.Models.DataTables.DP01>();

            _logger.LogInformation("🔄 [DP01_IMPORT] Bắt đầu xử lý {Count} dòng dữ liệu, FileName: {FileName}", dataItems.Count, fileName);

            foreach (var item in dataItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.RawData)) continue;

                    var rowData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                    if (rowData == null || !rowData.Any()) continue;

                    // Use the NgayDL passed from the calling method
                    _logger.LogInformation("📅 [DP01_IMPORT] Using NgayDL: {NgayDL} for row {RowNum}", ngayDL, processedCount + 1);

                    // Tạo entity mới với mapping đầy đủ các trường
                    var dp01Entity = new TinhKhoanApp.Api.Models.DataTables.DP01
                    {
                        NgayDL = ngayDL,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,

                        // Map CSV columns to model properties - giữ nguyên tên cột từ CSV
                        MA_CN = GetStringValue(rowData, "MA_CN"),
                        MA_PGD = GetStringValue(rowData, "MA_PGD"),
                        TAI_KHOAN_HACH_TOAN = GetStringValue(rowData, "TAI_KHOAN_HACH_TOAN"),
                        CURRENT_BALANCE = GetDecimalValue(rowData, "CURRENT_BALANCE"),
                        SO_DU_DAU_KY = GetDecimalValue(rowData, "SO_DU_DAU_KY"),
                        SO_PHAT_SINH_NO = GetDecimalValue(rowData, "SO_PHAT_SINH_NO"),
                        SO_PHAT_SINH_CO = GetDecimalValue(rowData, "SO_PHAT_SINH_CO"),
                        SO_DU_CUOI_KY = GetDecimalValue(rowData, "SO_DU_CUOI_KY")
                    };

                    // Log chi tiết từng dòng để debug mapping
                    _logger.LogInformation("📊 [DP01_ROW_{RowNum}] NgayDL={NgayDL}, MA_CN={MaCN}, TAI_KHOAN={TaiKhoan}, BALANCE={Balance}",
                        processedCount + 1, dp01Entity.NgayDL, dp01Entity.MA_CN, dp01Entity.TAI_KHOAN_HACH_TOAN, dp01Entity.CURRENT_BALANCE);

                    entitiesToAdd.Add(dp01Entity);
                    processedCount++;

                    // Batch insert cho hiệu năng - xử lý theo từng chunk 1000 bản ghi
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.DP01_News.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("💾 [DP01_BATCH] Đã lưu {Count} bản ghi vào database", entitiesToAdd.Count);
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("⚠️ [DP01_ERROR] Lỗi xử lý dòng {RowNum}: {Error}", processedCount + 1, ex.Message);
                }
            }

            // Insert các bản ghi còn lại
            if (entitiesToAdd.Any())
            {
                _context.DP01_News.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
                _logger.LogInformation("💾 [DP01_FINAL] Đã lưu {Count} bản ghi cuối cùng vào database", entitiesToAdd.Count);
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DP01 records to database";

            _logger.LogInformation("✅ [DP01_COMPLETE] Đã xử lý xong {Count} bản ghi DP01 với BatchId: {BatchId}", processedCount, batchId);
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

            _logger.LogInformation("🔄 [DT_KHKD1_IMPORT] Bắt đầu xử lý {Count} dòng dữ liệu từ file đặc biệt, FileName: {FileName}",
                dataItems.Count, fileName);

            // Extract year and month from filename or statement date
            int year = statementDate.Year;
            int month = statementDate.Month;
            // Try to extract from filename, format: *YYYYMM*
            var dateMatch = Regex.Match(fileName, @"(\d{4})(\d{2})");
            if (dateMatch.Success && dateMatch.Groups.Count >= 3)
            {
                if (int.TryParse(dateMatch.Groups[1].Value, out int fileYear))
                {
                    year = fileYear;
                }
                if (int.TryParse(dateMatch.Groups[2].Value, out int fileMonth))
                {
                    month = fileMonth;
                }
            }

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
                        YEAR = year,  // Use extracted year
                        MONTH = month, // Use extracted month
                        QUARTER = (month - 1) / 3 + 1, // Calculate quarter from month

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
                        ACHIEVEMENT_RATE = GetDecimalValue(rowData, "ACHIEVEMENT_RATE")
                    };

                    // Log chi tiết từng dòng để debug mapping
                    _logger.LogDebug("📊 [DT_KHKD1_ROW_{RowNum}] NgayDL={NgayDL}, BRCD={BRCD}, INDICATOR_NAME={INDICATOR_NAME}, YEAR={YEAR}, MONTH={MONTH}",
                        processedCount + 1, dtKhkd1Entity.NgayDL, dtKhkd1Entity.BRCD, dtKhkd1Entity.INDICATOR_NAME,
                        dtKhkd1Entity.YEAR, dtKhkd1Entity.MONTH);

                    entitiesToAdd.Add(dtKhkd1Entity);
                    processedCount++;

                    // Batch insert for performance - process in chunks of 1000
                    if (entitiesToAdd.Count >= 1000)
                    {
                        _context.DT_KHKD1s.AddRange(entitiesToAdd);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("💾 [DT_KHKD1_BATCH] Đã lưu {Count} bản ghi vào database", entitiesToAdd.Count);
                        entitiesToAdd.Clear();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {processedCount + 1} error: {ex.Message}");
                    _logger.LogWarning("⚠️ [DT_KHKD1_ERROR] Lỗi xử lý dòng {RowNum}: {Error}", processedCount + 1, ex.Message);
                }
            }

            // Insert remaining entities
            if (entitiesToAdd.Any())
            {
                _context.DT_KHKD1s.AddRange(entitiesToAdd);
                await _context.SaveChangesAsync();
                _logger.LogInformation("💾 [DT_KHKD1_FINAL] Đã lưu {Count} bản ghi cuối cùng vào database", entitiesToAdd.Count);
            }

            result.Success = true;
            result.ProcessedRecords = processedCount;
            result.Message = $"Successfully processed {processedCount} DT_KHKD1 records to database";

            _logger.LogInformation("✅ [DT_KHKD1_COMPLETE] Đã xử lý xong {Count} bản ghi DT_KHKD1 với BatchId: {BatchId}",
                processedCount, batchId);
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

        /// <summary>
        /// Extract NgayDL từ filename theo pattern *yyyymmdd.csv và format về dd/MM/yyyy
        /// </summary>
        private string ExtractNgayDLFromFileName(string fileName)
        {
            _logger.LogInformation("🔍 [EXTRACT_DEBUG] Bắt đầu extract NgayDL từ filename: {FileName}", fileName);

            try
            {
                // Pattern để extract date: *20241231.csv -> 31/12/2024
                var datePattern = @"(\d{4})(\d{2})(\d{2})";
                var match = Regex.Match(fileName, datePattern);

                _logger.LogInformation("🔍 [EXTRACT_DEBUG] Pattern: {Pattern}, Match: {Success}, Groups: {GroupCount}",
                    datePattern, match.Success, match.Groups.Count);

                if (match.Success && match.Groups.Count >= 4)
                {
                    var year = match.Groups[1].Value;
                    var month = match.Groups[2].Value;
                    var day = match.Groups[3].Value;

                    _logger.LogInformation("🔍 [EXTRACT_DEBUG] Extracted: Year={Year}, Month={Month}, Day={Day}", year, month, day);

                    // Validate và parse date từ format yyyymmdd
                    if (DateTime.TryParseExact($"{year}-{month}-{day}", "yyyy-MM-dd", null,
                        System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        var formattedDate = parsedDate.ToString("dd/MM/yyyy");
                        _logger.LogInformation("📅 [EXTRACT_DATE] Filename: {FileName} -> {Year}-{Month}-{Day} -> {FormattedDate}",
                            fileName, year, month, day, formattedDate);
                        return formattedDate;
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ [EXTRACT_DEBUG] Parse date failed for {Year}-{Month}-{Day}", year, month, day);
                    }
                }

                // Fallback to current date if pattern not found
                var fallback = DateTime.Now.ToString("dd/MM/yyyy");
                _logger.LogWarning("⚠️ [EXTRACT_DATE] Không extract được date từ filename {FileName}, pattern not match, dùng fallback: {Date}", fileName, fallback);
                return fallback;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ Không thể extract NgayDL từ filename {FileName}: {Error}", fileName, ex.Message);
                return DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        #endregion
    }
}
