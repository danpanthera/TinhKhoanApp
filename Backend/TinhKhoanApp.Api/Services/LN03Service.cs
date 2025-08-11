using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service quản lý dữ liệu LN03 (Khoản vay)
    /// </summary>
    public class LN03Service : ILN03Service
    {
        private readonly LN03Repository _repository;

        public LN03Service(ILN03Repository repository)
        {
            _repository = (LN03Repository)repository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetAllAsync()
        {
            try
            {
                var response = await _repository.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return response.Data.Select(ConvertToDTO);
                }
                return new List<LN03DTO>();
            }
            catch (Exception ex)
            {
                return new List<LN03DTO>();
            }
        }

        /// <inheritdoc/>
        public async Task<LN03DTO?> GetByIdAsync(long id)
        {
            var response = await _repository.GetByIdAsync(id);
            if (response.Success && response.Data != null)
            {
                return ConvertToDTO(response.Data);
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetRecentAsync(int count = 10)
        {
            var pagedResponse = await _repository.GetPagedAsync(1, count);
            if (pagedResponse != null && pagedResponse.Items.Any())
            {
                return pagedResponse.Items.Select(ConvertToDTO);
            }
            return new List<LN03DTO>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var entities = await _repository.GetByDateAsync(date);
            return entities.Take(maxResults).Select(ConvertToDTO);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            // Tạm thời trả về empty list vì Repository chưa có method này
            return new List<LN03DTO>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            // Tạm thời trả về empty list vì Repository chưa có method này
            return new List<LN03DTO>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByOfficerCodeAsync(string officerCode, int maxResults = 100)
        {
            // Tạm thời trả về empty list vì Repository chưa có method này
            return new List<LN03DTO>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100)
        {
            // Tạm thời trả về empty list vì Repository chưa có method này
            return new List<LN03DTO>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            // Tạm thời trả về empty list vì Repository chưa có method này
            return new List<LN03DTO>();
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalRiskAmountByBranchAsync(string branchCode, DateTime? date = null)
        {
            // Tạm thời trả về 0 vì Repository chưa có method này
            return 0;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalDebtRecoveryByBranchAsync(string branchCode, DateTime? date = null)
        {
            // Tạm thời trả về 0 vì Repository chưa có method này
            return 0;
        }

        /// <inheritdoc/>
        public async Task<LN03DTO> CreateAsync(CreateLN03DTO createDto)
        {
            // Tạo entity từ CreateDto
            var entity = new LN03Entity
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
                FileName = createDto.FileName
            };

            var result = await _repository.BulkInsertAsync(new List<LN03Entity> { entity });
            if (result.SuccessCount > 0)
            {
                return ConvertToDTO(entity);
            }
            throw new InvalidOperationException("Failed to create LN03 record");
        }

        /// <inheritdoc/>
        public async Task<LN03DTO?> UpdateAsync(long id, UpdateLN03DTO updateDto)
        {
            // Tạm thời trả về null vì chưa có implementation
            return null;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(long id)
        {
            var result = await _repository.BulkDeleteAsync(new List<long> { id });
            return result.SuccessCount > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(long id)
        {
            var entity = await _repository.GetEntityByIdAsync(id);
            return entity != null;
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChangesAsync()
        {
            // Repository pattern đã handle save changes
            return true;
        }

        /// <summary>
        /// Convert LN03Entity to LN03DTO
        /// </summary>
        private LN03DTO ConvertToDTO(LN03Entity entity)
        {
            return new LN03DTO
            {
                Id = entity.Id,
                MACHINHANH = entity.MACHINHANH,
                TENCHINHANH = entity.TENCHINHANH,
                MAKH = entity.MAKH,
                TENKH = entity.TENKH,
                NGAY_DL = entity.NGAY_DL
            };
        }

        /// <summary>
        /// Convert LN03PreviewDto to LN03DTO
        /// </summary>
        private LN03DTO ConvertToDTO(LN03PreviewDto dto)
        {
            return new LN03DTO
            {
                Id = dto.Id,
                MACHINHANH = dto.MACHINHANH,
                TENCHINHANH = dto.TENCHINHANH,
                MAKH = dto.MAKH,
                TENKH = dto.TENKH,
                NGAY_DL = dto.NGAY_DL
            };
        }

        /// <summary>
        /// Convert LN03DetailsDto to LN03DTO
        /// </summary>
        private LN03DTO ConvertToDTO(LN03DetailsDto dto)
        {
            return new LN03DTO
            {
                Id = dto.Id,
                MACHINHANH = dto.MACHINHANH,
                TENCHINHANH = dto.TENCHINHANH,
                MAKH = dto.MAKH,
                TENKH = dto.TENKH,
                NGAY_DL = dto.NGAY_DL
            };
        }
    }
}
