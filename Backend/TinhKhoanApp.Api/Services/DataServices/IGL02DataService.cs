using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho GL02 Data Service - xử lý business logic liên quan đến dữ liệu GL02 (Giao dịch sổ cái)
    /// </summary>
    public interface IGL02DataService
    {
        /// <summary>
        /// Lấy preview dữ liệu GL02
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi GL02
        /// </summary>
        Task<GL02DetailDto?> GetGL02DetailAsync(long id);

        /// <summary>
        /// Lấy dữ liệu GL02 theo ngày
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã đơn vị
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByUnitAsync(string unit, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã tài khoản
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByCustomerAsync(string customer, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo loại giao dịch
        /// </summary>
        Task<IEnumerable<GL02PreviewDto>> GetGL02ByTransactionTypeAsync(string transactionType, int maxResults = 100);

        /// <summary>
        /// Lấy tổng hợp GL02 theo ngày
        /// </summary>
        Task<GL02SummaryDto> GetGL02SummaryByDateAsync(DateTime date);

        /// <summary>
        /// Lấy tổng hợp GL02 theo chi nhánh
        /// </summary>
        Task<GL02SummaryDto> GetGL02SummaryByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng hợp GL02 theo đơn vị
        /// </summary>
        Task<GL02SummaryDto> GetGL02SummaryByUnitAsync(string unit, DateTime? date = null);

        /// <summary>
        /// Tìm kiếm GL02 theo nhiều tiêu chí
        /// </summary>
        Task<PagedApiResponse<GL02PreviewDto>> SearchGL02Async(
            string? keyword,
            string? branchCode,
            string? unit,
            string? accountCode,
            string? customer,
            DateTime? fromDate,
            DateTime? toDate,
            string? transactionType,
            int page = 1,
            int pageSize = 20);
    }
}
