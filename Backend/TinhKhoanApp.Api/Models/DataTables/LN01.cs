using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// LN01 - Loan Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("LN01")]
    public class LN01
    {
        // System Column - NGAY_DL first (extracted from filename)
        [Column("NGAY_DL", Order = 0)]

        public DateTime NGAY_DL { get; set; }

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("BRCD", Order = 1)]
        [StringLength(200)]
        public string BRCD { get; set; } = "";

        [Column("CUSTSEQ", Order = 2)]
        [StringLength(200)]
        public string CUSTSEQ { get; set; } = "";

        [Column("CUSTNM", Order = 3)]
        [StringLength(200)]
        public string CUSTNM { get; set; } = "";

        [Column("TAI_KHOAN", Order = 4)]
        [StringLength(200)]
        public string TAI_KHOAN { get; set; } = "";

        [Column("CCY", Order = 5)]
        [StringLength(200)]
        public string CCY { get; set; } = "";

        [Column("DU_NO", Order = 6)]
        [StringLength(200)]
        public string DU_NO { get; set; } = "";

        [Column("DSBSSEQ", Order = 7)]
        [StringLength(200)]
        public string DSBSSEQ { get; set; } = "";

        [Column("TRANSACTION_DATE", Order = 8)]
        [StringLength(200)]
        public string TRANSACTION_DATE { get; set; } = "";

        [Column("DSBSDT", Order = 9)]
        [StringLength(200)]
        public string DSBSDT { get; set; } = "";

        [Column("DISBUR_CCY", Order = 10)]
        [StringLength(200)]
        public string DISBUR_CCY { get; set; } = "";

        [Column("DISBURSEMENT_AMOUNT", Order = 11)]
        [StringLength(200)]
        public string DISBURSEMENT_AMOUNT { get; set; } = "";

        [Column("DSBSMATDT", Order = 12)]
        [StringLength(200)]
        public string DSBSMATDT { get; set; } = "";

        [Column("BSRTCD", Order = 13)]
        [StringLength(200)]
        public string BSRTCD { get; set; } = "";

        [Column("INTEREST_RATE", Order = 14)]
        [StringLength(200)]
        public string INTEREST_RATE { get; set; } = "";

        [Column("APPRSEQ", Order = 15)]
        [StringLength(200)]
        public string APPRSEQ { get; set; } = "";

        [Column("APPRDT", Order = 16)]
        [StringLength(200)]
        public string APPRDT { get; set; } = "";

        [Column("APPR_CCY", Order = 17)]
        [StringLength(200)]
        public string APPR_CCY { get; set; } = "";

        [Column("APPRAMT", Order = 18)]
        [StringLength(200)]
        public string APPRAMT { get; set; } = "";

        [Column("APPRMATDT", Order = 19)]
        [StringLength(200)]
        public string APPRMATDT { get; set; } = "";

        [Column("LOAN_TYPE", Order = 20)]
        [StringLength(200)]
        public string LOAN_TYPE { get; set; } = "";

        [Column("FUND_RESOURCE_CODE", Order = 21)]
        [StringLength(200)]
        public string FUND_RESOURCE_CODE { get; set; } = "";

        [Column("FUND_PURPOSE_CODE", Order = 22)]
        [StringLength(200)]
        public string FUND_PURPOSE_CODE { get; set; } = "";

        [Column("REPAYMENT_AMOUNT", Order = 23)]
        [StringLength(200)]
        public string REPAYMENT_AMOUNT { get; set; } = "";

        [Column("NEXT_REPAY_DATE", Order = 24)]
        [StringLength(200)]
        public string NEXT_REPAY_DATE { get; set; } = "";

        [Column("NEXT_REPAY_AMOUNT", Order = 25)]
        [StringLength(200)]
        public string NEXT_REPAY_AMOUNT { get; set; } = "";

        [Column("NEXT_INT_REPAY_DATE", Order = 26)]
        [StringLength(200)]
        public string NEXT_INT_REPAY_DATE { get; set; } = "";

        [Column("OFFICER_ID", Order = 27)]
        [StringLength(200)]
        public string OFFICER_ID { get; set; } = "";

        [Column("OFFICER_NAME", Order = 28)]
        [StringLength(200)]
        public string OFFICER_NAME { get; set; } = "";

        [Column("INTEREST_AMOUNT", Order = 29)]
        [StringLength(200)]
        public string INTEREST_AMOUNT { get; set; } = "";

        [Column("PASTDUE_INTEREST_AMOUNT", Order = 30)]
        [StringLength(200)]
        public string PASTDUE_INTEREST_AMOUNT { get; set; } = "";

        [Column("TOTAL_INTEREST_REPAY_AMOUNT", Order = 31)]
        [StringLength(200)]
        public string TOTAL_INTEREST_REPAY_AMOUNT { get; set; } = "";

        [Column("CUSTOMER_TYPE_CODE", Order = 32)]
        [StringLength(200)]
        public string CUSTOMER_TYPE_CODE { get; set; } = "";

        [Column("CUSTOMER_TYPE_CODE_DETAIL", Order = 33)]
        [StringLength(200)]
        public string CUSTOMER_TYPE_CODE_DETAIL { get; set; } = "";

        [Column("TRCTCD", Order = 34)]
        [StringLength(200)]
        public string TRCTCD { get; set; } = "";

        [Column("TRCTNM", Order = 35)]
        [StringLength(200)]
        public string TRCTNM { get; set; } = "";

        [Column("ADDR1", Order = 36)]
        [StringLength(200)]
        public string ADDR1 { get; set; } = "";

        [Column("PROVINCE", Order = 37)]
        [StringLength(200)]
        public string PROVINCE { get; set; } = "";

        [Column("LCLPROVINNM", Order = 38)]
        [StringLength(200)]
        public string LCLPROVINNM { get; set; } = "";

        [Column("DISTRICT", Order = 39)]
        [StringLength(200)]
        public string DISTRICT { get; set; } = "";

        [Column("LCLDISTNM", Order = 40)]
        [StringLength(200)]
        public string LCLDISTNM { get; set; } = "";

        [Column("COMMCD", Order = 41)]
        [StringLength(200)]
        public string COMMCD { get; set; } = "";

        [Column("LCLWARDNM", Order = 42)]
        [StringLength(200)]
        public string LCLWARDNM { get; set; } = "";

        [Column("LAST_REPAY_DATE", Order = 43)]
        [StringLength(200)]
        public string LAST_REPAY_DATE { get; set; } = "";

        [Column("SECURED_PERCENT", Order = 44)]
        [StringLength(200)]
        public string SECURED_PERCENT { get; set; } = "";

        [Column("NHOM_NO", Order = 45)]
        [StringLength(200)]
        public string NHOM_NO { get; set; } = "";

        [Column("LAST_INT_CHARGE_DATE", Order = 46)]
        [StringLength(200)]
        public string LAST_INT_CHARGE_DATE { get; set; } = "";

        [Column("EXEMPTINT", Order = 47)]
        [StringLength(200)]
        public string EXEMPTINT { get; set; } = "";

        [Column("EXEMPTINTTYPE", Order = 48)]
        [StringLength(200)]
        public string EXEMPTINTTYPE { get; set; } = "";

        [Column("EXEMPTINTAMT", Order = 49)]
        [StringLength(200)]
        public string EXEMPTINTAMT { get; set; } = "";

        [Column("GRPNO", Order = 50)]
        [StringLength(200)]
        public string GRPNO { get; set; } = "";

        [Column("BUSCD", Order = 51)]
        [StringLength(200)]
        public string BUSCD { get; set; } = "";

        [Column("BSNSSCLTPCD", Order = 52)]
        [StringLength(200)]
        public string BSNSSCLTPCD { get; set; } = "";

        [Column("USRIDOP", Order = 53)]
        [StringLength(200)]
        public string USRIDOP { get; set; } = "";

        [Column("ACCRUAL_AMOUNT", Order = 54)]
        [StringLength(200)]
        public string ACCRUAL_AMOUNT { get; set; } = "";

        [Column("ACCRUAL_AMOUNT_END_OF_MONTH", Order = 55)]
        [StringLength(200)]
        public string ACCRUAL_AMOUNT_END_OF_MONTH { get; set; } = "";

        [Column("INTCMTH", Order = 56)]
        [StringLength(200)]
        public string INTCMTH { get; set; } = "";

        [Column("INTRPYMTH", Order = 57)]
        [StringLength(200)]
        public string INTRPYMTH { get; set; } = "";

        [Column("INTTRMMTH", Order = 58)]
        [StringLength(200)]
        public string INTTRMMTH { get; set; } = "";

        [Column("YRDAYS", Order = 59)]
        [StringLength(200)]
        public string YRDAYS { get; set; } = "";

        [Column("REMARK", Order = 60)]
        [StringLength(200)]
        public string REMARK { get; set; } = "";

        [Column("CHITIEU", Order = 61)]
        [StringLength(200)]
        public string CHITIEU { get; set; } = "";

        [Column("CTCV", Order = 62)]
        [StringLength(200)]
        public string CTCV { get; set; } = "";

        [Column("CREDIT_LINE_YPE", Order = 63)]
        [StringLength(200)]
        public string CREDIT_LINE_YPE { get; set; } = "";

        [Column("INT_LUMPSUM_PARTIAL_TYPE", Order = 64)]
        [StringLength(200)]
        public string INT_LUMPSUM_PARTIAL_TYPE { get; set; } = "";

        [Column("INT_PARTIAL_PAYMENT_TYPE", Order = 65)]
        [StringLength(200)]
        public string INT_PARTIAL_PAYMENT_TYPE { get; set; } = "";

        [Column("INT_PAYMENT_INTERVAL", Order = 66)]
        [StringLength(200)]
        public string INT_PAYMENT_INTERVAL { get; set; } = "";

        [Column("AN_HAN_LAI", Order = 67)]
        [StringLength(200)]
        public string AN_HAN_LAI { get; set; } = "";

        [Column("PHUONG_THUC_GIAI_NGAN_1", Order = 68)]
        [StringLength(200)]
        public string PHUONG_THUC_GIAI_NGAN_1 { get; set; } = "";

        [Column("TAI_KHOAN_GIAI_NGAN_1", Order = 69)]
        [StringLength(200)]
        public string TAI_KHOAN_GIAI_NGAN_1 { get; set; } = "";

        [Column("SO_TIEN_GIAI_NGAN_1", Order = 70)]
        [StringLength(200)]
        public string SO_TIEN_GIAI_NGAN_1 { get; set; } = "";

        [Column("PHUONG_THUC_GIAI_NGAN_2", Order = 71)]
        [StringLength(200)]
        public string PHUONG_THUC_GIAI_NGAN_2 { get; set; } = "";

        [Column("TAI_KHOAN_GIAI_NGAN_2", Order = 72)]
        [StringLength(200)]
        public string TAI_KHOAN_GIAI_NGAN_2 { get; set; } = "";

        [Column("SO_TIEN_GIAI_NGAN_2", Order = 73)]
        [StringLength(200)]
        public string SO_TIEN_GIAI_NGAN_2 { get; set; } = "";

        [Column("CMT_HC", Order = 74)]
        [StringLength(200)]
        public string CMT_HC { get; set; } = "";

        [Column("NGAY_SINH", Order = 75)]
        [StringLength(200)]
        public string NGAY_SINH { get; set; } = "";

        [Column("MA_CB_AGRI", Order = 76)]
        [StringLength(200)]
        public string MA_CB_AGRI { get; set; } = "";

        [Column("MA_NGANH_KT", Order = 77)]
        [StringLength(200)]
        public string MA_NGANH_KT { get; set; } = "";

        [Column("TY_GIA", Order = 78)]
        [StringLength(200)]
        public string TY_GIA { get; set; } = "";

        [Column("OFFICER_IPCAS", Order = 79)]
        [StringLength(200)]
        public string OFFICER_IPCAS { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 80)]
        public long Id { get; set; }

        // Temporal columns are shadow properties managed by EF Core automatically
        // ValidFrom/ValidTo removed - managed as shadow properties by ApplicationDbContext

        [Column("CREATED_DATE", Order = 81)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 82)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 85)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";
    }
}
