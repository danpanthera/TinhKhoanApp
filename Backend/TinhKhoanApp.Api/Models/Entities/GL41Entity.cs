using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// GL41 Entity - General Ledger Balance Table
    /// 13 business columns + system columns
    /// CSV Source: GL41 balance data
    /// </summary>
    [Table("GL41")]
    [Index(nameof(MA_CN), Name = "IX_GL41_MA_CN")]
    [Index(nameof(LOAI_TIEN), Name = "IX_GL41_LOAI_TIEN")]
    [Index(nameof(MA_TK), Name = "IX_GL41_MA_TK")]
    [Index(nameof(LOAI_BT), Name = "IX_GL41_LOAI_BT")]
    public class GL41Entity : ITemporalEntity
    {
        // === SYSTEM COLUMNS (từ ITemporalEntity) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2(3)")]
        public DateTime? UpdatedAt { get; set; }

        // Temporal table support
        [Column(TypeName = "datetime2(3)")]
        public DateTime SysStartTime { get; set; }

        [Column(TypeName = "datetime2(3)")]
        public DateTime SysEndTime { get; set; }

        // === BUSINESS COLUMNS (13 columns theo CSV structure) ===

        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string MA_CN { get; set; } = string.Empty;

        /// <summary>
        /// Loại tiền - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string LOAI_TIEN { get; set; } = string.Empty;

        /// <summary>
        /// Mã tài khoản - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string MA_TK { get; set; } = string.Empty;

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Loại bút toán - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? LOAI_BT { get; set; }

        /// <summary>
        /// Dư nợ đầu kỳ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DN_DAUKY { get; set; }

        /// <summary>
        /// Dư có đầu kỳ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DC_DAUKY { get; set; }

        /// <summary>
        /// Số bút toán nợ trong kỳ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SBT_NO { get; set; }

        /// <summary>
        /// Số tiền ghi nợ trong kỳ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ST_GHINO { get; set; }

        /// <summary>
        /// Số bút toán có trong kỳ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SBT_CO { get; set; }

        /// <summary>
        /// Số tiền ghi có trong kỳ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ST_GHICO { get; set; }

        /// <summary>
        /// Dư nợ cuối kỳ - BUSINESS VALUE
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DN_CUOIKY { get; set; }

        /// <summary>
        /// Dư có cuối kỳ - BUSINESS VALUE
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DC_CUOIKY { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import
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
