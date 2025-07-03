using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.NguonVon;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller tính toán Nguồn vốn theo yêu cầu mới của anh
    /// Hỗ trợ tính cho Hội Sở, các Chi nhánh, PGD và Toàn tỉnh
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NguonVonButtonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguonVonButtonController> _logger;

        // Mapping theo yêu cầu của anh
        private readonly Dictionary<string, BranchInfo> _branchMapping = new Dictionary<string, BranchInfo>
        {
            // Hội Sở và các Chi nhánh chính
            { "HoiSo", new BranchInfo("7800", null, "Hội Sở") },
            { "CnBinhLu", new BranchInfo("7801", null, "CN Bình Lư") },
            { "CnPhongTho", new BranchInfo("7802", null, "CN Phong Thổ") },
            { "CnSinHo", new BranchInfo("7803", null, "CN Sìn Hồ") },
            { "CnBumTo", new BranchInfo("7804", null, "CN Bum Tở") },
            { "CnThanUyen", new BranchInfo("7805", null, "CN Than Uyên") },
            { "CnDoanKet", new BranchInfo("7806", null, "CN Đoàn Kết") },
            { "CnTanUyen", new BranchInfo("7807", null, "CN Tân Uyên") },
            { "CnNamHang", new BranchInfo("7808", null, "CN Nậm Hàng") },

            // Các PGD
            { "CnPhongTho-PGD5", new BranchInfo("7802", "01", "CN Phong Thổ - PGD Số 5") },
            { "CnThanUyen-PGD6", new BranchInfo("7805", "01", "CN Than Uyên - PGD Số 6") },
            { "CnDoanKet-PGD1", new BranchInfo("7806", "01", "CN Đoàn Kết - PGD Số 1") },
            { "CnDoanKet-PGD2", new BranchInfo("7806", "02", "CN Đoàn Kết - PGD Số 2") },
            { "CnTanUyen-PGD3", new BranchInfo("7807", "01", "CN Tân Uyên - PGD Số 3") },

            // Toàn tỉnh
            { "ToanTinh", new BranchInfo("ALL", null, "Toàn tỉnh") }
        };

        public NguonVonButtonController(ApplicationDbContext context, ILogger<NguonVonButtonController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tính Nguồn vốn theo button được ấn
        /// </summary>
        /// <param name="unitKey">Key đơn vị (HoiSo, CnBinhLu, CnPhongTho-PGD5, ToanTinh...)</param>
        /// <param name="targetDate">Ngày tính toán (optional, mặc định là ngày hiện tại)</param>
        [HttpPost("calculate/{unitKey}")]
        public async Task<IActionResult> CalculateNguonVon(string unitKey, [FromQuery] DateTime? targetDate = null)
        {
            try
            {
                if (!_branchMapping.ContainsKey(unitKey))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Không tìm thấy đơn vị '{unitKey}'. Các đơn vị hỗ trợ: {string.Join(", ", _branchMapping.Keys)}"
                    });
                }

                var branchInfo = _branchMapping[unitKey];
                var calculationDate = targetDate ?? DateTime.Now;

                _logger.LogInformation("💰 Bắt đầu tính Nguồn vốn cho {UnitName} ({UnitKey}) - MA_CN: {MaCN}, MA_PGD: {MaPGD}",
                    branchInfo.DisplayName, unitKey, branchInfo.MaCN, branchInfo.MaPGD ?? "ALL");

                decimal totalNguonVon;
                int recordCount;
                List<object> topAccounts;

                if (unitKey == "ToanTinh")
                {
                    // Tính toàn tỉnh (tổng từ 7800-7808)
                    var result = await CalculateAllProvince(calculationDate);
                    totalNguonVon = result.Total;
                    recordCount = result.RecordCount;
                    topAccounts = result.TopAccounts;
                }
                else
                {
                    // Tính cho đơn vị cụ thể
                    var result = await CalculateSingleUnit(branchInfo, calculationDate);
                    totalNguonVon = result.Total;
                    recordCount = result.RecordCount;
                    topAccounts = result.TopAccounts;
                }

                _logger.LogInformation("✅ Kết quả tính Nguồn vốn: {Total:N0} VND từ {Count} bản ghi", totalNguonVon, recordCount);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        unitKey = unitKey,
                        unitName = branchInfo.DisplayName,
                        maCN = branchInfo.MaCN,
                        maPGD = branchInfo.MaPGD,
                        totalNguonVon = totalNguonVon,
                        totalNguonVonTrieuVND = Math.Round(totalNguonVon / 1_000_000m, 2),
                        recordCount = recordCount,
                        calculationDate = calculationDate.ToString("dd/MM/yyyy"),
                        topAccounts = topAccounts.Take(10).ToList(), // Top 10 tài khoản
                        formula = "Tổng CURRENT_BALANCE - (loại trừ TK 40*, 41*, 427*, 211108)"
                    },
                    message = $"Tính toán thành công cho {branchInfo.DisplayName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tính Nguồn vốn cho {UnitKey}", unitKey);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi khi tính toán: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Tính Nguồn vốn cho toàn tỉnh (tổng từ 7800-7808)
        /// </summary>
        private async Task<CalculationResult> CalculateAllProvince(DateTime targetDate)
        {
            var allBranchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };

            // Sử dụng bảng DP01 trực tiếp từ KHO DỮ LIỆU THÔ
            var query = _context.DP01s
                .Where(d => d.DATA_DATE.Date == targetDate.Date && allBranchCodes.Contains(d.MA_CN))
                .Where(d =>
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                    d.TAI_KHOAN_HACH_TOAN != "211108"
                );

            var totalNguonVon = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
            var recordCount = await query.CountAsync();

            // Top accounts
            var topAccounts = await query
                .GroupBy(d => d.TAI_KHOAN_HACH_TOAN)
                .Select(g => new
                {
                    AccountCode = g.Key,
                    TotalBalance = g.Sum(x => x.CURRENT_BALANCE ?? 0),
                    RecordCount = g.Count()
                })
                .OrderByDescending(a => Math.Abs(a.TotalBalance))
                .Take(20)
                .ToListAsync();

            return new CalculationResult
            {
                Total = totalNguonVon,
                RecordCount = recordCount,
                TopAccounts = topAccounts.Cast<object>().ToList()
            };
        }

        /// <summary>
        /// Tính Nguồn vốn cho một đơn vị cụ thể
        /// </summary>
        private async Task<CalculationResult> CalculateSingleUnit(BranchInfo branchInfo, DateTime targetDate)
        {
            // Sử dụng bảng DP01 trực tiếp từ KHO DỮ LIỆU THÔ
            var query = _context.DP01s
                .Where(d => d.DATA_DATE.Date == targetDate.Date && d.MA_CN == branchInfo.MaCN);

            // Lọc theo PGD nếu có
            if (!string.IsNullOrEmpty(branchInfo.MaPGD))
            {
                query = query.Where(d => d.MA_PGD == branchInfo.MaPGD);
            }

            // Lọc tài khoản theo điều kiện: bỏ đi 40*, 41*, 427*, 211108
            query = query.Where(d =>
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                d.TAI_KHOAN_HACH_TOAN != "211108"
            );

            var totalNguonVon = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
            var recordCount = await query.CountAsync();

            // Top accounts
            var topAccounts = await query
                .GroupBy(d => d.TAI_KHOAN_HACH_TOAN)
                .Select(g => new
                {
                    AccountCode = g.Key,
                    TotalBalance = g.Sum(x => x.CURRENT_BALANCE ?? 0),
                    RecordCount = g.Count()
                })
                .OrderByDescending(a => Math.Abs(a.TotalBalance))
                .Take(20)
                .ToListAsync();

            return new CalculationResult
            {
                Total = totalNguonVon,
                RecordCount = recordCount,
                TopAccounts = topAccounts.Cast<object>().ToList()
            };
        }

        /// <summary>
        /// Lấy danh sách tất cả đơn vị hỗ trợ
        /// </summary>
        [HttpGet("units")]
        public IActionResult GetSupportedUnits()
        {
            var units = _branchMapping.Select(kvp => new
            {
                unitKey = kvp.Key,
                unitName = kvp.Value.DisplayName,
                maCN = kvp.Value.MaCN,
                maPGD = kvp.Value.MaPGD
            }).ToList();

            return Ok(new
            {
                success = true,
                data = units,
                message = "Danh sách các đơn vị hỗ trợ"
            });
        }
    }

    /// <summary>
    /// Thông tin chi nhánh/PGD
    /// </summary>
    public class BranchInfo
    {
        public string MaCN { get; set; }
        public string? MaPGD { get; set; }
        public string DisplayName { get; set; }

        public BranchInfo(string maCN, string? maPGD, string displayName)
        {
            MaCN = maCN;
            MaPGD = maPGD;
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// Kết quả tính toán
    /// </summary>
    public class CalculationResult
    {
        public decimal Total { get; set; }
        public int RecordCount { get; set; }
        public List<object> TopAccounts { get; set; } = new List<object>();
    }
}
