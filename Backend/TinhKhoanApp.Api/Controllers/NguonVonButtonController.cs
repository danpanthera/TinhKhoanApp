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
    /// SỬ DỤNG BẢNG DP01_New (DP01_News DbSet) - CHÍNH THỨC
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NguonVonButtonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguonVonButtonController> _logger;

        // Mapping theo yêu cầu của anh - CẬP NHẬT CHÍNH XÁC
        private readonly Dictionary<string, BranchInfo> _branchMapping = new Dictionary<string, BranchInfo>
        {
            // Mapping theo yêu cầu của anh - CẬP NHẬT CHÍNH XÁC
            { "HoiSo", new BranchInfo("7800", null, "Hội Sở") },
            { "CnBinhLu", new BranchInfo("7801", null, "CN Bình Lư") },
            { "CnPhongTho", new BranchInfo("7802", null, "CN Phong Thổ") },
            { "CnSinHo", new BranchInfo("7803", null, "CN Sìn Hồ") },
            { "CnBumTo", new BranchInfo("7804", null, "CN Bum Tở") },
            { "CnThanUyen", new BranchInfo("7805", null, "CN Than Uyên") },
            { "CnDoanKet", new BranchInfo("7806", null, "CN Đoàn Kết") },
            { "CnTanUyen", new BranchInfo("7807", null, "CN Tân Uyên") },
            { "CnNamNhun", new BranchInfo("7808", null, "CN Nậm Nhùn") }, // Sửa từ Nậm Hàng

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
        /// Tính Nguồn vốn theo button được ấn - CẬP NHẬT THEO YÊU CẦU CHÍNH XÁC
        /// Hỗ trợ lọc theo date/month/year và sử dụng bảng DP01_New
        /// </summary>
        /// <param name="unitKey">Key đơn vị (HoiSo, CnBinhLu, CnPhongTho-PGD5, ToanTinh...)</param>
        /// <param name="targetDate">Ngày cụ thể (dd/MM/yyyy)</param>
        /// <param name="targetMonth">Tháng và năm (MM/yyyy) - sẽ lọc ngày cuối tháng</param>
        /// <param name="targetYear">Năm (yyyy) - sẽ lọc ngày 31/12/yyyy</param>
        [HttpPost("calculate/{unitKey}")]
        public async Task<IActionResult> CalculateNguonVon(
            string unitKey,
            [FromQuery] string? targetDate = null,
            [FromQuery] string? targetMonth = null,
            [FromQuery] string? targetYear = null)
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

                // Xác định NgayDL để lọc theo yêu cầu của anh
                string ngayDLFilter = DetermineNgayDLFilter(targetDate, targetMonth, targetYear);

                _logger.LogInformation("💰 Bắt đầu tính Nguồn vốn cho {UnitName} ({UnitKey}) - MA_CN: {MaCN}, MA_PGD: {MaPGD}, NgayDL: {NgayDL}",
                    branchInfo.DisplayName, unitKey, branchInfo.MaCN, branchInfo.MaPGD ?? "ALL", ngayDLFilter);

                decimal totalNguonVon;
                int recordCount;
                List<object> topAccounts;

                if (unitKey == "ToanTinh")
                {
                    // Tính toàn tỉnh (tổng từ 7800-7808)
                    var result = await CalculateAllProvince(ngayDLFilter);
                    totalNguonVon = result.Total;
                    recordCount = result.RecordCount;
                    topAccounts = result.TopAccounts;
                }
                else
                {
                    // Tính cho đơn vị cụ thể
                    var result = await CalculateSingleUnit(branchInfo, ngayDLFilter);
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
                        filterDate = ngayDLFilter,
                        topAccounts = topAccounts.Take(10).ToList(), // Top 10 tài khoản
                        formula = "Tổng CURRENT_BALANCE từ DP01_New - (loại trừ TK 40*, 41*, 427*, 211108)",
                        appliedFilters = new
                        {
                            maCN = branchInfo.MaCN,
                            maPGD = branchInfo.MaPGD,
                            ngayDL = ngayDLFilter,
                            excludedAccounts = new[] { "40*", "41*", "427*", "211108" }
                        }
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
        /// Xác định NgayDL filter theo priority: targetDate > targetMonth > targetYear > today
        /// </summary>
        private string DetermineNgayDLFilter(string? targetDate, string? targetMonth, string? targetYear)
        {
            // 1. Nếu có targetDate cụ thể (dd/MM/yyyy)
            if (!string.IsNullOrEmpty(targetDate))
            {
                return targetDate;
            }

            // 2. Nếu có targetMonth (MM/yyyy) -> lấy ngày cuối tháng
            if (!string.IsNullOrEmpty(targetMonth))
            {
                if (DateTime.TryParseExact(targetMonth, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime monthDate))
                {
                    var lastDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month));
                    return lastDayOfMonth.ToString("dd/MM/yyyy");
                }
            }

            // 3. Nếu có targetYear (yyyy) -> lấy ngày 31/12/yyyy
            if (!string.IsNullOrEmpty(targetYear))
            {
                if (int.TryParse(targetYear, out int year))
                {
                    var lastDayOfYear = new DateTime(year, 12, 31);
                    return lastDayOfYear.ToString("dd/MM/yyyy");
                }
            }

            // 4. Mặc định là ngày hôm nay
            return DateTime.Now.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Tính Nguồn vốn cho toàn tỉnh (tổng từ 7800-7808) - SỬ DỤNG DP01_New
        /// Lọc theo MA_CN + NgayDL (dd/MM/yyyy) + loại trừ tài khoản theo nghiệp vụ
        /// </summary>
        private async Task<CalculationResult> CalculateAllProvince(string ngayDLFilter)
        {
            var allBranchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };

            decimal totalNguonVon = 0;
            int totalRecordCount = 0;
            var allTopAccounts = new List<object>();

            // Tính tổng cho từng chi nhánh dựa trên MA_CN và NgayDL từ bảng DP01_New
            foreach (var maCN in allBranchCodes)
            {
                _logger.LogInformation("📊 Đang tính cho chi nhánh: {MaCN} ngày {Date}", maCN, ngayDLFilter);

                // Query dữ liệu từ bảng DP01_New (NEW TABLE) - using smart import data
                var query = _context.DP01_News
                    .Where(d => d.MA_CN == maCN && d.NgayDL == ngayDLFilter)
                    .Where(d =>
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                        d.TAI_KHOAN_HACH_TOAN != "211108"
                    );

                var branchTotal = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
                var branchCount = await query.CountAsync();

                totalNguonVon += branchTotal;
                totalRecordCount += branchCount;

                _logger.LogInformation("✅ Chi nhánh {MaCN}: {Total:N0} VND từ {Count} bản ghi",
                    maCN, branchTotal, branchCount);

                // Lấy top accounts của chi nhánh này
                if (branchCount > 0)
                {
                    var branchTopAccounts = await query
                        .OrderByDescending(d => Math.Abs(d.CURRENT_BALANCE ?? 0))
                        .Take(5)
                        .Select(d => new
                        {
                            MaCN = maCN,
                            AccountCode = d.TAI_KHOAN_HACH_TOAN,
                            CurrentBalance = d.CURRENT_BALANCE ?? 0,
                            NgayDL = d.NgayDL
                        })
                        .ToListAsync();

                    allTopAccounts.AddRange(branchTopAccounts.Cast<object>());
                }
            }

            // Sắp xếp lại top accounts theo số dư
            var topAccounts = allTopAccounts
                .Cast<dynamic>()
                .OrderByDescending(x => Math.Abs((decimal)x.CurrentBalance))
                .Take(20)
                .Cast<object>()
                .ToList();

            _logger.LogInformation("🏆 Tổng nguồn vốn toàn tỉnh: {Total:N0} VND từ {Count} bản ghi (NgayDL: {NgayDL})",
                totalNguonVon, totalRecordCount, ngayDLFilter);

            return new CalculationResult
            {
                Total = totalNguonVon,
                RecordCount = totalRecordCount,
                TopAccounts = topAccounts
            };
        }

        /// <summary>
        /// Tính Nguồn vốn cho một đơn vị cụ thể - SỬ DỤNG DP01_New
        /// Lọc theo MA_CN + MA_PGD (nếu có) + NgayDL + loại trừ tài khoản
        /// </summary>
        private async Task<CalculationResult> CalculateSingleUnit(BranchInfo branchInfo, string ngayDLFilter)
        {
            _logger.LogInformation("💰 Tính nguồn vốn cho {UnitName} (MA_CN: {MaCN}) ngày {NgayDL}",
                branchInfo.DisplayName, branchInfo.MaCN, ngayDLFilter);

            // Query dữ liệu từ bảng DP01_New (NEW TABLE) - using smart import data
            var query = _context.DP01_News
                .Where(d => d.MA_CN == branchInfo.MaCN && d.NgayDL == ngayDLFilter);

            // Áp dụng điều kiện lọc tài khoản theo yêu cầu của anh
            query = query.Where(d =>
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                d.TAI_KHOAN_HACH_TOAN != "211108"
            );

            // Nếu là PGD thì lọc thêm theo MA_PGD
            if (!string.IsNullOrEmpty(branchInfo.MaPGD))
            {
                query = query.Where(d => d.MA_PGD == branchInfo.MaPGD);
                _logger.LogInformation("🏪 Lọc thêm theo PGD: {MaPGD}", branchInfo.MaPGD);
            }

            var totalNguonVon = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
            var recordCount = await query.CountAsync();

            // Lấy top 20 tài khoản có số dư lớn nhất để debug và báo cáo
            var topAccounts = await query
                .OrderByDescending(d => Math.Abs(d.CURRENT_BALANCE ?? 0))
                .Take(20)
                .Select(d => new
                {
                    AccountCode = d.TAI_KHOAN_HACH_TOAN,
                    CurrentBalance = d.CURRENT_BALANCE ?? 0,
                    MaCN = d.MA_CN,
                    MaPGD = d.MA_PGD,
                    NgayDL = d.NgayDL,
                    FileName = d.FileName
                })
                .ToListAsync();

            _logger.LogInformation("✅ Kết quả {UnitName}: {Total:N0} VND từ {Count} bản ghi (NgayDL: {NgayDL})",
                branchInfo.DisplayName, totalNguonVon, recordCount, ngayDLFilter);

            if (recordCount == 0)
            {
                _logger.LogWarning("⚠️ Không tìm thấy dữ liệu cho MA_CN: {MaCN}, NgayDL: {NgayDL}", branchInfo.MaCN, ngayDLFilter);
            }

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

        /// <summary>
        /// Debug: Lấy danh sách các file DP01_New có trong database
        /// </summary>
        [HttpGet("debug/files")]
        public async Task<IActionResult> GetDP01Files()
        {
            try
            {
                var files = await _context.DP01_News
                    .Where(d => d.FileName != null)
                    .Select(d => d.FileName)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToListAsync();

                var fileInfo = files.Select(f => new
                {
                    FileName = f,
                    MaCN = f?.Length >= 4 ? f.Substring(0, 4) : "",
                    DatePart = f?.Length >= 18 ? f.Substring(10, 8) : "",
                    ParsedDate = f?.Length >= 18 ? ParseDateFromFileName(f.Substring(10, 8)) : null
                }).ToList();

                // Thống kê theo NgayDL
                var dateStats = await _context.DP01_News
                    .GroupBy(d => d.NgayDL)
                    .Select(g => new
                    {
                        NgayDL = g.Key,
                        RecordCount = g.Count()
                    })
                    .OrderBy(x => x.NgayDL)
                    .ToListAsync();

                _logger.LogInformation("🔍 Tìm thấy {Count} file DP01_New trong database", files.Count);

                return Ok(new
                {
                    success = true,
                    totalFiles = files.Count,
                    files = fileInfo.Take(20), // Chỉ hiển thị 20 files đầu
                    dateStatistics = dateStats.Take(20), // Top 20 ngày có dữ liệu
                    message = "Danh sách các file DP01_New trong database"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy danh sách file DP01_New");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Parse ngày từ tên file (yyyyMMdd -> DateTime)
        /// </summary>
        private DateTime? ParseDateFromFileName(string datePart)
        {
            if (string.IsNullOrEmpty(datePart) || datePart.Length != 8) return null;

            if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            return null;
        }

        /// <summary>
        /// Test endpoint để verify logic theo ví dụ của anh
        /// VD: Chi nhánh Bình Lư (7801) với ngày 30/04/2025
        /// </summary>
        [HttpPost("test-logic")]
        public async Task<IActionResult> TestNguonVonLogic([FromBody] TestNguonVonRequest request)
        {
            try
            {
                _logger.LogInformation("🧪 Testing Nguồn vốn logic: {BranchName} - {Date}", request.BranchName, request.Date);

                // Tìm branch mapping
                var branchKey = request.BranchKey ??
                    _branchMapping.FirstOrDefault(kvp => kvp.Value.DisplayName.Contains(request.BranchName ?? "")).Key;

                if (string.IsNullOrEmpty(branchKey) || !_branchMapping.ContainsKey(branchKey))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Không tìm thấy chi nhánh '{request.BranchName}'. Available: {string.Join(", ", _branchMapping.Values.Select(v => v.DisplayName))}"
                    });
                }

                var branchInfo = _branchMapping[branchKey];
                var ngayDL = request.Date;

                _logger.LogInformation("📋 Test parameters: MaCN={MaCN}, MaPGD={MaPGD}, NgayDL={NgayDL}",
                    branchInfo.MaCN, branchInfo.MaPGD, ngayDL);

                // Query step by step để debug
                var allRecords = await _context.DP01s
                    .Where(d => d.MA_CN == branchInfo.MaCN && d.DATA_DATE == DateTime.ParseExact(ngayDL, "dd/MM/yyyy", null))
                    .CountAsync();

                var afterAccountFilter = await _context.DP01s
                    .Where(d => d.MA_CN == branchInfo.MaCN && d.DATA_DATE == DateTime.ParseExact(ngayDL, "dd/MM/yyyy", null))
                    .Where(d =>
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                        d.TAI_KHOAN_HACH_TOAN != "211108"
                    )
                    .CountAsync();

                var finalQuery = _context.DP01s
                    .Where(d => d.MA_CN == branchInfo.MaCN && d.DATA_DATE == DateTime.ParseExact(ngayDL, "dd/MM/yyyy", null))
                    .Where(d =>
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                        d.TAI_KHOAN_HACH_TOAN != "211108"
                    );

                if (!string.IsNullOrEmpty(branchInfo.MaPGD))
                {
                    finalQuery = finalQuery.Where(d => d.MA_PGD == branchInfo.MaPGD);
                }

                var finalCount = await finalQuery.CountAsync();
                var totalBalance = await finalQuery.SumAsync(d => d.CURRENT_BALANCE ?? 0);

                var excludedAccounts = await _context.DP01s
                    .Where(d => d.MA_CN == branchInfo.MaCN && d.DATA_DATE == DateTime.ParseExact(ngayDL, "dd/MM/yyyy", null))
                    .Where(d =>
                        d.TAI_KHOAN_HACH_TOAN.StartsWith("40") ||
                        d.TAI_KHOAN_HACH_TOAN.StartsWith("41") ||
                        d.TAI_KHOAN_HACH_TOAN.StartsWith("427") ||
                        d.TAI_KHOAN_HACH_TOAN == "211108"
                    )
                    .GroupBy(d => d.TAI_KHOAN_HACH_TOAN)
                    .Select(g => new
                    {
                        AccountCode = g.Key,
                        Count = g.Count(),
                        TotalBalance = g.Sum(d => d.CURRENT_BALANCE ?? 0)
                    })
                    .OrderByDescending(x => Math.Abs(x.TotalBalance))
                    .Take(10)
                    .ToListAsync();

                var topAccounts = await finalQuery
                    .OrderByDescending(d => Math.Abs(d.CURRENT_BALANCE ?? 0))
                    .Take(10)
                    .Select(d => new
                    {
                        AccountCode = d.TAI_KHOAN_HACH_TOAN,
                        CurrentBalance = d.CURRENT_BALANCE ?? 0,
                        MaCN = d.MA_CN,
                        MaPGD = d.MA_PGD
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    testInput = new
                    {
                        branchName = request.BranchName,
                        branchKey = branchKey,
                        date = request.Date,
                        maCN = branchInfo.MaCN,
                        maPGD = branchInfo.MaPGD
                    },
                    filterSteps = new
                    {
                        step1_totalRecords = allRecords,
                        step2_afterAccountFilter = afterAccountFilter,
                        step3_finalWithPGD = finalCount,
                        step4_totalBalance = totalBalance,
                        step5_balanceInTrieuVND = Math.Round(totalBalance / 1_000_000m, 2)
                    },
                    excludedAccountsSample = excludedAccounts,
                    topAccountsIncluded = topAccounts,
                    conclusion = new
                    {
                        isImplementedCorrectly = finalCount > 0,
                        message = finalCount > 0
                            ? $"✅ Logic implemented correctly. Found {finalCount} records with total balance {totalBalance:N0} VND"
                            : $"❌ No data found for MaCN={branchInfo.MaCN}, NgayDL={ngayDL}. Check data availability."
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error testing Nguồn vốn logic");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Debug: Kiểm tra dữ liệu thực tế trong bảng DP01 cũ
        /// </summary>
        [HttpGet("debug/dp01-data")]
        public async Task<IActionResult> GetDP01Data()
        {
            try
            {
                // Lấy các mã chi nhánh có trong DP01
                var branchCodes = await _context.DP01s
                    .Select(x => x.MA_CN)
                    .Where(x => x != null)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                // Lấy các ngày có data, đặc biệt là 2024
                var datesIn2024 = await _context.DP01s
                    .Where(x => x.DATA_DATE.Year == 2024)
                    .Select(x => x.DATA_DATE)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .Take(20)
                    .ToListAsync();

                // Kiểm tra đặc biệt cho MA_CN 7800
                var hoisoData = await _context.DP01s
                    .Where(x => x.MA_CN == "7800")
                    .Select(x => new { x.DATA_DATE, x.FileName })
                    .Distinct()
                    .OrderByDescending(x => x.DATA_DATE)
                    .Take(10)
                    .ToListAsync();

                // Tổng số bản ghi trong DP01
                var totalRecords = await _context.DP01s.CountAsync();

                return Ok(new
                {
                    success = true,
                    totalRecords = totalRecords,
                    branchCodes = branchCodes,
                    datesIn2024 = datesIn2024.Select(d => d.ToString("dd/MM/yyyy")).ToList(),
                    hoisoData = hoisoData.Select(x => new
                    {
                        date = x.DATA_DATE.ToString("dd/MM/yyyy"),
                        fileName = x.FileName
                    }).ToList(),
                    message = "Thông tin dữ liệu bảng DP01 cũ"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    message = "Lỗi khi truy vấn bảng DP01"
                });
            }
        }

        /// <summary>
        /// Debug: Kiểm tra dữ liệu trong bảng ImportedDataRecords
        /// </summary>
        [HttpGet("debug/imported-data")]
        public async Task<IActionResult> GetImportedDataInfo()
        {
            try
            {
                // Lấy tổng số bản ghi
                var totalRecords = await _context.ImportedDataRecords.CountAsync();

                // Lấy thống kê theo file type
                var fileTypes = await _context.ImportedDataRecords
                    .GroupBy(x => x.FileType)
                    .Select(g => new
                    {
                        FileType = g.Key,
                        Count = g.Count(),
                        LatestImport = g.Max(x => x.ImportDate)
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                // Lấy các file DP01 cụ thể
                var dp01Files = await _context.ImportedDataRecords
                    .Where(x => x.FileType == "DP01" || x.FileName.Contains("DP01") || x.FileName.Contains("dp01"))
                    .Select(x => new
                    {
                        x.FileName,
                        x.ImportDate,
                        x.StatementDate,
                        x.RecordsCount,
                        x.Status
                    })
                    .OrderByDescending(x => x.StatementDate)
                    .Take(20)
                    .ToListAsync();

                // Kiểm tra dữ liệu 2024
                var data2024 = await _context.ImportedDataRecords
                    .Where(x => x.StatementDate.HasValue && x.StatementDate.Value.Year == 2024)
                    .GroupBy(x => x.FileType)
                    .Select(g => new
                    {
                        FileType = g.Key,
                        Count = g.Count(),
                        LatestDate = g.Max(x => x.StatementDate)
                    })
                    .OrderBy(x => x.FileType)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    totalRecords = totalRecords,
                    fileTypeStatistics = fileTypes,
                    dp01Files = dp01Files,
                    data2024Statistics = data2024,
                    message = "Thông tin dữ liệu trong bảng ImportedDataRecords"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    message = "Lỗi khi truy vấn ImportedDataRecords"
                });
            }
        }

        /// <summary>
        /// 🔄 LEGACY DEBUG: Kiểm tra dữ liệu chi tiết trong ImportedDataItems (actual data records)
        /// NOTE: Sử dụng cho legacy data only, new workflow sẽ query trực tiếp từ bảng DP01
        /// </summary>
        [HttpGet("debug/imported-data-items")]
        public async Task<IActionResult> GetImportedDataItems([FromQuery] string? fileType = "DP01")
        {
            try
            {
                var query = _context.ImportedDataItems.AsQueryable();

                if (!string.IsNullOrEmpty(fileType))
                {
                    query = query.Where(x => x.ImportedDataRecord.FileType == fileType);
                }

                // Lấy sample data
                var sampleData = await query
                    .Include(x => x.ImportedDataRecord)
                    .OrderByDescending(x => x.ImportedDataRecord.StatementDate)
                    .Take(50)
                    .Select(x => new
                    {
                        x.Id,
                        x.RawData,
                        x.ProcessedDate,
                        FileInfo = new
                        {
                            x.ImportedDataRecord.FileName,
                            x.ImportedDataRecord.FileType,
                            x.ImportedDataRecord.StatementDate,
                            x.ImportedDataRecord.ImportDate
                        }
                    })
                    .ToListAsync();

                // Thống kê theo năm
                var yearStats = await query
                    .Where(x => x.ImportedDataRecord.StatementDate.HasValue)
                    .GroupBy(x => x.ImportedDataRecord.StatementDate.Value.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        RecordCount = g.Count(),
                        FileCount = g.Select(x => x.ImportedDataRecord.Id).Distinct().Count()
                    })
                    .OrderByDescending(x => x.Year)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    sampleDataCount = sampleData.Count,
                    sampleData = sampleData.Take(10).ToList(),
                    yearStatistics = yearStats,
                    filters = new { fileType },
                    message = "Dữ liệu chi tiết từ ImportedDataItems"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    message = "Lỗi khi truy vấn ImportedDataItems"
                });
            }
        }

        /// <summary>
        /// Debug: Kiểm tra tất cả các bảng dữ liệu có sẵn để tìm data 2024
        /// </summary>
        [HttpGet("debug/all-tables-summary")]
        public async Task<IActionResult> GetAllTablesSummary()
        {
            try
            {
                var summary = new List<object>();

                // Kiểm tra DP01s (old table)
                var dp01Count = await _context.DP01s.CountAsync();
                var dp01_2024 = await _context.DP01s.Where(x => x.DATA_DATE.Year == 2024).CountAsync();
                summary.Add(new { TableName = "DP01s", TotalRecords = dp01Count, Records2024 = dp01_2024 });

                // Kiểm tra DP01NewTables (new table)
                var dp01NewCount = await _context.DP01NewTables.CountAsync();
                var dp01New_2024 = await _context.DP01NewTables.Where(x => x.NgayDL.Contains("2024")).CountAsync();
                summary.Add(new { TableName = "DP01NewTables", TotalRecords = dp01NewCount, Records2024 = dp01New_2024 });

                // Kiểm tra LN01s
                var ln01Count = await _context.LN01s.CountAsync();
                var ln01_2024 = await _context.LN01s.Where(x => x.NgayDL.Contains("2024")).CountAsync();
                summary.Add(new { TableName = "LN01s", TotalRecords = ln01Count, Records2024 = ln01_2024 });

                // Kiểm tra GL01s
                var gl01Count = await _context.GL01s.CountAsync();
                var gl01_2024 = await _context.GL01s.Where(x => x.NgayDL.Contains("2024")).CountAsync();
                summary.Add(new { TableName = "GL01s", TotalRecords = gl01Count, Records2024 = gl01_2024 });

                // Kiểm tra ImportedDataRecords
                var importedCount = await _context.ImportedDataRecords.CountAsync();
                var imported_2024 = await _context.ImportedDataRecords
                    .Where(x => x.StatementDate.HasValue && x.StatementDate.Value.Year == 2024)
                    .CountAsync();
                summary.Add(new { TableName = "ImportedDataRecords", TotalRecords = importedCount, Records2024 = imported_2024 });

                // Tìm bảng nào có nhiều data nhất cho 2024
                var bestTable = summary.OrderByDescending(x => ((dynamic)x).Records2024).FirstOrDefault();

                return Ok(new
                {
                    success = true,
                    allTablesStatus = summary,
                    recommendation = bestTable != null ? $"Bảng {((dynamic)bestTable).TableName} có nhiều data 2024 nhất ({((dynamic)bestTable).Records2024} records)" : "Không tìm thấy data 2024",
                    message = "Tóm tắt tất cả các bảng dữ liệu"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    message = "Lỗi khi kiểm tra các bảng dữ liệu"
                });
            }
        }

        /// <summary>
        /// Debug: Raw SQL query to check DP01_New table directly
        /// </summary>
        [HttpGet("debug/raw-dp01-check")]
        public async Task<IActionResult> GetRawDP01Check()
        {
            try
            {
                // Direct count query
                var totalCount = await _context.DP01_News.CountAsync();

                // Get sample records with all fields
                var sampleRecords = await _context.DP01_News
                    .OrderByDescending(d => d.CreatedDate)
                    .Take(5)
                    .ToListAsync();

                // Check unique NgayDL values
                var uniqueDates = await _context.DP01_News
                    .Select(d => d.NgayDL)
                    .Distinct()
                    .ToListAsync();

                // Check unique MA_CN values
                var uniqueBranches = await _context.DP01_News
                    .Select(d => d.MA_CN)
                    .Distinct()
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    totalRecords = totalCount,
                    sampleRecords = sampleRecords,
                    uniqueDates = uniqueDates,
                    uniqueBranches = uniqueBranches,
                    message = "Raw DP01_New table check"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }

    /// <summary>
    /// Request model cho test endpoint
    /// </summary>
    public class TestNguonVonRequest
    {
        public string? BranchKey { get; set; }
        public string? BranchName { get; set; }
        public string Date { get; set; } = "";
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
