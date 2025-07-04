using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL41 - Dữ liệu tài khoản kế toán
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "GL41"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("GL41")]
    public class GL41
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
        /// Số tài khoản
        /// </summary>
        [Column("SO_TK")]
        [StringLength(50)]
        public string? SO_TK { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        [Column("TEN_TK")]
        [StringLength(200)]
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Số dư tài khoản
        /// </summary>
        [Column("SO_DU")]
        public decimal? SO_DU { get; set; }

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

        // Temporal Tables columns sẽ được thêm bởi EF Core khi config
        // SysStartTime, SysEndTime sẽ được SQL Server tự quản lý
    }
}
