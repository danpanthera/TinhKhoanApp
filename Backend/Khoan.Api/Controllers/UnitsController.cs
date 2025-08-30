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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitDto dto)
        {
            try
            {
                _logger.LogInformation("🔄 [Units] Cập nhật unit ID: {Id}, Code: {Code}", id, dto.Code);

                // Tìm unit cần cập nhật
                var existingUnit = await _context.Units
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (existingUnit == null)
                {
                    _logger.LogWarning("⚠️ [Units] Unit ID {Id} không tồn tại", id);
                    return NotFound(new { error = "Không tìm thấy unit để cập nhật" });
                }

                // Kiểm tra trùng lặp Code (nếu thay đổi Code)
                if (dto.Code != existingUnit.Code)
                {
                    var duplicateUnit = await _context.Units
                        .Where(u => u.Code == dto.Code && u.Id != id && !u.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (duplicateUnit != null)
                    {
                        _logger.LogWarning("⚠️ [Units] Mã unit {Code} đã tồn tại (ID: {ExistingId})", dto.Code, duplicateUnit.Id);
                        return BadRequest(new { error = "Mã unit đã tồn tại" });
                    }
                }

                // Cập nhật thông tin
                existingUnit.Code = dto.Code;
                existingUnit.Name = dto.Name;
                existingUnit.Type = dto.Type;
                existingUnit.ParentUnitId = dto.ParentUnitId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ [Units] Unit cập nhật thành công: {Code} (ID: {Id})", existingUnit.Code, existingUnit.Id);
                return Ok(existingUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Units] Lỗi khi cập nhật unit ID: {Id}", id);
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

    public class UpdateUnitDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public int? ParentUnitId { get; set; }
    }
}
