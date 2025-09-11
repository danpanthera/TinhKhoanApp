using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// GL41 Entity - General Ledger Balance Analytics (Heavy File Optimized)
    /// PARTITIONED COLUMNSTORE (NOT TEMPORAL) - Optimized for analytics performance
    /// Structure: NGAY_DL -> 13 Business Columns (CSV exact order) -> 4 System Columns
    /// Total: 17 columns, Import policy: Only files containing "gl41"
    /// Heavy File Config: MaxFileSize 2GB, BulkInsert BatchSize 10,000, Upload timeout 15 minutes
    /// </summary>
    [Table("GL41")]
    public class GL41Entity
    {
        // NGAY_DL - DateTime2 from filename gl41_YYYYMMDD (Position 0) - dd/mm/yyyy format
        [Required]
        [Column("NGAY_DL", Order = 0, TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // ===== 13 BUSINESS COLUMNS (CSV EXACT ORDER) =====

        [Required]
        [Column("MA_CN", Order = 1, TypeName = "nvarchar(200)")]
        public string MA_CN { get; set; } = string.Empty;

        [Required]
        [Column("LOAI_TIEN", Order = 2, TypeName = "nvarchar(200)")]
        public string LOAI_TIEN { get; set; } = string.Empty;

        [Required]
        [Column("MA_TK", Order = 3, TypeName = "nvarchar(200)")]
        public string MA_TK { get; set; } = string.Empty;

        [Column("TEN_TK", Order = 4, TypeName = "nvarchar(200)")]
        public string? TEN_TK { get; set; }

        [Column("LOAI_BT", Order = 5, TypeName = "nvarchar(200)")]
        public string? LOAI_BT { get; set; }

        // BALANCE/AMOUNT columns - decimal(18,2) format #,###.00
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

        // ===== 4 SYSTEM COLUMNS (NOT TEMPORAL) =====

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", Order = 14)]
        public long Id { get; set; }

        [Required]
        [Column("CREATED_DATE", Order = 15, TypeName = "datetime2")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public DateTime? UPDATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("FILE_NAME", Order = 17, TypeName = "nvarchar(500)")]
        public string? FILE_NAME { get; set; }
    }
}
