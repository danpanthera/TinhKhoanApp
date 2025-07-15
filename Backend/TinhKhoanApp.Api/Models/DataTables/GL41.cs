using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL41 - Phân loại đầu tư chứng khoán
    /// STRUCTURE: [13 Business Columns] + [System/Temporal Columns]
    /// HEADERS: MA_CN,MA_TKPK,TEN_TKPK,NGUON_VON,SO_DKPK,NGAY_DKPK,NGAY_MUAPK,NGAY_BANHPK,SO_LUONG,GIA_MUA,GIA_BAN,GIA_TRI_BOOK,GIA_TRI_THITRUONG
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        [Key]
        public int Id { get; set; }

        // === 13 CỘT BUSINESS DATA THEO CSV GỐC (Positions 2-14) ===

        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("MA_TKPK")]
        [StringLength(50)]
        public string? MA_TKPK { get; set; }

        [Column("TEN_TKPK")]
        [StringLength(255)]
        public string? TEN_TKPK { get; set; }

        [Column("NGUON_VON")]
        [StringLength(50)]
        public string? NGUON_VON { get; set; }

        [Column("SO_DKPK")]
        [StringLength(50)]
        public string? SO_DKPK { get; set; }

        [Column("NGAY_DKPK")]
        [StringLength(20)]
        public string? NGAY_DKPK { get; set; }

        [Column("NGAY_MUAPK")]
        [StringLength(20)]
        public string? NGAY_MUAPK { get; set; }

        [Column("NGAY_BANHPK")]
        [StringLength(20)]
        public string? NGAY_BANHPK { get; set; }

        [Column("SO_LUONG")]
        public decimal? SO_LUONG { get; set; }

        [Column("GIA_MUA")]
        public decimal? GIA_MUA { get; set; }

        [Column("GIA_BAN")]
        public decimal? GIA_BAN { get; set; }

        [Column("GIA_TRI_BOOK")]
        public decimal? GIA_TRI_BOOK { get; set; }

        [Column("GIA_TRI_THITRUONG")]
        public decimal? GIA_TRI_THITRUONG { get; set; }

        // === SYSTEM/TEMPORAL COLUMNS (Positions 15+) ===

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
