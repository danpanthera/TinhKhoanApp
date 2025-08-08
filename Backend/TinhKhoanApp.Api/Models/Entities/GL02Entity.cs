using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// GL02 Entity - General Ledger Summary Table
    /// 17 business columns + system columns
    /// CSV Source: 7800_gl02_20241231.csv
    /// </summary>
    [Table("GL02")]
    [Index(nameof(TRDATE), Name = "IX_GL02_TRDATE")]
    [Index(nameof(TRBRCD), Name = "IX_GL02_TRBRCD")]
    [Index(nameof(LOCAC), Name = "IX_GL02_LOCAC")]
    [Index(nameof(CCY), Name = "IX_GL02_CCY")]
    [Index(nameof(CUSTOMER), Name = "IX_GL02_CUSTOMER")]
    public class GL02Entity : ITemporalEntity
    {
        // === SYSTEM COLUMNS (từ ITemporalEntity) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2(3)")]
        public DateTime UpdatedAt { get; set; }

        // Temporal table support
        [Column(TypeName = "datetime2(3)")]
        public DateTime SysStartTime { get; set; }

        [Column(TypeName = "datetime2(3)")]
        public DateTime SysEndTime { get; set; }

        // === BUSINESS COLUMNS (17 columns theo CSV structure) ===

        /// <summary>
        /// Transaction date - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? TRDATE { get; set; }

        /// <summary>
        /// Transaction branch code - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string TRBRCD { get; set; } = string.Empty;

        /// <summary>
        /// User ID
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? USERID { get; set; }

        /// <summary>
        /// Journal sequence - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? JOURSEQ { get; set; }

        /// <summary>
        /// Daily transaction sequence - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? DYTRSEQ { get; set; }

        /// <summary>
        /// Local account - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string LOCAC { get; set; } = string.Empty;

        /// <summary>
        /// Currency - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string CCY { get; set; } = string.Empty;

        /// <summary>
        /// Business code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? BUSCD { get; set; }

        /// <summary>
        /// Unit
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? UNIT { get; set; }

        /// <summary>
        /// Transaction code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRCD { get; set; }

        /// <summary>
        /// Customer - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CUSTOMER { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRTP { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Remark/Note
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? REMARK { get; set; }

        /// <summary>
        /// Debit amount - BUSINESS VALUE
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DRAMOUNT { get; set; }

        /// <summary>
        /// Credit amount - BUSINESS VALUE
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CRAMOUNT { get; set; }

        /// <summary>
        /// Created datetime
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? CRTDTM { get; set; }

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
