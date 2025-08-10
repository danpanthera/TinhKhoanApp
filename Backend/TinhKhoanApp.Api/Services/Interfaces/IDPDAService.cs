using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho DPDA Service - Business logic layer cho thẻ nộp đơn gửi tiết kiệm
    /// Implements service methods with DTO mapping và business rules
    /// 13 business methods expected following DP01 pattern
    /// </summary>
    public interface IDPDAService
    {
        #region CRUD Operations với DTO Mapping

        /// <summary>
        /// Lấy tất cả DPDA với paging và DTO mapping
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>PagedResult với DPDA Preview DTOs</returns>
        Task<PagedResult<DPDAPreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy DPDA theo ID với full details DTO
        /// </summary>
        /// <param name="id">DPDA ID</param>
        /// <returns>DPDA Details DTO</returns>
        Task<DPDADetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới DPDA từ Create DTO
        /// </summary>
        /// <param name="createDto">DPDA Create DTO</param>
        /// <returns>Created DPDA Details DTO</returns>
        Task<DPDADetailsDto> CreateAsync(DPDACreateDto createDto);

        /// <summary>
        /// Cập nhật DPDA từ Update DTO
        /// </summary>
        /// <param name="id">DPDA ID</param>
        /// <param name="updateDto">DPDA Update DTO</param>
        /// <returns>Updated DPDA Details DTO</returns>
        Task<DPDADetailsDto?> UpdateAsync(int id, DPDAUpdateDto updateDto);

        /// <summary>
        /// Xóa DPDA theo ID
        /// </summary>
        /// <param name="id">DPDA ID</param>
        /// <returns>True nếu xóa thành công</returns>
        Task<bool> DeleteAsync(int id);

        #endregion

        #region Business Query Methods với DTO Mapping

        /// <summary>
        /// Lấy DPDA theo ngày dữ liệu với Preview DTO
        /// </summary>
        /// <param name="date">Ngày dữ liệu</param>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>PagedResult với DPDA Preview DTOs theo ngày</returns>
        Task<PagedResult<DPDAPreviewDto>> GetByDateAsync(DateTime date, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy DPDA theo mã chi nhánh với Summary DTO
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA Summary DTOs theo chi nhánh</returns>
        Task<IEnumerable<DPDASummaryDto>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy DPDA theo mã khách hàng với Details DTO
        /// </summary>
        /// <param name="customerCode">Mã khách hàng</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA Details DTOs theo khách hàng</returns>
        Task<IEnumerable<DPDADetailsDto>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy DPDA theo số tài khoản với Preview DTO
        /// </summary>
        /// <param name="accountNumber">Số tài khoản</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA Preview DTOs theo tài khoản</returns>
        Task<IEnumerable<DPDAPreviewDto>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy DPDA theo số thẻ với Details DTO
        /// </summary>
        /// <param name="cardNumber">Số thẻ</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA Details DTOs theo số thẻ</returns>
        Task<IEnumerable<DPDADetailsDto>> GetByCardNumberAsync(string cardNumber, int maxResults = 100);

        /// <summary>
        /// Lấy DPDA theo trạng thái với Summary DTO
        /// </summary>
        /// <param name="status">Trạng thái thẻ</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA Summary DTOs theo trạng thái</returns>
        Task<IEnumerable<DPDASummaryDto>> GetByStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê DPDA theo chi nhánh và trạng thái
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="status">Trạng thái</param>
        /// <returns>Số lượng thẻ theo điều kiện</returns>
        Task<int> GetCardCountByBranchAndStatusAsync(string branchCode, string status);

        #endregion

        #region CSV Import Operations

        /// <summary>
        /// Import DPDA data từ CSV với validation và DTO result
        /// </summary>
        /// <param name="csvData">CSV data content</param>
        /// <param name="fileName">Tên file CSV</param>
        /// <returns>Import Result DTO với thống kê</returns>
        Task<DPDAImportResultDto> ImportFromCsvAsync(string csvData, string fileName);

        #endregion
    }
}
