using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Khoan.Api.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace Khoan.Api.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;
        private readonly IConfiguration _configuration;

        public DatabaseHealthCheck(ApplicationDbContext context, ILogger<DatabaseHealthCheck> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var data = new Dictionary<string, object>();

                // Step 1: Fast server-level check against master to rule out TCP/DNS issues
                var masterConnMs = -1L;
                Exception? masterEx = null;
                try
                {
                    var cs = _configuration.GetConnectionString("DefaultConnection");
                    var csb = new SqlConnectionStringBuilder(cs)
                    {
                        InitialCatalog = "master",
                        ConnectTimeout = 5,
                        ConnectRetryCount = 0,
                        ConnectRetryInterval = 1
                    };
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    await using var conn = new SqlConnection(csb.ConnectionString);
                    await conn.OpenAsync(cancellationToken);
                    sw.Stop();
                    masterConnMs = sw.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    masterEx = ex;
                    _logger.LogError(ex, "Master connectivity check failed");
                }

                // Step 2: Default DB check via EF DbContext (TinhKhoanDB)
                var dbConnMs = -1L;
                Exception? dbEx = null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    await _context.Database.CanConnectAsync(cancellationToken);
                    sw.Stop();
                    dbConnMs = sw.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    dbEx = ex;
                    _logger.LogError(ex, "Default DB connectivity check failed");
                }

                data["MasterConnectMs"] = masterConnMs;
                data["DbConnectMs"] = dbConnMs;
                data["DatabaseProvider"] = _context.Database.ProviderName ?? "Unknown";
                data["LastChecked"] = DateTime.UtcNow;
                if (masterEx != null) data["MasterError"] = masterEx.Message;
                if (dbEx != null) data["DbError"] = dbEx.Message;

                if (masterEx != null)
                {
                    return HealthCheckResult.Unhealthy("Cannot reach SQL Server (master)", masterEx, data);
                }

                if (dbEx != null)
                {
                    return HealthCheckResult.Unhealthy("Default database is unreachable", dbEx, data);
                }

                if (dbConnMs > 5000)
                {
                    return HealthCheckResult.Degraded("Default database connection is slow", null, data);
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
                var recentImports = await _context.ImportedDataRecords
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
                    tags: new[] { "cache", "ready" });
            // Tạm comment performance check vì CompressedData column issue
            // .AddCheck<PerformanceHealthCheck>("performance",
            //     failureStatus: HealthStatus.Degraded,
            //     tags: new[] { "performance", "ready" });

            return services;
        }
    }
}
