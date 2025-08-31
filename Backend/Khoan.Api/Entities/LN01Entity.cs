using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Entities
{
    [Table("LN01")]
    public class LN01Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Core Business Columns (79 columns from CSV)
        [Column("BRCD")]
        [MaxLength(10)]
        public string? BRCD { get; set; }

        [Column("CUSTSEQ")]
        [MaxLength(20)]
        public string? CUSTSEQ { get; set; }

        [Column("CUSTNM")]
        [MaxLength(200)]
        public string? CUSTNM { get; set; }

        [Column("TAI_KHOAN")]
        [MaxLength(50)]
        public string? TAI_KHOAN { get; set; }

        [Column("CCY")]
        [MaxLength(10)]
        public string? CCY { get; set; }

        [Column("DU_NO")]
        [MaxLength(30)]
        public string? DU_NO { get; set; }

        [Column("DSBSSEQ")]
        [MaxLength(50)]
        public string? DSBSSEQ { get; set; }

        [Column("TRANSACTION_DATE")]
        [MaxLength(20)]
        public string? TRANSACTION_DATE { get; set; }

        [Column("DSBSDT")]
        [MaxLength(20)]
        public string? DSBSDT { get; set; }

        [Column("DISBUR_CCY")]
        [MaxLength(10)]
        public string? DISBUR_CCY { get; set; }

        [Column("DISBURSEMENT_AMOUNT")]
        [MaxLength(30)]
        public string? DISBURSEMENT_AMOUNT { get; set; }

        [Column("DSBSMATDT")]
        [MaxLength(20)]
        public string? DSBSMATDT { get; set; }

        [Column("BSRTCD")]
        [MaxLength(10)]
        public string? BSRTCD { get; set; }

        [Column("INTEREST_RATE")]
        [MaxLength(30)]
        public string? INTEREST_RATE { get; set; }

        [Column("APRSEQ")]
        [MaxLength(50)]
        public string? APRSEQ { get; set; }

        [Column("APPRDT")]
        [MaxLength(20)]
        public string? APPRDT { get; set; }

        [Column("APPR_CCY")]
        [MaxLength(10)]
        public string? APPR_CCY { get; set; }

        [Column("APPRAMT")]
        [MaxLength(30)]
        public string? APPRAMT { get; set; }

        [Column("APPRMATDT")]
        [MaxLength(20)]
        public string? APPRMATDT { get; set; }

        [Column("LOAN_TYPE")]
        [MaxLength(200)]
        public string? LOAN_TYPE { get; set; }

        [Column("FUND_RESOURCE_CODE")]
        [MaxLength(200)]
        public string? FUND_RESOURCE_CODE { get; set; }

        [Column("FUND_PURPOSE_CODE")]
        [MaxLength(200)]
        public string? FUND_PURPOSE_CODE { get; set; }

        [Column("REPAYMENT_AMOUNT")]
        [MaxLength(30)]
        public string? REPAYMENT_AMOUNT { get; set; }

        [Column("NEXT_REPAY_DATE")]
        [MaxLength(20)]
        public string? NEXT_REPAY_DATE { get; set; }

        [Column("NEXT_REPAY_AMOUNT")]
        [MaxLength(30)]
        public string? NEXT_REPAY_AMOUNT { get; set; }

        [Column("NEXT_INT_REPAY_DATE")]
        [MaxLength(20)]
        public string? NEXT_INT_REPAY_DATE { get; set; }

        [Column("OFFICER_ID")]
        [MaxLength(20)]
        public string? OFFICER_ID { get; set; }

        [Column("OFFICER_NAME")]
        [MaxLength(200)]
        public string? OFFICER_NAME { get; set; }

        [Column("INTEREST_AMOUNT")]
        [MaxLength(30)]
        public string? INTEREST_AMOUNT { get; set; }

        [Column("PASTDUE_INTEREST_AMOUNT")]
        [MaxLength(30)]
        public string? PASTDUE_INTEREST_AMOUNT { get; set; }

        [Column("TOTAL_INTEREST_REPAY_AMOUNT")]
        [MaxLength(30)]
        public string? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [Column("CUSTOMER_TYPE_CODE")]
        [MaxLength(10)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        [Column("CUSTOMER_TYPE_CODE_DETAIL")]
        [MaxLength(10)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        [Column("TRCTCD")]
        [MaxLength(50)]
        public string? TRCTCD { get; set; }

        [Column("TRCTNM")]
        [MaxLength(200)]
        public string? TRCTNM { get; set; }

        [Column("ADDR1")]
        [MaxLength(200)]
        public string? ADDR1 { get; set; }

        [Column("PROVINCE")]
        [MaxLength(10)]
        public string? PROVINCE { get; set; }

        [Column("LCLPROVINNM")]
        [MaxLength(200)]
        public string? LCLPROVINNM { get; set; }

        [Column("DISTRICT")]
        [MaxLength(10)]
        public string? DISTRICT { get; set; }

        [Column("LCLDISTNM")]
        [MaxLength(200)]
        public string? LCLDISTNM { get; set; }

        [Column("COMMCD")]
        [MaxLength(10)]
        public string? COMMCD { get; set; }

        [Column("LCLWARDNM")]
        [MaxLength(200)]
        public string? LCLWARDNM { get; set; }

        [Column("LAST_REPAY_DATE")]
        [MaxLength(20)]
        public string? LAST_REPAY_DATE { get; set; }

        [Column("SECURED_PERCENT")]
        [MaxLength(10)]
        public string? SECURED_PERCENT { get; set; }

        [Column("NHOM_NO")]
        [MaxLength(10)]
        public string? NHOM_NO { get; set; }

        [Column("LAST_INT_CHARGE_DATE")]
        [MaxLength(20)]
        public string? LAST_INT_CHARGE_DATE { get; set; }

        [Column("EXEMPTINT")]
        [MaxLength(10)]
        public string? EXEMPTINT { get; set; }

        [Column("EXEMPTINTTYPE")]
        [MaxLength(10)]
        public string? EXEMPTINTTYPE { get; set; }

        [Column("EXEMPTINTAMT")]
        [MaxLength(30)]
        public string? EXEMPTINTAMT { get; set; }

        [Column("GRPNO")]
        [MaxLength(10)]
        public string? GRPNO { get; set; }

        [Column("BUSCD")]
        [MaxLength(10)]
        public string? BUSCD { get; set; }

        [Column("BSNSSCLTPCD")]
        [MaxLength(10)]
        public string? BSNSSCLTPCD { get; set; }

        [Column("USRIDOP")]
        [MaxLength(20)]
        public string? USRIDOP { get; set; }

        [Column("ACCRUAL_AMOUNT")]
        [MaxLength(30)]
        public string? ACCRUAL_AMOUNT { get; set; }

        [Column("ACCRUAL_AMOUNT_END_OF_MONTH")]
        [MaxLength(30)]
        public string? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        [Column("INTCMTH")]
        [MaxLength(10)]
        public string? INTCMTH { get; set; }

        [Column("INTRPYMTH")]
        [MaxLength(10)]
        public string? INTRPYMTH { get; set; }

        [Column("INTTRMMTH")]
        [MaxLength(10)]
        public string? INTTRMMTH { get; set; }

        [Column("YRDAYS")]
        [MaxLength(10)]
        public string? YRDAYS { get; set; }

        [Column("REMARK")]
        [MaxLength(500)]
        public string? REMARK { get; set; }

        [Column("CHITIEU")]
        [MaxLength(10)]
        public string? CHITIEU { get; set; }

        [Column("CTCV")]
        [MaxLength(200)]
        public string? CTCV { get; set; }

        [Column("CREDIT_LINE_TYPE")]
        [MaxLength(200)]
        public string? CREDIT_LINE_TYPE { get; set; }

        [Column("INT_LUMPSUM_PARTIAL_TYPE")]
        [MaxLength(10)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        [Column("INT_PARTIAL_PAYMENT_TYPE")]
        [MaxLength(10)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        [Column("INT_PAYMENT_INTERVAL")]
        [MaxLength(10)]
        public string? INT_PAYMENT_INTERVAL { get; set; }

        [Column("AN_HAN_LAI")]
        [MaxLength(10)]
        public string? AN_HAN_LAI { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_1")]
        [MaxLength(50)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_1")]
        [MaxLength(50)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_1")]
        [MaxLength(30)]
        public string? SO_TIEN_GIAI_NGAN_1 { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_2")]
        [MaxLength(50)]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_2")]
        [MaxLength(50)]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_2")]
        [MaxLength(30)]
        public string? SO_TIEN_GIAI_NGAN_2 { get; set; }

        [Column("CMT_HC")]
        [MaxLength(50)]
        public string? CMT_HC { get; set; }

        [Column("NGAY_SINH")]
        [MaxLength(20)]
        public string? NGAY_SINH { get; set; }

        [Column("MA_CB_AGRI")]
        [MaxLength(20)]
        public string? MA_CB_AGRI { get; set; }

        [Column("MA_NGANH_KT")]
        [MaxLength(20)]
        public string? MA_NGANH_KT { get; set; }

        [Column("TY_GIA")]
        [MaxLength(30)]
        public string? TY_GIA { get; set; }

        [Column("OFFICER_IPCAS")]
        [MaxLength(20)]
        public string? OFFICER_IPCAS { get; set; }

        // System/Control Columns (4 columns)
        [Column("NGAY_DL", TypeName = "datetime2")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Column("CREATED_DATE", TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }

        [Column("IS_DELETED")]
        public bool IsDeleted { get; set; } = false;

        // Temporal Table System Columns are configured as shadow properties
        // SysStartTime and SysEndTime are not declared here - they're shadow properties
    }
}
