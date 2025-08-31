using Khoan.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.Entities;

namespace Khoan.Api.Repositories
{
    public class GL02Repository : Repository<GL02Entity>, IGL02Repository
    {
        public GL02Repository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<GL02Entity> data, int totalCount)> GetAllPagedAsync(int page, int size)
        {
            var query = _context.Set<GL02Entity>().AsQueryable();
            var totalCount = await query.CountAsync();
            var data = await query
                .OrderByDescending(x => x.NGAY_DL)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
            
            return (data, totalCount);
        }

        public async Task<bool> DeleteByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var records = _context.Set<GL02Entity>()
                .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate);
            
            _context.Set<GL02Entity>().RemoveRange(records);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<object>> GetSummaryByUnitAsync()
        {
            return await _context.Set<GL02Entity>()
                .GroupBy(x => x.UNIT)
                .Select(g => new
                {
                    Unit = g.Key,
                    Count = g.Count(),
                    TotalDrAmount = g.Sum(x => x.DRAMOUNT ?? 0),
                    TotalCrAmount = g.Sum(x => x.CRAMOUNT ?? 0)
                })
                .ToListAsync();
        }

        public async Task BulkInsertAsync(IEnumerable<GL02Entity> entities)
        {
            await _context.Set<GL02Entity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<GL02Entity?> GetByIdAsync(int id)
        {
            return await _context.Set<GL02Entity>().FindAsync((long)id);
        }

        public async Task<GL02Entity> CreateAsync(GL02Entity entity)
        {
            entity.CREATED_DATE = DateTime.UtcNow;
            entity.UPDATED_DATE = DateTime.UtcNow;
            await _context.Set<GL02Entity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<GL02Entity> UpdateAsync(GL02Entity entity)
        {
            entity.UPDATED_DATE = DateTime.UtcNow;
            _context.Set<GL02Entity>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<GL02Entity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
