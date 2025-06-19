using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Runtime.CompilerServices;
using TinhKhoanApp.Api.Services;
using System.Diagnostics;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamingExportController : ControllerBase
    {
        private readonly IStreamingExportService _streamingExportService;
        private readonly ILogger<StreamingExportController> _logger;

        public StreamingExportController(
            IStreamingExportService streamingExportService,
            ILogger<StreamingExportController> logger)
        {
            _streamingExportService = streamingExportService;
            _logger = logger;
        }

        /// <summary>
        /// Export employees with real-time progress tracking
        /// </summary>
        /// <param name="unitId">Optional unit filter</param>
        /// <param name="format">Export format (excel, csv)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Streaming export progress</returns>
        [HttpGet("employees/stream")]
        [Produces("application/json")]
        public async IAsyncEnumerable<ExportProgress> StreamEmployeesExport(
            [FromQuery] int? unitId = null,
            [FromQuery] string format = "excel",
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            _logger.LogInformation("Starting employee export stream. UnitId: {UnitId}, Format: {Format}", 
                unitId, format);

            await foreach (var progress in _streamingExportService.ExportEmployeesStreamAsync(unitId, format, cancellationToken))
            {
                // Add server-side metadata
                progress.Metadata ??= new Dictionary<string, object>();
                progress.Metadata["ServerTime"] = DateTime.UtcNow;
                progress.Metadata["RequestId"] = HttpContext.TraceIdentifier;
                
                yield return progress;

                // Add small delay for demo purposes
                await Task.Delay(100, cancellationToken);
            }

            stopwatch.Stop();
            _logger.LogInformation("Completed employee export stream in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Export raw data with real-time progress tracking
        /// </summary>
        /// <param name="importId">Optional import filter</param>
        /// <param name="fromDate">Optional from date filter</param>
        /// <param name="toDate">Optional to date filter</param>
        /// <param name="format">Export format (excel, csv)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Streaming export progress</returns>
        [HttpGet("rawdata/stream")]
        [Produces("application/json")]
        public async IAsyncEnumerable<ExportProgress> StreamRawDataExport(
            [FromQuery] int? importId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string format = "excel",
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            _logger.LogInformation("Starting raw data export stream. ImportId: {ImportId}, FromDate: {FromDate}, ToDate: {ToDate}, Format: {Format}", 
                importId, fromDate, toDate, format);

            await foreach (var progress in _streamingExportService.ExportRawDataStreamAsync(importId, fromDate, toDate, format, cancellationToken))
            {
                // Add server-side metadata
                progress.Metadata ??= new Dictionary<string, object>();
                progress.Metadata["ServerTime"] = DateTime.UtcNow;
                progress.Metadata["RequestId"] = HttpContext.TraceIdentifier;
                
                yield return progress;

                // Add small delay for demo purposes
                await Task.Delay(50, cancellationToken);
            }

            stopwatch.Stop();
            _logger.LogInformation("Completed raw data export stream in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Export employees to downloadable file
        /// </summary>
        /// <param name="unitId">Optional unit filter</param>
        /// <param name="format">Export format (excel, csv)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File download</returns>
        [HttpGet("employees/download")]
        public async Task<IActionResult> DownloadEmployeesExport(
            [FromQuery] int? unitId = null,
            [FromQuery] string format = "excel",
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var parameters = new Dictionary<string, object>();
                if (unitId.HasValue)
                {
                    parameters["unitId"] = unitId.Value;
                }

                var stream = await _streamingExportService.ExportToStreamAsync("employees", parameters, format, cancellationToken);
                
                stopwatch.Stop();
                
                var fileName = $"DanhSach_NhanVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var contentType = format.ToLower() switch
                {
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "csv" => "text/csv",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation("Generated employee export file {FileName} in {ElapsedMs}ms", 
                    fileName, stopwatch.ElapsedMilliseconds);

                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error generating employee export in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { 
                    message = "Lỗi khi xuất dữ liệu nhân viên", 
                    error = ex.Message,
                    elapsedMs = stopwatch.ElapsedMilliseconds
                });
            }
        }

        /// <summary>
        /// Export raw data to downloadable file
        /// </summary>
        /// <param name="importId">Optional import filter</param>
        /// <param name="fromDate">Optional from date filter</param>
        /// <param name="toDate">Optional to date filter</param>
        /// <param name="format">Export format (excel, csv)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File download</returns>
        [HttpGet("rawdata/download")]
        public async Task<IActionResult> DownloadRawDataExport(
            [FromQuery] int? importId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string format = "excel",
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var parameters = new Dictionary<string, object>();
                if (importId.HasValue)
                {
                    parameters["importId"] = importId.Value;
                }
                if (fromDate.HasValue)
                {
                    parameters["fromDate"] = fromDate.Value;
                }
                if (toDate.HasValue)
                {
                    parameters["toDate"] = toDate.Value;
                }

                var stream = await _streamingExportService.ExportToStreamAsync("rawdata", parameters, format, cancellationToken);
                
                stopwatch.Stop();
                
                var fileName = $"RawData_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var contentType = format.ToLower() switch
                {
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "csv" => "text/csv",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation("Generated raw data export file {FileName} in {ElapsedMs}ms", 
                    fileName, stopwatch.ElapsedMilliseconds);

                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error generating raw data export in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { 
                    message = "Lỗi khi xuất dữ liệu thô", 
                    error = ex.Message,
                    elapsedMs = stopwatch.ElapsedMilliseconds
                });
            }
        }

        /// <summary>
        /// Get export progress by session ID (for tracking background exports)
        /// </summary>
        /// <param name="sessionId">Export session ID</param>
        /// <returns>Export progress</returns>
        [HttpGet("progress/{sessionId}")]
        public Task<IActionResult> GetExportProgress(string sessionId)
        {
            try
            {
                // In a real implementation, you'd store and retrieve progress from cache/database
                // For demo purposes, return a mock response
                var progress = new ExportProgress
                {
                    Stage = "Processing",
                    TotalRecords = 10000,
                    ProcessedRecords = 7500,
                    CurrentBatch = 8,
                    PercentComplete = 75.0,
                    ElapsedTime = TimeSpan.FromMinutes(2),
                    EstimatedTimeRemaining = TimeSpan.FromMinutes(1),
                    IsCompleted = false,
                    Metadata = new Dictionary<string, object>
                    {
                        ["SessionId"] = sessionId,
                        ["ServerTime"] = DateTime.UtcNow,
                        ["RequestId"] = HttpContext.TraceIdentifier
                    }
                };

                return Task.FromResult<IActionResult>(Ok(progress));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting export progress for session {SessionId}", sessionId);
                return Task.FromResult<IActionResult>(StatusCode(500, new { 
                    message = "Lỗi khi lấy tiến độ export", 
                    error = ex.Message 
                }));
            }
        }

        /// <summary>
        /// Cancel export by session ID
        /// </summary>
        /// <param name="sessionId">Export session ID</param>
        /// <returns>Cancellation result</returns>
        [HttpPost("cancel/{sessionId}")]
        public Task<IActionResult> CancelExport(string sessionId)
        {
            try
            {
                // In a real implementation, you'd manage cancellation tokens per session
                // For demo purposes, return a mock response
                _logger.LogInformation("Export cancellation requested for session {SessionId}", sessionId);

                return Task.FromResult<IActionResult>(Ok(new { 
                    message = "Export đã được hủy", 
                    sessionId = sessionId,
                    cancelledAt = DateTime.UtcNow
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling export for session {SessionId}", sessionId);
                return Task.FromResult<IActionResult>(StatusCode(500, new { 
                    message = "Lỗi khi hủy export", 
                    error = ex.Message 
                }));
            }
        }

        /// <summary>
        /// Get available export formats and their capabilities
        /// </summary>
        /// <returns>Export format information</returns>
        [HttpGet("formats")]
        public IActionResult GetExportFormats()
        {
            var formats = new[]
            {
                new {
                    Format = "excel",
                    DisplayName = "Microsoft Excel",
                    Extension = ".xlsx",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    MaxRows = 1048576,
                    SupportsFormatting = true,
                    SupportsMultipleSheets = true
                },
                new {
                    Format = "csv",
                    DisplayName = "Comma Separated Values",
                    Extension = ".csv",
                    MimeType = "text/csv",
                    MaxRows = int.MaxValue,
                    SupportsFormatting = false,
                    SupportsMultipleSheets = false
                }
            };

            return Ok(new { 
                formats = formats,
                serverTime = DateTime.UtcNow
            });
        }
    }
}
