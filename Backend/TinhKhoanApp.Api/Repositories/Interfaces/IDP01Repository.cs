using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.DP01;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for DP01 table operations
    /// Provides data access layer for 63-column DP01 production data
    /// </summary>
    public interface IDP01Repository
    {
        // === BASIC CRUD OPERATIONS ===
        Task<ApiResponse<IEnumerable<DP01PreviewDto>>> GetAllAsync();
        Task<ApiResponse<DP01DetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<DP01DetailsDto>> CreateAsync(DP01CreateDto createDto);
        Task<ApiResponse<DP01DetailsDto>> UpdateAsync(long id, DP01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // === PAGINATION & SEARCH ===
        Task<ApiResponse<PagedResult>DP01PreviewDto>>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PagedResult>DP01PreviewDto>>>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<IEnumerable<DP01PreviewDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<IEnumerable<DP01PreviewDto>>> GetByAccountAsync(string accountNumber);

        // === BULK OPERATIONS ===
        Task<ApiResponse<DP01ImportResultDto>> BulkInsertAsync(IEnumerable<DP01CreateDto> createDtos);
        Task<ApiResponse<bool>> BulkUpdateAsync(IEnumerable<DP01UpdateDto> updateDtos);
        Task<ApiResponse<bool>> BulkDeleteAsync(IEnumerable<long> ids);

        // === BUSINESS LOGIC ===
        Task<ApiResponse<DP01SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<IEnumerable<DP01PreviewDto>>> GetRecentAsync(int count = 100);
        Task<ApiResponse<bool>> ExistsAsync(long id);
        Task<ApiResponse<long>> CountAsync();

        // === TEMPORAL OPERATIONS ===
        Task<ApiResponse<IEnumerable<DP01DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<DP01DetailsDto?>> GetAsOfAsync(long id, DateTime asOfDate);
    }
}
