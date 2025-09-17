using Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Interface cho RR01 Repository - Dữ liệu tỷ giá ngoại tệ
    /// RR01: 25 business columns - Exchange rate data
    /// </summary>
    public interface IRR01Repository : IRepository<RR01>
    {
        /// <summary>
        /// Lấy dữ liệu RR01 gần đây nhất
        /// </summary>
        new Task<IEnumerable<RR01>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu RR01 theo ngày dữ liệu (NGAY_DL)
        /// </summary>
        Task<IEnumerable<RR01>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu RR01 theo mã chi nhánh (MA_CN)
        /// </summary>
        Task<IEnumerable<RR01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu RR01 theo loại tiền (LOAI_TIEN)
        /// </summary>
        Task<IEnumerable<RR01>> GetByCurrencyTypeAsync(string currencyType, int maxResults = 100);

        /// <summary>
        /// Lấy tổng số dư theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Kiểm tra trùng lặp dữ liệu
        /// </summary>
        Task<bool> IsDuplicateAsync(string branchCode, string currencyType, DateTime dataDate);

        /// <summary>
        /// Lấy danh sách chi nhánh có dữ liệu
        /// </summary>
        Task<IEnumerable<string>> GetDistinctBranchCodesAsync(DateTime? date = null);

        /// <summary>
        /// Lấy danh sách loại tiền có dữ liệu
        /// </summary>
        Task<IEnumerable<string>> GetDistinctCurrencyTypesAsync(DateTime? date = null);

        /// <summary>
        /// Bulk insert cho import CSV nhanh
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<RR01> entities);

        /// <summary>
        /// Lấy thống kê tổng quan
        /// </summary>
        Task<Dictionary<string, object>> GetStatisticsAsync(DateTime? date = null);
    }
}
