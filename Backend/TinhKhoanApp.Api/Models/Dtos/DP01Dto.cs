namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu DP01 - hiển thị thông tin cơ bản
    /// </summary>
    public class DP01PreviewDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu (từ filename)
        /// </summary>
        public DateTime? NGAY_DL { get; set; }

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
        /// Loại tiền tệ
        /// </summary>
        public string? CCY { get; set; }

        /// <summary>
        /// Số dư hiện tại
        /// </summary>
        public decimal? CURRENT_BALANCE { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        public decimal? RATE { get; set; }

        /// <summary>
        /// Số tài khoản
        /// </summary>
        public string? SO_TAI_KHOAN { get; set; }

        /// <summary>
        /// Ngày mở tài khoản
        /// </summary>
        public DateTime? OPENING_DATE { get; set; }

        /// <summary>
        /// Ngày đáo hạn
        /// </summary>
        public DateTime? MATURITY_DATE { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ DP01
    /// </summary>
    public class DP01DetailDto : DP01PreviewDto
    {
        // Thêm các trường khác từ DP01 nếu cần

        /// <summary>
        /// Địa chỉ khách hàng
        /// </summary>
        public string? ADDRESS { get; set; }

        /// <summary>
        /// Số chứng từ
        /// </summary>
        public string? NOTENO { get; set; }

        /// <summary>
        /// Kỳ hạn theo tháng
        /// </summary>
        public string? MONTH_TERM { get; set; }

        /// <summary>
        /// Tên kỳ hạn tiền gửi
        /// </summary>
        public string? TERM_DP_NAME { get; set; }

        /// <summary>
        /// Tên thời gian tiền gửi
        /// </summary>
        public string? TIME_DP_NAME { get; set; }

        /// <summary>
        /// Mã phòng giao dịch
        /// </summary>
        public string? MA_PGD { get; set; }

        /// <summary>
        /// Tên phòng giao dịch
        /// </summary>
        public string? TEN_PGD { get; set; }

        /// <summary>
        /// Nguồn dữ liệu import
        /// </summary>
        public string? DataSource { get; set; }

        /// <summary>
        /// Thời điểm import
        /// </summary>
        public DateTime ImportDateTime { get; set; }
    }
}
