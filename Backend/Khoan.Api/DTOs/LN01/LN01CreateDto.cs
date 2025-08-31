namespace Khoan.Api.Dtos.LN01
{
    /// <summary>
    /// LN01 Create DTO - For creating new records
    /// Required fields for loan creation
    /// </summary>
    public class LN01CreateDto
    {
        public DateTime? NGAY_DL { get; set; }
        
        // Required core fields
        public string? BRCD { get; set; }
        public string? CUSTNM { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? CCY { get; set; }
        public decimal? DU_NO { get; set; }
        
        // Loan details
        public decimal? DISBURSEMENT_AMOUNT { get; set; }
        public decimal? APPRAMT { get; set; }
        public decimal? INTEREST_RATE { get; set; }
        public string? LOAN_TYPE { get; set; }
        
        // Officer
        public string? OFFICER_ID { get; set; }
        public string? OFFICER_NAME { get; set; }
        
        // Optional fields (can be updated later)
        public DateTime? TRANSACTION_DATE { get; set; }
        public DateTime? APPRDT { get; set; }
        public DateTime? APPRMATDT { get; set; }
        public string? REMARK { get; set; }
    }
}
