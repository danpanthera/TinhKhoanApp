namespace Khoan.Api.Dtos.LN01
{
    /// <summary>
    /// LN01 Preview DTO - For list display with key fields
    /// Contains most important loan information for overview
    /// </summary>
    public class LN01PreviewDto
    {
        public int Id { get; set; }
        public DateTime? NGAY_DL { get; set; }
        
        // Core loan identification
        public string? BRCD { get; set; }
        public string? CUSTSEQ { get; set; }
        public string? CUSTNM { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? CCY { get; set; }
        
        // Financial summary
        public decimal? DU_NO { get; set; }
        public decimal? DISBURSEMENT_AMOUNT { get; set; }
        public decimal? APPRAMT { get; set; }
        public decimal? INTEREST_RATE { get; set; }
        
        // Dates
        public DateTime? TRANSACTION_DATE { get; set; }
        public DateTime? APPRMATDT { get; set; }
        
        // Loan type
        public string? LOAN_TYPE { get; set; }
        
        // Officer
        public string? OFFICER_NAME { get; set; }
        public string? OFFICER_ID { get; set; }
        
        // System
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
