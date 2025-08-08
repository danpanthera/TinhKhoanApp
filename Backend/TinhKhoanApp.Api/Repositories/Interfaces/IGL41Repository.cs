using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.GL41;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for GL41 table operations
    /// Provides data access layer for 13-column GL41 general ledger data
    /// </summary>
    public interface IGL41Repository
    {
        // === BASIC CRUD OPERATIONS ===
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetAllAsync();
        Task<ApiResponse<GL41DetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<GL41DetailsDto>> CreateAsync(GL41CreateDto createDto);
        Task<ApiResponse<GL41DetailsDto>> UpdateAsync(long id, GL41UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // === PAGINATION & SEARCH ===
        Task<ApiResponse<PagedResult<EI01PreviewDto>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PagedResult<EI01PreviewDto>>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetByAccountAsync(string accountNumber);

        // === BULK OPERATIONS ===
        Task<ApiResponse<GL41ImportResultDto>> BulkInsertAsync(IEnumerable<GL41CreateDto> createDtos);
        Task<ApiResponse<bool>> BulkUpdateAsync(IEnumerable<GL41UpdateDto> updateDtos);
        Task<ApiResponse<bool>> BulkDeleteAsync(IEnumerable<long> ids);

        // === BUSINESS LOGIC ===
        Task<ApiResponse<GL41SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetRecentAsync(int count = 100);
        Task<ApiResponse<bool>> ExistsAsync(long id);
        Task<ApiResponse<long>> CountAsync();

        // === TEMPORAL OPERATIONS ===
        Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<GL41DetailsDto?>> GetAsOfAsync(long id, DateTime asOfDate);
    }
}
