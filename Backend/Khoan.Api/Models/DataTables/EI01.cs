using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// EI01 - Electronic Banking Information Data Model (UNIFIED STRUCTURE)
    /// Structure: NGAY_DL (datetime2) -> 24 Business Columns -> System + Temporal Columns
    /// Import policy: Only files containing "ei01" in filename
    /// Business columns: All nvarchar(200), DATE/NGAY columns as datetime2 (dd/MM/yyyy)
    /// All fields support NULL values (business requirements)
    /// Direct Import: Business column names match CSV headers exactly (no transformation)
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        // === NGAY_DL - System Column FIRST (Order 0) - from filename as datetime2 ===
        [Column("NGAY_DL", TypeName = "datetime2", Order = 0)]
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === 24 BUSINESS COLUMNS - Exact CSV order, all nvarchar(200) + DATE as datetime2 ===

        [Column("MA_CN", TypeName = "nvarchar(200)", Order = 1)]
        public string? MA_CN { get; set; }

        [Column("MA_KH", TypeName = "nvarchar(200)", Order = 2)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH", TypeName = "nvarchar(200)", Order = 3)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH", TypeName = "nvarchar(200)", Order = 4)]
        public string? LOAI_KH { get; set; }

        [Column("SDT_EMB", TypeName = "nvarchar(200)", Order = 5)]
        public string? SDT_EMB { get; set; }

        [Column("TRANG_THAI_EMB", TypeName = "nvarchar(200)", Order = 6)]
        public string? TRANG_THAI_EMB { get; set; }

        // DATE column -> datetime2 (dd/MM/yyyy format)
        [Column("NGAY_DK_EMB", TypeName = "datetime2", Order = 7)]
        public DateTime? NGAY_DK_EMB { get; set; }

        [Column("SDT_OTT", TypeName = "nvarchar(200)", Order = 8)]
        public string? SDT_OTT { get; set; }

        [Column("TRANG_THAI_OTT", TypeName = "nvarchar(200)", Order = 9)]
        public string? TRANG_THAI_OTT { get; set; }

        // DATE column -> datetime2 (dd/MM/yyyy format)
        [Column("NGAY_DK_OTT", TypeName = "datetime2", Order = 10)]
        public DateTime? NGAY_DK_OTT { get; set; }

        [Column("SDT_SMS", TypeName = "nvarchar(200)", Order = 11)]
        public string? SDT_SMS { get; set; }

        [Column("TRANG_THAI_SMS", TypeName = "nvarchar(200)", Order = 12)]
        public string? TRANG_THAI_SMS { get; set; }

        // DATE column -> datetime2 (dd/MM/yyyy format)
        [Column("NGAY_DK_SMS", TypeName = "datetime2", Order = 13)]
        public DateTime? NGAY_DK_SMS { get; set; }

        [Column("SDT_SAV", TypeName = "nvarchar(200)", Order = 14)]
        public string? SDT_SAV { get; set; }

        [Column("TRANG_THAI_SAV", TypeName = "nvarchar(200)", Order = 15)]
        public string? TRANG_THAI_SAV { get; set; }

        // DATE column -> datetime2 (dd/MM/yyyy format)
        [Column("NGAY_DK_SAV", TypeName = "datetime2", Order = 16)]
        public DateTime? NGAY_DK_SAV { get; set; }

        [Column("SDT_LN", TypeName = "nvarchar(200)", Order = 17)]
        public string? SDT_LN { get; set; }

        [Column("TRANG_THAI_LN", TypeName = "nvarchar(200)", Order = 18)]
        public string? TRANG_THAI_LN { get; set; }

        // DATE column -> datetime2 (dd/MM/yyyy format)
        [Column("NGAY_DK_LN", TypeName = "datetime2", Order = 19)]
        public DateTime? NGAY_DK_LN { get; set; }

        [Column("USER_EMB", TypeName = "nvarchar(200)", Order = 20)]
        public string? USER_EMB { get; set; }

        [Column("USER_OTT", TypeName = "nvarchar(200)", Order = 21)]
        public string? USER_OTT { get; set; }

        [Column("USER_SMS", TypeName = "nvarchar(200)", Order = 22)]
        public string? USER_SMS { get; set; }

        [Column("USER_SAV", TypeName = "nvarchar(200)", Order = 23)]
        public string? USER_SAV { get; set; }

        [Column("USER_LN", TypeName = "nvarchar(200)", Order = 24)]
        public string? USER_LN { get; set; }

        // === SYSTEM/TEMPORAL COLUMNS - Always last ===
        [Key]
        [Column("Id", Order = 25)]
        public long Id { get; set; }

        [Column("CREATED_DATE", TypeName = "datetime2", Order = 26)]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", TypeName = "datetime2", Order = 27)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;

        // Temporal columns - System generated (ITemporalEntity interface)
        [Column("ValidFrom", TypeName = "datetime2", Order = 29)]
        public DateTime ValidFrom { get; set; }

        [Column("ValidTo", TypeName = "datetime2", Order = 30)]
        public DateTime ValidTo { get; set; }
    }
}
