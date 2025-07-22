using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng LN01 - Dữ liệu Tín dụng chi tiết
    /// CSV Business columns FIRST (79 columns), then System/Temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("LN01")]
    public class LN01
    {
        // ======= CSV BUSINESS COLUMNS (1-79) - EXACT ORDER FROM CSV =======
        [Column("BRCD")]
        [StringLength(50)]
        public string? BRCD { get; set; }

        [Column("CUSTSEQ")]
        [StringLength(100)]
        public string? CUSTSEQ { get; set; }

        [Column("CUSTNM")]
        [StringLength(500)]
        public string? CUSTNM { get; set; }

        [Column("TAI_KHOAN")]
        [StringLength(100)]
        public string? TAI_KHOAN { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? CCY { get; set; }

        [Column("DU_NO", TypeName = "decimal(18,2)")]
        public decimal? DU_NO { get; set; }

        [Column("DSBSSEQ")]
        [StringLength(100)]
        public string? DSBSSEQ { get; set; }

        [Column("TRANSACTION_DATE")]
        public DateTime? TRANSACTION_DATE { get; set; }

        [Column("DSBSDT")]
        public DateTime? DSBSDT { get; set; }

        [Column("DISBUR_CCY")]
        [StringLength(10)]
        public string? DISBUR_CCY { get; set; }

        [Column("DISBURSEMENT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        [Column("DSBSMATDT")]
        public DateTime? DSBSMATDT { get; set; }

        [Column("BSRTCD")]
        [StringLength(50)]
        public string? BSRTCD { get; set; }

        [Column("INTEREST_RATE", TypeName = "decimal(10,4)")]
        public decimal? INTEREST_RATE { get; set; }

        [Column("APPRSEQ")]
        [StringLength(100)]
        public string? APPRSEQ { get; set; }

        [Column("APPRDT")]
        public DateTime? APPRDT { get; set; }

        [Column("APPR_CCY")]
        [StringLength(10)]
        public string? APPR_CCY { get; set; }

        [Column("APPRAMT", TypeName = "decimal(18,2)")]
        public decimal? APPRAMT { get; set; }

        [Column("APPRMATDT")]
        public DateTime? APPRMATDT { get; set; }

        [Column("LOAN_TYPE")]
        [StringLength(100)]
        public string? LOAN_TYPE { get; set; }

        [Column("FUND_RESOURCE_CODE")]
        [StringLength(50)]
        public string? FUND_RESOURCE_CODE { get; set; }

        [Column("FUND_PURPOSE_CODE")]
        [StringLength(50)]
        public string? FUND_PURPOSE_CODE { get; set; }

        [Column("REPAYMENT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        [Column("NEXT_REPAY_DATE")]
        public DateTime? NEXT_REPAY_DATE { get; set; }

        [Column("NEXT_REPAY_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        [Column("NEXT_INT_REPAY_DATE")]
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        [Column("OFFICER_ID")]
        [StringLength(50)]
        public string? OFFICER_ID { get; set; }

        [Column("OFFICER_NAME")]
        [StringLength(255)]
        public string? OFFICER_NAME { get; set; }

        [Column("INTEREST_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? INTEREST_AMOUNT { get; set; }

        [Column("PASTDUE_INTEREST_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }

        [Column("TOTAL_INTEREST_REPAY_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [Column("CUSTOMER_TYPE_CODE")]
        [StringLength(50)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        [Column("CUSTOMER_TYPE_CODE_DETAIL")]
        [StringLength(100)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        [Column("TRCTCD")]
        [StringLength(50)]
        public string? TRCTCD { get; set; }

        [Column("TRCTNM")]
        [StringLength(255)]
        public string? TRCTNM { get; set; }

        [Column("ADDR1")]
        [StringLength(500)]
        public string? ADDR1 { get; set; }

        [Column("PROVINCE")]
        [StringLength(100)]
        public string? PROVINCE { get; set; }

        [Column("LCLPROVINNM")]
        [StringLength(100)]
        public string? LCLPROVINNM { get; set; }

        [Column("DISTRICT")]
        [StringLength(100)]
        public string? DISTRICT { get; set; }

        [Column("LCLDISTNM")]
        [StringLength(100)]
        public string? LCLDISTNM { get; set; }

        [Column("COMMCD")]
        [StringLength(50)]
        public string? COMMCD { get; set; }

        [Column("LCLWARDNM")]
        [StringLength(100)]
        public string? LCLWARDNM { get; set; }

        [Column("LAST_REPAY_DATE")]
        public DateTime? LAST_REPAY_DATE { get; set; }

        [Column("SECURED_PERCENT", TypeName = "decimal(10,4)")]
        public decimal? SECURED_PERCENT { get; set; }

        [Column("NHOM_NO")]
        public int? NHOM_NO { get; set; }

        [Column("LAST_INT_CHARGE_DATE")]
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        [Column("EXEMPTINT")]
        [StringLength(10)]
        public string? EXEMPTINT { get; set; }

        [Column("EXEMPTINTTYPE")]
        [StringLength(50)]
        public string? EXEMPTINTTYPE { get; set; }

        [Column("EXEMPTINTAMT", TypeName = "decimal(18,2)")]
        public decimal? EXEMPTINTAMT { get; set; }

        [Column("GRPNO")]
        public int? GRPNO { get; set; }

        [Column("BUSCD")]
        [StringLength(50)]
        public string? BUSCD { get; set; }

        [Column("BSNSSCLTPCD")]
        [StringLength(50)]
        public string? BSNSSCLTPCD { get; set; }

        [Column("USRIDOP")]
        [StringLength(50)]
        public string? USRIDOP { get; set; }

        [Column("ACCRUAL_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        [Column("ACCRUAL_AMOUNT_END_OF_MONTH", TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        [Column("INTCMTH")]
        public int? INTCMTH { get; set; }

        [Column("INTRPYMTH")]
        public int? INTRPYMTH { get; set; }

        [Column("INTTRMMTH")]
        public int? INTTRMMTH { get; set; }

        [Column("YRDAYS")]
        public int? YRDAYS { get; set; }

        [Column("REMARK")]
        [StringLength(500)]
        public string? REMARK { get; set; }

        [Column("CHITIEU")]
        [StringLength(100)]
        public string? CHITIEU { get; set; }

        [Column("CTCV")]
        [StringLength(100)]
        public string? CTCV { get; set; }

        [Column("CREDIT_LINE_YPE")]
        [StringLength(50)]
        public string? CREDIT_LINE_YPE { get; set; }

        [Column("INT_LUMPSUM_PARTIAL_TYPE")]
        [StringLength(50)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        [Column("INT_PARTIAL_PAYMENT_TYPE")]
        [StringLength(50)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        [Column("INT_PAYMENT_INTERVAL")]
        public int? INT_PAYMENT_INTERVAL { get; set; }

        [Column("AN_HAN_LAI")]
        [StringLength(50)]
        public string? AN_HAN_LAI { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_1")]
        [StringLength(50)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_1")]
        [StringLength(100)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_1", TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_2")]
        [StringLength(50)]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_2")]
        [StringLength(100)]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_2", TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }

        [Column("CMT_HC")]
        [StringLength(50)]
        public string? CMT_HC { get; set; }

        [Column("NGAY_SINH")]
        public DateTime? NGAY_SINH { get; set; }

        [Column("MA_CB_AGRI")]
        [StringLength(50)]
        public string? MA_CB_AGRI { get; set; }

        [Column("MA_NGANH_KT")]
        [StringLength(50)]
        public string? MA_NGANH_KT { get; set; }

        [Column("TY_GIA", TypeName = "decimal(10,4)")]
        public decimal? TY_GIA { get; set; }

        [Column("OFFICER_IPCAS")]
        [StringLength(50)]
        public string? OFFICER_IPCAS { get; set; }

        // ======= SYSTEM/TEMPORAL COLUMNS (80+) =======
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("NGAY_DL")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // ======= TEMPORAL COLUMNS (managed by SQL Server) =======
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime UPDATED_DATE { get; set; }

        // Aliases for compatibility with legacy code
        public DateTime? CreatedAt => CREATED_DATE;
        public DateTime? UpdatedAt => UPDATED_DATE;
    }
}
