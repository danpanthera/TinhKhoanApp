using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    [Table("RR01")]
    public class RR01
    {
        [Key]
        public long Id { get; set; }

        public DateTime ImportedAt { get; set; }

        public string? StatementDate { get; set; }

        // Business columns - all string type as required
        public string? CN_LOAI_I { get; set; }
        public string? BRCD { get; set; }
        public string? MA_KH { get; set; }
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

        // Common fields
        public string? MA_CN { get; set; }
        public string? NGAY_DL { get; set; }
        public string? FILE_NAME { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
