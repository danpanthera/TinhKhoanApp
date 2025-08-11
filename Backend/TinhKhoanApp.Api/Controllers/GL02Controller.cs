using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.Dtos.GL02;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// GL02 Controller - API endpoints cho bảng GL02 (Dữ liệu kế toán chi tiết)
    /// Xử lý 16 business columns theo CSV structure
    /// Business columns từ CSV là single source of truth
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL02Controller : ControllerBase
    {
        private readonly IGL02Service _gl02Service;
        private readonly ILogger<GL02Controller> _logger;

        public GL02Controller(IGL02Service gl02Service, ILogger<GL02Controller> logger)
        {
            _gl02Service = gl02Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy GL02 records với paging và search
        /// </summary>
        [HttpGet("preview")]
        public async Task<ActionResult<ApiResponse<PagedResult<GL02PreviewDto>>>> GetPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting GL02 preview - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                    pageNumber, pageSize, searchTerm);

                var result = await _gl02Service.GetAllAsync(pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<GL02PreviewDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL02 preview");
                return StatusCode(500, ApiResponse<PagedResult<GL02PreviewDto>>.Error("Lỗi server khi lấy dữ liệu GL02"));
            }
        }

        /// <summary>
        /// Import CSV file vào GL02 table
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<GL02ImportResultDto>>> ImportCsv(IFormFile file, [FromQuery] string uploadedBy = "System")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<GL02ImportResultDto>.Error("File CSV không hợp lệ"));
                }

                _logger.LogInformation("Starting GL02 CSV import - File: {FileName}, Size: {FileSize}, UploadedBy: {UploadedBy}",
                    file.FileName, file.Length, uploadedBy);

                // Convert IFormFile to string for service
                using var reader = new StreamReader(file.OpenReadStream());
                var csvContent = await reader.ReadToEndAsync();

                var result = await _gl02Service.ImportFromCsvAsync(csvContent, file.FileName);
                return Ok(ApiResponse<GL02ImportResultDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL02 CSV - File: {FileName}", file?.FileName);
                return StatusCode(500, ApiResponse<GL02ImportResultDto>.Error("Lỗi server khi import dữ liệu CSV"));
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                Status = "Healthy",
                Service = "GL02Controller",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
