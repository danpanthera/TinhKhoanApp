using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL01 - Dữ liệu sổ cái tổng hợp
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "GL01"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("GL01")]
    public class GL01
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
        /// Số dư đầu kỳ
        /// </summary>
        [Column("SO_DU_DAU_KY")]
        public decimal? SO_DU_DAU_KY { get; set; }

        /// <summary>
        /// Phát sinh nợ
        /// </summary>
        [Column("PHAT_SINH_NO")]
        public decimal? PHAT_SINH_NO { get; set; }

        /// <summary>
        /// Phát sinh có
        /// </summary>
        [Column("PHAT_SINH_CO")]
        public decimal? PHAT_SINH_CO { get; set; }

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
    }
}
