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
        public long ID { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? BRCD { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string? DEPCD { get; set; }

        /// <summary>
        /// Mã tài khoản giao dịch
        /// </summary>
        public string? TRAD_ACCT { get; set; }

        /// <summary>
        /// Số tiền giao dịch
        /// </summary>
        public decimal? TR_AMOUNT { get; set; }

        /// <summary>
        /// Loại giao dịch (DR/CR)
        /// </summary>
        public string? DR_CR_FLG { get; set; }

        /// <summary>
        /// Loại tiền tệ
        /// </summary>
        public string? CCY { get; set; }

        /// <summary>
        /// Mô tả giao dịch
        /// </summary>
        public string? TR_DESC { get; set; }

        /// <summary>
        /// Mã giao dịch
        /// </summary>
        public string? TR_CD { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime? CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ GL01
    /// </summary>
    public class GL01DetailDto : GL01PreviewDto
    {
        /// <summary>
        /// Ngày hiệu lực
        /// </summary>
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? CIF { get; set; }

        /// <summary>
        /// Mã tham chiếu
        /// </summary>
        public string? REF_NO { get; set; }

        /// <summary>
        /// Số chứng từ
        /// </summary>
        public string? VOUCHER_NO { get; set; }

        /// <summary>
        /// Mã tài khoản 1
        /// </summary>
        public string? ACCTNO1 { get; set; }

        /// <summary>
        /// Mã tài khoản 2
        /// </summary>
        public string? ACCTNO2 { get; set; }

        /// <summary>
        /// Tỷ giá
        /// </summary>
        public decimal? EXCH_RATE { get; set; }

        /// <summary>
        /// Số tiền quy đổi
        /// </summary>
        public decimal? CONV_AMT { get; set; }

        /// <summary>
        /// Số tiền VNĐ
        /// </summary>
        public decimal? VND_AMT { get; set; }

        /// <summary>
        /// Mã người duyệt
        /// </summary>
        public string? APPR_ID { get; set; }

        /// <summary>
        /// Trạng thái giao dịch
        /// </summary>
        public string? STATUS { get; set; }

        /// <summary>
        /// Thời điểm cập nhật
        /// </summary>
        public DateTime? UPDATED_DATE { get; set; }
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
