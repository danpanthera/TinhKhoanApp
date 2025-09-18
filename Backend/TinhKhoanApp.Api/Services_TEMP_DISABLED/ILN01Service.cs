using Khoan.Api.Models.DataTables;
using Khoan.Api.Models.Dtos;

namespace Khoan.Api.Services
{
    /// <summary>
    /// Interface dịch vụ cho dữ liệu LN01 (Thông tin khoản vay)
    /// </summary>
    public interface ILN01Service
    {
        /// <summary>
        /// Lấy tất cả bản ghi LN01
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetAllAsync();

        /// <summary>
        /// Lấy bản ghi LN01 theo ID
        /// </summary>
        Task<LN01DTO?> GetByIdAsync(long id);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 gần đây nhất
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 theo ngày
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 theo số tài khoản
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 theo nhóm nợ
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách bản ghi LN01 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<LN01DTO>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy tổng dư nợ theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalLoanAmountByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng dư nợ theo loại tiền
        /// </summary>
        Task<decimal> GetTotalLoanAmountByCurrencyAsync(string currency, DateTime? date = null);

        /// <summary>
        /// Tạo mới bản ghi LN01
        /// </summary>
        Task<LN01DTO> CreateAsync(CreateLN01DTO createDto);

        /// <summary>
        /// Cập nhật bản ghi LN01
        /// </summary>
        Task<LN01DTO?> UpdateAsync(long id, UpdateLN01DTO updateDto);

        /// <summary>
        /// Xóa bản ghi LN01
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Kiểm tra sự tồn tại của bản ghi LN01 theo ID
        /// </summary>
        Task<bool> ExistsAsync(long id);

        /// <summary>
        /// Lưu các thay đổi
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}
