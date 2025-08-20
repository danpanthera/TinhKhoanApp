using System.Collections.Generic;

namespace TinhKhoanApp.Api.Contracts.KpiAssignments;

/// <summary>
/// Request model for assigning KPI targets
/// </summary>
[Serializable]
public class AssignKpiRequest
{
    public int EmployeeId { get; set; }
    public int KhoanPeriodId { get; set; }
    public List<KpiTargetRequest> Targets { get; set; } = new();
}

/// <summary>
/// Individual KPI target in the assignment request
/// </summary>
[Serializable]
public class KpiTargetRequest
{
    public int IndicatorId { get; set; }
    public decimal TargetValue { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request model để cập nhật giá trị thực hiện của một KPI Assignment
/// </summary>
[Serializable]
public class UpdateEmployeeActualRequest
{
    public int AssignmentId { get; set; }
    public decimal? ActualValue { get; set; }
}
