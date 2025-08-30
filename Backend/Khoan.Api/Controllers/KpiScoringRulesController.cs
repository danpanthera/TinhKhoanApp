using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KpiScoringRulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiScoringRulesController> _logger;

        public KpiScoringRulesController(ApplicationDbContext context, ILogger<KpiScoringRulesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all KPI scoring rules
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KpiScoringRule>>> GetKpiScoringRules()
        {
            try
            {
                var rules = await _context.KpiScoringRules
                    .OrderBy(r => r.KpiIndicatorName)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} KPI scoring rules", rules.Count);
                return Ok(rules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI scoring rules");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get KPI scoring rule by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<KpiScoringRule>> GetKpiScoringRule(int id)
        {
            try
            {
                var rule = await _context.KpiScoringRules.FindAsync(id);

                if (rule == null)
                {
                    return NotFound();
                }

                return Ok(rule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI scoring rule with ID {Id}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update KPI scoring rule
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKpiScoringRule(int id, KpiScoringRule kpiScoringRule)
        {
            if (id != kpiScoringRule.Id)
            {
                return BadRequest();
            }

            _context.Entry(kpiScoringRule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated KPI scoring rule: {RuleName}", kpiScoringRule.KpiIndicatorName);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KpiScoringRuleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating KPI scoring rule with ID {Id}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }

            return NoContent();
        }

        /// <summary>
        /// Create new KPI scoring rule
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<KpiScoringRule>> PostKpiScoringRule(KpiScoringRule kpiScoringRule)
        {
            try
            {
                _context.KpiScoringRules.Add(kpiScoringRule);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new KPI scoring rule: {RuleName}", kpiScoringRule.KpiIndicatorName);
                return CreatedAtAction("GetKpiScoringRule", new { id = kpiScoringRule.Id }, kpiScoringRule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating KPI scoring rule");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete KPI scoring rule
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKpiScoringRule(int id)
        {
            try
            {
                var kpiScoringRule = await _context.KpiScoringRules.FindAsync(id);
                if (kpiScoringRule == null)
                {
                    return NotFound();
                }

                _context.KpiScoringRules.Remove(kpiScoringRule);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted KPI scoring rule: {RuleName}", kpiScoringRule.KpiIndicatorName);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting KPI scoring rule with ID {Id}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private bool KpiScoringRuleExists(int id)
        {
            return _context.KpiScoringRules.Any(e => e.Id == id);
        }
    }
}
