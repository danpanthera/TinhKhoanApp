using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    [Table("EmployeeKhoanAssignments")]
    public class EmployeeKhoanAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [Required]
        public int KhoanPeriodId { get; set; }
        [ForeignKey("KhoanPeriodId")]
        public virtual KhoanPeriod? KhoanPeriod { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }

        // Navigation: KPI chi tiết giao khoán cho nhân viên này
        public virtual ICollection<EmployeeKhoanAssignmentDetail> AssignmentDetails { get; set; }

        public EmployeeKhoanAssignment()
        {
            AssignmentDetails = new HashSet<EmployeeKhoanAssignmentDetail>();
        }
    }

    [Table("EmployeeKhoanAssignmentDetails")]
    public class EmployeeKhoanAssignmentDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeKhoanAssignmentId { get; set; }
        [ForeignKey("EmployeeKhoanAssignmentId")]
        public virtual EmployeeKhoanAssignment? EmployeeKhoanAssignment { get; set; }

        // TODO: Will be replaced with new KPI assignment table references
        public string? LegacyKPICode { get; set; }
        public string? LegacyKPIName { get; set; }

        [Required]
        public decimal TargetValue { get; set; } // Giá trị khoán giao

        public decimal? ActualValue { get; set; } // Giá trị thực hiện
        public decimal? Score { get; set; } // Điểm đạt được
        public string? Note { get; set; }
    }
}
