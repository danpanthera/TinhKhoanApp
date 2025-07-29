namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO thống kê tổng hợp DP01 theo chi nhánh
    /// </summary>
    public class DP01SummaryDto
    {
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MA_CN { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime? NGAY_DL { get; set; }

        /// <summary>
        /// Tổng số tài khoản
        /// </summary>
        public int AccountCount { get; set; }

        /// <summary>
        /// Tổng số dư
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Tổng số tài khoản VND
        /// </summary>
        public int VNDAccountCount { get; set; }

        /// <summary>
        /// Tổng số dư VND
        /// </summary>
        public decimal VNDTotalBalance { get; set; }

        /// <summary>
        /// Tổng số tài khoản ngoại tệ
        /// </summary>
        public int ForeignCurrencyAccountCount { get; set; }

        /// <summary>
        /// Tổng số dư ngoại tệ (quy đổi VND)
        /// </summary>
        public decimal ForeignCurrencyTotalBalance { get; set; }
    }
}
