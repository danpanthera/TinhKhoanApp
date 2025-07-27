using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    [Table("LN01")]
    public class LN01
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // NGAY_DL Column - extracted from filename
        [Column(Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // Business Columns from CSV (79 columns)
        [Column(Order = 1)]
        [StringLength(200)]
        public string? BRCD { get; set; }

        [Column(Order = 2)]
        [StringLength(200)]
        public string? CUSTSEQ { get; set; }

        [Column(Order = 3)]
        [StringLength(200)]
        public string? CUSTNM { get; set; }

        [Column(Order = 4)]
        [StringLength(200)]
        public string? TAI_KHOAN { get; set; }

        [Column(Order = 5)]
        [StringLength(200)]
        public string? CCY { get; set; }

        [Column(Order = 6, TypeName = "decimal(18,2)")]
        public decimal? DU_NO { get; set; }

        [Column(Order = 7)]
        [StringLength(200)]
        public string? DSBSSEQ { get; set; }

        [Column(Order = 8)]
        public DateTime? TRANSACTION_DATE { get; set; }

        [Column(Order = 9)]
        public DateTime? DSBSDT { get; set; }

        [Column(Order = 10)]
        [StringLength(200)]
        public string? DISBUR_CCY { get; set; }

        [Column(Order = 11, TypeName = "decimal(18,2)")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        [Column(Order = 12)]
        public DateTime? DSBSMATDT { get; set; }

        [Column(Order = 13)]
        [StringLength(200)]
        public string? BSRTCD { get; set; }

        [Column(Order = 14, TypeName = "decimal(18,2)")]
        public decimal? INTEREST_RATE { get; set; }

        [Column(Order = 15)]
        [StringLength(200)]
        public string? APPRSEQ { get; set; }

        [Column(Order = 16)]
        public DateTime? APPRDT { get; set; }

        [Column(Order = 17)]
        [StringLength(200)]
        public string? APPR_CCY { get; set; }

        [Column(Order = 18, TypeName = "decimal(18,2)")]
        public decimal? APPRAMT { get; set; }

        [Column(Order = 19)]
        public DateTime? APPRMATDT { get; set; }

        [Column(Order = 20)]
        [StringLength(200)]
        public string? LOAN_TYPE { get; set; }

        [Column(Order = 21)]
        [StringLength(200)]
        public string? FUND_RESOURCE_CODE { get; set; }

        [Column(Order = 22)]
        [StringLength(200)]
        public string? FUND_PURPOSE_CODE { get; set; }

        [Column(Order = 23, TypeName = "decimal(18,2)")]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        [Column(Order = 24)]
        public DateTime? NEXT_REPAY_DATE { get; set; }

        [Column(Order = 25, TypeName = "decimal(18,2)")]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        [Column(Order = 26)]
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        [Column(Order = 27)]
        [StringLength(200)]
        public string? OFFICER_ID { get; set; }

        [Column(Order = 28)]
        [StringLength(200)]
        public string? OFFICER_NAME { get; set; }

        [Column(Order = 29, TypeName = "decimal(18,2)")]
        public decimal? INTEREST_AMOUNT { get; set; }

        [Column(Order = 30, TypeName = "decimal(18,2)")]
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }

        [Column(Order = 31, TypeName = "decimal(18,2)")]
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [Column(Order = 32)]
        [StringLength(200)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        [Column(Order = 33)]
        [StringLength(200)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        [Column(Order = 34)]
        [StringLength(200)]
        public string? TRCTCD { get; set; }

        [Column(Order = 35)]
        [StringLength(200)]
        public string? TRCTNM { get; set; }

        [Column(Order = 36)]
        [StringLength(1000)]
        public string? ADDR1 { get; set; }

        [Column(Order = 37)]
        [StringLength(200)]
        public string? PROVINCE { get; set; }

        [Column(Order = 38)]
        [StringLength(200)]
        public string? LCLPROVINNM { get; set; }

        [Column(Order = 39)]
        [StringLength(200)]
        public string? DISTRICT { get; set; }

        [Column(Order = 40)]
        [StringLength(200)]
        public string? LCLDISTNM { get; set; }

        [Column(Order = 41)]
        [StringLength(200)]
        public string? COMMCD { get; set; }

        [Column(Order = 42)]
        [StringLength(200)]
        public string? LCLWARDNM { get; set; }

        [Column(Order = 43)]
        public DateTime? LAST_REPAY_DATE { get; set; }

        [Column(Order = 44, TypeName = "decimal(18,2)")]
        public decimal? SECURED_PERCENT { get; set; }

        [Column(Order = 45)]
        [StringLength(200)]
        public string? NHOM_NO { get; set; }

        [Column(Order = 46)]
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        [Column(Order = 47)]
        [StringLength(200)]
        public string? EXEMPTINT { get; set; }

        [Column(Order = 48)]
        [StringLength(200)]
        public string? EXEMPTINTTYPE { get; set; }

        [Column(Order = 49, TypeName = "decimal(18,2)")]
        public decimal? EXEMPTINTAMT { get; set; }

        [Column(Order = 50)]
        [StringLength(200)]
        public string? GRPNO { get; set; }

        [Column(Order = 51)]
        [StringLength(200)]
        public string? BUSCD { get; set; }

        [Column(Order = 52)]
        [StringLength(200)]
        public string? BSNSSCLTPCD { get; set; }

        [Column(Order = 53)]
        [StringLength(200)]
        public string? USRIDOP { get; set; }

        [Column(Order = 54, TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        [Column(Order = 55, TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        [Column(Order = 56)]
        [StringLength(200)]
        public string? INTCMTH { get; set; }

        [Column(Order = 57)]
        [StringLength(200)]
        public string? INTRPYMTH { get; set; }

        [Column(Order = 58)]
        [StringLength(200)]
        public string? INTTRMMTH { get; set; }

        [Column(Order = 59)]
        [StringLength(200)]
        public string? YRDAYS { get; set; }

        [Column(Order = 60)]
        [StringLength(1000)]
        public string? REMARK { get; set; }

        [Column(Order = 61)]
        [StringLength(200)]
        public string? CHITIEU { get; set; }

        [Column(Order = 62)]
        [StringLength(200)]
        public string? CTCV { get; set; }

        [Column(Order = 63)]
        [StringLength(200)]
        public string? CREDIT_LINE_YPE { get; set; }

        [Column(Order = 64)]
        [StringLength(200)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        [Column(Order = 65)]
        [StringLength(200)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        [Column(Order = 66)]
        [StringLength(200)]
        public string? INT_PAYMENT_INTERVAL { get; set; }

        [Column(Order = 67, TypeName = "decimal(18,2)")]
        public decimal? AN_HAN_LAI { get; set; }

        [Column(Order = 68)]
        [StringLength(200)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        [Column(Order = 69)]
        [StringLength(200)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        [Column(Order = 70, TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        [Column(Order = 71)]
        [StringLength(200)]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        [Column(Order = 72)]
        [StringLength(200)]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        [Column(Order = 73, TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }

        [Column(Order = 74)]
        [StringLength(200)]
        public string? CMT_HC { get; set; }

        [Column(Order = 75)]
        public DateTime? NGAY_SINH { get; set; }

        [Column(Order = 76)]
        [StringLength(200)]
        public string? MA_CB_AGRI { get; set; }

        [Column(Order = 77)]
        [StringLength(200)]
        public string? MA_NGANH_KT { get; set; }

        [Column(Order = 78, TypeName = "decimal(18,2)")]
        public decimal? TY_GIA { get; set; }

        [Column(Order = 79)]
        [StringLength(200)]
        public string? OFFICER_IPCAS { get; set; }

        // System Columns
        [Column(Order = 80)]
        [StringLength(500)]
        public string? FILE_NAME { get; set; }

        [Column(Order = 81)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column(Order = 82)]
        [StringLength(100)]
        public string? CREATED_BY { get; set; }

        [Column(Order = 83)]
        public DateTime? UPDATED_DATE { get; set; }

        [Column(Order = 84)]
        [StringLength(100)]
        public string? UPDATED_BY { get; set; }

        [Column(Order = 85)]
        public bool IS_ACTIVE { get; set; } = true;

        [Column(Order = 86)]
        [StringLength(500)]
        public string? NOTES { get; set; }
    }
}
