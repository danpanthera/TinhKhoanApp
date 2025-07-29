namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu GL01 - hiển thị thông tin cơ bản
    /// </summary>
    public class GL01PreviewDto
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
        public string? POST_BR { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string? DEPT_CODE { get; set; }

        /// <summary>
        /// Mã tài khoản giao dịch
        /// </summary>
        public string? TAI_KHOAN { get; set; }

        /// <summary>
        /// Số tiền giao dịch
        /// </summary>
        public decimal? SO_TIEN_GD { get; set; }

        /// <summary>
        /// Loại giao dịch (DR/CR)
        /// </summary>
        public string? DR_CR { get; set; }

        /// <summary>
        /// Loại tiền tệ
        /// </summary>
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Mô tả giao dịch
        /// </summary>
        public string? TR_NAME { get; set; }

        /// <summary>
        /// Mã giao dịch
        /// </summary>
        public string? TR_CODE { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ GL01
    /// </summary>
    public class GL01DetailDto : GL01PreviewDto
    {
        /// <summary>
        /// Trạng thái giao dịch
        /// </summary>
        public string? STS { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime? NGAY_GD { get; set; }

        /// <summary>
        /// Người tạo giao dịch
        /// </summary>
        public string? NGUOI_TAO { get; set; }

        /// <summary>
        /// Loại giao dịch
        /// </summary>
        public string? TR_TYPE { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TEN_KH { get; set; }

        /// <summary>
        /// Tỷ giá giao dịch
        /// </summary>
        public string? TR_EX_RT { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? REMARK { get; set; }

        /// <summary>
        /// Mã nghiệp vụ
        /// </summary>
        public string? BUS_CODE { get; set; }

        /// <summary>
        /// Mã nghiệp vụ đơn vị
        /// </summary>
        public string? UNIT_BUS_CODE { get; set; }

        /// <summary>
        /// Mã tham chiếu
        /// </summary>
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Ngày hiệu lực
        /// </summary>
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// Thời gian giao dịch
        /// </summary>
        public string? TR_TIME { get; set; }

        /// <summary>
        /// Xác nhận
        /// </summary>
        public string? COMFIRM { get; set; }

        /// <summary>
        /// Thời điểm dữ liệu giao dịch
        /// </summary>
        public string? TRDT_TIME { get; set; }

        /// <summary>
        /// Thời điểm cập nhật
        /// </summary>
        public DateTime UPDATED_DATE { get; set; }

        /// <summary>
        /// Tên file nguồn
        /// </summary>
        public string? FILE_NAME { get; set; }
    }

    /// <summary>
    /// DTO thống kê tổng hợp GL01
    /// </summary>
    public class GL01SummaryDto
    {
        /// <summary>
        /// Tổng số giao dịch
        /// </summary>
        public int TotalTransactions { get; set; }

        /// <summary>
        /// Tổng số giao dịch ghi nợ
        /// </summary>
        public int TotalDebitTransactions { get; set; }

        /// <summary>
        /// Tổng số giao dịch ghi có
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
        /// Mã đơn vị
        /// </summary>
        public string? UnitCode { get; set; }
    }
}
