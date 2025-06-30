using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller debug để kiểm tra dữ liệu DP01 trong database
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DebugDP01Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DebugDP01Controller> _logger;

        public DebugDP01Controller(ApplicationDbContext context, ILogger<DebugDP01Controller> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Kiểm tra các file DP01 đã import
        /// </summary>
        [HttpGet("check-files")]
        public async Task<IActionResult> CheckDP01Files()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra files DP01 trong database");

                // Tìm tất cả records có category DP01
                var dp01Records = await _context.ImportedDataRecords
                    .Where(x => x.Category == "DP01" || x.FileName.Contains("DP01"))
                    .OrderByDescending(x => x.ImportDate)
                    .ToListAsync();

                var result = new
                {
                    TotalRecords = dp01Records.Count,
                    Files = dp01Records.Select(r => new
                    {
                        r.Id,
                        r.FileName,
                        r.Category,
                        ImportedAt = r.ImportDate,
                        TotalRows = r.RecordsCount,
                        r.Status
                    }).ToList()
                };

                _logger.LogInformation("✅ Tìm thấy {Count} files DP01", dp01Records.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi kiểm tra files DP01");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu DP01 chi tiết cho chi nhánh 7800
        /// </summary>
        [HttpGet("check-7800-data")]
        public async Task<IActionResult> Check7800Data()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra dữ liệu DP01 cho chi nhánh 7800");

                // Tìm files DP01 có chứa 7800
                var dp01Records = await _context.ImportedDataRecords
                    .Where(x => (x.Category == "DP01" || x.FileName.Contains("DP01")) && x.FileName.Contains("7800"))
                    .ToListAsync();

                if (!dp01Records.Any())
                {
                    return Ok(new
                    {
                        Message = "Không tìm thấy file DP01 nào cho 7800",
                        AllDP01Files = await _context.ImportedDataRecords
                            .Where(x => x.Category == "DP01" || x.FileName.Contains("DP01"))
                            .Select(x => x.FileName)
                            .ToListAsync()
                    });
                }

                var results = new List<object>();

                foreach (var record in dp01Records)
                {
                    // Lấy 5 mẫu dữ liệu từ file này
                    var sampleItems = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == record.Id)
                        .Take(5)
                        .Select(x => x.RawData)
                        .ToListAsync();

                    var parsedSamples = new List<object>();
                    decimal totalBalance = 0;
                    int totalItems = 0;

                    // Đếm tổng số items
                    totalItems = await _context.ImportedDataItems
                        .CountAsync(x => x.ImportedDataRecordId == record.Id);

                    // Parse một vài mẫu để xem cấu trúc
                    foreach (var rawData in sampleItems)
                    {
                        try
                        {
                            var jsonDoc = JsonDocument.Parse(rawData);
                            var root = jsonDoc.RootElement;

                            // Thử parse các trường có thể có
                            var sample = new
                            {
                                MA_CN = root.TryGetProperty("MA_CN", out var maCnProp) ? maCnProp.GetString() : null,
                                MA_PGD = root.TryGetProperty("MA_PGD", out var maPgdProp) ? maPgdProp.GetString() : null,
                                TAI_KHOAN_HACH_TOAN = root.TryGetProperty("TAI_KHOAN_HACH_TOAN", out var tkProp) ? tkProp.GetString() : null,
                                CURRENT_BALANCE = root.TryGetProperty("CURRENT_BALANCE", out var balanceProp) ? balanceProp.GetString() : null,
                                // Thử các tên khác có thể có
                                TaiKhoanHachToan = root.TryGetProperty("TaiKhoanHachToan", out var tk2Prop) ? tk2Prop.GetString() : null,
                                CurrentBalance = root.TryGetProperty("CurrentBalance", out var bal2Prop) ? bal2Prop.GetString() : null,
                                SoDu = root.TryGetProperty("SoDu", out var soDuProp) ? soDuProp.GetString() : null,
                                AllKeys = root.EnumerateObject().Select(p => p.Name).ToList()
                            };

                            parsedSamples.Add(sample);
                        }
                        catch (JsonException)
                        {
                            parsedSamples.Add(new { Error = "Invalid JSON", RawData = rawData.Substring(0, Math.Min(100, rawData.Length)) });
                        }
                    }

                    results.Add(new
                    {
                        FileName = record.FileName,
                        RecordId = record.Id,
                        ImportedAt = record.ImportDate,
                        TotalItems = totalItems,
                        SampleData = parsedSamples
                    });
                }

                return Ok(new
                {
                    Message = $"Tìm thấy {dp01Records.Count} file(s) DP01 cho 7800",
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi kiểm tra dữ liệu 7800");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Tính tổng nguồn vốn thực từ DP01 cho 7800
        /// </summary>
        [HttpGet("calculate-7800-total")]
        public async Task<IActionResult> Calculate7800Total()
        {
            try
            {
                _logger.LogInformation("💰 Tính tổng nguồn vốn thực cho 7800 từ DP01");

                // Tìm files DP01 cho 7800
                var dp01Records = await _context.ImportedDataRecords
                    .Where(x => (x.Category == "DP01" || x.FileName.Contains("DP01")) && x.FileName.Contains("7800"))
                    .ToListAsync();

                if (!dp01Records.Any())
                {
                    return Ok(new { Message = "Không tìm thấy file DP01 cho 7800" });
                }

                decimal totalNguonVon = 0;
                int totalAccounts = 0;
                int excludedAccounts = 0;
                var accountDetails = new List<object>();

                foreach (var record in dp01Records)
                {
                    var allItems = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == record.Id)
                        .Select(x => x.RawData)
                        .ToListAsync();

                    foreach (var rawData in allItems)
                    {
                        try
                        {
                            var jsonDoc = JsonDocument.Parse(rawData);
                            var root = jsonDoc.RootElement;

                            // Thử tất cả các cách có thể để lấy số dư
                            string? taiKhoan = null;
                            decimal balance = 0;

                            // Thử các tên trường khác nhau
                            if (root.TryGetProperty("TAI_KHOAN_HACH_TOAN", out var tkProp))
                                taiKhoan = tkProp.GetString();
                            else if (root.TryGetProperty("TaiKhoanHachToan", out var tk2Prop))
                                taiKhoan = tk2Prop.GetString();

                            if (root.TryGetProperty("CURRENT_BALANCE", out var balProp))
                            {
                                if (balProp.ValueKind == JsonValueKind.Number)
                                    balance = balProp.GetDecimal();
                                else if (balProp.ValueKind == JsonValueKind.String)
                                    decimal.TryParse(balProp.GetString(), out balance);
                            }
                            else if (root.TryGetProperty("CurrentBalance", out var bal2Prop))
                            {
                                if (bal2Prop.ValueKind == JsonValueKind.Number)
                                    balance = bal2Prop.GetDecimal();
                                else if (bal2Prop.ValueKind == JsonValueKind.String)
                                    decimal.TryParse(bal2Prop.GetString(), out balance);
                            }
                            else if (root.TryGetProperty("SoDu", out var soDuProp))
                            {
                                if (soDuProp.ValueKind == JsonValueKind.Number)
                                    balance = soDuProp.GetDecimal();
                                else if (soDuProp.ValueKind == JsonValueKind.String)
                                    decimal.TryParse(soDuProp.GetString(), out balance);
                            }

                            if (!string.IsNullOrEmpty(taiKhoan) && balance != 0)
                            {
                                totalAccounts++;

                                // Kiểm tra tài khoản loại trừ: 40*, 41*, 427*, 211108
                                bool isExcluded = taiKhoan.StartsWith("40") ||
                                                  taiKhoan.StartsWith("41") ||
                                                  taiKhoan.StartsWith("427") ||
                                                  taiKhoan == "211108";

                                if (isExcluded)
                                {
                                    excludedAccounts++;
                                }
                                else
                                {
                                    totalNguonVon += balance;
                                }

                                // Lưu chi tiết 20 tài khoản đầu
                                if (accountDetails.Count < 20)
                                {
                                    accountDetails.Add(new
                                    {
                                        TaiKhoan = taiKhoan,
                                        Balance = balance,
                                        IsExcluded = isExcluded,
                                        BalanceFormatted = (balance / 1_000_000_000m).ToString("N2", new System.Globalization.CultureInfo("vi-VN")) + " tỷ"
                                    });
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            // Skip invalid JSON
                        }
                    }
                }

                return Ok(new
                {
                    FileName = string.Join(", ", dp01Records.Select(r => r.FileName)),
                    TotalNguonVon = totalNguonVon,
                    TotalNguonVonTy = Math.Round(totalNguonVon / 1_000_000_000m, 2),
                    TotalAccounts = totalAccounts,
                    IncludedAccounts = totalAccounts - excludedAccounts,
                    ExcludedAccounts = excludedAccounts,
                    Formula = "Tổng CURRENT_BALANCE - (TK 40*, 41*, 427*, 211108)",
                    AccountDetails = accountDetails,
                    CalculatedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tính tổng nguồn vốn 7800");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
