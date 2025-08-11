using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.DTOs.GL01;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// GL01 Controller - API endpoints cho bảng GL01 (Tổng hợp kế toán)
    /// Xử lý 27 business columns theo CSV structure
    /// Business columns từ CSV là single source of truth
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL01Controller : ControllerBase
    {
        private readonly IGL01Service _gl01Service;
        private readonly ILogger<GL01Controller> _logger;

        public GL01Controller(IGL01Service gl01Service, ILogger<GL01Controller> logger)
        {
            _gl01Service = gl01Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy GL01 records với paging và search
        /// </summary>
        [HttpGet("preview")]
        public async Task<ActionResult<ApiResponse<PagedResult<GL01PreviewDto>>>> GetPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting GL01 preview - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                    pageNumber, pageSize, searchTerm);

                var result = await _gl01Service.GetAllAsync(pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<GL01PreviewDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL01 preview");
                return StatusCode(500, ApiResponse<PagedResult<GL01PreviewDto>>.Error("Lỗi server khi lấy dữ liệu GL01"));
            }
        }

        /// <summary>
        /// Import CSV file vào GL01 table
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<GL01ImportResultDto>>> ImportCsv(IFormFile file, [FromQuery] string uploadedBy = "System")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<GL01ImportResultDto>.Error("File CSV không hợp lệ"));
                }

                _logger.LogInformation("Starting GL01 CSV import - File: {FileName}, Size: {FileSize}, UploadedBy: {UploadedBy}",
                    file.FileName, file.Length, uploadedBy);

                // Convert IFormFile to string for service
                using var reader = new StreamReader(file.OpenReadStream());
                var csvContent = await reader.ReadToEndAsync();

                var result = await _gl01Service.ImportFromCsvAsync(csvContent, file.FileName);
                return Ok(ApiResponse<GL01ImportResultDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL01 CSV - File: {FileName}", file?.FileName);
                return StatusCode(500, ApiResponse<GL01ImportResultDto>.Error("Lỗi server khi import dữ liệu CSV"));
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
                Service = "GL01Controller",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
