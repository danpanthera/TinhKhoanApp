using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng LN03 - Dữ liệu Cơ cấu lại nợ
    /// CSV Business columns FIRST (17 columns), then System/Temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        // ======= CSV BUSINESS COLUMNS (1-17) - EXACT ORDER FROM CSV =======
        [Column("MACHINHANH")]
        [StringLength(50)]
        public string? MACHINHANH { get; set; }

        [Column("TENCHINHANH")]
        [StringLength(255)]
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
        public int? NHOMNO { get; set; }

        [Column("MACBTD")]
        [StringLength(50)]
        public string? MACBTD { get; set; }

        [Column("TENCBTD")]
        [StringLength(255)]
        public string? TENCBTD { get; set; }

        [Column("MAPGD")]
        [StringLength(50)]
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

        // ======= SYSTEM/TEMPORAL COLUMNS (18+) =======
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("NGAY_DL")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // ======= TEMPORAL COLUMNS (managed by SQL Server) =======
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime UPDATED_DATE { get; set; }
    }
}
