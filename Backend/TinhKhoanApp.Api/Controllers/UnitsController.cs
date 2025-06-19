using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models; // Đảm bảo Sếp đã using namespace chứa UnitListItemDto

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // SỬA PHƯƠNG THỨC NÀY
        // GET: api/Units 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitListItemDto>>> GetUnits() // Thay đổi kiểu trả về
        {
            return await _context.Units
                                 .OrderBy(u => u.SortOrder ?? int.MaxValue) // Use SortOrder first, then fall back to MaxValue for nulls
                                 .ThenBy(u => u.Name) // Secondary sort by Name
                                 .Select(u => new UnitListItemDto // Sử dụng Select để map sang DTO
                                 {
                                     Id = u.Id,
                                     Code = u.Code,
                                     Name = u.Name,
                                     Type = u.Type,
                                     ParentUnitId = u.ParentUnitId,
                                     SortOrder = u.SortOrder,
                                     // Sử dụng LEFT JOIN thông qua DefaultIfEmpty()
                                     ParentUnitName = u.ParentUnit != null ? u.ParentUnit.Name : null
                                 })
                                 .ToListAsync();
        }

        // ... (các phương thức GetUnit(id), PostUnit, PutUnit, DeleteUnit giữ nguyên, chúng vẫn làm việc với model Unit đầy đủ) ...
        // GET: api/Units/5 
        [HttpGet("{id}")]
        public async Task<ActionResult<Unit>> GetUnit(int id) // Get chi tiết vẫn trả về Unit đầy đủ
        {
            var unit = await _context.Units
                                     .Include(u => u.ParentUnit) // Có thể include khi lấy chi tiết
                                     .Include(u => u.ChildUnits)
                                     .FirstOrDefaultAsync(u => u.Id == id);

            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }
        
        // POST: api/Units
        [HttpPost]
        public async Task<ActionResult<Unit>> PostUnit([FromBody] Unit unit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _context.Units.AnyAsync(u => u.Code == unit.Code))
            {
                ModelState.AddModelError("Code", "Mã đơn vị đã tồn tại.");
                return BadRequest(ModelState);
            }

            // Default ID assignment logic for branches
            if (!unit.ParentUnitId.HasValue)
            {
                if (unit.Type == "CNL2")
                {
                    // CNL2 units should default to the first available CNL1 unit as parent
                    var defaultCNL1 = await _context.Units
                        .Where(u => u.Type == "CNL1")
                        .OrderBy(u => u.Id)
                        .FirstOrDefaultAsync();
                    
                    if (defaultCNL1 != null)
                    {
                        unit.ParentUnitId = defaultCNL1.Id;
                    }
                    else
                    {
                        ModelState.AddModelError("ParentUnitId", "Không tìm thấy CNL1 nào để làm đơn vị cha cho CNL2. Vui lòng tạo CNL1 trước.");
                        return BadRequest(ModelState);
                    }
                }
                // CNL1 units remain with ParentUnitId = null (they are root units)
            }

            if (unit.ParentUnitId.HasValue && !await _context.Units.AnyAsync(u => u.Id == unit.ParentUnitId.Value))
            {
                ModelState.AddModelError("ParentUnitId", "Đơn vị cha không tồn tại.");
                return BadRequest(ModelState);
            }
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, unit);
        }

        // PUT: api/Units/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnit(int id, [FromBody] Unit unit)
        {
            if (id != unit.Id)
            {
                return BadRequest("ID trong URL không khớp với ID trong nội dung request.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _context.Units.AnyAsync(u => u.Code == unit.Code && u.Id != id))
            {
                ModelState.AddModelError("Code", "Mã đơn vị đã tồn tại cho một đơn vị khác.");
                return BadRequest(ModelState);
            }
            if (unit.ParentUnitId.HasValue)
            {
                if (unit.ParentUnitId.Value == id) {
                    ModelState.AddModelError("ParentUnitId", "Đơn vị không thể tự làm cha của chính nó.");
                    return BadRequest(ModelState);
                }
                if (!await _context.Units.AnyAsync(u => u.Id == unit.ParentUnitId.Value))
                {
                    ModelState.AddModelError("ParentUnitId", "Đơn vị cha không tồn tại.");
                    return BadRequest(ModelState);
                }
            }
            _context.Entry(unit).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Units.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/Units/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            var hasChildUnits = await _context.Units.AnyAsync(u => u.ParentUnitId == id);
            if (hasChildUnits)
            {
                return BadRequest(new { message = "Đơn vị này đang có các đơn vị con và không thể xóa. Hãy xóa hoặc di chuyển các đơn vị con trước." });
            }
            var hasEmployees = await _context.Employees.AnyAsync(e => e.UnitId == id);
            if (hasEmployees)
            {
                 return BadRequest(new { message = "Đơn vị này đang có nhân viên và không thể xóa. Hãy di chuyển nhân viên sang đơn vị khác trước." });
            }
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}