using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Generic Repository - triển khai cơ bản cho IRepository
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// DbContext for database access
        /// </summary>
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// DbSet for entity operations
        /// </summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Application DB Context</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Lấy tất cả entities
        /// </summary>
        /// <returns>Collection of entities</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Lấy entity theo điều kiện
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <returns>Collection of filtered entities</returns>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Lấy một entity theo id
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity or null</returns>
        public virtual async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Lấy các entities gần đây nhất, sắp xếp theo CreatedAt giảm dần
        /// </summary>
        /// <param name="count">Number of entities to return</param>
        /// <returns>Collection of most recent entities</returns>
        public virtual async Task<IEnumerable<T>> GetRecentAsync(int count)
        {
            // Note: This assumes T has a CreatedAt property
            // If that's not the case, override this in derived repositories
            return await _dbSet
                .OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Thêm entity mới
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Thêm nhiều entities
        /// </summary>
        /// <param name="entities">Collection of entities to add</param>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Xóa entity
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Xóa nhiều entities
        /// </summary>
        /// <param name="entities">Collection of entities to remove</param>
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        /// Đếm số lượng entities
        /// </summary>
        /// <returns>Total count of entities</returns>
        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        /// <summary>
        /// Đếm số lượng entities theo điều kiện
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <returns>Count of entities matching the condition</returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Kiểm tra entity tồn tại
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <returns>True if any entity matches the condition</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Lưu các thay đổi
        /// </summary>
        /// <returns>Number of affected rows</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
