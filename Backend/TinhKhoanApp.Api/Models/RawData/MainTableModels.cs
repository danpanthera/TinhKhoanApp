using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.RawData
{


    // =======================================
    // 📊 GAHR26 - Dữ liệu nhân viên Model (Main Table)
    // =======================================
    [Table("GAHR26")]
    public class GAHR26 : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields - Giữ nguyên tên cột CSV gốc
        [StringLength(50)]
        [Column("EMP_ID")]
        public string? EMP_ID { get; set; }

        [StringLength(500)]
        [Column("EMP_NAME")]
        public string? EMP_NAME { get; set; }

        [StringLength(20)]
        [Column("ID_NUMBER")]
        public string? ID_NUMBER { get; set; }

        [StringLength(100)]
        [Column("POSITION")]
        public string? POSITION { get; set; }

        [StringLength(20)]
        [Column("BRCD")]
        public string? BRCD { get; set; }

        [StringLength(200)]
        [Column("BRANCH_NAME")]
        public string? BRANCH_NAME { get; set; }

        [StringLength(100)]
        [Column("DEPARTMENT")]
        public string? DEPARTMENT { get; set; }

        [Column("JOIN_DATE")]
        public DateTime? JOIN_DATE { get; set; }

        [Column("BIRTH_DATE")]
        public DateTime? BIRTH_DATE { get; set; }

        [StringLength(10)]
        [Column("GENDER")]
        public string? GENDER { get; set; }

        [StringLength(500)]
        [Column("ADDRESS")]
        public string? ADDRESS { get; set; }

        [StringLength(20)]
        [Column("PHONE")]
        public string? PHONE { get; set; }

        [StringLength(100)]
        [Column("EMAIL")]
        public string? EMAIL { get; set; }

        [StringLength(50)]
        [Column("STATUS")]
        public string? STATUS { get; set; }

        [Column("BASIC_SALARY", TypeName = "decimal(18,2)")]
        public decimal? BASIC_SALARY { get; set; }

        [Column("ALLOWANCE", TypeName = "decimal(18,2)")]
        public decimal? ALLOWANCE { get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CREATED_DATE { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================
    // 📊 GL41 - Dữ liệu sổ cái chi tiết Model (Main Table)
    // =======================================
    [Table("GL41")]
    public class GL41 : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields - Giữ nguyên tên cột CSV gốc
        [StringLength(50)]
        [Column("JOURNAL_NO")]
        public string? JOURNAL_NO { get; set; }

        [StringLength(50)]
        [Column("ACCOUNT_NO")]
        public string? ACCOUNT_NO { get; set; }

        [StringLength(500)]
        [Column("ACCOUNT_NAME")]
        public string? ACCOUNT_NAME { get; set; }

        [StringLength(50)]
        [Column("CUSTOMER_ID")]
        public string? CUSTOMER_ID { get; set; }

        [StringLength(500)]
        [Column("CUSTOMER_NAME")]
        public string? CUSTOMER_NAME { get; set; }

        [Column("TRANSACTION_DATE")]
        public DateTime? TRANSACTION_DATE { get; set; }

        [Column("POSTING_DATE")]
        public DateTime? POSTING_DATE { get; set; }

        [StringLength(1000)]
        [Column("DESCRIPTION")]
        public string? DESCRIPTION { get; set; }

        [Column("DEBIT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? DEBIT_AMOUNT { get; set; }

        [Column("CREDIT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? CREDIT_AMOUNT { get; set; }

        [Column("DEBIT_BALANCE", TypeName = "decimal(18,2)")]
        public decimal? DEBIT_BALANCE { get; set; }

        [Column("CREDIT_BALANCE", TypeName = "decimal(18,2)")]
        public decimal? CREDIT_BALANCE { get; set; }

        [StringLength(20)]
        [Column("BRCD")]
        public string? BRCD { get; set; }

        [StringLength(200)]
        [Column("BRANCH_NAME")]
        public string? BRANCH_NAME { get; set; }

        [StringLength(100)]
        [Column("TRANSACTION_TYPE")]
        public string? TRANSACTION_TYPE { get; set; }

        [StringLength(50)]
        [Column("ORIGINAL_TRANS_ID")]
        public string? ORIGINAL_TRANS_ID { get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CREATED_DATE { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }
}
