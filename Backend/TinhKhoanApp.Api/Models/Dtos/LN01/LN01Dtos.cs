using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.LN01
{
    /// <summary>
    /// LN01 DTOs - 79 business columns từ CSV LN01
    /// </summary>

    /// <summary>
    /// DTO cho preview/listing LN01
    /// </summary>
    public class LN01PreviewDto
    {
        public long Id { get; set; }
        public DateTime? NGAY_DL { get; set; }
        public string? BRCD { get; set; }
        public string? CUSTSEQ { get; set; }
        public string? CUSTNM { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? CCY { get; set; }
        public decimal? DU_NO { get; set; }
        public DateTime? DSBSDT { get; set; }
        public decimal? DISBURSEMENT_AMOUNT { get; set; }
        public DateTime? APPRDT { get; set; }
        public decimal? APPRAMT { get; set; }
        public string? LOAN_TYPE { get; set; }
        public string? NHOM_NO { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho tạo mới LN01
    /// </summary>
    public class LN01CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

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
        public string? SECURED_PERCENT { get; set; }
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
        public string? INTCMTH { get; set; }
        public string? INTRPYMTH { get; set; }
        public string? INTTRMMTH { get; set; }
        public string? YRDAYS { get; set; }
        public string? REMARK { get; set; }
        public string? CHITIEU { get; set; }
        public string? CTCV { get; set; }
        public string? CREDIT_LINE_YPE { get; set; }
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }
        public string? INT_PAYMENT_INTERVAL { get; set; }
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
        public string? FILE_NAME { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật LN01
    /// </summary>
    public class LN01UpdateDto
    {
        public long Id { get; set; }

        public DateTime NGAY_DL { get; set; }
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
        public string? SECURED_PERCENT { get; set; }
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
        public string? INTCMTH { get; set; }
        public string? INTRPYMTH { get; set; }
        public string? INTTRMMTH { get; set; }
        public string? YRDAYS { get; set; }
        public string? REMARK { get; set; }
        public string? CHITIEU { get; set; }
        public string? CTCV { get; set; }
        public string? CREDIT_LINE_YPE { get; set; }
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }
        public string? INT_PAYMENT_INTERVAL { get; set; }
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
        public string? FILE_NAME { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ LN01 (tất cả 79 business columns)
    /// </summary>
    public class LN01DetailsDto : LN01UpdateDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê/tóm tắt LN01
    /// </summary>
    public class LN01SummaryDto
    {
        public int TotalRecords { get; set; }
        public decimal? TotalLoanAmount { get; set; }
        public decimal? TotalDisbursementAmount { get; set; }
        public decimal? TotalInterestAmount { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCustomers { get; set; }
        public DateTime? EarliestDate { get; set; }
        public DateTime? LatestDate { get; set; }
        public List<BranchSummaryDto> BranchSummaries { get; set; } = new();
        public List<DebtGroupSummaryDto> DebtGroupSummaries { get; set; } = new();
    }

    /// <summary>
    /// DTO cho tóm tắt theo chi nhánh
    /// </summary>
    public class BranchSummaryDto
    {
        public string? BRCD { get; set; }
        public int LoanCount { get; set; }
        public decimal? TotalLoanAmount { get; set; }
        public decimal? AverageAmount { get; set; }
    }

    /// <summary>
    /// DTO cho tóm tắt theo nhóm nợ
    /// </summary>
    public class DebtGroupSummaryDto
    {
        public string? NHOM_NO { get; set; }
        public int LoanCount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Percentage { get; set; }
    }

    /// <summary>
    /// DTO cho kết quả import LN01
    /// </summary>
    public class LN01ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ProcessedRecords { get; set; }
        public int InsertedRecords { get; set; }
        public int ErrorRecords { get; set; }
        public string? NgayDL { get; set; }
        public string? FileName { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
