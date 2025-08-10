using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Interface cho GL01 Repository - mở rộng từ IRepository
    /// </summary>
    public interface IGL01Repository : IRepository<GL01>
    {
        /// <summary>
        /// Lấy dữ liệu GL01 gần đây nhất
        /// </summary>
        new Task<IEnumerable<GL01>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu GL01 theo ngày dữ liệu (NGAY_DL)
        /// </summary>
        Task<IEnumerable<GL01>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu GL01 theo mã đơn vị
        /// </summary>
        Task<IEnumerable<GL01>> GetByUnitCodeAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL01 theo tài khoản
        /// </summary>
        Task<IEnumerable<GL01>> GetByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Lấy tổng giao dịch theo mã đơn vị và loại giao dịch (DR/CR)
        /// </summary>
        Task<decimal> GetTotalTransactionsByUnitAsync(string unitCode, string drCrFlag, DateTime? date = null);

        /// <summary>
        /// Lấy tổng giao dịch theo ngày và loại giao dịch (DR/CR)
        /// </summary>
        Task<decimal> GetTotalTransactionsByDateAsync(DateTime date, string drCrFlag);

        /// <summary>
        /// Cập nhật nhiều GL01
        /// </summary>
        void UpdateRange(IEnumerable<GL01> entities);
    }
}
