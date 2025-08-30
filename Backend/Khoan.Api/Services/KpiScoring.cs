using System;

namespace TinhKhoanApp.Api.Services;

/// <summary>
/// Helper tính điểm KPI (Score) dựa trên Target, Actual và MaxScore.
/// </summary>
public static class KpiScoring
{
    /// <summary>
    /// Công thức: (Actual / Target) * MaxScore, cắt ngưỡng MaxScore, làm tròn 2 chữ số.
    /// Trả về null nếu thiếu dữ liệu hợp lệ.
    /// </summary>
    public static decimal? CalculateScore(decimal targetValue, decimal? actualValue, decimal maxScore)
    {
        if (targetValue <= 0 || !actualValue.HasValue)
            return null;

        if (maxScore <= 0)
            return 0; // phòng vệ – trường hợp cấu hình sai

        var raw = (actualValue.Value / targetValue) * maxScore;
        if (raw > maxScore) raw = maxScore;
        return Math.Round(raw, 2);
    }
}
