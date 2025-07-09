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

        [StringLength(1000)]
        public string? Notes { get; set; }

        // 🕒 Temporal Table columns sẽ được quản lý bởi EF Core như shadow properties
        // Không cần khai báo ở đây, sẽ được cấu hình trong OnModelCreating

        // ✅ CLEANED: Removed ImportedDataItems navigation property for Direct Import workflow
        // Data is now stored directly in specific tables (DP01, LN01, etc.)
    }
}
