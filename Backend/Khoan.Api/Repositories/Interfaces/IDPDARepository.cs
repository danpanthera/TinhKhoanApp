using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// DPDA Repository Interface - Data access layer cho bảng DPDA
    /// </summary>
    public interface IDPDARepository
    {
        // Basic CRUD - match Repository<T> pattern
        Task<IEnumerable<DPDAEntity>> GetAllAsync();
        Task<DPDAEntity?> GetByIdAsync(long id);
        Task<DPDAEntity> CreateAsync(DPDAEntity entity);
        Task<DPDAEntity> UpdateAsync(DPDAEntity entity);
        Task<bool> DeleteAsync(long id);

        // Paging support
        Task<(IEnumerable<DPDAEntity> entities, long totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);

        // DPDA-specific query methods
        Task<IEnumerable<DPDAEntity>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);
        Task<IEnumerable<DPDAEntity>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);
        Task<IEnumerable<DPDAEntity>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);
        Task<IEnumerable<DPDAEntity>> GetByCardNumberAsync(string cardNumber, int maxResults = 100);
        Task<IEnumerable<DPDAEntity>> GetByStatusAsync(string status, int maxResults = 100);
        Task<IEnumerable<DPDAEntity>> GetByDateAsync(DateTime date, int maxResults = 100);

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
