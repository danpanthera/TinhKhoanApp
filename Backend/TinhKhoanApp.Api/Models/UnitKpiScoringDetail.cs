using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Chi tiết chấm điểm từng chỉ tiêu KPI của Chi nhánh
    /// </summary>
    public class UnitKpiScoringDetail
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID bảng chấm điểm chính
        /// </summary>
        [Required]
        public int UnitKpiScoringId { get; set; }

        /// <summary>
        /// ID chỉ tiêu KPI (tham chiếu đến KPIDefinition)
        /// </summary>
        [Required]
        public int KpiDefinitionId { get; set; }

        /// <summary>
        /// ID KpiIndicator (tham chiếu đến KpiIndicator nếu có)
        /// </summary>
        public int? KpiIndicatorId { get; set; }

        /// <summary>
        /// Tên chỉ tiêu (lưu tạm để dễ tra cứu)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string IndicatorName { get; set; }

        /// <summary>
        /// Giá trị kế hoạch
        /// </summary>
        [Column(TypeName = "decimal(15,2)")]
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Giá trị thực hiện
        /// </summary>
        [Column(TypeName = "decimal(15,2)")]
        public decimal? ActualValue { get; set; }

        /// <summary>
        /// Tỷ lệ hoàn thành (%)
        /// </summary>
        [Column(TypeName = "decimal(10,4)")]
        public decimal? CompletionRate { get; set; }

        /// <summary>
        /// Điểm gốc của chỉ tiêu
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseScore { get; set; }

        /// <summary>
        /// Điểm cộng trừ của chỉ tiêu
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal AdjustmentScore { get; set; }

        /// <summary>
        /// Điểm cuối cùng sau cộng trừ
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalScore { get; set; }

        /// <summary>
        /// Điểm đạt được (alias cho FinalScore)
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal Score { get; set; }

        /// <summary>
        /// Loại chỉ tiêu: Quantitative, Qualitative
        /// </summary>
        [Required]
        [StringLength(20)]
        public string IndicatorType { get; set; }

        /// <summary>
        /// Công thức tính điểm áp dụng
        /// </summary>
        [StringLength(50)]
        public string? ScoringFormula { get; set; }

        /// <summary>
        /// Ghi chú cho chỉ tiêu này
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UnitKpiScoringId")]
        public virtual UnitKpiScoring UnitKpiScoring { get; set; } = null!;
        
        [ForeignKey("KpiDefinitionId")]
        public virtual KPIDefinition KpiDefinition { get; set; } = null!;
        
        [ForeignKey("KpiIndicatorId")]
        public virtual KpiIndicator? KpiIndicator { get; set; }
    }
}
