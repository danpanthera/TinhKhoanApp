using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN03 - 20 cột theo header_7808_ln03_20241231.csv
    /// MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON,R,S,T
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 20 CỘT THEO HEADER CSV GỐC (17 + 3 cột cuối R, S, T) ===
        [Column("MACHINHANH")]
        [StringLength(50)]
        public string? MACHINHANH { get; set; }

        [Column("TENCHINHANH")]
        [StringLength(255)]
        public string? TENCHINHANH { get; set; }

        [Column("MAKH")]
        [StringLength(50)]
        public string? MAKH { get; set; }

        [Column("TENKH")]
        [StringLength(255)]
        public string? TENKH { get; set; }

        [Column("SOHOPDONG")]
        [StringLength(50)]
        public string? SOHOPDONG { get; set; }

        [Column("SOTIENXLRR")]
        public decimal? SOTIENXLRR { get; set; }

        [Column("NGAYPHATSINHXL")]
        [StringLength(20)]
        public string? NGAYPHATSINHXL { get; set; }

        [Column("THUNOSAUXL")]
        public decimal? THUNOSAUXL { get; set; }

        [Column("CONLAINGOAIBANG")]
        public decimal? CONLAINGOAIBANG { get; set; }

        [Column("DUNONOIBANG")]
        public decimal? DUNONOIBANG { get; set; }

        [Column("NHOMNO")]
        [StringLength(20)]
        public string? NHOMNO { get; set; }

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
        [StringLength(50)]
        public string? TAIKHOANHACHTOAN { get; set; }

        [Column("REFNO")]
        [StringLength(50)]
        public string? REFNO { get; set; }

        [Column("LOAINGUONVON")]
        [StringLength(50)]
        public string? LOAINGUONVON { get; set; }

        // === 3 CỘT CUỐI (TRỐNG TIÊU ĐỀ NHƯNG CÓ DỮ LIỆU) ===
        [Column("R")]
        [StringLength(50)]
        public string? R { get; set; }

        [Column("S")]
        [StringLength(50)]
        public string? S { get; set; }

        [Column("T")]
        public decimal? T { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
