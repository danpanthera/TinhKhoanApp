using Khoan.Api.Models;

namespace Khoan.Api.Data
{
    /// <summary>
    /// Seed data cho bảng KpiScoringRules với định nghĩa chuẩn theo database thực tế
    /// </summary>
    public static class SeedKpiScoringRules
    {
        public static void SeedKpiRules(ApplicationDbContext context)
        {
            if (context.KpiScoringRules.Any())
            {
                return; // Đã có dữ liệu rồi
            }

            var rules = new List<KpiScoringRule>
            {
                new KpiScoringRule
                {
                    KpiIndicatorName = "Nguồn vốn huy động cuối kỳ",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.0m,
                    PenaltyPoints = 0.5m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Nguồn vốn huy động BQ",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.5m,
                    PenaltyPoints = 0.75m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Tỷ lệ nợ xấu",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 2.0m,
                    PenaltyPoints = 1.0m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Thu dịch vụ",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.5m,
                    PenaltyPoints = 0.75m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Lợi nhuận",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 2.0m,
                    PenaltyPoints = 1.0m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Chi phí hoạt động",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.0m,
                    PenaltyPoints = 0.5m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Tăng trưởng dư nợ",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.2m,
                    PenaltyPoints = 0.6m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "CIR",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 0.8m,
                    PenaltyPoints = 0.4m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "ROA",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 0.5m,
                    PenaltyPoints = 0.25m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "ROE",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 0.3m,
                    PenaltyPoints = 0.15m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Phát sinh Thu nhập dịch vụ BQ",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.0m,
                    PenaltyPoints = 0.5m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Tăng trưởng dư nợ tín dụng trong tổng dư nợ",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.0m,
                    PenaltyPoints = 0.5m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Tỷ lệ nợ quá hạn từ 1-90 ngày",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.0m,
                    PenaltyPoints = 0.5m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new KpiScoringRule
                {
                    KpiIndicatorName = "Tỷ lệ tổn thất tín dụng",
                    RuleType = "COMPLETION_RATE",
                    BonusPoints = 1.0m,
                    PenaltyPoints = 0.5m,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };

            context.KpiScoringRules.AddRange(rules);
            context.SaveChanges();
        }
    }
}
