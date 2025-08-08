using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.LN01
{
    /// <summary>
    /// LN01 (Loan Detail) DTOs - 79 business columns
    /// CSV Structure: BRCD,CUSTSEQ,CUSTNM,TAI_KHOAN,CCY,DU_NO,DSBSSEQ,TRANSACTION_DATE,DSBSDT,DISBUR_CCY,DISBURSEMENT_AMOUNT,DSBSMATDT,BSRTCD,INTEREST_RATE,APPRSEQ,APPRDT,APPR_CCY,APPRAMT,APPRMATDT,LOAN_TYPE,FUND_RESOURCE_CODE,FUND_PURPOSE_CODE,REPAYMENT_AMOUNT,NEXT_REPAY_DATE,NEXT_REPAY_AMOUNT,NEXT_INT_REPAY_DATE,OFFICER_ID,OFFICER_NAME,INTEREST_AMOUNT,PASTDUE_INTEREST_AMOUNT,TOTAL_INTEREST_REPAY_AMOUNT,CUSTOMER_TYPE_CODE,CUSTOMER_TYPE_CODE_DETAIL,TRCTCD,TRCTNM,ADDR1,PROVINCE,LCLPROVINNM,DISTRICT,LCLDISTNM,COMMCD,LCLWARDNM,LAST_REPAY_DATE,SECURED_PERCENT,NHOM_NO,LAST_INT_CHARGE_DATE,EXEMPTINT,EXEMPTINTTYPE,EXEMPTINTAMT,GRPNO,BUSCD,BSNSSCLTPCD,USRIDOP,ACCRUAL_AMOUNT,ACCRUAL_AMOUNT_END_OF_MONTH,INTCMTH,INTRPYMTH,INTTRMMTH,YRDAYS,REMARK,CHITIEU,CTCV,CREDIT_LINE_YPE,INT_LUMPSUM_PARTIAL_TYPE,INT_PARTIAL_PAYMENT_TYPE,INT_PAYMENT_INTERVAL,AN_HAN_LAI,PHUONG_THUC_GIAI_NGAN_1,TAI_KHOAN_GIAI_NGAN_1,SO_TIEN_GIAI_NGAN_1,PHUONG_THUC_GIAI_NGAN_2,TAI_KHOAN_GIAI_NGAN_2,SO_TIEN_GIAI_NGAN_2,CMT_HC,NGAY_SINH,MA_CB_AGRI,MA_NGANH_KT,TY_GIA,OFFICER_IPCAS
    /// </summary>

    /// <summary>
    /// LN01 Preview DTO - For list/grid display
    /// </summary>
    public class LN01PreviewDto
    {
        public long Id { get; set; }
        public string BRCD { get; set; } = string.Empty;
        public string CUSTSEQ { get; set; } = string.Empty;
        public string CUSTNM { get; set; } = string.Empty;
        public string TAI_KHOAN { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public decimal DU_NO { get; set; }
        public DateTime? TRANSACTION_DATE { get; set; }
        public string LOAN_TYPE { get; set; } = string.Empty;
        public decimal DISBURSEMENT_AMOUNT { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public string OFFICER_NAME { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// LN01 Create DTO - For create operations (All 79 business columns)
    /// </summary>
    public class LN01CreateDto
    {
        [Required]
        [StringLength(10)]
        public string BRCD { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string CUSTSEQ { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string CUSTNM { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TAI_KHOAN { get; set; } = string.Empty;

        [StringLength(10)]
        public string CCY { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal DU_NO { get; set; }

        [StringLength(50)]
        public string DSBSSEQ { get; set; } = string.Empty;

        public DateTime? TRANSACTION_DATE { get; set; }

        public DateTime? DSBSDT { get; set; }

        [StringLength(10)]
        public string DISBUR_CCY { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal DISBURSEMENT_AMOUNT { get; set; }

        public DateTime? DSBSMATDT { get; set; }

        [StringLength(20)]
        public string BSRTCD { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal INTEREST_RATE { get; set; }

        [StringLength(50)]
        public string APPRSEQ { get; set; } = string.Empty;

        public DateTime? APPRDT { get; set; }

        [StringLength(10)]
        public string APPR_CCY { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal APPRAMT { get; set; }

        public DateTime? APPRMATDT { get; set; }

        [StringLength(50)]
        public string LOAN_TYPE { get; set; } = string.Empty;

        [StringLength(50)]
        public string FUND_RESOURCE_CODE { get; set; } = string.Empty;

        [StringLength(50)]
        public string FUND_PURPOSE_CODE { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal REPAYMENT_AMOUNT { get; set; }

        public DateTime? NEXT_REPAY_DATE { get; set; }

        [Range(0, double.MaxValue)]
        public decimal NEXT_REPAY_AMOUNT { get; set; }

        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        [StringLength(100)]
        public string OFFICER_ID { get; set; } = string.Empty;

        [StringLength(255)]
        public string OFFICER_NAME { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal INTEREST_AMOUNT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PASTDUE_INTEREST_AMOUNT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [StringLength(50)]
        public string CUSTOMER_TYPE_CODE { get; set; } = string.Empty;

        [StringLength(100)]
        public string CUSTOMER_TYPE_CODE_DETAIL { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRCTCD { get; set; } = string.Empty;

        [StringLength(255)]
        public string TRCTNM { get; set; } = string.Empty;

        [StringLength(500)]
        public string ADDR1 { get; set; } = string.Empty;

        [StringLength(100)]
        public string PROVINCE { get; set; } = string.Empty;

        [StringLength(255)]
        public string LCLPROVINNM { get; set; } = string.Empty;

        [StringLength(100)]
        public string DISTRICT { get; set; } = string.Empty;

        [StringLength(255)]
        public string LCLDISTNM { get; set; } = string.Empty;

        [StringLength(100)]
        public string COMMCD { get; set; } = string.Empty;

        [StringLength(255)]
        public string LCLWARDNM { get; set; } = string.Empty;

        public DateTime? LAST_REPAY_DATE { get; set; }

        [Range(0, 100)]
        public decimal SECURED_PERCENT { get; set; }

        [StringLength(50)]
        public string NHOM_NO { get; set; } = string.Empty;

        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        [StringLength(10)]
        public string EXEMPTINT { get; set; } = string.Empty;

        [StringLength(50)]
        public string EXEMPTINTTYPE { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal EXEMPTINTAMT { get; set; }

        [StringLength(50)]
        public string GRPNO { get; set; } = string.Empty;

        [StringLength(50)]
        public string BUSCD { get; set; } = string.Empty;

        [StringLength(50)]
        public string BSNSSCLTPCD { get; set; } = string.Empty;

        [StringLength(100)]
        public string USRIDOP { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal ACCRUAL_AMOUNT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        public int INTCMTH { get; set; }

        public int INTRPYMTH { get; set; }

        public int INTTRMMTH { get; set; }

        public int YRDAYS { get; set; }

        [StringLength(1000)]
        public string REMARK { get; set; } = string.Empty;

        [StringLength(100)]
        public string CHITIEU { get; set; } = string.Empty;

        [StringLength(100)]
        public string CTCV { get; set; } = string.Empty;

        [StringLength(50)]
        public string CREDIT_LINE_YPE { get; set; } = string.Empty;

        [StringLength(50)]
        public string INT_LUMPSUM_PARTIAL_TYPE { get; set; } = string.Empty;

        [StringLength(50)]
        public string INT_PARTIAL_PAYMENT_TYPE { get; set; } = string.Empty;

        public int INT_PAYMENT_INTERVAL { get; set; }

        [StringLength(100)]
        public string AN_HAN_LAI { get; set; } = string.Empty;

        [StringLength(100)]
        public string PHUONG_THUC_GIAI_NGAN_1 { get; set; } = string.Empty;

        [StringLength(50)]
        public string TAI_KHOAN_GIAI_NGAN_1 { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SO_TIEN_GIAI_NGAN_1 { get; set; }

        [StringLength(100)]
        public string PHUONG_THUC_GIAI_NGAN_2 { get; set; } = string.Empty;

        [StringLength(50)]
        public string TAI_KHOAN_GIAI_NGAN_2 { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SO_TIEN_GIAI_NGAN_2 { get; set; }

        [StringLength(50)]
        public string CMT_HC { get; set; } = string.Empty;

        public DateTime? NGAY_SINH { get; set; }

        [StringLength(100)]
        public string MA_CB_AGRI { get; set; } = string.Empty;

        [StringLength(100)]
        public string MA_NGANH_KT { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal TY_GIA { get; set; }

        [StringLength(100)]
        public string OFFICER_IPCAS { get; set; } = string.Empty;
    }

    /// <summary>
    /// LN01 Update DTO - For update operations
    /// </summary>
    public class LN01UpdateDto
    {
        [StringLength(255)]
        public string? CUSTNM { get; set; }

        [StringLength(10)]
        public string? CCY { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DU_NO { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? INTEREST_RATE { get; set; }

        [StringLength(255)]
        public string? OFFICER_NAME { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        public DateTime? NEXT_REPAY_DATE { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        [StringLength(500)]
        public string? ADDR1 { get; set; }

        [StringLength(100)]
        public string? PROVINCE { get; set; }

        [StringLength(100)]
        public string? DISTRICT { get; set; }

        [Range(0, 100)]
        public decimal? SECURED_PERCENT { get; set; }

        [StringLength(1000)]
        public string? REMARK { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? TY_GIA { get; set; }
    }

    /// <summary>
    /// LN01 Details DTO - For full record display (All 79 business columns)
    /// </summary>
    public class LN01DetailsDto
    {
        public long Id { get; set; }

        // All 79 business columns
        public string BRCD { get; set; } = string.Empty;
        public string CUSTSEQ { get; set; } = string.Empty;
        public string CUSTNM { get; set; } = string.Empty;
        public string TAI_KHOAN { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public decimal DU_NO { get; set; }
        public string DSBSSEQ { get; set; } = string.Empty;
        public DateTime? TRANSACTION_DATE { get; set; }
        public DateTime? DSBSDT { get; set; }
        public string DISBUR_CCY { get; set; } = string.Empty;
        public decimal DISBURSEMENT_AMOUNT { get; set; }
        public DateTime? DSBSMATDT { get; set; }
        public string BSRTCD { get; set; } = string.Empty;
        public decimal INTEREST_RATE { get; set; }
        public string APPRSEQ { get; set; } = string.Empty;
        public DateTime? APPRDT { get; set; }
        public string APPR_CCY { get; set; } = string.Empty;
        public decimal APPRAMT { get; set; }
        public DateTime? APPRMATDT { get; set; }
        public string LOAN_TYPE { get; set; } = string.Empty;
        public string FUND_RESOURCE_CODE { get; set; } = string.Empty;
        public string FUND_PURPOSE_CODE { get; set; } = string.Empty;
        public decimal REPAYMENT_AMOUNT { get; set; }
        public DateTime? NEXT_REPAY_DATE { get; set; }
        public decimal NEXT_REPAY_AMOUNT { get; set; }
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }
        public string OFFICER_ID { get; set; } = string.Empty;
        public string OFFICER_NAME { get; set; } = string.Empty;
        public decimal INTEREST_AMOUNT { get; set; }
        public decimal PASTDUE_INTEREST_AMOUNT { get; set; }
        public decimal TOTAL_INTEREST_REPAY_AMOUNT { get; set; }
        public string CUSTOMER_TYPE_CODE { get; set; } = string.Empty;
        public string CUSTOMER_TYPE_CODE_DETAIL { get; set; } = string.Empty;
        public string TRCTCD { get; set; } = string.Empty;
        public string TRCTNM { get; set; } = string.Empty;
        public string ADDR1 { get; set; } = string.Empty;
        public string PROVINCE { get; set; } = string.Empty;
        public string LCLPROVINNM { get; set; } = string.Empty;
        public string DISTRICT { get; set; } = string.Empty;
        public string LCLDISTNM { get; set; } = string.Empty;
        public string COMMCD { get; set; } = string.Empty;
        public string LCLWARDNM { get; set; } = string.Empty;
        public DateTime? LAST_REPAY_DATE { get; set; }
        public decimal SECURED_PERCENT { get; set; }
        public string NHOM_NO { get; set; } = string.Empty;
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }
        public string EXEMPTINT { get; set; } = string.Empty;
        public string EXEMPTINTTYPE { get; set; } = string.Empty;
        public decimal EXEMPTINTAMT { get; set; }
        public string GRPNO { get; set; } = string.Empty;
        public string BUSCD { get; set; } = string.Empty;
        public string BSNSSCLTPCD { get; set; } = string.Empty;
        public string USRIDOP { get; set; } = string.Empty;
        public decimal ACCRUAL_AMOUNT { get; set; }
        public decimal ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }
        public int INTCMTH { get; set; }
        public int INTRPYMTH { get; set; }
        public int INTTRMMTH { get; set; }
        public int YRDAYS { get; set; }
        public string REMARK { get; set; } = string.Empty;
        public string CHITIEU { get; set; } = string.Empty;
        public string CTCV { get; set; } = string.Empty;
        public string CREDIT_LINE_YPE { get; set; } = string.Empty;
        public string INT_LUMPSUM_PARTIAL_TYPE { get; set; } = string.Empty;
        public string INT_PARTIAL_PAYMENT_TYPE { get; set; } = string.Empty;
        public int INT_PAYMENT_INTERVAL { get; set; }
        public string AN_HAN_LAI { get; set; } = string.Empty;
        public string PHUONG_THUC_GIAI_NGAN_1 { get; set; } = string.Empty;
        public string TAI_KHOAN_GIAI_NGAN_1 { get; set; } = string.Empty;
        public decimal SO_TIEN_GIAI_NGAN_1 { get; set; }
        public string PHUONG_THUC_GIAI_NGAN_2 { get; set; } = string.Empty;
        public string TAI_KHOAN_GIAI_NGAN_2 { get; set; } = string.Empty;
        public decimal SO_TIEN_GIAI_NGAN_2 { get; set; }
        public string CMT_HC { get; set; } = string.Empty;
        public DateTime? NGAY_SINH { get; set; }
        public string MA_CB_AGRI { get; set; } = string.Empty;
        public string MA_NGANH_KT { get; set; } = string.Empty;
        public decimal TY_GIA { get; set; }
        public string OFFICER_IPCAS { get; set; } = string.Empty;

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// LN01 Summary DTO - For aggregate/summary data
    /// </summary>
    public class LN01SummaryDto
    {
        public long TotalRecords { get; set; }
        public long TotalActiveLoans { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalDisbursementAmount { get; set; }
        public decimal TotalInterestAmount { get; set; }
        public decimal TotalPastdueInterestAmount { get; set; }
        public decimal AverageInterestRate { get; set; }
        public Dictionary<string, long> LoansByType { get; set; } = new();
        public Dictionary<string, long> LoansByBranch { get; set; } = new();
        public Dictionary<string, decimal> OutstandingByCurrency { get; set; } = new();
        public Dictionary<string, long> LoansByOfficer { get; set; } = new();
        public long OverdueLoansCount { get; set; }
        public decimal OverdueAmount { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// LN01 Import Result DTO - For import operation results
    /// </summary>
    public class LN01ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long TotalRecords { get; set; }
        public long ProcessedRecords { get; set; }
        public long SuccessfulRecords { get; set; }
        public long FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public DateTime ImportedAt { get; set; }
        public Dictionary<string, object> ImportStatistics { get; set; } = new();
    }
}
