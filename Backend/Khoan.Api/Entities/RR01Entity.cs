using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Entities
{
    /// <summary>
    /// RR01 Entity - Risk Report Table với 25 business columns
    /// Tuân thủ 100% yêu cầu specification: NGAY_DL → Business Columns → System Columns
    /// CSV-First Architecture: Column names trùng khớp hoàn toàn với CSV headers
    /// Temporal Table + Columnstore Indexes support
    /// </summary>
    [Table("RR01")]
    public class RR01Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // === NGAY_DL FIRST (extracted từ filename) ===
        [Column("NGAY_DL", TypeName = "datetime2")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === 25 BUSINESS COLUMNS (từ RR01 CSV headers) ===
        
        // Column 1-5: Chi nhánh và khách hàng info
        [Column("CN_LOAI_I")]
        [MaxLength(200)]
        public string? CN_LOAI_I { get; set; }

        [Column("BRCD")]
        [MaxLength(200)]
        public string? BRCD { get; set; }

        [Column("MA_KH")]
        [MaxLength(200)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [MaxLength(200)]
        public string? TEN_KH { get; set; }

        [Column("SO_LDS")]
        [MaxLength(200)]
        public string? SO_LDS { get; set; }

        // Column 6-10: Loan và contract info
        [Column("CCY")]
        [MaxLength(200)]
        public string? CCY { get; set; }

        [Column("SO_LAV")]
        [MaxLength(200)]
        public string? SO_LAV { get; set; }

        [Column("LOAI_KH")]
        [MaxLength(200)]
        public string? LOAI_KH { get; set; }

        [Column("NGAY_GIAI_NGAN", TypeName = "datetime2")]
        public DateTime? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN", TypeName = "datetime2")]
        public DateTime? NGAY_DEN_HAN { get; set; }

        // Column 11-15: Processing và amounts
        [Column("VAMC_FLG")]
        [MaxLength(200)]
        public string? VAMC_FLG { get; set; }

        [Column("NGAY_XLRR", TypeName = "datetime2")]
        public DateTime? NGAY_XLRR { get; set; }

        [Column("DUNO_GOC_BAN_DAU", TypeName = "decimal(18,2)")]
        public decimal? DUNO_GOC_BAN_DAU { get; set; }

        [Column("DUNO_LAI_TICHLUY_BD", TypeName = "decimal(18,2)")]
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

        [Column("DOC_DAUKY_DA_THU_HT", TypeName = "decimal(18,2)")]
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

        // Column 16-20: Current amounts
        [Column("DUNO_GOC_HIENTAI", TypeName = "decimal(18,2)")]
        public decimal? DUNO_GOC_HIENTAI { get; set; }

        [Column("DUNO_LAI_HIENTAI", TypeName = "decimal(18,2)")]
        public decimal? DUNO_LAI_HIENTAI { get; set; }

        [Column("DUNO_NGAN_HAN", TypeName = "decimal(18,2)")]
        public decimal? DUNO_NGAN_HAN { get; set; }

        [Column("DUNO_TRUNG_HAN", TypeName = "decimal(18,2)")]
        public decimal? DUNO_TRUNG_HAN { get; set; }

        [Column("DUNO_DAI_HAN", TypeName = "decimal(18,2)")]
        public decimal? DUNO_DAI_HAN { get; set; }

        // Column 21-25: Recovery và classification
        [Column("THU_GOC", TypeName = "decimal(18,2)")]
        public decimal? THU_GOC { get; set; }

        [Column("THU_LAI", TypeName = "decimal(18,2)")]
        public decimal? THU_LAI { get; set; }

        [Column("BDS", TypeName = "decimal(18,2)")]
        public decimal? BDS { get; set; }

        [Column("DS", TypeName = "decimal(18,2)")]
        public decimal? DS { get; set; }

        [Column("TSK", TypeName = "decimal(18,2)")]
        public decimal? TSK { get; set; }

        // === SYSTEM COLUMNS ===
        [Column("CREATED_DATE", TypeName = "datetime2")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", TypeName = "datetime2")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("IS_DELETED")]
        public bool IS_DELETED { get; set; } = false;

        // Temporal Table System Columns configured as shadow properties
        // SysStartTime và SysEndTime không declare ở đây - EF shadow properties
    }
}
