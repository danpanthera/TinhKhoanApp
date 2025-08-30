using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// 🏦 Bảng dữ liệu Sao kê TSBD (Tiền gửi tiết kiệm Bảo đảm) - Hỗ trợ Temporal Tables cho audit trail
    /// </summary>
    [Table("MSIT72_TSBD")]
    public class MSIT72_TSBD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public int ImportedDataRecordId { get; set; }

        // 📄 Dữ liệu thô từ file CSV/Excel
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? RawData { get; set; }

        // 🔧 Dữ liệu đã xử lý (JSON format)
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? ProcessedData { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        // 🕐 Temporal Tables columns - tự động quản lý bởi SQL Server
        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }

        // 🔗 Navigation property
        [ForeignKey("ImportedDataRecordId")]
        public virtual ImportedDataRecord? ImportedDataRecord { get; set; }
    }
}
