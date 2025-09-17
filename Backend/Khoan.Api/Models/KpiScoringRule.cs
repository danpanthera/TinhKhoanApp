using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    /// <summary>
    /// Bảng định nghĩa quy tắc tính điểm cộng/trừ cho các chỉ tiêu KPI chi nhánh
    /// </summary>
    [Table("KpiScoringRules")]
    public class KpiScoringRule
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tên chỉ tiêu KPI (VD: "Nguồn vốn huy động bình quân")
        /// </summary>
        [Required]
        [StringLength(200)]
        public string KpiIndicatorName { get; set; }

        /// <summary>
        /// Loại quy tắc: COMPLETION_RATE (theo % hoàn thành)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RuleType { get; set; } = "COMPLETION_RATE";

        /// <summary>
        /// Giá trị tối thiểu
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Giá trị tối đa
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Công thức tính điểm
        /// </summary>
        public string? ScoreFormula { get; set; }

        /// <summary>
        /// Điểm cộng thêm
        /// </summary>
        [Required]
        public decimal BonusPoints { get; set; } = 0;

        /// <summary>
        /// Điểm trừ
        /// </summary>
        [Required]
        public decimal PenaltyPoints { get; set; } = 0;

        /// <summary>
        /// Có hiệu lực không
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
