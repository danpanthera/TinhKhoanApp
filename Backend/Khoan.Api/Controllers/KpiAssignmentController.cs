using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;
using Khoan.Api.Services;
using Khoan.Api.Contracts.KpiAssignments;

namespace Khoan.Api.Controllers
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
        /// Tìm kiếm (search) các giao khoán KPI của nhân viên với các tiêu chí tùy chọn
        /// </summary>
        /// <param name="employeeId">Lọc theo EmployeeId</param>
        /// <param name="periodId">Lọc theo KhoanPeriodId</param>
        /// <param name="unitId">Lọc theo UnitId của nhân viên</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployeeAssignments([FromQuery] int? employeeId, [FromQuery] int? periodId, [FromQuery] int? unitId)
        {
            try
            {
                var query = _context.EmployeeKpiAssignments
                    .Include(a => a.Employee).ThenInclude(e => e.Position)
                    .Include(a => a.KpiDefinition)
                    .Include(a => a.KhoanPeriod)
                    .AsQueryable();

                if (employeeId.HasValue)
                {
                    query = query.Where(a => a.EmployeeId == employeeId.Value);
                }

                if (periodId.HasValue)
                {
                    query = query.Where(a => a.KhoanPeriodId == periodId.Value);
                }

                if (unitId.HasValue)
                {
                    // Join qua bảng Employees để lọc theo đơn vị
                    query = query.Where(a => a.Employee.UnitId == unitId.Value);
                }

                var assignments = await query.ToListAsync();

                if (!assignments.Any())
                {
                    // Khi chưa có assignment nào: trả về danh sách nhân viên (theo unitId filter nếu có)
                    var employeeQuery = _context.Employees
                        .Include(e => e.Position)
                        .AsQueryable();

                    if (unitId.HasValue)
                    {
                        employeeQuery = employeeQuery.Where(e => e.UnitId == unitId.Value);
                    }

                    var employees = await employeeQuery
                        .OrderBy(e => e.FullName)
                        .Select(e => new
                        {
                            id = e.Id,
                            fullName = e.FullName,
                            unitId = e.UnitId,
                            positionName = e.Position != null ? e.Position.Name : null
                        })
                        .ToListAsync();

                    return Ok(new
                    {
                        assignments = new List<object>(),
                        employees,
                        message = "No KPI assignments yet"
                    });
                }

                // Có assignment: trả về danh sách chuẩn hoá
                var result = assignments.Select(a => new
                {
                    a.Id,
                    employeeId = a.EmployeeId,
                    employee = new
                    {
                        id = a.Employee.Id,
                        fullName = a.Employee.FullName,
                        unitId = a.Employee.UnitId,
                        positionName = a.Employee.Position != null ? a.Employee.Position.Name : null
                    },
                    khoanPeriodId = a.KhoanPeriodId,
                    khoanPeriod = a.KhoanPeriod == null ? null : new
                    {
                        id = a.KhoanPeriod.Id,
                        periodName = a.KhoanPeriod.Name,
                        startDate = a.KhoanPeriod.StartDate,
                        endDate = a.KhoanPeriod.EndDate
                    },
                    indicator = new
                    {
                        id = a.KpiDefinition.Id,
                        indicatorName = a.KpiDefinition.KpiName,
                        maxScore = a.KpiDefinition.MaxScore,
                        unit = a.KpiDefinition.UnitOfMeasure
                    },
                    targetValue = a.TargetValue,
                    actualValue = a.ActualValue,
                    score = a.Score,
                    a.Notes,
                    createdDate = a.CreatedDate,
                    updatedDate = a.UpdatedDate
                }).ToList();

                return Ok(new { assignments = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching KPI assignments (employeeId={EmployeeId}, periodId={PeriodId}, unitId={UnitId})", employeeId, periodId, unitId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật giá trị thực hiện (ActualValue) cho một giao khoán KPI của nhân viên
        /// </summary>
        [HttpPut("update-single-actual")]
        public async Task<IActionResult> UpdateSingleActual([FromBody] UpdateEmployeeActualRequest request)
        {
            if (request == null || request.AssignmentId <= 0)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            try
            {
                var assignment = await _context.EmployeeKpiAssignments
                    .Include(a => a.KpiDefinition)
                    .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);

                if (assignment == null)
                {
                    return NotFound(new { Message = "Assignment not found" });
                }

                assignment.ActualValue = request.ActualValue;
                assignment.UpdatedDate = DateTime.UtcNow;

                // Tính điểm đơn giản: (Actual / Target) * MaxScore (cắt ngưỡng)
                assignment.Score = KpiScoring.CalculateScore(assignment.TargetValue, assignment.ActualValue, assignment.KpiDefinition.MaxScore);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    assignment.Id,
                    assignment.ActualValue,
                    assignment.Score
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating actual value for assignment {AssignmentId}", request.AssignmentId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

}
