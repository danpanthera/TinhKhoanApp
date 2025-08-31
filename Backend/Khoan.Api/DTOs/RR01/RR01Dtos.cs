using System.ComponentModel.DataAnnotations;

namespace Khoan.Api.Dtos.RR01
{
    /// <summary>
    /// RR01 Preview DTO - Hiển thị tóm tắt cho listing/grid
    /// Sử dụng business column names từ CSV, không transformation tiếng Việt
    /// </summary>
    public class RR01PreviewDto
    {
        public int Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // Key business fields for preview (exact CSV column names)
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
        public decimal? BDS { get; set; }
        public decimal? DS { get; set; }

        // System fields
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// RR01 Details DTO - Chi tiết đầy đủ với tất cả 25 business columns
    /// Column names trùng khớp hoàn toàn với CSV headers
    /// </summary>
    public class RR01DetailsDto
    {
        public int Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // === 25 BUSINESS COLUMNS (exact từ CSV) ===
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

        // System columns
        public DateTime CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public bool IS_DELETED { get; set; }

        // Temporal columns (for history tracking)
        public DateTime? SysStartTime { get; set; }
        public DateTime? SysEndTime { get; set; }
    }

    /// <summary>
    /// RR01 Create DTO - Tạo mới record với business column validation
    /// </summary>
    public class CreateRR01Dto
    {
        public DateTime? NGAY_DL { get; set; }

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
    }

    /// <summary>
    /// RR01 Update DTO - Cập nhật record
    /// </summary>
    public class UpdateRR01Dto : CreateRR01Dto
    {
    }

    /// <summary>
    /// RR01 Summary DTO - Analytics tổng hợp theo business logic
    /// </summary>
    public class RR01SummaryDto
    {
        public DateTime NGAY_DL { get; set; }
        public int TotalRecords { get; set; }
        public int UniqueBranches { get; set; }
        public int UniqueCustomers { get; set; }

        // Tổng amounts theo business columns
        public decimal TotalDUNO_GOC_HIENTAI { get; set; }
        public decimal TotalDUNO_LAI_HIENTAI { get; set; }
        public decimal TotalTHU_GOC { get; set; }
        public decimal TotalTHU_LAI { get; set; }
        public decimal TotalBDS { get; set; }
        public decimal TotalDS { get; set; }
        public decimal TotalTSK { get; set; }

        // Recovery analytics
        public decimal RecoveryRate => TotalDUNO_GOC_HIENTAI > 0 ? (TotalTHU_GOC / TotalDUNO_GOC_HIENTAI) * 100 : 0;
        public decimal InterestRecoveryRate => TotalDUNO_LAI_HIENTAI > 0 ? (TotalTHU_LAI / TotalDUNO_LAI_HIENTAI) * 100 : 0;
    }

    /// <summary>
    /// RR01 Import Result DTO - Kết quả CSV import
    /// </summary>
    public class RR01ImportResultDto
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
        
        // Business validation metrics
        public int RecordsWithVAMCFlag { get; set; }
        public int RecordsWithRecovery { get; set; }
        public decimal TotalProcessedAmount { get; set; }
        
        public TimeSpan ProcessingDuration => ProcessingEndTime - ProcessingStartTime;
        public decimal SuccessRate => TotalRows > 0 ? (decimal)SuccessfulRows / TotalRows * 100 : 0;
    }

    /// <summary>
    /// RR01 Branch Analytics DTO - Phân tích theo chi nhánh
    /// </summary>
    public class RR01BranchAnalyticsDto
    {
        public string? BRCD { get; set; }
        public string? BranchName { get; set; }
        public int CustomerCount { get; set; }
        public int LoanCount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalRecoveredAmount { get; set; }
        public decimal RecoveryPercentage { get; set; }
        public int VAMCFlaggedCount { get; set; }
    }
}
