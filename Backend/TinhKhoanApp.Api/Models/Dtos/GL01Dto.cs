namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu GL01 - hiển thị thông tin cơ bản
    /// Đồng bộ với cấu trúc CSV thực tế (27 business columns)
    /// </summary>
    public class GL01PreviewDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu (từ TR_TIME)
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string? STS { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime? NGAY_GD { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? NGUOI_TAO { get; set; }

        /// <summary>
        /// Số thứ tự trong ngày
        /// </summary>
        public string? DYSEQ { get; set; }

        /// <summary>
        /// Loại giao dịch
        /// </summary>
        public string? TR_TYPE { get; set; }

        /// <summary>
        /// Số thứ tự chi tiết
        /// </summary>
        public string? DT_SEQ { get; set; }

        /// <summary>
        /// Tài khoản
        /// </summary>
        public string? TAI_KHOAN { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Số tiền giao dịch
        /// </summary>
        public decimal? SO_TIEN_GD { get; set; }

        /// <summary>
        /// Chi nhánh post
        /// </summary>
        public string? POST_BR { get; set; }

        /// <summary>
        /// Loại tiền
        /// </summary>
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Ghi nợ/có (DR/CR)
        /// </summary>
        public string? DR_CR { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TEN_KH { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime? CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO đầy đủ cho dữ liệu GL01 - tất cả 27 business columns
    /// Đồng bộ hoàn toàn với cấu trúc CSV thực tế
    /// </summary>
    public class GL01FullDto
    {
        /// <summary>
        /// ID bản ghi (tự động tăng)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu (từ TR_TIME)
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string? STS { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime? NGAY_GD { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? NGUOI_TAO { get; set; }

        /// <summary>
        /// Số thứ tự trong ngày
        /// </summary>
        public string? DYSEQ { get; set; }

        /// <summary>
        /// Loại giao dịch
        /// </summary>
        public string? TR_TYPE { get; set; }

        /// <summary>
        /// Số thứ tự chi tiết
        /// </summary>
        public string? DT_SEQ { get; set; }

        /// <summary>
        /// Tài khoản
        /// </summary>
        public string? TAI_KHOAN { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Số tiền giao dịch
        /// </summary>
        public decimal? SO_TIEN_GD { get; set; }

        /// <summary>
        /// Chi nhánh post
        /// </summary>
        public string? POST_BR { get; set; }

        /// <summary>
        /// Loại tiền
        /// </summary>
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Ghi nợ/có (DR/CR)
        /// </summary>
        public string? DR_CR { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TEN_KH { get; set; }

        /// <summary>
        /// ID người dùng CCA
        /// </summary>
        public string? CCA_USRID { get; set; }

        /// <summary>
        /// Tỷ lệ trao đổi giao dịch
        /// </summary>
        public decimal? TR_EX_RT { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? REMARK { get; set; }

        /// <summary>
        /// Mã nghiệp vụ
        /// </summary>
        public string? BUS_CODE { get; set; }

        /// <summary>
        /// Mã đơn vị nghiệp vụ
        /// </summary>
        public string? UNIT_BUS_CODE { get; set; }

        /// <summary>
        /// Mã giao dịch
        /// </summary>
        public string? TR_CODE { get; set; }

        /// <summary>
        /// Tên giao dịch
        /// </summary>
        public string? TR_NAME { get; set; }

        /// <summary>
        /// Số tham chiếu
        /// </summary>
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Ngày giá trị
        /// </summary>
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string? DEPT_CODE { get; set; }

        /// <summary>
        /// Thời gian giao dịch (từ CSV)
        /// </summary>
        public string? TR_TIME { get; set; }

        /// <summary>
        /// Xác nhận
        /// </summary>
        public string? COMFIRM { get; set; }

        /// <summary>
        /// Thời gian giao dịch chi tiết
        /// </summary>
        public string? TRDT_TIME { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime? CREATED_DATE { get; set; }

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string? CREATED_BY { get; set; }

        /// <summary>
        /// Thời điểm cập nhật
        /// </summary>
        public DateTime? UPDATED_DATE { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public string? UPDATED_BY { get; set; }
    }

    /// <summary>
    /// DTO chi tiết GL01 - mở rộng từ Preview với các trường bổ sung
    /// </summary>
    public class GL01DetailDto : GL01PreviewDto
    {
        /// <summary>
        /// ID người dùng CCA
        /// </summary>
        public string? CCA_USRID { get; set; }

        /// <summary>
        /// Tỷ lệ trao đổi giao dịch
        /// </summary>
        public decimal? TR_EX_RT { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? REMARK { get; set; }

        /// <summary>
        /// Mã nghiệp vụ
        /// </summary>
        public string? BUS_CODE { get; set; }

        /// <summary>
        /// Mã đơn vị nghiệp vụ
        /// </summary>
        public string? UNIT_BUS_CODE { get; set; }

        /// <summary>
        /// Mã giao dịch
        /// </summary>
        public string? TR_CODE { get; set; }

        /// <summary>
        /// Tên giao dịch
        /// </summary>
        public string? TR_NAME { get; set; }

        /// <summary>
        /// Số tham chiếu
        /// </summary>
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Ngày giá trị
        /// </summary>
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string? DEPT_CODE { get; set; }

        /// <summary>
        /// Thời gian giao dịch (từ CSV)
        /// </summary>
        public string? TR_TIME { get; set; }

        /// <summary>
        /// Xác nhận
        /// </summary>
        public string? COMFIRM { get; set; }

        /// <summary>
        /// Thời gian giao dịch chi tiết
        /// </summary>
        public string? TRDT_TIME { get; set; }
    }

    /// <summary>
    /// DTO thống kê tổng hợp GL01 - theo cấu trúc CSV thực tế
    /// </summary>
    public class GL01SummaryDto
    {
        /// <summary>
        /// Tổng số giao dịch
        /// </summary>
        public int TotalTransactions { get; set; }

        /// <summary>
        /// Tổng số giao dịch ghi nợ (DR)
        /// </summary>
        public int TotalDebitTransactions { get; set; }

        /// <summary>
        /// Tổng số giao dịch ghi có (CR)
        /// </summary>
        public int TotalCreditTransactions { get; set; }

        /// <summary>
        /// Tổng giá trị giao dịch ghi nợ
        /// </summary>
        public decimal TotalDebitAmount { get; set; }

        /// <summary>
        /// Tổng giá trị giao dịch ghi có
        /// </summary>
        public decimal TotalCreditAmount { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Chi nhánh Post (POST_BR)
        /// </summary>
        public string? PostBranch { get; set; }

        /// <summary>
        /// Mã phòng ban (DEPT_CODE)
        /// </summary>
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// Loại tiền chủ yếu (LOAI_TIEN)
        /// </summary>
        public string? PrimaryCurrency { get; set; }

        /// <summary>
        /// Thời gian từ
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Thời gian đến
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}
