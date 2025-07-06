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

        // GET: api/Units
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitListItemDto>>> GetUnits()
        {
            var units = await _context.Units
                                     .Select(u => new UnitListItemDto
                                     {
                                         Id = u.Id,
                                         Code = u.Code,
                                         Name = u.Name,
                                         Type = u.Type,
                                         ParentUnitId = u.ParentUnitId,
                                         ParentUnitName = u.ParentUnit != null ? u.ParentUnit.Name : null
                                     })
                                     .ToListAsync();

            // Sắp xếp theo thứ tự yêu cầu
            return units.OrderBy(u => GetBranchSortOrder(u.Code))
                       .ThenBy(u => GetDepartmentSortOrder(u.Name))
                       .ToList();
        }

        private int GetBranchSortOrder(string code)
        {
            return code switch
            {
                "CnLaiChau" => 1,
                // Tên cũ (để backward compatibility)
                "CnTamDuong" => 2,
                "CnPhongTho" => 3,
                "CnSinHo" => 4,
                "CnMuongTe" => 5,
                "CnThanUyen" => 6,
                "CnThanhPho" => 7,
                "CnTanUyen" => 8,
                // Tên mới theo quy ước anh
                "CnBinhLu" => 2,
                "CnBumTo" => 5,
                "CnDoanKet" => 7,
                "CnNamHang" => 9,
                "CnNamNhun" => 9,
                _ => 999 // Các units khác sẽ được sắp xếp cuối
            };
        }

        // GET: api/Units/departments-by-branch/{branchId}
        [HttpGet("departments-by-branch/{branchId}")]
        public async Task<ActionResult<IEnumerable<UnitListItemDto>>> GetDepartmentsByBranch(int branchId)
        {
            var departments = await _context.Units
                                          .Where(u => u.ParentUnitId == branchId)
                                          .Where(u => u.Type != null && (u.Type.Contains("PNVL") || u.Type == "PGD"))
                                          .Select(u => new UnitListItemDto
                                          {
                                              Id = u.Id,
                                              Code = u.Code,
                                              Name = u.Name,
                                              Type = u.Type,
                                              ParentUnitId = u.ParentUnitId,
                                              ParentUnitName = u.ParentUnit != null ? u.ParentUnit.Name : null
                                          })
                                          .ToListAsync();

            // Sắp xếp theo thứ tự yêu cầu
            return departments.OrderBy(d => GetDepartmentSortOrder(d.Name))
                            .ToList();
        }

        private int GetDepartmentSortOrder(string name)
        {
            return name switch
            {
                "Ban Giám đốc" => 1,
                "Phòng Khách hàng" => 2,
                "Phòng Khách hàng cá nhân" => 2,
                "Phòng Khách hàng doanh nghiệp" => 3,
                "Phòng Kế toán & Ngân quỹ" => 4,
                "Phòng giao dịch" => 5,
                _ => 999 // Các phòng ban khác sẽ được sắp xếp cuối
            };
        }

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
                if (unit.ParentUnitId.Value == id)
                {
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

        // POST: api/Units/restore-original-data (Admin only)
        [HttpPost("restore-original-data")]
        public async Task<ActionResult> RestoreOriginalData()
        {
            try
            {
                // Đọc script SQL từ file
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "restore_original_data.sql");
                if (!System.IO.File.Exists(scriptPath))
                {
                    return BadRequest(new { message = "Script file not found: restore_original_data.sql" });
                }

                var sqlScript = await System.IO.File.ReadAllTextAsync(scriptPath);

                // Execute SQL script
                await _context.Database.ExecuteSqlRawAsync(sqlScript);

                // Verify results
                var unitsCount = await _context.Units.CountAsync(u => !u.IsDeleted);
                var positionsCount = await _context.Positions.CountAsync();
                var employeesCount = await _context.Employees.CountAsync(e => e.IsActive);

                return Ok(new
                {
                    message = "✅ Đã khôi phục dữ liệu gốc thành công!",
                    results = new
                    {
                        units = unitsCount,
                        positions = positionsCount,
                        employees = employeesCount
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Lỗi khi khôi phục dữ liệu",
                    error = ex.Message,
                    details = ex.ToString()
                });
            }
        }
    }
}
