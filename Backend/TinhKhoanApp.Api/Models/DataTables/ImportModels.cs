using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Import model cho GL01 - chỉ cần các field cần thiết cho import
    /// </summary>
    public class GL01ImportModel
    {
        [Required]
        public DateTime NGAY_DL { get; set; }
        public string? MA_CN { get; set; }
        public string? MA_TK { get; set; }
        public string? TEN_TK { get; set; }
        public decimal? SO_TIEN_GD { get; set; }
        public string? POST_BR { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? DR_CR { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? CCA_USRID { get; set; }
        public decimal? TR_EX_RT { get; set; }
        public string? REMARK { get; set; }
        public string? BUS_CODE { get; set; }
        public string? UNIT_BUS_CODE { get; set; }
        public string? TR_CODE { get; set; }
        public string? TR_NAME { get; set; }
        public string? REFERENCE { get; set; }
        public string? VALUE_DATE { get; set; }
        public string? DEPT_CODE { get; set; }
        public string? TR_TIME { get; set; }
        public string? COMFIRM { get; set; }
        public string? TRDT_TIME { get; set; }

        // Mapping properties cho compatibility với controller cũ
        public DateTime DataDate => NGAY_DL;
        public string? BranchCode => MA_CN;
        public string? AccountNumber => MA_TK;
        public string? CustomerCode => MA_KH;
        public string? Currency => LOAI_TIEN;
        public decimal? DebitAmount => DR_CR == "D" ? SO_TIEN_GD : 0;
        public decimal? CreditAmount => DR_CR == "C" ? SO_TIEN_GD : 0;
        public decimal? Balance => SO_TIEN_GD;
        public string? TransactionCode => TR_CODE;
        public string? Description => REMARK;
        public string? Reference => REFERENCE;
        public string? ValueDate => VALUE_DATE;
        public string? BookingDate => TR_TIME;
    }

    /// <summary>
    /// Import model cho DP01 - chỉ cần các field cần thiết cho import
    /// </summary>
    public class DP01ImportModel
    {
        [Required]
        public DateTime NGAY_DL { get; set; }
        public string? MA_CN { get; set; }
        public string? TAI_KHOAN_HACH_TOAN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? DP_TYPE_NAME { get; set; }
        public string? CCY { get; set; }
        public decimal? CURRENT_BALANCE { get; set; }
        public decimal? RATE { get; set; }
        public string? SO_TAI_KHOAN { get; set; }
        public string? OPENING_DATE { get; set; }
        public string? MATURITY_DATE { get; set; }
        public string? ADDRESS { get; set; }
        public string? NOTENO { get; set; }
        public string? MONTH_TERM { get; set; }
        public string? TERM_DP_NAME { get; set; }
        public string? TIME_DP_NAME { get; set; }
        public string? MA_PGD { get; set; }
        public string? TEN_PGD { get; set; }

        // Mapping properties cho compatibility với controller cũ
        public DateTime DataDate => NGAY_DL;
        public string? BranchCode => MA_CN;
        public string? AccountNumber => TAI_KHOAN_HACH_TOAN;
        public string? CustomerCode => MA_KH;
        public string? ProductType => DP_TYPE_NAME;
        public string? Currency => CCY;
        public decimal? DepositAmount => CURRENT_BALANCE;
        public decimal? Balance => CURRENT_BALANCE;
        public decimal? InterestRate => RATE;
        public string? MaturityDate => MATURITY_DATE;
        public string? OpeningDate => OPENING_DATE;
        public string? Status => "Active";
    }

    /// <summary>
    /// Table data for bulk import
    /// </summary>
    public class TableImportData
    {
        public string TableName { get; set; } = string.Empty;
        public List<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
    }

    /// <summary>
    /// Bulk import request cho tất cả DataTables
    /// </summary>
    public class DataTablesBulkImportRequest
    {
        public string TableName { get; set; } = string.Empty;
        public List<object> Data { get; set; } = new List<object>();
        public bool ClearExistingData { get; set; } = false;
        public string? ImportNote { get; set; }
        public List<TableImportData> Tables { get; set; } = new List<TableImportData>(); // Change back to proper structure
    }

    /// <summary>
    /// DataTable summary response model
    /// </summary>
    public class DataTableSummary
    {
        public string TableName { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result cho bulk import operation
    /// </summary>
    public class DataTableImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ErrorCount { get; set; }
        public int TotalRecordsProcessed { get; set; }
        public List<DataTableSummary> TableSummaries { get; set; } = new List<DataTableSummary>();
        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    }
}
