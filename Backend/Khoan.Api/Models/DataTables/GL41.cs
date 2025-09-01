using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// GL41 - General Ledger Balance Analytics (DataTables Pattern - Standardized)
    /// PARTITIONED COLUMNSTORE (NOT TEMPORAL) - Optimized for analytics performance
    /// Structure: NGAY_DL -> 13 Business Columns (CSV exact order) -> 6 System Columns
    /// Total: 19 columns matching database exactly
    /// Import policy: Only files containing "gl41" in filename
    /// Heavy File Config: MaxFileSize 2GB, BulkInsert BatchSize 10,000, Upload timeout 15 minutes
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        // === NGAY_DL - DateTime2 from filename gl41_YYYYMMDD (Order 0) ===
        [Required]
        [Column("NGAY_DL", Order = 0, TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // === 13 BUSINESS COLUMNS (CSV EXACT ORDER) ===

        [Column("MA_CN", Order = 1, TypeName = "nvarchar(200)")]
        public string? MA_CN { get; set; }

        [Column("LOAI_TIEN", Order = 2, TypeName = "nvarchar(200)")]
        public string? LOAI_TIEN { get; set; }

        [Column("MA_TK", Order = 3, TypeName = "nvarchar(200)")]
        public string? MA_TK { get; set; }

        [Column("TEN_TK", Order = 4, TypeName = "nvarchar(200)")]
        public string? TEN_TK { get; set; }

        [Column("LOAI_BT", Order = 5, TypeName = "nvarchar(200)")]
        public string? LOAI_BT { get; set; }

        // BALANCE/AMOUNT columns - decimal(18,2) format
        [Column("DN_DAUKY", Order = 6, TypeName = "decimal(18,2)")]
        public decimal? DN_DAUKY { get; set; }

        [Column("DC_DAUKY", Order = 7, TypeName = "decimal(18,2)")]
        public decimal? DC_DAUKY { get; set; }

        [Column("SBT_NO", Order = 8, TypeName = "decimal(18,2)")]
        public decimal? SBT_NO { get; set; }

        [Column("ST_GHINO", Order = 9, TypeName = "decimal(18,2)")]
        public decimal? ST_GHINO { get; set; }

        [Column("SBT_CO", Order = 10, TypeName = "decimal(18,2)")]
        public decimal? SBT_CO { get; set; }

        [Column("ST_GHICO", Order = 11, TypeName = "decimal(18,2)")]
        public decimal? ST_GHICO { get; set; }

        [Column("DN_CUOIKY", Order = 12, TypeName = "decimal(18,2)")]
        public decimal? DN_CUOIKY { get; set; }

        [Column("DC_CUOIKY", Order = 13, TypeName = "decimal(18,2)")]
        public decimal? DC_CUOIKY { get; set; }

        // === 6 SYSTEM COLUMNS (matching database exactly) ===

        [Column("FILE_NAME", Order = 14, TypeName = "nvarchar(500)")]
        public string? FILE_NAME { get; set; }

        [Key]
        [Column("ID", Order = 15)]  // Note: Database uses "ID" not "Id"
        public long ID { get; set; }

        [Column("BATCH_ID", Order = 16, TypeName = "nvarchar(200)")]
        public string? BATCH_ID { get; set; }

        [Column("CREATED_DATE", Order = 17, TypeName = "datetime2")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("IMPORT_SESSION_ID", Order = 18, TypeName = "nvarchar(200)")]
        public string? IMPORT_SESSION_ID { get; set; }
    }
}
