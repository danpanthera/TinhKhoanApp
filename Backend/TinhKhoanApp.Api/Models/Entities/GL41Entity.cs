using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// GL41 Entity - Trial Balance (Bảng cân đối kế toán)
    /// CSV Source: 7800_gl41_yyyymmdd.csv
    /// Business Columns: MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY
    /// 100% Business Column Compliance
    /// </summary>
    [Table("GL41")]
    public class GL41Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // === CSV BUSINESS COLUMNS - 100% MATCH ===

        /// <summary>
        /// Mã chi nhánh - Business Column 1
        /// </summary>
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string? MA_CN { get; set; }

        /// <summary>
        /// Loại tiền - Business Column 2
        /// </summary>
        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Mã tài khoản - Business Column 3
        /// </summary>
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? MA_TK { get; set; }

        /// <summary>
        /// Tên tài khoản - Business Column 4
        /// </summary>
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Loại bút toán - Business Column 5
        /// </summary>
        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string? LOAI_BT { get; set; }

        /// <summary>
        /// Dư nợ đầu kỳ - Business Column 6
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DN_DAUKY { get; set; }

        /// <summary>
        /// Dư có đầu kỳ - Business Column 7
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DC_DAUKY { get; set; }

        /// <summary>
        /// Số bút toán nợ - Business Column 8
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SBT_NO { get; set; }

        /// <summary>
        /// Số tiền ghi nợ - Business Column 9
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ST_GHINO { get; set; }

        /// <summary>
        /// Số bút toán có - Business Column 10
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SBT_CO { get; set; }

        /// <summary>
        /// Số tiền ghi có - Business Column 11
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ST_GHICO { get; set; }

        /// <summary>
        /// Dư nợ cuối kỳ - Business Column 12
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DN_CUOIKY { get; set; }

        /// <summary>
        /// Dư có cuối kỳ - Business Column 13
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DC_CUOIKY { get; set; }

        // === SYSTEM COLUMNS ===

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? NGAY_DL { get; set; }

        /// <summary>
        /// Tên file nguồn
        /// </summary>
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? FILE_NAME { get; set; }

        /// <summary>
        /// Ngày tạo record
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày cập nhật record
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? UpdatedAt { get; set; }
    }
}
