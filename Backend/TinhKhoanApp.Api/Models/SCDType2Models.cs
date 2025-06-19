using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Base class for SCD Type 2 entities with versioning support
    /// </summary>
    public abstract class SCDType2BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Business key - identifies the logical record across versions
        /// </summary>
        public string SourceId { get; set; } = string.Empty;
        
        /// <summary>
        /// When this version became valid
        /// </summary>
        public DateTime ValidFrom { get; set; }
        
        /// <summary>
        /// When this version became invalid (null for current version)
        /// </summary>
        public DateTime? ValidTo { get; set; }
        
        /// <summary>
        /// Is this the current active version?
        /// </summary>
        public bool IsCurrent { get; set; }
        
        /// <summary>
        /// Version number for this record
        /// </summary>
        public int VersionNumber { get; set; }
        
        /// <summary>
        /// Hash of the record content for change detection
        /// </summary>
        public string RecordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// When this record was created in the system
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Source of the data (e.g., file name)
        /// </summary>
        public string DataSource { get; set; } = string.Empty;
        
        /// <summary>
        /// Is this record marked as deleted (not present in latest import)?
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// User who created this record
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;
        
        /// <summary>
        /// When this record was last modified
        /// </summary>
        public DateTime? ModifiedAt { get; set; }
        
        /// <summary>
        /// User who last modified this record
        /// </summary>
        public string? ModifiedBy { get; set; }
    }

    /// <summary>
    /// SCD Type 2 version of RawDataRecord - stores JSON data with versioning
    /// </summary>
    [Table("RawDataRecords_SCD")]
    public class RawDataRecord_SCD : SCDType2BaseEntity
    {
        /// <summary>
        /// Reference to the original RawDataImport
        /// </summary>
        public int RawDataImportId { get; set; }
        
        /// <summary>
        /// The actual data stored as JSON
        /// </summary>
        [Required]
        public string JsonData { get; set; } = string.Empty;
        
        /// <summary>
        /// Data type (LN01, GL01, etc.)
        /// </summary>
        public string DataType { get; set; } = string.Empty;
        
        /// <summary>
        /// Branch code extracted from filename or data
        /// </summary>
        public string BranchCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Statement date parsed from filename
        /// </summary>
        public DateTime StatementDate { get; set; }
        
        /// <summary>
        /// Original filename
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;
        
        /// <summary>
        /// When data was imported into the system
        /// </summary>
        public DateTime ImportDate { get; set; }
        
        /// <summary>
        /// Processing status
        /// </summary>
        public string ProcessingStatus { get; set; } = "Pending";
        
        /// <summary>
        /// Error message if any
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// Processing notes
        /// </summary>
        public string? ProcessingNotes { get; set; }
        
        /// <summary>
        /// Relationship to original import
        /// </summary>
        [ForeignKey("RawDataImportId")]
        public virtual RawDataImport? RawDataImport { get; set; }
    }

    /// <summary>
    /// DTO for querying historical data
    /// </summary>
    public class HistoricalQueryRequest
    {
        public string? SourceId { get; set; }
        public DateTime? AsOfDate { get; set; }
        public int? VersionNumber { get; set; }
        public bool IncludeHistory { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    /// <summary>
    /// Response for historical queries
    /// </summary>
    public class HistoricalQueryResponse<T> where T : SCDType2BaseEntity
    {
        public IEnumerable<T> Records { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// SCD Type 2 operation result
    /// </summary>
    public class SCDType2Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecordsProcessed { get; set; }
        public int RecordsInserted { get; set; }
        public int RecordsUpdated { get; set; }
        public int RecordsExpired { get; set; }
        public int RecordsDeleted { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public long ProcessingTimeMs { get; set; }
    }
}
