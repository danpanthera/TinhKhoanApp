using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    [Table("FinalPayouts")]
    public class FinalPayout
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [Required]
        public int KhoanPeriodId { get; set; }
        [ForeignKey("KhoanPeriodId")]
        public virtual KhoanPeriod? KhoanPeriod { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal? V1 { get; set; }
        public decimal? V2 { get; set; }
        public decimal? CompletionFactor { get; set; }
        public string? Note { get; set; }
    }
}
