using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Dashboard;
using TinhKhoanApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BusinessPlanTargetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusinessPlanTargetController> _logger;

        public BusinessPlanTargetController(ApplicationDbContext context, ILogger<BusinessPlanTargetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessPlanTarget>>> GetTargets(
            [FromQuery] int? year,
            [FromQuery] int? quarter,
            [FromQuery] int? month,
            [FromQuery] int? unitId,
            [FromQuery] int? indicatorId,
            [FromQuery] string? status)
        {
            try
            {
                var query = _context.BusinessPlanTargets
                    .Include(t => t.Unit)
                    .Include(t => t.DashboardIndicator)
                    .Where(t => !t.IsDeleted);

                if (year.HasValue)
                    query = query.Where(t => t.Year == year.Value);

                if (quarter.HasValue)
                    query = query.Where(t => t.Quarter == quarter.Value);

                if (month.HasValue)
                    query = query.Where(t => t.Month == month.Value);

                if (unitId.HasValue)
                    query = query.Where(t => t.UnitId == unitId.Value);

                if (indicatorId.HasValue)
                    query = query.Where(t => t.DashboardIndicatorId == indicatorId.Value);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(t => t.Status == status);

                var targets = await query.OrderBy(t => t.Year)
                    .ThenBy(t => t.Quarter)
                    .ThenBy(t => t.Month)
                    .ThenBy(t => t.Unit!.Name)
                    .ToListAsync();

                return Ok(targets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business plan targets");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessPlanTarget>> GetTarget(int id)
        {
            try
            {
                var target = await _context.BusinessPlanTargets
                    .Include(t => t.Unit)
                    .Include(t => t.DashboardIndicator)
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (target == null)
                    return NotFound();

                return Ok(target);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business plan target {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BusinessPlanTarget>> CreateTarget(BusinessPlanTarget target)
        {
            try
            {
                // Validate unit exists
                var unit = await _context.Units.FindAsync(target.UnitId);
                if (unit == null)
                    return BadRequest("Unit not found");

                // Validate indicator exists
                var indicator = await _context.DashboardIndicators.FindAsync(target.DashboardIndicatorId);
                if (indicator == null)
                    return BadRequest("Dashboard indicator not found");

                // Check for duplicate target
                var existingTarget = await _context.BusinessPlanTargets
                    .FirstOrDefaultAsync(t => t.UnitId == target.UnitId &&
                                            t.DashboardIndicatorId == target.DashboardIndicatorId &&
                                            t.Year == target.Year &&
                                            t.Quarter == target.Quarter &&
                                            t.Month == target.Month &&
                                            !t.IsDeleted);

                if (existingTarget != null)
                    return BadRequest("Target already exists for this unit, indicator, and period");

                target.CreatedDate = DateTime.UtcNow;
                target.CreatedBy = User.Identity?.Name ?? "System";
                target.Status = target.Status ?? "Draft";

                _context.BusinessPlanTargets.Add(target);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTarget), new { id = target.Id }, target);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating business plan target");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarget(int id, BusinessPlanTarget target)
        {
            if (id != target.Id)
                return BadRequest();

            try
            {
                var existingTarget = await _context.BusinessPlanTargets
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (existingTarget == null)
                    return NotFound();

                // Update fields
                existingTarget.TargetValue = target.TargetValue;
                existingTarget.Notes = target.Notes;
                existingTarget.Status = target.Status;
                existingTarget.ModifiedDate = DateTime.UtcNow;
                existingTarget.ModifiedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating business plan target {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarget(int id)
        {
            try
            {
                var target = await _context.BusinessPlanTargets
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (target == null)
                    return NotFound();

                target.IsDeleted = true;
                target.ModifiedDate = DateTime.UtcNow;
                target.ModifiedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business plan target {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("batch")]
        public async Task<ActionResult> CreateBatchTargets([FromBody] List<BusinessPlanTarget> targets)
        {
            try
            {
                var createdTargets = new List<BusinessPlanTarget>();
                var errors = new List<string>();

                foreach (var target in targets)
                {
                    // Validate unit exists
                    var unit = await _context.Units.FindAsync(target.UnitId);
                    if (unit == null)
                    {
                        errors.Add($"Unit {target.UnitId} not found");
                        continue;
                    }

                    // Validate indicator exists
                    var indicator = await _context.DashboardIndicators.FindAsync(target.DashboardIndicatorId);
                    if (indicator == null)
                    {
                        errors.Add($"Dashboard indicator {target.DashboardIndicatorId} not found");
                        continue;
                    }

                    // Check for duplicate target
                    var existingTarget = await _context.BusinessPlanTargets
                        .FirstOrDefaultAsync(t => t.UnitId == target.UnitId &&
                                                t.DashboardIndicatorId == target.DashboardIndicatorId &&
                                                t.Year == target.Year &&
                                                t.Quarter == target.Quarter &&
                                                t.Month == target.Month &&
                                                !t.IsDeleted);

                    if (existingTarget != null)
                    {
                        errors.Add($"Target already exists for unit {target.UnitId}, indicator {target.DashboardIndicatorId}, period {target.Year}-{target.Quarter}-{target.Month}");
                        continue;
                    }

                    target.CreatedDate = DateTime.UtcNow;
                    target.CreatedBy = User.Identity?.Name ?? "System";
                    target.Status = target.Status ?? "Draft";

                    _context.BusinessPlanTargets.Add(target);
                    createdTargets.Add(target);
                }

                if (createdTargets.Any())
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new { 
                    CreatedCount = createdTargets.Count,
                    ErrorCount = errors.Count,
                    Errors = errors 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating batch business plan targets");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("distribute")]
        public async Task<ActionResult> DistributeTargets([FromBody] DistributeTargetsRequest request)
        {
            try
            {
                var parentUnit = await _context.Units.FindAsync(request.ParentUnitId);
                if (parentUnit == null)
                    return BadRequest("Parent unit not found");

                var indicator = await _context.DashboardIndicators.FindAsync(request.DashboardIndicatorId);
                if (indicator == null)
                    return BadRequest("Dashboard indicator not found");

                // Get child units
                var childUnits = await _context.Units
                    .Where(u => u.ParentId == request.ParentUnitId && !u.IsDeleted)
                    .ToListAsync();

                if (!childUnits.Any())
                    return BadRequest("No child units found for distribution");

                var createdTargets = new List<BusinessPlanTarget>();

                foreach (var distribution in request.Distributions)
                {
                    var childUnit = childUnits.FirstOrDefault(u => u.Id == distribution.UnitId);
                    if (childUnit == null)
                        continue;

                    // Check for existing target
                    var existingTarget = await _context.BusinessPlanTargets
                        .FirstOrDefaultAsync(t => t.UnitId == distribution.UnitId &&
                                                t.DashboardIndicatorId == request.DashboardIndicatorId &&
                                                t.Year == request.Year &&
                                                t.Quarter == request.Quarter &&
                                                t.Month == request.Month &&
                                                !t.IsDeleted);

                    if (existingTarget != null)
                    {
                        // Update existing target
                        existingTarget.TargetValue = distribution.TargetValue;
                        existingTarget.ModifiedDate = DateTime.UtcNow;
                        existingTarget.ModifiedBy = User.Identity?.Name ?? "System";
                    }
                    else
                    {
                        // Create new target
                        var newTarget = new BusinessPlanTarget
                        {
                            UnitId = distribution.UnitId,
                            DashboardIndicatorId = request.DashboardIndicatorId,
                            Year = request.Year,
                            Quarter = request.Quarter,
                            Month = request.Month,
                            TargetValue = distribution.TargetValue,
                            Notes = $"Distributed from {parentUnit.Name}",
                            Status = "Active",
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = User.Identity?.Name ?? "System"
                        };

                        _context.BusinessPlanTargets.Add(newTarget);
                        createdTargets.Add(newTarget);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    Message = "Targets distributed successfully",
                    CreatedCount = createdTargets.Count 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error distributing targets");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult> GetTargetSummary(
            [FromQuery] int year,
            [FromQuery] int? quarter,
            [FromQuery] int? month,
            [FromQuery] int? unitId)
        {
            try
            {
                var query = _context.BusinessPlanTargets
                    .Include(t => t.Unit)
                    .Include(t => t.DashboardIndicator)
                    .Where(t => !t.IsDeleted && t.Year == year);

                if (quarter.HasValue)
                    query = query.Where(t => t.Quarter == quarter.Value);

                if (month.HasValue)
                    query = query.Where(t => t.Month == month.Value);

                if (unitId.HasValue)
                    query = query.Where(t => t.UnitId == unitId.Value);

                var targets = await query.ToListAsync();

                var summary = targets
                    .GroupBy(t => new { t.DashboardIndicatorId, t.DashboardIndicator!.Name })
                    .Select(g => new
                    {
                        IndicatorId = g.Key.DashboardIndicatorId,
                        IndicatorName = g.Key.Name,
                        TotalTargets = g.Count(),
                        TotalValue = g.Sum(t => t.TargetValue),
                        AverageValue = g.Average(t => t.TargetValue),
                        ActiveTargets = g.Count(t => t.Status == "Active"),
                        DraftTargets = g.Count(t => t.Status == "Draft")
                    })
                    .ToList();

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting target summary");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class DistributeTargetsRequest
    {
        public int ParentUnitId { get; set; }
        public int DashboardIndicatorId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public List<TargetDistribution> Distributions { get; set; } = new();
    }

    public class TargetDistribution
    {
        public int UnitId { get; set; }
        public decimal TargetValue { get; set; }
    }
}
