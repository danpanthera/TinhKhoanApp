using Khoan.Api.Services;
using Xunit;

namespace Khoan.Api.Tests;

public class InMemoryImportMetricsTests
{
    [Fact]
    public void RecordsAndPrometheusFormatting_Work()
    {
        var m = new InMemoryImportMetrics();
        m.RecordBatch("DP01", 100, 123);
        m.RecordBatch("GL01", 50, 45);

        // Verify TotalRows aggregates
        Assert.True(m.TotalRows >= 150);

        // Verify Prometheus exposition matches new metric names
        var text = m.ToPrometheus();
        Assert.Contains("# HELP direct_import_total_rows", text);
        Assert.Contains("direct_import_total_rows{table=\"DP01\"}", text);
        Assert.Contains("direct_import_total_rows{table=\"GL01\"}", text);
        Assert.Contains("# HELP direct_import_last_duration_seconds", text);
    }
}
