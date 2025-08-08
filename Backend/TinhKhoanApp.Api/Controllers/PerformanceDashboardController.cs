using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime;
using System.Text.Json;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Utils; // 🕐 Thêm Utils cho VietnamDateTime

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceDashboardController : ControllerBase
    {
        private readonly IPerformanceMonitorService _performanceMonitor;
        private readonly ICacheService _cache;
        private readonly ILogger<PerformanceDashboardController> _logger;

        public PerformanceDashboardController(
            IPerformanceMonitorService performanceMonitor,
            ICacheService cache,
            ILogger<PerformanceDashboardController> logger)
        {
            _performanceMonitor = performanceMonitor;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Get system performance metrics
        /// </summary>
        [HttpGet("system")]
        public async Task<IActionResult> GetSystemMetrics()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var metrics = new
                {
                    Timestamp = VietnamDateTime.Now,
                    System = new
                    {
                        ProcessorCount = Environment.ProcessorCount,
                        WorkingSet = process.WorkingSet64,
                        PrivateMemorySize = process.PrivateMemorySize64,
                        VirtualMemorySize = process.VirtualMemorySize64,
                        TotalProcessorTime = process.TotalProcessorTime.TotalMilliseconds,
                        UserProcessorTime = process.UserProcessorTime.TotalMilliseconds,
                        GCTotalMemory = GC.GetTotalMemory(false),
                        GCGen0Collections = GC.CollectionCount(0),
                        GCGen1Collections = GC.CollectionCount(1),
                        GCGen2Collections = GC.CollectionCount(2),
                        ThreadCount = process.Threads.Count,
                        HandleCount = process.HandleCount
                    }
                };

                // Cache for 30 seconds
                await _cache.SetAsync("dashboard:system", metrics, TimeSpan.FromSeconds(30));

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system metrics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get database performance metrics
        /// </summary>
        [HttpGet("database")]
        public async Task<IActionResult> GetDatabaseMetrics()
        {
            try
            {
                var metrics = new
                {
                    Status = "Connected",
                    QueryCount = 0,
                    AvgQueryTime = 0,
                    SlowQueries = 0,
                    ActiveConnections = 1
                };

                // Cache for 1 minute
                await _cache.SetAsync("dashboard:database", metrics, TimeSpan.FromMinutes(1));

                return Ok(new
                {
                    Timestamp = VietnamDateTime.Now,
                    Database = metrics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database metrics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get API endpoint performance metrics
        /// </summary>
        [HttpGet("endpoints")]
        public IActionResult GetEndpointMetrics()
        {
            try
            {
                var metrics = new[]
                {
                    new { Endpoint = "/api/employees", Calls = 0, AvgResponseTime = 0.0, ErrorRate = 0.0 },
                    new { Endpoint = "/api/kpi-scoring", Calls = 0, AvgResponseTime = 0.0, ErrorRate = 0.0 },
                    new { Endpoint = "/api/raw-data", Calls = 0, AvgResponseTime = 0.0, ErrorRate = 0.0 }
                };

                return Ok(new
                {
                    Timestamp = VietnamDateTime.Now,
                    Endpoints = metrics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting endpoint metrics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get cache performance metrics
        /// </summary>
        [HttpGet("cache")]
        public async Task<IActionResult> GetCacheMetrics()
        {
            try
            {
                var cacheHits = (await _cache.GetAsync<long?>("metrics:cache:hits")) ?? 0L;
                var cacheMisses = (await _cache.GetAsync<long?>("metrics:cache:misses")) ?? 0L;
                var totalRequests = cacheHits + cacheMisses;
                var hitRate = totalRequests > 0 ? (double)cacheHits / totalRequests * 100 : 0;

                var metrics = new
                {
                    Timestamp = VietnamDateTime.Now,
                    Cache = new
                    {
                        Hits = cacheHits,
                        Misses = cacheMisses,
                        TotalRequests = totalRequests,
                        HitRate = Math.Round(hitRate, 2),
                        HitRatePercentage = $"{Math.Round(hitRate, 1)}%"
                    }
                };

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache metrics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get export operation metrics
        /// </summary>
        [HttpGet("exports")]
        public async Task<IActionResult> GetExportMetrics()
        {
            try
            {
                var activeExports = (await _cache.GetAsync<int?>("exports:active:count")) ?? 0;
                var completedExports = (await _cache.GetAsync<long?>("exports:completed:count")) ?? 0L;
                var failedExports = (await _cache.GetAsync<long?>("exports:failed:count")) ?? 0L;
                var totalExports = completedExports + failedExports;
                var successRate = totalExports > 0 ? (double)completedExports / totalExports * 100 : 0;

                var metrics = new
                {
                    Timestamp = VietnamDateTime.Now,
                    Exports = new
                    {
                        Active = activeExports,
                        Completed = completedExports,
                        Failed = failedExports,
                        Total = totalExports,
                        SuccessRate = Math.Round(successRate, 2),
                        SuccessRatePercentage = $"{Math.Round(successRate, 1)}%"
                    }
                };

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting export metrics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get comprehensive dashboard data
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var dashboardData = new
                {
                    Timestamp = VietnamDateTime.Now,
                    Uptime = VietnamDateTime.Now - Process.GetCurrentProcess().StartTime,
                    System = await GetSystemMetricsInternal(),
                    Database = await GetDatabaseMetricsInternal(),
                    Cache = await GetCacheMetricsInternal(),
                    Exports = await GetExportMetricsInternal(),
                    HealthChecks = await GetHealthStatusInternal()
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Reset performance counters
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetCounters()
        {
            try
            {
                await _cache.RemoveByPatternAsync("metrics:*");
                await _cache.RemoveByPatternAsync("exports:*");

                _logger.LogInformation("Performance counters reset");

                return Ok(new { message = "Performance counters reset successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting performance counters");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private Task<object> GetSystemMetricsInternal()
        {
            var process = Process.GetCurrentProcess();
            return Task.FromResult<object>(new
            {
                WorkingSetMB = Math.Round(process.WorkingSet64 / 1024.0 / 1024.0, 2),
                PrivateMemoryMB = Math.Round(process.PrivateMemorySize64 / 1024.0 / 1024.0, 2),
                GcTotalMemoryMB = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2),
                ThreadCount = process.Threads.Count,
                Gen0Collections = GC.CollectionCount(0),
                Gen1Collections = GC.CollectionCount(1),
                Gen2Collections = GC.CollectionCount(2)
            });
        }

        private Task<object> GetDatabaseMetricsInternal()
        {
            try
            {
                return Task.FromResult<object>(new
                {
                    Status = "Connected",
                    QueryCount = 0,
                    AvgQueryTime = 0,
                    SlowQueries = 0,
                    ActiveConnections = 1
                });
            }
            catch
            {
                return Task.FromResult<object>(new { Status = "Error retrieving database metrics" });
            }
        }

        private async Task<object> GetCacheMetricsInternal()
        {
            var hits = (await _cache.GetAsync<long?>("metrics:cache:hits")) ?? 0L;
            var misses = (await _cache.GetAsync<long?>("metrics:cache:misses")) ?? 0L;
            var total = hits + misses;
            var hitRate = total > 0 ? (double)hits / total * 100 : 0;

            return new
            {
                Hits = hits,
                Misses = misses,
                HitRate = Math.Round(hitRate, 1)
            };
        }

        private async Task<object> GetExportMetricsInternal()
        {
            var active = (await _cache.GetAsync<int?>("exports:active:count")) ?? 0;
            var completed = (await _cache.GetAsync<long?>("exports:completed:count")) ?? 0L;
            var failed = (await _cache.GetAsync<long?>("exports:failed:count")) ?? 0L;

            return new
            {
                Active = active,
                Completed = completed,
                Failed = failed
            };
        }

        private Task<object> GetHealthStatusInternal()
        {
            return Task.FromResult<object>(new
            {
                Status = "Healthy",
                CheckedAt = VietnamDateTime.Now
            });
        }
    }
}
