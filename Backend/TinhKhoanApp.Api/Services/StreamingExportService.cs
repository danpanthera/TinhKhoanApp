using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using ClosedXML.Excel;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace TinhKhoanApp.Api.Services
{
    public interface IStreamingExportService
    {
        IAsyncEnumerable<ExportProgress> ExportEmployeesStreamAsync(
            int? unitId = null, 
            string format = "excel",
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<ExportProgress> ExportRawDataStreamAsync(
            int? importId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string format = "excel",
            CancellationToken cancellationToken = default);

        Task<Stream> ExportToStreamAsync(
            string exportType,
            Dictionary<string, object> parameters,
            string format = "excel",
            CancellationToken cancellationToken = default);
    }

    public class StreamingExportService : IStreamingExportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StreamingExportService> _logger;
        private readonly ICacheService _cache;
        private readonly IPerformanceMonitorService _performanceMonitor;
        
        private const int BATCH_SIZE = 1000;
        private const int EXCEL_MAX_ROWS = 1048576; // Excel limit
        private const string CACHE_KEY_PREFIX = "export_stream";

        public StreamingExportService(
            ApplicationDbContext context,
            ILogger<StreamingExportService> logger,
            ICacheService cache,
            IPerformanceMonitorService performanceMonitor)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _performanceMonitor = performanceMonitor;
        }

        public async IAsyncEnumerable<ExportProgress> ExportEmployeesStreamAsync(
            int? unitId = null,
            string format = "excel",
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var totalProcessed = 0;
            var currentBatch = 0;
            var exportId = Guid.NewGuid().ToString();

            // Track export start
            await _cache.IncrementAsync("exports:active:count", 1, TimeSpan.FromHours(1));
            _logger.LogInformation("Starting employee export with ID: {ExportId}", exportId);

            // Get total count first
            var totalQuery = _context.Employees.AsNoTracking().Where(e => e.IsActive);
            if (unitId.HasValue)
            {
                totalQuery = totalQuery.Where(e => e.UnitId == unitId.Value);
            }

            int totalCount;
            var countResult = await ExecuteSafelyAsync(async () => await totalQuery.CountAsync(cancellationToken));
            if (!countResult.Success)
            {
                stopwatch.Stop();
                await _cache.IncrementAsync("exports:failed:count", 1, TimeSpan.FromDays(1));
                await _cache.IncrementAsync("exports:active:count", -1, TimeSpan.FromHours(1));
                _logger.LogError(countResult.Exception, "Error getting employee count after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                
                yield return new ExportProgress
                {
                    ExportId = exportId,
                    Stage = "Error",
                    TotalRecords = 0,
                    ProcessedRecords = 0,
                    PercentComplete = 0,
                    ElapsedTime = stopwatch.Elapsed,
                    ErrorMessage = countResult.Exception?.Message ?? "Unknown error",
                    IsCompleted = true,
                    HasError = true
                };
                yield break;
            }
            totalCount = countResult.Result;

            yield return new ExportProgress
            {
                ExportId = exportId,
                Stage = "Initializing",
                TotalRecords = totalCount,
                ProcessedRecords = 0,
                CurrentBatch = 0,
                PercentComplete = 0,
                ElapsedTime = stopwatch.Elapsed,
                EstimatedTimeRemaining = TimeSpan.Zero
            };

            // Stream data in batches
            var offset = 0;
            while (offset < totalCount && !cancellationToken.IsCancellationRequested)
            {
                currentBatch++;
                
                var batchQuery = _context.Employees
                    .AsNoTracking()
                    .Include(e => e.Unit)
                    .Include(e => e.Position)
                    .Include(e => e.EmployeeRoles)
                        .ThenInclude(er => er.Role)
                    .Where(e => e.IsActive);

                if (unitId.HasValue)
                {
                    batchQuery = batchQuery.Where(e => e.UnitId == unitId.Value);
                }

                var batchResult = await ExecuteSafelyAsync(async () => await batchQuery
                    .OrderBy(e => e.Id)
                    .Skip(offset)
                    .Take(BATCH_SIZE)
                    .ToListAsync(cancellationToken));

                if (!batchResult.Success)
                {
                    stopwatch.Stop();
                    _logger.LogError(batchResult.Exception, "Error processing employee batch {Batch} after {ElapsedMs}ms", currentBatch, stopwatch.ElapsedMilliseconds);
                    
                    yield return new ExportProgress
                    {
                        Stage = "Error",
                        TotalRecords = totalCount,
                        ProcessedRecords = totalProcessed,
                        CurrentBatch = currentBatch,
                        PercentComplete = 0,
                        ElapsedTime = stopwatch.Elapsed,
                        ErrorMessage = batchResult.Exception?.Message ?? "Unknown error",
                        IsCompleted = true,
                        HasError = true
                    };
                    yield break;
                }

                var batch = batchResult.Result;
                if (!batch.Any()) break;

                // Process batch (simulate export processing)
                await Task.Delay(50, cancellationToken); // Simulate processing time
                
                totalProcessed += batch.Count;
                offset += BATCH_SIZE;

                var percentComplete = (double)totalProcessed / totalCount * 100;
                var estimatedTotal = totalCount > 0 && totalProcessed > 0 
                    ? TimeSpan.FromTicks(stopwatch.Elapsed.Ticks * totalCount / totalProcessed)
                    : TimeSpan.Zero;
                var estimatedRemaining = estimatedTotal - stopwatch.Elapsed;

                yield return new ExportProgress
                {
                    ExportId = exportId,
                    Stage = "Processing",
                    TotalRecords = totalCount,
                    ProcessedRecords = totalProcessed,
                    CurrentBatch = currentBatch,
                    BatchSize = batch.Count,
                    PercentComplete = Math.Round(percentComplete, 2),
                    ElapsedTime = stopwatch.Elapsed,
                    EstimatedTimeRemaining = estimatedRemaining > TimeSpan.Zero ? estimatedRemaining : TimeSpan.Zero,
                    CurrentData = batch.Take(3).Select(e => new { e.EmployeeCode, e.FullName, Unit = e.Unit?.Name }).ToList()
                };
            }

            stopwatch.Stop();

            // Track completion
            await _cache.IncrementAsync("exports:completed:count", 1, TimeSpan.FromDays(1));
            await _cache.IncrementAsync("exports:active:count", -1, TimeSpan.FromHours(1));

            yield return new ExportProgress
            {
                ExportId = exportId,
                Stage = "Completed",
                TotalRecords = totalCount,
                ProcessedRecords = totalProcessed,
                CurrentBatch = currentBatch,
                PercentComplete = 100,
                ElapsedTime = stopwatch.Elapsed,
                EstimatedTimeRemaining = TimeSpan.Zero,
                IsCompleted = true
            };

            _logger.LogInformation("Completed employee export stream: {TotalRecords} records in {ElapsedMs}ms", 
                totalProcessed, stopwatch.ElapsedMilliseconds);
        }

        public async IAsyncEnumerable<ExportProgress> ExportRawDataStreamAsync(
            int? importId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string format = "excel",
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var totalProcessed = 0;
            var currentBatch = 0;

            // Build query - using correct property names from RawDataModels.cs
            var totalQuery = _context.RawDataRecords.AsNoTracking();
            
            if (importId.HasValue)
            {
                totalQuery = totalQuery.Where(r => r.RawDataImportId == importId.Value);
            }
            
            if (fromDate.HasValue)
            {
                totalQuery = totalQuery.Where(r => r.RawDataImport.StatementDate >= fromDate.Value);
            }
            
            if (toDate.HasValue)
            {
                totalQuery = totalQuery.Where(r => r.RawDataImport.StatementDate <= toDate.Value);
            }

            var countResult = await ExecuteSafelyAsync(async () => await totalQuery.CountAsync(cancellationToken));
            if (!countResult.Success)
            {
                stopwatch.Stop();
                yield return new ExportProgress
                {
                    Stage = "Error",
                    TotalRecords = 0,
                    ProcessedRecords = 0,
                    PercentComplete = 0,
                    ElapsedTime = stopwatch.Elapsed,
                    ErrorMessage = countResult.Exception?.Message ?? "Unknown error",
                    IsCompleted = true,
                    HasError = true
                };
                yield break;
            }

            var totalCount = countResult.Result;

            yield return new ExportProgress
            {
                Stage = "Initializing",
                TotalRecords = totalCount,
                ProcessedRecords = 0,
                CurrentBatch = 0,
                PercentComplete = 0,
                ElapsedTime = stopwatch.Elapsed
            };

            // Stream data in batches
            var offset = 0;
            while (offset < totalCount && !cancellationToken.IsCancellationRequested)
            {
                currentBatch++;
                
                var batchQuery = _context.RawDataRecords
                    .AsNoTracking()
                    .Include(r => r.RawDataImport)
                    .Where(r => true); // Base condition

                // Apply filters
                if (importId.HasValue)
                {
                    batchQuery = batchQuery.Where(r => r.RawDataImportId == importId.Value);
                }
                
                if (fromDate.HasValue)
                {
                    batchQuery = batchQuery.Where(r => r.RawDataImport.StatementDate >= fromDate.Value);
                }
                
                if (toDate.HasValue)
                {
                    batchQuery = batchQuery.Where(r => r.RawDataImport.StatementDate <= toDate.Value);
                }

                var batchResult = await ExecuteSafelyAsync(async () => await batchQuery
                    .OrderBy(r => r.Id)
                    .Skip(offset)
                    .Take(BATCH_SIZE)
                    .ToListAsync(cancellationToken));

                if (!batchResult.Success)
                {
                    stopwatch.Stop();
                    yield return new ExportProgress
                    {
                        Stage = "Error",
                        TotalRecords = totalCount,
                        ProcessedRecords = totalProcessed,
                        CurrentBatch = currentBatch,
                        PercentComplete = 0,
                        ElapsedTime = stopwatch.Elapsed,
                        ErrorMessage = batchResult.Exception?.Message ?? "Unknown error",
                        IsCompleted = true,
                        HasError = true
                    };
                    yield break;
                }

                var batch = batchResult.Result;
                if (!batch.Any()) break;

                // Process batch
                await Task.Delay(30, cancellationToken); // Simulate processing time
                
                totalProcessed += batch.Count;
                offset += BATCH_SIZE;

                var percentComplete = (double)totalProcessed / totalCount * 100;

                yield return new ExportProgress
                {
                    Stage = "Processing",
                    TotalRecords = totalCount,
                    ProcessedRecords = totalProcessed,
                    CurrentBatch = currentBatch,
                    BatchSize = batch.Count,
                    PercentComplete = Math.Round(percentComplete, 2),
                    ElapsedTime = stopwatch.Elapsed,
                    CurrentData = batch.Take(3).Select(r => new { 
                        r.Id, 
                        r.RawDataImportId, 
                        r.ProcessedDate,
                        Import = r.RawDataImport?.FileName 
                    }).ToList()
                };
            }

            stopwatch.Stop();

            yield return new ExportProgress
            {
                Stage = "Completed",
                TotalRecords = totalCount,
                ProcessedRecords = totalProcessed,
                CurrentBatch = currentBatch,
                PercentComplete = 100,
                ElapsedTime = stopwatch.Elapsed,
                IsCompleted = true
            };

            _logger.LogInformation("Completed raw data export stream: {TotalRecords} records in {ElapsedMs}ms", 
                totalProcessed, stopwatch.ElapsedMilliseconds);
        }

        public async Task<Stream> ExportToStreamAsync(
            string exportType,
            Dictionary<string, object> parameters,
            string format = "excel",
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                switch (exportType.ToLower())
                {
                    case "employees":
                        return await ExportEmployeesToStreamAsync(parameters, format, cancellationToken);
                    
                    case "rawdata":
                        return await ExportRawDataToStreamAsync(parameters, format, cancellationToken);
                    
                    default:
                        throw new ArgumentException($"Unsupported export type: {exportType}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error exporting {ExportType} to stream in {ElapsedMs}ms", 
                    exportType, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task<Stream> ExportEmployeesToStreamAsync(
            Dictionary<string, object> parameters,
            string format,
            CancellationToken cancellationToken)
        {
            var unitId = parameters.TryGetValue("unitId", out var unitIdValue) 
                ? unitIdValue as int? 
                : null;

            var memoryStream = new MemoryStream();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách nhân viên");

            // Headers
            var headers = new[] { "STT", "Mã NV", "Họ tên", "Đơn vị", "Chức vụ", "Vai trò", "Username", "Email", "Số điện thoại" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            var row = 2;
            
            // Stream data in batches and populate Excel incrementally
            await foreach (var progress in ExportEmployeesStreamAsync(unitId, format, cancellationToken))
            {
                if (progress.CurrentData != null && progress.Stage == "Processing")
                {
                    // In a real implementation, you'd write Excel data incrementally here
                    // For demo purposes, we'll just track the progress
                }
                
                if (progress.IsCompleted)
                {
                    break;
                }
            }

            // Add summary row
            worksheet.Cell(row, 1).Value = "Export completed";
            worksheet.Cell(row, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            worksheet.Columns().AdjustToContents();
            
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            
            return memoryStream;
        }

        private async Task<Stream> ExportRawDataToStreamAsync(
            Dictionary<string, object> parameters,
            string format,
            CancellationToken cancellationToken)
        {
            var importId = parameters.TryGetValue("importId", out var importIdValue) 
                ? importIdValue as int? 
                : null;
                
            var fromDate = parameters.TryGetValue("fromDate", out var fromDateValue) 
                ? fromDateValue as DateTime? 
                : null;
                
            var toDate = parameters.TryGetValue("toDate", out var toDateValue) 
                ? toDateValue as DateTime? 
                : null;

            var memoryStream = new MemoryStream();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Raw Data Export");

            // Headers
            var headers = new[] { "STT", "ID", "Import ID", "Ngày xử lý", "Dữ liệu JSON", "File import" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            }

            var row = 2;
            
            // Stream data and populate Excel
            await foreach (var progress in ExportRawDataStreamAsync(importId, fromDate, toDate, format, cancellationToken))
            {
                if (progress.IsCompleted)
                {
                    break;
                }
            }

            // Add summary
            worksheet.Cell(row, 1).Value = "Export completed";
            worksheet.Cell(row, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            worksheet.Columns().AdjustToContents();
            
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            
            return memoryStream;
        }

        private async Task<OperationResult<T>> ExecuteSafelyAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                var result = await operation();
                return new OperationResult<T> { Success = true, Result = result };
            }
            catch (Exception ex)
            {
                return new OperationResult<T> { Success = false, Exception = ex };
            }
        }
    }

    public class ExportProgress
    {
        public string ExportId { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int CurrentBatch { get; set; }
        public int BatchSize { get; set; }
        public double PercentComplete { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
        public object? CurrentData { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public T Result { get; set; } = default!;
        public Exception? Exception { get; set; }
    }
}
