using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.RR01
{
    /// <summary>
    /// RR01 Preview DTO - để hiển thị preview data từ bảng RR01
    /// Chứa 25 business columns theo CSV structure + system columns
    /// </summary>
    public class RR01PreviewDto
    {
        // === SYSTEM COLUMNS ===
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (25 columns - CSV aligned) ===
        public string? CN_LOAI_I { get; set; }
        public string? BRCD { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public decimal? SO_LDS { get; set; }
        public string? CCY { get; set; }
        public string? SO_LAV { get; set; }
        public string? LOAI_KH { get; set; }
        public DateTime? NGAY_GIAI_NGAN { get; set; }
        public DateTime? NGAY_DAO_HAN { get; set; }
        public DateTime? NGAY_DEN_HAN { get; set; }
        public string? VAMC_FLG { get; set; }
        public DateTime? NGAY_XLRR { get; set; }
        public decimal? SO_TIEN_GIAI_NGAN { get; set; }
        public decimal? DUNO_GOC_BAN_DAU { get; set; }
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
        public decimal? DOC_DAUKY_DA_THU_GOC { get; set; }
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }
        public decimal? SL_TRA_LAI_TRONG_KY { get; set; }
        public decimal? SL_TRA_GOC_TRONG_KY { get; set; }
        public decimal? DUNO_CL_CUOIKY { get; set; }
        public decimal? DUNO_LAI_TICHLUY_CK { get; set; }
        public decimal? DUNO_GOC_HIENTAI { get; set; }
        public decimal? DUNO_LAI_HIENTAI { get; set; }
        public decimal? DUNO_NGAN_HAN { get; set; }
        public decimal? DUNO_TRUNG_HAN { get; set; }
        public decimal? DUNO_DAI_HAN { get; set; }
        public string? NHOM_NO { get; set; }
        public string? NHOM_NO_HT { get; set; }
        public string? DT_PHAN_LOAI { get; set; }
        public string? NHAN_VIEN_CHT { get; set; }
        public decimal? THU_GOC { get; set; }
        public decimal? THU_LAI { get; set; }
        public decimal? BDS { get; set; }
        public decimal? DS { get; set; }
        public decimal? TSK { get; set; }

        // === TEMPORAL/SYSTEM METADATA ===
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// RR01 Create DTO - để tạo record mới RR01
    /// </summary>
    public class RR01CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (25 columns - CSV aligned) ===
        [MaxLength(200)]
        public string? CN_LOAI_I { get; set; }

        [MaxLength(200)]
        public string? BRCD { get; set; }

        [MaxLength(200)]
        public string? MA_KH { get; set; }

        [MaxLength(200)]
        public string? TEN_KH { get; set; }

        public decimal? SO_LDS { get; set; }

        [MaxLength(200)]
        public string? CCY { get; set; }

        [MaxLength(200)]
        public string? SO_LAV { get; set; }

        [MaxLength(200)]
        public string? LOAI_KH { get; set; }

        public DateTime? NGAY_GIAI_NGAN { get; set; }  // DATE field
        public DateTime? NGAY_DEN_HAN { get; set; }    // DATE field

        [MaxLength(200)]
        public string? VAMC_FLG { get; set; }

        public DateTime? NGAY_XLRR { get; set; }       // DATE field

        // DUNO fields - decimal type as per requirements
        public decimal? DUNO_GOC_BAN_DAU { get; set; }
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }  // DATHU field
        public decimal? DUNO_GOC_HIENTAI { get; set; }
        public decimal? DUNO_LAI_HIENTAI { get; set; }
        public decimal? DUNO_NGAN_HAN { get; set; }
        public decimal? DUNO_TRUNG_HAN { get; set; }
        public decimal? DUNO_DAI_HAN { get; set; }

        // THU_GOC, THU_LAI fields - decimal type as per requirements
        public decimal? THU_GOC { get; set; }
        public decimal? THU_LAI { get; set; }

        // BDS, DS fields - decimal type as per requirements
        public decimal? BDS { get; set; }
        public decimal? DS { get; set; }

        public decimal? TSK { get; set; }

        // === SYSTEM METADATA ===
        [MaxLength(400)]
        public string? FileName { get; set; }
    }

    /// <summary>
    /// RR01 Update DTO - để cập nhật record RR01
    /// </summary>
    public class RR01UpdateDto : RR01CreateDto
    {
        [Required]
        public long Id { get; set; }
    }

    /// <summary>
    /// RR01 Details DTO - để hiển thị chi tiết record RR01
    /// Bao gồm temporal tracking information
    /// </summary>
    public class RR01DetailsDto : RR01PreviewDto
    {
        // === TEMPORAL TRACKING ===
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }

        // === AUDIT INFORMATION ===
        public string? ImportBatch { get; set; }
        public string? DataSource { get; set; }

        // === METADATA ===
        public int RecordVersion { get; set; }
        public bool IsActive { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// RR01 Summary DTO - để hiển thị tóm tắt/aggregate data
    /// </summary>
    public class RR01SummaryDto
    {
        public DateTime NgayDL { get; set; }
        public int TotalRecords { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalOutstandingPrincipal { get; set; }
        public decimal TotalAccumulatedInterest { get; set; }
        public decimal TotalInterestRepayments { get; set; }
        public decimal TotalPrincipalRepayments { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// RR01 Import Result DTO - kết quả import CSV
    /// </summary>
    public class RR01ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int SuccessRecords { get; set; }
        public int FailedRecords { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime NgayDL { get; set; }
        public string FileName { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
