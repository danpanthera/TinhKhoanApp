using TinhKhoanApp.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DTOs.LN03;
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
                var entities = await GetAllEntitiesAsync();
                if (!entities.IsSuccess || entities.Data == null)
                {
                    return ApiResponse<IEnumerable<LN03PreviewDto>>.Error(entities.Message);
                }

                var dtos = entities.Data.Select(ConvertToPreviewDto).ToList();
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Success(dtos,
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
                var entityResponse = await GetByIdEntityAsync(id);
                if (!entityResponse.IsSuccess || entityResponse.Data == null)
                {
                    return ApiResponse<LN03DetailsDto?>.Error(entityResponse.Message);
                }

                var dto = ConvertToDetailsDto(entityResponse.Data);
                return ApiResponse<LN03DetailsDto?>.Success(dto, "LN03 record retrieved successfully");
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
                var entity = ConvertToEntity(createDto);
                var createdResponse = await CreateEntityAsync(entity);

                if (!createdResponse.IsSuccess || createdResponse.Data == null)
                {
                    return ApiResponse<LN03DetailsDto>.Error(createdResponse.Message);
                }

                var dto = ConvertToDetailsDto(createdResponse.Data);
                return ApiResponse<LN03DetailsDto>.Success(dto, "LN03 record created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.CreateAsync");
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
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Success(dtos,
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
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Success(dtos,
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
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Success(dtos,
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

                return ApiResponse<decimal>.Success(total,
                    $"Total LN03 processing amount: {total:C}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Repository.GetTotalProcessingAmountAsync");
                return ApiResponse<decimal>.Error($"Repository error: {ex.Message}");
            }
        }

        // === DTO CONVERSION METHODS ===

        private LN03PreviewDto ConvertToPreviewDto(LN03Entity entity)
        {
            return new LN03PreviewDto
            {
                Id = entity.Id,
                MACHINHANH = entity.MACHINHANH ?? string.Empty,
                MAKH = entity.MAKH ?? string.Empty,
                SOHOPDONG = entity.SOHOPDONG ?? string.Empty,
                SOTIENXLRR = entity.SOTIENXLRR ?? 0,
                TRANGTHAI = entity.TRANGTHAI ?? string.Empty,
                NGAYXULY = entity.NGAYXULY,
                LOAIVAY = entity.LOAIVAY ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt
            };
        }

        private LN03DetailsDto ConvertToDetailsDto(LN03Entity entity)
        {
            return new LN03DetailsDto
            {
                Id = entity.Id,
                MACHINHANH = entity.MACHINHANH ?? string.Empty,
                MAKH = entity.MAKH ?? string.Empty,
                SOHOPDONG = entity.SOHOPDONG ?? string.Empty,
                SOTIENXLRR = entity.SOTIENXLRR ?? 0,
                TRANGTHAI = entity.TRANGTHAI ?? string.Empty,
                NGAYXULY = entity.NGAYXULY,
                NGAYHETHAN = entity.NGAYHETHAN,
                LOAIVAY = entity.LOAIVAY ?? string.Empty,
                LAIXUAT = entity.LAIXUAT ?? 0,
                KYHANGUI = entity.KYHANGUI ?? string.Empty,
                MANHANVIEN = entity.MANHANVIEN ?? string.Empty,
                SOTIENGOC = entity.SOTIENGOC ?? 0,
                SOTIENLAI = entity.SOTIENLAI ?? 0,
                GHICHU = entity.GHICHU ?? string.Empty,
                NGAYCAPNHAT = entity.NGAYCAPNHAT,
                MADONVI = entity.MADONVI ?? string.Empty,
                // Headerless columns
                Column18 = entity.Column18 ?? string.Empty,
                Column19 = entity.Column19 ?? string.Empty,
                Column20 = entity.Column20 ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt,
                FileName = entity.FileName,
                ImportId = entity.ImportId,
                ImportMetadata = entity.ImportMetadata
            };
        }

        private LN03Entity ConvertToEntity(LN03CreateDto createDto)
        {
            return new LN03Entity
            {
                MACHINHANH = createDto.MACHINHANH,
                MAKH = createDto.MAKH,
                SOHOPDONG = createDto.SOHOPDONG,
                SOTIENXLRR = createDto.SOTIENXLRR,
                TRANGTHAI = createDto.TRANGTHAI,
                NGAYXULY = createDto.NGAYXULY,
                NGAYHETHAN = createDto.NGAYHETHAN,
                LOAIVAY = createDto.LOAIVAY,
                LAIXUAT = createDto.LAIXUAT,
                KYHANGUI = createDto.KYHANGUI,
                MANHANVIEN = createDto.MANHANVIEN,
                SOTIENGOC = createDto.SOTIENGOC,
                SOTIENLAI = createDto.SOTIENLAI,
                GHICHU = createDto.GHICHU,
                NGAYCAPNHAT = createDto.NGAYCAPNHAT,
                MADONVI = createDto.MADONVI,
                Column18 = createDto.Column18,
                Column19 = createDto.Column19,
                Column20 = createDto.Column20,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
