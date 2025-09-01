using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// DPDA Entity - Debit Card Table
    /// Cấu trúc: NGAY_DL -> 13 Business Columns -> System + Temporal Columns
    /// CSV Source: 7800_dpda_20250331.csv
    /// Business Columns: MA_CHI_NHANH,MA_KHACH_HANG,TEN_KHACH_HANG,SO_TAI_KHOAN,LOAI_THE,SO_THE,NGAY_NOP_DON,NGAY_PHAT_HANH,USER_PHAT_HANH,TRANG_THAI,PHAN_LOAI,GIAO_THE,LOAI_PHAT_HANH
    /// </summary>
    [Table("DPDA")]
    public class DPDAEntity : ITemporalEntity
    {
        // === PRIMARY KEY ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // === NGAY_DL - System Column (lấy từ filename) ===
        /// <summary>
        /// Ngày dữ liệu - lấy từ filename format: 7800_dpda_YYYYMMDD.csv -> datetime2 (dd/mm/yyyy)
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (13 columns theo CSV structure) ===

        /// <summary>
        /// Mã chi nhánh - Column 1
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string MA_CHI_NHANH { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - Column 2
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string MA_KHACH_HANG { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng - Column 3
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TEN_KHACH_HANG { get; set; }

        /// <summary>
        /// Số tài khoản - Column 4
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SO_TAI_KHOAN { get; set; }

        /// <summary>
        /// Loại thẻ - Column 5
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? LOAI_THE { get; set; }

        /// <summary>
        /// Số thẻ - Column 6
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SO_THE { get; set; }

        /// <summary>
        /// Ngày nộp đơn - Column 7 (DATE format dd/mm/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_NOP_DON { get; set; }

        /// <summary>
        /// Ngày phát hành - Column 8 (DATE format dd/mm/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_PHAT_HANH { get; set; }

        /// <summary>
        /// User phát hành - Column 9
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USER_PHAT_HANH { get; set; }

        /// <summary>
        /// Trạng thái - Column 10
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Phân loại - Column 11
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? PHAN_LOAI { get; set; }

        /// <summary>
        /// Giao thẻ - Column 12
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? GIAO_THE { get; set; }

        /// <summary>
        /// Loại phát hành - Column 13
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? LOAI_PHAT_HANH { get; set; }

        // === SYSTEM COLUMNS ===
        /// <summary>
        /// Created date - System column (IEntity interface)
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Updated date - System column (IEntity interface)
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // === TEMPORAL COLUMNS (cuối cùng - ITemporalEntity interface) ===
        /// <summary>
        /// Temporal table start time - System generated (ITemporalEntity interface)
        /// </summary>
        [Column("ValidFrom")]
        public DateTime SysStartTime { get; set; }

        /// <summary>
        /// Temporal table end time - System generated (ITemporalEntity interface)
        /// </summary>
        [Column("ValidTo")]
        public DateTime SysEndTime { get; set; }
    }
}
