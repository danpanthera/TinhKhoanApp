using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.DTOs.LN01;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// LN01 Controller - API endpoints cho bảng LN01 (Loan Data)
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
        /// Lấy preview LN01 (danh sách tóm tắt)
        /// </summary>
        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] int take = 20)
        {
            var response = await _service.PreviewAsync(take);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy chi tiết LN01 theo ID
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Tạo mới LN01
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LN01CreateDto createDto)
        {
            var response = await _service.CreateAsync(createDto);
            return StatusCode(response.StatusCode ?? 201, response);
        }

        /// <summary>
        /// Cập nhật LN01
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] LN01UpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(ApiResponse<object>.Error("ID không khớp", "ID_MISMATCH"));
            }

            var response = await _service.UpdateAsync(updateDto);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Xóa LN01
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy LN01 theo ngày dữ liệu
        /// </summary>
        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByDateAsync(date, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy LN01 theo mã chi nhánh
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        public async Task<IActionResult> GetByBranchCode(string branchCode, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByBranchCodeAsync(branchCode, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy LN01 theo mã khách hàng
        /// </summary>
        [HttpGet("by-customer/{customerCode}")]
        public async Task<IActionResult> GetByCustomerCode(string customerCode, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByCustomerCodeAsync(customerCode, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy LN01 theo số tài khoản
        /// </summary>
        [HttpGet("by-account/{accountNumber}")]
        public async Task<IActionResult> GetByAccountNumber(string accountNumber, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByAccountNumberAsync(accountNumber, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy LN01 theo nhóm nợ
        /// </summary>
        [HttpGet("by-debt-group/{debtGroup}")]
        public async Task<IActionResult> GetByDebtGroup(string debtGroup, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByDebtGroupAsync(debtGroup, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy LN01 trong khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByDateRangeAsync(fromDate, toDate, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy thống kê tổng quan LN01
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            var response = await _service.GetSummaryAsync(fromDate, toDate);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy tổng số tiền vay theo chi nhánh
        /// </summary>
        [HttpGet("total-loan-amount/branch/{branchCode}")]
        public async Task<IActionResult> GetTotalLoanAmountByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            var response = await _service.GetTotalLoanAmountByBranchAsync(branchCode, date);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy tổng số tiền giải ngân theo ngày
        /// </summary>
        [HttpGet("total-disbursement/{date}")]
        public async Task<IActionResult> GetTotalDisbursementByDate(DateTime date)
        {
            var response = await _service.GetTotalDisbursementByDateAsync(date);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy số lượng khoản vay theo nhóm nợ
        /// </summary>
        [HttpGet("loan-count-by-debt-group")]
        public async Task<IActionResult> GetLoanCountByDebtGroup([FromQuery] DateTime? date = null)
        {
            var response = await _service.GetLoanCountByDebtGroupAsync(date);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy top customers có dư nợ cao nhất
        /// </summary>
        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomersByLoanAmount([FromQuery] int topCount = 10, [FromQuery] DateTime? date = null)
        {
            var response = await _service.GetTopCustomersByLoanAmountAsync(topCount, date);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
