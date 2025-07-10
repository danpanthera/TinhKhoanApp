using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN02 - 11 cột theo header_7800_ln02_20250430.csv
    /// TENCHINHANH,MAKHACHHANG,TENKHACHHANG,NGAYCHUYENNHOMNO,DUNO,NHOMNOMOI,NHOMNOCU,NGUYENNHAN,MACANBO,TENCANBO,MANGANH
    /// </summary>
    [Table("LN02")]
    public class LN02
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 11 CỘT THEO HEADER CSV GỐC ===
        [Column("TENCHINHANH")]
        [StringLength(255)]
        public string? TENCHINHANH { get; set; }

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        public string? MAKHACHHANG { get; set; }

        [Column("TENKHACHHANG")]
        [StringLength(255)]
        public string? TENKHACHHANG { get; set; }

        [Column("NGAYCHUYENNHOMNO")]
        [StringLength(20)]
        public string? NGAYCHUYENNHOMNO { get; set; }

        [Column("DUNO")]
        public decimal? DUNO { get; set; }

        [Column("NHOMNOMOI")]
        [StringLength(20)]
        public string? NHOMNOMOI { get; set; }

        [Column("NHOMNOCU")]
        [StringLength(20)]
        public string? NHOMNOCU { get; set; }

        [Column("NGUYENNHAN")]
        [StringLength(500)]
        public string? NGUYENNHAN { get; set; }

        [Column("MACANBO")]
        [StringLength(50)]
        public string? MACANBO { get; set; }

        [Column("TENCANBO")]
        [StringLength(255)]
        public string? TENCANBO { get; set; }

        [Column("MANGANH")]
        [StringLength(50)]
        public string? MANGANH { get; set; }

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
