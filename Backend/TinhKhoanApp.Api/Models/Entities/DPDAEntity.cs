using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// DPDA Entity - Debit Card Table
    /// 13 business columns + system columns
    /// CSV Source: 7800_dpda_20241231.csv
    /// </summary>
    [Table("DPDA")]
    [Index(nameof(MA_CHI_NHANH), Name = "IX_DPDA_MA_CHI_NHANH")]
    [Index(nameof(MA_KHACH_HANG), Name = "IX_DPDA_MA_KHACH_HANG")]
    [Index(nameof(SO_TAI_KHOAN), Name = "IX_DPDA_SO_TAI_KHOAN")]
    [Index(nameof(SO_THE), Name = "IX_DPDA_SO_THE")]
    public class DPDAEntity : ITemporalEntity
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

        // === BUSINESS COLUMNS (13 columns theo CSV structure) ===

        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string MA_CHI_NHANH { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string MA_KHACH_HANG { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? TEN_KHACH_HANG { get; set; }

        /// <summary>
        /// Số tài khoản - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string SO_TAI_KHOAN { get; set; } = string.Empty;

        /// <summary>
        /// Loại thẻ
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? LOAI_THE { get; set; }

        /// <summary>
        /// Số thẻ - BUSINESS KEY
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? SO_THE { get; set; }

        /// <summary>
        /// Ngày nộp đơn
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_NOP_DON { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_PHAT_HANH { get; set; }

        /// <summary>
        /// User phát hành
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? USER_PHAT_HANH { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Phân loại
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? PHAN_LOAI { get; set; }

        /// <summary>
        /// Giao thẻ
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? GIAO_THE { get; set; }

        /// <summary>
        /// Loại phát hành
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? LOAI_PHAT_HANH { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import (7800_dpda_20241231.csv)
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
