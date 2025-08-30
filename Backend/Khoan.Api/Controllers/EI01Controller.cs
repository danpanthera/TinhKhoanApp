using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TinhKhoanApp.Api.Models.Dtos.EI01;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// EI01 Controller - E-Banking Information API endpoints
    /// Following DP01Controller and DPDAController pattern
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EI01Controller : ControllerBase
    {
        private readonly IEI01Service _service;
        private readonly ILogger<EI01Controller> _logger;

        public EI01Controller(IEI01Service service, ILogger<EI01Controller> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // === BASIC CRUD ENDPOINTS ===

        /// <summary>
        /// Lấy preview dữ liệu EI01 - E-Banking Information
        /// </summary>
        /// <param name="count">Số lượng bản ghi trả về</param>
        /// <returns>Danh sách preview EI01</returns>
        [HttpGet("preview")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<EI01PreviewDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EI01PreviewDto>>>> GetEI01Preview([FromQuery] int count = 10)
        {
            try
            {
                var data = await _service.GetEI01PreviewAsync(count);
                return Ok(new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = true,
                    Message = "Lấy dữ liệu EI01 thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy preview dữ liệu EI01");
                return StatusCode(500, new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết bản ghi EI01 theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi EI01</param>
        /// <returns>Chi tiết EI01</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<EI01DetailsDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<EI01DetailsDto>>> GetEI01Detail(long id)
        {
            try
            {
                var data = await _service.GetEI01DetailAsync(id);
                if (data == null)
                {
                    return NotFound(new ApiResponse<EI01DetailsDto>
                    {
                        Success = false,
                        Message = $"Không tìm thấy bản ghi EI01 với ID: {id}"
                    });
                }

                return Ok(new ApiResponse<EI01DetailsDto>
                {
                    Success = true,
                    Message = "Lấy chi tiết EI01 thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết EI01 với ID: {Id}", id);
                return StatusCode(500, new ApiResponse<EI01DetailsDto>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Tạo mới bản ghi EI01
        /// </summary>
        /// <param name="createDto">Dữ liệu tạo mới EI01</param>
        /// <returns>ID của bản ghi mới tạo</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<long>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<long>>> CreateEI01([FromBody] EI01CreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<long>
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    });
                }

                var id = await _service.CreateEI01Async(createDto);
                return CreatedAtAction(nameof(GetEI01Detail), new { id }, new ApiResponse<long>
                {
                    Success = true,
                    Message = "Tạo mới EI01 thành công",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới EI01");
                return StatusCode(500, new ApiResponse<long>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tạo mới dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Cập nhật bản ghi EI01
        /// </summary>
        /// <param name="id">ID của bản ghi cần cập nhật</param>
        /// <param name="updateDto">Dữ liệu cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateEI01(long id, [FromBody] EI01UpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    });
                }

                var success = await _service.UpdateEI01Async(id, updateDto);
                if (!success)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"Không tìm thấy bản ghi EI01 với ID: {id}"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Cập nhật EI01 thành công",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật EI01 với ID: {Id}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Xóa bản ghi EI01
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEI01(long id)
        {
            try
            {
                var success = await _service.DeleteEI01Async(id);
                if (!success)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"Không tìm thấy bản ghi EI01 với ID: {id}"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Xóa EI01 thành công",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa EI01 với ID: {Id}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xóa dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        // === BUSINESS QUERY ENDPOINTS ===

        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        /// <param name="date">Ngày dữ liệu</param>
        /// <returns>Danh sách EI01 theo ngày</returns>
        [HttpGet("by-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<EI01PreviewDto>>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<EI01PreviewDto>>>> GetEI01ByDate([FromQuery] DateTime date)
        {
            try
            {
                var data = await _service.GetEI01ByDateAsync(date);
                return Ok(new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = true,
                    Message = $"Lấy dữ liệu EI01 theo ngày {date:dd/MM/yyyy} thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy EI01 theo ngày: {Date}", date);
                return StatusCode(500, new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng bản ghi tối đa</param>
        /// <returns>Danh sách EI01 theo chi nhánh</returns>
        [HttpGet("by-branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<EI01PreviewDto>>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<EI01PreviewDto>>>> GetEI01ByBranch(
            string branchCode, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _service.GetEI01ByBranchAsync(branchCode, maxResults);
                return Ok(new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = true,
                    Message = $"Lấy dữ liệu EI01 chi nhánh {branchCode} thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy EI01 theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại</param>
        /// <param name="maxResults">Số lượng bản ghi tối đa</param>
        /// <returns>Danh sách EI01 theo số điện thoại</returns>
        [HttpGet("by-phone/{phoneNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<EI01PreviewDto>>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<EI01PreviewDto>>>> GetEI01ByPhone(
            string phoneNumber, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _service.GetEI01ByPhoneNumberAsync(phoneNumber, maxResults);
                return Ok(new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = true,
                    Message = $"Lấy dữ liệu EI01 theo SĐT {phoneNumber} thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy EI01 theo SĐT: {PhoneNumber}", phoneNumber);
                return StatusCode(500, new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        // === E-BANKING SERVICE ENDPOINTS ===

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ EMB
        /// </summary>
        /// <param name="status">Trạng thái (0: Inactive, 1: Active)</param>
        /// <param name="maxResults">Số lượng bản ghi tối đa</param>
        /// <returns>Danh sách EI01 theo trạng thái EMB</returns>
        [HttpGet("by-emb-status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<EI01PreviewDto>>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<EI01PreviewDto>>>> GetEI01ByEMBStatus(
            string status, [FromQuery] int maxResults = 100)
        {
            try
            {
                var data = await _service.GetEI01ByEMBStatusAsync(status, maxResults);
                return Ok(new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = true,
                    Message = $"Lấy dữ liệu EI01 theo trạng thái EMB {status} thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy EI01 theo trạng thái EMB: {Status}", status);
                return StatusCode(500, new ApiResponse<IEnumerable<EI01PreviewDto>>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }

        /// <summary>
        /// Lấy thống kê EI01 theo chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <returns>Thống kê EI01 theo chi nhánh</returns>
        [HttpGet("summary/branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<EI01SummaryDto>))]
        public async Task<ActionResult<ApiResponse<EI01SummaryDto>>> GetEI01SummaryByBranch(string branchCode)
        {
            try
            {
                var data = await _service.GetEI01SummaryByBranchAsync(branchCode);
                return Ok(new ApiResponse<EI01SummaryDto>
                {
                    Success = true,
                    Message = $"Lấy thống kê EI01 chi nhánh {branchCode} thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê EI01 theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, new ApiResponse<EI01SummaryDto>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau."
                });
            }
        }
    }
}
