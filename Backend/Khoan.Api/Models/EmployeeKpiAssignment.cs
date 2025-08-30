using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Model lưu trữ việc giao khoán KPI cho nhân viên cụ thể
    /// Đây là bảng chính cho hệ thống chấm điểm KPI
    /// </summary>
    [Table("EmployeeKpiAssignments")]
    public class EmployeeKpiAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int KpiDefinitionId { get; set; }

        [Required]
        public int KhoanPeriodId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetValue { get; set; } // Giá trị mục tiêu được giao

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualValue { get; set; } // Giá trị thực hiện

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Score { get; set; } // Điểm đạt được (có thể tính tự động hoặc nhập thủ công)

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; } // Ghi chú

        // Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("KpiDefinitionId")]
        public virtual KPIDefinition KpiDefinition { get; set; } = null!;

        [ForeignKey("KhoanPeriodId")]
        public virtual KhoanPeriod KhoanPeriod { get; set; } = null!;
    }
}
