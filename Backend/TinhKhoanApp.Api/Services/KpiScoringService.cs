using TinhKhoanApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Services
{
    // Service xử lý chấm điểm KPI với logic phân loại chính xác
    public interface IKpiScoringService
    {
        Task<KpiScoringResult> UpdateManualScoreAsync(int targetId, decimal actualValue);
        Task<KpiScoringResult> BulkImportQuantitativeScoresAsync(List<QuantitativeScoreDto> scores);
        Task<KpiScoringResult> CalculateRatioKpiAsync(int targetId, decimal numerator, decimal denominator);
        KpiInputType GetKpiInputType(KpiIndicator indicator);
    }

    public class KpiScoringService : IKpiScoringService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiScoringService> _logger;

        public KpiScoringService(ApplicationDbContext context, ILogger<KpiScoringService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 🎯 Phân loại KPI để xử lý đúng scoring method
        public KpiInputType GetKpiInputType(KpiIndicator indicator)
        {
            // Định lượng tuyệt đối: ValueType = NUMBER
            if (indicator.ValueType == KpiValueType.NUMBER)
            {
                return KpiInputType.QUANTITATIVE_ABSOLUTE;
            }

            // Cả 2 loại còn lại đều có ValueType = PERCENTAGE
            if (indicator.ValueType == KpiValueType.PERCENTAGE)
            {
                // 📊 Kiểm tra xem có phải là tỷ lệ tính toán không
                var calculatedRatioKpis = new[] { "TYLENOXAU", "PHATTRIENKHACHHANG" };
                var indicatorCode = GetKpiCodeByName(indicator.IndicatorName);
                
                if (calculatedRatioKpis.Contains(indicatorCode))
                {
                    return KpiInputType.QUANTITATIVE_RATIO; // Cần tính từ công thức
                }
                else
                {
                    return KpiInputType.QUALITATIVE; // Chấm tay
                }
            }

            return KpiInputType.QUALITATIVE; // Default
        }

        // 🔍 Helper method để lấy mã KPI từ tên (có thể cần cải thiện)
        private string GetKpiCodeByName(string indicatorName)
        {
            var ratioKeywords = new Dictionary<string, string>
            {
                { "Tỷ lệ nợ xấu", "TYLENOXAU" },
                { "Phát triển khách hàng mới", "PHATTRIENKHACHHANG" }
            };

            foreach (var keyword in ratioKeywords)
            {
                if (indicatorName.Contains(keyword.Key))
                {
                    return keyword.Value;
                }
            }

            return "UNKNOWN";
        }

        // 📝 Chấm điểm thủ công cho KPI định tính
        public async Task<KpiScoringResult> UpdateManualScoreAsync(int targetId, decimal actualValue)
        {
            var result = new KpiScoringResult();

            try
            {
                var target = await _context.EmployeeKpiTargets
                    .Include(t => t.Indicator)
                    .FirstOrDefaultAsync(t => t.Id == targetId);

                if (target == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Không tìm thấy KPI target";
                    return result;
                }

                // Kiểm tra loại KPI
                var inputType = GetKpiInputType(target.Indicator);
                if (inputType != KpiInputType.QUALITATIVE)
                {
                    result.Success = false;
                    result.ErrorMessage = "KPI này không được phép chấm điểm thủ công";
                    return result;
                }

                // Validate giá trị nhập vào
                if (actualValue < 0 || actualValue > 100)
                {
                    result.Success = false;
                    result.ErrorMessage = "Giá trị phải từ 0-100%";
                    return result;
                }

                // Cập nhật điểm
                target.ActualValue = actualValue;
                target.Score = CalculateScore(actualValue, target);
                target.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                result.Success = true;
                result.UpdatedTarget = target;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật điểm thủ công cho target {TargetId}", targetId);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        // 📊 Import hàng loạt cho KPI định lượng tuyệt đối
        public async Task<KpiScoringResult> BulkImportQuantitativeScoresAsync(List<QuantitativeScoreDto> scores)
        {
            var result = new KpiScoringResult();
            var successCount = 0;
            var errorMessages = new List<string>();

            try
            {
                foreach (var scoreDto in scores)
                {
                    var target = await _context.EmployeeKpiTargets
                        .Include(t => t.Indicator)
                        .FirstOrDefaultAsync(t => t.Id == scoreDto.TargetId);

                    if (target == null)
                    {
                        errorMessages.Add($"Không tìm thấy target ID {scoreDto.TargetId}");
                        continue;
                    }

                    // Kiểm tra loại KPI
                    var inputType = GetKpiInputType(target.Indicator);
                    if (inputType != KpiInputType.QUANTITATIVE_ABSOLUTE)
                    {
                        errorMessages.Add($"Target ID {scoreDto.TargetId} không phải định lượng tuyệt đối");
                        continue;
                    }

                    // Cập nhật giá trị và tính điểm
                    target.ActualValue = scoreDto.ActualValue;
                    target.Score = CalculateScore(scoreDto.ActualValue, target);
                    target.UpdatedDate = DateTime.UtcNow;

                    successCount++;
                }

                await _context.SaveChangesAsync();

                result.Success = true;
                result.SuccessCount = successCount;
                result.ErrorMessages = errorMessages;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import hàng loạt điểm KPI");
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        // 🧮 Tính toán KPI tỷ lệ từ tử số và mẫu số
        public async Task<KpiScoringResult> CalculateRatioKpiAsync(int targetId, decimal numerator, decimal denominator)
        {
            var result = new KpiScoringResult();

            try
            {
                var target = await _context.EmployeeKpiTargets
                    .Include(t => t.Indicator)
                    .FirstOrDefaultAsync(t => t.Id == targetId);

                if (target == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Không tìm thấy KPI target";
                    return result;
                }

                // Kiểm tra loại KPI
                var inputType = GetKpiInputType(target.Indicator);
                if (inputType != KpiInputType.QUANTITATIVE_RATIO)
                {
                    result.Success = false;
                    result.ErrorMessage = "KPI này không phải loại tỷ lệ tính toán";
                    return result;
                }

                // Validate mẫu số
                if (denominator == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "Mẫu số không được bằng 0";
                    return result;
                }

                // Tính tỷ lệ phần trăm
                var ratioPercentage = (numerator / denominator) * 100;
                
                // Cập nhật kết quả
                target.ActualValue = Math.Round(ratioPercentage, 2); // Làm tròn 2 chữ số thập phân
                target.Score = CalculateScore(target.ActualValue.Value, target);
                target.UpdatedDate = DateTime.UtcNow;
                target.Notes = $"Tính từ: {numerator:N2} / {denominator:N2} = {ratioPercentage:N2}%";

                await _context.SaveChangesAsync();

                result.Success = true;
                result.UpdatedTarget = target;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán KPI tỷ lệ cho target {TargetId}", targetId);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        // 🎯 Logic tính điểm chung cho tất cả các loại KPI
        private decimal CalculateScore(decimal actualValue, EmployeeKpiTarget target)
        {
            if (!target.TargetValue.HasValue || target.TargetValue.Value == 0)
                return 0;

            decimal ratio;
            var inputType = GetKpiInputType(target.Indicator);

            if (inputType == KpiInputType.QUALITATIVE || inputType == KpiInputType.QUANTITATIVE_RATIO)
            {
                // Với KPI định tính và tỷ lệ %: ActualValue đã là % (0-100)
                ratio = actualValue / 100;
            }
            else
            {
                // Với KPI định lượng tuyệt đối: Tính tỷ lệ hoàn thành
                ratio = actualValue / target.TargetValue.Value;
            }

            // Áp dụng cap ở MaxScore
            var score = Math.Min(ratio * target.Indicator.MaxScore, target.Indicator.MaxScore);
            
            return Math.Round(score, 2); // Làm tròn 2 chữ số thập phân
        }
    }

    // 📋 DTOs và Enums
    public enum KpiInputType
    {
        QUALITATIVE,           // Định tính - chấm tay
        QUANTITATIVE_RATIO,    // Định lượng tỷ lệ - tính toán
        QUANTITATIVE_ABSOLUTE  // Định lượng tuyệt đối - import
    }

    public class KpiScoringResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public int SuccessCount { get; set; }
        public EmployeeKpiTarget? UpdatedTarget { get; set; }
    }

    public class QuantitativeScoreDto
    {
        public int TargetId { get; set; }
        public decimal ActualValue { get; set; }
        public string? Notes { get; set; }
    }
}
