namespace TinhKhoanApp.Api.Models.DTO;

public class Gahr26ImportRowDto
{
    public string EmployeeCode { get; set; } = string.Empty; // Mã NV
    public string? CBCode { get; set; } // Mã CB (9 digits)
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? UserAD { get; set; }
    public string? Email { get; set; }
    public string? UserIPCAS { get; set; }
    public string? MaCBTD { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public int? RoleId { get; set; }
}

public class Gahr26BulkImportRequest
{
    public List<Gahr26ImportRowDto> Rows { get; set; } = new();
    public bool OverwriteExisting { get; set; } = true; // update existing
    public bool AutoGenerateMissingUsernames { get; set; } = true;
}

public class Gahr26BulkImportResult
{
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}
