using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL41 - Sổ cái chi tiết
    /// Cấu trúc theo file: 7808_gl41_20250630.csv
    /// 13 cột business data + temporal columns
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        [Key]
        public int Id { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("NGAY_DL")]
        public DateTime NGAY_DL { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // === 13 CỘT BUSINESS DATA THEO CSV GỐC ===

        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("MA_TK")]
        [StringLength(50)]
        public string? MA_TK { get; set; }

        [Column("TEN_TK")]
        [StringLength(255)]
        public string? TEN_TK { get; set; }

        [Column("LOAI_BT")]
        [StringLength(10)]
        public string? LOAI_BT { get; set; }

        [Column("DN_DAUKY")]
        public decimal? DN_DAUKY { get; set; }

        [Column("DC_DAUKY")]
        public decimal? DC_DAUKY { get; set; }

        [Column("SBT_NO")]
        public decimal? SBT_NO { get; set; }

        [Column("ST_GHINO")]
        public decimal? ST_GHINO { get; set; }

        [Column("SBT_CO")]
        public decimal? SBT_CO { get; set; }

        [Column("ST_GHICO")]
        public decimal? ST_GHICO { get; set; }

        [Column("DN_CUOIKY")]
        public decimal? DN_CUOIKY { get; set; }

        [Column("DC_CUOIKY")]
        public decimal? DC_CUOIKY { get; set; }
    }
}
