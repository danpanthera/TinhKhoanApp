using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos.GL01
{
    // Preview DTO for list views
    public class GL01PreviewDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? TEN_TK { get; set; }
        public decimal? SO_TIEN_GD { get; set; }
        public string? DR_CR { get; set; }
        public string? POST_BR { get; set; }
        public DateTime? NGAY_GD { get; set; }
        public string? MA_KH { get; set; }
    }

    // Details DTO (full entity representation)
    public class GL01DetailsDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        public string? STS { get; set; }
        public DateTime? NGAY_GD { get; set; }
        public string? NGUOI_TAO { get; set; }
        public string? DYSEQ { get; set; }
        public string? TR_TYPE { get; set; }
        public string? DT_SEQ { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? TEN_TK { get; set; }
        public decimal? SO_TIEN_GD { get; set; }
        public string? POST_BR { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? DR_CR { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? CCA_USRID { get; set; }
        public decimal? TR_EX_RT { get; set; }
        public string? REMARK { get; set; }
        public string? BUS_CODE { get; set; }
        public string? UNIT_BUS_CODE { get; set; }
        public string? TR_CODE { get; set; }
        public string? TR_NAME { get; set; }
        public string? REFERENCE { get; set; }
        public DateTime? VALUE_DATE { get; set; }
        public string? DEPT_CODE { get; set; }
        public string? TR_TIME { get; set; }
        public string? COMFIRM { get; set; }
        public string? TRDT_TIME { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string? FILE_NAME { get; set; }
    }

    // Create DTO
    public class GL01CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }
        [Required]
        [MaxLength(200)]
        public string TAI_KHOAN { get; set; } = string.Empty;
        [Required]
        [MaxLength(2)]
        public string DR_CR { get; set; } = string.Empty;
        public decimal? SO_TIEN_GD { get; set; }
        public string? POST_BR { get; set; }
    }

    // Update DTO
    public class GL01UpdateDto
    {
        public decimal? SO_TIEN_GD { get; set; }
        public string? REMARK { get; set; }
    }

    // Summary DTOs
    public class GL01SummaryByUnitDto
    {
        public string? POST_BR { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }

    public class GL01ImportResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string DataType { get; set; } = "GL01";
        public int ProcessedRecords { get; set; }
        public bool Success { get; set; }
        public string? NgayDL { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
