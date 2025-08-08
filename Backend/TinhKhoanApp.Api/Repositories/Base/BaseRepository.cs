using TinhKhoanApp.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Repositories.Base
{
    /// <summary>
    /// Base Repository - Generic implementation cho common CRUD operations
    /// Provides standardized data access patterns cho tất cả entities
    /// </summary>
    /// <typeparam name="TEntity">Entity type implementing IEntity</typeparam>
    public abstract class BaseRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger<BaseRepository<TEntity>> _logger;

        protected BaseRepository(ApplicationDbContext context, ILogger<BaseRepository<TEntity>> logger)
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
                _logger.LogInformation("Getting all {EntityType} records", typeof(TEntity).Name);

                var entities = await _dbSet
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();

                return ApiResponse<IEnumerable<TEntity>>.Success(
                    entities,
                    $"Retrieved {entities.Count} {typeof(TEntity).Name} records"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all {EntityType} records", typeof(TEntity).Name);
                return ApiResponse<IEnumerable<TEntity>>.Error(
                    $"Failed to retrieve {typeof(TEntity).Name} records: {ex.Message}"
                );
            }
        }

        public virtual async Task<ApiResponse<TEntity?>> GetByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);

                var entity = await _dbSet.FindAsync(id);

                if (entity == null)
                {
                    return ApiResponse<TEntity?>.Error(
                        $"{typeof(TEntity).Name} with ID {id} not found",
                        404
                    );
                }

                return ApiResponse<TEntity?>.Success(
                    entity,
                    $"{typeof(TEntity).Name} found"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);
                return ApiResponse<TEntity?>.Error(
                    $"Failed to get {typeof(TEntity).Name}: {ex.Message}"
                );
            }
        }

        public virtual async Task<ApiResponse<TEntity>> CreateAsync(TEntity entity)
        {
            try
            {
                _logger.LogInformation("Creating new {EntityType} record", typeof(TEntity).Name);

                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created {EntityType} with ID: {Id}", typeof(TEntity).Name, entity.Id);

                return ApiResponse<TEntity>.Success(
                    entity,
                    $"{typeof(TEntity).Name} created successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {EntityType}", typeof(TEntity).Name);
                return ApiResponse<TEntity>.Error(
                    $"Failed to create {typeof(TEntity).Name}: {ex.Message}"
                );
            }
        }

        public virtual async Task<ApiResponse<TEntity>> UpdateAsync(long id, TEntity entity)
        {
            try
            {
                _logger.LogInformation("Updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null)
                {
                    return ApiResponse<TEntity>.Error(
                        $"{typeof(TEntity).Name} with ID {id} not found",
                        404
                    );
                }

                entity.Id = id;
                entity.CreatedAt = existingEntity.CreatedAt; // Preserve creation time
                entity.UpdatedAt = DateTime.UtcNow;

                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                return ApiResponse<TEntity>.Success(
                    entity,
                    $"{typeof(TEntity).Name} updated successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                return ApiResponse<TEntity>.Error(
                    $"Failed to update {typeof(TEntity).Name}: {ex.Message}"
                );
            }
        }

        public virtual async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            try
            {
                _logger.LogInformation("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return ApiResponse<bool>.Error(
                        $"{typeof(TEntity).Name} with ID {id} not found",
                        404
                    );
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                return ApiResponse<bool>.Success(
                    true,
                    $"{typeof(TEntity).Name} deleted successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                return ApiResponse<bool>.Error(
                    $"Failed to delete {typeof(TEntity).Name}: {ex.Message}"
                );
            }
        }

        // === PAGINATION & SEARCH ===

        public virtual async Task<ApiResponse<PagedResult>TEntity>>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Getting paged {EntityType} - Page: {Page}, Size: {Size}",
                    typeof(TEntity).Name, pageNumber, pageSize);

                var totalCount = await _dbSet.CountAsync();
                var items = await _dbSet
                    .OrderByDescending(e => e.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResult = new PagedResult<TEntity>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<PagedResult>TEntity>>>.Success(
                    pagedResult,
                    $"Retrieved page {pageNumber} of {typeof(TEntity).Name} records"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged {EntityType}", typeof(TEntity).Name);
                return ApiResponse<PagedResult>TEntity>>>.Error(
                    $"Failed to get paged {typeof(TEntity).Name}: {ex.Message}"
                );
            }
        }

        // === BULK OPERATIONS ===

        public virtual async Task<ApiResponse<bool>> BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                var entityList = entities.ToList();
                _logger.LogInformation("Bulk inserting {Count} {EntityType} records",
                    entityList.Count, typeof(TEntity).Name);

                var now = DateTime.UtcNow;
                foreach (var entity in entityList)
                {
                    entity.CreatedAt = now;
                    entity.UpdatedAt = now;
                }

                await _dbSet.AddRangeAsync(entityList);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully bulk inserted {Count} {EntityType} records",
                    entityList.Count, typeof(TEntity).Name);

                return ApiResponse<bool>.Success(
                    true,
                    $"Bulk inserted {entityList.Count} {typeof(TEntity).Name} records"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk inserting {EntityType} records", typeof(TEntity).Name);
                return ApiResponse<bool>.Error(
                    $"Failed to bulk insert {typeof(TEntity).Name}: {ex.Message}"
                );
            }
        }

        // === UTILITY METHODS ===

        public virtual async Task<ApiResponse<bool>> ExistsAsync(long id)
        {
            try
            {
                var exists = await _dbSet.AnyAsync(e => e.Id == id);
                return ApiResponse<bool>.Success(exists, $"{typeof(TEntity).Name} existence checked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking {EntityType} existence for ID: {Id}", typeof(TEntity).Name, id);
                return ApiResponse<bool>.Error($"Failed to check existence: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<long>> CountAsync()
        {
            try
            {
                var count = await _dbSet.CountAsync();
                return ApiResponse<long>.Success(count, $"{typeof(TEntity).Name} count retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting {EntityType} records", typeof(TEntity).Name);
                return ApiResponse<long>.Error($"Failed to count records: {ex.Message}");
            }
        }
    }
}
