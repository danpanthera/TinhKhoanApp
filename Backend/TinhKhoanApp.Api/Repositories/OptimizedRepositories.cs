using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Services;
using System.Diagnostics;

namespace TinhKhoanApp.Api.Repositories
{
    public interface IOptimizedRepository<T> where T : class
    {
        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null);
        Task<List<T>> GetByFilterAsync(Expression<Func<T, bool>> filter, int? limit = null);
        Task<T?> GetByIdAsync(object id);
        Task<List<T>> GetBatchAsync(IEnumerable<object> ids);
        Task<T> CreateAsync(T entity);
        Task<List<T>> CreateBatchAsync(List<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
    }

    public class OptimizedEmployeeRepository : IOptimizedRepository<Employee>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<OptimizedEmployeeRepository> _logger;
        private const string CACHE_KEY_PREFIX = "employee";
        private const int DEFAULT_CACHE_MINUTES = 10;

        public OptimizedEmployeeRepository(
            ApplicationDbContext context,
            ICacheService cache,
            ILogger<OptimizedEmployeeRepository> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<Employee, bool>>? filter = null)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Build cache key based on parameters
                var filterString = filter?.ToString() ?? "all";
                var cacheKey = $"{CACHE_KEY_PREFIX}:paged:{pageNumber}:{pageSize}:{filterString.GetHashCode()}";
                
                // Try get from cache
                var cachedResult = await _cache.GetAsync<PagedResult<Employee>>(cacheKey);
                if (cachedResult != null)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("Retrieved paged employees from cache in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                    return cachedResult;
                }

                // Build query with optimizations
                var query = _context.Employees
                    .AsNoTracking()
                    .Include(e => e.Unit)
                    .Include(e => e.Position)
                    .Where(e => e.IsActive);

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                // Execute count and items queries in parallel
                var countTask = query.CountAsync();
                var itemsTask = query
                    .OrderBy(e => e.EmployeeCode)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                await Task.WhenAll(countTask, itemsTask);

                var result = new PagedResult<Employee>
                {
                    Items = itemsTask.Result,
                    TotalCount = countTask.Result,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Cache result
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(DEFAULT_CACHE_MINUTES));

                stopwatch.Stop();
                _logger.LogDebug("Retrieved {Count} paged employees from database in {ElapsedMs}ms", 
                    result.Items.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error retrieving paged employees in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<List<Employee>> GetByFilterAsync(Expression<Func<Employee, bool>> filter, int? limit = null)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}:filter:{filter.ToString().GetHashCode()}:{limit}";
                
                var cachedResult = await _cache.GetAsync<List<Employee>>(cacheKey);
                if (cachedResult != null)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("Retrieved filtered employees from cache in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                    return cachedResult;
                }

                var query = _context.Employees
                    .AsNoTracking()
                    .Include(e => e.Unit)
                    .Include(e => e.Position)
                    .Where(filter);

                if (limit.HasValue)
                {
                    query = query.Take(limit.Value);
                }

