using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN03 - Auto-generated from database structure
    /// Generated: $(date '+%Y-%m-%d %H:%M:%S')
    /// Temporal Table with History tracking
    /// </summary>
    [Table("LN03")]
    public class LN03
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

        // Column: SO_HSV, Type: nvarchar
        [Column("SO_HSV")]
        public string? SO_HSV { get; set; }

        // Column: MA_KH, Type: nvarchar
        [Column("MA_KH")]
        public string? MA_KH { get; set; }

        // Column: TEN_KH, Type: nvarchar
        [Column("TEN_KH")]
        public string? TEN_KH { get; set; }

        // Column: SO_TD, Type: nvarchar
        [Column("SO_TD")]
        public string? SO_TD { get; set; }

        // Column: LOAI_TIEN, Type: nvarchar
        [Column("LOAI_TIEN")]
        public string? LOAI_TIEN { get; set; }

        // Column: SO_TIEN_TU, Type: decimal
        [Column("SO_TIEN_TU")]
        public decimal? SO_TIEN_TU { get; set; }

        // Column: LAI_SUAT, Type: decimal
        [Column("LAI_SUAT")]
        public decimal? LAI_SUAT { get; set; }

        // Column: NGAY_BAT_DAU, Type: nvarchar
        [Column("NGAY_BAT_DAU")]
        public string? NGAY_BAT_DAU { get; set; }

        // Column: NGAY_KET_THUC, Type: nvarchar
        [Column("NGAY_KET_THUC")]
        public string? NGAY_KET_THUC { get; set; }

        // Column: SO_DU, Type: decimal
        [Column("SO_DU")]
        public decimal? SO_DU { get; set; }

        // Column: TK_TIEN_GUI, Type: nvarchar
        [Column("TK_TIEN_GUI")]
        public string? TK_TIEN_GUI { get; set; }

        // Column: TK_TIEN_LAI, Type: nvarchar
        [Column("TK_TIEN_LAI")]
        public string? TK_TIEN_LAI { get; set; }

        // Column: TRANG_THAI, Type: nvarchar
        [Column("TRANG_THAI")]
        public string? TRANG_THAI { get; set; }

        // Column: NGAY_TAO, Type: nvarchar
        [Column("NGAY_TAO")]
        public string? NGAY_TAO { get; set; }

        // Column: NGUOI_TAO, Type: nvarchar
        [Column("NGUOI_TAO")]
        public string? NGUOI_TAO { get; set; }

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

        // Column: (24rowsaffected), Type: 
        [Column("(24rowsaffected)")]
        public string? (24rowsaffected) { get; set; }

    }
}
