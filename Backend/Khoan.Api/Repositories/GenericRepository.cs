using TinhKhoanApp.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Repositories.Interfaces;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Generic Repository Implementation - Concrete implementation of IBaseRepository
    /// Supports all entities that implement IEntity interface
    /// </summary>
    /// <typeparam name="TEntity">Entity type implementing IEntity</typeparam>
    public class GenericRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger<GenericRepository<TEntity>> _logger;

        public GenericRepository(ApplicationDbContext context, ILogger<GenericRepository<TEntity>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = _context.Set<TEntity>();
        }

        // === BASIC CRUD OPERATIONS ===

        public virtual async Task<ApiResponse<IEnumerable<TEntity>>> GetAllAsync()
        {
            try
            {
                var entities = await _dbSet.ToListAsync();
                return ApiResponse<IEnumerable<TEntity>>.Ok(entities,
                    $"Retrieved {entities.Count} {typeof(TEntity).Name} records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.GetAllAsync for {EntityType}", typeof(TEntity).Name);
                return ApiResponse<IEnumerable<TEntity>>.Error($"Database error: {ex.Message}");
            }
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, DateTime? ngayDL = null)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                // Apply date filter if provided
                if (ngayDL.HasValue)
                {
                    // Assuming all entities have NGAY_DL property
                    var propertyInfo = typeof(TEntity).GetProperty("NGAY_DL");
                    if (propertyInfo != null)
                    {
                        query = query.Where(e => EF.Property<DateTime>(e, "NGAY_DL").Date == ngayDL.Value.Date);
                    }
                }

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<TEntity>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.GetPagedAsync for {EntityType}", typeof(TEntity).Name);
                return new PagedResult<TEntity>
                {
                    Items = new List<TEntity>(),
                    TotalCount = 0,
                    PageNumber = page,
                    PageSize = pageSize
                };
            }
        }

        public virtual async Task<TEntity?> GetByIdAsync(long id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.GetByIdAsync for {EntityType}, Id: {Id}", typeof(TEntity).Name, id);
                return null;
            }
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                _dbSet.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.CreateAsync for {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity?> UpdateAsync(long id, TEntity entity)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null)
                {
                    return null;
                }

                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existingEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.UpdateAsync for {EntityType}, Id: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(long id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return false;
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.DeleteAsync for {EntityType}, Id: {Id}", typeof(TEntity).Name, id);
                return false;
            }
        }

        public virtual async Task<List<TEntity>> GetByDateAsync(DateTime ngayDL)
        {
            try
            {
                var propertyInfo = typeof(TEntity).GetProperty("NGAY_DL");
                if (propertyInfo == null)
                {
                    return new List<TEntity>();
                }

                return await _dbSet
                    .Where(e => EF.Property<DateTime>(e, "NGAY_DL").Date == ngayDL.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.GetByDateAsync for {EntityType}", typeof(TEntity).Name);
                return new List<TEntity>();
            }
        }

        // === BATCH OPERATIONS ===

        public virtual async Task<BulkOperationResult> BulkInsertAsync(List<TEntity> entities)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await _dbSet.AddRangeAsync(entities);
                var affectedRows = await _context.SaveChangesAsync();
                stopwatch.Stop();

                return new BulkOperationResult
                {
                    TotalProcessed = entities.Count,
                    SuccessCount = affectedRows,
                    ErrorCount = 0,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error in GenericRepository.BulkInsertAsync for {EntityType}", typeof(TEntity).Name);
                return new BulkOperationResult
                {
                    TotalProcessed = entities.Count,
                    SuccessCount = 0,
                    ErrorCount = entities.Count,
                    Errors = new List<string> { ex.Message },
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        public virtual async Task<BulkOperationResult> BulkUpdateAsync(List<TEntity> entities)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _dbSet.UpdateRange(entities);
                var affectedRows = await _context.SaveChangesAsync();
                stopwatch.Stop();

                return new BulkOperationResult
                {
                    TotalProcessed = entities.Count,
                    SuccessCount = affectedRows,
                    ErrorCount = 0,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error in GenericRepository.BulkUpdateAsync for {EntityType}", typeof(TEntity).Name);
                return new BulkOperationResult
                {
                    TotalProcessed = entities.Count,
                    SuccessCount = 0,
                    ErrorCount = entities.Count,
                    Errors = new List<string> { ex.Message },
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        public virtual async Task<BulkOperationResult> BulkDeleteAsync(List<long> ids)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
                _dbSet.RemoveRange(entities);
                var affectedRows = await _context.SaveChangesAsync();
                stopwatch.Stop();

                return new BulkOperationResult
                {
                    TotalProcessed = ids.Count,
                    SuccessCount = affectedRows,
                    ErrorCount = 0,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error in GenericRepository.BulkDeleteAsync for {EntityType}", typeof(TEntity).Name);
                return new BulkOperationResult
                {
                    TotalProcessed = ids.Count,
                    SuccessCount = 0,
                    ErrorCount = ids.Count,
                    Errors = new List<string> { ex.Message },
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        // === TEMPORAL OPERATIONS ===

        public virtual async Task<List<TEntity>> GetHistoryAsync(long id)
        {
            try
            {
                // Default implementation - may be overridden for temporal tables
                var entity = await GetByIdAsync(id);
                return entity != null ? new List<TEntity> { entity } : new List<TEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.GetHistoryAsync for {EntityType}, Id: {Id}", typeof(TEntity).Name, id);
                return new List<TEntity>();
            }
        }

        public virtual async Task<TEntity?> GetAsOfDateAsync(long id, DateTime asOfDate)
        {
            try
            {
                // Default implementation - returns current entity
                return await GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.GetAsOfDateAsync for {EntityType}, Id: {Id}", typeof(TEntity).Name, id);
                return null;
            }
        }

        // === SYSTEM OPERATIONS ===

        public virtual async Task<long> CountAsync(DateTime? ngayDL = null)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (ngayDL.HasValue)
                {
                    var propertyInfo = typeof(TEntity).GetProperty("NGAY_DL");
                    if (propertyInfo != null)
                    {
                        query = query.Where(e => EF.Property<DateTime>(e, "NGAY_DL").Date == ngayDL.Value.Date);
                    }
                }

                return await query.LongCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.CountAsync for {EntityType}", typeof(TEntity).Name);
                return 0;
            }
        }

        public virtual async Task<bool> ExistsAsync(long id)
        {
            try
            {
                return await _dbSet.AnyAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenericRepository.ExistsAsync for {EntityType}, Id: {Id}", typeof(TEntity).Name, id);
                return false;
            }
        }

        public virtual async Task<bool> IsHealthyAsync()
        {
            try
            {
                // Basic health check - try to connect and count records
                var count = await _dbSet.Take(1).CountAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for {EntityType}", typeof(TEntity).Name);
                return false;
            }
        }
    }
}
