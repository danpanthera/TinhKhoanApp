using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// GL02 - General Ledger Transactions Data Model with exact CSV column structure (17 business columns)
    /// SPECIAL: NGAY_DL comes from TRDATE column in CSV, partitioned columnstore (not temporal)
    /// Structure: NGAY_DL -> 17 Business Columns (CSV order) -> System Columns
    /// Import policy: Only files containing "gl02" in filename
    /// </summary>
    [Table("GL02")]
    public class GL02
    {
        // NGAY_DL - DateTime from TRDATE column (Order 0)
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // 17 Business Columns - Exact CSV order with proper data types
        [Column("TRBRCD", Order = 1)]
        [StringLength(200)]
        public string? TRBRCD { get; set; }

        [Column("USERID", Order = 2)]
        [StringLength(200)]
        public string? USERID { get; set; }

        [Column("JOURSEQ", Order = 3)]
        [StringLength(200)]
        public string? JOURSEQ { get; set; }

        [Column("DYTRSEQ", Order = 4)]
        [StringLength(200)]
        public string? DYTRSEQ { get; set; }

        [Column("LOCAC", Order = 5)]
        [StringLength(200)]
        public string? LOCAC { get; set; }

        [Column("CCY", Order = 6)]
        [StringLength(200)]
        public string? CCY { get; set; }

        [Column("BUSCD", Order = 7)]
        [StringLength(200)]
        public string? BUSCD { get; set; }

        [Column("UNIT", Order = 8)]
        [StringLength(200)]
        public string? UNIT { get; set; }

        [Column("TRCD", Order = 9)]
        [StringLength(200)]
        public string? TRCD { get; set; }

        [Column("CUSTOMER", Order = 10)]
        [StringLength(200)]
        public string? CUSTOMER { get; set; }

        [Column("TRTP", Order = 11)]
        [StringLength(200)]
        public string? TRTP { get; set; }

        [Column("REFERENCE", Order = 12)]
        [StringLength(200)]
        public string? REFERENCE { get; set; }

        [Column("REMARK", Order = 13)]
        [StringLength(1000)]
        public string? REMARK { get; set; }

        [Column("DRAMOUNT", Order = 14, TypeName = "decimal(18,2)")]
        public decimal? DRAMOUNT { get; set; }

        [Column("CRAMOUNT", Order = 15, TypeName = "decimal(18,2)")]
        public decimal? CRAMOUNT { get; set; }

        [Column("CRTDTM", Order = 16)]
        public DateTime? CRTDTM { get; set; }

        // System Columns - Primary Key (GL02 is Partitioned Columnstore, NOT Temporal)
        [Key]
        [Column("Id", Order = 17)]
        public long Id { get; set; }

        [Column("CREATED_DATE", Order = 18)]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE", Order = 19)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("FILE_NAME", Order = 20)]
        [StringLength(500)]
        public string? FILE_NAME { get; set; }

        // CSV Mapping Property - TRDATE from CSV converts to NGAY_DL
        [NotMapped]
        public string? TRDATE
        {
            get => NGAY_DL.ToString("yyyyMMdd");
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 8)
                {
                    if (DateTime.TryParseExact(value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        NGAY_DL = result;
                    }
                }
            }
        }
    }
}
