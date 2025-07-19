using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Model cho bảng DP01 - Dữ liệu Tiền gửi có kỳ hạn
    /// Business columns first (63 columns from CSV), then system columns, then temporal columns
    /// Temporal table with history tracking and columnstore index
    /// </summary>
    [Table("DP01")]
    public class DP01
    {
        // ======= BUSINESS COLUMNS (63 columns - exactly from CSV) =======
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("TAI_KHOAN_HACH_TOAN")]
        [StringLength(100)]
        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        [Column("MA_KH")]
        [StringLength(100)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(500)]
        public string? TEN_KH { get; set; }

        [Column("DP_TYPE_NAME")]
        [StringLength(200)]
        public string? DP_TYPE_NAME { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? CCY { get; set; }

        [Column("CURRENT_BALANCE", TypeName = "decimal(18,2)")]
        public decimal? CURRENT_BALANCE { get; set; }

        [Column("RATE", TypeName = "decimal(10,4)")]
        public decimal? RATE { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(100)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("OPENING_DATE")]
        public DateTime? OPENING_DATE { get; set; }

        [Column("MATURITY_DATE")]
        public DateTime? MATURITY_DATE { get; set; }

        [Column("ADDRESS")]
        [StringLength(1000)]
        public string? ADDRESS { get; set; }

        [Column("NOTENO")]
        [StringLength(100)]
        public string? NOTENO { get; set; }

        [Column("MONTH_TERM")]
        public int? MONTH_TERM { get; set; }

        [Column("TERM_DP_NAME")]
        [StringLength(200)]
        public string? TERM_DP_NAME { get; set; }

        [Column("TIME_DP_NAME")]
        [StringLength(200)]
        public string? TIME_DP_NAME { get; set; }

        [Column("MA_PGD")]
        [StringLength(100)]
        public string? MA_PGD { get; set; }

        [Column("TEN_PGD")]
        [StringLength(500)]
        public string? TEN_PGD { get; set; }

        [Column("DP_TYPE_CODE")]
        [StringLength(50)]
        public string? DP_TYPE_CODE { get; set; }

        [Column("RENEW_DATE")]
        public DateTime? RENEW_DATE { get; set; }

        [Column("CUST_TYPE")]
        [StringLength(50)]
        public string? CUST_TYPE { get; set; }

        [Column("CUST_TYPE_NAME")]
        [StringLength(200)]
        public string? CUST_TYPE_NAME { get; set; }

        [Column("CUST_TYPE_DETAIL")]
        [StringLength(50)]
        public string? CUST_TYPE_DETAIL { get; set; }

        [Column("CUST_DETAIL_NAME")]
        [StringLength(200)]
        public string? CUST_DETAIL_NAME { get; set; }

        [Column("PREVIOUS_DP_CAP_DATE")]
        public DateTime? PREVIOUS_DP_CAP_DATE { get; set; }

        [Column("NEXT_DP_CAP_DATE")]
        public DateTime? NEXT_DP_CAP_DATE { get; set; }

        [Column("ID_NUMBER")]
        [StringLength(100)]
        public string? ID_NUMBER { get; set; }

        [Column("ISSUED_BY")]
        [StringLength(200)]
        public string? ISSUED_BY { get; set; }

        [Column("ISSUE_DATE")]
        public DateTime? ISSUE_DATE { get; set; }

        [Column("SEX_TYPE")]
        [StringLength(10)]
        public string? SEX_TYPE { get; set; }

        [Column("BIRTH_DATE")]
        public DateTime? BIRTH_DATE { get; set; }

        [Column("TELEPHONE")]
        [StringLength(50)]
        public string? TELEPHONE { get; set; }

        [Column("ACRUAL_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? ACRUAL_AMOUNT { get; set; }

        [Column("ACRUAL_AMOUNT_END", TypeName = "decimal(18,2)")]
        public decimal? ACRUAL_AMOUNT_END { get; set; }

        [Column("ACCOUNT_STATUS")]
        [StringLength(50)]
        public string? ACCOUNT_STATUS { get; set; }

        [Column("DRAMT", TypeName = "decimal(18,2)")]
        public decimal? DRAMT { get; set; }

        [Column("CRAMT", TypeName = "decimal(18,2)")]
        public decimal? CRAMT { get; set; }

        [Column("EMPLOYEE_NUMBER")]
        [StringLength(100)]
        public string? EMPLOYEE_NUMBER { get; set; }

        [Column("EMPLOYEE_NAME")]
        [StringLength(500)]
        public string? EMPLOYEE_NAME { get; set; }

        [Column("SPECIAL_RATE", TypeName = "decimal(10,4)")]
        public decimal? SPECIAL_RATE { get; set; }

        [Column("AUTO_RENEWAL")]
        [StringLength(10)]
        public string? AUTO_RENEWAL { get; set; }

        [Column("CLOSE_DATE")]
        public DateTime? CLOSE_DATE { get; set; }

        [Column("LOCAL_PROVIN_NAME")]
        [StringLength(200)]
        public string? LOCAL_PROVIN_NAME { get; set; }

        [Column("LOCAL_DISTRICT_NAME")]
        [StringLength(200)]
        public string? LOCAL_DISTRICT_NAME { get; set; }

        [Column("LOCAL_WARD_NAME")]
        [StringLength(200)]
        public string? LOCAL_WARD_NAME { get; set; }

        [Column("TERM_DP_TYPE")]
        [StringLength(50)]
        public string? TERM_DP_TYPE { get; set; }

        [Column("TIME_DP_TYPE")]
        [StringLength(50)]
        public string? TIME_DP_TYPE { get; set; }

        [Column("STATES_CODE")]
        [StringLength(50)]
        public string? STATES_CODE { get; set; }

        [Column("ZIP_CODE")]
        [StringLength(20)]
        public string? ZIP_CODE { get; set; }

        [Column("COUNTRY_CODE")]
        [StringLength(10)]
        public string? COUNTRY_CODE { get; set; }

        [Column("TAX_CODE_LOCATION")]
        [StringLength(100)]
        public string? TAX_CODE_LOCATION { get; set; }

        [Column("MA_CAN_BO_PT")]
        [StringLength(100)]
        public string? MA_CAN_BO_PT { get; set; }

        [Column("TEN_CAN_BO_PT")]
        [StringLength(500)]
        public string? TEN_CAN_BO_PT { get; set; }

        [Column("PHONG_CAN_BO_PT")]
        [StringLength(200)]
        public string? PHONG_CAN_BO_PT { get; set; }

        [Column("NGUOI_NUOC_NGOAI")]
        [StringLength(10)]
        public string? NGUOI_NUOC_NGOAI { get; set; }

        [Column("QUOC_TICH")]
        [StringLength(100)]
        public string? QUOC_TICH { get; set; }

        [Column("MA_CAN_BO_AGRIBANK")]
        [StringLength(100)]
        public string? MA_CAN_BO_AGRIBANK { get; set; }

        [Column("NGUOI_GIOI_THIEU")]
        [StringLength(100)]
        public string? NGUOI_GIOI_THIEU { get; set; }

        [Column("TEN_NGUOI_GIOI_THIEU")]
        [StringLength(500)]
        public string? TEN_NGUOI_GIOI_THIEU { get; set; }

        [Column("CONTRACT_COUTS_DAY")]
        public int? CONTRACT_COUTS_DAY { get; set; }

        [Column("SO_KY_AD_LSDB")]
        [StringLength(100)]
        public string? SO_KY_AD_LSDB { get; set; }

        [Column("UNTBUSCD")]
        [StringLength(50)]
        public string? UNTBUSCD { get; set; }

        [Column("TYGIA", TypeName = "decimal(10,4)")]
        public decimal? TYGIA { get; set; }

        // ======= SYSTEM COLUMNS =======
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("NGAY_DL")]
        [Required]
        public DateTime NGAY_DL { get; set; }

        [Column("CreatedAt")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("IsDeleted")]
        [Required]
        public bool IsDeleted { get; set; } = false;

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // ======= TEMPORAL COLUMNS (managed by SQL Server) =======
        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }
    }
}
