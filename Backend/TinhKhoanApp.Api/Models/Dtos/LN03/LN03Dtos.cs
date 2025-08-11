using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.LN03
{
    /// <summary>
    /// LN03 (Risk/Credit Recovery) DTOs - 20 business columns (17 CSV + 3 additional)
    /// CSV Structure: MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
    /// Additional: 3 extra business columns
    /// </summary>

    /// <summary>
    /// LN03 Preview DTO - For list/grid display (key fields only)
    /// </summary>
    public class LN03PreviewDto
    {
        public long Id { get; set; }
        public string MACHINHANH { get; set; } = string.Empty;
        public string TENCHINHANH { get; set; } = string.Empty;
        public string MAKH { get; set; } = string.Empty;
        public string TENKH { get; set; } = string.Empty;
        public string SOHOPDONG { get; set; } = string.Empty;
        public decimal SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal CONLAINGOAIBANG { get; set; }
        public decimal DUNONOIBANG { get; set; }
        public string NHOMNO { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// LN03 Details DTO - Complete record for detail views
    /// </summary>
    public class LN03DetailsDto
    {
        public long Id { get; set; }

        // 17 CSV Business Columns
        public string MACHINHANH { get; set; } = string.Empty;
        public string TENCHINHANH { get; set; } = string.Empty;
        public string MAKH { get; set; } = string.Empty;
        public string TENKH { get; set; } = string.Empty;
        public string SOHOPDONG { get; set; } = string.Empty;
        public decimal SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal THUNOSAUXL { get; set; }
        public decimal CONLAINGOAIBANG { get; set; }
        public decimal DUNONOIBANG { get; set; }
        public string NHOMNO { get; set; } = string.Empty;
        public string MACBTD { get; set; } = string.Empty;
        public string TENCBTD { get; set; } = string.Empty;
        public string MAPGD { get; set; } = string.Empty;
        public string TAIKHOANHACHTOAN { get; set; } = string.Empty;
        public string REFNO { get; set; } = string.Empty;
        public string LOAINGUONVON { get; set; } = string.Empty;

        // 3 Additional Business Columns
        public string ADDITIONAL_COL1 { get; set; } = string.Empty;
        public string ADDITIONAL_COL2 { get; set; } = string.Empty;
        public decimal ADDITIONAL_COL3 { get; set; }

        // System Columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? NGAY_DL { get; set; }
    }

    /// <summary>
    /// LN03 Create DTO - For create operations
    /// </summary>
    public class LN03CreateDto
    {
        [Required]
        [StringLength(50)]
        public string MACHINHANH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TENCHINHANH { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MAKH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TENKH { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string SOHOPDONG { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SOTIENXLRR { get; set; }

        public DateTime? NGAYPHATSINHXL { get; set; }

        [Range(0, double.MaxValue)]
        public decimal THUNOSAUXL { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CONLAINGOAIBANG { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNONOIBANG { get; set; }

        [StringLength(50)]
        public string NHOMNO { get; set; } = string.Empty;

        [StringLength(50)]
        public string MACBTD { get; set; } = string.Empty;

        [StringLength(255)]
        public string TENCBTD { get; set; } = string.Empty;

        [StringLength(50)]
        public string MAPGD { get; set; } = string.Empty;

        [StringLength(100)]
        public string TAIKHOANHACHTOAN { get; set; } = string.Empty;

        [StringLength(200)]
        public string REFNO { get; set; } = string.Empty;

        [StringLength(100)]
        public string LOAINGUONVON { get; set; } = string.Empty;

        // 3 Additional columns
        [StringLength(100)]
        public string ADDITIONAL_COL1 { get; set; } = string.Empty;

        [StringLength(100)]
        public string ADDITIONAL_COL2 { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal ADDITIONAL_COL3 { get; set; }

        public DateTime? NGAY_DL { get; set; }
    }

    /// <summary>
    /// LN03 Update DTO - For update operations
    /// </summary>
    public class LN03UpdateDto
    {
        [Required]
        [StringLength(50)]
        public string MACHINHANH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TENCHINHANH { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MAKH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TENKH { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string SOHOPDONG { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SOTIENXLRR { get; set; }

        public DateTime? NGAYPHATSINHXL { get; set; }

        [Range(0, double.MaxValue)]
        public decimal THUNOSAUXL { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CONLAINGOAIBANG { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNONOIBANG { get; set; }

        [StringLength(50)]
        public string NHOMNO { get; set; } = string.Empty;

        [StringLength(50)]
        public string MACBTD { get; set; } = string.Empty;

        [StringLength(255)]
        public string TENCBTD { get; set; } = string.Empty;

        [StringLength(50)]
        public string MAPGD { get; set; } = string.Empty;

        [StringLength(100)]
        public string TAIKHOANHACHTOAN { get; set; } = string.Empty;

        [StringLength(200)]
        public string REFNO { get; set; } = string.Empty;

        [StringLength(100)]
        public string LOAINGUONVON { get; set; } = string.Empty;

        // 3 Additional columns
        [StringLength(100)]
        public string ADDITIONAL_COL1 { get; set; } = string.Empty;

        [StringLength(100)]
        public string ADDITIONAL_COL2 { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal ADDITIONAL_COL3 { get; set; }

        public DateTime? NGAY_DL { get; set; }
    }

    /// <summary>
    /// LN03 Import Result DTO - For CSV import operations
    /// </summary>
    public class LN03ImportResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int SuccessRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public DateTime ImportedAt { get; set; }
        public string ImportedBy { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    /// <summary>
    /// LN03 Summary DTO - For dashboard/statistics display
    /// </summary>
    public class LN03SummaryDto
    {
        public int TotalRecords { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalContracts { get; set; }
        public decimal TotalRiskAmount { get; set; }
        public decimal TotalRecoveryAmount { get; set; }
        public decimal TotalOutstandingBalance { get; set; }
        public decimal TotalInternalBalance { get; set; }
        public DateTime? LatestDataDate { get; set; }
        public DateTime? OldestDataDate { get; set; }
        public List<BranchRiskSummaryDto> BranchSummaries { get; set; } = new();
        public List<RiskGroupSummaryDto> RiskGroupSummaries { get; set; } = new();
    }

    /// <summary>
    /// Branch Risk Summary DTO - Summary by branch
    /// </summary>
    public class BranchRiskSummaryDto
    {
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public decimal TotalRiskAmount { get; set; }
        public decimal TotalRecoveryAmount { get; set; }
        public decimal TotalOutstandingBalance { get; set; }
    }

    /// <summary>
    /// Risk Group Summary DTO - Summary by risk group
    /// </summary>
    public class RiskGroupSummaryDto
    {
        public string RiskGroup { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public decimal TotalRiskAmount { get; set; }
        public decimal TotalOutstandingBalance { get; set; }
    }
}
