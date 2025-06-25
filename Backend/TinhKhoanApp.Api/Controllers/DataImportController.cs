using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Dtos;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Globalization;
using TinhKhoanApp.Api.Services;
using System.Text.Json;
using System.Text;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataImportController> _logger;
        private readonly ICompressionService _compressionService;
        private readonly IStatementDateService _statementDateService;

        public DataImportController(
            ApplicationDbContext context, 
            ILogger<DataImportController> logger,
            ICompressionService compressionService,
            IStatementDateService statementDateService)
        {
            _context = context;
            _logger = logger;
            _compressionService = compressionService;
            _statementDateService = statementDateService;
        }

        // GET: api/DataImport - Get all imported data records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportedDataRecord>>> GetImportedData()
        {
            try
            {
                var importedData = await _context.ImportedDataRecords
                    .OrderByDescending(r => r.ImportDate)
                    .Select(r => new 
                    {
                        r.Id,
                        r.FileName,
                        r.FileType,
                        r.Category,
                        r.ImportDate,
                        r.StatementDate,
                        r.ImportedBy,
                        r.Status,
                        r.RecordsCount,
                        r.Notes
                        // Skip CompressedData và CompressionRatio để tránh lỗi database
                    })
                    .ToListAsync();

                return Ok(importedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving imported data");
                return StatusCode(500, new { message = "Error retrieving imported data", error = ex.Message });
            }
        }

        // POST: api/DataImport/upload - Upload and process files
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] DataImportRequest request)
        {
            try
            {
                if (request.Files == null || !request.Files.Any())
                {
                    return BadRequest(new { message = "No files provided" });
                }

                var results = new List<ImportResult>();

                foreach (var file in request.Files)
                {
                    var result = await ProcessFile(file, request.Category ?? "General");
                    results.Add(result);
                }

                return Ok(new { 
                    message = $"Successfully processed {results.Count(r => r.Success)} out of {results.Count} files",
                    results = results 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading files");
                return StatusCode(500, new { message = "Error uploading files", error = ex.Message });
            }
        }

        // GET: api/DataImport/{id}/preview - Get preview of imported data
        [HttpGet("{id}/preview")]
        public async Task<ActionResult<DataPreviewResponse>> GetDataPreview(int id)
        {
            try
            {
                _logger.LogInformation("🔍 Getting data preview for record ID: {RecordId}", id);
                
                var record = await _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (record == null)
                {
                    _logger.LogWarning("⚠️ Import record with ID {RecordId} not found", id);
                    return NotFound(new { message = "Import record not found" });
                }

                _logger.LogInformation("📊 Found record: {FileName}, Items count: {ItemsCount}", 
                    record.FileName, record.ImportedDataItems.Count);

                // ✅ ULTRA PRECISION: Load ALL records hoặc lấy tối đa 1000 bản ghi đầu tiên để xem chi tiết
                var maxPreviewItems = Request.Query.ContainsKey("all") ? int.MaxValue : 1000;
                var dataItems = record.ImportedDataItems
                    .OrderBy(i => i.Id) // Đảm bảo theo thứ tự tăng dần của ID để giữ đúng thứ tự file gốc 
                    .Take(maxPreviewItems)
                    .ToList();
                
                _logger.LogInformation("📝 Loading preview with {PreviewCount}/{TotalCount} records for detailed viewing", 
                    dataItems.Count, record.ImportedDataItems.Count);
                
                var previewData = new List<Dictionary<string, object>>();
                int parsedCount = 0;
                int errorCount = 0;
                int emptyCount = 0;
                
                foreach (var item in dataItems)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.RawData))
                        {
                            var parsedData = ParseJsonDataSafely(item.RawData);
                            if (parsedData != null && parsedData.Count > 0)
                            {
                                // Đánh dấu rõ record có dữ liệu
                                parsedData["_hasData"] = true;
                                previewData.Add(parsedData);
                                parsedCount++;
                            }
                            else if (item.RawData == "{}" || item.RawData == "[]" || IsEmptyJsonObject(item.RawData))
                            {
                                // Xử lý dòng trống (thêm bản ghi rỗng vào preview)
                                var emptyRecord = new Dictionary<string, object>();
                                emptyRecord["_isEmpty"] = true;
                                emptyRecord["_rowId"] = item.Id;
                                // Thêm các key rỗng từ header nếu có
                                if (previewData.Count > 0 && previewData[0].Count > 0)
                                {
                                    foreach (var key in previewData[0].Keys.Where(k => !k.StartsWith("_")))
                                    {
                                        emptyRecord[key] = "";
                                    }
                                }
                                previewData.Add(emptyRecord);
                                parsedCount++;
                            }
                            else
                            {
                                // Nếu không parse được JSON, thử parse như CSV hoặc text thuần
                                var fallbackData = ParseRawDataAsFallback(item.RawData, (int)item.Id);
                                if (fallbackData != null && fallbackData.Count > 0)
                                {
                                    fallbackData["_fallback"] = true;
                                    previewData.Add(fallbackData);
                                    parsedCount++;
                                }
                            }
                        }
                        else
                        {
                            // Xử lý dòng trống
                            var emptyRecord = new Dictionary<string, object>();
                            emptyRecord["_isEmpty"] = true;
                            emptyRecord["_rowId"] = item.Id;
                            previewData.Add(emptyRecord);
                        }
                    }
                    catch (Exception parseEx)
                    {
                        errorCount++;
                        _logger.LogWarning("⚠️ Failed to parse item {ItemId}: {Error}", item.Id, parseEx.Message);
                        _logger.LogDebug("Raw data content: {RawData}", item.RawData?.Length > 200 ? 
                            item.RawData.Substring(0, 200) + "..." : item.RawData);
                    }
                }

                // ✅ FIX: Đảm bảo luôn có RecordsCount chính xác từ database
                var actualRecordsCount = record.ImportedDataItems.Count;

                var preview = new DataPreviewResponse
                {
                    Id = record.Id,
                    FileName = record.FileName,
                    Category = record.Category,
                    ImportDate = record.ImportDate,
                    ImportedBy = record.ImportedBy,
                    RecordsCount = actualRecordsCount, // Số lượng thực tế từ database
                    Columns = GetColumnsFromPreviewData(previewData),
                    PreviewData = previewData
                };

                _logger.LogInformation("✅ Preview created: {DataCount}/{TotalCount} records parsed successfully, {ErrorCount} errors, Total records in DB: {ActualCount}", 
                    parsedCount, dataItems.Count, errorCount, actualRecordsCount);
                
                // Thêm thông tin trạng thái hiển thị
                preview.Status = new
                {
                    TotalDbRecords = actualRecordsCount,
                    LoadedForPreview = dataItems.Count,
                    ParsedSuccessfully = parsedCount,
                    Errors = errorCount,
                    IsComplete = dataItems.Count >= actualRecordsCount
                };
                
                // Thêm hiển thị chi tiết về số lượng và pagination
                preview.SummaryText = dataItems.Count >= actualRecordsCount
                    ? $"Hiển thị tất cả {actualRecordsCount} bản ghi (100%)"
                    : $"Hiển thị {dataItems.Count}/{actualRecordsCount} bản ghi ({(int)(dataItems.Count * 100.0 / actualRecordsCount)}%)";
                
                return Ok(preview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting data preview for record {RecordId}: {ErrorMessage}", id, ex.Message);
                
                // ✅ FIX: Không trả về mock data nữa, trả về lỗi thực tế
                return StatusCode(500, new { 
                    message = "Failed to load preview data", 
                    error = ex.Message,
                    details = "Check server logs for more information"
                });
            }
        }

        // GET: api/DataImport/{id}/download - Download original file
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadOriginalFile(int id)
        {
            try
            {
                var record = await _context.ImportedDataRecords.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { message = "Import record not found" });
                }

                if (record.OriginalFileData == null)
                {
                    return NotFound(new { message = "Original file data not found" });
                }

                return File(record.OriginalFileData, record.FileType, record.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file for record {RecordId}", id);
                return StatusCode(500, new { message = "Error downloading file", error = ex.Message });
            }
        }

        // DELETE: api/DataImport/{id} - Xóa bản ghi import theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImportRecord(string id)
        {
            try
            {
                _logger.LogInformation($"🗑️ Attempting to delete import record with ID: {id}");
                
                // Parse ID as integer if possible
                if (!int.TryParse(id, out int recordId))
                {
                    _logger.LogWarning($"❌ Invalid ID format: {id}");
                    return BadRequest(new { message = $"ID không hợp lệ: {id}" });
                }
                
                // Tìm bản ghi cần xóa
                var record = await _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .FirstOrDefaultAsync(r => r.Id == recordId);
                
                if (record == null)
                {
                    _logger.LogWarning($"❌ Import record with ID {recordId} not found");
                    return NotFound(new { message = $"Không tìm thấy bản ghi với ID {recordId}" });
                }
                
                // Lưu thông tin để trả về
                var fileName = record.FileName;
                var category = record.Category;
                var recordsCount = record.RecordsCount;
                
                // Xóa các ImportedDataItem liên quan trước
                if (record.ImportedDataItems != null && record.ImportedDataItems.Any())
                {
                    _logger.LogInformation($"🗑️ Deleting {record.ImportedDataItems.Count} related data items");
                    _context.ImportedDataItems.RemoveRange(record.ImportedDataItems);
                }
                
                // Xóa bản ghi chính
                _context.ImportedDataRecords.Remove(record);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"✅ Successfully deleted import record {recordId}: {fileName}");
                
                return Ok(new { 
                    message = $"Đã xóa thành công bản ghi {fileName}",
                    id = recordId,
                    fileName = fileName,
                    category = category,
                    recordsCount = recordsCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error deleting import record {id}");
                return StatusCode(500, new { message = $"Lỗi khi xóa bản ghi: {ex.Message}" });
            }
        }

        // GET: api/DataImport/export - Export all data to Excel
        [HttpGet("export")]
        public async Task<IActionResult> ExportAllData()
        {
            try
            {
                var allRecords = await _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .OrderByDescending(r => r.ImportDate)
                    .ToListAsync();

                using var workbook = new XLWorkbook();
                
                // Summary sheet
                var summarySheet = workbook.Worksheets.Add("Summary");
                summarySheet.Cell(1, 1).Value = "Import Summary";
                summarySheet.Cell(2, 1).Value = "File Name";
                summarySheet.Cell(2, 2).Value = "Category";
                summarySheet.Cell(2, 3).Value = "Import Date";
                summarySheet.Cell(2, 4).Value = "Records Count";
                summarySheet.Cell(2, 5).Value = "Imported By";

                int row = 3;
                foreach (var record in allRecords)
                {
                    summarySheet.Cell(row, 1).Value = record.FileName;
                    summarySheet.Cell(row, 2).Value = record.Category;
                    summarySheet.Cell(row, 3).Value = record.ImportDate;
                    summarySheet.Cell(row, 4).Value = record.RecordsCount;
                    summarySheet.Cell(row, 5).Value = record.ImportedBy;
                    row++;
                }

                // Create sheets for each category
                var categories = allRecords.GroupBy(r => r.Category);
                foreach (var categoryGroup in categories)
                {
                    var sheet = workbook.Worksheets.Add(categoryGroup.Key);
                    var categoryRecords = categoryGroup.SelectMany(r => r.ImportedDataItems).ToList();
                    
                    if (categoryRecords.Any())
                    {
                        var firstRecord = ParseJsonData(categoryRecords.First().RawData);
                        var columns = firstRecord.Keys.ToList();
                        
                        // Headers
                        for (int i = 0; i < columns.Count; i++)
                        {
                            sheet.Cell(1, i + 1).Value = columns[i];
                        }
                        
                        // Data
                        int dataRow = 2;
                        foreach (var item in categoryRecords)
                        {
                            var data = ParseJsonData(item.RawData);
                            for (int i = 0; i < columns.Count; i++)
                            {
                                if (data.ContainsKey(columns[i]))
                                {
                                    sheet.Cell(dataRow, i + 1).Value = data[columns[i]]?.ToString() ?? "";
                                }
                            }
                            dataRow++;
                        }
                    }
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"KPI_Data_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data");
                return StatusCode(500, new { message = "Error exporting data", error = ex.Message });
            }
        }

        // GET: api/DataImport/export/{id} - Export specific record to Excel
        [HttpGet("export/{id}")]
        public async Task<IActionResult> ExportRecord(int id)
        {
            try
            {
                var record = await _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (record == null)
                {
                    return NotFound(new { message = "Record not found" });
                }

                using var workbook = new XLWorkbook();
                
                // Create sheet with record name
                var sheetName = $"{record.Category}_{record.Id}";
                var worksheet = workbook.Worksheets.Add(sheetName);

                if (record.ImportedDataItems?.Any() == true)
                {
                    // Get columns from first item
                    var firstItem = record.ImportedDataItems.First();
                    var columns = GetColumnsFromData(firstItem.RawData);
                    
                    // Add headers
                    for (int i = 0; i < columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = columns[i];
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    // Add data rows
                    int rowIndex = 2;
                    foreach (var item in record.ImportedDataItems)
                    {
                        var data = ParseJsonData(item.RawData);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            var value = data.ContainsKey(columns[i]) ? data[columns[i]]?.ToString() : "";
                            worksheet.Cell(rowIndex, i + 1).Value = value;
                        }
                        rowIndex++;
                    }

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();
                }
                else
                {
                    worksheet.Cell(1, 1).Value = "No data available";
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{record.FileName.Replace('.', '_')}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting record {id}", id);
                return StatusCode(500, new { message = "Error exporting record", error = ex.Message });
            }
        }

        // Private helper methods
        private async Task<ImportResult> ProcessFile(IFormFile file, string category)
        {
            try
            {
                // Extract statement date from filename
                var statementDate = _statementDateService.ExtractStatementDateFromFileName(file.FileName);
                
                var record = new ImportedDataRecord
                {
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    Category = category ?? "General",
                    ImportDate = DateTime.UtcNow,
                    StatementDate = statementDate,
                    ImportedBy = "System", // TODO: Get from auth context
                    Status = "Processing"
                };

                // Store original file
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                record.OriginalFileData = memoryStream.ToArray();

                // Process file based on type
                var items = new List<ImportedDataItem>();
                if (file.ContentType == "application/zip" || file.FileName.EndsWith(".zip"))
                {
                    // 📦 Xử lý file ZIP - Tự động giải nén và import các CSV
                    items = await ProcessZipFile(file, category, statementDate);
                }
                else if (file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
                    file.ContentType == "application/vnd.ms-excel" ||
                    file.FileName.EndsWith(".xlsx") || file.FileName.EndsWith(".xls"))
                {
                    items = await ProcessExcelFile(file);
                }
                else if (file.ContentType == "text/csv" || file.FileName.EndsWith(".csv"))
                {
                    items = await ProcessCsvFile(file);
                }
                else if (file.ContentType == "application/pdf" || file.FileName.EndsWith(".pdf"))
                {
                    items = await ProcessPdfFile(file);
                }

                // Compress data if there are items
                if (items.Any())
                {
                    // Tạm thời comment out compression để tránh lỗi database schema
                    // var dataToCompress = items.Select(item => new { item.RawData, item.ProcessingNotes }).ToList();
                    // var (compressedData, compressionRatio) = await _compressionService.CompressDataAsync(dataToCompress);
                    
                    // record.CompressedData = compressedData;
                    // record.CompressionRatio = compressionRatio;
                    
                    // _logger.LogInformation($"Compressed {items.Count} records with ratio {compressionRatio:P2} for file {file.FileName}");
                }

                record.RecordsCount = items.Count;
                record.Status = items.Any() ? "Completed" : "Failed";
                record.ImportedDataItems = items;

                _context.ImportedDataRecords.Add(record);
                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    Success = true,
                    FileName = file.FileName,
                    RecordsProcessed = items.Count,
                    Message = $"Successfully processed {items.Count} records" + 
                             (statementDate.HasValue ? $" for statement date {statementDate:yyyy-MM-dd}" : "")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FileName}", file.FileName);
                return new ImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    RecordsProcessed = 0,
                    Message = $"Error processing file: {ex.Message}"
                };
            }
        }

        private Task<List<ImportedDataItem>> ProcessExcelFile(IFormFile file)
        {
            return Task.Run(() =>
            {
                var items = new List<ImportedDataItem>();
                
                _logger.LogInformation("🔍 Processing Excel file: {FileName}, Size: {FileSize} bytes", file.FileName, file.Length);
                
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                
                var headers = new List<string>();
                var headerRow = worksheet.Row(1);
                var lastColumn = worksheet.ColumnsUsed().Count();
                
                for (int col = 1; col <= lastColumn; col++)
                {
                    var headerValue = headerRow.Cell(col).GetString().Trim();
                    headers.Add(headerValue);
                }
                
                _logger.LogInformation("📋 Excel Headers found: {HeaderCount} columns - {Headers}", headers.Count, string.Join(", ", headers.Take(5)));

                var lastRow = worksheet.RowsUsed().Count();
                int addedRecords = 0;
                int processedEmptyAsNull = 0;
                int totalDataRowsInFile = lastRow - 1; // Trừ header
                
                // ✅ ULTRA PRECISION: Process EVERY row after header, including empty ones
                for (int row = 2; row <= lastRow; row++) // Bắt đầu từ hàng 2 (bỏ header)
                {
                    var data = new Dictionary<string, object>();
                    var values = new List<string>();
                    bool hasData = false;
                    
                    // ✅ SIÊU FIX: Lấy tất cả giá trị trong hàng
                    for (int col = 1; col <= headers.Count; col++)
                    {
                        var cellValue = worksheet.Row(row).Cell(col).Value;
                        string stringValue = cellValue.ToString().Trim() ?? "";
                        values.Add(stringValue);
                        
                        data[headers[col - 1]] = stringValue;
                        
                        if (!string.IsNullOrWhiteSpace(stringValue))
                        {
                            hasData = true;
                        }
                    }

                    if (!hasData)
                    {
                        processedEmptyAsNull++;
                        _logger.LogDebug("📝 Processed empty row {RowNumber} as null record", row);
                    }
                    
                    // ✅ ULTRA PRECISION: Lưu MỌI hàng (kể cả rỗng) để đảm bảo số lượng chính xác
                    items.Add(new ImportedDataItem
                    {
                        RawData = System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = false }),
                        ProcessedDate = DateTime.UtcNow
                    });
                    addedRecords++;
                    
                    if (addedRecords <= 5) // Log 5 bản ghi đầu để debug
                    {
                        _logger.LogDebug("✅ Added record {RecordNumber}: {RecordData}", addedRecords, 
                            System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = false }));
                    }
                }

                _logger.LogInformation("✅ Excel Processing ULTRA PRECISION completed: {FileName}" +
                    "\n📊 Total data rows in file: {TotalDataRows}" +
                    "\n✅ Added ALL records: {AddedRecords}" + 
                    "\n📝 Empty rows processed as null: {ProcessedEmpty}" +
                    "\n🎯 EXACT MATCH: {ExactMatch}% (should be 100%)", 
                    file.FileName, totalDataRowsInFile, addedRecords, processedEmptyAsNull,
                    totalDataRowsInFile == addedRecords ? 100.0 : 0.0);

                return items;
            });
        }

        private async Task<List<ImportedDataItem>> ProcessCsvFile(IFormFile file)
        {
            var items = new List<ImportedDataItem>();
            
            _logger.LogInformation("🔍 Processing CSV file: {FileName}, Size: {FileSize} bytes", file.FileName, file.Length);
            
            using var reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.UTF8);
            var allContent = await reader.ReadToEndAsync();
            var lines = allContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            if (lines.Length == 0 || string.IsNullOrWhiteSpace(lines[0]))
            {
                _logger.LogWarning("⚠️ Empty or missing header line in CSV file: {FileName}", file.FileName);
                return items;
            }
            
            // ✅ SIÊU FIX: Parse header chính xác với RFC 4180 CSV standard
            var headers = ParseCsvLine(lines[0]);
            _logger.LogInformation("📋 CSV Headers found: {HeaderCount} columns - {Headers}", headers.Count, string.Join(", ", headers.Take(5)));

            int addedRecords = 0;
            int processedEmptyAsNull = 0;
            int totalDataLinesInFile = lines.Length - 1; // Trừ header
            
            // ✅ ULTRA PRECISION: Process EVERY line after header, including empty ones
            for (int i = 1; i < lines.Length; i++) // Bắt đầu từ dòng 2 (index 1)
            {
                var line = lines[i];
                int lineNumber = i + 1; // Line number bắt đầu từ 1
                
                // ✅ SIÊU FIX: Xử lý mọi dòng, kể cả dòng trống
                List<string> values;
                
                if (string.IsNullOrWhiteSpace(line) || line.Trim() == "")
                {
                    // Tạo một record rỗng cho dòng trống để đảm bảo đếm đúng
                    values = new List<string>();
                    for (int j = 0; j < headers.Count; j++)
                    {
                        values.Add(""); // Thêm giá trị rỗng cho mỗi cột
                    }
                    processedEmptyAsNull++;
                    _logger.LogDebug("📝 Processed empty line {LineNumber} as null record", lineNumber);
                }
                else
                {
                    // ✅ SIÊU FIX: Parse line với RFC 4180 chuẩn
                    values = ParseCsvLine(line);
                    
                    // ✅ SIÊU FIX: Đảm bảo số cột đúng, thêm cột rỗng nếu thiếu, cắt bỏ nếu thừa
                    while (values.Count < headers.Count)
                    {
                        values.Add("");
                    }
                    if (values.Count > headers.Count)
                    {
                        values = values.Take(headers.Count).ToList();
                    }
                }
                
                var data = new Dictionary<string, object>();
                
                // ✅ SIÊU FIX: Map chính xác từng cột với header
                for (int j = 0; j < headers.Count; j++)
                {
                    string cleanHeader = headers[j].Trim();
                    string cleanValue = values[j].Trim();
                    data[cleanHeader] = cleanValue;
                }

                // ✅ ULTRA PRECISION: Thêm MỌI record (kể cả rỗng) để đảm bảo số lượng chính xác
                items.Add(new ImportedDataItem
                {
                    RawData = System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = false }),
                    ProcessedDate = DateTime.UtcNow
                });
                addedRecords++;
                
                if (addedRecords <= 5) // Log 5 bản ghi đầu để debug
                {
                    _logger.LogDebug("✅ Added record {RecordNumber}: {RecordData}", addedRecords, 
                        System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = false }));
                }
            }

            _logger.LogInformation("✅ CSV Processing ULTRA PRECISION completed: {FileName}" +
                "\n📊 Total data lines in file: {TotalDataLines}" +
                "\n✅ Added ALL records: {AddedRecords}" + 
                "\n📝 Empty lines processed as null: {ProcessedEmpty}" +
                "\n🎯 EXACT MATCH: {ExactMatch}% (should be 100%)", 
                file.FileName, totalDataLinesInFile, addedRecords, processedEmptyAsNull,
                totalDataLinesInFile == addedRecords ? 100.0 : 0.0);

            return items;
        }
        
        // ✅ SIÊU HELPER: Parse CSV line theo chuẩn RFC 4180 (xử lý dấu ngoặc kép, dấu phẩy trong giá trị)
        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new StringBuilder();
            bool insideQuotes = false;
            bool nextCharIsEscaped = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (nextCharIsEscaped)
                {
                    currentValue.Append(c);
                    nextCharIsEscaped = false;
                    continue;
                }
                
                if (c == '"')
                {
                    if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Double quote escape: "" -> "
                        currentValue.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        // Toggle quote state
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (c == ',' && !insideQuotes)
                {
                    // End of field
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            
            // Add last field
            values.Add(currentValue.ToString());
            
            return values;
        }
        
        // ✅ SIÊU HELPER: Kiểm tra xem có phải dòng mẫu (sample data) không
        // ⚠️ SIÊU IMPORTANT: Vô hiệu hóa hoàn toàn việc kiểm tra dòng mẫu
        // để đảm bảo giữ nguyên số lượng bản ghi từ file gốc. Tất cả dòng
        // không rỗng hoàn toàn sẽ được lưu vào DB.
        private bool IsSampleDataLine(List<string> values, List<string> headers)
        {
            // Chỉ trả về true cho dòng hoàn toàn trống
            // KHÔNG LỌC bất kỳ dòng nào khác kể cả dòng mẫu/dòng test để đảm bảo số lượng bản ghi chính xác
            return values.Count == 0 || values.All(v => string.IsNullOrWhiteSpace(v));
        }
        
        // ✅ SIÊU HELPER: Kiểm tra xem bản ghi có dữ liệu không trống không
        // ⚠️ ULTRA FIXED: Giờ chỉ kiểm tra dòng có hoàn toàn trống không
        // Chấp nhận MỌI dòng có ít nhất 1 giá trị bất kỳ (kể cả "0", "-", "N/A", và các ký tự đặc biệt)
        // để đảm bảo số lượng bản ghi đúng 100% với file gốc
        private bool HasMeaningfulData(Dictionary<string, object> data)
        {
            if (data == null || data.Count == 0) return false;
            
            // Chấp nhận MỌI dòng có ít nhất 1 giá trị không trống hoàn toàn
            // KHÔNG lọc bất kỳ nội dung nào, kể cả dòng có ký tự đặc biệt hoặc dòng mẫu
            return data.Any(kvp => !string.IsNullOrWhiteSpace(kvp.Value?.ToString()));
        }
        
        // ✅ SIÊU HELPER: Kiểm tra chuỗi chỉ có ký tự đặc biệt
        private bool IsOnlySpecialChars(string value)
        {
            return value.All(c => !char.IsLetterOrDigit(c));
        }

        private Task<List<ImportedDataItem>> ProcessPdfFile(IFormFile file)
        {
            // PDF processing would require additional libraries like iTextSharp
            // For now, return empty list with a note
            var items = new List<ImportedDataItem>
            {
                new ImportedDataItem
                {
                    RawData = System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
                    {
                        ["Note"] = "PDF processing not yet implemented",
                        ["FileName"] = file.FileName,
                        ["Size"] = file.Length
                    }),
                    ProcessedDate = DateTime.UtcNow
                }
            };

            return Task.FromResult(items);
        }

        // ✅ Parse JSON data safely

        private List<string> GetColumnsFromData(string rawData)
        {
            if (string.IsNullOrEmpty(rawData)) return new List<string>();
            
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(rawData);
                return data?.Keys.ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        // ✅ Safe JSON parsing method
        private Dictionary<string, object>? ParseJsonDataSafely(string rawData)
        {
            if (string.IsNullOrEmpty(rawData)) return null;
            
            try
            {
                var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(rawData);
                return result?.Count > 0 ? result : null;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Failed to parse as JSON: {Error}", ex.Message);
                return null;
            }
        }

        // ✅ Fallback parsing for non-JSON data
        private Dictionary<string, object>? ParseRawDataAsFallback(string rawData, int itemId)
        {
            if (string.IsNullOrEmpty(rawData)) return null;
            
            try
            {
                // Try parsing as CSV or delimited data
                var lines = rawData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    var firstLine = lines[0].Trim();
                    
                    // Check if it looks like CSV
                    if (firstLine.Contains(',') || firstLine.Contains(';') || firstLine.Contains('\t'))
                    {
                        var delimiter = firstLine.Contains('\t') ? '\t' : 
                                       firstLine.Contains(';') ? ';' : ',';
                        
                        var values = firstLine.Split(delimiter);
                        var result = new Dictionary<string, object>();
                        
                        for (int i = 0; i < values.Length && i < 10; i++) // Limit to 10 columns
                        {
                            result[$"Column{i + 1}"] = values[i].Trim().Trim('"');
                        }
                        
                        return result.Count > 0 ? result : null;
                    }
                    
                    // Fallback: treat as single text value
                    return new Dictionary<string, object>
                    {
                        ["RawData"] = rawData.Length > 100 ? rawData.Substring(0, 100) + "..." : rawData,
                        ["ItemId"] = itemId,
                        ["DataType"] = "Text"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Failed fallback parsing for item {ItemId}: {Error}", itemId, ex.Message);
            }
            
            return null;
        }

        // ✅ Get columns from preview data
        private List<string> GetColumnsFromPreviewData(List<Dictionary<string, object>> previewData)
        {
            if (previewData == null || previewData.Count == 0)
                return new List<string>();
            
            var allColumns = new HashSet<string>();
            foreach (var row in previewData.Take(5)) // Check first 5 rows for all possible columns
            {
                foreach (var key in row.Keys)
                {
                    allColumns.Add(key);
                }
            }
            
            return allColumns.OrderBy(c => c).ToList();
        }

        // ✅ Update existing ParseJsonData method
        private Dictionary<string, object> ParseJsonData(string rawData)
        {
            return ParseJsonDataSafely(rawData) ?? new Dictionary<string, object>();
        }

        // POST: api/DataImport/upload-advanced - Enhanced upload with ZIP support and auto-categorization
        [HttpPost("upload-advanced")]
        public async Task<IActionResult> UploadFilesAdvanced([FromForm] AdvancedDataImportRequest request)
        {
            try
            {
                if (request.Files == null || !request.Files.Any())
                {
                    return BadRequest(new { message = "No files provided" });
                }

                var allResults = new List<ImportResult>();
                var processedFiles = new List<ProcessedFileInfo>();

                foreach (var file in request.Files)
                {
                    var fileInfo = AnalyzeFile(file.FileName);
                    processedFiles.Add(fileInfo);
                    
                    if (IsArchiveFile(file.FileName))
                    {
                        var archiveResults = await ProcessArchiveFile(file, request.Password ?? "", fileInfo);
                        allResults.AddRange(archiveResults);
                    }
                    else
                    {
                        var result = await ProcessSingleFile(file, fileInfo);
                        allResults.Add(result);
                    }
                }

                // Sort results by branch code ascending (7800 -> 7801 -> 7802...)
                var sortedResults = allResults.OrderBy(r => r.BranchCode).ToList();

                return Ok(new { 
                    message = $"Successfully processed {sortedResults.Count(r => r.Success)} out of {sortedResults.Count} files",
                    results = sortedResults,
                    processedFiles = processedFiles,
                    totalArchivesProcessed = processedFiles.Count(f => f.IsArchive),
                    totalRegularFilesProcessed = processedFiles.Count(f => !f.IsArchive)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in advanced file upload");
                return StatusCode(500, new { message = "Error processing files", error = ex.Message });
            }
        }

        // 📋 Phân tích thông tin file từ tên file
        private ProcessedFileInfo AnalyzeFile(string fileName)
        {
            var fileInfo = new ProcessedFileInfo
            {
                OriginalFileName = fileName,
                IsArchive = IsArchiveFile(fileName)
            };

            // Pattern: MaCN_MaBC_NamThangNgay.extension
            // Example: 7800_GL01_20250531.zip, 7800_LN01_20250531.csv
            var pattern = @"^(\d{4})_([A-Za-z0-9]+)_(\d{8})\.(.+)$";
            var match = Regex.Match(fileName, pattern);

            if (match.Success)
            {
                fileInfo.BranchCode = match.Groups[1].Value;
                fileInfo.ReportCode = match.Groups[2].Value;
                fileInfo.DateString = match.Groups[3].Value;
                fileInfo.Extension = match.Groups[4].Value.ToLower();
                fileInfo.StatementDate = ParseDateFromFileName(fileInfo.DateString);
                fileInfo.Category = DetermineCategory(fileInfo.BranchCode, fileInfo.ReportCode);
                fileInfo.IsValidFormat = true;
            }
            else
            {
                // Fallback for non-standard file names
                fileInfo.Category = "General";
                fileInfo.IsValidFormat = false;
                fileInfo.Extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
            }

            return fileInfo;
        }

        // 📅 Chuyển đổi ngày từ tên file (yyyyMMdd -> dd/MM/yyyy)
        private DateTime? ParseDateFromFileName(string dateString)
        {
            if (string.IsNullOrEmpty(dateString) || dateString.Length != 8)
                return null;

            if (DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            return null;
        }

        // 🏢 Xác định danh mục dựa trên mã chi nhánh và mã báo cáo
        private string DetermineCategory(string branchCode, string reportCode)
        {
            var branchName = GetBranchName(branchCode);
            var reportType = GetReportType(reportCode);
            
            return $"{branchName}_{reportType}";
        }

        private string GetBranchName(string branchCode)
        {
            return branchCode switch
            {
                "7800" => "CnLaiChau",
                "7801" => "CnTamDuong", 
                "7802" => "CnPhongTho",
                "7803" => "CnSinHo",
                "7804" => "CnMuongTe",
                "7805" => "CnThanUyen",
                "7806" => "CnThanhPho",
                "7807" => "CnTanUyen",
                "7808" => "CnNamNhun",
                
                _ => $"Chi_nhanh_{branchCode}"
            };
        }

        private string GetReportType(string reportCode)
        {
            return reportCode switch
            {
                "GL01" => "So_cai_tong_hop",
                "LN01" => "Bao_cao_tin_dung",
                "DP01" => "Bao_cao_tien_gui",
                "TR01" => "Bao_cao_giao_dich",
                "KH01" => "Bao_cao_khach_hang",
                _ => reportCode
            };
        }

        // 🗜️ Kiểm tra file có phải archive không
        private bool IsArchiveFile(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return new[] { ".zip", ".7z", ".rar", ".tar", ".gz" }.Contains(extension);
        }

        // 📦 Xử lý file nén
        private async Task<List<ImportResult>> ProcessArchiveFile(IFormFile archiveFile, string password, ProcessedFileInfo fileInfo)
        {
            var results = new List<ImportResult>();

            try
            {
                using var archiveStream = archiveFile.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await archiveStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Xử lý file ZIP
                if (fileInfo.Extension == "zip")
                {
                    using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                    
                    var entries = archive.Entries
                        .Where(e => !e.FullName.EndsWith("/") && !string.IsNullOrEmpty(e.Name))
                        .Where(e => IsSupportedFileType(e.Name))
                        .OrderBy(e => e.Name) // Sắp xếp theo tên file
                        .ToList();

                    foreach (var entry in entries)
                    {
                        try
                        {
                            var extractedFileInfo = AnalyzeFile(entry.Name);
                            extractedFileInfo.IsFromArchive = true;
                            extractedFileInfo.ArchiveFileName = archiveFile.FileName;

                            using var entryStream = entry.Open();
                            
                            // Kiểm tra nếu entry bị bảo vệ bằng mật khẩu
                            try
                            {
                                // Thử đọc một số byte đầu để kiểm tra password
                                var buffer = new byte[10];
                                await entryStream.ReadAsync(buffer, 0, buffer.Length);
                                entryStream.Position = 0; // Reset position
                            }
                            catch (InvalidDataException)
                            {
                                // Có thể file bị password protect hoặc corrupted
                                if (!string.IsNullOrEmpty(password))
                                {
                                    // TODO: Implement password-protected ZIP extraction
                                    // Hiện tại .NET ZipArchive không hỗ trợ password, cần thư viện khác như SharpCompress
                                    results.Add(new ImportResult
                                    {
                                        Success = false,
                                        FileName = entry.Name,
                                        Message = "Password-protected ZIP files require additional library support",
                                        BranchCode = fileInfo.BranchCode
                                    });
                                    continue;
                                }
                                else
                                {
                                    results.Add(new ImportResult
                                    {
                                        Success = false,
                                        FileName = entry.Name,
                                        Message = "File may be password-protected or corrupted",
                                        BranchCode = fileInfo.BranchCode
                                    });
                                    continue;
                                }
                            }

                            var result = await ProcessStreamAsFile(entryStream, entry.Name, extractedFileInfo);
                            results.Add(result);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing archive entry {EntryName}", entry.Name);
                            results.Add(new ImportResult
                            {
                                Success = false,
                                FileName = entry.Name,
                                Message = $"Error processing archive entry: {ex.Message}",
                                BranchCode = fileInfo.BranchCode
                            });
                        }
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing archive file {FileName}", archiveFile.FileName);
                return new List<ImportResult>
                {
                    new ImportResult
                    {
                        Success = false,
                        FileName = archiveFile.FileName,
                        Message = $"Error processing archive: {ex.Message}",
                        BranchCode = fileInfo.BranchCode
                    }
                };
            }
        }

        // 📄 Xử lý file đơn lẻ
        private async Task<ImportResult> ProcessSingleFile(IFormFile file, ProcessedFileInfo fileInfo)
        {
            try
            {
                using var stream = file.OpenReadStream();
                return await ProcessStreamAsFile(stream, file.FileName, fileInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing single file {FileName}", file.FileName);
                return new ImportResult
                {
                    Success = false,
                    FileName = file.FileName,
                    Message = $"Error processing file: {ex.Message}",
                    BranchCode = fileInfo.BranchCode
                };
            }
        }

        // 🔄 Xử lý stream như file
        private async Task<ImportResult> ProcessStreamAsFile(Stream stream, string fileName, ProcessedFileInfo fileInfo)
        {
            var record = new ImportedDataRecord
            {
                FileName = fileName,
                FileType = GetMimeType(fileName),
                Category = fileInfo.Category,
                ImportDate = DateTime.UtcNow,
                ImportedBy = "System", // TODO: Get from auth context
                Status = "Processing"
            };

            // Store original file data
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            record.OriginalFileData = memoryStream.ToArray();
            memoryStream.Position = 0;

            // Process based on file type
            var items = new List<ImportedDataItem>();
            var extension = Path.GetExtension(fileName).ToLower();

            switch (extension)
            {
                case ".xlsx":
                case ".xls":
                    items = await ProcessExcelStream(memoryStream, fileInfo);
                    break;
                case ".csv":
                    items = await ProcessCsvStream(memoryStream, fileInfo);
                    break;
                default:
                    throw new NotSupportedException($"File type {extension} is not supported");
            }

            record.RecordsCount = items.Count;
            record.Status = items.Any() ? "Completed" : "Failed";
            record.ImportedDataItems = items;

            // Add metadata
            if (fileInfo.StatementDate.HasValue)
            {
                record.Notes = $"Statement Date: {fileInfo.StatementDate.Value:dd/MM/yyyy}";
            }

            _context.ImportedDataRecords.Add(record);
            await _context.SaveChangesAsync();

            return new ImportResult
            {
                Success = true,
                FileName = fileName,
                RecordsProcessed = items.Count,
                Message = $"Successfully processed {items.Count} records",
                BranchCode = fileInfo.BranchCode,
                ReportCode = fileInfo.ReportCode,
                StatementDate = fileInfo.StatementDate,
                Category = fileInfo.Category
            };
        }

        // 📊 Xử lý Excel stream
        private Task<List<ImportedDataItem>> ProcessExcelStream(Stream stream, ProcessedFileInfo fileInfo)
        {
            var items = new List<ImportedDataItem>();
            
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
                var rowData = new Dictionary<string, object>();
                for (int col = 1; col <= headers.Count; col++)
                {
                    var cellValue = worksheet.Cell(row, col).Value;
                    rowData[headers[col - 1]] = cellValue.ToString() ?? "";
                }

                // Add metadata
                rowData["_BranchCode"] = fileInfo.BranchCode ?? "";
                rowData["_ReportCode"] = fileInfo.ReportCode ?? "";
                rowData["_StatementDate"] = fileInfo.StatementDate?.ToString("yyyy-MM-dd") ?? "";
                rowData["_ProcessedDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                items.Add(new ImportedDataItem
                {
                    RawData = System.Text.Json.JsonSerializer.Serialize(rowData),
                    ProcessedDate = DateTime.UtcNow
                });
            }

            return Task.FromResult(items);
        }

        // 📝 Xử lý CSV stream  
        private async Task<List<ImportedDataItem>> ProcessCsvStream(Stream stream, ProcessedFileInfo fileInfo)
        {
            var items = new List<ImportedDataItem>();
            
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            if (firstLine == null) return items;

            var headers = firstLine.Split(',').Select(h => h.Trim()).ToArray();

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var values = line.Split(',').Select(v => v.Trim()).ToArray();
                var rowData = new Dictionary<string, object>();

                for (int i = 0; i < Math.Min(headers.Length, values.Length); i++)
                {
                    rowData[headers[i]] = values[i];
                }

                // Add metadata
                rowData["_BranchCode"] = fileInfo.BranchCode ?? "";
                rowData["_ReportCode"] = fileInfo.ReportCode ?? "";
                rowData["_StatementDate"] = fileInfo.StatementDate?.ToString("yyyy-MM-dd") ?? "";
                rowData["_ProcessedDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                items.Add(new ImportedDataItem
                {
                    RawData = System.Text.Json.JsonSerializer.Serialize(rowData),
                    ProcessedDate = DateTime.UtcNow
                });
            }

            return items;
        }

        // 🔍 Kiểm tra loại file được hỗ trợ
        private bool IsSupportedFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return new[] { ".csv", ".xlsx", ".xls" }.Contains(extension);
        }

        // 📎 Lấy MIME type
        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".csv" => "text/csv",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        // GET: api/DataImport/statistics - Get import statistics and branch performance
        [HttpGet("statistics")]
        public async Task<ActionResult<ImportStatisticsResponse>> GetImportStatistics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var query = _context.ImportedDataRecords.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(r => r.ImportDate >= fromDate.Value);
                
                if (toDate.HasValue)
                    query = query.Where(r => r.ImportDate <= toDate.Value);

                var records = await query
                    .Include(r => r.ImportedDataItems)
                    .ToListAsync();

                var statistics = new ImportStatisticsResponse
                {
                    TotalFiles = records.Count,
                    TotalRecords = records.Sum(r => r.RecordsCount),
                    SuccessfulImports = records.Count(r => r.Status == "Completed"),
                    FailedImports = records.Count(r => r.Status == "Failed"),
                    FromDate = fromDate,
                    ToDate = toDate,
                    GeneratedAt = DateTime.UtcNow
                };

                // Branch-wise statistics
                var branchStats = records
                    .Where(r => r.Category.Contains("Chi_nhanh_"))
                    .GroupBy(r => ExtractBranchFromCategory(r.Category))
                    .Select(g => new BranchStatistic
                    {
                        BranchName = g.Key,
                        FileCount = g.Count(),
                        RecordCount = g.Sum(r => r.RecordsCount),
                        LastImportDate = g.Max(r => r.ImportDate)
                    })
                    .OrderBy(b => b.BranchName)
                    .ToList();

                statistics.BranchStatistics = branchStats;

                // Report type statistics
                var reportStats = records
                    .Where(r => r.Category.Contains("_"))
                    .GroupBy(r => ExtractReportTypeFromCategory(r.Category))
                    .Select(g => new ReportTypeStatistic
                    {
                        ReportType = g.Key,
                        FileCount = g.Count(),
                        RecordCount = g.Sum(r => r.RecordsCount)
                    })
                    .OrderBy(r => r.ReportType)
                    .ToList();

                statistics.ReportTypeStatistics = reportStats;

                // Recent imports (last 10)
                statistics.RecentImports = records
                    .OrderByDescending(r => r.ImportDate)
                    .Take(10)
                    .Select(r => new RecentImport
                    {
                        FileName = r.FileName,
                        Category = r.Category,
                        ImportDate = r.ImportDate,
                        RecordsCount = r.RecordsCount,
                        Status = r.Status
                    })
                    .ToList();

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving import statistics");
                return StatusCode(500, new { message = "Error retrieving import statistics", error = ex.Message });
            }
        }

        // 🔍 Helper methods for statistics
        private string ExtractBranchFromCategory(string category)
        {
            var parts = category.Split('_');
            // Handle both old format (Chi_nhanh_TenCN) and new format (CnTenCN_ReportType)
            if (parts.Length >= 1 && parts[0].StartsWith("Cn"))
                return parts[0]; // New format: CnLaiChau
            else if (parts.Length >= 3)
                return $"{parts[0]}_{parts[1]}_{parts[2]}"; // Old format: Chi_nhanh_TenCN
            return category;
        }

        private string ExtractReportTypeFromCategory(string category)
        {
            var parts = category.Split('_');
            // Handle both old format (Chi_nhanh_TenCN_ReportType) and new format (CnTenCN_ReportType)
            if (parts.Length >= 2 && parts[0].StartsWith("Cn"))
                return string.Join("_", parts.Skip(1)); // New format: Skip CnTenCN, get ReportType
            else if (parts.Length >= 4)
                return string.Join("_", parts.Skip(3)); // Old format: Skip Chi_nhanh_TenCN, get ReportType
            return category;
        }

        // GET: api/DataImport/by-statement-date - Get imported data by statement date
        [HttpGet("by-statement-date")]
        public async Task<ActionResult<IEnumerable<ImportedDataRecord>>> GetImportedDataByStatementDate(
            [FromQuery] DateTime? statementDate,
            [FromQuery] string? fileType = null)
        {
            try
            {
                var query = _context.ImportedDataRecords.AsQueryable();

                if (statementDate.HasValue)
                {
                    query = query.Where(r => r.StatementDate.HasValue && 
                                           r.StatementDate.Value.Date == statementDate.Value.Date);
                }

                if (!string.IsNullOrEmpty(fileType))
                {
                    query = query.Where(r => r.Category.Contains(fileType) || r.FileName.Contains(fileType));
                }

                var importedData = await query
                    .Include(r => r.ImportedDataItems)
                    .OrderByDescending(r => r.StatementDate ?? r.ImportDate)
                    .ToListAsync();

                return Ok(importedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving imported data by statement date");
                return StatusCode(500, new { message = "Error retrieving imported data by statement date", error = ex.Message });
            }
        }

        // GET: api/DataImport/{id}/decompress - Get decompressed data
        // [HttpGet("{id}/decompress")]
        // Tạm thời comment out để tránh lỗi CompressedData trong database
        /*
        public async Task<ActionResult<object>> GetDecompressedData(int id)
        {
            try
            {
                var record = await _context.ImportedDataRecords.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { message = "Import record not found" });
                }

                if (record.CompressedData == null)
                {
                    return NotFound(new { message = "No compressed data found for this record" });
                }

                var decompressedData = await _compressionService.DecompressDataAsync<List<object>>(record.CompressedData);
                
                return Ok(new 
                {
                    Id = record.Id,
                    FileName = record.FileName,
                    StatementDate = record.StatementDate,
                    CompressionRatio = record.CompressionRatio,
                    RecordsCount = record.RecordsCount,
                    Data = decompressedData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decompressing data for record {RecordId}", id);
                return StatusCode(500, new { message = "Error decompressing data", error = ex.Message });
            }
        }
        */

        /// <summary>
        /// 📦 Xử lý file ZIP - Tự động giải nén và import các CSV
        /// </summary>
        private async Task<List<ImportedDataItem>> ProcessZipFile(IFormFile zipFile, string? category, DateTime? defaultStatementDate)
        {
            var allItems = new List<ImportedDataItem>();
            
            try
            {
                using var zipStream = zipFile.OpenReadStream();
                using var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Read);
                
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"📋 Processing CSV from ZIP: {entry.FullName}");
                        
                        // Trích xuất ngày từ tên file CSV
                        var fileName = Path.GetFileNameWithoutExtension(entry.Name);
                        var dateMatch = System.Text.RegularExpressions.Regex.Match(fileName, @"(\d{8})");
                        var statementDate = defaultStatementDate;
                        
                        if (dateMatch.Success && DateTime.TryParseExact(dateMatch.Value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime extractedDate))
                        {
                            statementDate = extractedDate;
                        }
                        
                        // Đọc và xử lý CSV từ ZIP entry
                        using var entryStream = entry.Open();
                        using var reader = new StreamReader(entryStream);
                        
                        var csvContent = await reader.ReadToEndAsync();
                        var csvItems = await ProcessCsvContent(csvContent, entry.Name);
                        
                        // Add metadata to processing notes
                        foreach (var item in csvItems)
                        {
                            var metadata = new Dictionary<string, object>
                            {
                                ["sourceFileName"] = entry.Name,
                                ["statementDate"] = statementDate?.ToString("yyyy-MM-dd") ?? "N/A",
                                ["category"] = category ?? "Unknown",
                                ["extractedFromZip"] = zipFile.FileName
                            };
                            
                            item.ProcessingNotes = System.Text.Json.JsonSerializer.Serialize(metadata);
                        }
                        
                        allItems.AddRange(csvItems);
                        Console.WriteLine($"✅ Processed {csvItems.Count} records from {entry.Name}");
                    }
                }
                
                Console.WriteLine($"📦 ZIP processing complete. Total records: {allItems.Count}");
                return allItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing ZIP file: {ex.Message}");
                throw new Exception($"Không thể xử lý file ZIP: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Xử lý nội dung CSV từ string
        /// </summary>
        private async Task<List<ImportedDataItem>> ProcessCsvContent(string csvContent, string sourceFileName)
        {
            var items = new List<ImportedDataItem>();
            
            try
            {
                using var reader = new StringReader(csvContent);
                var firstLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(firstLine)) return items;
                
                var headers = firstLine.Split(',').Select(h => h.Trim('"')).ToList();
                
                string? line;
                int rowIndex = 1;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    rowIndex++;
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    
                    var values = line.Split(',').Select(v => v.Trim('"')).ToList();
                    var data = new Dictionary<string, object>();
                    
                    for (int i = 0; i < Math.Min(headers.Count, values.Count); i++)
                    {
                        data[headers[i]] = values[i];
                    }
                    
                    // Add metadata to the data
                    data["_sourceFileName"] = sourceFileName;
                    data["_rowIndex"] = rowIndex;
                    
                    items.Add(new ImportedDataItem
                    {
                        RawData = System.Text.Json.JsonSerializer.Serialize(data),
                        ProcessedDate = DateTime.UtcNow,
                        ProcessingNotes = $"Extracted from {sourceFileName}, row {rowIndex}"
                    });
                }
                
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing CSV content from {sourceFileName}: {ex.Message}");
                throw;
            }
        }

        // Helper method to check if JSON is empty
        private bool IsEmptyJsonObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return true;
            
            try
            {
                // Thử parse thành Dictionary
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (dict == null || dict.Count == 0)
                    return true;
                
                // Kiểm tra nếu tất cả giá trị đều null hoặc rỗng
                bool allEmpty = true;
                foreach (var value in dict.Values)
                {
                    if (value != null && value.ToString() != "")
                    {
                        allEmpty = false;
                        break;
                    }
                }
                
                return allEmpty;
            }
            catch
            {
                // Thử parse thành List
                try 
                {
                    var list = System.Text.Json.JsonSerializer.Deserialize<List<object>>(json);
                    return list == null || list.Count == 0;
                }
                catch
                {
                    // Nếu không parse được, kiểm tra nếu chỉ có { }, [ ] hoặc whitespace
                    json = json.Trim();
                    return json == "{}" || json == "[]" ||
                           (json.StartsWith("{") && json.EndsWith("}") && json.Length <= 4) ||
                           (json.StartsWith("[") && json.EndsWith("]") && json.Length <= 4);
                }
            }
        }
    }

    // 📊 Data Transfer Objects và Models
    public class AdvancedDataImportRequest
    {
        public required IFormFileCollection Files { get; set; }
        public string? Password { get; set; } // For encrypted archives
    }

    public class ProcessedFileInfo
    {
        public required string OriginalFileName { get; set; }
        public string? BranchCode { get; set; }
        public string? ReportCode { get; set; }
        public string? DateString { get; set; }
        public string? Extension { get; set; }
        public DateTime? StatementDate { get; set; }
        public string Category { get; set; } = "General";
        public bool IsValidFormat { get; set; }
        public bool IsArchive { get; set; }
        public bool IsFromArchive { get; set; }
        public string? ArchiveFileName { get; set; }
    }

    public class DataImportRequest
    {
        public required IFormFileCollection Files { get; set; }
        public string? Category { get; set; }
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public required string FileName { get; set; }
        public int RecordsProcessed { get; set; }
        public required string Message { get; set; }
        public string? BranchCode { get; set; } // Added BranchCode
        public string? ReportCode { get; set; } // Added ReportCode  
        public DateTime? StatementDate { get; set; } // Added StatementDate
        public string? Category { get; set; } // Added Category
    }

    public class DataPreviewResponse
    {
        public long Id { get; set; }
        public required string FileName { get; set; }
        public required string Category { get; set; }
        public DateTime ImportDate { get; set; }
        public required string ImportedBy { get; set; }
        public int RecordsCount { get; set; }
        public List<Dictionary<string, object>> PreviewData { get; set; } = new();
        public List<string> Columns { get; set; } = new();
        
        // Thêm thông tin chi tiết về trạng thái hiển thị
        public object? Status { get; set; }
        public string? SummaryText { get; set; }
    }
}
