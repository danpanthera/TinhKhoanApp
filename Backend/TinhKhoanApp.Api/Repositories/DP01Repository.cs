using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Entities;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// DP01 Repository - triển khai IDP01Repository
    /// CSV Business Columns: 63 columns from 7800_dp01_20241231.csv
    /// </summary>
    public class DP01Repository : Repository<DP01Entity>, IDP01Repository
    {
        public DP01Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy dữ liệu DP01 gần đây nhất, sắp xếp theo CreatedAt
        /// </summary>
        public new async Task<IEnumerable<DP01Entity>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(dp01 => dp01.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01Entity>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(dp01 => dp01.NGAY_DL.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01Entity>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(dp01 => dp01.MA_CN == branchCode)
                .OrderByDescending(dp01 => dp01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01Entity>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(dp01 => dp01.MA_KH == customerCode)
                .OrderByDescending(dp01 => dp01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01Entity>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            return await _dbSet
                .Where(dp01 => dp01.SO_TAI_KHOAN == accountNumber)
                .OrderByDescending(dp01 => dp01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null)
        {
            var query = _dbSet.Where(dp01 => dp01.MA_CN == branchCode);

            if (date.HasValue)
            {
                query = query.Where(dp01 => dp01.NGAY_DL.Date == date.Value.Date);
            }

            return await query
                .Where(dp01 => dp01.CURRENT_BALANCE.HasValue)
                .SumAsync(dp01 => dp01.CURRENT_BALANCE ?? 0);
        }
    }
}
