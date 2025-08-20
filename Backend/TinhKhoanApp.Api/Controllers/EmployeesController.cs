using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
    {
        try
        {
            _logger.LogInformation("Fetching all employees");
            var employees = await _context.Employees
                .Include(e => e.Unit)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .OrderBy(e => e.EmployeeCode)
                .ToListAsync();

            // Transform to include role info properly
            var result = employees.Select(e => new
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
                UnitName = e.Unit?.Name,
                PositionId = e.PositionId,
                PositionName = e.Position?.Name,
                RoleId = e.EmployeeRoles?.FirstOrDefault()?.RoleId,
                RoleName = e.EmployeeRoles?.FirstOrDefault()?.Role?.Name
            }).ToList();

            _logger.LogInformation("Found {Count} employees", result.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employees");
            return StatusCode(500, new { message = "Lỗi khi lấy danh sách nhân viên", details = ex.Message });
        }
    }

    // GET: api/Employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetEmployee(int id)
    {
        try
        {
            var employee = await _context.Employees
                .Include(e => e.Unit)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên" });
            }

            var result = new
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
                Unit = employee.Unit,
                PositionId = employee.PositionId,
                Position = employee.Position,
                Roles = employee.EmployeeRoles?.Select(er => er.Role).ToList(),
                RoleId = employee.EmployeeRoles?.FirstOrDefault()?.RoleId
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employee {Id}", id);
            return StatusCode(500, new { message = "Lỗi khi lấy thông tin nhân viên", details = ex.Message });
        }
    }

    // POST: api/Employees
    [HttpPost]
    public async Task<ActionResult<object>> CreateEmployee(EmployeeCreateDto employeeDto)
    {
        try
        {
            _logger.LogInformation("Creating new employee: {Username}", employeeDto.Username);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(employeeDto.Username))
            {
                return BadRequest(new { message = "Username là bắt buộc" });
            }

            if (string.IsNullOrWhiteSpace(employeeDto.FullName))
            {
                return BadRequest(new { message = "Họ tên là bắt buộc" });
            }

            // Check if username already exists
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username.ToLower() == employeeDto.Username.ToLower());

            if (existingEmployee != null)
            {
                return BadRequest(new { message = "Username đã tồn tại" });
            }

            // Create employee
            var employee = new Employee
            {
                EmployeeCode = employeeDto.EmployeeCode ?? string.Empty,
                CBCode = employeeDto.CBCode ?? string.Empty,
                FullName = employeeDto.FullName,
                Username = employeeDto.Username,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                IsActive = employeeDto.IsActive ?? true,
                UnitId = employeeDto.UnitId,
                PositionId = employeeDto.PositionId
            };

            // Set password
            if (!string.IsNullOrWhiteSpace(employeeDto.PasswordHash))
            {
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employeeDto.PasswordHash);
            }
            else
            {
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"); // Default password
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Add role if specified
            if (employeeDto.RoleId.HasValue)
            {
                var roleExists = await _context.Roles.AnyAsync(r => r.Id == employeeDto.RoleId.Value);
                if (roleExists)
                {
                    var employeeRole = new EmployeeRole
                    {
                        EmployeeId = employee.Id,
                        RoleId = employeeDto.RoleId.Value
                    };
                    _context.EmployeeRoles.Add(employeeRole);
                    await _context.SaveChangesAsync();
                }
            }

            // Return created employee
            var createdEmployee = await _context.Employees
                .Include(e => e.Unit)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == employee.Id);

            var result = new
            {
                Id = createdEmployee!.Id,
                EmployeeCode = createdEmployee.EmployeeCode,
                CBCode = createdEmployee.CBCode,
                FullName = createdEmployee.FullName,
                Username = createdEmployee.Username,
                Email = createdEmployee.Email,
                PhoneNumber = createdEmployee.PhoneNumber,
                IsActive = createdEmployee.IsActive,
                UnitId = createdEmployee.UnitId,
                UnitName = createdEmployee.Unit?.Name,
                PositionId = createdEmployee.PositionId,
                PositionName = createdEmployee.Position?.Name,
                RoleId = createdEmployee.EmployeeRoles?.FirstOrDefault()?.RoleId,
                RoleName = createdEmployee.EmployeeRoles?.FirstOrDefault()?.Role?.Name
            };

            _logger.LogInformation("Employee created successfully with Id: {Id}", employee.Id);
            // Audit log (CREATE)
            _context.EmployeeAuditLogs.Add(new EmployeeAuditLog
            {
                EmployeeId = employee.Id,
                Action = "CREATE",
                PerformedBy = employeeDto.CreatedBy ?? "system",
                FieldChanged = null,
                OldValue = null,
                NewValue = $"Username={employee.Username};FullName={employee.FullName};RoleId={createdEmployee.EmployeeRoles?.FirstOrDefault()?.RoleId}"
            });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, new { message = "Lỗi khi tạo nhân viên", details = ex.Message });
        }
    }

    // DELETE: api/Employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên" });
            }

            // Don't allow deleting admin user
            if (employee.Username?.ToLower() == "admin")
            {
                return BadRequest(new { message = "Không thể xóa tài khoản admin" });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Employee {Id} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee {Id}", id);
            return StatusCode(500, new { message = "Lỗi khi xóa nhân viên", details = ex.Message });
        }
    }

    // PUT: api/Employees/5
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateEmployee(int id, EmployeeUpdateDto dto)
    {
        try
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeRoles)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên" });
            }

            var changes = new List<EmployeeAuditLog>();

            // Username uniqueness check if changed
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != employee.Username)
            {
                var exists = await _context.Employees.AnyAsync(e => e.Username.ToLower() == dto.Username.ToLower() && e.Id != id);
                if (exists)
                {
                    return BadRequest(new { message = "Username đã tồn tại" });
                }
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "Username", OldValue = employee.Username, NewValue = dto.Username, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.Username = dto.Username;
            }

            // Update simple scalar fields (only if provided / not null for nullable types)
            if (!string.IsNullOrWhiteSpace(dto.FullName) && dto.FullName != employee.FullName)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "FullName", OldValue = employee.FullName, NewValue = dto.FullName, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.FullName = dto.FullName;
            }
            if (!string.IsNullOrWhiteSpace(dto.EmployeeCode)) employee.EmployeeCode = dto.EmployeeCode;
            if (!string.IsNullOrWhiteSpace(dto.CBCode)) employee.CBCode = dto.CBCode;
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != employee.Email)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "Email", OldValue = employee.Email, NewValue = dto.Email, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.Email = dto.Email;
            }
            else if (dto.Email == string.Empty && employee.Email != null)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "Email", OldValue = employee.Email, NewValue = null, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.Email = null;
            }
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != employee.PhoneNumber)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "PhoneNumber", OldValue = employee.PhoneNumber, NewValue = dto.PhoneNumber, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.PhoneNumber = dto.PhoneNumber;
            }
            else if (dto.PhoneNumber == string.Empty && employee.PhoneNumber != null)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "PhoneNumber", OldValue = employee.PhoneNumber, NewValue = null, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.PhoneNumber = null;
            }
            if (dto.IsActive.HasValue && dto.IsActive.Value != employee.IsActive)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "IsActive", OldValue = employee.IsActive.ToString(), NewValue = dto.IsActive.Value.ToString(), PerformedBy = dto.UpdatedBy ?? "system" });
                employee.IsActive = dto.IsActive.Value;
            }
            if (dto.UnitId.HasValue && dto.UnitId.Value != employee.UnitId)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "UnitId", OldValue = employee.UnitId.ToString(), NewValue = dto.UnitId.Value.ToString(), PerformedBy = dto.UpdatedBy ?? "system" });
                employee.UnitId = dto.UnitId.Value;
            }
            if (dto.PositionId.HasValue && dto.PositionId.Value != employee.PositionId)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "PositionId", OldValue = employee.PositionId.ToString(), NewValue = dto.PositionId.Value.ToString(), PerformedBy = dto.UpdatedBy ?? "system" });
                employee.PositionId = dto.PositionId.Value;
            }

            // Password change: treat incoming PasswordHash field as plain text password like create path
            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "PasswordHash", OldValue = "***", NewValue = "***", PerformedBy = dto.UpdatedBy ?? "system" });
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);
            }

            // Role update: allow single role id
            if (dto.RoleId.HasValue)
            {
                // Remove existing roles if different
                var currentRoleId = employee.EmployeeRoles.FirstOrDefault()?.RoleId;
                if (currentRoleId != dto.RoleId.Value)
                {
                    if (employee.EmployeeRoles.Any())
                    {
                        _context.EmployeeRoles.RemoveRange(employee.EmployeeRoles);
                        employee.EmployeeRoles.Clear();
                    }
                    var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId.Value);
                    if (!roleExists)
                    {
                        return BadRequest(new { message = "Role không tồn tại" });
                    }
                    _context.EmployeeRoles.Add(new EmployeeRole { EmployeeId = employee.Id, RoleId = dto.RoleId.Value });
                    changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "RoleId", OldValue = currentRoleId?.ToString(), NewValue = dto.RoleId.Value.ToString(), PerformedBy = dto.UpdatedBy ?? "system" });
                }
            }
            else if (dto.RoleId == null && dto.ClearRole == true)
            {
                if (employee.EmployeeRoles.Any())
                {
                    _context.EmployeeRoles.RemoveRange(employee.EmployeeRoles);
                    employee.EmployeeRoles.Clear();
                    changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "RoleId", OldValue = "(removed)", NewValue = null, PerformedBy = dto.UpdatedBy ?? "system" });
                }
            }

            await _context.SaveChangesAsync();

            if (changes.Any())
            {
                _context.EmployeeAuditLogs.AddRange(changes);
                await _context.SaveChangesAsync();
            }

            // Return updated employee shape (reuse Get style)
            var updated = await _context.Employees
                .Include(e => e.Unit)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRoles).ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id);

            var result = new
            {
                Id = updated!.Id,
                EmployeeCode = updated.EmployeeCode,
                CBCode = updated.CBCode,
                FullName = updated.FullName,
                Username = updated.Username,
                Email = updated.Email,
                PhoneNumber = updated.PhoneNumber,
                IsActive = updated.IsActive,
                UnitId = updated.UnitId,
                UnitName = updated.Unit?.Name,
                PositionId = updated.PositionId,
                PositionName = updated.Position?.Name,
                RoleId = updated.EmployeeRoles.FirstOrDefault()?.RoleId,
                RoleName = updated.EmployeeRoles.FirstOrDefault()?.Role?.Name
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {Id}", id);
            return StatusCode(500, new { message = "Lỗi khi cập nhật nhân viên", details = ex.Message });
        }
    }
}

// DTO for creating employees
public class EmployeeCreateDto
{
    public string? EmployeeCode { get; set; }
    public string? CBCode { get; set; }
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public int UnitId { get; set; }
    public int PositionId { get; set; }
    public int? RoleId { get; set; }
    public string? CreatedBy { get; set; }
}

// DTO for updating employees (all optional except at least one field)
public class EmployeeUpdateDto
{
    public string? EmployeeCode { get; set; }
    public string? CBCode { get; set; }
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; } // treated as raw password input
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public int? RoleId { get; set; }
    public bool? ClearRole { get; set; } // when true and RoleId null -> remove role
    public string? UpdatedBy { get; set; }
}
