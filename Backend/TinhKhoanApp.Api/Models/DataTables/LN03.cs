using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// LN03 - Loan Recovery Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        // System Column - NGAY_DL first (extracted from filename)
        [Column("NGAY_DL", Order = 0)]
        [StringLength(10)]
        public string NGAY_DL { get; set; } = "";

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("MACHINHANH", Order = 1)]
        [StringLength(50)]
        public string MACHINHANH { get; set; } = "";

        [Column("TENCHINHANH", Order = 2)]
        [StringLength(50)]
        public string TENCHINHANH { get; set; } = "";

        [Column("MAKH", Order = 3)]
        [StringLength(50)]
        public string MAKH { get; set; } = "";

        [Column("TENKH", Order = 4)]
        [StringLength(50)]
        public string TENKH { get; set; } = "";

        [Column("SOHOPDONG", Order = 5)]
        [StringLength(50)]
        public string SOHOPDONG { get; set; } = "";

        [Column("SOTIENXLRR", Order = 6)]
        [StringLength(50)]
        public string SOTIENXLRR { get; set; } = "";

        [Column("NGAYPHATSINHXL", Order = 7)]
        [StringLength(50)]
        public string NGAYPHATSINHXL { get; set; } = "";

        [Column("THUNOSAUXL", Order = 8)]
        [StringLength(50)]
        public string THUNOSAUXL { get; set; } = "";

        [Column("CONLAINGOAIBANG", Order = 9)]
        [StringLength(50)]
        public string CONLAINGOAIBANG { get; set; } = "";

        [Column("DUNONOIBANG", Order = 10)]
        [StringLength(50)]
        public string DUNONOIBANG { get; set; } = "";

        [Column("NHOMNO", Order = 11)]
        [StringLength(50)]
        public string NHOMNO { get; set; } = "";

        [Column("MACBTD", Order = 12)]
        [StringLength(50)]
        public string MACBTD { get; set; } = "";

        [Column("TENCBTD", Order = 13)]
        [StringLength(50)]
        public string TENCBTD { get; set; } = "";

        [Column("MAPGD", Order = 14)]
        [StringLength(50)]
        public string MAPGD { get; set; } = "";

        [Column("TAIKHOANHACHTOAN", Order = 15)]
        [StringLength(50)]
        public string TAIKHOANHACHTOAN { get; set; } = "";

        [Column("REFNO", Order = 16)]
        [StringLength(50)]
        public string REFNO { get; set; } = "";

        [Column("LOAINGUONVON", Order = 17)]
        [StringLength(50)]
        public string LOAINGUONVON { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 18)]
        public int Id { get; set; }

        [Column("CREATED_DATE", Order = 19)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 20)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 21)]
        [StringLength(255)]
        public string FILE_NAME { get; set; } = "";
    }
}
