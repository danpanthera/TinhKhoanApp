using Khoan.Api.Models.Dtos.EI01;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho EI01 Service - E-Banking Information Service
    /// Tương đương với DP01Service và DPDAService pattern
    /// </summary>
    public interface IEI01Service
    {
        // === BASIC CRUD OPERATIONS ===
        /// <summary>
        /// Lấy preview dữ liệu EI01 gần đây
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi EI01 theo ID
        /// </summary>
        Task<EI01DetailsDto?> GetEI01DetailAsync(long id);

        /// <summary>
        /// Tạo mới bản ghi EI01
        /// </summary>
        Task<long> CreateEI01Async(EI01CreateDto createDto);

        /// <summary>
        /// Cập nhật bản ghi EI01
        /// </summary>
        Task<bool> UpdateEI01Async(long id, EI01UpdateDto updateDto);

        /// <summary>
        /// Xóa bản ghi EI01
        /// </summary>
        Task<bool> DeleteEI01Async(long id);

        // === BUSINESS QUERY OPERATIONS ===
        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo loại khách hàng
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerTypeAsync(string customerType, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại (tìm trong tất cả dịch vụ)
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByPhoneNumberAsync(string phoneNumber, int maxResults = 100);

        // === E-BANKING SERVICE SPECIFIC OPERATIONS ===
        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ EMB
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByEMBStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ OTT
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByOTTStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ SMS
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01BySMSStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 được đăng ký trong khoảng thời gian
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByRegistrationDateRangeAsync(DateTime fromDate, DateTime toDate, string serviceType, int maxResults = 100);

        // === ANALYTICS & SUMMARY OPERATIONS ===
        /// <summary>
        /// Lấy thống kê EI01 theo chi nhánh và loại dịch vụ
        /// </summary>
        Task<EI01SummaryDto> GetEI01SummaryByBranchAsync(string branchCode);

        /// <summary>
        /// Lấy số lượng khách hàng theo chi nhánh và dịch vụ với trạng thái
        /// </summary>
        Task<int> GetCustomerCountByBranchAndServiceAsync(string branchCode, string serviceType, string status);

        // === BATCH OPERATIONS ===
        /// <summary>
        /// Import dữ liệu EI01 từ CSV
        /// </summary>
        Task<EI01ImportResultDto> ImportEI01FromCsvAsync(Stream csvStream, string fileName);

        /// <summary>
        /// Cập nhật batch nhiều bản ghi EI01
        /// </summary>
        Task<int> UpdateEI01BatchAsync(IEnumerable<EI01UpdateDto> updates);

        /// <summary>
        /// Xóa batch nhiều bản ghi EI01
        /// </summary>
        Task<int> DeleteEI01BatchAsync(IEnumerable<long> ids);
    }
}
