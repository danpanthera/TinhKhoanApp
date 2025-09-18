using Khoan.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Dtos;
using Khoan.Api.Models.Dtos.GL01;
using Khoan.Api.Services.DataServices;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// GL01 Controller - cung cấp API cho dữ liệu General Ledger
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL01Controller : ControllerBase
    {
        private readonly IGL01DataService _gl01DataService;
        private readonly ILogger<GL01Controller> _logger;

        public GL01Controller(
            IGL01DataService gl01DataService,
            ILogger<GL01Controller> logger)
        {
            _gl01DataService = gl01DataService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy preview dữ liệu GL01 mới nhất
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01Preview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _gl01DataService.GetGL01PreviewAsync(count);
                return Ok(ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data, "GL01 preview data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 preview data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 preview data", "GL01_PREVIEW_ERROR"));
            }
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi GL01
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GL01DetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01Detail(long id)
        {
            try
            {
                var data = await _gl01DataService.GetGL01DetailAsync(id);
                if (data == null)
                    return NotFound(ApiResponse<object>.Error($"GL01 record with ID {id} not found", "GL01_NOT_FOUND"));

                return Ok(ApiResponse<GL01DetailsDto>.Ok(data, "GL01 detail retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 detail for ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 detail", "GL01_DETAIL_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL01 theo ngày
        /// </summary>
        [HttpGet("date")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01ByDate([FromQuery] DateTime date, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _gl01DataService.GetGL01ByDateAsync(date, maxResults);
                return Ok(ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data, $"GL01 data for date {date:yyyy-MM-dd} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 data for date {Date}", date);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 data for date", "GL01_DATE_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL01 theo đơn vị
        /// </summary>
        [HttpGet("unit/{unitCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01ByUnit(string unitCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _gl01DataService.GetGL01ByUnitCodeAsync(unitCode, maxResults);
                return Ok(ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data, $"GL01 data for unit {unitCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 data for unit {UnitCode}", unitCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 data for unit", "GL01_UNIT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL01 theo tài khoản
        /// </summary>
        [HttpGet("account/{accountCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01ByAccount(string accountCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _gl01DataService.GetGL01ByAccountCodeAsync(accountCode, maxResults);
                return Ok(ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data, $"GL01 data for account {accountCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 data for account {AccountCode}", accountCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 data for account", "GL01_ACCOUNT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp GL01 theo đơn vị
        /// </summary>
        [HttpGet("summary/unit/{unitCode}")]
        [ProducesResponseType(typeof(ApiResponse<GL01SummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01SummaryByUnit(string unitCode, [FromQuery] DateTime? date = null)
        {
            try
            {
                var data = await _gl01DataService.GetGL01SummaryByUnitAsync(unitCode, date);
                return Ok(ApiResponse<GL01SummaryDto>.Ok(data, $"GL01 summary for unit {unitCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 summary for unit {UnitCode}", unitCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 summary for unit", "GL01_SUMMARY_UNIT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp GL01 theo ngày
        /// </summary>
        [HttpGet("summary/date")]
        [ProducesResponseType(typeof(ApiResponse<GL01SummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL01SummaryByDate([FromQuery] DateTime date)
        {
            try
            {
                var data = await _gl01DataService.GetGL01SummaryByDateAsync(date);
                return Ok(ApiResponse<GL01SummaryDto>.Ok(data, $"GL01 summary for date {date:yyyy-MM-dd} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 summary for date {Date}", date);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL01 summary for date", "GL01_SUMMARY_DATE_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm GL01 theo nhiều tiêu chí
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<ApiResponse<PagedResult<GL01PreviewDto>>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SearchGL01(
            [FromQuery] string? keyword = null,
            [FromQuery] string? unitCode = null,
            [FromQuery] string? accountCode = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var data = await _gl01DataService.SearchGL01Async(
                    keyword, unitCode, accountCode, transactionType, fromDate, toDate, page, pageSize);

                return Ok(ApiResponse<ApiResponse<PagedResult<GL01PreviewDto>>>.Ok(data, "GL01 search results retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching GL01 data");
                return BadRequest(ApiResponse<object>.Error("Failed to search GL01 data", "GL01_SEARCH_ERROR"));
            }
        }
    }
}
