using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.RawData;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Services;
using System.Text;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller xử lý dữ liệu bảng LN01 - Khoản vay cá nhân
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LN01Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILN01Service _ln01Service;
        private readonly ILogger<LN01Controller> _logger;

        public LN01Controller(ApplicationDbContext context, ILN01Service ln01Service, ILogger<LN01Controller> logger)
        {
            _context = context;
            _ln01Service = ln01Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả dữ liệu LN01 (Thông tin khoản vay) theo mô hình Repository
        /// </summary>
        [HttpGet("all-records")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetAllRecordsNew()
        {
            try
            {
                var result = await _ln01Service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả dữ liệu LN01 từ service");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo ID
        /// </summary>
        [HttpGet("id/{id}")]
        [Authorize]
        public async Task<ActionResult<LN01DTO>> GetByIdNew(long id)
        {
            try
            {
                var result = await _ln01Service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound($"Không tìm thấy bản ghi LN01 với ID: {id}");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 với ID: {ID}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 gần đây
        /// </summary>
        [HttpGet("recent-records")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetRecentRecords([FromQuery] int count = 10)
        {
            try
            {
                var result = await _ln01Service.GetRecentAsync(count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 gần đây");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo ngày
        /// </summary>
        [HttpGet("by-date-new")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetByDateNew([FromQuery] DateTime date, [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln01Service.GetByDateAsync(date, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 theo ngày: {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range-new")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetByDateRangeNew(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                }

                var result = await _ln01Service.GetByDateRangeAsync(fromDate, toDate, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 theo khoảng thời gian: {FromDate} - {ToDate}",
                    fromDate, toDate);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo mã chi nhánh
        /// </summary>
        [HttpGet("by-branch-new/{branchCode}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetByBranchCodeNew(
            string branchCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln01Service.GetByBranchCodeAsync(branchCode, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 theo mã chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo mã khách hàng
        /// </summary>
        [HttpGet("by-customer-new/{customerCode}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetByCustomerCodeNew(
            string customerCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln01Service.GetByCustomerCodeAsync(customerCode, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 theo mã khách hàng: {CustomerCode}", customerCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo số tài khoản
        /// </summary>
        [HttpGet("by-account-new/{accountNumber}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetByAccountNumberNew(
            string accountNumber,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln01Service.GetByAccountNumberAsync(accountNumber, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 theo số tài khoản: {AccountNumber}", accountNumber);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN01 theo nhóm nợ
        /// </summary>
        [HttpGet("by-debt-group-new/{debtGroup}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LN01DTO>>> GetByDebtGroupNew(
            string debtGroup,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln01Service.GetByDebtGroupAsync(debtGroup, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN01 theo nhóm nợ: {DebtGroup}", debtGroup);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy tổng dư nợ theo chi nhánh
        /// </summary>
        [HttpGet("total-loan-by-branch-new/{branchCode}")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetTotalLoanAmountByBranchNew(
            string branchCode,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _ln01Service.GetTotalLoanAmountByBranchAsync(branchCode, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng dư nợ theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy tổng dư nợ theo loại tiền
        /// </summary>
        [HttpGet("total-loan-by-currency-new/{currency}")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetTotalLoanAmountByCurrencyNew(
            string currency,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _ln01Service.GetTotalLoanAmountByCurrencyAsync(currency, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng dư nợ theo loại tiền: {Currency}", currency);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Tạo mới dữ liệu LN01
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<ActionResult<LN01DTO>> CreateNew([FromBody] CreateLN01DTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _ln01Service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByIdNew), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới dữ liệu LN01");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu LN01
        /// </summary>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<ActionResult<LN01DTO>> UpdateNew(long id, [FromBody] UpdateLN01DTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var exists = await _ln01Service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound($"Không tìm thấy bản ghi LN01 với ID: {id}");
                }

                var result = await _ln01Service.UpdateAsync(id, updateDto);
                if (result == null)
                {
                    return NotFound($"Không tìm thấy bản ghi LN01 với ID: {id}");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật dữ liệu LN01 với ID: {ID}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Xóa dữ liệu LN01
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<ActionResult> DeleteNew(long id)
        {
            try
            {
                var exists = await _ln01Service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound($"Không tìm thấy bản ghi LN01 với ID: {id}");
                }

                var result = await _ln01Service.DeleteAsync(id);
                if (!result)
                {
                    return StatusCode(500, "Xóa dữ liệu không thành công");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dữ liệu LN01 với ID: {ID}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy danh sách thay đổi của chi nhánh 7808 từ 30/4/2025 đến 31/5/2025 từ bảng LN01 mới
        /// </summary>
        [HttpGet("changes/branch-7808")]
        public async Task<IActionResult> GetBranch7808Changes()
        {
            try
            {
                // Truy vấn trực tiếp từ bảng LN01 mới
                var targetDateStrings = new[] { "30-04-2025", "31-05-2025" };
                // Convert target date strings to DateTime for comparison
                var targetDates = targetDateStrings
                    .Where(d => DateTime.TryParse(d, out _))
                    .Select(d => DateTime.Parse(d).Date)
                    .ToList();

                var lnData = await _context.LN01s
                    .Where(x => x.BRCD == "7808")
                    .ToListAsync();

                // Filter by dates on client side
                lnData = lnData.Where(x => x.NGAY_DL.HasValue && targetDates.Contains(x.NGAY_DL.Value.Date))
                    .OrderByDescending(x => x.NGAY_DL)
                    .ToList();

                if (!lnData.Any())
                {
                    return Ok(new
                    {
                        Summary = new
                        {
                            TotalChanges = 0,
                            TimeRange = "30/04/2025 - 31/05/2025",
                            BranchCode = "7808",
                            QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                            DataSource = "LN01 Table (Optimized)",
                            Message = "No LN01 data found for branch 7808 in specified date range"
                        }
                    });
                }

                // Define target dates
                var april30 = new DateTime(2025, 4, 30);
                var may31 = new DateTime(2025, 5, 31);

                // Group by NGAY_DL to calculate changes
                var april30Data = lnData.Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == new DateTime(2025, 4, 30)).ToList();
                var may31Data = lnData.Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == new DateTime(2025, 5, 31)).ToList();

                return Ok(new
                {
                    Summary = new
                    {
                        TotalChanges = lnData.Count,
                        TimeRange = "30/04/2025 - 31/05/2025",
                        BranchCode = "7808",
                        QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        DataSource = "LN01 Table (Optimized)",
                        April30Records = april30Data.Count,
                        May31Records = may31Data.Count
                    },
                    Data = new
                    {
                        April30 = april30Data.Take(10).ToList(), // Limit for performance
                        May31 = may31Data.Take(10).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Lỗi khi truy vấn dữ liệu LN01",
                    details = ex.Message,
                    source = "LN01 Table"
                });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan dữ liệu LN01
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var totalRecords = await _context.LN01s.CountAsync();
                var uniqueBranches = await _context.LN01s.Select(x => x.BRCD).Distinct().CountAsync();
                var latestDate = await _context.LN01s.OrderByDescending(x => x.NGAY_DL).Select(x => x.NGAY_DL).FirstOrDefaultAsync();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    UniqueBranches = uniqueBranches,
                    LatestDataDate = latestDate,
                    QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    DataSource = "LN01 Table (Optimized)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Lỗi khi truy vấn thống kê LN01",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// 🔧 CHUẨN HÓA: Extract mã chi nhánh từ filename theo format MaCN_LoaiFile_Ngay.ext
        /// Format: 7800_LN01_20241231.csv hoặc 7801_DP01_20241130.xlsx
        /// Fallback: Tìm mã chi nhánh 4 số bất kỳ đâu trong string
        /// </summary>
        private static string ExtractBranchCode(string fileName)
        {
            try
            {
                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext (7800_LN01_20241231.csv)
                var standardMatch = System.Text.RegularExpressions.Regex.Match(fileName, @"^(78\d{2})_[A-Z0-9_]+_\d{8}\.(csv|xlsx?)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    return standardMatch.Groups[1].Value;
                }

                // Strategy 2: Fallback - tìm mã chi nhánh bất kỳ đâu trong filename
                var fallbackMatch = System.Text.RegularExpressions.Regex.Match(fileName, @"(78\d{2})");
                if (fallbackMatch.Success)
                {
                    return fallbackMatch.Groups[1].Value;
                }

                // Strategy 3: Legacy fallback - tìm 4 số bất kỳ
                var legacyMatch = System.Text.RegularExpressions.Regex.Match(fileName, @"\d{4}");
                return legacyMatch.Success ? legacyMatch.Value : "7800";
            }
            catch
            {
                return "7800";
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan thay đổi LN01
        /// </summary>
        [HttpGet("changes/summary")]
        public async Task<IActionResult> GetChangesSummary()
        {
            try
            {
                // Lấy thống kê từ LN01_History
                var stats = await _context.LN01History
                    .GroupBy(x => x.SourceID.Contains("7808") ? "7808" : "Other")
                    .Select(g => new
                    {
                        BranchGroup = g.Key,
                        TotalRecords = g.Count(),
                        ChangesInPeriod = g.Count(x => x.ValidFrom >= new DateTime(2025, 4, 30) &&
                                                      x.ValidFrom <= new DateTime(2025, 5, 31)),
                        EarliestRecord = g.Min(x => x.ValidFrom),
                        LatestRecord = g.Max(x => x.ValidFrom),
                        EarliestRecordFormatted = g.Min(x => x.ValidFrom).ToString("dd/MM/yyyy"),
                        LatestRecordFormatted = g.Max(x => x.ValidFrom).ToString("dd/MM/yyyy"),
                        CurrentRecords = g.Count(x => x.IsCurrent),
                        HistoryRecords = g.Count(x => !x.IsCurrent)
                    })
                    .ToListAsync();

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy thống kê",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả dữ liệu LN01 hiện có trong hệ thống
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRecords()
        {
            try
            {
                var allRecords = await _context.LN01History
                    .OrderByDescending(x => x.ValidFrom)
                    .Take(100) // Giới hạn 100 bản ghi để tránh quá tải
                    .Select(r => new
                    {
                        HistoryID = r.HistoryID,
                        SourceID = r.SourceID,
                        ValidFrom = r.ValidFrom.ToString("dd/MM/yyyy HH:mm:ss"),
                        ValidTo = r.ValidTo.ToString("dd/MM/yyyy HH:mm:ss"),
                        IsCurrent = r.IsCurrent,
                        VersionNumber = r.VersionNumber,
                        CreatedDate = r.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        BranchCode = "N/A",
                        MANDT = r.MANDT,
                        BUKRS = r.BUKRS,
                        LAND1 = r.LAND1,
                        WAERS = r.WAERS
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalFound = allRecords.Count,
                    Message = allRecords.Any() ? "Đã tìm thấy dữ liệu LN01" : "Chưa có dữ liệu LN01 trong hệ thống",
                    Records = allRecords
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy tất cả dữ liệu",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Tạo dữ liệu mẫu cho chi nhánh 7808 để test
        /// </summary>
        [HttpPost("create-sample-data")]
        public async Task<IActionResult> CreateSampleData()
        {
            try
            {
                var sampleRecords = new List<LN01History>();

                // Tạo 5 bản ghi mẫu cho chi nhánh 7808
                for (int i = 1; i <= 5; i++)
                {
                    var record = new LN01History
                    {
                        SourceID = $"LN01_7808_{i:000}",
                        ValidFrom = new DateTime(2025, 4, 30).AddDays(i),
                        ValidTo = new DateTime(9999, 12, 31, 23, 59, 59),
                        IsCurrent = true,
                        VersionNumber = 1,
                        RecordHash = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,

                        // Dữ liệu LN01 mẫu
                        MANDT = "100",
                        BUKRS = "7808",
                        LAND1 = "VN",
                        WAERS = "VND",
                        SPRAS = "V",
                        KTOPL = "AGR1",
                        WAABW = "01",
                        PERIV = "K4",
                        KOKFI = "X",
                        RCOMP = "100000",
                        ADRNR = $"100000{i:00}",
                        STCEG = $"0123456789{i}",
                        FIKRS = "AGR1",
                        XFMCO = "X",
                        XFMCB = "X",
                        XFMCA = "X",
                        TXJCD = "V1"
                    };

                    sampleRecords.Add(record);
                }

                await _context.LN01History.AddRangeAsync(sampleRecords);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Đã tạo {sampleRecords.Count} bản ghi mẫu cho chi nhánh 7808",
                    CreatedRecords = sampleRecords.Select(r => new
                    {
                        SourceID = r.SourceID,
                        ValidFrom = r.ValidFrom.ToString("dd/MM/yyyy"),
                        BUKRS = r.BUKRS
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi tạo dữ liệu mẫu",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Debug: Lấy thông tin tất cả file LN01 của chi nhánh 7808
        /// </summary>
        [HttpGet("debug/branch-7808-files")]
        public async Task<IActionResult> GetBranch7808Files()
        {
            try
            {
                var allLN01Files = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderByDescending(x => x.ImportDate)
                    .ToListAsync();

                var result = allLN01Files.Select(x => new
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    StatementDate = x.StatementDate?.ToString("dd/MM/yyyy") ?? "NULL",
                    ImportDate = x.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    RecordsCount = x.RecordsCount,
                    Status = x.Status,
                    ImportedBy = x.ImportedBy,
                    BranchCode = "7808"
                }).ToList();

                return Ok(new
                {
                    Message = $"Tìm thấy {result.Count} file LN01 của chi nhánh 7808",
                    Files = result,
                    FilterInfo = new
                    {
                        RequestedDateRange = "30/04/2025 - 31/05/2025",
                        ActualDatesFound = result.Select(f => f.StatementDate).Distinct().ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi debug files",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Đơn giản: Lấy tất cả file LN01 chi nhánh 7808
        /// </summary>
        [HttpGet("simple-files")]
        public async Task<IActionResult> GetSimpleFiles()
        {
            try
            {
                var files = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .ToListAsync();

                return Ok(files);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// So sánh chi tiết dữ liệu LN01 giữa hai ngày cho chi nhánh 7808
        /// </summary>
        [HttpGet("detailed-comparison/branch-7808")]
        public async Task<IActionResult> GetDetailedComparison()
        {
            try
            {
                // Lấy dữ liệu từ file ngày 30/4/2025
                var april30Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 4, 30).Date)
                    .FirstOrDefaultAsync();

                // Lấy dữ liệu từ file ngày 31/5/2025
                var may31Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 5, 31).Date)
                    .FirstOrDefaultAsync();

                if (april30Record == null && may31Record == null)
                {
                    return Ok(new
                    {
                        Summary = new
                        {
                            Status = "NoData",
                            Message = "Không tìm thấy dữ liệu cho cả hai ngày",
                            QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        }
                    });
                }

                var comparison = new
                {
                    Summary = new
                    {
                        BranchCode = "7808",
                        ComparisonDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        April30File = april30Record?.FileName ?? "Không có",
                        May31File = may31Record?.FileName ?? "Không có",
                        April30Records = april30Record?.RecordsCount ?? 0,
                        May31Records = may31Record?.RecordsCount ?? 0,
                        RecordsDifference = (may31Record?.RecordsCount ?? 0) - (april30Record?.RecordsCount ?? 0),
                        HasBothFiles = april30Record != null && may31Record != null
                    },
                    Files = new
                    {
                        April30 = april30Record != null ? new
                        {
                            Id = april30Record.Id,
                            FileName = april30Record.FileName,
                            StatementDate = april30Record.StatementDate?.ToString("dd/MM/yyyy"),
                            ImportDate = april30Record.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                            RecordsCount = april30Record.RecordsCount,
                            Status = april30Record.Status,
                            ImportedBy = april30Record.ImportedBy
                        } : null,
                        May31 = may31Record != null ? new
                        {
                            Id = may31Record.Id,
                            FileName = may31Record.FileName,
                            StatementDate = may31Record.StatementDate?.ToString("dd/MM/yyyy"),
                            ImportDate = may31Record.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                            RecordsCount = may31Record.RecordsCount,
                            Status = may31Record.Status,
                            ImportedBy = may31Record.ImportedBy
                        } : null
                    }
                };

                // Nếu có cả hai file, lấy sample data để so sánh từ bảng LN01
                if (april30Record != null && may31Record != null)
                {
                    var april30Date = new DateTime(2025, 4, 30);
                    var may31Date = new DateTime(2025, 5, 31);

                    var april30Data = await _context.LN01s
                        .Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == april30Date && x.BRCD == "7808")
                        .Take(20)
                        .Select(x => new
                        {
                            Id = x.Id,
                            RawData = $"Branch: {x.BRCD}, Customer: {x.CUSTSEQ}, Contract: {x.APPRSEQ}, Amount: {x.DU_NO}",
                            ProcessedDate = x.NGAY_DL
                        })
                        .ToListAsync();

                    var may31Data = await _context.LN01s
                        .Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == may31Date && x.BRCD == "7808")
                        .Take(20)
                        .Select(x => new
                        {
                            Id = x.Id,
                            RawData = $"Branch: {x.BRCD}, Customer: {x.CUSTSEQ}, Contract: {x.APPRSEQ}, Amount: {x.DU_NO}",
                            ProcessedDate = x.NGAY_DL
                        })
                        .ToListAsync();

                    return Ok(new
                    {
                        Summary = comparison.Summary,
                        Files = comparison.Files,
                        SampleComparison = new
                        {
                            April30Sample = april30Data,
                            May31Sample = may31Data,
                            ComparisonNote = "Hiển thị 20 bản ghi đầu tiên từ mỗi file để so sánh"
                        }
                    });
                }

                return Ok(comparison);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi so sánh dữ liệu chi tiết",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả thay đổi LN01 chi nhánh 7808 với định dạng đơn giản
        /// </summary>
        [HttpGet("all-changes/branch-7808")]
        public async Task<IActionResult> GetAllBranch7808Changes()
        {
            try
            {
                // Lấy tất cả file LN01 của chi nhánh 7808
                var allFiles = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderBy(x => x.StatementDate)
                    .ToListAsync();

                var changes = new List<object>();
                var april30Date = new DateTime(2025, 4, 30);

                foreach (var file in allFiles)
                {
                    // Lấy một vài bản ghi mẫu từ bảng LN01 dựa vào ngày và chi nhánh
                    var sampleData = await _context.LN01s
                        .Where(x => x.BRCD == "7808" && x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == april30Date)
                        .Take(5)
                        .Select(x => new
                        {
                            RawData = $"Branch: {x.BRCD}, Customer: {x.CUSTSEQ}, Contract: {x.APPRSEQ}",
                            ProcessedDate = x.NGAY_DL
                        })
                        .ToListAsync();

                    changes.Add(new
                    {
                        FileName = file.FileName,
                        StatementDate = file.StatementDate?.ToString("dd/MM/yyyy") ?? "N/A",
                        ImportDate = file.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        RecordsCount = file.RecordsCount,
                        Status = file.Status,
                        ImportedBy = file.ImportedBy ?? "System",
                        SampleRecords = sampleData
                    });
                }

                return Ok(new
                {
                    BranchCode = "7808",
                    TotalFiles = allFiles.Count,
                    TotalRecords = allFiles.Sum(x => x.RecordsCount),
                    TimeRange = allFiles.Any() ?
                        $"{allFiles.Min(x => x.StatementDate)?.ToString("dd/MM/yyyy")} - {allFiles.Max(x => x.StatementDate)?.ToString("dd/MM/yyyy")}" :
                        "N/A",
                    QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Changes = changes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy tất cả thay đổi",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Xuất tất cả thay đổi LN01 chi nhánh 7808 ra file CSV
        /// </summary>
        [HttpGet("export/csv/branch-7808")]
        public async Task<IActionResult> ExportBranch7808ToCSV()
        {
            try
            {
                // Lấy tất cả file LN01 của chi nhánh 7808
                var allFiles = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderBy(x => x.StatementDate)
                    .ToListAsync();

                var csvData = new StringBuilder();

                // Header cho CSV
                csvData.AppendLine("STT,FileName,StatementDate,ImportDate,RecordsCount,Status,ImportedBy,CustomerSeq,CustomerName,AccountNumber,Currency,DebtAmount,LoanType,InterestRate,OfficerName,NextRepayDate,Province,District,LastRepayDate");

                int stt = 1;
                var april30Date = new DateTime(2025, 4, 30);

                foreach (var file in allFiles)
                {
                    // Lấy tất cả dữ liệu chi tiết từ bảng LN01 cho file này
                    var detailData = await _context.LN01s
                        .Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == april30Date && x.BRCD == "7808")
                        .ToListAsync();

                    foreach (var lnRecord in detailData)
                    {
                        try
                        {
                            // Escape CSV special characters
                            var custName = EscapeCSVField(lnRecord.CUSTSEQ ?? "");
                            var loanType = EscapeCSVField(lnRecord.LOAN_TYPE ?? "");
                            var officerName = EscapeCSVField(lnRecord.OFFICER_NAME ?? "");
                            var province = EscapeCSVField(lnRecord.PROVINCE ?? "");
                            var district = EscapeCSVField(lnRecord.DISTRICT ?? "");

                            csvData.AppendLine($"{stt},{file.FileName},{file.StatementDate?.ToString("dd/MM/yyyy")},{file.ImportDate:dd/MM/yyyy HH:mm:ss},{file.RecordsCount},{file.Status},{file.ImportedBy},{lnRecord.CUSTSEQ},{custName},{lnRecord.APPRSEQ},{lnRecord.BRCD},{lnRecord.DU_NO},{loanType},{lnRecord.INTEREST_RATE},{officerName},{lnRecord.NEXT_REPAY_DATE},{province},{district},{lnRecord.TRANSACTION_DATE}");
                            stt++;
                        }
                        catch (JsonException)
                        {
                            // Nếu không parse được JSON, thêm dòng với thông tin cơ bản
                            csvData.AppendLine($"{stt},{file.FileName},{file.StatementDate?.ToString("dd/MM/yyyy")},{file.ImportDate:dd/MM/yyyy HH:mm:ss},{file.RecordsCount},{file.Status},{file.ImportedBy},,,,,,,,,,,");
                            stt++;
                        }
                    }
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = $"LN01_Branch_7808_Changes_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi xuất file CSV",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Xuất tóm tắt thay đổi LN01 chi nhánh 7808 ra file CSV (chỉ thông tin file)
        /// </summary>
        [HttpGet("export/summary-csv/branch-7808")]
        public async Task<IActionResult> ExportBranch7808SummaryToCSV()
        {
            try
            {
                // Lấy tất cả file LN01 của chi nhánh 7808
                var allFiles = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderBy(x => x.StatementDate)
                    .ToListAsync();

                var csvData = new StringBuilder();

                // Header cho CSV tóm tắt
                csvData.AppendLine("STT,FileName,StatementDate,ImportDate,RecordsCount,Status,ImportedBy,FileType,Category,Notes");

                int stt = 1;
                foreach (var file in allFiles)
                {
                    var notes = EscapeCSVField(file.Notes ?? "");
                    csvData.AppendLine($"{stt},{file.FileName},{file.StatementDate?.ToString("dd/MM/yyyy")},{file.ImportDate:dd/MM/yyyy HH:mm:ss},{file.RecordsCount},{file.Status},{file.ImportedBy},{file.FileType},{file.Category},{notes}");
                    stt++;
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = $"LN01_Branch_7808_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi xuất file CSV tóm tắt",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Xuất so sánh chi tiết giữa hai ngày ra file CSV
        /// </summary>
        [HttpGet("export/comparison-csv/branch-7808")]
        public async Task<IActionResult> ExportComparisonToCSV()
        {
            try
            {
                // Lấy dữ liệu từ file ngày 30/4/2025
                var april30Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 4, 30).Date)
                    .FirstOrDefaultAsync();

                // Lấy dữ liệu từ file ngày 31/5/2025
                var may31Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 5, 31).Date)
                    .FirstOrDefaultAsync();

                if (april30Record == null || may31Record == null)
                {
                    return BadRequest(new
                    {
                        Error = "Không tìm thấy đủ dữ liệu để so sánh",
                        Message = $"File 30/4: {(april30Record != null ? "Có" : "Không có")}, File 31/5: {(may31Record != null ? "Có" : "Không có")}"
                    });
                }

                var csvData = new StringBuilder();

                // Header cho CSV so sánh
                csvData.AppendLine("Type,FileName,StatementDate,CustomerSeq,CustomerName,AccountNumber,Currency,DebtAmount,LoanType,InterestRate,OfficerName,Province,District,LastRepayDate");

                var april30Date = new DateTime(2025, 4, 30);
                var may31Date = new DateTime(2025, 5, 31);

                // Lấy dữ liệu từ bảng LN01 tháng 4
                var april30Data = await _context.LN01s
                    .Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == april30Date && x.BRCD == "7808")
                    .ToListAsync();

                // Lấy dữ liệu từ bảng LN01 tháng 5
                var may31Data = await _context.LN01s
                    .Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == may31Date && x.BRCD == "7808")
                    .ToListAsync();

                // Xuất dữ liệu tháng 4
                foreach (var lnRecord in april30Data.Take(100)) // Giới hạn 100 bản ghi đầu tiên
                {
                    AddLN01DataRowToCSV(csvData, "2025-04-30", april30Record.FileName, lnRecord);
                }

                // Xuất dữ liệu tháng 5
                foreach (var lnRecord in may31Data.Take(100)) // Giới hạn 100 bản ghi đầu tiên
                {
                    AddLN01DataRowToCSV(csvData, "2025-05-31", may31Record.FileName, lnRecord);
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = $"LN01_Branch_7808_Comparison_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi xuất file CSV so sánh",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Helper method để thêm dữ liệu LN01 vào CSV so sánh
        /// </summary>
        private void AddLN01DataRowToCSV(StringBuilder csvData, string type, string fileName, LN01 lnRecord)
        {
            try
            {
                // Escape CSV special characters
                var custName = EscapeCSVField(lnRecord.CUSTSEQ ?? "");
                var loanType = EscapeCSVField(lnRecord.LOAN_TYPE ?? "");
                var officerName = EscapeCSVField(lnRecord.OFFICER_NAME ?? "");
                var province = EscapeCSVField(lnRecord.PROVINCE ?? "");
                var district = EscapeCSVField(lnRecord.DISTRICT ?? "");

                csvData.AppendLine($"{type},{fileName},{type},{lnRecord.CUSTSEQ},{custName},{lnRecord.APPRSEQ},{lnRecord.BRCD},{lnRecord.DU_NO},{loanType},{lnRecord.INTEREST_RATE},{officerName},{province},{district},{lnRecord.TRANSACTION_DATE}");
            }
            catch (Exception)
            {
                csvData.AppendLine($"{type},{fileName},{type},,,,,,,,,,,");
            }
        }

        /// <summary>
        /// Helper method để thêm dòng dữ liệu vào CSV
        /// </summary>
        private void AddDataRowToCSV(StringBuilder csvData, string type, string fileName, string rawData)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(rawData);
                var root = jsonDoc.RootElement;

                var custSeq = root.TryGetProperty("CUSTSEQ", out var custSeqProp) ? custSeqProp.GetString() : "";
                var custName = root.TryGetProperty("CUSTNM", out var custNameProp) ? custNameProp.GetString() : "";
                var account = root.TryGetProperty("TAI_KHOAN", out var accountProp) ? accountProp.GetString() : "";
                var currency = root.TryGetProperty("CCY", out var ccyProp) ? ccyProp.GetString() : "";
                var debtAmount = root.TryGetProperty("DU_NO", out var debtProp) ? debtProp.GetString() : "";
                var loanType = root.TryGetProperty("LOAN_TYPE", out var loanTypeProp) ? loanTypeProp.GetString() : "";
                var interestRate = root.TryGetProperty("INTEREST_RATE", out var rateProp) ? rateProp.GetString() : "";
                var officerName = root.TryGetProperty("OFFICER_NAME", out var officerProp) ? officerProp.GetString() : "";
                var province = root.TryGetProperty("LCLPROVINNM", out var provinceProp) ? provinceProp.GetString() : "";
                var district = root.TryGetProperty("LCLDISTNM", out var districtProp) ? districtProp.GetString() : "";
                var lastRepayDate = root.TryGetProperty("LAST_REPAY_DATE", out var lastRepayProp) ? lastRepayProp.GetString() : "";

                // Escape CSV special characters
                custName = EscapeCSVField(custName);
                loanType = EscapeCSVField(loanType);
                officerName = EscapeCSVField(officerName);
                province = EscapeCSVField(province);
                district = EscapeCSVField(district);

                csvData.AppendLine($"{type},{fileName},{type},{custSeq},{custName},{account},{currency},{debtAmount},{loanType},{interestRate},{officerName},{province},{district},{lastRepayDate}");
            }
            catch (JsonException)
            {
                csvData.AppendLine($"{type},{fileName},{type},,,,,,,,,,,");
            }
        }

        /// <summary>
        /// Helper method để escape các ký tự đặc biệt trong CSV
        /// </summary>
        private string EscapeCSVField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // Nếu có dấu phẩy, dấu ngoặc kép, hoặc xuống dòng thì bọc trong dấu ngoặc kép
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                // Escape dấu ngoặc kép bằng cách nhân đôi
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }
    }
}
