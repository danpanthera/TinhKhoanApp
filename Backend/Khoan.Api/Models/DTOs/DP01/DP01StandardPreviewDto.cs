namespace Khoan.Api.Models.Dtos.DP01
{
    /// <summary>
    /// DP01 Preview DTO - chuẩn hóa cho DataTables.DP01 model
    /// Chỉ hiển thị các trường quan trọng nhất từ 63 business columns CSV
    /// </summary>
    public class DP01StandardPreviewDto
    {
        public int Id { get; set; }
        public DateTime? NGAY_DL { get; set; }

        // Key business fields từ DataTables.DP01 structure
        public string? MA_CN { get; set; }
        public string? TAI_KHOAN_HACH_TOAN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? DP_TYPE_NAME { get; set; }
        public string? CCY { get; set; }
        public decimal? CURRENT_BALANCE { get; set; }
        public decimal? RATE { get; set; }
        public string? SO_TAI_KHOAN { get; set; }
        public DateTime? OPENING_DATE { get; set; }

        // System fields
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
