using Khoan.Api.Models.Dtos.DPDA;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.DataTables;
using Microsoft.AspNetCore.Http;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// DPDA Service Interface - Business logic layer cho bảng DPDA
    /// </summary>
    public interface IDPDAService
    {
        // Controller Support Methods
        Task<ApiResponse<PagedResult<DPDAPreviewDto>>> GetPreviewAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ApiResponse<DPDAImportResultDto>> ImportFromCsvAsync(IFormFile file, string uploadedBy);

        // Core CRUD operations
        Task<ApiResponse<DPDADetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<DPDADetailsDto>> CreateAsync(DPDACreateDto createDto);
        Task<ApiResponse<DPDADetailsDto>> UpdateAsync(DPDAUpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // Search and Query operations
        Task<ApiResponse<List<DPDAPreviewDto>>> GetByCustomerCodeAsync(string customerCode, int limit = 100);
        Task<ApiResponse<List<DPDAPreviewDto>>> GetByBranchCodeAsync(string branchCode, int limit = 100);
        Task<ApiResponse<List<DPDAPreviewDto>>> GetByAccountNumberAsync(string accountNumber, int limit = 100);
        Task<ApiResponse<List<DPDAPreviewDto>>> GetByCardNumberAsync(string cardNumber, int limit = 100);
        Task<ApiResponse<List<DPDAPreviewDto>>> GetByStatusAsync(string status, int limit = 100);
        Task<ApiResponse<List<DPDAPreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100);

        // Analytics operations
        Task<ApiResponse<DPDASummaryDto>> GetStatisticsAsync(DateTime? asOfDate = null);
    }
}
