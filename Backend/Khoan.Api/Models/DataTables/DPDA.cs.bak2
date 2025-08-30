using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng DPDA - Dữ liệu Thẻ ATM/Debit
    /// Business columns first (13 columns from CSV), then system columns, then temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("DPDA")]
    public class DPDA
    {
        // ======= BUSINESS COLUMNS (13 columns - exactly from CSV) =======
        [Column("MA_CHI_NHANH")]
        [StringLength(50)]
        public string? MA_CHI_NHANH { get; set; }

        [Column("MA_KHACH_HANG")]
        [StringLength(100)]
        public string? MA_KHACH_HANG { get; set; }

        [Column("TEN_KHACH_HANG")]
        [StringLength(500)]
        public string? TEN_KHACH_HANG { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(100)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("LOAI_THE")]
        [StringLength(50)]
        public string? LOAI_THE { get; set; }

        [Column("SO_THE")]
        [StringLength(100)]
        public string? SO_THE { get; set; }

        [Column("NGAY_NOP_DON")]
        public DateTime? NGAY_NOP_DON { get; set; }

        [Column("NGAY_PHAT_HANH")]
        public DateTime? NGAY_PHAT_HANH { get; set; }

        [Column("USER_PHAT_HANH")]
        [StringLength(100)]
        public string? USER_PHAT_HANH { get; set; }

        [Column("TRANG_THAI")]
        [StringLength(50)]
        public string? TRANG_THAI { get; set; }

        [Column("PHAN_LOAI")]
        [StringLength(50)]
        public string? PHAN_LOAI { get; set; }

        [Column("GIAO_THE")]
        [StringLength(50)]
        public string? GIAO_THE { get; set; }

        [Column("LOAI_PHAT_HANH")]
        [StringLength(50)]
        public string? LOAI_PHAT_HANH { get; set; }

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
