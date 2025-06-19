namespace TinhKhoanApp.Api.Models.Dtos
{
    public class ImportStatisticsResponse
    {
        public int TotalImports { get; set; }
        public int TotalFiles { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public double SuccessRate { get; set; }
        public long TotalRecords { get; set; }
        public double AverageCompressionRatio { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<BranchStatistic> BranchStatistics { get; set; } = new List<BranchStatistic>();
        public List<ReportTypeStatistic> ReportTypeStatistics { get; set; } = new List<ReportTypeStatistic>();
        public List<RecentImport> RecentImports { get; set; } = new List<RecentImport>();
    }

    public class BranchStatistic
    {
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int ImportCount { get; set; }
        public int FileCount { get; set; }
        public int RecordCount { get; set; }
        public DateTime LastImport { get; set; }
        public DateTime LastImportDate { get; set; }
    }

    public class ReportTypeStatistic
    {
        public string ReportType { get; set; } = string.Empty;
        public int ImportCount { get; set; }
        public int FileCount { get; set; }
        public int RecordCount { get; set; }
        public DateTime LastImport { get; set; }
    }

    public class RecentImport
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public DateTime? StatementDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RecordsCount { get; set; }
        public double CompressionRatio { get; set; }
    }
}
