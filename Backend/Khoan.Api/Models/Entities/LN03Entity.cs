using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// LN03 Entity - Loan Processing Table
    /// 20 business columns (17 with headers + 3 without headers) + system columns
    /// CSV Source: 7800_ln03_20241231.csv
    /// </summary>
    [Table("LN03")]
    public class LN03Entity : ITemporalEntity
    {
        // === SYSTEM COLUMNS (từ ITemporalEntity) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // Not stored in DB for LN03 -> prevent EF from including these
        [NotMapped]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public DateTime UpdatedAt { get; set; }

        // Temporal table support
        [NotMapped]
        public DateTime SysStartTime { get; set; }

        [NotMapped]
        public DateTime SysEndTime { get; set; }

        // === BUSINESS COLUMNS (20 columns theo CSV structure) ===

        /// <summary>
        /// Ngày dữ liệu - Business primary key
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS WITH HEADERS (17 columns) ===

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MACHINHANH { get; set; }

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TENCHINHANH { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MAKH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TENKH { get; set; }

        /// <summary>
        /// Số hợp đồng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? SOHOPDONG { get; set; }

        /// <summary>
        /// Số tiền xử lý rủi ro
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SOTIENXLRR { get; set; }

        /// <summary>
        /// Ngày phát sinh xử lý
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAYPHATSINHXL { get; set; }

        /// <summary>
        /// Thu nợ sau xử lý
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? THUNOSAUXL { get; set; }

        /// <summary>
        /// Còn lại ngoài bảng
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CONLAINGOAIBANG { get; set; }

        /// <summary>
        /// Dư nợ nội bảng
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DUNONOIBANG { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? NHOMNO { get; set; }

        /// <summary>
        /// Mã cán bộ tín dụng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MACBTD { get; set; }

        /// <summary>
        /// Tên cán bộ tín dụng
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TENCBTD { get; set; }

        /// <summary>
        /// Mã phòng giao dịch
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MAPGD { get; set; }

        /// <summary>
        /// Tài khoản hạch toán
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TAIKHOANHACHTOAN { get; set; }

        /// <summary>
        /// Reference number
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? REFNO { get; set; }

        /// <summary>
        /// Loại nguồn vốn
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? LOAINGUONVON { get; set; }

        // === BUSINESS COLUMNS WITHOUT HEADERS (3 columns - positions 18, 19, 20) ===

        /// <summary>
    /// Column 18 mapped to MALOAI in DB
    /// </summary>
    [Column("MALOAI", TypeName = "nvarchar(200)")]
    public string? Column18 { get; set; }

        /// <summary>
    /// Column 19 mapped to LOAIKHACHHANG in DB
    /// </summary>
    [Column("LOAIKHACHHANG", TypeName = "nvarchar(200)")]
    public string? Column19 { get; set; }

        /// <summary>
    /// Column 20 mapped to SOTIEN in DB
    /// </summary>
    [Column("SOTIEN", TypeName = "decimal(18,2)")]
    public decimal? Column20 { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
    // Metadata columns below are not present in DB -> remove mapping
    [NotMapped]
    public string? FileName { get; set; }

        /// <summary>
    [NotMapped]
    public Guid? ImportId { get; set; }

        /// <summary>
    [NotMapped]
    public string? ImportMetadata { get; set; }

        // === MISSING PROPERTIES NEEDED BY CODE ===

        /// <summary>
        /// Soft delete flag
        /// </summary>
    // Not in DB
    [NotMapped]
    public bool IS_DELETED { get; set; } = false;

        /// <summary>
        /// Columns without headers - Column 18
        /// </summary>
    [NotMapped]
    public string? COLUMN_18 { get; set; }

        /// <summary>
        /// Columns without headers - Column 19
        /// </summary>
    [NotMapped]
    public string? COLUMN_19 { get; set; }

        /// <summary>
        /// Columns without headers - Column 20
        /// </summary>
    [NotMapped]
    public string? COLUMN_20 { get; set; }

        /// <summary>
        /// Created date (matching database schema)
        /// </summary>
    [NotMapped]
    public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Updated date (matching database schema)
        /// </summary>
    [NotMapped]
    public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;
    }
}
