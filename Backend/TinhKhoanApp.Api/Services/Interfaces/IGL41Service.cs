using TinhKhoanApp.Api.Models.Dtos.GL41;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu GL41 (Bảng cân đối kế toán tổng hợp)
    /// Xử lý 18 business columns theo CSV structure
    /// </summary>
    public interface IGL41Service
    {
        #region CRUD Operations với DTO Mapping

        /// <summary>
        /// Lấy tất cả GL41 với paging và DTO mapping
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>PagedResult với GL41 Preview DTOs</returns>
        Task<PagedResult<GL41PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy GL41 theo ID với full details DTO
        /// </summary>
        /// <param name="id">GL41 ID</param>
        /// <returns>GL41 Details DTO</returns>
        Task<GL41DetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới GL41 từ Create DTO
        /// </summary>
        /// <param name="createDto">GL41 Create DTO</param>
        /// <returns>Created GL41 Details DTO</returns>
        Task<GL41DetailsDto> CreateAsync(GL41CreateDto createDto);

        /// <summary>
        /// Cập nhật GL41 từ Update DTO
        /// </summary>
        /// <param name="id">GL41 ID</param>
        /// <param name="updateDto">GL41 Update DTO</param>
        /// <returns>Updated GL41 Details DTO</returns>
        Task<GL41DetailsDto?> UpdateAsync(int id, GL41UpdateDto updateDto);

        /// <summary>
        /// Xóa GL41 theo ID
        /// </summary>
        /// <param name="id">GL41 ID</param>
        /// <returns>True nếu xóa thành công</returns>
        Task<bool> DeleteAsync(int id);

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Lấy GL41 theo mã tài khoản
        /// </summary>
        /// <param name="accountCode">Mã tài khoản</param>
        /// <returns>Danh sách GL41 Details DTOs</returns>
        Task<IEnumerable<GL41DetailsDto>> GetByAccountCodeAsync(string accountCode);

        /// <summary>
        /// Lấy GL41 theo chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách GL41 Preview DTOs</returns>
        Task<IEnumerable<GL41PreviewDto>> GetByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy GL41 theo kỳ báo cáo
        /// </summary>
        /// <param name="reportPeriod">Kỳ báo cáo</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách GL41 Preview DTOs</returns>
        Task<IEnumerable<GL41PreviewDto>> GetByReportPeriodAsync(string reportPeriod, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê GL41 Summary
        /// </summary>
        /// <returns>GL41 Summary DTO với thống kê</returns>
        Task<GL41SummaryDto> GetSummaryAsync();

        #endregion

        #region CSV Import Operations

        /// <summary>
        /// Import GL41 data từ CSV với validation và DTO result
        /// </summary>
        /// <param name="csvData">CSV data content</param>
        /// <param name="fileName">Tên file CSV</param>
        /// <returns>Import Result DTO với thống kê</returns>
        Task<GL41ImportResultDto> ImportFromCsvAsync(string csvData, string fileName);

        #endregion
    }
}
