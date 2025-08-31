using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Entities
{
    [Table("LN03_Simple")]
    public class LN03SimpleEntity  // Temporary simple version
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Essential business columns only
        [Column("NGAY_DL")]
        public DateTime? NGAY_DL { get; set; }

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

        [Column("DUNONOIBANG", TypeName = "decimal(18,2)")]
        public decimal? DUNONOIBANG { get; set; }

        [Column("CONLAINGOAIBANG", TypeName = "decimal(18,2)")]
        public decimal? CONLAINGOAIBANG { get; set; }

        [Column("NHOMNO")]
        [MaxLength(200)]
        public string? NHOMNO { get; set; }

        [Column("REFNO")]
        [MaxLength(200)]
        public string? REFNO { get; set; }

        [Column("LOAINGUONVON")]
        [MaxLength(200)]
        public string? LOAINGUONVON { get; set; }

        // System columns (required by repository)
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("IS_DELETED")]
        public bool IS_DELETED { get; set; } = false;

        // No temporal features for now - just get basic CRUD working
    }
}
