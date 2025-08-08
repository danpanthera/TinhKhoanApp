using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.EI01;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EI01Controller : ControllerBase
    {
        private readonly IEI01DataService _service;
        private readonly ILogger<EI01Controller> _logger;

        public EI01Controller(IEI01DataService service, ILogger<EI01Controller> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy preview dữ liệu EI01 - Ngân hàng điện tử
        /// </summary>
        [HttpGet("preview")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01Preview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _service.GetEI01PreviewAsync(count);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy preview dữ liệu EI01");
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy chi tiết bản ghi EI01 theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EI01DetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01Detail(long id)
        {
            try
            {
                var data = await _service.GetEI01DetailAsync(id);
                if (data == null)
                {
                    return NotFound($"Không tìm thấy bản ghi EI01 với ID: {id}");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết bản ghi EI01 với ID {Id}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        [HttpGet("by-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByDate([FromQuery] DateTime date, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _service.GetEI01ByDateAsync(date, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo ngày {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo chi nhánh
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
            {
                return BadRequest("Mã chi nhánh không được để trống");
            }

            try
            {
                var data = await _service.GetEI01ByBranchAsync(branchCode, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo chi nhánh {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo khách hàng
        /// </summary>
        [HttpGet("by-customer/{customerCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByCustomer(string customerCode, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(customerCode))
            {
                return BadRequest("Mã khách hàng không được để trống");
            }

            try
            {
                var data = await _service.GetEI01ByCustomerAsync(customerCode, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo mã khách hàng {CustomerCode}", customerCode);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại
        /// </summary>
        [HttpGet("by-phone/{phoneNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByPhoneNumber(string phoneNumber, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest("Số điện thoại không được để trống");
            }

            try
            {
                var data = await _service.GetEI01ByPhoneNumberAsync(phoneNumber, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo số điện thoại {PhoneNumber}", phoneNumber);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo loại khách hàng
        /// </summary>
        [HttpGet("by-customer-type/{customerType}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByCustomerType(string customerType, [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(customerType))
            {
                return BadRequest("Loại khách hàng không được để trống");
            }

            try
            {
                var data = await _service.GetEI01ByCustomerTypeAsync(customerType, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo loại khách hàng {CustomerType}", customerType);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ
        /// </summary>
        [HttpGet("by-service-status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByServiceStatus(
            [FromQuery] string serviceType,
            [FromQuery] string status,
            [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(serviceType) || string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Loại dịch vụ và trạng thái không được để trống");
            }

            try
            {
                var data = await _service.GetEI01ByServiceStatusAsync(serviceType, status, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo trạng thái dịch vụ {ServiceType} - {Status}", serviceType, status);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo khoảng thời gian đăng ký
        /// </summary>
        [HttpGet("by-registration-date-range")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EI01PreviewDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01ByRegistrationDateRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] string serviceType,
            [FromQuery] int maxResults = 100)
        {
            if (string.IsNullOrWhiteSpace(serviceType))
            {
                return BadRequest("Loại dịch vụ không được để trống");
            }

            if (fromDate > toDate)
            {
                return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
            }

            try
            {
                var data = await _service.GetEI01ByRegistrationDateRangeAsync(fromDate, toDate, serviceType, maxResults);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo khoảng thời gian đăng ký {FromDate} - {ToDate}, dịch vụ {ServiceType}",
                    fromDate, toDate, serviceType);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp EI01 theo chi nhánh
        /// </summary>
        [HttpGet("summary/by-branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EI01SummaryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01SummaryByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
            {
                return BadRequest("Mã chi nhánh không được để trống");
            }

            try
            {
                var data = await _service.GetEI01SummaryByBranchAsync(branchCode, date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tổng hợp EI01 theo chi nhánh {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Lấy thống kê tổng hợp EI01 theo ngày
        /// </summary>
        [HttpGet("summary/by-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EI01SummaryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEI01SummaryByDate([FromQuery] DateTime date)
        {
            try
            {
                var data = await _service.GetEI01SummaryByDateAsync(date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tổng hợp EI01 theo ngày {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Tìm kiếm EI01 theo nhiều tiêu chí
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResult<EI01PreviewDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchEI01(
            [FromQuery] string? keyword = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? customerCode = null,
            [FromQuery] string? customerType = null,
            [FromQuery] string? phoneNumber = null,
            [FromQuery] string? serviceType = null,
            [FromQuery] string? serviceStatus = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var data = await _service.SearchEI01Async(
                    keyword,
                    branchCode,
                    customerCode,
                    customerType,
                    phoneNumber,
                    serviceType,
                    serviceStatus,
                    fromDate,
                    toDate,
                    page,
                    pageSize);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm dữ liệu EI01");
                return StatusCode(500, "Đã xảy ra lỗi khi tìm kiếm dữ liệu. Vui lòng thử lại sau.");
            }
        }
    }
}
