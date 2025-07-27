using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// RR01 - Risk Rating/Recovery Report Model
    /// 25 Business Columns + System Columns
    /// Temporal Table with Columnstore Index Support
    /// Direct import from *rr01* CSV files only
    /// NGAY_DL -> Business Columns (CSV order) -> Temporal + System columns
    /// </summary>
    [Table("RR01")]
    public class RR01
    {
        [Key]
        public long Id { get; set; }

        // =================== SYSTEM COLUMN FIRST ===================
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // =================== BUSINESS COLUMNS (25 total) - CSV ORDER ===================

        // String columns (8 columns)
        [Column("CN_LOAI_I", Order = 1)]
        [MaxLength(200)]
        public string? CN_LOAI_I { get; set; }

        [Column("BRCD", Order = 2)]
        [MaxLength(200)]
        public string? BRCD { get; set; }

        [Column("MA_KH", Order = 3)]
        [MaxLength(200)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH", Order = 4)]
        [MaxLength(200)]
        public string? TEN_KH { get; set; }

        [Column("SO_LDS", Order = 5)]
        [MaxLength(200)]
        public string? SO_LDS { get; set; }

        [Column("CCY", Order = 6)]
        [MaxLength(200)]
        public string? CCY { get; set; }

        [Column("SO_LAV", Order = 7)]
        [MaxLength(200)]
        public string? SO_LAV { get; set; }

        [Column("LOAI_KH", Order = 8)]
        [MaxLength(200)]
        public string? LOAI_KH { get; set; }

        // Date columns (3 columns)
        [Column("NGAY_GIAI_NGAN", Order = 9)]
        public DateTime? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN", Order = 10)]
        public DateTime? NGAY_DEN_HAN { get; set; }

        [Column("VAMC_FLG", Order = 11)]
        [MaxLength(200)]
        public string? VAMC_FLG { get; set; }

        [Column("NGAY_XLRR", Order = 12)]
        public DateTime? NGAY_XLRR { get; set; }

        // Amount/Financial columns (13 columns)
        [Column("DUNO_GOC_BAN_DAU", Order = 13)]
        public decimal? DUNO_GOC_BAN_DAU { get; set; }

        [Column("DUNO_LAI_TICHLUY_BD", Order = 14)]
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

        [Column("DOC_DAUKY_DA_THU_HT", Order = 15)]
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

        [Column("DUNO_GOC_HIENTAI", Order = 16)]
        public decimal? DUNO_GOC_HIENTAI { get; set; }

        [Column("DUNO_LAI_HIENTAI", Order = 17)]
        public decimal? DUNO_LAI_HIENTAI { get; set; }

        [Column("DUNO_NGAN_HAN", Order = 18)]
        public decimal? DUNO_NGAN_HAN { get; set; }

        [Column("DUNO_TRUNG_HAN", Order = 19)]
        public decimal? DUNO_TRUNG_HAN { get; set; }

        [Column("DUNO_DAI_HAN", Order = 20)]
        public decimal? DUNO_DAI_HAN { get; set; }

        [Column("THU_GOC", Order = 21)]
        public decimal? THU_GOC { get; set; }

        [Column("THU_LAI", Order = 22)]
        public decimal? THU_LAI { get; set; }

        [Column("BDS", Order = 23)]
        public decimal? BDS { get; set; }

        [Column("DS", Order = 24)]
        public decimal? DS { get; set; }

        [Column("TSK", Order = 25)]
        public decimal? TSK { get; set; }

        // =================== SYSTEM COLUMNS ===================
        [Column("FILE_NAME")]
        [MaxLength(200)]
        public string? FILE_NAME { get; set; }

        // Temporal columns (configured as shadow properties in DbContext)
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }

        // Additional system columns
        [Column("IMPORT_BATCH_ID")]
        [MaxLength(200)]
        public string? IMPORT_BATCH_ID { get; set; }

        [Column("DATA_SOURCE")]
        [MaxLength(200)]
        public string? DATA_SOURCE { get; set; }

        [Column("PROCESSING_STATUS")]
        [MaxLength(50)]
        public string? PROCESSING_STATUS { get; set; }

        [Column("ERROR_MESSAGE")]
        [MaxLength(1000)]
        public string? ERROR_MESSAGE { get; set; }

        [Column("ROW_HASH")]
        [MaxLength(100)]
        public string? ROW_HASH { get; set; }
    }
}
