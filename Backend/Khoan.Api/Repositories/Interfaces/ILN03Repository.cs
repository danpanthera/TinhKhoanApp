using Khoan.Api.Entities;
using Khoan.Api.Dtos.LN03;

namespace Khoan.Api.Repositories.Interfaces
{
    public interface ILN03Repository
    {
        // Basic CRUD Operations with Temporal Table support
        Task<(IEnumerable<LN03PreviewDto> Data, int TotalCount)> GetAllPagedAsync(int page, int pageSize, DateTime? fromDate = null, DateTime? toDate = null);
        Task<LN03DetailsDto?> GetByIdAsync(int id);
        Task<LN03Entity> CreateAsync(LN03Entity entity);
        Task<LN03Entity?> UpdateAsync(int id, LN03Entity entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id);

        // Bulk Operations for CSV Import
        Task<int> BulkCreateAsync(IEnumerable<LN03Entity> entities);
        Task<int> BulkDeleteAsync(IEnumerable<int> ids);
        Task<bool> TruncateAsync();

        // Temporal Table Operations
        Task<LN03DetailsDto?> GetAsOfDateAsync(int id, DateTime asOfDate);
        Task<IEnumerable<LN03DetailsDto>> GetHistoryAsync(int id);
        Task<IEnumerable<LN03DetailsDto>> GetChangedBetweenAsync(DateTime startDate, DateTime endDate);

        // Business Logic Queries
        Task<IEnumerable<LN03DetailsDto>> GetByBranchCodeAsync(string branchCode);
        Task<IEnumerable<LN03DetailsDto>> GetByCustomerCodeAsync(string customerCode);
        Task<IEnumerable<LN03DetailsDto>> GetByContractNumberAsync(string contractNumber);
        Task<IEnumerable<LN03DetailsDto>> GetByDebtGroupAsync(string debtGroup);
        Task<IEnumerable<LN03DetailsDto>> GetByDateAsync(DateTime date);
        Task<IEnumerable<LN03DetailsDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Analytics and Reporting (optimized for columnstore)
        Task<LN03SummaryDto> GetSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, decimal>> GetTotalAmountByBranchAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, decimal>> GetOutstandingBalanceByDateAsync(DateTime date);
        Task<Dictionary<string, int>> GetContractCountByDebtGroupAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<(string CustomerCode, string CustomerName, decimal TotalAmount)>> GetTopCustomersByAmountAsync(int topCount = 10, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, decimal>> GetTotalAmountByDateRangeAsync(DateTime startDate, DateTime endDate, string groupBy = "month");

        // Data Validation and Integrity
        Task<int> GetRecordCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<string>> ValidateDataIntegrityAsync();
        Task<DateTime?> GetLatestDataDateAsync();
        Task<DateTime?> GetOldestDataDateAsync();
    }
}
