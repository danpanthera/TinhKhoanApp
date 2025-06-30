using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Utils;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Request model cho debug date filter
    /// </summary>
    public class DebugDateFilterRequest
    {
        public string BranchId { get; set; } = string.Empty;
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
    }

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

        /// <summary>
        /// Debug dữ liệu DP01 cho ngày 31/12/2024, mã chi nhánh 7800 (Hội Sở)
        /// </summary>
        [HttpGet("debug-dp01-2024")]
        public async Task<ActionResult> DebugDP01Data2024()
        {
            try
            {
                var targetDate = new DateTime(2024, 12, 31);
                var branchCode = "7800"; // Hội Sở

                _logger.LogInformation("🔍 Debug DP01 data cho ngày {Date}, mã chi nhánh {BranchCode}",
                    targetDate.ToString("yyyy-MM-dd"), branchCode);

                // Tìm file DP01 cho ngày 31/12/2024
                var importRecords = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date == targetDate.Date &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("DP01") || r.Category.Contains("Nguồn vốn")))
                    .OrderByDescending(r => r.ImportDate)
                    .ToListAsync();

                if (!importRecords.Any())
                {
                    return Ok(new
                    {
                        message = $"Không tìm thấy file DP01 cho ngày {targetDate:yyyy-MM-dd}",
                        availableDates = await _context.ImportedDataRecords
                            .Where(r => r.Category.Contains("DP01"))
                            .Select(r => r.StatementDate)
                            .Distinct()
                            .OrderByDescending(d => d)
                            .Take(10)
                            .ToListAsync()
                    });
                }

                var latestRecord = importRecords.First();

                // Lấy dữ liệu chi tiết
                var importedItems = await _context.ImportedDataItems
                    .Where(i => i.ImportedDataRecordId == latestRecord.Id)
                    .Select(i => i.RawData)
                    .ToListAsync();

                decimal totalNguonVon = 0;
                var excludedPrefixes = new[] { "2", "40", "41", "427" };
                var accountDetails = new List<object>();
                var processedRecords = 0;

                foreach (var rawDataJson in importedItems)
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(rawDataJson);
                        var root = jsonDoc.RootElement;

                        if (root.TryGetProperty("ACCOUNT_CODE", out var accountCodeElement) ||
                            root.TryGetProperty("AccountCode", out accountCodeElement) ||
                            root.TryGetProperty("account_code", out accountCodeElement))
                        {
                            var accountCode = accountCodeElement.GetString() ?? "";

                            // Kiểm tra chi nhánh
                            var branchField = "";
                            if (root.TryGetProperty("BRANCH_CODE", out var branchElement))
                                branchField = branchElement.GetString() ?? "";

                            var belongsToBranch = branchField == branchCode;

                            if (root.TryGetProperty("CURRENT_BALANCE", out var balanceElement) ||
                                root.TryGetProperty("CurrentBalance", out balanceElement) ||
                                root.TryGetProperty("current_balance", out balanceElement))
                            {
                                if (decimal.TryParse(balanceElement.GetString(), out var balance))
                                {
                                    var isExcluded = excludedPrefixes.Any(prefix => accountCode.StartsWith(prefix));

                                    accountDetails.Add(new
                                    {
                                        accountCode,
                                        branchCode = branchField,
                                        balance,
                                        balanceTrieuVnd = Math.Round(balance / 1_000_000m, 2),
                                        belongsToBranch,
                                        isExcluded,
                                        willBeIncluded = belongsToBranch && !isExcluded
                                    });

                                    if (belongsToBranch && !isExcluded)
                                    {
                                        totalNguonVon += balance;
                                        processedRecords++;
                                    }
                                }
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning("Lỗi parse JSON: {Error}", ex.Message);
                        continue;
                    }
                }

                var finalValueTrieuVnd = Math.Round(totalNguonVon / 1_000_000m, 2);

                return Ok(new
                {
                    targetDate = targetDate.ToString("yyyy-MM-dd"),
                    branchCode,
                    fileInfo = new
                    {
                        latestRecord.FileName,
                        latestRecord.StatementDate,
                        latestRecord.ImportDate,
                        latestRecord.Category
                    },
                    calculation = new
                    {
                        totalRecords = importedItems.Count,
                        processedRecords,
                        excludedPrefixes,
                        totalNguonVonVnd = totalNguonVon,
                        totalNguonVonTrieuVnd = finalValueTrieuVnd
                    },
                    accountDetails = accountDetails
                        .Where(a => ((dynamic)a).belongsToBranch)
                        .OrderByDescending(a => ((dynamic)a).balance)
                        .Take(20) // Top 20 tài khoản có số dư lớn nhất
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi debug DP01 data");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Debug cấu trúc dữ liệu DP01 - xem sample records
        /// </summary>
        [HttpGet("debug-dp01-structure")]
        public async Task<ActionResult> DebugDP01Structure()
        {
            try
            {
                var targetDate = new DateTime(2024, 12, 31);

                _logger.LogInformation("🔍 Debug cấu trúc dữ liệu DP01 cho ngày {Date}",
                    targetDate.ToString("yyyy-MM-dd"));

                // Tìm file DP01 cho ngày 31/12/2024
                var latestRecord = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date == targetDate.Date &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("DP01") || r.Category.Contains("Nguồn vốn")))
                    .OrderByDescending(r => r.ImportDate)
                    .FirstOrDefaultAsync();

                if (latestRecord == null)
                {
                    return NotFound(new { message = "Không tìm thấy file DP01 cho ngày 31/12/2024" });
                }

                // Lấy 5 sample records
                var sampleItems = await _context.ImportedDataItems
                    .Where(i => i.ImportedDataRecordId == latestRecord.Id)
                    .Select(i => i.RawData)
                    .Take(5)
                    .ToListAsync();

                var parsedSamples = new List<object>();
                var allFieldNames = new HashSet<string>();

                foreach (var rawDataJson in sampleItems)
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(rawDataJson);
                        var root = jsonDoc.RootElement;

                        var fieldData = new Dictionary<string, string>();

                        foreach (var property in root.EnumerateObject())
                        {
                            allFieldNames.Add(property.Name);
                            fieldData[property.Name] = property.Value.ToString();
                        }

                        parsedSamples.Add(fieldData);
                    }
                    catch (JsonException ex)
                    {
                        parsedSamples.Add(new { error = ex.Message, rawData = rawDataJson });
                    }
                }

                return Ok(new
                {
                    fileInfo = new
                    {
                        latestRecord.FileName,
                        latestRecord.StatementDate,
                        latestRecord.ImportDate,
                        latestRecord.Category
                    },
                    totalRecords = await _context.ImportedDataItems
                        .Where(i => i.ImportedDataRecordId == latestRecord.Id)
                        .CountAsync(),
                    allFieldNames = allFieldNames.OrderBy(x => x).ToList(),
                    sampleRecords = parsedSamples
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi debug cấu trúc DP01");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test định dạng số theo chuẩn Việt Nam
        /// </summary>
        [HttpGet("test-number-format")]
        public ActionResult TestNumberFormat()
        {
            try
            {
                var testResult = NumberFormatter.TestFormat();

                return Ok(new
                {
                    testResult = testResult.Split('\n'),
                    examples = new
                    {
                        correctValue = new
                        {
                            original = 499616000000m,
                            inTrieuVnd = 499616000000m / 1_000_000m,
                            formatted = NumberFormatter.FormatNumber(499616000000m / 1_000_000m, 2),
                            withCurrency = NumberFormatter.FormatCurrency(499616000000m)
                        },
                        wrongValue = new
                        {
                            original = 1042128780000m,
                            inTrieuVnd = 1042128780000m / 1_000_000m,
                            formatted = NumberFormatter.FormatNumber(1042128780000m / 1_000_000m, 2),
                            withCurrency = NumberFormatter.FormatCurrency(1042128780000m)
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Liệt kê tất cả file DP01 đã import trong hệ thống
        /// </summary>
        [HttpGet("list-all-dp01-files")]
        public async Task<ActionResult> ListAllDP01Files()
        {
            try
            {
                _logger.LogInformation("🔍 Liệt kê tất cả file DP01 trong hệ thống");

                var dp01Records = await _context.ImportedDataRecords
                    .Where(r => r.Category.Contains("DP01") || r.Category.Contains("Nguồn vốn"))
                    .OrderByDescending(r => r.StatementDate)
                    .ThenByDescending(r => r.ImportDate)
                    .Select(r => new
                    {
                        r.Id,
                        r.FileName,
                        r.Category,
                        r.StatementDate,
                        r.ImportDate,
                        r.Status,
                        TotalRecords = _context.ImportedDataItems.Count(i => i.ImportedDataRecordId == r.Id)
                    })
                    .ToListAsync();

                // Phân tích tên file để tìm mã chi nhánh
                var filesWithBranchInfo = dp01Records.Select(r =>
                {
                    string branchCode = "Unknown";
                    if (!string.IsNullOrEmpty(r.FileName))
                    {
                        // Pattern: 7800_dp01_20241231.csv
                        var parts = r.FileName.Split('_');
                        if (parts.Length >= 2 && parts[1].ToLower().Contains("dp01"))
                        {
                            branchCode = parts[0];
                        }
                    }

                    string branchName = branchCode switch
                    {
                        "7800" => "Hội Sở",
                        "7801" => "Chi nhánh Tam Đường",
                        "7802" => "Chi nhánh Phong Thổ",
                        "7803" => "Chi nhánh Sin Hồ",
                        "7804" => "Chi nhánh Mường Tè",
                        "7805" => "Chi nhánh Than Uyên",
                        "7806" => "Chi nhánh Thành phố",
                        "7807" => "Chi nhánh Tân Uyên",
                        "7808" => "Chi nhánh Nậm Nhùn",
                        "9999" => "Chi nhánh Lai Châu",
                        _ => $"Chi nhánh {branchCode}"
                    };

                    return new
                    {
                        r.Id,
                        r.FileName,
                        r.Category,
                        r.StatementDate,
                        r.ImportDate,
                        r.Status,
                        r.TotalRecords,
                        BranchCode = branchCode,
                        BranchName = branchName,
                        IsHoiSo = branchCode == "7800"
                    };
                }).ToList();

                var hoiSoFiles = filesWithBranchInfo.Where(f => f.IsHoiSo).ToList();

                return Ok(new
                {
                    totalDP01Files = dp01Records.Count,
                    hoiSoFiles = new
                    {
                        count = hoiSoFiles.Count,
                        files = hoiSoFiles
                    },
                    allBranches = filesWithBranchInfo
                        .GroupBy(f => new { f.BranchCode, f.BranchName })
                        .Select(g => new
                        {
                            g.Key.BranchCode,
                            g.Key.BranchName,
                            FileCount = g.Count(),
                            LatestFile = g.OrderByDescending(f => f.StatementDate).FirstOrDefault()
                        })
                        .OrderBy(b => b.BranchCode)
                        .ToList(),
                    allFiles = filesWithBranchInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi liệt kê file DP01");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test tính toán nguồn vốn trực tiếp với file 7800_dp01_20241231.csv
        /// </summary>
        [HttpGet("test-7800-calculation")]
        public async Task<ActionResult> Test7800Calculation()
        {
            try
            {
                var branchCode = "7800"; // Hội Sở
                var targetDate = new DateTime(2024, 12, 31);

                _logger.LogInformation("🔍 Test tính toán trực tiếp file 7800_dp01_20241231.csv");

                // Tìm file cụ thể 7800_dp01_20241231.csv
                var specificRecord = await _context.ImportedDataRecords
                    .FirstOrDefaultAsync(r => r.FileName == "7800_dp01_20241231.csv" &&
                                            r.Status == "Completed");

                if (specificRecord == null)
                {
                    return NotFound(new { message = "Không tìm thấy file 7800_dp01_20241231.csv" });
                }

                // Lấy sample dữ liệu từ file này
                var sampleItems = await _context.ImportedDataItems
                    .Where(i => i.ImportedDataRecordId == specificRecord.Id)
                    .Select(i => i.RawData)
                    .Take(10)
                    .ToListAsync();

                var accountAnalysis = new List<object>();
                decimal totalBalance = 0;
                var excludedPrefixes = new[] { "2", "40", "41", "427" };
                var processedCount = 0;

                foreach (var rawDataJson in sampleItems)
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(rawDataJson);
                        var root = jsonDoc.RootElement;

                        var analysis = new Dictionary<string, object>();

                        // Lấy tất cả fields
                        foreach (var prop in root.EnumerateObject())
                        {
                            analysis[prop.Name] = prop.Value.ToString();
                        }

                        // Kiểm tra logic
                        var accountCode = "";
                        if (root.TryGetProperty("TAI_KHOAN_HACH_TOAN", out var tkhElement))
                            accountCode = tkhElement.GetString() ?? "";

                        var fileBranchCode = "";
                        if (root.TryGetProperty("MA_CN", out var maCnElement))
                            fileBranchCode = maCnElement.GetString() ?? "";

                        var balance = 0m;
                        if (root.TryGetProperty("CURRENT_BALANCE", out var balanceElement) &&
                            decimal.TryParse(balanceElement.GetString(), out balance))
                        {
                            // Logic kiểm tra
                            var belongsToBranch = fileBranchCode == branchCode;
                            var isExcluded = excludedPrefixes.Any(prefix => accountCode.StartsWith(prefix));
                            var willInclude = belongsToBranch && !isExcluded;

                            if (willInclude)
                            {
                                totalBalance += balance;
                                processedCount++;
                            }

                            analysis["_LOGIC"] = new
                            {
                                accountCode,
                                fileBranchCode,
                                balance = balance.ToString("N0"),
                                belongsToBranch,
                                isExcluded,
                                willInclude
                            };
                        }

                        accountAnalysis.Add(analysis);
                    }
                    catch (Exception ex)
                    {
                        accountAnalysis.Add(new { error = ex.Message });
                    }
                }

                return Ok(new
                {
                    fileInfo = new
                    {
                        specificRecord.FileName,
                        specificRecord.StatementDate,
                        specificRecord.Id,
                        TotalRecordsInFile = await _context.ImportedDataItems
                            .CountAsync(i => i.ImportedDataRecordId == specificRecord.Id)
                    },
                    searchCriteria = new
                    {
                        targetBranchCode = branchCode,
                        targetDate = targetDate.ToString("yyyy-MM-dd"),
                        excludedPrefixes
                    },
                    sampleAnalysis = new
                    {
                        totalSampleRecords = sampleItems.Count,
                        processedCount,
                        totalBalance = totalBalance.ToString("N0"),
                        totalBalanceTrieuVnd = (totalBalance / 1_000_000m).ToString("N2"),
                        sampleDetails = accountAnalysis
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi test file 7800");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Tính toán TOÀN BỘ nguồn vốn từ file 7800_dp01_20241231.csv
        /// </summary>
        [HttpGet("calculate-full-7800")]
        public async Task<ActionResult> CalculateFull7800()
        {
            try
            {
                var branchCode = "7800";
                var targetDate = new DateTime(2024, 12, 31);

                _logger.LogInformation("💰 Tính toán TOÀN BỘ nguồn vốn từ file 7800_dp01_20241231.csv");

                var specificRecord = await _context.ImportedDataRecords
                    .FirstOrDefaultAsync(r => r.FileName == "7800_dp01_20241231.csv" &&
                                            r.Status == "Completed");

                if (specificRecord == null)
                {
                    return NotFound(new { message = "Không tìm thấy file 7800_dp01_20241231.csv" });
                }

                // Lấy TOÀN BỘ dữ liệu từ file này
                var allItems = await _context.ImportedDataItems
                    .Where(i => i.ImportedDataRecordId == specificRecord.Id)
                    .Select(i => i.RawData)
                    .ToListAsync();

                decimal totalBalance = 0;
                var excludedPrefixes = new[] { "2", "40", "41", "427" };
                var processedCount = 0;
                var skippedCount = 0;
                var errorCount = 0;
                var accountBreakdown = new Dictionary<string, decimal>();

                foreach (var rawDataJson in allItems)
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(rawDataJson);
                        var root = jsonDoc.RootElement;

                        // Lấy thông tin cần thiết
                        var accountCode = "";
                        if (root.TryGetProperty("TAI_KHOAN_HACH_TOAN", out var tkhElement))
                            accountCode = tkhElement.GetString() ?? "";

                        var fileBranchCode = "";
                        if (root.TryGetProperty("MA_CN", out var maCnElement))
                            fileBranchCode = maCnElement.GetString() ?? "";

                        if (root.TryGetProperty("CURRENT_BALANCE", out var balanceElement) &&
                            decimal.TryParse(balanceElement.GetString(), out var balance))
                        {
                            var belongsToBranch = fileBranchCode == branchCode;
                            var isExcluded = excludedPrefixes.Any(prefix => accountCode.StartsWith(prefix));

                            if (belongsToBranch && !isExcluded)
                            {
                                totalBalance += balance;
                                processedCount++;

                                // Thống kê theo tài khoản
                                if (!accountBreakdown.ContainsKey(accountCode))
                                    accountBreakdown[accountCode] = 0;
                                accountBreakdown[accountCode] += balance;
                            }
                            else
                            {
                                skippedCount++;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        errorCount++;
                    }
                }

                var finalTrieuVnd = totalBalance / 1_000_000m;

                return Ok(new
                {
                    fileInfo = new
                    {
                        specificRecord.FileName,
                        specificRecord.StatementDate,
                        TotalRecordsInFile = allItems.Count
                    },
                    calculation = new
                    {
                        totalRecords = allItems.Count,
                        processedCount,
                        skippedCount,
                        errorCount,
                        totalBalanceVnd = NumberFormatter.FormatNumber(totalBalance, 0),
                        totalBalanceTrieuVnd = NumberFormatter.FormatNumber(finalTrieuVnd, 2),
                        finalResult = $"{NumberFormatter.FormatNumber(finalTrieuVnd, 2)} triệu VND"
                    },
                    accountBreakdown = accountBreakdown
                        .OrderByDescending(kvp => kvp.Value)
                        .Take(10)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => new
                            {
                                balanceVnd = NumberFormatter.FormatNumber(kvp.Value, 0),
                                balanceTrieuVnd = NumberFormatter.FormatNumber(kvp.Value / 1_000_000m, 2)
                            }
                        ),
                    excludedPrefixes,
                    searchCriteria = new { branchCode, targetDate = targetDate.ToString("yyyy-MM-dd") }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính toán toàn bộ file 7800");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách StatementDate có sẵn trong database cho DP01
        /// </summary>
        [HttpGet("statement-dates")]
        public async Task<ActionResult> GetAvailableStatementDates()
        {
            try
            {
                _logger.LogInformation("🔍 Lấy danh sách StatementDate có sẵn");

                var statementDates = await _context.ImportedDataRecords
                    .Where(x => x.Category == "DP01" && x.StatementDate.HasValue)
                    .Select(x => x.StatementDate.Value)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .ToListAsync();

                var formattedDates = statementDates.Select(date => new
                {
                    date = date.ToString("yyyy-MM-dd"),
                    vietnameseFormat = VietnamDateTime.ToVietnameseDateString(date),
                    recordCount = _context.ImportedDataRecords
                        .Count(x => x.Category == "DP01" && x.StatementDate.HasValue && x.StatementDate.Value.Date == date.Date)
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = $"Tìm thấy {statementDates.Count} ngày StatementDate khác nhau",
                    dates = formattedDates,
                    totalRecords = await _context.ImportedDataRecords.CountAsync(x => x.Category == "DP01")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy StatementDate");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Debug filter theo ngày - kiểm tra tại sao 2 ngày khác nhau ra cùng kết quả
        /// </summary>
        [HttpPost("debug-date-filter")]
        public async Task<ActionResult> DebugDateFilter([FromBody] DebugDateFilterRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Debug filter theo ngày - {Date1} vs {Date2}",
                    request.Date1?.ToString("dd/MM/yyyy") ?? "null",
                    request.Date2?.ToString("dd/MM/yyyy") ?? "null");

                var result1 = request.Date1.HasValue
                    ? await _branchCalculationService.CalculateNguonVonByBranch(request.BranchId, request.Date1)
                    : await _branchCalculationService.CalculateNguonVonByBranch(request.BranchId);

                var result2 = request.Date2.HasValue
                    ? await _branchCalculationService.CalculateNguonVonByBranch(request.BranchId, request.Date2)
                    : await _branchCalculationService.CalculateNguonVonByBranch(request.BranchId);

                // Kiểm tra dữ liệu thực tế trong database cho mỗi ngày
                var records1 = request.Date1.HasValue
                    ? await _context.ImportedDataRecords
                        .Where(x => x.Category == "DP01" && x.StatementDate.HasValue && x.StatementDate.Value.Date == request.Date1.Value.Date)
                        .CountAsync()
                    : await _context.ImportedDataRecords
                        .Where(x => x.Category == "DP01" && x.StatementDate.HasValue)
                        .CountAsync();

                var records2 = request.Date2.HasValue
                    ? await _context.ImportedDataRecords
                        .Where(x => x.Category == "DP01" && x.StatementDate.HasValue && x.StatementDate.Value.Date == request.Date2.Value.Date)
                        .CountAsync()
                    : await _context.ImportedDataRecords
                        .Where(x => x.Category == "DP01" && x.StatementDate.HasValue)
                        .CountAsync();

                return Ok(new
                {
                    success = true,
                    branchId = request.BranchId,
                    date1 = new
                    {
                        date = request.Date1?.ToString("dd/MM/yyyy") ?? "latest",
                        result = result1,
                        resultTrieuVnd = Math.Round(result1 / 1_000_000m, 2),
                        recordsFound = records1
                    },
                    date2 = new
                    {
                        date = request.Date2?.ToString("dd/MM/yyyy") ?? "latest",
                        result = result2,
                        resultTrieuVnd = Math.Round(result2 / 1_000_000m, 2),
                        recordsFound = records2
                    },
                    analysis = new
                    {
                        isSameResult = result1 == result2,
                        difference = Math.Abs(result1 - result2),
                        differenceTrieuVnd = Math.Round(Math.Abs(result1 - result2) / 1_000_000m, 2),
                        possibleIssues = result1 == result2 && request.Date1 != request.Date2
                            ? new[] { "Filter theo ngày không hoạt động", "Cùng sử dụng ngày gần nhất", "Dữ liệu trùng lặp" }
                            : new[] { "Logic filter theo ngày hoạt động bình thường" }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi debug date filter");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
