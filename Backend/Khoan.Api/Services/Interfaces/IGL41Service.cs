using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.GL41;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho GL41 Service - cung cấp business logic cho GL41
    /// </summary>
    public interface IGL41Service
    {
        /// <summary>
        /// Lấy tất cả GL41 với phân trang
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetAllAsync(int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// Lấy preview GL41 (danh sách với thông tin tóm tắt)
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41PreviewDto>>> PreviewAsync(int take = 20);

        /// <summary>
        /// Lấy chi tiết GL41 theo ID
        /// </summary>
        Task<ApiResponse<GL41DetailsDto>> GetByIdAsync(long id);

        /// <summary>
        /// Lấy GL41 theo ngày dữ liệu
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy GL41 theo mã đơn vị
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByUnitAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Lấy GL41 theo mã tài khoản
        /// </summary>
        Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Tạo mới GL41
        /// </summary>
        Task<ApiResponse<GL41DetailsDto>> CreateAsync(GL41CreateDto dto);

        /// <summary>
        /// Cập nhật GL41
        /// </summary>
        Task<ApiResponse<GL41DetailsDto>> UpdateAsync(GL41UpdateDto dto);

        /// <summary>
        /// Xóa GL41
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(long id);

        /// <summary>
        /// Lấy thống kê GL41 theo đơn vị
        /// </summary>
        Task<ApiResponse<GL41SummaryByUnitDto>> SummaryByUnitAsync(string unitCode, DateTime? date = null);
    }
}
