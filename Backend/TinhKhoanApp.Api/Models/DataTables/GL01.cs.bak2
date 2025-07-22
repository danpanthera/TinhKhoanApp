using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng GL01 - Dữ liệu Giao dịch chi tiết
    /// Business columns first (27 columns from CSV), then system columns
    /// Partitioned Columnstore table (NO temporal columns)
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        // ======= BUSINESS COLUMNS (27 columns - exactly from CSV) =======
        [Column("STS")]
        [StringLength(50)]
        public string? STS { get; set; }

        [Column("NGAY_GD")]
        public DateTime? NGAY_GD { get; set; }

        [Column("NGUOI_TAO")]
        [StringLength(100)]
        public string? NGUOI_TAO { get; set; }

        [Column("DYSEQ")]
        [StringLength(100)]
        public string? DYSEQ { get; set; }

        [Column("TR_TYPE")]
        [StringLength(50)]
        public string? TR_TYPE { get; set; }

        [Column("DT_SEQ")]
        [StringLength(100)]
        public string? DT_SEQ { get; set; }

        [Column("TAI_KHOAN")]
        [StringLength(100)]
        public string? TAI_KHOAN { get; set; }

        [Column("TEN_TK")]
        [StringLength(500)]
        public string? TEN_TK { get; set; }

        [Column("SO_TIEN_GD", TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GD { get; set; }

        [Column("POST_BR")]
        [StringLength(50)]
        public string? POST_BR { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("DR_CR")]
        [StringLength(10)]
        public string? DR_CR { get; set; }

        [Column("MA_KH")]
        [StringLength(100)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(500)]
        public string? TEN_KH { get; set; }

        [Column("CCA_USRID")]
        [StringLength(100)]
        public string? CCA_USRID { get; set; }

        [Column("TR_EX_RT", TypeName = "decimal(10,4)")]
        public decimal? TR_EX_RT { get; set; }

        [Column("REMARK")]
        [StringLength(1000)]
        public string? REMARK { get; set; }

        [Column("BUS_CODE")]
        [StringLength(50)]
        public string? BUS_CODE { get; set; }

        [Column("UNIT_BUS_CODE")]
        [StringLength(50)]
        public string? UNIT_BUS_CODE { get; set; }

        [Column("TR_CODE")]
        [StringLength(50)]
        public string? TR_CODE { get; set; }

        [Column("TR_NAME")]
        [StringLength(200)]
        public string? TR_NAME { get; set; }

        [Column("REFERENCE")]
        [StringLength(200)]
        public string? REFERENCE { get; set; }

        [Column("VALUE_DATE")]
        public DateTime? VALUE_DATE { get; set; }

        [Column("DEPT_CODE")]
        [StringLength(50)]
        public string? DEPT_CODE { get; set; }

        [Column("TR_TIME")]
        [StringLength(50)]
        public string? TR_TIME { get; set; }

        [Column("COMFIRM")]
        [StringLength(10)]
        public string? COMFIRM { get; set; }

        [Column("TRDT_TIME")]
        [StringLength(50)]
        public string? TRDT_TIME { get; set; }

        // ======= SYSTEM COLUMNS =======
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("NGAY_DL")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Column("CreatedAt")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("IsDeleted")]
        [Required]
        public bool IsDeleted { get; set; } = false;

        // ======= NO TEMPORAL COLUMNS (Partitioned Columnstore) =======
    }
}
