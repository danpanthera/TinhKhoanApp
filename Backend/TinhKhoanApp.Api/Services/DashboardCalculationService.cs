using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Dashboard;
using TinhKhoanApp.Api.Utils;
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

                // Xác định ngày cụ thể cần tìm file dựa trên chỉ tiêu lũy kế
                var targetStatementDate = GetTargetStatementDate(date);

                _logger.LogInformation("Tìm file DP01 có StatementDate = {TargetDate}", targetStatementDate.Value.ToString("yyyy-MM-dd"));

                // Tìm file import DP01 có StatementDate chính xác và đúng mã chi nhánh
                var allRecordsForDate = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date == targetStatementDate.Value.Date &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("DP01") || r.Category.Contains("Nguồn vốn")))
                    .OrderByDescending(r => r.ImportDate)
                    .ToListAsync();

                // Ưu tiên tìm file có mã chi nhánh đúng trong tên file
                var latestImportRecord = allRecordsForDate
                    .FirstOrDefault(r => !string.IsNullOrEmpty(r.FileName) &&
                                        r.FileName.StartsWith($"{branchCode}_dp01_"));

                // Nếu không tìm thấy file đúng mã chi nhánh, lấy file đầu tiên
                if (latestImportRecord == null)
                {
                    latestImportRecord = allRecordsForDate.FirstOrDefault();
                }

                if (latestImportRecord == null)
                {
                    var errorMessage = $"Không tìm thấy file import DP01 cho ngày {targetStatementDate:yyyy-MM-dd}. Vui lòng kiểm tra dữ liệu đã được import chưa.";
                    _logger.LogWarning(errorMessage);

                    await SaveCalculationError("NguonVon", unitId, date, errorMessage, startTime);
                    throw new InvalidOperationException(errorMessage);
                }

                // Lấy dữ liệu chi tiết từ bảng DP01 mới nhất
                var dp01Data = await _context.DP01
                    .Where(i => i.DataSource == latestImportRecord.FileName)
                    .ToListAsync();

                decimal totalNguonVon = 0;
                var excludedPrefixes = new[] { "2", "40", "41", "427" };
                var processedRecords = 0;

                foreach (var dp01Record in dp01Data)
                {
                    try
                    {
                        // Lấy thông tin tài khoản hạch toán từ DP01
                        string accountCode = dp01Record.TAI_KHOAN_HACH_TOAN ?? "";

                        if (string.IsNullOrEmpty(accountCode)) continue;

                        // Kiểm tra chi nhánh
                        string fileBranchCode = dp01Record.MA_CN ?? "";

                        // Kiểm tra có thuộc chi nhánh đang tính không
                        var belongsToBranch = fileBranchCode == branchCode;

                        if (!belongsToBranch) continue;

                        // Chỉ tính các tài khoản không thuộc danh sách loại trừ
                        if (!excludedPrefixes.Any(prefix => accountCode.StartsWith(prefix)))
                        {
                            var balance = dp01Record.CURRENT_BALANCE ?? 0;
                            totalNguonVon += balance;
                            processedRecords++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Lỗi xử lý dữ liệu DP01: {Error}", ex.Message);
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
        /// <summary>
        /// Tính toán Dư nợ cho vay từ dữ liệu LN01
        /// Công thức: Tổng DISBURSEMENT_AMOUNT theo chi nhánh từ file LN01 chính xác theo ngày
        /// </summary>
        public async Task<decimal> CalculateDuNo(int unitId, DateTime date)
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
                _logger.LogInformation("Tính toán Dư nợ cho {UnitName} (Code: {BranchCode}) ngày {Date}",
                    unit.Name, branchCode, date.ToString("yyyy-MM-dd"));

                // Xác định ngày cụ thể cần tìm file LN01 dựa trên chỉ tiêu lũy kế
                var targetStatementDate = GetTargetStatementDate(date);

                _logger.LogInformation("Tìm file LN01 có StatementDate = {TargetDate}", targetStatementDate.Value.ToString("yyyy-MM-dd"));

                // Tìm file import LN01 có StatementDate chính xác
                var latestImportRecord = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date == targetStatementDate.Value.Date &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("LN01") || r.Category.Contains("Dư nợ")))
                    .OrderByDescending(r => r.ImportDate) // Sắp xếp theo ngày import nếu có nhiều file cùng ngày
                    .FirstOrDefaultAsync();

                if (latestImportRecord == null)
                {
                    var errorMessage = $"Không tìm thấy file import LN01 cho ngày {targetStatementDate:yyyy-MM-dd}. Vui lòng kiểm tra dữ liệu đã được import chưa.";
                    _logger.LogWarning(errorMessage);

                    await SaveCalculationError("DuNo", unitId, date, errorMessage, startTime);
                    throw new InvalidOperationException(errorMessage);
                }

                // Lấy dữ liệu chi tiết từ bảng LN01 mới nhất
                var ln01Data = await _context.LN01s
                    .Where(i => i.NGAY_DL.HasValue && i.NGAY_DL.Value.Date == date.Date)
                    .ToListAsync();

                decimal totalDisbursement = 0;
                var processedRecords = 0;
                var nhomNoBreakdown = new Dictionary<string, decimal>();

                foreach (var lnRecord in ln01Data)
                {
                    try
                    {
                        // Kiểm tra có thuộc chi nhánh đang tính không
                        var belongsToBranch = lnRecord.BRCD == branchCode;

                        if (!belongsToBranch) continue;

                        // Lấy số tiền giải ngân từ LN01
                        var disbursement = lnRecord.DISBURSEMENT_AMOUNT ?? 0M;
                        if (disbursement > 0)
                        {
                            totalDisbursement += disbursement;
                            processedRecords++;

                            // Phân tích theo loại hình cho vay (thay vì nhóm nợ)
                            var loaiHinh = lnRecord.LOAN_TYPE ?? "01"; // Default

                            if (!nhomNoBreakdown.ContainsKey(loaiHinh))
                                nhomNoBreakdown[loaiHinh] = 0;
                            nhomNoBreakdown[loaiHinh] += disbursement;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Lỗi xử lý dữ liệu LN01: {Error}", ex.Message);
                        continue;
                    }
                }

                var finalValue = totalDisbursement / 1_000_000m; // Chuyển sang triệu VND

                var calculationDetails = new
                {
                    Formula = "Tổng DISBURSEMENT_AMOUNT từ file LN01 chính xác theo ngày",
                    SourceFile = latestImportRecord.FileName,
                    StatementDate = latestImportRecord.StatementDate,
                    ImportDate = latestImportRecord.ImportDate,
                    TotalDisbursement = totalDisbursement,
                    FinalValue = finalValue,
                    Unit = "Triệu VND",
                    ProcessedRecords = processedRecords,
                    NhomNoBreakdown = nhomNoBreakdown,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("DuNo", unitId, date, finalValue, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Dư nợ từ file thực: {Value} triệu VND (từ {Records} bản ghi)",
                    finalValue, processedRecords);
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
        /// Công thức: (Nợ nhóm 3,4,5 / Tổng dư nợ) * 100 từ file LN01 chính xác theo ngày
        /// </summary>
        public async Task<decimal> CalculateTyLeNoXau(int unitId, DateTime date)
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
                _logger.LogInformation("Tính toán Tỷ lệ nợ xấu cho {UnitName} (Code: {BranchCode}) ngày {Date}",
                    unit.Name, branchCode, date.ToString("yyyy-MM-dd"));

                // Xác định ngày cụ thể cần tìm file LN01 dựa trên chỉ tiêu lũy kế
                var targetStatementDate = GetTargetStatementDate(date);

                _logger.LogInformation("Tìm file LN01 có StatementDate = {TargetDate}", targetStatementDate.Value.ToString("yyyy-MM-dd"));

                // Tìm file import LN01 có StatementDate chính xác (sử dụng lại file đã có từ CalculateDuNo)
                var latestImportRecord = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date == targetStatementDate.Value.Date &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("LN01") || r.Category.Contains("Dư nợ")))
                    .OrderByDescending(r => r.ImportDate)
                    .FirstOrDefaultAsync();

                if (latestImportRecord == null)
                {
                    var errorMessage = $"Không tìm thấy file import LN01 cho ngày {targetStatementDate:yyyy-MM-dd}. Vui lòng kiểm tra dữ liệu đã được import chưa.";
                    _logger.LogWarning(errorMessage);

                    await SaveCalculationError("TyLeNoXau", unitId, date, errorMessage, startTime);
                    throw new InvalidOperationException(errorMessage);
                }

                // Lấy dữ liệu chi tiết từ bảng LN01 mới nhất
                var ln01Data = await _context.LN01s
                    .Where(i => i.NGAY_DL.HasValue && i.NGAY_DL.Value.Date == date.Date)
                    .ToListAsync();

                decimal totalDebt = 0;
                decimal badDebt = 0;
                var processedRecords = 0;
                var nhomNoBreakdown = new Dictionary<string, decimal>();
                var badDebtGroups = new[] { "03", "04", "05" };

                foreach (var lnRecord in ln01Data)
                {
                    try
                    {
                        // Kiểm tra có thuộc chi nhánh đang tính không
                        var belongsToBranch = lnRecord.BRCD == branchCode;

                        if (!belongsToBranch) continue;

                        // Lấy số tiền dư nợ từ LN01
                        var debtAmount = lnRecord.DU_NO ?? 0M;
                        if (debtAmount > 0)
                        {
                            totalDebt += debtAmount;

                            // Sử dụng nhóm nợ để phân loại nợ xấu
                            var nhomNo = lnRecord.NHOM_NO ?? "1"; // Default

                            if (!nhomNoBreakdown.ContainsKey(nhomNo))
                                nhomNoBreakdown[nhomNo] = 0;
                            nhomNoBreakdown[nhomNo] += debtAmount;

                            // Tính nợ xấu (dựa trên nhóm nợ)
                            // Giả định các nhóm nợ xấu: "03", "04", "05"
                            var isBadDebt = badDebtGroups.Contains(nhomNo);

                            if (isBadDebt)
                            {
                                badDebt += debtAmount;
                            }

                            processedRecords++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Lỗi xử lý dữ liệu LN01: {Error}", ex.Message);
                        continue;
                    }
                }

                var ratio = totalDebt > 0 ? (badDebt / totalDebt * 100) : 0;

                var calculationDetails = new
                {
                    Formula = "(Nợ nhóm 3,4,5 / Tổng dư nợ) * 100 từ file LN01 chính xác theo ngày",
                    SourceFile = latestImportRecord.FileName,
                    StatementDate = latestImportRecord.StatementDate,
                    ImportDate = latestImportRecord.ImportDate,
                    TotalDebt = totalDebt,
                    BadDebt = badDebt,
                    BadDebtGroups = badDebtGroups,
                    Ratio = ratio,
                    Unit = "%",
                    ProcessedRecords = processedRecords,
                    NhomNoBreakdown = nhomNoBreakdown,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("TyLeNoXau", unitId, date, ratio, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Tỷ lệ nợ xấu từ file thực: {Value}% (từ {Records} bản ghi)",
                    ratio, processedRecords);
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
        /// <summary>
        /// Tính toán Thu hồi XLRR từ dữ liệu ThuXLRR
        /// Công thức: Tổng số tiền thu hồi từ nợ đã xử lý rủi ro theo chi nhánh từ bảng ThuXLRR chính xác theo ngày
        /// </summary>
        public async Task<decimal> CalculateThuHoiXLRR(int unitId, DateTime date)
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
                _logger.LogInformation("Tính toán Thu hồi XLRR cho {UnitName} (Code: {BranchCode}) ngày {Date}",
                    unit.Name, branchCode, date.ToString("yyyy-MM-dd"));

                // Xác định ngày cụ thể cần tìm file dựa trên chỉ tiêu lũy kế
                var targetStatementDate = GetTargetStatementDate(date);

                _logger.LogInformation("Tìm file ThuXLRR có StatementDate = {TargetDate}", targetStatementDate.Value.ToString("yyyy-MM-dd"));

                // Tìm file import ThuXLRR có StatementDate chính xác
                var latestImportRecord = await _context.ImportedDataRecords
                    .Where(r => r.StatementDate.HasValue &&
                               r.StatementDate.Value.Date == targetStatementDate.Value.Date &&
                               r.Status == "Completed" &&
                               (r.Category.Contains("ThuXLRR") || r.Category.Contains("Thu hồi XLRR")))
                    .OrderByDescending(r => r.ImportDate)
                    .FirstOrDefaultAsync();

                if (latestImportRecord == null)
                {
                    var errorMessage = $"Không tìm thấy file import ThuXLRR cho ngày {targetStatementDate:yyyy-MM-dd}. Vui lòng kiểm tra dữ liệu đã được import chưa.";
                    _logger.LogWarning(errorMessage);

                    await SaveCalculationError("ThuHoiXLRR", unitId, date, errorMessage, startTime);
                    throw new InvalidOperationException(errorMessage);
                }

                // Lấy dữ liệu chi tiết từ bảng ThuXLRR
                var importedItems = await _context.ThuXLRR
                    .Where(i => i.ImportedDataRecordId == latestImportRecord.Id)
                    .Select(i => i.RawData)
                    .ToListAsync();

                decimal totalThuHoiXLRR = 0;
                var processedRecords = 0;

                foreach (var rawDataJson in importedItems)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(rawDataJson)) continue;

                        var jsonDoc = JsonDocument.Parse(rawDataJson);
                        var root = jsonDoc.RootElement;

                        // Kiểm tra có thuộc chi nhánh đang tính không
                        var belongsToBranch = root.TryGetProperty("BRANCH_CODE", out var branchElement) &&
                                            branchElement.GetString() == branchCode;

                        if (!belongsToBranch) continue;

                        // Tìm trường thu hồi XLRR - có thể là RECOVERED_AMOUNT hoặc tương tự
                        if (root.TryGetProperty("RECOVERED_AMOUNT", out var recoveredElement) ||
                            root.TryGetProperty("RecoveredAmount", out recoveredElement) ||
                            root.TryGetProperty("recovered_amount", out recoveredElement) ||
                            root.TryGetProperty("SO_TIEN_THU_HOI", out recoveredElement))
                        {
                            if (decimal.TryParse(recoveredElement.GetString(), out var recoveredAmount))
                            {
                                totalThuHoiXLRR += recoveredAmount;
                                processedRecords++;
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning("Lỗi parse JSON dữ liệu import: {Error}", ex.Message);
                        continue;
                    }
                }

                var finalValue = totalThuHoiXLRR / 1_000_000m; // Chuyển sang triệu VND

                var calculationDetails = new
                {
                    Formula = "Tổng RECOVERED_AMOUNT từ bảng ThuXLRR mới nhất theo chi nhánh",
                    SourceFile = latestImportRecord.FileName,
                    StatementDate = latestImportRecord.StatementDate,
                    ImportDate = latestImportRecord.ImportDate,
                    TotalThuHoiXLRR = totalThuHoiXLRR,
                    FinalValue = finalValue,
                    Unit = "Triệu VND",
                    ProcessedRecords = processedRecords,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("ThuHoiXLRR", unitId, date, finalValue, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Thu hồi XLRR từ bảng ThuXLRR: {Value} triệu VND (từ {Records} bản ghi)",
                    finalValue, processedRecords);
                return finalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Thu hồi XLRR cho unit {UnitId}", unitId);
                await SaveCalculationError("ThuHoiXLRR", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Thu dịch vụ từ dữ liệu GL41
        /// Công thức: Tổng thu nhập từ các tài khoản dịch vụ theo chi nhánh từ file GL41 chính xác theo ngày
        /// </summary>
        public async Task<decimal> CalculateThuDichVu(int unitId, DateTime date)
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
                _logger.LogInformation("Tính toán Thu dịch vụ cho {UnitName} (Code: {BranchCode}) ngày {Date}",
                    unit.Name, branchCode, date.ToString("yyyy-MM-dd"));

                // Xác định ngày cụ thể cần tìm dữ liệu dựa trên chỉ tiêu lũy kế
                var targetStatementDate = GetTargetStatementDate(date);

                _logger.LogInformation("Tìm dữ liệu GL41 có NGAY_DL = {TargetDate}", targetStatementDate.Value.ToString("yyyy-MM-dd"));

                // Lấy dữ liệu GL41 trực tiếp từ bảng với NGAY_DL = targetStatementDate và MA_CN
                var targetDate = targetStatementDate.Value;
                var gl41Records = await _context.GL41s
                    .Where(g => g.NGAY_DL.Date == targetDate.Date && g.MA_CN == branchCode)
                    .ToListAsync();

                if (!gl41Records.Any())
                {
                    var errorMessage = $"Không tìm thấy dữ liệu GL41 cho chi nhánh {branchCode} ngày {targetStatementDate:dd/MM/yyyy}";
                    _logger.LogWarning(errorMessage);
                    await SaveCalculationError("ThuDichVu", unitId, date, errorMessage, startTime);
                    throw new InvalidOperationException(errorMessage);
                }

                decimal totalServiceRevenue = 0;
                var processedRecords = 0;

                // Các tài khoản thu dịch vụ (có thể cần điều chỉnh theo thực tế)
                var serviceRevenueAccounts = new[] { "7111", "7112", "7113", "7114", "7115", "7121", "7122" };

                foreach (var record in gl41Records)
                {
                    var accountCode = record.MA_TK ?? "";

                    // Chỉ tính các tài khoản thu dịch vụ
                    if (serviceRevenueAccounts.Any(acc => accountCode.StartsWith(acc)))
                    {
                        // Thu dịch vụ = Credit - Debit
                        var credit = record.ST_GHICO ?? 0;
                        var debit = record.ST_GHINO ?? 0;
                        var serviceAmount = credit - debit;
                        totalServiceRevenue += serviceAmount;
                        processedRecords++;
                    }
                }

                var finalValue = totalServiceRevenue / 1_000_000m; // Chuyển sang triệu VND

                var calculationDetails = new
                {
                    Formula = "Tổng (Credit - Debit) từ các tài khoản thu dịch vụ trong bảng GL41",
                    DataSource = "GL41 table",
                    StatementDate = targetStatementDate,
                    TotalServiceRevenue = totalServiceRevenue,
                    FinalValue = finalValue,
                    Unit = "Triệu VND",
                    ProcessedRecords = processedRecords,
                    ServiceRevenueAccounts = serviceRevenueAccounts,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("ThuDichVu", unitId, date, finalValue, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Thu dịch vụ: {Value} triệu VND (từ {Records} bản ghi)",
                    finalValue, processedRecords);
                return finalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán Thu dịch vụ cho unit {UnitId}", unitId);
                await SaveCalculationError("ThuDichVu", unitId, date, ex.Message, startTime);
                return 0;
            }
        }

        /// <summary>
        /// Tính toán Lợi nhuận tài chính từ dữ liệu GL41 thực tế
        /// Công thức: (Tài khoản 7 + 790001 + 8511) - (Tài khoản 8 + 882) từ file GL41 chính xác theo ngày
        /// </summary>
        public async Task<decimal> CalculateLoiNhuan(int unitId, DateTime date)
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
                _logger.LogInformation("Tính toán Lợi nhuận cho {UnitName} (Code: {BranchCode}) ngày {Date}",
                    unit.Name, branchCode, date.ToString("yyyy-MM-dd"));

                // Xác định ngày cụ thể cần tìm file dựa trên chỉ tiêu lũy kế
                var targetStatementDate = GetTargetStatementDate(date);

                _logger.LogInformation("Tìm dữ liệu GL41 có NGAY_DL = {TargetDate}", targetStatementDate.Value.ToString("yyyy-MM-dd"));

                // Lấy dữ liệu GL41 trực tiếp từ bảng với NGAY_DL = targetStatementDate và MA_CN (mã chi nhánh)
                var targetDate2 = targetStatementDate.Value;
                var gl41Records = await _context.GL41s
                    .Where(g => g.NGAY_DL.Date == targetDate2.Date && g.MA_CN == branchCode)
                    .ToListAsync();

                if (!gl41Records.Any())
                {
                    var errorMessage = $"Không tìm thấy dữ liệu GL41 cho chi nhánh {branchCode} ngày {targetStatementDate:dd/MM/yyyy}. Vui lòng kiểm tra dữ liệu đã được import chưa.";
                    _logger.LogWarning(errorMessage);
                    await SaveCalculationError("LoiNhuan", unitId, date, errorMessage, startTime);
                    throw new InvalidOperationException(errorMessage);
                }

                decimal totalRevenue = 0; // Thu nhập
                decimal totalExpense = 0; // Chi phí
                var processedRecords = 0;

                // Thu nhập (7 + 790001 + 8511)
                var revenueAccounts = new[] { "7", "790001", "8511" };
                // Chi phí (8 + 882)
                var expenseAccounts = new[] { "8", "882" };

                foreach (var record in gl41Records)
                {
                    var accountCode = record.MA_TK ?? "";

                    // Tính thu nhập từ các tài khoản thu
                    if (revenueAccounts.Any(acc => accountCode.StartsWith(acc)))
                    {
                        var revCredit = record.ST_GHICO ?? 0;
                        var revDebit = record.ST_GHINO ?? 0;
                        totalRevenue += revCredit - revDebit; // Credit - Debit cho tài khoản thu
                    }

                    // Tính chi phí từ các tài khoản chi
                    if (expenseAccounts.Any(acc => accountCode.StartsWith(acc)))
                    {
                        var expDebit = record.ST_GHINO ?? 0;
                        var expCredit = record.ST_GHICO ?? 0;
                        totalExpense += expDebit - expCredit; // Debit - Credit cho tài khoản chi
                    }

                    processedRecords++;
                }

                var profit = totalRevenue - totalExpense;
                var finalValue = profit / 1_000_000m; // Chuyển sang triệu VND

                var calculationDetails = new
                {
                    Formula = "(TK 7+790001+8511) - (TK 8+882) từ bảng GL41",
                    DataSource = "GL41 table",
                    StatementDate = targetStatementDate,
                    TotalRevenue = totalRevenue,
                    TotalExpense = totalExpense,
                    Profit = profit,
                    FinalValue = finalValue,
                    Unit = "Triệu VND",
                    ProcessedRecords = processedRecords,
                    RevenueAccounts = revenueAccounts,
                    ExpenseAccounts = expenseAccounts,
                    CalculationDate = date,
                    UnitInfo = new { unit.Code, unit.Name },
                    BranchCode = branchCode
                };

                await SaveCalculation("LoiNhuan", unitId, date, finalValue, calculationDetails, startTime);

                _logger.LogInformation("Hoàn thành tính Lợi nhuận từ file thực: {Value} triệu VND (từ {Records} bản ghi)",
                    finalValue, processedRecords);
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
                "HoiSo" => "7800",          // Hội Sở
                "CnLaiChau" => "9999",      // Chi nhánh Lai Châu
                // Tên cũ (để backward compatibility)
                "CnTamDuong" => "7801",     // Chi nhánh Bình Lư (tên cũ)
                "CnPhongTho" => "7802",     // Chi nhánh Phong Thổ
                "CnSinHo" => "7803",        // Chi nhánh Sin Hồ
                "CnMuongTe" => "7804",      // Chi nhánh Bum Tở (tên cũ)
                "CnThanUyen" => "7805",     // Chi nhánh Than Uyên
                "CnThanhPho" => "7806",     // Chi nhánh Đoàn Kết (tên cũ)
                "CnTanUyen" => "7807",      // Chi nhánh Tân Uyên
                "CnNamNhun" => "7808",      // Chi nhánh Nậm Hàng (tên cũ)
                // Tên mới theo quy ước anh
                "CnBinhLu" => "7801",       // Chi nhánh Bình Lư
                "CnBumTo" => "7804",        // Chi nhánh Bum Tở
                "CnDoanKet" => "7806",      // Chi nhánh Đoàn Kết
                "CnNamHang" => "7808",      // Chi nhánh Nậm Hàng
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
                "HuyDong" or "NguonVon" => "DP01",
                "DuNo" or "TyLeNoXau" => "LN01",
                "ThuHoiXLRR" => "ThuXLRR",
                "ThuDichVu" or "LoiNhuan" => "GL41",
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
                        calculationDetails = "Tính từ dữ liệu GL41 - Thu hồi nợ xử lý rủi ro";
                        break;
                    case "ThuDichVu":
                        value = await CalculateThuDichVu(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu GL41 - Doanh thu dịch vụ";
                        break;
                    case "LoiNhuan":
                        value = await CalculateLoiNhuan(unitId ?? 0, calculationDate);
                        calculationDetails = "Tính từ dữ liệu GL41 - Lợi nhuận trước thuế";
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
                "ThuHoiXLRR" => "ThuXLRR",
                "ThuDichVu" => "GL41",
                "LoiNhuan" => "GL41",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Helper method: Xác định ngày Statement cụ thể dựa trên kỳ được chọn
        /// Quy ước: Nếu chọn năm -> ngày cuối năm, nếu chọn tháng -> ngày cuối tháng, nếu chọn ngày cụ thể -> ngày đó
        /// </summary>
        private DateTime? GetTargetStatementDate(DateTime selectedDate)
        {
            // Kiểm tra các loại kỳ:

            // 1. Nếu chọn cả tháng và ngày cụ thể (ngày != 1) -> lấy chính ngày đó
            if (selectedDate.Day > 1)
            {
                return selectedDate.Date;
            }

            // 2. Nếu chọn tháng (ngày = 1) -> lấy ngày cuối tháng
            if (selectedDate.Month > 0 && selectedDate.Day == 1)
            {
                return new DateTime(selectedDate.Year, selectedDate.Month, DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month));
            }

            // 3. Nếu chọn năm (không chỉ định tháng) -> lấy ngày cuối năm (31/12)
            return new DateTime(selectedDate.Year, 12, 31);
        }

        /// <summary>
        /// Helper method: Tạo thông báo lỗi cho các tháng/năm chưa có dữ liệu
        /// </summary>
        private string GetDataNotAvailableMessage(DateTime selectedDate)
        {
            var targetDate = GetTargetStatementDate(selectedDate);
            return $"Chưa có dữ liệu file cho ngày {targetDate:yyyy-MM-dd} (tương ứng với kỳ {selectedDate.Month}/{selectedDate.Year}). " +
                   "Vui lòng kiểm tra xem file dữ liệu đã được import chưa hoặc chọn kỳ khác.";
        }

    }
}
