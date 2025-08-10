using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository implementation cho GL02 - Giao dịch sổ cái
    /// </summary>
    public class GL02Repository : Repository<GL02>, IGL02Repository
    {
        private readonly ApplicationDbContext _context;

        public GL02Repository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Trả về DbContext cho việc sử dụng trong service layer
        /// </summary>
        public ApplicationDbContext GetDbContext() => _context;

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetRecentAsync(int count = 10)
        {
            return await _context.GL02
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.NGAY_DL.Date == date.Date)
                .OrderByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByUnitAsync(string unit, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.UNIT == unit)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.TRBRCD == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.LOCAC == accountCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByTransactionCodeAsync(string transactionCode, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.TRCD == transactionCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByCustomerAsync(string customer, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.CUSTOMER == customer)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByTransactionTypeAsync(string transactionType, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.TRTP == transactionType)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalTransactionsByUnitAsync(string unit, string drCrFlag)
        {
            if (drCrFlag.ToUpper() == "DR")
            {
                return await _context.GL02
                    .Where(x => x.UNIT == unit && x.DRAMOUNT > 0)
                    .SumAsync(x => x.DRAMOUNT ?? 0);
            }
            else
            {
                return await _context.GL02
                    .Where(x => x.UNIT == unit && x.CRAMOUNT > 0)
                    .SumAsync(x => x.CRAMOUNT ?? 0);
            }
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalTransactionsByDateAsync(DateTime date, string drCrFlag)
        {
            if (drCrFlag.ToUpper() == "DR")
            {
                return await _context.GL02
                    .Where(x => x.NGAY_DL.Date == date.Date && x.DRAMOUNT > 0)
                    .SumAsync(x => x.DRAMOUNT ?? 0);
            }
            else
            {
                return await _context.GL02
                    .Where(x => x.NGAY_DL.Date == date.Date && x.CRAMOUNT > 0)
                    .SumAsync(x => x.CRAMOUNT ?? 0);
            }
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalTransactionsByBranchAsync(string branchCode, string drCrFlag)
        {
            if (drCrFlag.ToUpper() == "DR")
            {
                return await _context.GL02
                    .Where(x => x.TRBRCD == branchCode && x.DRAMOUNT > 0)
                    .SumAsync(x => x.DRAMOUNT ?? 0);
            }
            else
            {
                return await _context.GL02
                    .Where(x => x.TRBRCD == branchCode && x.CRAMOUNT > 0)
                    .SumAsync(x => x.CRAMOUNT ?? 0);
            }
        }

        /// <inheritdoc/>
        public async Task<GL02?> GetByIdAsync(long id)
        {
            return await _context.GL02.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            return await _context.GL02
                .Where(x => x.NGAY_DL.Date >= fromDate.Date && x.NGAY_DL.Date <= toDate.Date)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }
    }
}
