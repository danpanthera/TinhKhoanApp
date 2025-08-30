using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// GL01 - General Ledger Data Model (UNIFIED STRUCTURE - HEAVY FILE OPTIMIZED)
    /// Structure: NGAY_DL (từ TR_TIME) -> 27 Business Columns -> 4 System Columns (NON-TEMPORAL)
    /// Import policy: Only files containing "gl01" in filename
    /// Business columns: All nvarchar(200), REMARK as nvarchar(1000)
    /// Date columns: datetime2, Decimal columns: decimal(18,2) or decimal(18,6)
    /// Heavy File Support: Partitioned Columnstore for ~200MB CSV files
    /// Direct Import: Business column names match CSV headers exactly (no transformation)
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        // === NGAY_DL - System Column FIRST (Order 0) - từ TR_TIME column ===
        [Column("NGAY_DL", TypeName = "datetime2", Order = 0)]
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === 27 BUSINESS COLUMNS - Exact CSV order ===

        [Column("STS", TypeName = "nvarchar(200)", Order = 1)]
        public string? STS { get; set; }

        [Column("NGAY_GD", TypeName = "datetime2", Order = 2)]
        public DateTime? NGAY_GD { get; set; }

        [Column("NGUOI_TAO", TypeName = "nvarchar(200)", Order = 3)]
        public string? NGUOI_TAO { get; set; }

        [Column("DYSEQ", TypeName = "nvarchar(200)", Order = 4)]
        public string? DYSEQ { get; set; }

        [Column("TR_TYPE", TypeName = "nvarchar(200)", Order = 5)]
        public string? TR_TYPE { get; set; }

        [Column("DT_SEQ", TypeName = "nvarchar(200)", Order = 6)]
        public string? DT_SEQ { get; set; }

        [Column("TAI_KHOAN", TypeName = "nvarchar(200)", Order = 7)]
        public string? TAI_KHOAN { get; set; }

        [Column("TEN_TK", TypeName = "nvarchar(200)", Order = 8)]
        public string? TEN_TK { get; set; }

        // SO_TIEN column -> decimal(18,2) for money format
        [Column("SO_TIEN_GD", TypeName = "decimal(18,2)", Order = 9)]
        public decimal? SO_TIEN_GD { get; set; }

        [Column("POST_BR", TypeName = "nvarchar(200)", Order = 10)]
        public string? POST_BR { get; set; }

        [Column("LOAI_TIEN", TypeName = "nvarchar(200)", Order = 11)]
        public string? LOAI_TIEN { get; set; }

        [Column("DR_CR", TypeName = "nvarchar(200)", Order = 12)]
        public string? DR_CR { get; set; }

        [Column("MA_KH", TypeName = "nvarchar(200)", Order = 13)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH", TypeName = "nvarchar(200)", Order = 14)]
        public string? TEN_KH { get; set; }

        [Column("CCA_USRID", TypeName = "nvarchar(200)", Order = 15)]
        public string? CCA_USRID { get; set; }

        [Column("TR_EX_RT", TypeName = "decimal(18,6)", Order = 16)]
        public decimal? TR_EX_RT { get; set; }

        // REMARK column -> nvarchar(1000) as per requirement
        [Column("REMARK", TypeName = "nvarchar(1000)", Order = 17)]
        public string? REMARK { get; set; }

        [Column("BUS_CODE", TypeName = "nvarchar(200)", Order = 18)]
        public string? BUS_CODE { get; set; }

        [Column("UNIT_BUS_CODE", TypeName = "nvarchar(200)", Order = 19)]
        public string? UNIT_BUS_CODE { get; set; }

        [Column("TR_CODE", TypeName = "nvarchar(200)", Order = 20)]
        public string? TR_CODE { get; set; }

        [Column("TR_NAME", TypeName = "nvarchar(200)", Order = 21)]
        public string? TR_NAME { get; set; }

        [Column("REFERENCE", TypeName = "nvarchar(200)", Order = 22)]
        public string? REFERENCE { get; set; }

        // DATE column -> datetime2
        [Column("VALUE_DATE", TypeName = "datetime2", Order = 23)]
        public DateTime? VALUE_DATE { get; set; }

        [Column("DEPT_CODE", TypeName = "nvarchar(200)", Order = 24)]
        public string? DEPT_CODE { get; set; }

        // TR_TIME column -> datetime2 (used to populate NGAY_DL)
        [Column("TR_TIME", TypeName = "datetime2", Order = 25)]
        public DateTime? TR_TIME { get; set; }

        [Column("COMFIRM", TypeName = "nvarchar(200)", Order = 26)]
        public string? COMFIRM { get; set; }

        [Column("TRDT_TIME", TypeName = "datetime2", Order = 27)]
        public DateTime? TRDT_TIME { get; set; }

        // === 4 SYSTEM COLUMNS - Always last (Order 28-31) ===
        [Key]
        [Column("Id", Order = 28)]
        public long Id { get; set; }

        [Column("CreatedAt", TypeName = "datetime2", Order = 29)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt", TypeName = "datetime2", Order = 30)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("FileName", TypeName = "nvarchar(500)", Order = 31)]
        public string? FileName { get; set; }

        // Heavy file tracking column
        [Column("ImportBatchId", TypeName = "nvarchar(100)", Order = 32)]
        public string? ImportBatchId { get; set; }
    }
}
