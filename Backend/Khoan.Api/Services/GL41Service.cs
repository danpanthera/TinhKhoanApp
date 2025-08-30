using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.GL41;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// GL41 Service - implements IGL41Service with CSV-first model (13 business columns + NGAY_DL)
    /// </summary>
    public class GL41Service : IGL41Service
    {
        private readonly IGL41Repository _gl41Repository;

        public GL41Service(IGL41Repository gl41Repository)
        {
            _gl41Repository = gl41Repository;
        }

        public async Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var items = await _gl41Repository.GetAllPagedAsync(pageIndex, pageSize);
                var data = items.Select(e => new GL41PreviewDto
                {
                    NGAY_DL = e.NGAY_DL,
                    MA_CN = e.MA_CN,
                    LOAI_TIEN = e.LOAI_TIEN,
                    MA_TK = e.MA_TK,
                    TEN_TK = e.TEN_TK,
                    DN_DAUKY = e.DN_DAUKY,
                    DC_DAUKY = e.DC_DAUKY,
                    DN_CUOIKY = e.DN_CUOIKY,
                    DC_CUOIKY = e.DC_CUOIKY
                });
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Error($"Error retrieving GL41 data: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<GL41PreviewDto>>> PreviewAsync(int take = 20)
        {
            var items = await _gl41Repository.GetRecentAsync(take);
            var data = items.Select(e => new GL41PreviewDto
            {
                NGAY_DL = e.NGAY_DL,
                MA_CN = e.MA_CN,
                LOAI_TIEN = e.LOAI_TIEN,
                MA_TK = e.MA_TK,
                TEN_TK = e.TEN_TK,
                DN_DAUKY = e.DN_DAUKY,
                DC_DAUKY = e.DC_DAUKY,
                DN_CUOIKY = e.DN_CUOIKY,
                DC_CUOIKY = e.DC_CUOIKY
            });
            return ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(data);
        }

        public async Task<ApiResponse<GL41DetailsDto>> GetByIdAsync(long id)
        {
            var entity = await _gl41Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return ApiResponse<GL41DetailsDto>.Error($"GL41 record with ID {id} not found", 404);
            }
            return ApiResponse<GL41DetailsDto>.Ok(MapToDetails(entity));
        }

        public async Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var items = await _gl41Repository.GetByDateAsync(date);
            var data = items.Take(maxResults).Select(MapToDetails);
            return ApiResponse<IEnumerable<GL41DetailsDto>>.Ok(data);
        }

        public async Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByUnitAsync(string unitCode, int maxResults = 100)
        {
            var items = await _gl41Repository.GetByUnitCodeAsync(unitCode, maxResults);
            var data = items.Select(MapToDetails);
            return ApiResponse<IEnumerable<GL41DetailsDto>>.Ok(data);
        }

        public async Task<ApiResponse<IEnumerable<GL41DetailsDto>>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            var items = await _gl41Repository.GetByAccountCodeAsync(accountCode, maxResults);
            var data = items.Select(MapToDetails);
            return ApiResponse<IEnumerable<GL41DetailsDto>>.Ok(data);
        }

        public async Task<ApiResponse<GL41DetailsDto>> CreateAsync(GL41CreateDto dto)
        {
            // Map to entity
            var entity = new GL41Entity
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
                CreatedAt = DateTime.UtcNow
            };

            await _gl41Repository.AddAsync(entity);
            await _gl41Repository.SaveChangesAsync();

            return ApiResponse<GL41DetailsDto>.Ok(MapToDetails(entity), "Created");
        }

        public async Task<ApiResponse<GL41DetailsDto>> UpdateAsync(GL41UpdateDto dto)
        {
            var entity = await _gl41Repository.GetByIdAsync(dto.ID);
            if (entity == null)
            {
                return ApiResponse<GL41DetailsDto>.Error($"GL41 record with ID {dto.ID} not found", 404);
            }

            // Update business fields only
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

            _gl41Repository.Update(entity);
            await _gl41Repository.SaveChangesAsync();

            return ApiResponse<GL41DetailsDto>.Ok(MapToDetails(entity), "Updated");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var entity = await _gl41Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return ApiResponse<bool>.Error($"GL41 record with ID {id} not found", 404);
            }

            _gl41Repository.Remove(entity);
            await _gl41Repository.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Deleted");
        }

        public async Task<ApiResponse<GL41SummaryByUnitDto>> SummaryByUnitAsync(string unitCode, DateTime? date = null)
        {
            var opening = await _gl41Repository.GetTotalOpeningBalanceByUnitAsync(unitCode, date);
            var closing = await _gl41Repository.GetTotalClosingBalanceByUnitAsync(unitCode, date);
            var (debit, credit) = await _gl41Repository.GetTotalTransactionsByUnitAsync(unitCode, date);

            var dto = new GL41SummaryByUnitDto
            {
                MA_CN = unitCode,
                TotalOpeningBalance = opening,
                TotalClosingBalance = closing,
                TotalDebitTransactions = debit,
                TotalCreditTransactions = credit
            };

            return ApiResponse<GL41SummaryByUnitDto>.Ok(dto);
        }

        private static GL41DetailsDto MapToDetails(GL41Entity e)
        {
            return new GL41DetailsDto
            {
                ID = e.Id,
                NGAY_DL = e.NGAY_DL,
                MA_CN = e.MA_CN,
                LOAI_TIEN = e.LOAI_TIEN,
                MA_TK = e.MA_TK,
                TEN_TK = e.TEN_TK,
                LOAI_BT = e.LOAI_BT,
                DN_DAUKY = e.DN_DAUKY,
                DC_DAUKY = e.DC_DAUKY,
                SBT_NO = e.SBT_NO,
                ST_GHINO = e.ST_GHINO,
                SBT_CO = e.SBT_CO,
                ST_GHICO = e.ST_GHICO,
                DN_CUOIKY = e.DN_CUOIKY,
                DC_CUOIKY = e.DC_CUOIKY,
                FILE_NAME = e.FILE_NAME,
                CREATED_DATE = e.CreatedAt
            };
        }
    }
}
