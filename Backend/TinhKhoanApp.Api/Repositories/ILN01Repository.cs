using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository interface cho LN01 - Thông tin khoản vay
    /// </summary>
    public interface ILN01Repository : IRepository<LN01>
    {
        /// <summary>
        /// Lấy dữ liệu LN01 gần đây theo số lượng chỉ định
        /// </summary>
        Task<IEnumerable<LN01>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu LN01 theo ngày
        /// </summary>
        Task<IEnumerable<LN01>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu LN01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<LN01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu LN01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<LN01>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu LN01 theo số tài khoản
        /// </summary>
        Task<IEnumerable<LN01>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy tổng dư nợ theo mã chi nhánh
        /// </summary>
        Task<decimal> GetTotalLoanAmountByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng dư nợ theo loại tiền
        /// </summary>
        Task<decimal> GetTotalLoanAmountByCurrencyAsync(string currency, DateTime? date = null);

        /// <summary>
        /// Lấy dữ liệu LN01 theo nhóm nợ
        /// </summary>
        Task<IEnumerable<LN01>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu LN01 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<LN01>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy chi tiết LN01 theo ID
        /// </summary>
        Task<LN01?> GetByIdAsync(long id);

        /// <summary>
        /// Trả về DbContext cho việc sử dụng trong service layer
        /// </summary>
        /// <returns>ApplicationDbContext instance</returns>
        ApplicationDbContext GetDbContext();
    }
}
