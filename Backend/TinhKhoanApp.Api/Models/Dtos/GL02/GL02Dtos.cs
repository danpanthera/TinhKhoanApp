using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.GL02
{
    /// <summary>
    /// GL02 (General Ledger Summary) DTOs - 17 business columns
    /// CSV Structure: TRDATE,TRBRCD,USERID,JOURSEQ,DYTRSEQ,LOCAC,CCY,BUSCD,UNIT,TRCD,CUSTOMER,TRTP,REFERENCE,REMARK,DRAMOUNT,CRAMOUNT,CRTDTM
    /// </summary>

    /// <summary>
    /// GL02 Preview DTO - For list/grid display
    /// </summary>
    public class GL02PreviewDto
    {
        public long Id { get; set; }
        public DateTime? TRDATE { get; set; }
        public string TRBRCD { get; set; } = string.Empty;
        public string USERID { get; set; } = string.Empty;
        public string LOCAC { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string TRCD { get; set; } = string.Empty;
        public string CUSTOMER { get; set; } = string.Empty;
        public decimal DRAMOUNT { get; set; }
        public decimal CRAMOUNT { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// GL02 Create DTO - For create operations
    /// </summary>
    public class GL02CreateDto
    {
        public DateTime? TRDATE { get; set; }

        [StringLength(10)]
        public string TRBRCD { get; set; } = string.Empty;

        [StringLength(50)]
        public string USERID { get; set; } = string.Empty;

        [StringLength(50)]
        public string JOURSEQ { get; set; } = string.Empty;

        [StringLength(50)]
        public string DYTRSEQ { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LOCAC { get; set; } = string.Empty;

        [StringLength(10)]
        public string CCY { get; set; } = string.Empty;

        [StringLength(50)]
        public string BUSCD { get; set; } = string.Empty;

        [StringLength(50)]
        public string UNIT { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRCD { get; set; } = string.Empty;

        [StringLength(50)]
        public string CUSTOMER { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRTP { get; set; } = string.Empty;

        [StringLength(100)]
        public string REFERENCE { get; set; } = string.Empty;

        [StringLength(500)]
        public string REMARK { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal DRAMOUNT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CRAMOUNT { get; set; }

        public DateTime? CRTDTM { get; set; }
    }

    /// <summary>
    /// GL02 Update DTO - For update operations
    /// </summary>
    public class GL02UpdateDto
    {
        [StringLength(50)]
        public string? USERID { get; set; }

        [StringLength(500)]
        public string? REMARK { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DRAMOUNT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CRAMOUNT { get; set; }
    }

    /// <summary>
    /// GL02 Details DTO - For full record display
    /// </summary>
    public class GL02DetailsDto
    {
        public long Id { get; set; }
        public DateTime? TRDATE { get; set; }
        public string TRBRCD { get; set; } = string.Empty;
        public string USERID { get; set; } = string.Empty;
        public string JOURSEQ { get; set; } = string.Empty;
        public string DYTRSEQ { get; set; } = string.Empty;
        public string LOCAC { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string BUSCD { get; set; } = string.Empty;
        public string UNIT { get; set; } = string.Empty;
        public string TRCD { get; set; } = string.Empty;
        public string CUSTOMER { get; set; } = string.Empty;
        public string TRTP { get; set; } = string.Empty;
        public string REFERENCE { get; set; } = string.Empty;
        public string REMARK { get; set; } = string.Empty;
        public decimal DRAMOUNT { get; set; }
        public decimal CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// GL02 Summary DTO - For aggregate/summary data
    /// </summary>
    public class GL02SummaryDto
    {
        public DateTime? TrDate { get; set; }
        public long TotalRecords { get; set; }
        public decimal TotalDebitAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal NetAmount { get; set; }
        public Dictionary<string, decimal> AmountByCurrency { get; set; } = new();
        public Dictionary<string, long> RecordsByBranch { get; set; } = new();
        public Dictionary<string, long> RecordsByTransactionType { get; set; } = new();
        public Dictionary<string, decimal> DebitAmountByAccount { get; set; } = new();
        public Dictionary<string, decimal> CreditAmountByAccount { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// GL02 Import Result DTO - For import operation results
    /// </summary>
    public class GL02ImportResultDto
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
