using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.GL41;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for GL41 service operations with partitioned columnstore optimization
    /// Handles GL41 balance analytics with direct CSV import (filename containing "gl41")
    /// NGAY_DL extracted from filename and converted to datetime2 (dd/mm/yyyy)
    /// 13 business columns + 4 system columns = 17 total columns
    /// </summary>
    public interface IGL41Service
    {
        /// <summary>
        /// Get paginated GL41 data with preview information
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Get detailed GL41 record by ID
        /// </summary>
        Task<ApiResponse<GL41DetailsDto?>> GetByIdAsync(long id);

        /// <summary>
        /// Import GL41 CSV file with direct database insertion
        /// Only processes files containing "gl41" in filename
        /// NGAY_DL extracted from filename (yyyyMMdd format)
        /// Proper decimal conversion for AMOUNT/BALANCE columns (#,###.00 format)
        /// </summary>
        Task<ApiResponse<GL41ImportResultDto>> ImportCsvAsync(IFormFile file, string? fileName = null);

        /// <summary>
        /// Delete GL41 records by date range
        /// </summary>
        Task<ApiResponse<bool>> DeleteByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get GL41 summary analytics by unit
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41SummaryByUnitDto>>> GetSummaryByUnitAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get GL41 analytics configuration
        /// </summary>
        Task<ApiResponse<GL41AnalyticsConfigDto>> GetAnalyticsConfigAsync();

        /// <summary>
        /// Get GL41 records by unit code
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetByUnitCodeAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Get GL41 records by account code
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Get GL41 balance summary for specific unit and date
        /// </summary>
        Task<ApiResponse<decimal>> GetBalanceSummaryAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Check if GL41 data exists for specific date (before import)
        /// </summary>
        Task<ApiResponse<bool>> HasDataForDateAsync(DateTime date);
    }
}
