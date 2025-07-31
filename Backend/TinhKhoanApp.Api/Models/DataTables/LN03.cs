using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// LN03 - Loan Recovery Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        // System Column - NGAY_DL first (extracted from filename)
        /// <summary>
        /// Ngày dữ liệu (trích xuất từ tên file)
        /// </summary>
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column("MACHINHANH", Order = 1)]
        [StringLength(200)]
        public string MACHINHANH { get; set; } = "";

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        [Column("TENCHINHANH", Order = 2)]
        [StringLength(200)]
        public string TENCHINHANH { get; set; } = "";

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [Column("MAKH", Order = 3)]
        [StringLength(200)]
        public string MAKH { get; set; } = "";

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column("TENKH", Order = 4)]
        [StringLength(200)]
        public string TENKH { get; set; } = "";

        /// <summary>
        /// Số hợp đồng
        /// </summary>
        [Column("SOHOPDONG", Order = 5)]
        [StringLength(200)]
        public string SOHOPDONG { get; set; } = "";

        /// <summary>
        /// Số tiền xử lý rủi ro
        /// </summary>
        [Column("SOTIENXLRR", Order = 6, TypeName = "decimal(18,2)")]
        public decimal? SOTIENXLRR { get; set; }

        /// <summary>
        /// Ngày phát sinh xử lý
        /// </summary>
        [Column("NGAYPHATSINHXL", Order = 7, TypeName = "datetime2")]
        public DateTime? NGAYPHATSINHXL { get; set; }

        /// <summary>
        /// Thu nợ sau xử lý
        /// </summary>
        [Column("THUNOSAUXL", Order = 8, TypeName = "decimal(18,2)")]
        public decimal? THUNOSAUXL { get; set; }

        /// <summary>
        /// Còn lại ngoài bảng
        /// </summary>
        [Column("CONLAINGOAIBANG", Order = 9, TypeName = "decimal(18,2)")]
        public decimal? CONLAINGOAIBANG { get; set; }

        /// <summary>
        /// Dư nợ nội bảng
        /// </summary>
        [Column("DUNONOIBANG", Order = 10, TypeName = "decimal(18,2)")]
        public decimal? DUNONOIBANG { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [Column("NHOMNO", Order = 11)]
        [StringLength(200)]
        public string NHOMNO { get; set; } = "";

        /// <summary>
        /// Mã cán bộ tín dụng
        /// </summary>
        [Column("MACBTD", Order = 12)]
        [StringLength(200)]
        public string MACBTD { get; set; } = "";

        /// <summary>
        /// Tên cán bộ tín dụng
        /// </summary>
        [Column("TENCBTD", Order = 13)]
        [StringLength(200)]
        public string TENCBTD { get; set; } = "";

        /// <summary>
        /// Mã phòng giao dịch
        /// </summary>
        [Column("MAPGD", Order = 14)]
        [StringLength(200)]
        public string MAPGD { get; set; } = "";

        /// <summary>
        /// Tài khoản hạch toán
        /// </summary>
        [Column("TAIKHOANHACHTOAN", Order = 15)]
        [StringLength(200)]
        public string TAIKHOANHACHTOAN { get; set; } = "";

        /// <summary>
        /// Số tham chiếu
        /// </summary>
        [Column("REFNO", Order = 16)]
        [StringLength(200)]
        public string REFNO { get; set; } = "";

        /// <summary>
        /// Loại nguồn vốn
        /// </summary>
        [Column("LOAINGUONVON", Order = 17)]
        [StringLength(200)]
        public string LOAINGUONVON { get; set; } = "";

        /// <summary>
        /// Cột 18 - Không có header trong CSV
        /// </summary>
        [Column("COLUMN_18", Order = 18)]
        [StringLength(200)]
        public string? COLUMN_18 { get; set; }

        /// <summary>
        /// Cột 19 - Không có header trong CSV
        /// </summary>
        [Column("COLUMN_19", Order = 19)]
        [StringLength(200)]
        public string? COLUMN_19 { get; set; }

        /// <summary>
        /// Cột 20 - Không có header trong CSV (số tiền)
        /// </summary>
        [Column("COLUMN_20", Order = 20, TypeName = "decimal(18,2)")]
        public decimal? COLUMN_20 { get; set; }

        // Temporal/System Columns - Always last
        /// <summary>
        /// ID bản ghi (tự động tăng)
        /// </summary>
        [Key]
        [Column("Id", Order = 21)]
        public long Id { get; set; }

        // Temporal columns are shadow properties managed by EF Core automatically
        // ValidFrom/ValidTo removed - managed as shadow properties by ApplicationDbContext

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Column("CREATED_DATE", Order = 22)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        [Column("UPDATED_DATE", Order = 23)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        /// <summary>
        /// Tên file nguồn
        /// </summary>
        [Column("FILE_NAME", Order = 24)]
        [StringLength(255)]
        public string FILE_NAME { get; set; } = "";
    }
}
