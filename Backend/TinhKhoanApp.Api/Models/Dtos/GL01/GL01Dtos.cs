using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.GL01
{
    /// <summary>
    /// GL01 (General Ledger Detail) DTOs - 27 business columns
    /// CSV Structure: STS,NGAY_GD,NGUOI_TAO,DYSEQ,TR_TYPE,DT_SEQ,TAI_KHOAN,TEN_TK,SO_TIEN_GD,POST_BR,LOAI_TIEN,DR_CR,MA_KH,TEN_KH,CCA_USRID,TR_EX_RT,REMARK,BUS_CODE,UNIT_BUS_CODE,TR_CODE,TR_NAME,REFERENCE,VALUE_DATE,DEPT_CODE,TR_TIME,COMFIRM,TRDT_TIME
    /// </summary>

    /// <summary>
    /// GL01 Preview DTO - For list/grid display
    /// </summary>
    public class GL01PreviewDto
    {
        public long Id { get; set; }
        public string STS { get; set; } = string.Empty;
        public DateTime? NGAY_GD { get; set; }
        public string NGUOI_TAO { get; set; } = string.Empty;
        public string TAI_KHOAN { get; set; } = string.Empty;
        public string TEN_TK { get; set; } = string.Empty;
        public decimal SO_TIEN_GD { get; set; }
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string DR_CR { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// GL01 Create DTO - For create operations
    /// </summary>
    public class GL01CreateDto
    {
        [Required]
        [StringLength(10)]
        public string STS { get; set; } = string.Empty;

        public DateTime? NGAY_GD { get; set; }

        [StringLength(100)]
        public string NGUOI_TAO { get; set; } = string.Empty;

        [StringLength(50)]
        public string DYSEQ { get; set; } = string.Empty;

        [StringLength(10)]
        public string TR_TYPE { get; set; } = string.Empty;

        [StringLength(50)]
        public string DT_SEQ { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TAI_KHOAN { get; set; } = string.Empty;

        [StringLength(255)]
        public string TEN_TK { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SO_TIEN_GD { get; set; }

        [StringLength(10)]
        public string POST_BR { get; set; } = string.Empty;

        [StringLength(10)]
        public string LOAI_TIEN { get; set; } = string.Empty;

        [Required]
        [StringLength(5)]
        public string DR_CR { get; set; } = string.Empty;

        [StringLength(50)]
        public string MA_KH { get; set; } = string.Empty;

        [StringLength(255)]
        public string TEN_KH { get; set; } = string.Empty;

        [StringLength(100)]
        public string CCA_USRID { get; set; } = string.Empty;

        public decimal? TR_EX_RT { get; set; }

        [StringLength(500)]
        public string REMARK { get; set; } = string.Empty;

        [StringLength(50)]
        public string BUS_CODE { get; set; } = string.Empty;

        [StringLength(50)]
        public string UNIT_BUS_CODE { get; set; } = string.Empty;

        [StringLength(50)]
        public string TR_CODE { get; set; } = string.Empty;

        [StringLength(255)]
        public string TR_NAME { get; set; } = string.Empty;

        [StringLength(100)]
        public string REFERENCE { get; set; } = string.Empty;

        public DateTime? VALUE_DATE { get; set; }

        [StringLength(50)]
        public string DEPT_CODE { get; set; } = string.Empty;

        public DateTime? TR_TIME { get; set; }

        [StringLength(10)]
        public string COMFIRM { get; set; } = string.Empty;

        public DateTime? TRDT_TIME { get; set; }
    }

    /// <summary>
    /// GL01 Update DTO - For update operations
    /// </summary>
    public class GL01UpdateDto
    {
        [StringLength(10)]
        public string? STS { get; set; }

        [StringLength(255)]
        public string? TEN_TK { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SO_TIEN_GD { get; set; }

        [StringLength(255)]
        public string? TEN_KH { get; set; }

        public decimal? TR_EX_RT { get; set; }

        [StringLength(500)]
        public string? REMARK { get; set; }

        [StringLength(10)]
        public string? COMFIRM { get; set; }
    }

    /// <summary>
    /// GL01 Details DTO - For full record display
    /// </summary>
    public class GL01DetailsDto
    {
        public long Id { get; set; }
        public string STS { get; set; } = string.Empty;
        public DateTime? NGAY_GD { get; set; }
        public string NGUOI_TAO { get; set; } = string.Empty;
        public string DYSEQ { get; set; } = string.Empty;
        public string TR_TYPE { get; set; } = string.Empty;
        public string DT_SEQ { get; set; } = string.Empty;
        public string TAI_KHOAN { get; set; } = string.Empty;
        public string TEN_TK { get; set; } = string.Empty;
        public decimal SO_TIEN_GD { get; set; }
        public string POST_BR { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string DR_CR { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public string CCA_USRID { get; set; } = string.Empty;
        public decimal? TR_EX_RT { get; set; }
        public string REMARK { get; set; } = string.Empty;
        public string BUS_CODE { get; set; } = string.Empty;
        public string UNIT_BUS_CODE { get; set; } = string.Empty;
        public string TR_CODE { get; set; } = string.Empty;
        public string TR_NAME { get; set; } = string.Empty;
        public string REFERENCE { get; set; } = string.Empty;
        public DateTime? VALUE_DATE { get; set; }
        public string DEPT_CODE { get; set; } = string.Empty;
        public DateTime? TR_TIME { get; set; }
        public string COMFIRM { get; set; } = string.Empty;
        public DateTime? TRDT_TIME { get; set; }

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// GL01 Summary DTO - For aggregate/summary data
    /// </summary>
    public class GL01SummaryDto
    {
        public DateTime? NgayGD { get; set; }
        public long TotalRecords { get; set; }
        public long TotalDebitRecords { get; set; }
        public long TotalCreditRecords { get; set; }
        public decimal TotalDebitAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal NetAmount { get; set; }
        public Dictionary<string, decimal> AmountByCurrency { get; set; } = new();
        public Dictionary<string, long> RecordsByTransactionType { get; set; } = new();
        public Dictionary<string, long> RecordsByBranch { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// GL01 Import Result DTO - For import operation results
    /// </summary>
    public class GL01ImportResultDto
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
