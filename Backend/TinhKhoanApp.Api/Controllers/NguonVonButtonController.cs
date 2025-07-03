using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.NguonVon;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller tính toán Nguồn vốn theo yêu cầu mới của anh
    /// Hỗ trợ tính cho Hội Sở, các Chi nhánh, PGD và Toàn tỉnh
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NguonVonButtonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguonVonButtonController> _logger;

        // Mapping theo yêu cầu của anh
        private readonly Dictionary<string, BranchInfo> _branchMapping = new Dictionary<string, BranchInfo>
        {
            // Hội Sở và các Chi nhánh chính
            { "HoiSo", new BranchInfo("7800", null, "Hội Sở") },
            { "CnBinhLu", new BranchInfo("7801", null, "CN Bình Lư") },
            { "CnPhongTho", new BranchInfo("7802", null, "CN Phong Thổ") },
            { "CnSinHo", new BranchInfo("7803", null, "CN Sìn Hồ") },
            { "CnBumTo", new BranchInfo("7804", null, "CN Bum Tở") },
            { "CnThanUyen", new BranchInfo("7805", null, "CN Than Uyên") },
            { "CnDoanKet", new BranchInfo("7806", null, "CN Đoàn Kết") },
            { "CnTanUyen", new BranchInfo("7807", null, "CN Tân Uyên") },
            { "CnNamHang", new BranchInfo("7808", null, "CN Nậm Hàng") },

            // Các PGD
            { "CnPhongTho-PGD5", new BranchInfo("7802", "01", "CN Phong Thổ - PGD Số 5") },
            { "CnThanUyen-PGD6", new BranchInfo("7805", "01", "CN Than Uyên - PGD Số 6") },
            { "CnDoanKet-PGD1", new BranchInfo("7806", "01", "CN Đoàn Kết - PGD Số 1") },
            { "CnDoanKet-PGD2", new BranchInfo("7806", "02", "CN Đoàn Kết - PGD Số 2") },
            { "CnTanUyen-PGD3", new BranchInfo("7807", "01", "CN Tân Uyên - PGD Số 3") },

            // Toàn tỉnh
            { "ToanTinh", new BranchInfo("ALL", null, "Toàn tỉnh") }
        };

        public NguonVonButtonController(ApplicationDbContext context, ILogger<NguonVonButtonController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tính Nguồn vốn theo button được ấn
        /// </summary>
        /// <param name="unitKey">Key đơn vị (HoiSo, CnBinhLu, CnPhongTho-PGD5, ToanTinh...)</param>
        /// <param name="targetDate">Ngày tính toán (optional, mặc định là ngày hiện tại)</param>
        [HttpPost("calculate/{unitKey}")]
        public async Task<IActionResult> CalculateNguonVon(string unitKey, [FromQuery] DateTime? targetDate = null)
        {
            try
            {
                if (!_branchMapping.ContainsKey(unitKey))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Không tìm thấy đơn vị '{unitKey}'. Các đơn vị hỗ trợ: {string.Join(", ", _branchMapping.Keys)}"
                    });
                }

                var branchInfo = _branchMapping[unitKey];
                var calculationDate = targetDate ?? DateTime.Now;

                _logger.LogInformation("💰 Bắt đầu tính Nguồn vốn cho {UnitName} ({UnitKey}) - MA_CN: {MaCN}, MA_PGD: {MaPGD}",
                    branchInfo.DisplayName, unitKey, branchInfo.MaCN, branchInfo.MaPGD ?? "ALL");

                decimal totalNguonVon;
                int recordCount;
                List<object> topAccounts;

                if (unitKey == "ToanTinh")
                {
                    // Tính toàn tỉnh (tổng từ 7800-7808)
                    var result = await CalculateAllProvince(calculationDate);
                    totalNguonVon = result.Total;
                    recordCount = result.RecordCount;
                    topAccounts = result.TopAccounts;
                }
                else
                {
                    // Tính cho đơn vị cụ thể
                    var result = await CalculateSingleUnit(branchInfo, calculationDate);
                    totalNguonVon = result.Total;
                    recordCount = result.RecordCount;
                    topAccounts = result.TopAccounts;
                }

                _logger.LogInformation("✅ Kết quả tính Nguồn vốn: {Total:N0} VND từ {Count} bản ghi", totalNguonVon, recordCount);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        unitKey = unitKey,
                        unitName = branchInfo.DisplayName,
                        maCN = branchInfo.MaCN,
                        maPGD = branchInfo.MaPGD,
                        totalNguonVon = totalNguonVon,
                        totalNguonVonTrieuVND = Math.Round(totalNguonVon / 1_000_000m, 2),
                        recordCount = recordCount,
                        calculationDate = calculationDate.ToString("dd/MM/yyyy"),
                        topAccounts = topAccounts.Take(10).ToList(), // Top 10 tài khoản
                        formula = "Tổng CURRENT_BALANCE - (loại trừ TK 40*, 41*, 427*, 211108)"
                    },
                    message = $"Tính toán thành công cho {branchInfo.DisplayName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tính Nguồn vốn cho {UnitKey}", unitKey);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi khi tính toán: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Tính Nguồn vốn cho toàn tỉnh (tổng từ 7800-7808)
        /// </summary>
        private async Task<CalculationResult> CalculateAllProvince(DateTime targetDate)
        {
            var allBranchCodes = new[] { "7800", "7801", "7802", "7803", "7804", "7805", "7806", "7807", "7808" };
            var dateString = targetDate.ToString("yyyyMMdd"); // VD: 20250430

            decimal totalNguonVon = 0;
            int totalRecordCount = 0;
            var allTopAccounts = new List<object>();

            // Tính tổng cho từng chi nhánh dựa trên FileName
            foreach (var maCN in allBranchCodes)
            {
                var fileNamePattern = $"{maCN}_dp01_{dateString}.csv"; // VD: 7801_dp01_20250430.csv
                _logger.LogInformation("📊 Đang tính cho file: {FileName}", fileNamePattern);

                // Query dữ liệu từ bảng DP01 với điều kiện FileName
                var query = _context.DP01s
                    .Where(d => d.FileName == fileNamePattern)
                    .Where(d =>
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                        !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                        d.TAI_KHOAN_HACH_TOAN != "211108"
                    );

                var branchTotal = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
                var branchCount = await query.CountAsync();

                totalNguonVon += branchTotal;
                totalRecordCount += branchCount;

                _logger.LogInformation("✅ Chi nhánh {MaCN}: {Total:N0} VND từ {Count} bản ghi",
                    maCN, branchTotal, branchCount);

                // Lấy top accounts của chi nhánh này
                if (branchCount > 0)
                {
                    var branchTopAccounts = await query
                        .OrderByDescending(d => Math.Abs(d.CURRENT_BALANCE ?? 0))
                        .Take(5)
                        .Select(d => new
                        {
                            MaCN = maCN,
                            AccountCode = d.TAI_KHOAN_HACH_TOAN,
                            AccountName = d.TEN_TAI_KHOAN,
                            TotalBalance = d.CURRENT_BALANCE ?? 0
                        })
                        .ToListAsync();

                    allTopAccounts.AddRange(branchTopAccounts.Cast<object>());
                }
            }

            // Sắp xếp lại top accounts theo số dư
            var topAccounts = allTopAccounts
                .Cast<dynamic>()
                .OrderByDescending(x => Math.Abs((decimal)x.TotalBalance))
                .Take(20)
                .Cast<object>()
                .ToList();

            _logger.LogInformation("🏆 Tổng nguồn vốn toàn tỉnh: {Total:N0} VND từ {Count} bản ghi",
                totalNguonVon, totalRecordCount);

            return new CalculationResult
            {
                Total = totalNguonVon,
                RecordCount = totalRecordCount,
                TopAccounts = topAccounts
            };
        }

        /// <summary>
        /// Tính Nguồn vốn cho một đơn vị cụ thể
        /// </summary>
        private async Task<CalculationResult> CalculateSingleUnit(BranchInfo branchInfo, DateTime targetDate)
        {
            _logger.LogInformation("� Tính nguồn vốn cho {UnitName} (MA_CN: {MaCN}) ngày {Date}",
                branchInfo.DisplayName, branchInfo.MaCN, targetDate.ToString("yyyy-MM-dd"));

            // Query dữ liệu từ bảng DP01 dựa trên MA_CN và DATA_DATE
            // Tương đương với việc tìm file {maCN}_dp01_{yyyyMMdd}.csv
            var query = _context.DP01s
                .Where(d => d.MA_CN == branchInfo.MaCN && d.DATA_DATE.Date == targetDate.Date);

            // Áp dụng điều kiện lọc tài khoản theo yêu cầu
            query = query.Where(d =>
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("40") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("41") &&
                !d.TAI_KHOAN_HACH_TOAN.StartsWith("427") &&
                d.TAI_KHOAN_HACH_TOAN != "211108"
            );

            // Nếu là PGD thì lọc thêm theo MA_PGD
            if (!string.IsNullOrEmpty(branchInfo.MaPGD))
            {
                query = query.Where(d => d.MA_PGD == branchInfo.MaPGD);
                _logger.LogInformation("🏪 Lọc thêm theo PGD: {MaPGD}", branchInfo.MaPGD);
            }

            var totalNguonVon = await query.SumAsync(d => d.CURRENT_BALANCE ?? 0);
            var recordCount = await query.CountAsync();

            // Lấy top 20 tài khoản có số dư lớn nhất để debug và báo cáo
            var topAccounts = await query
                .OrderByDescending(d => Math.Abs(d.CURRENT_BALANCE ?? 0))
                .Take(20)
                .Select(d => new
                {
                    AccountCode = d.TAI_KHOAN_HACH_TOAN,
                    AccountName = d.TEN_TAI_KHOAN,
                    TotalBalance = d.CURRENT_BALANCE ?? 0,
                    MaPGD = d.MA_PGD
                })
                .ToListAsync();

            _logger.LogInformation("✅ Kết quả {UnitName}: {Total:N0} VND từ {Count} bản ghi",
                branchInfo.DisplayName, totalNguonVon, recordCount);

            if (recordCount == 0)
            {
                _logger.LogWarning("⚠️ Không tìm thấy dữ liệu cho MA_CN: {MaCN}, ngày: {Date}", branchInfo.MaCN, targetDate.ToString("yyyy-MM-dd"));
            }

            return new CalculationResult
            {
                Total = totalNguonVon,
                RecordCount = recordCount,
                TopAccounts = topAccounts.Cast<object>().ToList()
            };
        }

        /// <summary>
        /// Lấy danh sách tất cả đơn vị hỗ trợ
        /// </summary>
        [HttpGet("units")]
        public IActionResult GetSupportedUnits()
        {
            var units = _branchMapping.Select(kvp => new
            {
                unitKey = kvp.Key,
                unitName = kvp.Value.DisplayName,
                maCN = kvp.Value.MaCN,
                maPGD = kvp.Value.MaPGD
            }).ToList();

            return Ok(new
            {
                success = true,
                data = units,
                message = "Danh sách các đơn vị hỗ trợ"
            });
        }

        /// <summary>
        /// Debug: Lấy danh sách các file DP01 có trong database
        /// </summary>
        [HttpGet("debug/files")]
        public async Task<IActionResult> GetDP01Files()
        {
            try
            {
                var files = await _context.DP01s
                    .Where(d => d.FileName != null)
                    .Select(d => d.FileName)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToListAsync();

                var fileInfo = files.Select(f => new
                {
                    FileName = f,
                    MaCN = f?.Length >= 4 ? f.Substring(0, 4) : "",
                    DatePart = f?.Length >= 18 ? f.Substring(10, 8) : "",
                    ParsedDate = f?.Length >= 18 ? ParseDateFromFileName(f.Substring(10, 8)) : null
                }).ToList();

                _logger.LogInformation("🔍 Tìm thấy {Count} file DP01 trong database", files.Count);

                return Ok(new
                {
                    success = true,
                    totalFiles = files.Count,
                    files = fileInfo,
                    message = "Danh sách các file DP01 trong database"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy danh sách file DP01");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Parse ngày từ tên file (yyyyMMdd -> DateTime)
        /// </summary>
        private DateTime? ParseDateFromFileName(string datePart)
        {
            try
            {
                if (datePart.Length == 8 && DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Test endpoint đơn giản - kiểm tra có dữ liệu DP01 hay không
        /// </summary>
        [HttpGet("test-dp01")]
        public async Task<IActionResult> TestDP01()
        {
            try
            {
                // Đếm tổng số record
                var totalCount = await _context.DP01s.CountAsync();

                // Đếm record có FileName
                var withFileNameCount = await _context.DP01s.CountAsync(d => d.FileName != null && d.FileName != "");

                // Lấy sample record đầu tiên
                var firstRecord = await _context.DP01s.FirstOrDefaultAsync();

                return Ok(new
                {
                    success = true,
                    totalRecords = totalCount,
                    recordsWithFileName = withFileNameCount,
                    sampleRecord = firstRecord != null ? new
                    {
                        firstRecord.Id,
                        firstRecord.MA_CN,
                        firstRecord.DATA_DATE,
                        firstRecord.TAI_KHOAN_HACH_TOAN,
                        firstRecord.CURRENT_BALANCE,
                        firstRecord.FileName
                    } : null,
                    message = $"Tổng {totalCount:N0} bản ghi DP01, {withFileNameCount:N0} có FileName"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi test DP01");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Tạm thời: Populate FileName cho dữ liệu DP01 dựa trên MA_CN và DATA_DATE
        /// </summary>
        [HttpPost("populate-filename")]
        public async Task<IActionResult> PopulateFileName()
        {
            try
            {
                _logger.LogInformation("🔄 Bắt đầu populate FileName cho dữ liệu DP01...");

                // Lấy dữ liệu DP01 chưa có FileName (batch nhỏ để tránh timeout)
                var recordsToUpdate = await _context.DP01s
                    .Where(d => d.FileName == null || d.FileName == "")
                    .Take(1000) // Chỉ xử lý 1000 records để test
                    .ToListAsync();

                _logger.LogInformation($"📊 Tìm thấy {recordsToUpdate.Count} bản ghi cần update FileName");

                int updateCount = 0;
                foreach (var record in recordsToUpdate)
                {
                    if (!string.IsNullOrEmpty(record.MA_CN))
                    {
                        // Tạo FileName từ MA_CN và DATA_DATE
                        // VD: 7801_dp01_20250430.csv
                        var dateString = record.DATA_DATE.ToString("yyyyMMdd");
                        record.FileName = $"{record.MA_CN}_dp01_{dateString}.csv";
                        updateCount++;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Đã update FileName cho {updateCount} bản ghi");

                return Ok(new
                {
                    success = true,
                    updatedRecords = updateCount,
                    totalProcessed = recordsToUpdate.Count,
                    message = $"Đã populate FileName cho {updateCount} bản ghi DP01"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi populate FileName");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Thông tin chi nhánh/PGD
    /// </summary>
    public class BranchInfo
    {
        public string MaCN { get; set; }
        public string? MaPGD { get; set; }
        public string DisplayName { get; set; }

        public BranchInfo(string maCN, string? maPGD, string displayName)
        {
            MaCN = maCN;
            MaPGD = maPGD;
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// Kết quả tính toán
    /// </summary>
    public class CalculationResult
    {
        public decimal Total { get; set; }
        public int RecordCount { get; set; }
        public List<object> TopAccounts { get; set; } = new List<object>();
    }
}
