using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Smart Data Import Service - Automatically routes files to correct data tables
    /// based on filename patterns and provides centralized import processing
    /// </summary>
    public interface ISmartDataImportService
    {
        Task<SmartImportResult> ImportFileSmartAsync(IFormFile file, string? category = null);
        Task<SmartImportResult> ProcessImportedRecordToTablesAsync(int importedDataRecordId);
        Task<List<SmartImportResult>> ImportMultipleFilesSmartAsync(List<IFormFile> files);
        Task<TableRoutingInfo> AnalyzeFileRoutingAsync(string fileName, string? existingCategory = null);
        Task<List<string>> GetAvailableDataTablesAsync();
        Task<TableVerificationResult> VerifyTemporalTablesAsync();
        Task<TableVerificationResult> SetupMissingTemporalTablesAsync();
    }

    public class SmartDataImportService : ISmartDataImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRawDataProcessingService _processingService;
        private readonly ILogger<SmartDataImportService> _logger;

        public SmartDataImportService(
            ApplicationDbContext context,
            IRawDataProcessingService processingService,
            ILogger<SmartDataImportService> logger)
        {
            _context = context;
            _processingService = processingService;
            _logger = logger;
        }

        /// <summary>
        /// Smart import that automatically routes files to correct data tables
        /// </summary>
        public async Task<SmartImportResult> ImportFileSmartAsync(IFormFile file, string? category = null)
        {
            var result = new SmartImportResult
            {
                FileName = file.FileName,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // 1. Analyze file to determine routing
                var routingInfo = await AnalyzeFileRoutingAsync(file.FileName);
                result.RoutingInfo = routingInfo;

                if (!routingInfo.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not determine data table for file: {file.FileName}";
                    return result;
                }

                // 2. Import file to ImportedDataRecords first (existing process)
                var importedRecord = await ImportToRawDataAsync(file, routingInfo.DeterminedCategory);
                result.ImportedDataRecordId = importedRecord.Id;

                // 3. Process to specific data table
                var processingResult = await _processingService.ProcessImportedDataToHistoryAsync(
                    importedRecord.Id,
                    routingInfo.DeterminedCategory,
                    routingInfo.StatementDate);

                result.Success = processingResult.Success;
                result.ProcessedRecords = processingResult.ProcessedRecords;
                result.ErrorMessage = processingResult.Success ? null : string.Join("; ", processingResult.Errors);
                result.TargetTable = routingInfo.TargetDataTable;
                result.BatchId = processingResult.BatchId;

                _logger.LogInformation("✅ Smart import completed for {FileName} -> {TargetTable} ({ProcessedRecords} records)",
                    file.FileName, routingInfo.TargetDataTable, processingResult.ProcessedRecords);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "❌ Smart import failed for {FileName}", file.FileName);
            }

            result.EndTime = DateTime.UtcNow;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        /// <summary>
        /// Process an existing ImportedDataRecord to specific data tables
        /// </summary>
        public async Task<SmartImportResult> ProcessImportedRecordToTablesAsync(int importedDataRecordId)
        {
            var result = new SmartImportResult { StartTime = DateTime.UtcNow };

            try
            {
                var record = await _context.ImportedDataRecords
                    .FirstOrDefaultAsync(r => r.Id == importedDataRecordId);

                if (record == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"ImportedDataRecord with ID {importedDataRecordId} not found";
                    return result;
                }

                result.FileName = record.FileName;
                result.ImportedDataRecordId = importedDataRecordId;

                // Analyze routing based on filename and category
                var routingInfo = await AnalyzeFileRoutingAsync(record.FileName, record.Category);
                result.RoutingInfo = routingInfo;

                if (!routingInfo.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not determine data table for record: {record.FileName}";
                    return result;
                }

                // Process to specific data table
                var processingResult = await _processingService.ProcessImportedDataToHistoryAsync(
                    importedDataRecordId,
                    routingInfo.DeterminedCategory,
                    routingInfo.StatementDate);

                result.Success = processingResult.Success;
                result.ProcessedRecords = processingResult.ProcessedRecords;
                result.ErrorMessage = processingResult.Success ? null : string.Join("; ", processingResult.Errors);
                result.TargetTable = routingInfo.TargetDataTable;
                result.BatchId = processingResult.BatchId;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "❌ Processing failed for ImportedDataRecord ID {ImportedDataRecordId}", importedDataRecordId);
            }

            result.EndTime = DateTime.UtcNow;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        /// <summary>
        /// Import multiple files with smart routing
        /// </summary>
        public async Task<List<SmartImportResult>> ImportMultipleFilesSmartAsync(List<IFormFile> files)
        {
            var results = new List<SmartImportResult>();

            foreach (var file in files)
            {
                var result = await ImportFileSmartAsync(file);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Analyze file name to determine routing information
        /// </summary>
        public async Task<TableRoutingInfo> AnalyzeFileRoutingAsync(string fileName, string? existingCategory = null)
        {
            var routingInfo = new TableRoutingInfo
            {
                FileName = fileName,
                IsValid = false
            };

            try
            {
                // Pattern 1: Standard format: BranchCode_DataType_YYYYMMDD.ext
                // Example: 7800_DP01_20241231.csv, 7808_LN01_20241130.xlsx
                var standardPattern = @"^(\d{4})_([A-Za-z0-9]+)_(\d{8})\.(.+)$";
                var match = Regex.Match(fileName, standardPattern);

                if (match.Success)
                {
                    routingInfo.BranchCode = match.Groups[1].Value;
                    routingInfo.DataTypeCode = match.Groups[2].Value.ToUpper();
                    routingInfo.DateString = match.Groups[3].Value;
                    routingInfo.FileExtension = match.Groups[4].Value.ToLower();

                    // Parse statement date
                    if (DateTime.TryParseExact(routingInfo.DateString, "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out var date))
                    {
                        routingInfo.StatementDate = date;
                    }

                    routingInfo.IsStandardFormat = true;
                }
                else
                {
                    // Pattern 2: Contains data type anywhere in filename
                    // Example: DP01_data_2024.csv, LN01_report.xlsx
                    routingInfo.DataTypeCode = ExtractDataTypeFromFileName(fileName);
                    routingInfo.IsStandardFormat = false;
                }

                // Use existing category if provided
                if (!string.IsNullOrEmpty(existingCategory))
                {
                    routingInfo.DataTypeCode = existingCategory.ToUpper();
                }

                // Map to target table and category
                if (!string.IsNullOrEmpty(routingInfo.DataTypeCode))
                {
                    var mapping = GetDataTableMapping(routingInfo.DataTypeCode);
                    routingInfo.TargetDataTable = mapping.TableName;
                    routingInfo.DeterminedCategory = mapping.Category;
                    routingInfo.Description = mapping.Description;
                    routingInfo.IsValid = !string.IsNullOrEmpty(mapping.TableName);

                    // Extract NgayDL từ filename pattern
                    var extractedNgayDL = ExtractNgayDLFromFileName(fileName);
                    _logger.LogInformation("📅 Extracted NgayDL: {NgayDL} from filename: {FileName}", extractedNgayDL, fileName);
                }

                _logger.LogInformation("📋 File routing analysis: {FileName} -> {DataType} -> {TargetTable} (Valid: {IsValid})",
                    fileName, routingInfo.DataTypeCode, routingInfo.TargetDataTable, routingInfo.IsValid);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error analyzing file routing for {FileName}", fileName);
                routingInfo.ErrorMessage = ex.Message;
            }

            return routingInfo;
        }

        /// <summary>
        /// Get list of available data tables that can be imported to
        /// </summary>
        public async Task<List<string>> GetAvailableDataTablesAsync()
        {
            return await _processingService.GetValidCategoriesAsync();
        }

        /// <summary>
        /// Verify that all required temporal tables and columnstore indexes exist
        /// </summary>
        public async Task<TableVerificationResult> VerifyTemporalTablesAsync()
        {
            var result = new TableVerificationResult
            {
                CheckTime = DateTime.UtcNow,
                TablesChecked = new List<TableStatus>()
            };

            try
            {
                var dataTableTypes = new[]
                {
                    "DP01", "LN01", "LN02", "LN03", "GL01", "GL41",
                    "DB01", "DPDA", "EI01", "KH03", "RR01", "DT_KHKD1"
                };

                foreach (var tableType in dataTableTypes)
                {
                    var status = new TableStatus
                    {
                        TableName = tableType,
                        Exists = await CheckTableExistsAsync(tableType),
                        HasTemporalTables = await CheckTemporalTablesAsync(tableType),
                        HasColumnstoreIndex = await CheckColumnstoreIndexAsync(tableType)
                    };

                    result.TablesChecked.Add(status);
                }

                result.AllTablesValid = result.TablesChecked.All(t => t.Exists && t.HasTemporalTables && t.HasColumnstoreIndex);
                result.MissingTables = result.TablesChecked.Where(t => !t.Exists).Select(t => t.TableName).ToList();
                result.MissingTemporalTables = result.TablesChecked.Where(t => t.Exists && !t.HasTemporalTables).Select(t => t.TableName).ToList();
                result.MissingColumnstoreIndexes = result.TablesChecked.Where(t => t.Exists && !t.HasColumnstoreIndex).Select(t => t.TableName).ToList();

                _logger.LogInformation("📊 Temporal tables verification completed. Valid: {AllValid}, Missing: {MissingCount}",
                    result.AllTablesValid, result.MissingTables.Count + result.MissingTemporalTables.Count + result.MissingColumnstoreIndexes.Count);

            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "❌ Error verifying temporal tables");
            }

            return result;
        }

        /// <summary>
        /// Setup missing temporal tables and columnstore indexes
        /// </summary>
        public async Task<TableVerificationResult> SetupMissingTemporalTablesAsync()
        {
            var result = await VerifyTemporalTablesAsync();

            try
            {
                if (!result.AllTablesValid)
                {
                    _logger.LogInformation("🔧 Setting up missing temporal tables and indexes...");

                    // Execute SQL script to setup missing components
                    var sqlScript = await File.ReadAllTextAsync("complete_temporal_setup.sql");

                    await _context.Database.ExecuteSqlRawAsync(sqlScript);

                    _logger.LogInformation("✅ Temporal tables setup completed");

                    // Re-verify
                    result = await VerifyTemporalTablesAsync();
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Setup failed: {ex.Message}";
                _logger.LogError(ex, "❌ Error setting up temporal tables");
            }

            return result;
        }

        #region Private Helper Methods

        private async Task<ImportedDataRecord> ImportToRawDataAsync(IFormFile file, string category)
        {
            var record = new ImportedDataRecord
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                Category = category,
                ImportDate = DateTime.UtcNow,
                ImportedBy = "SmartImportService",
                Status = "Processing"
            };

            // Store file data
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            record.OriginalFileData = memoryStream.ToArray();

            // Process file content
            var items = new List<ImportedDataItem>();
            var extension = Path.GetExtension(file.FileName).ToLower();

            memoryStream.Position = 0;
            switch (extension)
            {
                case ".csv":
                    items = await ProcessCsvFileAsync(memoryStream, file.FileName);
                    break;
                case ".xlsx":
                case ".xls":
                    items = await ProcessExcelFileAsync(memoryStream, file.FileName);
                    break;
                default:
                    throw new NotSupportedException($"File type {extension} not supported");
            }

            record.RecordsCount = items.Count;
            record.Status = items.Any() ? "Completed" : "Failed";
            record.ImportedDataItems = items;

            _context.ImportedDataRecords.Add(record);
            await _context.SaveChangesAsync();

            return record;
        }

        private async Task<List<ImportedDataItem>> ProcessCsvFileAsync(Stream stream, string fileName = "")
        {
            var items = new List<ImportedDataItem>();

            // Special handling for DT_KHKD1 files which start from row 13 and have no header
            bool isDT_KHKD1 = fileName.Contains("DT_KHKD1", StringComparison.OrdinalIgnoreCase);

            using var reader = new StreamReader(stream);

            // For DT_KHKD1, skip first 12 rows and use predefined headers
            if (isDT_KHKD1)
            {
                _logger.LogInformation("📄 Special handling for DT_KHKD1 file: {FileName} - Skipping first 12 rows", fileName);

                // Skip first 12 rows
                for (int i = 0; i < 12; i++)
                {
                    await reader.ReadLineAsync();
                }

                // Use predefined headers for DT_KHKD1
                var dt_khkd1_headers = new[] {
                    "BRCD", "BRANCH_NAME", "INDICATOR_TYPE", "INDICATOR_NAME",
                    "PLAN_YEAR", "PLAN_QUARTER", "PLAN_MONTH",
                    "ACTUAL_YEAR", "ACTUAL_QUARTER", "ACTUAL_MONTH",
                    "ACHIEVEMENT_RATE", "YEAR", "QUARTER", "MONTH"
                };

                string? lineContent;
                while ((lineContent = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(lineContent)) continue;

                    var values = ParseCsvLine(lineContent);
                    var rowData = new Dictionary<string, object>();

                    for (int i = 0; i < Math.Min(dt_khkd1_headers.Length, values.Count); i++)
                    {
                        rowData[dt_khkd1_headers[i]] = values[i];
                    }

                    items.Add(new ImportedDataItem
                    {
                        RawData = JsonSerializer.Serialize(rowData),
                        ProcessedDate = DateTime.UtcNow
                    });
                }

                return items;
            }

            // Standard processing for normal CSV files
            var firstLine = await reader.ReadLineAsync();
            if (firstLine == null) return items;

            var headers = firstLine.Split(',').Select(h => h.Trim('"').Trim()).ToArray();

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = ParseCsvLine(line);
                var rowData = new Dictionary<string, object>();

                for (int i = 0; i < Math.Min(headers.Length, values.Count); i++)
                {
                    rowData[headers[i]] = values[i];
                }

                items.Add(new ImportedDataItem
                {
                    RawData = JsonSerializer.Serialize(rowData),
                    ProcessedDate = DateTime.UtcNow
                });
            }

            return items;
        }

        private async Task<List<ImportedDataItem>> ProcessExcelFileAsync(Stream stream, string fileName = "")
        {
            var items = new List<ImportedDataItem>();

            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            // Special handling for DT_KHKD1 files which start from row 13 and have no header
            bool isDT_KHKD1 = fileName.Contains("DT_KHKD1", StringComparison.OrdinalIgnoreCase);

            if (isDT_KHKD1)
            {
                _logger.LogInformation("📊 Special handling for DT_KHKD1 Excel file: {FileName} - Starting from row 13", fileName);

                // Use predefined headers for DT_KHKD1
                var dt_khkd1_headers = new[] {
                    "BRCD", "BRANCH_NAME", "INDICATOR_TYPE", "INDICATOR_NAME",
                    "PLAN_YEAR", "PLAN_QUARTER", "PLAN_MONTH",
                    "ACTUAL_YEAR", "ACTUAL_QUARTER", "ACTUAL_MONTH",
                    "ACHIEVEMENT_RATE", "YEAR", "QUARTER", "MONTH"
                };

                // Start from row 13
                for (int row = 13; row <= worksheet.RowsUsed().Count(); row++)
                {
                    var rowData = new Dictionary<string, object>();
                    bool hasData = false;

                    // Read values starting from the first column
                    for (int col = 1; col <= dt_khkd1_headers.Length && col <= worksheet.ColumnsUsed().Count(); col++)
                    {
                        var cellValue = worksheet.Cell(row, col).Value;
                        var stringValue = cellValue.ToString() ?? "";

                        if (col <= dt_khkd1_headers.Length)
                        {
                            rowData[dt_khkd1_headers[col - 1]] = stringValue;

                            if (!string.IsNullOrWhiteSpace(stringValue))
                                hasData = true;
                        }
                    }

                    if (hasData)
                    {
                        items.Add(new ImportedDataItem
                        {
                            RawData = JsonSerializer.Serialize(rowData),
                            ProcessedDate = DateTime.UtcNow
                        });
                    }
                }

                return items;
            }

            // Standard Excel processing for normal files
            var headers = new List<string>();
            var headerRow = worksheet.Row(1);
            for (int col = 1; col <= worksheet.ColumnsUsed().Count(); col++)
            {
                headers.Add(headerRow.Cell(col).GetString());
            }

            for (int row = 2; row <= worksheet.RowsUsed().Count(); row++)
            {
                var rowData = new Dictionary<string, object>();
                bool hasData = false;

                for (int col = 1; col <= headers.Count; col++)
                {
                    var cellValue = worksheet.Cell(row, col).Value;
                    var stringValue = cellValue.ToString() ?? "";
                    rowData[headers[col - 1]] = stringValue;

                    if (!string.IsNullOrWhiteSpace(stringValue))
                        hasData = true;
                }

                if (hasData)
                {
                    items.Add(new ImportedDataItem
                    {
                        RawData = JsonSerializer.Serialize(rowData),
                        ProcessedDate = DateTime.UtcNow
                    });
                }
            }

            return items;
        }

        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var inQuotes = false;
            var currentValue = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString().Trim());
            return values;
        }

        private string ExtractDataTypeFromFileName(string fileName)
        {
            var dataTypes = new[] { "DP01", "LN01", "LN02", "LN03", "GL01", "GL41", "DB01", "DPDA", "EI01", "KH03", "RR01", "DT_KHKD1" };

            foreach (var dataType in dataTypes)
            {
                if (fileName.ToUpper().Contains(dataType))
                {
                    return dataType;
                }
            }

            return string.Empty;
        }

        private (string TableName, string Category, string Description) GetDataTableMapping(string dataTypeCode)
        {
            return dataTypeCode.ToUpper() switch
            {
                "DP01" => ("DP01_New", "DP01", "Báo cáo tiền gửi"),
                "LN01" => ("LN01", "LN01", "Báo cáo tín dụng"),
                "LN02" => ("LN02", "LN02", "Báo cáo kỳ hạn thanh toán"),
                "LN03" => ("LN03", "LN03", "Báo cáo nợ XLRR"),
                "GL01" => ("GL01", "GL01", "Sổ cái tổng hợp"),
                "GL41" => ("GL41", "GL41", "Báo cáo tài khoản"),
                "DB01" => ("DB01", "DB01", "Báo cáo TSDB"),
                "DPDA" => ("DPDA", "DPDA", "Báo cáo phát hành thẻ"),
                "EI01" => ("EI01", "EI01", "Báo cáo Mobile Banking"),
                "KH03" => ("KH03", "KH03", "Báo cáo khách hàng"),
                "RR01" => ("RR01", "RR01", "Báo cáo tỷ lệ"),
                "DT_KHKD1" => ("7800_DT_KHKD1", "DT_KHKD1", "Báo cáo kế hoạch kinh doanh"),
                _ => (string.Empty, string.Empty, string.Empty)
            };
        }

        private async Task<bool> CheckTableExistsAsync(string tableName)
        {
            try
            {
                var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}";
                var count = await _context.Database.SqlQueryRaw<int>(sql, tableName).FirstOrDefaultAsync();
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CheckTemporalTablesAsync(string tableName)
        {
            try
            {
                var sql = @"
                    SELECT COUNT(*)
                    FROM sys.tables t
                    WHERE t.name = {0}
                    AND t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE'";

                var count = await _context.Database.SqlQueryRaw<int>(sql, tableName).FirstOrDefaultAsync();
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CheckColumnstoreIndexAsync(string tableName)
        {
            try
            {
                var sql = @"
                    SELECT COUNT(*)
                    FROM sys.indexes i
                    INNER JOIN sys.tables t ON i.object_id = t.object_id
                    WHERE t.name = {0}
                    AND i.type IN (5, 6)"; // COLUMNSTORE INDEX

                var count = await _context.Database.SqlQueryRaw<int>(sql, tableName).FirstOrDefaultAsync();
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extract NgayDL từ filename theo pattern *yyyymmdd.csv và format về dd/MM/yyyy
        /// </summary>
        private string ExtractNgayDLFromFileName(string fileName)
        {
            try
            {
                // Pattern để extract date: *20241231.csv -> 31/12/2024
                var datePattern = @"(\d{4})(\d{2})(\d{2})";
                var match = Regex.Match(fileName, datePattern);

                if (match.Success && match.Groups.Count >= 4)
                {
                    var year = match.Groups[1].Value;
                    var month = match.Groups[2].Value;
                    var day = match.Groups[3].Value;

                    // Validate date
                    if (DateTime.TryParseExact($"{day}/{month}/{year}", "dd/MM/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        return parsedDate.ToString("dd/MM/yyyy");
                    }
                }

                // Fallback to current date if pattern not found
                return DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ Không thể extract NgayDL từ filename {FileName}: {Error}", fileName, ex.Message);
                return DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        #endregion
    }

    #region DTOs and Result Classes

    public class SmartImportResult
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int? ImportedDataRecordId { get; set; }
        public string? TargetTable { get; set; }
        public string? BatchId { get; set; }
        public int ProcessedRecords { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TableRoutingInfo? RoutingInfo { get; set; }
    }

    public class TableRoutingInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? DataTypeCode { get; set; }
        public string? DateString { get; set; }
        public DateTime? StatementDate { get; set; }
        public string? FileExtension { get; set; }
        public string? TargetDataTable { get; set; }
        public string? DeterminedCategory { get; set; }
        public string? Description { get; set; }
        public bool IsStandardFormat { get; set; }
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class TableVerificationResult
    {
        public DateTime CheckTime { get; set; }
        public bool AllTablesValid { get; set; }
        public List<TableStatus> TablesChecked { get; set; } = new();
        public List<string> MissingTables { get; set; } = new();
        public List<string> MissingTemporalTables { get; set; } = new();
        public List<string> MissingColumnstoreIndexes { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class TableStatus
    {
        public string TableName { get; set; } = string.Empty;
        public bool Exists { get; set; }
        public bool HasTemporalTables { get; set; }
        public bool HasColumnstoreIndex { get; set; }
        public string? ErrorMessage { get; set; }
    }

    #endregion
}
