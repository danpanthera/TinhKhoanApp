using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, UpdateRoleDto roleDto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.Name = roleDto.Name;
            role.Description = roleDto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(CreateRoleDto roleDto)
        {
            var role = new Role
            {
                Name = roleDto.Name,
                Description = roleDto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = role.Id }, role);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Roles/batch
        [HttpDelete("batch")]
        public async Task<IActionResult> DeleteRoles([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("No role IDs provided");
            }

            var roles = await _context.Roles
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();

            if (roles.Count == 0)
            {
                return NotFound("No roles found with the provided IDs");
            }

            _context.Roles.RemoveRange(roles);
            await _context.SaveChangesAsync();

            return Ok($"Deleted {roles.Count} roles");
        }

        // POST: api/Roles/seed
        [HttpPost("seed")]
        public async Task<IActionResult> SeedRoles()
        {
            // Xóa tất cả roles hiện tại
            var existingRoles = await _context.Roles.ToListAsync();
            if (existingRoles.Count > 0)
            {
                _context.Roles.RemoveRange(existingRoles);
                await _context.SaveChangesAsync();
            }

            // Tạo 23 vai trò chuẩn
            var roles = new List<Role>
            {
                new Role { Name = "TruongphongKhdn", Description = "Trưởng phòng KHDN" },
                new Role { Name = "TruongphongKhcn", Description = "Trưởng phòng KHCN" },
                new Role { Name = "PhophongKhdn", Description = "Phó phòng KHDN" },
                new Role { Name = "PhophongKhcn", Description = "Phó phòng KHCN" },
                new Role { Name = "TruongphongKhqlrr", Description = "Trưởng phòng KH&QLRR" },
                new Role { Name = "PhophongKhqlrr", Description = "Phó phòng KH&QLRR" },
                new Role { Name = "Cbtd", Description = "Cán bộ tín dụng" },
                new Role { Name = "TruongphongKtnqCnl1", Description = "Trưởng phòng KTNQ CNL1" },
                new Role { Name = "PhophongKtnqCnl1", Description = "Phó phòng KTNQ CNL1" },
                new Role { Name = "Gdv", Description = "GDV" },
                new Role { Name = "TqHkKtnb", Description = "Thủ quỹ | Hậu kiểm | KTNB" },
                new Role { Name = "TruongphoItThKtgs", Description = "Trưởng phó IT | Tổng hợp | KTGS" },
                new Role { Name = "CBItThKtgsKhqlrr", Description = "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR" },
                new Role { Name = "GiamdocPgd", Description = "Giám đốc Phòng giao dịch" },
                new Role { Name = "PhogiamdocPgd", Description = "Phó giám đốc Phòng giao dịch" },
                new Role { Name = "PhogiamdocPgdCbtd", Description = "Phó giám đốc Phòng giao dịch kiêm CBTD" },
                new Role { Name = "GiamdocCnl2", Description = "Giám đốc CNL2" },
                new Role { Name = "PhogiamdocCnl2Td", Description = "Phó giám đốc CNL2 phụ trách Tín dụng" },
                new Role { Name = "PhogiamdocCnl2Kt", Description = "Phó giám đốc CNL2 phụ trách Kế toán" },
                new Role { Name = "TruongphongKhCnl2", Description = "Trưởng phòng Khách hàng CNL2" },
                new Role { Name = "PhophongKhCnl2", Description = "Phó phòng Khách hàng CNL2" },
                new Role { Name = "TruongphongKtnqCnl2", Description = "Trưởng phòng KTNQ CNL2" },
                new Role { Name = "PhophongKtnqCnl2", Description = "Phó phòng KTNQ CNL2" }
            };

            _context.Roles.AddRange(roles);
            await _context.SaveChangesAsync();

            return Ok($"Successfully seeded {roles.Count} roles. Total roles: {await _context.Roles.CountAsync()}");
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }

    // DTOs for Role operations
    public class CreateRoleDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }
    }

    public class UpdateRoleDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
