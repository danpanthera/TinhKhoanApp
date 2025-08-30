using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.GL01;

namespace Khoan.Api.Services.Interfaces
{
    public interface IGL01Service
    {
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetPreviewAsync(int count = 10);
        Task<ApiResponse<GL01DetailsDto?>> GetByIdAsync(long id);
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100);
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByUnitAsync(string unitCode, int maxResults = 100);
        Task<ApiResponse<IEnumerable<GL01PreviewDto>>> GetByAccountAsync(string accountCode, int maxResults = 100);
        Task<ApiResponse<GL01DetailsDto>> CreateAsync(GL01CreateDto dto);
        Task<ApiResponse<GL01DetailsDto?>> UpdateAsync(long id, GL01UpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<GL01SummaryByUnitDto>> GetSummaryByUnitAsync(string unitCode, DateTime? date = null);
    }
}
