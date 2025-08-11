using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.RR01
{
    /// <summary>
    /// RR01 (Exchange Rate Data) DTOs - 25 business columns
    /// CSV Structure: CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN,VAMC_FLG,NGAY_XLRR,DUNO_GOC_BAN_DAU,DUNO_LAI_TICHLUY_BD,DOC_DAUKY_DA_THU_HT,DUNO_GOC_HIENTAI,DUNO_LAI_HIENTAI,DUNO_NGAN_HAN,DUNO_TRUNG_HAN,DUNO_DAI_HAN,THU_GOC,THU_LAI,BDS,DS,TSK
    /// </summary>

    /// <summary>
    /// RR01 Preview DTO - For list/grid display (key fields only)
    /// </summary>
    public class RR01PreviewDto
    {
        public long Id { get; set; }
        public string CN_LOAI_I { get; set; } = string.Empty;
        public string BRCD { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public string SO_LDS { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string SO_LAV { get; set; } = string.Empty;
        public decimal DUNO_GOC_BAN_DAU { get; set; }
        public decimal DUNO_GOC_HIENTAI { get; set; }
        public decimal DUNO_LAI_HIENTAI { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// RR01 Details DTO - Complete record for detail views
    /// </summary>
    public class RR01DetailsDto
    {
        public long Id { get; set; }

        // 25 Business Columns
        public string CN_LOAI_I { get; set; } = string.Empty;
        public string BRCD { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public string SO_LDS { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string SO_LAV { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public DateTime? NGAY_GIAI_NGAN { get; set; }
        public DateTime? NGAY_DEN_HAN { get; set; }
        public string VAMC_FLG { get; set; } = string.Empty;
        public DateTime? NGAY_XLRR { get; set; }
        public decimal DUNO_GOC_BAN_DAU { get; set; }
        public decimal DUNO_LAI_TICHLUY_BD { get; set; }
        public decimal DOC_DAUKY_DA_THU_HT { get; set; }
        public decimal DUNO_GOC_HIENTAI { get; set; }
        public decimal DUNO_LAI_HIENTAI { get; set; }
        public decimal DUNO_NGAN_HAN { get; set; }
        public decimal DUNO_TRUNG_HAN { get; set; }
        public decimal DUNO_DAI_HAN { get; set; }
        public decimal THU_GOC { get; set; }
        public decimal THU_LAI { get; set; }
        public decimal BDS { get; set; }
        public decimal DS { get; set; }
        public decimal TSK { get; set; }

        // System Columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? NGAY_DL { get; set; }
    }

    /// <summary>
    /// RR01 Create DTO - For create operations
    /// </summary>
    public class RR01CreateDto
    {
        [Required]
        [StringLength(50)]
        public string CN_LOAI_I { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BRCD { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MA_KH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TEN_KH { get; set; } = string.Empty;

        [StringLength(50)]
        public string SO_LDS { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string CCY { get; set; } = string.Empty;

        [StringLength(50)]
        public string SO_LAV { get; set; } = string.Empty;

        [StringLength(10)]
        public string LOAI_KH { get; set; } = string.Empty;

        public DateTime? NGAY_GIAI_NGAN { get; set; }
        public DateTime? NGAY_DEN_HAN { get; set; }

        [StringLength(10)]
        public string VAMC_FLG { get; set; } = string.Empty;

        public DateTime? NGAY_XLRR { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_GOC_BAN_DAU { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_LAI_TICHLUY_BD { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DOC_DAUKY_DA_THU_HT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_GOC_HIENTAI { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_LAI_HIENTAI { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_NGAN_HAN { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_TRUNG_HAN { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_DAI_HAN { get; set; }

        [Range(0, double.MaxValue)]
        public decimal THU_GOC { get; set; }

        [Range(0, double.MaxValue)]
        public decimal THU_LAI { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BDS { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DS { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TSK { get; set; }

        public DateTime? NGAY_DL { get; set; }
    }

    /// <summary>
    /// RR01 Update DTO - For update operations
    /// </summary>
    public class RR01UpdateDto
    {
        [Required]
        [StringLength(50)]
        public string CN_LOAI_I { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BRCD { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MA_KH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TEN_KH { get; set; } = string.Empty;

        [StringLength(50)]
        public string SO_LDS { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string CCY { get; set; } = string.Empty;

        [StringLength(50)]
        public string SO_LAV { get; set; } = string.Empty;

        [StringLength(10)]
        public string LOAI_KH { get; set; } = string.Empty;

        public DateTime? NGAY_GIAI_NGAN { get; set; }
        public DateTime? NGAY_DEN_HAN { get; set; }

        [StringLength(10)]
        public string VAMC_FLG { get; set; } = string.Empty;

        public DateTime? NGAY_XLRR { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_GOC_BAN_DAU { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_LAI_TICHLUY_BD { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DOC_DAUKY_DA_THU_HT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_GOC_HIENTAI { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_LAI_HIENTAI { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_NGAN_HAN { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_TRUNG_HAN { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DUNO_DAI_HAN { get; set; }

        [Range(0, double.MaxValue)]
        public decimal THU_GOC { get; set; }

        [Range(0, double.MaxValue)]
        public decimal THU_LAI { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BDS { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DS { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TSK { get; set; }

        public DateTime? NGAY_DL { get; set; }
    }

    /// <summary>
    /// RR01 Import Result DTO - For CSV import operations
    /// </summary>
    public class RR01ImportResultDto
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
    /// RR01 Summary DTO - For dashboard/statistics display
    /// </summary>
    public class RR01SummaryDto
    {
        public int TotalRecords { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCurrencies { get; set; }
        public decimal TotalOutstandingBalance { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalRecoveries { get; set; }
        public DateTime? LatestDataDate { get; set; }
        public DateTime? OldestDataDate { get; set; }
        public List<BranchSummaryDto> BranchSummaries { get; set; } = new();
        public List<CurrencySummaryDto> CurrencySummaries { get; set; } = new();
    }

    /// <summary>
    /// Branch Summary DTO - Summary by branch
    /// </summary>
    public class BranchSummaryDto
    {
        public string BranchCode { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalInterest { get; set; }
    }

    /// <summary>
    /// Currency Summary DTO - Summary by currency
    /// </summary>
    public class CurrencySummaryDto
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalInterest { get; set; }
    }
}
