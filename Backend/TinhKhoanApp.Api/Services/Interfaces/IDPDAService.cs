using TinhKhoanApp.Api.Models.Dtos.DPDA;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    public interface IDPDAService
    {
        Task<PagedResult<DPDAPreviewDto>> GetPagedAsync(int page, int pageSize, DateTime? asOfDate = null);
        Task<DPDADetailsDto?> GetByIdAsync(long id);
        Task<DPDADetailsDto> CreateAsync(DPDACreateDto dto);
        Task<DPDADetailsDto> UpdateAsync(long id, DPDAUpdateDto dto);
        Task<bool> DeleteAsync(long id);
        Task<DPDASummaryDto> GetSummaryAsync(DateTime? asOfDate = null);
    }
}
