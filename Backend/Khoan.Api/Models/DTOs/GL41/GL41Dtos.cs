namespace Khoan.Api.Models.Dtos.GL41
{
    /// <summary>
    /// DTO for GL41 preview (list view)
    /// </summary>
    public class GL41PreviewDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        public string MA_CN { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string? TEN_TK { get; set; }
        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }
    }

    /// <summary>
    /// DTO for GL41 detailed view
    /// </summary>
    public class GL41DetailsDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        
        // 13 Business columns (matching GL41Entity exactly)
        public string MA_CN { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string? TEN_TK { get; set; }
        public string? LOAI_BT { get; set; }
        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? SBT_NO { get; set; }
        public decimal? ST_GHINO { get; set; }
        public decimal? SBT_CO { get; set; }
        public decimal? ST_GHICO { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }

        // System columns
        public string? FILE_NAME { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO for creating GL41 record
    /// </summary>
    public class GL41CreateDto
    {
        public DateTime NGAY_DL { get; set; }
        
        // 13 Business columns
        public string MA_CN { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string? TEN_TK { get; set; }
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
    /// DTO for updating GL41 record
    /// </summary>
    public class GL41UpdateDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        
        // 13 Business columns
        public string MA_CN { get; set; } = string.Empty;
        public string LOAI_TIEN { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string? TEN_TK { get; set; }
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
    /// DTO for GL41 CSV import result
    /// </summary>
    public class GL41ImportResultDto
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
    /// DTO for GL41 summary by unit
    /// </summary>
    public class GL41SummaryByUnitDto
    {
        public string UnitCode { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public decimal TotalDebitBalance { get; set; }
        public decimal TotalCreditBalance { get; set; }
        public decimal NetBalance => TotalDebitBalance - TotalCreditBalance;
        public DateTime ReportDate { get; set; }
        public List<string> Currencies { get; set; } = new();
    }

    /// <summary>
    /// DTO for GL41 analytics configuration
    /// </summary>
    public class GL41AnalyticsConfigDto
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
