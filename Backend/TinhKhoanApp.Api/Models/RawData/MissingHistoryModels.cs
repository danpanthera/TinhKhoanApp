using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.RawData
{
    // =======================================
    // 📊 7800_DT_KHKD1 - Báo cáo kế hoạch kinh doanh History Model
    // =======================================
    // =======================================
    // 📊 7800_DT_KHKD1 - Báo cáo kế hoạch kinh doanh History Model
    // =======================================
    [Table("7800_DT_KHKD1_History")]
    public class DT_KHKD1_History : IExtendedHistoryModel
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

        // 📊 Business Data Fields for 7800_DT_KHKD1 (Kế hoạch kinh doanh)
        // Giữ nguyên tên cột CSV gốc
        [StringLength(20)]
        [Column("BRCD")]
        public string? BRCD { get; set; }

        [StringLength(200)]
        [Column("BRANCH_NAME")]
        public string? BRANCH_NAME { get; set; }

        [StringLength(100)]
        [Column("INDICATOR_TYPE")]
        public string? INDICATOR_TYPE { get; set; }

        [StringLength(500)]
        [Column("INDICATOR_NAME")]
        public string? INDICATOR_NAME { get; set; }

        [Column("PLAN_YEAR", TypeName = "decimal(18,2)")]
        public decimal? PLAN_YEAR { get; set; }

        [Column("PLAN_QUARTER", TypeName = "decimal(18,2)")]
        public decimal? PLAN_QUARTER { get; set; }

        [Column("PLAN_MONTH", TypeName = "decimal(18,2)")]
        public decimal? PLAN_MONTH { get; set; }

        [Column("ACTUAL_YEAR", TypeName = "decimal(18,2)")]
        public decimal? ACTUAL_YEAR { get; set; }

        [Column("ACTUAL_QUARTER", TypeName = "decimal(18,2)")]
        public decimal? ACTUAL_QUARTER { get; set; }

        [Column("ACTUAL_MONTH", TypeName = "decimal(18,2)")]
        public decimal? ACTUAL_MONTH { get; set; }

        [Column("ACHIEVEMENT_RATE", TypeName = "decimal(10,4)")]
        public decimal? ACHIEVEMENT_RATE { get; set; }

        [Column("YEAR")]
        public int? YEAR { get; set; }

        [Column("QUARTER")]
        public int? QUARTER { get; set; }

        [Column("MONTH")]
        public int? MONTH { get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CREATED_DATE { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        // 📝 Additional JSON field for storing complete raw data
        public string? RawDataJson { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================
    // 📊 GAHR26 - Dữ liệu nhân viên History Model
    // =======================================
    [Table("GAHR26_History")]
    public class GAHR26_History : IExtendedHistoryModel
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

        // 📊 Business Data Fields for GAHR26 (Nhân viên)
        // Giữ nguyên tên cột CSV gốc
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
    // 📊 GL41 - Dữ liệu sổ cái chi tiết History Model
    // =======================================
    [Table("GL41_History")]
    public class GL41_History : IExtendedHistoryModel
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

        // 📊 Business Data Fields for GLCB41 (Sổ cái chi tiết)
        // Giữ nguyên tên cột CSV gốc
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

        // 📝 Additional JSON field for storing complete raw data
        public string? RawDataJson { get; set; }

        // 📝 Additional field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================
    // 📊 LN01 - Loan Data History Model
    // =======================================
    [Table("LN01_CsvHistory")]
    public class LN01_History : IExtendedHistoryModel
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

        // 📊 Business Data Fields for LN01 (Loan data)
        // Giữ nguyên tên cột CSV gốc
        [StringLength(20)]
        [Column("BRCD")]
        public string? BRCD { get; set; }

        [StringLength(50)]
        [Column("CUSTSEQ")]
        public string? CUSTSEQ { get; set; }

        [StringLength(200)]
        [Column("CUSTNM")]
        public string? CUSTNM { get; set; }

        [StringLength(50)]
        [Column("TAI_KHOAN")]
        public string? TAI_KHOAN { get; set; }

        [StringLength(3)]
        [Column("CCY")]
        public string? CCY { get; set; }

        [Column("DU_NO", TypeName = "decimal(18,2)")]
        public decimal? DU_NO { get; set; }

        [StringLength(50)]
        [Column("DSBSSEQ")]
        public string? DSBSSEQ { get; set; }

        [Column("TRANSACTION_DATE")]
        public DateTime? TRANSACTION_DATE { get; set; }

        [Column("DSBSDT")]
        public DateTime? DSBSDT { get; set; }

        [StringLength(3)]
        [Column("DISBUR_CCY")]
        public string? DISBUR_CCY { get; set; }

        [Column("DISBURSEMENT_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? RawDataJson { get; set; }

        // 📝 Additional field for storing raw data
        public string? AdditionalData { get; set; }
    }
}
