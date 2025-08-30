namespace TinhKhoanApp.Api.Models.ViewModels
{
    /// <summary>
    /// ViewModel hiển thị tóm tắt dữ liệu GL41 trên Dashboard
    /// </summary>
    public class GL41DashboardViewModel
    {
        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Tổng số bản ghi GL41
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Tổng số đơn vị có dữ liệu
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// Tổng số tài khoản có dữ liệu
        /// </summary>
        public int TotalAccounts { get; set; }

        /// <summary>
        /// Ngày nhập dữ liệu gần nhất
        /// </summary>
        public DateTime LatestImportDate { get; set; }

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
        /// Cân đối đầu kỳ (nợ - có)
        /// </summary>
        public decimal BalanceOpening => TotalDebitOpening - TotalCreditOpening;

        /// <summary>
        /// Cân đối phát sinh (nợ - có)
        /// </summary>
        public decimal BalanceTransaction => TotalDebitTransaction - TotalCreditTransaction;

        /// <summary>
        /// Cân đối cuối kỳ (nợ - có)
        /// </summary>
        public decimal BalanceClosing => TotalDebitClosing - TotalCreditClosing;

        /// <summary>
        /// Cân đối tổng cộng
        /// </summary>
        public bool IsBalanced =>
            Math.Abs(BalanceClosing - (BalanceOpening + BalanceTransaction)) < 0.01m;

        /// <summary>
        /// Top 5 đơn vị có số dư lớn nhất
        /// </summary>
        public List<UnitBalanceSummary> TopUnitsByBalance { get; set; } = new();
    }

    /// <summary>
    /// Thông tin tóm tắt số dư theo đơn vị
    /// </summary>
    public class UnitBalanceSummary
    {
        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string? MA_CN { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string? TEN_CN { get; set; }

        /// <summary>
        /// Số dư cuối kỳ (Nợ - Có)
        /// </summary>
        public decimal ClosingBalance { get; set; }
    }
}
