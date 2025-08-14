using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Base Repository Interface - Common operations for all tables
    /// </summary>
    public interface IBaseRepository<T> where T : class
    {
        // Preview operations
        Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, DateTime? ngayDL = null);
        Task<ApiResponse<IEnumerable<T>>> GetAllAsync();

        // Basic CRUD operations
        Task<T?> GetByIdAsync(long id);
        Task<T> CreateAsync(T entity);
        Task<T?> UpdateAsync(long id, T entity);
        Task<bool> DeleteAsync(long id);
        Task<List<T>> GetByDateAsync(DateTime ngayDL);

        // Batch operations
        Task<BulkOperationResult> BulkInsertAsync(List<T> entities);
        Task<BulkOperationResult> BulkUpdateAsync(List<T> entities);
        Task<BulkOperationResult> BulkDeleteAsync(List<long> ids);

        // Temporal operations (if supported)
        Task<List<T>> GetHistoryAsync(long id);
        Task<T?> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<long> CountAsync(DateTime? ngayDL = null);
        Task<bool> ExistsAsync(long id);
        Task<bool> IsHealthyAsync();
    }
}
