using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller cho Direct Import - Import trực tiếp vào bảng riêng biệt
    /// Bỏ hoàn toàn ImportedDataItems để tăng hiệu năng 2-5x
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DirectImportController : ControllerBase
    {
        private readonly IDirectImportService _directImportService;
        private readonly ILogger<DirectImportController> _logger;

        public DirectImportController(
            IDirectImportService directImportService,
            ILogger<DirectImportController> logger)
        {
            _directImportService = directImportService;
            _logger = logger;
        }

        /// <summary>
        /// Import thông minh - tự động phát hiện loại file và import trực tiếp
        /// </summary>
        /// <param name="file">File dữ liệu CSV/Excel</param>
        /// <param name="statementDate">Ngày báo cáo (optional)</param>
        /// <returns>Kết quả import với thông tin chi tiết</returns>
        [HttpPost("smart")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DirectImportResult>> ImportSmartDirect(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                _logger.LogInformation("🚀 [DIRECT_IMPORT_API] Smart import start: {FileName}, Size: {FileSize}MB",
                    file.FileName, file.Length / 1024.0 / 1024.0);

                var result = await _directImportService.ImportSmartDirectAsync(file, statementDate);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [DIRECT_IMPORT_API] Smart import success: {Records} records in {Duration}ms",
                        result.ProcessedRecords, result.Duration.TotalMilliseconds);
                    return Ok(result);
                }
                else
                {
                    _logger.LogError("❌ [DIRECT_IMPORT_API] Smart import failed: {Error}", result.ErrorMessage);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [DIRECT_IMPORT_API] Smart import exception: {FileName}", file?.FileName);
                return StatusCode(500, new DirectImportResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown"
                });
            }
        }

        /// <summary>
        /// Import trực tiếp file DP01 vào bảng DP01
        /// </summary>
        [HttpPost("dp01")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DirectImportResult>> ImportDP01Direct(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                _logger.LogInformation("🚀 [DP01_DIRECT_API] Import start: {FileName}", file.FileName);

                var result = await _directImportService.ImportDP01DirectAsync(file, statementDate);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [DP01_DIRECT_API] Import success: {Records} records", result.ProcessedRecords);
                    return Ok(result);
                }
                else
                {
                    _logger.LogError("❌ [DP01_DIRECT_API] Import failed: {Error}", result.ErrorMessage);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [DP01_DIRECT_API] Import exception: {FileName}", file?.FileName);
                return StatusCode(500, new DirectImportResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown"
                });
            }
        }

        /// <summary>
        /// Import trực tiếp file LN01 vào bảng LN01
        /// </summary>
        [HttpPost("ln01")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DirectImportResult>> ImportLN01Direct(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                var result = await _directImportService.ImportLN01DirectAsync(file, statementDate);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [LN01_DIRECT_API] Import exception: {FileName}", file?.FileName);
                return StatusCode(500, new DirectImportResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin trạng thái import
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<object>> GetImportStatus()
        {
            try
            {
                // TODO: Implement status tracking
                return Ok(new
                {
                    Status = "Direct Import System Online",
                    Version = "1.0.0",
                    Features = new[]
                    {
                        "✅ Smart Detection - Tự động phát hiện loại file",
                        "✅ SqlBulkCopy - Tăng tốc import 2-5x",
                        "✅ Temporal Tables - Lịch sử thay đổi tự động",
                        "✅ Columnstore Indexes - Hiệu năng query 10-100x",
                        "✅ Metadata Tracking - Chỉ lưu metadata, không lưu raw data"
                    },
                    SupportedDataTypes = new[]
                    {
                        "DP01 - Báo cáo tài chính",
                        "LN01 - Dữ liệu cho vay",
                        "GL01 - Dữ liệu sổ cái",
                        "GL41 - Dữ liệu giao dịch",
                        "DPDA - Dữ liệu phân tích",
                        "EI01 - Dữ liệu lãi suất",
                        "RR01 - Dữ liệu rủi ro",
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [DIRECT_IMPORT_API] Status check exception");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy số lượng records thực tế từ database tables (không phải metadata)
        /// </summary>
        [HttpGet("table-counts")]
        public async Task<ActionResult<Dictionary<string, int>>> GetTableCounts()
        {
            try
            {
                _logger.LogInformation("📊 Getting actual table record counts");

                var counts = await _directImportService.GetTableRecordCountsAsync();

                return Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting table counts");
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
