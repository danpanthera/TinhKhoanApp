using System.Text.Json.Serialization;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Model để deserialize dữ liệu DP01 từ JSON trong ImportedDataItems
    /// </summary>
    public class DP01Data
    {
        [JsonPropertyName("MA_CN")]
        public string MA_CN { get; set; } = string.Empty;

        [JsonPropertyName("MA_PGD")]
        public string MA_PGD { get; set; } = string.Empty;

        [JsonPropertyName("TAI_KHOAN_HACH_TOAN")]
        public string TAI_KHOAN_HACH_TOAN { get; set; } = string.Empty;

        [JsonPropertyName("SO_DU_CUOI_KY")]
        public decimal SO_DU_CUOI_KY { get; set; }

        [JsonPropertyName("CURRENT_BALANCE")]
        public decimal CURRENT_BALANCE { get; set; }

        [JsonPropertyName("DATA_DATE")]
        public DateTime? DATA_DATE { get; set; }

        [JsonPropertyName("TEN_TAI_KHOAN")]
        public string? TEN_TAI_KHOAN { get; set; }
    }
}
