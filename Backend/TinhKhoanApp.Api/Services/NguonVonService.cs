using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.NguonVon;
using TinhKhoanApp.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service tính toán nguồn vốn từ bảng DP01
    /// </summary>
    public class NguonVonService : INguonVonService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguonVonService> _logger;

        // Mapping mã chi nhánh theo yêu cầu
        private readonly Dictionary<string, string> _branchMapping = new Dictionary<string, string>
        {
            { "HoiSo", "7800" },           // Hội sở
            { "CnBinhLu", "7801" },        // Chi nhánh Bình Lư (Tam Đường cũ)
            { "CnPhongTho", "7802" },      // Chi nhánh Phong Thổ
            { "CnSinHo", "7803" },         // Chi nhánh Sìn Hồ
            { "CnBumTo", "7804" },         // Chi nhánh Bum Tở (Mường Tè cũ)
            { "CnThanUyen", "7805" },      // Chi nhánh Than Uyên
            { "CnDoanKet", "7806" },       // Chi nhánh Đoàn Kết (Thành Phố cũ)
            { "CnTanUyen", "7807" },       // Chi nhánh Tân Uyên
            { "CnNamHang", "7808" }        // Chi nhánh Nậm Hàng (Nậm Nhùn cũ)
        };

        // Mapping PGD theo yêu cầu
        private readonly Dictionary<string, (string branchCode, string pgdCode)> _pgdMapping = new Dictionary<string, (string, string)>
        {
            { "CnPhongThoPgdSo5", ("7802", "01") },    // CN Phong Thổ - PGD Số 5 (Mường So cũ)
            { "CnThanUyenPgdSo6", ("7805", "01") },    // CN Than Uyên - PGD Số 6 (Mường Than cũ)
            { "CnDoanKetPgdSo1", ("7806", "01") },     // CN Đoàn Kết - PGD Số 1
            { "CnDoanKetPgdSo2", ("7806", "02") },     // CN Đoàn Kết - PGD Số 2
            { "CnTanUyenPgdSo3", ("7807", "01") }      // CN Tân Uyên - PGD Số 3
        };

        public NguonVonService(ApplicationDbContext context, ILogger<NguonVonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Xác định ngày target dựa trên loại input
        /// </summary>
        public DateTime DetermineTargetDate(DateTime inputDate, string dateType)
        {
            switch (dateType?.ToLower())
            {
                case "year":
                    // Nếu input là năm thì lấy ngày cuối năm (31/12/yyyy)
                    return new DateTime(inputDate.Year, 12, 31);

                case "month":
                    // Nếu input là tháng thì lấy ngày cuối tháng
                    var daysInMonth = DateTime.DaysInMonth(inputDate.Year, inputDate.Month);
                    return new DateTime(inputDate.Year, inputDate.Month, daysInMonth);

                case "day":
                default:
                    // Nếu input là ngày cụ thể thì lấy ngày đó
                    return inputDate.Date;
            }
        }

        /// <summary>
        /// Tính toán nguồn vốn theo đơn vị và ngày
        /// </summary>
        public async Task<NguonVonResult> CalculateNguonVonAsync(string unitCode, DateTime targetDate)
        {
            try
            {
                _logger.LogInformation("📊 Bắt đầu tính toán nguồn vốn cho đơn vị: {Unit}, ngày: {Date}", unitCode, targetDate.ToString("dd/MM/yyyy"));

                // Nếu chọn "Tất cả đơn vị" thì tính tổng cho toàn tỉnh
                if (unitCode == "ALL" || unitCode == "TatCaDonVi")
                {
                    return await CalculateAllUnitsAsync(targetDate);
                }

                // Tính cho đơn vị cụ thể (chi nhánh hoặc PGD)
                return await CalculateSingleUnitAsync(unitCode, targetDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tính toán nguồn vốn cho {Unit}", unitCode);
                throw;
            }
        }

        /// <summary>
        /// Tính nguồn vốn cho một đơn vị cụ thể
        /// </summary>
        private async Task<NguonVonResult> CalculateSingleUnitAsync(string unitCode, DateTime targetDate)
        {
            // Xác định mã chi nhánh và PGD
            string maCN = null;
            string maPGD = null;

            // Kiểm tra xem có phải là PGD không
            if (_pgdMapping.ContainsKey(unitCode))
            {
                var pgdInfo = _pgdMapping[unitCode];
                maCN = pgdInfo.branchCode;
                maPGD = pgdInfo.pgdCode;
                _logger.LogInformation("🏦 Tính cho PGD: {Unit} -> CN: {Branch}, PGD: {PGD}", unitCode, maCN, maPGD);
            }
            else if (_branchMapping.ContainsKey(unitCode))
            {
                maCN = _branchMapping[unitCode];
                _logger.LogInformation("🏢 Tính cho Chi nhánh: {Unit} -> Mã: {Branch}", unitCode, maCN);
            }
            else
            {
                throw new ArgumentException($"Không tìm thấy mã đơn vị: {unitCode}");
            }

            // Query dữ liệu từ DP01 theo ngày và chi nhánh
            var query = _context.DP01s
                .Where(d => d.NgayDL == targetDate.ToString("dd/MM/yyyy") && d.MA_CN == maCN);

            // Nếu là PGD thì lọc thêm theo MA_PGD
            if (!string.IsNullOrEmpty(maPGD))
            {
                query = query.Where(d => d.MA_PGD == maPGD);
            }

            // Lọc tài khoản theo điều kiện: bỏ đi 40*, 41*, 427*, 211108
            query = query.Where(d =>
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                d.TAI_KHOAN_HACH_TOAN != "211108"
            );

            // Tính tổng CURRENT_BALANCE
            var totalBalance = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
            var recordCount = await query.CountAsync();

            _logger.LogInformation("💰 Kết quả tính toán: {Balance:N0} VND từ {Count} bản ghi", totalBalance, recordCount);

            // Nếu không có dữ liệu thì kiểm tra xem có bản ghi nào không
            if (totalBalance == 0 && recordCount == 0)
            {
                var hasAnyData = await _context.DP01s
                    .AnyAsync(d => d.NgayDL == targetDate.ToString("dd/MM/yyyy") && d.MA_CN == maCN);

                if (!hasAnyData)
                {
                    _logger.LogWarning("⚠️ Không tìm thấy dữ liệu cho ngày {Date} và mã CN {Branch}", targetDate.ToString("dd/MM/yyyy"), maCN);
                    return null; // Không có dữ liệu cho ngày này
                }
            }

            return new NguonVonResult
            {
                UnitCode = unitCode,
                UnitName = GetUnitName(unitCode),
                TotalBalance = totalBalance,
                CalculatedDate = targetDate,
                RecordCount = recordCount
            };
        }

        /// <summary>
        /// Tính nguồn vốn cho tất cả đơn vị (toàn tỉnh)
        /// </summary>
        private async Task<NguonVonResult> CalculateAllUnitsAsync(DateTime targetDate)
        {
            _logger.LogInformation("🌍 Tính toán nguồn vốn toàn tỉnh (mã 7800-7808)");

            // Lấy tất cả mã chi nhánh (7800-7808)
            var allBranchCodes = _branchMapping.Values.ToList();

            // Query tổng cho tất cả chi nhánh
            var query = _context.DP01s
                .Where(d => d.NgayDL == targetDate.ToString("dd/MM/yyyy") && allBranchCodes.Contains(d.MA_CN))
                .Where(d =>
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                    !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                    d.TAI_KHOAN_HACH_TOAN != "211108"
                );

            var totalBalance = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
            var recordCount = await query.CountAsync();

            _logger.LogInformation("💰 Tổng nguồn vốn toàn tỉnh: {Balance:N0} VND từ {Count} bản ghi", totalBalance, recordCount);

            // Kiểm tra xem có dữ liệu không
            if (totalBalance == 0 && recordCount == 0)
            {
                var hasAnyData = await _context.DP01s
                    .AnyAsync(d => d.NgayDL == targetDate.ToString("dd/MM/yyyy") && allBranchCodes.Contains(d.MA_CN));

                if (!hasAnyData)
                {
                    _logger.LogWarning("⚠️ Không tìm thấy dữ liệu toàn tỉnh cho ngày {Date}", targetDate.ToString("dd/MM/yyyy"));
                    return null; // Không có dữ liệu cho ngày này
                }
            }

            return new NguonVonResult
            {
                UnitCode = "ALL",
                UnitName = "Tất cả đơn vị (Toàn tỉnh)",
                TotalBalance = totalBalance,
                CalculatedDate = targetDate,
                RecordCount = recordCount
            };
        }

        /// <summary>
        /// Lấy chi tiết nguồn vốn để debug
        /// </summary>
        public async Task<NguonVonDetails> GetNguonVonDetailsAsync(string unitCode, DateTime targetDate)
        {
            // Tính toán tổng kết
            var result = await CalculateNguonVonAsync(unitCode, targetDate);

            if (result == null)
            {
                return new NguonVonDetails
                {
                    Message = "Không tìm thấy dữ liệu cho ngày chỉ định",
                    HasData = false
                };
            }

            // Lấy chi tiết các tài khoản (top 20 có số dư lớn nhất)
            string maCN = null;
            string maPGD = null;

            if (_pgdMapping.ContainsKey(unitCode))
            {
                var pgdInfo = _pgdMapping[unitCode];
                maCN = pgdInfo.branchCode;
                maPGD = pgdInfo.pgdCode;
            }
            else if (_branchMapping.ContainsKey(unitCode))
            {
                maCN = _branchMapping[unitCode];
            }
            else if (unitCode == "ALL")
            {
                // Xử lý tất cả đơn vị
                var allBranchCodes = _branchMapping.Values.ToList();
                var accountDetails = await _context.DP01s
                    .Where(d => d.NgayDL == targetDate.ToString("dd/MM/yyyy") && allBranchCodes.Contains(d.MA_CN))
                    .Where(d =>
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                        d.TAI_KHOAN_HACH_TOAN != "211108"
                    )
                    .GroupBy(d => d.TAI_KHOAN_HACH_TOAN)
                    .Select(g => new AccountDetail
                    {
                        AccountCode = g.Key,
                        TotalBalance = g.Sum(x => x.CURRENT_BALANCE ?? 0),
                        RecordCount = g.Count()
                    })
                    .OrderByDescending(a => a.TotalBalance)
                    .Take(20)
                    .ToListAsync();

                return new NguonVonDetails
                {
                    Summary = result,
                    TopAccounts = accountDetails,
                    HasData = true,
                    Message = "Thành công"
                };
            }

            // Lấy chi tiết cho đơn vị cụ thể
            if (!string.IsNullOrEmpty(maCN))
            {
                var query = _context.DP01s
                    .Where(d => d.NgayDL == targetDate.ToString("dd/MM/yyyy") && d.MA_CN == maCN)
                    .Where(d =>
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                        d.TAI_KHOAN_HACH_TOAN != "211108"
                    );

                // Nếu là PGD thì lọc thêm
                if (!string.IsNullOrEmpty(maPGD))
                {
                    query = query.Where(d => d.MA_PGD == maPGD);
                }

                var accountDetails = await query
                    .GroupBy(d => d.TAI_KHOAN_HACH_TOAN)
                    .Select(g => new AccountDetail
                    {
                        AccountCode = g.Key,
                        TotalBalance = g.Sum(x => x.CURRENT_BALANCE ?? 0),
                        RecordCount = g.Count()
                    })
                    .OrderByDescending(a => a.TotalBalance)
                    .Take(20)
                    .ToListAsync();

                return new NguonVonDetails
                {
                    Summary = result,
                    TopAccounts = accountDetails,
                    HasData = true,
                    Message = "Thành công"
                };
            }

            return new NguonVonDetails
            {
                Summary = result,
                TopAccounts = new List<AccountDetail>(),
                HasData = true,
                Message = "Thành công"
            };
        }

        /// <summary>
        /// Lấy tên đơn vị từ mã
        /// </summary>
        private string GetUnitName(string unitCode)
        {
            var unitNames = new Dictionary<string, string>
            {
                { "HoiSo", "Hội sở" },
                { "CnBinhLu", "Chi nhánh Bình Lư" },
                { "CnPhongTho", "Chi nhánh Phong Thổ" },
                { "CnSinHo", "Chi nhánh Sìn Hồ" },
                { "CnBumTo", "Chi nhánh Bum Tở" },
                { "CnThanUyen", "Chi nhánh Than Uyên" },
                { "CnDoanKet", "Chi nhánh Đoàn Kết" },
                { "CnTanUyen", "Chi nhánh Tân Uyên" },
                { "CnNamHang", "Chi nhánh Nậm Hàng" },
                { "CnPhongThoPgdSo5", "CN Phong Thổ - PGD Số 5" },
                { "CnThanUyenPgdSo6", "CN Than Uyên - PGD Số 6" },
                { "CnDoanKetPgdSo1", "CN Đoàn Kết - PGD Số 1" },
                { "CnDoanKetPgdSo2", "CN Đoàn Kết - PGD Số 2" },
                { "CnTanUyenPgdSo3", "CN Tân Uyên - PGD Số 3" },
                { "ALL", "Tất cả đơn vị (Toàn tỉnh)" }
            };

            return unitNames.ContainsKey(unitCode) ? unitNames[unitCode] : unitCode;
        }
    }
}
