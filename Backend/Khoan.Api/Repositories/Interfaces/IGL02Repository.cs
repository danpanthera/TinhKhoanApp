using Khoan.Api.Models.Entities;

namespace Khoan.Api.Repositories.Interfaces
{
    public interface IGL02Repository
    {
        Task<IEnumerable<GL02Entity>> GetAllAsync();
        Task<(IEnumerable<GL02Entity> data, int totalCount)> GetAllPagedAsync(int page, int size);
        Task<GL02Entity?> GetByIdAsync(int id);
        Task<GL02Entity> CreateAsync(GL02Entity entity);
        Task<GL02Entity> UpdateAsync(GL02Entity entity);
        Task DeleteAsync(int id);
        Task<bool> DeleteByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<object>> GetSummaryByUnitAsync();
        Task BulkInsertAsync(IEnumerable<GL02Entity> entities);
    }
}
