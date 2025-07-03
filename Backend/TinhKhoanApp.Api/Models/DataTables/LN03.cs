using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN03 - Dữ liệu nợ xấu
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "LN03"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("LN03")]
    public class LN03
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
        /// Nhóm nợ
        /// </summary>
        [Column("NHOM_NO")]
        [StringLength(10)]
        public string? NHOM_NO { get; set; }

        /// <summary>
        /// Dư nợ gốc
        /// </summary>
        [Column("DU_NO_GOC")]
        public decimal? DU_NO_GOC { get; set; }

        /// <summary>
        /// Dư nợ lãi
        /// </summary>
        [Column("DU_NO_LAI")]
        public decimal? DU_NO_LAI { get; set; }

        /// <summary>
        /// Số ngày quá hạn
        /// </summary>
        [Column("SO_NGAY_QUA_HAN")]
        public int? SO_NGAY_QUA_HAN { get; set; }

        /// <summary>
        /// Tỷ lệ dự phòng
        /// </summary>
        [Column("TY_LE_DU_PHONG")]
        public decimal? TY_LE_DU_PHONG { get; set; }

        /// <summary>
        /// Số tiền dự phòng
        /// </summary>
        [Column("SO_TIEN_DU_PHONG")]
        public decimal? SO_TIEN_DU_PHONG { get; set; }

        /// <summary>
        /// Ngày phân loại nợ
        /// </summary>
        [Column("NGAY_PHAN_LOAI_NO")]
        public DateTime? NGAY_PHAN_LOAI_NO { get; set; }

        /// <summary>
        /// Trạng thái nợ
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
