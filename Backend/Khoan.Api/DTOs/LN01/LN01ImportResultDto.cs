namespace Khoan.Api.Dtos.LN01
{
    /// <summary>
    /// LN01 Import Result DTO - For CSV import operations
    /// </summary>
    public class LN01ImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int ErrorRows { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public DateTime ImportDateTime { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime? ExtractedDate { get; set; }
        public string ImportSummary => $"Imported {SuccessRows}/{TotalRows} records. {ErrorRows} errors.";
        public bool IsSuccess => ErrorRows == 0 && SuccessRows > 0;
    }
}
