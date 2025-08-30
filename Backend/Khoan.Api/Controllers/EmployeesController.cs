using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using System.ComponentModel.DataAnnotations;
using Khoan.Api.Models;

namespace Khoan.Api.Controllers;

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

    // GET: api/Employees (optional filter by unitId, isActive)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployees([FromQuery] int? unitId, [FromQuery] bool? isActive)
    {
        try
        {
            _logger.LogInformation("Fetching employees. unitId={UnitId}, isActive={IsActive}", unitId, isActive);
            var query = _context.Employees
                .Include(e => e.Unit)
                .Include(e => e.Position)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .AsQueryable();

            if (unitId.HasValue)
            {
                query = query.Where(e => e.UnitId == unitId.Value);
            }
            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            var employees = await query
                .OrderBy(e => e.CBCode)
                .ThenBy(e => e.FullName)
                .ToListAsync();

            // Transform to include role info properly
            var result = employees.Select(e => new
            {
                Id = e.Id,
                EmployeeCode = (string?)null,
                CBCode = e.CBCode,
                FullName = e.FullName,
                Username = e.Username,
                Email = e.Email,
                UserAD = e.UserAD,
                UserIPCAS = e.UserIPCAS,
                MaCBTD = e.MaCBTD,
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

    // POST: api/Employees/import-cbcode
    // Bulk import/upsert employees using CBCode as the unique business key.
    [HttpPost("import-cbcode")]
    public async Task<ActionResult<object>> ImportEmployeesByCBCode([FromBody] BulkImportByCBCodeRequest request)
    {
        // Log model state errors for debugging
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("❌ Model binding failed");
            foreach (var error in ModelState)
            {
                _logger.LogWarning("🚨 Field '{Field}' has errors: {Errors}",
                    error.Key, string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
            }
            return BadRequest(ModelState);
        }

        _logger.LogInformation("📦 Import request received with {Count} rows", request?.Rows?.Count ?? 0);

        // Log complete request payload for debugging
        if (request?.Rows != null)
        {
            _logger.LogInformation("🔍 Request details: OverwriteExisting={Overwrite}, AutoGenerate={AutoGen}",
                request.OverwriteExisting, request.AutoGenerateMissingUsernames);

            for (int i = 0; i < Math.Min(request.Rows.Count, 3); i++)
            {
                var row = request.Rows[i];
                _logger.LogInformation("📋 Row {Index}: CBCode='{CBCode}', FullName='{FullName}', MaCBTD='{MaCBTD}', UnitId={UnitId}, PositionId={PositionId}",
                    i, row.CBCode, row.FullName, row.MaCBTD, row.UnitId, row.PositionId);
            }
        }

        if (request == null)
        {
            _logger.LogWarning("❌ Import request is null");
            return BadRequest(new { message = "Request không hợp lệ" });
        }

        if (request.Rows == null || request.Rows.Count == 0)
        {
            _logger.LogWarning("❌ Import rows is null or empty");
            return BadRequest(new { message = "Dữ liệu import rỗng" });
        }

        _logger.LogInformation("📋 Processing {Count} import rows", request.Rows.Count);

        var inserted = 0;
        var updated = 0;
        var skipped = 0;
        var errors = new List<string>();

        // Cache lookups to reduce DB roundtrips
        var cbCodes = request.Rows.Where(r => !string.IsNullOrWhiteSpace(r.CBCode)).Select(r => r.CBCode!.Trim()).Distinct().ToList();
        var existingEmployees = await _context.Employees
            .Where(e => cbCodes.Contains(e.CBCode))
            .ToDictionaryAsync(e => e.CBCode, e => e);

        // Cache all existing usernames to avoid repeated database queries during updates
        var existingUsernamesList = await _context.Employees
            .Select(e => e.Username.ToLower())
            .ToListAsync();
        var allUsernames = new HashSet<string>(existingUsernamesList);

        foreach (var (row, idx) in request.Rows.Select((r, i) => (r, i + 1)))
        {
            try
            {
                var cb = row.CBCode?.Trim();
                if (string.IsNullOrWhiteSpace(cb))
                {
                    skipped++;
                    errors.Add($"Dòng {idx}: Thiếu Mã CB");
                    continue;
                }
                if (cb.Length != 9 || !cb.All(char.IsDigit))
                {
                    skipped++;
                    errors.Add($"Dòng {idx}: Mã CB phải gồm đúng 9 chữ số");
                    continue;
                }

                existingEmployees.TryGetValue(cb, out var emp);

                var username = string.IsNullOrWhiteSpace(row.Username)
                    ? cb.ToLowerInvariant()
                    : row.Username!.Trim();

                if (emp == null)
                {
                    // Require UnitId & PositionId for new rows to satisfy FK constraints
                    if (!row.UnitId.HasValue || !row.PositionId.HasValue)
                    {
                        skipped++;
                        errors.Add($"Dòng {idx}: Thiếu UnitId/PositionId cho nhân viên mới {cb}");
                        continue;
                    }

                    // Ensure username uniqueness using cached data
                    var baseUsername = username;
                    var candidate = baseUsername;
                    var suffix = 1;
                    while (allUsernames.Contains(candidate.ToLower()))
                    {
                        candidate = $"{baseUsername}{suffix}";
                        suffix++;
                        // Prevent infinite loops
                        if (suffix > 1000)
                        {
                            skipped++;
                            errors.Add($"Dòng {idx}: Không thể tạo username duy nhất cho {cb}");
                            break;
                        }
                    }

                    // Skip this row if username generation failed
                    if (suffix > 1000) continue;

                    username = candidate;
                    // Add new username to cache
                    allUsernames.Add(username.ToLower());

                    // Create new employee
                    var newEmp = new Employee
                    {
                        CBCode = cb,
                        FullName = row.FullName?.Trim() ?? username,
                        Username = username,
                        Email = string.IsNullOrWhiteSpace(row.Email) ? null : row.Email!.Trim(),
                        PhoneNumber = string.IsNullOrWhiteSpace(row.PhoneNumber) ? null : row.PhoneNumber!.Trim(),
                        UserAD = string.IsNullOrWhiteSpace(row.UserAD) ? null : row.UserAD!.Trim(),
                        UserIPCAS = string.IsNullOrWhiteSpace(row.UserIPCAS) ? null : row.UserIPCAS!.Trim(),
                        MaCBTD = string.IsNullOrWhiteSpace(row.MaCBTD) ? null : row.MaCBTD!.Trim(),
                        IsActive = true,
                        UnitId = row.UnitId!.Value,
                        PositionId = row.PositionId!.Value,
                        PasswordHash = !string.IsNullOrWhiteSpace(row.Password)
                            ? BCrypt.Net.BCrypt.HashPassword(row.Password)
                            : BCrypt.Net.BCrypt.HashPassword("123456")
                    };

                    _context.Employees.Add(newEmp);
                    existingEmployees[cb] = newEmp;
                    inserted++;

                    // Bỏ gán vai trò trong import
                }
                else
                {
                    if (!request.OverwriteExisting)
                    {
                        skipped++;
                        continue;
                    }

                    // Update existing fields if provided
                    if (!string.IsNullOrWhiteSpace(row.FullName) && row.FullName!.Trim() != emp.FullName)
                        emp.FullName = row.FullName!.Trim();
                    if (!string.IsNullOrWhiteSpace(username) && username != emp.Username)
                    {
                        // Use cached usernames instead of expensive database query
                        var lowerUsername = username.ToLower();
                        if (!allUsernames.Contains(lowerUsername))
                        {
                            emp.Username = username;
                            // Update cache with new username
                            allUsernames.Add(lowerUsername);
                        }
                    }
                    emp.Email = string.IsNullOrWhiteSpace(row.Email) ? emp.Email : row.Email!.Trim();
                    emp.PhoneNumber = string.IsNullOrWhiteSpace(row.PhoneNumber) ? emp.PhoneNumber : row.PhoneNumber!.Trim();
                    emp.UserAD = string.IsNullOrWhiteSpace(row.UserAD) ? emp.UserAD : row.UserAD!.Trim();
                    emp.UserIPCAS = string.IsNullOrWhiteSpace(row.UserIPCAS) ? emp.UserIPCAS : row.UserIPCAS!.Trim();
                    emp.MaCBTD = string.IsNullOrWhiteSpace(row.MaCBTD) ? emp.MaCBTD : row.MaCBTD!.Trim();
                    // Bỏ cập nhật trạng thái từ file import
                    if (row.UnitId.HasValue) emp.UnitId = row.UnitId.Value;
                    if (row.PositionId.HasValue) emp.PositionId = row.PositionId.Value;
                    // Bỏ cập nhật vai trò từ file import

                    updated++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import error at row {Row}", idx);
                errors.Add($"Dòng {idx}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { inserted, updated, skipped, errors });
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
                EmployeeCode = (string?)null,
                CBCode = employee.CBCode,
                FullName = employee.FullName,
                Username = employee.Username,
                Email = employee.Email,
                UserAD = employee.UserAD,
                UserIPCAS = employee.UserIPCAS,
                MaCBTD = employee.MaCBTD,
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
                CBCode = employeeDto.CBCode ?? string.Empty,
                FullName = employeeDto.FullName,
                Username = employeeDto.Username,
                Email = employeeDto.Email,
                UserAD = employeeDto.UserAD,
                UserIPCAS = employeeDto.UserIPCAS,
                MaCBTD = employeeDto.MaCBTD,
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
                EmployeeCode = (string?)null,
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

    // DELETE: api/Employees/bulk
    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDeleteEmployees([FromBody] BulkDeleteRequest request)
    {
        try
        {
            if (request == null || request.Ids == null || request.Ids.Count == 0)
            {
                return BadRequest(new { message = "Danh sách ID không hợp lệ" });
            }

            // Get employees to delete
            var employees = await _context.Employees
                .Where(e => request.Ids.Contains(e.Id))
                .ToListAsync();

            if (employees.Count == 0)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên nào để xóa" });
            }

            // Check for admin users
            var adminEmployees = employees.Where(e => e.Username?.ToLower() == "admin").ToList();
            if (adminEmployees.Any())
            {
                return BadRequest(new { message = "Không thể xóa tài khoản admin" });
            }

            // Remove employees
            _context.Employees.RemoveRange(employees);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Bulk deleted {Count} employees", employees.Count);
            return Ok(new
            {
                message = $"Đã xóa {employees.Count} nhân viên",
                deletedCount = employees.Count,
                deletedIds = employees.Select(e => e.Id).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk delete employees");
            return StatusCode(500, new { message = "Lỗi khi xóa nhân viên", details = ex.Message });
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
            // GAHR26 business columns: UserAD, UserIPCAS, MaCBTD
            if (!string.IsNullOrWhiteSpace(dto.UserAD) && dto.UserAD != employee.UserAD)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "UserAD", OldValue = employee.UserAD, NewValue = dto.UserAD, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.UserAD = dto.UserAD;
            }
            else if (dto.UserAD == string.Empty && employee.UserAD != null)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "UserAD", OldValue = employee.UserAD, NewValue = null, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.UserAD = null;
            }
            if (!string.IsNullOrWhiteSpace(dto.UserIPCAS) && dto.UserIPCAS != employee.UserIPCAS)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "UserIPCAS", OldValue = employee.UserIPCAS, NewValue = dto.UserIPCAS, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.UserIPCAS = dto.UserIPCAS;
            }
            else if (dto.UserIPCAS == string.Empty && employee.UserIPCAS != null)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "UserIPCAS", OldValue = employee.UserIPCAS, NewValue = null, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.UserIPCAS = null;
            }
            if (!string.IsNullOrWhiteSpace(dto.MaCBTD) && dto.MaCBTD != employee.MaCBTD)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "MaCBTD", OldValue = employee.MaCBTD, NewValue = dto.MaCBTD, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.MaCBTD = dto.MaCBTD;
            }
            else if (dto.MaCBTD == string.Empty && employee.MaCBTD != null)
            {
                changes.Add(new EmployeeAuditLog { EmployeeId = id, Action = "UPDATE", FieldChanged = "MaCBTD", OldValue = employee.MaCBTD, NewValue = null, PerformedBy = dto.UpdatedBy ?? "system" });
                employee.MaCBTD = null;
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
                EmployeeCode = (string?)null,
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
    public string? CBCode { get; set; }
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? UserAD { get; set; }
    public string? UserIPCAS { get; set; }
    public string? MaCBTD { get; set; }
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
    public string? CBCode { get; set; }
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; } // treated as raw password input
    public string? Email { get; set; }
    public string? UserAD { get; set; }
    public string? UserIPCAS { get; set; }
    public string? MaCBTD { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public int? RoleId { get; set; }
    public bool? ClearRole { get; set; } // when true and RoleId null -> remove role
    public string? UpdatedBy { get; set; }
}

// === Bulk Import by CBCode DTOs ===
public class BulkImportByCBCodeRequest
{
    [Required(ErrorMessage = "Danh sách nhân viên không được rỗng")]
    [MinLength(1, ErrorMessage = "Cần có ít nhất 1 nhân viên")]
    public List<BulkImportByCBCodeRow> Rows { get; set; } = new();
    public bool OverwriteExisting { get; set; } = true;
    public bool AutoGenerateMissingUsernames { get; set; } = true;
}

public class BulkImportByCBCodeRow
{
    [Required(ErrorMessage = "Mã CB là bắt buộc")]
    [StringLength(9, MinimumLength = 9, ErrorMessage = "Mã CB phải có đúng 9 ký tự")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Mã CB phải gồm 9 chữ số")]
    public string? CBCode { get; set; }

    [StringLength(255, ErrorMessage = "Họ tên không được quá 255 ký tự")]
    public string? FullName { get; set; }

    [StringLength(50, ErrorMessage = "Username không được quá 50 ký tự")]
    public string? Username { get; set; }

    [StringLength(100, ErrorMessage = "Password không được quá 100 ký tự")]
    public string? Password { get; set; }

    [StringLength(100, ErrorMessage = "UserAD không được quá 100 ký tự")]
    public string? UserAD { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "UserIPCAS không được quá 100 ký tự")]
    public string? UserIPCAS { get; set; }

    [StringLength(50, ErrorMessage = "Mã CBTD không được quá 50 ký tự")]
    public string? MaCBTD { get; set; }

    [StringLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
    public string? PhoneNumber { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UnitId phải lớn hơn 0")]
    public int? UnitId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PositionId phải lớn hơn 0")]
    public int? PositionId { get; set; }
}

// === Bulk Delete DTO ===
public class BulkDeleteRequest
{
    public List<int> Ids { get; set; } = new();
}
