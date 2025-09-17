using System.Linq.Expressions;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Generic Repository Interface - định nghĩa các phương thức cơ bản để truy cập dữ liệu
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Lấy tất cả entities
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy entity theo điều kiện
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Lấy một entity theo id
        /// </summary>
        Task<T?> GetByIdAsync(long id);

        /// <summary>
        /// Lấy các entities gần đây nhất, sắp xếp theo CreatedAt giảm dần
        /// </summary>
        Task<IEnumerable<T>> GetRecentAsync(int count);

        /// <summary>
        /// Thêm entity mới
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Thêm nhiều entities
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Xóa entity
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Xóa nhiều entities
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Đếm số lượng entities
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Đếm số lượng entities theo điều kiện
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Kiểm tra entity tồn tại
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Lưu các thay đổi
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
