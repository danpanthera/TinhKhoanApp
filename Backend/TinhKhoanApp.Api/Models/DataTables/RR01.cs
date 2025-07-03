using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng RR01 - Dữ liệu tỷ lệ
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "RR01"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("RR01")]
    public class RR01
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
        /// Loại tỷ lệ
        /// </summary>
        [Column("LOAI_TY_LE")]
        [StringLength(100)]
        public string? LOAI_TY_LE { get; set; }

        /// <summary>
        /// Tên chỉ tiêu
        /// </summary>
        [Column("TEN_CHI_TIEU")]
        [StringLength(200)]
        public string? TEN_CHI_TIEU { get; set; }

        /// <summary>
        /// Giá trị tỷ lệ
        /// </summary>
        [Column("GIA_TRI_TY_LE")]
        public decimal? GIA_TRI_TY_LE { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        [Column("DON_VI_TINH")]
        [StringLength(20)]
        public string? DON_VI_TINH { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        [Column("NGAY_AP_DUNG")]
        public DateTime? NGAY_AP_DUNG { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Column("GHI_CHU")]
        [StringLength(500)]
        public string? GHI_CHU { get; set; }

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
