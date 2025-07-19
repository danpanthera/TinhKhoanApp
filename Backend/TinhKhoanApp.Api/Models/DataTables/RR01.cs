using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng RR01 - Auto-generated from database structure
    /// Generated: $(date '+%Y-%m-%d %H:%M:%S')
    /// Temporal Table with History tracking
    /// </summary>
    [Table("RR01")]
    public class RR01
    {
        // Column: Id, Type: bigint
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        // Column: NGAY_DL, Type: date
        [Column("NGAY_DL")]
        public DateTime NGAY_DL { get; set; }

        // Column: MA_CN, Type: nvarchar
        [Column("MA_CN")]
        public string? MA_CN { get; set; }

        // Column: MA_KH, Type: nvarchar
        [Column("MA_KH")]
        public string? MA_KH { get; set; }

        // Column: TEN_KH, Type: nvarchar
        [Column("TEN_KH")]
        public string? TEN_KH { get; set; }

        // Column: SO_TAI_KHOAN, Type: nvarchar
        [Column("SO_TAI_KHOAN")]
        public string? SO_TAI_KHOAN { get; set; }

        // Column: TEN_TAI_KHOAN, Type: nvarchar
        [Column("TEN_TAI_KHOAN")]
        public string? TEN_TAI_KHOAN { get; set; }

        // Column: LOAI_TIEN, Type: nvarchar
        [Column("LOAI_TIEN")]
        public string? LOAI_TIEN { get; set; }

        // Column: SO_DU_TD, Type: decimal
        [Column("SO_DU_TD")]
        public decimal? SO_DU_TD { get; set; }

        // Column: SO_DU_KT, Type: decimal
        [Column("SO_DU_KT")]
        public decimal? SO_DU_KT { get; set; }

        // Column: NGAY_MO_TK, Type: nvarchar
        [Column("NGAY_MO_TK")]
        public string? NGAY_MO_TK { get; set; }

        // Column: TRANG_THAI_TK, Type: nvarchar
        [Column("TRANG_THAI_TK")]
        public string? TRANG_THAI_TK { get; set; }

        // Column: NGAY_DONG_TK, Type: nvarchar
        [Column("NGAY_DONG_TK")]
        public string? NGAY_DONG_TK { get; set; }

        // Column: LY_DO_DONG, Type: nvarchar
        [Column("LY_DO_DONG")]
        public string? LY_DO_DONG { get; set; }

        // Column: MA_NHAN_VIEN, Type: nvarchar
        [Column("MA_NHAN_VIEN")]
        public string? MA_NHAN_VIEN { get; set; }

        // Column: TEN_NHAN_VIEN, Type: nvarchar
        [Column("TEN_NHAN_VIEN")]
        public string? TEN_NHAN_VIEN { get; set; }

        // Column: MA_PHONG, Type: nvarchar
        [Column("MA_PHONG")]
        public string? MA_PHONG { get; set; }

        // Column: TEN_PHONG, Type: nvarchar
        [Column("TEN_PHONG")]
        public string? TEN_PHONG { get; set; }

        // Column: LOAI_TK, Type: nvarchar
        [Column("LOAI_TK")]
        public string? LOAI_TK { get; set; }

        // Column: UB_VAY, Type: decimal
        [Column("UB_VAY")]
        public decimal? UB_VAY { get; set; }

        // Column: SO_THE, Type: nvarchar
        [Column("SO_THE")]
        public string? SO_THE { get; set; }

        // Column: TRANG_THAI_THE, Type: nvarchar
        [Column("TRANG_THAI_THE")]
        public string? TRANG_THAI_THE { get; set; }

        // Column: HINH_THUC_TT, Type: nvarchar
        [Column("HINH_THUC_TT")]
        public string? HINH_THUC_TT { get; set; }

        // Column: THAM_SO_LAI, Type: decimal
        [Column("THAM_SO_LAI")]
        public decimal? THAM_SO_LAI { get; set; }

        // Column: BIEN_DO_LAI, Type: decimal
        [Column("BIEN_DO_LAI")]
        public decimal? BIEN_DO_LAI { get; set; }

        // Column: LAI_SUAT_HIEN_TAI, Type: decimal
        [Column("LAI_SUAT_HIEN_TAI")]
        public decimal? LAI_SUAT_HIEN_TAI { get; set; }

        // Column: GHI_CHU, Type: nvarchar
        [Column("GHI_CHU")]
        public string? GHI_CHU { get; set; }

        // Column: CreatedAt, Type: datetime2
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        // Column: UpdatedAt, Type: datetime2
        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Column: IsDeleted, Type: bit
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        // Column: SysStartTime, Type: datetime2
        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        // Column: SysEndTime, Type: datetime2
        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }

        // Column: (32rowsaffected), Type: 
        [Column("(32rowsaffected)")]
        public string? (32rowsaffected) { get; set; }

    }
}
