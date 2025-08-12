using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs.GL41;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// GL41 Service - xử lý business logic cho GL41 với manual mapping
    /// </summary>
    public class GL41Service : IGL41Service
    {
        private readonly IGL41Repository _repository;
        private readonly ILogger<GL41Service> _logger;

        public GL41Service(IGL41Repository repository, ILogger<GL41Service> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GL41PreviewDto>>> PreviewAsync(int take = 20)
        {
            try
            {
                var entities = await _repository.GetRecentAsync(take);
                var previews = entities.Select(ToPreview);
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.PreviewAsync");
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Error($"Lỗi lấy preview GL41: {ex.Message}");
            }
        }

        public async Task<ApiResponse<GL41DetailsDto>> GetByIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    return ApiResponse<GL41DetailsDto>.Error("Không tìm thấy GL41", 404);
                }

                var details = ToDetails(entity);
                return ApiResponse<GL41DetailsDto>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.GetByIdAsync with ID: {Id}", id);
                return ApiResponse<GL41DetailsDto>.Error($"Lỗi lấy GL41 theo ID: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date);
                var details = entities.Take(maxResults).Select(ToDetails);
                return ApiResponse<IEnumerable<GL41DetailsDto>>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.GetByDateAsync with date: {Date}", date);
                return ApiResponse<IEnumerable<GL41DetailsDto>>.Error($"Lỗi lấy GL41 theo ngày: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByUnitAsync(string unitCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByUnitCodeAsync(unitCode, maxResults);
                var details = entities.Select(ToDetails);
                return ApiResponse<IEnumerable<GL41DetailsDto>>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.GetByUnitAsync with unit: {Unit}", unitCode);
                return ApiResponse<IEnumerable<GL41DetailsDto>>.Error($"Lỗi lấy GL41 theo đơn vị: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByAccountCodeAsync(accountCode, maxResults);
                var details = entities.Select(ToDetails);
                return ApiResponse<IEnumerable<GL41DetailsDto>>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.GetByAccountCodeAsync with account: {Account}", accountCode);
                return ApiResponse<IEnumerable<GL41DetailsDto>>.Error($"Lỗi lấy GL41 theo mã tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<GL41DetailsDto>> CreateAsync(GL41CreateDto dto)
        {
            try
            {
                var entity = FromCreate(dto);
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                var details = ToDetails(entity);
                return ApiResponse<GL41DetailsDto>.Ok(details, "Tạo GL41 thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.CreateAsync");
                return ApiResponse<GL41DetailsDto>.Error($"Lỗi tạo GL41: {ex.Message}");
            }
        }

        public async Task<ApiResponse<GL41DetailsDto>> UpdateAsync(GL41UpdateDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)dto.ID);
                if (entity == null)
                {
                    return ApiResponse<GL41DetailsDto>.Error("Không tìm thấy GL41", 404);
                }

                ApplyUpdate(entity, dto);
                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                var details = ToDetails(entity);
                return ApiResponse<GL41DetailsDto>.Ok(details, "Cập nhật GL41 thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.UpdateAsync with ID: {Id}", dto.ID);
                return ApiResponse<GL41DetailsDto>.Error($"Lỗi cập nhật GL41: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    return ApiResponse<bool>.Error("Không tìm thấy GL41", 404);
                }

                _repository.Remove(entity);
                await _repository.SaveChangesAsync();

                return ApiResponse<bool>.Ok(true, "Xóa GL41 thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.DeleteAsync with ID: {Id}", id);
                return ApiResponse<bool>.Error($"Lỗi xóa GL41: {ex.Message}");
            }
        }

        public async Task<ApiResponse<GL41SummaryByUnitDto>> SummaryByUnitAsync(string unitCode, DateTime? date = null)
        {
            try
            {
                var openingBalance = await _repository.GetTotalOpeningBalanceByUnitAsync(unitCode, date);
                var closingBalance = await _repository.GetTotalClosingBalanceByUnitAsync(unitCode, date);
                var (debitTransactions, creditTransactions) = await _repository.GetTotalTransactionsByUnitAsync(unitCode, date);

                var summary = new GL41SummaryByUnitDto
                {
                    MA_CN = unitCode,
                    TotalOpeningBalance = openingBalance,
                    TotalClosingBalance = closingBalance,
                    TotalDebitTransactions = debitTransactions,
                    TotalCreditTransactions = creditTransactions
                };

                return ApiResponse<GL41SummaryByUnitDto>.Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GL41Service.SummaryByUnitAsync with unit: {Unit}", unitCode);
                return ApiResponse<GL41SummaryByUnitDto>.Error($"Lỗi lấy thống kê GL41: {ex.Message}");
            }
        }

        #region Private Mapping Methods

        /// <summary>
        /// Chuyển đổi Entity thành PreviewDto
        /// </summary>
        private static GL41PreviewDto ToPreview(GL41 entity)
        {
            return new GL41PreviewDto
            {
                NGAY_DL = entity.NGAY_DL,
                MA_CN = entity.MA_CN,
                LOAI_TIEN = entity.LOAI_TIEN,
                MA_TK = entity.MA_TK,
                TEN_TK = entity.TEN_TK,
                DN_DAUKY = entity.DN_DAUKY,
                DC_DAUKY = entity.DC_DAUKY,
                DN_CUOIKY = entity.DN_CUOIKY,
                DC_CUOIKY = entity.DC_CUOIKY
            };
        }

        /// <summary>
        /// Chuyển đổi Entity thành DetailsDto
        /// </summary>
        private static GL41DetailsDto ToDetails(GL41 entity)
        {
            return new GL41DetailsDto
            {
                ID = entity.ID,
                NGAY_DL = entity.NGAY_DL,
                MA_CN = entity.MA_CN,
                LOAI_TIEN = entity.LOAI_TIEN,
                MA_TK = entity.MA_TK,
                TEN_TK = entity.TEN_TK,
                LOAI_BT = entity.LOAI_BT,
                DN_DAUKY = entity.DN_DAUKY,
                DC_DAUKY = entity.DC_DAUKY,
                SBT_NO = entity.SBT_NO,
                ST_GHINO = entity.ST_GHINO,
                SBT_CO = entity.SBT_CO,
                ST_GHICO = entity.ST_GHICO,
                DN_CUOIKY = entity.DN_CUOIKY,
                DC_CUOIKY = entity.DC_CUOIKY,
                FILE_NAME = entity.FILE_NAME,
                CREATED_DATE = entity.CREATED_DATE
            };
        }

        /// <summary>
        /// Chuyển đổi CreateDto thành Entity
        /// </summary>
        private static GL41 FromCreate(GL41CreateDto dto)
        {
            return new GL41
            {
                NGAY_DL = dto.NGAY_DL,
                MA_CN = dto.MA_CN,
                LOAI_TIEN = dto.LOAI_TIEN,
                MA_TK = dto.MA_TK,
                TEN_TK = dto.TEN_TK,
                LOAI_BT = dto.LOAI_BT,
                DN_DAUKY = dto.DN_DAUKY,
                DC_DAUKY = dto.DC_DAUKY,
                SBT_NO = dto.SBT_NO,
                ST_GHINO = dto.ST_GHINO,
                SBT_CO = dto.SBT_CO,
                ST_GHICO = dto.ST_GHICO,
                DN_CUOIKY = dto.DN_CUOIKY,
                DC_CUOIKY = dto.DC_CUOIKY,
                CREATED_DATE = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Áp dụng UpdateDto lên Entity
        /// </summary>
        private static void ApplyUpdate(GL41 entity, GL41UpdateDto dto)
        {
            entity.NGAY_DL = dto.NGAY_DL;
            entity.MA_CN = dto.MA_CN;
            entity.LOAI_TIEN = dto.LOAI_TIEN;
            entity.MA_TK = dto.MA_TK;
            entity.TEN_TK = dto.TEN_TK;
            entity.LOAI_BT = dto.LOAI_BT;
            entity.DN_DAUKY = dto.DN_DAUKY;
            entity.DC_DAUKY = dto.DC_DAUKY;
            entity.SBT_NO = dto.SBT_NO;
            entity.ST_GHINO = dto.ST_GHINO;
            entity.SBT_CO = dto.SBT_CO;
            entity.ST_GHICO = dto.ST_GHICO;
            entity.DN_CUOIKY = dto.DN_CUOIKY;
            entity.DC_CUOIKY = dto.DC_CUOIKY;
        }

        #endregion
    }
}
