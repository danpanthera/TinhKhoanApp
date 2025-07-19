using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng LN03 - Dữ liệu Cơ cấu lại nợ 
    /// Business columns first (17 columns from CSV), then system columns, then temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        // ======= BUSINESS COLUMNS (17 columns - exactly from CSV) =======
        [Column("MACHINHANH")]
        [StringLength(50)]
        public string? MACHINHANH { get; set; }

        [Column("TENCHINHANH")]
        [StringLength(500)]
        public string? TENCHINHANH { get; set; }

        [Column("MAKH")]
        [StringLength(100)]
        public string? MAKH { get; set; }

        [Column("TENKH")]
        [StringLength(500)]
        public string? TENKH { get; set; }

        [Column("SOHOPDONG")]
        [StringLength(100)]
        public string? SOHOPDONG { get; set; }

        [Column("SOTIENXLRR", TypeName = "decimal(18,2)")]
        public decimal? SOTIENXLRR { get; set; }

        [Column("NGAYPHATSINHXL")]
        public DateTime? NGAYPHATSINHXL { get; set; }

        [Column("THUNOSAUXL", TypeName = "decimal(18,2)")]
        public decimal? THUNOSAUXL { get; set; }

        [Column("CONLAINGOAIBANG", TypeName = "decimal(18,2)")]
        public decimal? CONLAINGOAIBANG { get; set; }

        [Column("DUNONOIBANG", TypeName = "decimal(18,2)")]
        public decimal? DUNONOIBANG { get; set; }

        [Column("NHOMNO")]
        [StringLength(50)]
        public string? NHOMNO { get; set; }

        [Column("MACBTD")]
        [StringLength(100)]
        public string? MACBTD { get; set; }

        [Column("TENCBTD")]
        [StringLength(500)]
        public string? TENCBTD { get; set; }

        [Column("MAPGD")]
        [StringLength(100)]
        public string? MAPGD { get; set; }

        [Column("TAIKHOANHACHTOAN")]
        [StringLength(100)]
        public string? TAIKHOANHACHTOAN { get; set; }

        [Column("REFNO")]
        [StringLength(100)]
        public string? REFNO { get; set; }

        [Column("LOAINGUONVON")]
        [StringLength(100)]
        public string? LOAINGUONVON { get; set; }

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
