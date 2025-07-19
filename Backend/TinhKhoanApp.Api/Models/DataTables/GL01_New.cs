using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL01 - Dữ liệu sổ cái (Partitioned Table)
    /// CSV structure: 27 columns exactly matching database
    /// NGAY_DL từ TR_TIME column trong CSV
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        // System columns first
        [Key]
        public long Id { get; set; }

        [Column("NGAY_DL")]
        public DateTime NGAY_DL { get; set; }

        // Business columns (27 columns from CSV, exact order)
        [Column("STS")]
        [StringLength(50)]
        public string? STS { get; set; }

        [Column("NGAY_GD")]
        [StringLength(50)]
        public string? NGAY_GD { get; set; }

        [Column("NGUOI_TAO")]
        [StringLength(100)]
        public string? NGUOI_TAO { get; set; }

        [Column("DYSEQ")]
        [StringLength(50)]
        public string? DYSEQ { get; set; }

        [Column("TR_TYPE")]
        [StringLength(50)]
        public string? TR_TYPE { get; set; }

        [Column("DT_SEQ")]
        [StringLength(50)]
        public string? DT_SEQ { get; set; }

        [Column("TAI_KHOAN")]
        [StringLength(100)]
        public string? TAI_KHOAN { get; set; }

        [Column("TEN_TK")]
        [StringLength(500)]
        public string? TEN_TK { get; set; }

        [Column("SO_TIEN_GD")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GD { get; set; }

        [Column("POST_BR")]
        [StringLength(50)]
        public string? POST_BR { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(20)]
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

        [Column("TR_EX_RT")]
        [Column(TypeName = "decimal(10,4)")]
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
        [StringLength(50)]
        public string? VALUE_DATE { get; set; }

        [Column("DEPT_CODE")]
        [StringLength(50)]
        public string? DEPT_CODE { get; set; }

        [Column("TR_TIME")]
        [StringLength(50)]
        public string? TR_TIME { get; set; }

        [Column("COMFIRM")]
        [StringLength(50)]
        public string? COMFIRM { get; set; }

        [Column("TRDT_TIME")]
        [StringLength(50)]
        public string? TRDT_TIME { get; set; }

        // System columns last
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
