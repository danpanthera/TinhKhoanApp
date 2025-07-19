using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DPDA - Auto-generated from database structure
    /// Generated: $(date '+%Y-%m-%d %H:%M:%S')
    /// Temporal Table with History tracking
    /// </summary>
    [Table("DPDA")]
    public class DPDA
    {
        // Column: Id, Type: bigint
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        // Column: NGAY_DL, Type: date
        [Column("NGAY_DL")]
        public DateTime NGAY_DL { get; set; }

        // Column: MA_CHI_NHANH, Type: nvarchar
        [Column("MA_CHI_NHANH")]
        public string? MA_CHI_NHANH { get; set; }

        // Column: MA_KHACH_HANG, Type: nvarchar
        [Column("MA_KHACH_HANG")]
        public string? MA_KHACH_HANG { get; set; }

        // Column: TEN_KHACH_HANG, Type: nvarchar
        [Column("TEN_KHACH_HANG")]
        public string? TEN_KHACH_HANG { get; set; }

        // Column: SO_TAI_KHOAN, Type: nvarchar
        [Column("SO_TAI_KHOAN")]
        public string? SO_TAI_KHOAN { get; set; }

        // Column: LOAI_THE, Type: nvarchar
        [Column("LOAI_THE")]
        public string? LOAI_THE { get; set; }

        // Column: SO_THE, Type: nvarchar
        [Column("SO_THE")]
        public string? SO_THE { get; set; }

        // Column: NGAY_NOP_DON, Type: nvarchar
        [Column("NGAY_NOP_DON")]
        public string? NGAY_NOP_DON { get; set; }

        // Column: NGAY_PHAT_HANH, Type: nvarchar
        [Column("NGAY_PHAT_HANH")]
        public string? NGAY_PHAT_HANH { get; set; }

        // Column: USER_PHAT_HANH, Type: nvarchar
        [Column("USER_PHAT_HANH")]
        public string? USER_PHAT_HANH { get; set; }

        // Column: TRANG_THAI, Type: nvarchar
        [Column("TRANG_THAI")]
        public string? TRANG_THAI { get; set; }

        // Column: PHAN_LOAI, Type: nvarchar
        [Column("PHAN_LOAI")]
        public string? PHAN_LOAI { get; set; }

        // Column: GIAO_THE, Type: nvarchar
        [Column("GIAO_THE")]
        public string? GIAO_THE { get; set; }

        // Column: LOAI_PHAT_HANH, Type: nvarchar
        [Column("LOAI_PHAT_HANH")]
        public string? LOAI_PHAT_HANH { get; set; }

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

        // Column: (20rowsaffected), Type: 
        [Column("(20rowsaffected)")]
        public string? (20rowsaffected) { get; set; }

    }
}
