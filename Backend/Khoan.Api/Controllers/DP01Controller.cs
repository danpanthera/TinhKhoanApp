using Khoan.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Dtos.DP01;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Controller cho dữ liệu DP01 (Sổ tiết kiệm)
    /// Hỗ trợ CRUD operations và CSV import/export
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DP01Controller : ControllerBase
    {
        private readonly IDP01Service _dp01Service;
        private readonly ILogger<DP01Controller> _logger;

        /// <summary>
        /// Khởi tạo controller với các dependencies
        /// </summary>
        public DP01Controller(
            IDP01Service dp01Service,
            ILogger<DP01Controller> logger)
        {
            _dp01Service = dp01Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả DP01 với phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DP01PreviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetAllDP01([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 data, page: {Page}, pageSize: {PageSize}", page, pageSize);
                var result = await _dp01Service.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(result, "DP01 data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 data", "DP01_GET_ALL_ERROR"));
            }
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi DP01
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DP01DetailsDto), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01Detail(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 detail for ID {Id}", id);
                var data = await _dp01Service.GetByIdAsync(id);
                if (data == null)
                    return NotFound(ApiResponse<object>.Error($"DP01 record with ID {id} not found", "DP01_NOT_FOUND"));

                return Ok(ApiResponse<DP01DetailsDto>.Ok(data, "DP01 detail retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 detail for ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 detail", "DP01_DETAIL_ERROR"));
            }
        }

        /// <summary>
        /// Tạo mới bản ghi DP01
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(DP01DetailsDto), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateDP01([FromBody] DP01CreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.Error("Invalid input data", "DP01_INVALID_INPUT"));

                _logger.LogInformation("Creating new DP01 record");
                var result = await _dp01Service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetDP01Detail), new { id = result.Id }, ApiResponse<DP01DetailsDto>.Ok(result, "DP01 record created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating DP01 record");
                return BadRequest(ApiResponse<object>.Error("Failed to create DP01 record", "DP01_CREATE_ERROR"));
            }
        }

        /// <summary>
        /// Cập nhật bản ghi DP01
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DP01DetailsDto), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateDP01(int id, [FromBody] DP01UpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.Error("Invalid input data", "DP01_INVALID_INPUT"));

                _logger.LogInformation("Updating DP01 record with ID {Id}", id);
                var result = await _dp01Service.UpdateAsync(id, updateDto);
                if (result == null)
                    return NotFound(ApiResponse<object>.Error($"DP01 record with ID {id} not found", "DP01_NOT_FOUND"));

                return Ok(ApiResponse<DP01DetailsDto>.Ok(result, "DP01 record updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating DP01 record with ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to update DP01 record", "DP01_UPDATE_ERROR"));
            }
        }

        /// <summary>
        /// Xóa bản ghi DP01
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> DeleteDP01(int id)
        {
            try
            {
                _logger.LogInformation("Deleting DP01 record with ID {Id}", id);
                var result = await _dp01Service.DeleteAsync(id);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"DP01 record with ID {id} not found", "DP01_NOT_FOUND"));

                return Ok(ApiResponse<bool>.Ok(result, "DP01 record deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting DP01 record with ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to delete DP01 record", "DP01_DELETE_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm DP01 theo mã chi nhánh
        /// </summary>
        [HttpGet("search/branch/{branchCode}")]
        [ProducesResponseType(typeof(IEnumerable<DP01PreviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Searching DP01 by branch {BranchCode}, maxResults: {MaxResults}",
                    branchCode, maxResults);
                var result = await _dp01Service.GetByBranchCodeAsync(branchCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(result, $"DP01 data for branch {branchCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DP01 by branch {BranchCode}", branchCode);
                return BadRequest(ApiResponse<object>.Error("Failed to search DP01 by branch", "DP01_SEARCH_BRANCH_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm DP01 theo mã khách hàng
        /// </summary>
        [HttpGet("search/customer/{customerCode}")]
        [ProducesResponseType(typeof(IEnumerable<DP01PreviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByCustomer(string customerCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Searching DP01 by customer {CustomerCode}, maxResults: {MaxResults}",
                    customerCode, maxResults);
                var result = await _dp01Service.GetByCustomerCodeAsync(customerCode, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(result, $"DP01 data for customer {customerCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DP01 by customer {CustomerCode}", customerCode);
                return BadRequest(ApiResponse<object>.Error("Failed to search DP01 by customer", "DP01_SEARCH_CUSTOMER_ERROR"));
            }
        }

        /// <summary>
        /// Tìm kiếm DP01 theo số tài khoản
        /// </summary>
        [HttpGet("search/account/{accountNumber}")]
        [ProducesResponseType(typeof(IEnumerable<DP01PreviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01ByAccount(string accountNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Searching DP01 by account {AccountNumber}, maxResults: {MaxResults}",
                    accountNumber, maxResults);
                var result = await _dp01Service.GetByAccountNumberAsync(accountNumber, maxResults);
                return Ok(ApiResponse<IEnumerable<DP01PreviewDto>>.Ok(result, $"DP01 data for account {accountNumber} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DP01 by account {AccountNumber}", accountNumber);
                return BadRequest(ApiResponse<object>.Error("Failed to search DP01 by account", "DP01_SEARCH_ACCOUNT_ERROR"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan DP01
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(DP01SummaryDto), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetDP01Statistics([FromQuery] DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Retrieving DP01 statistics for date: {Date}", date);
                var result = await _dp01Service.GetStatisticsAsync(date);
                return Ok(ApiResponse<DP01SummaryDto>.Ok(result, "DP01 statistics retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 statistics");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve DP01 statistics", "DP01_STATISTICS_ERROR"));
            }
        }

        /// <summary>
        /// Lấy tổng số dư theo chi nhánh
        /// </summary>
        [HttpGet("analytics/branch/{branchCode}/balance")]
        [ProducesResponseType(typeof(decimal), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetBranchTotalBalance(string branchCode, [FromQuery] DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Retrieving total balance for branch {BranchCode}, date: {Date}", branchCode, date);
                var result = await _dp01Service.GetTotalBalanceByBranchAsync(branchCode, date);
                return Ok(ApiResponse<decimal>.Ok(result, $"Total balance for branch {branchCode} retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total balance for branch {BranchCode}", branchCode);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve branch total balance", "DP01_BRANCH_BALANCE_ERROR"));
            }
        }

        /// <summary>
        /// Lấy tổng số lượng bản ghi
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetTotalCount()
        {
            try
            {
                _logger.LogInformation("Retrieving total DP01 record count");
                var result = await _dp01Service.GetTotalCountAsync();
                return Ok(ApiResponse<int>.Ok(result, "Total DP01 record count retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total DP01 record count");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve total record count", "DP01_COUNT_ERROR"));
            }
        }
    }
}
