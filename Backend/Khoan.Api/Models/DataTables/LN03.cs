using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.DataTables;

/// <summary>
/// LN03 - Loan data với 20 business columns (17 có header + 3 không header)
/// Temporal Table với shadow properties cho system versioning
/// Headers từ CSV: MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
/// 3 cột không có header: Column18, Column19, Column20
/// </summary>
[Table("LN03")]
public class LN03
{
    /// <summary>
    /// Ngày dữ liệu - lấy từ filename, format datetime2
    /// </summary>
    [Column(Order = 1)]
    public DateTime NGAY_DL { get; set; }

    // 17 business columns có header từ CSV file thực tế

    /// <summary>
    /// Mã chi nhánh
    /// </summary>
    [Column(Order = 2)]
    [StringLength(200)]
    public string? MACHINHANH { get; set; }

    /// <summary>
    /// Tên chi nhánh
    /// </summary>
    [Column(Order = 3)]
    [StringLength(200)]
    public string? TENCHINHANH { get; set; }

    /// <summary>
    /// Mã khách hàng
    /// </summary>
    [Column(Order = 4)]
    [StringLength(200)]
    public string? MAKH { get; set; }

    /// <summary>
    /// Tên khách hàng
    /// </summary>
    [Column(Order = 5)]
    [StringLength(200)]
    public string? TENKH { get; set; }

    /// <summary>
    /// Số hợp đồng
    /// </summary>
    [Column(Order = 6)]
    [StringLength(200)]
    public string? SOHOPDONG { get; set; }

    /// <summary>
    /// Số tiền xử lý rủi ro - SOTIENXLRR
    /// </summary>
    [Column(Order = 7, TypeName = "decimal(18,2)")]
    public decimal? SOTIENXLRR { get; set; }

    /// <summary>
    /// Ngày phát sinh xử lý - NGAYPHATSINHXL
    /// </summary>
    [Column(Order = 8, TypeName = "datetime2")]
    public DateTime? NGAYPHATSINHXL { get; set; }

    /// <summary>
    /// Thu nợ sau xử lý - THUNOSAUXL
    /// </summary>
    [Column(Order = 9, TypeName = "decimal(18,2)")]
    public decimal? THUNOSAUXL { get; set; }

    /// <summary>
    /// Còn lại ngoài bảng - CONLAINGOAIBANG
    /// </summary>
    [Column(Order = 10, TypeName = "decimal(18,2)")]
    public decimal? CONLAINGOAIBANG { get; set; }

    /// <summary>
    /// Primary key (database auto-generated, not in CSV)
    /// </summary>
    [Key]
    [Column(Order = 11)]
    public int Id { get; set; }

    /// <summary>
    /// Dư nợ nội bảng - DUNONOIBANG
    /// </summary>
    [Column(Order = 12, TypeName = "decimal(18,2)")]
    public decimal? DUNONOIBANG { get; set; }

    /// <summary>
    /// Nhóm nợ - NHOMNO
    /// </summary>
    [Column(Order = 13)]
    [StringLength(200)]
    public string? NHOMNO { get; set; }

    /// <summary>
    /// Mã cán bộ tín dụng - MACBTD
    /// </summary>
    [Column(Order = 14)]
    [StringLength(200)]
    public string? MACBTD { get; set; }

    /// <summary>
    /// Tên cán bộ tín dụng - TENCBTD
    /// </summary>
    [Column(Order = 15)]
    [StringLength(200)]
    public string? TENCBTD { get; set; }

    /// <summary>
    /// Mã phòng giao dịch - MAPGD
    /// </summary>
    [Column(Order = 16)]
    [StringLength(200)]
    public string? MAPGD { get; set; }

    /// <summary>
    /// Tài khoản hạch toán - TAIKHOANHACHTOAN
    /// </summary>
    [Column(Order = 17)]
    [StringLength(200)]
    public string? TAIKHOANHACHTOAN { get; set; }

    /// <summary>
    /// Reference number - REFNO
    /// </summary>
    [Column(Order = 18)]
    [StringLength(200)]
    public string? REFNO { get; set; }

    /// <summary>
    /// Loại nguồn vốn - LOAINGUONVON
    /// </summary>
    [Column(Order = 19)]
    [StringLength(200)]
    public string? LOAINGUONVON { get; set; }

    // 3 business columns không có header trong CSV - database names: MALOAI, LOAIKHACHHANG, SOTIEN

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
    /// Cột 20 không có header - SOTIEN trong database
    /// </summary>
    [Column("SOTIEN", Order = 22, TypeName = "decimal(18,2)")]
    public decimal? Column20 { get; set; }    // System fields

    /// <summary>
    /// Người tạo
    /// </summary>
    [Column(Order = 23)]
    [StringLength(200)]
    public string? CREATED_BY { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    [Column(Order = 24)]
    public DateTime CREATED_DATE { get; set; }

    /// <summary>
    /// File gốc
    /// </summary>
    [Column(Order = 25)]
    [StringLength(500)]
    public string? FILE_ORIGIN { get; set; }

    // Temporal columns (shadow properties - không cần định nghĩa)
    // ValidFrom và ValidTo được quản lý bởi SQL Server temporal tables
}
