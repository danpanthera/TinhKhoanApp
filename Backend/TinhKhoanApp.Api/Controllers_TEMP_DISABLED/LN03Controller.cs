using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs.LN03;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// API Controller for LN03 (Loan Recovery Data) - Phase 2B
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LN03Controller : ControllerBase
    {
        private readonly ILN03Service _ln03Service;
        private readonly ILogger<LN03Controller> _logger;

        /// <summary>
        /// Initialize LN03 controller
        /// </summary>
        public LN03Controller(
            ILN03Service ln03Service,
            ILogger<LN03Controller> logger)
        {
            _ln03Service = ln03Service;
            _logger = logger;
        }

        /// <summary>
        /// Get all LN03 records (Loan Recovery Data)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LN03PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _ln03Service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all LN03 data");
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Get LN03 record by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LN03DetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _ln03Service.GetByIdAsync(id);
                if (result.IsSuccess && result.Data != null)
                {
                    return Ok(result);
                }
                return NotFound(ApiResponse<string>.Error($"LN03 record with ID {id} not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 record with ID: {Id}", id);
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Create new LN03 record
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LN03DetailsDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> Create([FromBody] LN03CreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.Error("Invalid model state"));
                }

                var result = await _ln03Service.CreateAsync(createDto);
                if (result.IsSuccess)
                {
                    return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating LN03 record");
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Update LN03 record
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LN03DetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> Update(long id, [FromBody] LN03UpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.Error("Invalid model state"));
                }

                var result = await _ln03Service.UpdateAsync(id, updateDto);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LN03 record with ID: {Id}", id);
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Delete LN03 record
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _ln03Service.DeleteAsync(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LN03 record with ID: {Id}", id);
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }



        /// <summary>
        /// Lấy dữ liệu LN03 gần đây
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetRecent([FromQuery] int count = 10)
        {
            try
            {
                var result = await _ln03Service.GetRecentAsync(count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 gần đây");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo ngày
        /// </summary>
        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetByDate(
            [FromQuery] DateTime date,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByDateAsync(date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo ngày: {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetByDateRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                }

                var result = await _ln03Service.GetByDateAsync(fromDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo khoảng thời gian: {FromDate} - {ToDate}",
                    fromDate, toDate);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã chi nhánh
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetByBranchCode(
            string branchCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByBranchAsync(branchCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo mã chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã khách hàng
        /// </summary>
        [HttpGet("by-customer/{customerCode}")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetByCustomerCode(
            string customerCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByCustomerAsync(customerCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo mã khách hàng: {CustomerCode}", customerCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /*
        /// <summary>
        /// Lấy dữ liệu LN03 theo mã cán bộ tín dụng - TEMPORARILY DISABLED
        /// </summary>
        [HttpGet("by-officer/{officerCode}")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetByOfficerCode(
            string officerCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByOfficerCodeAsync(officerCode, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo mã cán bộ tín dụng: {OfficerCode}", officerCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }
        */

        /// <summary>
        /// Lấy dữ liệu LN03 theo nhóm nợ
        /// </summary>
        [HttpGet("by-debt-group/{debtGroup}")]
        public async Task<ActionResult<IEnumerable<LN03PreviewDto>>> GetByDebtGroup(
            string debtGroup,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByDebtGroupAsync(debtGroup);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo nhóm nợ: {DebtGroup}", debtGroup);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy tổng số tiền xử lý rủi ro theo chi nhánh
        /// </summary>
        [HttpGet("total-risk-amount/branch/{branchCode}")]
        public async Task<ActionResult<decimal>> GetTotalRiskAmountByBranch(
            string branchCode,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _ln03Service.GetTotalRiskAmountByBranchAsync(branchCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng số tiền xử lý rủi ro theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy tổng thu nợ sau xử lý theo chi nhánh
        /// </summary>
        [HttpGet("total-debt-recovery/branch/{branchCode}")]
        public async Task<ActionResult<decimal>> GetTotalDebtRecoveryByBranch(
            string branchCode,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _ln03Service.GetTotalDebtRecoveryByBranchAsync(branchCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng thu nợ sau xử lý theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }







        /// <summary>
        /// Import dữ liệu LN03 từ file CSV
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> ImportLN03File(IFormFile file, [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Không có file được tải lên");
                }

                // Kiểm tra tên file có chứa ln03 không
                if (!file.FileName.ToLower().Contains("ln03"))
                {
                    return BadRequest("File không phải là file LN03. Tên file phải chứa 'ln03'");
                }

                // Sử dụng DirectImportService với phương thức cải tiến để import dữ liệu
                // DISABLED: var result = await _directImportService.ImportLN03EnhancedAsync(file, statementDate);
                // Temporary placeholder - service method doesn't exist yet
                var result = await _ln03Service.ImportCsvAsync(file); // Use actual service method instead

                if (result.IsSuccess && result.Data != null)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = $"Import thành công {result.Data.ProcessedRecords} bản ghi",
                        FileName = result.Data.FileName ?? "Unknown",
                        RecordsCount = result.Data.ProcessedRecords,
                        NgayDL = statementDate ?? DateTime.Now.ToString("yyyy-MM-dd"),
                        BatchId = Guid.NewGuid().ToString()
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message ?? "Import failed",
                        Errors = result.Data?.Errors ?? new List<string> { "Unknown error occurred" }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import file LN03");
                return StatusCode(500, $"Đã xảy ra lỗi khi xử lý yêu cầu: {ex.Message}");
            }
        }

        // === BUSINESS-SPECIFIC ENDPOINTS ===

        /// <summary>
        /// Get loans by branch code
        /// </summary>
        [HttpGet("branch/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LN03PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetByBranch(string branchCode)
        {
            try
            {
                var result = await _ln03Service.GetByBranchAsync(branchCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 records for branch: {BranchCode}", branchCode);
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Get loans by account number
        /// </summary>
        [HttpGet("account/{accountNumber}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LN03PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetByAccount(string accountNumber)
        {
            try
            {
                var result = await _ln03Service.GetByAccountAsync(accountNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 records for account: {AccountNumber}", accountNumber);
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Get processing summary for loan recovery
        /// </summary>
        [HttpGet("summary/processing")]
        [ProducesResponseType(typeof(ApiResponse<LN03SummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetProcessingSummary()
        {
            try
            {
                var result = await _ln03Service.GetProcessingSummaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 processing summary");
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }

        /// <summary>
        /// Get overdue contracts
        /// </summary>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LN03PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetOverdueContracts()
        {
            try
            {
                var result = await _ln03Service.GetOverdueContractsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue contracts");
                return StatusCode(500, ApiResponse<string>.Error("Internal server error occurred"));
            }
        }
    }
}
