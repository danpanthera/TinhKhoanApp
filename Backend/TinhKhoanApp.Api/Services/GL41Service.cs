using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// GL41 Service - Bảng cân đối tài khoản (Partitioned Columnstore)
    /// </summary>
    public class GL41Service
    {
        private readonly IGL41Repository _gl41Repository;

        public GL41Service(IGL41Repository gl41Repository)
        {
            _gl41Repository = gl41Repository;
        }

        /// <summary>
        /// Lấy danh sách GL41 gần đây
        /// </summary>
        public async Task<IEnumerable<GL41DTO>> GetRecentAsync(int count = 10)
        {
            var entities = await _gl41Repository.GetRecentAsync(count);
            return entities.Select(MapToDTO);
        }

        /// <summary>
        /// Lấy GL41 theo ngày
        /// </summary>
        public async Task<IEnumerable<GL41DTO>> GetByDateAsync(DateTime date)
        {
            var entities = await _gl41Repository.GetByDateAsync(date);
            return entities.Select(MapToDTO);
        }

        /// <summary>
        /// Lấy GL41 theo mã chi nhánh
        /// </summary>
        public async Task<IEnumerable<GL41DTO>> GetByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            var entities = await _gl41Repository.GetByUnitCodeAsync(unitCode, maxResults);
            return entities.Select(MapToDTO);
        }

        /// <summary>
        /// Lấy GL41 theo mã tài khoản
        /// </summary>
        public async Task<IEnumerable<GL41DTO>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            var entities = await _gl41Repository.GetByAccountCodeAsync(accountCode, maxResults);
            return entities.Select(MapToDTO);
        }

        /// <summary>
        /// Tính tổng dư đầu kỳ theo chi nhánh
        /// </summary>
        public async Task<decimal> GetTotalOpeningBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            return await _gl41Repository.GetTotalOpeningBalanceByUnitAsync(unitCode, date);
        }

        /// <summary>
        /// Tính tổng dư cuối kỳ theo chi nhánh
        /// </summary>
        public async Task<decimal> GetTotalClosingBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            return await _gl41Repository.GetTotalClosingBalanceByUnitAsync(unitCode, date);
        }

        /// <summary>
        /// Tính tổng phát sinh theo chi nhánh
        /// </summary>
        public async Task<(decimal Debit, decimal Credit)> GetTotalTransactionsByUnitAsync(string unitCode, DateTime? date = null)
        {
            return await _gl41Repository.GetTotalTransactionsByUnitAsync(unitCode, date);
        }

        /// <summary>
        /// Thêm danh sách GL41Entity
        /// </summary>
        public async Task AddBulkAsync(IEnumerable<GL41Entity> entities)
        {
            await _gl41Repository.AddRangeAsync(entities);
        }

        /// <summary>
        /// Cập nhật danh sách GL41Entity
        /// </summary>
        public async Task UpdateBulkAsync(IEnumerable<GL41Entity> entities)
        {
            _gl41Repository.UpdateRange(entities);
            await _gl41Repository.SaveChangesAsync();
        }

        /// <summary>
        /// Map GL41Entity to GL41DTO
        /// </summary>
        private static GL41DTO MapToDTO(GL41Entity entity)
        {
            return new GL41DTO
            {
                Id = entity.Id,
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
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                FILE_NAME = entity.FILE_NAME
            };
        }

        /// <summary>
        /// Map GL41DTO to GL41Entity
        /// </summary>
        public static GL41Entity MapToEntity(GL41DTO dto, string fileName)
        {
            return new GL41Entity
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
                FILE_NAME = fileName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
