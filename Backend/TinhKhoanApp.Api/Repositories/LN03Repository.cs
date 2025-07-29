using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository cho dữ liệu LN03 (Dữ liệu phục hồi khoản vay)
    /// </summary>
    public class LN03Repository : Repository<LN03>, ILN03Repository
    {
        private readonly ApplicationDbContext _context;

        public LN03Repository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Trả về DbContext cho việc sử dụng trong service layer
        /// </summary>
        public ApplicationDbContext GetDbContext() => _context;

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetRecentAsync(int count = 10)
        {
            return await _context.LN03s
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<LN03?> GetByIdAsync(long id)
        {
            return await _context.LN03s.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.NGAY_DL.Date == date.Date)
                .OrderByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.MACHINHANH == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.MAKH == customerCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetByOfficerCodeAsync(string officerCode, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.MACBTD == officerCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.NHOMNO == debtGroup)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.NGAY_DL.Date >= fromDate.Date && x.NGAY_DL.Date <= toDate.Date)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalRiskAmountByBranchAsync(string branchCode, DateTime? date = null)
        {
            var query = _context.LN03s.Where(x => x.MACHINHANH == branchCode);

            if (date.HasValue)
            {
                query = query.Where(x => x.NGAY_DL.Date == date.Value.Date);
            }
            else
            {
                // Nếu không có ngày cụ thể, lấy dữ liệu ngày mới nhất
                var latestDate = await _context.LN03s
                    .Where(x => x.MACHINHANH == branchCode)
                    .OrderByDescending(x => x.NGAY_DL)
                    .Select(x => x.NGAY_DL)
                    .FirstOrDefaultAsync();

                if (latestDate != default)
                {
                    query = query.Where(x => x.NGAY_DL.Date == latestDate.Date);
                }
            }

            // Tổng số tiền xử lý rủi ro (chuyển từ string sang decimal)
            decimal total = 0;
            var results = await query.ToListAsync();

            foreach (var item in results)
            {
                if (decimal.TryParse(item.SOTIENXLRR?.Replace(",", ""), out decimal amount))
                {
                    total += amount;
                }
            }

            return total;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalDebtRecoveryByBranchAsync(string branchCode, DateTime? date = null)
        {
            var query = _context.LN03s.Where(x => x.MACHINHANH == branchCode);

            if (date.HasValue)
            {
                query = query.Where(x => x.NGAY_DL.Date == date.Value.Date);
            }
            else
            {
                // Nếu không có ngày cụ thể, lấy dữ liệu ngày mới nhất
                var latestDate = await _context.LN03s
                    .Where(x => x.MACHINHANH == branchCode)
                    .OrderByDescending(x => x.NGAY_DL)
                    .Select(x => x.NGAY_DL)
                    .FirstOrDefaultAsync();

                if (latestDate != default)
                {
                    query = query.Where(x => x.NGAY_DL.Date == latestDate.Date);
                }
            }

            // Tổng thu nợ sau xử lý (chuyển từ string sang decimal)
            decimal total = 0;
            var results = await query.ToListAsync();

            foreach (var item in results)
            {
                if (decimal.TryParse(item.THUNOSAUXL?.Replace(",", ""), out decimal amount))
                {
                    total += amount;
                }
            }

            return total;
        }
    }
}
