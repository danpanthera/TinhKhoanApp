using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(ApplicationDbContext context, ILogger<DatabaseHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // Test database connection
                await _context.Database.CanConnectAsync(cancellationToken);
                
                // Test a simple query
                var recordCount = await _context.RawDataImports.CountAsync(cancellationToken);
                
                stopwatch.Stop();
                
                var data = new Dictionary<string, object>
                {
                    ["ConnectionTime"] = $"{stopwatch.ElapsedMilliseconds}ms",
                    ["TotalImports"] = recordCount,
                    ["DatabaseProvider"] = _context.Database.ProviderName ?? "Unknown",
                    ["LastChecked"] = DateTime.UtcNow
                };

                if (stopwatch.ElapsedMilliseconds > 5000) // 5 seconds threshold
                {
                    return HealthCheckResult.Degraded("Database connection is slow", null, data);
                }

                return HealthCheckResult.Healthy("Database is healthy", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
            }
        }
    }

    public class MemoryCacheHealthCheck : IHealthCheck
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheHealthCheck> _logger;

        public MemoryCacheHealthCheck(IMemoryCache cache, ILogger<MemoryCacheHealthCheck> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Test cache functionality
                var testKey = $"health_check_{Guid.NewGuid()}";
                var testValue = DateTime.UtcNow;
                
                _cache.Set(testKey, testValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10),
                    Size = 1 // Specify cache entry size
                });
                var retrievedValue = _cache.Get<DateTime?>(testKey);
                _cache.Remove(testKey);

                if (retrievedValue == testValue)
                {
                    var data = new Dictionary<string, object>
                    {
                        ["CacheWorking"] = true,
                        ["LastChecked"] = DateTime.UtcNow
                    };

                    return Task.FromResult(HealthCheckResult.Healthy("Memory cache is working", data));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Memory cache read/write test failed"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Memory cache health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy("Memory cache is unhealthy", ex));
            }
        }
    }

    public class PerformanceHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PerformanceHealthCheck> _logger;

        public PerformanceHealthCheck(ApplicationDbContext context, ILogger<PerformanceHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // Test optimized query performance
                var recentImports = await _context.RawDataImports
                    .OrderByDescending(x => x.ImportDate)
                    .Take(10)
                    .ToListAsync(cancellationToken);

                stopwatch.Stop();
                var queryTime = stopwatch.ElapsedMilliseconds;

                // Get system memory info
                var memoryUsage = GC.GetTotalMemory(false);
                var memoryUsageMB = memoryUsage / (1024 * 1024);

                var data = new Dictionary<string, object>
                {
                    ["QueryTime"] = $"{queryTime}ms",
                    ["MemoryUsageMB"] = memoryUsageMB,
                    ["LastChecked"] = DateTime.UtcNow,
                    ["RecentImportsCount"] = recentImports.Count
                };

                // Performance thresholds
                if (queryTime > 1000) // 1 second
                {
                    return HealthCheckResult.Degraded("Query performance is slow", null, data);
                }

                if (memoryUsageMB > 500) // 500 MB
                {
                    return HealthCheckResult.Degraded("High memory usage detected", null, data);
                }

                return HealthCheckResult.Healthy("Performance is good", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Performance health check failed");
                return HealthCheckResult.Unhealthy("Performance check failed", ex);
            }
        }
    }

    // Custom health check response writer
    public static class HealthCheckExtensions
    {
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = result.Status.ToString(),
                timestamp = DateTime.UtcNow,
                totalDuration = result.TotalDuration.TotalMilliseconds,
                checks = result.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration.TotalMilliseconds,
                    description = entry.Value.Description,
                    data = entry.Value.Data,
                    exception = entry.Value.Exception?.Message
                })
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            }));
        }

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database", 
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "ready" })
                .AddCheck<MemoryCacheHealthCheck>("cache",
                    failureStatus: HealthStatus.Degraded, 
                    tags: new[] { "cache", "ready" })
                .AddCheck<PerformanceHealthCheck>("performance",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "performance", "ready" });

            return services;
        }
    }
}
