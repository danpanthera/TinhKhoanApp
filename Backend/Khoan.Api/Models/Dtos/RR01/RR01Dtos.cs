namespace Khoan.Api.Models.DTOs.RR01;

// Preview DTO for lightweight list views
public class RR01PreviewDto
{
	public int Id { get; set; }
	public DateTime NGAY_DL { get; set; }
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
	public DateTime? CreatedAt { get; set; }
	public string? FileName { get; set; }
}

// Full details DTO for CRUD operations
public class RR01DetailsDto : RR01PreviewDto
{
	public string? SO_LAV { get; set; }
	public string? LOAI_KH { get; set; }
	public DateTime? NGAY_GIAI_NGAN { get; set; }
	public DateTime? NGAY_DEN_HAN { get; set; }
	public string? VAMC_FLG { get; set; }
	public DateTime? NGAY_XLRR { get; set; }
	public decimal? DUNO_GOC_BAN_DAU { get; set; }
	public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
	public decimal? DOC_DAUKY_DA_THU_HT { get; set; }
	public decimal? DUNO_NGAN_HAN { get; set; }
	public decimal? DUNO_TRUNG_HAN { get; set; }
	public decimal? DUNO_DAI_HAN { get; set; }
	public decimal? BDS { get; set; }
	public decimal? DS { get; set; }
	public decimal? TSK { get; set; }
	public string? FILE_NAME { get; set; }
	public DateTime? UpdatedAt { get; set; }
}

// Create DTO
public class RR01CreateDto
{
	public DateTime NGAY_DL { get; set; }
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
	public string? FILE_NAME { get; set; }
}

// Update DTO (same as create for now; separated for future validation differences)
public class RR01UpdateDto : RR01CreateDto { }

// Processing summary DTO
public class RR01ProcessingSummaryDto
{
	public DateTime Date { get; set; }
	public int TotalProcessingRecords { get; set; }
	public decimal TotalProcessingAmount { get; set; }
	public decimal AverageProcessingAmount { get; set; }
	public int VAMCFlaggedCount { get; set; }
	public Dictionary<string, int>? ProcessingByBranch { get; set; }
	public Dictionary<string, decimal>? RecoveryByType { get; set; }
}
