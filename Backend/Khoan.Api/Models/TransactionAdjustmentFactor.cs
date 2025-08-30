using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    [Table("TransactionAdjustmentFactors")]
    public class TransactionAdjustmentFactor
    {
        [Key]
        public int Id { get; set; }

        // TODO: This will be modified when implementing new KPI system
        // For now, keep as int to prevent build errors
        public int? LegacyKPIDefinitionId { get; set; }

        [Required]
        public decimal Factor { get; set; }

        public string? Note { get; set; }
    }
}
