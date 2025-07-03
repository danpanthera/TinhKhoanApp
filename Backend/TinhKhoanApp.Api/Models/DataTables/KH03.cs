using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng KH03 - Dữ liệu khách hàng
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "KH03"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("KH03")]
    public class KH03
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu theo định dạng dd/MM/yyyy
        /// Được parse từ tên file *yyyymmdd.csv
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
        /// Tên khách hàng
        /// </summary>
        [Column("TEN_KH")]
        [StringLength(200)]
        public string? TEN_KH { get; set; }

        /// <summary>
        /// Loại khách hàng
        /// </summary>
        [Column("LOAI_KH")]
        [StringLength(100)]
        public string? LOAI_KH { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        [Column("SO_CMND")]
        [StringLength(20)]
        public string? SO_CMND { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [Column("DIA_CHI")]
        [StringLength(500)]
        public string? DIA_CHI { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [Column("SO_DIEN_THOAI")]
        [StringLength(20)]
        public string? SO_DIEN_THOAI { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Column("EMAIL")]
        [StringLength(100)]
        public string? EMAIL { get; set; }

        /// <summary>
        /// Ngày mở tài khoản
        /// </summary>
        [Column("NGAY_MO_TK")]
        public DateTime? NGAY_MO_TK { get; set; }

        /// <summary>
        /// Trạng thái khách hàng
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
    }
}
