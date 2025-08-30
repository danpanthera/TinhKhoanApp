using Xunit;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Tests;

public class KpiScoringTests
{
    [Theory]
    [InlineData(100, 100, 10, 10)] // đạt đúng target => full
    [InlineData(100, 120, 10, 10)] // vượt target => capped
    [InlineData(100, 50, 10, 5)]   // 50% target
    public void CalculateScore_BasicCases(decimal target, decimal actual, decimal maxScore, decimal expected)
    {
        var result = KpiScoring.CalculateScore(target, actual, maxScore);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateScore_ReturnsNull_WhenNoActual()
    {
        var result = KpiScoring.CalculateScore(100, null, 10);
        Assert.Null(result);
    }

    [Fact]
    public void CalculateScore_ReturnsNull_WhenTargetZero()
    {
        var result = KpiScoring.CalculateScore(0, 50, 10);
        Assert.Null(result);
    }
}
