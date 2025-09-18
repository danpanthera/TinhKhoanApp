using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.Entities;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// EI01 Repository - triển khai IEI01Repository
    /// </summary>
    public class EI01Repository : Repository<EI01Entity>, IEI01Repository
    {
        public EI01Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy dữ liệu EI01 gần đây nhất, sắp xếp theo CreatedAt
        /// </summary>
        public new async Task<IEnumerable<EI01Entity>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(ei01 => ei01.NGAY_DL.Date == date.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã chi nhánh
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 => ei01.MA_CN == branchCode)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã khách hàng
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 => ei01.MA_KH == customerCode)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo loại khách hàng
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByCustomerTypeAsync(string customerType, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 => ei01.LOAI_KH == customerType)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại (bất kỳ loại dịch vụ nào)
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByPhoneNumberAsync(string phoneNumber, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 =>
                    ei01.SDT_EMB == phoneNumber ||
                    ei01.SDT_OTT == phoneNumber ||
                    ei01.SDT_SMS == phoneNumber ||
                    ei01.SDT_SAV == phoneNumber ||
                    ei01.SDT_LN == phoneNumber)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ EMB
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByEMBStatusAsync(string status, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 => ei01.TRANG_THAI_EMB == status)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ OTT
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByOTTStatusAsync(string status, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 => ei01.TRANG_THAI_OTT == status)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ SMS
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetBySMSStatusAsync(string status, int maxResults = 100)
        {
            return await _dbSet
                .Where(ei01 => ei01.TRANG_THAI_SMS == status)
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy số lượng khách hàng theo chi nhánh và loại dịch vụ+trạng thái
        /// </summary>
        public async Task<int> GetCustomerCountByBranchAndServiceAsync(string branchCode, string serviceType, string status)
        {
            var query = _dbSet.Where(ei01 => ei01.MA_CN == branchCode);

            switch (serviceType.ToUpper())
            {
                case "EMB":
                    query = query.Where(ei01 => ei01.TRANG_THAI_EMB == status);
                    break;
                case "OTT":
                    query = query.Where(ei01 => ei01.TRANG_THAI_OTT == status);
                    break;
                case "SMS":
                    query = query.Where(ei01 => ei01.TRANG_THAI_SMS == status);
                    break;
                case "SAV":
                    query = query.Where(ei01 => ei01.TRANG_THAI_SAV == status);
                    break;
                case "LN":
                    query = query.Where(ei01 => ei01.TRANG_THAI_LN == status);
                    break;
                default:
                    // Mặc định không lọc theo trạng thái
                    break;
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Lấy dữ liệu EI01 được đăng ký trong khoảng thời gian
        /// </summary>
        public async Task<IEnumerable<EI01Entity>> GetByRegistrationDateRangeAsync(DateTime fromDate, DateTime toDate, string serviceType, int maxResults = 100)
        {
            var query = _dbSet.AsQueryable();

            switch (serviceType.ToUpper())
            {
                case "EMB":
                    query = query.Where(ei01 =>
                        ei01.NGAY_DK_EMB.HasValue &&
                        ei01.NGAY_DK_EMB >= fromDate &&
                        ei01.NGAY_DK_EMB <= toDate);
                    break;
                case "OTT":
                    query = query.Where(ei01 =>
                        ei01.NGAY_DK_OTT.HasValue &&
                        ei01.NGAY_DK_OTT >= fromDate &&
                        ei01.NGAY_DK_OTT <= toDate);
                    break;
                case "SMS":
                    query = query.Where(ei01 =>
                        ei01.NGAY_DK_SMS.HasValue &&
                        ei01.NGAY_DK_SMS >= fromDate &&
                        ei01.NGAY_DK_SMS <= toDate);
                    break;
                case "SAV":
                    query = query.Where(ei01 =>
                        ei01.NGAY_DK_SAV.HasValue &&
                        ei01.NGAY_DK_SAV >= fromDate &&
                        ei01.NGAY_DK_SAV <= toDate);
                    break;
                case "LN":
                    query = query.Where(ei01 =>
                        ei01.NGAY_DK_LN.HasValue &&
                        ei01.NGAY_DK_LN >= fromDate &&
                        ei01.NGAY_DK_LN <= toDate);
                    break;
                default:
                    // Mặc định không lọc theo ngày đăng ký
                    return Enumerable.Empty<EI01Entity>().ToList();
            }

            return await query
                .OrderByDescending(ei01 => ei01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Cập nhật nhiều EI01
        /// </summary>
        public void UpdateRange(IEnumerable<EI01Entity> entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
