using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos;
using Khoan.Api.Models.Dtos.EI01;

namespace Khoan.Api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for EI01 table operations
    /// Provides data access layer for 24-column EI01 exchange income data
    /// </summary>
    public interface IEI01Repository
    {
        // === BASIC CRUD OPERATIONS ===
        Task<ApiResponse<IEnumerable<EI01PreviewDto>>> GetAllAsync();
        Task<ApiResponse<EI01DetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<EI01DetailsDto>> CreateAsync(EI01CreateDto createDto);
        Task<ApiResponse<EI01DetailsDto>> UpdateAsync(long id, EI01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // === PAGINATION & SEARCH ===
        Task<ApiResponse<PagedResult<EI01PreviewDto>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PagedResult<EI01PreviewDto>>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<IEnumerable<EI01PreviewDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<IEnumerable<EI01PreviewDto>>> GetByAccountAsync(string accountNumber);

        // === BULK OPERATIONS ===
        Task<ApiResponse<EI01ImportResultDto>> BulkInsertAsync(IEnumerable<EI01CreateDto> createDtos);
        Task<ApiResponse<bool>> BulkUpdateAsync(IEnumerable<EI01UpdateDto> updateDtos);
        Task<ApiResponse<bool>> BulkDeleteAsync(IEnumerable<long> ids);

        // === BUSINESS LOGIC ===
        Task<ApiResponse<EI01SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<IEnumerable<EI01PreviewDto>>> GetRecentAsync(int count = 100);
        Task<ApiResponse<bool>> ExistsAsync(long id);
        Task<ApiResponse<long>> CountAsync();

        // === TEMPORAL OPERATIONS ===
        Task<ApiResponse<IEnumerable<EI01DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<EI01DetailsDto?>> GetAsOfAsync(long id, DateTime asOfDate);
    }
}
