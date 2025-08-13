using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository cho dữ liệu LN03 (20 business columns theo CSV)
    /// Dựa trên Repository<T> dùng DataTables.LN03 để giữ CSV-first schema
    /// </summary>
    public interface ILN03DataRepository : IRepository<LN03>
    {
        Task<IEnumerable<LN03>> GetByDateAsync(DateTime date, int maxResults = 100);
        Task<IEnumerable<LN03>> GetByBranchAsync(string branchCode, int maxResults = 100);
        Task<IEnumerable<LN03>> GetByCustomerAsync(string customerCode, int maxResults = 100);
    }

    public class LN03Repository : Repository<LN03>, ILN03DataRepository
    {
        private readonly new ApplicationDbContext _context;

        public LN03Repository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LN03>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.NGAY_DL.Date == date.Date)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<LN03>> GetByBranchAsync(string branchCode, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.MACHINHANH == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<LN03>> GetByCustomerAsync(string customerCode, int maxResults = 100)
        {
            return await _context.LN03s
                .Where(x => x.MAKH == customerCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }
    }
}
