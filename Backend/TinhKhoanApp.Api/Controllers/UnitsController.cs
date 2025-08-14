using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(ApplicationDbContext context, ILogger<UnitsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("📋 [Units] Lấy tất cả units");
                
                var units = await _context.Units
                    .Where(u => !u.IsDeleted)
                    .Include(u => u.ParentUnit)
                    .Select(u => new
                    {
                        u.Id,
                        u.Code,
                        u.Name,
                        u.Type,
                        u.ParentUnitId,
                        ParentUnitName = u.ParentUnit != null ? u.ParentUnit.Name : null,
                        ParentUnitCode = u.ParentUnit != null ? u.ParentUnit.Code : null
                    })
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                _logger.LogInformation("✅ [Units] Tìm thấy {Count} units", units.Count);
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Units] Lỗi khi lấy tất cả units");
                return StatusCode(500, new { error = "Lỗi server nội bộ", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
        {
            try
            {
                _logger.LogInformation("➕ [Units] Tạo unit mới: {Code}", dto.Code);

                var existingUnit = await _context.Units
                    .FirstOrDefaultAsync(u => u.Code == dto.Code && !u.IsDeleted);
                
                if (existingUnit != null)
                {
                    return BadRequest(new { error = "Mã unit đã tồn tại" });
                }

                var unit = new Unit
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Type = dto.Type?.Trim(),
                    ParentUnitId = dto.ParentUnitId,
                    IsDeleted = false
                };

                _context.Units.Add(unit);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ [Units] Unit tạo thành công: {Code} (ID: {Id})", unit.Code, unit.Id);
                return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Units] Lỗi khi tạo unit: {Code}", dto.Code);
                return StatusCode(500, new { error = "Lỗi server nội bộ", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var unit = await _context.Units
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (unit == null)
                {
                    return NotFound(new { error = "Không tìm thấy unit" });
                }

                return Ok(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Units] Lỗi khi lấy unit theo ID: {Id}", id);
                return StatusCode(500, new { error = "Lỗi server nội bộ", details = ex.Message });
            }
        }
    }

    public class CreateUnitDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public int? ParentUnitId { get; set; }
    }
}
