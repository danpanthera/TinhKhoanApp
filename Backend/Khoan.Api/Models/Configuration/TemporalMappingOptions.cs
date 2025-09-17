namespace Khoan.Api.Models.Configuration
{
    public class TemporalMappingOptions
    {
        // Enable temporal mapping per table. Defaults chosen for safe startup.
        public bool DP01 { get; set; } = true;   // Canonical temporal table
        public bool LN01 { get; set; } = false;
        public bool LN03 { get; set; } = false;
        public bool GL01 { get; set; } = false;  // GL01 stays non-temporal
        public bool GL02 { get; set; } = false;  // GL02 stays non-temporal
        public bool GL41 { get; set; } = false;
        public bool DPDA { get; set; } = false;
        public bool EI01 { get; set; } = false;
        public bool RR01 { get; set; } = false;
    }
}
