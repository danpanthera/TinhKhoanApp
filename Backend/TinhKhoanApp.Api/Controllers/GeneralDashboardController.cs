using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GeneralDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GeneralDashboardController> _logger;

        public GeneralDashboardController(
            ApplicationDbContext context,
            ILogger<GeneralDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lấy dữ liệu 6 chỉ tiêu chính
        [HttpGet("indicators/{branchId}")]
        public async Task<ActionResult> GetIndicators(string branchId)
        {
            try
            {
                var currentDate = DateTime.Now;
                var yearStartDate = new DateTime(currentDate.Year, 1, 1);

                // Lấy unit ID từ branch code
                var unit = await _context.Units
                    .FirstOrDefaultAsync(u => u.Code == branchId);

                if (unit == null)
                {
                    return NotFound(new { message = "Không tìm thấy chi nhánh" });
                }

                var result = new
                {
                    indicators = new[]
                    {
                        new
                        {
                            id = "nguon_von",
                            name = "Nguồn vốn",
                            icon = "💰",
                            @class = "nguon-von",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = await GetCurrentValue("NguonVon", unit.Id, currentDate),
                            targetValue = await GetTargetValue("NguonVon", unit.Id, currentDate),
                            completionRate = await GetCompletionRate("NguonVon", unit.Id, currentDate),
                            changeFromYearStart = await GetChangeFromYearStart("NguonVon", unit.Id, yearStartDate, currentDate),
                            changeFromYearStartPercent = await GetChangeFromYearStartPercent("NguonVon", unit.Id, yearStartDate, currentDate)
                        },
                        new
                        {
                            id = "du_no",
                            name = "Dư nợ",
                            icon = "💳",
                            @class = "du-no",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = await GetCurrentValue("DuNo", unit.Id, currentDate),
                            targetValue = await GetTargetValue("DuNo", unit.Id, currentDate),
                            completionRate = await GetCompletionRate("DuNo", unit.Id, currentDate),
                            changeFromYearStart = await GetChangeFromYearStart("DuNo", unit.Id, yearStartDate, currentDate),
                            changeFromYearStartPercent = await GetChangeFromYearStartPercent("DuNo", unit.Id, yearStartDate, currentDate)
                        },
                        new
                        {
                            id = "no_xau",
                            name = "Nợ Xấu",
                            icon = "⚠️",
                            @class = "no-xau",
                            unit = "%",
                            format = "percent",
                            currentValue = await GetCurrentValue("TyLeNoXau", unit.Id, currentDate),
                            targetValue = await GetTargetValue("TyLeNoXau", unit.Id, currentDate),
                            completionRate = await GetCompletionRate("TyLeNoXau", unit.Id, currentDate),
                            changeFromYearStart = await GetChangeFromYearStart("TyLeNoXau", unit.Id, yearStartDate, currentDate),
                            changeFromYearStartPercent = await GetChangeFromYearStartPercent("TyLeNoXau", unit.Id, yearStartDate, currentDate)
                        },
                        new
                        {
                            id = "thu_no_xlrr",
                            name = "Thu nợ đã XLRR",
                            icon = "📈",
                            @class = "thu-no-xlrr",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = await GetCurrentValue("ThuHoiXLRR", unit.Id, currentDate),
                            targetValue = await GetTargetValue("ThuHoiXLRR", unit.Id, currentDate),
                            completionRate = await GetCompletionRate("ThuHoiXLRR", unit.Id, currentDate),
                            changeFromYearStart = await GetChangeFromYearStart("ThuHoiXLRR", unit.Id, yearStartDate, currentDate),
                            changeFromYearStartPercent = await GetChangeFromYearStartPercent("ThuHoiXLRR", unit.Id, yearStartDate, currentDate)
                        },
                        new
                        {
                            id = "thu_dich_vu",
                            name = "Thu dịch vụ",
                            icon = "🏦",
                            @class = "thu-dich-vu",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = await GetCurrentValue("ThuDichVu", unit.Id, currentDate),
                            targetValue = await GetTargetValue("ThuDichVu", unit.Id, currentDate),
                            completionRate = await GetCompletionRate("ThuDichVu", unit.Id, currentDate),
                            changeFromYearStart = await GetChangeFromYearStart("ThuDichVu", unit.Id, yearStartDate, currentDate),
                            changeFromYearStartPercent = await GetChangeFromYearStartPercent("ThuDichVu", unit.Id, yearStartDate, currentDate)
                        },
                        new
                        {
                            id = "tai_chinh",
                            name = "Tài chính",
                            icon = "💵",
                            @class = "tai-chinh",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = await GetCurrentValue("LoiNhuan", unit.Id, currentDate),
                            targetValue = await GetTargetValue("LoiNhuan", unit.Id, currentDate),
                            completionRate = await GetCompletionRate("LoiNhuan", unit.Id, currentDate),
                            changeFromYearStart = await GetChangeFromYearStart("LoiNhuan", unit.Id, yearStartDate, currentDate),
                            changeFromYearStartPercent = await GetChangeFromYearStartPercent("LoiNhuan", unit.Id, yearStartDate, currentDate)
                        }
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu dashboard tổng hợp");
                return StatusCode(500, "Lỗi server");
            }
        }

        // Helper methods để tính toán các giá trị
        private async Task<decimal> GetCurrentValue(string indicatorCode, int unitId, DateTime date)
        {
            var indicator = await _context.DashboardIndicators
                .FirstOrDefaultAsync(i => i.Code == indicatorCode);

            if (indicator == null) return 0;

            var calculation = await _context.DashboardCalculations
                .Where(c => c.DashboardIndicatorId == indicator.Id && 
                           c.UnitId == unitId &&
                           c.CalculationDate.Date == date.Date)
                .OrderByDescending(c => c.CreatedDate)
                .FirstOrDefaultAsync();

            return calculation?.ActualValue ?? 0;
        }

        private async Task<decimal> GetTargetValue(string indicatorCode, int unitId, DateTime date)
        {
            var indicator = await _context.DashboardIndicators
                .FirstOrDefaultAsync(i => i.Code == indicatorCode);

            if (indicator == null) return 0;

            var target = await _context.BusinessPlanTargets
                .Where(t => t.DashboardIndicatorId == indicator.Id && 
                           t.UnitId == unitId &&
                           t.Year == date.Year &&
                           t.Month == date.Month)
                .FirstOrDefaultAsync();

            return target?.TargetValue ?? 0;
        }

        private async Task<decimal> GetCompletionRate(string indicatorCode, int unitId, DateTime date)
        {
            var current = await GetCurrentValue(indicatorCode, unitId, date);
            var target = await GetTargetValue(indicatorCode, unitId, date);

            if (target == 0) return 0;

            return Math.Round((current / target) * 100, 1);
        }

        private async Task<decimal> GetChangeFromYearStart(string indicatorCode, int unitId, DateTime yearStart, DateTime currentDate)
        {
            var currentValue = await GetCurrentValue(indicatorCode, unitId, currentDate);
            var yearStartValue = await GetCurrentValue(indicatorCode, unitId, yearStart);

            return currentValue - yearStartValue;
        }

        private async Task<decimal> GetChangeFromYearStartPercent(string indicatorCode, int unitId, DateTime yearStart, DateTime currentDate)
        {
            var yearStartValue = await GetCurrentValue(indicatorCode, unitId, yearStart);
            var change = await GetChangeFromYearStart(indicatorCode, unitId, yearStart, currentDate);

            if (yearStartValue == 0) return 0;

            return Math.Round((change / yearStartValue) * 100, 1);
        }
    }
}
