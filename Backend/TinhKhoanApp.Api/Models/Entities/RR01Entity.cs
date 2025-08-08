using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// RR01 Entity - Risk Report Table
    /// Temporal table với 25 business columns + system columns
    /// CSV Source: 7800_rr01_20241231.csv
    /// </summary>
    [Table("RR01")]
    [Index(nameof(NGAY_DL), Name = "IX_RR01_NGAY_DL")]
    [Index(nameof(BRCD), Name = "IX_RR01_BRCD")]
    [Index(nameof(MA_KH), Name = "IX_RR01_MA_KH")]
    [Index(nameof(NGAY_DL), nameof(BRCD), Name = "IX_RR01_NGAY_DL_BRCD")]
    public class RR01Entity : ITemporalEntity
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

        // === BUSINESS COLUMNS (25 columns theo CSV structure) ===

        /// <summary>
        /// Ngày dữ liệu - Business primary key
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Loại chi nhánh I
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? CN_LOAI_I { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? BRCD { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TEN_KH { get; set; }

        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SO_LDS { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? CCY { get; set; }

        /// <summary>
        /// Số LAV
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SO_LAV { get; set; }

        /// <summary>
        /// Loại khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? LOAI_KH { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_GIAI_NGAN { get; set; }

        /// <summary>
        /// Ngày đến hạn
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_DEN_HAN { get; set; }

        /// <summary>
        /// VAMC Flag
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? VAMC_FLG { get; set; }

        /// <summary>
        /// Ngày xử lý rủi ro
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_XLRR { get; set; }

        /// <summary>
        /// Dư nợ gốc ban đầu
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_GOC_BAN_DAU { get; set; }

        /// <summary>
        /// Dư nợ lãi tích lũy ban đầu
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

        /// <summary>
        /// Dư nợ đầu kỳ đã thu hiện tại
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

        /// <summary>
        /// Dư nợ gốc hiện tại
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_GOC_HIENTAI { get; set; }

        /// <summary>
        /// Dư nợ lãi hiện tại
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_LAI_HIENTAI { get; set; }

        /// <summary>
        /// Dư nợ ngắn hạn
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_NGAN_HAN { get; set; }

        /// <summary>
        /// Dư nợ trung hạn
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_TRUNG_HAN { get; set; }

        /// <summary>
        /// Dư nợ dài hạn
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNO_DAI_HAN { get; set; }

        /// <summary>
        /// Thu gốc
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? THU_GOC { get; set; }

        /// <summary>
        /// Thu lãi
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? THU_LAI { get; set; }

        /// <summary>
        /// Bất động sản
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BDS { get; set; }

        /// <summary>
        /// Động sản
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DS { get; set; }

        /// <summary>
        /// Tài sản khác
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TSK { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import (7800_rr01_20241231.csv)
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
