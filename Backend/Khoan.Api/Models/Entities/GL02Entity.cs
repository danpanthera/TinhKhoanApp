using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// GL02 Entity - General Ledger Transactions (Heavy File Optimized)
    /// PARTITIONED COLUMNSTORE (NOT TEMPORAL) - Optimized for ~200MB CSV files
    /// Structure: NGAY_DL -> 17 Business Columns (CSV exact order) -> 4 System Columns
    /// Total: 21 columns, Import policy: Only files containing "gl02"
    /// Heavy File Config: MaxFileSize 2GB, BulkInsert BatchSize 10,000, Upload timeout 15 minutes
    /// </summary>
    [Table("GL02")]
    public class GL02Entity
    {
        // NGAY_DL - DateTime2 from TRDATE column (Position 0) - dd/mm/yyyy format
        [Required]
        [Column("NGAY_DL", Order = 0, TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // ===== 17 BUSINESS COLUMNS (CSV EXACT ORDER) =====

        [Required]
        [Column("TRBRCD", Order = 1, TypeName = "nvarchar(200)")]
        public string TRBRCD { get; set; } = string.Empty;

        [Column("USERID", Order = 2, TypeName = "nvarchar(200)")]
        public string? USERID { get; set; }

        [Column("JOURSEQ", Order = 3, TypeName = "nvarchar(200)")]
        public string? JOURSEQ { get; set; }

        [Column("DYTRSEQ", Order = 4, TypeName = "nvarchar(200)")]
        public string? DYTRSEQ { get; set; }

        [Required]
        [Column("LOCAC", Order = 5, TypeName = "nvarchar(200)")]
        public string LOCAC { get; set; } = string.Empty;

        [Required]
        [Column("CCY", Order = 6, TypeName = "nvarchar(200)")]
        public string CCY { get; set; } = string.Empty;

        [Column("BUSCD", Order = 7, TypeName = "nvarchar(200)")]
        public string? BUSCD { get; set; }

        [Column("UNIT", Order = 8, TypeName = "nvarchar(200)")]
        public string? UNIT { get; set; }

        [Column("TRCD", Order = 9, TypeName = "nvarchar(200)")]
        public string? TRCD { get; set; }

        [Column("CUSTOMER", Order = 10, TypeName = "nvarchar(200)")]
        public string? CUSTOMER { get; set; }

        [Column("TRTP", Order = 11, TypeName = "nvarchar(200)")]
        public string? TRTP { get; set; }

        [Column("REFERENCE", Order = 12, TypeName = "nvarchar(200)")]
        public string? REFERENCE { get; set; }

        [Column("REMARK", Order = 13, TypeName = "nvarchar(1000)")]
        public string? REMARK { get; set; }

        // AMOUNT columns - decimal(18,2) format #,###.00
        [Column("DRAMOUNT", Order = 14, TypeName = "decimal(18,2)")]
        public decimal? DRAMOUNT { get; set; }

        [Column("CRAMOUNT", Order = 15, TypeName = "decimal(18,2)")]
        public decimal? CRAMOUNT { get; set; }

        // CRTDTM - datetime2 format dd/mm/yyyy hh:mm:ss
        [Column("CRTDTM", Order = 16, TypeName = "datetime2")]
        public DateTime? CRTDTM { get; set; }

        // ===== 4 SYSTEM COLUMNS (NOT TEMPORAL) =====

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", Order = 17)]
        public long Id { get; set; }

        [Required]
        [Column("CREATED_DATE", Order = 18, TypeName = "datetime2")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", Order = 19, TypeName = "datetime2")]
        public DateTime? UPDATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("FILE_NAME", Order = 20, TypeName = "nvarchar(500)")]
        public string? FILE_NAME { get; set; }
    }
}
