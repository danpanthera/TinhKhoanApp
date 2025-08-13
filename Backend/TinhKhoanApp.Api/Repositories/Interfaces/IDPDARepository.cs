using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// DPDA Repository Interface - Data access layer cho bảng DPDA
    /// </summary>
    public interface IDPDARepository
    {
        // Basic CRUD - match Repository<T> pattern
        Task<IEnumerable<DPDA>> GetAllAsync();
        Task<DPDA?> GetByIdAsync(long id);
        Task<DPDA> CreateAsync(DPDA entity);
        Task<DPDA> UpdateAsync(DPDA entity);
        Task<bool> DeleteAsync(long id);

        // Paging support
        Task<(IEnumerable<DPDA> entities, long totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);

        // DPDA-specific query methods
        Task<IEnumerable<DPDA>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);
        Task<IEnumerable<DPDA>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);
        Task<IEnumerable<DPDA>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);
        Task<IEnumerable<DPDA>> GetByCardNumberAsync(string cardNumber, int maxResults = 100);
        Task<IEnumerable<DPDA>> GetByStatusAsync(string status, int maxResults = 100);
        Task<IEnumerable<DPDA>> GetByDateAsync(DateTime date, int maxResults = 100);

        // Analytics methods
        Task<long> GetTotalCountByBranchAsync(string branchCode, DateTime? date = null);
        Task<long> GetTotalCountAsync();
        Task<Dictionary<string, long>> GetCardTypeDistributionAsync(DateTime? asOfDate = null);
        Task<Dictionary<string, long>> GetStatusDistributionAsync(DateTime? asOfDate = null);

        // Additional analytics methods used by service
        Task<Dictionary<string, long>> GetCardCountByStatusAsync(DateTime? asOfDate = null);
        Task<Dictionary<string, long>> GetCardCountByTypeAsync(DateTime? asOfDate = null);
        Task<Dictionary<string, long>> GetCardCountByBranchAsync(DateTime? asOfDate = null);
        Task<Dictionary<string, long>> GetCardCountByCategoryAsync(DateTime? asOfDate = null);
        Task<Dictionary<string, long>> GetCardCountByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
