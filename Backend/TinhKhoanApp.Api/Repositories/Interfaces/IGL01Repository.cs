using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.GL01;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for GL01 table operations
    /// Provides data access layer for 27-column GL01 general ledger data
    /// </summary>
    public interface IGL01Repository
    {
        // === BASIC CRUD OPERATIONS ===
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetAllAsync();
        Task<ApiResponse<GL01DetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<GL01DetailsDto>> CreateAsync(GL01CreateDto createDto);
        Task<ApiResponse<GL01DetailsDto>> UpdateAsync(long id, GL01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // === PAGINATION & SEARCH ===
        Task<ApiResponse<PagedResult>GL01PreviewDto>>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PagedResult>GL01PreviewDto>>>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByAccountAsync(string accountNumber);

        // === BULK OPERATIONS ===
        Task<ApiResponse<GL01ImportResultDto>> BulkInsertAsync(IEnumerable<GL01CreateDto> createDtos);
        Task<ApiResponse<bool>> BulkUpdateAsync(IEnumerable<GL01UpdateDto> updateDtos);
        Task<ApiResponse<bool>> BulkDeleteAsync(IEnumerable<long> ids);

        // === BUSINESS LOGIC ===
        Task<ApiResponse<GL01SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetRecentAsync(int count = 100);
        Task<ApiResponse<bool>> ExistsAsync(long id);
        Task<ApiResponse<long>> CountAsync();

        // === TEMPORAL OPERATIONS ===
        Task<ApiResponse<IEnumerable<GL01DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<GL01DetailsDto?>> GetAsOfAsync(long id, DateTime asOfDate);
    }
}
