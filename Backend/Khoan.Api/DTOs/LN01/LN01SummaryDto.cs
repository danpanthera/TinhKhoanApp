namespace Khoan.Api.Dtos.LN01
{
    /// <summary>
    /// LN01 Summary DTO - For reporting and analytics
    /// </summary>
    public class LN01SummaryDto
    {
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public int TotalLoans { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalDisbursement { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal AverageInterestRate { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
