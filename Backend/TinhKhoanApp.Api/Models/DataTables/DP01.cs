using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    [Table("DP01")]
    public class DP01
    {
        // System Columns
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // NGAY_DL Column (from filename)
        [Column("NGAY_DL", TypeName = "datetime2")]
        public DateTime? NGAY_DL { get; set; }

        // 63 Business Columns - EXACT from CSV
        [Column("MA_CN")]
        [StringLength(200)]
        public string? MA_CN { get; set; }

        [Column("TAI_KHOAN_HACH_TOAN")]
        [StringLength(200)]
        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        [Column("MA_KH")]
        [StringLength(200)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(200)]
        public string? TEN_KH { get; set; }

        [Column("DP_TYPE_NAME")]
        [StringLength(200)]
        public string? DP_TYPE_NAME { get; set; }

        [Column("CCY")]
        [StringLength(200)]
        public string? CCY { get; set; }

        [Column("CURRENT_BALANCE", TypeName = "decimal(18,2)")]
        public decimal? CURRENT_BALANCE { get; set; }

        [Column("RATE", TypeName = "decimal(18,6)")]
        public decimal? RATE { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(200)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("OPENING_DATE", TypeName = "datetime2")]
        public DateTime? OPENING_DATE { get; set; }

        [Column("MATURITY_DATE", TypeName = "datetime2")]
        public DateTime? MATURITY_DATE { get; set; }

        [Column("ADDRESS")]
        [StringLength(1000)] // Special case: 1000 chars
        public string? ADDRESS { get; set; }

        [Column("NOTENO")]
        [StringLength(200)]
        public string? NOTENO { get; set; }

        [Column("MONTH_TERM")]
        [StringLength(200)]
        public string? MONTH_TERM { get; set; }

        [Column("TERM_DP_NAME")]
        [StringLength(200)]
        public string? TERM_DP_NAME { get; set; }

        [Column("TIME_DP_NAME")]
        [StringLength(200)]
        public string? TIME_DP_NAME { get; set; }

        [Column("MA_PGD")]
        [StringLength(200)]
        public string? MA_PGD { get; set; }

        [Column("TEN_PGD")]
        [StringLength(200)]
        public string? TEN_PGD { get; set; }

        [Column("DP_TYPE_CODE")]
        [StringLength(200)]
        public string? DP_TYPE_CODE { get; set; }

        [Column("RENEW_DATE", TypeName = "datetime2")]
        public DateTime? RENEW_DATE { get; set; }

        [Column("CUST_TYPE")]
        [StringLength(200)]
        public string? CUST_TYPE { get; set; }

        [Column("CUST_TYPE_NAME")]
        [StringLength(200)]
        public string? CUST_TYPE_NAME { get; set; }

        [Column("CUST_TYPE_DETAIL")]
        [StringLength(200)]
        public string? CUST_TYPE_DETAIL { get; set; }

        [Column("CUST_DETAIL_NAME")]
        [StringLength(200)]
        public string? CUST_DETAIL_NAME { get; set; }

        [Column("PREVIOUS_DP_CAP_DATE", TypeName = "datetime2")]
        public DateTime? PREVIOUS_DP_CAP_DATE { get; set; }

        [Column("NEXT_DP_CAP_DATE", TypeName = "datetime2")]
        public DateTime? NEXT_DP_CAP_DATE { get; set; }

        [Column("ID_NUMBER")]
        [StringLength(200)]
        public string? ID_NUMBER { get; set; }

        [Column("ISSUED_BY")]
        [StringLength(200)]
        public string? ISSUED_BY { get; set; }

        [Column("ISSUE_DATE", TypeName = "datetime2")]
        public DateTime? ISSUE_DATE { get; set; }

        [Column("SEX_TYPE")]
        [StringLength(200)]
        public string? SEX_TYPE { get; set; }

        [Column("BIRTH_DATE", TypeName = "datetime2")]
        public DateTime? BIRTH_DATE { get; set; }

        [Column("TELEPHONE")]
        [StringLength(200)]
        public string? TELEPHONE { get; set; }

        [Column("ACRUAL_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal? ACRUAL_AMOUNT { get; set; }

        [Column("ACRUAL_AMOUNT_END", TypeName = "decimal(18,2)")]
        public decimal? ACRUAL_AMOUNT_END { get; set; }

        [Column("ACCOUNT_STATUS")]
        [StringLength(200)]
        public string? ACCOUNT_STATUS { get; set; }

        [Column("DRAMT", TypeName = "decimal(18,2)")]
        public decimal? DRAMT { get; set; }

        [Column("CRAMT", TypeName = "decimal(18,2)")]
        public decimal? CRAMT { get; set; }

        [Column("EMPLOYEE_NUMBER")]
        [StringLength(200)]
        public string? EMPLOYEE_NUMBER { get; set; }

        [Column("EMPLOYEE_NAME")]
        [StringLength(200)]
        public string? EMPLOYEE_NAME { get; set; }

        [Column("SPECIAL_RATE", TypeName = "decimal(18,6)")]
        public decimal? SPECIAL_RATE { get; set; }

        [Column("AUTO_RENEWAL")]
        [StringLength(200)]
        public string? AUTO_RENEWAL { get; set; }

        [Column("CLOSE_DATE", TypeName = "datetime2")]
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
        [StringLength(200)]
        public string? TERM_DP_TYPE { get; set; }

        [Column("TIME_DP_TYPE")]
        [StringLength(200)]
        public string? TIME_DP_TYPE { get; set; }

        [Column("STATES_CODE")]
        [StringLength(200)]
        public string? STATES_CODE { get; set; }

        [Column("ZIP_CODE")]
        [StringLength(200)]
        public string? ZIP_CODE { get; set; }

        [Column("COUNTRY_CODE")]
        [StringLength(200)]
        public string? COUNTRY_CODE { get; set; }

        [Column("TAX_CODE_LOCATION")]
        [StringLength(200)]
        public string? TAX_CODE_LOCATION { get; set; }

        [Column("MA_CAN_BO_PT")]
        [StringLength(200)]
        public string? MA_CAN_BO_PT { get; set; }

        [Column("TEN_CAN_BO_PT")]
        [StringLength(200)]
        public string? TEN_CAN_BO_PT { get; set; }

        [Column("PHONG_CAN_BO_PT")]
        [StringLength(200)]
        public string? PHONG_CAN_BO_PT { get; set; }

        [Column("NGUOI_NUOC_NGOAI")]
        [StringLength(200)]
        public string? NGUOI_NUOC_NGOAI { get; set; }

        [Column("QUOC_TICH")]
        [StringLength(200)]
        public string? QUOC_TICH { get; set; }

        [Column("MA_CAN_BO_AGRIBANK")]
        [StringLength(200)]
        public string? MA_CAN_BO_AGRIBANK { get; set; }

        [Column("NGUOI_GIOI_THIEU")]
        [StringLength(200)]
        public string? NGUOI_GIOI_THIEU { get; set; }

        [Column("TEN_NGUOI_GIOI_THIEU")]
        [StringLength(200)]
        public string? TEN_NGUOI_GIOI_THIEU { get; set; }

        [Column("CONTRACT_COUTS_DAY")]
        [StringLength(200)]
        public string? CONTRACT_COUTS_DAY { get; set; }

        [Column("SO_KY_AD_LSDB")]
        [StringLength(200)]
        public string? SO_KY_AD_LSDB { get; set; }

        [Column("UNTBUSCD")]
        [StringLength(200)]
        public string? UNTBUSCD { get; set; }

        [Column("TYGIA", TypeName = "decimal(18,6)")]
        public decimal? TYGIA { get; set; }

        // System Audit Fields
        [Column("FILE_NAME")]
        [StringLength(500)]
        public string? FILE_NAME { get; set; }

        [Column("DataSource")]
        [StringLength(500)]
        public string? DataSource { get; set; }

        [Column("ImportDateTime", TypeName = "datetime2")]
        public DateTime ImportDateTime { get; set; }

        [Column("CreatedAt", TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt", TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        [Column("CreatedBy")]
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column("UpdatedBy")]
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        // Temporal Table Columns
        [Column("SysStartTime", TypeName = "datetime2")]
        public DateTime SysStartTime { get; set; }

        [Column("SysEndTime", TypeName = "datetime2")]
        public DateTime SysEndTime { get; set; }
    }
}
