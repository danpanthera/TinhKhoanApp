using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// GL01 Entity - General Ledger Detail Table
    /// 27 business columns + system columns
    /// CSV Source: 7800_gl01_20241231.csv
    /// </summary>
    [Table("GL01")]
    [Index(nameof(NGAY_GD), Name = "IX_GL01_NGAY_GD")]
    [Index(nameof(TAI_KHOAN), Name = "IX_GL01_TAI_KHOAN")]
    [Index(nameof(MA_KH), Name = "IX_GL01_MA_KH")]
    [Index(nameof(DYSEQ), Name = "IX_GL01_DYSEQ")]
    [Index(nameof(DR_CR), Name = "IX_GL01_DR_CR")]
    public class GL01Entity
    {
        // === SYSTEM COLUMNS (GL01 là NON-TEMPORAL) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2(3)")]
        public DateTime UpdatedAt { get; set; }

        // === NGAY_DL (lấy từ TR_TIME cột 25) ===
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // === FILE_NAME (track source file) ===
        [Column(TypeName = "nvarchar(255)")]
        public string? FILE_NAME { get; set; }

        // === BUSINESS COLUMNS (27 columns theo CSV structure) ===

        /// <summary>
        /// Status - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string STS { get; set; } = string.Empty;

        /// <summary>
        /// Ngày giao dịch - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_GD { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? NGUOI_TAO { get; set; }

        /// <summary>
        /// Daily sequence - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? DYSEQ { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? TR_TYPE { get; set; }

        /// <summary>
        /// Detail sequence
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? DT_SEQ { get; set; }

        /// <summary>
        /// Tài khoản - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string TAI_KHOAN { get; set; } = string.Empty;

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Số tiền giao dịch
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GD { get; set; }

        /// <summary>
        /// Post branch
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? POST_BR { get; set; }

        /// <summary>
        /// Loại tiền
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Debit/Credit - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(2)")]
        public string DR_CR { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? TEN_KH { get; set; }

        /// <summary>
        /// CCA User ID
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CCA_USRID { get; set; }

        /// <summary>
        /// Transaction exchange rate
        /// </summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal? TR_EX_RT { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? REMARK { get; set; }

        /// <summary>
        /// Business code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? BUS_CODE { get; set; }

        /// <summary>
        /// Unit business code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? UNIT_BUS_CODE { get; set; }

        /// <summary>
        /// Transaction code
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? TR_CODE { get; set; }

        /// <summary>
        /// Transaction name
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? TR_NAME { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Value date
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// Department code
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? DEPT_CODE { get; set; }

        /// <summary>
        /// Transaction time
        /// </summary>
        [Column(TypeName = "time")]
        public TimeSpan? TR_TIME { get; set; }

        /// <summary>
        /// Confirm
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? COMFIRM { get; set; }

        /// <summary>
        /// Transaction datetime
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? TRDT_TIME { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import (7800_gl01_20241231.csv)
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
