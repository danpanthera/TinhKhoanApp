using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DP01_New - Dữ liệu báo cáo tài chính theo ngày (New Structure)
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "DP01"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("DP01_New")]
    public class DP01
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
        /// Tài khoản hạch toán
        /// </summary>
        [Column("TAI_KHOAN_HACH_TOAN")]
        [StringLength(50)]
        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        /// <summary>
        /// Số dư hiện tại
        /// </summary>
        [Column("CURRENT_BALANCE")]
        public decimal? CURRENT_BALANCE { get; set; }

        /// <summary>
        /// Số dư đầu kỳ
        /// </summary>
        [Column("SO_DU_DAU_KY")]
        public decimal? SO_DU_DAU_KY { get; set; }

        /// <summary>
        /// Số phát sinh nợ
        /// </summary>
        [Column("SO_PHAT_SINH_NO")]
        public decimal? SO_PHAT_SINH_NO { get; set; }

        /// <summary>
        /// Số phát sinh có
        /// </summary>
        [Column("SO_PHAT_SINH_CO")]
        public decimal? SO_PHAT_SINH_CO { get; set; }

        /// <summary>
        /// Số dư cuối kỳ
        /// </summary>
        [Column("SO_DU_CUOI_KY")]
        public decimal? SO_DU_CUOI_KY { get; set; }

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
