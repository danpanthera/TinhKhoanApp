using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// GL01 - General Ledger Data Model with exact CSV column structure (27 business columns)
    /// SPECIAL: NGAY_DL comes from TR_TIME column in CSV, partitioned columnstore (not temporal)
    /// Structure: NGAY_DL -> 27 Business Columns (CSV order) -> System Columns
    /// Import policy: Only files containing "gl01" in filename
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        // NGAY_DL - DateTime from TR_TIME column (Order 0)
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // 27 Business Columns - Exact CSV order with proper data types
        [Column("STS", Order = 1)]
        [StringLength(200)]
        public string? STS { get; set; }

        [Column("NGAY_GD", Order = 2)]
        public DateTime? NGAY_GD { get; set; }

        [Column("NGUOI_TAO", Order = 3)]
        [StringLength(200)]
        public string? NGUOI_TAO { get; set; }

        [Column("DYSEQ", Order = 4)]
        [StringLength(200)]
        public string? DYSEQ { get; set; }

        [Column("TR_TYPE", Order = 5)]
        [StringLength(200)]
        public string? TR_TYPE { get; set; }

        [Column("DT_SEQ", Order = 6)]
        [StringLength(200)]
        public string? DT_SEQ { get; set; }

        [Column("TAI_KHOAN", Order = 7)]
        [StringLength(200)]
        public string? TAI_KHOAN { get; set; }

        [Column("TEN_TK", Order = 8)]
        [StringLength(200)]
        public string? TEN_TK { get; set; }

        [Column("SO_TIEN_GD", Order = 9, TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GD { get; set; }

        [Column("POST_BR", Order = 10)]
        [StringLength(200)]
        public string? POST_BR { get; set; }

        [Column("LOAI_TIEN", Order = 11)]
        [StringLength(200)]
        public string? LOAI_TIEN { get; set; }

        [Column("DR_CR", Order = 12)]
        [StringLength(200)]
        public string? DR_CR { get; set; }

        [Column("MA_KH", Order = 13)]
        [StringLength(200)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH", Order = 14)]
        [StringLength(200)]
        public string? TEN_KH { get; set; }

        [Column("CCA_USRID", Order = 15)]
        [StringLength(200)]
        public string? CCA_USRID { get; set; }

        [Column("TR_EX_RT", Order = 16)]
        [StringLength(200)]
        public string? TR_EX_RT { get; set; }

        [Column("REMARK", Order = 17)]
        [StringLength(1000)]
        public string? REMARK { get; set; }

        [Column("BUS_CODE", Order = 18)]
        [StringLength(200)]
        public string? BUS_CODE { get; set; }

        [Column("UNIT_BUS_CODE", Order = 19)]
        [StringLength(200)]
        public string? UNIT_BUS_CODE { get; set; }

        [Column("TR_CODE", Order = 20)]
        [StringLength(200)]
        public string? TR_CODE { get; set; }

        [Column("TR_NAME", Order = 21)]
        [StringLength(200)]
        public string? TR_NAME { get; set; }

        [Column("REFERENCE", Order = 22)]
        [StringLength(200)]
        public string? REFERENCE { get; set; }

        [Column("VALUE_DATE", Order = 23)]
        public DateTime? VALUE_DATE { get; set; }

        [Column("DEPT_CODE", Order = 24)]
        [StringLength(200)]
        public string? DEPT_CODE { get; set; }

        [Column("TR_TIME", Order = 25)]
        [StringLength(200)]
        public string? TR_TIME { get; set; }

        [Column("COMFIRM", Order = 26)]
        [StringLength(200)]
        public string? COMFIRM { get; set; }

        [Column("TRDT_TIME", Order = 27)]
        [StringLength(200)]
        public string? TRDT_TIME { get; set; }

        // System Columns - Primary Key (GL01 is Partitioned Columnstore, NOT Temporal)
        [Key]
        [Column("Id", Order = 28)]
        public long Id { get; set; }

        [Column("CREATED_DATE", Order = 29)]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", Order = 30)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("FILE_NAME", Order = 31)]
        [StringLength(500)]
        public string? FILE_NAME { get; set; }
    }
}
