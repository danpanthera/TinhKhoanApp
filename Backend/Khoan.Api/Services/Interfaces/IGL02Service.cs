using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.GL02;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for GL02 (General Ledger) service operations
    /// Heavy File Optimized - Supports up to 2GB CSV files with 17 business columns
    /// </summary>
    public interface IGL02Service
    {
        /// <summary>
        /// Get all GL02 records with pagination
        /// </summary>
        Task<ApiResponse<IEnumerable<GL02PreviewDto>>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Get GL02 record by ID with full details
        /// </summary>
        Task<ApiResponse<GL02DetailsDto>> GetByIdAsync(int id);

        /// <summary>
        /// Import CSV file with heavy file support (up to 2GB)
        /// Batch processing: 10,000 records per batch
        /// Timeout: 15 minutes
        /// Only allows filenames containing "gl02"
        /// </summary>
        Task<ApiResponse<GL02ImportResultDto>> ImportCsvAsync(IFormFile file);

        /// <summary>
        /// Delete GL02 records by date range
        /// </summary>
        Task<ApiResponse<int>> DeleteByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get summary statistics by unit
        /// </summary>
        Task<ApiResponse<GL02SummaryByUnitDto>> GetSummaryByUnitAsync(string unitCode, DateTime startDate, DateTime endDate);
    }
}
