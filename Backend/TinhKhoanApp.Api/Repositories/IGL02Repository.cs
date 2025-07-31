using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository interface cho GL02 - Giao dịch sổ cái
    /// </summary>
    public interface IGL02Repository : IRepository<GL02>
    {
        /// <summary>
        /// Lấy dữ liệu GL02 gần đây theo số lượng chỉ định
        /// </summary>
        new Task<IEnumerable<GL02>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu GL02 theo ngày
        /// </summary>
        Task<IEnumerable<GL02>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã đơn vị
        /// </summary>
        Task<IEnumerable<GL02>> GetByUnitAsync(string unit, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<GL02>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo tài khoản
        /// </summary>
        Task<IEnumerable<GL02>> GetByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã giao dịch
        /// </summary>
        Task<IEnumerable<GL02>> GetByTransactionCodeAsync(string transactionCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<GL02>> GetByCustomerAsync(string customer, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL02 theo loại giao dịch (DR/CR)
        /// </summary>
        Task<IEnumerable<GL02>> GetByTransactionTypeAsync(string transactionType, int maxResults = 100);

        /// <summary>
        /// Lấy tổng giao dịch theo đơn vị và loại (DR/CR)
        /// </summary>
        Task<decimal> GetTotalTransactionsByUnitAsync(string unit, string drCrFlag);

        /// <summary>
        /// Lấy tổng giao dịch theo ngày và loại (DR/CR)
        /// </summary>
        Task<decimal> GetTotalTransactionsByDateAsync(DateTime date, string drCrFlag);

        /// <summary>
        /// Lấy tổng giao dịch theo chi nhánh và loại (DR/CR)
        /// </summary>
        Task<decimal> GetTotalTransactionsByBranchAsync(string branchCode, string drCrFlag);

        /// <summary>
        /// Lấy chi tiết giao dịch theo ID
        /// </summary>
        Task<GL02?> GetByIdAsync(long id);

        /// <summary>
        /// Lấy dữ liệu GL02 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<GL02>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);
    }
}
