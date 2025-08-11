using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository implementation for RR01 (Risk Report) data
    /// Direct implementation without BaseRepository since DataTables.RR01 doesn't implement IEntity
    /// </summary>
    public class RR01Repository : IRR01Repository
    {
        private readonly ApplicationDbContext _context;

        public RR01Repository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Implementation của IBaseRepository<RR01>

        public async Task<ApiResponse<IEnumerable<RR01>>> GetAllAsync()
        {
            try
            {
                var entities = await _context.RR01
                    .OrderByDescending(x => x.NGAY_DL)
                    .ThenBy(x => x.BRCD)
                    .ToListAsync();

                return ApiResponse<IEnumerable<RR01>>.Ok(entities,
                    $"Retrieved {entities.Count} RR01 records");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RR01>>.Error($"Error retrieving RR01 records: {ex.Message}");
            }
        }

        public async Task<RR01?> GetByIdAsync(long id)
        {
            return await _context.RR01.FindAsync((int)id);
        }

        public async Task<RR01> CreateAsync(RR01 entity)
        {
            _context.RR01.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<RR01?> UpdateAsync(long id, RR01 entity)
        {
            var existing = await _context.RR01.FindAsync((int)id);
            if (existing == null) return null;

            // Update properties
            _context.Entry(existing).CurrentValues.SetValues(entity);
            existing.UPDATED_DATE = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.RR01.FindAsync((int)id);
            if (entity == null) return false;

            _context.RR01.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.RR01.AnyAsync(x => x.Id == (int)id);
        }

        // Implementation của IRR01Repository specific methods

        /// <summary>
        /// Get paged RR01 records with optional date filtering
        /// </summary>
        public async Task<PagedResult<RR01>> GetPagedAsync(int pageNumber, int pageSize, DateTime? fromDate = null)
        {
            var query = _context.RR01.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.NGAY_DL >= fromDate.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RR01>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get RR01 entity by Id
        /// </summary>
        public async Task<RR01?> GetEntityByIdAsync(long id)
        {
            return await _context.RR01.FindAsync((int)id);
        }

        /// <summary>
        /// Get RR01 records by specific date
        /// </summary>
        public async Task<List<RR01>> GetByDateAsync(DateTime date)
        {
            return await _context.RR01
                .Where(x => x.NGAY_DL.Date == date.Date)
                .OrderBy(x => x.BRCD)
                .ThenBy(x => x.MA_KH)
                .ToListAsync();
        }

        /// <summary>
        /// Bulk insert RR01 records
        /// </summary>
        public async Task<BulkOperationResult> BulkInsertAsync(List<RR01> entities)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _context.RR01.AddRange(entities);
                var recordsAffected = await _context.SaveChangesAsync();
                stopwatch.Stop();

                return new BulkOperationResult
                {
                    TotalProcessed = entities.Count,
                    SuccessCount = recordsAffected,
                    ErrorCount = 0,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
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

        /// <summary>
        /// Bulk update RR01 records
        /// </summary>
        public async Task<BulkOperationResult> BulkUpdateAsync(List<RR01> entities)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _context.RR01.UpdateRange(entities);
                var recordsAffected = await _context.SaveChangesAsync();
                stopwatch.Stop();

                return new BulkOperationResult
                {
                    TotalProcessed = entities.Count,
                    SuccessCount = recordsAffected,
                    ErrorCount = 0,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
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

        /// <summary>
        /// Bulk delete RR01 records by IDs
        /// </summary>
        public async Task<BulkOperationResult> BulkDeleteAsync(List<long> ids)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var intIds = ids.Select(x => (int)x).ToList();
                var entitiesToDelete = await _context.RR01
                    .Where(x => intIds.Contains(x.Id))
                    .ToListAsync();

                _context.RR01.RemoveRange(entitiesToDelete);
                var recordsAffected = await _context.SaveChangesAsync();
                stopwatch.Stop();

                return new BulkOperationResult
                {
                    TotalProcessed = ids.Count,
                    SuccessCount = recordsAffected,
                    ErrorCount = 0,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
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

        /// <summary>
        /// Get historical versions of an RR01 record (temporal table support)
        /// </summary>
        public async Task<List<RR01>> GetHistoryAsync(long id)
        {
            // Note: Since RR01 is temporal table, this would require FOR SYSTEM_TIME ALL query
            // For now, returning current record only
            var current = await GetEntityByIdAsync(id);
            return current != null ? new List<RR01> { current } : new List<RR01>();
        }

        /// <summary>
        /// Get RR01 record as of specific date (temporal table support)
        /// </summary>
        public async Task<RR01?> GetAsOfDateAsync(long id, DateTime asOfDate)
        {
            // Note: Since RR01 is temporal table, this would require FOR SYSTEM_TIME AS OF query
            // For now, returning current record if it exists and creation date <= asOfDate
            var current = await GetEntityByIdAsync(id);
            return current?.CREATED_DATE <= asOfDate ? current : null;
        }

        /// <summary>
        /// Count RR01 records with optional date filtering
        /// </summary>
        public async Task<long> CountAsync(DateTime? fromDate = null)
        {
            var query = _context.RR01.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.NGAY_DL >= fromDate.Value);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Check repository health
        /// </summary>
        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                // Simple health check - count records
                await _context.RR01.CountAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
