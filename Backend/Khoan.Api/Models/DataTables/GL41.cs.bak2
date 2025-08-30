using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng GL41 - Dữ liệu Số dư tài khoản
    /// Business columns first (13 columns from CSV), then system columns, then temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        // ======= BUSINESS COLUMNS (13 columns - exactly from CSV) =======
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("MA_TK")]
        [StringLength(100)]
        public string? MA_TK { get; set; }

        [Column("TEN_TK")]
        [StringLength(500)]
        public string? TEN_TK { get; set; }

        [Column("LOAI_BT")]
        [StringLength(50)]
        public string? LOAI_BT { get; set; }

        [Column("DN_DAUKY", TypeName = "decimal(18,2)")]
        public decimal? DN_DAUKY { get; set; }

        [Column("DC_DAUKY", TypeName = "decimal(18,2)")]
        public decimal? DC_DAUKY { get; set; }

        [Column("SBT_NO", TypeName = "decimal(18,2)")]
        public decimal? SBT_NO { get; set; }

        [Column("ST_GHINO", TypeName = "decimal(18,2)")]
        public decimal? ST_GHINO { get; set; }

        [Column("SBT_CO", TypeName = "decimal(18,2)")]
        public decimal? SBT_CO { get; set; }

        [Column("ST_GHICO", TypeName = "decimal(18,2)")]
        public decimal? ST_GHICO { get; set; }

        [Column("DN_CUOIKY", TypeName = "decimal(18,2)")]
        public decimal? DN_CUOIKY { get; set; }

        [Column("DC_CUOIKY", TypeName = "decimal(18,2)")]
        public decimal? DC_CUOIKY { get; set; }

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
