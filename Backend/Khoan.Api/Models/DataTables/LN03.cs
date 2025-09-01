using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables;

/// <summary>
/// LN03 - Loan data với 20 business columns (17 có header + 3 không header)
/// Temporal Table với shadow properties cho system versioning
/// Headers từ CSV: MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
/// 3 cột không có header: MALOAI, LOAIKHACHHANG, SOTIEN (columns 18-20)
/// </summary>
[Table("LN03")]
public class LN03
{
    /// <summary>
    /// Primary key (database generated)
    /// </summary>
    [Key]
    [Column("Id", Order = 1)]
    public int Id { get; set; }

    /// <summary>
    /// Ngày dữ liệu - lấy từ filename, format datetime2
    /// </summary>
    [Column("NGAY_DL", Order = 2)]
    public DateTime NGAY_DL { get; set; }

    // === 17 BUSINESS COLUMNS CÓ HEADER (Order 3-19) ===

    /// <summary>
    /// Mã chi nhánh
    /// </summary>
    [Column("MACHINHANH", Order = 3)]
    [StringLength(200)]
    public string? MACHINHANH { get; set; }

    /// <summary>
    /// Tên chi nhánh  
    /// </summary>
    [Column("TENCHINHANH", Order = 4)]
    [StringLength(200)]
    public string? TENCHINHANH { get; set; }

    /// <summary>
    /// Mã khách hàng
    /// </summary>
    [Column("MAKH", Order = 5)]
    [StringLength(200)]
    public string? MAKH { get; set; }

    /// <summary>
    /// Tên khách hàng
    /// </summary>
    [Column("TENKH", Order = 6)]
    [StringLength(200)]
    public string? TENKH { get; set; }

    /// <summary>
    /// Số hợp đồng
    /// </summary>
    [Column("SOHOPDONG", Order = 7)]
    [StringLength(200)]
    public string? SOHOPDONG { get; set; }

    /// <summary>
    /// Số tiền xử lý rủi ro - AMOUNT column
    /// </summary>
    [Column("SOTIENXLRR", TypeName = "decimal(18,2)", Order = 8)]
    public decimal? SOTIENXLRR { get; set; }

    /// <summary>
    /// Ngày phát sinh xử lý - DATE column
    /// </summary>
    [Column("NGAYPHATSINHXL", TypeName = "datetime2", Order = 9)]
    public DateTime? NGAYPHATSINHXL { get; set; }

    /// <summary>
    /// Thu nợ sau xử lý - AMOUNT column
    /// </summary>
    [Column("THUNOSAUXL", TypeName = "decimal(18,2)", Order = 10)]
    public decimal? THUNOSAUXL { get; set; }

    /// <summary>
    /// Còn lại ngoài bảng - BALANCE column
    /// </summary>
    [Column("CONLAINGOAIBANG", TypeName = "decimal(18,2)", Order = 11)]
    public decimal? CONLAINGOAIBANG { get; set; }

    /// <summary>
    /// Dư nợ nội bảng - BALANCE column
    /// </summary>
    [Column("DUNONOIBANG", TypeName = "decimal(18,2)", Order = 12)]
    public decimal? DUNONOIBANG { get; set; }

    /// <summary>
    /// Nhóm nợ
    /// </summary>
    [Column("NHOMNO", Order = 13)]
    [StringLength(200)]
    public string? NHOMNO { get; set; }

    /// <summary>
    /// Mã cán bộ tín dụng
    /// </summary>
    [Column("MACBTD", Order = 14)]
    [StringLength(200)]
    public string? MACBTD { get; set; }

    /// <summary>
    /// Tên cán bộ tín dụng
    /// </summary>
    [Column("TENCBTD", Order = 15)]
    [StringLength(200)]
    public string? TENCBTD { get; set; }

    /// <summary>
    /// Mã phòng giao dịch
    /// </summary>
    [Column("MAPGD", Order = 16)]
    [StringLength(200)]
    public string? MAPGD { get; set; }

    /// <summary>
    /// Tài khoản hạch toán
    /// </summary>
    [Column("TAIKHOANHACHTOAN", Order = 17)]
    [StringLength(200)]
    public string? TAIKHOANHACHTOAN { get; set; }

    /// <summary>
    /// Reference number
    /// </summary>
    [Column("REFNO", Order = 18)]
    [StringLength(200)]
    public string? REFNO { get; set; }

    /// <summary>
    /// Loại nguồn vốn
    /// </summary>
    [Column("LOAINGUONVON", Order = 19)]
    [StringLength(200)]
    public string? LOAINGUONVON { get; set; }

    // === 3 BUSINESS COLUMNS KHÔNG CÓ HEADER (Order 20-22) ===

    /// <summary>
    /// Cột 18 không có header - MALOAI trong database
    /// </summary>
    [Column("MALOAI", Order = 20)]
    [StringLength(200)]
    public string? Column18 { get; set; }

    /// <summary>
    /// Cột 19 không có header - LOAIKHACHHANG trong database
    /// </summary>
    [Column("LOAIKHACHHANG", Order = 21)]
    [StringLength(200)]
    public string? Column19 { get; set; }

    /// <summary>
    /// Cột 20 không có header - SOTIEN trong database - AMOUNT column
    /// </summary>
    [Column("SOTIEN", TypeName = "decimal(18,2)", Order = 22)]
    public decimal? Column20 { get; set; }

    // === SYSTEM COLUMNS (Order 23-25) ===

    /// <summary>
    /// Người tạo
    /// </summary>
    [Column("CREATED_BY", Order = 23)]
    [StringLength(200)]
    public string? CREATED_BY { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    [Column("CREATED_DATE", Order = 24)]
    public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// File gốc
    /// </summary>
    [Column("FILE_ORIGIN", Order = 25)]
    [StringLength(500)]
    public string? FILE_ORIGIN { get; set; }

    // ValidFrom và ValidTo được quản lý bởi SQL Server temporal tables (shadow properties)
}
