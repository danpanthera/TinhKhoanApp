using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN01 - Dữ liệu cho vay
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "LN01"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("LN01")]
    public class LN01
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
        /// Số hợp đồng cho vay
        /// </summary>
        [Column("SO_HD_CHO_VAY")]
        [StringLength(50)]
        public string? SO_HD_CHO_VAY { get; set; }

        /// <summary>
        /// Loại hình cho vay
        /// </summary>
        [Column("LOAI_HINH_CHO_VAY")]
        [StringLength(100)]
        public string? LOAI_HINH_CHO_VAY { get; set; }

        /// <summary>
        /// Số tiền cho vay
        /// </summary>
        [Column("SO_TIEN_CHO_VAY")]
        public decimal? SO_TIEN_CHO_VAY { get; set; }

        /// <summary>
        /// Dư nợ gốc
        /// </summary>
        [Column("DU_NO_GOC")]
        public decimal? DU_NO_GOC { get; set; }

        /// <summary>
        /// Lãi suất cho vay
        /// </summary>
        [Column("LAI_SUAT_CHO_VAY")]
        public decimal? LAI_SUAT_CHO_VAY { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        [Column("NGAY_GIAI_NGAN")]
        public DateTime? NGAY_GIAI_NGAN { get; set; }

        /// <summary>
        /// Ngày đến hạn
        /// </summary>
        [Column("NGAY_DEN_HAN")]
        public DateTime? NGAY_DEN_HAN { get; set; }

        /// <summary>
        /// Trạng thái khoản vay
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
