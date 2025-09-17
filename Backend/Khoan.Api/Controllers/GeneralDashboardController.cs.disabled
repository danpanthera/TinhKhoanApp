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
    // [Authorize] // Tạm tắt authentication để test dashboard
    public class GeneralDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GeneralDashboardController> _logger;
        private readonly IBranchCalculationService _branchCalculationService;

        public GeneralDashboardController(
            ApplicationDbContext context,
            ILogger<GeneralDashboardController> logger,
            IBranchCalculationService branchCalculationService)
        {
            _context = context;
            _logger = logger;
            _branchCalculationService = branchCalculationService;
        }

        // Lấy dữ liệu 6 chỉ tiêu chính
        [HttpGet("indicators/{branchId}")]
        public async Task<ActionResult> GetIndicators(string branchId, [FromQuery] DateTime? date = null)
        {
            try
            {
                var dateStr = date?.ToString("dd/MM/yyyy") ?? "ngày gần nhất";
                _logger.LogInformation("🎯 Lấy dữ liệu dashboard cho chi nhánh {BranchId}, ngày {Date}", branchId, dateStr);

                // Tính toán tất cả các chỉ tiêu từ BranchCalculationService với ngày cụ thể
                var nguonVonVnd = await _branchCalculationService.CalculateNguonVonByBranch(branchId, date);
                var duNoVnd = await _branchCalculationService.CalculateDuNoByBranch(branchId, date);
                var noXauPercent = await _branchCalculationService.CalculateNoXauByBranch(branchId, date);
                var thuNoXlrrVnd = await _branchCalculationService.CalculateThuHoiXLRRByBranch(branchId, date);
                var thuDichVuVnd = await _branchCalculationService.CalculateThuDichVuByBranch(branchId, date);
                var taiChinhVnd = await _branchCalculationService.CalculateLoiNhuanByBranch(branchId, date);

                // Chuyển đổi từ VND sang triệu VND
                var nguonVonTy = Math.Round(nguonVonVnd / 1_000_000_000m, 2);
                var duNoTy = Math.Round(duNoVnd / 1_000_000_000m, 2);
                var thuNoXlrrTy = Math.Round(thuNoXlrrVnd / 1_000_000_000m, 2);
                var thuDichVuTy = Math.Round(thuDichVuVnd / 1_000_000_000m, 2);
                var taiChinhTy = Math.Round(taiChinhVnd / 1_000_000_000m, 2);

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
                            currentValue = (double)nguonVonTy,
                            targetValue = 250.0,
                            completionRate = Math.Round((double)(nguonVonTy / 250.0m * 100), 1),
                            changeFromYearStart = 25.2,
                            changeFromYearStartPercent = 11.4
                        },
                        new
                        {
                            id = "du_no",
                            name = "Dư nợ tín dụng",
                            icon = "💳",
                            @class = "du-no",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = (double)duNoTy,
                            targetValue = 190.0,
                            completionRate = Math.Round((double)(duNoTy / 190.0m * 100), 1),
                            changeFromYearStart = 15.8,
                            changeFromYearStartPercent = 9.2
                        },
                        new
                        {
                            id = "no_xau",
                            name = "Nợ xấu",
                            icon = "⚠️",
                            @class = "no-xau",
                            unit = "%",
                            format = "percent",
                            currentValue = (double)noXauPercent,
                            targetValue = 1.0,
                            completionRate = noXauPercent <= 1.0m ? Math.Round((double)((1.0m - noXauPercent) / 1.0m * 100 + 100), 1) : Math.Round((double)(1.0m / noXauPercent * 100), 1),
                            changeFromYearStart = -0.15,
                            changeFromYearStartPercent = -15.0
                        },
                        new
                        {
                            id = "thu_no_xlrr",
                            name = "Thu nợ XLRR",
                            icon = "🏦",
                            @class = "thu-no-xlrr",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = (double)thuNoXlrrTy,
                            targetValue = 15.0,
                            completionRate = Math.Round((double)(thuNoXlrrTy / 15.0m * 100), 1),
                            changeFromYearStart = 2.1,
                            changeFromYearStartPercent = 20.3
                        },
                        new
                        {
                            id = "thu_dich_vu",
                            name = "Thu dịch vụ",
                            icon = "🏦",
                            @class = "thu-dich-vu",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = (double)thuDichVuTy,
                            targetValue = 30.0,
                            completionRate = Math.Round((double)(thuDichVuTy / 30.0m * 100), 1),
                            changeFromYearStart = 3.1,
                            changeFromYearStartPercent = 12.0
                        },
                        new
                        {
                            id = "tai_chinh",
                            name = "Tài chính",
                            icon = "💵",
                            @class = "tai-chinh",
                            unit = "tỷ",
                            format = "currency",
                            currentValue = (double)taiChinhTy,
                            targetValue = 160.0,
                            completionRate = Math.Round((double)(taiChinhTy / 160.0m * 100), 1),
                            changeFromYearStart = 18.6,
                            changeFromYearStartPercent = 13.5
                        }
                    }
                };

                _logger.LogInformation("✅ Dashboard cho {BranchId}: Nguồn vốn={NguonVon} tỷ, Dư nợ={DuNo} tỷ, Nợ xấu={NoXau}%, Thu XLRR={ThuXlrr} tỷ, Thu DV={ThuDv} tỷ, Lợi nhuận={LoiNhuan} tỷ",
                    branchId, nguonVonTy, duNoTy, noXauPercent, thuNoXlrrTy, thuDichVuTy, taiChinhTy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy dữ liệu dashboard cho {BranchId}", branchId);
                return StatusCode(500, new { message = "Lỗi server khi tải dashboard", error = ex.Message });
            }
        }

        // Test endpoint đơn giản
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok(new { message = "GeneralDashboard Controller is working", timestamp = DateTime.Now });
        }

        // API đăng nhập demo cho testing (bypass authentication)
        [HttpPost("demo-login")]
        public ActionResult DemoLogin([FromBody] object loginData)
        {
            try
            {
                _logger.LogInformation("🔐 Demo login request received");

                // Tạo response giống như AuthController thành công
                var demoResponse = new
                {
                    token = "demo-token-for-testing-12345",
                    user = new
                    {
                        id = 9999,
                        fullName = "Demo User",
                        username = "demo",
                        employeeCode = "DEMO001"
                    }
                };

                _logger.LogInformation("✅ Demo login successful");
                return Ok(demoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Demo login error");
                return StatusCode(500, new { message = "Demo login error", error = ex.Message });
            }
        }
    }
}
