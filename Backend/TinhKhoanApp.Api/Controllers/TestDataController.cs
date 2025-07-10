using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Test controller để verify dữ liệu sau import và migration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TestDataController> _logger;

        public TestDataController(ApplicationDbContext context, ILogger<TestDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Test DP01 data với model mới 63 cột
        /// </summary>
        [HttpGet("dp01/test")]
        public async Task<ActionResult> TestDP01Data()
        {
            try
            {
                var dp01Records = await _context.DP01_News
                    .OrderByDescending(x => x.CREATED_DATE)
                    .Take(5)
                    .Select(x => new
                    {
                        x.Id,
                        x.MA_CN,
                        x.MA_KH,
                        x.TEN_KH,
                        x.CCY,
                        CurrentBalance = x.CURRENT_BALANCE,
                        Rate = x.RATE,
                        SoTaiKhoan = x.SO_TAI_KHOAN,
                        CreatedDate = x.CREATED_DATE,
                        UpdatedDate = x.UPDATED_DATE
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "DP01 data test thành công",
                    Count = dp01Records.Count,
                    Data = dp01Records
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi test DP01 data");
                return BadRequest(new { Error = "Lỗi test DP01 data", Message = ex.Message });
            }
        }

        /// <summary>
        /// Test LN01 data với model mới 79 cột
        /// </summary>
        [HttpGet("ln01/test")]
        public async Task<ActionResult> TestLN01Data()
        {
            try
            {
                var ln01Records = await _context.LN01s
                    .OrderByDescending(x => x.CREATED_DATE)
                    .Take(5)
                    .Select(x => new
                    {
                        x.Id,
                        BranchCode = x.BRCD,
                        CustomerSeq = x.CUSTSEQ,
                        CustomerName = x.CUSTNM,
                        TaiKhoan = x.TAI_KHOAN,
                        DuNo = x.DU_NO,
                        LoanType = x.LOAN_TYPE,
                        InterestRate = x.INTEREST_RATE,
                        TransactionDate = x.TRANSACTION_DATE,
                        OfficerName = x.OFFICER_NAME,
                        CreatedDate = x.CREATED_DATE,
                        UpdatedDate = x.UPDATED_DATE
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "LN01 data test thành công",
                    Count = ln01Records.Count,
                    Data = ln01Records
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi test LN01 data");
                return BadRequest(new { Error = "Lỗi test LN01 data", Message = ex.Message });
            }
        }

        /// <summary>
        /// Summary về số lượng records trong các bảng chính
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult> GetDataSummary()
        {
            try
            {
                var summary = new
                {
                    DP01_Count = await _context.DP01_News.CountAsync(),
                    LN01_Count = await _context.LN01s.CountAsync(),
                    LN02_Count = await _context.LN02s.CountAsync(),
                    LN03_Count = await _context.LN03s.CountAsync(),
                    GL01_Count = await _context.GL01s.CountAsync(),
                    KH03_Count = await _context.KH03s.CountAsync(),
                    DPDA_Count = await _context.DPDAs.CountAsync(),
                    EI01_Count = await _context.EI01s.CountAsync(),
                    RR01_Count = await _context.RR01s.CountAsync(),
                    LastImport = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                return Ok(new
                {
                    Success = true,
                    Message = "Data summary",
                    Summary = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi get data summary");
                return BadRequest(new { Error = "Lỗi get data summary", Message = ex.Message });
            }
        }
    }
}
