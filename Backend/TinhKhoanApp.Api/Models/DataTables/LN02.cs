using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN02 - Dữ liệu cho vay chi tiết
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "LN02"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("LN02")]
    public class LN02
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
        /// Kỳ hạn thanh toán
        /// </summary>
        [Column("KY_HAN_THANH_TOAN")]
        public int? KY_HAN_THANH_TOAN { get; set; }

        /// <summary>
        /// Số tiền gốc phải trả
        /// </summary>
        [Column("SO_TIEN_GOC_PHAI_TRA")]
        public decimal? SO_TIEN_GOC_PHAI_TRA { get; set; }

        /// <summary>
        /// Số tiền lãi phải trả
        /// </summary>
        [Column("SO_TIEN_LAI_PHAI_TRA")]
        public decimal? SO_TIEN_LAI_PHAI_TRA { get; set; }

        /// <summary>
        /// Ngày đến hạn trả
        /// </summary>
        [Column("NGAY_DEN_HAN_TRA")]
        public DateTime? NGAY_DEN_HAN_TRA { get; set; }

        /// <summary>
        /// Ngày trả thực tế
        /// </summary>
        [Column("NGAY_TRA_THUC_TE")]
        public DateTime? NGAY_TRA_THUC_TE { get; set; }

        /// <summary>
        /// Số tiền đã trả
        /// </summary>
        [Column("SO_TIEN_DA_TRA")]
        public decimal? SO_TIEN_DA_TRA { get; set; }

        /// <summary>
        /// Trạng thái thanh toán
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
