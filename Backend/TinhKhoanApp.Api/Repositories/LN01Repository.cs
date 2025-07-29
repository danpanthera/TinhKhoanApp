using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository cho dữ liệu LN01 (Thông tin khoản vay)
    /// </summary>
    public class LN01Repository : Repository<LN01>, ILN01Repository
    {
        private readonly ApplicationDbContext _context;

        public LN01Repository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Trả về DbContext cho việc sử dụng trong service layer
        /// </summary>
        public ApplicationDbContext GetDbContext() => _context;

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetRecentAsync(int count = 10)
        {
            return await _context.LN01s
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            return await _context.LN01s
                .Where(x => x.NGAY_DL != null && x.NGAY_DL.Value.Date == date.Date)
                .OrderByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _context.LN01s
                .Where(x => x.BRCD == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _context.LN01s
                .Where(x => x.CUSTSEQ == customerCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            return await _context.LN01s
                .Where(x => x.TAI_KHOAN == accountNumber)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalLoanAmountByBranchAsync(string branchCode, DateTime? date = null)
        {
            var query = _context.LN01s.Where(x => x.BRCD == branchCode);

            if (date.HasValue)
            {
                query = query.Where(x => x.NGAY_DL != null && x.NGAY_DL.Value.Date == date.Value.Date);
            }
            else
            {
                // Nếu không có ngày cụ thể, lấy dữ liệu ngày mới nhất
                var latestDate = await _context.LN01s
                    .Where(x => x.BRCD == branchCode && x.NGAY_DL != null)
                    .OrderByDescending(x => x.NGAY_DL)
                    .Select(x => x.NGAY_DL)
                    .FirstOrDefaultAsync();

                if (latestDate.HasValue)
                {
                    query = query.Where(x => x.NGAY_DL != null && x.NGAY_DL.Value.Date == latestDate.Value.Date);
                }
            }

            return await query.SumAsync(x => x.DU_NO_THUC_TE ?? 0);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalLoanAmountByCurrencyAsync(string currency, DateTime? date = null)
        {
            var query = _context.LN01s.Where(x => x.CCY == currency);

            if (date.HasValue)
            {
                query = query.Where(x => x.NGAY_DL != null && x.NGAY_DL.Value.Date == date.Value.Date);
            }
            else
            {
                // Nếu không có ngày cụ thể, lấy dữ liệu ngày mới nhất
                var latestDate = await _context.LN01s
                    .Where(x => x.CCY == currency && x.NGAY_DL != null)
                    .OrderByDescending(x => x.NGAY_DL)
                    .Select(x => x.NGAY_DL)
                    .FirstOrDefaultAsync();

                if (latestDate.HasValue)
                {
                    query = query.Where(x => x.NGAY_DL != null && x.NGAY_DL.Value.Date == latestDate.Value.Date);
                }
            }

            return await query.SumAsync(x => x.DU_NO_THUC_TE ?? 0);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100)
        {
            return await _context.LN01s
                .Where(x => x.NHOM_NO == debtGroup)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            return await _context.LN01s
                .Where(x => x.NGAY_DL != null && x.NGAY_DL.Value.Date >= fromDate.Date && x.NGAY_DL.Value.Date <= toDate.Date)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<LN01?> GetByIdAsync(long id)
        {
            return await _context.LN01s.FindAsync(id);
        }
    }
}
