using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    [Table("UnitKhoanAssignments")]
    public class UnitKhoanAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit? Unit { get; set; }

        [Required]
        public int KhoanPeriodId { get; set; }
        [ForeignKey("KhoanPeriodId")]
        public virtual KhoanPeriod? KhoanPeriod { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }

        // Navigation: KPI chi tiết giao khoán cho đơn vị này
        public virtual ICollection<UnitKhoanAssignmentDetail> AssignmentDetails { get; set; }

        public UnitKhoanAssignment()
        {
            AssignmentDetails = new HashSet<UnitKhoanAssignmentDetail>();
        }
    }

    [Table("UnitKhoanAssignmentDetails")]
    public class UnitKhoanAssignmentDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UnitKhoanAssignmentId { get; set; }
        [ForeignKey("UnitKhoanAssignmentId")]
        public virtual UnitKhoanAssignment? UnitKhoanAssignment { get; set; }

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
