using System.Diagnostics;
using System.Collections.Concurrent;

namespace TinhKhoanApp.Api.Services
{
    public interface IPerformanceMonitorService
    {
        IDisposable TrackOperation(string operationName, Dictionary<string, object>? tags = null);
        Task<PerformanceStats> GetStatsAsync(string? operationName = null);
        Task LogSlowOperationsAsync(int thresholdMs = 1000);
        void RecordMetric(string metricName, double value, Dictionary<string, object>? tags = null);
    }

    public class PerformanceMonitorService : IPerformanceMonitorService
    {
        private readonly ILogger<PerformanceMonitorService> _logger;
        private readonly ConcurrentDictionary<string, OperationStats> _operationStats;
        private readonly ConcurrentQueue<PerformanceEvent> _recentEvents;
        private const int MAX_RECENT_EVENTS = 1000;

        public PerformanceMonitorService(ILogger<PerformanceMonitorService> logger)
        {
            _logger = logger;
            _operationStats = new ConcurrentDictionary<string, OperationStats>();
            _recentEvents = new ConcurrentQueue<PerformanceEvent>();
        }

        public IDisposable TrackOperation(string operationName, Dictionary<string, object>? tags = null)
        {
            return new OperationTracker(operationName, tags, this, _logger);
        }

        public Task<PerformanceStats> GetStatsAsync(string? operationName = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(operationName))
                {
                    if (_operationStats.TryGetValue(operationName, out var stats))
                    {
                        return Task.FromResult(new PerformanceStats
                        {
                            OperationName = operationName,
                            TotalCalls = stats.TotalCalls,
                            AverageMs = stats.TotalMs / Math.Max(stats.TotalCalls, 1),
                            MinMs = stats.MinMs,
                            MaxMs = stats.MaxMs,
                            ErrorCount = stats.ErrorCount,
                            LastUpdated = stats.LastUpdated
                        });
                    }
                }

                // Return aggregated stats
                var allStats = _operationStats.Values.ToList();
                var aggregated = new PerformanceStats
                {
                    OperationName = "All Operations",
                    TotalCalls = allStats.Sum(s => s.TotalCalls),
                    AverageMs = allStats.Any() ? allStats.Average(s => s.TotalMs / Math.Max(s.TotalCalls, 1)) : 0,
                    MinMs = allStats.Any() ? allStats.Min(s => s.MinMs) : 0,
                    MaxMs = allStats.Any() ? allStats.Max(s => s.MaxMs) : 0,
                    ErrorCount = allStats.Sum(s => s.ErrorCount),
                    LastUpdated = allStats.Any() ? allStats.Max(s => s.LastUpdated) : DateTime.MinValue,
                    DetailedStats = _operationStats.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new PerformanceStats
                        {
                            OperationName = kvp.Key,
                            TotalCalls = kvp.Value.TotalCalls,
                            AverageMs = kvp.Value.TotalMs / Math.Max(kvp.Value.TotalCalls, 1),
                            MinMs = kvp.Value.MinMs,
                            MaxMs = kvp.Value.MaxMs,
                            ErrorCount = kvp.Value.ErrorCount,
                            LastUpdated = kvp.Value.LastUpdated
                        })
                };

