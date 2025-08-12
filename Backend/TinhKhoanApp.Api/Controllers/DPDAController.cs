using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Dtos.DPDA;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// DPDA Controller - cung cấp API cho dữ liệu thẻ tiền gửi
    /// Sử dụng IDPDAService thay vì DataService cũ
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DPDAController : ControllerBase
    {
        private readonly IDPDAService _dpdaService;
        private readonly ILogger<DPDAController> _logger;

        public DPDAController(
            IDPDAService dpdaService,
            ILogger<DPDAController> logger)
        {
            _dpdaService = dpdaService;
            _logger = logger;
        }

        /// <summary>
        /// Tạo mới bản ghi DPDA
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DPDADetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Create([FromBody] DPDACreateDto dto)
        {
            var result = await _dpdaService.CreateAsync(dto);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Cập nhật bản ghi DPDA
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<DPDADetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Update([FromBody] DPDAUpdateDto dto)
        {
            var result = await _dpdaService.UpdateAsync(dto);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Xóa bản ghi DPDA theo ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _dpdaService.DeleteAsync(id);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Lấy preview dữ liệu DPDA mới nhất với paging
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<DPDAPreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDPDAPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var result = await _dpdaService.GetPreviewAsync(pageNumber, pageSize, searchTerm);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
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
                var result = await _dpdaService.GetByIdAsync(id);
                if (!result.Success || result.Data == null)
                    return NotFound(ApiResponse<object>.Error($"DPDA record with ID {id} not found", "DPDA_NOT_FOUND"));

                return Ok(result);
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
                var result = await _dpdaService.GetPreviewAsync(1, maxResults, date.ToString("yyyy-MM-dd"));
                return Ok(result);
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
                var result = await _dpdaService.GetByBranchCodeAsync(branchCode, maxResults);
                return Ok(result);
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
                var result = await _dpdaService.GetByCustomerCodeAsync(customerCode, maxResults);
                return Ok(result);
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
                var result = await _dpdaService.GetByAccountNumberAsync(accountNumber, maxResults);
                return Ok(result);
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
                var result = await _dpdaService.GetByCardNumberAsync(cardNumber, maxResults);
                return Ok(result);
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
                var result = await _dpdaService.GetPreviewAsync(1, maxResults, status);
                return Ok(result);
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
                var result = await _dpdaService.GetStatisticsAsync(date);
                return Ok(result);
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
                var result = await _dpdaService.GetStatisticsAsync(date);
                return Ok(result);
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
                // TaM THI deNG: Sed deNG searchTerm tran preview
                var result = await _dpdaService.GetPreviewAsync(page, pageSize, keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DPDA data");
                return BadRequest(ApiResponse<object>.Error("Failed to search DPDA data", "DPDA_SEARCH_ERROR"));
            }
        }
    }
}
