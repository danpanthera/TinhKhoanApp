using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models
{
    // =====================================================
    // OPTIMIZED QUERY REQUEST MODELS
    // =====================================================

    /// <summary>
    /// Request model cho optimized pagination queries
    /// </summary>
    public class OptimizedQueryRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page phải lớn hơn 0")]
        public int Page { get; set; } = 1;

        [Range(1, 1000, ErrorMessage = "PageSize phải từ 1 đến 1000")]
        public int PageSize { get; set; } = 50;

        public string? DataType { get; set; }
        public string? Status { get; set; }
        public string? ImportedBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Search { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Page, PageSize, DataType, Status, ImportedBy, FromDate, ToDate, Search);
        }
    }

    /// <summary>
    /// Request model cho virtual scrolling
    /// </summary>
    public class VirtualScrollRequest
    {
        [Range(0, int.MaxValue)]
        public int StartIndex { get; set; } = 0;

        [Range(10, 500, ErrorMessage = "ViewportSize phải từ 10 đến 500")]
        public int ViewportSize { get; set; } = 100;

        public long? ImportId { get; set; }
        public string? DataType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Search { get; set; }
    }

    /// <summary>
    /// Request model cho SCD optimized queries
    /// </summary>
    public class SCDOptimizedQueryRequest
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 1000)]
        public int PageSize { get; set; } = 50;

        public string? DataType { get; set; }
        public string? BranchCode { get; set; }
        public DateTime? AsOfDate { get; set; }
        public bool IncludeHistory { get; set; } = false;
        public DateTime? StatementDateFrom { get; set; }
        public DateTime? StatementDateTo { get; set; }
        public string? SourceId { get; set; }
        public int? VersionNumber { get; set; }
    }

    // =====================================================
    // RESPONSE MODELS
    // =====================================================

    /// <summary>
    /// Generic paginated response model
    /// </summary>
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Virtual scroll response model
    /// </summary>
    public class VirtualScrollResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int ViewportSize { get; set; }
        public bool HasMore { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    // =====================================================
    // SUMMARY/DTO MODELS
    // =====================================================

    /// <summary>
    /// Lightweight summary model cho RawDataImport
    /// </summary>
    public class RawDataImportSummary
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public DateTime StatementDate { get; set; }
        public string ImportedBy { get; set; } = string.Empty;
        public string? Status { get; set; }
        public int RecordsCount { get; set; }
        public bool IsArchiveFile { get; set; }
        public int ExtractedFilesCount { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Lightweight summary model cho RawDataRecord
    /// </summary>
    public class RawDataRecordSummary
    {
        public long Id { get; set; }
        public long ImportId { get; set; }
        public string ImportFileName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public DateTime ProcessedDate { get; set; }
        public string JsonDataPreview { get; set; } = string.Empty;
        public int JsonDataSize { get; set; }
    }

    /// <summary>
    /// Lightweight summary model cho SCD records
    /// </summary>
    public class SCDRecordSummary
    {
        public long HistoryID { get; set; }
        public string SourceId { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public DateTime StatementDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsCurrent { get; set; }
        public int VersionNumber { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool HasJsonData { get; set; }
        public int JsonDataSize { get; set; }
    }

    /// <summary>
    /// Dashboard statistics model
    /// </summary>
    public class DashboardStats
    {
        public int TotalImports { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentSCDRecords { get; set; }
        public int TotalRecordsProcessed { get; set; }
        public Dictionary<string, int> ImportsByDataType { get; set; } = new();
        public int ImportsLast30Days { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public DateTime LastUpdated { get; set; }
        
        // Performance metrics
        public double SuccessRate => TotalImports > 0 ? 
            (double)SuccessfulImports / TotalImports * 100 : 0;
        
        public double AvgRecordsPerImport => TotalImports > 0 ? 
            (double)TotalRecordsProcessed / TotalImports : 0;
    }

    // =====================================================
    // PERFORMANCE MONITORING MODELS
    // =====================================================

    /// <summary>
    /// Model để theo dõi performance của queries
    /// </summary>
    public class QueryPerformanceMetrics
    {
        public string QueryName { get; set; } = string.Empty;
        public DateTime ExecutedAt { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public int RecordsReturned { get; set; }
        public string? Parameters { get; set; }
        public bool IsCached { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Model cho cache statistics
    /// </summary>
    public class CacheStats
    {
        public int HitCount { get; set; }
        public int MissCount { get; set; }
        public double HitRate => (HitCount + MissCount) > 0 ? 
            (double)HitCount / (HitCount + MissCount) * 100 : 0;
        public DateTime LastReset { get; set; }
        public Dictionary<string, int> KeyHitCounts { get; set; } = new();
    }
}
