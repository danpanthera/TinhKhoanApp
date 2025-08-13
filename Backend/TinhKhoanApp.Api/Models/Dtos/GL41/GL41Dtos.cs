using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.GL41
{
    /// <summary>
    /// GL41 DTOs - 13 business columns từ CSV GL41
    /// </summary>

    /// <summary>
    /// DTO cho preview/listing GL41
    /// </summary>
    public class GL41PreviewDto
    {
        public DateTime NGAY_DL { get; set; }
        public string? MA_CN { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? MA_TK { get; set; }
        public string? TEN_TK { get; set; }
        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }
    }

    /// <summary>
    /// DTO cho GL41 details
    /// </summary>
    public class GL41DetailsDto
    {
        public long ID { get; set; }
        public DateTime NGAY_DL { get; set; }

        // 13 Business Columns theo CSV
        public string? MA_CN { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? MA_TK { get; set; }
        public string? TEN_TK { get; set; }
        public string? LOAI_BT { get; set; }
        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? SBT_NO { get; set; }
        public decimal? ST_GHINO { get; set; }
        public decimal? SBT_CO { get; set; }
        public decimal? ST_GHICO { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }

        // System fields
        public string? FILE_NAME { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO cho tạo mới GL41
    /// </summary>
    public class GL41CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Required]
        [StringLength(200)]
        public string MA_CN { get; set; } = null!;

        [StringLength(200)]
        public string? LOAI_TIEN { get; set; }

        [StringLength(200)]
        public string? MA_TK { get; set; }

        [StringLength(200)]
        public string? TEN_TK { get; set; }

        [StringLength(200)]
        public string? LOAI_BT { get; set; }

        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? SBT_NO { get; set; }
        public decimal? ST_GHINO { get; set; }
        public decimal? SBT_CO { get; set; }
        public decimal? ST_GHICO { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật GL41
    /// </summary>
    public class GL41UpdateDto
    {
        [Required]
        public long ID { get; set; }

        [Required]
        public DateTime NGAY_DL { get; set; }

        [Required]
        [StringLength(200)]
        public string MA_CN { get; set; } = null!;

        [StringLength(200)]
        public string? LOAI_TIEN { get; set; }

        [StringLength(200)]
        public string? MA_TK { get; set; }

        [StringLength(200)]
        public string? TEN_TK { get; set; }

        [StringLength(200)]
        public string? LOAI_BT { get; set; }

        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? SBT_NO { get; set; }
        public decimal? ST_GHINO { get; set; }
        public decimal? SBT_CO { get; set; }
        public decimal? ST_GHICO { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê GL41 theo đơn vị
    /// </summary>
    public class GL41SummaryByUnitDto
    {
        public string? MA_CN { get; set; }
        public decimal TotalOpeningBalance { get; set; }
        public decimal TotalClosingBalance { get; set; }
        public decimal TotalDebitTransactions { get; set; }
        public decimal TotalCreditTransactions { get; set; }
    }

    /// <summary>
    /// DTO cho kết quả import GL41
    /// </summary>
    public class GL41ImportResultDto
    {
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string? ImportSessionId { get; set; }
    }
}
