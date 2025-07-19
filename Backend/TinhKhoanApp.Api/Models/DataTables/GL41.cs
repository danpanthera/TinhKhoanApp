using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL41 - Auto-generated from database structure
    /// Generated: $(date '+%Y-%m-%d %H:%M:%S')
    /// Temporal Table with History tracking
    /// </summary>
    [Table("GL41")]
    public class GL41
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

        // Column: LOAI_TIEN, Type: nvarchar
        [Column("LOAI_TIEN")]
        public string? LOAI_TIEN { get; set; }

        // Column: MA_TK, Type: nvarchar
        [Column("MA_TK")]
        public string? MA_TK { get; set; }

        // Column: TEN_TK, Type: nvarchar
        [Column("TEN_TK")]
        public string? TEN_TK { get; set; }

        // Column: LOAI_BT, Type: nvarchar
        [Column("LOAI_BT")]
        public string? LOAI_BT { get; set; }

        // Column: DN_DAUKY, Type: decimal
        [Column("DN_DAUKY")]
        public decimal? DN_DAUKY { get; set; }

        // Column: DC_DAUKY, Type: decimal
        [Column("DC_DAUKY")]
        public decimal? DC_DAUKY { get; set; }

        // Column: SBT_NO, Type: decimal
        [Column("SBT_NO")]
        public decimal? SBT_NO { get; set; }

        // Column: ST_GHINO, Type: decimal
        [Column("ST_GHINO")]
        public decimal? ST_GHINO { get; set; }

        // Column: SBT_CO, Type: decimal
        [Column("SBT_CO")]
        public decimal? SBT_CO { get; set; }

        // Column: ST_GHICO, Type: decimal
        [Column("ST_GHICO")]
        public decimal? ST_GHICO { get; set; }

        // Column: DN_CUOIKY, Type: decimal
        [Column("DN_CUOIKY")]
        public decimal? DN_CUOIKY { get; set; }

        // Column: DC_CUOIKY, Type: decimal
        [Column("DC_CUOIKY")]
        public decimal? DC_CUOIKY { get; set; }

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
