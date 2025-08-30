using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.DTO;
using System.Text.RegularExpressions;

namespace TinhKhoanApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Gahr26ImportController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public Gahr26ImportController(ApplicationDbContext db) => _db = db;

    [HttpPost("bulk-employees")] // POST /api/Gahr26Import/bulk-employees
    public async Task<ActionResult<Gahr26BulkImportResult>> BulkUpsertEmployees([FromBody] Gahr26BulkImportRequest request, CancellationToken ct)
    {
        var result = new Gahr26BulkImportResult();
        if (request?.Rows == null || request.Rows.Count == 0)
        {
            result.Errors.Add("Không có dòng nào để import");
            return BadRequest(result);
        }

        var existing = await _db.Employees.AsNoTracking().ToListAsync(ct);
        var map = existing.ToDictionary(e => e.CBCode, StringComparer.OrdinalIgnoreCase);

        foreach (var row in request.Rows)
        {
            if (string.IsNullOrWhiteSpace(row.CBCode))
            {
                result.Errors.Add("Thiếu Mã CB ở một dòng");
                result.Skipped++;
                continue;
            }
            if (!Regex.IsMatch(row.CBCode!, "^\\d{9}$"))
            {
                result.Errors.Add($"Mã CB không hợp lệ (phải 9 chữ số): {row.CBCode}");
                result.Skipped++;
                continue;
            }

            if (map.TryGetValue(row.CBCode, out var existingEmp))
            {
                if (!request.OverwriteExisting)
                {
                    result.Skipped++;
                    continue;
                }
                existingEmp.CBCode = row.CBCode;
                existingEmp.FullName = row.FullName ?? existingEmp.FullName;
                existingEmp.Username = row.Username ?? existingEmp.Username;
                existingEmp.Email = row.Email ?? existingEmp.Email;
                existingEmp.PhoneNumber = row.PhoneNumber ?? existingEmp.PhoneNumber;
                existingEmp.UserAD = row.UserAD ?? existingEmp.UserAD;
                existingEmp.UserIPCAS = row.UserIPCAS ?? existingEmp.UserIPCAS;
                existingEmp.MaCBTD = row.MaCBTD ?? existingEmp.MaCBTD;
                if (row.IsActive.HasValue) existingEmp.IsActive = row.IsActive.Value;
                if (row.UnitId.HasValue) existingEmp.UnitId = row.UnitId.Value;
                if (row.PositionId.HasValue) existingEmp.PositionId = row.PositionId.Value;
                if (row.RoleId.HasValue)
                {
                    var currentRoles = _db.EmployeeRoles.Where(er => er.EmployeeId == existingEmp.Id);
                    _db.EmployeeRoles.RemoveRange(currentRoles);
                    _db.EmployeeRoles.Add(new EmployeeRole { EmployeeId = existingEmp.Id, RoleId = row.RoleId.Value });
                }
                _db.Employees.Update(existingEmp);
                result.Updated++;
            }
            else
            {
                var username = row.Username;
                if (string.IsNullOrWhiteSpace(username) && request.AutoGenerateMissingUsernames)
                {
                    username = row.CBCode.ToLowerInvariant();
                }
                if (string.IsNullOrWhiteSpace(username))
                {
                    result.Errors.Add($"Không thể tạo nhân viên vì thiếu Username cho mã CB {row.CBCode}");
                    result.Skipped++;
                    continue;
                }
                if (string.IsNullOrWhiteSpace(row.FullName))
                {
                    row.FullName = row.CBCode;
                }
                var unitId = row.UnitId ?? existing.FirstOrDefault()?.UnitId ?? 1;
                var positionId = row.PositionId ?? existing.FirstOrDefault()?.PositionId ?? 1;
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
                var emp = new Employee
                {
                    CBCode = row.CBCode,
                    FullName = row.FullName!,
                    Username = username,
                    PasswordHash = passwordHash,
                    Email = row.Email,
                    PhoneNumber = row.PhoneNumber,
                    UserAD = row.UserAD,
                    UserIPCAS = row.UserIPCAS,
                    MaCBTD = row.MaCBTD,
                    IsActive = row.IsActive ?? true,
                    UnitId = unitId,
                    PositionId = positionId,
                };
                _db.Employees.Add(emp);
                await _db.SaveChangesAsync(ct);
                if (row.RoleId.HasValue)
                {
                    _db.EmployeeRoles.Add(new EmployeeRole { EmployeeId = emp.Id, RoleId = row.RoleId.Value });
                }
                map[row.CBCode] = emp;
                result.Inserted++;
            }
        }

        await _db.SaveChangesAsync(ct);
        return Ok(result);
    }
}
