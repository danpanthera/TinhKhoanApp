namespace Khoan.Api.Models.Dtos.GL02
{
    /// <summary>
    /// DTO for GL02 preview (list view)
    /// </summary>
    public class GL02PreviewDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        public string TRBRCD { get; set; } = string.Empty;
        public string LOCAC { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public string? REMARK { get; set; }
    }

    /// <summary>
    /// DTO for GL02 detailed view
    /// </summary>
    public class GL02DetailsDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        
        // 17 Business columns (matching GL02Entity exactly)
        public string TRBRCD { get; set; } = string.Empty;
        public string? USERID { get; set; }
        public string? JOURSEQ { get; set; }
        public string? DYTRSEQ { get; set; }
        public string LOCAC { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string? BUSCD { get; set; }
        public string? UNIT { get; set; }
        public string? TRCD { get; set; }
        public string? CUSTOMER { get; set; }
        public string? TRTP { get; set; }
        public string? REFERENCE { get; set; }
        public string? REMARK { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }

        // System columns
        public string? FILE_NAME { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO for creating GL02 record
    /// </summary>
    public class GL02CreateDto
    {
        public DateTime NGAY_DL { get; set; }
        
        // 17 Business columns
        public string TRBRCD { get; set; } = string.Empty;
        public string? USERID { get; set; }
        public string? JOURSEQ { get; set; }
        public string? DYTRSEQ { get; set; }
        public string LOCAC { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string? BUSCD { get; set; }
        public string? UNIT { get; set; }
        public string? TRCD { get; set; }
        public string? CUSTOMER { get; set; }
        public string? TRTP { get; set; }
        public string? REFERENCE { get; set; }
        public string? REMARK { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }
    }

    /// <summary>
    /// DTO for updating GL02 record
    /// </summary>
    public class GL02UpdateDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        
        // 17 Business columns
        public string TRBRCD { get; set; } = string.Empty;
        public string? USERID { get; set; }
        public string? JOURSEQ { get; set; }
        public string? DYTRSEQ { get; set; }
        public string LOCAC { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string? BUSCD { get; set; }
        public string? UNIT { get; set; }
        public string? TRCD { get; set; }
        public string? CUSTOMER { get; set; }
        public string? TRTP { get; set; }
        public string? REFERENCE { get; set; }
        public string? REMARK { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }
    }

    /// <summary>
    /// DTO for GL02 CSV import result
    /// </summary>
    public class GL02ImportResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public DateTime ImportDateTime { get; set; }
        public TimeSpan ProcessingDuration { get; set; }
        public bool IsSuccess => FailedRecords == 0;
    }

    /// <summary>
    /// DTO for GL02 summary by unit
    /// </summary>
    public class GL02SummaryByUnitDto
    {
        public string UnitCode { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public decimal TotalDebitAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal NetAmount => TotalDebitAmount - TotalCreditAmount;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<string> Currencies { get; set; } = new();
    }

    /// <summary>
    /// DTO for heavy file configuration
    /// </summary>
    public class GL02HeavyFileConfigDto
    {
        public long MaxFileSizeBytes { get; set; }
        public int MaxBatchSize { get; set; }
        public int TimeoutMinutes { get; set; }
        public string AllowedFilePattern { get; set; } = string.Empty;
        public int SupportedColumns { get; set; }
        public string[] RequiredColumns { get; set; } = Array.Empty<string>();
        public string MaxFileSizeDisplay => $"{MaxFileSizeBytes / (1024 * 1024 * 1024)}GB";
    }
}
