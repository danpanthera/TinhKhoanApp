namespace TinhKhoanApp.Api.DTOs
{
    public class GL41PreviewDto
    {
        public DateTime NGAY_DL { get; set; }
        public string MA_DVCS { get; set; } = string.Empty;
        public string TEN_DVCS { get; set; } = string.Empty;
        public string MA_TK { get; set; } = string.Empty;
        public string TEN_TK { get; set; } = string.Empty;
        public decimal SO_DU_DAU_NO { get; set; }
        public decimal SO_DU_DAU_CO { get; set; }
        public decimal PHAT_SINH_NO { get; set; }
        public decimal PHAT_SINH_CO { get; set; }
        public decimal SO_DU_CUOI_NO { get; set; }
        public decimal SO_DU_CUOI_CO { get; set; }
        public string LOAI_TK { get; set; } = string.Empty;
        public int CAP_TK { get; set; }
        public string TT_TK { get; set; } = string.Empty;
    }
}
