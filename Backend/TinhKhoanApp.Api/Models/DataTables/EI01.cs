using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// EI01 - Electronic Banking Information Data Model with Temporal Tables + Columnstore
    /// Structure: NGAY_DL -> 24 Business Columns (CSV order) -> System + Temporal Columns
    /// Import policy: Only files containing "ei01" in filename
    /// Nullable: All fields support NULL values
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        // System Column - NGAY_DL first (Order 0) - extracted from filename
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // 24 Business Columns - Exact CSV order with proper data types and NULL support
        [Column("MA_CN", Order = 1)]
        [StringLength(200)]
        public string? MA_CN { get; set; }

        [Column("MA_KH", Order = 2)]
        [StringLength(200)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH", Order = 3)]
        [StringLength(200)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH", Order = 4)]
        [StringLength(200)]
        public string? LOAI_KH { get; set; }

        [Column("SDT_EMB", Order = 5)]
        [StringLength(200)]
        public string? SDT_EMB { get; set; }

        [Column("TRANG_THAI_EMB", Order = 6)]
        [StringLength(200)]
        public string? TRANG_THAI_EMB { get; set; }

        [Column("NGAY_DK_EMB", Order = 7)]
        public DateTime? NGAY_DK_EMB { get; set; }

        [Column("SDT_OTT", Order = 8)]
        [StringLength(200)]
        public string? SDT_OTT { get; set; }

        [Column("TRANG_THAI_OTT", Order = 9)]
        [StringLength(200)]
        public string? TRANG_THAI_OTT { get; set; }

        [Column("NGAY_DK_OTT", Order = 10)]
        public DateTime? NGAY_DK_OTT { get; set; }

        [Column("SDT_SMS", Order = 11)]
        [StringLength(200)]
        public string? SDT_SMS { get; set; }

        [Column("TRANG_THAI_SMS", Order = 12)]
        [StringLength(200)]
        public string? TRANG_THAI_SMS { get; set; }

        [Column("NGAY_DK_SMS", Order = 13)]
        public DateTime? NGAY_DK_SMS { get; set; }

        [Column("SDT_SAV", Order = 14)]
        [StringLength(200)]
        public string? SDT_SAV { get; set; }

        [Column("TRANG_THAI_SAV", Order = 15)]
        [StringLength(200)]
        public string? TRANG_THAI_SAV { get; set; }

        [Column("NGAY_DK_SAV", Order = 16)]
        public DateTime? NGAY_DK_SAV { get; set; }

        [Column("SDT_LN", Order = 17)]
        [StringLength(200)]
        public string? SDT_LN { get; set; }

        [Column("TRANG_THAI_LN", Order = 18)]
        [StringLength(200)]
        public string? TRANG_THAI_LN { get; set; }

        [Column("NGAY_DK_LN", Order = 19)]
        public DateTime? NGAY_DK_LN { get; set; }

        [Column("USER_EMB", Order = 20)]
        [StringLength(200)]
        public string? USER_EMB { get; set; }

        [Column("USER_OTT", Order = 21)]
        [StringLength(200)]
        public string? USER_OTT { get; set; }

        [Column("USER_SMS", Order = 22)]
        [StringLength(200)]
        public string? USER_SMS { get; set; }

        [Column("USER_SAV", Order = 23)]
        [StringLength(200)]
        public string? USER_SAV { get; set; }

        [Column("USER_LN", Order = 24)]
        [StringLength(200)]
        public string? USER_LN { get; set; }

        // Temporal/System Columns - Always last
        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 25)]
        public long Id { get; set; }

        [Column("CREATED_DATE", Order = 26)]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", Order = 27)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("FILE_NAME", Order = 28)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";

        // Temporal columns are shadow properties managed by EF Core automatically
        // ValidFrom/ValidTo removed - managed as shadow properties by ApplicationDbContext
    }
}
