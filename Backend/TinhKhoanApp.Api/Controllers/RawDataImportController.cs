using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models.RawData;
using OfficeOpenXml;
using System.Text;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]  // Temporarily disabled for testing
    public class RawDataImportController : ControllerBase
    {
        private readonly IRawDataImportService _importService;
        private readonly ILogger<RawDataImportController> _logger;

        public RawDataImportController(IRawDataImportService importService, ILogger<RawDataImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        /// <summary>
        /// Import LN01 data từ Excel file
        /// </summary>
        [HttpPost("ln01/upload")]
        public async Task<ActionResult<ImportResponseDto>> ImportLN01Data(IFormFile file, [FromForm] ImportRequestDto request)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File không được để trống");

                if (!IsExcelFile(file))
                    return BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");

                request.TableName = "LN01";
                request.CreatedBy = User.Identity?.Name ?? "Unknown";

                var data = await ParseLN01ExcelFile(file);
                var result = await _importService.ImportLN01DataAsync(request, data);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing LN01 data");
                return StatusCode(500, new ImportResponseDto
                {
                    Success = false,
                    Message = "Lỗi server khi import dữ liệu",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Import GL01 data từ Excel file
        /// </summary>
        [HttpPost("gl01/upload")]
        public async Task<ActionResult<ImportResponseDto>> ImportGL01Data(IFormFile file, [FromForm] ImportRequestDto request)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File không được để trống");

                if (!IsExcelFile(file))
                    return BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");

                request.TableName = "GL01";
                request.CreatedBy = User.Identity?.Name ?? "Unknown";

                var data = await ParseGL01ExcelFile(file);
                var result = await _importService.ImportGL01DataAsync(request, data);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL01 data");
                return StatusCode(500, new ImportResponseDto
                {
                    Success = false,
                    Message = "Lỗi server khi import dữ liệu",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp cho tất cả bảng
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetAllStatistics()
        {
            try
            {
                var ln01Stats = await _importService.GetImportStatisticsAsync("LN01");
                var gl01Stats = await _importService.GetImportStatisticsAsync("GL01");
                var dp01Stats = await _importService.GetImportStatisticsAsync("DP01");
                
                return Ok(new
                {
                    LN01 = ln01Stats,
                    GL01 = gl01Stats,
                    DP01 = dp01Stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all import statistics");
                return StatusCode(500, "Lỗi khi lấy thống kê import");
            }
        }

        /// <summary>
        /// Lấy thống kê import cho bảng cụ thể
        /// </summary>
        [HttpGet("statistics/{tableName}")]
        public async Task<ActionResult<ImportStatisticsDto>> GetImportStatistics(string tableName)
        {
            try
            {
                var stats = await _importService.GetImportStatisticsAsync(tableName);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting import statistics for {tableName}");
                return StatusCode(500, "Lỗi khi lấy thống kê import");
            }
        }

        /// <summary>
        /// Lấy danh sách import gần đây
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<List<ImportLog>>> GetRecentImports([FromQuery] int limit = 20)
        {
            try
            {
                var imports = await _importService.GetRecentImportsAsync(limit);
                return Ok(imports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent imports");
                return StatusCode(500, "Lỗi khi lấy danh sách import gần đây");
            }
        }

        /// <summary>
        /// Lấy lịch sử thay đổi của LN01 record
        /// </summary>
        [HttpGet("ln01/history/{sourceId}")]
        public async Task<ActionResult<List<LN01History>>> GetLN01History(string sourceId)
        {
            try
            {
                var history = await _importService.GetLN01HistoryAsync(sourceId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting LN01 history for {sourceId}");
                return StatusCode(500, "Lỗi khi lấy lịch sử LN01");
            }
        }

        /// <summary>
        /// Lấy lịch sử thay đổi của GL01 record
        /// </summary>
        [HttpGet("gl01/history/{sourceId}")]
        public async Task<ActionResult<List<GL01History>>> GetGL01History(string sourceId)
        {
            try
            {
                var history = await _importService.GetGL01HistoryAsync(sourceId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting GL01 history for {sourceId}");
                return StatusCode(500, "Lỗi khi lấy lịch sử GL01");
            }
        }

        /// <summary>
        /// Lấy snapshot LN01 tại thời điểm cụ thể
        /// </summary>
        [HttpGet("ln01/snapshot")]
        public async Task<ActionResult<List<LN01History>>> GetLN01Snapshot([FromQuery] DateTime snapshotDate)
        {
            try
            {
                var snapshot = await _importService.GetLN01SnapshotAsync(snapshotDate);
                return Ok(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting LN01 snapshot for {snapshotDate}");
                return StatusCode(500, "Lỗi khi lấy snapshot LN01");
            }
        }

        /// <summary>
        /// Lấy snapshot GL01 tại thời điểm cụ thể
        /// </summary>
        [HttpGet("gl01/snapshot")]
        public async Task<ActionResult<List<GL01History>>> GetGL01Snapshot([FromQuery] DateTime snapshotDate)
        {
            try
            {
                var snapshot = await _importService.GetGL01SnapshotAsync(snapshotDate);
                return Ok(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting GL01 snapshot for {snapshotDate}");
                return StatusCode(500, "Lỗi khi lấy snapshot GL01");
            }
        }

        /// <summary>
        /// Dọn dẹp dữ liệu cũ
        /// </summary>
        [HttpPost("cleanup/{tableName}")]
        public async Task<ActionResult<bool>> CleanupOldData(string tableName, [FromQuery] int retainMonths = 12)
        {
            try
            {
                var result = await _importService.CleanupOldDataAsync(tableName, retainMonths);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cleaning up old data for {tableName}");
                return StatusCode(500, "Lỗi khi dọn dẹp dữ liệu cũ");
            }
        }

        /// <summary>
        /// Validate Excel file format
        /// </summary>
        [HttpPost("validate-file")]
        public async Task<ActionResult> ValidateExcelFile(IFormFile file, [FromQuery] string tableType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File không được để trống");

                if (!IsExcelFile(file))
                    return BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");

                var validationResult = await ValidateExcelStructure(file, tableType);
                return Ok(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Excel file");
                return StatusCode(500, "Lỗi khi validate file Excel");
            }
        }

        private bool IsExcelFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }

        private async Task<List<LN01History>> ParseLN01ExcelFile(IFormFile file)
        {
            var data = new List<LN01History>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Skip header row
            {
                var record = new LN01History
                {
                    SourceID = worksheet.Cells[row, 1].Value?.ToString() ?? Guid.NewGuid().ToString(),
                    MANDT = worksheet.Cells[row, 2].Value?.ToString(),
                    BUKRS = worksheet.Cells[row, 3].Value?.ToString(),
                    LAND1 = worksheet.Cells[row, 4].Value?.ToString(),
                    WAERS = worksheet.Cells[row, 5].Value?.ToString(),
                    SPRAS = worksheet.Cells[row, 6].Value?.ToString(),
                    KTOPL = worksheet.Cells[row, 7].Value?.ToString(),
                    WAABW = worksheet.Cells[row, 8].Value?.ToString(),
                    PERIV = worksheet.Cells[row, 9].Value?.ToString(),
                    KOKFI = worksheet.Cells[row, 10].Value?.ToString(),
                    RCOMP = worksheet.Cells[row, 11].Value?.ToString(),
                    ADRNR = worksheet.Cells[row, 12].Value?.ToString(),
                    STCEG = worksheet.Cells[row, 13].Value?.ToString(),
                    FIKRS = worksheet.Cells[row, 14].Value?.ToString(),
                    XFMCO = worksheet.Cells[row, 15].Value?.ToString(),
                    XFMCB = worksheet.Cells[row, 16].Value?.ToString(),
                    XFMCA = worksheet.Cells[row, 17].Value?.ToString(),
                    TXJCD = worksheet.Cells[row, 18].Value?.ToString()
                };

                data.Add(record);
            }

            return data;
        }

        private async Task<List<GL01History>> ParseGL01ExcelFile(IFormFile file)
        {
            var data = new List<GL01History>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Skip header row
            {
                var record = new GL01History
                {
                    SourceID = worksheet.Cells[row, 1].Value?.ToString() ?? Guid.NewGuid().ToString(),
                    MANDT = worksheet.Cells[row, 2].Value?.ToString(),
                    BUKRS = worksheet.Cells[row, 3].Value?.ToString(),
                    GJAHR = worksheet.Cells[row, 4].Value?.ToString(),
                    BELNR = worksheet.Cells[row, 5].Value?.ToString(),
                    BUZEI = worksheet.Cells[row, 6].Value?.ToString(),
                    AUGDT = worksheet.Cells[row, 7].Value?.ToString(),
                    AUGCP = worksheet.Cells[row, 8].Value?.ToString(),
                    AUGBL = worksheet.Cells[row, 9].Value?.ToString(),
                    BSCHL = worksheet.Cells[row, 10].Value?.ToString(),
                    KOART = worksheet.Cells[row, 11].Value?.ToString(),
                    UMSKZ = worksheet.Cells[row, 12].Value?.ToString(),
                    UMSKS = worksheet.Cells[row, 13].Value?.ToString(),
                    ZUMSK = worksheet.Cells[row, 14].Value?.ToString(),
                    SHKZG = worksheet.Cells[row, 15].Value?.ToString(),
                    GSBER = worksheet.Cells[row, 16].Value?.ToString(),
                    PARGB = worksheet.Cells[row, 17].Value?.ToString(),
                    MWSKZ = worksheet.Cells[row, 18].Value?.ToString(),
                    QSSKZ = worksheet.Cells[row, 19].Value?.ToString(),
                    DMBTR = ParseDecimal(worksheet.Cells[row, 20].Value),
                    WRBTR = ParseDecimal(worksheet.Cells[row, 21].Value),
                    KZBTR = ParseDecimal(worksheet.Cells[row, 22].Value),
                    PSWBT = ParseDecimal(worksheet.Cells[row, 23].Value),
                    PSWSL = worksheet.Cells[row, 24].Value?.ToString(),
                    HKONT = worksheet.Cells[row, 25].Value?.ToString(),
                    KUNNR = worksheet.Cells[row, 26].Value?.ToString(),
                    LIFNR = worksheet.Cells[row, 27].Value?.ToString(),
                    SGTXT = worksheet.Cells[row, 28].Value?.ToString()
                    // Add more fields as needed
                };

                data.Add(record);
            }

            return data;
        }

        private decimal? ParseDecimal(object value)
        {
            if (value == null) return null;
            if (decimal.TryParse(value.ToString(), out var result))
                return result;
            return null;
        }

        private async Task<object> ValidateExcelStructure(IFormFile file, string tableType)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            
            if (package.Workbook.Worksheets.Count == 0)
                return new { IsValid = false, Message = "File Excel không có worksheet nào" };

            var worksheet = package.Workbook.Worksheets[0];
            var headers = GetHeaderRow(worksheet);

            var expectedHeaders = tableType.ToUpper() switch
            {
                "LN01" => new[] { "SourceID", "MANDT", "BUKRS", "LAND1", "WAERS", "SPRAS", "KTOPL", "WAABW", "PERIV", "KOKFI", "RCOMP", "ADRNR", "STCEG", "FIKRS", "XFMCO", "XFMCB", "XFMCA", "TXJCD" },
                "GL01" => new[] { "SourceID", "MANDT", "BUKRS", "GJAHR", "BELNR", "BUZEI", "AUGDT", "AUGCP", "AUGBL", "BSCHL", "KOART", "UMSKZ", "UMSKS", "ZUMSK", "SHKZG", "GSBER", "PARGB", "MWSKZ", "QSSKZ", "DMBTR", "WRBTR", "KZBTR", "PSWBT", "PSWSL", "HKONT", "KUNNR", "LIFNR", "SGTXT" },
                _ => new string[0]
            };

            var missingHeaders = expectedHeaders.Except(headers, StringComparer.OrdinalIgnoreCase).ToList();
            var extraHeaders = headers.Except(expectedHeaders, StringComparer.OrdinalIgnoreCase).ToList();

            return new
            {
                IsValid = missingHeaders.Count == 0,
                Message = missingHeaders.Count == 0 ? "File Excel hợp lệ" : $"Thiếu các cột: {string.Join(", ", missingHeaders)}",
                MissingHeaders = missingHeaders,
                ExtraHeaders = extraHeaders,
                RowCount = worksheet.Dimension?.Rows - 1 ?? 0 // Exclude header row
            };
        }

        private List<string> GetHeaderRow(ExcelWorksheet worksheet)
        {
            var headers = new List<string>();
            if (worksheet.Dimension == null) return headers;

            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(headerValue))
                    headers.Add(headerValue);
            }

            return headers;
        }
    }
}
