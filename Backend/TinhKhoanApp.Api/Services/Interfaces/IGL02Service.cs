using TinhKhoanApp.Api.Models.DTOs.GL02;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu GL02 (General Ledger Detail)
    /// Xử lý 16 business columns theo CSV structure
    /// </summary>
    public interface IGL02Service
    {
        #region CRUD Operations với DTO Mapping

        /// <summary>
        /// Lấy tất cả GL02 với paging và DTO mapping
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>PagedResult với GL02 Preview DTOs</returns>
        Task<PagedResult<GL02PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy GL02 theo ID với full details DTO
        /// </summary>
        /// <param name="id">GL02 ID</param>
        /// <returns>GL02 Details DTO</returns>
        Task<GL02DetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới GL02 từ Create DTO
        /// </summary>
        /// <param name="createDto">GL02 Create DTO</param>
        /// <returns>Created GL02 Details DTO</returns>
        Task<GL02DetailsDto> CreateAsync(GL02CreateDto createDto);

        /// <summary>
        /// Cập nhật GL02 từ Update DTO
        /// </summary>
        /// <param name="id">GL02 ID</param>
        /// <param name="updateDto">GL02 Update DTO</param>
        /// <returns>Updated GL02 Details DTO</returns>
        Task<GL02DetailsDto?> UpdateAsync(int id, GL02UpdateDto updateDto);

        /// <summary>
        /// Xóa GL02 theo ID
        /// </summary>
        /// <param name="id">GL02 ID</param>
        /// <returns>True nếu xóa thành công</returns>
        Task<bool> DeleteAsync(int id);

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Lấy GL02 theo số tài khoản con
        /// </summary>
        /// <param name="subAccountNumber">Số tài khoản con</param>
        /// <returns>Danh sách GL02 Details DTOs</returns>
        Task<IEnumerable<GL02DetailsDto>> GetBySubAccountAsync(string subAccountNumber);

        /// <summary>
        /// Lấy GL02 theo chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách GL02 Preview DTOs</returns>
        Task<IEnumerable<GL02PreviewDto>> GetByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy GL02 theo khoảng thời gian
        /// </summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách GL02 Preview DTOs</returns>
        Task<IEnumerable<GL02PreviewDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê GL02 Summary
        /// </summary>
        /// <returns>GL02 Summary DTO với thống kê</returns>
        Task<GL02SummaryDto> GetSummaryAsync();

        #endregion

        #region CSV Import Operations

        /// <summary>
        /// Import GL02 data từ CSV với validation và DTO result
        /// </summary>
        /// <param name="csvData">CSV data content</param>
        /// <param name="fileName">Tên file CSV</param>
        /// <returns>Import Result DTO với thống kê</returns>
        Task<GL02ImportResultDto> ImportFromCsvAsync(string csvData, string fileName);

        #endregion
    }
}
