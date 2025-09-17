using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// DP01 Entity - Production Data Table (63 business columns)
    /// Represents DP01 table structure with temporal table support
    /// CSV Source: 7800_dp01_20241231.csv
    /// </summary>
    [Table("DP01")]
    public class DP01Entity : ITemporalEntity
    {
        // === SYSTEM COLUMNS ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // === TEMPORAL TABLE COLUMNS ===
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }

        // === BUSINESS COLUMNS (63 columns from CSV) ===

        [StringLength(10)]
        public string BRCD { get; set; } = string.Empty;

        [StringLength(50)]
        public string CUSTSEQ { get; set; } = string.Empty;

        [StringLength(255)]
        public string CUSTNM { get; set; } = string.Empty;

        [StringLength(50)]
        public string TAI_KHOAN { get; set; } = string.Empty;

        [StringLength(10)]
        public string CCY { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DU_NO { get; set; }

        [StringLength(50)]
        public string DSBSSEQ { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        public DateTime? TRANSACTION_DATE { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DSBSDT { get; set; }

        [StringLength(10)]
        public string DISBUR_CCY { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DISBURSEMENT_AMOUNT { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DSBSMATDT { get; set; }

        [StringLength(20)]
        public string BSRTCD { get; set; } = string.Empty;

        [Column(TypeName = "decimal(8,4)")]
        public decimal INTEREST_RATE { get; set; }

        [StringLength(50)]
        public string APPRSEQ { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        public DateTime? APPRDT { get; set; }

        [StringLength(10)]
        public string APPR_CCY { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal APPRAMT { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? APPRMATDT { get; set; }

        [StringLength(50)]
        public string LOAN_TYPE { get; set; } = string.Empty;

        [StringLength(50)]
        public string FUND_RESOURCE_CODE { get; set; } = string.Empty;

        [StringLength(50)]
        public string FUND_PURPOSE_CODE { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal REPAYMENT_AMOUNT { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NEXT_REPAY_DATE { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NEXT_REPAY_AMOUNT { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        [StringLength(100)]
        public string OFFICER_ID { get; set; } = string.Empty;

        [StringLength(255)]
        public string OFFICER_NAME { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal INTEREST_AMOUNT { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PASTDUE_INTEREST_AMOUNT { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [StringLength(50)]
        public string CUSTOMER_TYPE_CODE { get; set; } = string.Empty;

        [StringLength(100)]
        public string CUSTOMER_TYPE_CODE_DETAIL { get; set; } = string.Empty;

        [StringLength(50)]
        public string TRCTCD { get; set; } = string.Empty;

        [StringLength(255)]
        public string TRCTNM { get; set; } = string.Empty;

        [StringLength(500)]
        public string ADDR1 { get; set; } = string.Empty;

        [StringLength(100)]
        public string PROVINCE { get; set; } = string.Empty;

        [StringLength(255)]
        public string LCLPROVINNM { get; set; } = string.Empty;

        [StringLength(100)]
        public string DISTRICT { get; set; } = string.Empty;

        [StringLength(255)]
        public string LCLDISTNM { get; set; } = string.Empty;

        [StringLength(100)]
        public string COMMCD { get; set; } = string.Empty;

        [StringLength(255)]
        public string LCLWARDNM { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        public DateTime? LAST_REPAY_DATE { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal SECURED_PERCENT { get; set; }

        [StringLength(50)]
        public string NHOM_NO { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        [StringLength(10)]
        public string EXEMPTINT { get; set; } = string.Empty;

        [StringLength(50)]
        public string EXEMPTINTTYPE { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal EXEMPTINTAMT { get; set; }

        [StringLength(50)]
        public string GRPNO { get; set; } = string.Empty;

        [StringLength(50)]
        public string BUSCD { get; set; } = string.Empty;

        [StringLength(50)]
        public string BSNSSCLTPCD { get; set; } = string.Empty;

        [StringLength(100)]
        public string USRIDOP { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ACCRUAL_AMOUNT { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        public int INTCMTH { get; set; }

        public int INTRPYMTH { get; set; }

        public int INTTRMMTH { get; set; }

        public int YRDAYS { get; set; }

        [StringLength(1000)]
        public string REMARK { get; set; } = string.Empty;

        [StringLength(100)]
        public string CHITIEU { get; set; } = string.Empty;

        [StringLength(100)]
        public string CTCV { get; set; } = string.Empty;

        // === CONSTRUCTOR ===
        public DP01Entity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
