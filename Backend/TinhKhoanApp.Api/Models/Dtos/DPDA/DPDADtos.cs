using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.DPDA
{
    /// <summary>
    /// DPDA (Debit Card) DTOs - 13 business columns
    /// CSV Structure: MA_CHI_NHANH,MA_KHACH_HANG,TEN_KHACH_HANG,SO_TAI_KHOAN,LOAI_THE,SO_THE,NGAY_NOP_DON,NGAY_PHAT_HANH,USER_PHAT_HANH,TRANG_THAI,PHAN_LOAI,GIAO_THE,LOAI_PHAT_HANH
    /// </summary>

    /// <summary>
    /// DPDA Preview DTO - For list/grid display
    /// </summary>
    public class DPDAPreviewDto
    {
        public long Id { get; set; }
        public string MA_CHI_NHANH { get; set; } = string.Empty;
        public string MA_KHACH_HANG { get; set; } = string.Empty;
        public string TEN_KHACH_HANG { get; set; } = string.Empty;
        public string SO_TAI_KHOAN { get; set; } = string.Empty;
        public string LOAI_THE { get; set; } = string.Empty;
        public string SO_THE { get; set; } = string.Empty;
        public DateTime? NGAY_NOP_DON { get; set; }
        public DateTime? NGAY_PHAT_HANH { get; set; }
        public string TRANG_THAI { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DPDA Create DTO - For create operations
    /// </summary>
    public class DPDACreateDto
    {
        [Required]
        [StringLength(50)]
        public string MA_CHI_NHANH { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MA_KHACH_HANG { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TEN_KHACH_HANG { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SO_TAI_KHOAN { get; set; } = string.Empty;

        [StringLength(50)]
        public string LOAI_THE { get; set; } = string.Empty;

        [StringLength(50)]
        public string SO_THE { get; set; } = string.Empty;

        public DateTime? NGAY_NOP_DON { get; set; }
        public DateTime? NGAY_PHAT_HANH { get; set; }

        [StringLength(100)]
        public string USER_PHAT_HANH { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRANG_THAI { get; set; } = string.Empty;

        [StringLength(50)]
        public string PHAN_LOAI { get; set; } = string.Empty;

        [StringLength(50)]
        public string GIAO_THE { get; set; } = string.Empty;

        [StringLength(50)]
        public string LOAI_PHAT_HANH { get; set; } = string.Empty;
    }

    /// <summary>
    /// DPDA Update DTO - For update operations
    /// </summary>
    public class DPDAUpdateDto
    {
        [StringLength(255)]
        public string? TEN_KHACH_HANG { get; set; }

        [StringLength(50)]
        public string? LOAI_THE { get; set; }

        public DateTime? NGAY_NOP_DON { get; set; }
        public DateTime? NGAY_PHAT_HANH { get; set; }

        [StringLength(100)]
        public string? USER_PHAT_HANH { get; set; }

        [StringLength(50)]
        public string? TRANG_THAI { get; set; }

        [StringLength(50)]
        public string? PHAN_LOAI { get; set; }

        [StringLength(50)]
        public string? GIAO_THE { get; set; }

        [StringLength(50)]
        public string? LOAI_PHAT_HANH { get; set; }
    }

    /// <summary>
    /// DPDA Details DTO - For full record display
    /// </summary>
    public class DPDADetailsDto
    {
        public long Id { get; set; }
        public string MA_CHI_NHANH { get; set; } = string.Empty;
        public string MA_KHACH_HANG { get; set; } = string.Empty;
        public string TEN_KHACH_HANG { get; set; } = string.Empty;
        public string SO_TAI_KHOAN { get; set; } = string.Empty;
        public string LOAI_THE { get; set; } = string.Empty;
        public string SO_THE { get; set; } = string.Empty;
        public DateTime? NGAY_NOP_DON { get; set; }
        public DateTime? NGAY_PHAT_HANH { get; set; }
        public string USER_PHAT_HANH { get; set; } = string.Empty;
        public string TRANG_THAI { get; set; } = string.Empty;
        public string PHAN_LOAI { get; set; } = string.Empty;
        public string GIAO_THE { get; set; } = string.Empty;
        public string LOAI_PHAT_HANH { get; set; } = string.Empty;

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// DPDA Summary DTO - For aggregate/summary data
    /// </summary>
    public class DPDASummaryDto
    {
        public long TotalRecords { get; set; }
        public Dictionary<string, long> RecordsByBranch { get; set; } = new();
        public Dictionary<string, long> RecordsByCardType { get; set; } = new();
        public Dictionary<string, long> RecordsByStatus { get; set; } = new();
        public long TotalActiveCards { get; set; }
        public long TotalInactiveCards { get; set; }
        public long TotalDeliveredCards { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// DPDA Import Result DTO - For import operation results
    /// </summary>
    public class DPDAImportResultDto
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
