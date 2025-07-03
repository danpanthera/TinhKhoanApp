using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DPDA - Dữ liệu tiền gửi của dân
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "DPDA"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("DPDA")]
    public class DPDA
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
        /// Số tài khoản tiền gửi
        /// </summary>
        [Column("SO_TK_TIEN_GUI")]
        [StringLength(50)]
        public string? SO_TK_TIEN_GUI { get; set; }

        /// <summary>
        /// Loại tiền gửi
        /// </summary>
        [Column("LOAI_TIEN_GUI")]
        [StringLength(100)]
        public string? LOAI_TIEN_GUI { get; set; }

        /// <summary>
        /// Số dư tiền gửi
        /// </summary>
        [Column("SO_DU_TIEN_GUI")]
        public decimal? SO_DU_TIEN_GUI { get; set; }

        /// <summary>
        /// Lãi suất tiền gửi
        /// </summary>
        [Column("LAI_SUAT")]
        public decimal? LAI_SUAT { get; set; }

        /// <summary>
        /// Ngày mở tài khoản
        /// </summary>
        [Column("NGAY_MO_TK")]
        public DateTime? NGAY_MO_TK { get; set; }

        /// <summary>
        /// Ngày đến hạn
        /// </summary>
        [Column("NGAY_DEN_HAN")]
        public DateTime? NGAY_DEN_HAN { get; set; }

        /// <summary>
        /// Trạng thái tài khoản
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
