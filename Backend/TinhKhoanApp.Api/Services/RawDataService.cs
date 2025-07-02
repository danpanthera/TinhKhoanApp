using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.NguonVon;
using TinhKhoanApp.Api.Services.Interfaces;
using System.Text.Json;

namespace TinhKhoanApp.Api.Services
{
    public class RawDataService : IRawDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RawDataService> _logger;

        public RawDataService(ApplicationDbContext context, ILogger<RawDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<NguonVonDetails> CalculateNguonVonFromRawDataAsync(NguonVonRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Starting calculation for NguonVon from raw data - Unit: {UnitCode}, DateType: {DateType}, Date: {Date}",
                    request.UnitCode, request.DateType, request.TargetDate);

                // Tìm các file DP01 tương ứng với chi nhánh và thời gian
                var query = _context.ImportedDataRecords
                    .Include(r => r.ImportedDataItems)
                    .Where(r => r.Category == "DP01" && r.Status == "Completed");

                // Lọc theo thời gian dựa trên StatementDate
                DateTime startDate, endDate;
                switch (request.DateType.ToLower())
                {
                    case "year":
                        startDate = new DateTime(request.TargetDate.Year, 1, 1);
                        endDate = new DateTime(request.TargetDate.Year, 12, 31);
                        break;
                    case "quarter":
                        var quarter = (request.TargetDate.Month - 1) / 3 + 1;
                        startDate = new DateTime(request.TargetDate.Year, (quarter - 1) * 3 + 1, 1);
                        endDate = startDate.AddMonths(3).AddDays(-1);
                        break;
                    case "month":
                        startDate = new DateTime(request.TargetDate.Year, request.TargetDate.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                        break;
                    case "day":
                        startDate = request.TargetDate.Date;
                        endDate = request.TargetDate.Date;
                        break;
                    default:
                        startDate = request.TargetDate.Date;
                        endDate = request.TargetDate.Date;
                        break;
                }

                query = query.Where(r => r.StatementDate >= startDate && r.StatementDate <= endDate);

                _logger.LogInformation("📅 Date range: {StartDate} to {EndDate}", startDate, endDate);

                var importedRecords = await query.ToListAsync();

                if (!importedRecords.Any())
                {
                    _logger.LogWarning("⚠️ No DP01 data found for the specified period");
                    return new NguonVonDetails
                    {
                        HasData = false,
                        Message = "Không tìm thấy dữ liệu DP01 cho khoảng thời gian được chọn",
                        Summary = new NguonVonResult
                        {
                            UnitCode = request.UnitCode,
                            TotalBalance = 0,
                            RecordCount = 0,
                            CalculatedDate = request.TargetDate
                        },
                        TopAccounts = new List<AccountDetail>()
                    };
                }

                decimal totalNguonVon = 0;
                int processedRecords = 0;
                var excludedAccounts = new[] { "40", "41", "427", "211108" };
                var accountBalances = new Dictionary<string, AccountDetail>();

                foreach (var record in importedRecords)
                {
                    foreach (var item in record.ImportedDataItems)
                    {
                        try
                        {
                            var jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.RawData);

                            // Lấy mã chi nhánh từ dữ liệu
                            if (jsonData.TryGetValue("MA_CN", out var maCnObj))
                            {
                                var maCn = maCnObj?.ToString();

                                // Nếu yêu cầu tính cho chi nhánh cụ thể (không phải ALL), chỉ lấy dữ liệu của chi nhánh đó
                                if (!string.IsNullOrEmpty(request.UnitCode) && request.UnitCode != "ALL" &&
                                    !string.Equals(maCn, request.UnitCode, StringComparison.OrdinalIgnoreCase))
                                {
                                    continue; // Bỏ qua bản ghi này
                                }
                            }

                            // Lấy tài khoản hạch toán
                            string? taiKhoan = null;
                            if (jsonData.TryGetValue("TAI_KHOAN_HACH_TOAN", out var taiKhoanObj))
                            {
                                taiKhoan = taiKhoanObj?.ToString();

                                // Loại trừ các tài khoản không tính vào nguồn vốn
                                if (!string.IsNullOrEmpty(taiKhoan))
                                {
                                    var shouldExclude = excludedAccounts.Any(excludedPrefix =>
                                        taiKhoan.StartsWith(excludedPrefix, StringComparison.OrdinalIgnoreCase));

                                    if (shouldExclude || taiKhoan.Equals("211108", StringComparison.OrdinalIgnoreCase))
                                    {
                                        continue; // Bỏ qua tài khoản này
                                    }
                                }
                            }

                            // Lấy số dư hiện tại
                            if (jsonData.TryGetValue("CURRENT_BALANCE", out var balanceObj))
                            {
                                var balanceStr = balanceObj?.ToString();
                                if (decimal.TryParse(balanceStr, out var balance))
                                {
                                    totalNguonVon += balance;
                                    processedRecords++;

                                    // Cập nhật thống kê theo tài khoản
                                    if (!string.IsNullOrEmpty(taiKhoan))
                                    {
                                        if (accountBalances.ContainsKey(taiKhoan))
                                        {
                                            accountBalances[taiKhoan].TotalBalance += balance;
                                            accountBalances[taiKhoan].RecordCount++;
                                        }
                                        else
                                        {
                                            accountBalances[taiKhoan] = new AccountDetail
                                            {
                                                AccountCode = taiKhoan,
                                                TotalBalance = balance,
                                                RecordCount = 1
                                            };
                                        }
                                    }
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning("⚠️ Failed to parse JSON data: {Error}", ex.Message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Error processing data item");
                        }
                    }
                }

                _logger.LogInformation("✅ Calculation completed - Total: {Total:N0}, Records: {Count}",
                    totalNguonVon, processedRecords);

                // Lấy top 20 tài khoản có số dư lớn nhất
                var topAccounts = accountBalances.Values
                    .OrderByDescending(a => a.TotalBalance)
                    .Take(20)
                    .ToList();

                return new NguonVonDetails
                {
                    HasData = true,
                    Message = "Tính toán nguồn vốn thành công",
                    Summary = new NguonVonResult
                    {
                        UnitCode = request.UnitCode,
                        TotalBalance = totalNguonVon,
                        RecordCount = processedRecords,
                        CalculatedDate = request.TargetDate
                    },
                    TopAccounts = topAccounts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error calculating NguonVon from raw data");
                return new NguonVonDetails
                {
                    HasData = false,
                    Message = $"Lỗi tính toán: {ex.Message}",
                    Summary = new NguonVonResult
                    {
                        UnitCode = request.UnitCode,
                        TotalBalance = 0,
                        RecordCount = 0,
                        CalculatedDate = request.TargetDate
                    },
                    TopAccounts = new List<AccountDetail>()
                };
            }
        }

        /// <summary>
        /// Lấy tên đơn vị từ mã đơn vị
        /// </summary>
        /// <param name="unitCode">Mã đơn vị</param>
        /// <returns>Tên đơn vị tương ứng</returns>
        private string GetUnitName(string unitCode)
        {
            if (string.IsNullOrEmpty(unitCode) || unitCode == "ALL")
            {
                return "🏛️ Toàn tỉnh (Tổng hợp)";
            }

            return unitCode switch
            {
                "7800" => "🏢 Hội Sở",
                "7801" => "🏦 Chi nhánh Bình Lư",
                "7802" => "🏦 Chi nhánh Phong Thổ",
                "7803" => "🏦 Chi nhánh Sìn Hồ",
                "7804" => "🏦 Chi nhánh Bum Tở",
                "7805" => "🏦 Chi nhánh Than Uyên",
                "7806" => "🏦 Chi nhánh Đoàn Kết",
                "7807" => "🏦 Chi nhánh Tân Uyên",
                "7808" => "🏦 Chi nhánh Nậm Hàng",
                _ => $"Chi nhánh {unitCode}"
            };
        }
    }
}
