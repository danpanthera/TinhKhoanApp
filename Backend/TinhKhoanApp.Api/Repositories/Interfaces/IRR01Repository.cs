using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Entities;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// RR01 Repository Interface - Risk Report table
    /// Extends base repository with RR01Entity-specific operations
    /// </summary>
    public interface IRR01Repository : IBaseRepository<RR01Entity>
    {
        // RR01-specific query operations
        Task<List<RR01>> GetByRiskCategoryAsync(string riskCategory, DateTime ngayDL);
        Task<List<RR01>> GetByRiskLevelAsync(int minRiskLevel, DateTime ngayDL);
        Task<decimal> CalculateTotalRiskExposureAsync(DateTime ngayDL);
        Task<Dictionary<string, decimal>> GetRiskByTypeAsync(DateTime ngayDL);

        // RR01-specific aggregation operations
        Task<decimal> GetTotalDUNOAsync(DateTime ngayDL);
        Task<decimal> GetTotalTHUAsync(DateTime ngayDL);
        Task<decimal> GetTotalBDSAsync(DateTime ngayDL);
        Task<decimal> GetTotalDSAsync(DateTime ngayDL);

        // RR01-specific search operations
        Task<List<RR01>> SearchByDonViAsync(string donVi, DateTime? ngayDL = null);
        Task<List<RR01>> GetHighRiskRecordsAsync(DateTime ngayDL, decimal threshold);
    }
}
