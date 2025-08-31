using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Khoan.Api.Dtos.LN01;

namespace Khoan.Api.Interfaces
{
    public interface ILN01Service
    {
        Task<IEnumerable<LN01PreviewDto>> GetAllAsync();
        Task<LN01DetailsDto> GetByIdAsync(int id);
        Task<LN01DetailsDto> CreateAsync(LN01CreateDto dto);
        Task<LN01DetailsDto> UpdateAsync(int id, LN01UpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<LN01SummaryDto> GetSummaryAsync();
        Task<IEnumerable<LN01PreviewDto>> GetByBranchAsync(string branchCode);
        Task<LN01ImportResultDto> ImportFromCsvAsync(Stream csvStream, LN01ConfigDto? config = null);
        Task<Stream> ExportToCsvAsync();
        Task<IEnumerable<LN01PreviewDto>> GetRecentAsync(int count = 100);
    }
}
