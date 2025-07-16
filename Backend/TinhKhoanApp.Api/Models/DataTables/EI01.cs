using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng EI01 - 24 cột theo header_7800_ei01_20250430.csv
    /// MA_CN,MA_KH,TEN_KH,LOAI_KH,SDT_EMB,TRANG_THAI_EMB,NGAY_DK_EMB,SDT_OTT,TRANG_THAI_OTT,NGAY_DK_OTT,SDT_SMS,TRANG_THAI_SMS,NGAY_DK_SMS,SDT_SAV,TRANG_THAI_SAV,NGAY_DK_SAV,SDT_LN,TRANG_THAI_LN,NGAY_DK_LN,USER_EMB,USER_OTT,USER_SMS,USER_SAV,USER_LN
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 24 CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("SDT_EMB")]
        [StringLength(20)]
        public string? SDT_EMB { get; set; }

        [Column("TRANG_THAI_EMB")]
        [StringLength(50)]
        public string? TRANG_THAI_EMB { get; set; }

        [Column("NGAY_DK_EMB")]
        [StringLength(20)]
        public string? NGAY_DK_EMB { get; set; }

        [Column("SDT_OTT")]
        [StringLength(20)]
        public string? SDT_OTT { get; set; }

        [Column("TRANG_THAI_OTT")]
        [StringLength(50)]
        public string? TRANG_THAI_OTT { get; set; }

        [Column("NGAY_DK_OTT")]
        [StringLength(20)]
        public string? NGAY_DK_OTT { get; set; }

        [Column("SDT_SMS")]
        [StringLength(20)]
        public string? SDT_SMS { get; set; }

        [Column("TRANG_THAI_SMS")]
        [StringLength(50)]
        public string? TRANG_THAI_SMS { get; set; }

        [Column("NGAY_DK_SMS")]
        [StringLength(20)]
        public string? NGAY_DK_SMS { get; set; }

        [Column("SDT_SAV")]
        [StringLength(20)]
        public string? SDT_SAV { get; set; }

        [Column("TRANG_THAI_SAV")]
        [StringLength(50)]
        public string? TRANG_THAI_SAV { get; set; }

        [Column("NGAY_DK_SAV")]
        [StringLength(20)]
        public string? NGAY_DK_SAV { get; set; }

        [Column("SDT_LN")]
        [StringLength(20)]
        public string? SDT_LN { get; set; }

        [Column("TRANG_THAI_LN")]
        [StringLength(50)]
        public string? TRANG_THAI_LN { get; set; }

        [Column("NGAY_DK_LN")]
        [StringLength(20)]
        public string? NGAY_DK_LN { get; set; }

        [Column("USER_EMB")]
        [StringLength(100)]
        public string? USER_EMB { get; set; }

        [Column("USER_OTT")]
        [StringLength(100)]
        public string? USER_OTT { get; set; }

        [Column("USER_SMS")]
        [StringLength(100)]
        public string? USER_SMS { get; set; }

        [Column("USER_SAV")]
        [StringLength(100)]
        public string? USER_SAV { get; set; }

        [Column("USER_LN")]
        [StringLength(100)]
        public string? USER_LN { get; set; }

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
