using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.DTOs
{
    public class GL41CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Required]
        [StringLength(20)]
        public string MA_DVCS { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TEN_DVCS { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string MA_TK { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TEN_TK { get; set; } = string.Empty;

        [Required]
        public decimal SO_DU_DAU_NO { get; set; }

        [Required]
        public decimal SO_DU_DAU_CO { get; set; }

        [Required]
        public decimal PHAT_SINH_NO { get; set; }

        [Required]
        public decimal PHAT_SINH_CO { get; set; }

        [Required]
        public decimal SO_DU_CUOI_NO { get; set; }

        [Required]
        public decimal SO_DU_CUOI_CO { get; set; }

        [Required]
        [StringLength(10)]
        public string LOAI_TK { get; set; } = string.Empty;

        [Required]
        public int CAP_TK { get; set; }

        [Required]
        [StringLength(10)]
        public string TT_TK { get; set; } = string.Empty;
    }
}
