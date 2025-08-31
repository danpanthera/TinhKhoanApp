using System.ComponentModel.DataAnnotations;

namespace Khoan.Api.Dtos.LN03
{
    /// <summary>
    /// LN03 Preview DTO - Shows key fields for list/grid view
    /// Used in: GetAllPagedAsync, search results, data grids
    /// </summary>
    public class LN03PreviewDto
    {
        public int Id { get; set; }
        public DateTime? NGAY_DL { get; set; }
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
    }

    /// <summary>
    /// LN03 Details DTO - Complete record with all fields including temporal data
    /// Used in: GetByIdAsync, detailed views, full record operations
    /// </summary>
    public class LN03DetailsDto
    {
        public int Id { get; set; }
        public DateTime? NGAY_DL { get; set; }

        // 17 Named Business Columns
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? THUNOSAUXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }

        // 3 Unnamed Business Columns
        public string? COLUMN_18 { get; set; }
        public string? COLUMN_19 { get; set; }
        public decimal? COLUMN_20 { get; set; }

        // System Columns
        public DateTime CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public bool IS_DELETED { get; set; }

        // Temporal Columns (for history tracking)
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// LN03 Create DTO - For creating new records
    /// Used in: CreateAsync, CSV import operations
    /// </summary>
    public class CreateLN03Dto
    {
        public DateTime? NGAY_DL { get; set; }

        // 17 Named Business Columns
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? THUNOSAUXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }

        // 3 Unnamed Business Columns
        public string? COLUMN_18 { get; set; }
        public string? COLUMN_19 { get; set; }
        public decimal? COLUMN_20 { get; set; }
    }

    /// <summary>
    /// LN03 Update DTO - For updating existing records
    /// Used in: UpdateAsync operations
    /// </summary>
    public class UpdateLN03Dto
    {
        // 17 Named Business Columns
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? THUNOSAUXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }

        // 3 Unnamed Business Columns
        public string? COLUMN_18 { get; set; }
        public string? COLUMN_19 { get; set; }
        public decimal? COLUMN_20 { get; set; }
    }

    /// <summary>
    /// LN03 Import Result DTO - For CSV import feedback
    /// Used in: ImportFromCsvStreamAsync results
    /// </summary>
    public class LN03ImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessfulRows { get; set; }
        public int FailedRows { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime ProcessingStartTime { get; set; }
        public DateTime ProcessingEndTime { get; set; }
        public string? FileName { get; set; }
        public DateTime? ExtractedNGAY_DL { get; set; }
        public int BatchCount { get; set; }
        public List<string> ValidationWarnings { get; set; } = new();
    }

    /// <summary>
    /// LN03 Summary DTO - For analytics and reporting
    /// Used in: GetSummaryAsync, dashboard views, analytics
    /// </summary>
    public class LN03SummaryDto
    {
        public int TotalRecords { get; set; }
        public DateTime? LatestNGAY_DL { get; set; }
        public DateTime? OldestNGAY_DL { get; set; }
        public int UniqueBranches { get; set; }
        public int UniqueCustomers { get; set; }
        public decimal TotalSOTIENXLRR { get; set; }
        public decimal TotalCONLAINGOAIBANG { get; set; }
        public decimal TotalDUNONOIBANG { get; set; }
        public Dictionary<string, int> DebtGroupDistribution { get; set; } = new();
        public Dictionary<string, decimal> BranchTotalDistribution { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// LN03 Configuration DTO - For service configuration
    /// Used in: Service initialization, configuration management
    /// </summary>
    public class LN03ConfigDto
    {
        public int BatchSize { get; set; } = 10000;
        public bool ValidateOnImport { get; set; } = true;
        public bool EnableHistoryTracking { get; set; } = true;
        public string DateFormat { get; set; } = "yyyyMMdd";
        public string DecimalSeparator { get; set; } = ".";
        public int MaxFileSize { get; set; } = 2147483647; // 2GB
        public List<string> RequiredColumns { get; set; } = new()
        {
            "MACHINHANH", "TENCHINHANH", "MAKH", "TENKH", "SOHOPDONG"
        };
        public Dictionary<string, string> ColumnMappings { get; set; } = new();
    }
}
