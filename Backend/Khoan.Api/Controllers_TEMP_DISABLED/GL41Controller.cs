using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Dtos.GL41;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// GL41 Controller - cung cấp API cho dữ liệu Trial Balance
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL41Controller : ControllerBase
    {
        private readonly IGL41DataService _gl41DataService;
        private readonly ILogger<GL41Controller> _logger;

        public GL41Controller(
            IGL41DataService gl41DataService,
            ILogger<GL41Controller> logger)
        {
            _gl41DataService = gl41DataService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy preview dữ liệu GL41 mới nhất
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL41PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL41Preview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _gl41DataService.GetGL41PreviewAsync(count);
                return Ok(ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(data, "GL41 preview data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 preview data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL41 preview data", "GL41_PREVIEW_ERROR"));
            }
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi GL41
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GL41DetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL41Detail(long id)
        {
            try
            {
                var data = await _gl41DataService.GetGL41DetailAsync(id);
                if (data == null)
                    return NotFound(ApiResponse<object>.Error($"GL41 record with ID {id} not found", "GL41_NOT_FOUND"));

                return Ok(ApiResponse<GL41DetailsDto>.Ok(data, "GL41 detail retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 detail for ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL41 detail", "GL41_DETAIL_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL41 theo đơn vị
        /// </summary>
        [HttpGet("unit/{unitCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL41PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL41ByUnit(string unitCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _gl41DataService.GetGL41ByUnitAsync(unitCode, maxResults);
                return Ok(ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(data, $"GL41 data for unit {unitCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data for unit {UnitCode}", unitCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL41 data for unit", "GL41_UNIT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL41 theo tài khoản
        /// </summary>
        [HttpGet("account/{accountCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL41PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL41ByAccount(string accountCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _gl41DataService.GetGL41ByAccountAsync(accountCode, maxResults);
                return Ok(ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(data, $"GL41 data for account {accountCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data for account {AccountCode}", accountCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL41 data for account", "GL41_ACCOUNT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp GL41 theo đơn vị
        /// </summary>
        [HttpGet("summary/unit/{unitCode}")]
        [ProducesResponseType(typeof(ApiResponse<GL41SummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL41SummaryByUnit(string unitCode, [FromQuery] DateTime? date = null)
        {
            try
            {
                var data = await _gl41DataService.GetGL41SummaryByUnitAsync(unitCode, date);
                return Ok(ApiResponse<GL41SummaryDto>.Ok(data, $"GL41 summary for unit {unitCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 summary for unit {UnitCode}", unitCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL41 summary for unit", "GL41_SUMMARY_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm dữ liệu GL41 theo nhiều tiêu chí
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<GL41PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SearchGL41(
            [FromQuery] string? keyword,
            [FromQuery] string? unitCode,
            [FromQuery] string? accountCode,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var data = await _gl41DataService.SearchGL41Async(
                    keyword,
                    unitCode,
                    accountCode,
                    fromDate,
                    toDate,
                    page,
                    pageSize);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching GL41 data");
                return BadRequest(ApiResponse<object>.Error("Failed to search GL41 data", "GL41_SEARCH_ERROR"));
            }
        }
    }
}
