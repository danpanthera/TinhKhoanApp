using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DB01 - Dữ liệu tài sản đảm bảo
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "DB01"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("DB01")]
    public class DB01
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu theo định dạng dd/MM/yyyy
        /// Được parse từ tên file *yyyymmdd.csv
        /// VD: 7801_db01_20250430.csv -> 30/04/2025
        /// </summary>
        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        /// <summary>
        /// Mã phòng giao dịch
        /// </summary>
        [Column("MA_PGD")]
        [StringLength(20)]
        public string? MA_PGD { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        /// <summary>
        /// Số hợp đồng tài sản đảm bảo
        /// </summary>
        [Column("SO_HD_TSDB")]
        [StringLength(50)]
        public string? SO_HD_TSDB { get; set; }

        /// <summary>
        /// Loại tài sản đảm bảo
        /// </summary>
        [Column("LOAI_TSDB")]
        [StringLength(100)]
        public string? LOAI_TSDB { get; set; }

        /// <summary>
        /// Giá trị tài sản đảm bảo
        /// </summary>
        [Column("GIA_TRI_TSDB")]
        public decimal? GIA_TRI_TSDB { get; set; }

        /// <summary>
        /// Giá trị định giá
        /// </summary>
        [Column("GIA_TRI_DINH_GIA")]
        public decimal? GIA_TRI_DINH_GIA { get; set; }

        /// <summary>
        /// Ngày định giá
        /// </summary>
        [Column("NGAY_DINH_GIA")]
        public DateTime? NGAY_DINH_GIA { get; set; }

        /// <summary>
        /// Trạng thái tài sản đảm bảo
        /// </summary>
        [Column("TRANG_THAI")]
        [StringLength(50)]
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        [Column("UPDATED_DATE")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Tên file gốc được import
        /// </summary>
        [Column("FILE_NAME")]
        [StringLength(200)]
        public string? FileName { get; set; }

        // Temporal Tables columns sẽ được thêm bởi EF Core khi config
        // SysStartTime, SysEndTime sẽ được SQL Server tự quản lý
    }
}
