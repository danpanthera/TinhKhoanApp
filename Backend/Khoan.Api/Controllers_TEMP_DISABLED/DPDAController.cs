using Khoan.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Dtos;
using Khoan.Api.Models.Dtos.DPDA;
using Khoan.Api.Services.DataServices;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// DPDA Controller - cung cấp API cho dữ liệu thẻ tiền gửi
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DPDAController : ControllerBase
    {
        private readonly IDPDADataService _dpdaDataService;
        private readonly ILogger<DPDAController> _logger;

        public DPDAController(
            IDPDADataService dpdaDataService,
            ILogger<DPDAController> logger)
        {
            _dpdaDataService = dpdaDataService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy preview dữ liệu DPDA mới nhất
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAPreview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAPreviewAsync(count);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, "DPDA preview data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA preview data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA preview data", "DPDA_PREVIEW_ERROR"));
            }
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi DPDA
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DPDADetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDADetail(int id)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDADetailAsync(id);
                if (data == null)
                    return NotFound(ApiResponse<object>.Error($"DPDA record with ID {id} not found", "DPDA_NOT_FOUND"));

                return Ok(ApiResponse<DPDADetailsDto>.Ok(data, "DPDA detail retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA detail for ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA detail", "DPDA_DETAIL_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo ngày
        /// </summary>
        [HttpGet("date")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAByDate([FromQuery] DateTime date, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAByDateAsync(date, maxResults);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, $"DPDA data for date {date:yyyy-MM-dd} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for date {Date}", date);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA data for date", "DPDA_DATE_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo chi nhánh
        /// </summary>
        [HttpGet("branch/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAByBranchAsync(branchCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, $"DPDA data for branch {branchCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for branch {BranchCode}", branchCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA data for branch", "DPDA_BRANCH_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo khách hàng
        /// </summary>
        [HttpGet("customer/{customerCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAByCustomer(string customerCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAByCustomerAsync(customerCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, $"DPDA data for customer {customerCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for customer {CustomerCode}", customerCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA data for customer", "DPDA_CUSTOMER_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo số tài khoản
        /// </summary>
        [HttpGet("account/{accountNumber}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAByAccount(string accountNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAByAccountNumberAsync(accountNumber, maxResults);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, $"DPDA data for account {accountNumber} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for account {AccountNumber}", accountNumber);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA data for account", "DPDA_ACCOUNT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo số thẻ
        /// </summary>
        [HttpGet("card/{cardNumber}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAByCardNumber(string cardNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAByCardNumberAsync(cardNumber, maxResults);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, $"DPDA data for card {cardNumber} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for card {CardNumber}", cardNumber);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA data for card", "DPDA_CARD_ERROR"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo trạng thái
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAByStatus(string status, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDAByStatusAsync(status, maxResults);
                return Ok(ApiResponse<IEnumerable<DPDAPreviewDto>>.Ok(data, $"DPDA data for status {status} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for status {Status}", status);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA data for status", "DPDA_STATUS_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp DPDA theo chi nhánh
        /// </summary>
        [HttpGet("summary/branch/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<DPDASummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDASummaryByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDASummaryByBranchAsync(branchCode, date);
                return Ok(ApiResponse<DPDASummaryDto>.Ok(data, $"DPDA summary for branch {branchCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA summary for branch {BranchCode}", branchCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA summary for branch", "DPDA_SUMMARY_BRANCH_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp DPDA theo ngày
        /// </summary>
        [HttpGet("summary/date")]
        [ProducesResponseType(typeof(ApiResponse<DPDASummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDASummaryByDate([FromQuery] DateTime date)
        {
            try
            {
                var data = await _dpdaDataService.GetDPDASummaryByDateAsync(date);
                return Ok(ApiResponse<DPDASummaryDto>.Ok(data, $"DPDA summary for date {date:yyyy-MM-dd} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA summary for date {Date}", date);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DPDA summary for date", "DPDA_SUMMARY_DATE_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm DPDA theo nhiều tiêu chí
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<ApiResponse<PagedResult<DPDAPreviewDto>>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SearchDPDA(
            [FromQuery] string? keyword = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? customerCode = null,
            [FromQuery] string? accountNumber = null,
            [FromQuery] string? cardNumber = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var data = await _dpdaDataService.SearchDPDAAsync(
                    keyword, branchCode, customerCode, accountNumber, cardNumber, status, fromDate, toDate, page, pageSize);

                return Ok(ApiResponse<ApiResponse<PagedResult<DPDAPreviewDto>>>.Ok(data, "DPDA search results retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DPDA data");
                return BadRequest(ApiResponse<object>.Error("Failed to search DPDA data", "DPDA_SEARCH_ERROR"));
            }
        }
    }
}
