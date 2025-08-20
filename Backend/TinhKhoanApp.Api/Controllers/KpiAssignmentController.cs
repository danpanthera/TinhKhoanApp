using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KpiAssignmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiAssignmentController> _logger;

        public KpiAssignmentController(ApplicationDbContext context, ILogger<KpiAssignmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Assign KPI targets to an employee for a specific period
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignKPI([FromBody] AssignKpiRequest request)
        {
            try
            {
                _logger.LogInformation("Assigning KPI for Employee {EmployeeId} in Period {PeriodId}",
                    request.EmployeeId, request.KhoanPeriodId);

                // Validate employee exists
                var employee = await _context.Employees.FindAsync(request.EmployeeId);
                if (employee == null)
                {
                    return BadRequest($"Employee with ID {request.EmployeeId} not found");
                }

                // Validate period exists
                var period = await _context.KhoanPeriods.FindAsync(request.KhoanPeriodId);
                if (period == null)
                {
                    return BadRequest($"Khoan Period with ID {request.KhoanPeriodId} not found");
                }

                // Remove existing assignments for this employee and period
                var existingAssignments = await _context.EmployeeKpiAssignments
                    .Where(e => e.EmployeeId == request.EmployeeId &&
                               e.KhoanPeriodId == request.KhoanPeriodId)
                    .ToListAsync();

                if (existingAssignments.Any())
                {
                    _context.EmployeeKpiAssignments.RemoveRange(existingAssignments);
                    _logger.LogInformation("Removed {Count} existing assignments", existingAssignments.Count);
                }

                // Create new assignments
                var newAssignments = new List<EmployeeKpiAssignment>();
                foreach (var target in request.Targets)
                {
                    // Validate indicator exists
                    var indicator = await _context.KpiIndicators.FindAsync(target.IndicatorId);
                    if (indicator == null)
                    {
                        _logger.LogWarning("Indicator {IndicatorId} not found, skipping", target.IndicatorId);
                        continue;
                    }

                    // Create or get KPIDefinition for this KpiIndicator
                    var kpiDefinition = await GetOrCreateKPIDefinition(indicator);

                    var assignment = new EmployeeKpiAssignment
                    {
                        EmployeeId = request.EmployeeId,
                        KpiDefinitionId = kpiDefinition.Id,
                        KhoanPeriodId = request.KhoanPeriodId,
                        TargetValue = target.TargetValue,
                        Notes = target.Notes,
                        CreatedDate = DateTime.UtcNow
                    };

                    newAssignments.Add(assignment);
                }

                if (newAssignments.Any())
                {
                    _context.EmployeeKpiAssignments.AddRange(newAssignments);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Successfully assigned {Count} KPI targets to Employee {EmployeeId}",
                        newAssignments.Count, request.EmployeeId);

                    return Ok(new
                    {
                        Success = true,
                        Message = $"Successfully assigned {newAssignments.Count} KPI targets",
                        AssignmentCount = newAssignments.Count
                    });
                }
                else
                {
                    return BadRequest("No valid KPI indicators found in the request");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning KPI for Employee {EmployeeId}", request.EmployeeId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get KPI assignments for an employee in a specific period
        /// </summary>
        [HttpGet("employee/{employeeId}/period/{periodId}")]
        public async Task<IActionResult> GetEmployeeKpiAssignments(int employeeId, int periodId)
        {
            try
            {
                var assignments = await _context.EmployeeKpiAssignments
                    .Include(e => e.Employee)
                    .Include(e => e.KpiDefinition)
                    .Include(e => e.KhoanPeriod)
                    .Where(e => e.EmployeeId == employeeId && e.KhoanPeriodId == periodId)
                    .ToListAsync();

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting KPI assignments for Employee {EmployeeId}", employeeId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get or create KPIDefinition based on KpiIndicator
        /// </summary>
        private async Task<KPIDefinition> GetOrCreateKPIDefinition(KpiIndicator indicator)
        {
            // Look for existing KPIDefinition by name
            var existing = await _context.KPIDefinitions
                .FirstOrDefaultAsync(k => k.KpiName == indicator.IndicatorName);

            if (existing != null)
                return existing;

            // Create new KPIDefinition
            var kpiDefinition = new KPIDefinition
            {
                KpiCode = $"KPI_{indicator.Id}",
                KpiName = indicator.IndicatorName,
                Description = $"Tự động tạo từ KpiIndicator ID: {indicator.Id}",
                ValueType = KpiValueType.NUMBER, // Default type
                MaxScore = indicator.MaxScore,
                UnitOfMeasure = indicator.Unit,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.KPIDefinitions.Add(kpiDefinition);
            await _context.SaveChangesAsync();
            return kpiDefinition;
        }
    }

    /// <summary>
    /// Request model for assigning KPI targets
    /// </summary>
    public class AssignKpiRequest
    {
        public int EmployeeId { get; set; }
        public int KhoanPeriodId { get; set; }
        public List<KpiTargetRequest> Targets { get; set; } = new List<KpiTargetRequest>();
    }

    /// <summary>
    /// Individual KPI target in the assignment request
    /// </summary>
    public class KpiTargetRequest
    {
        public int IndicatorId { get; set; }
        public decimal TargetValue { get; set; }
        public string? Notes { get; set; }
    }
}
