using Khoan.Api.Models.Common;
using DataTables = Khoan.Api.Models.DataTables;
using Khoan.Api.Models.DTOs.GL02;
using Khoan.Api.Repositories;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    public class GL02Service : IGL02Service
    {
        private readonly IGL02Repository _repository;
        private readonly ILogger<GL02Service> _logger;

        public GL02Service(IGL02Repository repository, ILogger<GL02Service> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GL02PreviewDto>>> PreviewAsync(int take = 20)
        {
            var entities = await _repository.GetRecentAsync(take);
            return ApiResponse<IEnumerable<GL02PreviewDto>>.Ok(entities.Select(MapToPreviewDto));
        }

        public async Task<ApiResponse<GL02DetailsDto>> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return ApiResponse<GL02DetailsDto>.Error("Không tìm thấy GL02", 404);
            return ApiResponse<GL02DetailsDto>.Ok(MapToDetailsDto(entity));
        }

        public async Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var entities = await _repository.GetByDateAsync(date, maxResults);
            return ApiResponse<IEnumerable<GL02DetailsDto>>.Ok(entities.Select(MapToDetailsDto));
        }

        public async Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByUnitAsync(string unit, int maxResults = 100)
        {
            var entities = await _repository.GetByUnitAsync(unit, maxResults);
            return ApiResponse<IEnumerable<GL02DetailsDto>>.Ok(entities.Select(MapToDetailsDto));
        }

        public async Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByTransactionCodeAsync(string trcd, int maxResults = 100)
        {
            var entities = await _repository.GetByTransactionCodeAsync(trcd, maxResults);
            return ApiResponse<IEnumerable<GL02DetailsDto>>.Ok(entities.Select(MapToDetailsDto));
        }

        public async Task<ApiResponse<GL02DetailsDto>> CreateAsync(GL02CreateDto dto)
        {
            var entity = MapFromCreateDto(dto);
            entity.CREATED_DATE = DateTime.UtcNow;
            entity.UPDATED_DATE = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ApiResponse<GL02DetailsDto>.Ok(MapToDetailsDto(entity), "Created");
        }

        public async Task<ApiResponse<GL02DetailsDto>> UpdateAsync(GL02UpdateDto dto)
        {
            var existingEntity = await _repository.GetByIdAsync(dto.Id);
            if (existingEntity == null) return ApiResponse<GL02DetailsDto>.Error("Không tìm thấy GL02", 404);

            UpdateFromDto(existingEntity, dto);
            existingEntity.UPDATED_DATE = DateTime.UtcNow;

            _repository.Update(existingEntity);
            await _repository.SaveChangesAsync();
            return ApiResponse<GL02DetailsDto>.Ok(MapToDetailsDto(existingEntity), "Updated");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return ApiResponse<bool>.Error("Không tìm thấy GL02", 404);

            _repository.Remove(entity);
            await _repository.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Deleted");
        }

        public async Task<ApiResponse<GL02SummaryByUnitDto>> SummaryByUnitAsync(string unit)
        {
            var totalDR = await _repository.GetTotalTransactionsByUnitAsync(unit, "DR");
            var totalCR = await _repository.GetTotalTransactionsByUnitAsync(unit, "CR");
            return ApiResponse<GL02SummaryByUnitDto>.Ok(new GL02SummaryByUnitDto
            {
                UNIT = unit,
                TotalDR = totalDR,
                TotalCR = totalCR
            });
        }

        /// <summary>
        /// Map Entity to PreviewDto
        /// Hiển thị các fields quan trọng nhất
        /// </summary>
        private static GL02PreviewDto MapToPreviewDto(DataTables.GL02 entity)
        {
            return new GL02PreviewDto
            {
                NGAY_DL = entity.NGAY_DL,
                UNIT = entity.UNIT,
                TRCD = entity.TRCD,
                CCY = entity.CCY,
                DRAMOUNT = entity.DRAMOUNT,
                CRAMOUNT = entity.CRAMOUNT,
                CRTDTM = entity.CRTDTM
            };
        }

        /// <summary>
        /// Map Entity to DetailsDto
        /// Đầy đủ tất cả 17 business columns + metadata
        /// </summary>
        private static GL02DetailsDto MapToDetailsDto(DataTables.GL02 entity)
        {
            return new GL02DetailsDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                // 17 Business Columns theo CSV structure
                TRDATE = null, // DataTables.GL02 maps TRDATE to NGAY_DL
                TRBRCD = entity.TRBRCD,
                USERID = entity.USERID,
                JOURSEQ = entity.JOURSEQ,
                DYTRSEQ = entity.DYTRSEQ,
                LOCAC = entity.LOCAC,
                CCY = entity.CCY,
                BUSCD = entity.BUSCD,
                UNIT = entity.UNIT,
                TRCD = entity.TRCD,
                CUSTOMER = entity.CUSTOMER,
                TRTP = entity.TRTP,
                REFERENCE = entity.REFERENCE,
                REMARK = entity.REMARK,
                DRAMOUNT = entity.DRAMOUNT,
                CRAMOUNT = entity.CRAMOUNT,
                CRTDTM = entity.CRTDTM,
                // System columns
                CreatedAt = entity.CREATED_DATE,
                UpdatedAt = entity.UPDATED_DATE,
                // Import tracking metadata
                FileName = entity.FILE_NAME ?? $"gl02_{entity.NGAY_DL:yyyyMMdd}.csv",
                ImportId = null,
                ImportMetadata = null
            };
        }

        /// <summary>
        /// Map CreateDto to Entity
        /// </summary>
        private static DataTables.GL02 MapFromCreateDto(GL02CreateDto dto)
        {
            return new DataTables.GL02
            {
                NGAY_DL = dto.NGAY_DL,
                // 17 Business Columns
                // TRDATE mapped via NGAY_DL in DataTables.GL02
                TRBRCD = dto.TRBRCD ?? string.Empty,
                USERID = dto.USERID,
                JOURSEQ = dto.JOURSEQ,
                DYTRSEQ = dto.DYTRSEQ,
                LOCAC = dto.LOCAC ?? string.Empty,
                CCY = dto.CCY ?? string.Empty,
                BUSCD = dto.BUSCD,
                UNIT = dto.UNIT,
                TRCD = dto.TRCD,
                CUSTOMER = dto.CUSTOMER,
                TRTP = dto.TRTP,
                REFERENCE = dto.REFERENCE,
                REMARK = dto.REMARK,
                DRAMOUNT = dto.DRAMOUNT,
                CRAMOUNT = dto.CRAMOUNT,
                CRTDTM = dto.CRTDTM,
                // System columns sẽ được set trong service
            };
        }

        /// <summary>
        /// Update Entity từ UpdateDto
        /// </summary>
        private static void UpdateFromDto(DataTables.GL02 entity, GL02UpdateDto dto)
        {
            entity.NGAY_DL = dto.NGAY_DL;
            // 17 Business Columns
            // TRDATE mapped via NGAY_DL in DataTables.GL02
            entity.TRBRCD = dto.TRBRCD ?? string.Empty;
            entity.USERID = dto.USERID;
            entity.JOURSEQ = dto.JOURSEQ;
            entity.DYTRSEQ = dto.DYTRSEQ;
            entity.LOCAC = dto.LOCAC ?? string.Empty;
            entity.CCY = dto.CCY ?? string.Empty;
            entity.BUSCD = dto.BUSCD;
            entity.UNIT = dto.UNIT;
            entity.TRCD = dto.TRCD;
            entity.CUSTOMER = dto.CUSTOMER;
            entity.TRTP = dto.TRTP;
            entity.REFERENCE = dto.REFERENCE;
            entity.REMARK = dto.REMARK;
            entity.DRAMOUNT = dto.DRAMOUNT;
            entity.CRAMOUNT = dto.CRAMOUNT;
            entity.CRTDTM = dto.CRTDTM;
        }
    }
}
