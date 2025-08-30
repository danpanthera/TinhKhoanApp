using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    // 📊 Model chính cho dữ liệu thô
    public class RawDataImport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FileName { get; set; } = null!; // Tên file gốc

        [Required]
        [StringLength(50)]
        public string DataType { get; set; } = null!; // LN01, LN03, DP01, GL01, EI01, DPDA, RR01, GL41

        [Required]
        public DateTime ImportDate { get; set; } // Ngày import

        [Required]
        public DateTime StatementDate { get; set; } // Ngày sao kê từ tên file (yyyymmdd)

        [Required]
        [StringLength(100)]
        public string ImportedBy { get; set; } = null!; // Người import

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = null!; // Processing, Completed, Failed

        public int RecordsCount { get; set; } // Số bản ghi

        public byte[]? OriginalFileData { get; set; } // File gốc

        [StringLength(500)]
        public string? Notes { get; set; } // Ghi chú

        //  Quan hệ với dữ liệu chi tiết
        public virtual ICollection<RawDataRecord> RawDataRecords { get; set; } = new List<RawDataRecord>();
    }

    // 📋 Model cho từng bản ghi dữ liệu thô
    public class RawDataRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RawDataImportId { get; set; }

        [Required]
        public string JsonData { get; set; } = null!; // Dữ liệu JSON từng dòng

        [Required]
        public DateTime ProcessedDate { get; set; }

        [StringLength(500)]
        public string? ProcessingNotes { get; set; }

        // 🔗 Quan hệ ngược
        [ForeignKey("RawDataImportId")]
        public virtual RawDataImport RawDataImport { get; set; } = null!;
    }

    // 📊 Enum định nghĩa loại dữ liệu (hoàn thiện theo Temporal Tables)
    public enum RawDataType
    {
        // Core banking data types
        LN01,         // Dữ liệu LOAN - Danh mục tín dụng
        LN03,         // Dữ liệu Nợ XLRR
        DP01,         // Dữ liệu Tiền gửi
        GL01,         // Dữ liệu bút toán GDV
        EI01,         // Dữ liệu mobile banking

        // Additional banking data types
        DPDA,         // Dữ liệu sao kê phát hành thẻ
        RR01,         // Sao kê dư nợ gốc, lãi XLRR
        GL41       // Bảng cân đối kế toán
    }

    // 📤 DTO cho request import
    public class RawDataImportRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một file")]
        public IFormFileCollection? Files { get; set; }

        public string? DataType { get; set; } // Loại dữ liệu (LN01, LN03, DP01, GL01, EI01, DPDA, RR01, GL41)
        public string? Notes { get; set; } // Ghi chú
    }

    // 📊 DTO cho kết quả import
    public class RawDataImportResult
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty; // ➕ Loại dữ liệu (XLSX, CSV, v.v.)
        public int RecordsProcessed { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? StatementDate { get; set; }
        public string TableName { get; set; } = string.Empty; // Tên table được tạo
        public bool IsArchiveDeleted { get; set; } = false; // ➕ Flag để báo file đã bị xóa
    }

    // 📋 DTO cho preview dữ liệu
    public class RawDataPreviewResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public DateTime StatementDate { get; set; }
        public string ImportedBy { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new List<string>();
        public List<Dictionary<string, object>> Records { get; set; } = new List<Dictionary<string, object>>();
    }
}
