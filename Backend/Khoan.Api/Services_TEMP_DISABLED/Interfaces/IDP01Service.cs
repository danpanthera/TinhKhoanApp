using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.DP01;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho DP01 Service - Business logic layer
    /// </summary>
    public interface IDP01Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<DP01PreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayDL = null);

        // CRUD operations
        Task<ApiResponse<DP01DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<DP01DetailsDto>> CreateAsync(DP01CreateDto createDto);
        Task<ApiResponse<DP01DetailsDto>> UpdateAsync(DP01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        // Business operations
        Task<ApiResponse<DP01SummaryDto>> GetSummaryAsync(DateTime ngayDL);
        Task<ApiResponse<DP01ImportResultDto>> ImportCsvAsync(IFormFile file);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile file);
    }
}
