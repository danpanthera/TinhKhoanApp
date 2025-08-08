using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.RR01;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// RR01 Service Interface - Risk Report table
    /// 25 business columns with temporal tracking
    /// </summary>
    public interface IRR01Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<DPDAPreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayDL = null);

        // CRUD operations
        Task<ApiResponse<RR01DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<RR01DetailsDto>> CreateAsync(RR01CreateDto createDto);
        Task<ApiResponse<RR01DetailsDto>> UpdateAsync(long id, RR01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<RR01DetailsDto> GetByDateAsync(DateTime ngayDL);

        // Import operations
        Task<ApiResponse<RR01ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<RR01ImportResultDto>> ImportBatchAsync(List<RR01CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<RR01SummaryDto>> GetSummaryAsync(DateTime ngayDL);
        Task<ApiResponse<List<RR01PreviewDto> GetRiskSummaryAsync(DateTime fromDate, DateTime toDate);
        Task<ApiResponse<decimal>> CalculateTotalRiskExposureAsync(DateTime ngayDL);

        // Temporal operations
        Task<ApiResponse<List<RR01DetailsDto> GetHistoryAsync(long id);
        Task<ApiResponse<RR01DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
