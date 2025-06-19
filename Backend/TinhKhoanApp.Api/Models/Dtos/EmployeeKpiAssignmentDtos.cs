using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    public class EmployeeKpiAssignmentDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int KpiDefinitionId { get; set; }
        public string KpiName { get; set; } = string.Empty;
        public int KhoanPeriodId { get; set; }
        public string KhoanPeriodName { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? Score { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateEmployeeKpiAssignmentDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int KpiDefinitionId { get; set; }

        [Required]
        public int KhoanPeriodId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Target value must be greater than or equal to 0")]
        public decimal TargetValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Actual value must be greater than or equal to 0")]
        public decimal? ActualValue { get; set; }

        public string? Notes { get; set; }
    }

    public class UpdateEmployeeKpiAssignmentDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Target value must be greater than or equal to 0")]
        public decimal TargetValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Actual value must be greater than or equal to 0")]
        public decimal? ActualValue { get; set; }

        public string? Notes { get; set; }
    }

    public class BulkCreateEmployeeKpiAssignmentDto
    {
        [Required]
        public List<CreateEmployeeKpiAssignmentDto> Assignments { get; set; } = new();
    }

    // 🔥 Bulk Operations DTOs
    public class BulkKpiAssignmentDto
    {
        [Required]
        public List<int> EmployeeIds { get; set; } = new();
        
        [Required]
        public List<int> KpiIds { get; set; } = new();
        
        [Required]
        public int KhoanPeriodId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Target value must be greater than or equal to 0")]
        public decimal TargetValue { get; set; }
        
        public string? Notes { get; set; }
    }

    public class BulkAssignmentResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int TotalCount { get; set; }
        public List<string> SuccessMessages { get; set; } = new();
        public List<string> ErrorMessages { get; set; } = new();
        public List<EmployeeKpiAssignmentDto> CreatedAssignments { get; set; } = new();
    }

    public class BulkScoreUpdateDto
    {
        [Required]
        public List<ScoreUpdateItem> ScoreUpdates { get; set; } = new();
    }

    public class ScoreUpdateItem
    {
        [Required]
        public int AssignmentId { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Actual value must be greater than or equal to 0")]
        public decimal? ActualValue { get; set; }
        
        public string? Notes { get; set; }
    }

    public class BulkUpdateResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int TotalCount { get; set; }
        public List<string> SuccessMessages { get; set; } = new();
        public List<string> ErrorMessages { get; set; } = new();
        public List<EmployeeKpiAssignmentDto> UpdatedAssignments { get; set; } = new();
    }

    public class BulkDeleteAssignmentDto
    {
        [Required]
        public List<int> AssignmentIds { get; set; } = new();
    }

    public class BulkDeleteResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int TotalCount { get; set; }
        public List<string> SuccessMessages { get; set; } = new();
        public List<string> ErrorMessages { get; set; } = new();
        public List<int> DeletedAssignmentIds { get; set; } = new();
    }
}