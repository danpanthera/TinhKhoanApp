using TinhKhoanApp.Api.Models.DataTables;
using System.Threading.Tasks;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Interface for RR01 Repository operations
    /// </summary>
    public interface IRR01Repository : IRepository<RR01>
    {
        /// <summary>
        /// Get all RR01 records for a specific statement date
        /// </summary>
        Task<IEnumerable<RR01>> GetByDateAsync(DateTime statementDate);

        /// <summary>
        /// Get all RR01 records for a specific branch
        /// </summary>
        Task<IEnumerable<RR01>> GetByBranchAsync(string branchCode, DateTime? statementDate = null);

        /// <summary>
        /// Get all RR01 records for a specific customer
        /// </summary>
        Task<IEnumerable<RR01>> GetByCustomerAsync(string customerId, DateTime? statementDate = null);

        /// <summary>
        /// Get all RR01 records imported from a specific file
        /// </summary>
        Task<IEnumerable<RR01>> GetByFileNameAsync(string fileName);

        /// <summary>
        /// Get RR01 records with pagination
        /// </summary>
        Task<(IEnumerable<RR01> Records, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            DateTime? statementDate = null,
            string? branchCode = null,
            string? customerId = null);

        /// <summary>
        /// Updates the entity in the database
        /// </summary>
        Task UpdateAsync(RR01 entity);

        /// <summary>
        /// Deletes the entity from the database
        /// </summary>
        Task DeleteAsync(RR01 entity);

        /// <summary>
        /// Saves all pending changes to the database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
