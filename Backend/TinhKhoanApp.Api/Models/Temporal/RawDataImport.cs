using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Temporal
{
    /// <summary>
    /// Represents a raw data import record with temporal table support.
    /// This model supports system versioning for tracking changes over time.
    /// </summary>
    [Table("OptimizedRawDataImports")]
    public class OptimizedRawDataImport
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        public DateTime ImportDate { get; set; }
        
        [Required]
        [StringLength(10)]
        public string BranchCode { get; set; }
        
        [Required]
        [StringLength(10)]
        public string DepartmentCode { get; set; }
        
        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; }
        
        [Required]
        [StringLength(20)]
        public string KpiCode { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal KpiValue { get; set; }
        
        [StringLength(10)]
        public string Unit { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Target { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Achievement { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Score { get; set; }
        
        [StringLength(500)]
        public string Note { get; set; }
        
        [Required]
        public Guid ImportBatchId { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = "SYSTEM";
        
        [Required]
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        [StringLength(100)]
        public string LastModifiedBy { get; set; } = "SYSTEM";
        
        [Required]
        public bool IsDeleted { get; set; } = false;
        
        // System versioning fields (handled by SQL Server)
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        
        // Additional compatibility fields
        [Required]
        [StringLength(200)]
        public string KpiName { get; set; } = "";
        
        [Required]
        [StringLength(50)]
        public string DataType { get; set; } = "";
        
        [Required]
        [StringLength(500)]
        public string FileName { get; set; } = "";
    }
}
