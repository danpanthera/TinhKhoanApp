using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller cho dữ liệu DP01 (Tài khoản tiền gửi)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DP01Controller : ControllerBase
    {
        private readonly IDP01DataService _dp01DataService;
        private readonly ILogger<DP01Controller> _logger;

        /// <summary>
        /// Khởi tạo controller với các dependencies
        /// </summary>
        public DP01Controller(
            IDP01DataService dp01DataService,
            ILogger<DP01Controller> logger)
        {
            _dp01DataService = dp01DataService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy preview dữ liệu DP01 mới nhất
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01Preview([FromQuery] int count = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 preview data, count: {Count}", count);
                var data = await _dp01DataService.GetDP01PreviewAsync(count);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data, "DP01 preview data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 preview data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 preview data", "DP01_PREVIEW_ERROR"));
            }
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi DP01
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DP01DetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01Detail(long id)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 detail for ID {Id}", id);
                var data = await _dp01DataService.GetDP01DetailAsync(id);
                if (data == null)
                    return NotFound(ApiResponse<object>.Error($"DP01 record with ID {id} not found", "DP01_NOT_FOUND"));

                return Ok(ApiResponse<DP01DetailDto>.Ok(data, "DP01 detail retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 detail for ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 detail", "DP01_DETAIL_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DP01 theo chi nhánh
        /// </summary>
        [HttpGet("branch/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 data for branch {BranchCode}, maxResults: {MaxResults}",
                    branchCode, maxResults);
                var data = await _dp01DataService.GetDP01ByBranchAsync(branchCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data,
                    $"DP01 data for branch {branchCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for branch {BranchCode}", branchCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 data for branch", "DP01_BRANCH_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DP01 theo khách hàng
        /// </summary>
        [HttpGet("customer/{customerCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByCustomer(string customerCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 data for customer {CustomerCode}, maxResults: {MaxResults}",
                    customerCode, maxResults);
                var data = await _dp01DataService.GetDP01ByCustomerAsync(customerCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data,
                    $"DP01 data for customer {customerCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for customer {CustomerCode}", customerCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 data for customer", "DP01_CUSTOMER_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DP01 theo số tài khoản
        /// </summary>
        [HttpGet("account/{accountNumber}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByAccountNumber(string accountNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 data for account {AccountNumber}, maxResults: {MaxResults}",
                    accountNumber, maxResults);
                var data = await _dp01DataService.GetDP01ByAccountNumberAsync(accountNumber, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data,
                    $"DP01 data for account {accountNumber} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for account {AccountNumber}", accountNumber);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 data for account", "DP01_ACCOUNT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp DP01 theo chi nhánh
        /// </summary>
        [HttpGet("summary/branch/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<DP01SummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01SummaryByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 summary for branch {BranchCode}, date: {Date}",
                    branchCode, date?.ToString("yyyy-MM-dd") ?? "latest");
                var data = await _dp01DataService.GetDP01SummaryByBranchAsync(branchCode, date);
                return Ok(ApiResponse<DP01SummaryDto>.Ok(data, $"DP01 summary for branch {branchCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 summary for branch {BranchCode}", branchCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 summary for branch", "DP01_SUMMARY_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm dữ liệu DP01 theo nhiều tiêu chí
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SearchDP01(
            [FromQuery] string? keyword,
            [FromQuery] string? branchCode,
            [FromQuery] string? customerCode,
            [FromQuery] string? accountNumber,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Searching DP01 data with parameters: keyword={Keyword}, branchCode={BranchCode}, " +
                    "customerCode={CustomerCode}, accountNumber={AccountNumber}, fromDate={FromDate}, toDate={ToDate}, " +
                    "page={Page}, pageSize={PageSize}",
                    keyword, branchCode, customerCode, accountNumber,
                    fromDate?.ToString("yyyy-MM-dd"), toDate?.ToString("yyyy-MM-dd"), page, pageSize);

                var data = await _dp01DataService.SearchDP01Async(
                    keyword,
                    branchCode,
                    customerCode,
                    accountNumber,
                    fromDate,
                    toDate,
                    page,
                    pageSize);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DP01 data");
                return BadRequest(ApiResponse<object>.Error("Failed to search DP01 data", "DP01_SEARCH_ERROR"));
            }
        }
    }
}
