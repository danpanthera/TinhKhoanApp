using Khoan.Api.Models.Entities;
using Khoan.Api.Repositories.Interfaces;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Interface for GL02Repository - General Ledger Summary operations
    /// Uses GL02Entity (Partitioned Columnstore - NON-TEMPORAL)
    /// </summary>
    public interface IGL02Repository : IRepository<GL02Entity>
    {
        /// <summary>
        /// Get recent GL02 records ordered by NGAY_DL desc
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Get GL02 records by NGAY_DL date
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Get GL02 records by Unit
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetByUnitAsync(string unit, int maxResults = 100);

        /// <summary>
        /// Get GL02 records by Transaction Code
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetByTransactionCodeAsync(string trcd, int maxResults = 100);

        /// <summary>
        /// Get GL02 records by Branch Code
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Get GL02 records by Customer
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetByCustomerAsync(string customer, int maxResults = 100);

        /// <summary>
        /// Get total transactions by Unit and type (DR/CR)
        /// </summary>
        Task<decimal> GetTotalTransactionsByUnitAsync(string unit, string type);

        /// <summary>
        /// Get GL02 records by Local Account
        /// </summary>
        Task<IEnumerable<GL02Entity>> GetByLocalAccountAsync(string locac, int maxResults = 100);
    }
}
