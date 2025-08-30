using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    [Table("EmployeeAuditLogs")]
    public class EmployeeAuditLog
    {
        [Key]
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        [MaxLength(100)] public string Action { get; set; } = string.Empty; // CREATE / UPDATE / DELETE
        [MaxLength(100)] public string PerformedBy { get; set; } = string.Empty; // username thực hiện
        [MaxLength(255)] public string? FieldChanged { get; set; } // null nếu nhiều field
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}
