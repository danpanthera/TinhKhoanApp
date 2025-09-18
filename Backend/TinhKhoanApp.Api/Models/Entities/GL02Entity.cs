using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// GL02 Entity - General Ledger Summary Table (Partitioned Columnstore)
    /// Structure: NGAY_DL -> 17 business columns -> System columns
    /// CSV Source: 7800_gl02_20241231.csv
    /// NGAY_DL derived from TRDATE column in CSV
    /// </summary>
    [Table("GL02")]
    [Index(nameof(NGAY_DL), Name = "IX_GL02_NGAY_DL")]
    [Index(nameof(TRBRCD), Name = "IX_GL02_TRBRCD")]
    [Index(nameof(LOCAC), Name = "IX_GL02_LOCAC")]
    [Index(nameof(CCY), Name = "IX_GL02_CCY")]
    [Index(nameof(CUSTOMER), Name = "IX_GL02_CUSTOMER")]
    public class GL02Entity
    {
        // === SYSTEM COLUMNS FIRST ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // === NGAY_DL - DERIVED FROM TRDATE ===
        /// <summary>
        /// Ngày dữ liệu - derived from TRDATE column in CSV
        /// Format: dd/mm/yyyy -> datetime2
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (17 columns theo CSV structure) ===

        /// <summary>
        /// Transaction date - BUSINESS COLUMN 1
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? TRDATE { get; set; }

        /// <summary>
        /// Transaction branch code - BUSINESS COLUMN 2
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string TRBRCD { get; set; } = string.Empty;

        /// <summary>
        /// User ID - BUSINESS COLUMN 3
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USERID { get; set; }

        /// <summary>
        /// Journal sequence - BUSINESS COLUMN 4
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? JOURSEQ { get; set; }

        /// <summary>
        /// Daily transaction sequence - BUSINESS COLUMN 5
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? DYTRSEQ { get; set; }

        /// <summary>
        /// Local account - BUSINESS COLUMN 6
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string LOCAC { get; set; } = string.Empty;

        /// <summary>
        /// Currency - BUSINESS COLUMN 7
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string CCY { get; set; } = string.Empty;

        /// <summary>
        /// Business code - BUSINESS COLUMN 8
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? BUSCD { get; set; }

        /// <summary>
        /// Unit - BUSINESS COLUMN 9
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? UNIT { get; set; }

        /// <summary>
        /// Transaction code - BUSINESS COLUMN 10
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRCD { get; set; }

        /// <summary>
        /// Customer - BUSINESS COLUMN 11
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? CUSTOMER { get; set; }

        /// <summary>
        /// Transaction type - BUSINESS COLUMN 12
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRTP { get; set; }

        /// <summary>
        /// Reference - BUSINESS COLUMN 13
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Remark/Note - BUSINESS COLUMN 14 (1000 char)
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string? REMARK { get; set; }

        /// <summary>
        /// Debit amount - BUSINESS COLUMN 15
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DRAMOUNT { get; set; }

        /// <summary>
        /// Credit amount - BUSINESS COLUMN 16
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CRAMOUNT { get; set; }

        /// <summary>
        /// Created datetime - BUSINESS COLUMN 17
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? CRTDTM { get; set; }

        // === SYSTEM COLUMNS ===

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import (7800_gl02_20241231.csv)
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? FileName { get; set; }

        /// <summary>
        /// Import batch ID để tracking
        /// </summary>
        [Column(TypeName = "uniqueidentifier")]
        public Guid? ImportId { get; set; }

        /// <summary>
        /// Additional metadata về import process
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string? ImportMetadata { get; set; }
    }
}
