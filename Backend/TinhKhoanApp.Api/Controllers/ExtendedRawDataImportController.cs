using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models.RawData;
using Microsoft.Extensions.Logging;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// 🆕 Controller mở rộng cho import dữ liệu thô tất cả các bảng SCD Type 2
    /// Bao gồm: LN03, EI01, DPDA, BC57
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExtendedRawDataImportController : ControllerBase
    {
        private readonly IExtendedRawDataImportService _importService;
        private readonly ILogger<ExtendedRawDataImportController> _logger;

        public ExtendedRawDataImportController(
            IExtendedRawDataImportService importService,
            ILogger<ExtendedRawDataImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        // =======================================
        // 🆕 Import Endpoints for Additional Tables
        // =======================================

        /// <summary>
        /// Import dữ liệu LN03 (Nợ XLRR)
        /// </summary>
        [HttpPost("import/ln03")]
        public async Task<ActionResult<ImportResponseDto>> ImportLN03Data([FromBody] ImportLN03RequestDto request)
        {
            try
            {
                _logger.LogInformation("Starting LN03 data import");

                var importRequest = new ImportRequestDto
                {
                    BatchId = request.BatchId,
                    CreatedBy = User.Identity?.Name ?? "System",
                    ImportDate = request.ImportDate
                };

                var result = await _importService.ImportLN03DataAsync(importRequest, request.Data);

                if (result.Success)
                {
                    _logger.LogInformation($"LN03 import completed successfully. BatchId: {result.BatchId}");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"LN03 import failed. BatchId: {result.BatchId}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing LN03 data");
                return StatusCode(500, new { error = "Internal server error during LN03 import" });
            }
        }

        /// <summary>
        /// Import dữ liệu EI01 (Mobile Banking)
        /// </summary>
        [HttpPost("import/ei01")]
        public async Task<ActionResult<ImportResponseDto>> ImportEI01Data([FromBody] ImportEI01RequestDto request)
        {
            try
            {
                _logger.LogInformation("Starting EI01 data import");

                var importRequest = new ImportRequestDto
                {
                    BatchId = request.BatchId,
                    CreatedBy = User.Identity?.Name ?? "System",
                    ImportDate = request.ImportDate
                };

                var result = await _importService.ImportEI01DataAsync(importRequest, request.Data);

                if (result.Success)
                {
                    _logger.LogInformation($"EI01 import completed successfully. BatchId: {result.BatchId}");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"EI01 import failed. BatchId: {result.BatchId}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing EI01 data");
                return StatusCode(500, new { error = "Internal server error during EI01 import" });
            }
        }

        /// <summary>
        /// Import dữ liệu DPDA (Phát hành thẻ)
        /// </summary>
        [HttpPost("import/dpda")]
        public async Task<ActionResult<ImportResponseDto>> ImportDPDAData([FromBody] ImportDPDARequestDto request)
        {
            try
            {
                _logger.LogInformation("Starting DPDA data import");

                var importRequest = new ImportRequestDto
                {
                    BatchId = request.BatchId,
                    CreatedBy = User.Identity?.Name ?? "System",
                    ImportDate = request.ImportDate
                };

                var result = await _importService.ImportDPDADataAsync(importRequest, request.Data);

                if (result.Success)
                {
                    _logger.LogInformation($"DPDA import completed successfully. BatchId: {result.BatchId}");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"DPDA import failed. BatchId: {result.BatchId}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing DPDA data");
                return StatusCode(500, new { error = "Internal server error during DPDA import" });
            }
        }

        /// <summary>
        /// Import dữ liệu BC57 (Lãi dự thu)
        /// </summary>
        [HttpPost("import/bc57")]
        public async Task<ActionResult<ImportResponseDto>> ImportBC57Data([FromBody] ImportBC57RequestDto request)
        {
            try
            {
                _logger.LogInformation("Starting BC57 data import");

                var importRequest = new ImportRequestDto
                {
                    BatchId = request.BatchId,
                    CreatedBy = User.Identity?.Name ?? "System",
                    ImportDate = request.ImportDate
                };

                var result = await _importService.ImportBC57DataAsync(importRequest, request.Data);

                if (result.Success)
                {
                    _logger.LogInformation($"BC57 import completed successfully. BatchId: {result.BatchId}");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"BC57 import failed. BatchId: {result.BatchId}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing BC57 data");
                return StatusCode(500, new { error = "Internal server error during BC57 import" });
            }
        }

        // =======================================
        // 📊 Statistics Endpoints
        // =======================================

        /// <summary>
        /// Lấy tổng hợp thống kê tất cả các bảng
        /// </summary>
        [HttpGet("statistics/all-tables")]
        public async Task<ActionResult<List<TableSummaryDto>>> GetAllTablesStatistics()
        {
            try
            {
                _logger.LogInformation("Getting all tables statistics");

                var statistics = await _importService.GetAllTablesSummaryAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tables statistics");
                return StatusCode(500, new { error = "Internal server error while getting statistics" });
            }
        }

        /// <summary>
        /// Endpoint kiểm tra sức khỏe của service
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "ExtendedRawDataImportService",
                supportedTables = new[] { "LN03", "EI01", "DPDA", "BC57" }
            });
        }

        // =======================================
        // 🔍 Data Query Endpoints (Optional)
        // =======================================

        /// <summary>
        /// Validate dữ liệu trước khi import
        /// </summary>
        [HttpPost("validate/{tableName}")]
        public IActionResult ValidateData(string tableName, [FromBody] object data)
        {
            try
            {
                // Basic validation logic
                var supportedTables = new[] { "LN03", "EI01", "DPDA", "BC57" };

                if (!supportedTables.Contains(tableName.ToUpper()))
                {
                    return BadRequest(new { error = $"Table {tableName} is not supported" });
                }

                if (data == null)
                {
                    return BadRequest(new { error = "Data cannot be null" });
                }

                return Ok(new
                {
                    valid = true,
                    tableName = tableName.ToUpper(),
                    message = "Data validation passed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating data for table {tableName}");
                return StatusCode(500, new { error = "Internal server error during validation" });
            }
        }
    }

    // =======================================
    // 🆕 Request DTOs for Extended Tables
    // =======================================

    public class ImportLN03RequestDto
    {
        public string? BatchId { get; set; }
        public DateTime? ImportDate { get; set; }
        public List<LN03History> Data { get; set; } = new List<LN03History>();
    }

    public class ImportEI01RequestDto
    {
        public string? BatchId { get; set; }
        public DateTime? ImportDate { get; set; }
        public List<EI01History> Data { get; set; } = new List<EI01History>();
    }

    public class ImportDPDARequestDto
    {
        public string? BatchId { get; set; }
        public DateTime? ImportDate { get; set; }
        public List<DPDAHistory> Data { get; set; } = new List<DPDAHistory>();
    }

    public class ImportBC57RequestDto
    {
        public string? BatchId { get; set; }
        public DateTime? ImportDate { get; set; }
        public List<BC57History> Data { get; set; } = new List<BC57History>();
    }
}
