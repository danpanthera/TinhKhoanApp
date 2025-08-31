namespace Khoan.Api.Dtos.LN01
{
    /// <summary>
    /// LN01 Update DTO - For updating existing records
    /// Allows partial updates
    /// </summary>
    public class LN01UpdateDto
    {
        public DateTime? NGAY_DL { get; set; }
        public string? CUSTNM { get; set; }
        public decimal? DU_NO { get; set; }
        public decimal? DISBURSEMENT_AMOUNT { get; set; }
        public decimal? APPRAMT { get; set; }
        public decimal? INTEREST_RATE { get; set; }
        public string? LOAN_TYPE { get; set; }
        public string? OFFICER_ID { get; set; }
        public string? OFFICER_NAME { get; set; }
        public DateTime? TRANSACTION_DATE { get; set; }
        public DateTime? APPRDT { get; set; }
        public DateTime? APPRMATDT { get; set; }
        public string? REMARK { get; set; }
        public decimal? REPAYMENT_AMOUNT { get; set; }
        public DateTime? NEXT_REPAY_DATE { get; set; }
        public string? NHOM_NO { get; set; }
        public string? ADDR1 { get; set; }
        public string? PROVINCE { get; set; }
        public string? DISTRICT { get; set; }
    }
}
