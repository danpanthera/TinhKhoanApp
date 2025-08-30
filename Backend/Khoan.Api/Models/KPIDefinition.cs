using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    // Enum for KPI value types
    public enum KpiValueType
    {
        NUMBER = 1,
        PERCENTAGE = 2,
        POINTS = 3,
        CURRENCY = 4
    }

    // KPI Definition model
    public class KPIDefinition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string KpiCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string KpiName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaxScore { get; set; }

        [Required]
        public KpiValueType ValueType { get; set; } = KpiValueType.NUMBER;

        [MaxLength(50)]
        public string? UnitOfMeasure { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(20)]
        public string Version { get; set; } = "1.0";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<KpiIndicator>? KpiIndicators { get; set; }
    }
}
