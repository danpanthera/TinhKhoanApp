using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.GL02;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for GL02 table operations
    /// Provides data access layer for 17-column GL02 general ledger data
    /// </summary>
    public interface IGL02Repository
    {
        // === BASIC CRUD OPERATIONS ===
        Task<ApiResponse<IEnumerable<GL02PreviewDto>>> GetAllAsync();
        Task<ApiResponse<GL02DetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<GL02DetailsDto>> CreateAsync(GL02CreateDto createDto);
        Task<ApiResponse<GL02DetailsDto>> UpdateAsync(long id, GL02UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // === PAGINATION & SEARCH ===
        Task<ApiResponse<PagedResult<GL02PreviewDto>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PagedResult<GL02PreviewDto>>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<IEnumerable<GL02PreviewDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<IEnumerable<GL02PreviewDto>>> GetByAccountAsync(string accountNumber);

        // === BULK OPERATIONS ===
        Task<ApiResponse<GL02ImportResultDto>> BulkInsertAsync(IEnumerable<GL02CreateDto> createDtos);
        Task<ApiResponse<bool>> BulkUpdateAsync(IEnumerable<GL02UpdateDto> updateDtos);
        Task<ApiResponse<bool>> BulkDeleteAsync(IEnumerable<long> ids);

        // === BUSINESS LOGIC ===
        Task<ApiResponse<GL02SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<IEnumerable<GL02PreviewDto>>> GetRecentAsync(int count = 100);
        Task<ApiResponse<bool>> ExistsAsync(long id);
        Task<ApiResponse<long>> CountAsync();

        // === TEMPORAL OPERATIONS ===
        Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<GL02DetailsDto?>> GetAsOfAsync(long id, DateTime asOfDate);
    }
}
