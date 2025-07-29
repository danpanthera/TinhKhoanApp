using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Interface cho DP01 Repository - mở rộng từ IRepository
    /// </summary>
    public interface IDP01Repository : IRepository<DP01>
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
        /// Lấy tổng số dư theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);
    }
}
