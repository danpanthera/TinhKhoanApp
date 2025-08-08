using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.GL41
{
    /// <summary>
    /// GL41 (General Ledger Balance) DTOs - 13 business columns
    /// CSV Structure: MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY
    /// </summary>

    /// <summary>
    /// GL41 Preview DTO - For list/grid display
    /// </summary>
    public class GL41PreviewDto
    {
        public long Id { get; set; }
        public string MA_CN { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string TEN_TK { get; set; } = string.Empty;
        public string LOAI_BT { get; set; } = string.Empty;
        public decimal DN_DAUKY { get; set; }
        public decimal DC_DAUKY { get; set; }
        public decimal DN_CUOIKY { get; set; }
        public decimal DC_CUOIKY { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// GL41 Create DTO - For create operations
    /// </summary>
    public class GL41CreateDto
    {
        [Required]
        [StringLength(10)]
        public string MA_CN { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string LOAI_TIEN { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MA_TK { get; set; } = string.Empty;

        [StringLength(255)]
        public string TEN_TK { get; set; } = string.Empty;

        [StringLength(50)]
        public string LOAI_BT { get; set; } = string.Empty;

        public decimal DN_DAUKY { get; set; }
        public decimal DC_DAUKY { get; set; }
        public decimal SBT_NO { get; set; }
        public decimal ST_GHINO { get; set; }
        public decimal SBT_CO { get; set; }
        public decimal ST_GHICO { get; set; }
        public decimal DN_CUOIKY { get; set; }
        public decimal DC_CUOIKY { get; set; }
    }

    /// <summary>
    /// GL41 Update DTO - For update operations
    /// </summary>
    public class GL41UpdateDto
    {
        [StringLength(255)]
        public string? TEN_TK { get; set; }

        [StringLength(50)]
        public string? LOAI_BT { get; set; }

        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? SBT_NO { get; set; }
        public decimal? ST_GHINO { get; set; }
        public decimal? SBT_CO { get; set; }
        public decimal? ST_GHICO { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }
    }

    /// <summary>
    /// GL41 Details DTO - For full record display
    /// </summary>
    public class GL41DetailsDto
    {
        public long Id { get; set; }
        public string MA_CN { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string TEN_TK { get; set; } = string.Empty;
        public string LOAI_BT { get; set; } = string.Empty;
        public decimal DN_DAUKY { get; set; }
        public decimal DC_DAUKY { get; set; }
        public decimal SBT_NO { get; set; }
        public decimal ST_GHINO { get; set; }
        public decimal SBT_CO { get; set; }
        public decimal ST_GHICO { get; set; }
        public decimal DN_CUOIKY { get; set; }
        public decimal DC_CUOIKY { get; set; }

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// GL41 Summary DTO - For aggregate/summary data
    /// </summary>
    public class GL41SummaryDto
    {
        public long TotalRecords { get; set; }
        public Dictionary<string, long> RecordsByBranch { get; set; } = new();
        public Dictionary<string, long> RecordsByCurrency { get; set; } = new();
        public Dictionary<string, decimal> TotalBalanceByAccount { get; set; } = new();
        public decimal TotalOpeningDebitBalance { get; set; }
        public decimal TotalOpeningCreditBalance { get; set; }
        public decimal TotalClosingDebitBalance { get; set; }
        public decimal TotalClosingCreditBalance { get; set; }
        public decimal TotalDebitMovement { get; set; }
        public decimal TotalCreditMovement { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// GL41 Import Result DTO - For import operation results
    /// </summary>
    public class GL41ImportResultDto
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
