using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.ViewModels;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller cung cấp API cho Dashboard
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IGL41Repository _gl41Repository;
        private readonly IGL41DataService _gl41DataService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IGL41Repository gl41Repository,
            IGL41DataService gl41DataService,
            ILogger<DashboardController> logger)
        {
            _gl41Repository = gl41Repository;
            _gl41DataService = gl41DataService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tổng quan dữ liệu GL41 cho Dashboard
        /// </summary>
        [HttpGet("gl41/summary")]
        [ProducesResponseType(typeof(ApiResponse<GL41DashboardViewModel>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetGL41Dashboard()
        {
            try
            {
                // Lấy dữ liệu GL41 gần đây nhất
                var recentRecords = await _gl41Repository.GetRecentAsync(1000);

                if (!recentRecords.Any())
                {
                    return Ok(ApiResponse<GL41DashboardViewModel>.Ok(
                        new GL41DashboardViewModel
                        {
                            NGAY_DL = DateTime.Now,
                            LatestImportDate = DateTime.Now
                        },
                        "No GL41 data available"));
                }

                // Xác định ngày dữ liệu mới nhất
                var latestDate = recentRecords.Max(r => r.NGAY_DL.Date);
                var recordsOnLatestDate = recentRecords.Where(r => r.NGAY_DL.Date == latestDate).ToList();

                // Thống kê theo đơn vị
                var unitGroups = recordsOnLatestDate
                    .GroupBy(r => r.MA_CN)
                    .Select(g => new
                    {
                        UnitCode = g.Key,
                        RecordCount = g.Count(),
                        DebitClosing = g.Sum(r => r.DN_CUOIKY ?? 0),
                        CreditClosing = g.Sum(r => r.DC_CUOIKY ?? 0),
                        Balance = g.Sum(r => (r.DN_CUOIKY ?? 0) - (r.DC_CUOIKY ?? 0))
                    })
                    .OrderByDescending(g => Math.Abs(g.Balance))
                    .Take(5)
                    .ToList();

                // Tạo dashboard view model
                var dashboard = new GL41DashboardViewModel
                {
                    NGAY_DL = latestDate,
                    TotalRecords = recordsOnLatestDate.Count,
                    TotalUnits = recordsOnLatestDate.Select(r => r.MA_CN).Distinct().Count(),
                    TotalAccounts = recordsOnLatestDate.Select(r => r.MA_TK).Distinct().Count(),
                    LatestImportDate = recentRecords.Max(r => r.CREATED_DATE),

                    TotalDebitOpening = recordsOnLatestDate.Sum(r => r.DN_DAUKY ?? 0),
                    TotalCreditOpening = recordsOnLatestDate.Sum(r => r.DC_DAUKY ?? 0),
                    TotalDebitTransaction = recordsOnLatestDate.Sum(r => r.ST_GHINO ?? 0),
                    TotalCreditTransaction = recordsOnLatestDate.Sum(r => r.ST_GHICO ?? 0),
                    TotalDebitClosing = recordsOnLatestDate.Sum(r => r.DN_CUOIKY ?? 0),
                    TotalCreditClosing = recordsOnLatestDate.Sum(r => r.DC_CUOIKY ?? 0),

                    TopUnitsByBalance = unitGroups.Select(g => new UnitBalanceSummary
                    {
                        MA_CN = g.UnitCode,
                        TEN_CN = $"Chi nhánh {g.UnitCode}", // Tạm thời, cần bổ sung mapping tên đơn vị
                        ClosingBalance = g.Balance
                    }).ToList()
                };

                return Ok(ApiResponse<GL41DashboardViewModel>.Ok(dashboard, "GL41 dashboard data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 dashboard data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL41 dashboard data", "GL41_DASHBOARD_ERROR"));
            }
        }
    }
}
