using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Validation;
using TinhKhoanApp.Api.Services;
using ClosedXML.Excel;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RawDataController> _logger;

        // 📋 Danh sách định nghĩa loại dữ liệu
        private static readonly Dictionary<string, string> DataTypeDefinitions = new()
        {
            { "LN01", "Dữ liệu LOAN" },
            { "LN03", "Dữ liệu Nợ XLRR" },
            { "DP01", "Dữ liệu Tiền gửi" },
            { "EI01", "Dữ liệu mobile banking" },
            { "GL01", "Dữ liệu bút toán GDV" },
            { "DPDA", "Dữ liệu sao kê phát hành thẻ" },
            { "DB01", "Sao kê TSDB và Không TSDB" },
            { "KH03", "Sao kê Khách hàng pháp nhân" },
            { "BC57", "Sao kê Lãi dự thu" }
        };

        public RawDataController(ApplicationDbContext context, ILogger<RawDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 📋 GET: api/RawData - Lấy danh sách tất cả dữ liệu thô
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RawDataImport>>> GetRawDataImports()
        {
            try
            {
                var imports = await _context.RawDataImports
                    .Include(r => r.RawDataRecords)
                    .OrderByDescending(r => r.ImportDate)
                    .ToListAsync();

                return Ok(imports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách dữ liệu thô");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách dữ liệu thô", error = ex.Message });
            }
        }

        // 📤 POST: api/RawData/import/{dataType} - Import dữ liệu theo loại
        [HttpPost("import/{dataType}")]
        public async Task<IActionResult> ImportRawData(string dataType, [FromForm] RawDataImportRequest request)
        {
            try
            {
                // ✅ Kiểm tra loại dữ liệu hợp lệ
                if (!DataTypeDefinitions.ContainsKey(dataType.ToUpper()))
                {
                    return BadRequest(new { message = $"Loại dữ liệu '{dataType}' không được hỗ trợ" });
                }

                if (request.Files == null || !request.Files.Any())
                {
                    return BadRequest(new { message = "Không có file nào được chọn" });
                }

                var results = new List<RawDataImportResult>();

                foreach (var file in request.Files)
                {
                    // 🔍 Kiểm tra file nén
                    if (IsArchiveFile(file.FileName))
                    {
                        var archiveResults = await ProcessArchiveFile(file, dataType, request.ArchivePassword, request.Notes);
                        results.AddRange(archiveResults);
                    }
                    else
                    {
                        // 🔍 Kiểm tra tên file chứa mã loại dữ liệu
                        if (!file.FileName.Contains(dataType, StringComparison.OrdinalIgnoreCase))
                        {
                            results.Add(new RawDataImportResult
                            {
                                Success = false,
                                FileName = file.FileName,
                                Message = $"❌ Tên file phải chứa mã '{dataType}'"
                            });
                            continue;
                        }

                        var result = await ProcessSingleFile(file, dataType, request.Notes);
                        results.Add(result);
                    }
                }

                var successCount = results.Count(r => r.Success);
                return Ok(new 
                { 
                    message = $"Xử lý thành công {successCount}/{results.Count} file",
                    results = results 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import dữ liệu loại {DataType}", dataType);
                return StatusCode(500, new { message = "Lỗi khi import dữ liệu", error = ex.Message });
            }
        }

        // 👁️ GET: api/RawData/{id}/preview - Xem trước dữ liệu
        [HttpGet("{id}/preview")]
        public async Task<ActionResult<RawDataPreviewResponse>> GetDataPreview(int id)
        {
            try
            {
                _logger.LogInformation("Getting data preview for import {ImportId}", id);

                var import = await _context.RawDataImports
                    .Include(r => r.RawDataRecords)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (import == null)
                {
                    _logger.LogWarning("Import {ImportId} not found", id);
                    return NotFound(new { message = "Không tìm thấy dữ liệu import" });
                }

                _logger.LogInformation("Import {ImportId} found: {FileName}, Records: {RecordCount}", 
                    id, import.FileName, import.RawDataRecords.Count);

                if (!import.RawDataRecords.Any())
                {
                    _logger.LogWarning("Import {ImportId} has no records", id);
                    return Ok(new RawDataPreviewResponse
                    {
                        Id = (int)import.Id,
                        FileName = import.FileName,
                        DataType = import.DataType,
                        ImportDate = import.ImportDate,
                        StatementDate = import.StatementDate ?? import.ImportDate,
                        ImportedBy = import.ImportedBy,
                        Columns = new List<string>(),
                        Records = new List<Dictionary<string, object>>()
                    });
                }

                var preview = new RawDataPreviewResponse
                {
                    Id = (int)import.Id,
                    FileName = import.FileName,
                    DataType = import.DataType,
                    ImportDate = import.ImportDate,
                    StatementDate = import.StatementDate ?? import.ImportDate,
                    ImportedBy = import.ImportedBy,
                    Columns = new List<string>(), // GetColumnsFromJsonData(import.RawDataRecords.FirstOrDefault()?.JsonData),
                    Records = new List<Dictionary<string, object>>() // import.RawDataRecords.Take(100).Select(r => ParseJsonData(r.JsonData)).ToList()
                };

                _logger.LogInformation("Successfully generated preview for import {ImportId} with {RecordCount} records", 
                    id, preview.Records.Count);

                return Ok(preview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem trước dữ liệu {ImportId}", id);
                return StatusCode(500, new { 
                    message = "Lỗi khi xem trước dữ liệu", 
                    error = ex.Message,
                    details = ex.InnerException?.Message 
                });
            }
        }

        // 🗑️ DELETE: api/RawData/{id} - Xóa dữ liệu import
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRawDataImport(int id)
        {
            try
            {
                var import = await _context.RawDataImports
                    .Include(r => r.RawDataRecords)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (import == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu import" });
                }

                // 🗂️ Xóa bảng động nếu tồn tại
                await DropDynamicTableIfExists(import.DataType, import.StatementDate ?? import.ImportDate);

                _context.RawDataImports.Remove(import);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dữ liệu import {ImportId}", id);
                return StatusCode(500, new { message = "Lỗi khi xóa dữ liệu import", error = ex.Message });
            }
        }

        // 🗑️ DELETE: api/RawData/clear-all - Xóa toàn bộ dữ liệu import
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllRawData()
        {
            try
            {
                // Get the count before deletion for reporting
                var totalImports = await _context.RawDataImports.CountAsync();
                var totalRecords = await _context.RawDataRecords.CountAsync();

                // Clear all records first (due to foreign key constraint)
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM RawDataRecords");
                
                // Clear all imports
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM RawDataImports");

                // Drop all dynamic tables
                await DropAllDynamicTables();

                return Ok(new { 
                    message = $"Đã xóa toàn bộ dữ liệu: {totalImports} imports, {totalRecords} records",
                    clearedImports = totalImports,
                    clearedRecords = totalRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa toàn bộ dữ liệu thô");
                return StatusCode(500, new { message = "Lỗi khi xóa dữ liệu", error = ex.Message });
            }
        }

        // 🔍 GET: api/RawData/check-duplicate/{dataType}/{statementDate} - Kiểm tra dữ liệu trùng lặp
        [HttpGet("check-duplicate/{dataType}/{statementDate}")]
        public async Task<IActionResult> CheckDuplicateData(string dataType, string statementDate, [FromQuery] string? fileName = null)
        {
            try
            {
                _logger.LogInformation("Checking duplicate for dataType: {DataType}, statementDate: {StatementDate}, fileName: {FileName}", 
                    dataType, statementDate, fileName);

                if (!DateTime.TryParseExact(statementDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                var query = _context.RawDataImports
                    .Where(r => r.DataType == dataType.ToUpper() && 
                               r.StatementDate.HasValue && 
                               r.StatementDate.Value.Date == parsedDate.Date);

                // Nếu có fileName, chỉ kiểm tra trùng lặp fileName chính xác (case-insensitive)
                if (!string.IsNullOrEmpty(fileName))
                {
                    _logger.LogInformation("Filtering by fileName: {FileName}", fileName);
                    query = query.Where(r => r.FileName.ToLower() == fileName.ToLower());
                }

                var existingImports = await query.ToListAsync();
                
                _logger.LogInformation("Found {Count} existing imports", existingImports.Count);
                foreach (var import in existingImports)
                {
                    _logger.LogInformation("Existing import: {FileName}", import.FileName);
                }

                return Ok(new {
                    hasDuplicate = existingImports.Any(),
                    existingImports = existingImports.Select(i => new {
                        i.Id,
                        i.FileName,
                        i.ImportDate,
                        i.RecordsCount,
                        i.ImportedBy
                    }),
                    message = !string.IsNullOrEmpty(fileName) && existingImports.Any() 
                        ? $"File '{fileName}' đã được import trước đó." 
                        : existingImports.Any() 
                            ? $"Đã có {existingImports.Count} file(s) loại {dataType} cho ngày {parsedDate:dd/MM/yyyy}"
                            : "Không có dữ liệu trùng lặp"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra dữ liệu trùng lặp");
                return StatusCode(500, new { message = "Lỗi khi kiểm tra dữ liệu trùng lặp", error = ex.Message });
            }
        }

        // 🗑️ DELETE: api/RawData/by-date/{dataType}/{statementDate} - Xóa dữ liệu theo ngày sao kê
        [HttpDelete("by-date/{dataType}/{statementDate}")]
        public async Task<IActionResult> DeleteByStatementDate(string dataType, string statementDate)
        {
            try
            {
                if (!DateTime.TryParseExact(statementDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                var importsToDelete = await _context.RawDataImports
                    .Include(r => r.RawDataRecords)
                    .Where(r => r.DataType == dataType.ToUpper() && 
                               r.StatementDate.HasValue && 
                               r.StatementDate.Value.Date == parsedDate.Date)
                    .ToListAsync();

                if (!importsToDelete.Any())
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu để xóa" });
                }

                var deletedCount = importsToDelete.Count;
                var deletedRecords = importsToDelete.Sum(i => i.RecordsCount);

                // Drop dynamic tables for this data type and date
                await DropDynamicTableIfExists(dataType.ToUpper(), parsedDate);

                // Remove from database
                _context.RawDataImports.RemoveRange(importsToDelete);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã xóa {deletedCount} import(s) với {deletedRecords} records cho {dataType} ngày {statementDate}",
                    deletedImports = deletedCount,
                    deletedRecords = deletedRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dữ liệu theo ngày");
                return StatusCode(500, new { message = "Lỗi khi xóa dữ liệu", error = ex.Message });
            }
        }

        // 📋 GET: api/RawData/by-date/{dataType}/{statementDate} - Lấy dữ liệu theo ngày sao kê
        [HttpGet("by-date/{dataType}/{statementDate}")]
        public async Task<IActionResult> GetByStatementDate(string dataType, string statementDate)
        {
            try
            {
                if (!DateTime.TryParseExact(statementDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                var imports = await _context.RawDataImports
                    .Include(r => r.RawDataRecords)
                    .Where(r => r.DataType == dataType.ToUpper() && 
                               r.StatementDate.HasValue && 
                               r.StatementDate.Value.Date == parsedDate.Date)
                    .OrderByDescending(r => r.ImportDate)
                    .ToListAsync();

                return Ok(imports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu theo ngày");
                return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu theo ngày", error = ex.Message });
            }
        }

        // 📋 GET: api/RawData/by-date-range/{dataType} - Lấy dữ liệu theo khoảng ngày
        [HttpGet("by-date-range/{dataType}")]
        public async Task<IActionResult> GetByDateRange(string dataType, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                if (!DateTime.TryParseExact(fromDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedFromDate) ||
                    !DateTime.TryParseExact(toDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedToDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                var imports = await _context.RawDataImports
                    .Include(r => r.RawDataRecords)
                    .Where(r => r.DataType == dataType.ToUpper() && 
                               r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date >= parsedFromDate.Date && 
                               r.StatementDate.Value.Date <= parsedToDate.Date)
                    .OrderByDescending(r => r.StatementDate)
                    .ThenByDescending(r => r.ImportDate)
                    .ToListAsync();

                return Ok(imports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu theo khoảng ngày");
                return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu theo khoảng ngày", error = ex.Message });
            }
        }

        // 🗄️ GET: api/RawData/table/{dataType} - Lấy dữ liệu thô trực tiếp từ bảng động
        [HttpGet("table/{dataType}")]
        public async Task<ActionResult> GetRawDataFromTable(string dataType, [FromQuery] string? statementDate = null)
        {
            try
            {
                _logger.LogInformation("Getting raw data from table for dataType: {DataType}, statementDate: {StatementDate}", 
                    dataType, statementDate);

                // Kiểm tra loại dữ liệu hợp lệ
                if (!DataTypeDefinitions.ContainsKey(dataType.ToUpper()))
                {
                    return BadRequest(new { message = $"Loại dữ liệu '{dataType}' không được hỗ trợ" });
                }

                // Tạo tên bảng động với format mới
                string tableName;
                if (!string.IsNullOrEmpty(statementDate))
                {
                    tableName = $"Raw_{dataType.ToUpper()}_{statementDate.Replace("-", "")}";
                }
                else
                {
                    // Lấy bảng mới nhất
                    var latestImport = await _context.RawDataImports
                        .Where(r => r.DataType == dataType.ToUpper())
                        .OrderByDescending(r => r.ImportDate)
                        .FirstOrDefaultAsync();

                    if (latestImport == null)
                    {
                        return NotFound(new { message = $"Không tìm thấy dữ liệu import nào cho {dataType}" });
                    }

                    tableName = $"Raw_{dataType.ToUpper()}_{latestImport.StatementDate:yyyyMMdd}";
                }

                // ✅ SQL Server compatible: Check if table exists
                var tableExistsQuery = @"
                    SELECT COUNT(*) 
                    FROM sys.tables 
                    WHERE name = @tableName";

                using var checkCommand = _context.Database.GetDbConnection().CreateCommand();
                checkCommand.CommandText = tableExistsQuery;
                var tableNameParam = checkCommand.CreateParameter();
                tableNameParam.ParameterName = "@tableName";
                tableNameParam.Value = tableName;
                checkCommand.Parameters.Add(tableNameParam);

                await _context.Database.OpenConnectionAsync();
                var tableExistsResult = await checkCommand.ExecuteScalarAsync();
                var tableExists = Convert.ToInt32(tableExistsResult) > 0;

                if (!tableExists)
                {
                    _logger.LogWarning("Table {TableName} does not exist", tableName);
                    return NotFound(new { message = $"Bảng {tableName} không tồn tại" });
                }

                // ✅ SQL Server compatible: Use TOP instead of LIMIT
                var rawDataQuery = $"SELECT TOP 1000 * FROM [{tableName}] ORDER BY Id";
                
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = rawDataQuery;
                
                if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }
                
                var results = new List<Dictionary<string, object>>();
                using var reader = await command.ExecuteReaderAsync();
                
                var columnNames = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnNames.Add(reader.GetName(i));
                }

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        // Convert DateTime to string for JSON serialization
                        if (value is DateTime dt)
                        {
                            value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        row[columnNames[i]] = value;
                    }
                    results.Add(row);
                }

                await _context.Database.CloseConnectionAsync();

                _logger.LogInformation("Successfully fetched {Count} records from table {TableName}", 
                    results.Count, tableName);

                return Ok(new
                {
                    tableName = tableName,
                    dataType = dataType,
                    recordCount = results.Count,
                    columns = columnNames,
                    records = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting raw data from table for dataType: {DataType}", dataType);
                return StatusCode(500, new { 
                    message = "Lỗi khi lấy dữ liệu thô từ bảng", 
                    error = ex.Message,
                    details = ex.InnerException?.Message 
                });
            }
        }

        // 🔧 PRIVATE METHODS

        // 🗂️ Xử lý file nén
        private async Task<List<RawDataImportResult>> ProcessArchiveFile(IFormFile file, string dataType, string password, string notes)
        {
            var results = new List<RawDataImportResult>();

            try
            {
                using var stream = file.OpenReadStream();
                using var archive = ArchiveFactory.Open(stream, new ReaderOptions { Password = password });

                // 📋 Lọc và sắp xếp file theo thứ tự từ 7800-7808
                var validFiles = archive.Entries
                    .Where(e => !e.IsDirectory && IsValidFileForImport(e.Key))
                    .Where(e => e.Key.Contains(dataType, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(e => GetFileOrder(e.Key))
                    .ToList();

                foreach (var entry in validFiles)
                {
                    using var entryStream = entry.OpenEntryStream();
                    var result = await ProcessStreamAsFile(entryStream, entry.Key, dataType, notes);
                    results.Add(result);
                }

                if (!results.Any())
                {
                    results.Add(new RawDataImportResult
                    {
                        Success = false,
                        FileName = file.FileName,
                        Message = $"❌ Không tìm thấy file có mã '{dataType}' trong file nén"
                    });
                }
            }
            catch (Exception ex)
            {
                results.Add(new RawDataImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    Message = $"❌ Lỗi xử lý file nén: {ex.Message}"
                });
            }

            return results;
        }

        // 📄 Xử lý file đơn
        private async Task<RawDataImportResult> ProcessSingleFile(IFormFile file, string dataType, string notes)
        {
            try
            {
                // 📅 Trích xuất ngày sao kê từ tên file
                var statementDate = ExtractStatementDate(file.FileName);
                if (statementDate == null)
                {
                    return new RawDataImportResult
                    {
                        Success = false,
                        FileName = file.FileName,
                        Message = "❌ Không tìm thấy ngày sao kê trong tên file (định dạng yyyymmdd)"
                    };
                }

                var rawDataImport = new RawDataImport
                {
                    FileName = file.FileName,
                    DataType = dataType.ToUpper(),
                    ImportDate = DateTime.UtcNow,
                    StatementDate = statementDate.Value,
                    ImportedBy = "System", // TODO: Lấy từ context user
                    Status = "Processing",
                    Notes = notes
                };

                // 💾 Lưu file gốc
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                rawDataImport.OriginalFileData = memoryStream.ToArray();

                // 📊 Xử lý dữ liệu
                var records = await ExtractDataFromFile(file);
                rawDataImport.RecordsCount = records.Count;
                rawDataImport.Status = records.Any() ? "Completed" : "Failed";

                // TODO: Fix this - this uses old RawDataImport model but context expects Temporal.RawDataImport
                // 💾 Lưu vào database
                // _context.RawDataImports.Add(rawDataImport);
                // await _context.SaveChangesAsync();

                // 💾 Lưu records
                // foreach (var record in records)
                // {
                //     record.RawDataImportId = rawDataImport.Id;
                // }
                // _context.RawDataRecords.AddRange(records);
                await _context.SaveChangesAsync();

                // 🗂️ Tạo bảng động
                var tableName = await CreateDynamicTable(dataType, statementDate.Value, records);

                return new RawDataImportResult
                {
                    Success = true,
                    FileName = file.FileName,
                    RecordsProcessed = records.Count,
                    Message = $"✅ Xử lý thành công {records.Count} bản ghi",
                    StatementDate = statementDate,
                    TableName = tableName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý file {FileName}", file.FileName);
                return new RawDataImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    Message = $"❌ Lỗi xử lý file: {ex.Message}"
                };
            }
        }

        // 🗂️ Xử lý stream như file
        private async Task<RawDataImportResult> ProcessStreamAsFile(Stream stream, string fileName, string dataType, string notes)
        {
            // Tạo FormFile tạm từ stream để tái sử dụng logic
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;

            var formFile = new FormFile(ms, 0, ms.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fileName)
            };

            return await ProcessSingleFile(formFile, dataType, notes);
        }

        // 📊 Trích xuất dữ liệu từ file
        private async Task<List<RawDataRecord>> ExtractDataFromFile(IFormFile file)
        {
            var records = new List<RawDataRecord>();
            var extension = Path.GetExtension(file.FileName).ToLower();

            switch (extension)
            {
                case ".csv":
                    records = await ProcessCsvStream(file.OpenReadStream());
                    break;
                case ".xlsx":
                case ".xls":
                    records = await ProcessExcelStream(file.OpenReadStream());
                    break;
                default:
                    throw new NotSupportedException($"Định dạng file {extension} không được hỗ trợ");
            }

            return records;
        }

        // 📊 Xử lý CSV
        private async Task<List<RawDataRecord>> ProcessCsvStream(Stream stream)
        {
            var records = new List<RawDataRecord>();
            using var reader = new StreamReader(stream);
            
            var headers = (await reader.ReadLineAsync())?.Split(',').Select(h => h.Trim('"')).ToList();
            if (headers == null) return records;

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var values = line.Split(',').Select(v => v.Trim('"')).ToList();
                var data = new Dictionary<string, object>();
                
                for (int i = 0; i < Math.Min(headers.Count, values.Count); i++)
                {
                    data[headers[i]] = values[i];
                }

                // 🎨 Format dữ liệu (ngày tháng và số)
                var formattedData = FormatDataValues(data);

                records.Add(new RawDataRecord
                {
                    JsonData = System.Text.Json.JsonSerializer.Serialize(formattedData),
                    ProcessedDate = DateTime.UtcNow
                });
            }

            return records;
        }

        // 📊 Xử lý Excel
        private async Task<List<RawDataRecord>> ProcessExcelStream(Stream stream)
        {
            var records = new List<RawDataRecord>();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            
            var headers = new List<string>();
            var headerRow = worksheet.Row(1);
            for (int col = 1; col <= worksheet.ColumnsUsed().Count(); col++)
            {
                headers.Add(headerRow.Cell(col).GetString());
            }

            for (int row = 2; row <= worksheet.RowsUsed().Count(); row++)
            {
                var data = new Dictionary<string, object>();
                for (int col = 1; col <= headers.Count; col++)
                {
                    var cellValue = worksheet.Row(row).Cell(col).Value;
                    data[headers[col - 1]] = cellValue.ToString() ?? "";
                }

                // 🎨 Format dữ liệu (ngày tháng và số)
                var formattedData = FormatDataValues(data);

                records.Add(new RawDataRecord
                {
                    JsonData = System.Text.Json.JsonSerializer.Serialize(formattedData),
                    ProcessedDate = DateTime.UtcNow
                });
            }

            return await Task.FromResult(records);
        }

        // 🗂️ Tạo bảng động - Improved version
        private async Task<string> CreateDynamicTable(string dataType, DateTime statementDate, List<RawDataRecord> records)
        {
            if (!records.Any())
            {
                _logger.LogWarning("No records to create dynamic table for {DataType} {StatementDate}", dataType, statementDate);
                return "";
            }

            var tableName = $"Raw_{dataType}_{statementDate:yyyyMMdd}";
            _logger.LogInformation("Creating dynamic table {TableName} with {RecordCount} records", tableName, records.Count);

            try
            {
                // Lấy cấu trúc từ record đầu tiên
                var firstRecord = ParseJsonData(records.First().JsonData);
                if (!firstRecord.Any())
                {
                    _logger.LogError("First record has no data for table {TableName}", tableName);
                    return "";
                }

                var columns = firstRecord.Keys.ToList();
                _logger.LogInformation("Table {TableName} will have columns: {Columns}", tableName, string.Join(", ", columns));

                // 1. Drop table if exists
                await DropDynamicTableIfExists(dataType, statementDate);
                
                // 2. Create table with safe column names - SQL Server compatible
                var createTableSql = $@"
                    CREATE TABLE [{tableName}] (
                        [Id] INT PRIMARY KEY IDENTITY(1,1),";

                foreach (var column in columns)
                {
                    var safeName = SanitizeColumnName(column);
                    createTableSql += $"\n        [{safeName}] TEXT,";
                }
                createTableSql = createTableSql.TrimEnd(',') + "\n    )";

                _logger.LogInformation("Creating table with SQL: {SQL}", createTableSql);
                await _context.Database.ExecuteSqlRawAsync(createTableSql);

                // 3. Bulk insert data using parameterized queries
                var insertedCount = 0;
                foreach (var record in records)
                {
                    var data = ParseJsonData(record.JsonData);
                    
                    var columnNames = columns.Select(c => $"[{SanitizeColumnName(c)}]").ToList();
                    var paramNames = columns.Select((c, i) => $"@p{i}").ToList();
                    
                    var insertSql = $@"
                        INSERT INTO [{tableName}] ({string.Join(", ", columnNames)})
                        VALUES ({string.Join(", ", paramNames)})";

                    using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = insertSql;

                    // Add parameters
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = $"@p{i}";
                        
                        // Convert JsonElement to string to avoid SQL Server mapping issues
                        var value = data.GetValueOrDefault(columns[i], null);
                        if (value == null)
                        {
                            param.Value = DBNull.Value;
                        }
                        else if (value is JsonElement jsonElement)
                        {
                            param.Value = jsonElement.ValueKind == JsonValueKind.String ? 
                                jsonElement.GetString() : jsonElement.ToString();
                        }
                        else
                        {
                            param.Value = value.ToString();
                        }
                        
                        command.Parameters.Add(param);
                    }

                    if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                    {
                        await _context.Database.OpenConnectionAsync();
                    }

                    await command.ExecuteNonQueryAsync();
                    insertedCount++;
                }

                _logger.LogInformation("Successfully created table {TableName} and inserted {InsertedCount} records", tableName, insertedCount);
                return tableName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create dynamic table {TableName}. Error: {Error}", tableName, ex.Message);
                
                // Cleanup on failure
                try
                {
                    await DropDynamicTableIfExists(dataType, statementDate);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to cleanup table {TableName} after creation failure", tableName);
                }
                
                return "";
            }
        }

        // Helper method to sanitize column names
        private string SanitizeColumnName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return "UnknownColumn";

            // Replace invalid characters
            var sanitized = columnName
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace(".", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace("(", "_")
                .Replace(")", "_")
                .Replace("[", "_")
                .Replace("]", "_")
                .Replace(",", "_")
                .Replace(";", "_")
                .Replace(":", "_")
                .Replace("'", "_")
                .Replace("\"", "_");

            // Ensure it starts with a letter or underscore
            if (!char.IsLetter(sanitized[0]) && sanitized[0] != '_')
            {
                sanitized = "Col_" + sanitized;
            }

            // Limit length
            if (sanitized.Length > 100)
            {
                sanitized = sanitized.Substring(0, 100);
            }

            return sanitized;
        }

        // 🗑️ Xóa bảng động - Improved version
        private async Task DropDynamicTableIfExists(string dataType, DateTime statementDate)
        {
            var tableName = $"Raw_{dataType}_{statementDate:yyyyMMdd}";
            try
            {
                _logger.LogInformation("Attempting to drop table {TableName} if exists", tableName);
                
                // Check if table exists first
                // ✅ SQL Server compatible: Check if table exists using sys.tables
                var checkTableSql = @"
                    SELECT COUNT(*) 
                    FROM sys.tables 
                    WHERE name = @tableName";

                using var checkCommand = _context.Database.GetDbConnection().CreateCommand();
                checkCommand.CommandText = checkTableSql;
                var tableNameParam = checkCommand.CreateParameter();
                tableNameParam.ParameterName = "@tableName";
                tableNameParam.Value = tableName;
                checkCommand.Parameters.Add(tableNameParam);

                if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                var tableExists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;

                if (tableExists)
                {
                    // ✅ SQL Server compatible: Simple DROP TABLE
                    var dropTableSql = $"DROP TABLE [{tableName}]";
                    await _context.Database.ExecuteSqlRawAsync(dropTableSql);
                    _logger.LogInformation("Successfully dropped table {TableName}", tableName);
                }
                else
                {
                    _logger.LogInformation("Table {TableName} does not exist, no need to drop", tableName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dropping table {TableName}: {Error}", tableName, ex.Message);
                // Don't throw - this is cleanup, should not fail the main operation
            }
        }

        // 🗑️ Xóa tất cả bảng động
        private async Task DropAllDynamicTables()
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa tất cả bảng động");

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                // Lấy danh sách tất cả bảng có tên bắt đầu với "Raw_"
                var getTablesQuery = @"
                    SELECT name FROM sys.tables 
                    WHERE name LIKE 'Raw_%'";

                var tableNames = new List<string>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = getTablesQuery;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }

                // Xóa từng bảng
                foreach (var tableName in tableNames)
                {
                    try
                    {
                        _logger.LogInformation($"Đang xóa bảng động: {tableName}");
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = $"DROP TABLE IF EXISTS [{tableName}]";
                            await command.ExecuteNonQueryAsync();
                        }
                        _logger.LogInformation($"Đã xóa bảng động: {tableName}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi khi xóa bảng động {tableName}: {ex.Message}");
                    }
                }

                _logger.LogInformation($"Hoàn thành xóa {tableNames.Count} bảng động");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa tất cả bảng động: {ex.Message}");
                throw;
            }
        }

        // 🔍 DEBUG: Endpoint để kiểm tra các bảng động hiện có
        [HttpGet("debug/tables")]
        public async Task<ActionResult> GetDynamicTables()
        {
            try
            {
                _logger.LogInformation("Getting list of dynamic tables");

                // Sử dụng SQL Server system table
                var query = @"
                    SELECT name FROM sys.tables 
                    WHERE name LIKE 'Raw_%'
                    ORDER BY name";

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = query;
                
                if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }
                
                var tables = new List<object>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString(0);
                    tables.Add(new { tableName = tableName });
                }

                await _context.Database.CloseConnectionAsync();

                _logger.LogInformation("Found {TableCount} dynamic tables", tables.Count);

                return Ok(new
                {
                    message = $"Found {tables.Count} dynamic tables",
                    tables = tables
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dynamic tables list");
                return StatusCode(500, new { 
                    message = "Error getting dynamic tables list", 
                    error = ex.Message 
                });
            }
        }

        // 🚀 PERFORMANCE OPTIMIZED ENDPOINTS
        
        // 📋 GET: api/RawData/optimized - Lấy dữ liệu với pagination và caching tối ưu
        [HttpGet("optimized/imports")]
        public async Task<ActionResult<PaginatedResponse<RawDataImportSummary>>> GetOptimizedRawDataImports(
            [FromQuery] ValidatedOptimizedQueryRequest request)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cacheKey = $"raw_data_imports_{request.GetHashCode()}";
                
                // Simple in-memory cache (trong production nên dùng Redis)
                var cacheEntry = HttpContext.Items[cacheKey] as PaginatedResponse<RawDataImportSummary>;
                if (cacheEntry != null)
                {
                    _logger.LogInformation("📋 Cache HIT for optimized imports query: {CacheKey}", cacheKey);
                    return Ok(cacheEntry);
                }

                var query = _context.RawDataImports.AsQueryable();

                // Filtering
                if (!string.IsNullOrEmpty(request.DataType))
                    query = query.Where(r => r.DataType == request.DataType);
                
                if (!string.IsNullOrEmpty(request.Status))
                    query = query.Where(r => r.Status == request.Status);

                if (request.FromDate.HasValue)
                    query = query.Where(r => r.ImportDate >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(r => r.ImportDate <= request.ToDate.Value);

                if (!string.IsNullOrEmpty(request.ImportedBy))
                    query = query.Where(r => r.ImportedBy == request.ImportedBy);

                // Total count với optimized query
                var totalCount = await query.CountAsync();

                // Pagination với projection để giảm data transfer
                var items = await query
                    .OrderByDescending(r => r.ImportDate)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(r => new RawDataImportSummary
                    {
                        Id = r.Id,
                        FileName = r.FileName,
                        DataType = r.DataType,
                        ImportDate = r.ImportDate,
                        StatementDate = r.StatementDate ?? r.ImportDate,
                        ImportedBy = r.ImportedBy,
                        Status = r.Status,
                        RecordsCount = r.RecordsCount,
                        IsArchiveFile = r.IsArchiveFile,
                        ExtractedFilesCount = r.ExtractedFilesCount
                    })
                    .ToListAsync();

                var response = new PaginatedResponse<RawDataImportSummary>
                {
                    Data = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                    HasNextPage = request.Page * request.PageSize < totalCount,
                    HasPreviousPage = request.Page > 1
                };

                // Cache response for 2 minutes
                HttpContext.Items[cacheKey] = response;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu tối ưu hóa");
                return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu tối ưu hóa", error = ex.Message });
            }
        }

        // 📋 GET: api/RawData/temporal/records - Lấy records với temporal queries support
        [HttpGet("optimized/records/{importId}")]
        public async Task<ActionResult<VirtualScrollResponse<RawDataRecordSummary>>> GetOptimizedRawDataRecords(
            string importId, [FromQuery] ValidatedVirtualScrollRequest request)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var query = _context.RawDataRecords
                    .Include(r => r.RawDataImport)
                    .AsQueryable();

                // Filter by import ID if provided
                if (importId != "all" && int.TryParse(importId, out var id))
                {
                    query = query.Where(r => r.RawDataImportId == id);
                }

                if (!string.IsNullOrEmpty(request.DataType))
                    query = query.Where(r => r.RawDataImport.DataType == request.DataType);

                if (request.FromDate.HasValue)
                    query = query.Where(r => r.ProcessedDate >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(r => r.ProcessedDate <= request.ToDate.Value);

                // Optimized count query
                var totalCount = await query.CountAsync();

                // Virtual scrolling - chỉ lấy data cần thiết cho viewport
                var items = await query
                    .OrderByDescending(r => r.ProcessedDate)
                    .Skip(request.StartIndex)
                    .Take(request.ViewportSize)
                    .Select(r => new RawDataRecordSummary
                    {
                        Id = r.Id,
                        ImportId = r.RawDataImportId,
                        ImportFileName = r.RawDataImport.FileName,
                        DataType = r.RawDataImport.DataType,
                        ProcessedDate = r.ProcessedDate,
                        // JsonData được truncate để giảm bandwidth
                        JsonDataPreview = r.JsonData.Length > 200 ? 
                            r.JsonData.Substring(0, 200) + "..." : r.JsonData,
                        JsonDataSize = r.JsonData.Length
                    })
                    .ToListAsync();

                var response = new VirtualScrollResponse<RawDataRecordSummary>
                {
                    Data = items,
                    TotalCount = totalCount,
                    StartIndex = request.StartIndex,
                    EndIndex = Math.Min(request.StartIndex + request.ViewportSize - 1, totalCount - 1),
                    ViewportSize = request.ViewportSize,
                    HasMore = request.StartIndex + request.ViewportSize < totalCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy records tối ưu hóa");
                return StatusCode(500, new { message = "Lỗi khi lấy records tối ưu hóa", error = ex.Message });
            }
        }

        // 📋 GET: api/RawData/dashboard/stats - Dashboard statistics với caching
        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<DashboardStats>> GetDashboardStats()
        {
            try
            {
                const string cacheKey = "dashboard_stats";
                var cached = HttpContext.Items[cacheKey] as DashboardStats;
                if (cached != null)
                {
                    return Ok(cached);
                }

                // Parallel queries để tăng tốc
                var totalImportsTask = _context.RawDataImports.CountAsync();
                var totalRecordsTask = _context.RawDataRecords.CountAsync();
                var totalProcessedTask = _context.RawDataImports.SumAsync(r => r.RecordsCount);
                var dataTypeGroupsTask = _context.RawDataImports
                    .GroupBy(r => r.DataType)
                    .Select(g => new { DataType = g.Key, Count = g.Count() })
                    .ToListAsync();
                var importsLast30DaysTask = _context.RawDataImports
                    .Where(r => r.ImportDate >= DateTime.Today.AddDays(-30))
                    .CountAsync();
                var successfulImportsTask = _context.RawDataImports
                    .Where(r => r.Status == "Success")
                    .CountAsync();
                var failedImportsTask = _context.RawDataImports
                    .Where(r => r.Status == "Failed")
                    .CountAsync();

                await Task.WhenAll(
                    totalImportsTask,
                    totalRecordsTask,
                    totalProcessedTask,
                    dataTypeGroupsTask,
                    importsLast30DaysTask,
                    successfulImportsTask,
                    failedImportsTask
                );

                var stats = new DashboardStats
                {
                    TotalImports = await totalImportsTask,
                    TotalRecords = await totalRecordsTask,
                    TotalRecordsProcessed = await totalProcessedTask,
                    ImportsByDataType = (await dataTypeGroupsTask).ToDictionary(x => x.DataType, x => x.Count),
                    ImportsLast30Days = await importsLast30DaysTask,
                    SuccessfulImports = await successfulImportsTask,
                    FailedImports = await failedImportsTask,
                    LastUpdated = DateTime.UtcNow
                };

                // Cache for 5 minutes
                HttpContext.Items[cacheKey] = stats;

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dashboard stats");
                return StatusCode(500, new { message = "Lỗi khi lấy dashboard stats", error = ex.Message });
            }
        }

        // 🔧 UTILITY METHODS

        // 📅 Trích xuất ngày sao kê từ tên file
        private DateTime? ExtractStatementDate(string fileName)
        {
            // Pattern 1: 7800_LN01_20250531 hoặc tương tự
            var match = Regex.Match(fileName, @"(\d{4})_[A-Z0-9]+_(\d{8})");
            if (match.Success)
            {
                var dateStr = match.Groups[2].Value; // Lấy phần 20250531
                if (DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            // Pattern 2: LN01_20240101_test-data.csv hoặc tương tự  
            match = Regex.Match(fileName, @"[A-Z0-9]+_(\d{8})");
            if (match.Success)
            {
                var dateStr = match.Groups[1].Value;
                if (DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            // Pattern 3: Chỉ có 8 chữ số liên tiếp (fallback)
            match = Regex.Match(fileName, @"\d{8}");
            if (match.Success && DateTime.TryParseExact(match.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date2))
            {
                return date2;
            }

            return null;
        }

        // 🗂️ Kiểm tra file nén
        private bool IsArchiveFile(string fileName) =>
            new[] { ".zip", ".7z", ".rar", ".tar", ".gz" }
                .Contains(Path.GetExtension(fileName).ToLower());

        // ✅ Kiểm tra file hợp lệ cho import
        private bool IsValidFileForImport(string fileName) =>
            new[] { ".csv", ".xlsx", ".xls" }
                .Contains(Path.GetExtension(fileName).ToLower());

        // 🔢 Lấy thứ tự file (7800-7808)
        private int GetFileOrder(string fileName)
        {
            var match = Regex.Match(fileName, @"78(\d{2})");
            return match.Success ? int.Parse(match.Groups[1].Value) : 999;
        }

        // 📄 Lấy content type
        private string GetContentType(string fileName) =>
            Path.GetExtension(fileName).ToLower() switch
            {
                ".csv" => "text/csv",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                _ => "application/octet-stream"
            };

        // 📊 Parse JSON data
        private Dictionary<string, object> ParseJsonData(string jsonData)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData) 
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        // 📋 Lấy columns từ JSON
        private List<string> GetColumnsFromJsonData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) return new List<string>();
            
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
                return data?.Keys.ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        // 🎨 Format dữ liệu sau khi import
        private Dictionary<string, object> FormatDataValues(Dictionary<string, object> data)
        {
            var formattedData = new Dictionary<string, object>();

            foreach (var kvp in data)
            {
                var key = kvp.Key;
                var value = kvp.Value?.ToString();

                if (string.IsNullOrWhiteSpace(value))
                {
                    formattedData[key] = value ?? "";
                    continue;
                }

                // Thử format dạng ngày tháng
                var formattedDate = TryFormatAsDate(value);
                if (formattedDate != null)
                {
                    formattedData[key] = formattedDate;
                    continue;
                }

                // Thử format dạng số với dấu phân cách hàng nghìn
                var formattedNumber = TryFormatAsNumber(value);
                if (formattedNumber != null)
                {
                    formattedData[key] = formattedNumber;
                    continue;
                }

                // Giữ nguyên giá trị nếu không match format nào
                formattedData[key] = value;
            }

            return formattedData;
        }

        // 📅 Thử format dạng ngày tháng về dd/mm/yyyy
        private string? TryFormatAsDate(string value)
        {
            // Loại bỏ khoảng trắng
            value = value.Trim();

            // Pattern 1: yyyymmdd (8 chữ số)
            if (Regex.IsMatch(value, @"^\d{8}$"))
            {
                if (DateTime.TryParseExact(value, "yyyyMMdd", null, DateTimeStyles.None, out var date1))
                {
                    return date1.ToString("dd/MM/yyyy");
                }
            }

            // Pattern 2: yyyy-mm-dd
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", null, DateTimeStyles.None, out var date2))
            {
                return date2.ToString("dd/MM/yyyy");
            }

            // Pattern 3: yyyy/mm/dd
            if (DateTime.TryParseExact(value, "yyyy/MM/dd", null, DateTimeStyles.None, out var date3))
            {
                return date3.ToString("dd/MM/yyyy");
            }

            // Pattern 4: dd/mm/yyyy (đã đúng format)
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out var date4))
            {
                return value; // Giữ nguyên
            }

            // Pattern 5: dd-mm-yyyy
            if (DateTime.TryParseExact(value, "dd-MM-yyyy", null, DateTimeStyles.None, out var date5))
            {
                return date5.ToString("dd/MM/yyyy");
            }

            // Pattern 6: mm/dd/yyyy (American format)
            if (DateTime.TryParseExact(value, "MM/dd/yyyy", null, DateTimeStyles.None, out var date6))
            {
                return date6.ToString("dd/MM/yyyy");
            }

            return null; // Không phải ngày tháng
        }

        // 🔢 Thử format dạng số với dấu phân cách hàng nghìn
        private string? TryFormatAsNumber(string value)
        {
            // Loại bỏ khoảng trắng và dấu phân cách có sẵn
            value = value.Trim().Replace(",", "").Replace(" ", "");

            // Thử parse integer
            if (long.TryParse(value, out var longValue))
            {
                // Chỉ format số từ 1000 trở lên
                if (Math.Abs(longValue) >= 1000)
                {
                    return longValue.ToString("#,###");
                }
                return longValue.ToString();
            }

            // Thử parse decimal
            if (decimal.TryParse(value, out var decimalValue))
            {
                // Chỉ format số từ 1000 trở lên
                if (Math.Abs(decimalValue) >= 1000)
                {
                    // Nếu là số nguyên thì không hiển thị phần thập phân
                    if (decimalValue == Math.Floor(decimalValue))
                    {
                        return ((long)decimalValue).ToString("#,###");
                    }
                    else
                    {
                        return decimalValue.ToString("#,###.##");
                    }
                }
                return decimalValue.ToString();
            }

            return null; // Không phải số
        }
    }
}
