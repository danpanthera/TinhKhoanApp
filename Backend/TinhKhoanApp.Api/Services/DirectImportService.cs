using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
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
    public class DirectImportService : IDirectImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DirectImportService> _logger;
        private readonly string _connectionString;

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
                    "LN02" => await ImportLN02DirectAsync(file, statementDate),
                    "LN03" => await ImportLN03DirectAsync(file, statementDate),
                    "DB01" => await ImportDB01DirectAsync(file, statementDate),
                    "GL01" => await ImportGL01DirectAsync(file, statementDate),
                    "GL41" => await ImportGL41DirectAsync(file, statementDate),
                    "DPDA" => await ImportDPDADirectAsync(file, statementDate),
                    "EI01" => await ImportEI01DirectAsync(file, statementDate),
                    "KH03" => await ImportKH03DirectAsync(file, statementDate),
                    "RR01" => await ImportRR01DirectAsync(file, statementDate),
                    "DT_KHKD1" => await ImportDT_KHKD1DirectAsync(file, statementDate),
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
        /// </summary>
        public async Task<DirectImportResult> ImportDP01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<TinhKhoanApp.Api.Models.DataTables.DP01>("DP01", "DP01_New", file, statementDate);
        }

        /// <summary>
        /// Import LN01 - Loan data
        /// </summary>
        public async Task<DirectImportResult> ImportLN01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<LN01>("LN01", "LN01", file, statementDate);
        }

        /// <summary>
        /// Import LN02 - Loan payment schedule
        /// </summary>
        public async Task<DirectImportResult> ImportLN02DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<LN02>("LN02", "LN02", file, statementDate);
        }

        /// <summary>
        /// Import LN03 - Bad debt data
        /// </summary>
        public async Task<DirectImportResult> ImportLN03DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<LN03>("LN03", "LN03", file, statementDate);
        }

        /// <summary>
        /// Import DB01 - Deposit data
        /// </summary>
        public async Task<DirectImportResult> ImportDB01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<DB01>("DB01", "DB01", file, statementDate);
        }

        /// <summary>
        /// Import GL01 - General ledger
        /// </summary>
        public async Task<DirectImportResult> ImportGL01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<GL01>("GL01", "GL01", file, statementDate);
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
            return await ImportGenericCSVAsync<DPDA>("DPDA", "DPDA", file, statementDate);
        }

        /// <summary>
        /// Import EI01 - Exchange rate data
        /// </summary>
        public async Task<DirectImportResult> ImportEI01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<EI01>("EI01", "EI01", file, statementDate);
        }

        /// <summary>
        /// Import KH03 - Customer data
        /// </summary>
        public async Task<DirectImportResult> ImportKH03DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<KH03>("KH03", "KH03", file, statementDate);
        }

        /// <summary>
        /// Import RR01 - Risk rating data
        /// </summary>
        public async Task<DirectImportResult> ImportRR01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<RR01>("RR01", "RR01", file, statementDate);
        }

        /// <summary>
        /// Import DT_KHKD1 - Business plan data (CSV for testing)
        /// </summary>
        public async Task<DirectImportResult> ImportDT_KHKD1DirectAsync(IFormFile file, string? statementDate = null)
        {
            // Temporary: Use CSV import for testing (should be Excel eventually)
            return await ImportGenericCSVAsync<DT_KHKD1>("DT_KHKD1", "7800_DT_KHKD1", file, statementDate);
        }

        #endregion

        #region Generic Import Methods

        /// <summary>
        /// Generic CSV import method
        /// </summary>
        private async Task<DirectImportResult> ImportGenericCSVAsync<T>(string dataType, string tableName, IFormFile file, string? statementDate = null)
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
                    var insertedCount = await BulkInsertGenericAsync(records, tableName);
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

        #endregion

        #region Helper Methods

        /// <summary>
        /// Detect data type từ filename
        /// </summary>
        private string? DetectDataTypeFromFileName(string fileName)
        {
            var upperFileName = fileName.ToUpper();

            // Priority order detection
            if (upperFileName.Contains("DP01")) return "DP01";
            if (upperFileName.Contains("LN01")) return "LN01";
            if (upperFileName.Contains("LN02")) return "LN02";
            if (upperFileName.Contains("LN03")) return "LN03";
            if (upperFileName.Contains("DB01")) return "DB01";
            if (upperFileName.Contains("GL01")) return "GL01";
            if (upperFileName.Contains("GL41")) return "GL41";
            if (upperFileName.Contains("DPDA")) return "DPDA";
            if (upperFileName.Contains("EI01")) return "EI01";
            if (upperFileName.Contains("KH03")) return "KH03";
            if (upperFileName.Contains("RR01")) return "RR01";
            if (upperFileName.Contains("DT_KHKD1")) return "DT_KHKD1";

            return null;
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
        /// Generic CSV parsing method
        /// </summary>
        private async Task<List<T>> ParseGenericCSVAsync<T>(IFormFile file, string? statementDate = null)
            where T : class, new()
        {
            var records = new List<T>();
            var ngayDL = ExtractNgayDLFromFileName(file.FileName);

            _logger.LogInformation("🔍 [CSV_PARSE] Bắt đầu parse CSV: {FileName}, Target Type: {TypeName}", file.FileName, typeof(T).Name);

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Configure CSV để bỏ qua missing fields và handle auto-increment fields
            csv.Context.Configuration.MissingFieldFound = null; // Bỏ qua fields không tồn tại
            csv.Context.Configuration.HeaderValidated = null; // Bỏ qua validation header
            csv.Context.Configuration.PrepareHeaderForMatch = args => args.Header.ToUpper(); // Case insensitive

            // Auto-configure CSV mapping
            csv.Read();
            csv.ReadHeader();

            // Log headers để debug
            var headers = csv.HeaderRecord;
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
                try
                {
                    var record = new T();

                    // Manual mapping chỉ các fields có trong CSV headers
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        // Bỏ qua auto-increment và system fields
                        if (prop.Name == "Id" || prop.Name == "CreatedDate" || prop.Name == "UpdatedDate")
                            continue;

                        // Tìm header tương ứng (case insensitive)
                        var headerName = headers?.FirstOrDefault(h =>
                            string.Equals(h, prop.Name, StringComparison.OrdinalIgnoreCase));

                        if (!string.IsNullOrEmpty(headerName))
                        {
                            try
                            {
                                var value = csv.GetField(headerName);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    // Convert value based on property type
                                    var convertedValue = ConvertCsvValue(value, prop.PropertyType);
                                    if (convertedValue != null)
                                    {
                                        prop.SetValue(record, convertedValue);
                                    }
                                }
                            }
                            catch (Exception fieldEx)
                            {
                                _logger.LogWarning("⚠️ [CSV_PARSE] Field mapping error for {PropertyName}: {Error}", prop.Name, fieldEx.Message);
                            }
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
        /// Set common properties for all entities
        /// </summary>
        private void SetCommonProperties<T>(T record, string ngayDL, string fileName)
        {
            var type = typeof(T);

            // Set NgayDL if property exists
            var ngayDLProp = type.GetProperty("NgayDL");
            if (ngayDLProp != null && ngayDLProp.CanWrite)
            {
                ngayDLProp.SetValue(record, ngayDL);
            }

            // Set FileName if property exists
            var fileNameProp = type.GetProperty("FileName");
            if (fileNameProp != null && fileNameProp.CanWrite)
            {
                fileNameProp.SetValue(record, fileName);
            }

            // Set CreatedDate if property exists
            var createdDateProp = type.GetProperty("CreatedDate");
            if (createdDateProp != null && createdDateProp.CanWrite)
            {
                createdDateProp.SetValue(record, DateTime.Now);
            }
        }

        /// <summary>
        /// Generic bulk insert method
        /// </summary>
        private async Task<int> BulkInsertGenericAsync<T>(List<T> records, string tableName)
        {
            if (!records.Any()) return 0;

            var dataTable = ConvertToDataTable(records);

            _logger.LogInformation("💾 [BULK_INSERT] Table: {TableName}, DataTable columns: {Columns}",
                tableName, string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = tableName,
                BatchSize = 10000,
                BulkCopyTimeout = 300
            };

            // Auto-map columns
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                _logger.LogInformation("💾 [BULK_MAPPING] {SourceColumn} -> {DestColumn}", column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable);

            _logger.LogInformation("💾 [{TableName}_BULK] Bulk insert hoàn thành: {Count} records", tableName, records.Count);
            return records.Count;
        }

        /// <summary>
        /// Convert generic list to DataTable - sử dụng Column attribute names
        /// </summary>
        private DataTable ConvertToDataTable<T>(List<T> records)
        {
            var table = new DataTable();
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "UpdatedDate") // Chỉ bỏ qua Id và UpdatedDate, giữ lại CreatedDate
                .ToArray();

            var columnMappings = new Dictionary<string, string>(); // PropertyName -> ColumnName

            _logger.LogInformation("📊 [DATATABLE] Creating DataTable with columns: {Columns}",
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
                    row[columnName] = value ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            _logger.LogInformation("📊 [DATATABLE] Created DataTable: {RowCount} rows, {ColumnCount} columns",
                table.Rows.Count, table.Columns.Count);

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
        /// Convert CSV string value to proper type với number formatting chuẩn
        /// </summary>
        private object? ConvertCsvValue(string csvValue, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(csvValue))
                return null;

            try
            {
                // Handle nullable types
                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (underlyingType == typeof(string))
                {
                    return csvValue.Trim();
                }
                else if (underlyingType == typeof(decimal))
                {
                    // Chuẩn hóa format số: ngăn cách hàng nghìn = dấu phẩy, thập phân = dấu chấm
                    var normalizedValue = csvValue.Replace(",", "").Trim(); // Bỏ dấu phẩy ngăn cách hàng nghìn
                    return decimal.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalResult)
                        ? decimalResult : (decimal?)null;
                }
                else if (underlyingType == typeof(int))
                {
                    var normalizedValue = csvValue.Replace(",", "").Trim();
                    return int.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var intResult)
                        ? intResult : (int?)null;
                }
                else if (underlyingType == typeof(long))
                {
                    var normalizedValue = csvValue.Replace(",", "").Trim();
                    return long.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var longResult)
                        ? longResult : (long?)null;
                }
                else if (underlyingType == typeof(double))
                {
                    var normalizedValue = csvValue.Replace(",", "").Trim();
                    return double.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var doubleResult)
                        ? doubleResult : (double?)null;
                }
                else if (underlyingType == typeof(DateTime))
                {
                    return DateTime.TryParse(csvValue, out var dateResult) ? dateResult : (DateTime?)null;
                }
                else if (underlyingType == typeof(bool))
                {
                    return bool.TryParse(csvValue, out var boolResult) ? boolResult : (bool?)null;
                }
                else
                {
                    // Fallback: try direct conversion
                    return Convert.ChangeType(csvValue.Trim(), underlyingType, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ [CSV_CONVERT] Cannot convert '{Value}' to {TargetType}: {Error}", csvValue, targetType.Name, ex.Message);
                return null;
            }
        }

        #endregion

        #region Preview and Delete Methods

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

                // Trả về thông tin cơ bản (preview rows sẽ trống vì DirectImport không lưu JSON)
                return new
                {
                    ImportId = importRecord.Id,
                    FileName = importRecord.FileName,
                    Category = importRecord.Category,
                    ImportDate = importRecord.ImportDate,
                    TotalRecords = importRecord.RecordsCount,
                    PreviewRows = new List<object>() // DirectImport không lưu raw data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting preview data for import {ImportId}", importId);
                throw;
            }
        }

        /// <summary>
        /// Xóa import record và dữ liệu liên quan
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

                // Xóa import record từ ImportedDataRecords
                _context.ImportedDataRecords.Remove(importRecord);
                var deletedRecords = await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Successfully deleted import record {ImportId}, deleted {RecordsDeleted} records",
                    importId, deletedRecords);

                return (true, "Import record deleted successfully", deletedRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting import record {ImportId}", importId);
                return (false, $"Error deleting import: {ex.Message}", 0);
            }
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

        #endregion
    }
}
