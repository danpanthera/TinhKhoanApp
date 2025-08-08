using System.ComponentModel.DataAnnotations;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO for RR01 entity
    /// </summary>
    public class RR01DTO
    {
        // System Column - NGAY_DL first (extracted from filename)
        public DateTime NGAY_DL { get; set; }

        // Business Columns - Exact CSV order with proper data types
        public string CN_LOAI_I { get; set; } = string.Empty;
        public string BRCD { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public decimal? SO_LDS { get; set; }
        public string CCY { get; set; } = string.Empty;
        public string SO_LAV { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public DateTime? NGAY_GIAI_NGAN { get; set; }
        public DateTime? NGAY_DEN_HAN { get; set; }
        public string VAMC_FLG { get; set; } = string.Empty;
        public DateTime? NGAY_XLRR { get; set; }
        public decimal? DUNO_GOC_BAN_DAU { get; set; }
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }
        public decimal? DUNO_GOC_HIENTAI { get; set; }
        public decimal? DUNO_LAI_HIENTAI { get; set; }
        public decimal? DUNO_NGAN_HAN { get; set; }
        public decimal? DUNO_TRUNG_HAN { get; set; }
        public decimal? DUNO_DAI_HAN { get; set; }
        public decimal? THU_GOC { get; set; }
        public decimal? THU_LAI { get; set; }
        public decimal? BDS { get; set; }
        public decimal? DS { get; set; }
        public decimal? TSK { get; set; }

        // System columns
        public long Id { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string FILE_NAME { get; set; } = string.Empty;
        public string? IMPORT_BATCH_ID { get; set; }
        public string? DATA_SOURCE { get; set; }
        public string? PROCESSING_STATUS { get; set; }
        public string? ERROR_MESSAGE { get; set; }
        public string? ROW_HASH { get; set; }

        /// <summary>
        /// Maps RR01 entity to RR01DTO
        /// </summary>
        public static RR01DTO FromEntity(DataTables.RR01 entity)
        {
            return new RR01DTO
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                CN_LOAI_I = entity.CN_LOAI_I,
                BRCD = entity.BRCD,
                MA_KH = entity.MA_KH,
                TEN_KH = entity.TEN_KH,
                SO_LDS = entity.SO_LDS,
                CCY = entity.CCY,
                SO_LAV = entity.SO_LAV,
                LOAI_KH = entity.LOAI_KH,
                NGAY_GIAI_NGAN = entity.NGAY_GIAI_NGAN,
                NGAY_DEN_HAN = entity.NGAY_DEN_HAN,
                VAMC_FLG = entity.VAMC_FLG,
                NGAY_XLRR = entity.NGAY_XLRR,
                DUNO_GOC_BAN_DAU = entity.DUNO_GOC_BAN_DAU,
                DUNO_LAI_TICHLUY_BD = entity.DUNO_LAI_TICHLUY_BD,
                DOC_DAUKY_DA_THU_HT = entity.DOC_DAUKY_DA_THU_HT,
                DUNO_GOC_HIENTAI = entity.DUNO_GOC_HIENTAI,
                DUNO_LAI_HIENTAI = entity.DUNO_LAI_HIENTAI,
                DUNO_NGAN_HAN = entity.DUNO_NGAN_HAN,
                DUNO_TRUNG_HAN = entity.DUNO_TRUNG_HAN,
                DUNO_DAI_HAN = entity.DUNO_DAI_HAN,
                THU_GOC = entity.THU_GOC,
                THU_LAI = entity.THU_LAI,
                BDS = entity.BDS,
                DS = entity.DS,
                TSK = entity.TSK,
                CREATED_DATE = entity.CREATED_DATE,
                UPDATED_DATE = entity.UPDATED_DATE,
                FILE_NAME = entity.FILE_NAME,
                IMPORT_BATCH_ID = entity.IMPORT_BATCH_ID,
                DATA_SOURCE = entity.DATA_SOURCE,
                PROCESSING_STATUS = entity.PROCESSING_STATUS,
                ERROR_MESSAGE = entity.ERROR_MESSAGE,
                ROW_HASH = entity.ROW_HASH
            };
        }
    }

    /// <summary>
    /// DTO for creating a new RR01 record
    /// </summary>
    public class CreateRR01DTO
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        // Business fields with proper data types
        public string CN_LOAI_I { get; set; } = string.Empty;
        public string BRCD { get; set; } = string.Empty;

        [Required]
        public string MA_KH { get; set; } = string.Empty;

        public string TEN_KH { get; set; } = string.Empty;
        public decimal? SO_LDS { get; set; }
        public string CCY { get; set; } = string.Empty;
        public string SO_LAV { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public DateTime? NGAY_GIAI_NGAN { get; set; }
        public DateTime? NGAY_DEN_HAN { get; set; }
        public string VAMC_FLG { get; set; } = string.Empty;
        public DateTime? NGAY_XLRR { get; set; }
        public decimal? DUNO_GOC_BAN_DAU { get; set; }
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }
        public decimal? DUNO_GOC_HIENTAI { get; set; }
        public decimal? DUNO_LAI_HIENTAI { get; set; }
        public decimal? DUNO_NGAN_HAN { get; set; }
        public decimal? DUNO_TRUNG_HAN { get; set; }
        public decimal? DUNO_DAI_HAN { get; set; }
        public decimal? THU_GOC { get; set; }
        public decimal? THU_LAI { get; set; }
        public decimal? BDS { get; set; }
        public decimal? DS { get; set; }
        public decimal? TSK { get; set; }

        /// <summary>
        /// Maps CreateRR01DTO to RR01 entity
        /// </summary>
        public DataTables.RR01 ToEntity()
        {
            return new DataTables.RR01
            {
                NGAY_DL = NGAY_DL,
                CN_LOAI_I = CN_LOAI_I,
                BRCD = BRCD,
                MA_KH = MA_KH,
                TEN_KH = TEN_KH,
                SO_LDS = SO_LDS,
                CCY = CCY,
                SO_LAV = SO_LAV,
                LOAI_KH = LOAI_KH,
                NGAY_GIAI_NGAN = NGAY_GIAI_NGAN,
                NGAY_DEN_HAN = NGAY_DEN_HAN,
                VAMC_FLG = VAMC_FLG,
                NGAY_XLRR = NGAY_XLRR,
                DUNO_GOC_BAN_DAU = DUNO_GOC_BAN_DAU,
                DUNO_LAI_TICHLUY_BD = DUNO_LAI_TICHLUY_BD,
                DOC_DAUKY_DA_THU_HT = DOC_DAUKY_DA_THU_HT,
                DUNO_GOC_HIENTAI = DUNO_GOC_HIENTAI,
                DUNO_LAI_HIENTAI = DUNO_LAI_HIENTAI,
                DUNO_NGAN_HAN = DUNO_NGAN_HAN,
                DUNO_TRUNG_HAN = DUNO_TRUNG_HAN,
                DUNO_DAI_HAN = DUNO_DAI_HAN,
                THU_GOC = THU_GOC,
                THU_LAI = THU_LAI,
                BDS = BDS,
                DS = DS,
                TSK = TSK,
                CREATED_DATE = DateTime.Now,
                UPDATED_DATE = DateTime.Now
            };
        }
    }

    /// <summary>
    /// DTO for updating an existing RR01 record
    /// </summary>
    public class UpdateRR01DTO
    {
        public string? TEN_KH { get; set; }
        public string? SO_LDS { get; set; }
        public string? CCY { get; set; }
        public string? SO_LAV { get; set; }
        public string? LOAI_KH { get; set; }
        public string? NGAY_GIAI_NGAN { get; set; }
        public string? NGAY_DEN_HAN { get; set; }
        public string? VAMC_FLG { get; set; }
        public string? NGAY_XLRR { get; set; }
        public string? DUNO_GOC_BAN_DAU { get; set; }
        public string? DUNO_LAI_TICHLUY_BD { get; set; }
        public string? DOC_DAUKY_DA_THU_HT { get; set; }
        public string? DUNO_GOC_HIENTAI { get; set; }
        public string? DUNO_LAI_HIENTAI { get; set; }
        public string? DUNO_NGAN_HAN { get; set; }
        public string? DUNO_TRUNG_HAN { get; set; }
        public string? DUNO_DAI_HAN { get; set; }
        public string? THU_GOC { get; set; }
        public string? THU_LAI { get; set; }
        public string? BDS { get; set; }
        public string? DS { get; set; }
        public string? TSK { get; set; }
    }
}
