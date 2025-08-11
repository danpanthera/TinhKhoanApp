using TinhKhoanApp.Api.Models.Dtos.RR01;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu RR01 (Risk Report)
    /// </summary>
    public interface IRR01Service
    {
        /// <summary>
        /// Lấy tất cả bản ghi RR01
        /// </summary>
        Task<IEnumerable<RR01DetailsDto>> GetAllAsync();

        /// <summary>
        /// Lấy bản ghi RR01 theo ID
        /// </summary>
        Task<RR01DetailsDto?> GetByIdAsync(long id);

        /// <summary>
        /// Lấy danh sách bản ghi RR01 gần đây nhất
        /// </summary>
        Task<IEnumerable<RR01PreviewDto>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy danh sách bản ghi RR01 theo ngày
        /// </summary>
        Task<IEnumerable<RR01PreviewDto>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Tạo bản ghi RR01 mới
        /// </summary>
        Task<RR01DetailsDto> CreateAsync(RR01CreateDto createDto);

        /// <summary>
        /// Cập nhật bản ghi RR01
        /// </summary>
        Task<RR01DetailsDto?> UpdateAsync(long id, RR01UpdateDto updateDto);

        /// <summary>
        /// Xóa bản ghi RR01
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Lấy thống kê tổng quan RR01
        /// </summary>
        Task<RR01SummaryDto> GetSummaryAsync();

        /// <summary>
        /// Lấy danh sách RR01 với phân trang
        /// </summary>
        Task<(IEnumerable<RR01PreviewDto> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);

        /// <summary>
        /// Tìm kiếm RR01 theo criteria
        /// </summary>
        Task<IEnumerable<RR01PreviewDto>> SearchAsync(string? keyword, string? brcd = null, string? loaiKh = null);

        /// <summary>
        /// Lấy lịch sử thay đổi của RR01
        /// </summary>
        Task<IEnumerable<RR01DetailsDto>> GetHistoryAsync(long id);

        /// <summary>
        /// Bulk import RR01 data
        /// </summary>
        Task<(int Success, int Failed)> BulkImportAsync(IEnumerable<RR01CreateDto> dtos);

        /// <summary>
        /// Validate dữ liệu RR01
        /// </summary>
        Task<bool> ValidateAsync(RR01CreateDto dto);

        /// <summary>
        /// Kiểm tra sức khỏe của service
        /// </summary>
        Task<bool> IsHealthyAsync();
    }
}
