using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.LN03;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    public interface ILN03Service
    {
        Task<ApiResponse<PagedResult<LN03PreviewDto>>> GetPreviewAsync(int pageNumber, int pageSize, DateTime? ngayDL = null);
        Task<ApiResponse<LN03DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<LN03DetailsDto>> CreateAsync(LN03CreateDto dto);
        Task<ApiResponse<LN03DetailsDto>> UpdateAsync(long id, LN03UpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(long id);

        Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100);
        Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByBranchAsync(string branchCode, int maxResults = 100);
        Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByCustomerAsync(string customerCode, int maxResults = 100);

        Task<ApiResponse<LN03ProcessingSummaryDto>> GetProcessingSummaryAsync(DateTime? ngayDL = null);
    }
}
