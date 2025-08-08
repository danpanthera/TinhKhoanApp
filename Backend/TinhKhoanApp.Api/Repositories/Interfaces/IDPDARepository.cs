using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.DPDA;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for DPDA table operations
    /// Provides data access layer for 13-column DPDA deposit account data
    /// </summary>
    public interface IDPDARepository
    {
        // === BASIC CRUD OPERATIONS ===
        Task<ApiResponse<IEnumerable<DPDAPreviewDto>>> GetAllAsync();
        Task<ApiResponse<DPDADetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<DPDADetailsDto>> CreateAsync(DPDACreateDto createDto);
        Task<ApiResponse<DPDADetailsDto>> UpdateAsync(long id, DPDAUpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // === PAGINATION & SEARCH ===
        Task<ApiResponse<PagedResult>DPDAPreviewDto>>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PagedResult>DPDAPreviewDto>>>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<IEnumerable<DPDAPreviewDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<IEnumerable<DPDAPreviewDto>>> GetByAccountAsync(string accountNumber);

        // === BULK OPERATIONS ===
        Task<ApiResponse<DPDAImportResultDto>> BulkInsertAsync(IEnumerable<DPDACreateDto> createDtos);
        Task<ApiResponse<bool>> BulkUpdateAsync(IEnumerable<DPDAUpdateDto> updateDtos);
        Task<ApiResponse<bool>> BulkDeleteAsync(IEnumerable<long> ids);

        // === BUSINESS LOGIC ===
        Task<ApiResponse<DPDASummaryDto>> GetSummaryAsync();
        Task<ApiResponse<IEnumerable<DPDAPreviewDto>>> GetRecentAsync(int count = 100);
        Task<ApiResponse<bool>> ExistsAsync(long id);
        Task<ApiResponse<long>> CountAsync();

        // === TEMPORAL OPERATIONS ===
        Task<ApiResponse<IEnumerable<DPDADetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<DPDADetailsDto?>> GetAsOfAsync(long id, DateTime asOfDate);
    }
}
