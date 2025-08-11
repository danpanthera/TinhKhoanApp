using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Dtos.GL02;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GL02Controller : ControllerBase
    {
        private readonly IGL02DataService _service;
        private readonly ILogger<GL02Controller> _logger;

        public GL02Controller(IGL02DataService service, ILogger<GL02Controller> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy preview dữ liệu GL02 - Giao dịch sổ cái
        /// </summary>
        [HttpGet("preview")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02Preview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _service.GetGL02PreviewAsync(count);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy preview dữ liệu GL02");
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy chi tiết bản ghi GL02 theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GL02DetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02Detail(long id)
        {
            try
            {
                var data = await _service.GetGL02DetailAsync(id);
                if (data == null)
                {
                    return NotFound($"Không tìm thấy bản ghi GL02 với ID: {id}");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết bản ghi GL02 với ID {Id}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo ngày
        /// </summary>
        [HttpGet("by-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByDate([FromQuery] DateTime date, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _service.GetGL02ByDateAsync(date, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo ngày {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByDateRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int maxResults = 100)
        {
            if (fromDate > toDate)
            {
                return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
            }

            try
            {
                var data = await _service.GetGL02ByDateRangeAsync(fromDate, toDate, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo khoảng thời gian {FromDate} - {ToDate}", fromDate, toDate);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo chi nhánh
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
            {
                return BadRequest("Mã chi nhánh không được để trống");
            }

            try
            {
                var data = await _service.GetGL02ByBranchCodeAsync(branchCode, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo chi nhánh {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo đơn vị
        /// </summary>
        [HttpGet("by-unit/{unit}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByUnit(string unit, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(unit))
            {
                return BadRequest("Mã đơn vị không được để trống");
            }

            try
            {
                var data = await _service.GetGL02ByUnitAsync(unit, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo đơn vị {Unit}", unit);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo tài khoản
        /// </summary>
        [HttpGet("by-account/{accountCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByAccount(string accountCode, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(accountCode))
            {
                return BadRequest("Mã tài khoản không được để trống");
            }

            try
            {
                var data = await _service.GetGL02ByAccountCodeAsync(accountCode, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo tài khoản {AccountCode}", accountCode);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo khách hàng
        /// </summary>
        [HttpGet("by-customer/{customer}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByCustomer(string customer, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(customer))
            {
                return BadRequest("Mã khách hàng không được để trống");
            }

            try
            {
                var data = await _service.GetGL02ByCustomerAsync(customer, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo khách hàng {Customer}", customer);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu GL02 theo loại giao dịch
        /// </summary>
        [HttpGet("by-transaction-type/{transactionType}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GL02PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02ByTransactionType(string transactionType, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(transactionType))
            {
                return BadRequest("Loại giao dịch không được để trống");
            }

            try
            {
                var data = await _service.GetGL02ByTransactionTypeAsync(transactionType, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo loại giao dịch {TransactionType}", transactionType);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy tổng hợp GL02 theo ngày
        /// </summary>
        [HttpGet("summary/by-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GL02SummaryDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02SummaryByDate([FromQuery] DateTime date)
        {
            try
            {
                var data = await _service.GetGL02SummaryByDateAsync(date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng hợp GL02 theo ngày {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy tổng hợp GL02 theo chi nhánh
        /// </summary>
        [HttpGet("summary/by-branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GL02SummaryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02SummaryByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
            {
                return BadRequest("Mã chi nhánh không được để trống");
            }

            try
            {
                var data = await _service.GetGL02SummaryByBranchAsync(branchCode, date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng hợp GL02 theo chi nhánh {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy tổng hợp GL02 theo đơn vị
        /// </summary>
        [HttpGet("summary/by-unit/{unit}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GL02SummaryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGL02SummaryByUnit(string unit, [FromQuery] DateTime? date = null)
        {
            if (string.IsNullOrWhiteSpace(unit))
            {
                return BadRequest("Mã đơn vị không được để trống");
            }

            try
            {
                var data = await _service.GetGL02SummaryByUnitAsync(unit, date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng hợp GL02 theo đơn vị {Unit}", unit);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Tìm kiếm GL02 theo nhiều tiêu chí
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResult<GL02PreviewDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchGL02(
            [FromQuery] string? keyword = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? unit = null,
            [FromQuery] string? accountCode = null,
            [FromQuery] string? customer = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var data = await _service.SearchGL02Async(
                    keyword,
                    branchCode,
                    unit,
                    accountCode,
                    customer,
                    fromDate,
                    toDate,
                    transactionType,
                    page,
                    pageSize);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm dữ liệu GL02");
                return StatusCode(500, "Đã xảy ra lỗi khi tìm kiếm dữ liệu. Vui lòng thử lại sau.");
            }
        }
    }
}
