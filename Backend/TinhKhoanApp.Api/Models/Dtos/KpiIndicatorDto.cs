using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Models.Dtos
{
    public class KpiIndicatorDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string IndicatorCode { get; set; } = "";
        public string IndicatorName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal MaxScore { get; set; }
        public int OrderIndex { get; set; }
        public KpiValueType ValueType { get; set; } = KpiValueType.NUMBER;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TableName { get; set; } = "";
        public string Category { get; set; } = "";
    }
}
