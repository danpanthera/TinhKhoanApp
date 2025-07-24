using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// DP01 - Deposit Data Model with exact CSV column structure (63 business columns)
    /// Structure: NGAY_DL -> 63 Business Columns (CSV order) -> System + Temporal Columns
    /// Import policy: Only files containing "dp01" in filename
    /// </summary>
    [Table("DP01")]
    public class DP01
    {
        // System Column - NGAY_DL first (Order 0) - extracted from filename
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // 63 Business Columns - Exact CSV order (basic string columns for now)
        [Column("MA_CN", Order = 1)]
        [StringLength(200)]
        public string MA_CN { get; set; } = "";

        [Column("TAI_KHOAN_HACH_TOAN", Order = 2)]
        [StringLength(200)]
        public string TAI_KHOAN_HACH_TOAN { get; set; } = "";

        [Column("MA_KH", Order = 3)]
        [StringLength(200)]
        public string MA_KH { get; set; } = "";

        [Column("TEN_KH", Order = 4)]
        [StringLength(200)]
        public string TEN_KH { get; set; } = "";

        [Column("DP_TYPE_NAME", Order = 5)]
        [StringLength(200)]
        public string DP_TYPE_NAME { get; set; } = "";

        [Column("CCY", Order = 6)]
        [StringLength(200)]
        public string CCY { get; set; } = "";

        [Column("CURRENT_BALANCE", Order = 7)]
        [StringLength(200)]
        public string CURRENT_BALANCE { get; set; } = "";

        [Column("RATE", Order = 8)]
        [StringLength(200)]
        public string RATE { get; set; } = "";

        [Column("SO_TAI_KHOAN", Order = 9)]
        [StringLength(200)]
        public string SO_TAI_KHOAN { get; set; } = "";

        [Column("OPENING_DATE", Order = 10)]
        [StringLength(200)]
        public string OPENING_DATE { get; set; } = "";

        // ... Simplified for compilation - will expand later
        // Adding remaining columns as string for now

        [Column("MATURITY_DATE", Order = 11)]
        [StringLength(200)]
        public string MATURITY_DATE { get; set; } = "";

        [Column("ADDRESS", Order = 12)]
        [StringLength(1000)]
        public string ADDRESS { get; set; } = "";

        // Note: Full 63 columns structure is in database
        // This model is simplified for compilation and testing
        // We'll use string properties to match the data uniformly

        // System/Temporal Columns - Always last
        [Key]
        [Column("Id", Order = 64)]
        public long Id { get; set; }

        [Column("FILE_NAME", Order = 67)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";
    }
}
