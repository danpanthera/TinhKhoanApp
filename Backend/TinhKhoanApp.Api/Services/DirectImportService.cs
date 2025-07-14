using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Reflection;
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
                    "LN03" => await ImportLN03DirectAsync(file, statementDate),
                    "GL01" => await ImportGL01DirectAsync(file, statementDate),
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
        /// </summary>
        public async Task<DirectImportResult> ImportDP01DirectAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [DP01_DIRECT] Import vào bảng DP01");
            return await ImportGenericCSVAsync<TinhKhoanApp.Api.Models.DataTables.DP01>("DP01", "DP01", file, statementDate);
        }

        /// <summary>
        /// Import LN01 - Loan data
        /// </summary>
        public async Task<DirectImportResult> ImportLN01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<LN01>("LN01", "LN01", file, statementDate);
        }

        /// <summary>
        /// Import LN03 - Bad debt data
        /// </summary>
        public async Task<DirectImportResult> ImportLN03DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<LN03>("LN03", "LN03", file, statementDate);
        }

        /// <summary>
        /// Import GL01 - Bảng đặc biệt với filename range và TR_TIME làm NGAY_DL
        /// </summary>
        public async Task<DirectImportResult> ImportGL01DirectAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [GL01_SPECIAL] Import vào bảng GL01 với xử lý đặc biệt");
            return await ImportGL01SpecialAsync(file, statementDate);
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
        /// Import RR01 - Risk rating data
        /// </summary>
        public async Task<DirectImportResult> ImportRR01DirectAsync(IFormFile file, string? statementDate = null)
        {
            return await ImportGenericCSVAsync<RR01>("RR01", "RR01", file, statementDate);
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
            if (upperFileName.Contains("LN03")) return "LN03";
            if (upperFileName.Contains("GL01")) return "GL01";
            if (upperFileName.Contains("GL41")) return "GL41";
            if (upperFileName.Contains("DPDA")) return "DPDA";
            if (upperFileName.Contains("EI01")) return "EI01";
            if (upperFileName.Contains("RR01")) return "RR01";

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

            _logger.LogInformation("💾 [BULK_INSERT] IMPORTANT: Table: {TableName}, DataTable columns: {Columns}",
                tableName, string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = tableName,
                BatchSize = 10000,
                BulkCopyTimeout = 300
            };

            _logger.LogWarning("🎯 [BULK_INSERT] DEFINITELY inserting into table: {TableName}", tableName);

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
                                    targetDate = parsedDate.ToString("dd/MM/yyyy");
                                    _logger.LogInformation("📅 Extracted date from filename: {Date}", targetDate);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(targetDate))
                        {
                            // Filter theo NGAY_DL với ngày extracted từ filename
                            sql = $@"SELECT TOP 20 * FROM {tableName}
                                   WHERE NGAY_DL = @targetDate
                                   ORDER BY ID ASC";
                            countSql = $@"SELECT COUNT(*) FROM {tableName}
                                        WHERE NGAY_DL = @targetDate";
                        }
                        else
                        {
                            // Fallback: Lấy dữ liệu theo ImportDate
                            var importDateStr = importRecord.ImportDate.ToString("dd/MM/yyyy");
                            sql = $@"SELECT TOP 20 * FROM {tableName}
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
                            var fallbackSql = $@"SELECT TOP 20 * FROM {tableName}
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
                    "DP01", "LN01", "LN03", "GL01", "GL41",
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

        #endregion

        #region Data Management Methods

        /// <summary>
        /// Kiểm tra xem dữ liệu có tồn tại cho dataType và date cụ thể
        /// </summary>
        public async Task<DataCheckResult> CheckDataExistsAsync(string dataType, string date)
        {
            var result = new DataCheckResult();
            
            try
            {
                _logger.LogInformation("🔍 Checking data exists for {DataType} on {Date}", dataType, date);

                // Convert date string to NGAY_DL format (dd/MM/yyyy)
                var ngayDL = ConvertDateStringToNgayDL(date);
                
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = $@"
                    SELECT COUNT(*) 
                    FROM [{dataType}] 
                    WHERE NGAY_DL = @NgayDL";

                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add("@NgayDL", SqlDbType.NVarChar).Value = ngayDL;

                var count = (int)await cmd.ExecuteScalarAsync();
                
                result.DataExists = count > 0;
                result.RecordCount = count;
                result.Message = result.DataExists ? 
                    $"Found {count} records for {dataType} on {ngayDL}" : 
                    $"No data found for {dataType} on {ngayDL}";

                _logger.LogInformation("📊 Data check result: {Count} records found", count);
                return result;
            }
            catch (Exception ex)
            {
                result.DataExists = false;
                result.RecordCount = 0;
                result.Message = $"Error checking data: {ex.Message}";
                _logger.LogError(ex, "❌ Error checking data exists for {DataType}", dataType);
                return result;
            }
        }

        /// <summary>
        /// Xóa toàn bộ dữ liệu của một bảng cụ thể
        /// </summary>
        public async Task<DirectImportResult> ClearTableDataAsync(string dataType)
        {
            var result = new DirectImportResult
            {
                DataType = dataType,
                StartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("🗑️ Clearing all data from table: {DataType}", dataType);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // First count records
                var countSql = $"SELECT COUNT(*) FROM [{dataType}]";
                using var countCmd = new SqlCommand(countSql, connection);
                var recordCount = (int)await countCmd.ExecuteScalarAsync();

                if (recordCount == 0)
                {
                    result.Success = true;
                    result.ProcessedRecords = 0;
                    result.Details = $"Table {dataType} is already empty";
                    result.EndTime = DateTime.UtcNow;
                    return result;
                }

                // Clear the table
                var deleteSql = $"DELETE FROM [{dataType}]";
                using var deleteCmd = new SqlCommand(deleteSql, connection);
                var deletedCount = await deleteCmd.ExecuteNonQueryAsync();

                // Also clear from ImportedDataRecords
                var clearImportSql = @"
                    DELETE FROM ImportedDataRecords 
                    WHERE Category = @DataType OR FileType = @DataType";
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
        /// Convert date string (yyyyMMdd) to NGAY_DL format (dd/MM/yyyy)
        /// </summary>
        private string ConvertDateStringToNgayDL(string dateStr)
        {
            try
            {
                if (dateStr.Length == 8) // yyyyMMdd
                {
                    var year = dateStr.Substring(0, 4);
                    var month = dateStr.Substring(4, 2);
                    var day = dateStr.Substring(6, 2);
                    return $"{day}/{month}/{year}";
                }
                return dateStr; // Return as-is if not in expected format
            }
            catch
            {
                return dateStr; // Return as-is if conversion fails
            }
        }

        #endregion

        #region GL41 Structure Fix

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

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var messages = new List<string>();

                // 1. Rename SO_TK to MA_TK if exists
                var checkSOTK = @"
                    SELECT COUNT(*) FROM sys.columns
                    WHERE object_id = OBJECT_ID('GL41') AND name = 'SO_TK'";

                using (var cmd = new SqlCommand(checkSOTK, connection))
                {
                    var existsResult = await cmd.ExecuteScalarAsync();
                    int exists = existsResult != null ? (int)existsResult : 0;
                    if (exists > 0)
                    {
                        var renameSql = "EXEC sp_rename 'GL41.SO_TK', 'MA_TK', 'COLUMN'";
                        using var renameCmd = new SqlCommand(renameSql, connection);
                        await renameCmd.ExecuteNonQueryAsync();
                        messages.Add("✅ Renamed SO_TK to MA_TK");
                        _logger.LogInformation("✅ Renamed SO_TK to MA_TK");
                    }
                    else
                    {
                        messages.Add("ℹ️ SO_TK column not found or already renamed");
                        _logger.LogInformation("ℹ️ SO_TK column not found or already renamed");
                    }
                }

                // 2. Add LOAI_TIEN column if not exists
                var checkLOAI_TIEN = @"
                    SELECT COUNT(*) FROM sys.columns
                    WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_TIEN'";

                using (var cmd = new SqlCommand(checkLOAI_TIEN, connection))
                {
                    var existsResult = await cmd.ExecuteScalarAsync();
                    int exists = existsResult != null ? (int)existsResult : 0;
                    if (exists == 0)
                    {
                        var addSql = "ALTER TABLE GL41 ADD LOAI_TIEN NVARCHAR(50)";
                        using var addCmd = new SqlCommand(addSql, connection);
                        await addCmd.ExecuteNonQueryAsync();
                        messages.Add("✅ Added LOAI_TIEN column");
                        _logger.LogInformation("✅ Added LOAI_TIEN column");
                    }
                    else
                    {
                        messages.Add("ℹ️ LOAI_TIEN column already exists");
                        _logger.LogInformation("ℹ️ LOAI_TIEN column already exists");
                    }
                }

                // 3. Add LOAI_BT column if not exists
                var checkLOAI_BT = @"
                    SELECT COUNT(*) FROM sys.columns
                    WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_BT'";

                using (var cmd = new SqlCommand(checkLOAI_BT, connection))
                {
                    var existsResult = await cmd.ExecuteScalarAsync();
                    int exists = existsResult != null ? (int)existsResult : 0;
                    if (exists == 0)
                    {
                        var addSql = "ALTER TABLE GL41 ADD LOAI_BT NVARCHAR(50)";
                        using var addCmd = new SqlCommand(addSql, connection);
                        await addCmd.ExecuteNonQueryAsync();
                        messages.Add("✅ Added LOAI_BT column");
                        _logger.LogInformation("✅ Added LOAI_BT column");
                    }
                    else
                    {
                        messages.Add("ℹ️ LOAI_BT column already exists");
                        _logger.LogInformation("ℹ️ LOAI_BT column already exists");
                    }
                }

                // 4. Verify final structure
                var verifySql = @"
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'GL41'";

                using (var cmd = new SqlCommand(verifySql, connection))
                {
                    var countResult = await cmd.ExecuteScalarAsync();
                    int columnCount = countResult != null ? (int)countResult : 0;
                    messages.Add($"📊 GL41 now has {columnCount} columns");
                    _logger.LogInformation("📊 GL41 now has {ColumnCount} columns", columnCount);

                    result.ProcessedRecords = columnCount;
                }

                result.Success = true;
                result.Details = string.Join("\n", messages);
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("🎉 GL41 structure fix completed successfully");
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

        #endregion

        #region GL01 Special Processing

        /// <summary>
        /// Import GL01 đặc biệt - filename có range ngày, NGAY_DL từ TR_TIME column
        /// VD: 7800_gl01_2025050120250531.csv (từ 01/05/2025 -> 31/05/2025)
        /// </summary>
        private async Task<DirectImportResult> ImportGL01SpecialAsync(IFormFile file, string? statementDate = null)
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
                _logger.LogInformation("🚀 [GL01_SPECIAL] Starting GL01 special import: {FileName}", file.FileName);

                // Parse filename to extract date range
                var dateRange = ParseGL01DateRange(file.FileName);
                if (dateRange == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Không thể parse date range từ filename: {file.FileName}. Expected format: 7800_gl01_YYYYMMDDYYYYMMDD.csv";
                    return result;
                }

                _logger.LogInformation("📅 [GL01_SPECIAL] Date range: {FromDate} -> {ToDate}", dateRange.FromDate, dateRange.ToDate);

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = new List<GL01>();
                var dataTable = new DataTable();
                var processedRows = 0;
                var errorRows = 0;

                // Setup DataTable structure based on GL01 model
                SetupGL01DataTable(dataTable);

                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var record = new GL01();
                        
                        // Map all CSV columns to model properties
                        MapGL01Record(csv, record);

                        // SPECIAL: Convert TR_TIME to NGAY_DL format
                        if (!string.IsNullOrEmpty(record.TR_TIME))
                        {
                            record.NGAY_DL = ConvertTrTimeToNgayDL(record.TR_TIME);
                        }

                        // Add to DataTable for bulk insert
                        var row = dataTable.NewRow();
                        PopulateGL01DataRow(row, record);
                        dataTable.Rows.Add(row);

                        processedRows++;
                    }
                    catch (Exception ex)
                    {
                        errorRows++;
                        _logger.LogWarning("⚠️ [GL01_SPECIAL] Error processing row {Row}: {Error}", processedRows + errorRows, ex.Message);
                    }
                }

                // Bulk insert using SqlBulkCopy
                if (dataTable.Rows.Count > 0)
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    using var bulkCopy = new SqlBulkCopy(connection);
                    bulkCopy.DestinationTableName = "GL01";
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.BulkCopyTimeout = 300;

                    // Map columns
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                // Save import record
                await SaveImportRecord(result, dateRange.FromDate, dateRange.ToDate);

                result.Success = true;
                result.ProcessedRecords = processedRows;
                result.ErrorRecords = errorRows;
                result.NgayDL = $"{dateRange.FromDate} - {dateRange.ToDate}";
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ [GL01_SPECIAL] Import completed: {ProcessedRecords} records", processedRows);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.UtcNow;
                _logger.LogError(ex, "❌ [GL01_SPECIAL] Import failed: {FileName}", file.FileName);
                return result;
            }
        }

        /// <summary>
        /// Parse GL01 filename để extract date range
        /// VD: 7800_gl01_2025050120250531.csv -> FromDate: 01/05/2025, ToDate: 31/05/2025
        /// </summary>
        private (string FromDate, string ToDate)? ParseGL01DateRange(string fileName)
        {
            try
            {
                var pattern = @"_gl01_(\d{8})(\d{8})\.csv$";
                var match = Regex.Match(fileName.ToLower(), pattern);
                
                if (!match.Success)
                {
                    return null;
                }

                var fromDateStr = match.Groups[1].Value; // YYYYMMDD
                var toDateStr = match.Groups[2].Value;   // YYYYMMDD

                var fromDate = ConvertDateStringToNgayDL(fromDateStr);
                var toDate = ConvertDateStringToNgayDL(toDateStr);

                return (fromDate, toDate);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert TR_TIME (01-MAY-2025) to NGAY_DL format (01/05/2025)
        /// </summary>
        private string ConvertTrTimeToNgayDL(string trTime)
        {
            try
            {
                // Parse "01-MAY-2025" format
                if (DateTime.TryParseExact(trTime, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date.ToString("dd/MM/yyyy");
                }

                // Fallback: try other common formats
                if (DateTime.TryParse(trTime, out DateTime fallbackDate))
                {
                    return fallbackDate.ToString("dd/MM/yyyy");
                }

                return trTime; // Return as-is if conversion fails
            }
            catch
            {
                return trTime;
            }
        }

        /// <summary>
        /// Setup DataTable structure for GL01
        /// </summary>
        private void SetupGL01DataTable(DataTable dataTable)
        {
            var properties = typeof(GL01).GetProperties()
                .Where(p => !p.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                .ToList();

            foreach (var prop in properties)
            {
                var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (columnType == typeof(string))
                {
                    dataTable.Columns.Add(prop.Name, typeof(string));
                }
                else if (columnType == typeof(int))
                {
                    dataTable.Columns.Add(prop.Name, typeof(int));
                }
                else if (columnType == typeof(decimal))
                {
                    dataTable.Columns.Add(prop.Name, typeof(decimal));
                }
                else if (columnType == typeof(DateTime))
                {
                    dataTable.Columns.Add(prop.Name, typeof(DateTime));
                }
                else
                {
                    dataTable.Columns.Add(prop.Name, typeof(string));
                }
            }
        }

        /// <summary>
        /// Map CSV record to GL01 object
        /// </summary>
        private void MapGL01Record(CsvReader csv, GL01 record)
        {
            // TODO: Implement actual column mapping based on CSV structure
            // This will need to be customized based on the actual GL01 CSV columns
            
            // Example mapping (adjust based on actual CSV columns):
            record.NGAY_DL = csv.GetField("NGAY_DL") ?? "";
            record.MA_CN = csv.GetField("MA_CN") ?? "";
            record.TR_TIME = csv.GetField("TR_TIME") ?? "";
            // ... map other columns as needed
        }

        /// <summary>
        /// Populate DataTable row with GL01 data
        /// </summary>
        private void PopulateGL01DataRow(DataRow row, GL01 record)
        {
            var properties = typeof(GL01).GetProperties()
                .Where(p => !p.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                .ToList();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(record);
                row[prop.Name] = value ?? DBNull.Value;
            }
        }

        #endregion
    }
}
