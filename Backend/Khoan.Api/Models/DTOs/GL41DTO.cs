namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// GL41 DTO - Bảng cân đối tài khoản (Partitioned Columnstore)
    /// </summary>
    public class GL41DTO
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // 13 Business Columns from CSV
        public string? MA_CN { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? MA_TK { get; set; }
        public string? TEN_TK { get; set; }
        public string? LOAI_BT { get; set; }
        public decimal? DN_DAUKY { get; set; }
        public decimal? DC_DAUKY { get; set; }
        public decimal? SBT_NO { get; set; }
        public decimal? ST_GHINO { get; set; }
        public decimal? SBT_CO { get; set; }
        public decimal? ST_GHICO { get; set; }
        public decimal? DN_CUOIKY { get; set; }
        public decimal? DC_CUOIKY { get; set; }

        // System columns
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? FILE_NAME { get; set; }
    }
}
