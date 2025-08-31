using Khoan.Api.Models.Entities;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Interface for GL41 Repository - Partitioned Columnstore Optimized
    /// 13 business columns + 4 system columns = 17 total columns
    /// Direct import policy: Only files containing "gl41"
    /// NGAY_DL extracted from filename and converted to datetime2 (dd/mm/yyyy)
    /// </summary>
    public interface IGL41Repository : IRepository<GL41Entity>
    {
        /// <summary>
        /// Get paginated GL41 data with total count (for analytics performance)
        /// </summary>
        Task<(IEnumerable<GL41Entity> Data, int TotalCount)> GetAllPagedAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Get GL41 records by date range (partitioned columnstore optimized)
        /// </summary>
        Task<IEnumerable<GL41Entity>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get GL41 records by unit code with pagination
        /// </summary>
        Task<IEnumerable<GL41Entity>> GetByUnitCodeAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Get GL41 records by account code with pagination
        /// </summary>
        Task<IEnumerable<GL41Entity>> GetByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Bulk insert GL41 records (partitioned columnstore optimized)
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<GL41Entity> entities);

        /// <summary>
        /// Delete GL41 records by date range (for cleanup operations)
        /// </summary>
        Task<int> DeleteByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get summary analytics by unit for GL41 balance data
        /// </summary>
        Task<IEnumerable<dynamic>> GetSummaryByUnitAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get balance summary for specific unit and date
        /// </summary>
        Task<decimal> GetTotalOpeningBalanceByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Get closing balance summary for specific unit and date
        /// </summary>
        Task<decimal> GetTotalClosingBalanceByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Get debit/credit transactions summary by unit
        /// </summary>
        Task<(decimal DebitTotal, decimal CreditTotal)> GetTransactionSummaryByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Check if GL41 data exists for specific date (before import)
        /// </summary>
        Task<bool> HasDataForDateAsync(DateTime date);

        /// <summary>
        /// Get distinct currencies for GL41 analytics
        /// </summary>
        Task<List<string>> GetDistinctCurrenciesAsync();

        /// <summary>
        /// Update range of GL41 entities (bulk update)
        /// </summary>
        void UpdateRange(IEnumerable<GL41Entity> entities);
    }
}
