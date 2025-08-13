using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.Dtos.GL01;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    public class GL01Service : IGL01Service
    {
        private readonly IGL01Repository _repo;
        private readonly ILogger<GL01Service> _logger;

        public GL01Service(IGL01Repository repo, ILogger<GL01Service> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetPreviewAsync(int count = 10)
        {
            var data = await _repo.GetRecentAsync(count);
            return ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data.Select(MapToPreview));
        }

        public async Task<ApiResponse<GL01DetailsDto?>> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync((int)id);
            return ApiResponse<GL01DetailsDto?>.Ok(entity == null ? null : MapToDetails(entity));
        }

        public async Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var data = await _repo.GetByDateAsync(date);
            return ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data.Take(maxResults).Select(MapToPreview));
        }

        public async Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByUnitAsync(string unitCode, int maxResults = 100)
        {
            var data = await _repo.GetByUnitCodeAsync(unitCode, maxResults);
            return ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data.Select(MapToPreview));
        }

        public async Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByAccountAsync(string accountCode, int maxResults = 100)
        {
            var data = await _repo.GetByAccountCodeAsync(accountCode, maxResults);
            return ApiResponse<IEnumerable<GL01PreviewDto>>.Ok(data.Select(MapToPreview));
        }

        public async Task<ApiResponse<GL01DetailsDto>> CreateAsync(GL01CreateDto dto)
        {
            var entity = new GL01Entity
            {
                NGAY_DL = dto.NGAY_DL,
                TAI_KHOAN = dto.TAI_KHOAN,
                DR_CR = dto.DR_CR,
                SO_TIEN_GD = dto.SO_TIEN_GD,
                POST_BR = dto.POST_BR,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return ApiResponse<GL01DetailsDto>.Ok(MapToDetails(entity), "Created");
        }

        public async Task<ApiResponse<GL01DetailsDto?>> UpdateAsync(long id, GL01UpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync((int)id);
            if (existing == null)
                return ApiResponse<GL01DetailsDto?>.Error("Not found", "NOT_FOUND");

            if (dto.SO_TIEN_GD.HasValue) existing.SO_TIEN_GD = dto.SO_TIEN_GD;
            if (!string.IsNullOrWhiteSpace(dto.REMARK)) existing.REMARK = dto.REMARK;
            existing.UpdatedAt = DateTime.UtcNow;

            _repo.Update(existing);
            await _repo.SaveChangesAsync();
            return ApiResponse<GL01DetailsDto?>.Ok(MapToDetails(existing));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var existing = await _repo.GetByIdAsync((int)id);
            if (existing == null)
                return ApiResponse<bool>.Error("Not found", "NOT_FOUND");
            _repo.Remove(existing);
            await _repo.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }

        public async Task<ApiResponse<GL01SummaryByUnitDto>> GetSummaryByUnitAsync(string unitCode, DateTime? date = null)
        {
            var debit = await _repo.GetTotalTransactionsByUnitAsync(unitCode, "DR", date);
            var credit = await _repo.GetTotalTransactionsByUnitAsync(unitCode, "CR", date);
            return ApiResponse<GL01SummaryByUnitDto>.Ok(new GL01SummaryByUnitDto
            {
                POST_BR = unitCode,
                TotalDebit = debit,
                TotalCredit = credit
            });
        }

        private static GL01PreviewDto MapToPreview(GL01Entity e) => new()
        {
            Id = e.Id,
            NGAY_DL = e.NGAY_DL,
            TAI_KHOAN = e.TAI_KHOAN,
            TEN_TK = e.TEN_TK,
            SO_TIEN_GD = e.SO_TIEN_GD,
            DR_CR = e.DR_CR,
            POST_BR = e.POST_BR,
            NGAY_GD = e.NGAY_GD,
            MA_KH = e.MA_KH
        };

        private static GL01DetailsDto MapToDetails(GL01Entity e) => new()
        {
            Id = e.Id,
            NGAY_DL = e.NGAY_DL,
            STS = e.STS,
            NGAY_GD = e.NGAY_GD,
            NGUOI_TAO = e.NGUOI_TAO,
            DYSEQ = e.DYSEQ,
            TR_TYPE = e.TR_TYPE,
            DT_SEQ = e.DT_SEQ,
            TAI_KHOAN = e.TAI_KHOAN,
            TEN_TK = e.TEN_TK,
            SO_TIEN_GD = e.SO_TIEN_GD,
            POST_BR = e.POST_BR,
            LOAI_TIEN = e.LOAI_TIEN,
            DR_CR = e.DR_CR,
            MA_KH = e.MA_KH,
            TEN_KH = e.TEN_KH,
            CCA_USRID = e.CCA_USRID,
            TR_EX_RT = e.TR_EX_RT,
            REMARK = e.REMARK,
            BUS_CODE = e.BUS_CODE,
            UNIT_BUS_CODE = e.UNIT_BUS_CODE,
            TR_CODE = e.TR_CODE,
            TR_NAME = e.TR_NAME,
            REFERENCE = e.REFERENCE,
            VALUE_DATE = e.VALUE_DATE,
            DEPT_CODE = e.DEPT_CODE,
            TR_TIME = e.TR_TIME?.ToString(@"hh\:mm\:ss"),
            COMFIRM = e.COMFIRM,
            TRDT_TIME = e.TRDT_TIME?.ToString("yyyy-MM-dd HH:mm:ss"),
            CREATED_DATE = e.CreatedAt,
            UPDATED_DATE = e.UpdatedAt,
            FILE_NAME = e.FILE_NAME
        };
    }
}
