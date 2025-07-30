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
        [Column("NGAY_DL", Order = 0, TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // Business Columns - Exact CSV order with proper data types
        [Column("CN_LOAI_I", Order = 1, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string CN_LOAI_I { get; set; } = "";

        [Column("BRCD", Order = 2, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string BRCD { get; set; } = "";

        [Column("MA_KH", Order = 3, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string MA_KH { get; set; } = "";

        [Column("TEN_KH", Order = 4, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string TEN_KH { get; set; } = "";

        [Column("SO_LDS", Order = 5, TypeName = "decimal(18,2)")]
        public decimal? SO_LDS { get; set; }

        [Column("CCY", Order = 6, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string CCY { get; set; } = "";

        [Column("SO_LAV", Order = 7, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string SO_LAV { get; set; } = "";

        [Column("LOAI_KH", Order = 8, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string LOAI_KH { get; set; } = "";

        [Column("NGAY_GIAI_NGAN", Order = 9, TypeName = "datetime2")]
        public DateTime? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN", Order = 10, TypeName = "datetime2")]
        public DateTime? NGAY_DEN_HAN { get; set; }

        [Column("VAMC_FLG", Order = 11, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string VAMC_FLG { get; set; } = "";

        [Column("NGAY_XLRR", Order = 12, TypeName = "datetime2")]
        public DateTime? NGAY_XLRR { get; set; }

        [Column("DUNO_GOC_BAN_DAU", Order = 13, TypeName = "decimal(18,2)")]
        public decimal? DUNO_GOC_BAN_DAU { get; set; }

        [Column("DUNO_LAI_TICHLUY_BD", Order = 14, TypeName = "decimal(18,2)")]
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

        [Column("DOC_DAUKY_DA_THU_HT", Order = 15, TypeName = "decimal(18,2)")]
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

        [Column("DUNO_GOC_HIENTAI", Order = 16, TypeName = "decimal(18,2)")]
        public decimal? DUNO_GOC_HIENTAI { get; set; }

        [Column("DUNO_LAI_HIENTAI", Order = 17, TypeName = "decimal(18,2)")]
        public decimal? DUNO_LAI_HIENTAI { get; set; }

        [Column("DUNO_NGAN_HAN", Order = 18, TypeName = "decimal(18,2)")]
        public decimal? DUNO_NGAN_HAN { get; set; }

        [Column("DUNO_TRUNG_HAN", Order = 19, TypeName = "decimal(18,2)")]
        public decimal? DUNO_TRUNG_HAN { get; set; }

        [Column("DUNO_DAI_HAN", Order = 20, TypeName = "decimal(18,2)")]
        public decimal? DUNO_DAI_HAN { get; set; }

        [Column("THU_GOC", Order = 21, TypeName = "decimal(18,2)")]
        public decimal? THU_GOC { get; set; }

        [Column("THU_LAI", Order = 22, TypeName = "decimal(18,2)")]
        public decimal? THU_LAI { get; set; }

        [Column("BDS", Order = 23, TypeName = "decimal(18,2)")]
        public decimal? BDS { get; set; }

        [Column("DS", Order = 24, TypeName = "decimal(18,2)")]
        public decimal? DS { get; set; }

        [Column("TSK", Order = 25, TypeName = "decimal(18,2)")]
        public decimal? TSK { get; set; }

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 26)]
        public long Id { get; set; }

        // Temporal columns are shadow properties managed by EF Core automatically
        // ValidFrom/ValidTo removed - managed as shadow properties by ApplicationDbContext

        [Column("CREATED_DATE", Order = 27, TypeName = "datetime2")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 28, TypeName = "datetime2")]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 29, TypeName = "nvarchar(400)")]
        [StringLength(400)]
        public string FILE_NAME { get; set; } = "";
        
        [Column("IMPORT_BATCH_ID", Order = 30, TypeName = "nvarchar(400)")]
        [StringLength(400)]
        public string? IMPORT_BATCH_ID { get; set; }
        
        [Column("DATA_SOURCE", Order = 31, TypeName = "nvarchar(400)")]
        [StringLength(400)]
        public string? DATA_SOURCE { get; set; }
        
        [Column("PROCESSING_STATUS", Order = 32, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string? PROCESSING_STATUS { get; set; }
        
        [Column("ERROR_MESSAGE", Order = 33, TypeName = "nvarchar(2000)")]
        [StringLength(2000)]
        public string? ERROR_MESSAGE { get; set; }
        
        [Column("ROW_HASH", Order = 34, TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string? ROW_HASH { get; set; }
    }
}
