using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Temporal
{
    /// <summary>
    /// Main temporal table for raw data imports with system versioning
    /// Optimized for high-volume imports (100K-200K records/day)
    /// </summary>
    [Table("RawDataImports")]
    public class RawDataImport
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime ImportDate { get; set; }

        [Required]
        [StringLength(10)]
        public string BranchCode { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string KpiCode { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal KpiValue { get; set; }

        [StringLength(10)]
        public string? Unit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Target { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Achievement { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Score { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        // Metadata fields
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

        // Temporal columns (handled by SQL Server)
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        
        // Additional properties that services expect (for compatibility)
        public string KpiName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        
        // Properties that services can assign to (not read-only)
        public decimal Value 
        { 
            get => KpiValue; 
            set => KpiValue = value; 
        }
        
        public string ImportedBy 
        { 
            get => CreatedBy; 
            set => CreatedBy = value; 
        }
        
        public DateTime ImportedAt 
        { 
            get => CreatedDate; 
            set => CreatedDate = value; 
        }
        
        // Additional properties that controllers expect
        public string Status { get; set; } = "PENDING";
        public DateTime? StatementDate { get; set; }
        public int RecordsCount { get; set; } = 0;
        public bool IsArchiveFile { get; set; } = false;
        public int ExtractedFilesCount { get; set; } = 0;
        
        // Navigation property for records (not mapped to avoid EF issues)
        [NotMapped]
        public virtual List<object> RawDataRecords { get; set; } = new();
    }

    /// <summary>
    /// Archive table for old temporal data
    /// Used for long-term storage and compliance
    /// </summary>
    [Table("RawDataImportArchives")]
    public class RawDataImportArchive
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime ImportDate { get; set; }

        [Required]
        [StringLength(10)]
        public string BranchCode { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string KpiCode { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal KpiValue { get; set; }

        [StringLength(10)]
        public string? Unit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Target { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Achievement { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Score { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        // Metadata fields
        [Required]
        public Guid ImportBatchId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public DateTime LastModifiedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string LastModifiedBy { get; set; } = string.Empty;

        [Required]
        public bool IsDeleted { get; set; }

        // Archive metadata
        [Required]
        public DateTime ArchivedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string ArchivedBy { get; set; } = "SYSTEM";

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }
    }

    /// <summary>
    /// Import log for tracking batch operations
    /// </summary>
    [Table("ImportLogs")]
    public class ImportLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public Guid ImportBatchId { get; set; }

        [Required]
        public DateTime ImportDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ImportSource { get; set; } = string.Empty;

        [Required]
        public int RecordsCount { get; set; }

        [Required]
        public int SuccessCount { get; set; }

        [Required]
        public int ErrorCount { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public long ProcessingTimeMs { get; set; }

        [StringLength(1000)]
        public string? ErrorDetails { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // SUCCESS, FAILED, PARTIAL

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = "SYSTEM";

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Additional properties for compatibility with services
        [StringLength(50)]
        public string BatchId 
        { 
            get => ImportBatchId.ToString(); 
            set => ImportBatchId = Guid.TryParse(value, out var guid) ? guid : Guid.NewGuid(); 
        }
        
        [StringLength(50)]
        public string TableName { get; set; } = string.Empty;
        
        public int TotalRecords 
        { 
            get => RecordsCount; 
            set => RecordsCount = value; 
        }
        
        public int ProcessedRecords { get; set; } = 0;
        public int NewRecords { get; set; } = 0;
        public int UpdatedRecords { get; set; } = 0;
        public int DeletedRecords { get; set; } = 0;
        
        public string? ErrorMessage 
        { 
            get => ErrorDetails; 
            set => ErrorDetails = value; 
        }
        
        public int? Duration { get; set; } // seconds
    }

    // DTOs for API operations
    public class TemporalImportRequest
    {
        public DateTime ImportDate { get; set; }
        public string ImportSource { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ImportedBy { get; set; } = "SYSTEM";
        public List<RawDataImportDto> Data { get; set; } = new();
    }

    public class RawDataImportDto
    {
        public string BranchCode { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string KpiCode { get; set; } = string.Empty;
        public string KpiName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Unit { get; set; }
        public decimal? Target { get; set; }
        public decimal? Achievement { get; set; }
        public decimal? Score { get; set; }
        public string? Note { get; set; }
    }

    public class TemporalDataComparison
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string KpiCode { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public decimal? ValueDate1 { get; set; }
        public decimal? ValueDate2 { get; set; }
        public decimal? Difference { get; set; }
        public decimal? PercentageChange { get; set; }
        public string ChangeType { get; set; } = string.Empty; // ADDED, MODIFIED, REMOVED, UNCHANGED
        
        // Additional properties that TemporalDataService expects
        public DateTime ImportDate { get; set; }
        public string KpiName { get; set; } = string.Empty;
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? ValueChange { get; set; }
        public string? Unit { get; set; }
    }

    public class DailyChangeSummary
    {
        public DateTime ReportDate { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int NewRecords { get; set; }
        public int ModifiedRecords { get; set; }
        public int DeletedRecords { get; set; }
        public int UnchangedRecords { get; set; }
        public Dictionary<string, int> KpiChangeCounts { get; set; } = new();
        
        // Additional property that TemporalDataService expects
        public int TotalComparisons { get; set; }
    }

    public class PerformanceAnalytics
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string KpiCode { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public decimal AverageValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal? Target { get; set; }
        public decimal? AchievementRate { get; set; }
        public string Trend { get; set; } = string.Empty; // IMPROVING, DECLINING, STABLE
        public int DataPoints { get; set; }
        public DateTime FirstRecordDate { get; set; }
        public DateTime LastRecordDate { get; set; }
        
        // Additional properties that TemporalDataService expects
        public DateTime ImportDate { get; set; }
        public string KpiName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Unit { get; set; }
        public decimal MovingAverage7Days { get; set; }
        public decimal DayOverDayChange { get; set; }
        public decimal PercentileRank { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class ArchiveConfiguration
    {
        public int ArchiveAfterMonths { get; set; } = 12;
        public int DeleteAfterMonths { get; set; } = 24;
        public bool EnableCompression { get; set; } = true;
        public string CompressionLevel { get; set; } = "PAGE";
        public int BatchSize { get; set; } = 10000;
        public bool EnableParallelProcessing { get; set; } = true;
    }

    public class ArchiveStatistics
    {
        public DateTime OperationDate { get; set; }
        public int RecordsArchived { get; set; }
        public int RecordsDeleted { get; set; }
        public long ProcessingTimeMs { get; set; }
        public long StorageFreedMB { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class DailyImportSummary
    {
        public DateTime ImportDate { get; set; }
        public int TotalImports { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public long TotalRecords { get; set; }
        public long AverageProcessingTimeMs { get; set; }
        public Dictionary<string, int> BranchImportCounts { get; set; } = new();
        public Dictionary<string, int> KpiImportCounts { get; set; } = new();
    }

    public class TemporalHealthCheck
    {
        public DateTime CheckDate { get; set; }
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, object> Metrics { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
