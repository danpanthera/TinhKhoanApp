using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// RR01 Repository Interface - Phase 2B
    /// Contract for Loan Processing table operations (25 business columns)
    /// </summary>
    public interface IRR01Repository : IBaseRepository<RR01>
    {
        // === SPECIFIC RR01 OPERATIONS ===

        /// <summary>
        /// Get paged RR01 records with optional date filtering
        /// </summary>
        Task<PagedResult<RR01>> GetPagedAsync(int pageNumber, int pageSize, DateTime? fromDate = null);

        /// <summary>
        /// Get RR01 entity by Id
        /// </summary>
        Task<RR01?> GetEntityByIdAsync(long id);

        /// <summary>
        /// Get RR01 records by specific date
        /// </summary>
        Task<List<RR01>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Bulk insert RR01 records
        /// </summary>
        Task<BulkOperationResult> BulkInsertAsync(List<RR01> entities);

        /// <summary>
        /// Bulk update RR01 records
        /// </summary>
        Task<BulkOperationResult> BulkUpdateAsync(List<RR01> entities);

        /// <summary>
        /// Bulk delete RR01 records by IDs
        /// </summary>
        Task<BulkOperationResult> BulkDeleteAsync(List<long> ids);

        /// <summary>
        /// Get historical versions of an RR01 record
        /// </summary>
        Task<List<RR01>> GetHistoryAsync(long id);

        /// <summary>
        /// Get RR01 record as of specific date
        /// </summary>
        Task<RR01?> GetAsOfDateAsync(long id, DateTime asOfDate);

        /// <summary>
        /// Count RR01 records with optional date filtering
        /// </summary>
        Task<long> CountAsync(DateTime? fromDate = null);

        /// <summary>
        /// Check repository health
        /// </summary>
        Task<bool> IsHealthyAsync();
    }
}
