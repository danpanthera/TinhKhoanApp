using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// GL41 - General Ledger Balance Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        // NGAY_DL - DateTime from filename (Order 0)
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("MA_CN", Order = 1)]
        [StringLength(200)]
        public string MA_CN { get; set; } = "";

        [Column("LOAI_TIEN", Order = 2)]
        [StringLength(200)]
        public string LOAI_TIEN { get; set; } = "";

        [Column("MA_TK", Order = 3)]
        [StringLength(200)]
        public string MA_TK { get; set; } = "";

        [Column("TEN_TK", Order = 4)]
        [StringLength(200)]
        public string TEN_TK { get; set; } = "";

        [Column("LOAI_BT", Order = 5)]
        [StringLength(200)]
        public string LOAI_BT { get; set; } = "";

        [Column("DN_DAUKY", Order = 6)]
        [StringLength(200)]
        public string DN_DAUKY { get; set; } = "";

        [Column("DC_DAUKY", Order = 7)]
        [StringLength(200)]
        public string DC_DAUKY { get; set; } = "";

        [Column("SBT_NO", Order = 8)]
        [StringLength(200)]
        public string SBT_NO { get; set; } = "";

        [Column("ST_GHINO", Order = 9)]
        [StringLength(200)]
        public string ST_GHINO { get; set; } = "";

        [Column("SBT_CO", Order = 10)]
        [StringLength(200)]
        public string SBT_CO { get; set; } = "";

        [Column("ST_GHICO", Order = 11)]
        [StringLength(200)]
        public string ST_GHICO { get; set; } = "";

        [Column("DN_CUOIKY", Order = 12)]
        [StringLength(200)]
        public string DN_CUOIKY { get; set; } = "";

        [Column("DC_CUOIKY", Order = 13)]
        [StringLength(200)]
        public string DC_CUOIKY { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 14)]
        public long Id { get; set; }

        // Temporal columns are shadow properties managed by EF Core automatically
        // ValidFrom/ValidTo removed - managed as shadow properties by ApplicationDbContext

        [Column("CREATED_DATE", Order = 15)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 16)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 19)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";
    }
}
