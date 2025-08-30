namespace Khoan.Api.Models.Configuration
{
    /// <summary>
    /// Configuration model cho Direct Import settings
    /// </summary>
    public class DirectImportSettings
    {
        /// <summary>
        /// Cấu hình LN03 chuyên biệt
        /// </summary>
        public LN03Settings LN03 { get; set; } = new();

        /// <summary>
        /// Cấu hình mặc định cho tất cả data types
        /// </summary>
        public DefaultImportSettings DefaultSettings { get; set; } = new();
    }

    /// <summary>
    /// Cấu hình chuyên biệt cho LN03
    /// </summary>
    public class LN03Settings
    {
        /// <summary>
        /// LN03 luôn được import trực tiếp (bỏ qua tất cả bước trung gian)
        /// </summary>
        public bool AlwaysDirectImport { get; set; } = true;

        /// <summary>
        /// Bỏ qua tất cả xử lý trung gian
        /// </summary>
        public bool BypassIntermediateProcessing { get; set; } = true;

        /// <summary>
        /// Sử dụng bulk insert cho hiệu năng cao
        /// </summary>
        public bool EnableBulkInsert { get; set; } = true;

        /// <summary>
        /// Tối ưu hóa column mapping
        /// </summary>
        public bool EnableColumnMappingOptimization { get; set; } = true;

        /// <summary>
        /// Kích thước batch cho bulk insert
        /// </summary>
        public int BatchSize { get; set; } = 1000;

        /// <summary>
        /// Sử dụng custom parser cho 20 cột LN03
        /// </summary>
        public bool UseCustomParser { get; set; } = true;

        /// <summary>
        /// Mô tả cấu hình
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Cấu hình mặc định cho Direct Import
    /// </summary>
    public class DefaultImportSettings
    {
        /// <summary>
        /// Bật Direct Import mặc định
        /// </summary>
        public bool EnableDirectImport { get; set; } = true;

        /// <summary>
        /// Log chi tiết quá trình import
        /// </summary>
        public bool LogDetailedProgress { get; set; } = true;

        /// <summary>
        /// Sử dụng transaction scope
        /// </summary>
        public bool EnableTransactionScope { get; set; } = false;

        /// <summary>
        /// Số lần retry tối đa khi có lỗi
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;
    }
}
