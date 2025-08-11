using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.Dtos.EI01;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// EI01 Controller - API endpoints cho bảng EI01 (Dữ liệu nhân viên)
    /// Xử lý 24 business columns theo CSV structure
    /// Business columns từ CSV là single source of truth
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EI01Controller : ControllerBase
    {
        private readonly IEI01Service _ei01Service;
        private readonly ILogger<EI01Controller> _logger;

        public EI01Controller(IEI01Service ei01Service, ILogger<EI01Controller> logger)
        {
            _ei01Service = ei01Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy EI01 records với paging và search
        /// </summary>
        [HttpGet("preview")]
        public async Task<ActionResult<ApiResponse<PagedResult<EI01PreviewDto>>>> GetPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                _logger.LogInformation("Getting EI01 preview - Page: {PageNumber}, Size: {PageSize}",
                    pageNumber, pageSize);

                var result = await _ei01Service.GetAllAsync(pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<EI01PreviewDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 preview");
                return StatusCode(500, ApiResponse<PagedResult<EI01PreviewDto>>.Error("Lỗi server khi lấy dữ liệu EI01"));
            }
        }        /// <summary>
                 /// Import CSV file vào EI01 table
                 /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<EI01ImportResultDto>>> ImportCsv(IFormFile file, [FromQuery] string uploadedBy = "System")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<EI01ImportResultDto>.Error("File CSV không hợp lệ"));
                }

                _logger.LogInformation("Starting EI01 CSV import - File: {FileName}, Size: {FileSize}, UploadedBy: {UploadedBy}",
                    file.FileName, file.Length, uploadedBy);

                // Convert IFormFile to string for service
                using var reader = new StreamReader(file.OpenReadStream());
                var csvContent = await reader.ReadToEndAsync();

                var result = await _ei01Service.ImportFromCsvAsync(csvContent, file.FileName);
                return Ok(ApiResponse<EI01ImportResultDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing EI01 CSV - File: {FileName}", file?.FileName);
                return StatusCode(500, ApiResponse<EI01ImportResultDto>.Error("Lỗi server khi import dữ liệu CSV"));
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
                Service = "EI01Controller",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
