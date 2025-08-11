using TinhKhoanApp.Api.Models.DTOs.GL01;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu GL01 (General Ledger)
    /// Xử lý 27 business columns theo CSV structure
    /// </summary>
    public interface IGL01Service
    {
        #region CRUD Operations với DTO Mapping

        /// <summary>
        /// Lấy tất cả GL01 với paging và DTO mapping
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>PagedResult với GL01 Preview DTOs</returns>
        Task<PagedResult<GL01PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy GL01 theo ID với full details DTO
        /// </summary>
        /// <param name="id">GL01 ID</param>
        /// <returns>GL01 Details DTO</returns>
        Task<GL01DetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới GL01 từ Create DTO
        /// </summary>
        /// <param name="createDto">GL01 Create DTO</param>
        /// <returns>Created GL01 Details DTO</returns>
        Task<GL01DetailsDto> CreateAsync(GL01CreateDto createDto);

        /// <summary>
        /// Cập nhật GL01 từ Update DTO
        /// </summary>
        /// <param name="id">GL01 ID</param>
        /// <param name="updateDto">GL01 Update DTO</param>
        /// <returns>Updated GL01 Details DTO</returns>
        Task<GL01DetailsDto?> UpdateAsync(int id, GL01UpdateDto updateDto);

        /// <summary>
        /// Xóa GL01 theo ID
        /// </summary>
        /// <param name="id">GL01 ID</param>
        /// <returns>True nếu xóa thành công</returns>
        Task<bool> DeleteAsync(int id);

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Lấy GL01 theo số tài khoản
        /// </summary>
        /// <param name="accountNumber">Số tài khoản</param>
        /// <returns>Danh sách GL01 Details DTOs</returns>
        Task<IEnumerable<GL01DetailsDto>> GetByAccountNumberAsync(string accountNumber);

        /// <summary>
        /// Lấy GL01 theo chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách GL01 Preview DTOs</returns>
        Task<IEnumerable<GL01PreviewDto>> GetByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy GL01 theo ngày giao dịch
        /// </summary>
        /// <param name="transactionDate">Ngày giao dịch</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách GL01 Preview DTOs</returns>
        Task<IEnumerable<GL01PreviewDto>> GetByTransactionDateAsync(DateTime transactionDate, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê GL01 Summary
        /// </summary>
        /// <returns>GL01 Summary DTO với thống kê</returns>
        Task<GL01SummaryDto> GetSummaryAsync();

        #endregion

        #region CSV Import Operations

        /// <summary>
        /// Import GL01 data từ CSV với validation và DTO result
        /// </summary>
        /// <param name="csvData">CSV data content</param>
        /// <param name="fileName">Tên file CSV</param>
        /// <returns>Import Result DTO với thống kê</returns>
        Task<GL01ImportResultDto> ImportFromCsvAsync(string csvData, string fileName);

        #endregion
    }
}
