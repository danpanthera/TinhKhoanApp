namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu GL41 - hiển thị thông tin cơ bản
    /// </summary>
    public class GL41PreviewDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Ngày dữ liệu (từ filename)
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MA_CN { get; set; }

        /// <summary>
        /// Tài khoản kế toán
        /// </summary>
        public string? MA_TK { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Loại tiền tệ
        /// </summary>
        public string? CCY { get; set; }

        /// <summary>
        /// Dư nợ đầu kỳ
        /// </summary>
        public decimal? DN_DAUKY { get; set; }

        /// <summary>
        /// Dư có đầu kỳ
        /// </summary>
        public decimal? DC_DAUKY { get; set; }

        /// <summary>
        /// Phát sinh ghi nợ
        /// </summary>
        public decimal? ST_GHINO { get; set; }

        /// <summary>
        /// Phát sinh ghi có
        /// </summary>
        public decimal? ST_GHICO { get; set; }

        /// <summary>
        /// Dư nợ cuối kỳ
        /// </summary>
        public decimal? DN_CUOIKY { get; set; }

        /// <summary>
        /// Dư có cuối kỳ
        /// </summary>
        public decimal? DC_CUOIKY { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime CREATED_DATE { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết đầy đủ GL41
    /// </summary>
    public class GL41DetailDto : GL41PreviewDto
    {
        /// <summary>
        /// Số dư cuối kỳ (DN_CUOIKY - DC_CUOIKY)
        /// </summary>
        public decimal ClosingBalance => (DN_CUOIKY ?? 0) - (DC_CUOIKY ?? 0);

        /// <summary>
        /// Số dư đầu kỳ (DN_DAUKY - DC_DAUKY)
        /// </summary>
        public decimal OpeningBalance => (DN_DAUKY ?? 0) - (DC_DAUKY ?? 0);

        /// <summary>
        /// Tổng phát sinh ròng (ST_GHINO - ST_GHICO)
        /// </summary>
        public decimal NetTransaction => (ST_GHINO ?? 0) - (ST_GHICO ?? 0);

        /// <summary>
        /// Tên file import
        /// </summary>
        public string? FILE_NAME { get; set; }

        /// <summary>
        /// Mã batch import
        /// </summary>
        public string? BATCH_ID { get; set; }

        /// <summary>
        /// Mã phiên import
        /// </summary>
        public string? IMPORT_SESSION_ID { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê tổng hợp GL41
    /// </summary>
    public class GL41SummaryDto
    {
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MA_CN { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Tổng dư nợ đầu kỳ
        /// </summary>
        public decimal TotalDebitOpening { get; set; }

        /// <summary>
        /// Tổng dư có đầu kỳ
        /// </summary>
        public decimal TotalCreditOpening { get; set; }

        /// <summary>
        /// Tổng phát sinh nợ
        /// </summary>
        public decimal TotalDebitTransaction { get; set; }

        /// <summary>
        /// Tổng phát sinh có
        /// </summary>
        public decimal TotalCreditTransaction { get; set; }

        /// <summary>
        /// Tổng dư nợ cuối kỳ
        /// </summary>
        public decimal TotalDebitClosing { get; set; }

        /// <summary>
        /// Tổng dư có cuối kỳ
        /// </summary>
        public decimal TotalCreditClosing { get; set; }

        /// <summary>
        /// Số lượng tài khoản
        /// </summary>
        public int AccountCount { get; set; }
    }
}
