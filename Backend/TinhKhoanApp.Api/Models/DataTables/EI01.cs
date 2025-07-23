using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// EI01 - Electronic Banking Information Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        // System Column - NGAY_DL first (extracted from filename)
        [Column("NGAY_DL", Order = 0)]
        [StringLength(10)]
        public string NGAY_DL { get; set; } = "";

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("MA_CN", Order = 1)]
        [StringLength(50)]
        public string MA_CN { get; set; } = "";

        [Column("MA_KH", Order = 2)]
        [StringLength(50)]
        public string MA_KH { get; set; } = "";

        [Column("TEN_KH", Order = 3)]
        [StringLength(50)]
        public string TEN_KH { get; set; } = "";

        [Column("LOAI_KH", Order = 4)]
        [StringLength(50)]
        public string LOAI_KH { get; set; } = "";

        [Column("SDT_EMB", Order = 5)]
        [StringLength(50)]
        public string SDT_EMB { get; set; } = "";

        [Column("TRANG_THAI_EMB", Order = 6)]
        [StringLength(50)]
        public string TRANG_THAI_EMB { get; set; } = "";

        [Column("NGAY_DK_EMB", Order = 7)]
        [StringLength(50)]
        public string NGAY_DK_EMB { get; set; } = "";

        [Column("SDT_OTT", Order = 8)]
        [StringLength(50)]
        public string SDT_OTT { get; set; } = "";

        [Column("TRANG_THAI_OTT", Order = 9)]
        [StringLength(50)]
        public string TRANG_THAI_OTT { get; set; } = "";

        [Column("NGAY_DK_OTT", Order = 10)]
        [StringLength(50)]
        public string NGAY_DK_OTT { get; set; } = "";

        [Column("SDT_SMS", Order = 11)]
        [StringLength(50)]
        public string SDT_SMS { get; set; } = "";

        [Column("TRANG_THAI_SMS", Order = 12)]
        [StringLength(50)]
        public string TRANG_THAI_SMS { get; set; } = "";

        [Column("NGAY_DK_SMS", Order = 13)]
        [StringLength(50)]
        public string NGAY_DK_SMS { get; set; } = "";

        [Column("SDT_SAV", Order = 14)]
        [StringLength(50)]
        public string SDT_SAV { get; set; } = "";

        [Column("TRANG_THAI_SAV", Order = 15)]
        [StringLength(50)]
        public string TRANG_THAI_SAV { get; set; } = "";

        [Column("NGAY_DK_SAV", Order = 16)]
        [StringLength(50)]
        public string NGAY_DK_SAV { get; set; } = "";

        [Column("SDT_LN", Order = 17)]
        [StringLength(50)]
        public string SDT_LN { get; set; } = "";

        [Column("TRANG_THAI_LN", Order = 18)]
        [StringLength(50)]
        public string TRANG_THAI_LN { get; set; } = "";

        [Column("NGAY_DK_LN", Order = 19)]
        [StringLength(50)]
        public string NGAY_DK_LN { get; set; } = "";

        [Column("USER_EMB", Order = 20)]
        [StringLength(50)]
        public string USER_EMB { get; set; } = "";

        [Column("USER_OTT", Order = 21)]
        [StringLength(50)]
        public string USER_OTT { get; set; } = "";

        [Column("USER_SMS", Order = 22)]
        [StringLength(50)]
        public string USER_SMS { get; set; } = "";

        [Column("USER_SAV", Order = 23)]
        [StringLength(50)]
        public string USER_SAV { get; set; } = "";

        [Column("USER_LN", Order = 24)]
        [StringLength(50)]
        public string USER_LN { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 25)]
        public int Id { get; set; }

        [Column("CREATED_DATE", Order = 26)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 27)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 28)]
        [StringLength(255)]
        public string FILE_NAME { get; set; } = "";
    }
}
