using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Interface for RR01 service operations
    /// </summary>
    public interface IRR01Service
    {
        /// <summary>
        /// Get all RR01 records
        /// </summary>
        Task<IEnumerable<RR01DTO>> GetAllAsync();

        /// <summary>
        /// Get RR01 record by ID
        /// </summary>
        Task<RR01DTO?> GetByIdAsync(long id);

        /// <summary>
        /// Get RR01 records by statement date
        /// </summary>
        Task<IEnumerable<RR01DTO>> GetByDateAsync(DateTime statementDate);

        /// <summary>
        /// Get RR01 records by branch code
        /// </summary>
        Task<IEnumerable<RR01DTO>> GetByBranchAsync(string branchCode, DateTime? statementDate = null);

        /// <summary>
        /// Get RR01 records by customer ID
        /// </summary>
        Task<IEnumerable<RR01DTO>> GetByCustomerAsync(string customerId, DateTime? statementDate = null);

        /// <summary>
        /// Get RR01 records by file name
        /// </summary>
        Task<IEnumerable<RR01DTO>> GetByFileNameAsync(string fileName);

        /// <summary>
        /// Get paged RR01 records with optional filters
        /// </summary>
        Task<(IEnumerable<RR01DTO> Records, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            DateTime? statementDate = null,
            string? branchCode = null,
            string? customerId = null);

        /// <summary>
        /// Create a new RR01 record
        /// </summary>
        Task<RR01DTO> CreateAsync(CreateRR01DTO createDto);

        /// <summary>
        /// Update an existing RR01 record
        /// </summary>
        Task<RR01DTO?> UpdateAsync(long id, UpdateRR01DTO updateDto);

        /// <summary>
        /// Delete an RR01 record
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Get distinct statement dates available for RR01 records
        /// </summary>
        Task<IEnumerable<DateTime>> GetDistinctDatesAsync();

        /// <summary>
        /// Get distinct branch codes for RR01 records
        /// </summary>
        Task<IEnumerable<string>> GetDistinctBranchesAsync(DateTime? statementDate = null);

        /// <summary>
        /// Get summary statistics for RR01 data
        /// </summary>
        Task<object> GetSummaryStatisticsAsync(DateTime statementDate);
    }
}
