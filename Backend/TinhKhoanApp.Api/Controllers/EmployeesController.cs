using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Dtos;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")] // Đường dẫn sẽ là "api/Employees"
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees (Lấy tất cả Nhân viên với thông tin Unit và Position)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeListItemDto>>> GetEmployees()
        {
            try
            {
                // Sử dụng EF Core navigation properties
                var employees = await _context.Employees
                    .Include(e => e.Unit)
                    .Include(e => e.Position)
                    .Select(e => new EmployeeListItemDto
                    {
                        Id = e.Id,
                        EmployeeCode = e.EmployeeCode ?? "",
                        CBCode = e.CBCode ?? "",
                        FullName = e.FullName ?? "",
                        Username = e.Username ?? "",
                        Email = e.Email ?? "",
                        PhoneNumber = e.PhoneNumber ?? "",
                        IsActive = e.IsActive,
                        UnitId = e.UnitId,
                        UnitName = e.Unit != null ? e.Unit.Name : "",
                        PositionId = e.PositionId,
                        PositionName = e.Position != null ? e.Position.Name : "",
                        Roles = new List<RoleDto>() // Empty for now
                    })
                    .OrderBy(e => e.EmployeeCode)
                    .ToListAsync();

                return employees;
            }
            catch (Exception)
            {
                // Log error and return a simpler result
                var simpleEmployees = await _context.Database
                    .SqlQueryRaw<SimpleEmployeeDto>(@"
                        SELECT
                            ""Id"",
                            COALESCE(""EmployeeCode"", '') as ""EmployeeCode"",
                            COALESCE(""CBCode"", '') as ""CBCode"",
                            COALESCE(""FullName"", '') as ""FullName"",
                            COALESCE(""Username"", '') as ""Username"",
                            COALESCE(""Email"", '') as ""Email"",
                            COALESCE(""PhoneNumber"", '') as ""PhoneNumber"",
                            ""IsActive"",
                            ""UnitId"",
                            ""PositionId""
                        FROM ""Employees""
                        ORDER BY ""EmployeeCode""
                    ")
                    .ToListAsync();

                var result = simpleEmployees.Select(e => new EmployeeListItemDto
                {
                    Id = e.Id,
                    EmployeeCode = e.EmployeeCode,
                    CBCode = e.CBCode,
                    FullName = e.FullName,
                    Username = e.Username,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    IsActive = e.IsActive,
                    UnitId = e.UnitId,
                    UnitName = null,
                    PositionId = e.PositionId ?? 0,
                    PositionName = null,
                    Roles = new List<RoleDto>()
                }).ToList();

                return result;
            }
        }

        public class SimpleEmployeeDto
        {
            public int Id { get; set; }
            public string EmployeeCode { get; set; } = "";
            public string CBCode { get; set; } = "";
            public string FullName { get; set; } = "";
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public bool IsActive { get; set; }
            public int UnitId { get; set; }
            public int? PositionId { get; set; }
        }

        // GET: api/Employees/5 (Lấy một Nhân viên theo Id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Unit)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/Employees (Tạo một Nhân viên mới)
        [HttpPost]
        public async Task<ActionResult<EmployeeListItemDto>> PostEmployee([FromBody] EmployeeRequestDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra tồn tại và tính hợp lệ của UnitId
            var unit = await _context.Units.FindAsync(employeeDto.UnitId);
            if (unit == null)
            {
                ModelState.AddModelError("UnitId", "Đơn vị không tồn tại.");
            }

            // Kiểm tra tồn tại và tính hợp lệ của PositionId
            var position = await _context.Positions.FindAsync(employeeDto.PositionId);
            if (position == null)
            {
                ModelState.AddModelError("PositionId", "Chức vụ không tồn tại.");
            }

            // Kiểm tra trùng lặp EmployeeCode
            if (await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeDto.EmployeeCode))
            {
                ModelState.AddModelError("EmployeeCode", "Mã nhân viên đã tồn tại.");
            }

            // Kiểm tra trùng lặp Username
            if (await _context.Employees.AnyAsync(e => e.Username == employeeDto.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
            }

            // Kiểm tra role IDs có tồn tại không
            var validRoleIds = new List<int>();
            if (employeeDto.RoleIds != null && employeeDto.RoleIds.Any())
            {
                validRoleIds = await _context.Roles
                    .Where(r => employeeDto.RoleIds.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync();

                var invalidRoleIds = employeeDto.RoleIds.Except(validRoleIds).ToList();
                if (invalidRoleIds.Any())
                {
                    ModelState.AddModelError("RoleIds", $"Các vai trò không tồn tại: {string.Join(", ", invalidRoleIds)}");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tạo employee entity
            var employee = new Employee
            {
                EmployeeCode = employeeDto.EmployeeCode,
                CBCode = employeeDto.CBCode,
                FullName = employeeDto.FullName,
                Username = employeeDto.Username,
                PasswordHash = employeeDto.PasswordHash ?? string.Empty,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                IsActive = employeeDto.IsActive,
                UnitId = employeeDto.UnitId,
                PositionId = employeeDto.PositionId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Gán roles cho employee
            if (validRoleIds.Any())
            {
                var employeeRoles = validRoleIds.Select(roleId => new EmployeeRole
                {
                    EmployeeId = employee.Id,
                    RoleId = roleId
                }).ToList();

                _context.EmployeeRoles.AddRange(employeeRoles);
                await _context.SaveChangesAsync();
            }

            // Trả về DTO thay vì entity để đảm bảo format dữ liệu nhất quán
            var roles = await _context.Roles
                .Where(r => validRoleIds.Contains(r.Id))
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name, Description = r.Description })
                .ToListAsync();

            var employeeResult = new EmployeeListItemDto
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                CBCode = employee.CBCode,
                FullName = employee.FullName,
                Username = employee.Username,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                IsActive = employee.IsActive,
                UnitId = employee.UnitId,
                UnitName = unit?.Name,
                PositionId = employee.PositionId,
                PositionName = position?.Name,
                Roles = roles
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employeeResult);
        }

        // PUT: api/Employees/5 (Cập nhật một Nhân viên đã có)
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeListItemDto>> PutEmployee(int id, [FromBody] EmployeeRequestDto employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return BadRequest("ID trong URL không khớp với ID trong dữ liệu.");
            }

            var existingEmployee = await _context.Employees
                .Include(e => e.EmployeeRoles)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Kiểm tra tồn tại và tính hợp lệ của UnitId
            var unit = await _context.Units.FindAsync(employeeDto.UnitId);
            if (unit == null)
            {
                ModelState.AddModelError("UnitId", "Đơn vị không tồn tại.");
            }

            // Kiểm tra tồn tại và tính hợp lệ của PositionId
            var position = await _context.Positions.FindAsync(employeeDto.PositionId);
            if (position == null)
            {
                ModelState.AddModelError("PositionId", "Chức vụ không tồn tại.");
            }

            // Kiểm tra trùng lặp EmployeeCode với nhân viên khác
            if (await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeDto.EmployeeCode && e.Id != id))
            {
                ModelState.AddModelError("EmployeeCode", "Mã nhân viên đã tồn tại cho một người khác.");
            }

            // Kiểm tra trùng lặp Username với nhân viên khác
            if (await _context.Employees.AnyAsync(e => e.Username == employeeDto.Username && e.Id != id))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại cho một người khác.");
            }

            // Kiểm tra role IDs có tồn tại không
            var validRoleIds = new List<int>();
            if (employeeDto.RoleIds != null && employeeDto.RoleIds.Any())
            {
                validRoleIds = await _context.Roles
                    .Where(r => employeeDto.RoleIds.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync();

                var invalidRoleIds = employeeDto.RoleIds.Except(validRoleIds).ToList();
                if (invalidRoleIds.Any())
                {
                    ModelState.AddModelError("RoleIds", $"Các vai trò không tồn tại: {string.Join(", ", invalidRoleIds)}");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Cập nhật thông tin employee
            existingEmployee.EmployeeCode = employeeDto.EmployeeCode;
            existingEmployee.CBCode = employeeDto.CBCode;
            existingEmployee.FullName = employeeDto.FullName;
            existingEmployee.Username = employeeDto.Username;
            existingEmployee.Email = employeeDto.Email ?? string.Empty;
            existingEmployee.PhoneNumber = employeeDto.PhoneNumber;
            existingEmployee.IsActive = employeeDto.IsActive;
            existingEmployee.UnitId = employeeDto.UnitId;
            existingEmployee.PositionId = employeeDto.PositionId;

            // Cập nhật password chỉ khi có giá trị mới
            if (!string.IsNullOrEmpty(employeeDto.PasswordHash))
            {
                existingEmployee.PasswordHash = employeeDto.PasswordHash;
            }

            // Cập nhật role assignments
            // Xóa tất cả role assignments hiện tại
            _context.EmployeeRoles.RemoveRange(existingEmployee.EmployeeRoles);

            // Thêm role assignments mới
            if (validRoleIds.Any())
            {
                var newEmployeeRoles = validRoleIds.Select(roleId => new EmployeeRole
                {
                    EmployeeId = existingEmployee.Id,
                    RoleId = roleId
                }).ToList();

                _context.EmployeeRoles.AddRange(newEmployeeRoles);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            // Trả về DTO thay vì entity
            var roles = await _context.Roles
                .Where(r => validRoleIds.Contains(r.Id))
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name, Description = r.Description })
                .ToListAsync();

            var employeeResult = new EmployeeListItemDto
            {
                Id = existingEmployee.Id,
                EmployeeCode = existingEmployee.EmployeeCode,
                CBCode = existingEmployee.CBCode,
                FullName = existingEmployee.FullName,
                Username = existingEmployee.Username,
                Email = existingEmployee.Email,
                PhoneNumber = existingEmployee.PhoneNumber,
                IsActive = existingEmployee.IsActive,
                UnitId = existingEmployee.UnitId,
                UnitName = unit?.Name,
                PositionId = existingEmployee.PositionId,
                PositionName = position?.Name,
                Roles = roles
            };

            return Ok(employeeResult);
        }

        // DELETE: api/Employees/5 (Xóa một Nhân viên)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.EmployeeRoles)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                // Xóa employee (CASCADE sẽ tự động xóa EmployeeRoles)
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting employee", details = ex.Message });
            }
        }

        // DELETE: api/Employees/bulk (Xóa nhiều nhân viên cùng lúc)
        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteMultipleEmployees([FromBody] List<int> employeeIds)
        {
            if (employeeIds == null || !employeeIds.Any())
            {
                return BadRequest("Danh sách ID nhân viên không được để trống.");
            }

            // Lọc các ID hợp lệ
            var validIds = employeeIds.Where(id => id > 0).Distinct().ToList();
            if (!validIds.Any())
            {
                return BadRequest("Không có ID nhân viên hợp lệ nào được cung cấp.");
            }

            // Lấy danh sách nhân viên cần xóa với safe NULL handling
            var employeesToDelete = await _context.Employees
                .Where(e => validIds.Contains(e.Id))
                .Select(e => new
                {
                    e.Id,
                    e.EmployeeCode,
                    FullName = e.FullName ?? "",
                    Username = e.Username ?? "",
                    Employee = e
                })
                .ToListAsync();

            if (!employeesToDelete.Any())
            {
                return NotFound("Không tìm thấy nhân viên nào với các ID đã cung cấp.");
            }

            // Kiểm tra nhân viên admin (không được xóa)
            var adminEmployees = employeesToDelete.Where(e => e.Username == "admin").ToList();
            if (adminEmployees.Any())
            {
                return BadRequest("Không thể xóa tài khoản admin.");
            }

            try
            {
                // Xóa các liên kết EmployeeRoles trước
                var employeeRolesToDelete = await _context.EmployeeRoles
                    .Where(er => validIds.Contains(er.EmployeeId))
                    .ToListAsync();

                if (employeeRolesToDelete.Any())
                {
                    _context.EmployeeRoles.RemoveRange(employeeRolesToDelete);
                }

                // Xóa nhân viên bằng Entity Framework để tránh SQL injection
                var employeesToDeleteFromDb = await _context.Employees
                    .Where(e => validIds.Contains(e.Id))
                    .ToListAsync();

                if (employeesToDeleteFromDb.Any())
                {
                    _context.Employees.RemoveRange(employeesToDeleteFromDb);
                    await _context.SaveChangesAsync();
                }

                // Hoặc nếu muốn dùng soft delete với Entity Framework
                // foreach (var emp in employeesToDeleteFromDb)
                // {
                //     emp.IsActive = false;
                // }
                // await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Đã xóa thành công {employeesToDelete.Count} nhân viên.",
                    deletedCount = employeesToDelete.Count,
                    deletedEmployees = employeesToDelete.Select(e => new { e.Id, e.EmployeeCode, e.FullName }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa nhân viên: {ex.Message}");
            }
        }

        // POST: api/Employees/update-admin-position (Update admin user position)
        [HttpPost("update-admin-position")]
        public async Task<IActionResult> UpdateAdminPosition()
        {
            try
            {
                // Find admin user
                var adminUser = await _context.Employees
                    .Include(e => e.Position)
                    .FirstOrDefaultAsync(e => e.Username == "admin");

                if (adminUser == null)
                {
                    return NotFound("Admin user not found.");
                }

                // Find a basic position - try "Nhân viên" first, then any non-director position
                var basicPosition = await _context.Positions
                    .FirstOrDefaultAsync(p => p.Name == "Nhân viên") ??
                    await _context.Positions
                    .FirstOrDefaultAsync(p => p.Name != "Giám đốc" && !p.Name.Contains("Giám đốc"));

                if (basicPosition == null)
                {
                    return BadRequest("No suitable basic position found in the system.");
                }

                var oldPositionName = adminUser.Position?.Name ?? "Unknown";

                // Update admin position to basic position
                adminUser.PositionId = basicPosition.Id;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Admin user position updated successfully from '{oldPositionName}' to '{basicPosition.Name}'",
                    adminId = adminUser.Id,
                    oldPosition = oldPositionName,
                    newPosition = basicPosition.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating admin position: {ex.Message}");
            }
        }

        // 🔍 TEST: Direct Preview endpoint trong existing controller
        [HttpGet("direct-preview-test")]
        public async Task<ActionResult<object>> DirectPreviewTest()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM DP01";

                var result = await command.ExecuteScalarAsync();
                var count = result != null ? Convert.ToInt32(result) : 0;

                await connection.CloseAsync();

                return Ok(new
                {
                    Message = "✅ Direct Preview Test Success",
                    DP01Count = count,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
