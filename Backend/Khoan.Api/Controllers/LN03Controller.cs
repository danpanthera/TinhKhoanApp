using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Dtos.LN03;
using Khoan.Api.Models.Common;
using Khoan.Api.Services.Interfaces;
using Khoan.Api.Data;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// LN03 Controller - Loan Processing Information API endpoints (Temporal Table)
    /// Temporal Table với 20 cột nghiệp vụ + lịch sử thay đổi tự động
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Tạm thời bỏ để test
    public class LN03Controller : ControllerBase
    {
        private readonly ILN03Service _service;
        private readonly ILogger<LN03Controller> _logger;
        private readonly ApplicationDbContext _context;

        public LN03Controller(ILN03Service service, ILogger<LN03Controller> logger, ApplicationDbContext context)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // === DEBUG ENDPOINT ===
        [HttpGet("debug-list")]
        public async Task<ActionResult> GetListDebug()
        {
            try
            {
                var records = await _context.LN03
                    .Where(x => !x.IS_DELETED)
                    .Take(3)
                    .Select(x => new {
                        Id = x.Id,
                        NGAY_DL = x.NGAY_DL,
                        MACHINHANH = x.MACHINHANH,
                        MAKH = x.MAKH
                    })
                    .ToListAsync();
                
                return Ok(new { 
                    Count = records.Count,
                    Data = records
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }

        // === DEBUG ENDPOINT ===
        [HttpGet("debug-first")]
        public async Task<ActionResult> GetFirstRecordDebug()
        {
            try
            {
                var firstRecord = await _context.LN03.Select(x => new {
                    Id = x.Id,
                    NGAY_DL = x.NGAY_DL,
                    MACHINHANH = x.MACHINHANH,
                    MAKH = x.MAKH
                }).FirstOrDefaultAsync();
                
                return Ok(new { 
                    Found = firstRecord != null,
                    Data = firstRecord
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }

        // === BASIC CRUD ENDPOINTS ===

        /// <summary>
        /// Lấy danh sách LN03 theo trang - Loan Processing Information
        /// </summary>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="fromDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="toDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Danh sách LN03 phân trang</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50, 
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var result = await _service.GetAllPagedAsync(page, pageSize, fromDate, toDate);
                
                if (result.Success)
                {
                    return Ok(ApiResponse<object>.Ok(new {
                        Data = result.Data.Data,
                        TotalCount = result.Data.TotalCount,
                        Page = page,
                        PageSize = pageSize
                    }, "Data retrieved successfully"));
                }
                
                return StatusCode(500, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách LN03 trang {Page}", page);
                return StatusCode(500, ApiResponse<object>.Failure("Lỗi hệ thống khi lấy danh sách LN03"));
            }
        }

        /// <summary>
        /// Lấy chi tiết bản ghi LN03 theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi LN03</param>
        /// <returns>Chi tiết LN03</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LN03DetailsDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LN03DetailsDto>>> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết LN03 với ID: {Id}", id);
                return StatusCode(500, ApiResponse<LN03DetailsDto>.Failure("Lỗi hệ thống khi lấy chi tiết LN03"));
            }
        }

        /// <summary>
        /// Tạo mới bản ghi LN03
        /// </summary>
        /// <param name="dto">Dữ liệu tạo LN03</param>
        /// <returns>LN03 đã được tạo</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<LN03DetailsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LN03DetailsDto>>> Create([FromBody] CreateLN03Dto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<LN03DetailsDto>.Error("Dữ liệu không hợp lệ", 400));
                }

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới LN03");
                return StatusCode(500, ApiResponse<LN03DetailsDto>.Failure("Lỗi hệ thống khi tạo LN03"));
            }
        }

        /// <summary>
        /// Cập nhật bản ghi LN03
        /// </summary>
        /// <param name="id">ID của bản ghi cần cập nhật</param>
        /// <param name="dto">Dữ liệu cập nhật</param>
        /// <returns>LN03 đã được cập nhật</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LN03DetailsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LN03DetailsDto>>> Update(int id, [FromBody] UpdateLN03Dto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<LN03DetailsDto>.Error("Dữ liệu không hợp lệ", 400));
                }

                var result = await _service.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật LN03 với ID: {Id}", id);
                return StatusCode(500, ApiResponse<LN03DetailsDto>.Failure("Lỗi hệ thống khi cập nhật LN03"));
            }
        }

        /// <summary>
        /// Xóa vĩnh viễn bản ghi LN03
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa LN03 với ID: {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Failure("Lỗi hệ thống khi xóa LN03"));
            }
        }

        /// <summary>
        /// Xóa mềm bản ghi LN03 (đánh dấu IS_DELETED = true)
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa mềm</param>
        /// <returns>Kết quả xóa mềm</returns>
        [HttpPatch("{id:int}/soft-delete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
        {
            try
            {
                var result = await _service.SoftDeleteAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mềm LN03 với ID: {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Failure("Lỗi hệ thống khi xóa mềm LN03"));
            }
        }

        // === TEMPORAL TABLE ENDPOINTS ===

        /// <summary>
        /// Lấy dữ liệu LN03 tại một thời điểm cụ thể (Temporal Table)
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <param name="asOfDate">Thời điểm cần xem dữ liệu</param>
        /// <returns>Dữ liệu tại thời điểm đã chỉ định</returns>
        [HttpGet("{id:int}/as-of/{asOfDate:datetime}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LN03DetailsDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LN03DetailsDto>>> GetAsOfDate(int id, DateTime asOfDate)
        {
            try
            {
                var result = await _service.GetAsOfDateAsync(id, asOfDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử LN03 ID: {Id} tại {Date}", id, asOfDate);
                return StatusCode(500, ApiResponse<LN03DetailsDto>.Failure("Lỗi hệ thống khi lấy dữ liệu lịch sử"));
            }
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử thay đổi của một bản ghi LN03 (Temporal Table)
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Danh sách lịch sử thay đổi</returns>
        [HttpGet("{id:int}/history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<LN03DetailsDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN03DetailsDto>>>> GetHistory(int id)
        {
            try
            {
                var result = await _service.GetHistoryAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử LN03 với ID: {Id}", id);
                return StatusCode(500, ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Lỗi hệ thống khi lấy lịch sử"));
            }
        }

        /// <summary>
        /// Lấy tất cả bản ghi đã thay đổi trong khoảng thời gian (Temporal Table)
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Danh sách bản ghi đã thay đổi</returns>
        [HttpGet("changed-between")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<LN03DetailsDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN03DetailsDto>>>> GetChangedBetween(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(ApiResponse<IEnumerable<LN03DetailsDto>>.Error("Ngày bắt đầu không thể lớn hơn ngày kết thúc", 400));
                }

                var result = await _service.GetChangedBetweenAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thay đổi LN03 từ {Start} đến {End}", startDate, endDate);
                return StatusCode(500, ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Lỗi hệ thống khi lấy dữ liệu thay đổi"));
            }
        }

        // === BUSINESS LOGIC ENDPOINTS ===

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <returns>Danh sách LN03 theo chi nhánh</returns>
        [HttpGet("by-branch/{branchCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<LN03DetailsDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN03DetailsDto>>>> GetByBranchCode(string branchCode)
        {
            try
            {
                var result = await _service.GetByBranchCodeAsync(branchCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy LN03 theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Lỗi hệ thống khi lấy dữ liệu theo chi nhánh"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã khách hàng
        /// </summary>
        /// <param name="customerCode">Mã khách hàng</param>
        /// <returns>Danh sách LN03 theo khách hàng</returns>
        [HttpGet("by-customer/{customerCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<LN03DetailsDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN03DetailsDto>>>> GetByCustomerCode(string customerCode)
        {
            try
            {
                var result = await _service.GetByCustomerCodeAsync(customerCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy LN03 theo khách hàng: {CustomerCode}", customerCode);
                return StatusCode(500, ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Lỗi hệ thống khi lấy dữ liệu theo khách hàng"));
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo ngày
        /// </summary>
        /// <param name="date">Ngày cần lấy dữ liệu</param>
        /// <returns>Danh sách LN03 theo ngày</returns>
        [HttpGet("by-date/{date:datetime}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<LN03DetailsDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<LN03DetailsDto>>>> GetByDate(DateTime date)
        {
            try
            {
                var result = await _service.GetByDateAsync(date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy LN03 theo ngày: {Date}", date);
                return StatusCode(500, ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Lỗi hệ thống khi lấy dữ liệu theo ngày"));
            }
        }

        // === ANALYTICS & REPORTING ENDPOINTS ===

        /// <summary>
        /// Lấy tóm tắt dữ liệu LN03
        /// </summary>
        /// <param name="fromDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="toDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Tóm tắt thống kê</returns>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LN03SummaryDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LN03SummaryDto>>> GetSummary(
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var result = await _service.GetSummaryAsync(fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tóm tắt LN03");
                return StatusCode(500, ApiResponse<LN03SummaryDto>.Failure("Lỗi hệ thống khi lấy tóm tắt"));
            }
        }

        // === DATA MANAGEMENT ENDPOINTS ===

        /// <summary>
        /// Xóa toàn bộ dữ liệu LN03 (chỉ dành cho admin)
        /// </summary>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("truncate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> TruncateData()
        {
            try
            {
                var result = await _service.TruncateDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa toàn bộ dữ liệu LN03");
                return StatusCode(500, ApiResponse<bool>.Failure("Lỗi hệ thống khi xóa dữ liệu"));
            }
        }

        /// <summary>
        /// Lấy số lượng bản ghi LN03
        /// </summary>
        /// <param name="fromDate">Ngày bắt đầu (tùy chọn)</param>
        /// <param name="toDate">Ngày kết thúc (tùy chọn)</param>
        /// <returns>Số lượng bản ghi</returns>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<int>>> GetRecordCount(
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var result = await _service.GetRecordCountAsync(fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm bản ghi LN03");
                return StatusCode(500, ApiResponse<int>.Failure("Lỗi hệ thống khi đếm bản ghi"));
            }
        }

        /// <summary>
        /// Kiểm tra tính toàn vẹn dữ liệu LN03
        /// </summary>
        /// <returns>Danh sách các vấn đề được tìm thấy</returns>
        [HttpGet("validate-integrity")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> ValidateDataIntegrity()
        {
            try
            {
                var result = await _service.ValidateDataIntegrityAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra tính toàn vẹn LN03");
                return StatusCode(500, ApiResponse<IEnumerable<string>>.Failure("Lỗi hệ thống khi kiểm tra tính toàn vẹn"));
            }
        }

        /// <summary>
        /// Lấy cấu hình LN03
        /// </summary>
        /// <returns>Cấu hình hiện tại</returns>
        [HttpGet("config")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LN03ConfigDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LN03ConfigDto>>> GetConfiguration()
        {
            try
            {
                var result = await _service.GetConfigurationAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy cấu hình LN03");
                return StatusCode(500, ApiResponse<LN03ConfigDto>.Failure("Lỗi hệ thống khi lấy cấu hình"));
            }
        }
    }
}
