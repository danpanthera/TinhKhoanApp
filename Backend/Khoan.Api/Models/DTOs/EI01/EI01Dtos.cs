using System.ComponentModel.DataAnnotations;

namespace Khoan.Api.Models.Dtos.EI01
{
    /// <summary>
    /// EI01 Preview DTO - Hiển thị danh sách E-Banking Information
    /// Chỉ các trường quan trọng để preview
    /// </summary>
    public class EI01PreviewDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MA_CN { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TEN_KH { get; set; }

        /// <summary>
        /// Loại khách hàng
        /// </summary>
        public string? LOAI_KH { get; set; }

        /// <summary>
        /// Số điện thoại EMB
        /// </summary>
        public string? SDT_EMB { get; set; }

        /// <summary>
        /// Trạng thái EMB
        /// </summary>
        public string? TRANG_THAI_EMB { get; set; }

        /// <summary>
        /// Ngày đăng ký EMB
        /// </summary>
        public DateTime? NGAY_DK_EMB { get; set; }

        /// <summary>
        /// Số điện thoại OTT
        /// </summary>
        public string? SDT_OTT { get; set; }

        /// <summary>
        /// Trạng thái OTT
        /// </summary>
        public string? TRANG_THAI_OTT { get; set; }

        /// <summary>
        /// Ngày đăng ký OTT
        /// </summary>
        public DateTime? NGAY_DK_OTT { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// EI01 Details DTO - Chi tiết đầy đủ bản ghi EI01
    /// Tất cả 24 business columns + system columns
    /// </summary>
    public class EI01DetailsDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (24 columns) ===
        public string? MA_CN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? LOAI_KH { get; set; }

        // EMB Service
        public string? SDT_EMB { get; set; }
        public string? TRANG_THAI_EMB { get; set; }
        public DateTime? NGAY_DK_EMB { get; set; }
        public string? USER_EMB { get; set; }

        // OTT Service
        public string? SDT_OTT { get; set; }
        public string? TRANG_THAI_OTT { get; set; }
        public DateTime? NGAY_DK_OTT { get; set; }
        public string? USER_OTT { get; set; }

        // SMS Service
        public string? SDT_SMS { get; set; }
        public string? TRANG_THAI_SMS { get; set; }
        public DateTime? NGAY_DK_SMS { get; set; }
        public string? USER_SMS { get; set; }

        // SAV Service
        public string? SDT_SAV { get; set; }
        public string? TRANG_THAI_SAV { get; set; }
        public DateTime? NGAY_DK_SAV { get; set; }
        public string? USER_SAV { get; set; }

        // LN Service
        public string? SDT_LN { get; set; }
        public string? TRANG_THAI_LN { get; set; }
        public DateTime? NGAY_DK_LN { get; set; }
        public string? USER_LN { get; set; }

        // === SYSTEM COLUMNS ===
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    /// <summary>
    /// EI01 Create DTO - Tạo mới bản ghi EI01
    /// Không có Id, CreatedAt, UpdatedAt (auto-generated)
    /// </summary>
    public class EI01CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS ===
        public string? MA_CN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? LOAI_KH { get; set; }

        // EMB Service
        public string? SDT_EMB { get; set; }
        public string? TRANG_THAI_EMB { get; set; }
        public DateTime? NGAY_DK_EMB { get; set; }
        public string? USER_EMB { get; set; }

        // OTT Service
        public string? SDT_OTT { get; set; }
        public string? TRANG_THAI_OTT { get; set; }
        public DateTime? NGAY_DK_OTT { get; set; }
        public string? USER_OTT { get; set; }

        // SMS Service
        public string? SDT_SMS { get; set; }
        public string? TRANG_THAI_SMS { get; set; }
        public DateTime? NGAY_DK_SMS { get; set; }
        public string? USER_SMS { get; set; }

        // SAV Service
        public string? SDT_SAV { get; set; }
        public string? TRANG_THAI_SAV { get; set; }
        public DateTime? NGAY_DK_SAV { get; set; }
        public string? USER_SAV { get; set; }

        // LN Service
        public string? SDT_LN { get; set; }
        public string? TRANG_THAI_LN { get; set; }
        public DateTime? NGAY_DK_LN { get; set; }
        public string? USER_LN { get; set; }
    }

    /// <summary>
    /// EI01 Update DTO - Cập nhật bản ghi EI01
    /// Tất cả trường có thể update (không có Id, CreatedAt)
    /// </summary>
    public class EI01UpdateDto
    {
        public DateTime? NGAY_DL { get; set; }

        // === BUSINESS COLUMNS ===
        public string? MA_CN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? LOAI_KH { get; set; }

        // EMB Service
        public string? SDT_EMB { get; set; }
        public string? TRANG_THAI_EMB { get; set; }
        public DateTime? NGAY_DK_EMB { get; set; }
        public string? USER_EMB { get; set; }

        // OTT Service
        public string? SDT_OTT { get; set; }
        public string? TRANG_THAI_OTT { get; set; }
        public DateTime? NGAY_DK_OTT { get; set; }
        public string? USER_OTT { get; set; }

        // SMS Service
        public string? SDT_SMS { get; set; }
        public string? TRANG_THAI_SMS { get; set; }
        public DateTime? NGAY_DK_SMS { get; set; }
        public string? USER_SMS { get; set; }

        // SAV Service
        public string? SDT_SAV { get; set; }
        public string? TRANG_THAI_SAV { get; set; }
        public DateTime? NGAY_DK_SAV { get; set; }
        public string? USER_SAV { get; set; }

        // LN Service
        public string? SDT_LN { get; set; }
        public string? TRANG_THAI_LN { get; set; }
        public DateTime? NGAY_DK_LN { get; set; }
        public string? USER_LN { get; set; }
    }

    /// <summary>
    /// EI01 Summary DTO - Thống kê tổng quan EI01
    /// </summary>
    public class EI01SummaryDto
    {
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public int TotalCustomers { get; set; }

        // EMB Statistics
        public int EMB_Active { get; set; }
        public int EMB_Inactive { get; set; }
        public int EMB_Total { get; set; }

        // OTT Statistics
        public int OTT_Active { get; set; }
        public int OTT_Inactive { get; set; }
        public int OTT_Total { get; set; }

        // SMS Statistics
        public int SMS_Active { get; set; }
        public int SMS_Inactive { get; set; }
        public int SMS_Total { get; set; }

        // SAV Statistics
        public int SAV_Active { get; set; }
        public int SAV_Inactive { get; set; }
        public int SAV_Total { get; set; }

        // LN Statistics
        public int LN_Active { get; set; }
        public int LN_Inactive { get; set; }
        public int LN_Total { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// EI01 Import Result DTO - Kết quả import từ CSV
    /// </summary>
    public class EI01ImportResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public DateTime ImportDate { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
