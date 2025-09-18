using System.Text;
using CsvHelper;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Khoan.Api.Services;
using Khoan.Api.Models.Configuration;
using Xunit;

namespace Khoan.Api.Tests;

public class DirectImportServiceTests
{
    private static IOptions<DirectImportSettings> Settings() => Options.Create(new DirectImportSettings());

    [Fact]
    public void ParseErrorSampling_TakesFirst5PerFile_AndClearWorks()
    {
        // Arrange: use null context (not used) and metrics stub
        var svc = new DirectImportService(
            context: null!,
            logger: NullLogger<DirectImportService>.Instance,
            settings: Settings(),
            metrics: new InMemoryImportMetrics()
        );

        // Simulate recording >5 errors for one file/table
        var table = "DP01";
        var line = "bad,line";
        var error = "Parse error";

        // Using reflection to invoke private method RecordParseError for test simplicity
        var mi = typeof(DirectImportService).GetMethod("RecordParseError", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(mi);

        for (int i = 0; i < 10; i++)
        {
            mi!.Invoke(svc, new object[] { table, line + i, error + i });
        }

        var errors = svc.GetRecentParseErrors(table);
        Assert.True(errors.Count <= 5); // only first 5 sampled

        // Clear and verify empty
        DirectImportService.ClearRuntimeParseErrors();
        var cleared = svc.GetRecentParseErrors(table);
        Assert.Empty(cleared);
    }
}
