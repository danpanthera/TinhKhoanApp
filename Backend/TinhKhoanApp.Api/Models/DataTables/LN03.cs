using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// LN03 - Bad Debt Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> 20 Business Columns (17 with headers + 3 without) -> Temporal + System Columns
    /// Requirements: 20 business columns, datetime2 for dates, decimal(18,2) for amounts
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        // System Column - NGAY_DL first (extracted from filename)
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // ===== 20 BUSINESS COLUMNS (17 with headers + 3 without) =====

        // Columns 1-17: With headers from CSV
        [Column("MACHINHANH", Order = 1)]
        [StringLength(200)]
        public string MACHINHANH { get; set; } = "";

        [Column("TENCHINHANH", Order = 2)]
        [StringLength(200)]
        public string TENCHINHANH { get; set; } = "";

        [Column("MAKH", Order = 3)]
        [StringLength(200)]
        public string MAKH { get; set; } = "";

        [Column("TENKH", Order = 4)]
        [StringLength(200)]
        public string TENKH { get; set; } = "";

        [Column("SOHOPDONG", Order = 5)]
        [StringLength(200)]
        public string SOHOPDONG { get; set; } = "";

        // SOTIENXLRR contains "SOTIEN" -> decimal format
        [Column("SOTIENXLRR", Order = 6)]
        public decimal? SOTIENXLRR { get; set; }

        // NGAYPHATSINHXL contains "NGAY" -> datetime2 format
        [Column("NGAYPHATSINHXL", Order = 7)]
        public DateTime? NGAYPHATSINHXL { get; set; }

        // THUNOSAUXL contains "THUNO" -> decimal format
        [Column("THUNOSAUXL", Order = 8)]
        public decimal? THUNOSAUXL { get; set; }

        // CONLAINGOAIBANG - string column
        [Column("CONLAINGOAIBANG", Order = 9)]
        [StringLength(200)]
        public string CONLAINGOAIBANG { get; set; } = "";

        // DUNONOIBANG contains "DUNO" -> decimal format
        [Column("DUNONOIBANG", Order = 10)]
        public decimal? DUNONOIBANG { get; set; }

        [Column("NHOMNO", Order = 11)]
        [StringLength(200)]
        public string NHOMNO { get; set; } = "";

        [Column("MACBTD", Order = 12)]
        [StringLength(200)]
        public string MACBTD { get; set; } = "";

        [Column("TENCBTD", Order = 13)]
        [StringLength(200)]
        public string TENCBTD { get; set; } = "";

        [Column("MAPGD", Order = 14)]
        [StringLength(200)]
        public string MAPGD { get; set; } = "";

        [Column("TAIKHOANHACHTOAN", Order = 15)]
        [StringLength(200)]
        public string TAIKHOANHACHTOAN { get; set; } = "";

        [Column("REFNO", Order = 16)]
        [StringLength(200)]
        public string REFNO { get; set; } = "";

        [Column("LOAINGUONVON", Order = 17)]
        [StringLength(200)]
        public string LOAINGUONVON { get; set; } = "";

        // Columns 18-20: Without headers but with data
        [Column("COLUMN_18", Order = 18)]
        [StringLength(200)]
        public string COLUMN_18 { get; set; } = "";

        [Column("COLUMN_19", Order = 19)]
        [StringLength(200)]
        public string COLUMN_19 { get; set; } = "";

        // Column 20: No header, contains amount data -> decimal format
        [Column("COLUMN_20", Order = 20)]
        public decimal? COLUMN_20 { get; set; }

        // ===== SYSTEM COLUMNS =====
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", Order = 21)]
        public int Id { get; set; }

        [Column("FILE_NAME", Order = 22)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";

        [Column("CREATED_DATE", Order = 23)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("CREATED_BY", Order = 24)]
        [StringLength(200)]
        public string CREATED_BY { get; set; } = "";

        [Column("UPDATED_DATE", Order = 25)]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("UPDATED_BY", Order = 26)]
        [StringLength(200)]
        public string UPDATED_BY { get; set; } = "";

        [Column("IS_ACTIVE", Order = 27)]
        public bool IS_ACTIVE { get; set; } = true;

        [Column("NOTES", Order = 28)]
        [StringLength(1000)]
        public string NOTES { get; set; } = "";

        // ===== TEMPORAL TABLE COLUMNS (Shadow Properties - managed by EF Core) =====
        // ValidFrom và ValidTo sẽ được tự động quản lý bởi EF Core như shadow properties
        // Không cần khai báo explicit properties
    }
}
