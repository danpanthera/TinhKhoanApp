using TinhKhoanApp.Api.Models.Dtos.DP01;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho DP01 Service - sử dụng DataTables.DP01 model
    /// Chuẩn hóa: CSV (63 columns) -> Database -> DataTables.DP01 -> Service
    /// </summary>
    public interface IDP01DataTableService
    {
        Task<IEnumerable<DP01StandardPreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 100);
        Task<DP01DetailsDto?> GetByIdAsync(int id);
        Task<IEnumerable<DP01StandardPreviewDto>> GetRecentAsync(int count = 10);
        Task<IEnumerable<DP01StandardPreviewDto>> GetByDateAsync(DateTime date);
        Task<IEnumerable<DP01StandardPreviewDto>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);
        Task<IEnumerable<DP01StandardPreviewDto>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);
        Task<IEnumerable<DP01StandardPreviewDto>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);
        Task<DP01SummaryDto> GetStatisticsAsync(DateTime? date = null);
        Task<DP01DetailsDto> CreateAsync(DP01CreateDto createDto);
        Task<DP01DetailsDto?> UpdateAsync(int id, DP01UpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
