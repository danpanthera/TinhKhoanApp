using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos.DP01
{
    /// <summary>
    /// DP01 Preview DTO - để hiển thị preview data từ bảng DP01
    /// Chứa 63 business columns theo CSV structure + system columns
    /// </summary>
    public class DP01PreviewDto
    {
        // === SYSTEM COLUMNS ===
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (63 columns - CSV aligned) ===
        public string? MA_CN { get; set; }
        public string? TAI_KHOAN_HACH_TOAN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? DP_TYPE_NAME { get; set; }
        public string? CCY { get; set; }
        public decimal? CURRENT_BALANCE { get; set; }
        public decimal? RATE { get; set; }
        public string? SO_TAI_KHOAN { get; set; }
        public DateTime? OPENING_DATE { get; set; }
        public DateTime? MATURITY_DATE { get; set; }
        public string? ADDRESS { get; set; }
        public string? NOTENO { get; set; }
        public string? MONTH_TERM { get; set; }
        public string? TERM_DP_NAME { get; set; }
        public string? TIME_DP_NAME { get; set; }
        public string? MA_PGD { get; set; }
        public string? TEN_PGD { get; set; }
        public string? DP_TYPE_CODE { get; set; }
        public DateTime? RENEW_DATE { get; set; }
        public string? CUST_TYPE { get; set; }
        public string? CUST_TYPE_NAME { get; set; }
        public string? CUST_TYPE_DETAIL { get; set; }
        public string? CUST_DETAIL_NAME { get; set; }
        public DateTime? PREVIOUS_DP_CAP_DATE { get; set; }
        public DateTime? NEXT_DP_CAP_DATE { get; set; }
        public string? ID_NUMBER { get; set; }
        public string? ISSUED_BY { get; set; }
        public DateTime? ISSUE_DATE { get; set; }
        public string? SEX_TYPE { get; set; }
        public DateTime? BIRTH_DATE { get; set; }
        public string? TELEPHONE { get; set; }
        public decimal? ACRUAL_AMOUNT { get; set; }
        public decimal? ACRUAL_AMOUNT_END { get; set; }
        public string? ACCOUNT_STATUS { get; set; }
        public decimal? DRAMT { get; set; }
        public decimal? CRAMT { get; set; }
        public string? EMPLOYEE_NUMBER { get; set; }
        public string? EMPLOYEE_NAME { get; set; }
        public decimal? SPECIAL_RATE { get; set; }
        public string? AUTO_RENEWAL { get; set; }
        public DateTime? CLOSE_DATE { get; set; }
        public string? LOCAL_PROVIN_NAME { get; set; }
        public string? LOCAL_DISTRICT_NAME { get; set; }
        public string? LOCAL_WARD_NAME { get; set; }
        public string? TERM_DP_TYPE { get; set; }
        public string? TIME_DP_TYPE { get; set; }
        public string? STATES_CODE { get; set; }
        public string? ZIP_CODE { get; set; }
        public string? COUNTRY_CODE { get; set; }
        public string? TAX_CODE_LOCATION { get; set; }
        public string? MA_CAN_BO_PT { get; set; }
        public string? TEN_CAN_BO_PT { get; set; }
        public string? PHONG_CAN_BO_PT { get; set; }
        public string? NGUOI_NUOC_NGOAI { get; set; }
        public string? QUOC_TICH { get; set; }
        public string? MA_CAN_BO_AGRIBANK { get; set; }
        public string? NGUOI_GIOI_THIEU { get; set; }
        public string? TEN_NGUOI_GIOI_THIEU { get; set; }
        public string? CONTRACT_COUTS_DAY { get; set; }
        public string? SO_KY_AD_LSDB { get; set; }
        public string? UNTBUSCD { get; set; }
        public decimal? TYGIA { get; set; }

        // === TEMPORAL/SYSTEM METADATA ===
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DP01 Create DTO - để tạo record mới DP01
    /// </summary>
    public class DP01CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (63 columns - CSV aligned) ===
        [Required]
        [MaxLength(200)]
        public string MA_CN { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        [MaxLength(200)]
        public string? MA_KH { get; set; }

        [MaxLength(200)]
        public string? TEN_KH { get; set; }

        [MaxLength(200)]
        public string? DP_TYPE_NAME { get; set; }

        [MaxLength(200)]
        public string? CCY { get; set; }

        public decimal? CURRENT_BALANCE { get; set; }
        public decimal? RATE { get; set; }

        [MaxLength(200)]
        public string? SO_TAI_KHOAN { get; set; }

        public DateTime? OPENING_DATE { get; set; }
        public DateTime? MATURITY_DATE { get; set; }

        [MaxLength(1000)]  // Special length for ADDRESS field
        public string? ADDRESS { get; set; }

        [MaxLength(200)]
        public string? NOTENO { get; set; }

        [MaxLength(200)]
        public string? MONTH_TERM { get; set; }

        [MaxLength(200)]
        public string? TERM_DP_NAME { get; set; }

        [MaxLength(200)]
        public string? TIME_DP_NAME { get; set; }

        [MaxLength(200)]
        public string? MA_PGD { get; set; }

        [MaxLength(200)]
        public string? TEN_PGD { get; set; }

        [MaxLength(200)]
        public string? DP_TYPE_CODE { get; set; }

        public DateTime? RENEW_DATE { get; set; }

        [MaxLength(200)]
        public string? CUST_TYPE { get; set; }

        [MaxLength(200)]
        public string? CUST_TYPE_NAME { get; set; }

        [MaxLength(200)]
        public string? CUST_TYPE_DETAIL { get; set; }

        [MaxLength(200)]
        public string? CUST_DETAIL_NAME { get; set; }

        public DateTime? PREVIOUS_DP_CAP_DATE { get; set; }
        public DateTime? NEXT_DP_CAP_DATE { get; set; }

        [MaxLength(200)]
        public string? ID_NUMBER { get; set; }

        [MaxLength(200)]
        public string? ISSUED_BY { get; set; }

        public DateTime? ISSUE_DATE { get; set; }

        [MaxLength(200)]
        public string? SEX_TYPE { get; set; }

        public DateTime? BIRTH_DATE { get; set; }

        [MaxLength(200)]
        public string? TELEPHONE { get; set; }

        public decimal? ACRUAL_AMOUNT { get; set; }
        public decimal? ACRUAL_AMOUNT_END { get; set; }

        [MaxLength(200)]
        public string? ACCOUNT_STATUS { get; set; }

        public decimal? DRAMT { get; set; }
        public decimal? CRAMT { get; set; }

        [MaxLength(200)]
        public string? EMPLOYEE_NUMBER { get; set; }

        [MaxLength(200)]
        public string? EMPLOYEE_NAME { get; set; }

        public decimal? SPECIAL_RATE { get; set; }

        [MaxLength(200)]
        public string? AUTO_RENEWAL { get; set; }

        public DateTime? CLOSE_DATE { get; set; }

        [MaxLength(200)]
        public string? LOCAL_PROVIN_NAME { get; set; }

        [MaxLength(200)]
        public string? LOCAL_DISTRICT_NAME { get; set; }

        [MaxLength(200)]
        public string? LOCAL_WARD_NAME { get; set; }

        [MaxLength(200)]
        public string? TERM_DP_TYPE { get; set; }

        [MaxLength(200)]
        public string? TIME_DP_TYPE { get; set; }

        [MaxLength(200)]
        public string? STATES_CODE { get; set; }

        [MaxLength(200)]
        public string? ZIP_CODE { get; set; }

        [MaxLength(200)]
        public string? COUNTRY_CODE { get; set; }

        [MaxLength(200)]
        public string? TAX_CODE_LOCATION { get; set; }

        [MaxLength(200)]
        public string? MA_CAN_BO_PT { get; set; }

        [MaxLength(200)]
        public string? TEN_CAN_BO_PT { get; set; }

        [MaxLength(200)]
        public string? PHONG_CAN_BO_PT { get; set; }

        [MaxLength(200)]
        public string? NGUOI_NUOC_NGOAI { get; set; }

        [MaxLength(200)]
        public string? QUOC_TICH { get; set; }

        [MaxLength(200)]
        public string? MA_CAN_BO_AGRIBANK { get; set; }

        [MaxLength(200)]
        public string? NGUOI_GIOI_THIEU { get; set; }

        [MaxLength(200)]
        public string? TEN_NGUOI_GIOI_THIEU { get; set; }

        [MaxLength(200)]
        public string? CONTRACT_COUTS_DAY { get; set; }

        [MaxLength(200)]
        public string? SO_KY_AD_LSDB { get; set; }

        [MaxLength(200)]
        public string? UNTBUSCD { get; set; }

        public decimal? TYGIA { get; set; }

        // === SYSTEM METADATA ===
        [MaxLength(400)]
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DP01 Update DTO - để cập nhật record DP01
    /// </summary>
    public class DP01UpdateDto : DP01CreateDto
    {
        [Required]
        public long Id { get; set; }
    }

    /// <summary>
    /// DP01 Details DTO - để hiển thị chi tiết record DP01
    /// Bao gồm temporal tracking information
    /// </summary>
    public class DP01DetailsDto : DP01PreviewDto
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
    /// DP01 Summary DTO - để hiển thị tóm tắt/aggregate data
    /// </summary>
    public class DP01SummaryDto
    {
        public DateTime NgayDL { get; set; }
        public int TotalRecords { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AverageBalance { get; set; }
        public string? TopCurrency { get; set; }
        public DateTime LastImportDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DP01 Import Result DTO - kết quả import CSV
    /// </summary>
    public class DP01ImportResultDto
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
