namespace TinhKhoanApp.Api.Models.CsvModels
{
    /// <summary>
    /// GL41 CSV Row - for parsing CSV data (13 business columns)
    /// </summary>
    public class GL41CsvRow
    {
        public string? MA_CN { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? MA_TK { get; set; }
        public string? TEN_TK { get; set; }
        public string? LOAI_BT { get; set; }
        public string? DN_DAUKY { get; set; }
        public string? DC_DAUKY { get; set; }
        public string? SBT_NO { get; set; }
        public string? ST_GHINO { get; set; }
        public string? SBT_CO { get; set; }
        public string? ST_GHICO { get; set; }
        public string? DN_CUOIKY { get; set; }
        public string? DC_CUOIKY { get; set; }
    }
}
