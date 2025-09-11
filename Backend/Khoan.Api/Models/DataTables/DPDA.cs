using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// DPDA - Deposit Card Application Data Model with exact CSV column structure (13 business columns)
    /// Structure: NGAY_DL -> 13 Business Columns (CSV order) -> System + Temporal Columns
    /// Import policy: Only files containing "dpda" in filename
    /// ALL BUSINESS COLUMNS ALLOW NULL as per requirements
    /// </summary>
    [Table("DPDA")]
    public class DPDA
    {
        // Primary Key
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // NGAY_DL Column - FIRST as per requirement - extracted from filename
        [Column("NGAY_DL", TypeName = "datetime2")]
        public DateTime? NGAY_DL { get; set; }

        // 13 Business Columns - Exact CSV order with NULLABLE types (as per requirements)
        [Column("MA_CHI_NHANH")]
        [StringLength(200)]
        public string? MA_CHI_NHANH { get; set; }

        [Column("MA_KHACH_HANG")]
        [StringLength(200)]
        public string? MA_KHACH_HANG { get; set; }

        [Column("TEN_KHACH_HANG")]
        [StringLength(200)]
        public string? TEN_KHACH_HANG { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(200)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("LOAI_THE")]
        [StringLength(200)]
        public string? LOAI_THE { get; set; }

        [Column("SO_THE")]
        [StringLength(200)]
        public string? SO_THE { get; set; }

        // Date columns - datetime2 format as per requirements
        [Column("NGAY_NOP_DON", TypeName = "datetime2")]
        public DateTime? NGAY_NOP_DON { get; set; }

        [Column("NGAY_PHAT_HANH", TypeName = "datetime2")]
        public DateTime? NGAY_PHAT_HANH { get; set; }

        [Column("USER_PHAT_HANH")]
        [StringLength(200)]
        public string? USER_PHAT_HANH { get; set; }

        [Column("TRANG_THAI")]
        [StringLength(200)]
        public string? TRANG_THAI { get; set; }

        [Column("PHAN_LOAI")]
        [StringLength(200)]
        public string? PHAN_LOAI { get; set; }

        [Column("GIAO_THE")]
        [StringLength(200)]
        public string? GIAO_THE { get; set; }

        [Column("LOAI_PHAT_HANH")]
        [StringLength(200)]
        public string? LOAI_PHAT_HANH { get; set; }

    // === SYSTEM COLUMNS (matching database schema) ===
    // Lưu ý: Cột FILE_NAME không tồn tại trong DB -> không khai báo trong model để EF không generate column khi INSERT

        [Column("CREATED_DATE", TypeName = "datetime2")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", TypeName = "datetime2")]
        public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;

        // Temporal columns (managed by EF Core - shadow properties)
        // ValidFrom and ValidTo are handled automatically
    }
}
