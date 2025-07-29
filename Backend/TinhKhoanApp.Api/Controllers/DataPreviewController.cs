using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Data Preview Controller - cung cấp các API để xem và tìm kiếm dữ liệu
    /// Thay thế TestDataController với cấu trúc clean hơn, sử dụng repository và service layer
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DataPreviewController : ControllerBase
    {
        private readonly IDataPreviewService _dataPreviewService;
        private readonly ILogger<DataPreviewController> _logger;

        public DataPreviewController(
            IDataPreviewService dataPreviewService,
            ILogger<DataPreviewController> logger)
        {
            _dataPreviewService = dataPreviewService;
            _logger = logger;
        }

        #region DP01 Endpoints

        /// <summary>
        /// Lấy preview dữ liệu DP01 mới nhất
        /// </summary>
        [HttpGet("dp01")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01Preview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _dataPreviewService.GetDP01PreviewAsync(count);
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
        [HttpGet("dp01/{id}")]
        [ProducesResponseType(typeof(ApiResponse<DP01DetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01Detail(int id)
        {
            try
            {
                var data = await _dataPreviewService.GetDP01DetailAsync(id);
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
        [HttpGet("dp01/branch/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dataPreviewService.GetDP01ByBranchAsync(branchCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data, $"DP01 data for branch {branchCode} retrieved successfully"));
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
        [HttpGet("dp01/customer/{customerCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByCustomer(string customerCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dataPreviewService.GetDP01ByCustomerAsync(customerCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data, $"DP01 data for customer {customerCode} retrieved successfully"));
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
        [HttpGet("dp01/account/{accountNumber}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByAccountNumber(string accountNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dataPreviewService.GetDP01ByAccountNumberAsync(accountNumber, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(data, $"DP01 data for account {accountNumber} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for account {AccountNumber}", accountNumber);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 data for account", "DP01_ACCOUNT_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm dữ liệu DP01 theo nhiều tiêu chí
        /// </summary>
        [HttpGet("dp01/search")]
        [ProducesResponseType(typeof(PagedApiResponse<DP01PreviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SearchDP01(
            [FromQuery] string? keyword,
            [FromQuery] string? branchCode,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var data = await _dataPreviewService.SearchDP01Async(
                    keyword,
                    branchCode,
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

        #endregion

        // TODO: Add other data type endpoints (LN01, GL01, etc.) following the same pattern
    }
}
