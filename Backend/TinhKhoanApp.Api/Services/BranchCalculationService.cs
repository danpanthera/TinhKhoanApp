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
                // Lấy dữ liệu thực từ bảng DP01 theo ngày cụ thể
                var dp01Data = await GetDP01DataForBranch(branchCode, pgdCode, date);

                if (dp01Data.Any())
                {
                    var totalNguonVon = CalculateNguonVonFromDP01(dp01Data, branchCode, pgdCode);
                    _logger.LogInformation("Nguồn vốn thực tế từ DP01 ngày {Date} - Chi nhánh {BranchCode} PGD {PgdCode}: {Total}",
                        date?.ToString("dd/MM/yyyy") ?? "latest", branchCode, pgdCode ?? "ALL", totalNguonVon);
                    return totalNguonVon;
                }
                else
                {
                    // Nếu không có dữ liệu thực cho ngày đó
                    var dateStr = date?.ToString("dd/MM/yyyy") ?? "latest";
                    _logger.LogWarning("❌ Không tìm thấy dữ liệu DP01 cho ngày {Date}, chi nhánh {BranchCode}", dateStr, branchCode);
                    throw new InvalidOperationException($"Không có dữ liệu nguồn vốn cho ngày {dateStr}, chi nhánh {branchCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nguồn vốn cho chi nhánh {BranchCode} ngày {Date}", branchCode, date?.ToString("dd/MM/yyyy"));
                throw;
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
            try
            {
                // TODO: Implement real LN01 data calculation when available
                // For now, using improved mock data that's proportional to branch size

                await Task.Delay(50); // Simulate async operation

                // Base amounts for different branch sizes (in VND)
                var mockData = branchCode switch
                {
                    "7800" => trctCode switch
                    {
                        "01" => 45.2m * 1_000_000_000,  // PGD 1: ~45.2 tỷ
                        "02" => 38.7m * 1_000_000_000,  // PGD 2: ~38.7 tỷ
                        _ => 187.6m * 1_000_000_000     // Hội Sở total: ~187.6 tỷ
                    },
                    "7801" => 156.3m * 1_000_000_000,   // CN Tam Đường: ~156.3 tỷ
                    "7802" => 134.8m * 1_000_000_000,   // CN Phong Thổ: ~134.8 tỷ
                    "7803" => 98.2m * 1_000_000_000,    // CN Sin Hồ: ~98.2 tỷ
                    "7804" => 76.4m * 1_000_000_000,    // CN Mường Tè: ~76.4 tỷ
                    "7805" => 143.7m * 1_000_000_000,   // CN Than Uyên: ~143.7 tỷ
                    "7806" => 165.5m * 1_000_000_000,   // CN Thành Phố: ~165.5 tỷ
                    "7807" => 124.9m * 1_000_000_000,   // CN Tân Uyên: ~124.9 tỷ
                    "7808" => 89.1m * 1_000_000_000,    // CN Nậm Nhùn: ~89.1 tỷ
                    _ => 120.0m * 1_000_000_000         // Default: ~120 tỷ
                };

                _logger.LogInformation("💳 Dư nợ tín dụng (mock) - Chi nhánh {BranchCode} TRCT {TrctCode}: {Total:F2} tỷ",
                    branchCode, trctCode ?? "ALL", mockData / 1_000_000_000);

                return mockData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Dư nợ cho chi nhánh {BranchCode}", branchCode);
                return 0m;
            }
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
            try
            {
                // TODO: Implement real LN01 data calculation when available
                // NPL rate should be realistic (typically 0.5% - 3% for different branches)

                await Task.Delay(50); // Simulate async operation

                var mockNplRate = branchCode switch
                {
                    "7800" => trctCode switch
                    {
                        "01" => 1.15m,  // PGD 1: 1.15%
                        "02" => 1.35m,  // PGD 2: 1.35%
                        _ => 0.85m      // Hội Sở: 0.85%
                    },
                    "7801" => 0.76m,    // CN Tam Đường: 0.76%
                    "7802" => 1.42m,    // CN Phong Thổ: 1.42%
                    "7803" => 2.18m,    // CN Sin Hồ: 2.18%
                    "7804" => 1.67m,    // CN Mường Tè: 1.67%
                    "7805" => 1.23m,    // CN Than Uyên: 1.23%
                    "7806" => 0.94m,    // CN Thành Phố: 0.94%
                    "7807" => 1.56m,    // CN Tân Uyên: 1.56%
                    "7808" => 2.05m,    // CN Nậm Nhùn: 2.05%
                    _ => 1.50m          // Default: 1.50%
                };

                _logger.LogInformation("⚠️ Tỷ lệ nợ xấu (mock) - Chi nhánh {BranchCode} TRCT {TrctCode}: {Rate:F2}%",
                    branchCode, trctCode ?? "ALL", mockNplRate);

                return mockNplRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nợ xấu cho chi nhánh {BranchCode}", branchCode);
                return 1.50m; // Default NPL rate
            }
        }

        // Các chỉ tiêu khác sẽ được implement tương tự
        public async Task<decimal> CalculateThuHoiXLRRByBranch(string branchId, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Thu hồi XLRR cho chi nhánh {BranchId}", branchId);

                if (branchId == "CnLaiChau")
                {
                    // CN Lai Châu = tổng toàn tỉnh
                    var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
                    var totalThuHoi = 0m;

                    foreach (var code in branchCodes)
                    {
                        var branchValue = await CalculateThuHoiXLRRForSingleBranch(code, null, date);
                        totalThuHoi += branchValue;
                    }

                    _logger.LogInformation("Tổng Thu hồi XLRR CN Lai Châu: {Total:F2} tỷ", totalThuHoi / 1_000_000_000);
                    return totalThuHoi;
                }
                else
                {
                    var branchCode = GetBranchCode(branchId);
                    var trctCode = GetTrctCode(branchId);
                    return await CalculateThuHoiXLRRForSingleBranch(branchCode, trctCode, date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Thu hồi XLRR cho chi nhánh {BranchId}", branchId);
                return 0m;
            }
        }

        private async Task<decimal> CalculateThuHoiXLRRForSingleBranch(string branchCode, string? trctCode, DateTime? date)
        {
            try
            {
                // TODO: Implement real XLRR recovery data calculation when available
                await Task.Delay(50);

                var mockData = branchCode switch
                {
                    "7800" => trctCode switch
                    {
                        "01" => 3.2m * 1_000_000_000,   // PGD 1: ~3.2 tỷ
                        "02" => 2.8m * 1_000_000_000,   // PGD 2: ~2.8 tỷ
                        _ => 12.4m * 1_000_000_000      // Hội Sở total: ~12.4 tỷ
                    },
                    "7801" => 8.7m * 1_000_000_000,     // CN Tam Đường: ~8.7 tỷ
                    "7802" => 6.9m * 1_000_000_000,     // CN Phong Thổ: ~6.9 tỷ
                    "7803" => 4.8m * 1_000_000_000,     // CN Sin Hồ: ~4.8 tỷ
                    "7804" => 3.6m * 1_000_000_000,     // CN Mường Tè: ~3.6 tỷ
                    "7805" => 7.2m * 1_000_000_000,     // CN Than Uyên: ~7.2 tỷ
                    "7806" => 9.8m * 1_000_000_000,     // CN Thành Phố: ~9.8 tỷ
                    "7807" => 6.3m * 1_000_000_000,     // CN Tân Uyên: ~6.3 tỷ
                    "7808" => 4.1m * 1_000_000_000,     // CN Nậm Nhùn: ~4.1 tỷ
                    _ => 5.0m * 1_000_000_000           // Default: ~5 tỷ
                };

                _logger.LogInformation("🏦 Thu hồi XLRR (mock) - Chi nhánh {BranchCode} TRCT {TrctCode}: {Total:F2} tỷ",
                    branchCode, trctCode ?? "ALL", mockData / 1_000_000_000);

                return mockData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Thu hồi XLRR cho chi nhánh {BranchCode}", branchCode);
                return 0m;
            }
        }

        public async Task<decimal> CalculateThuDichVuByBranch(string branchId, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Thu dịch vụ cho chi nhánh {BranchId}", branchId);

                if (branchId == "CnLaiChau")
                {
                    // CN Lai Châu = tổng toàn tỉnh
                    var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
                    var totalThuDichVu = 0m;

                    foreach (var code in branchCodes)
                    {
                        var branchValue = await CalculateThuDichVuForSingleBranch(code, null, date);
                        totalThuDichVu += branchValue;
                    }

                    _logger.LogInformation("Tổng Thu dịch vụ CN Lai Châu: {Total:F2} tỷ", totalThuDichVu / 1_000_000_000);
                    return totalThuDichVu;
                }
                else
                {
                    var branchCode = GetBranchCode(branchId);
                    var trctCode = GetTrctCode(branchId);
                    return await CalculateThuDichVuForSingleBranch(branchCode, trctCode, date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Thu dịch vụ cho chi nhánh {BranchId}", branchId);
                return 0m;
            }
        }

        private async Task<decimal> CalculateThuDichVuForSingleBranch(string branchCode, string? trctCode, DateTime? date)
        {
            try
            {
                // TODO: Implement real service income data calculation when available
                await Task.Delay(50);

                var mockData = branchCode switch
                {
                    "7800" => trctCode switch
                    {
                        "01" => 8.5m * 1_000_000_000,   // PGD 1: ~8.5 tỷ
                        "02" => 7.2m * 1_000_000_000,   // PGD 2: ~7.2 tỷ
                        _ => 28.9m * 1_000_000_000      // Hội Sở total: ~28.9 tỷ
                    },
                    "7801" => 22.6m * 1_000_000_000,    // CN Tam Đường: ~22.6 tỷ
                    "7802" => 18.3m * 1_000_000_000,    // CN Phong Thổ: ~18.3 tỷ
                    "7803" => 14.2m * 1_000_000_000,    // CN Sin Hồ: ~14.2 tỷ
                    "7804" => 11.5m * 1_000_000_000,    // CN Mường Tè: ~11.5 tỷ
                    "7805" => 20.4m * 1_000_000_000,    // CN Than Uyên: ~20.4 tỷ
                    "7806" => 26.7m * 1_000_000_000,    // CN Thành Phố: ~26.7 tỷ
                    "7807" => 19.8m * 1_000_000_000,    // CN Tân Uyên: ~19.8 tỷ
                    "7808" => 13.1m * 1_000_000_000,    // CN Nậm Nhùn: ~13.1 tỷ
                    _ => 15.0m * 1_000_000_000          // Default: ~15 tỷ
                };

                _logger.LogInformation("🏦 Thu dịch vụ (mock) - Chi nhánh {BranchCode} TRCT {TrctCode}: {Total:F2} tỷ",
                    branchCode, trctCode ?? "ALL", mockData / 1_000_000_000);

                return mockData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Thu dịch vụ cho chi nhánh {BranchCode}", branchCode);
                return 0m;
            }
        }

        public async Task<decimal> CalculateLoiNhuanByBranch(string branchId, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Lợi nhuận cho chi nhánh {BranchId}", branchId);

                if (branchId == "CnLaiChau")
                {
                    // CN Lai Châu = tổng toàn tỉnh
                    var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
                    var totalLoiNhuan = 0m;

                    foreach (var code in branchCodes)
                    {
                        var branchValue = await CalculateLoiNhuanForSingleBranch(code, null, date);
                        totalLoiNhuan += branchValue;
                    }

                    _logger.LogInformation("Tổng Lợi nhuận CN Lai Châu: {Total:F2} tỷ", totalLoiNhuan / 1_000_000_000);
                    return totalLoiNhuan;
                }
                else
                {
                    var branchCode = GetBranchCode(branchId);
                    var trctCode = GetTrctCode(branchId);
                    return await CalculateLoiNhuanForSingleBranch(branchCode, trctCode, date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Lợi nhuận cho chi nhánh {BranchId}", branchId);
                return 0m;
            }
        }

        private async Task<decimal> CalculateLoiNhuanForSingleBranch(string branchCode, string? trctCode, DateTime? date)
        {
            try
            {
                // TODO: Implement real profit calculation when available
                await Task.Delay(50);

                var mockData = branchCode switch
                {
                    "7800" => trctCode switch
                    {
                        "01" => 45.8m * 1_000_000_000,  // PGD 1: ~45.8 tỷ
                        "02" => 38.2m * 1_000_000_000,  // PGD 2: ~38.2 tỷ
                        _ => 156.4m * 1_000_000_000     // Hội Sở total: ~156.4 tỷ
                    },
                    "7801" => 124.7m * 1_000_000_000,   // CN Tam Đường: ~124.7 tỷ
                    "7802" => 108.2m * 1_000_000_000,   // CN Phong Thổ: ~108.2 tỷ
                    "7803" => 89.6m * 1_000_000_000,    // CN Sin Hồ: ~89.6 tỷ
                    "7804" => 67.8m * 1_000_000_000,    // CN Mường Tè: ~67.8 tỷ
                    "7805" => 118.9m * 1_000_000_000,   // CN Than Uyên: ~118.9 tỷ
                    "7806" => 142.3m * 1_000_000_000,   // CN Thành Phố: ~142.3 tỷ
                    "7807" => 105.6m * 1_000_000_000,   // CN Tân Uyên: ~105.6 tỷ
                    "7808" => 78.4m * 1_000_000_000,    // CN Nậm Nhùn: ~78.4 tỷ
                    _ => 90.0m * 1_000_000_000          // Default: ~90 tỷ
                };

                _logger.LogInformation("💵 Lợi nhuận (mock) - Chi nhánh {BranchCode} TRCT {TrctCode}: {Total:F2} tỷ",
                    branchCode, trctCode ?? "ALL", mockData / 1_000_000_000);

                return mockData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Lợi nhuận cho chi nhánh {BranchCode}", branchCode);
                return 0m;
            }
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
        private async Task<List<dynamic>> GetDP01DataForBranch(string branchCode, string? pgdCode, DateTime? date = null)
        {
            try
            {
                _logger.LogInformation("🔍 Tìm dữ liệu DP01 cho chi nhánh {BranchCode}, PGD {PgdCode}, ngày {Date}",
                    branchCode, pgdCode ?? "NULL", date?.ToString("dd/MM/yyyy") ?? "latest");

                // Xây dựng query cho ImportedDataRecords có category DP01
                var query = _context.ImportedDataRecords.Where(x => x.Category == "DP01");

                // Lọc theo ngày nếu có tham số date
                if (date.HasValue)
                {
                    // Tìm dữ liệu cho ngày cụ thể
                    query = query.Where(x => x.StatementDate.HasValue && x.StatementDate.Value.Date == date.Value.Date);
                }
                else
                {
                    // Nếu không có tham số date, lấy ngày gần nhất
                    var latestDate = await _context.ImportedDataRecords
                        .Where(x => x.Category == "DP01" && x.StatementDate.HasValue)
                        .MaxAsync(x => x.StatementDate);

                    if (latestDate.HasValue)
                    {
                        query = query.Where(x => x.StatementDate.HasValue && x.StatementDate.Value.Date == latestDate.Value.Date);
                        _logger.LogInformation("📅 Sử dụng ngày gần nhất: {LatestDate}", latestDate.Value.ToString("dd/MM/yyyy"));
                    }
                }

                var dp01Records = await query.ToListAsync();

                _logger.LogInformation("📄 Tìm thấy {Count} records DP01 trong database cho điều kiện lọc", dp01Records.Count);

                if (!dp01Records.Any())
                {
                    var dateStr = date?.ToString("dd/MM/yyyy") ?? "ngày gần nhất";
                    _logger.LogWarning("❌ Không tìm thấy records DP01 nào cho {DateStr}", dateStr);
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

                            // Lọc theo chi nhánh và PGD - sử dụng logic đơn giản hơn để khớp với direct calculation
                            bool pgdMatch = string.IsNullOrEmpty(pgdCode) || maPgd == pgdCode;

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
