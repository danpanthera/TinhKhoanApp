using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Dashboard;
using TinhKhoanApp.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Kích hoạt authentication cho Dashboard
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DashboardCalculationService _calculationService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ApplicationDbContext context,
            DashboardCalculationService calculationService,
            ILogger<DashboardController> logger)
        {
            _context = context;
            _calculationService = calculationService;
            _logger = logger;
        }

        [HttpGet("test-auth")]
        [Authorize] // Test authentication
        public IActionResult TestAuth()
        {
            return Ok(new { message = "Authentication successful", user = User.Identity?.Name });
        }

        [HttpGet("indicators")]
        [Authorize] // Bắt buộc authentication cho endpoint này
        public async Task<ActionResult<IEnumerable<DashboardIndicator>>> GetIndicators()
        {
            try
            {
                var indicators = await _context.DashboardIndicators
                    .Where(i => !i.IsDeleted)
                    .OrderBy(i => i.SortOrder)
                    .ToListAsync();

                return Ok(indicators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard indicators");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("calculate")]
        public async Task<ActionResult> CalculateIndicators([FromBody] CalculationRequest request)
        {
            try
            {
                var results = new List<DashboardCalculation>();

                foreach (var indicatorId in request.IndicatorIds)
                {
                    var calculation = await _calculationService.CalculateIndicatorAsync(
                        indicatorId,
                        request.UnitId,
                        request.Year,
                        request.Quarter,
                        request.Month,
                        request.UserId ?? User.Identity?.Name ?? "System"
                    );

                    if (calculation != null)
                    {
                        results.Add(calculation);
                    }
                }

                return Ok(new
                {
                    Message = "Calculation completed",
                    Results = results,
                    CalculatedCount = results.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating indicators");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("calculations")]
        public async Task<ActionResult<IEnumerable<DashboardCalculation>>> GetCalculations(
            [FromQuery] int? indicatorId,
            [FromQuery] int? unitId,
            [FromQuery] int? year,
            [FromQuery] int? quarter,
            [FromQuery] int? month,
            [FromQuery] string? status)
        {
            try
            {
                var query = _context.DashboardCalculations
                    .Include(c => c.DashboardIndicator)
                    .Include(c => c.Unit)
                    .Where(c => !c.IsDeleted);

                if (indicatorId.HasValue)
                    query = query.Where(c => c.DashboardIndicatorId == indicatorId.Value);

                if (unitId.HasValue)
                    query = query.Where(c => c.UnitId == unitId.Value);

                if (year.HasValue)
                    query = query.Where(c => c.Year == year.Value);

                if (quarter.HasValue)
                    query = query.Where(c => c.Quarter == quarter.Value);

                if (month.HasValue)
                    query = query.Where(c => c.Month == month.Value);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(c => c.Status == status);

                var calculations = await query
                    .OrderByDescending(c => c.CalculationDate)
                    .ToListAsync();

                return Ok(calculations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving calculations");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("dashboard-data")]
        public async Task<ActionResult> GetDashboardData(
            [FromQuery] int year,
            [FromQuery] int? quarter,
            [FromQuery] int? month,
            [FromQuery] int? unitId,
            [FromQuery] string? unitCode) // Thêm unitCode để accept string từ frontend
        {
            try
            {
                // Get indicators
                var indicators = await _context.DashboardIndicators
                    .Where(i => !i.IsDeleted)
                    .OrderBy(i => i.SortOrder)
                    .ToListAsync();

                // Get targets
                var targetsQuery = _context.BusinessPlanTargets
                    .Include(t => t.Unit)
                    .Include(t => t.DashboardIndicator)
                    .Where(t => !t.IsDeleted && t.Year == year);

                if (quarter.HasValue)
                    targetsQuery = targetsQuery.Where(t => t.Quarter == quarter.Value);

                if (month.HasValue)
                    targetsQuery = targetsQuery.Where(t => t.Month == month.Value);

                // Filter by unitId (int) hoặc unitCode (string)
                if (unitId.HasValue)
                    targetsQuery = targetsQuery.Where(t => t.UnitId == unitId.Value);
                else if (!string.IsNullOrEmpty(unitCode))
                {
                    // Map unitCode sang unitId hoặc filter theo unit name/code
                    // Tạm thời log và skip filter để không crash
                    _logger.LogInformation("🔧 Dashboard API called with unitCode: {UnitCode}, skipping unit filter", unitCode);
                }

                var targets = await targetsQuery.ToListAsync();

                // Get calculations (actual values)
                var calculationsQuery = _context.DashboardCalculations
                    .Include(c => c.Unit)
                    .Include(c => c.DashboardIndicator)
                    .Where(c => !c.IsDeleted && c.Year == year && c.Status == "Completed");

                if (quarter.HasValue)
                    calculationsQuery = calculationsQuery.Where(c => c.Quarter == quarter.Value);

                if (month.HasValue)
                    calculationsQuery = calculationsQuery.Where(c => c.Month == month.Value);

                // Filter calculations by unitId (int) hoặc unitCode (string)
                if (unitId.HasValue)
                    calculationsQuery = calculationsQuery.Where(c => c.UnitId == unitId.Value);
                else if (!string.IsNullOrEmpty(unitCode))
                {
                    // Tạm thời skip filter cho calculations, cần mapping unitCode -> unitId
                    _logger.LogInformation("🔧 Calculations query: skipping unitCode filter for now");
                }

                var calculations = await calculationsQuery.ToListAsync();

                // Combine data
                var dashboardData = indicators.Select(indicator => new
                {
                    Indicator = indicator,
                    Targets = targets.Where(t => t.DashboardIndicatorId == indicator.Id).ToList(),
                    Actuals = calculations.Where(c => c.DashboardIndicatorId == indicator.Id).ToList(),
                    Summary = new
                    {
                        TotalTarget = targets.Where(t => t.DashboardIndicatorId == indicator.Id).Sum(t => t.TargetValue),
                        TotalActual = calculations.Where(c => c.DashboardIndicatorId == indicator.Id).Sum(c => c.ActualValue ?? 0),
                        Achievement = calculations.Where(c => c.DashboardIndicatorId == indicator.Id).Sum(c => c.ActualValue ?? 0) /
                                    Math.Max(targets.Where(t => t.DashboardIndicatorId == indicator.Id).Sum(t => t.TargetValue), 1) * 100,
                        UnitsCount = unitId.HasValue ? 1 :
                                   targets.Where(t => t.DashboardIndicatorId == indicator.Id).Select(t => t.UnitId).Distinct().Count()
                    }
                }).ToList();

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("comparison")]
        public async Task<ActionResult> GetComparisonData(
            [FromQuery] int year,
            [FromQuery] int? quarter,
            [FromQuery] int? month,
            [FromQuery] int? unitId,
            [FromQuery] int? compareYear,
            [FromQuery] int? compareQuarter,
            [FromQuery] int? compareMonth)
        {
            try
            {
                // Current period data
                var currentData = await GetPeriodData(year, quarter, month, unitId);

                // Comparison period data
                var compareData = await GetPeriodData(
                    compareYear ?? year - 1,
                    compareQuarter ?? quarter,
                    compareMonth ?? month,
                    unitId);

                var comparison = currentData.Select(current => new
                {
                    IndicatorId = current.IndicatorId,
                    IndicatorName = current.IndicatorName,
                    Current = new
                    {
                        Target = current.Target,
                        Actual = current.Actual,
                        Achievement = current.Achievement
                    },
                    Compare = compareData.FirstOrDefault(c => c.IndicatorId == current.IndicatorId) ?? new
                    {
                        IndicatorId = current.IndicatorId,
                        IndicatorName = current.IndicatorName,
                        Target = 0m,
                        Actual = 0m,
                        Achievement = 0m
                    },
                    Growth = new
                    {
                        TargetGrowth = current.Target - (compareData.FirstOrDefault(c => c.IndicatorId == current.IndicatorId)?.Target ?? 0),
                        ActualGrowth = current.Actual - (compareData.FirstOrDefault(c => c.IndicatorId == current.IndicatorId)?.Actual ?? 0),
                        AchievementGrowth = current.Achievement - (compareData.FirstOrDefault(c => c.IndicatorId == current.IndicatorId)?.Achievement ?? 0)
                    }
                }).ToList();

                return Ok(comparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving comparison data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("trend")]
        public async Task<ActionResult> GetTrendData(
            [FromQuery] int indicatorId,
            [FromQuery] int? unitId,
            [FromQuery] int fromYear,
            [FromQuery] int toYear,
            [FromQuery] string periodType = "month") // month, quarter, year
        {
            try
            {
                var trendData = new List<object>();

                for (int year = fromYear; year <= toYear; year++)
                {
                    if (periodType == "year")
                    {
                        var yearData = await GetPeriodData(year, null, null, unitId);
                        var indicatorData = yearData.FirstOrDefault(d => d.IndicatorId == indicatorId);

                        trendData.Add(new
                        {
                            Period = year.ToString(),
                            Year = year,
                            Quarter = (int?)null,
                            Month = (int?)null,
                            Target = indicatorData?.Target ?? 0,
                            Actual = indicatorData?.Actual ?? 0,
                            Achievement = indicatorData?.Achievement ?? 0
                        });
                    }
                    else if (periodType == "quarter")
                    {
                        for (int quarter = 1; quarter <= 4; quarter++)
                        {
                            var quarterData = await GetPeriodData(year, quarter, null, unitId);
                            var indicatorData = quarterData.FirstOrDefault(d => d.IndicatorId == indicatorId);

                            trendData.Add(new
                            {
                                Period = $"Q{quarter}/{year}",
                                Year = year,
                                Quarter = quarter,
                                Month = (int?)null,
                                Target = indicatorData?.Target ?? 0,
                                Actual = indicatorData?.Actual ?? 0,
                                Achievement = indicatorData?.Achievement ?? 0
                            });
                        }
                    }
                    else // month
                    {
                        for (int month = 1; month <= 12; month++)
                        {
                            var monthData = await GetPeriodData(year, null, month, unitId);
                            var indicatorData = monthData.FirstOrDefault(d => d.IndicatorId == indicatorId);

                            trendData.Add(new
                            {
                                Period = $"{month:D2}/{year}",
                                Year = year,
                                Quarter = (int?)null,
                                Month = month,
                                Target = indicatorData?.Target ?? 0,
                                Actual = indicatorData?.Actual ?? 0,
                                Achievement = indicatorData?.Achievement ?? 0
                            });
                        }
                    }
                }

                return Ok(trendData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trend data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("generate-sample-data")]
        public async Task<ActionResult> GenerateSampleData([FromBody] SampleDataRequest request)
        {
            try
            {
                await _calculationService.GenerateSampleDataAsync(
                    request.UnitId,
                    request.Year,
                    request.Quarter,
                    request.Month,
                    User.Identity?.Name ?? "System"
                );

                return Ok(new { Message = "Sample data generated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sample data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy kết quả tính toán dashboard theo năm và các filter khác
        /// Endpoint này được gọi từ frontend CalculationDashboard.vue
        /// </summary>
        [HttpGet("calculation-results")]
        public async Task<ActionResult> GetCalculationResults(
            [FromQuery] int year,
            [FromQuery] int? quarter = null,
            [FromQuery] int? month = null,
            [FromQuery] int? unitId = null,
            [FromQuery] string? unitCode = null, // Thêm unitCode parameter
            [FromQuery] string? periodType = null)
        {
            try
            {
                _logger.LogInformation("🔍 Getting calculation results for year {Year}, quarter {Quarter}, month {Month}, unitId {UnitId}, unitCode {UnitCode}, periodType {PeriodType}",
                    year, quarter, month, unitId, unitCode, periodType);

                // Lấy tất cả calculations theo filter
                var calculationsQuery = _context.DashboardCalculations
                    .Include(c => c.DashboardIndicator)
                    .Include(c => c.Unit)
                    .Where(c => !c.IsDeleted && c.Year == year);

                // Áp dụng filter theo loại kỳ
                if (quarter.HasValue)
                    calculationsQuery = calculationsQuery.Where(c => c.Quarter == quarter.Value);

                if (month.HasValue)
                    calculationsQuery = calculationsQuery.Where(c => c.Month == month.Value);

                // Filter by unitId (int) hoặc unitCode (string)
                if (unitId.HasValue)
                    calculationsQuery = calculationsQuery.Where(c => c.UnitId == unitId.Value);
                else if (!string.IsNullOrEmpty(unitCode))
                {
                    // Tạm thời log và skip filter, cần mapping unitCode -> unitId
                    _logger.LogInformation("🔧 Calculation results: skipping unitCode filter for now: {UnitCode}", unitCode);
                }

                var calculations = await calculationsQuery
                    .OrderByDescending(c => c.CalculationDate)
                    .ToListAsync();

                // Lấy targets tương ứng để so sánh
                var targetsQuery = _context.BusinessPlanTargets
                    .Include(t => t.DashboardIndicator)
                    .Include(t => t.Unit)
                    .Where(t => !t.IsDeleted && t.Year == year);

                if (quarter.HasValue)
                    targetsQuery = targetsQuery.Where(t => t.Quarter == quarter.Value);

                if (month.HasValue)
                    targetsQuery = targetsQuery.Where(t => t.Month == month.Value);

                // Filter targets by unitId (int) hoặc unitCode (string)
                if (unitId.HasValue)
                    targetsQuery = targetsQuery.Where(t => t.UnitId == unitId.Value);
                else if (!string.IsNullOrEmpty(unitCode))
                {
                    // Tạm thời log và skip filter cho targets
                    _logger.LogInformation("🔧 Targets query: skipping unitCode filter for now: {UnitCode}", unitCode);
                }

                var targets = await targetsQuery.ToListAsync();

                // Tạo response data
                var result = new
                {
                    year = year,
                    quarter = quarter,
                    month = month,
                    unitId = unitId,
                    periodType = periodType,
                    totalCalculations = calculations.Count,
                    totalTargets = targets.Count,
                    calculationResults = calculations.GroupBy(c => c.DashboardIndicatorId)
                        .Select(g => new
                        {
                            indicatorId = g.Key,
                            indicatorName = g.First().DashboardIndicator?.Name,
                            indicatorCode = g.First().DashboardIndicator?.Code,
                            calculations = g.Select(c => new
                            {
                                id = c.Id,
                                unitId = c.UnitId,
                                unitName = c.Unit?.Name,
                                actualValue = c.ActualValue,
                                targetValue = targets.FirstOrDefault(t => t.DashboardIndicatorId == c.DashboardIndicatorId && t.UnitId == c.UnitId)?.TargetValue,
                                calculationDate = c.CalculationDate,
                                status = c.Status,
                                dataSource = c.DataSource,
                                executionTime = c.ExecutionTime,
                                errorMessage = c.ErrorMessage
                            }).ToList(),
                            summary = new
                            {
                                totalActual = g.Sum(c => c.ActualValue ?? 0),
                                totalTarget = targets.Where(t => t.DashboardIndicatorId == g.Key).Sum(t => t.TargetValue),
                                achievementRate = targets.Where(t => t.DashboardIndicatorId == g.Key).Sum(t => t.TargetValue) > 0
                                    ? (g.Sum(c => c.ActualValue ?? 0) / targets.Where(t => t.DashboardIndicatorId == g.Key).Sum(t => t.TargetValue) * 100)
                                    : 0,
                                unitsCount = g.Select(c => c.UnitId).Distinct().Count()
                            }
                        }).ToList()
                };

                _logger.LogInformation("✅ Successfully retrieved {Count} calculation results", calculations.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving calculation results for year {Year}", year);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        private async Task<List<dynamic>> GetPeriodData(int year, int? quarter, int? month, int? unitId)
        {
            var targetsQuery = _context.BusinessPlanTargets
                .Where(t => !t.IsDeleted && t.Year == year);

            var calculationsQuery = _context.DashboardCalculations
                .Where(c => !c.IsDeleted && c.Year == year && c.Status == "Completed");

            if (quarter.HasValue)
            {
                targetsQuery = targetsQuery.Where(t => t.Quarter == quarter.Value);
                calculationsQuery = calculationsQuery.Where(c => c.Quarter == quarter.Value);
            }

            if (month.HasValue)
            {
                targetsQuery = targetsQuery.Where(t => t.Month == month.Value);
                calculationsQuery = calculationsQuery.Where(c => c.Month == month.Value);
            }

            if (unitId.HasValue)
            {
                targetsQuery = targetsQuery.Where(t => t.UnitId == unitId.Value);
                calculationsQuery = calculationsQuery.Where(c => c.UnitId == unitId.Value);
            }

            var targets = await targetsQuery.ToListAsync();
            var calculations = await calculationsQuery.ToListAsync();

            var indicators = await _context.DashboardIndicators
                .Where(i => !i.IsDeleted)
                .ToListAsync();

            return indicators.Select(indicator => new
            {
                IndicatorId = indicator.Id,
                IndicatorName = indicator.Name,
                Target = targets.Where(t => t.DashboardIndicatorId == indicator.Id).Sum(t => t.TargetValue),
                Actual = calculations.Where(c => c.DashboardIndicatorId == indicator.Id).Sum(c => c.ActualValue ?? 0),
                Achievement = calculations.Where(c => c.DashboardIndicatorId == indicator.Id).Sum(c => c.ActualValue ?? 0) /
                            Math.Max(targets.Where(t => t.DashboardIndicatorId == indicator.Id).Sum(t => t.TargetValue), 1) * 100
            }).Cast<dynamic>().ToList();
        }
    }

    public class CalculationRequest
    {
        public List<int> IndicatorIds { get; set; } = new();
        public int? UnitId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public string? UserId { get; set; }
    }

    public class SampleDataRequest
    {
        public int? UnitId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
    }
}
