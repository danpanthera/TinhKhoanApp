using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.Importing
{
    [Table("ImportHistory")]
    public class ImportHistory
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(32)]
        public string DataType { get; set; } = string.Empty; // DP01 / GL01 / ...
        [MaxLength(128)]
        public string FileName { get; set; } = string.Empty;
        [MaxLength(10)]
        public string? NgayDL { get; set; } // yyyy-MM-dd
        public long FileSizeBytes { get; set; }
        public int RecordsInserted { get; set; }
        public double DurationSeconds { get; set; }
        public DateTime StartedUtc { get; set; }
        public DateTime CompletedUtc { get; set; }
        [MaxLength(32)]
        public string Status { get; set; } = "SUCCESS"; // SUCCESS / FAILED / PARTIAL / ABORTED
        [MaxLength(512)]
        public string? ErrorMessage { get; set; }
        public int? AbortErrorThreshold { get; set; }
        public int? BatchSizeUsed { get; set; }
        public int? ProgressIntervalUsed { get; set; }
    }
}