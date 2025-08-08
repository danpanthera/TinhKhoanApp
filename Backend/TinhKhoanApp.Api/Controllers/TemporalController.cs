using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models.Temporal;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemporalController : ControllerBase
    {
        private readonly ITemporalDataService _temporalService;
        private readonly ILogger<TemporalController> _logger;

        public TemporalController(ITemporalDataService temporalService, ILogger<TemporalController> logger)
        {
            _temporalService = temporalService;
            _logger = logger;
        }

        /// <summary>
        /// Import dữ liệu hàng ngày vào temporal tables
        /// </summary>
        [HttpPost("import")]
        public async Task<IActionResult> ImportDailyData([FromBody] TemporalImportRequest request)
        {
            try
            {
                if (request.Data == null || !request.Data.Any())
                {
                    return BadRequest("No data provided for import");
                }

                var recordsImported = await _temporalService.ImportDailyDataAsync(request);
                
                return Ok(new
                {
                    success = true,
                    message = $"Successfully imported {recordsImported} records",
                    recordsImported = recordsImported,
                    importDate = request.ImportDate,
                    batchId = $"API_{DateTime.UtcNow:yyyyMMdd_HHmmss}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing temporal data");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Import failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu tại một thời điểm cụ thể (temporal query)
        /// </summary>
        [HttpGet("as-of")]
        public async Task<IActionResult> GetDataAsOf(
            [FromQuery] DateTime asOfDate,
            [FromQuery] string? employeeCode = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? kpiCode = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var data = await _temporalService.GetDataAsOfAsync(asOfDate, employeeCode, branchCode, kpiCode, startDate, endDate);
                
                return Ok(new
                {
                    success = true,
                    asOfDate = asOfDate,
                    recordCount = data.Count,
                    data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving temporal data as of {AsOfDate}", asOfDate);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Query failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// So sánh dữ liệu giữa 2 thời điểm
        /// </summary>
        [HttpGet("compare")]
        public async Task<IActionResult> CompareDataBetweenDates(
            [FromQuery] DateTime date1,
            [FromQuery] DateTime date2,
            [FromQuery] string? employeeCode = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? kpiCode = null)
        {
            try
            {
                var comparison = await _temporalService.CompareDataBetweenDatesAsync(date1, date2, employeeCode, branchCode, kpiCode);
                
                return Ok(new
                {
                    success = true,
                    date1 = date1,
                    date2 = date2,
                    comparisonCount = comparison.Count,
                    changes = comparison.GroupBy(c => c.ChangeType).ToDictionary(g => g.Key, g => g.Count()),
                    data = comparison
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing temporal data between {Date1} and {Date2}", date1, date2);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Comparison failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử thay đổi của một record cụ thể
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetRecordHistory(
            [FromQuery] string employeeCode = "",
            [FromQuery] string kpiCode = "",
            [FromQuery] string branchCode = "",
            [FromQuery] DateTime? importDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(kpiCode) || string.IsNullOrEmpty(branchCode))
                {
                    return BadRequest("KpiCode and BranchCode are required");
                }

                var history = await _temporalService.GetRecordHistoryAsync(employeeCode, kpiCode, branchCode, importDate);
                
                return Ok(new
                {
                    success = true,
                    employeeCode = employeeCode,
                    kpiCode = kpiCode,
                    branchCode = branchCode,
                    historyCount = history.Count,
                    history = history
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving record history for {EmployeeCode}/{KpiCode}/{BranchCode}", 
                    employeeCode, kpiCode, branchCode);
                return StatusCode(500, new
                {
                    success = false,
                    message = "History query failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tổng hợp thay đổi theo ngày
        /// </summary>
        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailyChangeSummary(
            [FromQuery] DateTime reportDate,
            [FromQuery] string? branchCode = null)
        {
            try
            {
                var summary = await _temporalService.GetDailyChangeSummaryAsync(reportDate, branchCode);
                
                return Ok(new
                {
                    success = true,
                    reportDate = reportDate,
                    branchCode = branchCode,
                    summaryCount = summary.Count,
                    totalChanges = summary.Sum(s => s.NewRecords + s.ModifiedRecords + s.DeletedRecords),
                    summary = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily change summary for {ReportDate}", reportDate);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Daily summary query failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy phân tích hiệu suất với moving averages và trends
        /// </summary>
        [HttpGet("analytics")]
        public async Task<IActionResult> GetPerformanceAnalytics(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? employeeCode = null)
        {
            try
            {
                var analytics = await _temporalService.GetPerformanceAnalyticsAsync(startDate, endDate, branchCode, employeeCode);
                
                return Ok(new
                {
                    success = true,
                    startDate = startDate,
                    endDate = endDate,
                    analyticsCount = analytics.Count,
                    dateRange = (endDate - startDate).Days,
                    analytics = analytics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance analytics for {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Analytics query failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Health check cho temporal tables system
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> GetHealthCheck()
        {
            try
            {
                var healthData = await _temporalService.GetHealthCheckAsync();
                
                return Ok(new
                {
                    success = true,
                    message = "Temporal tables health check completed",
                    timestamp = DateTime.UtcNow,
                    healthData = healthData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing temporal health check");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Health check failed",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Test import với sample data để kiểm tra temporal functionality
        /// </summary>
        [HttpPost("test-import")]
        public async Task<IActionResult> TestImport()
        {
            try
            {
                var testRequest = new TemporalImportRequest
                {
                    ImportDate = DateTime.Today,
                    ImportSource = "TEST_API",
                    DataType = "TEST_QUARTERLY",
                    FileName = "test_temporal_import.json",
                    ImportedBy = "SYSTEM_TEST",
                    Data = new List<RawDataImportDto>
                    {
                        new RawDataImportDto
                        {
                            BranchCode = "CNH_TEST",
                            DepartmentCode = "DEPT_TEST",
                            EmployeeCode = "EMP_TEST_001",
                            KpiCode = "TEST_KPI_001",
                            KpiName = "Test Capital Source",
                            Value = 1000.5m,
                            Unit = "Billion VND",
                            Note = "Test temporal import data"
                        },
                        new RawDataImportDto
                        {
                            BranchCode = "CNH_TEST",
                            DepartmentCode = "DEPT_TEST",
                            EmployeeCode = "EMP_TEST_002",
                            KpiCode = "TEST_KPI_002",
                            KpiName = "Test Debt Balance",
                            Value = 800.3m,
                            Unit = "Billion VND",
                            Note = "Test temporal import data"
                        }
                    }
                };

                var recordsImported = await _temporalService.ImportDailyDataAsync(testRequest);
                
                return Ok(new
                {
                    success = true,
                    message = "Test import completed successfully",
                    recordsImported = recordsImported,
                    testData = testRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing test temporal import");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Test import failed",
                    error = ex.Message
                });
            }
        }
    }
}
