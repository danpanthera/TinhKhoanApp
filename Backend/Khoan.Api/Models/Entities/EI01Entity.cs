using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// EI01 Entity - E-Banking Information Table (UNIFIED STRUCTURE)
    /// Cấu trúc: NGAY_DL (datetime2) -> 24 Business Columns (all nvarchar(200)) -> System + Temporal Columns
    /// CSV Source: 7800_ei01_20241231.csv
    /// Business Columns: MA_CN,MA_KH,TEN_KH,LOAI_KH,SDT_EMB,TRANG_THAI_EMB,NGAY_DK_EMB,SDT_OTT,TRANG_THAI_OTT,NGAY_DK_OTT,SDT_SMS,TRANG_THAI_SMS,NGAY_DK_SMS,SDT_SAV,TRANG_THAI_SAV,NGAY_DK_SAV,SDT_LN,TRANG_THAI_LN,NGAY_DK_LN,USER_EMB,USER_OTT,USER_SMS,USER_SAV,USER_LN
    /// Import Policy: Only files containing "ei01" in filename
    /// Date Format: DATE/NGAY columns as datetime2 (dd/MM/yyyy format)
    /// All Business Columns: nvarchar(200) with NULL support
    /// </summary>
    [Table("EI01")]
    [Index(nameof(MA_CN), Name = "IX_EI01_MA_CN")]
    [Index(nameof(MA_KH), Name = "IX_EI01_MA_KH")]
    [Index(nameof(TEN_KH), Name = "IX_EI01_TEN_KH")]
    [Index(nameof(NGAY_DL), Name = "IX_EI01_NGAY_DL")]
    public class EI01Entity : ITemporalEntity
    {
        // === PRIMARY KEY ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // === NGAY_DL - System Column FIRST (lấy từ filename) ===
        /// <summary>
        /// Ngày dữ liệu - lấy từ filename format: 7800_ei01_YYYYMMDD.csv -> datetime2 (dd/mm/yyyy)
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // === 24 BUSINESS COLUMNS - Exact CSV structure, all nvarchar(200) ===

        /// <summary>
        /// Mã chi nhánh - Column 1
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MA_CN { get; set; }

        /// <summary>
        /// Mã khách hàng - Column 2
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng - Column 3
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TEN_KH { get; set; }

        /// <summary>
        /// Loại khách hàng - Column 4
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? LOAI_KH { get; set; }

        /// <summary>
        /// SĐT EMB - Column 5
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SDT_EMB { get; set; }

        /// <summary>
        /// Trạng thái EMB - Column 6
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRANG_THAI_EMB { get; set; }

        /// <summary>
        /// Ngày đăng ký EMB - Column 7 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_DK_EMB { get; set; }

        /// <summary>
        /// SĐT OTT - Column 8
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SDT_OTT { get; set; }

        /// <summary>
        /// Trạng thái OTT - Column 9
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRANG_THAI_OTT { get; set; }

        /// <summary>
        /// Ngày đăng ký OTT - Column 10 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_DK_OTT { get; set; }

        /// <summary>
        /// SĐT SMS - Column 11
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SDT_SMS { get; set; }

        /// <summary>
        /// Trạng thái SMS - Column 12
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRANG_THAI_SMS { get; set; }

        /// <summary>
        /// Ngày đăng ký SMS - Column 13 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_DK_SMS { get; set; }

        /// <summary>
        /// SĐT SAV - Column 14
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SDT_SAV { get; set; }

        /// <summary>
        /// Trạng thái SAV - Column 15
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRANG_THAI_SAV { get; set; }

        /// <summary>
        /// Ngày đăng ký SAV - Column 16 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_DK_SAV { get; set; }

        /// <summary>
        /// SĐT LN - Column 17
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SDT_LN { get; set; }

        /// <summary>
        /// Trạng thái LN - Column 18
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TRANG_THAI_LN { get; set; }

        /// <summary>
        /// Ngày đăng ký LN - Column 19 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_DK_LN { get; set; }

        /// <summary>
        /// User EMB - Column 20
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USER_EMB { get; set; }

        /// <summary>
        /// User OTT - Column 21
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USER_OTT { get; set; }

        /// <summary>
        /// User SMS - Column 22
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USER_SMS { get; set; }

        /// <summary>
        /// User SAV - Column 23
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USER_SAV { get; set; }

        /// <summary>
        /// User LN - Column 24
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? USER_LN { get; set; }

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
        [Column("ValidFrom", TypeName = "datetime2")]
        public DateTime SysStartTime { get; set; }

        /// <summary>
        /// Temporal table end time - System generated (ITemporalEntity interface)
        /// </summary>
        [Column("ValidTo", TypeName = "datetime2")]
        public DateTime SysEndTime { get; set; }
    }
}
