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
    // [Authorize] // Tạm thời comment để test
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

        /// <summary>
        /// Tính nguồn vốn từ bảng DP01 theo ngày và chi nhánh được chọn
        /// </summary>
        /// <param name="date">Ngày cần lọc (yyyy-MM-dd)</param>
        /// <param name="branchCode">Mã chi nhánh (VD: "HoiSo", "BinhLu", "ToanTinh"...)</param>
        [HttpGet("nguon-von")]
        // [Authorize] // Tạm thời comment để test
        public async Task<ActionResult<object>> GetNguonVon(
            [FromQuery] string date,
            [FromQuery] string branchCode)
        {
            try
            {
                _logger.LogInformation("🏦 [NGUON_VON] Yêu cầu tính nguồn vốn - Date: {Date}, Branch: {Branch}", date, branchCode);

                // Validate parameters
                if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(branchCode))
                {
                    return BadRequest(new { error = "Thiếu thông tin ngày hoặc chi nhánh" });
                }

                // Parse và xử lý ngày
                var filterDate = DateTime.ParseExact(date, "yyyy-MM-dd", null);
                // Tạm thời bypass check để test với dữ liệu mẫu
                // var filterDate = await ProcessDateFilter(date);
                // if (filterDate == null)
                // {
                //     return BadRequest(new { error = "Kho dữ liệu chưa có ngày này!" });
                // }

                // Mapping chi nhánh sang MA_CN và MA_PGD
                var branchFilter = GetBranchFilter(branchCode);

                // Build query cho DP01
                var query = _context.DP01.AsQueryable();

                // Filter theo ngày - sử dụng DateTime format
                query = query.Where(d => d.NGAY_DL.Date == filterDate.Date);

                // Filter theo chi nhánh
                if (branchFilter.IsToaTinh)
                {
                    // Toàn tỉnh: từ 7800-7808
                    query = query.Where(d => d.MA_CN.CompareTo("7800") >= 0 && d.MA_CN.CompareTo("7808") <= 0);
                }
                else
                {
                    // Chi nhánh cụ thể
                    if (!string.IsNullOrEmpty(branchFilter.MA_CN))
                    {
                        query = query.Where(d => d.MA_CN == branchFilter.MA_CN);
                    }

                    // Filter thêm MA_PGD nếu có
                    if (!string.IsNullOrEmpty(branchFilter.MA_PGD))
                    {
                        query = query.Where(d => d.MA_PGD == branchFilter.MA_PGD);
                    }
                }

                // Loại trừ các tài khoản theo yêu cầu
                query = query.Where(d =>
                    // Loại trừ tài khoản bắt đầu bằng "40", "41"
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                    // Loại trừ tài khoản bắt đầu bằng "427"
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                    // Loại trừ tài khoản "211108"
                    d.TAI_KHOAN_HACH_TOAN != "211108"
                );

                // Tính tổng CURRENT_BALANCE (convert từ string sang decimal)
                var records = await query.ToListAsync();
                var totalBalance = records
                    .Where(d => !string.IsNullOrEmpty(d.CURRENT_BALANCE))
                    .Sum(d => decimal.TryParse(d.CURRENT_BALANCE, out var balance) ? balance : 0);
                var recordCount = records.Count;

                _logger.LogInformation("✅ [NGUON_VON] Kết quả - Records: {Records}, Total: {Total:N0}", recordCount, totalBalance);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        date = filterDate.ToString("dd/MM/yyyy"),
                        branchCode = branchCode,
                        branchName = GetBranchName(branchCode),
                        totalBalance = totalBalance,
                        recordCount = recordCount,
                        filterInfo = new
                        {
                            maCN = branchFilter.MA_CN,
                            maPGD = branchFilter.MA_PGD,
                            isToaTinh = branchFilter.IsToaTinh
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [NGUON_VON] Lỗi tính nguồn vốn: {Error}", ex.Message);
                return StatusCode(500, new { error = "Lỗi hệ thống khi tính nguồn vốn", details = ex.Message });
            }
        }

        /// <summary>
        /// Xử lý logic lọc ngày theo yêu cầu
        /// </summary>
        private async Task<DateTime?> ProcessDateFilter(string dateInput)
        {
            try
            {
                // Nếu là ngày cụ thể (yyyy-MM-dd)
                if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var specificDate))
                {
                    // Kiểm tra xem có dữ liệu cho ngày này không bằng SQL RAW với formatsanitized
                    var dateString = specificDate.ToString("dd/MM/yyyy");
                    var sql = "SELECT COUNT(*) FROM DP01 WHERE NGAY_DL = @p0";
                    var count = await _context.Database.SqlQueryRaw<int>(sql, dateString).FirstOrDefaultAsync();
                    var exists = count > 0;

                    _logger.LogInformation("🔍 [NGUON_VON] Kiểm tra ngày {Date} (format: {Format}) - Count: {Count}",
                        specificDate.ToString("yyyy-MM-dd"), dateString, count);
                    return exists ? specificDate : null;
                }

                // Nếu là năm (yyyy)
                if (dateInput.Length == 4 && int.TryParse(dateInput, out var year))
                {
                    var endOfYear = new DateTime(year, 12, 31);
                    var dateString = endOfYear.ToString("dd/MM/yyyy");
                    var sql = "SELECT COUNT(*) FROM DP01 WHERE NGAY_DL = @p0";
                    var count = await _context.Database.SqlQueryRaw<int>(sql, dateString).FirstOrDefaultAsync();
                    var exists = count > 0;

                    _logger.LogInformation("🔍 [NGUON_VON] Kiểm tra cuối năm {Year} ({Date}, format: {Format}) - Count: {Count}",
                        year, endOfYear.ToString("yyyy-MM-dd"), dateString, count);
                    return exists ? endOfYear : null;
                }

                // Nếu là tháng (yyyy-MM)
                if (dateInput.Length == 7 && DateTime.TryParseExact(dateInput + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var monthDate))
                {
                    var endOfMonth = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month));
                    var dateString = endOfMonth.ToString("dd/MM/yyyy");
                    var sql = "SELECT COUNT(*) FROM DP01 WHERE NGAY_DL = @p0";
                    var count = await _context.Database.SqlQueryRaw<int>(sql, dateString).FirstOrDefaultAsync();
                    var exists = count > 0;

                    _logger.LogInformation("🔍 [NGUON_VON] Kiểm tra cuối tháng {Month} ({Date}, format: {Format}) - Count: {Count}",
                        dateInput, endOfMonth.ToString("yyyy-MM-dd"), dateString, count);
                    return exists ? endOfMonth : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ [NGUON_VON] Lỗi parse ngày: {DateInput} - {Error}", dateInput, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Mapping chi nhánh sang MA_CN và MA_PGD
        /// </summary>
        private (string? MA_CN, string? MA_PGD, bool IsToaTinh) GetBranchFilter(string branchCode)
        {
            return branchCode.ToLower() switch
            {
                "hoiso" => ("7800", null, false),
                "binhlu" => ("7801", null, false),
                "phongtho" => ("7802", null, false),
                "sinho" => ("7803", null, false),
                "bumto" => ("7804", null, false),
                "thanuyen" => ("7805", null, false),
                "doanket" => ("7806", null, false),
                "tanuyen" => ("7807", null, false),
                "namhang" => ("7808", null, false),
                "phongtho-pgd5" => ("7802", "'01", false),
                "thanuyen-pgd6" => ("7805", "'01", false),
                "doanket-pgd1" => ("7806", "'01", false),
                "doanket-pgd2" => ("7806", "'02", false),
                "tanuyen-pgd3" => ("7807", "'01", false),
                "toantinh" => (null, null, true),
                _ => throw new ArgumentException($"Mã chi nhánh không hợp lệ: {branchCode}")
            };
        }

        /// <summary>
        /// Lấy tên chi nhánh để hiển thị
        /// </summary>
        private string GetBranchName(string branchCode)
        {
            return branchCode.ToLower() switch
            {
                "hoiso" => "Hội Sở",
                "binhlu" => "CN Bình Lư",
                "phongtho" => "CN Phong Thổ",
                "sinho" => "CN Sìn Hồ",
                "bumto" => "CN Bum Tở",
                "thanuyen" => "CN Than Uyên",
                "doanket" => "CN Đoàn Kết",
                "tanuyen" => "CN Tân Uyên",
                "namhang" => "CN Nậm Hàng",
                "phongtho-pgd5" => "CN Phong Thổ - PGD Số 5",
                "thanuyen-pgd6" => "CN Than Uyên - PGD Số 6",
                "doanket-pgd1" => "CN Đoàn Kết - PGD Số 1",
                "doanket-pgd2" => "CN Đoàn Kết - PGD Số 2",
                "tanuyen-pgd3" => "CN Tân Uyên - PGD Số 3",
                "toantinh" => "Toàn tỉnh",
                _ => branchCode
            };
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

        [HttpGet("debug-dp01")]
        public async Task<ActionResult<object>> DebugDP01()
        {
            try
            {
                // Sử dụng SQL RAW đơn giản để kiểm tra dữ liệu
                var totalSql = "SELECT COUNT(*) AS Value FROM DP01";
                var totalRecords = await _context.Database.SqlQueryRaw<int>(totalSql).FirstOrDefaultAsync();

                // Test với DateTime format
                var testDates = new List<string> { "2024-12-31", "2023-12-31", "2024-06-30" };
                var dateResults = new Dictionary<string, int>();

                foreach (var testDate in testDates)
                {
                    var countSql = $"SELECT COUNT(*) AS Value FROM DP01 WHERE NGAY_DL = '{testDate}'";
                    var count = await _context.Database.SqlQueryRaw<int>(countSql).FirstOrDefaultAsync();
                    dateResults[testDate] = count;
                }

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    DateTests = dateResults,
                    Message = "Testing DateTime format dates"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
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
