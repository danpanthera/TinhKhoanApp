using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitKhoanAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UnitKhoanAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UnitKhoanAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitKhoanAssignment>>> GetUnitKhoanAssignments()
        {
            return await _context.UnitKhoanAssignments
                .Include(u => u.Unit)
                .Include(u => u.KhoanPeriod)
                .Include(u => u.AssignmentDetails)
                .ToListAsync();
        }

        // GET: api/UnitKhoanAssignments/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UnitKhoanAssignmentDetail>>> SearchUnitAssignments(int? unitId, int? periodId)
        {
            var query = _context.UnitKhoanAssignmentDetails
                .Include(d => d.UnitKhoanAssignment)
                .ThenInclude(a => a.Unit)
                .Include(d => d.UnitKhoanAssignment)
                .ThenInclude(a => a.KhoanPeriod)
                .AsQueryable();

            if (unitId.HasValue)
            {
                query = query.Where(d => d.UnitKhoanAssignment.UnitId == unitId.Value);
            }

            if (periodId.HasValue)
            {
                query = query.Where(d => d.UnitKhoanAssignment.KhoanPeriodId == periodId.Value);
            }

            var results = await query.ToListAsync();
            return Ok(results);
        }

        // GET: api/UnitKhoanAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitKhoanAssignment>> GetUnitKhoanAssignment(int id)
        {
            var assignment = await _context.UnitKhoanAssignments
                .Include(u => u.Unit)
                .Include(u => u.KhoanPeriod)
                .Include(u => u.AssignmentDetails)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }
            return assignment;
        }

        // POST: api/UnitKhoanAssignments
        [HttpPost]
        public async Task<ActionResult<UnitKhoanAssignment>> PostUnitKhoanAssignment(UnitKhoanAssignment assignment)
        {
            _context.UnitKhoanAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUnitKhoanAssignment), new { id = assignment.Id }, assignment);
        }

        // PUT: api/UnitKhoanAssignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnitKhoanAssignment(int id, UnitKhoanAssignment assignment)
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
                if (!_context.UnitKhoanAssignments.Any(u => u.Id == id))
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

        // DELETE: api/UnitKhoanAssignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitKhoanAssignment(int id)
        {
            var assignment = await _context.UnitKhoanAssignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            _context.UnitKhoanAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/UnitKhoanAssignments/update-actual
        [HttpPut("update-actual")]
        public async Task<IActionResult> UpdateActualValue([FromBody] UpdateUnitActualValueRequest request)
        {
            var assignmentDetail = await _context.UnitKhoanAssignmentDetails
                .Include(d => d.UnitKhoanAssignment)
                .FirstOrDefaultAsync(d => d.Id == request.AssignmentId);

            if (assignmentDetail == null)
            {
                return NotFound("Assignment detail not found");
            }

            assignmentDetail.ActualValue = request.ActualValue;
            
            // Calculate score based on target vs actual
            if (request.ActualValue.HasValue && assignmentDetail.TargetValue > 0)
            {
                assignmentDetail.Score = (request.ActualValue.Value / assignmentDetail.TargetValue) * 100;
            }
            else
            {
                assignmentDetail.Score = null;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Score = assignmentDetail.Score });
        }

        public class UpdateUnitActualValueRequest
        {
            public int AssignmentId { get; set; }
            public decimal? ActualValue { get; set; }
        }

        // POST: api/UnitKhoanAssignments/create-test-data
        [HttpPost("create-test-data")]
        public async Task<IActionResult> CreateTestData()
        {
            try
            {
                // Get first CNL1 unit (Lai Chau)
                var laiChauUnit = await _context.Units
                    .FirstOrDefaultAsync(u => u.Code == "CNLAICHAU" && u.Type == "CNL1");
                
                // Get first active period
                var activePeriod = await _context.KhoanPeriods
                    .FirstOrDefaultAsync(p => p.Status == PeriodStatus.OPEN);

                if (laiChauUnit == null || activePeriod == null)
                {
                    return BadRequest("Could not find Lai Chau unit or active period");
                }

                // Check if assignment already exists
                var existingAssignment = await _context.UnitKhoanAssignments
                    .FirstOrDefaultAsync(ua => ua.UnitId == laiChauUnit.Id && ua.KhoanPeriodId == activePeriod.Id);

                if (existingAssignment != null)
                {
                    return Ok(new { message = "Test data already exists", assignmentId = existingAssignment.Id });
                }

                // Create new assignment
                var assignment = new UnitKhoanAssignment
                {
                    UnitId = laiChauUnit.Id,
                    KhoanPeriodId = activePeriod.Id,
                    AssignedDate = DateTime.UtcNow,
                    Note = "Test assignment created via API"
                };

                _context.UnitKhoanAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                // Create assignment details (KPIs)
                var details = new List<UnitKhoanAssignmentDetail>
                {
                    new UnitKhoanAssignmentDetail
                    {
                        UnitKhoanAssignmentId = assignment.Id,
                        LegacyKPICode = "DT001",
                        LegacyKPIName = "Doanh thu huy động tiền gửi",
                        TargetValue = 50000000000,
                        Note = "Chỉ tiêu doanh thu huy động"
                    },
                    new UnitKhoanAssignmentDetail
                    {
                        UnitKhoanAssignmentId = assignment.Id,
                        LegacyKPICode = "DT002",
                        LegacyKPIName = "Doanh thu tín dụng",
                        TargetValue = 30000000000,
                        Note = "Chỉ tiêu doanh thu tín dụng"
                    },
                    new UnitKhoanAssignmentDetail
                    {
                        UnitKhoanAssignmentId = assignment.Id,
                        LegacyKPICode = "CP001",
                        LegacyKPIName = "Chi phí hoạt động",
                        TargetValue = 15000000000,
                        Note = "Chỉ tiêu chi phí hoạt động"
                    },
                    new UnitKhoanAssignmentDetail
                    {
                        UnitKhoanAssignmentId = assignment.Id,
                        LegacyKPICode = "LN001",
                        LegacyKPIName = "Lợi nhuận trước thuế",
                        TargetValue = 8000000000,
                        Note = "Chỉ tiêu lợi nhuận"
                    }
                };

                _context.UnitKhoanAssignmentDetails.AddRange(details);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "Test data created successfully", 
                    assignmentId = assignment.Id,
                    unitName = laiChauUnit.Name,
                    periodName = activePeriod.Name,
                    kpiCount = details.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
