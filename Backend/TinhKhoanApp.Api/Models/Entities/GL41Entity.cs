using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// GL41 Entity - Bảng cân đối kế toán (13 business columns)
    /// Represents GL41 table structure with Partitioned Columnstore (NO temporal)
    /// CSV Source: 7800_gl41_yyyymmdd.csv (DuLieuMau folder)
    /// Structure: NGAY_DL -> Business Columns (1-13) -> System Columns
    /// </summary>
    [Table("GL41")]
    public class GL41Entity : IEntity
    {
        // === NGAY_DL - FIRST COLUMN ===
        // NGAY_DL - Ngày dữ liệu (lấy từ filename dd/mm/yyyy) - ALWAYS FIRST
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS (13 columns from CSV - EXACT ORDER) ===

        // Column 1: MA_CN - Mã chi nhánh
        [StringLength(200)]
        public string? MA_CN { get; set; }

        // Column 2: LOAI_TIEN - Loại tiền
        [StringLength(200)]
        public string? LOAI_TIEN { get; set; }

        // Column 3: MA_TK - Mã tài khoản
        [StringLength(200)]
        public string? MA_TK { get; set; }

        // Column 4: TEN_TK - Tên tài khoản
        [StringLength(200)]
        public string? TEN_TK { get; set; }

        // Column 5: LOAI_BT - Loại bút toán
        [StringLength(200)]
        public string? LOAI_BT { get; set; }

        // Column 6: DN_DAUKY - Dư nợ đầu kỳ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DN_DAUKY { get; set; }

        // Column 7: DC_DAUKY - Dư có đầu kỳ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DC_DAUKY { get; set; }

        // Column 8: SBT_NO - Số bút toán nợ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SBT_NO { get; set; }

        // Column 9: ST_GHINO - Số tiền ghi nợ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ST_GHINO { get; set; }

        // Column 10: SBT_CO - Số bút toán có
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SBT_CO { get; set; }

        // Column 11: ST_GHICO - Số tiền ghi có
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ST_GHICO { get; set; }

        // Column 12: DN_CUOIKY - Dư nợ cuối kỳ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DN_CUOIKY { get; set; }

        // Column 13: DC_CUOIKY - Dư có cuối kỳ
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DC_CUOIKY { get; set; }

        // === SYSTEM COLUMNS (cuối cùng) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // NO TEMPORAL COLUMNS - GL41 is Partitioned Columnstore only
    }
}
