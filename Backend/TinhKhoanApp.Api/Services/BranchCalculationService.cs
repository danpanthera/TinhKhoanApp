using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using System.Text.Json;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service tính toán 6 chỉ tiêu chính theo quy ước mới
    /// CN Lai Châu (9999) = tổng của 9 CN còn lại (7800-7808)
    /// </summary>
    public interface IBranchCalculationService
    {
        Task<decimal> CalculateNguonVonByBranch(string branchId, DateTime? date = null);
        Task<decimal> CalculateDuNoByBranch(string branchId, DateTime? date = null);
        Task<decimal> CalculateNoXauByBranch(string branchId, DateTime? date = null);
        Task<decimal> CalculateThuHoiXLRRByBranch(string branchId, DateTime? date = null);
        Task<decimal> CalculateThuDichVuByBranch(string branchId, DateTime? date = null);
        Task<decimal> CalculateLoiNhuanByBranch(string branchId, DateTime? date = null);
    }

    public class BranchCalculationService : IBranchCalculationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BranchCalculationService> _logger;

        public BranchCalculationService(
            ApplicationDbContext context,
            ILogger<BranchCalculationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 1. Nguồn vốn - từ bảng DP01 (Mock data)
        /// </summary>
        public async Task<decimal> CalculateNguonVonByBranch(string branchId, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Nguồn vốn cho chi nhánh {BranchId}", branchId);

                if (branchId == "CnLaiChau")
                {
                    // CN Lai Châu = tổng của 9 CN (7800-7808)
                    var totalNguonVon = 0m;
                    var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };

                    foreach (var code in branchCodes)
                    {
                        var branchValue = await CalculateNguonVonForSingleBranch(code, null, date);
                        totalNguonVon += branchValue;
                    }

                    _logger.LogInformation("Tổng Nguồn vốn CN Lai Châu: {Total}", totalNguonVon);
                    return totalNguonVon;
                }
                else
                {
                    // Chi nhánh đơn lẻ hoặc PGD
                    var branchCode = GetBranchCode(branchId);
                    var pgdCode = GetPgdCode(branchId);

                    return await CalculateNguonVonForSingleBranch(branchCode, pgdCode, date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nguồn vốn cho chi nhánh {BranchId}", branchId);
                return 0m;
            }
        }

        private async Task<decimal> CalculateNguonVonForSingleBranch(string branchCode, string? pgdCode, DateTime? date)
        {
            try
            {
                // Lấy dữ liệu thực từ bảng DP01 (ImportedDataRecords/ImportedDataItems)
                var dp01Data = await GetDP01DataForBranch(branchCode, pgdCode);

                if (dp01Data.Any())
                {
                    var totalNguonVon = CalculateNguonVonFromDP01(dp01Data, branchCode, pgdCode);
                    _logger.LogInformation("Nguồn vốn thực tế từ DP01 - Chi nhánh {BranchCode} PGD {PgdCode}: {Total}",
                        branchCode, pgdCode ?? "ALL", totalNguonVon);
                    return totalNguonVon;
                }
                else
                {
                    // Fallback về mock data nếu không có dữ liệu thực
                    _logger.LogWarning("Không tìm thấy dữ liệu DP01 thực, sử dụng mock data cho {BranchCode}", branchCode);

                    var mockData = branchCode switch
                    {
                        "7800" => pgdCode switch
                        {
                            "01" => 45.5m * 1000000000, // 45.5 tỷ
                            "02" => 38.2m * 1000000000, // 38.2 tỷ
                            _ => 125.7m * 1000000000    // 125.7 tỷ
                        },
                        "7801" => 89.3m * 1000000000,   // CN Tam Đường
                        "7802" => 76.8m * 1000000000,   // CN Phong Thổ
                        "7803" => 45.2m * 1000000000,   // CN Sin Hồ
                        "7804" => 34.6m * 1000000000,   // CN Mường Tè
                        "7805" => 67.4m * 1000000000,   // CN Than Uyên
                        "7806" => 98.5m * 1000000000,   // CN Thành Phố
                        "7807" => 55.9m * 1000000000,   // CN Tân Uyên
                        "7808" => 42.1m * 1000000000,   // CN Nậm Nhùn
                        _ => 50.0m * 1000000000         // Default
                    };

                    return mockData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nguồn vốn cho chi nhánh {BranchCode}", branchCode);
                return 0m;
            }
        }

        /// <summary>
        /// 2. Dư nợ - từ bảng LN01 (Mock data)
        /// </summary>
        public async Task<decimal> CalculateDuNoByBranch(string branchId, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Dư nợ cho chi nhánh {BranchId}", branchId);

                if (branchId == "CnLaiChau")
                {
                    // CN Lai Châu = tổng toàn tỉnh
                    var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
                    var totalDuNo = 0m;

                    foreach (var code in branchCodes)
                    {
                        var branchValue = await CalculateDuNoForSingleBranch(code, null, date);
                        totalDuNo += branchValue;
                    }

                    _logger.LogInformation("Tổng Dư nợ CN Lai Châu: {Total}", totalDuNo);
                    return totalDuNo;
                }
                else
                {
                    var branchCode = GetBranchCode(branchId);
                    var trctCode = GetTrctCode(branchId);

                    return await CalculateDuNoForSingleBranch(branchCode, trctCode, date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Dư nợ cho chi nhánh {BranchId}", branchId);
                return 0m;
            }
        }

        private async Task<decimal> CalculateDuNoForSingleBranch(string branchCode, string? trctCode, DateTime? date)
        {
            // Mock data tạm thời cho demo
            await Task.Delay(100); // Simulate async operation

            var mockData = branchCode switch
            {
                "7800" => trctCode switch
                {
                    "01" => 340.5m * 1000000000, // 340.5 tỷ
                    "02" => 298.2m * 1000000000, // 298.2 tỷ
                    _ => 825.7m * 1000000000    // 825.7 tỷ
                },
                "7801" => 589.3m * 1000000000,   // CN Tam Đường
                "7802" => 476.8m * 1000000000,   // CN Phong Thổ
                "7803" => 345.2m * 1000000000,   // CN Sin Hồ
                "7804" => 234.6m * 1000000000,   // CN Mường Tè
                "7805" => 567.4m * 1000000000,   // CN Than Uyên
                "7806" => 698.5m * 1000000000,   // CN Thành Phố
                "7807" => 455.9m * 1000000000,   // CN Tân Uyên
                "7808" => 342.1m * 1000000000,   // CN Nậm Nhùn
                _ => 400.0m * 1000000000         // Default
            };

            _logger.LogInformation("Mock Dư nợ chi nhánh {BranchCode} TRCT {TrctCode}: {Total}",
                branchCode, trctCode ?? "ALL", mockData);

            return mockData;
        }

        /// <summary>
        /// 3. Nợ xấu - từ bảng LN01 (Mock data)
        /// </summary>
        public async Task<decimal> CalculateNoXauByBranch(string branchId, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Nợ xấu cho chi nhánh {BranchId}", branchId);

                if (branchId == "CnLaiChau")
                {
                    // CN Lai Châu = tỷ lệ toàn tỉnh
                    var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
                    var totalDuNo = 0m;
                    var totalDuNoXau = 0m;

                    foreach (var code in branchCodes)
                    {
                        var duNo = await CalculateDuNoForSingleBranch(code, null, date);
                        var noXauRate = await CalculateNoXauForSingleBranch(code, null, date);

                        totalDuNo += duNo;
                        totalDuNoXau += duNo * (noXauRate / 100);
                    }

                    var tyLe = totalDuNo > 0 ? (totalDuNoXau / totalDuNo) * 100 : 0;
                    _logger.LogInformation("Tỷ lệ nợ xấu CN Lai Châu: {Rate}%", tyLe);
                    return tyLe;
                }
                else
                {
                    var branchCode = GetBranchCode(branchId);
                    var trctCode = GetTrctCode(branchId);

                    return await CalculateNoXauForSingleBranch(branchCode, trctCode, date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nợ xấu cho chi nhánh {BranchId}", branchId);
                return 0m;
            }
        }

        private async Task<decimal> CalculateNoXauForSingleBranch(string branchCode, string? trctCode, DateTime? date)
        {
            // Mock data tạm thời cho demo
            await Task.Delay(100); // Simulate async operation

            var mockData = branchCode switch
            {
                "7800" => trctCode switch
                {
                    "01" => 1.25m, // 1.25%
                    "02" => 1.45m, // 1.45%
                    _ => 1.15m     // 1.15%
                },
                "7801" => 0.89m,   // CN Tam Đường
                "7802" => 1.76m,   // CN Phong Thổ
                "7803" => 2.45m,   // CN Sin Hồ
                "7804" => 1.34m,   // CN Mường Tè
                "7805" => 1.67m,   // CN Than Uyên
                "7806" => 0.98m,   // CN Thành Phố
                "7807" => 1.55m,   // CN Tân Uyên
                "7808" => 2.42m,   // CN Nậm Nhùn
                _ => 1.50m         // Default
            };

            _logger.LogInformation("Mock Tỷ lệ nợ xấu chi nhánh {BranchCode} TRCT {TrctCode}: {Rate}%",
                branchCode, trctCode ?? "ALL", mockData);

            return mockData;
        }

        // Các chỉ tiêu khác sẽ được implement tương tự
        public async Task<decimal> CalculateThuHoiXLRRByBranch(string branchId, DateTime? date = null)
        {
            // Mock data
            await Task.Delay(50);
            var mockData = branchId switch
            {
                "CnLaiChau" => 156.7m * 1000000000, // Tổng toàn tỉnh
                "HoiSo" => 45.2m * 1000000000,
                "CnTamDuong" => 23.5m * 1000000000,
                "CnPhongTho" => 18.9m * 1000000000,
                _ => 15.0m * 1000000000
            };
            return mockData;
        }

        public async Task<decimal> CalculateThuDichVuByBranch(string branchId, DateTime? date = null)
        {
            // Mock data
            await Task.Delay(50);
            var mockData = branchId switch
            {
                "CnLaiChau" => 89.4m * 1000000000, // Tổng toàn tỉnh
                "HoiSo" => 28.7m * 1000000000,
                "CnTamDuong" => 12.3m * 1000000000,
                "CnPhongTho" => 9.8m * 1000000000,
                _ => 8.0m * 1000000000
            };
            return mockData;
        }

        public async Task<decimal> CalculateLoiNhuanByBranch(string branchId, DateTime? date = null)
        {
            // Mock data
            await Task.Delay(50);
            var mockData = branchId switch
            {
                "CnLaiChau" => 234.6m * 1000000000, // Tổng toàn tỉnh
                "HoiSo" => 67.8m * 1000000000,
                "CnTamDuong" => 34.2m * 1000000000,
                "CnPhongTho" => 28.9m * 1000000000,
                _ => 20.0m * 1000000000
            };
            return mockData;
        }

        /// <summary>
        /// Mapping BranchId thành mã chi nhánh
        /// </summary>
        private string GetBranchCode(string branchId)
        {
            return branchId switch
            {
                "HoiSo" => "7800",
                "CnTamDuong" => "7801",
                "CnPhongTho" => "7802",
                "CnSinHo" => "7803",
                "CnMuongTe" => "7804",
                "CnThanUyen" => "7805",
                "CnThanhPho" => "7806",
                "CnTanUyen" => "7807",
                "CnNamNhun" => "7808",
                "CnPhongThoPgdMuongSo" => "7802",
                "CnThanUyenPgdMuongThan" => "7805",
                "CnThanhPhoPgdSo1" => "7806",
                "CnThanhPhoPgdSo2" => "7806",
                "CnTanUyenPgdSo3" => "7807",
                _ => "7800" // Default Hội Sở
            };
        }

        /// <summary>
        /// Mapping BranchId thành mã PGD cho DP01
        /// </summary>
        private string? GetPgdCode(string branchId)
        {
            return branchId switch
            {
                "CnPhongThoPgdMuongSo" => "01",
                "CnThanUyenPgdMuongThan" => "01",
                "CnThanhPhoPgdSo1" => "01",
                "CnThanhPhoPgdSo2" => "02",
                "CnTanUyenPgdSo3" => "01",
                _ => null // Chi nhánh chính, không có PGD
            };
        }

        /// <summary>
        /// Mapping BranchId thành mã TRCTCD cho LN01
        /// </summary>
        private string? GetTrctCode(string branchId)
        {
            return branchId switch
            {
                "CnPhongThoPgdMuongSo" => "01",
                "CnThanUyenPgdMuongThan" => "01",
                "CnThanhPhoPgdSo1" => "01",
                "CnThanhPhoPgdSo2" => "02",
                "CnTanUyenPgdSo3" => "01",
                _ => null // Chi nhánh chính, không có TRCTCD
            };
        }

        /// <summary>
        /// Lấy dữ liệu DP01 từ database theo chi nhánh và PGD
        /// </summary>
        private async Task<List<dynamic>> GetDP01DataForBranch(string branchCode, string? pgdCode)
        {
            try
            {
                _logger.LogInformation("🔍 Tìm dữ liệu DP01 cho chi nhánh {BranchCode}, PGD {PgdCode}", branchCode, pgdCode ?? "NULL");

                // Tìm trong ImportedDataRecords có category DP01
                var dp01Records = await _context.ImportedDataRecords
                    .Where(x => x.Category == "DP01")
                    .ToListAsync();

                _logger.LogInformation("📄 Tìm thấy {Count} records DP01 trong database", dp01Records.Count);

                if (!dp01Records.Any())
                {
                    _logger.LogWarning("❌ Không tìm thấy records DP01 nào");
                    return new List<dynamic>();
                }

                var allItems = new List<dynamic>();
                int processedRecords = 0;
                int totalItems = 0;
                int matchedItems = 0;

                foreach (var record in dp01Records)
                {
                    processedRecords++;
                    _logger.LogInformation("🔧 Xử lý record {Index}/{Total}: {FileName}", processedRecords, dp01Records.Count, record.FileName);

                    var items = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == record.Id)
                        .Select(x => x.RawData)
                        .ToListAsync();

                    totalItems += items.Count;
                    _logger.LogInformation("📊 Record {FileName} có {ItemCount} items", record.FileName, items.Count);

                    foreach (var rawData in items)
                    {
                        try
                        {
                            var jsonDoc = JsonDocument.Parse(rawData);
                            var root = jsonDoc.RootElement;

                            // Kiểm tra MA_CN và MA_PGD
                            var maCn = root.TryGetProperty("MA_CN", out var maCnProp) ? maCnProp.GetString() : "";
                            var maPgd = root.TryGetProperty("MA_PGD", out var maPgdProp) ? maPgdProp.GetString() : "";
                            var taiKhoanHachToan = root.TryGetProperty("TAI_KHOAN_HACH_TOAN", out var tkProp) ? tkProp.GetString() : "";

                            // Parse CURRENT_BALANCE - có thể là string hoặc number
                            decimal currentBalance = 0;
                            if (root.TryGetProperty("CURRENT_BALANCE", out var balanceProp))
                            {
                                if (balanceProp.ValueKind == JsonValueKind.Number)
                                {
                                    currentBalance = balanceProp.GetDecimal();
                                }
                                else if (balanceProp.ValueKind == JsonValueKind.String)
                                {
                                    decimal.TryParse(balanceProp.GetString(), out currentBalance);
                                }
                            }

                            // Lọc theo chi nhánh và PGD - xử lý MA_PGD có thể có dấu nháy đơn
                            var cleanPgd = maPgd?.Trim('\'', '"').Trim();  // Loại bỏ dấu nháy đơn/kép
                            bool pgdMatch = (pgdCode == null && (cleanPgd == "00" || string.IsNullOrEmpty(cleanPgd))) ||
                                          (pgdCode != null && cleanPgd == pgdCode);

                            if (maCn == branchCode && pgdMatch && !string.IsNullOrEmpty(taiKhoanHachToan))
                            {
                                matchedItems++;
                                allItems.Add(new
                                {
                                    MA_CN = maCn,
                                    MA_PGD = maPgd,
                                    TAI_KHOAN_HACH_TOAN = taiKhoanHachToan,
                                    CURRENT_BALANCE = currentBalance
                                });

                                // Log 3 items đầu để debug
                                if (matchedItems <= 3)
                                {
                                    _logger.LogInformation("✅ Match #{Index}: MA_CN={MaCn}, MA_PGD={MaPgd}, TK={TK}, Balance={Balance}",
                                        matchedItems, maCn, maPgd, taiKhoanHachToan, currentBalance);
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning("❌ JSON parse error: {Error}", ex.Message);
                        }
                    }
                }

                _logger.LogInformation("📈 Kết quả: Tổng {TotalItems} items, Matched {MatchedItems} items cho {BranchCode}",
                    totalItems, matchedItems, branchCode);

                return allItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi khi lấy dữ liệu DP01 cho chi nhánh {BranchCode}", branchCode);
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Tính toán Nguồn vốn từ dữ liệu DP01 với điều kiện loại trừ
        /// Loại trừ: TK có 2 chữ số đầu là "40", "41", có 3 chữ số đầu là "427", và TK "211108"
        /// </summary>
        private decimal CalculateNguonVonFromDP01(List<dynamic> dp01Data, string branchCode, string? pgdCode)
        {
            decimal totalNguonVon = 0;
            int excludedCount = 0;
            int includedCount = 0;

            foreach (var item in dp01Data)
            {
                var taiKhoan = Convert.ToString(item.TAI_KHOAN_HACH_TOAN) ?? "";
                var balance = Convert.ToDecimal(item.CURRENT_BALANCE);

                // Kiểm tra tài khoản loại trừ: 40*, 41*, 427*, và 211108
                bool isExcluded = taiKhoan.StartsWith("40") ||
                                  taiKhoan.StartsWith("41") ||
                                  taiKhoan.StartsWith("427") ||
                                  taiKhoan == "211108";

                if (isExcluded)
                {
                    excludedCount++;
                }
                else
                {
                    includedCount++;
                    totalNguonVon += balance;
                }
            }

            _logger.LogInformation("Tính Nguồn vốn từ DP01 - Chi nhánh {BranchCode} PGD {PgdCode}: " +
                "Tổng {Total}, Bao gồm {IncludedCount} TK, Loại trừ {ExcludedCount} TK",
                branchCode, pgdCode ?? "ALL", totalNguonVon, includedCount, excludedCount);

            return totalNguonVon;
        }

    }
}
