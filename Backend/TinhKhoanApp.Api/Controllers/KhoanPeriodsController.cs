using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
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
                // Validate dates
                if (period.StartDate >= period.EndDate)
                {
                    return BadRequest(ApiResponse<object>.Error("Start date must be before end date", "INVALID_DATE_RANGE"));
                }

                // Set default status
                period.Status = PeriodStatus.DRAFT;

                _context.KhoanPeriods.Add(period);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Created khoan period: {period.Name}");
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
