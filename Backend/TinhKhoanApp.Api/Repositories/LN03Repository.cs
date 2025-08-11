using TinhKhoanApp.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Repositories.Base;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// LN03 Repository Implementation - Phase 2B
    /// Data access layer for Loan Processing table (20 business columns)
    /// </summary>
    public class LN03Repository : BaseRepository<LN03Entity>, ILN03Repository
    {
        public LN03Repository(ApplicationDbContext context, ILogger<LN03Repository> logger)
            : base(context, logger)
        {
        }

        // === BASIC CRUD WITH DTO CONVERSION ===

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetAllAsync()
        {
            try
            {
                var entities = await _dbSet.ToListAsync();
                var dtos = entities.Select(ConvertToPreviewDto).ToList();
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(dtos,
                    $"Retrieved {dtos.Count} LN03 records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetAllAsync");
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Repository error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto?>> GetByIdAsync(long id)
        {
            try
            {
                var entity = await base.GetByIdAsync(id);
                if (!entity.Success || entity.Data == null)
                {
                    return ApiResponse<LN03DetailsDto?>.Error(entity.Message ?? "Entity not found", 404);
                }

                var dto = ConvertToDetailsDto(entity.Data);
                return ApiResponse<LN03DetailsDto?>.Ok(dto, "LN03 record retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetByIdAsync for ID: {Id}", id);
                return ApiResponse<LN03DetailsDto?>.Error($"Repository error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> CreateAsync(LN03CreateDto createDto)
        {
            try
            {
                var entity = ConvertFromCreateDto(createDto);
                var createdEntity = await base.CreateAsync(entity);

                if (!createdEntity.Success || createdEntity.Data == null)
                {
                    return ApiResponse<LN03DetailsDto>.Error(createdEntity.Message ?? "Failed to create");
                }

                var dto = ConvertToDetailsDto(createdEntity.Data);
                return ApiResponse<LN03DetailsDto>.Ok(dto, "LN03 record created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.CreateAsync: {Error}", ex.Message);
                return ApiResponse<LN03DetailsDto>.Error($"Repository error: {ex.Message}");
            }
        }

        // === BUSINESS QUERY METHODS ===

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByBranchAsync(string maCN)
        {
            try
            {
                var entities = await _context.Set<LN03Entity>()
                    .Where(x => x.MACHINHANH == maCN)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var dtos = entities.Select(ConvertToPreviewDto).ToList();
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(dtos,
                    $"Retrieved {dtos.Count} LN03 records for branch {maCN}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetByBranchAsync for branch: {Branch}", maCN);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Repository error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByCustomerAsync(string maKH)
        {
            try
            {
                var entities = await _context.Set<LN03Entity>()
                    .Where(x => x.MAKH == maKH)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var dtos = entities.Select(ConvertToPreviewDto).ToList();
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(dtos,
                    $"Retrieved {dtos.Count} LN03 records for customer {maKH}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetByCustomerAsync for customer: {Customer}", maKH);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Repository error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByContractAsync(string soHopDong)
        {
            try
            {
                var entities = await _context.Set<LN03Entity>()
                    .Where(x => x.SOHOPDONG == soHopDong)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var dtos = entities.Select(ConvertToPreviewDto).ToList();
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(dtos,
                    $"Retrieved {dtos.Count} LN03 records for contract {soHopDong}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetByContractAsync for contract: {Contract}", soHopDong);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Repository error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalProcessingAmountAsync()
        {
            try
            {
                var total = await _context.Set<LN03Entity>()
                    .SumAsync(x => x.SOTIENXLRR ?? 0);

                return ApiResponse<decimal>.Ok(total,
                    $"Total LN03 processing amount: {total:C}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetTotalProcessingAmountAsync");
                return ApiResponse<decimal>.Error($"Repository error: {ex.Message}");
            }
        }

        // === DTO CONVERSION METHODS ===

        private LN03Entity ConvertFromCreateDto(LN03CreateDto createDto)
        {
            return new LN03Entity
            {
                NGAY_DL = createDto.NGAY_DL,
                MACHINHANH = createDto.MACHINHANH,
                TENCHINHANH = createDto.TENCHINHANH,
                MAKH = createDto.MAKH,
                TENKH = createDto.TENKH,
                SOHOPDONG = createDto.SOHOPDONG,
                SOTIENXLRR = createDto.SOTIENXLRR,
                NGAYPHATSINHXL = createDto.NGAYPHATSINHXL,
                THUNOSAUXL = createDto.THUNOSAUXL,
                CONLAINGOAIBANG = createDto.CONLAINGOAIBANG,
                DUNONOIBANG = createDto.DUNONOIBANG,
                NHOMNO = createDto.NHOMNO,
                MACBTD = createDto.MACBTD,
                TENCBTD = createDto.TENCBTD,
                MAPGD = createDto.MAPGD,
                TAIKHOANHACHTOAN = createDto.TAIKHOANHACHTOAN,
                REFNO = createDto.REFNO,
                LOAINGUONVON = createDto.LOAINGUONVON,
                Column18 = createDto.Column18,
                Column19 = createDto.Column19,
                Column20 = createDto.Column20,
                FileName = createDto.FileName,
                CreatedAt = DateTime.UtcNow
            };
        }

        private LN03PreviewDto ConvertToPreviewDto(LN03Entity entity)
        {
            return new LN03PreviewDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                MACHINHANH = entity.MACHINHANH ?? string.Empty,
                TENCHINHANH = entity.TENCHINHANH ?? string.Empty,
                MAKH = entity.MAKH ?? string.Empty,
                TENKH = entity.TENKH ?? string.Empty,
                SOHOPDONG = entity.SOHOPDONG ?? string.Empty,
                SOTIENXLRR = entity.SOTIENXLRR ?? 0,
                NGAYPHATSINHXL = entity.NGAYPHATSINHXL,
                THUNOSAUXL = entity.THUNOSAUXL ?? 0,
                CONLAINGOAIBANG = entity.CONLAINGOAIBANG ?? 0,
                DUNONOIBANG = entity.DUNONOIBANG ?? 0,
                NHOMNO = entity.NHOMNO ?? string.Empty,
                MACBTD = entity.MACBTD ?? string.Empty,
                TENCBTD = entity.TENCBTD ?? string.Empty,
                MAPGD = entity.MAPGD ?? string.Empty,
                TAIKHOANHACHTOAN = entity.TAIKHOANHACHTOAN ?? string.Empty,
                REFNO = entity.REFNO ?? string.Empty,
                LOAINGUONVON = entity.LOAINGUONVON ?? string.Empty,
                Column18 = entity.Column18 ?? string.Empty,
                Column19 = entity.Column19 ?? string.Empty,
                Column20 = entity.Column20 ?? 0,
                CreatedDate = entity.CreatedAt
            };
        }

        private LN03DetailsDto ConvertToDetailsDto(LN03Entity entity)
        {
            var preview = ConvertToPreviewDto(entity);

            return new LN03DetailsDto
            {
                // Copy all from preview
                Id = preview.Id,
                NGAY_DL = preview.NGAY_DL,
                MACHINHANH = preview.MACHINHANH,
                TENCHINHANH = preview.TENCHINHANH,
                MAKH = preview.MAKH,
                TENKH = preview.TENKH,
                SOHOPDONG = preview.SOHOPDONG,
                SOTIENXLRR = preview.SOTIENXLRR,
                NGAYPHATSINHXL = preview.NGAYPHATSINHXL,
                THUNOSAUXL = preview.THUNOSAUXL,
                CONLAINGOAIBANG = preview.CONLAINGOAIBANG,
                DUNONOIBANG = preview.DUNONOIBANG,
                NHOMNO = preview.NHOMNO,
                MACBTD = preview.MACBTD,
                TENCBTD = preview.TENCBTD,
                MAPGD = preview.MAPGD,
                TAIKHOANHACHTOAN = preview.TAIKHOANHACHTOAN,
                REFNO = preview.REFNO,
                LOAINGUONVON = preview.LOAINGUONVON,
                Column18 = preview.Column18,
                Column19 = preview.Column19,
                Column20 = preview.Column20,
                CreatedDate = preview.CreatedDate,

                // Add details-specific fields
                ValidFromDate = entity.SysStartTime,
                ValidToDate = entity.SysEndTime,
                ImportBatch = entity.ImportId?.ToString(),
                DataSource = entity.FileName,
                RecordVersion = 1, // Default version
                IsActive = true,
                LastModifiedBy = "System"
            };
        }

        // === ILN03REPOSITORY INTERFACE IMPLEMENTATION ===

        public async Task<ApiResponse<IEnumerable<LN03Entity>>> GetByProductCodeAsync(string productCode, DateTime? ngayDL = null)
        {
            try
            {
                var query = _context.Set<LN03Entity>().AsQueryable();
                if (ngayDL.HasValue)
                    query = query.Where(x => x.NGAY_DL.Date == ngayDL.Value.Date);

                var results = await query.Where(x => x.TENKH == productCode).ToListAsync();
                return ApiResponse<IEnumerable<LN03Entity>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 by product code {ProductCode}", productCode);
                return ApiResponse<IEnumerable<LN03Entity>>.Failure($"Error getting data by product code: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03Entity>>> GetByDonViAsync(string donVi, DateTime? ngayDL = null)
        {
            try
            {
                var query = _context.Set<LN03Entity>().AsQueryable();
                if (ngayDL.HasValue)
                    query = query.Where(x => x.NGAY_DL.Date == ngayDL.Value.Date);

                var results = await query.Where(x => x.MACHINHANH == donVi).ToListAsync();
                return ApiResponse<IEnumerable<LN03Entity>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 by don vi {DonVi}", donVi);
                return ApiResponse<IEnumerable<LN03Entity>>.Failure($"Error getting data by don vi: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03Entity>>> GetByLoanTypeAsync(string loanType, DateTime ngayDL)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.LOAINGUONVON == loanType && x.NGAY_DL.Date == ngayDL.Date)
                    .ToListAsync();
                return ApiResponse<IEnumerable<LN03Entity>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 by loan type {LoanType}", loanType);
                return ApiResponse<IEnumerable<LN03Entity>>.Failure($"Error getting data by loan type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> CalculateTotalLoanAmountAsync(DateTime ngayDL)
        {
            try
            {
                var total = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .SumAsync(x => x.SOTIENXLRR ?? 0);
                return ApiResponse<decimal>.Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total loan amount for date {NgayDL}", ngayDL);
                return ApiResponse<decimal>.Failure($"Error calculating total loan amount: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountByProductAsync(DateTime ngayDL)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .GroupBy(x => x.REFNO)
                    .Select(g => new { Product = g.Key, Amount = g.Sum(x => x.SOTIENXLRR ?? 0) })
                    .ToDictionaryAsync(x => x.Product ?? "", x => x.Amount);
                return ApiResponse<Dictionary<string, decimal>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan amount by product for date {NgayDL}", ngayDL);
                return ApiResponse<Dictionary<string, decimal>>.Failure($"Error getting loan amount by product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountByDonViAsync(DateTime ngayDL)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .GroupBy(x => x.MACHINHANH)
                    .Select(g => new { DonVi = g.Key, Amount = g.Sum(x => x.SOTIENXLRR ?? 0) })
                    .ToDictionaryAsync(x => x.DonVi ?? "", x => x.Amount);
                return ApiResponse<Dictionary<string, decimal>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan amount by don vi for date {NgayDL}", ngayDL);
                return ApiResponse<Dictionary<string, decimal>>.Failure($"Error getting loan amount by don vi: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Dictionary<string, int>>> GetLoanCountByProductAsync(DateTime ngayDL)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .GroupBy(x => x.REFNO)
                    .Select(g => new { Product = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Product ?? "", x => x.Count);
                return ApiResponse<Dictionary<string, int>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan count by product for date {NgayDL}", ngayDL);
                return ApiResponse<Dictionary<string, int>>.Failure($"Error getting loan count by product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03Entity>>> GetProductSummaryAsync(DateTime ngayDL)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .GroupBy(x => x.REFNO)
                    .Select(g => g.First())
                    .ToListAsync();
                return ApiResponse<IEnumerable<LN03Entity>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product summary for date {NgayDL}", ngayDL);
                return ApiResponse<IEnumerable<LN03Entity>>.Failure($"Error getting product summary: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> GetAverageLoanAmountAsync(DateTime ngayDL)
        {
            try
            {
                var average = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date && x.SOTIENXLRR.HasValue)
                    .AverageAsync(x => x.SOTIENXLRR.Value);
                return ApiResponse<decimal>.Ok(average);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average loan amount for date {NgayDL}", ngayDL);
                return ApiResponse<decimal>.Failure($"Error calculating average loan amount: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Dictionary<string, decimal>>> GetPortfolioDistributionAsync(DateTime ngayDL)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .GroupBy(x => x.NHOMNO)
                    .Select(g => new { Group = g.Key, Amount = g.Sum(x => x.SOTIENXLRR ?? 0) })
                    .ToDictionaryAsync(x => x.Group ?? "", x => x.Amount);
                return ApiResponse<Dictionary<string, decimal>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio distribution for date {NgayDL}", ngayDL);
                return ApiResponse<Dictionary<string, decimal>>.Failure($"Error getting portfolio distribution: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03Entity>>> GetLargeLoansAsync(DateTime ngayDL, decimal threshold)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date && x.SOTIENXLRR >= threshold)
                    .ToListAsync();
                return ApiResponse<IEnumerable<LN03Entity>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting large loans for date {NgayDL}, threshold {Threshold}", ngayDL, threshold);
                return ApiResponse<IEnumerable<LN03Entity>>.Failure($"Error getting large loans: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03Entity>>> GetSmallLoansAsync(DateTime ngayDL, decimal threshold)
        {
            try
            {
                var results = await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date && x.SOTIENXLRR < threshold)
                    .ToListAsync();
                return ApiResponse<IEnumerable<LN03Entity>>.Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting small loans for date {NgayDL}, threshold {Threshold}", ngayDL, threshold);
                return ApiResponse<IEnumerable<LN03Entity>>.Failure($"Error getting small loans: {ex.Message}");
            }
        }

        // === IBASEREPOSITORY INTERFACE IMPLEMENTATION ===

        public new async Task<PagedResult<LN03Entity>> GetPagedAsync(int page, int pageSize, DateTime? ngayDL = null)
        {
            try
            {
                var query = _context.Set<LN03Entity>().AsQueryable();
                if (ngayDL.HasValue)
                    query = query.Where(x => x.NGAY_DL.Date == ngayDL.Value.Date);

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<LN03Entity>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged LN03 data");
                return new PagedResult<LN03Entity> { Items = new List<LN03Entity>(), TotalCount = 0, PageNumber = page, PageSize = pageSize };
            }
        }

        public new async Task<LN03Entity?> GetEntityByIdAsync(long id)
        {
            try
            {
                return await _context.Set<LN03Entity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03Entity by id {Id}", id);
                return null;
            }
        }

        // Implementation for IBaseRepository<LN03Entity>
        async Task<LN03Entity?> IBaseRepository<LN03Entity>.GetByIdAsync(long id)
        {
            return await GetEntityByIdAsync(id);
        }

        public new async Task<LN03Entity> CreateAsync(LN03Entity entity)
        {
            try
            {
                _context.Set<LN03Entity>().Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating LN03Entity");
                throw;
            }
        }

        public new async Task<LN03Entity?> UpdateAsync(long id, LN03Entity entity)
        {
            try
            {
                var existing = await _context.Set<LN03Entity>().FindAsync(id);
                if (existing == null) return null;

                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LN03Entity with id {Id}", id);
                throw;
            }
        }

        public new async Task<bool> DeleteAsync(long id)
        {
            try
            {
                var entity = await _context.Set<LN03Entity>().FindAsync(id);
                if (entity == null) return false;

                _context.Set<LN03Entity>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LN03Entity with id {Id}", id);
                return false;
            }
        }

        public new async Task<List<LN03Entity>> GetByDateAsync(DateTime ngayDL)
        {
            try
            {
                return await _context.Set<LN03Entity>()
                    .Where(x => x.NGAY_DL.Date == ngayDL.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03Entity by date {NgayDL}", ngayDL);
                return new List<LN03Entity>();
            }
        }

        public new async Task<BulkOperationResult> BulkInsertAsync(List<LN03Entity> entities)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await _context.Set<LN03Entity>().AddRangeAsync(entities);
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
                _logger.LogError(ex, "Error bulk inserting LN03Entity records");
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

        public new async Task<BulkOperationResult> BulkUpdateAsync(List<LN03Entity> entities)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _context.Set<LN03Entity>().UpdateRange(entities);
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
                _logger.LogError(ex, "Error bulk updating LN03Entity records");
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

        public new async Task<BulkOperationResult> BulkDeleteAsync(List<long> ids)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var entities = await _context.Set<LN03Entity>().Where(x => ids.Contains(x.Id)).ToListAsync();
                _context.Set<LN03Entity>().RemoveRange(entities);
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
                _logger.LogError(ex, "Error bulk deleting LN03Entity records");
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

        public new async Task<List<LN03Entity>> GetHistoryAsync(long id)
        {
            try
            {
                // For now, return single current record - implement temporal table support later
                var entity = await _context.Set<LN03Entity>().FindAsync(id);
                return entity != null ? new List<LN03Entity> { entity } : new List<LN03Entity>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03Entity history for id {Id}", id);
                return new List<LN03Entity>();
            }
        }

        public new async Task<LN03Entity?> GetAsOfDateAsync(long id, DateTime asOfDate)
        {
            try
            {
                // For now, return current record if it matches the date - implement temporal table support later
                return await _context.Set<LN03Entity>()
                    .Where(x => x.Id == id && x.NGAY_DL.Date <= asOfDate.Date)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03Entity as of date {AsOfDate} for id {Id}", asOfDate, id);
                return null;
            }
        }

        public new async Task<long> CountAsync(DateTime? ngayDL = null)
        {
            try
            {
                var query = _context.Set<LN03Entity>().AsQueryable();
                if (ngayDL.HasValue)
                    query = query.Where(x => x.NGAY_DL.Date == ngayDL.Value.Date);

                return await query.LongCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting LN03Entity records");
                return 0;
            }
        }

        public new async Task<bool> ExistsAsync(long id)
        {
            try
            {
                return await _context.Set<LN03Entity>().AnyAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if LN03Entity exists with id {Id}", id);
                return false;
            }
        }

        public new async Task<bool> IsHealthyAsync()
        {
            try
            {
                var count = await _context.Set<LN03Entity>().Take(1).CountAsync();
                return count >= 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking LN03Repository health");
                return false;
            }
        }
    }
}
