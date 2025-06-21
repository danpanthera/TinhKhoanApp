using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Dashboard
{
    /// <summary>
    /// Kế hoạch giao chỉ tiêu cho từng đơn vị
    /// Hỗ trợ giao kế hoạch theo tháng/quý/năm cho toàn tỉnh và từng CNL2
    /// </summary>
    public class BusinessPlanTarget
    {
        public int Id { get; set; }
        
        [Required]
        public int DashboardIndicatorId { get; set; }
        public virtual DashboardIndicator? DashboardIndicator { get; set; }
        
        [Required]
        public int UnitId { get; set; }
        public virtual Unit? Unit { get; set; }
        
        [Required]
        public int Year { get; set; } // Năm kế hoạch (2025, 2026...)
        
        public int? Quarter { get; set; } // 1-4 cho quý (nullable)
        
        public int? Month { get; set; } // 1-12 cho tháng (nullable)
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetValue { get; set; } // Giá trị kế hoạch được giao
        
        [MaxLength(500)]
        public string? Notes { get; set; } // Ghi chú về kế hoạch
        
        [MaxLength(50)]
        public string? Status { get; set; } = "Draft"; // Draft, Active, Approved
        
        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        
        // Soft delete
        public bool IsDeleted { get; set; } = false;
        
        [MaxLength(100)]
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
