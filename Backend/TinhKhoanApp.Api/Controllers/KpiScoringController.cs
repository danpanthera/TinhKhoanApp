using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KpiScoringController : ControllerBase
    {
        private readonly IKpiScoringService _scoringService;
        private readonly ILogger<KpiScoringController> _logger;

        public KpiScoringController(IKpiScoringService scoringService, ILogger<KpiScoringController> logger)
        {
            _scoringService = scoringService;
            _logger = logger;
        }

        // 📝 API chấm điểm thủ công cho KPI định tính
        [HttpPost("manual-score")]
        public async Task<IActionResult> UpdateManualScore([FromBody] ManualScoreRequest request)
        {
            try
            {
                var result = await _scoringService.UpdateManualScoreAsync(request.TargetId, request.ActualValue);
                
                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật điểm thành công",
                        target = new
                        {
                            id = result.UpdatedTarget?.Id,
                            actualValue = result.UpdatedTarget?.ActualValue,
                            score = result.UpdatedTarget?.Score,
                            updatedDate = result.UpdatedTarget?.UpdatedDate
                        }
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi API chấm điểm thủ công");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }

        // 📊 API import hàng loạt điểm KPI định lượng
        [HttpPost("bulk-import")]
        public async Task<IActionResult> BulkImportScores([FromBody] BulkImportRequest request)
        {
            try
            {
                var result = await _scoringService.BulkImportQuantitativeScoresAsync(request.Scores);
                
                return Ok(new
                {
                    success = result.Success,
                    successCount = result.SuccessCount,
                    totalCount = request.Scores.Count,
                    errorCount = request.Scores.Count - result.SuccessCount,
                    errors = result.ErrorMessages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi API import hàng loạt");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }

        // 🧮 API tính toán KPI tỷ lệ từ tử số/mẫu số
        [HttpPost("calculate-ratio")]
        public async Task<IActionResult> CalculateRatioKpi([FromBody] RatioCalculationRequest request)
        {
            try
            {
                var result = await _scoringService.CalculateRatioKpiAsync(
                    request.TargetId, 
                    request.Numerator, 
                    request.Denominator
                );
                
                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Tính toán tỷ lệ thành công",
                        target = new
                        {
                            id = result.UpdatedTarget?.Id,
                            actualValue = result.UpdatedTarget?.ActualValue,
                            score = result.UpdatedTarget?.Score,
                            notes = result.UpdatedTarget?.Notes,
                            updatedDate = result.UpdatedTarget?.UpdatedDate
                        }
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi API tính toán tỷ lệ");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }

        // 🔍 API kiểm tra loại KPI
        [HttpGet("check-kpi-type/{indicatorId}")]
        public async Task<IActionResult> CheckKpiType(int indicatorId)
        {
            try
            {
                var context = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var indicator = await context.KpiIndicators.FindAsync(indicatorId);
                
                if (indicator == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy KPI" });
                }

                var inputType = _scoringService.GetKpiInputType(indicator);
                
                return Ok(new
                {
                    success = true,
                    indicatorId = indicator.Id,
                    indicatorName = indicator.IndicatorName,
                    inputType = inputType.ToString(),
                    valueType = indicator.ValueType.ToString(),
                    unit = indicator.Unit,
                    maxScore = indicator.MaxScore,
                    description = GetInputTypeDescription(inputType)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi API kiểm tra loại KPI");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }

        // 🏷️ Helper method mô tả loại KPI
        private string GetInputTypeDescription(KpiInputType inputType)
        {
            return inputType switch
            {
                KpiInputType.QUALITATIVE => "Định tính - Chấm điểm thủ công (0-100%)",
                KpiInputType.QUANTITATIVE_RATIO => "Định lượng tỷ lệ - Tính toán từ tử số/mẫu số",
                KpiInputType.QUANTITATIVE_ABSOLUTE => "Định lượng tuyệt đối - Import từ dữ liệu thực tế",
                _ => "Không xác định"
            };
        }
    }

    // 📋 Request DTOs
    public class ManualScoreRequest
    {
        public int TargetId { get; set; }
        public decimal ActualValue { get; set; } // 0-100%
        public string? Notes { get; set; }
    }

    public class BulkImportRequest
    {
        public List<QuantitativeScoreDto> Scores { get; set; } = new();
    }

    public class RatioCalculationRequest
    {
        public int TargetId { get; set; }
        public decimal Numerator { get; set; }   // Tử số
        public decimal Denominator { get; set; } // Mẫu số
        public string? Notes { get; set; }
    }
}
