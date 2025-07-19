using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng EI01 - Dữ liệu Ebanking/Internet Banking
    /// Business columns first (24 columns from CSV), then system columns, then temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        // ======= BUSINESS COLUMNS (24 columns - exactly from CSV) =======
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(100)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(500)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("SDT_EMB")]
        [StringLength(50)]
        public string? SDT_EMB { get; set; }

        [Column("TRANG_THAI_EMB")]
        [StringLength(50)]
        public string? TRANG_THAI_EMB { get; set; }

        [Column("NGAY_DK_EMB")]
        public DateTime? NGAY_DK_EMB { get; set; }

        [Column("SDT_OTT")]
        [StringLength(50)]
        public string? SDT_OTT { get; set; }

        [Column("TRANG_THAI_OTT")]
        [StringLength(50)]
        public string? TRANG_THAI_OTT { get; set; }

        [Column("NGAY_DK_OTT")]
        public DateTime? NGAY_DK_OTT { get; set; }

        [Column("SDT_SMS")]
        [StringLength(50)]
        public string? SDT_SMS { get; set; }

        [Column("TRANG_THAI_SMS")]
        [StringLength(50)]
        public string? TRANG_THAI_SMS { get; set; }

        [Column("NGAY_DK_SMS")]
        public DateTime? NGAY_DK_SMS { get; set; }

        [Column("SDT_SAV")]
        [StringLength(50)]
        public string? SDT_SAV { get; set; }

        [Column("TRANG_THAI_SAV")]
        [StringLength(50)]
        public string? TRANG_THAI_SAV { get; set; }

        [Column("NGAY_DK_SAV")]
        public DateTime? NGAY_DK_SAV { get; set; }

        [Column("SDT_LN")]
        [StringLength(50)]
        public string? SDT_LN { get; set; }

        [Column("TRANG_THAI_LN")]
        [StringLength(50)]
        public string? TRANG_THAI_LN { get; set; }

        [Column("NGAY_DK_LN")]
        public DateTime? NGAY_DK_LN { get; set; }

        [Column("USER_EMB")]
        [StringLength(100)]
        public string? USER_EMB { get; set; }

        [Column("USER_OTT")]
        [StringLength(100)]
        public string? USER_OTT { get; set; }

        [Column("USER_SMS")]
        [StringLength(100)]
        public string? USER_SMS { get; set; }

        [Column("USER_SAV")]
        [StringLength(100)]
        public string? USER_SAV { get; set; }

        [Column("USER_LN")]
        [StringLength(100)]
        public string? USER_LN { get; set; }

        // ======= SYSTEM COLUMNS =======
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("NGAY_DL")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Column("CreatedAt")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("IsDeleted")]
        [Required]
        public bool IsDeleted { get; set; } = false;

        // ======= TEMPORAL COLUMNS (managed by SQL Server) =======
        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }
    }
}
