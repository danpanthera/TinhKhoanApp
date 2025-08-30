using System;
using System.Collections.Generic;

namespace Khoan.Api.Contracts.KpiAssignments;

[Serializable]
public class AssignKpiRequest
{
    public int EmployeeId { get; set; }
    public int KhoanPeriodId { get; set; }
    public List<KpiTargetRequest> Targets { get; set; } = new();
}

[Serializable]
public class KpiTargetRequest
{
    public int IndicatorId { get; set; }
    public decimal TargetValue { get; set; }
    public string? Notes { get; set; }
}

[Serializable]
public class UpdateEmployeeActualRequest
{
    public int AssignmentId { get; set; }
    public decimal? ActualValue { get; set; }
}
