using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Dashboard;
using System.Text.Json;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Interface cho service tính toán các chỉ tiêu Dashboard
    /// </summary>
    public interface IDashboardCalculationService
    {
        Task<decimal> CalculateNguonVon(int unitId, DateTime date);
        Task<decimal> CalculateDuNo(int unitId, DateTime date);
        Task<decimal> CalculateTyLeNoXau(int unitId, DateTime date);
        Task<decimal> CalculateThuHoiXLRR(int unitId, DateTime date);
        Task<decimal> CalculateThuDichVu(int unitId, DateTime date);
        Task<decimal> CalculateLoiNhuan(int unitId, DateTime date);
        Task RecalculateAllIndicators(int unitId, DateTime date);
        Task<List<DashboardCalculation>> GetCalculationHistory(int unitId, string? indicatorCode = null, int days = 30);
    }

    /// <summary>
    /// Service tính toán các chỉ tiêu Dashboard từ dữ liệu thô
    /// Thực hiện 6 công thức tính chính: Nguồn vốn, Dư nợ, Tỷ lệ nợ xấu, Thu hồi XLRR, Thu dịch vụ, Lợi nhuận
    /// </summary>
    public class DashboardCalculationService : IDashboardCalculationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardCalculationService> _logger;

        public DashboardCalculationService(
            ApplicationDbContext context,
            ILogger<DashboardCalculationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tính toán Nguồn vốn huy động từ dữ liệu thực đã import
        /// Công thức: Lấy dữ liệu nguồn vốn từ ngày mới nhất (file import gần nhất trong tháng)
        /// VD: Tháng 4/2025 → lấy file có ngày 20250430, Tháng 5/2025 → lấy file có ngày 20250531
        /// </summary>
        public async Task<decimal> CalculateNguonVon(int unitId, DateTime date)
        {
            var startTime = DateTime.Now;
            try
            {
                var unit = await _context.Units.FindAsync(unitId);
                if (unit == null)
                {
                    _logger.LogWarning("Unit {UnitId} không tồn tại", unitId);
                    return 0;
                }

                var branchCode = GetBranchCode(unit.Code);
                _logger.LogInformation("Tính toán Nguồn vốn cho {UnitName} (Code: {BranchCode}) ngày {Date}",
                    unit.Name, branchCode, date.ToString("yyyy-MM-dd"));

                // Tìm file import gần nhất trong tháng được chọn
                var lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

                // Lấy file import mới nhất có StatementDate trong tháng và chứa dữ liệu của chi nhánh
                var latestImportRecord = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value >= firstDayOfMonth &&
                               r.StatementDate.Value <= lastDayOfMonth &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("DP01") || r.Category.Contains("Nguồn vốn")))
                    .OrderByDescending(r => r.StatementDate)
                    .FirstOrDefaultAsync();

                if (latestImportRecord == null)
                {
                    _logger.LogWarning("Không tìm thấy file import nguồn vốn cho tháng {Month}/{Year}", date.Month, date.Year);

                    // Fallback: sử dụng dữ liệu mẫu như cũ
                    var sampleData = GenerateSampleNguonVonData(branchCode, date);
                    var excludedPrefixes = new[] { "2", "40", "41", "427" };
                    var totalBalance = sampleData
                        .Where(d => !excludedPrefixes.Any(prefix => d.AccountCode.StartsWith(prefix)))
                        .Sum(d => d.CurrentBalance);

                    var finalValue = totalBalance / 1_000_000m; // Chuyển sang triệu VND

                    var calculationDetails = new
                    {
                        Formula = "Tổng CURRENT_BALANCE - TK(2,40,41,427) từ dữ liệu mẫu",
                        TotalBalance = totalBalance,
                        FinalValue = finalValue,
                        Unit = "Triệu VND",
                        Note = "Sử dụng dữ liệu mẫu do không có file import thực",
                        CalculationDate = date,
                        UnitInfo = new { unit.Code, unit.Name },
                        BranchCode = branchCode
                    };

                    await SaveCalculation("NguonVon", unitId, date, finalValue, calculationDetails, startTime);
                    return finalValue;
                }

                // Lấy dữ liệu chi tiết từ file import mới nhất
                var importedItems = await _context.ImportedDataItems
                    .Where(i => i.ImportedDataRecordId == latestImportRecord.Id)
                    .Select(i => i.RawData)
                    .ToListAsync();

                decimal totalNguonVon = 0;
                var excludedPrefixes = new[] { "2", "40", "41", "427" };
                var processedRecords = 0;

                foreach (var rawDataJson in importedItems)
                {
                    try
                    {
                        // Parse JSON data để lấy thông tin tài khoản và số dư
                        var jsonDoc = JsonDocument.Parse(rawDataJson);
                        var root = jsonDoc.RootElement;

                        // Tìm các field có thể chứa thông tin tài khoản và số dư
                        if (root.TryGetProperty("ACCOUNT_CODE", out var accountCodeElement) ||
                            root.TryGetProperty("AccountCode", out accountCodeElement) ||
                            root.TryGetProperty("account_code", out accountCodeElement))
                        {
                            var accountCode = accountCodeElement.GetString() ?? "";

                            // Kiểm tra có thuộc chi nhánh đang tính không
                            var belongsToBranch = root.TryGetProperty("BRANCH_CODE", out var branchElement) &&
                                                branchElement.GetString() == branchCode;

                            if (!belongsToBranch) continue;

                            // Chỉ tính các tài khoản không thuộc danh sách loại trừ
                            if (!excludedPrefixes.Any(prefix => accountCode.StartsWith(prefix)))
                            {
                                if (root.TryGetProperty("CURRENT_BALANCE", out var balanceElement) ||
                                    root.TryGetProperty("CurrentBalance", out balanceElement) ||
                                    root.TryGetProperty("current_balance", out balanceElement))
                                {
                                    if (decimal.TryParse(balanceElement.GetString(), out var balance))
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
                        _logger.LogWarning("Lỗi parse JSON dữ liệu import: {Error}", ex.Message);
                        continue;
                    }
                }

                var finalValueReal = totalNguonVon / 1_000_000m; // Chuyển sang triệu VND

                var calculationDetailsReal = new
                {
                    Formula = "Tổng CURRENT_BALANCE - TK(2,40,41,427) từ file import mới nhất",
                    SourceFile = latestImportRecord.FileName,
                    StatementDate = latestImportRecord.StatementDate,
                    ImportDate = latestImportRecord.ImportDate,
                    TotalNguonVon = totalNguonVon,
                    FinalValue = finalValueReal,
                    Unit = "Triệu VND",
                    ProcessedRecords = processedRecords,
                    ExcludedAccountPrefixes = excludedPrefixes,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("NguonVon", unitId, date, finalValueReal, calculationDetailsReal, startTime);

                _logger.LogInformation("Hoàn thành tính Nguồn vốn từ file thực: {Value} triệu VND (từ {Records} bản ghi)",
                    finalValueReal, processedRecords);
                return finalValueReal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Nguồn vốn cho unit {UnitId}", unitId);
                await SaveCalculationError("NguonVon", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Dư nợ cho vay từ dữ liệu LN01
        /// Công thức: Tổng DISBURSEMENT_AMOUNT theo chi nhánh và PGD
        /// </summary>
        public async Task<decimal> CalculateDuNo(int unitId, DateTime date)
        {
            var startTime = DateTime.Now;
            try
            {
                var unit = await _context.Units.FindAsync(unitId);
                if (unit == null) return 0;

                var branchCode = GetBranchCode(unit.Code);
                _logger.LogInformation("Tính toán Dư nợ cho {UnitName} ngày {Date}", unit.Name, date.ToString("yyyy-MM-dd"));

                // TODO: Thay bằng query thực từ bảng LN01
                var sampleData = GenerateSampleDuNoData(branchCode, date);

                // Tính tổng DISBURSEMENT_AMOUNT
                var totalDisbursement = sampleData.Sum(d => (decimal)d.DisbursementAmount);

                // Phân tích theo nhóm nợ
                var byGroup = sampleData
                    .GroupBy(d => d.NhomNo)
                    .Select(g => new
                    {
                        NhomNo = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(d => (decimal)d.DisbursementAmount)
                    })
                    .ToList();

                var calculationDetails = new
                {
                    Formula = "Tổng DISBURSEMENT_AMOUNT từ LN01",
                    TotalDisbursement = totalDisbursement,
                    ByNhomNo = byGroup,
                    TotalRecords = sampleData.Count,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                var finalValue = totalDisbursement / 1_000_000m; // Chuyển sang triệu VND

                await SaveCalculation("DuNo", unitId, date, finalValue, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Dư nợ: {Value} triệu VND", finalValue);
                return finalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Dư nợ cho unit {UnitId}", unitId);
                await SaveCalculationError("DuNo", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Tỷ lệ nợ xấu từ dữ liệu LN01
        /// Công thức: (Nợ nhóm 3,4,5 / Tổng dư nợ) * 100
        /// </summary>
        public async Task<decimal> CalculateTyLeNoXau(int unitId, DateTime date)
        {
            var startTime = DateTime.Now;
            try
            {
                var unit = await _context.Units.FindAsync(unitId);
                if (unit == null) return 0;

                var branchCode = GetBranchCode(unit.Code);
                _logger.LogInformation("Tính toán Tỷ lệ nợ xấu cho {UnitName} ngày {Date}", unit.Name, date.ToString("yyyy-MM-dd"));

                // TODO: Thay bằng query thực từ bảng LN01
                var sampleData = GenerateSampleDuNoData(branchCode, date);

                // Tổng dư nợ
                var totalDebt = sampleData.Sum(d => (decimal)d.DisbursementAmount);

                // Nợ xấu (nhóm 3, 4, 5)
                var badDebtGroups = new[] { "03", "04", "05" };
                var badDebt = sampleData
                    .Where(d => badDebtGroups.Contains((string)d.NhomNo))
                    .Sum(d => (decimal)d.DisbursementAmount);

                var ratio = totalDebt > 0 ? (badDebt / totalDebt * 100) : 0;

                var calculationDetails = new
                {
                    Formula = "(Nợ nhóm 3,4,5 / Tổng dư nợ) * 100",
                    TotalDebt = totalDebt,
                    BadDebt = badDebt,
                    BadDebtGroups = badDebtGroups,
                    Ratio = ratio,
                    GroupBreakdown = sampleData.GroupBy(d => d.NhomNo)
                        .Select(g => new { NhomNo = g.Key, Count = g.Count(), Amount = g.Sum(d => (decimal)d.DisbursementAmount) })
                        .ToList(),
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("TyLeNoXau", unitId, date, ratio, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Tỷ lệ nợ xấu: {Value}%", ratio);
                return ratio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Tỷ lệ nợ xấu cho unit {UnitId}", unitId);
                await SaveCalculationError("TyLeNoXau", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Thu hồi nợ đã XLRR (đơn vị: Triệu VND)
        /// TODO: Cần dữ liệu thực từ bảng import để có công thức chính xác
        /// </summary>
        public async Task<decimal> CalculateThuHoiXLRR(int unitId, DateTime date)
        {
            var startTime = DateTime.Now;
            try
            {
                var unit = await _context.Units.FindAsync(unitId);
                if (unit == null) return 0;

                var branchCode = GetBranchCode(unit.Code);
                _logger.LogInformation("Tính toán Thu hồi XLRR cho {UnitName} ngày {Date}", unit.Name, date.ToString("yyyy-MM-dd"));

                // Tạm thời dùng giá trị mẫu theo yêu cầu đơn vị Triệu VND
                var sampleValueTrieuVND = new Random().Next(10, 100); // Triệu VND

                var calculationDetails = new
                {
                    Formula = "Tổng số tiền thu hồi từ nợ đã XLRR",
                    Note = "Chờ dữ liệu thực từ file import LN01",
                    Unit = "Triệu VND",
                    SampleValue = sampleValueTrieuVND,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("ThuHoiXLRR", unitId, date, sampleValueTrieuVND, calculationDetails, startTime);
                _logger.LogInformation("Hoàn thành tính Thu hồi XLRR: {Value} triệu VND", sampleValueTrieuVND);
                return sampleValueTrieuVND;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Thu hồi XLRR cho unit {UnitId}", unitId);
                await SaveCalculationError("ThuHoiXLRR", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Thu dịch vụ
        /// TODO: Chờ anh cung cấp công thức chi tiết
        /// </summary>
        public async Task<decimal> CalculateThuDichVu(int unitId, DateTime date)
        {
            var startTime = DateTime.Now;
            try
            {
                _logger.LogInformation("Tính toán Thu dịch vụ - chờ công thức cụ thể");

                // Tạm thời trả về giá trị mẫu (triệu VND)
                var sampleValue = new Random().Next(50, 200);

                var calculationDetails = new
                {
                    Formula = "Chờ công thức từ anh",
                    Note = "Tính năng đang phát triển - đơn vị: Triệu VND",
                    SampleValue = sampleValue,
                    Unit = "Triệu VND",
                    CalculationDate = date
                };

                await SaveCalculation("ThuDichVu", unitId, date, sampleValue, calculationDetails, startTime);
                return sampleValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Thu dịch vụ cho unit {UnitId}", unitId);
                await SaveCalculationError("ThuDichVu", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Lợi nhuận tài chính từ dữ liệu GLCB41
        /// Công thức: (Tài khoản 7 + 790001 + 8511) - (Tài khoản 8 + 882)
        /// </summary>
        public async Task<decimal> CalculateLoiNhuan(int unitId, DateTime date)
        {
            var startTime = DateTime.Now;
            try
            {
                var unit = await _context.Units.FindAsync(unitId);
                if (unit == null) return 0;

                var branchCode = GetBranchCode(unit.Code);
                _logger.LogInformation("Tính toán Lợi nhuận cho {UnitName} ngày {Date}", unit.Name, date.ToString("yyyy-MM-dd"));

                // TODO: Thay bằng query thực từ bảng GLCB41
                var sampleData = GenerateSampleGLCB41Data(branchCode, date);

                // Thu nhập (7 + 790001 + 8511)
                var revenueAccounts = new[] { "7", "790001", "8511" };
                var revenue = sampleData
                    .Where(d => revenueAccounts.Any(acc => d.AccountCode.StartsWith(acc)))
                    .Sum(d => (decimal)d.CreditAmount - (decimal)d.DebitAmount); // Credit - Debit cho tài khoản thu

                // Chi phí (8 + 882)
                var expenseAccounts = new[] { "8", "882" };
                var expense = sampleData
                    .Where(d => expenseAccounts.Any(acc => d.AccountCode.StartsWith(acc)))
                    .Sum(d => (decimal)d.DebitAmount - (decimal)d.CreditAmount); // Debit - Credit cho tài khoản chi

                var profit = revenue - expense;

                var calculationDetails = new
                {
                    Formula = "(TK 7+790001+8511) - (TK 8+882)",
                    Revenue = revenue,
                    RevenueAccounts = revenueAccounts,
                    Expense = expense,
                    ExpenseAccounts = expenseAccounts,
                    Profit = profit,
                    RevenueDetail = sampleData
                        .Where(d => revenueAccounts.Any(acc => d.AccountCode.StartsWith(acc)))
                        .GroupBy(d => d.AccountCode.Substring(0, Math.Min(d.AccountCode.Length, 6)))
                        .Select(g => new { Account = g.Key, Amount = g.Sum(d => (decimal)d.CreditAmount - (decimal)d.DebitAmount) })
                        .ToList(),
                    ExpenseDetail = sampleData
                        .Where(d => expenseAccounts.Any(acc => d.AccountCode.StartsWith(acc)))
                        .GroupBy(d => d.AccountCode.Substring(0, Math.Min(d.AccountCode.Length, 3)))
                        .Select(g => new { Account = g.Key, Amount = g.Sum(d => (decimal)d.DebitAmount - (decimal)d.CreditAmount) })
                        .ToList(),
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                var finalValue = profit / 1_000_000m; // Chuyển sang triệu VND

                await SaveCalculation("LoiNhuan", unitId, date, finalValue, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Lợi nhuận: {Value} triệu VND", finalValue);
                return finalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Lợi nhuận cho unit {UnitId}", unitId);
                await SaveCalculationError("LoiNhuan", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán lại tất cả 6 chỉ tiêu
        /// </summary>
        public async Task RecalculateAllIndicators(int unitId, DateTime date)
        {
            _logger.LogInformation("Bắt đầu tính toán lại tất cả chỉ tiêu cho unit {UnitId} ngày {Date}", unitId, date);

            var tasks = new[]
            {
                CalculateNguonVon(unitId, date),
                CalculateDuNo(unitId, date),
                CalculateTyLeNoXau(unitId, date),
                CalculateThuHoiXLRR(unitId, date),
                CalculateThuDichVu(unitId, date),
                CalculateLoiNhuan(unitId, date)
            };

            await Task.WhenAll(tasks);

            _logger.LogInformation("Hoàn thành tính toán tất cả chỉ tiêu cho unit {UnitId}", unitId);
        }

        /// <summary>
        /// Lấy lịch sử tính toán
        /// </summary>
        public async Task<List<DashboardCalculation>> GetCalculationHistory(int unitId, string? indicatorCode = null, int days = 30)
        {
            var fromDate = DateTime.Now.AddDays(-days);

            var query = _context.DashboardCalculations
                .Include(c => c.DashboardIndicator)
                .Include(c => c.Unit)
                .Where(c => c.UnitId == unitId && c.CalculationDate >= fromDate);

            if (!string.IsNullOrEmpty(indicatorCode))
            {
                query = query.Where(c => c.DashboardIndicator!.Code == indicatorCode);
            }

            return await query
                .OrderByDescending(c => c.CalculationDate)
                .ThenByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // === HELPER METHODS ===

        /// <summary>
        /// Chuyển đổi Unit Code thành Branch Code
        /// </summary>
        private string GetBranchCode(string? unitCode)
        {
            return unitCode switch
            {
                "CnLaiChau" => "9999",      // Chi nhánh Lai Châu
                "CnTamDuong" => "7801",     // Chi nhánh Tam Đường
                "CnPhongTho" => "7802",     // Chi nhánh Phong Thổ
                "CnSinHo" => "7803",        // Chi nhánh Sin Hồ
                "CnMuongTe" => "7804",      // Chi nhánh Mường Tè
                "CnThanUyen" => "7805",     // Chi nhánh Than Uyên
                "CnThanhPho" => "7806",     // Chi nhánh Thành phố
                "CnTanUyen" => "7807",      // Chi nhánh Tân Uyên
                "CnNamNhun" => "7808",      // Chi nhánh Nậm Nhùn
                _ => "ALL"                  // Toàn tỉnh hoặc không xác định
            };
        }

        /// <summary>
        /// Lưu kết quả tính toán thành công
        /// </summary>
        private async Task SaveCalculation(string indicatorCode, int unitId, DateTime date, decimal value, object details, DateTime startTime)
        {
            var indicator = await _context.DashboardIndicators
                .FirstOrDefaultAsync(i => i.Code == indicatorCode);

            if (indicator == null)
            {
                _logger.LogWarning("Không tìm thấy indicator {IndicatorCode}", indicatorCode);
                return;
            }

            var calculation = await _context.DashboardCalculations
                .FirstOrDefaultAsync(c =>
                    c.DashboardIndicatorId == indicator.Id &&
                    c.UnitId == unitId &&
                    c.CalculationDate.Date == date.Date);

            var executionTime = DateTime.Now - startTime;

            if (calculation == null)
            {
                calculation = new DashboardCalculation
                {
                    DashboardIndicatorId = indicator.Id,
                    UnitId = unitId,
                    CalculationDate = date,
                    CreatedBy = "System"
                };
                _context.DashboardCalculations.Add(calculation);
            }

            calculation.ActualValue = value;
            calculation.CalculationDetails = JsonSerializer.Serialize(details, new JsonSerializerOptions { WriteIndented = true });
            calculation.DataDate = date;
            calculation.DataSource = GetDataSource(indicatorCode);
            calculation.Status = "Success";
            calculation.ExecutionTime = executionTime;
            calculation.ErrorMessage = null;

            await _context.SaveChangesAsync();
            _logger.LogDebug("Đã lưu kết quả tính toán {IndicatorCode} = {Value} (thời gian: {ExecutionTime}ms)",
                indicatorCode, value, executionTime.TotalMilliseconds);
        }

        /// <summary>
        /// Lưu lỗi tính toán
        /// </summary>
        private async Task SaveCalculationError(string indicatorCode, int unitId, DateTime date, string errorMessage, DateTime startTime)
        {
            var indicator = await _context.DashboardIndicators
                .FirstOrDefaultAsync(i => i.Code == indicatorCode);

            if (indicator == null) return;

            var calculation = await _context.DashboardCalculations
                .FirstOrDefaultAsync(c =>
                    c.DashboardIndicatorId == indicator.Id &&
                    c.UnitId == unitId &&
                    c.CalculationDate.Date == date.Date);

            var executionTime = DateTime.Now - startTime;

            if (calculation == null)
            {
                calculation = new DashboardCalculation
                {
                    DashboardIndicatorId = indicator.Id,
                    UnitId = unitId,
                    CalculationDate = date,
                    CreatedBy = "System"
                };
                _context.DashboardCalculations.Add(calculation);
            }

            calculation.ActualValue = 0;
            calculation.Status = "Failed";
            calculation.ErrorMessage = errorMessage;
            calculation.ExecutionTime = executionTime;
            calculation.DataSource = GetDataSource(indicatorCode);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Xác định nguồn dữ liệu cho từng chỉ tiêu
        /// </summary>
        private string GetDataSource(string indicatorCode)
        {
            return indicatorCode switch
            {
                "HuyDong" => "DP01",
                "DuNo" or "TyLeNoXau" => "LN01",
                "LoiNhuan" => "GLCB41",
                "ThuHoiXLRR" or "ThuDichVu" => "TBD", // To Be Determined
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Tính toán một chỉ tiêu cụ thể
        /// </summary>
        public async Task<DashboardCalculation?> CalculateIndicatorAsync(
            int indicatorId,
            int? unitId,
            int year,
            int? quarter,
            int? month,
            string userId)
        {
            try
            {
                var indicator = await _context.DashboardIndicators
                    .FirstOrDefaultAsync(i => i.Id == indicatorId);

                if (indicator == null)
                {
                    _logger.LogWarning("Indicator {IndicatorId} not found", indicatorId);
                    return null;
                }

                // Create calculation date from year/quarter/month
                var calculationDate = new DateTime(year, month ?? (quarter * 3) ?? 12, 1);

                decimal value = 0;
                string calculationDetails = "";

                // Calculate based on indicator code
                switch (indicator.Code)
                {
                    case "HuyDong":
                        value = await CalculateNguonVon(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu DP01 - Tổng CURRENT_BALANCE";
                        break;
                    case "DuNo":
                        value = await CalculateDuNo(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu LN01 - Tổng DisbursementAmount";
                        break;
                    case "TyLeNoXau":
                        value = await CalculateTyLeNoXau(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu LN01 - Tỷ lệ nợ nhóm 3,4,5";
                        break;
                    case "ThuHoiXLRR":
                        value = await CalculateThuHoiXLRR(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu GLCB41 - Thu hồi nợ xử lý rủi ro";
                        break;
                    case "ThuDichVu":
                        value = await CalculateThuDichVu(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu GLCB41 - Doanh thu dịch vụ";
                        break;
                    case "LoiNhuan":
                        value = await CalculateLoiNhuan(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu GLCB41 - Lợi nhuận trước thuế";
                        break;
                    default:
                        _logger.LogWarning("Unknown indicator code: {Code}", indicator.Code);
                        return null;
                }

                // Create calculation record
                var calculation = new DashboardCalculation
                {
                    DashboardIndicatorId = indicatorId,
                    UnitId = unitId ?? 0,
                    Year = year,
                    Quarter = quarter,
                    Month = month,
                    CalculationDate = DateTime.UtcNow,
                    ActualValue = value,
                    CalculationDetails = calculationDetails,
                    DataSource = GetDataSourceForIndicator(indicator.Code),
                    DataDate = calculationDate,
                    Status = "Completed",
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.DashboardCalculations.Add(calculation);
                await _context.SaveChangesAsync();

                return calculation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating indicator {IndicatorId}", indicatorId);

                // Create failed calculation record
                var failedCalculation = new DashboardCalculation
                {
                    DashboardIndicatorId = indicatorId,
                    UnitId = unitId ?? 0,
                    Year = year,
                    Quarter = quarter,
                    Month = month,
                    CalculationDate = DateTime.UtcNow,
                    Status = "Failed",
                    ErrorMessage = ex.Message,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.DashboardCalculations.Add(failedCalculation);
                await _context.SaveChangesAsync();

                return failedCalculation;
            }
        }

        /// <summary>
        /// Sinh dữ liệu mẫu cho demo
        /// </summary>
        public async Task GenerateSampleDataAsync(int? unitId, int year, int? quarter, int? month, string userId)
        {
            var indicators = await _context.DashboardIndicators
                .Where(i => !i.IsDeleted)
                .ToListAsync();

            foreach (var indicator in indicators)
            {
                await CalculateIndicatorAsync(indicator.Id, unitId, year, quarter, month, userId);
            }
        }

        private string GetDataSourceForIndicator(string code)
        {
            return code switch
            {
                "HuyDong" => "DP01",
                "DuNo" => "LN01",
                "TyLeNoXau" => "LN01",
                "ThuHoiXLRR" => "GLCB41",
                "ThuDichVu" => "GLCB41",
                "LoiNhuan" => "GLCB41",
                _ => "Unknown"
            };
        }

        // === SAMPLE DATA GENERATORS (Replace with real data queries) ===

        private List<dynamic> GenerateSampleNguonVonData(string branchCode, DateTime date)
        {
            var random = new Random(date.DayOfYear);
            var accounts = new[] { "421001", "421002", "423001", "423002", "111001", "112001", "211001", "401001", "411001", "427001" };

            return accounts.Select(acc => new
            {
                AccountCode = acc,
                CurrentBalance = random.Next(100_000_000, 2_000_000_000) // 100M - 2B VND
            }).Cast<dynamic>().ToList();
        }

        private List<dynamic> GenerateSampleDuNoData(string branchCode, DateTime date)
        {
            var random = new Random(date.DayOfYear);
            var groups = new[] { "01", "02", "03", "04", "05" };

            return Enumerable.Range(1, 50).Select(i => new
            {
                DisbursementAmount = random.Next(50_000_000, 500_000_000), // 50M - 500M VND
                NhomNo = groups[random.Next(groups.Length)]
            }).Cast<dynamic>().ToList();
        }

        private List<dynamic> GenerateSampleGLCB41Data(string branchCode, DateTime date)
        {
            var random = new Random(date.DayOfYear);
            var accounts = new[] { "7", "790001", "8511", "8", "882" };

            return accounts.SelectMany(acc =>
                Enumerable.Range(1, 10).Select(i => new
                {
                    AccountCode = acc + (i.ToString().PadLeft(3, '0')),
                    DebitAmount = random.Next(10_000_000, 100_000_000),  // 10M - 100M VND
                    CreditAmount = random.Next(10_000_000, 100_000_000)  // 10M - 100M VND
                })
            ).Cast<dynamic>().ToList();
        }
    }
}
