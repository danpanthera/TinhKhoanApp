using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Interface cho GL41 Repository - mở rộng từ IRepository
    /// </summary>
    public interface IGL41Repository : IRepository<GL41>
    {
        /// <summary>
        /// Lấy dữ liệu GL41 gần đây nhất
        /// </summary>
        new Task<IEnumerable<GL41>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu GL41 theo ngày dữ liệu (NGAY_DL)
        /// </summary>
        Task<IEnumerable<GL41>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu GL41 theo mã đơn vị
        /// </summary>
        Task<IEnumerable<GL41>> GetByUnitCodeAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL41 theo mã tài khoản
        /// </summary>
        Task<IEnumerable<GL41>> GetByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Lấy tổng dư đầu kỳ theo mã đơn vị
        /// </summary>
        Task<decimal> GetTotalOpeningBalanceByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng dư cuối kỳ theo mã đơn vị
        /// </summary>
        Task<decimal> GetTotalClosingBalanceByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng phát sinh theo mã đơn vị
        /// </summary>
        Task<(decimal Debit, decimal Credit)> GetTotalTransactionsByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Cập nhật nhiều GL41
        /// </summary>
        void UpdateRange(IEnumerable<GL41> entities);
    }
}
