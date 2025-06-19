using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Services
{
    public class UnitKpiScoringService
    {
        private readonly ApplicationDbContext _context;

        public UnitKpiScoringService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo bảng chấm điểm mới cho đơn vị trong kỳ
        /// </summary>
        public async Task<UnitKpiScoring> CreateScoringAsync(int unitKhoanAssignmentId, int khoanPeriodId, int unitId, string scoredBy)
        {
            var scoring = new UnitKpiScoring
            {
                UnitKhoanAssignmentId = unitKhoanAssignmentId,
                KhoanPeriodId = khoanPeriodId,
                UnitId = unitId,
                ScoredBy = scoredBy,
                ScoringDate = DateTime.Now,
                BaseScore = 0,
                AdjustmentScore = 0,
                TotalScore = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.UnitKpiScorings.Add(scoring);
            await _context.SaveChangesAsync();

            return scoring;
        }

        /// <summary>
        /// Cập nhật chấm điểm từ danh sách KPI indicators
        /// </summary>
        public async Task<UnitKpiScoring> UpdateScoringFromIndicatorsAsync(int scoringId, List<UnitKpiScoringDetail> details)
        {
            var scoring = await _context.UnitKpiScorings
                .Include(s => s.ScoringDetails)
                .Include(s => s.ScoringCriteria)
                .FirstOrDefaultAsync(s => s.Id == scoringId);

            if (scoring == null)
                throw new ArgumentException("Scoring record not found");

            // Xóa chi tiết cũ
            _context.UnitKpiScoringDetails.RemoveRange(scoring.ScoringDetails);

            // Thêm chi tiết mới và tính điểm
            foreach (var detail in details)
            {
                var scoringDetail = new UnitKpiScoringDetail
                {
                    UnitKpiScoringId = scoringId,
                    KpiDefinitionId = detail.KpiDefinitionId,
                    KpiIndicatorId = detail.KpiIndicatorId,
                    IndicatorName = detail.IndicatorName,
                    TargetValue = detail.TargetValue,
                    ActualValue = detail.ActualValue,
                    BaseScore = detail.BaseScore,
                    AdjustmentScore = 0,
                    IndicatorType = detail.IndicatorType,
                    ScoringFormula = detail.ScoringFormula,
                    CompletionRate = detail.ActualValue.HasValue && detail.TargetValue != 0 
                        ? (detail.ActualValue.Value / detail.TargetValue * 100)
                        : null
                };

                if (detail.ActualValue.HasValue)
                {
                    scoringDetail.AdjustmentScore = await CalculateQuantitativeScore(
                        scoringDetail.IndicatorName,
                        scoringDetail.TargetValue,
                        scoringDetail.ActualValue.Value,
                        scoringDetail.BaseScore);
                }

                scoringDetail.FinalScore = scoringDetail.BaseScore + scoringDetail.AdjustmentScore;
                scoringDetail.Score = scoringDetail.FinalScore;

                scoring.ScoringDetails.Add(scoringDetail);
            }

            // Tính tổng điểm
            scoring.BaseScore = scoring.ScoringDetails.Sum(d => d.BaseScore);
            scoring.AdjustmentScore = scoring.ScoringDetails.Sum(d => d.AdjustmentScore);
            scoring.TotalScore = scoring.BaseScore + scoring.AdjustmentScore;
            scoring.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return scoring;
        }

        public async Task UpdateComplianceViolationsAsync(int scoringId, List<UnitKpiScoringCriteria> violations)
        {
            var scoring = await _context.UnitKpiScorings
                .Include(s => s.ScoringCriteria)
                .FirstOrDefaultAsync(s => s.Id == scoringId);

            if (scoring == null)
                throw new ArgumentException("Scoring record not found");

            // Xóa vi phạm cũ
            _context.UnitKpiScoringCriterias.RemoveRange(scoring.ScoringCriteria);

            // Thêm vi phạm mới
            foreach (var violation in violations)
            {
                violation.UnitKpiScoringId = scoringId;
                scoring.ScoringCriteria.Add(violation);
            }

            // Tính điểm trừ vi phạm
            var violationPenalty = CalculateComplianceScore(violations);
            scoring.AdjustmentScore += violationPenalty;
            scoring.TotalScore = scoring.BaseScore + scoring.AdjustmentScore;
            scoring.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<List<UnitKpiScoring>> GetScoringsByPeriodAndUnitAsync(int periodId, int? unitId = null)
        {
            var query = _context.UnitKpiScorings
                .Include(s => s.Unit)
                .Include(s => s.ScoringDetails)
                .Include(s => s.ScoringCriteria)
                .Where(s => s.KhoanPeriodId == periodId);

            if (unitId.HasValue)
                query = query.Where(s => s.UnitId == unitId.Value);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Tính điểm cho chỉ tiêu định lượng dựa trên quy tắc cấu hình từ database
        /// </summary>
        public async Task<decimal> CalculateQuantitativeScore(string indicatorName, decimal targetValue, decimal actualValue, decimal baseScore, string unitType = "ALL")
        {
            if (targetValue == 0) return 0;

            // Tìm quy tắc tính điểm từ database
            var rule = await _context.KpiScoringRules
                .Where(r => r.IsActive)
                .Where(r => r.KpiIndicatorName == indicatorName)
                .FirstOrDefaultAsync();

            if (rule == null)
            {
                // Fallback về logic cũ nếu không tìm thấy quy tắc
                return CalculateQuantitativeScoreFallback(indicatorName, targetValue, actualValue, baseScore);
            }

            var completionRate = (actualValue / targetValue) * 100;
            var deviationPercent = completionRate - 100;

            // Simplified scoring based on actual KpiScoringRule properties
            if (rule.RuleType == "COMPLETION_RATE")
            {
                if (completionRate >= 100)
                {
                    // Award bonus points for over-achievement
                    return rule.BonusPoints;
                }
                else
                {
                    // Apply penalty for under-achievement
                    return -rule.PenaltyPoints;
                }
            }

            // Default fallback to old logic
            return CalculateQuantitativeScoreFallback(indicatorName, targetValue, actualValue, baseScore);
        }

        /// <summary>
        /// Logic cũ để fallback khi không tìm thấy quy tắc trong database
        /// </summary>
        private decimal CalculateQuantitativeScoreFallback(string indicatorName, decimal targetValue, decimal actualValue, decimal baseScore)
        {
            var completionRate = (actualValue / targetValue) * 100;
            var deviationPercent = completionRate - 100;

            return indicatorName switch
            {
                "Nguồn vốn huy động cuối kỳ" => CalculateLinearScore(deviationPercent, 10, 1),
                "Nguồn vốn huy động BQ" => CalculateLinearScore(deviationPercent, 5, 1.5m),
                "Tỷ lệ nợ xấu" => CalculateReverseLinearScore(deviationPercent, 10, -1),
                "Thu dịch vụ" => CalculateLinearScore(deviationPercent, 10, 1.5m),
                "Lợi nhuận" => CalculateLinearScore(deviationPercent, 15, 2),
                "Chi phí hoạt động" => CalculateReverseLinearScore(deviationPercent, 5, -1),
                "Tăng trưởng dư nợ" => CalculateLinearScore(deviationPercent, 8, 1.2m),
                "CIR" => CalculateReverseLinearScore(deviationPercent, 3, -0.8m),
                "ROA" => CalculateLinearScore(deviationPercent, 0.1m, 0.5m),
                "ROE" => CalculateLinearScore(deviationPercent, 1, 0.3m),
                _ => 0
            };
        }

        private decimal CalculateLinearScore(decimal deviationPercent, decimal thresholdPercent, decimal pointPerThreshold, decimal? maxScore = null, decimal? minScore = null)
        {
            if (thresholdPercent == 0) return 0;
            
            var score = (deviationPercent / thresholdPercent) * pointPerThreshold;
            
            // Áp dụng giới hạn min/max nếu có
            if (maxScore.HasValue && score > maxScore.Value)
                score = maxScore.Value;
            if (minScore.HasValue && score < minScore.Value)
                score = minScore.Value;
                
            return score;
        }

        private decimal CalculateReverseLinearScore(decimal deviationPercent, decimal thresholdPercent, decimal pointPerThreshold, decimal? maxScore = null, decimal? minScore = null)
        {
            if (thresholdPercent == 0) return 0;
            
            var score = (-deviationPercent / thresholdPercent) * Math.Abs(pointPerThreshold);
            
            // Áp dụng giới hạn min/max nếu có
            if (maxScore.HasValue && score > maxScore.Value)
                score = maxScore.Value;
            if (minScore.HasValue && score < minScore.Value)
                score = minScore.Value;
                
            return score;
        }

        private decimal CalculateThresholdScore(decimal completionRate, decimal requiredThreshold, decimal scoreForThreshold)
        {
            return completionRate >= requiredThreshold ? scoreForThreshold : 0;
        }

        private async Task<decimal> CalculateCustomScore(string formula, decimal targetValue, decimal actualValue, decimal completionRate, decimal deviationPercent)
        {
            // Placeholder cho công thức tùy chỉnh - có thể implement sau
            // Formula có thể chứa các biến: {target}, {actual}, {completion}, {deviation}
            if (string.IsNullOrEmpty(formula))
                return 0;

            // Ví dụ đơn giản - có thể mở rộng với expression evaluator
            return 0;
        }

        private decimal CalculateComplianceScore(List<UnitKpiScoringCriteria> violations)
        {
            decimal totalPenalty = 0;

            var hasWrittenViolation = violations.Any(v => v.ViolationLevel == "Written");
            var hasDisciplinaryViolation = violations.Any(v => v.ViolationLevel == "Disciplinary");

            if (hasDisciplinaryViolation)
                return -100;

            if (hasWrittenViolation)
                totalPenalty -= 10;

            foreach (var violation in violations)
            {
                totalPenalty += violation.ViolationLevel switch
                {
                    "Minor" => -2,
                    "Written" => -4,
                    "Disciplinary" => -10,
                    _ => 0
                };
            }

            return totalPenalty;
        }

        public async Task<UnitKpiScoring?> GetScoringByIdAsync(int id)
        {
            return await _context.UnitKpiScorings
                .Include(s => s.Unit)
                .Include(s => s.KhoanPeriod)
                .Include(s => s.ScoringDetails)
                .Include(s => s.ScoringCriteria)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> DeleteScoringAsync(int id)
        {
            var scoring = await _context.UnitKpiScorings.FindAsync(id);
            if (scoring == null) return false;

            _context.UnitKpiScorings.Remove(scoring);
            await _context.SaveChangesAsync();
            return true;
        }

        // Overloaded methods for backward compatibility
        private decimal CalculateLinearScore(decimal deviationPercent, decimal thresholdPercent, decimal pointPerThreshold)
        {
            return CalculateLinearScore(deviationPercent, thresholdPercent, pointPerThreshold, null, null);
        }

        private decimal CalculateReverseLinearScore(decimal deviationPercent, decimal thresholdPercent, decimal pointPerThreshold)
        {
            return CalculateReverseLinearScore(deviationPercent, thresholdPercent, pointPerThreshold, null, null);
        }
    }
}