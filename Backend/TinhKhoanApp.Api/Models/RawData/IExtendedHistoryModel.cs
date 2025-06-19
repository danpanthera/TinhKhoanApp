namespace TinhKhoanApp.Api.Models.RawData
{
    /// <summary>
    /// Interface cho các Extended History Models với SCD Type 2
    /// </summary>
    public interface IExtendedHistoryModel
    {
        string BusinessKey { get; set; }
        DateTime EffectiveDate { get; set; }
        DateTime? ExpiryDate { get; set; }
        bool IsCurrent { get; set; }
        int RowVersion { get; set; }
        string ImportId { get; set; }
        DateTime StatementDate { get; set; }
        DateTime ProcessedDate { get; set; }
        string DataHash { get; set; }
        string? AdditionalData { get; set; }
    }
}
