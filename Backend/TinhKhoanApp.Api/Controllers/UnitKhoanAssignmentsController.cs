using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitKhoanAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitKhoanAssignmentsController> _logger;

        public UnitKhoanAssignmentsController(ApplicationDbContext context, ILogger<UnitKhoanAssignmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Search unit KPI assignments by unit and period
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? unitId, [FromQuery] int? periodId)
        {
            try
            {
                var query = _context.UnitKhoanAssignments
                    .Include(u => u.Unit)
                    .Include(u => u.KhoanPeriod)
                    .Include(u => u.AssignmentDetails)
                    .AsQueryable();

                if (unitId.HasValue)
                {
                    query = query.Where(x => x.UnitId == unitId.Value);
                }

                if (periodId.HasValue)
                {
                    query = query.Where(x => x.KhoanPeriodId == periodId.Value);
                }

                var data = await query
                    .SelectMany(a => a.AssignmentDetails.Select(d => new
                    {
                        Id = d.Id,
                        assignmentId = a.Id,
                        unitId = a.UnitId,
                        unitName = a.Unit != null ? a.Unit.Name : null,
                        khoanPeriodId = a.KhoanPeriodId,
                        periodName = a.KhoanPeriod != null ? a.KhoanPeriod.Name : null,
                        legacyKPICode = d.LegacyKPICode,
                        legacyKPIName = d.LegacyKPIName,
                        targetValue = d.TargetValue,
                        actualValue = d.ActualValue,
                        score = d.Score,
                        assignedDate = a.AssignedDate,
                        note = d.Note
                    }))
                    .ToListAsync();

                if (!data.Any())
                {
                    return NotFound(new { Message = "No unit KPI assignments found" });
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching unit KPI assignments (unitId={UnitId}, periodId={PeriodId})", unitId, periodId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update single actual value for a unit KPI assignment detail
        /// </summary>
        [HttpPut("update-actual")]
        public async Task<IActionResult> UpdateActual([FromBody] UpdateUnitActualRequest request)
        {
            if (request == null || request.AssignmentDetailId <= 0)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            try
            {
                var detail = await _context.UnitKhoanAssignmentDetails.FirstOrDefaultAsync(d => d.Id == request.AssignmentDetailId);
                if (detail == null)
                {
                    return NotFound(new { Message = "Assignment detail not found" });
                }

                detail.ActualValue = request.ActualValue;
                // Simple scoring formula: completion % * 100 (cap at 100)
                if (detail.TargetValue > 0 && detail.ActualValue.HasValue)
                {
                    var completion = (double)detail.ActualValue.Value / (double)detail.TargetValue;
                    if (completion > 1) completion = 1;
                    detail.Score = (decimal)Math.Round(completion * 100, 2);
                }
                else
                {
                    detail.Score = null;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    detail.Id,
                    detail.ActualValue,
                    detail.Score
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit assignment detail actual value {DetailId}", request.AssignmentDetailId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class UpdateUnitActualRequest
    {
        public int AssignmentDetailId { get; set; }
        public decimal? ActualValue { get; set; }
    }
}
