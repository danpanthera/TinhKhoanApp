using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitKpiScoringController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UnitKpiScoringService _scoringService;

        public UnitKpiScoringController(ApplicationDbContext context, UnitKpiScoringService scoringService)
        {
            _context = context;
            _scoringService = scoringService;
        }

        // GET: api/UnitKpiScoring
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUnitKpiScorings()
        {
            try
            {
                // Query with KhoanPeriod information
                var scorings = await _context.UnitKpiScorings
                    .Select(s => new
                    {
                        s.Id,
                        s.UnitId,
                        s.KhoanPeriodId,
                        s.TotalScore,
                        s.BaseScore,
                        s.AdjustmentScore,
                        s.Notes,
                        s.ScoredBy,
                        s.CreatedAt,
                        UnitName = s.UnitId == 1 ? "Agribank Lai Châu" : 
                                  s.UnitId == 2 ? "Chi nhánh chính" : 
                                  s.UnitId == 3 ? "Phòng giao dịch 01" : "Unknown",
                        // Get KhoanPeriod name from context
                        KhoanPeriodName = _context.KhoanPeriods
                            .Where(kp => kp.Id == s.KhoanPeriodId)
                            .Select(kp => kp.Name)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return Ok(scorings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error loading unit KPI scorings", error = ex.Message });
            }
        }

        // GET: api/UnitKpiScoring/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitKpiScoring>> GetUnitKpiScoring(int id)
        {
            var unitKpiScoring = await _context.UnitKpiScorings
                .Include(s => s.Unit)
                .Include(s => s.KhoanPeriod)
                .Include(s => s.ScoringDetails)
                .ThenInclude(d => d.KpiDefinition)
                .Include(s => s.ScoringCriteria)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (unitKpiScoring == null)
            {
                return NotFound();
            }

            return unitKpiScoring;
        }

        // GET: api/UnitKpiScoring/unit/{unitId}/period/{periodId}
        [HttpGet("unit/{unitId}/period/{periodId}")]
        public async Task<ActionResult<UnitKpiScoring>> GetUnitKpiScoringByUnitAndPeriod(int unitId, int periodId)
        {
            var unitKpiScoring = await _context.UnitKpiScorings
                .Include(s => s.Unit)
                .Include(s => s.KhoanPeriod)
                .Include(s => s.ScoringDetails)
                .ThenInclude(d => d.KpiDefinition)
                .Include(s => s.ScoringCriteria)
                .FirstOrDefaultAsync(s => s.UnitId == unitId && s.KhoanPeriodId == periodId);

            if (unitKpiScoring == null)
            {
                return NotFound();
            }

            return unitKpiScoring;
        }

        // POST: api/UnitKpiScoring
        [HttpPost]
        public async Task<ActionResult<UnitKpiScoring>> PostUnitKpiScoring(CreateUnitKpiScoringRequest request)
        {
            try
            {
                var unitKpiScoring = await _scoringService.CreateScoringAsync(
                    request.UnitKhoanAssignmentId,
                    request.KhoanPeriodId,
                    request.UnitId,
                    request.ScoredBy ?? "System"
                );

                return CreatedAtAction("GetUnitKpiScoring", new { id = unitKpiScoring.Id }, unitKpiScoring);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/UnitKpiScoring/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnitKpiScoring(int id, UpdateUnitKpiScoringRequest request)
        {
            try
            {
                var existingScoring = await _context.UnitKpiScorings
                    .Include(s => s.ScoringDetails)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (existingScoring == null)
                {
                    return NotFound();
                }

                // Update basic properties
                existingScoring.Notes = request.Notes ?? existingScoring.Notes;
                existingScoring.UpdatedAt = DateTime.UtcNow;

                // Update actual values if provided
                if (request.ActualValues != null && request.ActualValues.Any())
                {
                    foreach (var actualValue in request.ActualValues)
                    {
                        var detail = existingScoring.ScoringDetails
                            .FirstOrDefault(d => d.KpiDefinitionId == actualValue.Key);
                        
                        if (detail != null)
                        {
                            detail.ActualValue = actualValue.Value;
                            
                            // Recalculate score for this KPI
                            var kpiDefinition = await _context.KPIDefinitions
                                .FirstOrDefaultAsync(k => k.Id == actualValue.Key);
                            
                            if (kpiDefinition != null)
                            {
                                detail.Score = await _scoringService.CalculateQuantitativeScore(
                                    kpiDefinition.KpiName, 
                                    detail.TargetValue, 
                                    actualValue.Value, 
                                    kpiDefinition.MaxScore);
                            }
                        }
                    }

                    // Recalculate total scores
                    existingScoring.BaseScore = existingScoring.ScoringDetails.Sum(d => d.Score);
                    existingScoring.TotalScore = existingScoring.BaseScore + existingScoring.AdjustmentScore;
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/UnitKpiScoring/5/compliance
        [HttpPost("{id}/compliance")]
        public async Task<IActionResult> UpdateComplianceViolations(int id, UpdateComplianceRequest request)
        {
            try
            {
                var allViolations = new List<UnitKpiScoringCriteria>();
                
                // Add process violations
                foreach (var violation in request.ProcessViolations)
                {
                    violation.ViolationType = "ProcessViolation";
                    allViolations.Add(violation);
                }
                
                // Add culture violations  
                foreach (var violation in request.CultureViolations)
                {
                    violation.ViolationType = "CultureViolation";
                    allViolations.Add(violation);
                }

                await _scoringService.UpdateComplianceViolationsAsync(id, allViolations);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/UnitKpiScoring/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitKpiScoring(int id)
        {
            var unitKpiScoring = await _context.UnitKpiScorings
                .Include(s => s.ScoringDetails)
                .Include(s => s.ScoringCriteria)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (unitKpiScoring == null)
            {
                return NotFound();
            }

            _context.UnitKpiScorings.Remove(unitKpiScoring);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/UnitKpiScoring/period/{periodId}/summary
        [HttpGet("period/{periodId}/summary")]
        public async Task<ActionResult<IEnumerable<UnitKpiScoringSummary>>> GetPeriodScoringSummary(int periodId)
        {
            var summary = await _context.UnitKpiScorings
                .Where(s => s.KhoanPeriodId == periodId)
                .Include(s => s.Unit)
                .Include(s => s.ScoringCriteria)
                .Select(s => new UnitKpiScoringSummary
                {
                    UnitId = s.UnitId,
                    UnitName = s.Unit.Name,
                    BaseScore = s.BaseScore,
                    AdjustmentScore = s.AdjustmentScore,
                    TotalScore = s.TotalScore,
                    ProcessViolationCount = s.ScoringCriteria.Where(c => c.ViolationType == "ProcessViolation").Sum(c => c.ViolationCount),
                    CultureViolationCount = s.ScoringCriteria.Where(c => c.ViolationType == "CultureViolation").Sum(c => c.ViolationCount),
                    LastUpdated = s.UpdatedAt ?? s.CreatedAt
                })
                .OrderByDescending(s => s.TotalScore)
                .ToListAsync();

            return summary;
        }

        // GET: api/UnitKpiScoring/kpi-templates/{unitId}
        [HttpGet("kpi-templates/{unitId}")]
        public async Task<ActionResult<IEnumerable<KpiTemplate>>> GetKpiTemplatesForUnit(int unitId)
        {
            var unit = await _context.Units.FindAsync(unitId);
            if (unit == null)
            {
                return NotFound("Unit not found");
            }

            var kpiDefinitions = await _context.KPIDefinitions
                .Where(k => k.IsActive) // Just get active KPIs for now
                .Select(k => new KpiTemplate
                {
                    Id = k.Id,
                    Name = k.KpiName,
                    TargetValue = 0, // This would come from assignment
                    ScoringRule = k.Description ?? "",
                    MaxScore = k.MaxScore,
                    IsQuantitative = k.ValueType == KpiValueType.NUMBER || k.ValueType == KpiValueType.CURRENCY
                })
                .ToListAsync();

            return kpiDefinitions;
        }
    }

    // Request/Response DTOs
    public class CreateUnitKpiScoringRequest
    {
        public int UnitKhoanAssignmentId { get; set; }
        public int UnitId { get; set; }
        public int KhoanPeriodId { get; set; }
        public string? ScoredBy { get; set; }
        public Dictionary<int, decimal> ActualValues { get; set; } = new();
        public string? Note { get; set; }
    }

    public class UpdateUnitKpiScoringRequest
    {
        public Dictionary<int, decimal>? ActualValues { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateComplianceRequest
    {
        public List<UnitKpiScoringCriteria> ProcessViolations { get; set; } = new();
        public List<UnitKpiScoringCriteria> CultureViolations { get; set; } = new();
    }

    public class ViolationRecord
    {
        public string Level { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class UnitKpiScoringSummary
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal BaseScore { get; set; }
        public decimal AdjustmentScore { get; set; }
        public decimal TotalScore { get; set; }
        public int ProcessViolationCount { get; set; }
        public int CultureViolationCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class KpiTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public string ScoringRule { get; set; } = string.Empty;
        public decimal MaxScore { get; set; }
        public bool IsQuantitative { get; set; }
    }
}
