using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using YourNamespace.Data; // Thay đổi theo không gian tên thực tế của bạn

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YourController : ControllerBase
    {
        private readonly YourDbContext _context;

        public YourController(YourDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Debug endpoint: Kiểm tra dữ liệu DP01 cho chi nhánh và ngày cụ thể
        /// </summary>
        [HttpGet("check-dp01-data")]
        public async Task<IActionResult> CheckDP01Data(string branchCode, string? date = null)
        {
            try
            {
                DateTime? targetDate = null;
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
                {
                    targetDate = parsedDate;
                }

                var query = _context.ImportedDataRecords.AsQueryable();

                // Filter theo ngày nếu có
                if (targetDate.HasValue)
                {
                    query = query.Where(r => r.ImportDate.Date == targetDate.Value.Date);
                }

                var records = await query
                    .Where(r => r.TableName == "DP01")
                    .ToListAsync();

                var result = new List<object>();
                int totalItems = 0;
                int matchedItems = 0;

                foreach (var record in records)
                {
                    if (string.IsNullOrEmpty(record.JsonData)) continue;

                    var jsonData = JsonDocument.Parse(record.JsonData);
                    if (jsonData.RootElement.ValueKind != JsonValueKind.Array) continue;

                    foreach (var item in jsonData.RootElement.EnumerateArray())
                    {
                        totalItems++;
                        
                        var maCn = item.TryGetProperty("MA_CN", out var cnProp) ? cnProp.GetString() : "";
                        var maPgd = item.TryGetProperty("MA_PGD", out var pgdProp) ? pgdProp.GetString() : "";
                        var taiKhoan = item.TryGetProperty("TAI_KHOAN_HACH_TOAN", out var tkProp) ? tkProp.GetString() : "";
                        
                        decimal currentBalance = 0;
                        if (item.TryGetProperty("CURRENT_BALANCE", out var balanceProp))
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

                        if (maCn == branchCode)
                        {
                            matchedItems++;
                            result.Add(new
                            {
                                MA_CN = maCn,
                                MA_PGD = maPgd,
                                TAI_KHOAN_HACH_TOAN = taiKhoan,
                                CURRENT_BALANCE = currentBalance,
                                IMPORT_DATE = record.ImportDate,
                                IS_EXCLUDED = IsExcludedAccount(taiKhoan ?? "")
                            });
                        }
                    }
                }

                return Ok(new
                {
                    BranchCode = branchCode,
                    TargetDate = targetDate?.ToString("yyyy-MM-dd"),
                    TotalRecords = records.Count,
                    TotalItems = totalItems,
                    MatchedItems = matchedItems,
                    Data = result.Take(20).ToList(), // Chỉ trả về 20 items đầu
                    Summary = new
                    {
                        ExcludedAccounts = result.Count(x => (bool)x.GetType().GetProperty("IS_EXCLUDED")?.GetValue(x)!),
                        IncludedAccounts = result.Count(x => !(bool)x.GetType().GetProperty("IS_EXCLUDED")?.GetValue(x)!),
                        TotalBalance = result.Sum(x => (decimal)x.GetType().GetProperty("CURRENT_BALANCE")?.GetValue(x)!)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private static bool IsExcludedAccount(string taiKhoan)
        {
            return taiKhoan.StartsWith("40") ||
                   taiKhoan.StartsWith("41") ||
                   taiKhoan.StartsWith("427") ||
                   taiKhoan == "211108";
        }

        // ...existing code...
    }
}