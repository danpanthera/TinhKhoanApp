using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables;

/// <summary>
/// LN03 - Loan data với 20 cột (17 có header + 3 không header)
/// Headers theo CSV thực tế: MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
/// 3 cột cuối không có header nhưng có dữ liệu: ["100", "Cá nhân", "6000000000"]
/// </summary>
[Table("LN03")]
public class LN03
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Ngày dữ liệu - system field phải đứng đầu theo quy tắc
    /// </summary>
    [Column(Order = 1)]
    public DateTime NGAY_DL { get; set; }

    // 17 cột có header từ file CSV thực tế

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
    /// Số tiền xử lý rủi ro
    /// </summary>
    [Column(Order = 7, TypeName = "decimal(18,2)")]
    public decimal? SOTIENXLRR { get; set; }

    /// <summary>
    /// Ngày phát sinh xử lý
    /// </summary>
    [Column(Order = 8, TypeName = "datetime2")]
    public DateTime? NGAYPHATSINHXL { get; set; }

    /// <summary>
    /// Thu nợ sau xử lý
    /// </summary>
    [Column(Order = 9, TypeName = "decimal(18,2)")]
    public decimal? THUNOSAUXL { get; set; }

    /// <summary>
    /// Còn lại ngoài bảng
    /// </summary>
    [Column(Order = 9, TypeName = "decimal(18,2)")]
    public decimal? CONLAINGOAIBANG { get; set; }

    /// <summary>
    /// Dư nợ nội bảng
    /// </summary>
    [Column(Order = 10, TypeName = "decimal(18,2)")]
    public decimal? DUNONOIBANG { get; set; }

    /// <summary>
    /// Nhóm nợ
    /// </summary>
    [Column(Order = 11)]
    [StringLength(200)]
    public string? NHOMNO { get; set; }

    /// <summary>
    /// Mã cán bộ tín dụng
    /// </summary>
    [Column(Order = 12)]
    [StringLength(200)]
    public string? MACBTD { get; set; }

    /// <summary>
    /// Tên cán bộ tín dụng
    /// </summary>
    [Column(Order = 13)]
    [StringLength(200)]
    public string? TENCBTD { get; set; }

    /// <summary>
    /// Mã phòng giao dịch
    /// </summary>
    [Column(Order = 14)]
    [StringLength(200)]
    public string? MAPGD { get; set; }

    /// <summary>
    /// Tài khoản hạch toán
    /// </summary>
    [Column(Order = 15)]
    [StringLength(200)]
    public string? TAIKHOANHACHTOAN { get; set; }

    /// <summary>
    /// Reference number
    /// </summary>
    [Column(Order = 16)]
    [StringLength(200)]
    public string? REFNO { get; set; }

    /// <summary>
    /// Loại nguồn vốn
    /// </summary>
    [Column(Order = 17)]
    [StringLength(200)]
    public string? LOAINGUONVON { get; set; }

    // 3 cột không có header - dựa trên CSV thực tế

    /// <summary>
    /// Cột 18 (không có header) - giá trị như "100"
    /// </summary>
    [Column(Order = 18)]
    [StringLength(200)]
    public string? Column18 { get; set; }

    /// <summary>
    /// Cột 19 (không có header) - giá trị như "Cá nhân"
    /// </summary>
    [Column(Order = 19)]
    [StringLength(200)]
    public string? Column19 { get; set; }

    /// <summary>
    /// Cột 20 (không có header) - giá trị số lớn như "6000000000"
    /// </summary>
    [Column(Order = 20, TypeName = "decimal(18,2)")]
    public decimal? Column20 { get; set; }    // System fields

    /// <summary>
    /// Người tạo
    /// </summary>
    [StringLength(200)]
    public string? CREATED_BY { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CREATED_DATE { get; set; }

    /// <summary>
    /// File gốc
    /// </summary>
    [StringLength(500)]
    public string? FILE_ORIGIN { get; set; }
}
