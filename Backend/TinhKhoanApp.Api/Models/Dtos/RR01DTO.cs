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

        // Business Columns - Exact CSV order
        public string CN_LOAI_I { get; set; } = string.Empty;
        public string BRCD { get; set; } = string.Empty;
        public string MA_KH { get; set; } = string.Empty;
        public string TEN_KH { get; set; } = string.Empty;
        public string SO_LDS { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string SO_LAV { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public string NGAY_GIAI_NGAN { get; set; } = string.Empty;
        public string NGAY_DEN_HAN { get; set; } = string.Empty;
        public string VAMC_FLG { get; set; } = string.Empty;
        public string NGAY_XLRR { get; set; } = string.Empty;
        public string DUNO_GOC_BAN_DAU { get; set; } = string.Empty;
        public string DUNO_LAI_TICHLUY_BD { get; set; } = string.Empty;
        public string DOC_DAUKY_DA_THU_HT { get; set; } = string.Empty;
        public string DUNO_GOC_HIENTAI { get; set; } = string.Empty;
        public string DUNO_LAI_HIENTAI { get; set; } = string.Empty;
        public string DUNO_NGAN_HAN { get; set; } = string.Empty;
        public string DUNO_TRUNG_HAN { get; set; } = string.Empty;
        public string DUNO_DAI_HAN { get; set; } = string.Empty;
        public string THU_GOC { get; set; } = string.Empty;
        public string THU_LAI { get; set; } = string.Empty;
        public string BDS { get; set; } = string.Empty;
        public string DS { get; set; } = string.Empty;
        public string TSK { get; set; } = string.Empty;

        // System columns
        public long Id { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string FILE_NAME { get; set; } = string.Empty;

        /// <summary>
        /// Maps RR01 entity to RR01DTO
        /// </summary>
        public static RR01DTO FromEntity(RR01 entity)
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
                FILE_NAME = entity.FILE_NAME
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

        // Business fields
        public string CN_LOAI_I { get; set; } = string.Empty;
        public string BRCD { get; set; } = string.Empty;

        [Required]
        public string MA_KH { get; set; } = string.Empty;

        public string TEN_KH { get; set; } = string.Empty;
        public string SO_LDS { get; set; } = string.Empty;
        public string CCY { get; set; } = string.Empty;
        public string SO_LAV { get; set; } = string.Empty;
        public string LOAI_KH { get; set; } = string.Empty;
        public string NGAY_GIAI_NGAN { get; set; } = string.Empty;
        public string NGAY_DEN_HAN { get; set; } = string.Empty;
        public string VAMC_FLG { get; set; } = string.Empty;
        public string NGAY_XLRR { get; set; } = string.Empty;
        public string DUNO_GOC_BAN_DAU { get; set; } = string.Empty;
        public string DUNO_LAI_TICHLUY_BD { get; set; } = string.Empty;
        public string DOC_DAUKY_DA_THU_HT { get; set; } = string.Empty;
        public string DUNO_GOC_HIENTAI { get; set; } = string.Empty;
        public string DUNO_LAI_HIENTAI { get; set; } = string.Empty;
        public string DUNO_NGAN_HAN { get; set; } = string.Empty;
        public string DUNO_TRUNG_HAN { get; set; } = string.Empty;
        public string DUNO_DAI_HAN { get; set; } = string.Empty;
        public string THU_GOC { get; set; } = string.Empty;
        public string THU_LAI { get; set; } = string.Empty;
        public string BDS { get; set; } = string.Empty;
        public string DS { get; set; } = string.Empty;
        public string TSK { get; set; } = string.Empty;

        /// <summary>
        /// Maps CreateRR01DTO to RR01 entity
        /// </summary>
        public RR01 ToEntity()
        {
            return new RR01
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
                UPDATED_DATE = DateTime.Now,
                FILE_NAME = "Manual Entry"
            };
        }
    }

    /// <summary>
    /// DTO for updating an existing RR01 record
    /// </summary>
    public class UpdateRR01DTO
    {
        // Business fields that can be updated
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

        /// <summary>
        /// Updates the entity with values from this DTO
        /// </summary>
        public void UpdateEntity(RR01 entity)
        {
            // Update only non-null properties
            if (TEN_KH != null) entity.TEN_KH = TEN_KH;
            if (SO_LDS != null) entity.SO_LDS = SO_LDS;
            if (CCY != null) entity.CCY = CCY;
            if (SO_LAV != null) entity.SO_LAV = SO_LAV;
            if (LOAI_KH != null) entity.LOAI_KH = LOAI_KH;
            if (NGAY_GIAI_NGAN != null) entity.NGAY_GIAI_NGAN = NGAY_GIAI_NGAN;
            if (NGAY_DEN_HAN != null) entity.NGAY_DEN_HAN = NGAY_DEN_HAN;
            if (VAMC_FLG != null) entity.VAMC_FLG = VAMC_FLG;
            if (NGAY_XLRR != null) entity.NGAY_XLRR = NGAY_XLRR;
            if (DUNO_GOC_BAN_DAU != null) entity.DUNO_GOC_BAN_DAU = DUNO_GOC_BAN_DAU;
            if (DUNO_LAI_TICHLUY_BD != null) entity.DUNO_LAI_TICHLUY_BD = DUNO_LAI_TICHLUY_BD;
            if (DOC_DAUKY_DA_THU_HT != null) entity.DOC_DAUKY_DA_THU_HT = DOC_DAUKY_DA_THU_HT;
            if (DUNO_GOC_HIENTAI != null) entity.DUNO_GOC_HIENTAI = DUNO_GOC_HIENTAI;
            if (DUNO_LAI_HIENTAI != null) entity.DUNO_LAI_HIENTAI = DUNO_LAI_HIENTAI;
            if (DUNO_NGAN_HAN != null) entity.DUNO_NGAN_HAN = DUNO_NGAN_HAN;
            if (DUNO_TRUNG_HAN != null) entity.DUNO_TRUNG_HAN = DUNO_TRUNG_HAN;
            if (DUNO_DAI_HAN != null) entity.DUNO_DAI_HAN = DUNO_DAI_HAN;
            if (THU_GOC != null) entity.THU_GOC = THU_GOC;
            if (THU_LAI != null) entity.THU_LAI = THU_LAI;
            if (BDS != null) entity.BDS = BDS;
            if (DS != null) entity.DS = DS;
            if (TSK != null) entity.TSK = TSK;

            // Always update the timestamp
            entity.UPDATED_DATE = DateTime.Now;
        }
    }
}
