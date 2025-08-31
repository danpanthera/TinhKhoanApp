using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Interfaces;
using Khoan.Api.Dtos.LN01;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// LN01 - Dữ liệu khoản vay đầy đủ (79 cột kinh doanh + 4 cột hệ thống)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LN01Controller : ControllerBase
    {
        private readonly ILN01Service _service;
        private readonly ILogger<LN01Controller> _logger;

        public LN01Controller(ILN01Service service, ILogger<LN01Controller> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách khoản vay LN01 (phân trang)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN01PreviewDto>>>> GetLN01List()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<LN01PreviewDto>>
                {
                    Data = result,
                    Success = true,
                    Message = "Lấy danh sách LN01 thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách LN01");
                return StatusCode(500, new ApiResponse<IEnumerable<LN01PreviewDto>>
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết một khoản vay LN01
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LN01DetailsDto>>> GetLN01Details(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<LN01DetailsDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy khoản vay LN01"
                    });
                }
                return Ok(new ApiResponse<LN01DetailsDto>
                {
                    Data = result,
                    Success = true,
                    Message = "Lấy chi tiết LN01 thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết LN01 ID: {Id}", id);
                return StatusCode(500, new ApiResponse<LN01DetailsDto>
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Import dữ liệu LN01 từ file CSV
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<LN01ImportResultDto>>> ImportFromCSV(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ApiResponse<LN01ImportResultDto>
                    {
                        Success = false,
                        Message = "File CSV không hợp lệ"
                    });
                }

                using var stream = file.OpenReadStream();
                var result = await _service.ImportFromCsvAsync(stream);
                
                return Ok(new ApiResponse<LN01ImportResultDto>
                {
                    Data = result,
                    Success = true,
                    Message = $"Import CSV thành công: {result.SuccessRows}/{result.TotalRows} dòng"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import CSV LN01: {FileName}", file?.FileName);
                return StatusCode(500, new ApiResponse<LN01ImportResultDto>
                {
                    Success = false,
                    Message = $"Lỗi import: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Export dữ liệu LN01 ra file CSV
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportToCSV()
        {
            try
            {
                var csvStream = await _service.ExportToCsvAsync();
                return File(csvStream, "text/csv", $"LN01_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi export CSV LN01");
                return StatusCode(500, new { error = $"Lỗi export: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan LN01
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<LN01SummaryDto>>> GetSummary()
        {
            try
            {
                var result = await _service.GetSummaryAsync();
                return Ok(new ApiResponse<LN01SummaryDto>
                {
                    Data = result,
                    Success = true,
                    Message = "Lấy thống kê LN01 thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê LN01");
                return StatusCode(500, new ApiResponse<LN01SummaryDto>
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo chi nhánh
        /// </summary>
        [HttpGet("branch/{branchCode}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN01PreviewDto>>>> GetByBranch(string branchCode)
        {
            try
            {
                var result = await _service.GetByBranchAsync(branchCode);
                return Ok(new ApiResponse<IEnumerable<LN01PreviewDto>>
                {
                    Data = result,
                    Success = true,
                    Message = $"Lấy dữ liệu chi nhánh {branchCode} thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu chi nhánh LN01: {BranchCode}", branchCode);
                return StatusCode(500, new ApiResponse<IEnumerable<LN01PreviewDto>>
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }
    }
}
