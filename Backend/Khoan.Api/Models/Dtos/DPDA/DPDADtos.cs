using System.ComponentModel.DataAnnotations;

namespace Khoan.Api.Models.Dtos.DPDA
{
    /// <summary>
    /// DPDA Preview DTO - Để hiển thị danh sách thẻ nộp đơn gửi tiết kiệm
    /// Chỉ các field cần thiết cho list view, theo pattern DP01PreviewDto
    /// </summary>
    public class DPDAPreviewDto
    {
        /// <summary>
        /// ID hệ thống
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY
        /// </summary>
        public string MA_CHI_NHANH { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - BUSINESS KEY
        /// </summary>
        public string MA_KHACH_HANG { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TEN_KHACH_HANG { get; set; }

        /// <summary>
        /// Số tài khoản - BUSINESS KEY
        /// </summary>
        public string SO_TAI_KHOAN { get; set; } = string.Empty;

        /// <summary>
        /// Loại thẻ
        /// </summary>
        public string? LOAI_THE { get; set; }

        /// <summary>
        /// Số thẻ - BUSINESS KEY
        /// </summary>
        public string? SO_THE { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public DateTime? NGAY_PHAT_HANH { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DPDA Create DTO - Để tạo mới thẻ nộp đơn gửi tiết kiệm
    /// Chứa validation rules và tất cả 13 business columns theo CSV structure
    /// </summary>
    public class DPDACreateDto
    {
        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY (Required)
        /// </summary>
        [Required(ErrorMessage = "Mã chi nhánh là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Mã chi nhánh không được vượt quá 50 ký tự")]
        public string MA_CHI_NHANH { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - BUSINESS KEY (Required)
        /// </summary>
        [Required(ErrorMessage = "Mã khách hàng là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Mã khách hàng không được vượt quá 50 ký tự")]
        public string MA_KHACH_HANG { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [MaxLength(255, ErrorMessage = "Tên khách hàng không được vượt quá 255 ký tự")]
        public string? TEN_KHACH_HANG { get; set; }

        /// <summary>
        /// Số tài khoản - BUSINESS KEY (Required)
        /// </summary>
        [Required(ErrorMessage = "Số tài khoản là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Số tài khoản không được vượt quá 50 ký tự")]
        public string SO_TAI_KHOAN { get; set; } = string.Empty;

        /// <summary>
        /// Loại thẻ
        /// </summary>
        [MaxLength(50, ErrorMessage = "Loại thẻ không được vượt quá 50 ký tự")]
        public string? LOAI_THE { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        [MaxLength(50, ErrorMessage = "Số thẻ không được vượt quá 50 ký tự")]
        public string? SO_THE { get; set; }

        /// <summary>
        /// Ngày nộp đơn
        /// </summary>
        public DateTime? NGAY_NOP_DON { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public DateTime? NGAY_PHAT_HANH { get; set; }

        /// <summary>
        /// User phát hành
        /// </summary>
        [MaxLength(100, ErrorMessage = "User phát hành không được vượt quá 100 ký tự")]
        public string? USER_PHAT_HANH { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [MaxLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Phân loại
        /// </summary>
        [MaxLength(50, ErrorMessage = "Phân loại không được vượt quá 50 ký tự")]
        public string? PHAN_LOAI { get; set; }

        /// <summary>
        /// Giao thẻ
        /// </summary>
        [MaxLength(50, ErrorMessage = "Giao thẻ không được vượt quá 50 ký tự")]
        public string? GIAO_THE { get; set; }

        /// <summary>
        /// Loại phát hành
        /// </summary>
        [MaxLength(50, ErrorMessage = "Loại phát hành không được vượt quá 50 ký tự")]
        public string? LOAI_PHAT_HANH { get; set; }
    }

    /// <summary>
    /// DPDA Update DTO - Để cập nhật thông tin thẻ nộp đơn gửi tiết kiệm
    /// Có thể có các field optional cho partial update
    /// </summary>
    public class DPDAUpdateDto
    {
        /// <summary>
        /// ID record cần update (Required)
        /// </summary>
        [Required(ErrorMessage = "ID là bắt buộc")]
        public long Id { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [MaxLength(255, ErrorMessage = "Tên khách hàng không được vượt quá 255 ký tự")]
        public string? TEN_KHACH_HANG { get; set; }

        /// <summary>
        /// Loại thẻ
        /// </summary>
        [MaxLength(50, ErrorMessage = "Loại thẻ không được vượt quá 50 ký tự")]
        public string? LOAI_THE { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        [MaxLength(50, ErrorMessage = "Số thẻ không được vượt quá 50 ký tự")]
        public string? SO_THE { get; set; }

        /// <summary>
        /// Ngày nộp đơn
        /// </summary>
        public DateTime? NGAY_NOP_DON { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public DateTime? NGAY_PHAT_HANH { get; set; }

        /// <summary>
        /// User phát hành
        /// </summary>
        [MaxLength(100, ErrorMessage = "User phát hành không được vượt quá 100 ký tự")]
        public string? USER_PHAT_HANH { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [MaxLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Phân loại
        /// </summary>
        [MaxLength(50, ErrorMessage = "Phân loại không được vượt quá 50 ký tự")]
        public string? PHAN_LOAI { get; set; }

        /// <summary>
        /// Giao thẻ
        /// </summary>
        [MaxLength(50, ErrorMessage = "Giao thẻ không được vượt quá 50 ký tự")]
        public string? GIAO_THE { get; set; }

        /// <summary>
        /// Loại phát hành
        /// </summary>
        [MaxLength(50, ErrorMessage = "Loại phát hành không được vượt quá 50 ký tự")]
        public string? LOAI_PHAT_HANH { get; set; }
    }

    /// <summary>
    /// DPDA Details DTO - Để hiển thị chi tiết đầy đủ thẻ nộp đơn gửi tiết kiệm
    /// Bao gồm tất cả 13 business columns + metadata
    /// </summary>
    public class DPDADetailsDto
    {
        /// <summary>
        /// ID hệ thống
        /// </summary>
        public long Id { get; set; }

        // === 13 BUSINESS COLUMNS theo CSV structure ===

        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY
        /// </summary>
        public string MA_CHI_NHANH { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - BUSINESS KEY
        /// </summary>
        public string MA_KHACH_HANG { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TEN_KHACH_HANG { get; set; }

        /// <summary>
        /// Số tài khoản - BUSINESS KEY
        /// </summary>
        public string SO_TAI_KHOAN { get; set; } = string.Empty;

        /// <summary>
        /// Loại thẻ
        /// </summary>
        public string? LOAI_THE { get; set; }

        /// <summary>
        /// Số thẻ - BUSINESS KEY
        /// </summary>
        public string? SO_THE { get; set; }

        /// <summary>
        /// Ngày nộp đơn
        /// </summary>
        public DateTime? NGAY_NOP_DON { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public DateTime? NGAY_PHAT_HANH { get; set; }

        /// <summary>
        /// User phát hành
        /// </summary>
        public string? USER_PHAT_HANH { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string? TRANG_THAI { get; set; }

        /// <summary>
        /// Phân loại
        /// </summary>
        public string? PHAN_LOAI { get; set; }

        /// <summary>
        /// Giao thẻ
        /// </summary>
        public string? GIAO_THE { get; set; }

        /// <summary>
        /// Loại phát hành
        /// </summary>
        public string? LOAI_PHAT_HANH { get; set; }

        // === SYSTEM/METADATA COLUMNS ===

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Tên file import
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Import batch ID
        /// </summary>
        public Guid? ImportId { get; set; }

        /// <summary>
        /// Import metadata
        /// </summary>
        public string? ImportMetadata { get; set; }
    }

    /// <summary>
    /// DPDA Summary DTO - Để hiển thị thống kê tổng quan thẻ nộp đơn gửi tiết kiệm
    /// Analytics và reporting data
    /// </summary>
    public class DPDASummaryDto
    {
        /// <summary>
        /// Tổng số thẻ
        /// </summary>
        public long TotalCards { get; set; }

        /// <summary>
        /// Số thẻ theo trạng thái
        /// </summary>
        public Dictionary<string, long> CardsByStatus { get; set; } = new();

        /// <summary>
        /// Số thẻ theo loại thẻ
        /// </summary>
        public Dictionary<string, long> CardsByType { get; set; } = new();

        /// <summary>
        /// Số thẻ theo chi nhánh (Top 10)
        /// </summary>
        public Dictionary<string, long> CardsByBranch { get; set; } = new();

        /// <summary>
        /// Số thẻ theo phân loại
        /// </summary>
        public Dictionary<string, long> CardsByCategory { get; set; } = new();

        /// <summary>
        /// Số thẻ phát hành trong tháng
        /// </summary>
        public long CardsIssuedThisMonth { get; set; }

        /// <summary>
        /// Số thẻ phát hành hôm nay
        /// </summary>
        public long CardsIssuedToday { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối của summary
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DPDA Import Result DTO - Kết quả import CSV file
    /// Tracking và error reporting cho import process
    /// </summary>
    public class DPDAImportResultDto
    {
        /// <summary>
        /// Import có thành công không
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Tổng số dòng trong file CSV
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Số dòng import thành công
        /// </summary>
        public int SuccessfulRows { get; set; }

        /// <summary>
        /// Số dòng bị lỗi
        /// </summary>
        public int ErrorRows { get; set; }

        /// <summary>
        /// Số dòng bị duplicate (đã tồn tại)
        /// </summary>
        public int DuplicateRows { get; set; }

        /// <summary>
        /// Thời gian import (milliseconds)
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Import batch ID để tracking
        /// </summary>
        public Guid ImportId { get; set; }

        /// <summary>
        /// Tên file đã import
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// User thực hiện import
        /// </summary>
        public string ImportedBy { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian import
        /// </summary>
        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Danh sách lỗi chi tiết (nếu có)
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Danh sách cảnh báo (nếu có)
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Chi tiết lỗi theo từng dòng
        /// Key: Row number, Value: Error message
        /// </summary>
        public Dictionary<int, string> RowErrors { get; set; } = new();

        /// <summary>
        /// Metadata bổ sung về quá trình import
        /// </summary>
        public string? AdditionalInfo { get; set; }
    }
}
