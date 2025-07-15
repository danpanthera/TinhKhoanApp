using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng RR01 - 25 cột theo header_7800_rr01_20250430.csv
    /// STRUCTURE: [25 Business Columns] + [System/Temporal Columns]
    /// HEADERS: CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN,VAMC_FLG,NGAY_XLRR,DUNO_GOC_BAN_DAU,DUNO_LAI_TICHLUY_BD,DOC_DAUKY_DA_THU_HT,DUNO_GOC_HIENTAI,DUNO_LAI_HIENTAI,DUNO_NGAN_HAN,DUNO_TRUNG_HAN,DUNO_DAI_HAN,THU_GOC,THU_LAI,BDS,DS,TSK
    /// </summary>
    [Table("RR01")]
    public class RR01
    {
        [Key]
        public int Id { get; set; }

        // === 25 CỘT BUSINESS DATA THEO CSV GỐC (Positions 2-26) ===
        [Column("CN_LOAI_I")]
        [StringLength(50)]
        public string? CN_LOAI_I { get; set; }

        [Column("BRCD")]
        [StringLength(50)]
        public string? BRCD { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SO_LDS")]
        [StringLength(50)]
        public string? SO_LDS { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? CCY { get; set; }

        [Column("SO_LAV")]
        [StringLength(50)]
        public string? SO_LAV { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("NGAY_GIAI_NGAN")]
        [StringLength(20)]
        public string? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN")]
        [StringLength(20)]
        public string? NGAY_DEN_HAN { get; set; }

        [Column("VAMC_FLG")]
        [StringLength(10)]
        public string? VAMC_FLG { get; set; }

        [Column("NGAY_XLRR")]
        [StringLength(20)]
        public string? NGAY_XLRR { get; set; }

        [Column("DUNO_GOC_BAN_DAU")]
        public decimal? DUNO_GOC_BAN_DAU { get; set; }

        [Column("DUNO_LAI_TICHLUY_BD")]
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

        [Column("DOC_DAUKY_DA_THU_HT")]
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

        [Column("DUNO_GOC_HIENTAI")]
        public decimal? DUNO_GOC_HIENTAI { get; set; }

        [Column("DUNO_LAI_HIENTAI")]
        public decimal? DUNO_LAI_HIENTAI { get; set; }

        [Column("DUNO_NGAN_HAN")]
        public decimal? DUNO_NGAN_HAN { get; set; }

        [Column("DUNO_TRUNG_HAN")]
        public decimal? DUNO_TRUNG_HAN { get; set; }

        [Column("DUNO_DAI_HAN")]
        public decimal? DUNO_DAI_HAN { get; set; }

        [Column("THU_GOC")]
        public decimal? THU_GOC { get; set; }

        [Column("THU_LAI")]
        public decimal? THU_LAI { get; set; }

        [Column("BDS")]
        public decimal? BDS { get; set; }

        [Column("DS")]
        public decimal? DS { get; set; }

        [Column("TSK")]
        public decimal? TSK { get; set; }

        // === SYSTEM/TEMPORAL COLUMNS (Positions 27+) ===

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
