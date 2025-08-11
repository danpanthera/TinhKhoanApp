using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables;

/// <summary>
/// RR01 - Risk Report data với 25 business columns
/// Headers theo CSV thực tế: CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN,VAMC_FLG,NGAY_XLRR,DUNO_GOC_BAN_DAU,DUNO_LAI_TICHLUY_BD,DOC_DAUKY_DA_THU_HT,DUNO_GOC_HIENTAI,DUNO_LAI_HIENTAI,DUNO_NGAN_HAN,DUNO_TRUNG_HAN,DUNO_DAI_HAN,THU_GOC,THU_LAI,BDS,DS,TSK
/// Structure: NGAY_DL -> 25 Business Columns -> System/Temporal Columns
/// </summary>
[Table("RR01")]
public class RR01
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Ngày dữ liệu - Lấy từ filename (format: yyyymmdd)
    /// </summary>
    [Required]
    [Column(TypeName = "date")]
    public DateTime NGAY_DL { get; set; }

    // === 25 BUSINESS COLUMNS FROM CSV ===

    /// <summary>
    /// CN_LOAI_I - Chi nhánh loại I
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? CN_LOAI_I { get; set; }

    /// <summary>
    /// BRCD - Branch Code
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? BRCD { get; set; }

    /// <summary>
    /// MA_KH - Mã khách hàng
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? MA_KH { get; set; }

    /// <summary>
    /// TEN_KH - Tên khách hàng
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? TEN_KH { get; set; }

    /// <summary>
    /// SO_LDS - Số LDS
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? SO_LDS { get; set; }

    /// <summary>
    /// CCY - Currency
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? CCY { get; set; }

    /// <summary>
    /// SO_LAV - Số LAV
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? SO_LAV { get; set; }

    /// <summary>
    /// LOAI_KH - Loại khách hàng
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? LOAI_KH { get; set; }

    /// <summary>
    /// NGAY_GIAI_NGAN - Ngày giải ngân
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NGAY_GIAI_NGAN { get; set; }

    /// <summary>
    /// NGAY_DEN_HAN - Ngày đến hạn
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NGAY_DEN_HAN { get; set; }

    /// <summary>
    /// VAMC_FLG - VAMC Flag
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? VAMC_FLG { get; set; }

    /// <summary>
    /// NGAY_XLRR - Ngày xử lý rủi ro
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NGAY_XLRR { get; set; }

    /// <summary>
    /// DUNO_GOC_BAN_DAU - Dư nợ gốc ban đầu
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_GOC_BAN_DAU { get; set; }

    /// <summary>
    /// DUNO_LAI_TICHLUY_BD - Dư nợ lãi tích lũy ban đầu
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

    /// <summary>
    /// DOC_DAUKY_DA_THU_HT - Đòi đầu kỳ đã thu hết
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

    /// <summary>
    /// DUNO_GOC_HIENTAI - Dư nợ gốc hiện tại
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_GOC_HIENTAI { get; set; }

    /// <summary>
    /// DUNO_LAI_HIENTAI - Dư nợ lãi hiện tại
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_LAI_HIENTAI { get; set; }

    /// <summary>
    /// DUNO_NGAN_HAN - Dư nợ ngắn hạn
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_NGAN_HAN { get; set; }

    /// <summary>
    /// DUNO_TRUNG_HAN - Dư nợ trung hạn
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_TRUNG_HAN { get; set; }

    /// <summary>
    /// DUNO_DAI_HAN - Dư nợ dài hạn
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DUNO_DAI_HAN { get; set; }

    /// <summary>
    /// THU_GOC - Thu gốc
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? THU_GOC { get; set; }

    /// <summary>
    /// THU_LAI - Thu lãi
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? THU_LAI { get; set; }

    /// <summary>
    /// BDS - Bất động sản
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? BDS { get; set; }

    /// <summary>
    /// DS - Động sản
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DS { get; set; }

    /// <summary>
    /// TSK - Tài sản khác
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? TSK { get; set; }

    // === SYSTEM COLUMNS ===

    /// <summary>
    /// Tên file CSV nguồn
    /// </summary>
    [Column(TypeName = "nvarchar(255)")]
    public string? FILE_NAME { get; set; }

    /// <summary>
    /// Ngày tạo record
    /// </summary>
    [Column(TypeName = "datetime2(3)")]
    public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật record
    /// </summary>
    [Column(TypeName = "datetime2(3)")]
    public DateTime? UPDATED_DATE { get; set; }

    // === TEMPORAL COLUMNS ===

    /// <summary>
    /// Temporal table start time
    /// </summary>
    [Column(TypeName = "datetime2(3)")]
    public DateTime SysStartTime { get; set; }

    /// <summary>
    /// Temporal table end time
    /// </summary>
    [Column(TypeName = "datetime2(3)")]
    public DateTime SysEndTime { get; set; }
}
