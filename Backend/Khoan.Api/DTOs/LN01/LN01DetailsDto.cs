namespace Khoan.Api.Dtos.LN01
{
    /// <summary>
    /// LN01 Details DTO - Complete record information
    /// All 79 business columns + system fields
    /// </summary>
    public class LN01DetailsDto
    {
        public int Id { get; set; }
        public DateTime? NGAY_DL { get; set; }

        // === ALL 79 BUSINESS COLUMNS ===
        public string? BRCD { get; set; }
        public string? CUSTSEQ { get; set; }
        public string? CUSTNM { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? CCY { get; set; }
        public decimal? DU_NO { get; set; }
        public string? DSBSSEQ { get; set; }
        public DateTime? TRANSACTION_DATE { get; set; }
        public DateTime? DSBSDT { get; set; }
        public string? DISBUR_CCY { get; set; }
        public decimal? DISBURSEMENT_AMOUNT { get; set; }
        public DateTime? DSBSMATDT { get; set; }
        public string? BSRTCD { get; set; }
        public decimal? INTEREST_RATE { get; set; }
        public string? APPRSEQ { get; set; }
        public DateTime? APPRDT { get; set; }
        public string? APPR_CCY { get; set; }
        public decimal? APPRAMT { get; set; }
        public DateTime? APPRMATDT { get; set; }
        public string? LOAN_TYPE { get; set; }
        public string? FUND_RESOURCE_CODE { get; set; }
        public string? FUND_PURPOSE_CODE { get; set; }
        public decimal? REPAYMENT_AMOUNT { get; set; }
        public DateTime? NEXT_REPAY_DATE { get; set; }
        public decimal? NEXT_REPAY_AMOUNT { get; set; }
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }
        public string? OFFICER_ID { get; set; }
        public string? OFFICER_NAME { get; set; }
        public decimal? INTEREST_AMOUNT { get; set; }
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }
        public string? CUSTOMER_TYPE_CODE { get; set; }
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }
        public string? TRCTCD { get; set; }
        public string? TRCTNM { get; set; }
        public string? ADDR1 { get; set; }
        public string? PROVINCE { get; set; }
        public string? LCLPROVINNM { get; set; }
        public string? DISTRICT { get; set; }
        public string? LCLDISTNM { get; set; }
        public string? COMMCD { get; set; }
        public string? LCLWARDNM { get; set; }
        public DateTime? LAST_REPAY_DATE { get; set; }
        public decimal? SECURED_PERCENT { get; set; }
        public string? NHOM_NO { get; set; }
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }
        public string? EXEMPTINT { get; set; }
        public string? EXEMPTINTTYPE { get; set; }
        public decimal? EXEMPTINTAMT { get; set; }
        public string? GRPNO { get; set; }
        public string? BUSCD { get; set; }
        public string? BSNSSCLTPCD { get; set; }
        public string? USRIDOP { get; set; }
        public decimal? ACCRUAL_AMOUNT { get; set; }
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }
        public int? INTCMTH { get; set; }
        public int? INTRPYMTH { get; set; }
        public int? INTTRMMTH { get; set; }
        public int? YRDAYS { get; set; }
        public string? REMARK { get; set; }
        public string? CHITIEU { get; set; }
        public string? CTCV { get; set; }
        public string? CREDIT_LINE_YPE { get; set; }
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }
        public int? INT_PAYMENT_INTERVAL { get; set; }
        public string? AN_HAN_LAI { get; set; }
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }
        public string? CMT_HC { get; set; }
        public DateTime? NGAY_SINH { get; set; }
        public string? MA_CB_AGRI { get; set; }
        public string? MA_NGANH_KT { get; set; }
        public decimal? TY_GIA { get; set; }
        public string? OFFICER_IPCAS { get; set; }

        // === SYSTEM FIELDS ===
        public string? DataSource { get; set; }
        public DateTime? ImportDateTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
