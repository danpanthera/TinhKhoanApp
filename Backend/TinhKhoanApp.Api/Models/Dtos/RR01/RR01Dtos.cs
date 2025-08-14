using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.RR01;

/// <summary>
/// RR01 Preview DTO - Hiển thị tóm tắt cho danh sách
/// </summary>
public class RR01PreviewDto
{
    public int Id { get; set; }
    public DateTime NGAY_DL { get; set; }

    // Key business fields for preview
    public string? CN_LOAI_I { get; set; }
    public string? BRCD { get; set; }
    public string? MA_KH { get; set; }
    public string? TEN_KH { get; set; }
    public string? SO_LDS { get; set; }
    public string? CCY { get; set; }
    public decimal? DUNO_GOC_HIENTAI { get; set; }
    public decimal? DUNO_LAI_HIENTAI { get; set; }
    public decimal? THU_GOC { get; set; }
    public decimal? THU_LAI { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public string? FileName { get; set; }
}

/// <summary>
/// RR01 Details DTO - Chi tiết đầy đủ 25 business columns
/// </summary>
public class RR01DetailsDto
{
    public int Id { get; set; }
    public DateTime NGAY_DL { get; set; }

    // === 25 BUSINESS COLUMNS ===
    public string? CN_LOAI_I { get; set; }
    public string? BRCD { get; set; }
    public string? MA_KH { get; set; }
    public string? TEN_KH { get; set; }
    public string? SO_LDS { get; set; }
    public string? CCY { get; set; }
    public string? SO_LAV { get; set; }
    public string? LOAI_KH { get; set; }
    public DateTime? NGAY_GIAI_NGAN { get; set; }
    public DateTime? NGAY_DEN_HAN { get; set; }
    public string? VAMC_FLG { get; set; }
    public DateTime? NGAY_XLRR { get; set; }
    public decimal? DUNO_GOC_BAN_DAU { get; set; }
    public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
    public decimal? DOC_DAUKY_DA_THU_HT { get; set; }
    public decimal? DUNO_GOC_HIENTAI { get; set; }
    public decimal? DUNO_LAI_HIENTAI { get; set; }
    public decimal? DUNO_NGAN_HAN { get; set; }
    public decimal? DUNO_TRUNG_HAN { get; set; }
    public decimal? DUNO_DAI_HAN { get; set; }
    public decimal? THU_GOC { get; set; }
    public decimal? THU_LAI { get; set; }
    public decimal? BDS { get; set; }
    public decimal? DS { get; set; }
    public decimal? TSK { get; set; }

    // System fields
    public string? FILE_NAME { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// RR01 Create DTO - Tạo mới record
/// </summary>
public class RR01CreateDto
{
    [Required]
    public DateTime NGAY_DL { get; set; }

    // === 25 BUSINESS COLUMNS ===
    public string? CN_LOAI_I { get; set; }
    public string? BRCD { get; set; }
    public string? MA_KH { get; set; }
    public string? TEN_KH { get; set; }
    public string? SO_LDS { get; set; }
    public string? CCY { get; set; }
    public string? SO_LAV { get; set; }
    public string? LOAI_KH { get; set; }
    public DateTime? NGAY_GIAI_NGAN { get; set; }
    public DateTime? NGAY_DEN_HAN { get; set; }
    public string? VAMC_FLG { get; set; }
    public DateTime? NGAY_XLRR { get; set; }
    public decimal? DUNO_GOC_BAN_DAU { get; set; }
    public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
    public decimal? DOC_DAUKY_DA_THU_HT { get; set; }
    public decimal? DUNO_GOC_HIENTAI { get; set; }
    public decimal? DUNO_LAI_HIENTAI { get; set; }
    public decimal? DUNO_NGAN_HAN { get; set; }
    public decimal? DUNO_TRUNG_HAN { get; set; }
    public decimal? DUNO_DAI_HAN { get; set; }
    public decimal? THU_GOC { get; set; }
    public decimal? THU_LAI { get; set; }
    public decimal? BDS { get; set; }
    public decimal? DS { get; set; }
    public decimal? TSK { get; set; }

    // System fields
    public string? FILE_NAME { get; set; }
}

/// <summary>
/// RR01 Update DTO - Cập nhật record
/// </summary>
public class RR01UpdateDto : RR01CreateDto
{
}

/// <summary>
/// RR01 Summary DTO - Tóm tắt thống kê
/// </summary>
public class RR01SummaryDto
{
    public DateTime Date { get; set; }
    public int TotalRecords { get; set; }
    public decimal TotalDuNoGoc { get; set; }
    public decimal TotalDuNoLai { get; set; }
    public decimal TotalThuGoc { get; set; }
    public decimal TotalThuLai { get; set; }
    public decimal TotalBDS { get; set; }
    public decimal TotalDS { get; set; }
    public decimal TotalTSK { get; set; }
    public int UniqueBranches { get; set; }
    public int UniqueCustomers { get; set; }
}

/// <summary>
/// RR01 Processing Summary DTO - Báo cáo xử lý rủi ro
/// </summary>
public class RR01ProcessingSummaryDto
{
    public DateTime Date { get; set; }
    public int TotalProcessingRecords { get; set; }
    public decimal TotalProcessingAmount { get; set; }
    public decimal AverageProcessingAmount { get; set; }
    public int VAMCFlaggedCount { get; set; }
    public Dictionary<string, int> ProcessingByBranch { get; set; } = new();
    public Dictionary<string, decimal> RecoveryByType { get; set; } = new();
}
