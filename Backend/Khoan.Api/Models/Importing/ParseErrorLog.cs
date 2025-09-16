using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.Importing
{
    [Table("ParseErrorLog")]
    public class ParseErrorLog
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(32)]
        public string Table { get; set; } = string.Empty;
        [MaxLength(512)]
        public string Error { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? LineSample { get; set; }
        [MaxLength(2000)]
        public string? Header { get; set; }
        [MaxLength(260)]
        public string? FileName { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        // Optional correlation id (import batch) for grouping
        [MaxLength(64)]
        public string? BatchId { get; set; }
    }
}