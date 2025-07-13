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
                var dp01Records = await _context.DP01
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
        /// Test EI01 data với model mới 24 cột
        /// </summary>
        [HttpGet("ei01/test")]
        public async Task<ActionResult> TestEI01Data()
        {
            try
            {
                var ei01Records = await _context.EI01s
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new
                    {
                        x.Id,
                        x.NgayDL,
                        x.MA_CN,
                        x.MA_KH,
                        x.TEN_KH,
                        x.LOAI_KH,
                        x.SDT_EMB,
                        x.TRANG_THAI_EMB,
                        x.NGAY_DK_EMB,
                        x.SDT_OTT,
                        x.TRANG_THAI_OTT,
                        x.NGAY_DK_OTT
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "EI01 data test thành công",
                    Count = ei01Records.Count,
                    Data = ei01Records
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi test EI01 data");
                return BadRequest(new { Error = "Lỗi test EI01 data", Message = ex.Message });
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
                    DP01_Count = await _context.DP01.CountAsync(),
                    LN01_Count = await _context.LN01.CountAsync(),
                    LN03_Count = await _context.LN03.CountAsync(),
                    GL01_Count = await _context.GL01.CountAsync(),
                    DPDA_Count = await _context.DPDA.CountAsync(),
                    EI01_Count = await _context.EI01.CountAsync(),
                    RR01_Count = await _context.RR01.CountAsync(),
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
