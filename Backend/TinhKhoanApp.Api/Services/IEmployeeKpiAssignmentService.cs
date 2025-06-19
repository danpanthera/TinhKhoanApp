using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Dtos;

namespace TinhKhoanApp.Api.Services
{
    public interface IEmployeeKpiAssignmentService
    {
        Task<IEnumerable<EmployeeKpiAssignmentDto>> GetAllAsync();
        Task<EmployeeKpiAssignmentDto?> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeKpiAssignmentDto>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeKpiAssignmentDto>> GetByKhoanPeriodIdAsync(int khoanPeriodId);
        Task<EmployeeKpiAssignmentDto> CreateAsync(CreateEmployeeKpiAssignmentDto createDto);
        Task<EmployeeKpiAssignmentDto?> UpdateAsync(int id, UpdateEmployeeKpiAssignmentDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<EmployeeKpiAssignmentDto>> CreateBulkAsync(IEnumerable<CreateEmployeeKpiAssignmentDto> createDtos);
        Task<bool> ExistsAsync(int employeeId, int kpiDefinitionId, int khoanPeriodId);

        // 🔥 Bulk Operations Methods
        Task<BulkAssignmentResultDto> BulkAssignKpisAsync(BulkKpiAssignmentDto request);
        Task<BulkUpdateResultDto> BulkUpdateScoresAsync(BulkScoreUpdateDto request);
        Task<BulkDeleteResultDto> BulkDeleteAssignmentsAsync(BulkDeleteAssignmentDto request);
    }
}
