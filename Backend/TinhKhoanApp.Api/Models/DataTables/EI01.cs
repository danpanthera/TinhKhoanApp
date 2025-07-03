using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng EI01 - Dữ liệu thu nhập khác
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "EI01"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("EI01")]
    public class EI01
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
        /// Mã tài khoản hạch toán
        /// </summary>
        [Column("MA_TK_HT")]
        [StringLength(20)]
        public string? MA_TK_HT { get; set; }

        /// <summary>
        /// Tên tài khoản hạch toán
        /// </summary>
        [Column("TEN_TK_HT")]
        [StringLength(200)]
        public string? TEN_TK_HT { get; set; }

        /// <summary>
        /// Loại thu nhập
        /// </summary>
        [Column("LOAI_THU_NHAP")]
        [StringLength(100)]
        public string? LOAI_THU_NHAP { get; set; }

        /// <summary>
        /// Số tiền thu nhập
        /// </summary>
        [Column("SO_TIEN_THU_NHAP")]
        public decimal? SO_TIEN_THU_NHAP { get; set; }

        /// <summary>
        /// Ngày phát sinh
        /// </summary>
        [Column("NGAY_PHAT_SINH")]
        public DateTime? NGAY_PHAT_SINH { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        [Column("DIEN_GIAI")]
        [StringLength(500)]
        public string? DIEN_GIAI { get; set; }

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
