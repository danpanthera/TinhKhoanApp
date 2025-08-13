using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TinhKhoanApp.Api.Models.Entities;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// LN01 - Loan Data Model with exact CSV column structure
    /// TEMPORAL TABLE: System-versioned temporal table với Columnstore Indexes
    /// Structure: NGAY_DL -> 79 Business Columns (CSV order) -> Temporal System Columns
    /// Import policy: Chỉ cho phép files có chứa "ln01" trong filename
    /// Đặc biệt: NGAY_DL lấy từ filename extraction (không có trong CSV)
    /// DATE columns: datetime2 format (dd/MM/yyyy)
    /// AMOUNT columns: decimal(18,2) format #,###.00
    /// </summary>
    [Table("LN01")]
    public class LN01 : ITemporalEntity
    {
        // Auto-increment ID
        /// <summary>
        /// Auto-increment ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // System Column - NGAY_DL first (extracted from filename)
        /// <summary>
        /// Ngày dữ liệu - trích xuất từ tên file (vd: 7800_LN01_20241231.csv -> 2024-12-31)
        /// </summary>
        [Column("NGAY_DL")]
        public DateTime? NGAY_DL { get; set; }

        // Business Columns - Exact CSV order with correct data types
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column("BRCD")]
        [StringLength(200)]
        public string? BRCD { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [Column("CUSTSEQ")]
        [StringLength(200)]
        public string? CUSTSEQ { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column("CUSTNM")]
        [StringLength(200)]
        public string? CUSTNM { get; set; }

        /// <summary>
        /// Số tài khoản
        /// </summary>
        [Column("TAI_KHOAN")]
        [StringLength(200)]
        public string? TAI_KHOAN { get; set; }

        /// <summary>
        /// Loại tiền tệ
        /// </summary>
        [Column("CCY")]
        [StringLength(200)]
        public string? CCY { get; set; }

        /// <summary>
        /// Dư nợ
        /// </summary>
        [Column("DU_NO", TypeName = "decimal(18,2)")]
        public decimal? DU_NO { get; set; }

        /// <summary>
        /// Mã giải ngân
        /// </summary>
        [Column("DSBSSEQ")]
        [StringLength(200)]
        public string? DSBSSEQ { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        [Column("TRANSACTION_DATE")]
        public DateTime? TRANSACTION_DATE { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        [Column("DSBSDT")]
        public DateTime? DSBSDT { get; set; }

        /// <summary>
        /// Loại tiền giải ngân
        /// </summary>
        [Column("DISBUR_CCY")]
        [StringLength(200)]
        public string? DISBUR_CCY { get; set; }

        /// <summary>
        /// Số tiền giải ngân
        /// </summary>
        [Column("DISBURSEMENT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        /// <summary>
        /// Ngày đáo hạn giải ngân
        /// </summary>
        [Column("DSBSMATDT")]
        public DateTime? DSBSMATDT { get; set; }

        /// <summary>
        /// Mã lãi suất
        /// </summary>
        [Column("BSRTCD")]
        [StringLength(200)]
        public string? BSRTCD { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        [Column("INTEREST_RATE", TypeName = "decimal(18,2)")]
        public decimal? INTEREST_RATE { get; set; }

        /// <summary>
        /// Mã phê duyệt
        /// </summary>
        [Column("APPRSEQ")]
        [StringLength(200)]
        public string? APPRSEQ { get; set; }

        /// <summary>
        /// Ngày phê duyệt
        /// </summary>
        [Column("APPRDT")]
        public DateTime? APPRDT { get; set; }

        /// <summary>
        /// Loại tiền phê duyệt
        /// </summary>
        [Column("APPR_CCY")]
        [StringLength(200)]
        public string? APPR_CCY { get; set; }

        /// <summary>
        /// Số tiền phê duyệt
        /// </summary>
        [Column("APPRAMT", TypeName = "decimal(18,2)")]
        public decimal? APPRAMT { get; set; }

        /// <summary>
        /// Ngày đáo hạn phê duyệt
        /// </summary>
        [Column("APPRMATDT")]
        public DateTime? APPRMATDT { get; set; }

        /// <summary>
        /// Loại khoản vay
        /// </summary>
        [Column("LOAN_TYPE")]
        [StringLength(200)]
        public string? LOAN_TYPE { get; set; }

        /// <summary>
        /// Mã nguồn vốn
        /// </summary>
        [Column("FUND_RESOURCE_CODE")]
        [StringLength(200)]
        public string? FUND_RESOURCE_CODE { get; set; }

        /// <summary>
        /// Mã mục đích vay
        /// </summary>
        [Column("FUND_PURPOSE_CODE")]
        [StringLength(200)]
        public string? FUND_PURPOSE_CODE { get; set; }

        /// <summary>
        /// Số tiền trả
        /// </summary>
        [Column("REPAYMENT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        /// <summary>
        /// Ngày trả tiếp theo
        /// </summary>
        [Column("NEXT_REPAY_DATE")]
        public DateTime? NEXT_REPAY_DATE { get; set; }

        /// <summary>
        /// Số tiền trả tiếp theo
        /// </summary>
        [Column("NEXT_REPAY_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        /// <summary>
        /// Ngày trả lãi tiếp theo
        /// </summary>
        [Column("NEXT_INT_REPAY_DATE")]
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        /// <summary>
        /// ID cán bộ
        /// </summary>
        [Column("OFFICER_ID")]
        [StringLength(200)]
        public string? OFFICER_ID { get; set; }

        /// <summary>
        /// Tên cán bộ
        /// </summary>
        [Column("OFFICER_NAME")]
        [StringLength(200)]
        public string? OFFICER_NAME { get; set; }

        /// <summary>
        /// Số tiền lãi
        /// </summary>
        [Column("INTEREST_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? INTEREST_AMOUNT { get; set; }

        /// <summary>
        /// Số tiền lãi quá hạn
        /// </summary>
        [Column("PASTDUE_INTEREST_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }

        /// <summary>
        /// Tổng số tiền lãi trả
        /// </summary>
        [Column("TOTAL_INTEREST_REPAY_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        /// <summary>
        /// Mã loại khách hàng
        /// </summary>
        [Column("CUSTOMER_TYPE_CODE")]
        [StringLength(200)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        /// <summary>
        /// Mã loại khách hàng chi tiết
        /// </summary>
        [Column("CUSTOMER_TYPE_CODE_DETAIL")]
        [StringLength(200)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        /// <summary>
        /// Mã ngành
        /// </summary>
        [Column("TRCTCD")]
        [StringLength(200)]
        public string? TRCTCD { get; set; }

        /// <summary>
        /// Tên ngành
        /// </summary>
        [Column("TRCTNM")]
        [StringLength(200)]
        public string? TRCTNM { get; set; }

        /// <summary>
        /// Địa chỉ 1
        /// </summary>
        [Column("ADDR1")]
        [StringLength(200)]
        public string? ADDR1 { get; set; }

        /// <summary>
        /// Tỉnh/thành phố
        /// </summary>
        [Column("PROVINCE")]
        [StringLength(200)]
        public string? PROVINCE { get; set; }

        /// <summary>
        /// Tên tỉnh
        /// </summary>
        [Column("LCLPROVINNM")]
        [StringLength(200)]
        public string? LCLPROVINNM { get; set; }

        /// <summary>
        /// Quận/huyện
        /// </summary>
        [Column("DISTRICT")]
        [StringLength(200)]
        public string? DISTRICT { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        [Column("LCLDISTNM")]
        [StringLength(200)]
        public string? LCLDISTNM { get; set; }

        /// <summary>
        /// Mã xã/phường
        /// </summary>
        [Column("COMMCD")]
        [StringLength(200)]
        public string? COMMCD { get; set; }

        /// <summary>
        /// Tên xã/phường
        /// </summary>
        [Column("LCLWARDNM")]
        [StringLength(200)]
        public string? LCLWARDNM { get; set; }

        /// <summary>
        /// Ngày trả gần nhất
        /// </summary>
        [Column("LAST_REPAY_DATE")]
        public DateTime? LAST_REPAY_DATE { get; set; }

        /// <summary>
        /// Phần trăm đảm bảo
        /// </summary>
        [Column("SECURED_PERCENT")]
        [StringLength(200)]
        public string? SECURED_PERCENT { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [Column("NHOM_NO")]
        [StringLength(200)]
        public string? NHOM_NO { get; set; }

        /// <summary>
        /// Ngày tính lãi gần nhất
        /// </summary>
        [Column("LAST_INT_CHARGE_DATE")]
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        /// <summary>
        /// Miễn lãi
        /// </summary>
        [Column("EXEMPTINT")]
        [StringLength(200)]
        public string? EXEMPTINT { get; set; }

        /// <summary>
        /// Loại miễn lãi
        /// </summary>
        [Column("EXEMPTINTTYPE")]
        [StringLength(200)]
        public string? EXEMPTINTTYPE { get; set; }

        /// <summary>
        /// Số tiền miễn lãi
        /// </summary>
        [Column("EXEMPTINTAMT", TypeName = "decimal(18,2)")]
        public decimal? EXEMPTINTAMT { get; set; }

        /// <summary>
        /// Nhóm số
        /// </summary>
        [Column("GRPNO")]
        [StringLength(200)]
        public string? GRPNO { get; set; }

        /// <summary>
        /// Mã kinh doanh
        /// </summary>
        [Column("BUSCD")]
        [StringLength(200)]
        public string? BUSCD { get; set; }

        /// <summary>
        /// Mã loại khách hàng kinh doanh
        /// </summary>
        [Column("BSNSSCLTPCD")]
        [StringLength(200)]
        public string? BSNSSCLTPCD { get; set; }

        /// <summary>
        /// ID người dùng thao tác
        /// </summary>
        [Column("USRIDOP")]
        [StringLength(200)]
        public string? USRIDOP { get; set; }

        /// <summary>
        /// Số tiền dồn tích
        /// </summary>
        [Column("ACCRUAL_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        /// <summary>
        /// Số tiền dồn tích cuối tháng
        /// </summary>
        [Column("ACCRUAL_AMOUNT_END_OF_MONTH", TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        /// <summary>
        /// Phương thức tính lãi
        /// </summary>
        [Column("INTCMTH")]
        [StringLength(200)]
        public string? INTCMTH { get; set; }

        /// <summary>
        /// Phương thức trả lãi
        /// </summary>
        [Column("INTRPYMTH")]
        [StringLength(200)]
        public string? INTRPYMTH { get; set; }

        /// <summary>
        /// Kỳ hạn lãi
        /// </summary>
        [Column("INTTRMMTH")]
        [StringLength(200)]
        public string? INTTRMMTH { get; set; }

        /// <summary>
        /// Số ngày trong năm
        /// </summary>
        [Column("YRDAYS")]
        [StringLength(200)]
        public string? YRDAYS { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Column("REMARK")]
        [StringLength(1000)]
        public string? REMARK { get; set; }

        /// <summary>
        /// Chỉ tiêu
        /// </summary>
        [Column("CHITIEU")]
        [StringLength(200)]
        public string? CHITIEU { get; set; }

        /// <summary>
        /// CTCV
        /// </summary>
        [Column("CTCV")]
        [StringLength(200)]
        public string? CTCV { get; set; }

        /// <summary>
        /// Loại hạn mức tín dụng
        /// </summary>
        [Column("CREDIT_LINE_YPE")]
        [StringLength(200)]
        public string? CREDIT_LINE_YPE { get; set; }

        /// <summary>
        /// Loại trả lãi lump sum một phần
        /// </summary>
        [Column("INT_LUMPSUM_PARTIAL_TYPE")]
        [StringLength(200)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        /// <summary>
        /// Loại thanh toán lãi một phần
        /// </summary>
        [Column("INT_PARTIAL_PAYMENT_TYPE")]
        [StringLength(200)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        /// <summary>
        /// Khoảng thời gian thanh toán lãi
        /// </summary>
        [Column("INT_PAYMENT_INTERVAL")]
        [StringLength(200)]
        public string? INT_PAYMENT_INTERVAL { get; set; }

        /// <summary>
        /// Ân hạn lãi
        /// </summary>
        [Column("AN_HAN_LAI")]
        [StringLength(200)]
        public string? AN_HAN_LAI { get; set; }

        /// <summary>
        /// Phương thức giải ngân 1
        /// </summary>
        [Column("PHUONG_THUC_GIAI_NGAN_1")]
        [StringLength(200)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        /// <summary>
        /// Tài khoản giải ngân 1
        /// </summary>
        [Column("TAI_KHOAN_GIAI_NGAN_1")]
        [StringLength(200)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        /// <summary>
        /// Số tiền giải ngân 1
        /// </summary>
        [Column("SO_TIEN_GIAI_NGAN_1", TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        /// <summary>
        /// Phương thức giải ngân 2
        /// </summary>
        [Column("PHUONG_THUC_GIAI_NGAN_2")]
        [StringLength(200)]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        /// <summary>
        /// Tài khoản giải ngân 2
        /// </summary>
        [Column("TAI_KHOAN_GIAI_NGAN_2")]
        [StringLength(200)]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        /// <summary>
        /// Số tiền giải ngân 2
        /// </summary>
        [Column("SO_TIEN_GIAI_NGAN_2", TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }

        /// <summary>
        /// CMND/CCCD/Hộ chiếu
        /// </summary>
        [Column("CMT_HC")]
        [StringLength(200)]
        public string? CMT_HC { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [Column("NGAY_SINH")]
        public DateTime? NGAY_SINH { get; set; }

        /// <summary>
        /// Mã cán bộ Agribank
        /// </summary>
        [Column("MA_CB_AGRI")]
        [StringLength(200)]
        public string? MA_CB_AGRI { get; set; }

        /// <summary>
        /// Mã ngành kinh tế
        /// </summary>
        [Column("MA_NGANH_KT")]
        [StringLength(200)]
        public string? MA_NGANH_KT { get; set; }

        /// <summary>
        /// Tỷ giá
        /// </summary>
        [Column("TY_GIA", TypeName = "decimal(18,2)")]
        public decimal? TY_GIA { get; set; }

        /// <summary>
        /// Cán bộ IPCAS
        /// </summary>
        [Column("OFFICER_IPCAS")]
        [StringLength(200)]
        public string? OFFICER_IPCAS { get; set; }

        // System Columns - common for all data tables
        /// <summary>
        /// Tên file nguồn
        /// </summary>
        [Column("FILE_NAME")]
        [StringLength(500)]
        public string? FILE_NAME { get; set; }

        // === SYSTEM COLUMNS (IEntity interface) ===
        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Column("CREATED_DATE")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        [Column("UPDATED_DATE")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // === TEMPORAL COLUMNS (cuối cùng - ITemporalEntity interface) ===
        /// <summary>
        /// Temporal table start time - System generated (ITemporalEntity interface)
        /// </summary>
        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        /// <summary>
        /// Temporal table end time - System generated (ITemporalEntity interface)
        /// </summary>
        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }
    }
}
