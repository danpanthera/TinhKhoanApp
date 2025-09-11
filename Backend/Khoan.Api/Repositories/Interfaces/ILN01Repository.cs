using Khoan.Api.Models.Entities;
using Khoan.Api.Dtos.LN01;

namespace Khoan.Api.Repositories.Interfaces
{
    public interface ILN01Repository
    {
        // Basic CRUD Operations with Temporal Table support
        Task<(IEnumerable<LN01PreviewDto> Data, int TotalCount)> GetAllPagedAsync(int page, int pageSize, DateTime? fromDate = null, DateTime? toDate = null);
        Task<LN01DetailsDto?> GetByIdAsync(int id);
        Task<LN01Entity> CreateAsync(LN01Entity entity);
        Task<LN01Entity?> UpdateAsync(int id, LN01Entity entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id);

        // Temporal Table Operations
        Task<IEnumerable<LN01DetailsDto>> GetHistoryAsync(int id);
        Task<LN01DetailsDto?> GetAsOfDateAsync(int id, DateTime asOfDate);
        Task<IEnumerable<LN01DetailsDto>> GetChangedBetweenAsync(int id, DateTime startDate, DateTime endDate);

        // Bulk Operations - Optimized for Temporal Table + Columnstore
        Task<int> BulkInsertAsync(IEnumerable<LN01Entity> entities);
        Task<int> BulkUpdateAsync(IEnumerable<LN01Entity> entities);
        Task<int> BulkDeleteAsync(IEnumerable<int> ids);

        // Data Import Operations
        Task<int> GetRecordCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ExistsByDateAsync(DateTime ngayDL);
        Task<int> DeleteByDateAsync(DateTime ngayDL);
        Task<IEnumerable<LN01Entity>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        // Loan Analytics - Optimized for Columnstore
        Task<LN01SummaryDto> GetSummaryAsync();
        Task<Dictionary<string, decimal>> GetLoanAmountsByProvinceAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, int>> GetLoanCountsByTypeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, decimal>> GetInterestRateDistributionAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<dynamic>> GetTopCustomersByLoanAmountAsync(int topCount = 10);
        Task<IEnumerable<dynamic>> GetLoanStatusDistributionAsync();

        // Officer & Branch Analytics
        Task<Dictionary<string, int>> GetLoansByOfficerAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, decimal>> GetLoanAmountsByBranchAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<dynamic>> GetOfficerPerformanceAsync();

        // Date-based Analytics
        Task<Dictionary<string, int>> GetRecordCountsByDateAsync();
        Task<IEnumerable<DateTime>> GetAvailableDatesAsync();
        Task<DateTime?> GetLatestDataDateAsync();
        Task<DateTime?> GetOldestDataDateAsync();

        // Search Operations
        Task<(IEnumerable<LN01PreviewDto> Data, int TotalCount)> SearchAsync(string searchTerm, int page, int pageSize);
        Task<IEnumerable<LN01PreviewDto>> GetByCustomerAsync(string custSeq);
        Task<IEnumerable<LN01PreviewDto>> GetByAccountAsync(string taiKhoan);
        Task<IEnumerable<LN01PreviewDto>> GetByOfficerAsync(string officerId);
    }
}
