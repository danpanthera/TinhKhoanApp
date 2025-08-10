using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models.DTOs.DP01;
using System.Net;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// DP01 Controller - API endpoints cho bảng DP01 (Tiền gửi có kỳ hạn)
    /// Xử lý 63 business columns theo CSV structure
    /// Business columns từ CSV là single source of truth
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DP01Controller : ControllerBase
    {
        private readonly IDP01Service _dp01Service;
        private readonly ILogger<DP01Controller> _logger;

        public DP01Controller(IDP01Service dp01Service, ILogger<DP01Controller> logger)
        {
            _dp01Service = dp01Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả DP01 records với paging
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DP01PreviewDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            try
            {
                _logger.LogInformation("Getting DP01 records - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

                var records = await _dp01Service.GetAllAsync(pageNumber, pageSize);
                var totalCount = await _dp01Service.GetTotalCountAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 records");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy DP01 theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DP01DetailsDto>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting DP01 record by ID: {Id}", id);

                var record = await _dp01Service.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"DP01 record with ID {id} not found");
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 record by ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy DP01 records gần nhất
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<DP01PreviewDto>>> GetRecent([FromQuery] int count = 10)
        {
            try
            {
                _logger.LogInformation("Getting recent DP01 records - Count: {Count}", count);

                var records = await _dp01Service.GetRecentAsync(count);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent DP01 records");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy DP01 theo ngày dữ liệu (NGAY_DL)
        /// </summary>
        [HttpGet("by-date/{date}")]
        public async Task<ActionResult<IEnumerable<DP01PreviewDto>>> GetByDate(DateTime date)
        {
            try
            {
                _logger.LogInformation("Getting DP01 records by date: {Date}", date);

                var records = await _dp01Service.GetByDateAsync(date);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 records by date: {Date}", date);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy DP01 theo mã chi nhánh (MA_CN) - Business column từ CSV
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        public async Task<ActionResult<IEnumerable<DP01PreviewDto>>> GetByBranchCode(string branchCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Getting DP01 records by branch code: {BranchCode}", branchCode);

                var records = await _dp01Service.GetByBranchCodeAsync(branchCode, maxResults);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 records by branch code: {BranchCode}", branchCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy DP01 theo mã khách hàng (MA_KH) - Business column từ CSV
        /// </summary>
        [HttpGet("by-customer/{customerCode}")]
        public async Task<ActionResult<IEnumerable<DP01PreviewDto>>> GetByCustomerCode(string customerCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Getting DP01 records by customer code: {CustomerCode}", customerCode);

                var records = await _dp01Service.GetByCustomerCodeAsync(customerCode, maxResults);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 records by customer code: {CustomerCode}", customerCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy DP01 theo số tài khoản (SO_TAI_KHOAN) - Business column từ CSV
        /// </summary>
        [HttpGet("by-account/{accountNumber}")]
        public async Task<ActionResult<IEnumerable<DP01PreviewDto>>> GetByAccountNumber(string accountNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                _logger.LogInformation("Getting DP01 records by account number: {AccountNumber}", accountNumber);

                var records = await _dp01Service.GetByAccountNumberAsync(accountNumber, maxResults);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 records by account number: {AccountNumber}", accountNumber);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy tổng số dư theo chi nhánh (CURRENT_BALANCE) - Business column từ CSV
        /// </summary>
        [HttpGet("total-balance/{branchCode}")]
        public async Task<ActionResult<decimal>> GetTotalBalanceByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Getting total balance for branch: {BranchCode}, Date: {Date}", branchCode, date);

                var totalBalance = await _dp01Service.GetTotalBalanceByBranchAsync(branchCode, date);
                return Ok(totalBalance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total balance for branch: {BranchCode}", branchCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan DP01
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<DP01SummaryDto>> GetStatistics([FromQuery] DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Getting DP01 statistics for date: {Date}", date);

                var statistics = await _dp01Service.GetStatisticsAsync(date);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tạo DP01 record mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DP01DetailsDto>> Create([FromBody] DP01CreateDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new DP01 record for customer: {CustomerCode}", createDto.MA_KH);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdRecord = await _dp01Service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, createdRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating DP01 record");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cập nhật DP01 record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DP01DetailsDto>> Update(int id, [FromBody] DP01UpdateDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating DP01 record ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedRecord = await _dp01Service.UpdateAsync(id, updateDto);
                if (updatedRecord == null)
                {
                    return NotFound($"DP01 record with ID {id} not found");
                }

                return Ok(updatedRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating DP01 record ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Xóa DP01 record
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting DP01 record ID: {Id}", id);

                var isDeleted = await _dp01Service.DeleteAsync(id);
                if (!isDeleted)
                {
                    return NotFound($"DP01 record with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting DP01 record ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
