using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos.DP01
{
    /// <summary>
    /// DP01 Preview DTO - Hiển thị danh sách sổ tiết kiệm với tất cả fields
    /// </summary>
    public class DP01PreviewDto
    {
        public long Id { get; set; }

        // NGAY_DL từ filename
        public DateTime? NGAY_DL { get; set; }

        // 63 Business Columns - EXACT matching với Model DP01
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

        // System Audit Fields
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DP01 Create DTO - Tạo mới sổ tiết kiệm
    /// </summary>
    public class DP01CreateDto
    {
        // NGAY_DL từ filename
        public DateTime? NGAY_DL { get; set; }

        [Required]
        public string? MA_CN { get; set; }

        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        [Required]
        public string? MA_KH { get; set; }

        [Required]
        public string? TEN_KH { get; set; }

        public string? DP_TYPE_NAME { get; set; }
        public string? CCY { get; set; }
        public decimal? CURRENT_BALANCE { get; set; }
        public decimal? RATE { get; set; }

        [Required]
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
    }

    /// <summary>
    /// DP01 Update DTO - Cập nhật sổ tiết kiệm
    /// </summary>
    public class DP01UpdateDto
    {
        // NGAY_DL từ filename
        public DateTime? NGAY_DL { get; set; }

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
    }

    /// <summary>
    /// DP01 Details DTO - Chi tiết đầy đủ sổ tiết kiệm
    /// </summary>
    public class DP01DetailsDto
    {
        public long Id { get; set; }

        // NGAY_DL từ filename
        public DateTime? NGAY_DL { get; set; }

        // 63 Business Columns
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

        // System Audit Fields
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DP01 Summary DTO - Thống kê tổng hợp
    /// </summary>
    public class DP01SummaryDto
    {
        public int TotalRecords { get; set; }
        public decimal? TotalBalance { get; set; }
        public decimal? AverageBalance { get; set; }
        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }
        public int ActiveAccounts { get; set; }
        public int ClosedAccounts { get; set; }
        public Dictionary<string, int> BranchDistribution { get; set; } = new();
        public Dictionary<string, decimal> CurrencyDistribution { get; set; } = new();
    }

    /// <summary>
    /// DP01 Import Result DTO - Kết quả import
    /// </summary>
    public class DP01ImportResultDto
    {
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public string? FileName { get; set; }
        public DateTime ImportTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    /// <summary>
    /// DP01 Search Request DTO
    /// </summary>
    public class DP01SearchRequestDto
    {
        public string? MA_CN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? SO_TAI_KHOAN { get; set; }
        public string? DP_TYPE_NAME { get; set; }
        public string? CCY { get; set; }
        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ACCOUNT_STATUS { get; set; }
    }

    /// <summary>
    /// DP01 Analytics DTO - Phân tích dữ liệu
    /// </summary>
    public class DP01AnalyticsDto
    {
        public Dictionary<string, decimal> BalanceByBranch { get; set; } = new();
        public Dictionary<string, decimal> BalanceByCurrency { get; set; } = new();
        public Dictionary<string, int> AccountsByType { get; set; } = new();
        public Dictionary<string, decimal> BalanceByCustomerType { get; set; } = new();
        public Dictionary<DateTime, decimal> BalanceByMonth { get; set; } = new();
    }

    /// <summary>
    /// DP01 Branch Statistics DTO
    /// </summary>
    public class DP01BranchStatisticsDto
    {
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AverageBalance { get; set; }
        public int ActiveAccounts { get; set; }
        public int ClosedAccounts { get; set; }
    }
}
