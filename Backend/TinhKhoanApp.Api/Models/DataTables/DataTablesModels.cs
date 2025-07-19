namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Models cho 8 bảng dữ liệu chính - Direct Import và Preview
    /// </summary>

    public class GL01ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? Currency { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? Balance { get; set; }
        public string? TransactionCode { get; set; }
        public string? Description { get; set; }
        public string? Reference { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? BookingDate { get; set; }
    }

    public class DP01ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? ProductType { get; set; }
        public string? Currency { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string? Status { get; set; }
    }

    public class DPDAImportModel
    {
        public DateTime DataDate { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? DepositType { get; set; }
        public decimal? Amount { get; set; }
        public int? Term { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MaturityAmount { get; set; }
    }

    public class EI01ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Position { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? TotalIncome { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Status { get; set; }
    }

    public class GL41ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? BranchCode { get; set; }
        public string? CostCenter { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public string? Description { get; set; }
    }

    public class LN01ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? LoanNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? BranchCode { get; set; }
        public string? LoanType { get; set; }
        public string? Currency { get; set; }
        public decimal? LoanAmount { get; set; }
        public decimal? OutstandingBalance { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public DateTime? DisbursementDate { get; set; }
        public string? Status { get; set; }
    }

    public class LN03ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? LoanNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? CollateralType { get; set; }
        public decimal? CollateralValue { get; set; }
        public string? CollateralDescription { get; set; }
        public DateTime? AppraisalDate { get; set; }
        public decimal? AppraisalValue { get; set; }
        public decimal? SecurityRatio { get; set; }
    }

    public class RR01ImportModel
    {
        public DateTime DataDate { get; set; }
        public string? RiskCategory { get; set; }
        public string? CustomerCode { get; set; }
        public string? LoanNumber { get; set; }
        public string? RiskRating { get; set; }
        public decimal? ExposureAmount { get; set; }
        public decimal? ProbabilityOfDefault { get; set; }
        public decimal? LossGivenDefault { get; set; }
        public decimal? ExpectedLoss { get; set; }
        public decimal? RiskWeight { get; set; }
    }

    public class DataTableSummary
    {
        public string TableName { get; set; } = string.Empty;
        public string StorageType { get; set; } = string.Empty;
        public string HasColumnstore { get; set; } = string.Empty;
        public long RecordCount { get; set; }
    }

    public class DataTablesBulkImportRequest
    {
        public List<DataTableImportData> Tables { get; set; } = new();
    }

    public class DataTableImportData
    {
        public string TableName { get; set; } = string.Empty;
        public List<Dictionary<string, object>> Data { get; set; } = new();
    }

    public class DataTableImportResult
    {
        public string TableName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int ImportedRecords { get; set; }
        public string Message { get; set; } = string.Empty;
        public string StorageType { get; set; } = string.Empty;
    }

    public class DataTablePreviewResponse
    {
        public bool Success { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string StorageType { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
