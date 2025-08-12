using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs.GL02;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    public class GL02Service : IGL02Service
    {
        private readonly IGL02Repository _repo;
        private readonly ILogger<GL02Service> _logger;

        public GL02Service(IGL02Repository repo, ILogger<GL02Service> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GL02PreviewDto>>> PreviewAsync(int take = 20)
        {
            var items = await _repo.GetRecentAsync(take);
            return ApiResponse<IEnumerable<GL02PreviewDto>>.Ok(items.Select(ToPreview));
        }

        public async Task<ApiResponse<GL02DetailsDto>> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ApiResponse<GL02DetailsDto>.Error("Không tìm thấy GL02", 404);
            return ApiResponse<GL02DetailsDto>.Ok(ToDetails(entity));
        }

        public async Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var items = await _repo.GetByDateAsync(date, maxResults);
            return ApiResponse<IEnumerable<GL02DetailsDto>>.Ok(items.Select(ToDetails));
        }

        public async Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByUnitAsync(string unit, int maxResults = 100)
        {
            var items = await _repo.GetByUnitAsync(unit, maxResults);
            return ApiResponse<IEnumerable<GL02DetailsDto>>.Ok(items.Select(ToDetails));
        }

        public async Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByTransactionCodeAsync(string trcd, int maxResults = 100)
        {
            var items = await _repo.GetByTransactionCodeAsync(trcd, maxResults);
            return ApiResponse<IEnumerable<GL02DetailsDto>>.Ok(items.Select(ToDetails));
        }

        public async Task<ApiResponse<GL02DetailsDto>> CreateAsync(GL02CreateDto dto)
        {
            var entity = FromCreate(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return ApiResponse<GL02DetailsDto>.Ok(ToDetails(entity), "Created");
        }

        public async Task<ApiResponse<GL02DetailsDto>> UpdateAsync(GL02UpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return ApiResponse<GL02DetailsDto>.Error("Không tìm thấy GL02", 404);
            ApplyUpdate(entity, dto);
            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return ApiResponse<GL02DetailsDto>.Ok(ToDetails(entity), "Updated");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ApiResponse<bool>.Error("Không tìm thấy GL02", 404);
            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Deleted");
        }

        public async Task<ApiResponse<GL02SummaryByUnitDto>> SummaryByUnitAsync(string unit)
        {
            var dr = await _repo.GetTotalTransactionsByUnitAsync(unit, "DR");
            var cr = await _repo.GetTotalTransactionsByUnitAsync(unit, "CR");
            return ApiResponse<GL02SummaryByUnitDto>.Ok(new GL02SummaryByUnitDto { UNIT = unit, TotalDR = dr, TotalCR = cr });
        }

        private static GL02PreviewDto ToPreview(GL02 e) => new()
        {
            NGAY_DL = e.NGAY_DL,
            UNIT = e.UNIT,
            TRCD = e.TRCD,
            CCY = e.CCY,
            DRAMOUNT = e.DRAMOUNT,
            CRAMOUNT = e.CRAMOUNT,
            CRTDTM = e.CRTDTM
        };

        private static GL02DetailsDto ToDetails(GL02 e) => new()
        {
            Id = e.Id,
            NGAY_DL = e.NGAY_DL,
            TRBRCD = e.TRBRCD,
            USERID = e.USERID,
            JOURSEQ = e.JOURSEQ,
            DYTRSEQ = e.DYTRSEQ,
            LOCAC = e.LOCAC,
            CCY = e.CCY,
            BUSCD = e.BUSCD,
            UNIT = e.UNIT,
            TRCD = e.TRCD,
            CUSTOMER = e.CUSTOMER,
            TRTP = e.TRTP,
            REFERENCE = e.REFERENCE,
            REMARK = e.REMARK,
            DRAMOUNT = e.DRAMOUNT,
            CRAMOUNT = e.CRAMOUNT,
            CRTDTM = e.CRTDTM,
            CREATED_DATE = e.CREATED_DATE,
            UPDATED_DATE = e.UPDATED_DATE,
            FILE_NAME = e.FILE_NAME
        };

        private static GL02 FromCreate(GL02CreateDto d) => new()
        {
            NGAY_DL = d.NGAY_DL,
            TRBRCD = d.TRBRCD,
            USERID = d.USERID,
            JOURSEQ = d.JOURSEQ,
            DYTRSEQ = d.DYTRSEQ,
            LOCAC = d.LOCAC,
            CCY = d.CCY,
            BUSCD = d.BUSCD,
            UNIT = d.UNIT,
            TRCD = d.TRCD,
            CUSTOMER = d.CUSTOMER,
            TRTP = d.TRTP,
            REFERENCE = d.REFERENCE,
            REMARK = d.REMARK,
            DRAMOUNT = d.DRAMOUNT,
            CRAMOUNT = d.CRAMOUNT,
            CRTDTM = d.CRTDTM,
            CREATED_DATE = DateTime.UtcNow,
            UPDATED_DATE = DateTime.UtcNow
        };

        private static void ApplyUpdate(GL02 e, GL02UpdateDto d)
        {
            e.NGAY_DL = d.NGAY_DL;
            e.TRBRCD = d.TRBRCD;
            e.USERID = d.USERID;
            e.JOURSEQ = d.JOURSEQ;
            e.DYTRSEQ = d.DYTRSEQ;
            e.LOCAC = d.LOCAC;
            e.CCY = d.CCY;
            e.BUSCD = d.BUSCD;
            e.UNIT = d.UNIT;
            e.TRCD = d.TRCD;
            e.CUSTOMER = d.CUSTOMER;
            e.TRTP = d.TRTP;
            e.REFERENCE = d.REFERENCE;
            e.REMARK = d.REMARK;
            e.DRAMOUNT = d.DRAMOUNT;
            e.CRAMOUNT = d.CRAMOUNT;
            e.CRTDTM = d.CRTDTM;
            e.UPDATED_DATE = DateTime.UtcNow;
        }
    }
}
