using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// 📊 Bảng chứa từng bản ghi dữ liệu đã import - Temporal Tables với Columnstore Index
    /// </summary>
    [Table("ImportedDataItems")]
    public class ImportedDataItem
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Foreign key tới ImportedDataRecord
        [Required]
        public int ImportedDataRecordId { get; set; }

        // 📊 Dữ liệu thô dạng JSON
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string RawData { get; set; } = string.Empty;

        // 📅 Ngày xử lý
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        // 📝 Ghi chú xử lý
        [StringLength(500)]
        public string? ProcessingNotes { get; set; }

        // 🔗 Navigation property
        [ForeignKey("ImportedDataRecordId")]
        public virtual ImportedDataRecord ImportedDataRecord { get; set; } = null!;

        // ⚠️ Temporal Tables system versioned columns - Do NOT include as properties
        // These are managed by SQL Server automatically as shadow properties
    }
}
