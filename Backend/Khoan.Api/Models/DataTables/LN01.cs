using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// LN01 - Loan Data Model with exact CSV column structure
    /// TEMPORAL TABLE: System-versioned temporal table với Columnstore Indexes
    /// Structure: NGAY_DL -> 79 Business Columns (CSV order) -> System Columns
    /// Import policy: Chỉ cho phép files có chứa "ln01" trong filename
    /// Đặc biệt: NGAY_DL lấy từ filename extraction (không có trong CSV)
    /// DATE columns: datetime2 format (dd/MM/yyyy)
    /// AMOUNT columns: decimal(18,2) format #,###.00
    /// Note: Uses EF Core temporal tables with shadow properties for SysStartTime/SysEndTime
    /// </summary>
    [Table("LN01")]
    public class LN01
    {
        // === NGAY_DL FIRST - POSITION 1 ===
        /// <summary>
        /// Ngày dữ liệu - trích xuất từ tên file (vd: 7800_LN01_20241231.csv -> 2024-12-31)
        /// </summary>
        [Column("NGAY_DL", Order = 1)]
        public DateTime? NGAY_DL { get; set; }

        // === 79 BUSINESS COLUMNS - POSITIONS 2-80 ===
        [Column("BRCD", Order = 2)]
        [StringLength(200)]
        public string? BRCD { get; set; }

        [Column("CUSTSEQ", Order = 3)]
        [StringLength(200)]
        public string? CUSTSEQ { get; set; }

        [Column("CUSTNM", Order = 4)]
        [StringLength(200)]
        public string? CUSTNM { get; set; }

        [Column("TAI_KHOAN", Order = 5)]
        [StringLength(200)]
        public string? TAI_KHOAN { get; set; }

        [Column("CCY", Order = 6)]
        [StringLength(200)]
        public string? CCY { get; set; }

        [Column("DU_NO", Order = 7, TypeName = "decimal(18,2)")]
        public decimal? DU_NO { get; set; }

        [Column("DSBSSEQ", Order = 8)]
        [StringLength(200)]
        public string? DSBSSEQ { get; set; }

        [Column("TRANSACTION_DATE", Order = 9)]
        public DateTime? TRANSACTION_DATE { get; set; }

        [Column("DSBSDT", Order = 10)]
        public DateTime? DSBSDT { get; set; }

        [Column("DISBUR_CCY", Order = 11)]
        [StringLength(200)]
        public string? DISBUR_CCY { get; set; }

        [Column("DISBURSEMENT_AMOUNT", Order = 12, TypeName = "decimal(18,2)")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        [Column("DSBSMATDT", Order = 13)]
        public DateTime? DSBSMATDT { get; set; }

        [Column("BSRTCD", Order = 14)]
        [StringLength(200)]
        public string? BSRTCD { get; set; }

        [Column("INTEREST_RATE", Order = 15, TypeName = "decimal(18,2)")]
        public decimal? INTEREST_RATE { get; set; }

        [Column("APPRSEQ", Order = 16)]
        [StringLength(200)]
        public string? APPRSEQ { get; set; }

        [Column("APPRDT", Order = 17)]
        public DateTime? APPRDT { get; set; }

        [Column("APPR_CCY", Order = 18)]
        [StringLength(200)]
        public string? APPR_CCY { get; set; }

        [Column("APPRAMT", Order = 19, TypeName = "decimal(18,2)")]
        public decimal? APPRAMT { get; set; }

        [Column("APPRMATDT", Order = 20)]
        public DateTime? APPRMATDT { get; set; }

        [Column("LOAN_TYPE", Order = 21)]
        [StringLength(200)]
        public string? LOAN_TYPE { get; set; }

        [Column("FUND_RESOURCE_CODE", Order = 22)]
        [StringLength(200)]
        public string? FUND_RESOURCE_CODE { get; set; }

        [Column("FUND_PURPOSE_CODE", Order = 23)]
        [StringLength(200)]
        public string? FUND_PURPOSE_CODE { get; set; }

        [Column("REPAYMENT_AMOUNT", Order = 24, TypeName = "decimal(18,2)")]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        [Column("NEXT_REPAY_DATE", Order = 25)]
        public DateTime? NEXT_REPAY_DATE { get; set; }

        [Column("NEXT_REPAY_AMOUNT", Order = 26, TypeName = "decimal(18,2)")]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        [Column("NEXT_INT_REPAY_DATE", Order = 27)]
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        [Column("OFFICER_ID", Order = 28)]
        [StringLength(200)]
        public string? OFFICER_ID { get; set; }

        [Column("OFFICER_NAME", Order = 29)]
        [StringLength(200)]
        public string? OFFICER_NAME { get; set; }

        [Column("INTEREST_AMOUNT", Order = 30, TypeName = "decimal(18,2)")]
        public decimal? INTEREST_AMOUNT { get; set; }

        [Column("PASTDUE_INTEREST_AMOUNT", Order = 31, TypeName = "decimal(18,2)")]
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }

        [Column("TOTAL_INTEREST_REPAY_AMOUNT", Order = 32, TypeName = "decimal(18,2)")]
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [Column("CUSTOMER_TYPE_CODE", Order = 33)]
        [StringLength(200)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        [Column("CUSTOMER_TYPE_CODE_DETAIL", Order = 34)]
        [StringLength(200)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        [Column("TRCTCD", Order = 35)]
        [StringLength(200)]
        public string? TRCTCD { get; set; }

        [Column("TRCTNM", Order = 36)]
        [StringLength(200)]
        public string? TRCTNM { get; set; }

        [Column("ADDR1", Order = 37)]
        [StringLength(200)]
        public string? ADDR1 { get; set; }

        [Column("PROVINCE", Order = 38)]
        [StringLength(200)]
        public string? PROVINCE { get; set; }

        [Column("LCLPROVINNM", Order = 39)]
        [StringLength(200)]
        public string? LCLPROVINNM { get; set; }

        [Column("DISTRICT", Order = 40)]
        [StringLength(200)]
        public string? DISTRICT { get; set; }

        [Column("LCLDISTNM", Order = 41)]
        [StringLength(200)]
        public string? LCLDISTNM { get; set; }

        [Column("COMMCD", Order = 42)]
        [StringLength(200)]
        public string? COMMCD { get; set; }

        [Column("LCLWARDNM", Order = 43)]
        [StringLength(200)]
        public string? LCLWARDNM { get; set; }

        [Column("LAST_REPAY_DATE", Order = 44)]
        public DateTime? LAST_REPAY_DATE { get; set; }

        [Column("SECURED_PERCENT", Order = 45)]
        [StringLength(200)]
        public string? SECURED_PERCENT { get; set; }

        [Column("NHOM_NO", Order = 46)]
        [StringLength(200)]
        public string? NHOM_NO { get; set; }

        [Column("LAST_INT_CHARGE_DATE", Order = 47)]
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        [Column("EXEMPTINT", Order = 48)]
        [StringLength(200)]
        public string? EXEMPTINT { get; set; }

        [Column("EXEMPTINTTYPE", Order = 49)]
        [StringLength(200)]
        public string? EXEMPTINTTYPE { get; set; }

        [Column("EXEMPTINTAMT", Order = 50, TypeName = "decimal(18,2)")]
        public decimal? EXEMPTINTAMT { get; set; }

        [Column("GRPNO", Order = 51)]
        [StringLength(200)]
        public string? GRPNO { get; set; }

        [Column("BUSCD", Order = 52)]
        [StringLength(200)]
        public string? BUSCD { get; set; }

        [Column("BSNSSCLTPCD", Order = 53)]
        [StringLength(200)]
        public string? BSNSSCLTPCD { get; set; }

        [Column("USRIDOP", Order = 54)]
        [StringLength(200)]
        public string? USRIDOP { get; set; }

        [Column("ACCRUAL_AMOUNT", Order = 55, TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        [Column("ACCRUAL_AMOUNT_END_OF_MONTH", Order = 56, TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        [Column("INTCMTH", Order = 57)]
        [StringLength(200)]
        public string? INTCMTH { get; set; }

        [Column("INTRPYMTH", Order = 58)]
        [StringLength(200)]
        public string? INTRPYMTH { get; set; }

        [Column("INTTRMMTH", Order = 59)]
        [StringLength(200)]
        public string? INTTRMMTH { get; set; }

        [Column("YRDAYS", Order = 60)]
        [StringLength(200)]
        public string? YRDAYS { get; set; }

        [Column("REMARK", Order = 61)]
        [StringLength(1000)]
        public string? REMARK { get; set; }

        [Column("CHITIEU", Order = 62)]
        [StringLength(200)]
        public string? CHITIEU { get; set; }

        [Column("CTCV", Order = 63)]
        [StringLength(200)]
        public string? CTCV { get; set; }

        [Column("CREDIT_LINE_YPE", Order = 64)]
        [StringLength(200)]
        public string? CREDIT_LINE_YPE { get; set; }

        [Column("INT_LUMPSUM_PARTIAL_TYPE", Order = 65)]
        [StringLength(200)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        [Column("INT_PARTIAL_PAYMENT_TYPE", Order = 66)]
        [StringLength(200)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        [Column("INT_PAYMENT_INTERVAL", Order = 67)]
        [StringLength(200)]
        public string? INT_PAYMENT_INTERVAL { get; set; }

        [Column("AN_HAN_LAI", Order = 68)]
        [StringLength(200)]
        public string? AN_HAN_LAI { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_1", Order = 69)]
        [StringLength(200)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_1", Order = 70)]
        [StringLength(200)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_1", Order = 71, TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_2", Order = 72)]
        [StringLength(200)]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_2", Order = 73)]
        [StringLength(200)]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_2", Order = 74, TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }

        [Column("CMT_HC", Order = 75)]
        [StringLength(200)]
        public string? CMT_HC { get; set; }

        [Column("NGAY_SINH", Order = 76)]
        public DateTime? NGAY_SINH { get; set; }

        [Column("MA_CB_AGRI", Order = 77)]
        [StringLength(200)]
        public string? MA_CB_AGRI { get; set; }

        [Column("MA_NGANH_KT", Order = 78)]
        [StringLength(200)]
        public string? MA_NGANH_KT { get; set; }

        [Column("TY_GIA", Order = 79, TypeName = "decimal(18,2)")]
        public decimal? TY_GIA { get; set; }

        [Column("OFFICER_IPCAS", Order = 80)]
        [StringLength(200)]
        public string? OFFICER_IPCAS { get; set; }

        // === SYSTEM COLUMNS - POSITIONS 81+ ===
        /// <summary>
        /// Khóa chính - ID duy nhất cho mỗi bản ghi
        /// </summary>
        [Key]
        [Column("Id", Order = 81)]
        public long Id { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Column("CREATED_DATE", Order = 83)]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        [Column("UPDATED_DATE", Order = 84)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;

        // Note: SysStartTime and SysEndTime are shadow properties managed by EF Core temporal tables
        // They are automatically at positions 85-86 without explicit Order attributes
    }
}
