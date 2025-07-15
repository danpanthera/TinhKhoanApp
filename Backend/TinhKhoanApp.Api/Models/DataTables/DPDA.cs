using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DPDA - 13 cột theo header_7800_dpda_20250430.csv
    /// MA_CHI_NHANH,MA_KHACH_HANG,TEN_KHACH_HANG,SO_TAI_KHOAN,LOAI_THE,SO_THE,NGAY_NOP_DON,NGAY_PHAT_HANH,USER_PHAT_HANH,TRANG_THAI,PHAN_LOAI,GIAO_THE,LOAI_PHAT_HANH
    /// </summary>
    [Table("DPDA")]
    public class DPDA
    {
        [Key]
        public int Id { get; set; }

        // === 13 CỘT BUSINESS DATA THEO CSV GỐC (Positions 2-14) ===
        [Column("MA_CHI_NHANH")]
        [StringLength(50)]
        public string? MA_CHI_NHANH { get; set; }

        [Column("MA_KHACH_HANG")]
        [StringLength(50)]
        public string? MA_KHACH_HANG { get; set; }

        [Column("TEN_KHACH_HANG")]
        [StringLength(255)]
        public string? TEN_KHACH_HANG { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(50)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("LOAI_THE")]
        [StringLength(50)]
        public string? LOAI_THE { get; set; }

        [Column("SO_THE")]
        [StringLength(50)]
        public string? SO_THE { get; set; }

        [Column("NGAY_NOP_DON")]
        [StringLength(20)]
        public string? NGAY_NOP_DON { get; set; }

        [Column("NGAY_PHAT_HANH")]
        [StringLength(20)]
        public string? NGAY_PHAT_HANH { get; set; }

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

        // === SYSTEM/TEMPORAL COLUMNS (Positions 15+) ===

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
