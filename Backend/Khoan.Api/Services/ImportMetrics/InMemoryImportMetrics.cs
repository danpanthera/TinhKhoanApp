using System.Collections.Concurrent;

namespace Khoan.Api.Services
{
    /// <summary>
    /// Lightweight in-memory metrics collector for import processes.
    /// Thread-safe & low overhead. Exposed via /metrics/import endpoint.
    /// </summary>
    public class InMemoryImportMetrics : DirectImportService.IImportMetrics
    {
        private class Stat
        {
            public long Imported; // total rows
            public double LastDurationSeconds; // last import duration
            public DateTime LastUpdatedUtc;
            public readonly Queue<(DateTime ts, int batchSize)> RecentBatches = new();
            public long ParseErrors; // total parse errors captured
        }

        private readonly ConcurrentDictionary<string, Stat> _stats = new(StringComparer.OrdinalIgnoreCase);

        // Adapter for DirectImportService.IImportMetrics expected API
        public void RecordBatch(string table, int rows, int milliseconds)
        {
            AddImported(table, rows);
            if (milliseconds > 0)
            {
                ObserveDuration(table, milliseconds / 1000.0);
            }
        }

        public long TotalRows
        {
            get
            {
                long total = 0;
                foreach (var st in _stats.Values)
                {
                    System.Threading.Interlocked.Add(ref total, System.Threading.Interlocked.Read(ref st.Imported));
                }
                return total;
            }
        }

        public void AddImported(string table, int count)
        {
            if (count <= 0) return;
            var st = _stats.GetOrAdd(table, _ => new Stat());
            Interlocked.Add(ref st.Imported, count);
            st.LastUpdatedUtc = DateTime.UtcNow;
            lock (st.RecentBatches)
            {
                st.RecentBatches.Enqueue((DateTime.UtcNow, count));
                while (st.RecentBatches.Count > 200) st.RecentBatches.Dequeue(); // ring buffer max 200 batches
            }
        }

        // Ghi nhận lỗi parse (row-level) để theo dõi chất lượng dữ liệu đầu vào
        public void AddParseError(string table, int count = 1)
        {
            if (count <= 0) return;
            var st = _stats.GetOrAdd(table, _ => new Stat());
            Interlocked.Add(ref st.ParseErrors, count);
            st.LastUpdatedUtc = DateTime.UtcNow;
        }

        public void ObserveDuration(string table, double seconds)
        {
            var st = _stats.GetOrAdd(table, _ => new Stat());
            st.LastDurationSeconds = seconds;
            st.LastUpdatedUtc = DateTime.UtcNow;
        }

        public object Snapshot()
        {
            return _stats.ToDictionary(kvp => kvp.Key, kvp => new
            {
                totalImported = kvp.Value.Imported,
                parseErrors = kvp.Value.ParseErrors,
                lastDurationSeconds = kvp.Value.LastDurationSeconds,
                lastUpdatedUtc = kvp.Value.LastUpdatedUtc
            });
        }

        public object Raw()
        {
            return _stats.ToDictionary(kvp => kvp.Key, kvp => new
            {
                batches = kvp.Value.RecentBatches.Select(b => new { timestamp = b.ts, rows = b.batchSize }).ToList(),
                parseErrors = kvp.Value.ParseErrors
            });
        }

        public string ToPrometheus()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# HELP direct_import_total_rows Tổng số dòng đã import theo bảng");
            sb.AppendLine("# TYPE direct_import_total_rows counter");
            foreach (var kv in _stats)
            {
                sb.Append("direct_import_total_rows{table=\"")
                  .Append(kv.Key)
                  .AppendLine("\"} " + kv.Value.Imported);
            }
                        sb.AppendLine("# HELP direct_import_parse_errors_total Tổng số lỗi parse (row) theo bảng");
                        sb.AppendLine("# TYPE direct_import_parse_errors_total counter");
                        foreach (var kv in _stats)
                        {
                                sb.Append("direct_import_parse_errors_total{table=\"")
                                    .Append(kv.Key)
                                    .AppendLine("\"} " + kv.Value.ParseErrors);
                        }
            sb.AppendLine("# HELP direct_import_last_duration_seconds Thời gian lần import cuối (giây)");
            sb.AppendLine("# TYPE direct_import_last_duration_seconds gauge");
            foreach (var kv in _stats)
            {
                sb.Append("direct_import_last_duration_seconds{table=\"")
                  .Append(kv.Key)
                  .AppendLine("\"} " + kv.Value.LastDurationSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }
    }
}
