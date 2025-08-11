using TinhKhoanApp.Api.Models.Dtos.DP01;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    public interface IDP01Service
    {
        Task<PagedResult<DP01PreviewDto>> GetPagedAsync(int page, int pageSize, DateTime? asOfDate = null);
        Task<DP01DetailsDto?> GetByIdAsync(long id);
        Task<DP01DetailsDto> CreateAsync(DP01CreateDto dto);
        Task<DP01DetailsDto> UpdateAsync(long id, DP01UpdateDto dto);
        Task<bool> DeleteAsync(long id);
        Task<DP01SummaryDto> GetSummaryAsync(DateTime? asOfDate = null);
    }
}
