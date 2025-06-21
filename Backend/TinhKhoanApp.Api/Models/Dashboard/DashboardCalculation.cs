using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Dashboard
{
    /// <summary>
    /// Kết quả tính toán chỉ tiêu từ dữ liệu thô DP01, LN01, GLCB41
    /// Lưu trữ chi tiết quá trình tính toán và kết quả
    /// </summary>
    public class DashboardCalculation
    {
        public int Id { get; set; }
        
        [Required]
        public int DashboardIndicatorId { get; set; }
        public virtual DashboardIndicator? DashboardIndicator { get; set; }
        
        [Required]
        public int UnitId { get; set; }
        public virtual Unit? Unit { get; set; }
        
        [Required]
        public int Year { get; set; } // Năm dữ liệu
        
        public int? Quarter { get; set; } // Quý dữ liệu (nullable)
        
        public int? Month { get; set; } // Tháng dữ liệu (nullable)
        
        [Required]
        public DateTime CalculationDate { get; set; } // Ngày thực hiện tính toán
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualValue { get; set; } // Giá trị thực tế tính toán được
        
        // Chi tiết tính toán theo công thức cho từng chỉ tiêu
        [Column(TypeName = "nvarchar(max)")]
        public string? CalculationDetails { get; set; } // JSON lưu chi tiết công thức
        
        // Metadata về nguồn dữ liệu
        [MaxLength(50)]
        public string? DataSource { get; set; } // DP01, LN01, GLCB41
        
        [Required]
        public DateTime DataDate { get; set; } // Ngày của dữ liệu nguồn
        
        // Thông tin audit
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        
        // Trạng thái tính toán
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
        
        [MaxLength(1000)]
        public string? ErrorMessage { get; set; } // Lỗi nếu có
        
        public TimeSpan? ExecutionTime { get; set; } // Thời gian thực hiện
        
        // Filters đã áp dụng (cho việc drill-down)
        [Column(TypeName = "nvarchar(max)")]
        public string? AppliedFilters { get; set; } // JSON lưu filters
        
        // Soft delete
        public bool IsDeleted { get; set; } = false;
    }
}
