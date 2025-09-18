using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    /// <summary>
    /// Mức độ vi phạm
    /// </summary>
    public enum ViolationLevel
    {
        Minor = 1,      // Nhắc nhở (-2 điểm)
        Written = 2,    // Khiển trách bằng văn bản (-4 điểm)
        Disciplinary = 3 // Kỷ luật (0 điểm)
    }

    /// <summary>
    /// Bảng tiêu chí cộng trừ điểm cho Chi nhánh (dành cho chỉ tiêu chấp hành quy chế)
    /// </summary>
    public class UnitKpiScoringCriteria
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID bảng chấm điểm chính
        /// </summary>
        [Required]
        public int UnitKpiScoringId { get; set; }

        /// <summary>
        /// Loại vi phạm: ProcessViolation (Vi phạm quy chế, quy trình), CultureViolation (Vi phạm văn hóa Agribank)
        /// </summary>
        [Required]
        [StringLength(30)]
        public string ViolationType { get; set; }

        /// <summary>
        /// Mức độ vi phạm: None (Không vi phạm), Minor (Nhắc nhở không văn bản), Written (Nhắc nhở văn bản), Disciplinary (Kỷ luật từ khiển trách trở lên)
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ViolationLevel { get; set; }

        /// <summary>
        /// Số lần vi phạm
        /// </summary>
        public int ViolationCount { get; set; } = 0;

        /// <summary>
        /// Điểm bị trừ cho loại vi phạm này
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal PenaltyScore { get; set; }

        /// <summary>
        /// Mô tả vi phạm
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Ngày ghi nhận vi phạm
        /// </summary>
        public DateTime? ViolationDate { get; set; }

        // Navigation property
        public virtual UnitKpiScoring UnitKpiScoring { get; set; }
    }
}
