using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// RR01 - Risk Classification Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("RR01")]
    public class RR01
    {
        // System Column - NGAY_DL first (extracted from filename)
        [Column("NGAY_DL", Order = 0)]
        [StringLength(10)]
        public string NGAY_DL { get; set; } = "";

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("CN_LOAI_I", Order = 1)]
        [StringLength(50)]
        public string CN_LOAI_I { get; set; } = "";

        [Column("BRCD", Order = 2)]
        [StringLength(50)]
        public string BRCD { get; set; } = "";

        [Column("MA_KH", Order = 3)]
        [StringLength(50)]
        public string MA_KH { get; set; } = "";

        [Column("TEN_KH", Order = 4)]
        [StringLength(50)]
        public string TEN_KH { get; set; } = "";

        [Column("SO_LDS", Order = 5)]
        [StringLength(50)]
        public string SO_LDS { get; set; } = "";

        [Column("CCY", Order = 6)]
        [StringLength(50)]
        public string CCY { get; set; } = "";

        [Column("SO_LAV", Order = 7)]
        [StringLength(50)]
        public string SO_LAV { get; set; } = "";

        [Column("LOAI_KH", Order = 8)]
        [StringLength(50)]
        public string LOAI_KH { get; set; } = "";

        [Column("NGAY_GIAI_NGAN", Order = 9)]
        [StringLength(50)]
        public string NGAY_GIAI_NGAN { get; set; } = "";

        [Column("NGAY_DEN_HAN", Order = 10)]
        [StringLength(50)]
        public string NGAY_DEN_HAN { get; set; } = "";

        [Column("VAMC_FLG", Order = 11)]
        [StringLength(50)]
        public string VAMC_FLG { get; set; } = "";

        [Column("NGAY_XLRR", Order = 12)]
        [StringLength(50)]
        public string NGAY_XLRR { get; set; } = "";

        [Column("DUNO_GOC_BAN_DAU", Order = 13)]
        [StringLength(50)]
        public string DUNO_GOC_BAN_DAU { get; set; } = "";

        [Column("DUNO_LAI_TICHLUY_BD", Order = 14)]
        [StringLength(50)]
        public string DUNO_LAI_TICHLUY_BD { get; set; } = "";

        [Column("DOC_DAUKY_DA_THU_HT", Order = 15)]
        [StringLength(50)]
        public string DOC_DAUKY_DA_THU_HT { get; set; } = "";

        [Column("DUNO_GOC_HIENTAI", Order = 16)]
        [StringLength(50)]
        public string DUNO_GOC_HIENTAI { get; set; } = "";

        [Column("DUNO_LAI_HIENTAI", Order = 17)]
        [StringLength(50)]
        public string DUNO_LAI_HIENTAI { get; set; } = "";

        [Column("DUNO_NGAN_HAN", Order = 18)]
        [StringLength(50)]
        public string DUNO_NGAN_HAN { get; set; } = "";

        [Column("DUNO_TRUNG_HAN", Order = 19)]
        [StringLength(50)]
        public string DUNO_TRUNG_HAN { get; set; } = "";

        [Column("DUNO_DAI_HAN", Order = 20)]
        [StringLength(50)]
        public string DUNO_DAI_HAN { get; set; } = "";

        [Column("THU_GOC", Order = 21)]
        [StringLength(50)]
        public string THU_GOC { get; set; } = "";

        [Column("THU_LAI", Order = 22)]
        [StringLength(50)]
        public string THU_LAI { get; set; } = "";

        [Column("BDS", Order = 23)]
        [StringLength(50)]
        public string BDS { get; set; } = "";

        [Column("DS", Order = 24)]
        [StringLength(50)]
        public string DS { get; set; } = "";

        [Column("TSK", Order = 25)]
        [StringLength(50)]
        public string TSK { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 26)]
        public int Id { get; set; }

        [Column("CREATED_DATE", Order = 27)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 28)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 29)]
        [StringLength(255)]
        public string FILE_NAME { get; set; } = "";
    }
}
