using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.LN03;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// LN03 Service Interface - Loan Summary table
    /// 20 business columns with temporal tracking
    /// </summary>
    public interface ILN03Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult>LN03PreviewDto>>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayDL = null);

        // CRUD operations
        Task<ApiResponse<LN03DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<LN03DetailsDto>> CreateAsync(LN03CreateDto createDto);
        Task<ApiResponse<LN03DetailsDto>> UpdateAsync(long id, LN03UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<LN03DetailsDto>>> GetByDateAsync(DateTime ngayDL);
        Task<ApiResponse<List<LN03DetailsDto>>> GetByProductCodeAsync(string productCode);

        // Import operations
        Task<ApiResponse<LN03ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<LN03ImportResultDto>> ImportBatchAsync(List<LN03CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<LN03SummaryDto>> GetSummaryAsync(DateTime ngayDL);
        Task<ApiResponse<List<LN03PreviewDto>>> GetProductSummaryAsync(DateTime ngayDL);
        Task<ApiResponse<decimal>> CalculateTotalLoanAmountAsync(DateTime ngayDL);
        Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountByProductAsync(DateTime ngayDL);

        // Temporal operations
        Task<ApiResponse<List<LN03DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<LN03DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