                var result = await query.ToListAsync();

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(DEFAULT_CACHE_MINUTES));

                stopwatch.Stop();
                _logger.LogDebug("Retrieved {Count} filtered employees from database in {ElapsedMs}ms", 
                    result.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error retrieving filtered employees in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<Employee?> GetByIdAsync(object id)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}:id:{id}";
                
                var cached = await _cache.GetAsync<Employee>(cacheKey);
                if (cached != null)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("Retrieved employee by ID from cache in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                    return cached;
                }

                var employee = await _context.Employees
                    .AsNoTracking()
                    .Include(e => e.Unit)
                    .Include(e => e.Position)
                    .FirstOrDefaultAsync(e => e.Id == (int)id && e.IsActive);

                if (employee != null)
                {
                    await _cache.SetAsync(cacheKey, employee, TimeSpan.FromMinutes(DEFAULT_CACHE_MINUTES));
                }

                stopwatch.Stop();
                _logger.LogDebug("Retrieved employee by ID from database in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return employee;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error retrieving employee by ID in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<List<Employee>> GetBatchAsync(IEnumerable<object> ids)
        {
            var stopwatch = Stopwatch.StartNew();
            var idList = ids.Cast<int>().ToList();
            
            try
            {
                // Try to get as many as possible from cache
                var result = new List<Employee>();
                var uncachedIds = new List<int>();

                for (int i = 0; i < idList.Count; i++)
                {
                    var cacheKey = $"{CACHE_KEY_PREFIX}:id:{idList[i]}";
                    var cached = await _cache.GetAsync<Employee>(cacheKey);
                    if (cached != null)
                    {
                        result.Add(cached);
                    }
                    else
                    {
                        uncachedIds.Add(idList[i]);
                    }
                }

                // Fetch uncached items from database
                if (uncachedIds.Any())
                {
                    var dbItems = await _context.Employees
                        .AsNoTracking()
                        .Include(e => e.Unit)
                        .Include(e => e.Position)
                        .Where(e => uncachedIds.Contains(e.Id) && e.IsActive)
                        .ToListAsync();

                    // Cache the newly fetched items
                    foreach (var item in dbItems)
                    {
                        var cacheKey = $"{CACHE_KEY_PREFIX}:id:{item.Id}";
                        await _cache.SetAsync(cacheKey, item, TimeSpan.FromMinutes(DEFAULT_CACHE_MINUTES));
                    }

                    result.AddRange(dbItems);
                }

                stopwatch.Stop();
                _logger.LogDebug("Retrieved {Count} employees in batch ({Cached} from cache, {DB} from database) in {ElapsedMs}ms", 
                    result.Count, result.Count - uncachedIds.Count, uncachedIds.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error retrieving employees in batch in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<Employee> CreateAsync(Employee entity)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                _context.Employees.Add(entity);
                await _context.SaveChangesAsync();

                // Invalidate related cache
                await _cache.RemoveByPatternAsync(CACHE_KEY_PREFIX);

                stopwatch.Stop();
                _logger.LogDebug("Created employee in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return entity;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error creating employee in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<List<Employee>> CreateBatchAsync(List<Employee> entities)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                _context.Employees.AddRange(entities);
                await _context.SaveChangesAsync();

                // Invalidate related cache
                await _cache.RemoveByPatternAsync(CACHE_KEY_PREFIX);

                stopwatch.Stop();
                _logger.LogDebug("Created {Count} employees in batch in {ElapsedMs}ms", 
                    entities.Count, stopwatch.ElapsedMilliseconds);

                return entities;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error creating employees in batch in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<Employee> UpdateAsync(Employee entity)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                _context.Employees.Update(entity);
                await _context.SaveChangesAsync();

                // Invalidate specific cache entries
                await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}:id:{entity.Id}");
                await _cache.RemoveByPatternAsync(CACHE_KEY_PREFIX);

                stopwatch.Stop();
                _logger.LogDebug("Updated employee in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return entity;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error updating employee in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(object id)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var employee = await _context.Employees.FindAsync((int)id);
                if (employee == null)
                {
                    return false;
                }

                // Soft delete
                employee.IsActive = false;
                await _context.SaveChangesAsync();

                // Invalidate cache
                await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}:id:{id}");
                await _cache.RemoveByPatternAsync(CACHE_KEY_PREFIX);

                stopwatch.Stop();
                _logger.LogDebug("Deleted employee in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error deleting employee in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<Employee, bool>>? filter = null)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var query = _context.Employees.Where(e => e.IsActive);
                
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var count = await query.CountAsync();

                stopwatch.Stop();
                _logger.LogDebug("Counted {Count} employees in {ElapsedMs}ms", count, stopwatch.ElapsedMilliseconds);

                return count;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error counting employees in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }

    // Similar pattern for SCD Repository
    public class OptimizedSCDRepository : IOptimizedRepository<RawDataRecord_SCD>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<OptimizedSCDRepository> _logger;
        private const string CACHE_KEY_PREFIX = "scd";

        public OptimizedSCDRepository(
            ApplicationDbContext context,
            ICacheService cache,
            ILogger<OptimizedSCDRepository> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResult<RawDataRecord_SCD>> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<RawDataRecord_SCD, bool>>? filter = null)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var query = _context.RawDataRecords_SCD.AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var countTask = query.CountAsync();
                var itemsTask = query
                    .OrderByDescending(r => r.StatementDate)
                    .ThenByDescending(r => r.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                await Task.WhenAll(countTask, itemsTask);

                var result = new PagedResult<RawDataRecord_SCD>
                {
                    Items = await itemsTask,
                    TotalCount = await countTask,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                stopwatch.Stop();
                _logger.LogDebug("Retrieved {Count} SCD records in {ElapsedMs}ms", 
                    result.Items.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error retrieving SCD records in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        // Implement other interface methods...
        public Task<List<RawDataRecord_SCD>> GetByFilterAsync(Expression<Func<RawDataRecord_SCD, bool>> filter, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public Task<RawDataRecord_SCD?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<List<RawDataRecord_SCD>> GetBatchAsync(IEnumerable<object> ids)
        {
            throw new NotImplementedException();
        }

        public Task<RawDataRecord_SCD> CreateAsync(RawDataRecord_SCD entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<RawDataRecord_SCD>> CreateBatchAsync(List<RawDataRecord_SCD> entities)
        {
            throw new NotImplementedException();
        }

        public Task<RawDataRecord_SCD> UpdateAsync(RawDataRecord_SCD entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<RawDataRecord_SCD, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }
    }
}
