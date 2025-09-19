using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Repositories
{
    public interface IUnitsRepository
    {
        Task<IEnumerable<Unit>> GetAllAsync();
        Task<Unit?> GetByIdAsync(int id);
        Task<IEnumerable<Unit>> GetByParentIdAsync(int? parentId);
        Task<IEnumerable<Unit>> GetRootUnitsAsync();
        Task<Unit> CreateAsync(Unit unit);
        Task<Unit> UpdateAsync(Unit unit);
        Task DeleteAsync(int id);
    }

    public class UnitsRepository : IUnitsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitsRepository> _logger;

        public UnitsRepository(ApplicationDbContext context, ILogger<UnitsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            return await _context.Units
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<Unit?> GetByIdAsync(int id)
        {
            return await _context.Units
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Unit>> GetByParentIdAsync(int? parentId)
        {
            return await _context.Units
                .Where(u => u.ParentUnitId == parentId && !u.IsDeleted)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetRootUnitsAsync()
        {
            return await GetByParentIdAsync(null);
        }

        public async Task<Unit> CreateAsync(Unit unit)
        {
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return unit;
        }

        public async Task<Unit> UpdateAsync(Unit unit)
        {
            _context.Units.Update(unit);
            await _context.SaveChangesAsync();
            return unit;
        }

        public async Task DeleteAsync(int id)
        {
            var unit = await GetByIdAsync(id);
            if (unit != null)
            {
                unit.IsDeleted = true; // Soft delete
                await _context.SaveChangesAsync();
            }
        }
    }
}