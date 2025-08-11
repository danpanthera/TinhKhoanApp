using TinhKhoanApp.Api.Models.Dtos.EI01;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu EI01 (Employee Information)
    /// Xử lý 24 business columns theo CSV structure
    /// </summary>
    public interface IEI01Service
    {
        #region CRUD Operations với DTO Mapping

        /// <summary>
        /// Lấy tất cả EI01 với paging và DTO mapping
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>PagedResult với EI01 Preview DTOs</returns>
        Task<PagedResult<EI01PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy EI01 theo ID với full details DTO
        /// </summary>
        /// <param name="id">EI01 ID</param>
        /// <returns>EI01 Details DTO</returns>
        Task<EI01DetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới EI01 từ Create DTO
        /// </summary>
        /// <param name="createDto">EI01 Create DTO</param>
        /// <returns>Created EI01 Details DTO</returns>
        Task<EI01DetailsDto> CreateAsync(EI01CreateDto createDto);

        /// <summary>
        /// Cập nhật EI01 từ Update DTO
        /// </summary>
        /// <param name="id">EI01 ID</param>
        /// <param name="updateDto">EI01 Update DTO</param>
        /// <returns>Updated EI01 Details DTO</returns>
        Task<EI01DetailsDto?> UpdateAsync(int id, EI01UpdateDto updateDto);

        /// <summary>
        /// Xóa EI01 theo ID
        /// </summary>
        /// <param name="id">EI01 ID</param>
        /// <returns>True nếu xóa thành công</returns>
        Task<bool> DeleteAsync(int id);

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Lấy EI01 theo mã nhân viên
        /// </summary>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <returns>EI01 Details DTO</returns>
        Task<EI01DetailsDto?> GetByEmployeeCodeAsync(string employeeCode);

        /// <summary>
        /// Lấy EI01 theo chi nhánh
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách EI01 Preview DTOs</returns>
        Task<IEnumerable<EI01PreviewDto>> GetByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy EI01 theo phòng ban
        /// </summary>
        /// <param name="department">Phòng ban</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách EI01 Preview DTOs</returns>
        Task<IEnumerable<EI01PreviewDto>> GetByDepartmentAsync(string department, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê EI01 Summary
        /// </summary>
        /// <returns>EI01 Summary DTO với thống kê</returns>
        Task<EI01SummaryDto> GetSummaryAsync();

        #endregion

        #region CSV Import Operations

        /// <summary>
        /// Import EI01 data từ CSV với validation và DTO result
        /// </summary>
        /// <param name="csvData">CSV data content</param>
        /// <param name="fileName">Tên file CSV</param>
        /// <returns>Import Result DTO với thống kê</returns>
        Task<EI01ImportResultDto> ImportFromCsvAsync(string csvData, string fileName);

        #endregion
    }
}
