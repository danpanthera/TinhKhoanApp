using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// DP01 Entity - Sổ tiết kiệm (63 business columns)
    /// Represents DP01 table structure with temporal table support
    /// CSV Source: 7800_dp01_20241231.csv (DuLieuMau folder)
    /// Structure: NGAY_DL -> Business Columns (1-63) -> Temporal/System Columns
    /// </summary>
    [Table("DP01")]
    public class DP01Entity : ITemporalEntity
    {
        // === NGAY_DL - FIRST COLUMN ===
        // NGAY_DL - Ngày dữ liệu (lấy từ filename dd/mm/yyyy) - ALWAYS FIRST
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (63 columns from CSV - EXACT ORDER) ===

        // Column 1: MA_CN - Mã chi nhánh
        [StringLength(200)]
        public string? MA_CN { get; set; }

        // Column 2: TAI_KHOAN_HACH_TOAN - Tài khoản hạch toán
        [StringLength(200)]
        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        // Column 3: MA_KH - Mã khách hàng
        [StringLength(200)]
        public string? MA_KH { get; set; }

        // Column 4: TEN_KH - Tên khách hàng
        [StringLength(200)]
        public string? TEN_KH { get; set; }

        // Column 5: DP_TYPE_NAME - Tên loại tiền gửi
        [StringLength(200)]
        public string? DP_TYPE_NAME { get; set; }

        // Column 6: CCY - Loại tiền tệ
        [StringLength(200)]
        public string? CCY { get; set; }

        // Column 7: CURRENT_BALANCE - Số dư hiện tại
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CURRENT_BALANCE { get; set; }

        // Column 8: RATE - Lãi suất
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RATE { get; set; }

        // Column 9: SO_TAI_KHOAN - Số tài khoản
        [StringLength(200)]
        public string? SO_TAI_KHOAN { get; set; }

        // Column 10: OPENING_DATE - Ngày mở sổ
        public DateTime? OPENING_DATE { get; set; }

        // Column 11: MATURITY_DATE - Ngày đến hạn
        public DateTime? MATURITY_DATE { get; set; }

        // Column 12: ADDRESS - Địa chỉ
        [StringLength(1000)]
        public string? ADDRESS { get; set; }

        // Column 13: NOTENO - Số ghi chú
        [StringLength(200)]
        public string? NOTENO { get; set; }

        // Column 14: MONTH_TERM - Kỳ hạn (tháng)
        [StringLength(200)]
        public string? MONTH_TERM { get; set; }

        // Column 15: TERM_DP_NAME - Tên kỳ hạn tiền gửi
        [StringLength(200)]
        public string? TERM_DP_NAME { get; set; }

        // Column 16: TIME_DP_NAME - Tên thời gian tiền gửi
        [StringLength(200)]
        public string? TIME_DP_NAME { get; set; }

        // Column 17: MA_PGD - Mã phòng giao dịch
        [StringLength(200)]
        public string? MA_PGD { get; set; }

        // Column 18: TEN_PGD - Tên phòng giao dịch
        [StringLength(200)]
        public string? TEN_PGD { get; set; }

        // Column 19: DP_TYPE_CODE - Mã loại tiền gửi
        [StringLength(200)]
        public string? DP_TYPE_CODE { get; set; }

        // Column 20: RENEW_DATE - Ngày gia hạn
        public DateTime? RENEW_DATE { get; set; }

        // Column 21: CUST_TYPE - Loại khách hàng
        [StringLength(200)]
        public string? CUST_TYPE { get; set; }

        // Column 22: CUST_TYPE_NAME - Tên loại khách hàng
        [StringLength(200)]
        public string? CUST_TYPE_NAME { get; set; }

        // Column 23: CUST_TYPE_DETAIL - Chi tiết loại khách hàng
        [StringLength(200)]
        public string? CUST_TYPE_DETAIL { get; set; }

        // Column 24: CUST_DETAIL_NAME - Tên chi tiết khách hàng
        [StringLength(200)]
        public string? CUST_DETAIL_NAME { get; set; }

        // Column 25: PREVIOUS_DP_CAP_DATE - Ngày gốc trước
        public DateTime? PREVIOUS_DP_CAP_DATE { get; set; }

        // Column 26: NEXT_DP_CAP_DATE - Ngày gốc tiếp theo
        public DateTime? NEXT_DP_CAP_DATE { get; set; }

        // Column 27: ID_NUMBER - Số CMND/CCCD
        [StringLength(200)]
        public string? ID_NUMBER { get; set; }

        // Column 28: ISSUED_BY - Nơi cấp
        [StringLength(200)]
        public string? ISSUED_BY { get; set; }

        // Column 29: ISSUE_DATE - Ngày cấp
        public DateTime? ISSUE_DATE { get; set; }

        // Column 30: SEX_TYPE - Giới tính
        [StringLength(200)]
        public string? SEX_TYPE { get; set; }

        // Column 31: BIRTH_DATE - Ngày sinh
        public DateTime? BIRTH_DATE { get; set; }

        // Column 32: TELEPHONE - Số điện thoại
        [StringLength(200)]
        public string? TELEPHONE { get; set; }

        // Column 33: ACRUAL_AMOUNT - Số tiền tích lũy
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ACRUAL_AMOUNT { get; set; }

        // Column 34: ACRUAL_AMOUNT_END - Số tiền tích lũy cuối kỳ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ACRUAL_AMOUNT_END { get; set; }

        // Column 35: ACCOUNT_STATUS - Trạng thái tài khoản
        [StringLength(200)]
        public string? ACCOUNT_STATUS { get; set; }

        // Column 36: DRAMT - Số tiền nợ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DRAMT { get; set; }

        // Column 37: CRAMT - Số tiền có
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CRAMT { get; set; }

        // Column 38: EMPLOYEE_NUMBER - Mã nhân viên
        [StringLength(200)]
        public string? EMPLOYEE_NUMBER { get; set; }

        // Column 39: EMPLOYEE_NAME - Tên nhân viên
        [StringLength(200)]
        public string? EMPLOYEE_NAME { get; set; }

        // Column 40: SPECIAL_RATE - Lãi suất đặc biệt
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SPECIAL_RATE { get; set; }

        // Column 41: AUTO_RENEWAL - Tự động gia hạn
        [StringLength(200)]
        public string? AUTO_RENEWAL { get; set; }

        // Column 42: CLOSE_DATE - Ngày đóng
        public DateTime? CLOSE_DATE { get; set; }

        // Column 43: LOCAL_PROVIN_NAME - Tên tỉnh
        [StringLength(200)]
        public string? LOCAL_PROVIN_NAME { get; set; }

        // Column 44: LOCAL_DISTRICT_NAME - Tên huyện
        [StringLength(200)]
        public string? LOCAL_DISTRICT_NAME { get; set; }

        // Column 45: LOCAL_WARD_NAME - Tên xã/phường
        [StringLength(200)]
        public string? LOCAL_WARD_NAME { get; set; }

        // Column 46: TERM_DP_TYPE - Loại kỳ hạn tiền gửi
        [StringLength(200)]
        public string? TERM_DP_TYPE { get; set; }

        // Column 47: TIME_DP_TYPE - Loại thời gian tiền gửi
        [StringLength(200)]
        public string? TIME_DP_TYPE { get; set; }

        // Column 48: STATES_CODE - Mã tỉnh/bang
        [StringLength(200)]
        public string? STATES_CODE { get; set; }

        // Column 49: ZIP_CODE - Mã bưu chính
        [StringLength(200)]
        public string? ZIP_CODE { get; set; }

        // Column 50: COUNTRY_CODE - Mã quốc gia
        [StringLength(200)]
        public string? COUNTRY_CODE { get; set; }

        // Column 51: TAX_CODE_LOCATION - Mã thuế địa điểm
        [StringLength(200)]
        public string? TAX_CODE_LOCATION { get; set; }

        // Column 52: MA_CAN_BO_PT - Mã cán bộ phụ trách
        [StringLength(200)]
        public string? MA_CAN_BO_PT { get; set; }

        // Column 53: TEN_CAN_BO_PT - Tên cán bộ phụ trách
        [StringLength(200)]
        public string? TEN_CAN_BO_PT { get; set; }

        // Column 54: PHONG_CAN_BO_PT - Phòng cán bộ phụ trách
        [StringLength(200)]
        public string? PHONG_CAN_BO_PT { get; set; }

        // Column 55: NGUOI_NUOC_NGOAI - Người nước ngoài
        [StringLength(200)]
        public string? NGUOI_NUOC_NGOAI { get; set; }

        // Column 56: QUOC_TICH - Quốc tịch
        [StringLength(200)]
        public string? QUOC_TICH { get; set; }

        // Column 57: MA_CAN_BO_AGRIBANK - Mã cán bộ Agribank
        [StringLength(200)]
        public string? MA_CAN_BO_AGRIBANK { get; set; }

        // Column 58: NGUOI_GIOI_THIEU - Người giới thiệu
        [StringLength(200)]
        public string? NGUOI_GIOI_THIEU { get; set; }

        // Column 59: TEN_NGUOI_GIOI_THIEU - Tên người giới thiệu
        [StringLength(200)]
        public string? TEN_NGUOI_GIOI_THIEU { get; set; }

        // Column 60: CONTRACT_COUTS_DAY - Số ngày hợp đồng
        [StringLength(200)]
        public string? CONTRACT_COUTS_DAY { get; set; }

        // Column 61: SO_KY_AD_LSDB - Số kỳ AD LSDB
        [StringLength(200)]
        public string? SO_KY_AD_LSDB { get; set; }

        // Column 62: UNTBUSCD - Mã đơn vị kinh doanh
        [StringLength(200)]
        public string? UNTBUSCD { get; set; }

        // Column 63: TYGIA - Tỷ giá
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TYGIA { get; set; }

        // === TEMPORAL TABLE COLUMNS (cuối cùng theo yêu cầu) ===
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }

        // === SYSTEM COLUMNS (cuối cùng) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
