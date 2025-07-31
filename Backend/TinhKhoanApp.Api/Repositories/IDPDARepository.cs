using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Interface Repository cho DPDA - mở rộng từ IRepository generic
    /// </summary>
    public interface IDPDARepository : IRepository<DPDA>
    {
        /// <summary>
        /// Lấy dữ liệu DPDA mới nhất
        /// </summary>
        Task<IEnumerable<DPDA>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu DPDA theo ngày
        /// </summary>
        Task<IEnumerable<DPDA>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu DPDA theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<DPDA>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo mã khách hàng
        /// </summary>
        Task<IEnumerable<DPDA>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo số tài khoản
        /// </summary>
        Task<IEnumerable<DPDA>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo số thẻ
        /// </summary>
        Task<IEnumerable<DPDA>> GetByCardNumberAsync(string cardNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo trạng thái
        /// </summary>
        Task<IEnumerable<DPDA>> GetByStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy số lượng thẻ theo chi nhánh và trạng thái
        /// </summary>
        Task<int> GetCardCountByBranchAndStatusAsync(string branchCode, string status);

        /// <summary>
        /// Cập nhật nhiều DPDA
        /// </summary>
        void UpdateRange(IEnumerable<DPDA> entities);
    }
}
