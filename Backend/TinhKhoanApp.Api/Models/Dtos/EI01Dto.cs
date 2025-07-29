namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu EI01 - hiển thị thông tin cơ bản
    /// </summary>
    public class EI01PreviewDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
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
        /// Số điện thoại đăng ký EMB
        /// </summary>
        public string? SDT_EMB { get; set; }

        /// <summary>
        /// Trạng thái dịch vụ EMB
        /// </summary>
        public string? TRANG_THAI_EMB { get; set; }

        /// <summary>
        /// Số điện thoại đăng ký OTT
        /// </summary>
        public string? SDT_OTT { get; set; }

        /// <summary>
        /// Trạng thái dịch vụ OTT
        /// </summary>
        public string? TRANG_THAI_OTT { get; set; }

        /// <summary>
        /// Số điện thoại đăng ký SMS
        /// </summary>
        public string? SDT_SMS { get; set; }

        /// <summary>
        /// Trạng thái dịch vụ SMS
        /// </summary>
        public string? TRANG_THAI_SMS { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ EI01
    /// </summary>
    public class EI01DetailDto : EI01PreviewDto
    {
        /// <summary>
        /// Ngày đăng ký dịch vụ EMB
        /// </summary>
        public DateTime? NGAY_DK_EMB { get; set; }

        /// <summary>
        /// Ngày đăng ký dịch vụ OTT
        /// </summary>
        public DateTime? NGAY_DK_OTT { get; set; }

        /// <summary>
        /// Ngày đăng ký dịch vụ SMS
        /// </summary>
        public DateTime? NGAY_DK_SMS { get; set; }

        /// <summary>
        /// Số điện thoại đăng ký SAV
        /// </summary>
        public string? SDT_SAV { get; set; }

        /// <summary>
        /// Trạng thái dịch vụ SAV
        /// </summary>
        public string? TRANG_THAI_SAV { get; set; }

        /// <summary>
        /// Ngày đăng ký dịch vụ SAV
        /// </summary>
        public DateTime? NGAY_DK_SAV { get; set; }

        /// <summary>
        /// Số điện thoại đăng ký LN
        /// </summary>
        public string? SDT_LN { get; set; }

        /// <summary>
        /// Trạng thái dịch vụ LN
        /// </summary>
        public string? TRANG_THAI_LN { get; set; }

        /// <summary>
        /// Ngày đăng ký dịch vụ LN
        /// </summary>
        public DateTime? NGAY_DK_LN { get; set; }

        /// <summary>
        /// User xử lý dịch vụ EMB
        /// </summary>
        public string? USER_EMB { get; set; }

        /// <summary>
        /// User xử lý dịch vụ OTT
        /// </summary>
        public string? USER_OTT { get; set; }

        /// <summary>
        /// User xử lý dịch vụ SMS
        /// </summary>
        public string? USER_SMS { get; set; }

        /// <summary>
        /// User xử lý dịch vụ SAV
        /// </summary>
        public string? USER_SAV { get; set; }

        /// <summary>
        /// User xử lý dịch vụ LN
        /// </summary>
        public string? USER_LN { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO thống kê tổng hợp EI01
    /// </summary>
    public class EI01SummaryDto
    {
        /// <summary>
        /// Tổng số khách hàng
        /// </summary>
        public int TotalCustomers { get; set; }

        /// <summary>
        /// Số lượng khách hàng đăng ký EMB
        /// </summary>
        public int EMBRegistrations { get; set; }

        /// <summary>
        /// Số lượng khách hàng đăng ký OTT
        /// </summary>
        public int OTTRegistrations { get; set; }

        /// <summary>
        /// Số lượng khách hàng đăng ký SMS
        /// </summary>
        public int SMSRegistrations { get; set; }

        /// <summary>
        /// Số lượng khách hàng đăng ký SAV
        /// </summary>
        public int SAVRegistrations { get; set; }

        /// <summary>
        /// Số lượng khách hàng đăng ký LN
        /// </summary>
        public int LNRegistrations { get; set; }

        /// <summary>
        /// Phân loại theo trạng thái dịch vụ EMB
        /// </summary>
        public Dictionary<string, int> EMBStatusDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Phân loại theo trạng thái dịch vụ OTT
        /// </summary>
        public Dictionary<string, int> OTTStatusDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Phân loại theo trạng thái dịch vụ SMS
        /// </summary>
        public Dictionary<string, int> SMSStatusDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Phân loại theo loại khách hàng
        /// </summary>
        public Dictionary<string, int> CustomerTypeDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Mã chi nhánh (nếu tìm theo chi nhánh)
        /// </summary>
        public string? BranchCode { get; set; }

        /// <summary>
        /// Ngày dữ liệu (nếu tìm theo ngày)
        /// </summary>
        public DateTime? Date { get; set; }
    }
}
