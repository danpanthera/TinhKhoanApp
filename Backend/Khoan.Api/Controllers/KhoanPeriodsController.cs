using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Controller để quản lý các kỳ khoán (KhoanPeriods)
    /// Quản lý các kỳ khoán cho việc giao chỉ tiêu KPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class KhoanPeriodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KhoanPeriodsController> _logger;

        public KhoanPeriodsController(ApplicationDbContext context, ILogger<KhoanPeriodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả kỳ khoán
        /// </summary>
        /// <returns>Danh sách kỳ khoán</returns>
        [HttpGet]
        public async Task<IActionResult> GetKhoanPeriods()
        {
            try
            {
                var periods = await _context.KhoanPeriods
                    .OrderByDescending(k => k.StartDate)
                    .ToListAsync();

                _logger.LogInformation($"✅ Loaded {periods.Count} khoan periods");
                return Ok(ApiResponse<IEnumerable<KhoanPeriod>>.Ok(periods, "Khoan periods loaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading khoan periods");
                return StatusCode(500, ApiResponse<object>.Error("Error loading khoan periods", "LOAD_ERROR"));
            }
        }

        /// <summary>
        /// Lấy kỳ khoán theo ID
        /// </summary>
        /// <param name="id">ID của kỳ khoán</param>
        /// <returns>Chi tiết kỳ khoán</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKhoanPeriod(int id)
        {
            try
            {
                var period = await _context.KhoanPeriods.FindAsync(id);

                if (period == null)
                {
                    return NotFound(ApiResponse<object>.Error($"Khoan period with ID {id} not found", "NOT_FOUND"));
                }

                return Ok(ApiResponse<KhoanPeriod>.Ok(period, "Khoan period loaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading khoan period {Id}", id);
                return StatusCode(500, ApiResponse<object>.Error("Error loading khoan period", "LOAD_ERROR"));
            }
        }

        /// <summary>
        /// Tạo kỳ khoán mới
        /// </summary>
        /// <param name="period">Thông tin kỳ khoán</param>
        /// <returns>Kỳ khoán đã tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateKhoanPeriod([FromBody] KhoanPeriod period)
        {
            try
            {
                _logger.LogInformation($"🔍 CreateKhoanPeriod called with: {System.Text.Json.JsonSerializer.Serialize(period)}");

                // Check model state validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    var errorMessage = $"Dữ liệu Kỳ Khoán nhận được không đúng định dạng: {string.Join(", ", errors.SelectMany(e => e.Errors))}";
                    _logger.LogError($"❌ Model validation failed: {System.Text.Json.JsonSerializer.Serialize(errors)}");
                    return BadRequest(ApiResponse<object>.Error(errorMessage, "VALIDATION_ERROR"));
                }

                // Validate input data
                if (period == null)
                {
                    return BadRequest(ApiResponse<object>.Error("Dữ liệu Kỳ Khoán không được để trống", "NULL_DATA"));
                }

                if (string.IsNullOrWhiteSpace(period.Name))
                {
                    return BadRequest(ApiResponse<object>.Error("Tên Kỳ Khoán không được để trống", "EMPTY_NAME"));
                }

                // Validate dates
                if (period.StartDate >= period.EndDate)
                {
                    return BadRequest(ApiResponse<object>.Error("Ngày bắt đầu phải nhỏ hơn ngày kết thúc", "INVALID_DATE_RANGE"));
                }

                // Set default status if not provided
                if (period.Status == 0)
                {
                    period.Status = PeriodStatus.DRAFT;
                }

                _context.KhoanPeriods.Add(period);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Created khoan period: {period.Name} (ID: {period.Id})");
                return CreatedAtAction(nameof(GetKhoanPeriod), new { id = period.Id }, ApiResponse<KhoanPeriod>.Ok(period, "Khoan period created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating khoan period");
                return StatusCode(500, ApiResponse<object>.Error("Error creating khoan period", "CREATE_ERROR"));
            }
        }

        /// <summary>
        /// Cập nhật kỳ khoán
        /// </summary>
        /// <param name="id">ID của kỳ khoán</param>
        /// <param name="period">Thông tin kỳ khoán cần cập nhật</param>
        /// <returns>Kỳ khoán đã cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKhoanPeriod(int id, [FromBody] KhoanPeriod period)
        {
            try
            {
                if (id != period.Id)
                {
                    return BadRequest(ApiResponse<object>.Error("ID mismatch", "ID_MISMATCH"));
                }

                var existingPeriod = await _context.KhoanPeriods.FindAsync(id);
                if (existingPeriod == null)
                {
                    return NotFound(ApiResponse<object>.Error($"Khoan period with ID {id} not found", "NOT_FOUND"));
                }

                // Validate dates
                if (period.StartDate >= period.EndDate)
                {
                    return BadRequest(ApiResponse<object>.Error("Start date must be before end date", "INVALID_DATE_RANGE"));
                }

                // Update properties
                existingPeriod.Name = period.Name;
                existingPeriod.Type = period.Type;
                existingPeriod.StartDate = period.StartDate;
                existingPeriod.EndDate = period.EndDate;
                existingPeriod.Status = period.Status;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Updated khoan period: {existingPeriod.Name}");
                return Ok(ApiResponse<KhoanPeriod>.Ok(existingPeriod, "Khoan period updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating khoan period {Id}", id);
                return StatusCode(500, ApiResponse<object>.Error("Error updating khoan period", "UPDATE_ERROR"));
            }
        }

        /// <summary>
        /// Xóa kỳ khoán
        /// </summary>
        /// <param name="id">ID của kỳ khoán</param>
        /// <returns>Xác nhận xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKhoanPeriod(int id)
        {
            try
            {
                var period = await _context.KhoanPeriods.FindAsync(id);
                if (period == null)
                {
                    return NotFound(ApiResponse<object>.Error($"Khoan period with ID {id} not found", "NOT_FOUND"));
                }

                _context.KhoanPeriods.Remove(period);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Deleted khoan period: {period.Name}");
                return Ok(ApiResponse<string>.Ok("Deleted successfully", "Khoan period deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting khoan period {Id}", id);
                return StatusCode(500, ApiResponse<object>.Error("Error deleting khoan period", "DELETE_ERROR"));
            }
        }
    }
}
