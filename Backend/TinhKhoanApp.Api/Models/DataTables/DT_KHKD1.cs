using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng 7800_DT_KHKD1 - Dữ liệu doanh thu kế hoạch kinh doanh 1
    /// Lưu trữ dữ liệu từ các file Excel có tên "*DT_KHKD1*" (xls, xlsx)
    /// Dữ liệu được lấy từ dòng 13 trở đi, không cần dòng tiêu đề
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("7800_DT_KHKD1")]
    public class DT_KHKD1
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu theo định dạng dd/MM/yyyy
        /// Được parse từ tên file *yyyymmdd.xls(x)
        /// </summary>
        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column("BRCD")]
        [StringLength(20)]
        public string? BRCD { get; set; }

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        [Column("BRANCH_NAME")]
        [StringLength(200)]
        public string? BRANCH_NAME { get; set; }

        /// <summary>
        /// Loại chỉ tiêu
        /// </summary>
        [Column("INDICATOR_TYPE")]
        [StringLength(100)]
        public string? INDICATOR_TYPE { get; set; }

        /// <summary>
        /// Tên chỉ tiêu
        /// </summary>
        [Column("INDICATOR_NAME")]
        [StringLength(500)]
        public string? INDICATOR_NAME { get; set; }

        /// <summary>
        /// Kế hoạch năm
        /// </summary>
        [Column("PLAN_YEAR")]
        public decimal? PLAN_YEAR { get; set; }

        /// <summary>
        /// Kế hoạch quý
        /// </summary>
        [Column("PLAN_QUARTER")]
        public decimal? PLAN_QUARTER { get; set; }

        /// <summary>
        /// Kế hoạch tháng
        /// </summary>
        [Column("PLAN_MONTH")]
        public decimal? PLAN_MONTH { get; set; }

        /// <summary>
        /// Thực hiện năm
        /// </summary>
        [Column("ACTUAL_YEAR")]
        public decimal? ACTUAL_YEAR { get; set; }

        /// <summary>
        /// Thực hiện quý
        /// </summary>
        [Column("ACTUAL_QUARTER")]
        public decimal? ACTUAL_QUARTER { get; set; }

        /// <summary>
        /// Thực hiện tháng
        /// </summary>
        [Column("ACTUAL_MONTH")]
        public decimal? ACTUAL_MONTH { get; set; }

        /// <summary>
        /// Tỷ lệ hoàn thành
        /// </summary>
        [Column("ACHIEVEMENT_RATE")]
        public decimal? ACHIEVEMENT_RATE { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        [Column("YEAR")]
        public int? YEAR { get; set; }

        /// <summary>
        /// Quý
        /// </summary>
        [Column("QUARTER")]
        public int? QUARTER { get; set; }

        /// <summary>
        /// Tháng
        /// </summary>
        [Column("MONTH")]
        public int? MONTH { get; set; }

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
