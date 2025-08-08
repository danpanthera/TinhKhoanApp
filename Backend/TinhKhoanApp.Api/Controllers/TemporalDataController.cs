using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models.Temporal;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemporalDataController : ControllerBase
    {
        private readonly ITemporalDataService _temporalDataService;
        private readonly ILogger<TemporalDataController> _logger;

        public TemporalDataController(ITemporalDataService temporalDataService, ILogger<TemporalDataController> logger)
        {
            _temporalDataService = temporalDataService;
            _logger = logger;
        }

        /// <summary>
        /// Import daily raw data with high performance temporal tracking
        /// Optimized for 100K-200K records/day
        /// </summary>
        [HttpPost("import")]
        public async Task<IActionResult> ImportDailyData([FromBody] TemporalImportRequest request)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var rowsProcessed = await _temporalDataService.ImportDailyDataAsync(request);
                var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                _logger.LogInformation("Successfully imported {RowsProcessed} records in {ProcessingTime}ms", 
                    rowsProcessed, processingTime);

                return Ok(new
                {
                    success = true,
                    rowsProcessed,
                    processingTimeMs = processingTime,
                    importDate = request.ImportDate,
                    dataType = request.DataType,
                    message = $"Successfully imported {rowsProcessed} records"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing temporal data for date {ImportDate}", request.ImportDate);
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    importDate = request.ImportDate
                });
            }
        }

        /// <summary>
        /// Get data as it existed at a specific point in time
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
                var data = await _temporalDataService.GetDataAsOfAsync(asOfDate, employeeCode, branchCode, kpiCode, startDate, endDate);
                
                return Ok(new
                {
                    success = true,
                    asOfDate,
                    recordCount = data.Count,
                    data,
                    filters = new
                    {
                        employeeCode,
                        branchCode,
                        kpiCode,
                        startDate,
                        endDate
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving temporal data as of {AsOfDate}", asOfDate);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Compare data between two points in time
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
                var comparisons = await _temporalDataService.CompareDataBetweenDatesAsync(date1, date2, employeeCode, branchCode, kpiCode);
                
                var summary = new
                {
                    totalChanges = comparisons.Count,
                    added = comparisons.Count(c => c.ChangeType == "ADDED"),
                    removed = comparisons.Count(c => c.ChangeType == "REMOVED"),
                    changed = comparisons.Count(c => c.ChangeType == "CHANGED"),
                    unchanged = comparisons.Count(c => c.ChangeType == "UNCHANGED")
                };

                return Ok(new
                {
                    success = true,
                    date1,
                    date2,
                    summary,
                    comparisons,
                    filters = new
                    {
                        employeeCode,
                        branchCode,
                        kpiCode
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing temporal data between {Date1} and {Date2}", date1, date2);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get complete history for a specific record
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetRecordHistory(
            [FromQuery] string employeeCode,
            [FromQuery] string kpiCode,
            [FromQuery] string branchCode,
            [FromQuery] DateTime? importDate = null)
        {
            try
            {
                var history = await _temporalDataService.GetRecordHistoryAsync(employeeCode, kpiCode, branchCode, importDate);
                
                return Ok(new
                {
                    success = true,
                    employeeCode,
                    kpiCode,
                    branchCode,
                    importDate,
                    historyCount = history.Count,
                    history
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving record history for {EmployeeCode}/{KpiCode}/{BranchCode}", 
                    employeeCode, kpiCode, branchCode);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get daily change summary report
        /// </summary>
        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailyChangeSummary(
            [FromQuery] DateTime reportDate,
            [FromQuery] string? branchCode = null)
        {
            try
            {
                var summary = await _temporalDataService.GetDailyChangeSummaryAsync(reportDate, branchCode);
                
                var totals = new
                {
                    totalBranches = summary.Count,
                    totalNewRecords = summary.Sum(s => s.NewRecords),
                    totalDeletedRecords = summary.Sum(s => s.DeletedRecords),
                    totalModifiedRecords = summary.Sum(s => s.ModifiedRecords),
                    totalUnchangedRecords = summary.Sum(s => s.UnchangedRecords)
                };

                return Ok(new
                {
                    success = true,
                    reportDate,
                    branchCode,
                    totals,
                    branchSummaries = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily change summary for {ReportDate}", reportDate);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get performance analytics with moving averages and trends
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
                var analytics = await _temporalDataService.GetPerformanceAnalyticsAsync(startDate, endDate, branchCode, employeeCode);
                
                var summary = new
                {
                    totalRecords = analytics.Count,
                    dateRange = new { startDate, endDate },
                    uniqueEmployees = analytics.Select(a => a.EmployeeCode).Distinct().Count(),
                    uniqueKpis = analytics.Select(a => a.KpiCode).Distinct().Count(),
                    avgValue = analytics.Any() ? analytics.Average(a => a.Value) : 0,
                    avgMovingAverage = analytics.Any() ? analytics.Average(a => a.MovingAverage7Days) : 0
                };

                return Ok(new
                {
                    success = true,
                    summary,
                    analytics,
                    filters = new
                    {
                        startDate,
                        endDate,
                        branchCode,
                        employeeCode
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance analytics for {StartDate} to {EndDate}", startDate, endDate);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Archive old temporal data for performance optimization
        /// </summary>
        [HttpPost("archive")]
        public async Task<IActionResult> ArchiveOldData(
            [FromQuery] int archiveMonthsOld = 12,
            [FromQuery] int deleteMonthsOld = 24)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var (rowsArchived, rowsDeleted) = await _temporalDataService.ArchiveOldDataAsync(archiveMonthsOld, deleteMonthsOld);
                var processingTime = (DateTime.UtcNow - startTime).TotalSeconds;

                _logger.LogInformation("Archiving completed: {RowsArchived} archived, {RowsDeleted} deleted in {ProcessingTime}s", 
                    rowsArchived, rowsDeleted, processingTime);

                return Ok(new
                {
                    success = true,
                    rowsArchived,
                    rowsDeleted,
                    processingTimeSeconds = processingTime,
                    archiveMonthsOld,
                    deleteMonthsOld,
                    message = $"Archived {rowsArchived} rows, deleted {rowsDeleted} old rows"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during archiving process");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Maintain temporal table indexes for optimal performance
        /// </summary>
        [HttpPost("maintain-indexes")]
        public async Task<IActionResult> MaintainIndexes()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                await _temporalDataService.MaintainIndexesAsync();
                var processingTime = (DateTime.UtcNow - startTime).TotalSeconds;

                return Ok(new
                {
                    success = true,
                    processingTimeSeconds = processingTime,
                    message = "Index maintenance completed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during index maintenance");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get temporal tables health check and statistics
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> GetHealthCheck()
        {
            try
            {
                var healthData = await _temporalDataService.GetHealthCheckAsync();
                
                return Ok(new
                {
                    success = true,
                    timestamp = DateTime.UtcNow,
                    healthData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during health check");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk import from JSON data (for testing and migration)
        /// </summary>
        [HttpPost("bulk-import")]
        public async Task<IActionResult> BulkImport([FromBody] List<TemporalImportRequest> requests)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var totalRows = 0;
                var successCount = 0;
                var errors = new List<string>();

                foreach (var request in requests)
                {
                    try
                    {
                        var rows = await _temporalDataService.ImportDailyDataAsync(request);
                        totalRows += rows;
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing {request.ImportDate}: {ex.Message}");
                    }
                }

                var processingTime = (DateTime.UtcNow - startTime).TotalSeconds;

                return Ok(new
                {
                    success = errors.Count == 0,
                    totalRequests = requests.Count,
                    successCount,
                    failureCount = requests.Count - successCount,
                    totalRowsProcessed = totalRows,
                    processingTimeSeconds = processingTime,
                    errors = errors.Any() ? errors : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk import");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
