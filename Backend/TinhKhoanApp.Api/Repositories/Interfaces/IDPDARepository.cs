using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Repositories.Interfaces;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho DPDA Repository - Thẻ nộp đơn gửi tiết kiệm
    /// Theo pattern IDP01Repository với 13 business columns từ CSV
    /// CSV-First: Business columns từ CSV là chuẩn cho tất cả layers
    /// </summary>
    public interface IDPDARepository : IBaseRepository<DPDAEntity>
    {
        #region Paging Support

        /// <summary>
        /// Get DPDA with paging and optional search
        /// </summary>
        Task<(IEnumerable<DPDAEntity> entities, long totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);

        #endregion

        #region Business Key Searches - CSV column names preserved

        /// <summary>
        /// Search DPDA by customer code (MA_KHACH_HANG)
        /// </summary>
        Task<IEnumerable<DPDAEntity>> GetByCustomerCodeAsync(string customerCode, int limit = 100);

        /// <summary>
        /// Search DPDA by branch code (MA_CHI_NHANH)
        /// </summary>
        Task<IEnumerable<DPDAEntity>> GetByBranchCodeAsync(string branchCode, int limit = 100);

        /// <summary>
        /// Search DPDA by account number (SO_TAI_KHOAN)
        /// </summary>
        Task<IEnumerable<DPDAEntity>> GetByAccountNumberAsync(string accountNumber, int limit = 100);

        /// <summary>
        /// Search DPDA by card number (SO_THE)
        /// </summary>
        Task<IEnumerable<DPDAEntity>> GetByCardNumberAsync(string cardNumber, int limit = 100);

        #endregion

        #region Analytics Support for Summary DTO

        /// <summary>
        /// Get total DPDA count
        /// </summary>
        Task<long> GetTotalCountAsync();

        /// <summary>
        /// Get card count by status (TRANG_THAI)
        /// </summary>
        Task<Dictionary<string, long>> GetCardCountByStatusAsync();

        /// <summary>
        /// Get card count by type (LOAI_THE)
        /// </summary>
        Task<Dictionary<string, long>> GetCardCountByTypeAsync();

        /// <summary>
        /// Get card count by branch (MA_CHI_NHANH) - Top N
        /// </summary>
        Task<Dictionary<string, long>> GetCardCountByBranchAsync(int topN = 10);

        /// <summary>
        /// Get card count by category (PHAN_LOAI)
        /// </summary>
        Task<Dictionary<string, long>> GetCardCountByCategoryAsync();

        /// <summary>
        /// Get card count issued in date range (NGAY_PHAT_HANH)
        /// </summary>
        Task<long> GetCardCountByDateRangeAsync(DateTime startDate, DateTime endDate);

        #endregion
    }
}
