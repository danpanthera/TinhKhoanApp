using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.DTOs.GL41;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// GL41 Controller - API endpoints cho bảng GL41 (Bảng cân đối kế toán tổng hợp)
    /// Xử lý 18 business columns theo CSV structure
    /// Business columns từ CSV là single source of truth
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL41Controller : ControllerBase
    {
        private readonly IGL41Service _gl41Service;
        private readonly ILogger<GL41Controller> _logger;

        public GL41Controller(IGL41Service gl41Service, ILogger<GL41Controller> logger)
        {
            _gl41Service = gl41Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy GL41 records với paging và search
        /// </summary>
        [HttpGet("preview")]
        public async Task<ActionResult<ApiResponse<PagedResult<GL41PreviewDto>>>> GetPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting GL41 preview - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                    pageNumber, pageSize, searchTerm);

                var result = await _gl41Service.GetAllAsync(pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<GL41PreviewDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL41 preview");
                return StatusCode(500, ApiResponse<PagedResult<GL41PreviewDto>>.Error("Lỗi server khi lấy dữ liệu GL41"));
            }
        }

        /// <summary>
        /// Lấy GL41 record by ID với full details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GL41DetailsDto>>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting GL41 by ID: {Id}", id);

                var result = await _gl41Service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(ApiResponse<GL41DetailsDto>.Error($"GL41 record với ID {id} không tồn tại"));
                }

                return Ok(ApiResponse<GL41DetailsDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL41 by ID: {Id}", id);
                return StatusCode(500, ApiResponse<GL41DetailsDto>.Error("Lỗi server khi lấy dữ liệu GL41"));
            }
        }

        /// <summary>
        /// Import CSV file vào GL41 table
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<GL41ImportResultDto>>> ImportCsv(IFormFile file, [FromQuery] string uploadedBy = "System")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<GL41ImportResultDto>.Error("File CSV không hợp lệ"));
                }

                _logger.LogInformation("Starting GL41 CSV import - File: {FileName}, Size: {FileSize}, UploadedBy: {UploadedBy}",
                    file.FileName, file.Length, uploadedBy);

                // Convert IFormFile to string for service
                using var reader = new StreamReader(file.OpenReadStream());
                var csvContent = await reader.ReadToEndAsync();

                var result = await _gl41Service.ImportFromCsvAsync(csvContent, file.FileName);
                return Ok(ApiResponse<GL41ImportResultDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL41 CSV - File: {FileName}", file?.FileName);
                return StatusCode(500, ApiResponse<GL41ImportResultDto>.Error("Lỗi server khi import dữ liệu CSV"));
            }
        }

        /// <summary>
        /// Lấy summary statistics cho GL41 data
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<GL41SummaryDto>>> GetSummary()
        {
            try
            {
                _logger.LogInformation("Getting GL41 summary");

                var result = await _gl41Service.GetSummaryAsync();
                return Ok(ApiResponse<GL41SummaryDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL41 summary");
                return StatusCode(500, ApiResponse<GL41SummaryDto>.Error("Lỗi server khi lấy thống kê GL41"));
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
                Service = "GL41Controller",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
