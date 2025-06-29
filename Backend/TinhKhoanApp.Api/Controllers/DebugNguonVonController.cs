using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Services;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugNguonVonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DebugNguonVonController> _logger;
        private readonly IBranchCalculationService _branchCalculationService;

        public DebugNguonVonController(
            ApplicationDbContext context,
            ILogger<DebugNguonVonController> logger,
            IBranchCalculationService branchCalculationService)
        {
            _context = context;
            _logger = logger;
            _branchCalculationService = branchCalculationService;
        }

        /// <summary>
        /// So sánh 2 cách tính Nguồn vốn: BranchCalculationService vs Direct calculation
        /// </summary>
        [HttpGet("compare/{branchId}")]
        public async Task<ActionResult> CompareNguonVonCalculations(string branchId)
        {
            try
            {
                _logger.LogInformation("🔍 Bắt đầu so sánh tính toán Nguồn vốn cho {BranchId}", branchId);

                // Cách 1: Qua BranchCalculationService
                var fromService = await _branchCalculationService.CalculateNguonVonByBranch(branchId);
                var fromServiceTy = Math.Round(fromService / 1_000_000_000m, 2);

                // Cách 2: Tính trực tiếp như DebugDP01Controller cũ
                var directCalculation = await CalculateNguonVonDirect(branchId);
                var directCalculationTy = Math.Round(directCalculation / 1_000_000_000m, 2);

                var result = new
                {
                    branchId = branchId,
                    fromBranchCalculationService = new
                    {
                        valueVnd = fromService,
                        valueTy = fromServiceTy,
                        source = "BranchCalculationService"
                    },
                    fromDirectCalculation = new
                    {
                        valueVnd = directCalculation,
                        valueTy = directCalculationTy,
                        source = "Direct DP01 calculation"
                    },
                    difference = new
                    {
                        vnd = Math.Abs(fromService - directCalculation),
                        ty = Math.Abs(fromServiceTy - directCalculationTy),
                        percentDiff = directCalculation > 0 ? Math.Round(Math.Abs(fromService - directCalculation) / directCalculation * 100, 4) : 0
                    },
                    isMatch = Math.Abs(fromServiceTy - directCalculationTy) < 0.01m,
                    timestamp = DateTime.Now
                };

                _logger.LogInformation("📊 So sánh kết quả: Service={ServiceTy} tỷ, Direct={DirectTy} tỷ, Diff={DiffTy} tỷ",
                    fromServiceTy, directCalculationTy, Math.Abs(fromServiceTy - directCalculationTy));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi so sánh tính toán Nguồn vốn cho {BranchId}", branchId);
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        /// <summary>
        /// Tính toán trực tiếp từ DP01 như logic cũ
        /// </summary>
        private async Task<decimal> CalculateNguonVonDirect(string branchId)
        {
            var branchCode = GetBranchCode(branchId);
            var pgdCode = GetPgdCode(branchId);

            _logger.LogInformation("🔍 Tính toán trực tiếp cho BranchCode={BranchCode}, PgdCode={PgdCode}", branchCode, pgdCode);

            if (branchId == "CnLaiChau")
            {
                // CN Lai Châu = tổng của 9 CN
                var branchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
                var total = 0m;

                foreach (var code in branchCodes)
                {
                    var branchValue = await CalculateNguonVonDirectForSingleBranch(code, null);
                    total += branchValue;
                    _logger.LogInformation("  Chi nhánh {Code}: {Value:F2} tỷ", code, branchValue / 1_000_000_000);
                }

                return total;
            }
            else
            {
                return await CalculateNguonVonDirectForSingleBranch(branchCode, pgdCode);
            }
        }

        private async Task<decimal> CalculateNguonVonDirectForSingleBranch(string branchCode, string? pgdCode)
        {
            try
            {
                // Lấy dữ liệu DP01 trực tiếp từ ImportedDataItems
                var dp01Items = await _context.ImportedDataRecords
                    .Where(x => x.Category == "DP01")
                    .SelectMany(x => x.ImportedDataItems)
                    .ToListAsync();

                _logger.LogInformation("📄 Tìm thấy {Count} items DP01", dp01Items.Count);

                decimal totalNguonVon = 0;
                int totalItems = dp01Items.Count;
                int matchedItems = 0;
                int excludedItems = 0;
                int includedItems = 0;

                foreach (var item in dp01Items)
                {
                    try
                    {
                        // Parse dữ liệu JSON từ item
                        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.RawData);

                        // Lấy các trường cần thiết
                        var maCn = data.TryGetValue("MA_CN", out var maCnElement) ?
                            (maCnElement.ValueKind == JsonValueKind.String ? maCnElement.GetString() : maCnElement.ToString()) : "";

                        var maPgd = data.TryGetValue("MA_PGD", out var maPgdElement) ?
                            (maPgdElement.ValueKind == JsonValueKind.String ? maPgdElement.GetString() : maPgdElement.ToString()) : "";

                        var taiKhoanHachToan = data.TryGetValue("TAI_KHOAN_HACH_TOAN", out var tkElement) ?
                            (tkElement.ValueKind == JsonValueKind.String ? tkElement.GetString() : tkElement.ToString()) : "";

                        // Parse CURRENT_BALANCE - xử lý cả string và number
                        decimal currentBalance = 0;
                        if (data.TryGetValue("CURRENT_BALANCE", out var balanceElement))
                        {
                            if (balanceElement.ValueKind == JsonValueKind.String)
                            {
                                decimal.TryParse(balanceElement.GetString(), out currentBalance);
                            }
                            else if (balanceElement.ValueKind == JsonValueKind.Number)
                            {
                                currentBalance = balanceElement.GetDecimal();
                            }
                        }

                        // Kiểm tra điều kiện lọc chi nhánh
                        bool branchMatch = maCn == branchCode;
                        bool pgdMatch = string.IsNullOrEmpty(pgdCode) || maPgd == pgdCode;

                        if (branchMatch && pgdMatch)
                        {
                            matchedItems++;

                            // Kiểm tra tài khoản loại trừ
                            bool isExcluded = taiKhoanHachToan.StartsWith("40") ||
                                              taiKhoanHachToan.StartsWith("41") ||
                                              taiKhoanHachToan.StartsWith("427") ||
                                              taiKhoanHachToan == "211108";

                            if (isExcluded)
                            {
                                excludedItems++;
                            }
                            else
                            {
                                includedItems++;
                                totalNguonVon += currentBalance;
                            }

                            // Log chi tiết 3 items đầu
                            if (matchedItems <= 3)
                            {
                                _logger.LogInformation("  Item #{Index}: MA_CN={MaCn}, MA_PGD={MaPgd}, TK={TK}, Balance={Balance}, Excluded={Excluded}",
                                    matchedItems, maCn, maPgd, taiKhoanHachToan, currentBalance, isExcluded);
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning("❌ JSON parse error: {Error}", ex.Message);
                    }
                }

                _logger.LogInformation("📈 Kết quả trực tiếp: CN={BranchCode}, PGD={PgdCode}, Total={TotalItems}, Matched={MatchedItems}, Included={IncludedItems}, Excluded={ExcludedItems}, Sum={Sum:F2} tỷ",
                    branchCode, pgdCode ?? "NULL", totalItems, matchedItems, includedItems, excludedItems, totalNguonVon / 1_000_000_000);

                return totalNguonVon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Lỗi tính toán trực tiếp cho {BranchCode}", branchCode);
                return 0m;
            }
        }

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
    }
}
