using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu LN03 (Dữ liệu phục hồi khoản vay)
    /// </summary>
    public interface ILN03Service
    {
        /// <summary>
        /// Lấy tất cả bản ghi LN03
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetAllAsync();

        /// <summary>
        /// Lấy bản ghi LN03 theo ID
        /// </summary>
        Task<LN03DTO?> GetByIdAsync(long id);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 gần đây nhất
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 theo ngày
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 theo mã cán bộ tín dụng
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetByOfficerCodeAsync(string officerCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 theo nhóm nợ
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN03 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<LN03DTO>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy tổng số tiền xử lý rủi ro theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalRiskAmountByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng thu nợ sau xử lý theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalDebtRecoveryByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Tạo mới bản ghi LN03
        /// </summary>
        Task<LN03DTO> CreateAsync(CreateLN03DTO createDto);

        /// <summary>
        /// Cập nhật bản ghi LN03
        /// </summary>
        Task<LN03DTO?> UpdateAsync(long id, UpdateLN03DTO updateDto);

        /// <summary>
        /// Xóa bản ghi LN03
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Kiểm tra sự tồn tại của bản ghi LN03 theo ID
        /// </summary>
        Task<bool> ExistsAsync(long id);

        /// <summary>
        /// Lưu các thay đổi
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}
