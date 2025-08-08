using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// EI01 Entity - E-Banking Information Table
    /// 24 business columns + system columns
    /// CSV Source: 7800_ei01_20241231.csv
    /// </summary>
    [Table("EI01")]
    [Index(nameof(MA_CN), Name = "IX_EI01_MA_CN")]
    [Index(nameof(MA_KH), Name = "IX_EI01_MA_KH")]
    [Index(nameof(LOAI_KH), Name = "IX_EI01_LOAI_KH")]
    [Index(nameof(SDT_EMB), Name = "IX_EI01_SDT_EMB")]
    [Index(nameof(TRANG_THAI_EMB), Name = "IX_EI01_TRANG_THAI_EMB")]
    public class EI01Entity : ITemporalEntity
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

        // === BUSINESS COLUMNS (24 columns theo CSV structure) ===

        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string MA_CN { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string MA_KH { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string TEN_KH { get; set; } = string.Empty;

        /// <summary>
        /// Loại khách hàng - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? LOAI_KH { get; set; }

        /// <summary>
        /// Số điện thoại EMB (E-Mobile Banking)
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        public string? SDT_EMB { get; set; }

        /// <summary>
        /// Trạng thái EMB - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRANG_THAI_EMB { get; set; }

        /// <summary>
        /// Ngày đăng ký EMB
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_DK_EMB { get; set; }

        /// <summary>
        /// Số điện thoại OTT (One Time Token)
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        public string? SDT_OTT { get; set; }

        /// <summary>
        /// Trạng thái OTT
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRANG_THAI_OTT { get; set; }

        /// <summary>
        /// Ngày đăng ký OTT
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_DK_OTT { get; set; }

        /// <summary>
        /// Số điện thoại SMS
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        public string? SDT_SMS { get; set; }

        /// <summary>
        /// Trạng thái SMS - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRANG_THAI_SMS { get; set; }

        /// <summary>
        /// Ngày đăng ký SMS
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_DK_SMS { get; set; }

        /// <summary>
        /// Số điện thoại SAV (Savings)
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        public string? SDT_SAV { get; set; }

        /// <summary>
        /// Trạng thái SAV
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRANG_THAI_SAV { get; set; }

        /// <summary>
        /// Ngày đăng ký SAV
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_DK_SAV { get; set; }

        /// <summary>
        /// Số điện thoại LN (Loan)
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        public string? SDT_LN { get; set; }

        /// <summary>
        /// Trạng thái LN
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRANG_THAI_LN { get; set; }

        /// <summary>
        /// Ngày đăng ký LN
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_DK_LN { get; set; }

        /// <summary>
        /// User EMB
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? USER_EMB { get; set; }

        /// <summary>
        /// User OTT
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? USER_OTT { get; set; }

        /// <summary>
        /// User SMS
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? USER_SMS { get; set; }

        /// <summary>
        /// User SAV
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? USER_SAV { get; set; }

        /// <summary>
        /// User LN
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? USER_LN { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import (7800_ei01_20241231.csv)
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
