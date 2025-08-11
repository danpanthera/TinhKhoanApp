using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.DTOs.DPDA;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// DPDA Controller - API endpoints cho bảng DPDA (Thẻ ghi nợ)
    /// Business columns từ CSV là single source of truth - 13 columns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DPDAController : ControllerBase
    {
        private readonly IDPDAService _dpdaService;
        private readonly ILogger<DPDAController> _logger;

        public DPDAController(IDPDAService dpdaService, ILogger<DPDAController> logger)
        {
            _dpdaService = dpdaService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy DPDA records với paging và search
        /// </summary>
        [HttpGet("preview")]
        public async Task<ActionResult<ApiResponse<PagedResult<DPDAPreviewDto>>>> GetPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting DPDA preview - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                    pageNumber, pageSize, searchTerm);

                var result = await _dpdaService.GetPreviewAsync(pageNumber, pageSize, searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA preview");
                return StatusCode(500, ApiResponse<PagedResult<DPDAPreviewDto>>.Error("Lỗi server khi lấy dữ liệu DPDA"));
            }
        }

        /// <summary>
        /// Import CSV file vào DPDA table
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<DPDAImportResultDto>>> ImportCsv(IFormFile file, [FromQuery] string uploadedBy = "System")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<DPDAImportResultDto>.Error("File CSV không hợp lệ"));
                }

                _logger.LogInformation("Starting DPDA CSV import - File: {FileName}, Size: {FileSize}, UploadedBy: {UploadedBy}",
                    file.FileName, file.Length, uploadedBy);

                var result = await _dpdaService.ImportCsvAsync(file, uploadedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing DPDA CSV - File: {FileName}", file?.FileName);
                return StatusCode(500, ApiResponse<DPDAImportResultDto>.Error("Lỗi server khi import dữ liệu CSV"));
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
                Service = "DPDAController",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
