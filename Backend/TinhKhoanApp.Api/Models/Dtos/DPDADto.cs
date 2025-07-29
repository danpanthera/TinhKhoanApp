namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu DPDA - hiển thị thông tin cơ bản
    /// </summary>
    public class DPDAPreviewDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu (từ filename)
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string MA_CHI_NHANH { get; set; } = "";

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string MA_KHACH_HANG { get; set; } = "";

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string TEN_KHACH_HANG { get; set; } = "";

        /// <summary>
        /// Số tài khoản
        /// </summary>
        public string SO_TAI_KHOAN { get; set; } = "";

        /// <summary>
        /// Loại thẻ
        /// </summary>
        public string LOAI_THE { get; set; } = "";

        /// <summary>
        /// Số thẻ
        /// </summary>
        public string SO_THE { get; set; } = "";

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string TRANG_THAI { get; set; } = "";

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public DateTime? NGAY_PHAT_HANH { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ DPDA
    /// </summary>
    public class DPDADetailDto : DPDAPreviewDto
    {
        /// <summary>
        /// Ngày nộp đơn
        /// </summary>
        public DateTime? NGAY_NOP_DON { get; set; }

        /// <summary>
        /// User phát hành
        /// </summary>
        public string USER_PHAT_HANH { get; set; } = "";

        /// <summary>
        /// Phân loại
        /// </summary>
        public string PHAN_LOAI { get; set; } = "";

        /// <summary>
        /// Giao thẻ
        /// </summary>
        public string GIAO_THE { get; set; } = "";

        /// <summary>
        /// Loại phát hành
        /// </summary>
        public string LOAI_PHAT_HANH { get; set; } = "";

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime CREATED_DATE { get; set; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất
        /// </summary>
        public DateTime UPDATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO thống kê tổng hợp DPDA
    /// </summary>
    public class DPDASummaryDto
    {
        /// <summary>
        /// Tổng số thẻ
        /// </summary>
        public int TotalCards { get; set; }

        /// <summary>
        /// Số thẻ đang hoạt động
        /// </summary>
        public int ActiveCards { get; set; }

        /// <summary>
        /// Số thẻ đã phát hành
        /// </summary>
        public int IssuedCards { get; set; }

        /// <summary>
        /// Số thẻ đang chờ duyệt
        /// </summary>
        public int PendingCards { get; set; }

        /// <summary>
        /// Số thẻ bị từ chối/hủy
        /// </summary>
        public int RejectedCards { get; set; }

        /// <summary>
        /// Thời gian trung bình xử lý (ngày)
        /// </summary>
        public double? AverageProcessingDays { get; set; }

        /// <summary>
        /// Phân loại theo loại thẻ
        /// </summary>
        public Dictionary<string, int> CardTypeDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Mã đơn vị (nếu tìm theo đơn vị)
        /// </summary>
        public string? BranchCode { get; set; }

        /// <summary>
        /// Ngày dữ liệu (nếu tìm theo ngày)
        /// </summary>
        public DateTime? Date { get; set; }
    }
}
