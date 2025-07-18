using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using System.Globalization;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NguonVonButtonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguonVonButtonController> _logger;

        public NguonVonButtonController(ApplicationDbContext context, ILogger<NguonVonButtonController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tính toán Nguồn vốn từ bảng DP01 theo chi nhánh và thời gian
        /// </summary>
        [HttpPost("calculate/{unitCode}")]
        public async Task<IActionResult> CalculateNguonVon(string unitCode, [FromQuery] string targetMonth)
        {
            try
            {
                _logger.LogInformation("🧮 [NGUON_VON] Tính toán cho unit: {UnitCode}, targetMonth: {TargetMonth}", unitCode, targetMonth);

                // Parse target date từ frontend
                var targetDate = ParseTargetDate(targetMonth);
                if (!targetDate.HasValue)
                {
                    return BadRequest(new { error = "Định dạng thời gian không hợp lệ" });
                }

                _logger.LogInformation("🗓️ [NGUON_VON] Target date parsed: {TargetDate}", targetDate.Value.ToString("dd/MM/yyyy"));

                // Get mã chi nhánh tương ứng
                var maCN = GetMaChiNhanh(unitCode);
                var maPGD = GetMaPhongGiaoDich(unitCode);

                _logger.LogInformation("🏢 [NGUON_VON] MA_CN: {MaCN}, MA_PGD: {MaPGD}", maCN, maPGD);

                // Kiểm tra có dữ liệu ngày này không
                var hasData = await CheckDataExists(targetDate.Value, maCN, maPGD);
                if (!hasData)
                {
                    return BadRequest(new { error = "Kho dữ liệu chưa có ngày này!" });
                }

                // Tính toán Nguồn vốn
                var result = await CalculateNguonVonFromDP01(targetDate.Value, maCN, maPGD, unitCode);

                _logger.LogInformation("💰 [NGUON_VON] Kết quả tính toán thành công");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [NGUON_VON] Lỗi tính toán nguồn vốn: {Error}", ex.Message);
                return StatusCode(500, new { error = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// Parse target date từ frontend (MM/yyyy, yyyy, dd/MM/yyyy)
        /// </summary>
        private DateTime? ParseTargetDate(string targetMonth)
        {
            if (string.IsNullOrEmpty(targetMonth))
                return null;

            try
            {
                // Format: MM/yyyy (tháng/năm) - lấy ngày cuối tháng
                if (DateTime.TryParseExact(targetMonth, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime monthYear))
                {
                    // Lấy ngày cuối tháng
                    return new DateTime(monthYear.Year, monthYear.Month, DateTime.DaysInMonth(monthYear.Year, monthYear.Month));
                }

                // Format: yyyy (năm) - lấy 31/12
                if (int.TryParse(targetMonth, out int year) && year >= 2020 && year <= 2030)
                {
                    return new DateTime(year, 12, 31);
                }

                // Format: dd/MM/yyyy (ngày cụ thể)
                if (DateTime.TryParseExact(targetMonth, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime specificDate))
                {
                    return specificDate;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get mã chi nhánh theo unit code
        /// </summary>
        private string? GetMaChiNhanh(string unitCode)
        {
            return unitCode switch
            {
                "HoiSo" => "7800",
                "BinhLu" => "7801",
                "PhongTho" => "7802",
                "SinHo" => "7803",
                "BumTo" => "7804",
                "ThanUyen" => "7805",
                "DoanKet" => "7806",
                "TanUyen" => "7807",
                "NamHang" => "7808",
                "PhongTho_PGD5" => "7802", // CN Phong Thổ + PGD
                "ThanUyen_PGD6" => "7805", // CN Than Uyên + PGD
                "DoanKet_PGD1" => "7806",  // CN Đoàn Kết + PGD 1
                "DoanKet_PGD2" => "7806",  // CN Đoàn Kết + PGD 2
                "TanUyen_PGD3" => "7807",  // CN Tân Uyên + PGD
                _ => null // Toàn tỉnh hoặc không xác định
            };
        }

        /// <summary>
        /// Get mã phòng giao dịch nếu có
        /// </summary>
        private string? GetMaPhongGiaoDich(string unitCode)
        {
            return unitCode switch
            {
                "PhongTho_PGD5" => "'01",
                "ThanUyen_PGD6" => "'01",
                "DoanKet_PGD1" => "'01",
                "DoanKet_PGD2" => "'02",
                "TanUyen_PGD3" => "'01",
                _ => null
            };
        }

        /// <summary>
        /// Kiểm tra có dữ liệu ngày này không
        /// </summary>
        private async Task<bool> CheckDataExists(DateTime targetDate, string? maCN, string? maPGD)
        {
            var query = _context.DP01.Where(x => x.NGAY_DL.Date == targetDate.Date);

            if (!string.IsNullOrEmpty(maCN))
            {
                query = query.Where(x => x.MA_CN == maCN);
            }

            if (!string.IsNullOrEmpty(maPGD))
            {
                query = query.Where(x => x.MA_PGD == maPGD);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Tính toán Nguồn vốn từ bảng DP01
        /// </summary>
        private async Task<object> CalculateNguonVonFromDP01(DateTime targetDate, string? maCN, string? maPGD, string unitCode)
        {
            var query = _context.DP01.Where(x => x.NGAY_DL.Date == targetDate.Date);

            // Lọc theo chi nhánh
            if (unitCode == "ToanTinh")
            {
                // Toàn tỉnh: lấy tất cả từ 7800-7808
                query = query.Where(x => x.MA_CN != null &&
                    (x.MA_CN == "7800" || x.MA_CN == "7801" || x.MA_CN == "7802" ||
                     x.MA_CN == "7803" || x.MA_CN == "7804" || x.MA_CN == "7805" ||
                     x.MA_CN == "7806" || x.MA_CN == "7807" || x.MA_CN == "7808"));
            }
            else if (!string.IsNullOrEmpty(maCN))
            {
                query = query.Where(x => x.MA_CN == maCN);

                // Lọc thêm theo PGD nếu có
                if (!string.IsNullOrEmpty(maPGD))
                {
                    query = query.Where(x => x.MA_PGD == maPGD);
                }
            }

            // Lọc bỏ các tài khoản không tính vào nguồn vốn
            query = query.Where(x => x.TAI_KHOAN_HACH_TOAN != null &&
                !x.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                !x.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                !x.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                x.TAI_KHOAN_HACH_TOAN != "211108");

            // Tính tổng CURRENT_BALANCE (convert từ string sang decimal)
            var records = await query.ToListAsync();
            var totalAmount = records
                .Where(x => !string.IsNullOrEmpty(x.CURRENT_BALANCE))
                .Sum(x =>
                {
                    if (decimal.TryParse(x.CURRENT_BALANCE, out decimal amount))
                        return amount;
                    return 0;
                });
            var recordCount = records.Count;

            _logger.LogInformation("📊 [NGUON_VON] Tổng {RecordCount} records, Total: {Amount:N0}", recordCount, totalAmount);

            return new
            {
                UnitCode = unitCode,
                UnitName = GetUnitDisplayName(unitCode),
                MaChiNhanh = maCN,
                MaPhongGiaoDich = maPGD,
                TargetDate = targetDate.ToString("dd/MM/yyyy"),
                TotalAmount = totalAmount,
                RecordCount = recordCount,
                CalculatedAt = DateTime.Now,
                ExcludedAccounts = new[] { "40*", "41*", "427*", "211108" },
                Message = $"Nguồn vốn {GetUnitDisplayName(unitCode)} tại ngày {targetDate:dd/MM/yyyy}"
            };
        }

        /// <summary>
        /// Get display name cho unit
        /// </summary>
        private string GetUnitDisplayName(string unitCode)
        {
            return unitCode switch
            {
                "HoiSo" => "Hội Sở",
                "BinhLu" => "CN Bình Lư",
                "PhongTho" => "CN Phong Thổ",
                "SinHo" => "CN Sìn Hồ",
                "BumTo" => "CN Bum Tở",
                "ThanUyen" => "CN Than Uyên",
                "DoanKet" => "CN Đoàn Kết",
                "TanUyen" => "CN Tân Uyên",
                "NamHang" => "CN Nậm Hàng",
                "PhongTho_PGD5" => "CN Phong Thổ - PGD Số 5",
                "ThanUyen_PGD6" => "CN Than Uyên - PGD Số 6",
                "DoanKet_PGD1" => "CN Đoàn Kết - PGD Số 1",
                "DoanKet_PGD2" => "CN Đoàn Kết - PGD Số 2",
                "TanUyen_PGD3" => "CN Tân Uyên - PGD Số 3",
                "ToanTinh" => "Toàn tỉnh",
                _ => unitCode
            };
        }
    }
}
