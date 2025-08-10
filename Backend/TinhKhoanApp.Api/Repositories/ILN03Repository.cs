using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DTOs.LN03;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// LN03 Repository Interface - Phase 2B
    /// Contract for Loan Processing table operations (17 business columns)
    /// </summary>
    public interface ILN03Repository : IBaseRepository<LN03Entity>
    {
        // === SPECIFIC LN03 OPERATIONS ===

        /// <summary>
        /// Get paged LN03 records with optional date filtering
        /// </summary>
        Task<PagedResult<LN03Entity>> GetPagedAsync(int pageNumber, int pageSize, DateTime? fromDate = null);

        /// <summary>
        /// Get LN03 entity by Id
        /// </summary>
        Task<LN03Entity?> GetEntityByIdAsync(long id);

        /// <summary>
        /// Get LN03 records by specific date
        /// </summary>
        Task<List<LN03Entity>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Bulk insert LN03 records
        /// </summary>
        Task<BulkOperationResult> BulkInsertAsync(List<LN03Entity> entities);

        /// <summary>
        /// Bulk update LN03 records
        /// </summary>
        Task<BulkOperationResult> BulkUpdateAsync(List<LN03Entity> entities);

        /// <summary>
        /// Bulk delete LN03 records by IDs
        /// </summary>
        Task<BulkOperationResult> BulkDeleteAsync(List<long> ids);

        /// <summary>
        /// Get historical versions of an LN03 record
        /// </summary>
        Task<List<LN03Entity>> GetHistoryAsync(long id);

        /// <summary>
        /// Get LN03 record as of specific date
        /// </summary>
        Task<LN03Entity?> GetAsOfDateAsync(long id, DateTime asOfDate);

        /// <summary>
        /// Count LN03 records with optional date filtering
        /// </summary>
        Task<long> CountAsync(DateTime? fromDate = null);

        /// <summary>
        /// Check repository health
        /// </summary>
        Task<bool> IsHealthyAsync();
    }
}
