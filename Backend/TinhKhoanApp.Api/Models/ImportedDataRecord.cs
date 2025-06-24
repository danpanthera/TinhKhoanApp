using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// 🏦 Bảng chính chứa thông tin các file import - Hỗ trợ Temporal Tables cho audit trail
    /// </summary>
    [Table("ImportedDataRecords")]
    public class ImportedDataRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime ImportDate { get; set; }

        // 📅 Ngày sao kê - từ tên file hoặc được chỉ định
        public DateTime? StatementDate { get; set; }

        [Required]
        [StringLength(100)]
        public string ImportedBy { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed

        public int RecordsCount { get; set; } = 0;

        // 💾 Store original file as binary data
        public byte[]? OriginalFileData { get; set; }

        // 🗜️ Store compressed data using optimized compression
        public byte[]? CompressedData { get; set; }
        
        // 📊 Compression ratio for statistics
        public double CompressionRatio { get; set; } = 0.0;

        [StringLength(1000)]
        public string? Notes { get; set; }

        // 🕒 Temporal Table columns sẽ được quản lý bởi EF Core như shadow properties
        // Không cần khai báo ở đây, sẽ được cấu hình trong OnModelCreating

        // Navigation property
        public virtual ICollection<ImportedDataItem> ImportedDataItems { get; set; }

        public ImportedDataRecord()
        {
            ImportedDataItems = new HashSet<ImportedDataItem>();
        }
    }

    /// <summary>
    /// 📊 Bảng chi tiết chứa dữ liệu đã import - Tối ưu với Columnstore Index cho analytics
    /// </summary>
    [Table("ImportedDataItems")]
    public class ImportedDataItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ImportedDataRecordId { get; set; }

        [ForeignKey("ImportedDataRecordId")]
        public virtual ImportedDataRecord ImportedDataRecord { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(max)")] // Cho phép lưu JSON lớn
        public string RawData { get; set; } = string.Empty; // JSON format

        public DateTime ProcessedDate { get; set; }

        [StringLength(1000)]
        public string? ProcessingNotes { get; set; }

        // 🕒 Temporal Table columns sẽ được quản lý bởi EF Core như shadow properties
        // Không cần khai báo ở đây, sẽ được cấu hình trong OnModelCreating
    }
}
