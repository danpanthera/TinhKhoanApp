using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Maintenance/delete-units-roles
        [HttpPost("delete-units-roles")]
        public async Task<IActionResult> DeleteUnitsAndRoles()
        {
            try
            {
                // 1. Delete EmployeeRoles if exists (without foreign key constraint issues)
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM EmployeeRoles");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EmployeeRoles deletion skipped: {ex.Message}");
                }

                // 2. Delete Employee KPI Assignments if exists
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM EmployeeKpiAssignments");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EmployeeKpiAssignments deletion skipped: {ex.Message}");
                }

                // 3. Delete Branch KPI Assignments if exists
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM BranchKpiAssignments");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BranchKpiAssignments deletion skipped: {ex.Message}");
                }

                // 4. Update Employees to remove Unit references (no CASCADE DELETE)
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("UPDATE Employees SET UnitId = NULL WHERE UnitId IS NOT NULL");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Employees update skipped: {ex.Message}");
                }

                // 5. Delete child Units first (those with ParentUnitId)
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Units WHERE ParentUnitId IS NOT NULL");

                // 6. Delete parent Units
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Units WHERE ParentUnitId IS NULL");

                // 7. Delete all Roles
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Roles");

                // 8. Reset identity columns
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Units', RESEED, 0)");
                    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Roles', RESEED, 0)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Identity reset skipped: {ex.Message}");
                }

                return Ok(new { message = "Units and Roles data deleted successfully", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error deleting data: {ex.Message}", success = false });
            }
        }

        // POST: api/Maintenance/backup-units-roles
        [HttpPost("backup-units-roles")]
        public async Task<IActionResult> BackupUnitsAndRoles()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                await _context.Database.ExecuteSqlRawAsync($"SELECT * INTO UnitsBackup_{timestamp} FROM Units");
                await _context.Database.ExecuteSqlRawAsync($"SELECT * INTO RolesBackup_{timestamp} FROM Roles");

                try
                {
                    await _context.Database.ExecuteSqlRawAsync($"SELECT * INTO EmployeeRolesBackup_{timestamp} FROM EmployeeRoles");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EmployeeRoles backup skipped: {ex.Message}");
                }

                return Ok(new { message = $"Backup created with timestamp {timestamp}", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error creating backup: {ex.Message}", success = false });
            }
        }
    }
}
