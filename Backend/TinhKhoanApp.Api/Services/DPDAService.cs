using TinhKhoanApp.Api.Models.Dtos.DPDA;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// DPDA Service Implementation - Business logic cho thẻ nộp đơn gửi tiết kiệm
    /// Theo pattern DP01Service với 13 business columns
    /// CSV-First: Business columns từ CSV là chuẩn cho tất cả layers
    /// </summary>
    public class DPDAService : IDPDAService
    {
        private readonly IDPDARepository _dpdaRepository;
        private readonly ILogger<DPDAService> _logger;

        public DPDAService(
            IDPDARepository dpdaRepository,
            ILogger<DPDAService> logger)
        {
            _dpdaRepository = dpdaRepository;
            _logger = logger;
        }

        #region Controller Support Methods

        /// <summary>
        /// Get DPDA preview with paging and search (Controller compatible)
        /// Mapping Entity -> PreviewDto cho list view
        /// </summary>
        public async Task<ApiResponse<PagedResult<DPDAPreviewDto>>> GetPreviewAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                var (entities, totalCount) = await _dpdaRepository.GetPagedAsync(pageNumber, pageSize, searchTerm);

                var previewDtos = entities.Select(MapToPreviewDto).ToList();

                var pagedResult = new PagedResult<DPDAPreviewDto>
                {
                    Items = previewDtos,
                    TotalCount = (int)totalCount, // Cast long to int
                    PageNumber = pageNumber,
                    PageSize = pageSize
                    // TotalPages is calculated property - remove assignment
                };

                return ApiResponse<PagedResult<DPDAPreviewDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA preview with search term: {SearchTerm}", searchTerm);
                return ApiResponse<PagedResult<DPDAPreviewDto>>.Error($"Lỗi khi lấy danh sách DPDA: {ex.Message}");
            }
        }

        /// <summary>
        /// Import CSV via IFormFile (Controller compatible)
        /// TODO: Implement DirectImport functionality later
        /// </summary>
        public async Task<ApiResponse<DPDAImportResultDto>> ImportFromCsvAsync(IFormFile file, string uploadedBy)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ApiResponse<DPDAImportResultDto>.Error("File không được để trống");

                // TODO: Implement DirectImport service integration
                await Task.CompletedTask; // Placeholder

                var resultDto = new DPDAImportResultDto
                {
                    Success = true,
                    TotalRows = 0,
                    SuccessfulRows = 0,
                    ErrorRows = 0,
                    DuplicateRows = 0, // TODO: Implement DirectImport
                    ProcessingTimeMs = 0, // TODO: Implement DirectImport
                    ImportId = Guid.NewGuid(), // TODO: Implement DirectImport
                    FileName = file.FileName,
                    ImportedBy = uploadedBy,
                    ImportedAt = DateTime.UtcNow,
                    Errors = new List<string>(), // TODO: Implement DirectImport
                    Warnings = new List<string>(), // TODO: Implement DirectImport
                    RowErrors = new Dictionary<int, string>() // TODO: Implement DirectImport
                };

                return ApiResponse<DPDAImportResultDto>.Ok(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing DPDA from CSV file: {FileName}", file?.FileName);
                return ApiResponse<DPDAImportResultDto>.Error($"Lỗi khi import CSV: {ex.Message}");
            }
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Get DPDA by ID with full details
        /// Mapping Entity -> DetailsDto
        /// </summary>
        public async Task<ApiResponse<DPDADetailsDto>> GetByIdAsync(long id)
        {
            try
            {
                var entity = await _dpdaRepository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<DPDADetailsDto>.Error("Không tìm thấy DPDA với ID này");

                var detailsDto = MapToDetailsDto(entity);
                return ApiResponse<DPDADetailsDto>.Ok(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA by ID: {Id}", id);
                return ApiResponse<DPDADetailsDto>.Error($"Lỗi khi lấy thông tin DPDA: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new DPDA record
        /// Mapping CreateDto -> Entity
        /// </summary>
        public async Task<ApiResponse<DPDADetailsDto>> CreateAsync(DPDACreateDto createDto)
        {
            try
            {
                var entity = MapFromCreateDto(createDto);
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                var createdEntity = await _dpdaRepository.CreateAsync(entity);
                var detailsDto = MapToDetailsDto(createdEntity);

                return ApiResponse<DPDADetailsDto>.Ok(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating DPDA: {@CreateDto}", createDto);
                return ApiResponse<DPDADetailsDto>.Error($"Lỗi khi tạo DPDA: {ex.Message}");
            }
        }

        /// <summary>
        /// Update existing DPDA record
        /// Mapping UpdateDto -> Entity với selective update
        /// </summary>
        public async Task<ApiResponse<DPDADetailsDto>> UpdateAsync(DPDAUpdateDto updateDto)
        {
            try
            {
                var existingEntity = await _dpdaRepository.GetByIdAsync(updateDto.Id);
                if (existingEntity == null)
                    return ApiResponse<DPDADetailsDto>.Error("Không tìm thấy DPDA để cập nhật");

                UpdateFromDto(existingEntity, updateDto);
                existingEntity.UpdatedAt = DateTime.UtcNow;

                var updatedEntity = await _dpdaRepository.UpdateAsync(existingEntity);
                var detailsDto = MapToDetailsDto(updatedEntity);

                return ApiResponse<DPDADetailsDto>.Ok(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating DPDA: {@UpdateDto}", updateDto);
                return ApiResponse<DPDADetailsDto>.Error($"Lỗi khi cập nhật DPDA: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete DPDA record by ID
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            try
            {
                var success = await _dpdaRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.Error("Không tìm thấy DPDA để xóa");

                return ApiResponse<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting DPDA with ID: {Id}", id);
                return ApiResponse<bool>.Error($"Lỗi khi xóa DPDA: {ex.Message}");
            }
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Search DPDA by customer code
        /// Business key search với CSV column names
        /// </summary>
        public async Task<ApiResponse<List<DPDAPreviewDto>>> GetByCustomerCodeAsync(string customerCode, int limit = 100)
        {
            try
            {
                var entities = await _dpdaRepository.GetByCustomerCodeAsync(customerCode, limit);
                var previewDtos = entities.Select(MapToPreviewDto).ToList();

                return ApiResponse<List<DPDAPreviewDto>>.Ok(previewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA by customer code: {CustomerCode}", customerCode);
                return ApiResponse<List<DPDAPreviewDto>>.Error($"Lỗi khi tìm DPDA theo mã khách hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Search DPDA by branch code
        /// Business key search với CSV column names
        /// </summary>
        public async Task<ApiResponse<List<DPDAPreviewDto>>> GetByBranchCodeAsync(string branchCode, int limit = 100)
        {
            try
            {
                var entities = await _dpdaRepository.GetByBranchCodeAsync(branchCode, limit);
                var previewDtos = entities.Select(MapToPreviewDto).ToList();

                return ApiResponse<List<DPDAPreviewDto>>.Ok(previewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA by branch code: {BranchCode}", branchCode);
                return ApiResponse<List<DPDAPreviewDto>>.Error($"Lỗi khi tìm DPDA theo mã chi nhánh: {ex.Message}");
            }
        }

        /// <summary>
        /// Search DPDA by account number
        /// Business key search với CSV column names
        /// </summary>
        public async Task<ApiResponse<List<DPDAPreviewDto>>> GetByAccountNumberAsync(string accountNumber, int limit = 100)
        {
            try
            {
                var entities = await _dpdaRepository.GetByAccountNumberAsync(accountNumber, limit);
                var previewDtos = entities.Select(MapToPreviewDto).ToList();

                return ApiResponse<List<DPDAPreviewDto>>.Ok(previewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA by account number: {AccountNumber}", accountNumber);
                return ApiResponse<List<DPDAPreviewDto>>.Error($"Lỗi khi tìm DPDA theo số tài khoản: {ex.Message}");
            }
        }

        /// <summary>
        /// Search DPDA by card number
        /// Business key search với CSV column names
        /// </summary>
        public async Task<ApiResponse<List<DPDAPreviewDto>>> GetByCardNumberAsync(string cardNumber, int limit = 100)
        {
            try
            {
                var entities = await _dpdaRepository.GetByCardNumberAsync(cardNumber, limit);
                var previewDtos = entities.Select(MapToPreviewDto).ToList();

                return ApiResponse<List<DPDAPreviewDto>>.Ok(previewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA by card number: {CardNumber}", cardNumber);
                return ApiResponse<List<DPDAPreviewDto>>.Error($"Lỗi khi tìm DPDA theo số thẻ: {ex.Message}");
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get DPDA statistics and summary
        /// Analytics data cho dashboard
        /// </summary>
        public async Task<ApiResponse<DPDASummaryDto>> GetStatisticsAsync(DateTime? asOfDate = null)
        {
            try
            {
                var effectiveDate = asOfDate ?? DateTime.Today;
                var summary = new DPDASummaryDto();

                // Total cards count
                summary.TotalCards = await _dpdaRepository.GetTotalCountAsync();

                // Cards by status
                var cardsByStatus = await _dpdaRepository.GetCardCountByStatusAsync();
                summary.CardsByStatus = cardsByStatus.ToDictionary(x => x.Key ?? "N/A", x => x.Value);

                // Cards by type
                var cardsByType = await _dpdaRepository.GetCardCountByTypeAsync();
                summary.CardsByType = cardsByType.ToDictionary(x => x.Key ?? "N/A", x => x.Value);

                // Cards by branch (Top 10)
                var cardsByBranch = await _dpdaRepository.GetCardCountByBranchAsync(asOfDate);
                summary.CardsByBranch = cardsByBranch.ToDictionary(x => x.Key ?? "N/A", x => x.Value);

                // Cards by category
                var cardsByCategory = await _dpdaRepository.GetCardCountByCategoryAsync();
                summary.CardsByCategory = cardsByCategory.ToDictionary(x => x.Key ?? "N/A", x => x.Value);

                // Cards issued this month
                var startOfMonth = new DateTime(effectiveDate.Year, effectiveDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var thisMonthData = await _dpdaRepository.GetCardCountByDateRangeAsync(startOfMonth, endOfMonth);
                summary.CardsIssuedThisMonth = thisMonthData.Values.Sum();

                // Cards issued today
                var todayData = await _dpdaRepository.GetCardCountByDateRangeAsync(effectiveDate, effectiveDate);
                summary.CardsIssuedToday = todayData.Values.Sum();

                summary.LastUpdated = DateTime.UtcNow;

                return ApiResponse<DPDASummaryDto>.Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DPDA statistics for date: {AsOfDate}", asOfDate);
                return ApiResponse<DPDASummaryDto>.Error($"Lỗi khi lấy thống kê DPDA: {ex.Message}");
            }
        }

        #endregion

        #region Private Mapping Methods
        // CSV-First: Business columns từ CSV được preserve nguyên vẹn trong mapping

        /// <summary>
        /// Map Entity to PreviewDto
        /// Chỉ các fields cần thiết cho list view
        /// </summary>
        private DPDAPreviewDto MapToPreviewDto(DPDAEntity entity)
        {
            return new DPDAPreviewDto
            {
                Id = entity.Id,
                MA_CHI_NHANH = entity.MA_CHI_NHANH,
                MA_KHACH_HANG = entity.MA_KHACH_HANG,
                TEN_KHACH_HANG = entity.TEN_KHACH_HANG,
                SO_TAI_KHOAN = entity.SO_TAI_KHOAN,
                LOAI_THE = entity.LOAI_THE,
                SO_THE = entity.SO_THE,
                NGAY_PHAT_HANH = entity.NGAY_PHAT_HANH,
                TRANG_THAI = entity.TRANG_THAI,
                UpdatedAt = entity.UpdatedAt
            };
        }

        /// <summary>
        /// Map Entity to DetailsDto
        /// Đầy đủ tất cả 13 business columns + metadata
        /// </summary>
        private DPDADetailsDto MapToDetailsDto(DPDAEntity entity)
        {
            return new DPDADetailsDto
            {
                Id = entity.Id,
                // 13 Business Columns theo CSV structure
                MA_CHI_NHANH = entity.MA_CHI_NHANH,
                MA_KHACH_HANG = entity.MA_KHACH_HANG,
                TEN_KHACH_HANG = entity.TEN_KHACH_HANG,
                SO_TAI_KHOAN = entity.SO_TAI_KHOAN,
                LOAI_THE = entity.LOAI_THE,
                SO_THE = entity.SO_THE,
                NGAY_NOP_DON = entity.NGAY_NOP_DON,
                NGAY_PHAT_HANH = entity.NGAY_PHAT_HANH,
                USER_PHAT_HANH = entity.USER_PHAT_HANH,
                TRANG_THAI = entity.TRANG_THAI,
                PHAN_LOAI = entity.PHAN_LOAI,
                GIAO_THE = entity.GIAO_THE,
                LOAI_PHAT_HANH = entity.LOAI_PHAT_HANH,
                // System columns từ ITemporalEntity
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                // Import tracking metadata - sẽ implement later trong DirectImport
                FileName = $"dpda_{entity.NGAY_DL:yyyyMMdd}.csv", // Derived từ NGAY_DL
                ImportId = Guid.NewGuid(), // Temporary - sẽ implement trong DirectImport
                ImportMetadata = $"DPDA data for {entity.NGAY_DL:dd/MM/yyyy}" // Temporary
            };
        }

        /// <summary>
        /// Map CreateDto to Entity
        /// 13 business columns theo CSV structure
        /// </summary>
        private DPDAEntity MapFromCreateDto(DPDACreateDto createDto)
        {
            return new DPDAEntity
            {
                // 13 Business Columns - CSV structure preserved
                MA_CHI_NHANH = createDto.MA_CHI_NHANH,
                MA_KHACH_HANG = createDto.MA_KHACH_HANG,
                TEN_KHACH_HANG = createDto.TEN_KHACH_HANG,
                SO_TAI_KHOAN = createDto.SO_TAI_KHOAN,
                LOAI_THE = createDto.LOAI_THE,
                SO_THE = createDto.SO_THE,
                NGAY_NOP_DON = createDto.NGAY_NOP_DON,
                NGAY_PHAT_HANH = createDto.NGAY_PHAT_HANH,
                USER_PHAT_HANH = createDto.USER_PHAT_HANH,
                TRANG_THAI = createDto.TRANG_THAI,
                PHAN_LOAI = createDto.PHAN_LOAI,
                GIAO_THE = createDto.GIAO_THE,
                LOAI_PHAT_HANH = createDto.LOAI_PHAT_HANH
            };
        }

        /// <summary>
        /// Update Entity from UpdateDto
        /// Selective update - chỉ update các field không null
        /// </summary>
        private void UpdateFromDto(DPDAEntity entity, DPDAUpdateDto updateDto)
        {
            // Chỉ update các field optional (không phải business keys)
            if (updateDto.TEN_KHACH_HANG != null) entity.TEN_KHACH_HANG = updateDto.TEN_KHACH_HANG;
            if (updateDto.LOAI_THE != null) entity.LOAI_THE = updateDto.LOAI_THE;
            if (updateDto.SO_THE != null) entity.SO_THE = updateDto.SO_THE;
            if (updateDto.NGAY_NOP_DON != null) entity.NGAY_NOP_DON = updateDto.NGAY_NOP_DON;
            if (updateDto.NGAY_PHAT_HANH != null) entity.NGAY_PHAT_HANH = updateDto.NGAY_PHAT_HANH;
            if (updateDto.USER_PHAT_HANH != null) entity.USER_PHAT_HANH = updateDto.USER_PHAT_HANH;
            if (updateDto.TRANG_THAI != null) entity.TRANG_THAI = updateDto.TRANG_THAI;
            if (updateDto.PHAN_LOAI != null) entity.PHAN_LOAI = updateDto.PHAN_LOAI;
            if (updateDto.GIAO_THE != null) entity.GIAO_THE = updateDto.GIAO_THE;
            if (updateDto.LOAI_PHAT_HANH != null) entity.LOAI_PHAT_HANH = updateDto.LOAI_PHAT_HANH;
        }

        #endregion
    }
}
