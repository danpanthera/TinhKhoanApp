using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// GL41 Entity - Trial Balance (Bảng cân đối kế toán)
    /// Partitioned Columnstore structure (NO temporal)
    /// CSV Source: 7800_gl41_yyyymmdd.csv
    /// Structure: Id + NGAY_DL + 13 business columns + 4 system columns = 19 total columns
    /// </summary>
    [Table("GL41")]
    public class GL41Entity : IEntity
    {
        // === PRIMARY KEY ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // === NGAY_DL - CORE DATA COLUMN ===
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime NGAY_DL { get; set; }

        // === 13 BUSINESS COLUMNS (theo CSV và README_DAT.md) ===

        /// <summary>
        /// Mã đơn vị cơ sở - BUSINESS COLUMN 1
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string MA_DVCS { get; set; } = string.Empty;

        /// <summary>
        /// Tên đơn vị cơ sở - BUSINESS COLUMN 2
        /// </summary>
        [Required]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string TEN_DVCS { get; set; } = string.Empty;

        /// <summary>
        /// Mã tài khoản - BUSINESS COLUMN 3
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string MA_TK { get; set; } = string.Empty;

        /// <summary>
        /// Tên tài khoản - BUSINESS COLUMN 4
        /// </summary>
        [Required]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string TEN_TK { get; set; } = string.Empty;

        /// <summary>
        /// Số dư đầu nợ - BUSINESS COLUMN 5
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SO_DU_DAU_NO { get; set; }

        /// <summary>
        /// Số dư đầu có - BUSINESS COLUMN 6
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SO_DU_DAU_CO { get; set; }

        /// <summary>
        /// Phát sinh nợ - BUSINESS COLUMN 7
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PHAT_SINH_NO { get; set; }

        /// <summary>
        /// Phát sinh có - BUSINESS COLUMN 8
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PHAT_SINH_CO { get; set; }

        /// <summary>
        /// Số dư cuối nợ - BUSINESS COLUMN 9
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SO_DU_CUOI_NO { get; set; }

        /// <summary>
        /// Số dư cuối có - BUSINESS COLUMN 10
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SO_DU_CUOI_CO { get; set; }

        /// <summary>
        /// Loại tài khoản - BUSINESS COLUMN 11
        /// </summary>
        [Required]
        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string LOAI_TK { get; set; } = string.Empty;

        /// <summary>
        /// Cấp tài khoản - BUSINESS COLUMN 12
        /// </summary>
        [Required]
        public int CAP_TK { get; set; }

        /// <summary>
        /// Trạng thái tài khoản - BUSINESS COLUMN 13
        /// </summary>
        [Required]
        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string TT_TK { get; set; } = string.Empty;

        // === 4 SYSTEM COLUMNS ===
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string CreatedBy { get; set; } = "SYSTEM";

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string LastModifiedBy { get; set; } = "SYSTEM";

        // === IEntity Implementation ===
        [NotMapped]
        public DateTime CreatedAt
        {
            get => CreatedDate;
            set => CreatedDate = value;
        }

        [NotMapped]
        public DateTime UpdatedAt
        {
            get => LastModifiedDate;
            set => LastModifiedDate = value;
        }

        // NO TEMPORAL COLUMNS - GL41 is Partitioned Columnstore only
    }
}