                return Task.FromResult(aggregated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance stats");
                return Task.FromResult(new PerformanceStats { OperationName = "Error" });
            }
        }

        public Task LogSlowOperationsAsync(int thresholdMs = 1000)
        {
            try
            {
                var slowOps = _operationStats
                    .Where(kvp => kvp.Value.MaxMs > thresholdMs)
                    .OrderByDescending(kvp => kvp.Value.MaxMs)
                    .Take(10)
                    .ToList();

                if (slowOps.Any())
                {
                    _logger.LogWarning("Slow operations detected (>{ThresholdMs}ms):", thresholdMs);
                    
                    foreach (var op in slowOps)
                    {
                        _logger.LogWarning(
                            "Operation: {OperationName}, Max: {MaxMs}ms, Avg: {AvgMs}ms, Calls: {TotalCalls}",
                            op.Key,
                            op.Value.MaxMs,
                            op.Value.TotalMs / Math.Max(op.Value.TotalCalls, 1),
                            op.Value.TotalCalls);
                    }
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging slow operations");
                return Task.CompletedTask;
            }
        }

        public void RecordMetric(string metricName, double value, Dictionary<string, object>? tags = null)
        {
            try
            {
                var perfEvent = new PerformanceEvent
                {
                    MetricName = metricName,
                    Value = value,
                    Tags = tags ?? new Dictionary<string, object>(),
                    Timestamp = DateTime.UtcNow
                };

                _recentEvents.Enqueue(perfEvent);

                // Keep only recent events
                while (_recentEvents.Count > MAX_RECENT_EVENTS)
                {
                    _recentEvents.TryDequeue(out _);
                }

                _logger.LogDebug("Recorded metric {MetricName}: {Value}", metricName, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording metric {MetricName}", metricName);
            }
        }

        internal void RecordOperation(string operationName, long elapsedMs, bool isError, Dictionary<string, object>? tags)
        {
            try
            {
                _operationStats.AddOrUpdate(operationName,
                    new OperationStats
                    {
                        TotalCalls = 1,
                        TotalMs = elapsedMs,
                        MinMs = elapsedMs,
                        MaxMs = elapsedMs,
                        ErrorCount = isError ? 1 : 0,
                        LastUpdated = DateTime.UtcNow
                    },
                    (key, existing) =>
                    {
                        existing.TotalCalls++;
                        existing.TotalMs += elapsedMs;
                        existing.MinMs = Math.Min(existing.MinMs, elapsedMs);
                        existing.MaxMs = Math.Max(existing.MaxMs, elapsedMs);
                        if (isError) existing.ErrorCount++;
                        existing.LastUpdated = DateTime.UtcNow;
                        return existing;
                    });

                // Record as metric for detailed tracking
                RecordMetric($"operation.{operationName}.duration", elapsedMs, tags);

                // Alert if too slow
                if (elapsedMs > 1000)
                {
                    _logger.LogWarning(
                        "Slow operation detected: {OperationName} took {ElapsedMs}ms",
                        operationName,
                        elapsedMs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording operation stats for {OperationName}", operationName);
            }
        }

        private class OperationTracker : IDisposable
        {
            private readonly string _operationName;
            private readonly Dictionary<string, object>? _tags;
            private readonly PerformanceMonitorService _monitor;
            private readonly ILogger _logger;
            private readonly Stopwatch _stopwatch;
            private bool _disposed = false;

            public OperationTracker(
                string operationName,
                Dictionary<string, object>? tags,
                PerformanceMonitorService monitor,
                ILogger logger)
            {
                _operationName = operationName;
                _tags = tags;
                _monitor = monitor;
                _logger = logger;
                _stopwatch = Stopwatch.StartNew();

                _logger.LogDebug("Started tracking operation: {OperationName}", operationName);
            }

            public void Dispose()
            {
                if (_disposed) return;

                try
                {
                    _stopwatch.Stop();
                    
                    var elapsedMs = _stopwatch.ElapsedMilliseconds;
                    
                    _logger.LogDebug(
                        "Operation {OperationName} completed in {ElapsedMs}ms",
                        _operationName,
                        elapsedMs);

                    _monitor.RecordOperation(_operationName, elapsedMs, false, _tags);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing operation tracker for {OperationName}", _operationName);
                    _monitor.RecordOperation(_operationName, _stopwatch.ElapsedMilliseconds, true, _tags);
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }

    public class OperationStats
    {
        public long TotalCalls { get; set; }
        public long TotalMs { get; set; }
        public long MinMs { get; set; } = long.MaxValue;
        public long MaxMs { get; set; }
        public long ErrorCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class PerformanceStats
    {
        public string OperationName { get; set; } = string.Empty;
        public long TotalCalls { get; set; }
        public double AverageMs { get; set; }
        public long MinMs { get; set; }
        public long MaxMs { get; set; }
        public long ErrorCount { get; set; }
        public DateTime LastUpdated { get; set; }
        public Dictionary<string, PerformanceStats>? DetailedStats { get; set; }
    }

    public class PerformanceEvent
    {
        public string MetricName { get; set; } = string.Empty;
        public double Value { get; set; }
        public Dictionary<string, object> Tags { get; set; } = new Dictionary<string, object>();
        public DateTime Timestamp { get; set; }
    }
}
