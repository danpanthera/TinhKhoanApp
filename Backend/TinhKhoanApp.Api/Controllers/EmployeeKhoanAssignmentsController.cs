using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using System.Security.Claims;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeKhoanAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeKhoanAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeKhoanAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeKhoanAssignment>>> GetEmployeeKhoanAssignments()
        {
            return await _context.EmployeeKhoanAssignments
                .Include(e => e.Employee)
                .Include(e => e.KhoanPeriod)
                .Include(e => e.AssignmentDetails)
                .ToListAsync();
        }

        // GET: api/EmployeeKhoanAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeKhoanAssignment>> GetEmployeeKhoanAssignment(int id)
        {
            var assignment = await _context.EmployeeKhoanAssignments
                .Include(e => e.Employee)
                .Include(e => e.KhoanPeriod)
                .Include(e => e.AssignmentDetails)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }
            return assignment;
        }

        // POST: api/EmployeeKhoanAssignments
        [HttpPost]
        public async Task<ActionResult<EmployeeKhoanAssignment>> PostEmployeeKhoanAssignment(EmployeeKhoanAssignment assignment)
        {
            _context.EmployeeKhoanAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetEmployeeKhoanAssignment", new { id = assignment.Id }, assignment);
        }

        // PUT: api/EmployeeKhoanAssignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeKhoanAssignment(int id, EmployeeKhoanAssignment assignment)
        {
            if (id != assignment.Id)
            {
                return BadRequest();
            }

            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeKhoanAssignmentExists(id))
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

        // DELETE: api/EmployeeKhoanAssignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeKhoanAssignment(int id)
        {
            var assignment = await _context.EmployeeKhoanAssignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.EmployeeKhoanAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/EmployeeKhoanAssignments/MyAssignment/{khoanPeriodId}
        [HttpGet("MyAssignment/{khoanPeriodId}")]
        public async Task<IActionResult> GetMyAssignment(int khoanPeriodId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Không tìm thấy thông tin người dùng.");
            if (!int.TryParse(userIdClaim.Value, out int userId)) return Unauthorized("ID người dùng không hợp lệ.");

            var assignment = await _context.EmployeeKhoanAssignments
                .Include(e => e.AssignmentDetails)
                .FirstOrDefaultAsync(e => e.EmployeeId == userId && e.KhoanPeriodId == khoanPeriodId);
            if (assignment == null)
            {
                return Ok(new { assignmentDetails = new List<object>() });
            }
            return Ok(new
            {
                assignmentDetails = assignment.AssignmentDetails.Select(d => new
                {
                    d.Id,
                    legacyKPICode = d.LegacyKPICode,
                    kpiName = d.LegacyKPIName,
                    d.TargetValue,
                    d.ActualValue
                }).ToList()
            });
        }

        // GET: api/EmployeeKhoanAssignments/ByEmployee/{employeeCode}/{khoanPeriodId}
        [HttpGet("ByEmployee/{employeeCode}/{khoanPeriodId}")]
        public async Task<IActionResult> GetByEmployee(string employeeCode, int khoanPeriodId)
        {
            var employee = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Unit)
                .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
            if (employee == null)
                return NotFound($"Không tìm thấy nhân viên với mã {employeeCode}");
            var assignment = await _context.EmployeeKhoanAssignments
                .Include(e => e.AssignmentDetails)
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.Id && e.KhoanPeriodId == khoanPeriodId);
            if (assignment == null)
            {
                // TODO: Logic CBType sẽ được cập nhật với 23 vai trò chuẩn mới
                // string cbType = (employee.Position?.Name ?? "") + "_" + (employee.Unit?.Code ?? "");
                string cbType = "TBD"; // To Be Determined với 23 vai trò mới
                return Ok(new {
                    cbType = cbType,
                    employeeName = employee.FullName,
                    employeeCode = employee.EmployeeCode,
                    assignmentDetails = new List<object>(),
                    note = "CBType đang được cập nhật với 23 vai trò chuẩn mới"
                });
            }
            return Ok(new
            {
                // TODO: Logic CBType sẽ được cập nhật với 23 vai trò chuẩn mới
                // cbType = (employee.Position?.Name ?? "") + "_" + (employee.Unit?.Code ?? ""),
                cbType = "TBD", // To Be Determined với 23 vai trò mới
                employeeName = employee.FullName,
                employeeCode = employee.EmployeeCode,
                note = "CBType đang được cập nhật với 23 vai trò chuẩn mới",
                assignmentDetails = assignment.AssignmentDetails.Select(d => new
                {
                    d.Id,
                    legacyKPICode = d.LegacyKPICode,
                    kpiName = d.LegacyKPIName,
                    d.TargetValue,
                    d.ActualValue
                }).ToList()
            });
        }

        private bool EmployeeKhoanAssignmentExists(int id)
        {
            return _context.EmployeeKhoanAssignments.Any(e => e.Id == id);
        }
    }
}
