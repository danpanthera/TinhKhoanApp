using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Entities
{
    [Table("LN03")]
    public class LN03Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // === NGAY_DL FIRST (extracted from filename) ===
        [Column("NGAY_DL")]
        public DateTime? NGAY_DL { get; set; }

        // === 17 NAMED BUSINESS COLUMNS (from CSV header) ===
        [Column("MACHINHANH")]
        [MaxLength(200)]
        public string? MACHINHANH { get; set; }

        [Column("TENCHINHANH")]
        [MaxLength(200)]
        public string? TENCHINHANH { get; set; }

        [Column("MAKH")]
        [MaxLength(200)]
        public string? MAKH { get; set; }

        [Column("TENKH")]
        [MaxLength(200)]
        public string? TENKH { get; set; }

        [Column("SOHOPDONG")]
        [MaxLength(200)]
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
        [MaxLength(200)]
        public string? NHOMNO { get; set; }

        [Column("MACBTD")]
        [MaxLength(200)]
        public string? MACBTD { get; set; }

        [Column("TENCBTD")]
        [MaxLength(200)]
        public string? TENCBTD { get; set; }

        [Column("MAPGD")]
        [MaxLength(200)]
        public string? MAPGD { get; set; }

        [Column("TAIKHOANHACHTOAN")]
        [MaxLength(200)]
        public string? TAIKHOANHACHTOAN { get; set; }

        [Column("REFNO")]
        [MaxLength(200)]
        public string? REFNO { get; set; }

        [Column("LOAINGUONVON")]
        [MaxLength(200)]
        public string? LOAINGUONVON { get; set; }

        // === 3 UNNAMED BUSINESS COLUMNS (no header, but have data) ===
        [Column("COLUMN_18")]
        [MaxLength(200)]
        public string? COLUMN_18 { get; set; }

        [Column("COLUMN_19")]
        [MaxLength(200)]
        public string? COLUMN_19 { get; set; }

        [Column("COLUMN_20", TypeName = "decimal(18,2)")]
        public decimal? COLUMN_20 { get; set; }

        // === SYSTEM COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("IS_DELETED")]
        public bool IS_DELETED { get; set; } = false;

        // Temporal Table System Columns are configured as shadow properties
        // SysStartTime and SysEndTime are not declared here - they're shadow properties
    }
}
