using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// GL41 - Bảng dữ liệu Trial Balance với 13 cột business từ CSV
    /// TEMPORAL TABLE: System-versioned temporal table với Columnstore Indexes
    /// Cấu trúc: NGAY_DL -> 13 Business Columns (theo thứ tự CSV) -> Temporal System Columns
    /// Import policy: Chỉ cho phép files có chứa "gl41" trong filename
    /// Đặc biệt: NGAY_DL lấy từ filename extraction (không có trong CSV)
    /// Cột amount: "DAUKY", "CUOIKY", "SBT", "ST", "GHINO", "GHICO" -> decimal(18,2) format #,###.00
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        // NGAY_DL - DateTime từ filename extraction (Order 0)
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // 13 Business Columns - Theo thứ tự CSV chính xác với kiểu dữ liệu phù hợp
        [Column("MA_CN", Order = 1)]
        [StringLength(200)]
        public string? MA_CN { get; set; }

        [Column("LOAI_TIEN", Order = 2)]
        [StringLength(200)]
        public string? LOAI_TIEN { get; set; }

        [Column("MA_TK", Order = 3)]
        [StringLength(200)]
        public string? MA_TK { get; set; }

        [Column("TEN_TK", Order = 4)]
        [StringLength(200)]
        public string? TEN_TK { get; set; }

        [Column("LOAI_BT", Order = 5)]
        [StringLength(200)]
        public string? LOAI_BT { get; set; }

        // Cột Financial Amount - decimal(18,2) format cho hiển thị #,###.00
        [Column("DN_DAUKY", Order = 6, TypeName = "decimal(18,2)")]
        public decimal? DN_DAUKY { get; set; }

        [Column("DC_DAUKY", Order = 7, TypeName = "decimal(18,2)")]
        public decimal? DC_DAUKY { get; set; }

        [Column("SBT_NO", Order = 8, TypeName = "decimal(18,2)")]
        public decimal? SBT_NO { get; set; }

        [Column("ST_GHINO", Order = 9, TypeName = "decimal(18,2)")]
        public decimal? ST_GHINO { get; set; }

        [Column("SBT_CO", Order = 10, TypeName = "decimal(18,2)")]
        public decimal? SBT_CO { get; set; }

        [Column("ST_GHICO", Order = 11, TypeName = "decimal(18,2)")]
        public decimal? ST_GHICO { get; set; }

        [Column("DN_CUOIKY", Order = 12, TypeName = "decimal(18,2)")]
        public decimal? DN_CUOIKY { get; set; }

        [Column("DC_CUOIKY", Order = 13, TypeName = "decimal(18,2)")]
        public decimal? DC_CUOIKY { get; set; }

        // System Columns - Temporal table sẽ tự động thêm ValidFrom, ValidTo
        [Column("FILE_NAME", Order = 14)]
        [StringLength(500)]
        public string? FILE_NAME { get; set; }

        [Column("CREATED_DATE", Order = 15)]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("BATCH_ID", Order = 16)]
        [StringLength(100)]
        public string? BATCH_ID { get; set; }

        [Column("IMPORT_SESSION_ID", Order = 17)]
        [StringLength(100)]
        public string? IMPORT_SESSION_ID { get; set; }

        // Primary Key - Composite key cho performance tốt
        [Key]
        [Column("ID", Order = 18)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
    }
}
