using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Bảng chấm điểm KPI cho Chi nhánh
    /// </summary>
    public class UnitKpiScoring
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID giao khoán của đơn vị (tham chiếu đến UnitKhoanAssignment)
        /// </summary>
        [Required]
        public int UnitKhoanAssignmentId { get; set; }

        /// <summary>
        /// Kỳ chấm điểm
        /// </summary>
        [Required]
        public int KhoanPeriodId { get; set; }

        /// <summary>
        /// ID đơn vị (Chi nhánh)
        /// </summary>
        [Required]
        public int UnitId { get; set; }

        /// <summary>
        /// Ngày chấm điểm
        /// </summary>
        public DateTime ScoringDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Tổng điểm cuối cùng sau khi cộng trừ
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalScore { get; set; }

        /// <summary>
        /// Điểm gốc trước khi cộng trừ
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseScore { get; set; }

        /// <summary>
        /// Điểm cộng trừ
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal AdjustmentScore { get; set; }

        /// <summary>
        /// Ghi chú chấm điểm
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Người chấm điểm
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ScoredBy { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UnitKhoanAssignmentId")]
        public virtual UnitKhoanAssignment UnitKhoanAssignment { get; set; } = null!;

        [ForeignKey("KhoanPeriodId")]
        public virtual KhoanPeriod KhoanPeriod { get; set; } = null!;

        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; } = null!;

        // Collection navigation properties
        public virtual ICollection<UnitKpiScoringDetail> ScoringDetails { get; set; } = new List<UnitKpiScoringDetail>();
        public virtual ICollection<UnitKpiScoringCriteria> ScoringCriteria { get; set; } = new List<UnitKpiScoringCriteria>();
    }
}
