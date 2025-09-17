using Khoan.Api.Models.Common;
using Khoan.Api.Models.DTOs.GL02;

namespace Khoan.Api.Services.Interfaces
{
    public interface IGL02Service
    {
        Task<ApiResponse<IEnumerable<GL02PreviewDto>>> PreviewAsync(int take = 20);
        Task<ApiResponse<GL02DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByDateAsync(DateTime date, int maxResults = 100);
        Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByUnitAsync(string unit, int maxResults = 100);
        Task<ApiResponse<IEnumerable<GL02DetailsDto>>> GetByTransactionCodeAsync(string trcd, int maxResults = 100);
        Task<ApiResponse<GL02DetailsDto>> CreateAsync(GL02CreateDto dto);
        Task<ApiResponse<GL02DetailsDto>> UpdateAsync(GL02UpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<GL02SummaryByUnitDto>> SummaryByUnitAsync(string unit);
    }
}
