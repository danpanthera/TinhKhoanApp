using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository cho DP01 - sử dụng DataTables.DP01 model
    /// Chuẩn hóa: CSV (63 columns) -> Database -> DataTables.DP01 -> Repository
    /// </summary>
    public class DP01DataTableRepository : Repository<DP01>, IDP01DataTableRepository
    {
        public DP01DataTableRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<IEnumerable<DP01>> GetRecentAsync(int count = 10)
        {
            return await _context.Set<DP01>()
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01>> GetByDateAsync(DateTime date)
        {
            return await _context.Set<DP01>()
                .Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == date.Date)
                .OrderBy(x => x.MA_CN)
                .ThenBy(x => x.MA_KH)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _context.Set<DP01>()
                .Where(x => x.MA_CN == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _context.Set<DP01>()
                .Where(x => x.MA_KH == customerCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            return await _context.Set<DP01>()
                .Where(x => x.SO_TAI_KHOAN == accountNumber)
                .OrderByDescending(x => x.NGAY_DL)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null)
        {
            var query = _context.Set<DP01>().Where(x => x.MA_CN == branchCode);

            if (date.HasValue)
            {
                query = query.Where(x => x.NGAY_DL.HasValue && x.NGAY_DL.Value.Date == date.Value.Date);
            }

            return await query
                .Where(x => x.CURRENT_BALANCE.HasValue)
                .SumAsync(x => x.CURRENT_BALANCE.Value);
        }

        public async Task<IEnumerable<DP01>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<DP01>()
                .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate)
                .OrderBy(x => x.NGAY_DL)
                .ThenBy(x => x.MA_CN)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01>> GetByCurrencyAsync(string currency)
        {
            return await _context.Set<DP01>()
                .Where(x => x.CCY == currency)
                .OrderByDescending(x => x.NGAY_DL)
                .ToListAsync();
        }

        public async Task<IEnumerable<DP01>> SearchAsync(string searchTerm)
        {
            return await _context.Set<DP01>()
                .Where(x => x.MA_CN.Contains(searchTerm) ||
                           x.MA_KH.Contains(searchTerm) ||
                           x.TEN_KH.Contains(searchTerm) ||
                           x.SO_TAI_KHOAN.Contains(searchTerm))
                .OrderByDescending(x => x.NGAY_DL)
                .Take(100)
                .ToListAsync();
        }

        public async Task<int> GetCountByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<DP01>()
                .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate)
                .CountAsync();
        }

        public async Task<int> BulkCreateAsync(IEnumerable<DP01> dp01Records)
        {
            await _context.Set<DP01>().AddRangeAsync(dp01Records);
            return await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, object>> GetStatisticsAsync()
        {
            var totalCount = await _context.Set<DP01>().CountAsync();
            var totalBalance = await _context.Set<DP01>()
                .Where(x => x.CURRENT_BALANCE.HasValue)
                .SumAsync(x => x.CURRENT_BALANCE.Value);

            var averageBalance = totalCount > 0 ? totalBalance / totalCount : 0;

            var branchCount = await _context.Set<DP01>()
                .Select(x => x.MA_CN)
                .Distinct()
                .CountAsync();

            var currencyCount = await _context.Set<DP01>()
                .Select(x => x.CCY)
                .Distinct()
                .CountAsync();

            return new Dictionary<string, object>
            {
                ["TotalRecords"] = totalCount,
                ["TotalBalance"] = totalBalance,
                ["AverageBalance"] = averageBalance,
                ["BranchCount"] = branchCount,
                ["CurrencyCount"] = currencyCount,
                ["LastUpdated"] = DateTime.Now
            };
        }
    }
}
