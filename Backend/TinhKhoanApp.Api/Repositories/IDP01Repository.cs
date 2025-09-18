using Khoan.Api.Models.Entities;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Interface cho DP01 Repository - mở rộng từ IRepository
    /// CSV Business Columns: 63 columns from 7800_dp01_20241231.csv
    /// </summary>
    public interface IDP01Repository : IRepository<DP01Entity>
    {
        /// <summary>
        /// Lấy dữ liệu DP01 gần đây nhất
        /// </summary>
        new Task<IEnumerable<DP01Entity>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu DP01 theo ngày dữ liệu (NGAY_DL)
        /// </summary>
        Task<IEnumerable<DP01Entity>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu DP01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<DP01Entity>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DP01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<DP01Entity>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DP01 theo số tài khoản
        /// </summary>
        Task<IEnumerable<DP01Entity>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy tổng số dư theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);
    }
}
