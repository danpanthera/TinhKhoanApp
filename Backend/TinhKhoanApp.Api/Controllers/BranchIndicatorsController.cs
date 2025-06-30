using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Utils;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchIndicatorsController : ControllerBase
    {
        private readonly IBranchCalculationService _branchCalculationService;
        private readonly ILogger<BranchIndicatorsController> _logger;
        private readonly ApplicationDbContext _context;

        public BranchIndicatorsController(
            IBranchCalculationService branchCalculationService,
            ILogger<BranchIndicatorsController> logger,
            ApplicationDbContext context)
        {
            _branchCalculationService = branchCalculationService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Tính toán Nguồn vốn theo chi nhánh
        /// </summary>
        [HttpPost("nguon-von")]
        public async Task<ActionResult<decimal>> CalculateNguonVon([FromBody] BranchCalculationRequest request)
        {
            try
            {
                // Validation request
                if (request == null || string.IsNullOrWhiteSpace(request.BranchId))
                {
                    _logger.LogWarning("❌ Request không hợp lệ - BranchId bị thiếu hoặc rỗng");
                    return BadRequest(new BranchCalculationResponse
                    {
                        IndicatorName = "Nguồn vốn",
                        Success = false,
                        ErrorMessage = "BranchId không được để trống"
                    });
                }

                _logger.LogInformation("🔧 Nhận request tính Nguồn vốn - BranchId: {BranchId}, Date: {Date}",
                    request.BranchId, request.Date?.ToString("dd/MM/yyyy") ?? "null");

                var result = await _branchCalculationService.CalculateNguonVonByBranch(
                    request.BranchId,
                    request.Date);

                return Ok(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Nguồn vốn",
                    Value = result,
                    Unit = "VND",
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nguồn vốn cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Nguồn vốn",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Tính toán Dư nợ theo chi nhánh
        /// </summary>
        [HttpPost("du-no")]
        public async Task<ActionResult<decimal>> CalculateDuNo([FromBody] BranchCalculationRequest request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Dư nợ cho chi nhánh {BranchId}", request.BranchId);

                var result = await _branchCalculationService.CalculateDuNoByBranch(
                    request.BranchId,
                    request.Date);

                return Ok(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Dư nợ",
                    Value = result,
                    Unit = "VND",
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Dư nợ cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Dư nợ",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Tính toán Nợ xấu theo chi nhánh
        /// </summary>
        [HttpPost("no-xau")]
        public async Task<ActionResult<decimal>> CalculateNoXau([FromBody] BranchCalculationRequest request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Nợ xấu cho chi nhánh {BranchId}", request.BranchId);

                var result = await _branchCalculationService.CalculateNoXauByBranch(
                    request.BranchId,
                    request.Date);

                return Ok(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Nợ xấu",
                    Value = result,
                    Unit = "%",
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Nợ xấu cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Nợ xấu",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Tính toán Thu hồi XLRR theo chi nhánh
        /// </summary>
        [HttpPost("thu-hoi-xlrr")]
        public async Task<ActionResult<decimal>> CalculateThuHoiXLRR([FromBody] BranchCalculationRequest request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Thu hồi XLRR cho chi nhánh {BranchId}", request.BranchId);

                var result = await _branchCalculationService.CalculateThuHoiXLRRByBranch(
                    request.BranchId,
                    request.Date);

                return Ok(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Thu hồi XLRR",
                    Value = result,
                    Unit = "VND",
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Thu hồi XLRR cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Thu hồi XLRR",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Tính toán Thu dịch vụ theo chi nhánh
        /// </summary>
        [HttpPost("thu-dich-vu")]
        public async Task<ActionResult<decimal>> CalculateThuDichVu([FromBody] BranchCalculationRequest request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Thu dịch vụ cho chi nhánh {BranchId}", request.BranchId);

                var result = await _branchCalculationService.CalculateThuDichVuByBranch(
                    request.BranchId,
                    request.Date);

                return Ok(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Thu dịch vụ",
                    Value = result,
                    Unit = "VND",
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Thu dịch vụ cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Thu dịch vụ",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Tính toán Lợi nhuận theo chi nhánh
        /// </summary>
        [HttpPost("loi-nhuan")]
        public async Task<ActionResult<decimal>> CalculateLoiNhuan([FromBody] BranchCalculationRequest request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính Lợi nhuận cho chi nhánh {BranchId}", request.BranchId);

                var result = await _branchCalculationService.CalculateLoiNhuanByBranch(
                    request.BranchId,
                    request.Date);

                return Ok(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Lợi nhuận",
                    Value = result,
                    Unit = "VND",
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính Lợi nhuận cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchCalculationResponse
                {
                    BranchId = request.BranchId,
                    IndicatorName = "Lợi nhuận",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Tính toán tất cả 6 chỉ tiêu cùng lúc
        /// </summary>
        [HttpPost("all-indicators")]
        public async Task<ActionResult<BranchAllIndicatorsResponse>> CalculateAllIndicators([FromBody] BranchCalculationRequest request)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính tất cả chỉ tiêu cho chi nhánh {BranchId}", request.BranchId);

                var response = new BranchAllIndicatorsResponse
                {
                    BranchId = request.BranchId,
                    CalculatedAt = VietnamDateTime.Now,
                    Success = true
                };

                // Tính toán song song các chỉ tiêu
                var tasks = new[]
                {
                    _branchCalculationService.CalculateNguonVonByBranch(request.BranchId, request.Date),
                    _branchCalculationService.CalculateDuNoByBranch(request.BranchId, request.Date),
                    _branchCalculationService.CalculateNoXauByBranch(request.BranchId, request.Date),
                    _branchCalculationService.CalculateThuHoiXLRRByBranch(request.BranchId, request.Date),
                    _branchCalculationService.CalculateThuDichVuByBranch(request.BranchId, request.Date),
                    _branchCalculationService.CalculateLoiNhuanByBranch(request.BranchId, request.Date)
                };

                var results = await Task.WhenAll(tasks);

                response.NguonVon = results[0];
                response.DuNo = results[1];
                response.NoXau = results[2];
                response.ThuHoiXLRR = results[3];
                response.ThuDichVu = results[4];
                response.LoiNhuan = results[5];

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tính tất cả chỉ tiêu cho chi nhánh {BranchId}", request.BranchId);
                return BadRequest(new BranchAllIndicatorsResponse
                {
                    BranchId = request.BranchId,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Debug endpoint - Kiểm tra dữ liệu DP01 có trong database
        /// </summary>
        [HttpGet("debug-dp01-data")]
        public async Task<ActionResult> DebugDP01Data()
        {
            try
            {
                // Kiểm tra tổng số records DP01
                var totalDP01Records = await _context.ImportedDataRecords
                    .Where(x => x.Category == "DP01")
                    .CountAsync();

                // Kiểm tra các ngày có dữ liệu
                var availableDates = await _context.ImportedDataRecords
                    .Where(x => x.Category == "DP01" && x.StatementDate.HasValue)
                    .Select(x => x.StatementDate.Value.Date)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .Take(10)
                    .ToListAsync();

                // Kiểm tra ngày gần nhất và dữ liệu
                var latestDate = availableDates.FirstOrDefault();
                var sampleData = new List<object>();

                if (latestDate != default)
                {
                    var latestRecords = await _context.ImportedDataRecords
                        .Where(x => x.Category == "DP01" && x.StatementDate.HasValue && x.StatementDate.Value.Date == latestDate)
                        .ToListAsync();

                    foreach (var record in latestRecords.Take(2))
                    {
                        var items = await _context.ImportedDataItems
                            .Where(x => x.ImportedDataRecordId == record.Id)
                            .Take(3)
                            .Select(x => x.RawData)
                            .ToListAsync();

                        sampleData.Add(new
                        {
                            FileName = record.FileName,
                            ItemCount = await _context.ImportedDataItems.Where(x => x.ImportedDataRecordId == record.Id).CountAsync(),
                            SampleItems = items.Take(2).ToList()
                        });
                    }
                }

                return Ok(new
                {
                    TotalDP01Records = totalDP01Records,
                    AvailableDates = availableDates.Select(d => d.ToString("yyyy-MM-dd")).ToList(),
                    LatestDate = latestDate.ToString("yyyy-MM-dd"),
                    SampleData = sampleData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi debug DP01 data");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class BranchCalculationRequest
    {
        public string BranchId { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
    }

    public class BranchCalculationResponse
    {
        public string BranchId { get; set; } = string.Empty;
        public string IndicatorName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime CalculatedAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class BranchAllIndicatorsResponse
    {
        public string BranchId { get; set; } = string.Empty;
        public decimal NguonVon { get; set; }
        public decimal DuNo { get; set; }
        public decimal NoXau { get; set; }
        public decimal ThuHoiXLRR { get; set; }
        public decimal ThuDichVu { get; set; }
        public decimal LoiNhuan { get; set; }
        public DateTime CalculatedAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
