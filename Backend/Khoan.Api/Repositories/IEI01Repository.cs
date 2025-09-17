using DataTables = Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Interface Repository cho EI01 - mở rộng từ IRepository generic
    /// </summary>
    public interface IEI01Repository : IRepository<DataTables.EI01>
    {
        /// <summary>
        /// Lấy dữ liệu EI01 mới nhất
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo loại khách hàng
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByCustomerTypeAsync(string customerType, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại (bất kỳ loại dịch vụ nào)
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByPhoneNumberAsync(string phoneNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ EMB
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByEMBStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ OTT
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByOTTStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ SMS
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetBySMSStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy số lượng khách hàng theo chi nhánh và loại dịch vụ+trạng thái
        /// </summary>
        Task<int> GetCustomerCountByBranchAndServiceAsync(string branchCode, string serviceType, string status);

        /// <summary>
        /// Lấy dữ liệu EI01 được đăng ký trong khoảng thời gian
        /// </summary>
        Task<IEnumerable<DataTables.EI01>> GetByRegistrationDateRangeAsync(DateTime fromDate, DateTime toDate, string serviceType, int maxResults = 100);

        /// <summary>
        /// Cập nhật nhiều EI01
        /// </summary>
        void UpdateRange(IEnumerable<DataTables.EI01> entities);
    }
}
