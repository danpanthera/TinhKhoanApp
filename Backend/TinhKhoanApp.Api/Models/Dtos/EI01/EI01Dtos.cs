using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.EI01
{
    /// <summary>
    /// EI01 (E-Banking Info) DTOs - 24 business columns
    /// CSV Structure: MA_CN,MA_KH,TEN_KH,LOAI_KH,SDT_EMB,TRANG_THAI_EMB,NGAY_DK_EMB,SDT_OTT,TRANG_THAI_OTT,NGAY_DK_OTT,SDT_SMS,TRANG_THAI_SMS,NGAY_DK_SMS,SDT_SAV,TRANG_THAI_SAV,NGAY_DK_SAV,SDT_LN,TRANG_THAI_LN,NGAY_DK_LN,USER_EMB,USER_OTT,USER_SMS,USER_SAV,USER_LN
    /// </summary>

    /// <summary>
    /// EI01 Preview DTO - For list/grid display
    /// </summary>
    public class EI01PreviewDto
    {
        public long Id { get; set; }
        public string MA_CN { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public string SDT_EMB { get; set; } = string.Empty;
        public string TRANG_THAI_EMB { get; set; } = string.Empty;
        public DateTime? NGAY_DK_EMB { get; set; }
        public string SDT_SMS { get; set; } = string.Empty;
        public string TRANG_THAI_SMS { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// EI01 Create DTO - For create operations
    /// </summary>
    public class EI01CreateDto
    {
        [Required]
        [StringLength(10)]
        public string MA_CN { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MA_KH { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TEN_KH { get; set; } = string.Empty;

        [StringLength(50)]
        public string LOAI_KH { get; set; } = string.Empty;

        [StringLength(20)]
        public string SDT_EMB { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRANG_THAI_EMB { get; set; } = string.Empty;

        public DateTime? NGAY_DK_EMB { get; set; }

        [StringLength(20)]
        public string SDT_OTT { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRANG_THAI_OTT { get; set; } = string.Empty;

        public DateTime? NGAY_DK_OTT { get; set; }

        [StringLength(20)]
        public string SDT_SMS { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRANG_THAI_SMS { get; set; } = string.Empty;

        public DateTime? NGAY_DK_SMS { get; set; }

        [StringLength(20)]
        public string SDT_SAV { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRANG_THAI_SAV { get; set; } = string.Empty;

        public DateTime? NGAY_DK_SAV { get; set; }

        [StringLength(20)]
        public string SDT_LN { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRANG_THAI_LN { get; set; } = string.Empty;

        public DateTime? NGAY_DK_LN { get; set; }

        [StringLength(100)]
        public string USER_EMB { get; set; } = string.Empty;

        [StringLength(100)]
        public string USER_OTT { get; set; } = string.Empty;

        [StringLength(100)]
        public string USER_SMS { get; set; } = string.Empty;

        [StringLength(100)]
        public string USER_SAV { get; set; } = string.Empty;

        [StringLength(100)]
        public string USER_LN { get; set; } = string.Empty;
    }

    /// <summary>
    /// EI01 Update DTO - For update operations
    /// </summary>
    public class EI01UpdateDto
    {
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [StringLength(20)]
        public string? SDT_EMB { get; set; }

        [StringLength(50)]
        public string? TRANG_THAI_EMB { get; set; }

        public DateTime? NGAY_DK_EMB { get; set; }

        [StringLength(20)]
        public string? SDT_OTT { get; set; }

        [StringLength(50)]
        public string? TRANG_THAI_OTT { get; set; }

        public DateTime? NGAY_DK_OTT { get; set; }

        [StringLength(20)]
        public string? SDT_SMS { get; set; }

        [StringLength(50)]
        public string? TRANG_THAI_SMS { get; set; }

        public DateTime? NGAY_DK_SMS { get; set; }

        [StringLength(20)]
        public string? SDT_SAV { get; set; }

        [StringLength(50)]
        public string? TRANG_THAI_SAV { get; set; }

        public DateTime? NGAY_DK_SAV { get; set; }

        [StringLength(20)]
        public string? SDT_LN { get; set; }

        [StringLength(50)]
        public string? TRANG_THAI_LN { get; set; }

        public DateTime? NGAY_DK_LN { get; set; }
    }

    /// <summary>
    /// EI01 Details DTO - For full record display
    /// </summary>
    public class EI01DetailsDto
    {
        public long Id { get; set; }
        public string MA_CN { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public string SDT_EMB { get; set; } = string.Empty;
        public string TRANG_THAI_EMB { get; set; } = string.Empty;
        public DateTime? NGAY_DK_EMB { get; set; }
        public string SDT_OTT { get; set; } = string.Empty;
        public string TRANG_THAI_OTT { get; set; } = string.Empty;
        public DateTime? NGAY_DK_OTT { get; set; }
        public string SDT_SMS { get; set; } = string.Empty;
        public string TRANG_THAI_SMS { get; set; } = string.Empty;
        public DateTime? NGAY_DK_SMS { get; set; }
        public string SDT_SAV { get; set; } = string.Empty;
        public string TRANG_THAI_SAV { get; set; } = string.Empty;
        public DateTime? NGAY_DK_SAV { get; set; }
        public string SDT_LN { get; set; } = string.Empty;
        public string TRANG_THAI_LN { get; set; } = string.Empty;
        public DateTime? NGAY_DK_LN { get; set; }
        public string USER_EMB { get; set; } = string.Empty;
        public string USER_OTT { get; set; } = string.Empty;
        public string USER_SMS { get; set; } = string.Empty;
        public string USER_SAV { get; set; } = string.Empty;
        public string USER_LN { get; set; } = string.Empty;

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// EI01 Summary DTO - For aggregate/summary data
    /// </summary>
    public class EI01SummaryDto
    {
        public long TotalRecords { get; set; }
        public Dictionary<string, long> RecordsByBranch { get; set; } = new();
        public Dictionary<string, long> RecordsByCustomerType { get; set; } = new();

        // E-Banking service statistics
        public long TotalEMBUsers { get; set; }
        public long ActiveEMBUsers { get; set; }
        public long TotalOTTUsers { get; set; }
        public long ActiveOTTUsers { get; set; }
        public long TotalSMSUsers { get; set; }
        public long ActiveSMSUsers { get; set; }
        public long TotalSAVUsers { get; set; }
        public long ActiveSAVUsers { get; set; }
        public long TotalLNUsers { get; set; }
        public long ActiveLNUsers { get; set; }

        public Dictionary<string, long> UsersByService { get; set; } = new();
        public Dictionary<string, long> ActiveUsersByService { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// EI01 Import Result DTO - For import operation results
    /// </summary>
    public class EI01ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long TotalRecords { get; set; }
        public long ProcessedRecords { get; set; }
        public long SuccessfulRecords { get; set; }
        public long FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public DateTime ImportedAt { get; set; }
        public Dictionary<string, object> ImportStatistics { get; set; } = new();
    }
}
