using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Bảng DP01 - Dữ liệu báo cáo tài chính theo ngày
    /// </summary>
    [Table("DP01")]
    public class DP01
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ngày báo cáo dữ liệu
        /// </summary>
        [Column("DATA_DATE")]
        public DateTime DATA_DATE { get; set; }

        /// <summary>
        /// Mã chi nhánh (7800: Hội sở, 7801-7808: Chi nhánh)
        /// </summary>
        [Column("MA_CN")]
        [StringLength(10)]
        public string MA_CN { get; set; }

        /// <summary>
        /// Mã phòng giao dịch (01, 02...)
        /// </summary>
        [Column("MA_PGD")]
        [StringLength(10)]
        public string MA_PGD { get; set; }

        /// <summary>
        /// Tài khoản hạch toán (lọc bỏ 40*, 41*, 427*, 211108)
        /// </summary>
        [Column("TAI_KHOAN_HACH_TOAN")]
        [StringLength(20)]
        public string TAI_KHOAN_HACH_TOAN { get; set; }

        /// <summary>
        /// Số dư hiện tại của tài khoản
        /// </summary>
        [Column("CURRENT_BALANCE")]
        public decimal? CURRENT_BALANCE { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Column("CREATED_DATE")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        [Column("UPDATED_DATE")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Tên file CSV được import (VD: 7801_dp01_20250430.csv)
        /// </summary>
        [Column("FILE_NAME")]
        [StringLength(100)]
        public string? FileName { get; set; }

        /// <summary>
        /// Tên tài khoản hạch toán
        /// </summary>
        [Column("TEN_TAI_KHOAN")]
        [StringLength(500)]
        public string? TEN_TAI_KHOAN { get; set; }

        /// <summary>
        /// Ngày dữ liệu theo định dạng dd/mm/yyyy (VD: 30/04/2025)
        /// Được lấy từ tên file *yyyymmdd.csv và chuyển đổi thành dd/mm/yyyy
        /// </summary>
        [Column("NGAY_DL")]
        [StringLength(10)]
        public string? NgayDL { get; set; }
    }
}
