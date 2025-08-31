using Khoan.Api.Models.DataTables;
using Khoan.Api.Dtos.RR01;

namespace Khoan.Api.Services.Interfaces;

/// <summary>
/// RR01 Service Interface - Risk Report operations với CSV import logic
/// Tuân thủ business column names, không transformation tiếng Việt
/// </summary>
public interface IRR01Service
{
    // === CSV IMPORT OPERATIONS ===
    DateTime? ExtractDateFromFilename(string fileName);
    bool ValidateFileName(string fileName);
        Task<List<RR01>> ParseGenericCSVAsync(string csvFilePath, DateTime ngayDl);
        Task<RR01ImportResultDto> BulkInsertGenericAsync(List<RR01> entities, string fileName);    // === DATA RETRIEVAL ===
    Task<IEnumerable<RR01PreviewDto>> GetRR01PreviewAsync(DateTime? ngayDl = null, string? branchCode = null, int skip = 0, int take = 100);
    Task<RR01DetailsDto?> GetRR01ByIdAsync(int id);
    Task<RR01SummaryDto> GetRR01SummaryAsync(DateTime ngayDl);
}
