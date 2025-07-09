using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Smart Data Import Controller - Automatically routes files to correct data tables
    /// Handles intelligent file processing based on filename patterns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SmartDataImportController : ControllerBase
    {
        private readonly ISmartDataImportService _smartImportService;
        private readonly ILogger<SmartDataImportController> _logger;
        private readonly ApplicationDbContext _context;

        public SmartDataImportController(
            ISmartDataImportService smartImportService,
            ILogger<SmartDataImportController> logger,
            ApplicationDbContext context)
        {
            _smartImportService = smartImportService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Smart import single file - automatically routes to correct data table
        /// Supports filename patterns like: 7800_DP01_20241231.csv, 7808_LN01_20241130.xlsx
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileSmart([FromForm] SmartImportRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                _logger.LogInformation("📤 Smart import started for file: {FileName}", request.File.FileName);

                var result = await _smartImportService.ImportFileSmartAsync(request.File, request.Category);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Successfully imported {result.ProcessedRecords} records to {result.TargetTable}",
                        fileName = result.FileName,
                        targetTable = result.TargetTable,
                        processedRecords = result.ProcessedRecords,
                        batchId = result.BatchId,
                        duration = result.Duration.TotalSeconds,
                        routingInfo = result.RoutingInfo
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.ErrorMessage,
                        fileName = result.FileName,
                        routingInfo = result.RoutingInfo
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Smart import failed for file: {FileName}",
                    request.File?.FileName ?? "unknown");
                return StatusCode(500, new { message = "Import failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Smart import multiple files
        /// </summary>
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleFilesSmart([FromForm] SmartImportMultipleRequest request)
        {
            try
            {
                if (request.Files == null || !request.Files.Any())
                {
                    return BadRequest(new { message = "No files provided" });
                }

                _logger.LogInformation("📤 Smart import started for {FileCount} files", request.Files.Count);

                var results = await _smartImportService.ImportMultipleFilesSmartAsync(request.Files);

                var successCount = results.Count(r => r.Success);
                var totalRecords = results.Sum(r => r.ProcessedRecords);

                return Ok(new
                {
                    success = successCount > 0,
                    message = $"Successfully imported {successCount}/{results.Count} files with {totalRecords} total records",
                    totalFiles = results.Count,
                    successCount = successCount,
                    totalRecords = totalRecords,
                    results = results.Select(r => new
                    {
                        fileName = r.FileName,
                        success = r.Success,
                        targetTable = r.TargetTable,
                        processedRecords = r.ProcessedRecords,
                        errorMessage = r.ErrorMessage,
                        duration = r.Duration.TotalSeconds,
                        routingInfo = r.RoutingInfo
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Multiple smart import failed");
                return StatusCode(500, new { message = "Multiple import failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Process existing ImportedDataRecord to data tables using smart routing
        /// </summary>
        [HttpPost("process-record/{importedDataRecordId}")]
        public async Task<IActionResult> ProcessExistingRecord(int importedDataRecordId)
        {
            try
            {
                _logger.LogInformation("🔄 Processing existing record ID: {ImportedDataRecordId}", importedDataRecordId);

                var result = await _smartImportService.ProcessImportedRecordToTablesAsync(importedDataRecordId);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Successfully processed {result.ProcessedRecords} records to {result.TargetTable}",
                        fileName = result.FileName,
                        targetTable = result.TargetTable,
                        processedRecords = result.ProcessedRecords,
                        batchId = result.BatchId,
                        duration = result.Duration.TotalSeconds,
                        routingInfo = result.RoutingInfo
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.ErrorMessage,
                        fileName = result.FileName,
                        routingInfo = result.RoutingInfo
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Processing existing record failed for ID: {ImportedDataRecordId}", importedDataRecordId);
                return StatusCode(500, new { message = "Processing failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Analyze file routing without actually importing
        /// </summary>
        [HttpPost("analyze-routing")]
        public async Task<IActionResult> AnalyzeFileRouting([FromBody] AnalyzeRoutingRequest request)
        {
            try
            {
                var routingInfo = await _smartImportService.AnalyzeFileRoutingAsync(request.FileName, request.Category);

                return Ok(new
                {
                    success = routingInfo.IsValid,
                    message = routingInfo.IsValid ? "File routing determined successfully" : "Could not determine routing",
                    routingInfo = routingInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ File routing analysis failed for: {FileName}", request.FileName);
                return StatusCode(500, new { message = "Analysis failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Get list of available data tables
        /// </summary>
        [HttpGet("available-tables")]
        public async Task<IActionResult> GetAvailableDataTables()
        {
            try
            {
                var tables = await _smartImportService.GetAvailableDataTablesAsync();

                return Ok(new
                {
                    success = true,
                    tables = tables,
                    count = tables.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to get available data tables");
                return StatusCode(500, new { message = "Failed to get tables", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify temporal tables and columnstore indexes setup
        /// </summary>
        [HttpGet("verify-temporal-tables")]
        public async Task<IActionResult> VerifyTemporalTables()
        {
            try
            {
                var result = await _smartImportService.VerifyTemporalTablesAsync();

                return Ok(new
                {
                    success = result.AllTablesValid,
                    message = result.AllTablesValid ? "All temporal tables are properly configured" : "Some temporal tables need setup",
                    checkTime = result.CheckTime,
                    allTablesValid = result.AllTablesValid,
                    tablesChecked = result.TablesChecked,
                    missingTables = result.MissingTables,
                    missingTemporalTables = result.MissingTemporalTables,
                    missingColumnstoreIndexes = result.MissingColumnstoreIndexes,
                    errorMessage = result.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to verify temporal tables");
                return StatusCode(500, new { message = "Verification failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Setup missing temporal tables and columnstore indexes
        /// </summary>
        [HttpPost("setup-temporal-tables")]
        public async Task<IActionResult> SetupTemporalTables()
        {
            try
            {
                _logger.LogInformation("🔧 Setting up temporal tables and columnstore indexes...");

                var result = await _smartImportService.SetupMissingTemporalTablesAsync();

                if (result.AllTablesValid)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "All temporal tables and columnstore indexes have been set up successfully",
                        checkTime = result.CheckTime,
                        tablesChecked = result.TablesChecked
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.ErrorMessage ?? "Failed to setup some temporal tables",
                        checkTime = result.CheckTime,
                        tablesChecked = result.TablesChecked,
                        missingTables = result.MissingTables,
                        missingTemporalTables = result.MissingTemporalTables,
                        missingColumnstoreIndexes = result.MissingColumnstoreIndexes
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to setup temporal tables");
                return StatusCode(500, new { message = "Setup failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Get routing examples and supported filename patterns
        /// </summary>
        [HttpGet("routing-examples")]
        public IActionResult GetRoutingExamples()
        {
            var examples = new
            {
                supportedPatterns = new[]
                {
                    "Standard format: {BranchCode}_{DataType}_{YYYYMMDD}.{ext}",
                    "Example: 7800_DP01_20241231.csv -> routes to DP01_New table",
                    "Example: 7808_LN01_20241130.xlsx -> routes to LN01 table"
                },
                dataTypes = new[]
                {
                    new { code = "DP01", table = "DP01_New", description = "Báo cáo tiền gửi" },
                    new { code = "LN01", table = "LN01", description = "Báo cáo tín dụng" },
                    new { code = "LN02", table = "LN02", description = "Báo cáo kỳ hạn thanh toán" },
                    new { code = "LN03", table = "LN03", description = "Báo cáo nợ XLRR" },
                    new { code = "GL01", table = "GL01", description = "Sổ cái tổng hợp" },
                    new { code = "GL41", table = "GL41", description = "Báo cáo tài khoản" },
                    new { code = "DB01", table = "DB01", description = "Báo cáo TSDB" },
                    new { code = "DPDA", table = "DPDA", description = "Báo cáo phát hành thẻ" },
                    new { code = "EI01", table = "EI01", description = "Báo cáo Mobile Banking" },
                    new { code = "KH03", table = "KH03", description = "Báo cáo khách hàng" },
                    new { code = "RR01", table = "RR01", description = "Báo cáo tỷ lệ" },
                    new { code = "DT_KHKD1", table = "DT_KHKD1", description = "Báo cáo kế hoạch kinh doanh" }
                },
                branchCodes = new[]
                {
                    new { code = "7800", name = "Chi nhánh Lai Châu" },
                    new { code = "7801", name = "Chi nhánh Bình Lư" },
                    new { code = "7802", name = "Chi nhánh Phong Thổ" },
                    new { code = "7803", name = "Chi nhánh Sin Hồ" },
                    new { code = "7804", name = "Chi nhánh Bum Tở" },
                    new { code = "7805", name = "Chi nhánh Than Uyên" },
                    new { code = "7806", name = "Chi nhánh Đoàn Kết" },
                    new { code = "7807", name = "Chi nhánh Tân Uyên" },
                    new { code = "7808", name = "Chi nhánh Nậm Hàng" },
                    new { code = "9999", name = "Hội sở" }
                }
            };

            return Ok(examples);
        }

        /// <summary>
        /// Debug endpoint - Check what data exists in DP01_New table
        /// </summary>
        [HttpGet("debug/dp01-data")]
        public async Task<IActionResult> GetDP01DebugData()
        {
            try
            {
                var totalRecords = await _context.DP01_News.CountAsync();
                var sampleRecords = await _context.DP01_News
                    .OrderByDescending(d => d.CreatedDate)
                    .Take(10)
                    .Select(d => new
                    {
                        d.Id,
                        d.NgayDL,
                        d.MA_CN,
                        d.TAI_KHOAN_HACH_TOAN,
                        d.CURRENT_BALANCE,
                        d.FileName,
                        d.CreatedDate
                    })
                    .ToListAsync();

                var dateGroups = await _context.DP01_News
                    .GroupBy(d => d.NgayDL)
                    .Select(g => new
                    {
                        NgayDL = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    totalRecords = totalRecords,
                    sampleRecords = sampleRecords,
                    dateGroups = dateGroups,
                    message = "DP01_New table debug data"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Simple database test
        /// </summary>
        [HttpGet("dbtest")]
        public async Task<ActionResult> DatabaseTest()
        {
            try
            {
                // Test very basic query
                var count = await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok(new { success = true, message = "Database connection working", result = count });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }

    }

    #region Request DTOs

    public class SmartImportRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? Category { get; set; }
    }

    public class SmartImportMultipleRequest
    {
        public List<IFormFile> Files { get; set; } = new();
    }

    public class AnalyzeRoutingRequest
    {
        public string FileName { get; set; } = null!;
        public string? Category { get; set; }
    }

    #endregion
}
