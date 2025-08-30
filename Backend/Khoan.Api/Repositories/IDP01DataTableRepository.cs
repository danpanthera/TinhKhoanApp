using Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Interface cho DP01 Repository - sử dụng DataTables.DP01 model
    /// Chuẩn hóa: CSV (63 columns) -> Database -> DataTables.DP01 -> Repository -> Service -> Controller
    /// </summary>
    public interface IDP01DataTableRepository : IRepository<DP01>
    {
        /// <summary>
        /// Lấy dữ liệu DP01 gần đây nhất
        /// </summary>
        new Task<IEnumerable<DP01>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu DP01 theo ngày dữ liệu (NGAY_DL)
        /// </summary>
        Task<IEnumerable<DP01>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu DP01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<DP01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DP01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<DP01>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DP01 theo số tài khoản
        /// </summary>
        Task<IEnumerable<DP01>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Tính tổng số dư theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy dữ liệu DP01 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<DP01>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Lấy dữ liệu DP01 theo loại tiền tệ
        /// </summary>
        Task<IEnumerable<DP01>> GetByCurrencyAsync(string currency);

        /// <summary>
        /// Tìm kiếm dữ liệu DP01 theo từ khóa
        /// </summary>
        Task<IEnumerable<DP01>> SearchAsync(string searchTerm);

        /// <summary>
        /// Đếm số bản ghi theo khoảng thời gian
        /// </summary>
        Task<int> GetCountByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Bulk insert dữ liệu DP01
        /// </summary>
        Task<int> BulkCreateAsync(IEnumerable<DP01> dp01Records);

        /// <summary>
        /// Lấy thống kê tổng hợp
        /// </summary>
        Task<Dictionary<string, object>> GetStatisticsAsync();
    }
}
