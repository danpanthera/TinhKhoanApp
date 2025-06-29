using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Validation;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Utils; // 🕐 Thêm Utils cho VietnamDateTime
using ClosedXML.Excel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Dapper; // 🔥 Thêm Dapper cho SQL queries
using Microsoft.Data.SqlClient; // 🔥 Thêm SqlClient cho kết nối SQL Server
using System.Text; // 🔥 Để build SQL queries

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RawDataController> _logger;
        private readonly IConfiguration _configuration; // 🔥 Thêm Configuration để lấy connection string
        private readonly IRawDataProcessingService _processingService; // 🔥 Inject processing service
        private readonly IFileNameParsingService _fileNameParsingService; // 🔧 CHUẨN HÓA: Inject filename parsing service
        private readonly ILegacyExcelReaderService _legacyExcelReaderService; // 📊 Inject legacy Excel reader service

        // 📋 Danh sách định nghĩa loại dữ liệu - ĐỒNG BỘ TẤT CẢ LOẠI
        private static readonly Dictionary<string, string> DataTypeDefinitions = new()
        {
            { "LN01", "Dữ liệu LOAN - Danh mục tín dụng" },
            { "LN02", "Sao kê biến động nhóm nợ - Theo dõi chất lượng tín dụng" },
            { "LN03", "Dữ liệu Nợ XLRR - Nợ xử lý rủi ro" },
            { "DP01", "Dữ liệu Tiền gửi - Huy động vốn" },
            { "EI01", "Dữ liệu mobile banking - Giao dịch điện tử" },
            { "GL01", "Dữ liệu bút toán GDV - Giao dịch viên" },
            { "DPDA", "Dữ liệu sao kê phát hành thẻ - Thẻ tín dụng/ghi nợ" },
            { "DB01", "Sao kê TSDB và Không TSDB - Tài sản đảm bảo" },
            { "KH03", "Sao kê Khách hàng pháp nhân - Doanh nghiệp" },
            { "BC57", "Sao kê Lãi dự thu - Dự phòng lãi" },
            { "RR01", "Sao kê dư nợ gốc, lãi XLRR - Rủi ro tín dụng" },
            { "7800_DT_KHKD1", "Báo cáo KHKD (DT) - Kế hoạch kinh doanh doanh thu" },
            { "GLCB41", "Bảng cân đối - Báo cáo tài chính" }
        };

        public RawDataController(ApplicationDbContext context, ILogger<RawDataController> logger, IConfiguration configuration, IRawDataProcessingService processingService, IFileNameParsingService fileNameParsingService, ILegacyExcelReaderService legacyExcelReaderService)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration; // 🔥 Inject configuration để lấy connection string
            _processingService = processingService; // 🔥 Inject processing service
            _fileNameParsingService = fileNameParsingService; // 🔧 CHUẨN HÓA: Inject filename parsing service
            _legacyExcelReaderService = legacyExcelReaderService; // 📊 Inject legacy Excel reader service
        }

        // 📋 GET: api/RawData - Lấy danh sách tất cả dữ liệu thô từ Temporal Tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRawDataImports()
        {
            try
            {
                _logger.LogInformation("🔍 Lấy danh sách dữ liệu từ Temporal Tables...");

                // 🔥 LẤY DỮ LIỆU THẬT TỪ LEGACY TABLES (File Import Tracking)
                var rawDataRecords = await _context.ImportedDataRecords
                    .OrderByDescending(x => x.ImportDate) // ✅ Sắp xếp theo ngày import mới nhất
                    .ToListAsync();

                var rawDataImports = rawDataRecords
                    .Select(x => new
                    {
                        x.Id,
                        x.FileName,
                        DataType = x.Category ?? x.FileType, // ✅ Ưu tiên Category trước, fallback về FileType
                        Category = x.Category ?? x.FileType, // ✅ Đảm bảo Category không null
                        FileType = x.FileType, // ✅ Giữ nguyên FileType
                        x.ImportDate,
                        x.StatementDate,
                        x.ImportedBy,
                        x.Status,
                        x.RecordsCount,
                        x.Notes,
                        BranchCode = "7800", // Default branch code - will be extracted later
                        // Tạo RecordsPreview từ imported data items
                        RecordsPreview = new List<object>
                        {
                            new { Id = x.Id * 10 + 1, ProcessedDate = x.ImportDate, ProcessingNotes = $"{x.FileType} data processed successfully" },
                            new { Id = x.Id * 10 + 2, ProcessedDate = x.ImportDate, ProcessingNotes = $"Import {x.FileName} completed" }
                        }
                    })
                    .OrderByDescending(x => x.ImportDate) // ✅ Sắp xếp theo ngày import mới nhất trước
                    .Select(x => new
                    {
                        x.Id,
                        x.FileName,
                        x.DataType,
                        x.Category,
                        x.FileType,
                        x.ImportDate,
                        x.StatementDate,
                        x.ImportedBy,
                        x.Status,
                        x.RecordsCount,
                        x.Notes,
                        BranchCode = "7800", // Default branch code - will be extracted later
                        x.RecordsPreview
                    })
                    .ToList();

                // Post-process to extract BranchCode from Notes
                var processedRawDataImports = new List<object>();
                foreach (var item in rawDataImports)
                {
                    // Inline branch code extraction to avoid EF translation issues
                    string branchCode = "7800"; // Default
                    if (!string.IsNullOrEmpty(item.Notes))
                    {
                        var match = Regex.Match(item.Notes, @"Branch: (78\d\d)");
                        if (match.Success)
                        {
                            branchCode = match.Groups[1].Value;
                        }
                    }

                    processedRawDataImports.Add(new
                    {
                        item.Id,
                        item.FileName,
                        item.DataType,
                        item.Category,
                        item.FileType,
                        item.ImportDate,
                        item.StatementDate,
                        item.ImportedBy,
                        item.Status,
                        item.RecordsCount,
                        item.Notes,
                        BranchCode = branchCode,
                        item.RecordsPreview
                    });
                }

                _logger.LogInformation("✅ Trả về {Count} import items từ ImportedDataRecords", processedRawDataImports.Count);

                return Ok(processedRawDataImports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi khi lấy danh sách Raw Data imports từ Temporal Tables");
                return StatusCode(500, new { message = "Lỗi server khi lấy dữ liệu từ database", error = ex.Message });
            }
        }




        // �📤 POST: api/RawData/import/{dataType} - Import dữ liệu theo loại
        [HttpPost("import/{dataType}")]
        public async Task<IActionResult> ImportRawData(string dataType, [FromForm] RawDataImportRequest request)
        {
            try
            {
                _logger.LogInformation($"🔄 Bắt đầu import dữ liệu với dataType: '{dataType}'");
                _logger.LogInformation($"📋 Request - Files: {request.Files?.Count ?? 0}, Notes: '{request.Notes}'");

                // ✅ Kiểm tra Model State
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value?.Errors ?? new Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection())
                                          .Select(x => x.ErrorMessage);
                    _logger.LogWarning($"❌ Model validation failed: {string.Join(", ", errors)}");

                    // 🔍 Debug đặc biệt cho GL01 với file lớn
                    if (dataType.ToUpper() == "GL01")
                    {
                        _logger.LogError($"🔍 GL01 Import Debug - Model State Invalid");
                        _logger.LogError($"🔍 File Count: {request.Files?.Count ?? 0}");
                        if (request.Files != null)
                        {
                            foreach (var file in request.Files)
                            {
                                _logger.LogError($"🔍 File: {file.FileName}, Size: {file.Length} bytes");
                            }
                        }
                        _logger.LogError($"🔍 Detailed Model State Errors:");
                        foreach (var kvp in ModelState)
                        {
                            foreach (var error in kvp.Value?.Errors ?? new Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection())
                            {
                                _logger.LogError($"🔍 Key: {kvp.Key}, Error: {error.ErrorMessage}");
                            }
                        }
                    }

                    return BadRequest(new { message = "Validation failed", errors = errors });
                }

                // ✅ Kiểm tra loại dữ liệu hợp lệ
                if (!DataTypeDefinitions.ContainsKey(dataType.ToUpper()))
                {
                    _logger.LogWarning($"❌ Loại dữ liệu không hỗ trợ: '{dataType}'. Các loại hỗ trợ: {string.Join(", ", DataTypeDefinitions.Keys)}");
                    return BadRequest(new { message = $"Loại dữ liệu '{dataType}' không được hỗ trợ" });
                }

                if (request.Files == null || !request.Files.Any())
                {
                    return BadRequest(new { message = "Không có file nào được chọn" });
                }

                // 🔍 Debug file size cho GL01
                if (dataType.ToUpper() == "GL01")
                {
                    _logger.LogInformation($"🔍 GL01 Upload Debug - Processing {request.Files.Count} files");
                    foreach (var file in request.Files)
                    {
                        _logger.LogInformation($"🔍 GL01 File: {file.FileName}, Size: {file.Length} bytes ({file.Length / 1024.0 / 1024.0:F2} MB)");
                        _logger.LogInformation($"🔍 GL01 Content Type: {file.ContentType}");
                    }
                }

                var results = new List<RawDataImportResult>();

                foreach (var file in request.Files)
                {
                    _logger.LogInformation("🔍 Validating file: {FileName} for dataType: {DataType}", file.FileName, dataType);

                    // 🔥 VALIDATION 1: Kiểm tra định dạng file (chỉ cho phép XLS, XLSX, CSV)
                    var allowedExtensions = new[] { ".xls", ".xlsx", ".csv" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        _logger.LogWarning("❌ File extension not allowed: {Extension}", fileExtension);
                        results.Add(new RawDataImportResult
                        {
                            Success = false,
                            FileName = file.FileName,
                            Message = $"❌ Định dạng file không được hỗ trợ. Chỉ cho phép: {string.Join(", ", allowedExtensions)}"
                        });
                        continue;
                    }

                    // � CHUẨN HÓA: Validation format filename theo chuẩn MaCN_LoaiFile_Ngay.ext
                    var parseResult = _fileNameParsingService.ParseFileName(file.FileName);

                    // Log kết quả parse filename
                    _logger.LogInformation("🔍 Filename parse result for {FileName}: Valid={IsValid}, BranchCode={BranchCode}, DataType={DataType}, Date={Date}",
                        file.FileName, parseResult.IsValid, parseResult.BranchCode, parseResult.DataType, parseResult.StatementDate);

                    // �🔥 VALIDATION 2: Kiểm tra format filename (khuyến nghị format chuẩn)
                    if (!parseResult.IsValid)
                    {
                        _logger.LogWarning("⚠️ Non-standard filename format: {FileName} - {Error}", file.FileName, parseResult.ErrorMessage);
                        // Không reject, chỉ warning vì vẫn có thể extract được thông tin cơ bản
                    }

                    // 🔥 VALIDATION 3: Kiểm tra loại dữ liệu từ filename có khớp với dataType không
                    if (!string.IsNullOrEmpty(parseResult.DataType) &&
                        !parseResult.DataType.Equals(dataType, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("⚠️ Data type mismatch: URL={URLDataType}, Filename={FilenameDataType}",
                            dataType, parseResult.DataType);
                        results.Add(new RawDataImportResult
                        {
                            Success = false,
                            FileName = file.FileName,
                            Message = $"❌ Loại dữ liệu không khớp: URL yêu cầu '{dataType}' nhưng filename chứa '{parseResult.DataType}'"
                        });
                        continue;
                    }

                    // 🔥 VALIDATION 4: Kiểm tra tên file chứa mã loại dữ liệu (fallback nếu không parse được)
                    bool isValidFileName = false;

                    // Special handling for GL01 - relax filename validation but still check extension
                    if (dataType.ToUpper() == "GL01")
                    {
                        isValidFileName = fileExtension == ".csv"; // GL01 chỉ cho phép CSV
                        _logger.LogInformation("🔍 GL01 validation: CSV extension = {IsValid}", isValidFileName);
                    }
                    // 🔥 VALIDATION ĐẶC BIỆT CHO BC57: Phải chứa "BCDT" trong tên file
                    else if (dataType.ToUpper() == "BC57")
                    {
                        isValidFileName = file.FileName.Contains(dataType, StringComparison.OrdinalIgnoreCase) &&
                                        file.FileName.Contains("BCDT", StringComparison.OrdinalIgnoreCase);
                        _logger.LogInformation("🔍 BC57 validation: filename contains BC57={ContainsBC57}, contains BCDT={ContainsBCDT}, overall={IsValid}",
                            file.FileName.Contains(dataType, StringComparison.OrdinalIgnoreCase),
                            file.FileName.Contains("BCDT", StringComparison.OrdinalIgnoreCase),
                            isValidFileName);
                    }
                    else
                    {
                        // Tất cả loại khác: tên file PHẢI chứa mã dataType
                        isValidFileName = file.FileName.Contains(dataType, StringComparison.OrdinalIgnoreCase);
                        _logger.LogInformation("🔍 {DataType} validation: filename contains dataType = {IsValid}", dataType, isValidFileName);
                    }

                    if (!isValidFileName)
                    {
                        var errorMsg = dataType.ToUpper() switch
                        {
                            "GL01" => "❌ GL01 file phải có định dạng .csv",
                            "BC57" => "❌ BC57 file phải chứa cả 'BC57' và 'BCDT' trong tên file",
                            _ => $"❌ Tên file phải chứa mã '{dataType}'"
                        };

                        _logger.LogWarning("❌ File validation failed: {Message}", errorMsg);
                        results.Add(new RawDataImportResult
                        {
                            Success = false,
                            FileName = file.FileName,
                            Message = errorMsg
                        });
                        continue;
                    }

                    // 🔥 VALIDATION PASSED - Process file
                    _logger.LogInformation("✅ File validation passed: {FileName}", file.FileName);

                    var result = await ProcessSingleFile(file, dataType, request.Notes ?? "");
                    results.Add(result);

                    // ✅ File đơn đã được xử lý thành công
                    if (result.Success)
                    {
                        _logger.LogInformation("✅ Đã xử lý file đơn {FileName} thành công", file.FileName);
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

        // 👁️ GET: api/RawData/{id}/preview - Xem trước dữ liệu đã import
        [HttpGet("{id:int}/preview")]
        public async Task<ActionResult<object>> PreviewRawDataImport(int id)
        {
            try
            {
                _logger.LogInformation("🔍 Preview request for import ID: {Id} từ ImportedDataRecords table", id);

                // 🔥 LẤY THÔNG TIN IMPORT TỪ IMPORTED DATA RECORDS (File Import Metadata)
                var import = await _context.ImportedDataRecords
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (import == null)
                {
                    _logger.LogWarning("❌ Import ID {Id} not found in ImportedDataRecords, returning mock data", id);
                    // ⚡ FALLBACK: Trả về dữ liệu mock nếu không tìm thấy
                    // 🚨 XÓA MOCK DATA: Không trả về mock data, trả về error thực tế
                    _logger.LogWarning("❌ Import record {ImportId} not found in database", id);
                    return NotFound(new { message = $"Không tìm thấy bản ghi import với ID {id}" });
                }

                _logger.LogInformation("✅ Found import: {FileName}, Category: {Category}, Records: {RecordsCount}",
                    import.FileName, import.Category, import.RecordsCount);

                // � CRITICAL FIX: Lấy dữ liệu THỰC từ database thay vì mock data
                var importedItems = await _context.ImportedDataItems
                    .Where(item => item.ImportedDataRecordId == import.Id)
                    .OrderBy(item => item.Id)
                    .ToListAsync();

                _logger.LogInformation("📊 Loading {ItemCount} real data items from database", importedItems.Count);

                // Parse dữ liệu thực từ database
                var realPreviewData = new List<object>();
                foreach (var item in importedItems)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.RawData))
                        {
                            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);
                            if (data != null && data.Count > 0)
                            {
                                realPreviewData.Add(data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("⚠️ Failed to parse item {ItemId}: {Error}", item.Id, ex.Message);
                    }
                }

                _logger.LogInformation("✅ Parsed {ParsedCount}/{TotalCount} real data records successfully",
                    realPreviewData.Count, importedItems.Count);

                var dataTypeForPreview = !string.IsNullOrEmpty(import.Category) ? import.Category : "LN01";

                var response = new
                {
                    importInfo = new
                    {
                        import.Id,
                        import.FileName,
                        DataType = dataTypeForPreview, // Map Category to DataType for frontend compatibility
                        import.ImportDate,
                        import.StatementDate,
                        import.RecordsCount,
                        import.Status,
                        import.ImportedBy
                    },
                    previewData = realPreviewData, // 🚨 DÙNG DỮ LIỆU THỰC thay vì mock
                    totalRecords = import.RecordsCount,
                    previewRecords = realPreviewData.Count, // Đếm số records thực tế
                    temporalTablesEnabled = true,
                    isMockData = false,
                    dataSource = "REAL_DATABASE" // Đánh dấu là dữ liệu thực
                };

                _logger.LogInformation("🎯 Generated preview with {PreviewCount} REAL records for {Category}",
                    realPreviewData.Count, import.Category);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi khi tạo preview cho import ID: {Id}. Error: {ErrorMessage}", id, ex.Message);

                // 🚨 XÓA MOCK DATA: Không trả về mock data, trả về lỗi thực tế
                return StatusCode(500, new
                {
                    message = "Lỗi khi lấy preview dữ liệu từ database",
                    error = ex.Message,
                    importId = id
                });
            }
        }

        // � XÓA MOCK DATA: Comment out method tạo mock data LOAN10001-LOAN10010
        /*
        // �🔄 Helper method để tạo dữ liệu preview theo loại
        private List<object> GeneratePreviewDataForType(string dataType, int totalRecords)
        {
            var records = new List<object>();
            int previewCount = Math.Min(10, totalRecords); // Hiển thị tối đa 10 records

            for (int i = 1; i <= previewCount; i++)
            {
                switch (dataType.ToUpper())
                {
                    case "LN01": // Dữ liệu LOAN
                        records.Add(new
                        {
                            soTaiKhoan = $"LOAN{10000 + i}",
                            tenKhachHang = $"Khách hàng vay {i}",
                            duNo = 100000000 + i * 10000000,
                            laiSuat = 6.5 + (i % 5) * 0.25,
                            hanMuc = 200000000 + i * 50000000,
                            ngayGiaiNgan = VietnamDateTime.Now.AddDays(-30 * (i % 12)).ToString("yyyy-MM-dd")
                        });
                        break;

                    case "DP01": // Dữ liệu tiền gửi
                        records.Add(new
                        {
                            soTaiKhoan = $"DP{20000 + i}",
                            tenKhachHang = $"Khách hàng tiền gửi {i}",
                            soTien = 50000000 + i * 5000000,
                            laiSuat = 3.2 + (i % 6) * 0.1,
                            kyHan = new string[] { "1 tháng", "3 tháng", "6 tháng", "12 tháng" }[i % 4],
                            ngayMoSo = VietnamDateTime.Now.AddDays(-60 * (i % 10)).ToString("yyyy-MM-dd")
                        });
                        break;

                    case "GL01": // Bút toán GDV
                        records.Add(new
                        {
                            soButToan = $"GL{50000 + i}",
                            maTaiKhoan = $"TK{1010 + (i % 10)}",
                            tenTaiKhoan = $"Tài khoản GL {i}",
                            soTienNo = (i % 2 == 0) ? 25000000 + i * 3000000 : 0,
                            soTienCo = (i % 2 == 1) ? 25000000 + i * 3000000 : 0,
                            ngayHachToan = VietnamDateTime.Now.AddDays(-i).ToString("yyyy-MM-dd")
                        });
                        break;

                    default: // Dữ liệu chung cho các loại khác
                        records.Add(new
                        {
                            id = i,
                            dataType = dataType,
                            sampleData = $"Sample data {i} for {dataType}",
                            recordValue = 1000000 + i * 100000,
                            processedDate = VietnamDateTime.Now.AddDays(-i).ToString("yyyy-MM-dd HH:mm:ss")
                        });
                        break;
                }
            }

            return records;
        }
        */

        // 👁️ GET: api/RawData/{id} - Lấy chi tiết một mẫu dữ liệu thô từ Temporal Tables
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetRawDataImport(int id)
        {
            try
            {
                _logger.LogInformation("🔍 Lấy chi tiết Raw Data import từ Temporal Tables với ID: {Id}", id);

                // 🔥 TÌM TRONG LEGACY TABLES
                var item = await _context.ImportedDataRecords
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        x.Id,
                        x.FileName,
                        DataType = x.Category, // Map Category to DataType
                        x.ImportDate,
                        x.StatementDate,
                        x.ImportedBy,
                        x.Status,
                        x.RecordsCount,
                        x.Notes,
                        // Tạo RecordsPreview mẫu
                        RecordsPreview = new List<object>
                        {
                            new { Id = x.Id * 10 + 1, ProcessedDate = x.ImportDate, ProcessingNotes = $"{x.Category} data processed successfully" },
                            new { Id = x.Id * 10 + 2, ProcessedDate = x.ImportDate, ProcessingNotes = $"Import {x.FileName} completed" }
                        }
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    _logger.LogWarning("❌ Không tìm thấy Raw Data import với ID: {Id}", id);
                    return NotFound(new { message = $"Không tìm thấy dữ liệu import với ID: {id}" });
                }

                _logger.LogInformation("✅ Đã tìm thấy Raw Data import với ID: {Id}, FileName: {FileName}", id, item.FileName);

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi khi lấy chi tiết Raw Data import với ID: {Id}", id);
                return StatusCode(500, new { message = "Lỗi server khi lấy chi tiết dữ liệu từ database", error = ex.Message });
            }
        }

        // ️ DELETE: api/RawData/{id} - Xóa dữ liệu import từ Temporal Tables
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRawDataImport(int id)
        {
            try
            {
                _logger.LogInformation("🗑️ Xóa dữ liệu import từ Temporal Tables với ID: {Id}", id);

                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ" });
                }

                // � TÌM VÀ XÓA TRONG TEMPORAL TABLES
                var import = await _context.ImportedDataRecords.FirstOrDefaultAsync(r => r.Id == id);

                if (import == null)
                {
                    _logger.LogWarning("❌ Không tìm thấy import với ID: {Id}", id);
                    return NotFound(new { message = $"Không tìm thấy dữ liệu import với ID: {id}" });
                }

                // Xóa dữ liệu
                _context.ImportedDataRecords.Remove(import);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Đã xóa thành công import với ID: {Id}, FileName: {FileName}", id, import.FileName);

                return Ok(new
                {
                    message = $"Xóa dữ liệu import ID {id} thành công từ Temporal Tables",
                    deletedId = id,
                    fileName = import.FileName,
                    temporalTablesEnabled = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi khi xóa dữ liệu import {ImportId} từ Temporal Tables", id);
                return StatusCode(500, new { message = "Lỗi khi xóa dữ liệu import từ database", error = ex.Message });
            }
        }

        /// <summary>
        /// 🗑️ Xóa triệt để toàn bộ dữ liệu đã import - KHÔNG THỂ HOÀN TÁC!
        /// </summary>
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllRawData()
        {
            try
            {
                _logger.LogWarning("🚨 Bắt đầu xóa TOÀN BỘ dữ liệu Import - KHÔNG THỂ HOÀN TÁC!");

                // 📊 Lấy count trước khi xóa để thông báo chi tiết
                int recordCount = await _context.ImportedDataRecords.CountAsync();
                int itemCount = await _context.ImportedDataItems.CountAsync();

                _logger.LogInformation($"📋 Sẽ xóa {recordCount} ImportedDataRecords và {itemCount} ImportedDataItems");

                // 🗑️ Xóa triệt để cả records và items với Raw SQL để tránh lỗi Temporal Tables
                using var deleteConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await deleteConnection.OpenAsync();

                // Xóa Items trước để tránh vi phạm foreign key
                if (itemCount > 0)
                {
                    await deleteConnection.ExecuteAsync("DELETE FROM ImportedDataItems");
                    _logger.LogInformation($"✅ Đã xóa {itemCount} ImportedDataItems bằng Raw SQL");
                }

                if (recordCount > 0)
                {
                    await deleteConnection.ExecuteAsync("DELETE FROM ImportedDataRecords");
                    _logger.LogInformation($"✅ Đã xóa {recordCount} ImportedDataRecords bằng Raw SQL");
                }

                // 🧹 Đếm và xóa các bảng dữ liệu động (nếu có)
                int dynamicTablesCleared = 0;
                foreach (var dataType in DataTypeDefinitions.Keys)
                {
                    try
                    {
                        var tableName = $"Data_{dataType}";
                        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                        // Kiểm tra bảng có tồn tại không
                        var tableExists = await connection.QueryFirstOrDefaultAsync<int>(
                            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName",
                            new { TableName = tableName });

                        if (tableExists > 0)
                        {
                            // Đếm records trước khi xóa
                            var recordsInTable = await connection.QueryFirstOrDefaultAsync<int>(
                                $"SELECT COUNT(*) FROM [{tableName}]");

                            if (recordsInTable > 0)
                            {
                                // Xóa dữ liệu trong bảng (giữ lại cấu trúc)
                                await connection.ExecuteAsync($"DELETE FROM [{tableName}]");
                                dynamicTablesCleared++;
                                _logger.LogInformation($"🗑️ Đã xóa {recordsInTable} records từ bảng {tableName}");
                            }
                        }
                    }
                    catch (Exception tableEx)
                    {
                        _logger.LogWarning($"⚠️ Không thể xóa bảng Data_{dataType}: {tableEx.Message}");
                    }
                }

                var successMessage = $"Đã xóa thành công {recordCount} bản ghi import, {itemCount} items dữ liệu và {dynamicTablesCleared} bảng dữ liệu động";
                _logger.LogInformation($"✅ {successMessage}");

                return Ok(new
                {
                    success = true,
                    message = successMessage,
                    recordsCleared = recordCount,
                    itemsCleared = itemCount,
                    dynamicTablesCleared = dynamicTablesCleared,
                    timestamp = VietnamDateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi xóa toàn bộ dữ liệu");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi xóa dữ liệu: " + ex.Message,
                    timestamp = VietnamDateTime.Now
                });
            }
        }

        // 🔍 GET: api/RawData/check-duplicate/{dataType}/{statementDate} - Kiểm tra dữ liệu trùng lặp từ Temporal Tables
        [HttpGet("check-duplicate/{dataType}/{statementDate}")]
        public async Task<IActionResult> CheckDuplicateData(string dataType, string statementDate, [FromQuery] string? fileName = null)
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra trùng lặp từ Temporal Tables - DataType: {DataType}, StatementDate: {StatementDate}, FileName: {FileName}",
                    dataType, statementDate, fileName);

                if (!DateTime.TryParseExact(statementDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                // � KIỂM TRA TRÙNG LẶP TRONG TEMPORAL TABLES
                try
                {
                    var existingImports = await _context.ImportedDataRecords
                        .Where(r => r.Category == dataType && r.StatementDate.HasValue && r.StatementDate.Value.Date == parsedDate.Date)
                        .Select(r => new
                        {
                            r.Id,
                            r.FileName,
                            r.ImportDate,
                            r.RecordsCount,
                            r.ImportedBy,
                            r.Status
                        })
                        .ToListAsync();

                    _logger.LogInformation("✅ Tìm thấy {Count} bản ghi trùng lặp trong Temporal Tables", existingImports.Count);

                    return Ok(new
                    {
                        hasDuplicate = existingImports.Any(),
                        existingImports = existingImports,
                        message = existingImports.Any()
                            ? $"Đã có {existingImports.Count} dữ liệu {dataType} cho ngày {parsedDate:dd/MM/yyyy}"
                            : "Không có dữ liệu trùng lặp",
                        temporalTablesEnabled = true
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 Lỗi khi kiểm tra trùng lặp trong Temporal Tables: {Error}", ex.Message);

                    // Trả về response lỗi với thông tin rõ ràng
                    return StatusCode(500, new
                    {
                        hasDuplicate = false,
                        existingImports = new object[] { },
                        message = "Lỗi khi kiểm tra trùng lặp trong database",
                        error = ex.Message,
                        temporalTablesEnabled = true
                    });
                }
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
                _logger.LogInformation("Attempting to delete data for type: {DataType}, date: {StatementDate}", dataType, statementDate);

                if (!DateTime.TryParseExact(statementDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                int deletedCount = 0;
                int deletedRecords = 0;

                try
                {
                    // � XÓA DỮ LIỆU THỰC TỪ TEMPORAL TABLES
                    var importsToDelete = await _context.ImportedDataRecords
                        .Where(r => r.Category == dataType && r.StatementDate.HasValue && r.StatementDate.Value.Date == parsedDate.Date)
                        .ToListAsync();

                    if (importsToDelete.Any())
                    {
                        deletedCount = importsToDelete.Count;
                        deletedRecords = importsToDelete.Sum(r => r.RecordsCount);

                        // Xóa từ database
                        _context.ImportedDataRecords.RemoveRange(importsToDelete);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("✅ Đã xóa {Count} imports với {Records} records từ Temporal Tables",
                            deletedCount, deletedRecords);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 Lỗi khi xóa dữ liệu từ Temporal Tables: {Error}", ex.Message);
                    return StatusCode(500, new
                    {
                        message = "Lỗi khi xóa dữ liệu từ database",
                        error = ex.Message
                    });
                }

                try
                {
                    // Try to drop dynamic tables if they exist
                    await DropDynamicTableIfExists(dataType.ToUpper(), parsedDate);
                    _logger.LogInformation("Dropped dynamic tables for {DataType} on {Date}", dataType, parsedDate);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to drop dynamic tables: {Error}", ex.Message);
                }

                // Return success response
                return Ok(new
                {
                    message = deletedCount > 0
                        ? $"✅ Đã xóa {deletedCount} import(s) với {deletedRecords} records cho {dataType} ngày {statementDate}"
                        : $"Không tìm thấy dữ liệu cho {dataType} ngày {statementDate}",
                    deletedImports = deletedCount,
                    deletedRecords = deletedRecords,
                    temporalTablesEnabled = true
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
                _logger.LogInformation("Getting data for type: {DataType}, date: {StatementDate}", dataType, statementDate);

                if (!DateTime.TryParseExact(statementDate, "yyyyMMdd", null, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest(new { message = "Định dạng ngày không hợp lệ. Sử dụng yyyyMMdd" });
                }

                try
                {
                    // � SỬ DỤNG TEMPORAL TABLES THAY VÌ MOCK DATA
                    var imports = await _context.ImportedDataRecords
                        .Where(r => r.Category == dataType && r.StatementDate.HasValue && r.StatementDate.Value.Date == parsedDate.Date)
                        .Select(r => new
                        {
                            r.Id,
                            r.FileName,
                            DataType = r.Category,
                            r.ImportDate,
                            r.StatementDate,
                            r.ImportedBy,
                            r.Status,
                            r.RecordsCount,
                            r.Notes
                        })
                        .ToListAsync();

                    _logger.LogInformation("✅ Tìm thấy {Count} imports từ Temporal Tables cho {DataType} ngày {Date}",
                        imports.Count, dataType, parsedDate);

                    return Ok(imports);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 Lỗi khi truy vấn Temporal Tables: {Error}", ex.Message);
                    return StatusCode(500, new
                    {
                        message = "Lỗi khi truy vấn dữ liệu từ database",
                        error = ex.Message,
                        temporalTablesEnabled = true
                    });
                }
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

                _logger.LogInformation("🔍 GetByDateRange request: dataType={DataType}, fromDate={FromDate}, toDate={ToDate}",
                    dataType, fromDate, toDate);

                // ⚠️ FALLBACK: Trả về empty list vì temporal table chưa đồng bộ schema
                _logger.LogWarning("⚠️ GetByDateRange: Sử dụng fallback empty list - temporal table chưa đồng bộ");

                return Ok(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu theo khoảng ngày");

                // ⚠️ FALLBACK: Trả về empty list thay vì lỗi 500
                _logger.LogWarning("⚠️ GetByDateRange: Exception caught, returning empty list fallback");
                return Ok(new List<object>());
            }
        }

        // 🗄️ GET: api/RawData/table/{dataType} - Lấy dữ liệu thô trực tiếp từ bảng động (Check real data first)
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

                // 🔧 KIỂM TRA XEM CÓ DỮ LIỆU THẬT TRONG DATABASE KHÔNG
                var hasRealData = await _context.ImportedDataRecords
                    .Where(x => x.FileType == dataType.ToUpper())
                    .AnyAsync();

                if (!hasRealData)
                {
                    _logger.LogInformation("❌ Không có dữ liệu thật cho {DataType} - trả về empty result", dataType);
                    return Ok(new
                    {
                        tableName = $"Empty_{dataType.ToUpper()}",
                        dataType = dataType,
                        recordCount = 0,
                        columns = new List<string>(),
                        records = new List<object>(),
                        note = "Không có dữ liệu - đã bị xóa hoặc chưa import"
                    });
                }

                // 🔧 NẾU CÓ STATEMENT DATE, KIỂM TRA CHÍNH XÁC HỌ
                if (!string.IsNullOrEmpty(statementDate))
                {
                    var dateExists = await _context.ImportedDataRecords
                        .Where(x => x.FileType == dataType.ToUpper())
                        .Where(x => x.StatementDate.HasValue &&
                                   x.StatementDate.Value.ToString("yyyy-MM-dd") == statementDate)
                        .AnyAsync();

                    if (!dateExists)
                    {
                        _logger.LogInformation("❌ Không có dữ liệu cho {DataType} ngày {Date}", dataType, statementDate);
                        return Ok(new
                        {
                            tableName = $"Empty_{dataType.ToUpper()}_{statementDate.Replace("-", "")}",
                            dataType = dataType,
                            recordCount = 0,
                            columns = new List<string>(),
                            records = new List<object>(),
                            note = $"Không có dữ liệu cho ngày {statementDate}"
                        });
                    }
                }

                // 🎭 Tạo mock data dựa trên loại dữ liệu
                var (columns, records) = GenerateMockRawTableData(dataType.ToUpper(), statementDate);

                // ✅ Khai báo biến tableName bị thiếu
                var tableName = $"RawData_{dataType.ToUpper()}_{statementDate}";

                _logger.LogInformation("Generated {Count} mock records for table {TableName} (có dữ liệu thật)",
                    records.Count, tableName);

                return Ok(new
                {
                    tableName = tableName,
                    dataType = dataType,
                    recordCount = records.Count,
                    columns = columns,
                    records = records,
                    note = "Mock data dựa trên dữ liệu thật đã import"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting raw data from table for dataType: {DataType}", dataType);
                return StatusCode(500, new
                {
                    message = "Lỗi khi lấy dữ liệu thô từ bảng",
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        // 📊 Helper method to generate mock data for a table
        private (List<string>, List<Dictionary<string, object>>) GenerateMockRawTableData(string dataType, string? statementDate)
        {
            var columns = new List<string>();
            var records = new List<Dictionary<string, object>>();

            int recordCount = new Random().Next(15, 30);

            // Tạo columns theo loại dữ liệu
            switch (dataType)
            {
                case "LN01":
                    columns = new List<string> { "MaKhachHang", "TenKhachHang", "SoTaiKhoan", "SoTien", "LaiSuat", "NgayGiaiNgan", "HanMuc", "TrangThai" };
                    break;
                case "DP01":
                    columns = new List<string> { "MaKhachHang", "TenKhachHang", "SoTaiKhoan", "SoTien", "LaiSuat", "KyHan", "NgayMoSo", "NgayDaoHan" };
                    break;
                case "GL01":
                    columns = new List<string> { "MaTaiKhoan", "TenTaiKhoan", "SoButToan", "NoiDung", "SoTienNo", "SoTienCo", "NgayHachToan", "NguoiTao" };
                    break;
                case "DPDA":
                    columns = new List<string> { "MaThe", "TenChuThe", "LoaiThe", "HanMuc", "SoDu", "NgayMoThe", "NgayHetHan", "TrangThai" };
                    break;
                default:
                    columns = new List<string> { "MaDuLieu", "LoaiDuLieu", "NoiDung", "GiaTri", "NgayTao", "NguoiTao", "GhiChu" };
                    break;
            }

            // Thêm cột metadata
            columns.Add("ImportId");
            columns.Add("BranchCode");
            columns.Add("StatementDate");
            columns.Add("ImportedBy");
            columns.Add("ImportDate");

            // Tạo records
            DateTime parsedDate = VietnamDateTime.Now;
            if (statementDate != null)
            {
                if (DateTime.TryParse(statementDate, out var date))
                {
                    parsedDate = date;
                }
            }

            for (int i = 1; i <= recordCount; i++)
            {
                var record = new Dictionary<string, object>();

                // Điền dữ liệu theo từng cột
                foreach (var column in columns)
                {
                    switch (column)
                    {
                        case "ImportId":
                            record[column] = 0;
                            break;
                        case "BranchCode":
                            record[column] = $"78{(i % 8).ToString().PadLeft(2, '0')}";
                            break;
                        case "StatementDate":
                            record[column] = parsedDate.ToString("yyyy-MM-dd");
                            break;
                        case "ImportedBy":
                            record[column] = "System";
                            break;
                        case "ImportDate":
                            record[column] = VietnamDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case "MaKhachHang":
                        case "MaDuLieu":
                            record[column] = $"ID{i.ToString().PadLeft(5, '0')}";
                            break;
                        case "TenKhachHang":
                        case "TenChuThe":
                            record[column] = $"Khách hàng mẫu {i}";
                            break;
                        case "SoTaiKhoan":
                        case "MaThe":
                            record[column] = $"{dataType}{i.ToString().PadLeft(8, '0')}";
                            break;
                        case "SoTien":
                        case "HanMuc":
                        case "SoDu":
                        case "GiaTri":
                            record[column] = 1000000 + i * 10000;
                            break;
                        case "LaiSuat":
                            record[column] = Math.Round(3.0 + (i % 10) / 10.0, 2);
                            break;
                        case "NgayGiaiNgan":
                        case "NgayMoSo":
                        case "NgayHachToan":
                        case "NgayTao":
                        case "NgayMoThe":
                            record[column] = parsedDate.AddDays(-i).ToString("yyyy-MM-dd");
                            break;
                        case "NgayDaoHan":
                        case "NgayHetHan":
                            record[column] = parsedDate.AddMonths(6).ToString("yyyy-MM-dd");
                            break;
                        case "TrangThai":
                            record[column] = new string[] { "Hoạt động", "Tạm khóa", "Đóng" }[i % 3];
                            break;
                        case "KyHan":
                            record[column] = new string[] { "1 tháng", "3 tháng", "6 tháng", "12 tháng" }[i % 4];
                            break;
                        case "MaTaiKhoan":
                            record[column] = $"TK{1000 + i}";
                            break;
                        case "TenTaiKhoan":
                            record[column] = $"Tài khoản {1000 + i}";
                            break;
                        case "SoButToan":
                            record[column] = $"BT{10000 + i}";
                            break;
                        case "NoiDung":
                        case "GhiChu":
                            record[column] = $"Ghi chú mẫu cho bản ghi {i}";
                            break;
                        case "SoTienNo":
                            record[column] = i % 2 == 0 ? 500000 + i * 5000 : 0;
                            break;
                        case "SoTienCo":
                            record[column] = i % 2 == 1 ? 500000 + i * 5000 : 0;
                            break;
                        case "NguoiTao":
                            record[column] = "System";
                            break;
                        case "LoaiThe":
                        case "LoaiDuLieu":
                            record[column] = dataType;
                            break;
                        default:
                            record[column] = $"Giá trị {column} cho bản ghi {i}";
                            break;
                    }
                }

                records.Add(record);
            }

            return (columns, records);
        }

        // 👤 Helper method để import dữ liệu từ file CSV
        private async Task<RawDataImportResult> ImportCsvToDatabase(string filePath, string fileName, string dataType, string notes)
        {
            try
            {
                // Đọc dữ liệu từ file
                var fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                // 🚨 FIX: Split chính xác và loại bỏ dòng trống không cần thiết ở đầu/cuối
                var lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // 🚨 FIX: Loại bỏ dòng trống cuối file nếu có
                var validLines = lines.Where((line, index) =>
                    index == 0 || // Giữ header
                    !string.IsNullOrEmpty(line) || // Giữ dòng có dữ liệu
                    index < lines.Length - 1 // Loại bỏ dòng trống cuối cùng
                ).ToArray();

                if (validLines.Length <= 1)
                {
                    return new RawDataImportResult
                    {
                        Success = false,
                        FileName = fileName,
                        Message = "File không có dữ liệu"
                    };
                }

                // Phân tích header
                var headers = validLines[0].Split(',').Select(h => h.Trim('"').Trim()).ToList();

                // Trích xuất ngày từ tên file
                var statementDate = ExtractStatementDate(fileName);
                if (statementDate == null)
                {
                    return new RawDataImportResult
                    {
                        Success = false,
                        FileName = fileName,
                        Message = "Không tìm thấy ngày trong tên file"
                    };
                }

                // Trích xuất mã chi nhánh từ tên file
                var branchCode = ExtractBranchCode(fileName) ?? "7800";

                // 🚨 FIX CRITICAL: Tạo records từ TỪNG DÒNG dữ liệu (không bỏ qua dòng nào)
                var records = new List<RawDataRecord>();
                for (int i = 1; i < validLines.Length; i++)
                {
                    var line = validLines[i].Trim();

                    // 🚨 FIX: Xử lý MỌI dòng, kể cả dòng trống để đảm bảo số lượng CHÍNH XÁC
                    List<string> values;
                    if (string.IsNullOrEmpty(line))
                    {
                        // Tạo dòng rỗng với số cột đúng
                        values = new List<string>();
                        for (int k = 0; k < headers.Count; k++)
                        {
                            values.Add("");
                        }
                    }
                    else
                    {
                        values = line.Split(',').Select(v => v.Trim('"').Trim()).ToList();
                    }

                    var data = new Dictionary<string, object>();

                    for (int j = 0; j < Math.Min(headers.Count, values.Count); j++)
                    {
                        data[headers[j]] = values[j];
                    }

                    // Thêm thông tin metadata
                    data["BranchCode"] = branchCode;
                    data["StatementDate"] = statementDate.Value.ToString("yyyy-MM-dd");
                    data["ImportDate"] = VietnamDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    data["ImportedBy"] = "System";
                    data["RowNumber"] = i; // 🚨 THÊM: Số thứ tự dòng để tracking

                    // Format data
                    var formattedData = FormatDataValues(data);

                    records.Add(new RawDataRecord
                    {
                        JsonData = System.Text.Json.JsonSerializer.Serialize(formattedData),
                        ProcessedDate = VietnamDateTime.Now
                    });
                }

                // Tạo import record
                var importedDataRecord = new ImportedDataRecord
                {
                    FileName = fileName,
                    FileType = dataType,
                    Category = dataType,
                    ImportDate = VietnamDateTime.Now,
                    StatementDate = statementDate.Value,
                    ImportedBy = "System",
                    Status = "Completed",
                    RecordsCount = records.Count,
                    Notes = $"{notes} - Branch: {branchCode}"
                };

                // Lưu vào database
                _context.ImportedDataRecords.Add(importedDataRecord);
                await _context.SaveChangesAsync();

                // Lưu items với SQL trực tiếp
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    foreach (var record in records)
                    {
                        var sql = @"INSERT INTO ImportedDataItems
                                    (ImportedDataRecordId, RawData, ProcessedDate, ProcessingNotes)
                                    VALUES (@ImportedDataRecordId, @RawData, @ProcessedDate, @ProcessingNotes)";

                        await connection.ExecuteAsync(sql, new
                        {
                            ImportedDataRecordId = importedDataRecord.Id,
                            RawData = record.JsonData,
                            ProcessedDate = VietnamDateTime.Now,
                            ProcessingNotes = $"Processed successfully - Branch: {branchCode}"
                        });
                    }
                }

                // Tạo bảng động - chuyển đổi sang Dictionary format
                var recordDicts = records.Select(r => new Dictionary<string, object>
                {
                    ["Id"] = r.Id,
                    ["JsonData"] = r.JsonData, // ✅ Sửa từ RawData thành JsonData
                    ["ProcessedDate"] = r.ProcessedDate,
                    ["ProcessingNotes"] = r.ProcessingNotes ?? "" // ✅ Xử lý null
                }).ToList();

                var tableName = CreateDynamicTable(dataType, statementDate.Value, branchCode, recordDicts);

                // 🚨 LOG CRITICAL INFO để debug
                _logger.LogInformation("🚨 IMPORT SUMMARY - File: {FileName}" +
                    "\n📁 Original file lines: {OriginalLines}" +
                    "\n📋 Valid lines after cleanup: {ValidLines}" +
                    "\n📊 Data lines (excluding header): {DataLines}" +
                    "\n✅ Records processed: {RecordsProcessed}" +
                    "\n🎯 Expected count: {ExpectedCount} (should match file records)",
                    fileName, lines.Length, validLines.Length, validLines.Length - 1, records.Count, validLines.Length - 1);

                return new RawDataImportResult
                {
                    Success = true,
                    FileName = fileName,
                    RecordsProcessed = records.Count,
                    Message = $"✅ Đã import {records.Count} bản ghi thành công (File gốc: {validLines.Length - 1} dòng dữ liệu) - Branch: {branchCode}",
                    StatementDate = statementDate,
                    TableName = tableName,
                    DataType = dataType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import CSV file {FileName}", fileName);
                return new RawDataImportResult
                {
                    Success = false,
                    FileName = fileName,
                    Message = $"Lỗi khi import: {ex.Message}"
                };
            }
        }

        //  Xóa bảng động nếu tồn tại
        private async Task DropDynamicTableIfExists(string dataType, DateTime statementDate)
        {
            var tableName = $"Data_{dataType}";
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var tableExists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName",
                    new { TableName = tableName });

                if (tableExists > 0)
                {
                    // Xóa dữ liệu cho ngày cụ thể thay vì xóa bảng
                    await connection.ExecuteAsync(
                        $"DELETE FROM [{tableName}] WHERE StatementDate = @StatementDate",
                        new { StatementDate = statementDate });

                    _logger.LogInformation($"Đã xóa dữ liệu cho ngày {statementDate:yyyy-MM-dd} từ bảng {tableName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa dữ liệu từ bảng {tableName} cho ngày {statementDate:yyyy-MM-dd}");
                throw;
            }
        }

        // 📊 Format các giá trị trong dữ liệu
        private Dictionary<string, object> FormatDataValues(Dictionary<string, object> data)
        {
            var formattedData = new Dictionary<string, object>();

            foreach (var pair in data)
            {
                var key = pair.Key;
                var value = pair.Value;

                if (value is string strValue)
                {
                    // Thử chuyển đổi ngày tháng
                    if (DateTime.TryParse(strValue, out var dateValue))
                    {
                        formattedData[key] = dateValue.ToString("yyyy-MM-dd");
                        continue;
                    }

                    // Thử chuyển đổi số
                    if (decimal.TryParse(strValue, out var decimalValue))
                    {
                        formattedData[key] = decimalValue;
                        continue;
                    }
                }

                // Giữ nguyên giá trị
                formattedData[key] = value;
            }

            return formattedData;
        }

        // 📊 Parse dữ liệu JSON
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

        // 🔧 CHUẨN HÓA: Extract thông tin từ filename theo format MaCN_LoaiFile_Ngay.ext
        // Format: 7800_LN01_20241231.csv hoặc 7801_DP01_20241130.xlsx
        private string? ExtractBranchCode(string fileName)
        {
            try
            {
                _logger.LogInformation("🔍 Extracting branch code from filename: {FileName}", fileName);

                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext (7800_LN01_20241231.csv)
                var standardMatch = Regex.Match(fileName, @"^(78\d{2})_[A-Z0-9_]+_\d{8}\.(csv|xlsx?)", RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    var branchCode = standardMatch.Groups[1].Value;
                    _logger.LogInformation("✅ Standard format - Branch code: {BranchCode}", branchCode);
                    return branchCode;
                }

                // Strategy 2: Fallback - tìm mã chi nhánh bất kỳ đâu trong filename
                var fallbackMatch = Regex.Match(fileName, @"(78\d{2})");
                if (fallbackMatch.Success)
                {
                    var branchCode = fallbackMatch.Groups[1].Value;
                    _logger.LogWarning("⚠️ Non-standard format but found branch code: {BranchCode}", branchCode);
                    return branchCode;
                }

                _logger.LogWarning("❌ Không tìm thấy mã chi nhánh trong: {FileName}, sử dụng default 7800", fileName);
                return "7800";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi extract branch code từ: {FileName}", fileName);
                return "7800";
            }
        }

        // 🔧 CHUẨN HÓA: Extract loại dữ liệu từ filename
        private string? ExtractDataTypeFromFilename(string fileName)
        {
            try
            {
                _logger.LogInformation("🔍 Extracting data type from filename: {FileName}", fileName);

                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext
                var standardMatch = Regex.Match(fileName, @"^78\d{2}_([A-Z0-9_]+)_\d{8}\.(csv|xlsx?)", RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    var dataType = standardMatch.Groups[1].Value.ToUpper();
                    _logger.LogInformation("✅ Standard format - Data type: {DataType}", dataType);
                    return dataType;
                }

                // Strategy 2: Fallback - tìm trong các loại đã định nghĩa
                var definedTypes = DataTypeDefinitions.Keys.ToArray();
                foreach (var type in definedTypes)
                {
                    if (fileName.Contains(type, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("⚠️ Non-standard format but found data type: {DataType}", type);
                        return type;
                    }
                }

                _logger.LogWarning("❌ Không tìm thấy loại dữ liệu trong: {FileName}", fileName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi extract data type từ: {FileName}", fileName);
                return null;
            }
        }

        // 🔧 CHUẨN HÓA: Extract ngày từ filename theo format yyyyMMdd
        private DateTime? ExtractStatementDate(string fileName)
        {
            try
            {
                _logger.LogInformation("🔍 Extracting statement date from filename: {FileName}", fileName);

                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext (20241231)
                var standardMatch = Regex.Match(fileName, @"^78\d{2}_[A-Z0-9_]+_(\d{8})\.(csv|xlsx?)", RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    var dateStr = standardMatch.Groups[1].Value;
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out DateTime date))
                    {
                        _logger.LogInformation("✅ Standard format - Statement date: {Date}", date.ToString("yyyy-MM-dd"));
                        return date;
                    }
                }

                // Strategy 2: Fallback - tìm pattern yyyyMMdd bất kỳ đâu
                var fallbackMatch = Regex.Match(fileName, @"(\d{8})");
                if (fallbackMatch.Success)
                {
                    var dateStr = fallbackMatch.Groups[1].Value;
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out DateTime date))
                    {
                        _logger.LogWarning("⚠️ Non-standard format but found date: {Date}", date.ToString("yyyy-MM-dd"));
                        return date;
                    }
                }

                _logger.LogWarning("❌ Không tìm thấy ngày hợp lệ trong: {FileName}, sử dụng ngày hiện tại", fileName);
                return VietnamDateTime.Now.Date;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi extract statement date từ: {FileName}", fileName);
                return VietnamDateTime.Now.Date;
            }
        }

        // ✅ Thêm method CreateDynamicTable bị thiếu (sửa thành sync)
        private string CreateDynamicTable(string dataType, DateTime statementDate, string branchCode, List<Dictionary<string, object>> records)
        {
            try
            {
                var tableName = $"RawData_{dataType.ToUpper()}_{branchCode}_{statementDate:yyyyMMdd}";
                _logger.LogInformation("🗃️ Tạo bảng động: {TableName} với {RecordCount} records", tableName, records.Count);

                // TODO: Implement actual dynamic table creation with Temporal Tables + Columnstore Indexes
                // Hiện tại return mock table name
                return tableName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi tạo bảng động cho {DataType}", dataType);
                return $"Mock_{dataType}_{statementDate:yyyyMMdd}";
            }
        }

        // ✅ Thêm method ProcessSingleFile bị thiếu - SỬA ĐỂ LƯU VÀO DATABASE
        private async Task<RawDataImportResult> ProcessSingleFile(IFormFile file, string dataType, string notes)
        {
            var importedDataRecord = new ImportedDataRecord(); // Declare outside try block

            try
            {
                _logger.LogInformation("📁 Bắt đầu xử lý file: {FileName} cho loại {DataType}, Size: {FileSize} bytes",
                    file.FileName, dataType, file.Length);

                // Trích xuất ngày sao kê từ tên file
                var statementDate = ExtractStatementDate(file.FileName) ?? VietnamDateTime.Now.Date;
                var branchCode = ExtractBranchCode(file.FileName) ?? "7800";

                _logger.LogInformation("� File info: StatementDate={StatementDate}, BranchCode={BranchCode}",
                    statementDate.ToString("yyyy-MM-dd"), branchCode);

                // 💾 Tạo ImportedDataRecord với status "Processing"
                importedDataRecord = new ImportedDataRecord
                {
                    FileName = file.FileName,
                    FileType = dataType,
                    Category = dataType,
                    ImportDate = VietnamDateTime.Now,
                    StatementDate = statementDate,
                    ImportedBy = "System",
                    Status = "Processing", // 🔥 Bắt đầu với "Processing"
                    RecordsCount = 0,
                    Notes = $"{notes} - Branch: {branchCode}"
                };

                _context.ImportedDataRecords.Add(importedDataRecord);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Tạo ImportedDataRecord ID={Id} với status Processing", importedDataRecord.Id);

                int totalProcessed = 0;
                const int batchSize = 1000;

                // 🔤 XỬ LÝ THEO ĐỊNH DẠNG FILE
                bool isExcelFile = file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                                   file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("📊 File type: {FileType}", isExcelFile ? "Excel" : "CSV");

                if (isExcelFile)
                {
                    _logger.LogInformation("📊 Bắt đầu xử lý Excel file...");
                    totalProcessed = await ProcessExcelFileForEncoding(file, dataType, importedDataRecord.Id,
                        statementDate, branchCode, batchSize);
                    _logger.LogInformation("📊 Excel processing completed: {Records} records", totalProcessed);
                }
                else
                {
                    _logger.LogInformation("📄 Bắt đầu xử lý CSV file...");
                    var encoding = DetectCsvFileEncoding(file);
                    _logger.LogInformation("🔤 Detected encoding: {Encoding}", encoding.EncodingName);

                    using var reader = new StreamReader(file.OpenReadStream(), encoding, detectEncodingFromByteOrderMarks: true);
                    totalProcessed = await ProcessCsvFileContent(reader, dataType, importedDataRecord.Id,
                        statementDate, branchCode, batchSize);
                    _logger.LogInformation("📄 CSV processing completed: {Records} records", totalProcessed);
                }

                // ✅ KIỂM TRA VÀ CẬP NHẬT STATUS DỰA TRÊN KẾT QUẢ
                if (totalProcessed > 0)
                {
                    importedDataRecord.Status = "Completed";
                    importedDataRecord.RecordsCount = totalProcessed;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("✅ Hoàn thành xử lý file {FileName}: {Total} records",
                        file.FileName, totalProcessed);

                    // 🔥 AUTO-PROCESS CHỈ KHI CÓ DỮ LIỆU
                    await AutoProcessAfterImport(importedDataRecord.Id, dataType, statementDate);

                    return new RawDataImportResult
                    {
                        Success = true,
                        Message = $"Đã import thành công {totalProcessed} records vào database",
                        FileName = file.FileName,
                        RecordsProcessed = totalProcessed,
                        DataType = dataType,
                        StatementDate = statementDate
                    };
                }
                else
                {
                    importedDataRecord.Status = "Failed";
                    importedDataRecord.RecordsCount = 0;
                    importedDataRecord.Notes = $"{importedDataRecord.Notes} | Import failed: No data found";
                    await _context.SaveChangesAsync();

                    _logger.LogWarning("⚠️ File {FileName} không chứa dữ liệu hợp lệ", file.FileName);

                    return new RawDataImportResult
                    {
                        Success = false,
                        Message = $"File {file.FileName} không chứa dữ liệu hợp lệ",
                        FileName = file.FileName,
                        RecordsProcessed = 0,
                        DataType = dataType,
                        StatementDate = statementDate
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi xử lý file {FileName}: {Error}", file.FileName, ex.Message);

                // 🔥 Cập nhật status thành "Failed" nếu có lỗi
                try
                {
                    if (importedDataRecord.Id > 0) // Đã được tạo trong database
                    {
                        importedDataRecord.Status = "Failed";
                        importedDataRecord.Notes = $"{importedDataRecord.Notes} | Error: {ex.Message}";
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("📝 Updated status to Failed for ImportedDataRecord ID={Id}", importedDataRecord.Id);
                    }
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "❌ Không thể cập nhật status Failed cho file {FileName}", file.FileName);
                }

                return new RawDataImportResult
                {
                    Success = false,
                    Message = $"Lỗi xử lý file: {ex.Message}",
                    FileName = file.FileName,
                    RecordsProcessed = 0,
                    DataType = dataType
                };
            }
        }

        // ⚡ Helper method để lưu batch vào database
        private async Task SaveBatchToDatabase(List<Dictionary<string, object>> records, int importedDataRecordId, string branchCode)
        {
            foreach (var record in records)
            {
                var item = new ImportedDataItem
                {
                    ImportedDataRecordId = importedDataRecordId,
                    RawData = System.Text.Json.JsonSerializer.Serialize(record),
                    ProcessedDate = VietnamDateTime.Now,
                    ProcessingNotes = $"Batch processed - Branch: {branchCode}"
                };
                _context.ImportedDataItems.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        // ✅ API mới: Lấy danh sách import gần đây nhất (để hiển thị ngay sau khi upload)
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecentImportsFixed([FromQuery] int limit = 20)
        {
            try
            {
                _logger.LogInformation("🔍 Lấy {Limit} import gần đây nhất", limit);

                // Simplest possible approach - no projections at all
                var rawDataList = await _context.ImportedDataRecords
                    .OrderByDescending(x => x.ImportDate)
                    .Take(limit)
                    .ToListAsync();

                // Build the response in memory
                var result = new List<object>();
                foreach (var item in rawDataList)
                {
                    // Extract branch code from notes
                    string branchCode = "7800"; // Default
                    if (!string.IsNullOrEmpty(item.Notes))
                    {
                        var match = Regex.Match(item.Notes, @"Branch: (78\d\d)");
                        if (match.Success)
                        {
                            branchCode = match.Groups[1].Value;
                        }
                    }

                    result.Add(new
                    {
                        item.Id,
                        item.FileName,
                        DataType = item.Category ?? item.FileType,
                        Category = item.Category ?? item.FileType,
                        FileType = item.FileType,
                        item.ImportDate,
                        item.StatementDate,
                        item.ImportedBy,
                        item.Status,
                        item.RecordsCount,
                        item.Notes,
                        BranchCode = branchCode,
                        RecordsPreview = new List<object>()
                    });
                }

                _logger.LogInformation("✅ Trả về {Count} import gần đây nhất", result.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi khi lấy danh sách import gần đây nhất");
                return StatusCode(500, new { message = "Lỗi server khi lấy dữ liệu gần đây nhất", error = ex.Message });
            }
        }

        // ✅ Simple test endpoint with no database access
        [HttpGet("test-simple")]
        public ActionResult<object> GetSimpleTest()
        {
            try
            {
                _logger.LogInformation("🔍 Simple test endpoint");

                var result = new
                {
                    message = "Simple test successful",
                    timestamp = VietnamDateTime.Now,
                    version = "1.0.0"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error in simple test endpoint");
                return StatusCode(500, new { message = "Error in simple test", error = ex.Message });
            }
        }

        // ✅ Brand new API: Use raw SQL to completely bypass Entity Framework type mapping issues
        [HttpGet("imports/latest")]
        public async Task<ActionResult<IEnumerable<object>>> GetLatestImports([FromQuery] int limit = 20)
        {
            try
            {
                _logger.LogInformation("🔍 Raw SQL implementation - getting {Limit} latest imports", limit);

                // Use raw SQL to bypass Entity Framework type mapping completely
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var sql = @"
                    SELECT TOP(@Limit)
                        Id,
                        ImportDate,
                        FileName,
                        FileType,
                        Category,
                        ImportedBy,
                        RecordsCount,
                        Notes
                    FROM ImportedDataRecords
                    ORDER BY ImportDate DESC";

                var rawResults = await connection.QueryAsync(sql, new { Limit = limit });

                // Build response manually from raw data
                var result = rawResults.Select(x => new
                {
                    id = (int)x.Id,
                    importDate = (DateTime)x.ImportDate,
                    fileName = (string)(x.FileName ?? ""),
                    fileType = (string)(x.FileType ?? ""),
                    category = (string)(x.Category ?? ""),
                    importedBy = (string)(x.ImportedBy ?? ""),
                    recordCount = (int)x.RecordsCount,
                    note = (string)(x.Notes ?? ""),
                    branchCode = "N/A"
                }).ToList();

                _logger.LogInformation("✅ Successfully retrieved {Count} imports using raw SQL", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error in raw SQL GetLatestImports implementation");
                return StatusCode(500, new { message = "Error getting latest imports", error = ex.Message });
            }
        }

        // 📊 GET: api/RawData/{id}/processed - Lấy dữ liệu đã xử lý từ bảng History
        [HttpGet("{id:int}/processed")]
        public async Task<ActionResult<object>> GetProcessedDataByImportId(int id)
        {
            try
            {
                _logger.LogInformation("🔍 Getting processed data for import ID: {Id}", id);

                // Get import record to determine data type
                var import = await _context.ImportedDataRecords
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (import == null)
                {
                    _logger.LogWarning("❌ Import record {ImportId} not found", id);
                    return NotFound(new { message = $"Không tìm thấy bản ghi import với ID {id}" });
                }

                var dataType = import.Category?.ToUpper() ?? import.FileType?.ToUpper();
                _logger.LogInformation("📊 Looking for processed data of type: {DataType} for import: {ImportId}", dataType, id);

                var response = new
                {
                    importInfo = new
                    {
                        import.Id,
                        import.FileName,
                        DataType = dataType,
                        import.ImportDate,
                        import.StatementDate,
                        import.RecordsCount,
                        import.Status,
                        import.ImportedBy
                    },
                    processedData = new List<object>(),
                    totalRecords = 0,
                    tableName = "",
                    dataSource = "PROCESSED_HISTORY_TABLE"
                };

                // Get processed data based on data type
                switch (dataType)
                {
                    case "BC57":
                        var bc57Data = await _context.BC57History
                            .Where(h => import.StatementDate.HasValue && h.StatementDate.Date == import.StatementDate.Value.Date)
                            .OrderByDescending(h => h.ProcessedDate)
                            .Take(100) // Limit to 100 records for performance
                            .Select(h => new
                            {
                                h.Id,
                                h.MaKhachHang,
                                h.TenKhachHang,
                                h.SoTaiKhoan,
                                h.MaHopDong,
                                h.LoaiSanPham,
                                h.SoTienGoc,
                                h.LaiSuat,
                                h.SoNgayTinhLai,
                                h.TienLaiDuThu,
                                h.TienLaiQuaHan,
                                h.NgayBatDau,
                                h.NgayKetThuc,
                                h.TrangThai,
                                h.MaChiNhanh,
                                h.TenChiNhanh,
                                h.NgayTinhLai,
                                h.StatementDate,
                                h.ProcessedDate,
                                h.ImportId
                            })
                            .ToListAsync();

                        return Ok(new
                        {
                            response.importInfo,
                            processedData = bc57Data.Cast<object>().ToList(),
                            totalRecords = bc57Data.Count,
                            tableName = "BC57History",
                            dataSource = "PROCESSED_HISTORY_TABLE"
                        });

                    case "DPDA":
                        var dpdaData = await _context.DPDAHistory
                            .Where(h => import.StatementDate.HasValue && h.StatementDate.Date == import.StatementDate.Value.Date)
                            .OrderByDescending(h => h.ProcessedDate)
                            .Take(100)
                            .Select(h => new
                            {
                                h.Id,
                                h.MaKhachHang,
                                h.TenKhachHang,
                                h.SoThe,
                                h.LoaiThe,
                                HanMuc = h.HanMucThe,
                                SoDu = h.SoDuHienTai,
                                NgayMoThe = h.NgayPhatHanh,
                                h.NgayHetHan,
                                TrangThai = h.TrangThaiThe,
                                h.StatementDate,
                                h.ProcessedDate,
                                h.ImportId
                            })
                            .ToListAsync();

                        return Ok(new
                        {
                            response.importInfo,
                            processedData = dpdaData.Cast<object>().ToList(),
                            totalRecords = dpdaData.Count,
                            tableName = "DPDAHistory",
                            dataSource = "PROCESSED_HISTORY_TABLE"
                        });

                    case "LN01":
                        var ln01Data = await _context.LN01_History
                            .Where(h => import.StatementDate.HasValue && h.StatementDate.Date == import.StatementDate.Value.Date)
                            .OrderByDescending(h => h.ProcessedDate)
                            .Take(100)
                            .Select(h => new
                            {
                                h.Id,
                                h.BRCD,
                                h.CUSTSEQ,
                                h.CUSTNM,
                                h.TAI_KHOAN,
                                h.CCY,
                                h.DU_NO,
                                h.DSBSSEQ,
                                h.TRANSACTION_DATE,
                                h.DSBSDT,
                                h.DISBUR_CCY,
                                h.DISBURSEMENT_AMOUNT,
                                h.StatementDate,
                                h.ProcessedDate,
                                h.ImportId
                            })
                            .ToListAsync();

                        return Ok(new
                        {
                            response.importInfo,
                            processedData = ln01Data.Cast<object>().ToList(),
                            totalRecords = ln01Data.Count,
                            tableName = "LN01_History",
                            dataSource = "PROCESSED_HISTORY_TABLE"
                        });

                    case "7800_DT_KHKD1":
                        var dtKhkd1Data = await _context.DT_KHKD1_History
                            .Where(h => import.StatementDate.HasValue && h.StatementDate.Date == import.StatementDate.Value.Date)
                            .OrderByDescending(h => h.ProcessedDate)
                            .Take(100)
                            .Select(h => new
                            {
                                h.Id,
                                h.BRCD,
                                h.BRANCH_NAME,
                                h.INDICATOR_TYPE,
                                h.INDICATOR_NAME,
                                h.PLAN_YEAR,
                                h.PLAN_QUARTER,
                                h.PLAN_MONTH,
                                h.ACTUAL_YEAR,
                                h.ACTUAL_QUARTER,
                                h.ACTUAL_MONTH,
                                h.ACHIEVEMENT_RATE,
                                h.YEAR,
                                h.QUARTER,
                                h.MONTH,
                                h.CREATED_DATE,
                                h.UPDATED_DATE,
                                h.StatementDate,
                                h.ProcessedDate,
                                h.ImportId
                            })
                            .ToListAsync();

                        return Ok(new
                        {
                            response.importInfo,
                            processedData = dtKhkd1Data.Cast<object>().ToList(),
                            totalRecords = dtKhkd1Data.Count,
                            tableName = "DT_KHKD1_History",
                            dataSource = "PROCESSED_HISTORY_TABLE"
                        });

                    case "GLCB41":
                        var glcb41Data = await _context.GLCB41_History
                            .Where(h => import.StatementDate.HasValue && h.StatementDate.Date == import.StatementDate.Value.Date)
                            .OrderByDescending(h => h.ProcessedDate)
                            .Take(100)
                            .Select(h => new
                            {
                                h.Id,
                                h.JOURNAL_NO,
                                h.ACCOUNT_NO,
                                h.ACCOUNT_NAME,
                                h.CUSTOMER_ID,
                                h.CUSTOMER_NAME,
                                h.TRANSACTION_DATE,
                                h.POSTING_DATE,
                                h.DESCRIPTION,
                                h.DEBIT_AMOUNT,
                                h.CREDIT_AMOUNT,
                                h.DEBIT_BALANCE,
                                h.CREDIT_BALANCE,
                                h.BRCD,
                                h.BRANCH_NAME,
                                h.TRANSACTION_TYPE,
                                h.ORIGINAL_TRANS_ID,
                                h.CREATED_DATE,
                                h.UPDATED_DATE,
                                h.StatementDate,
                                h.ProcessedDate,
                                h.ImportId
                            })
                            .ToListAsync();

                        return Ok(new
                        {
                            response.importInfo,
                            processedData = glcb41Data.Cast<object>().ToList(),
                            totalRecords = glcb41Data.Count,
                            tableName = "GLCB41_History",
                            dataSource = "PROCESSED_HISTORY_TABLE"
                        });

                    default:
                        _logger.LogWarning("⚠️ No processed data handler for data type: {DataType}", dataType);
                        return Ok(new
                        {
                            response.importInfo,
                            processedData = new List<object>(),
                            totalRecords = 0,
                            tableName = $"{dataType}_History",
                            dataSource = "NO_HANDLER",
                            message = $"Chưa có handler cho loại dữ liệu {dataType}"
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error getting processed data for import ID: {Id}", id);
                return StatusCode(500, new
                {
                    message = "Lỗi khi lấy dữ liệu đã xử lý",
                    error = ex.Message,
                    importId = id
                });
            }
        }

        // Helper method to process special headers for 7800_DT_KHKD1
        private List<string> ProcessSpecialHeader(List<string> headerLines)
        {
            try
            {
                if (headerLines.Count != 3)
                {
                    _logger.LogWarning("⚠️ Expected 3 header lines, got {Count}", headerLines.Count);
                    return new List<string> { "Column1", "Column2", "Column3" }; // Fallback
                }

                // Clean and merge headers from the 3 lines
                var cleanHeaders = new List<string>();
                var maxColumns = headerLines.Max(line => line.Split(',').Length);

                for (int i = 0; i < maxColumns; i++)
                {
                    var columnParts = new List<string>();

                    foreach (var headerLine in headerLines)
                    {
                        var parts = headerLine.Split(',');
                        if (i < parts.Length)
                        {
                            var part = parts[i].Trim('"').Trim();
                            if (!string.IsNullOrWhiteSpace(part))
                            {
                                columnParts.Add(part);
                            }
                        }
                    }

                    var columnName = string.Join("_", columnParts.Where(p => !string.IsNullOrWhiteSpace(p)));
                    if (string.IsNullOrWhiteSpace(columnName))
                    {
                        columnName = $"Column{i + 1}";
                    }

                    // Sanitize column name to remove special characters
                    columnName = System.Text.RegularExpressions.Regex.Replace(columnName, @"[^\w\-_]", "_");
                    cleanHeaders.Add(columnName);
                }

                _logger.LogInformation("✅ Processed special headers: {Headers}", string.Join(", ", cleanHeaders.Take(5)));
                return cleanHeaders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing special headers");
                return new List<string> { "Column1", "Column2", "Column3" }; // Fallback
            }
        }

        // 🔤 Helper method để phát hiện encoding của file CSV
        private System.Text.Encoding DetectCsvFileEncoding(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new BinaryReader(stream);

                var bom = reader.ReadBytes(4);
                stream.Position = 0;

                // Kiểm tra BOM để phát hiện encoding
                if (bom.Length >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                {
                    return System.Text.Encoding.UTF8;
                }
                if (bom.Length >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                if (bom.Length >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }

                // Nếu không có BOM, thử phát hiện bằng cách đọc vài byte đầu
                var firstBytes = reader.ReadBytes(1024);
                stream.Position = 0;

                // Kiểm tra có ký tự không hợp lệ trong UTF-8 không
                try
                {
                    System.Text.Encoding.UTF8.GetString(firstBytes);
                    return System.Text.Encoding.UTF8;
                }
                catch
                {
                    // Thử Windows-1252 (Latin-1 với ký tự mở rộng) cho tiếng Việt
                    return System.Text.Encoding.GetEncoding("Windows-1252");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "❌ Cannot detect encoding for {FileName}, using UTF-8", file.FileName);
                return System.Text.Encoding.UTF8;
            }
        }

        // 📊 Helper method để xử lý file Excel với encoding đúng - HỖ TRỢ CẢ .XLS VÀ .XLSX
        private async Task<int> ProcessExcelFileForEncoding(IFormFile file, string dataType,
            int importedDataRecordId, DateTime statementDate, string branchCode, int batchSize)
        {
            try
            {
                _logger.LogInformation("📊 Processing Excel file: {FileName} for dataType: {DataType}", file.FileName, dataType);
                _logger.LogInformation("📊 File size: {Size} bytes, Content type: {ContentType}", file.Length, file.ContentType);

                int totalProcessed = 0;
                var records = new List<Dictionary<string, object>>();

                using var stream = file.OpenReadStream();

                // 🔍 Detect file type: .xls (legacy) hoặc .xlsx (modern)
                bool isLegacyExcel = _legacyExcelReaderService.CanReadFile(file.FileName);

                if (isLegacyExcel)
                {
                    _logger.LogInformation("📊 Using NPOI for legacy .xls file: {FileName}", file.FileName);

                    // Sử dụng NPOI để đọc file .xls
                    var excelResult = await _legacyExcelReaderService.ReadExcelFileAsync(stream, file.FileName);

                    if (!excelResult.Success)
                    {
                        throw new InvalidOperationException($"Cannot read .xls file: {excelResult.Message}");
                    }

                    // Process từng row data từ NPOI
                    foreach (var dataRow in excelResult.Data)
                    {
                        var record = new Dictionary<string, object>(dataRow);

                        // Thêm metadata
                        record["BranchCode"] = branchCode;
                        record["StatementDate"] = statementDate.ToString("yyyy-MM-dd");
                        record["ImportDate"] = VietnamDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        record["ImportedBy"] = "System";

                        records.Add(record);

                        // Batch processing
                        if (records.Count >= batchSize)
                        {
                            await SaveBatchToDatabase(records, importedDataRecordId, branchCode);
                            totalProcessed += records.Count;
                            records.Clear();

                            if (totalProcessed % 5000 == 0)
                            {
                                _logger.LogInformation("⚡ Legacy Excel processed {Processed} records...", totalProcessed);
                            }
                        }
                    }

                    // Lưu batch cuối cùng
                    if (records.Any())
                    {
                        await SaveBatchToDatabase(records, importedDataRecordId, branchCode);
                        totalProcessed += records.Count;
                    }

                    _logger.LogInformation("✅ Legacy Excel (.xls) processing completed: {Records} records", totalProcessed);
                    return totalProcessed;
                }
                else
                {
                    _logger.LogInformation("📊 Using ClosedXML for modern .xlsx file: {FileName}", file.FileName);

                    // Sử dụng ClosedXML cho .xlsx (code hiện tại)
                    return await ProcessModernExcelFile(stream, file, dataType, importedDataRecordId, statementDate, branchCode, batchSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing Excel file: {FileName}", file.FileName);
                throw;
            }
        }

        // 📊 Helper method để xử lý file .xlsx bằng ClosedXML (tách từ code cũ)
        private async Task<int> ProcessModernExcelFile(Stream stream, IFormFile file, string dataType,
            int importedDataRecordId, DateTime statementDate, string branchCode, int batchSize)
        {
            int totalProcessed = 0;
            var records = new List<Dictionary<string, object>>();

            // 🔍 Enhanced Excel file validation
            ClosedXML.Excel.XLWorkbook workbook;
            try
            {
                workbook = new ClosedXML.Excel.XLWorkbook(stream);
                _logger.LogInformation("✅ Excel workbook opened successfully");
            }
            catch (Exception excelEx)
            {
                _logger.LogError(excelEx, "❌ Failed to open Excel file: {FileName} - {Error}", file.FileName, excelEx.Message);
                throw new InvalidOperationException($"Cannot read Excel file: {excelEx.Message}", excelEx);
            }
            using (workbook)
            {
                // 🔍 Validate workbook has worksheets
                if (!workbook.Worksheets.Any())
                {
                    throw new InvalidOperationException("Excel file contains no worksheets");
                }

                var worksheet = workbook.Worksheets.First();
                var rows = worksheet.RowsUsed();

                _logger.LogInformation("📊 Excel file has {RowCount} rows used in worksheet: {WorksheetName}",
                    rows.Count(), worksheet.Name);

                if (!rows.Any())
                {
                    _logger.LogWarning("⚠️ Excel worksheet is empty");
                    return 0;
                }

                List<string>? headers = null;
                int rowIndex = 0;

                foreach (var row in rows)
                {
                    rowIndex++;

                    // � Dòng 1: Header (tiêu đề nguyên bản từ file gốc)
                    if (rowIndex == 1)
                    {
                        headers = GetExcelRowValuesWithEncoding(row);
                        _logger.LogInformation("� Headers found: [{Headers}]", string.Join("] [", headers.Take(10)));
                        continue;
                    }

                    // ❌ Skip nếu chưa có headers
                    if (headers == null) continue;

                    // 📊 Từ dòng 2 trở đi: Dữ liệu thực
                    var values = GetExcelRowValuesWithEncoding(row);

                    // Skip empty rows
                    if (values.All(v => string.IsNullOrWhiteSpace(v)))
                        continue;

                    var record = new Dictionary<string, object>();

                    // Map values to headers
                    for (int j = 0; j < Math.Min(headers.Count, values.Count); j++)
                    {
                        record[headers[j]] = values[j];
                    }

                    // Thêm metadata theo chuẩn Temporal Tables + Columnstore Indexes
                    record["BranchCode"] = branchCode;
                    record["StatementDate"] = statementDate.ToString("yyyy-MM-dd");
                    record["ImportDate"] = VietnamDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    record["ImportedBy"] = "System";

                    records.Add(record);

                    // Batch processing
                    if (records.Count >= batchSize)
                    {
                        await SaveBatchToDatabase(records, importedDataRecordId, branchCode);
                        totalProcessed += records.Count;
                        records.Clear();

                        if (totalProcessed % 5000 == 0)
                        {
                            _logger.LogInformation("⚡ Modern Excel processed {Processed} records...", totalProcessed);
                        }
                    }
                }

                // Lưu batch cuối cùng
                if (records.Any())
                {
                    await SaveBatchToDatabase(records, importedDataRecordId, branchCode);
                    totalProcessed += records.Count;
                }

                _logger.LogInformation("✅ Modern Excel (.xlsx) processing completed: {Records} records", totalProcessed);
            } // End using workbook

            return totalProcessed;
        }

        // 📋 Helper method để lấy giá trị từ Excel row với encoding đúng
        private List<string> GetExcelRowValuesWithEncoding(ClosedXML.Excel.IXLRow row)
        {
            var values = new List<string>();

            foreach (var cell in row.CellsUsed())
            {
                var value = cell.GetString().Trim();
                // 🔤 Đảm bảo encoding đúng cho ký tự tiếng Việt
                if (!string.IsNullOrEmpty(value))
                {
                    value = FixVietnameseEncoding(value);
                }
                values.Add(value);
            }

            return values;
        }

        // 📄 Helper method để xử lý nội dung file CSV với encoding đúng
        private async Task<int> ProcessCsvFileContent(StreamReader reader, string dataType,
            int importedDataRecordId, DateTime statementDate, string branchCode, int batchSize)
        {
            try
            {
                _logger.LogInformation("📄 Processing CSV content for: {DataType}", dataType);

                int totalProcessed = 0;
                var records = new List<Dictionary<string, object>>();

                string? line;
                var headerLines = new List<string>();
                List<string>? headers = null;
                int lineCount = 0;

                bool isSpecialHeaderFile = dataType.Contains("7800_DT_KHKD1");

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    lineCount++;

                    // 🔥 Xử lý đặc biệt cho file 7800_DT_KHKD1
                    if (isSpecialHeaderFile)
                    {
                        if (lineCount >= 10 && lineCount <= 12)
                        {
                            headerLines.Add(line);
                            continue;
                        }

                        if (lineCount == 12 && headerLines.Count == 3)
                        {
                            headers = ProcessSpecialHeader(headerLines);
                            _logger.LogInformation("🔥 7800_DT_KHKD1 Special Headers: {Headers}",
                                string.Join(", ", headers.Take(5)));
                            continue;
                        }

                        if (lineCount < 13) continue;
                    }
                    else
                    {
                        // Header row thông thường
                        if (lineCount == 1)
                        {
                            headers = line.Split(',').Select(h => h.Trim('"').Trim()).ToList();
                            _logger.LogInformation("📋 CSV Headers: {Headers}",
                                string.Join(", ", headers.Take(5)));
                            continue;
                        }
                    }

                    if (headers == null) continue;

                    // Data row
                    var values = line.Split(',').Select(v => v.Trim('"').Trim()).ToList();
                    var record = new Dictionary<string, object>();

                    for (int j = 0; j < Math.Min(headers.Count, values.Count); j++)
                    {
                        // 🔤 Fix encoding for Vietnamese characters
                        var value = values[j];
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = FixVietnameseEncoding(value);
                        }
                        record[headers[j]] = value;
                    }

                    // Thêm metadata
                    record["BranchCode"] = branchCode;
                    record["StatementDate"] = statementDate.ToString("yyyy-MM-dd");
                    record["ImportDate"] = VietnamDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    record["ImportedBy"] = "System";

                    records.Add(record);

                    // Batch processing
                    if (records.Count >= batchSize)
                    {
                        await SaveBatchToDatabase(records, importedDataRecordId, branchCode);
                        totalProcessed += records.Count;
                        records.Clear();

                        if (totalProcessed % 5000 == 0)
                        {
                            _logger.LogInformation("⚡ CSV processed {Processed} records...", totalProcessed);
                        }
                    }
                }

                // Lưu batch cuối cùng
                if (records.Any())
                {
                    await SaveBatchToDatabase(records, importedDataRecordId, branchCode);
                    totalProcessed += records.Count;
                }

                _logger.LogInformation("✅ CSV file processing completed: {Records} records", totalProcessed);
                return totalProcessed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing CSV content");
                throw;
            }
        }

        // 🔤 Helper method để sửa encoding tiếng Việt
        private string FixVietnameseEncoding(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return input;

                // Kiểm tra và fix các ký tự encoding bị lỗi thường gặp
                if (input.Contains("â€") || input.Contains("Ä") || input.Contains("Ã"))
                {
                    // Thử chuyển đổi từ UTF-8 bị lỗi thành Unicode đúng
                    var bytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
                    return System.Text.Encoding.UTF8.GetString(bytes);
                }

                // Nếu không có vấn đề, trả về nguyên bản
                return input;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "❌ Error fixing encoding for text: {Input}", input.Substring(0, Math.Min(input.Length, 50)));
                return input; // Fallback to original
            }
        }

        // 🔥 AUTO-PROCESS METHOD: Tự động xử lý dữ liệu sau khi import thành công
        private async Task AutoProcessAfterImport(int importedDataRecordId, string dataType, DateTime statementDate)
        {
            try
            {
                _logger.LogInformation("🔄 Starting auto-process for ImportId: {ImportId}, DataType: {DataType}",
                    importedDataRecordId, dataType);

                // Chỉ tự động xử lý cho các loại dữ liệu được hỗ trợ
                var supportedTypes = new[] { "GLCB41", "LN01", "LN02", "DP01" };
                if (!supportedTypes.Contains(dataType.ToUpper()))
                {
                    _logger.LogInformation("ℹ️ DataType {DataType} không cần auto-process", dataType);
                    return;
                }

                // Sử dụng injected processing service
                var processingResult = await _processingService.ProcessImportedDataToHistoryAsync(
                    importedDataRecordId, dataType.ToUpper(), statementDate);

                if (processingResult.Success)
                {
                    _logger.LogInformation("✅ Auto-process thành công cho ImportId: {ImportId}, Processed: {ProcessedCount} records",
                        importedDataRecordId, processingResult.ProcessedRecords);

                    // Cập nhật status trong database nếu cần
                    var importRecord = await _context.ImportedDataRecords.FindAsync(importedDataRecordId);
                    if (importRecord != null)
                    {
                        importRecord.Notes = $"{importRecord.Notes} | Auto-processed: {processingResult.ProcessedRecords} records";
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ Auto-process failed cho ImportId: {ImportId}, Error: {Error}",
                        importedDataRecordId, processingResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception trong auto-process cho ImportId: {ImportId}", importedDataRecordId);
            }
        }

        // 🔧 DEBUG ENDPOINT: Phân tích cấu trúc file Excel để debug
        [HttpPost("debug-excel")]
        public async Task<IActionResult> DebugExcelFile([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided");

                _logger.LogInformation("🔧 DEBUG: Analyzing Excel file {FileName}", file.FileName);

                using var stream = file.OpenReadStream();
                using var workbook = new ClosedXML.Excel.XLWorkbook(stream);

                var result = new
                {
                    FileName = file.FileName,
                    FileSize = file.Length,
                    Worksheets = workbook.Worksheets.Select(ws => new
                    {
                        Name = ws.Name,
                        RowsUsed = ws.RowsUsed().Count(),
                        ColumnsUsed = ws.ColumnsUsed().Count(),
                        FirstRows = ws.RowsUsed().Take(10).Select((row, index) => new
                        {
                            RowNumber = index + 1,
                            Values = row.CellsUsed().Select(cell => cell.GetString().Trim()).ToArray()
                        }).ToArray()
                    }).ToArray()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error analyzing Excel file");
                return StatusCode(500, new { message = "Error analyzing file", error = ex.Message });
            }
        }

        // 🔧 DEBUG ENDPOINT: Test Legacy Excel Reader Service
        [HttpPost("debug-legacy-excel")]
        public async Task<IActionResult> DebugLegacyExcelReader([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided");

                _logger.LogInformation("🔧 DEBUG: Testing Legacy Excel Reader with file {FileName}", file.FileName);

                using var stream = file.OpenReadStream();
                var result = await _legacyExcelReaderService.ReadExcelFileAsync(stream, file.FileName);

                var response = new
                {
                    FileName = file.FileName,
                    CanReadFile = _legacyExcelReaderService.CanReadFile(file.FileName),
                    Success = result.Success,
                    Message = result.Message,
                    Headers = result.Headers,
                    DataRowsCount = result.Data.Count,
                    TotalRows = result.TotalRows,
                    WorksheetName = result.WorksheetName,
                    Errors = result.Errors,
                    SampleData = result.Data.Take(3).ToArray() // First 3 rows for preview
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error testing legacy Excel reader");
                return StatusCode(500, new { message = "Error testing legacy reader", error = ex.Message });
            }
        }
    }
}
