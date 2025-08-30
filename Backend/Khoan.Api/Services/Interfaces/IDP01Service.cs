using Khoan.Api.Models.Dtos.DP01;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Services.Interfaces
{
    public interface IDP01Service
    {
        // Basic CRUD operations - Match actual DP01Service implementation
        Task<IEnumerable<DP01PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 100);
        Task<DP01DetailsDto?> GetByIdAsync(int id);
        Task<DP01DetailsDto> CreateAsync(DP01CreateDto dto);
        Task<DP01DetailsDto?> UpdateAsync(int id, DP01UpdateDto dto);
        Task<bool> DeleteAsync(int id);

        // Query operations
        Task<IEnumerable<DP01PreviewDto>> GetRecentAsync(int count = 10);
        Task<IEnumerable<DP01PreviewDto>> GetByDateAsync(DateTime date);
        Task<IEnumerable<DP01PreviewDto>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);
        Task<IEnumerable<DP01PreviewDto>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);
        Task<IEnumerable<DP01PreviewDto>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        // Analytics operations
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);
        Task<DP01SummaryDto> GetStatisticsAsync(DateTime? date = null);
        Task<int> GetTotalCountAsync();
    }
}
