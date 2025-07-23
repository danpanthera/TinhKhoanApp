using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// DP01 - Deposit Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("DP01")]
    public class DP01
    {
        // NGAY_DL - DateTime from filename (Order 0)
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("MA_CN", Order = 1)]
        [StringLength(50)]
        public string MA_CN { get; set; } = "";

        [Column("TAI_KHOAN_HACH_TOAN", Order = 2)]
        [StringLength(50)]
        public string TAI_KHOAN_HACH_TOAN { get; set; } = "";

        [Column("MA_KH", Order = 3)]
        [StringLength(50)]
        public string MA_KH { get; set; } = "";

        [Column("TEN_KH", Order = 4)]
        [StringLength(50)]
        public string TEN_KH { get; set; } = "";

        [Column("DP_TYPE_NAME", Order = 5)]
        [StringLength(50)]
        public string DP_TYPE_NAME { get; set; } = "";

        [Column("CCY", Order = 6)]
        [StringLength(50)]
        public string CCY { get; set; } = "";

        [Column("CURRENT_BALANCE", Order = 7)]
        [StringLength(50)]
        public string CURRENT_BALANCE { get; set; } = "";

        [Column("RATE", Order = 8)]
        [StringLength(50)]
        public string RATE { get; set; } = "";

        [Column("SO_TAI_KHOAN", Order = 9)]
        [StringLength(50)]
        public string SO_TAI_KHOAN { get; set; } = "";

        [Column("OPENING_DATE", Order = 10)]
        [StringLength(50)]
        public string OPENING_DATE { get; set; } = "";

        [Column("MATURITY_DATE", Order = 11)]
        [StringLength(50)]
        public string MATURITY_DATE { get; set; } = "";

        [Column("ADDRESS", Order = 12)]
        [StringLength(50)]
        public string ADDRESS { get; set; } = "";

        [Column("NOTENO", Order = 13)]
        [StringLength(50)]
        public string NOTENO { get; set; } = "";

        [Column("MONTH_TERM", Order = 14)]
        [StringLength(50)]
        public string MONTH_TERM { get; set; } = "";

        [Column("TERM_DP_NAME", Order = 15)]
        [StringLength(50)]
        public string TERM_DP_NAME { get; set; } = "";

        [Column("TIME_DP_NAME", Order = 16)]
        [StringLength(50)]
        public string TIME_DP_NAME { get; set; } = "";

        [Column("MA_PGD", Order = 17)]
        [StringLength(50)]
        public string MA_PGD { get; set; } = "";

        [Column("TEN_PGD", Order = 18)]
        [StringLength(50)]
        public string TEN_PGD { get; set; } = "";

        [Column("DP_TYPE_CODE", Order = 19)]
        [StringLength(50)]
        public string DP_TYPE_CODE { get; set; } = "";

        [Column("RENEW_DATE", Order = 20)]
        [StringLength(50)]
        public string RENEW_DATE { get; set; } = "";

        [Column("CUST_TYPE", Order = 21)]
        [StringLength(50)]
        public string CUST_TYPE { get; set; } = "";

        [Column("CUST_TYPE_NAME", Order = 22)]
        [StringLength(50)]
        public string CUST_TYPE_NAME { get; set; } = "";

        [Column("CUST_TYPE_DETAIL", Order = 23)]
        [StringLength(50)]
        public string CUST_TYPE_DETAIL { get; set; } = "";

        [Column("CUST_DETAIL_NAME", Order = 24)]
        [StringLength(50)]
        public string CUST_DETAIL_NAME { get; set; } = "";

        [Column("PREVIOUS_DP_CAP_DATE", Order = 25)]
        [StringLength(50)]
        public string PREVIOUS_DP_CAP_DATE { get; set; } = "";

        [Column("NEXT_DP_CAP_DATE", Order = 26)]
        [StringLength(50)]
        public string NEXT_DP_CAP_DATE { get; set; } = "";

        [Column("ID_NUMBER", Order = 27)]
        [StringLength(50)]
        public string ID_NUMBER { get; set; } = "";

        [Column("ISSUED_BY", Order = 28)]
        [StringLength(50)]
        public string ISSUED_BY { get; set; } = "";

        [Column("ISSUE_DATE", Order = 29)]
        [StringLength(50)]
        public string ISSUE_DATE { get; set; } = "";

        [Column("SEX_TYPE", Order = 30)]
        [StringLength(50)]
        public string SEX_TYPE { get; set; } = "";

        [Column("BIRTH_DATE", Order = 31)]
        [StringLength(50)]
        public string BIRTH_DATE { get; set; } = "";

        [Column("TELEPHONE", Order = 32)]
        [StringLength(50)]
        public string TELEPHONE { get; set; } = "";

        [Column("ACRUAL_AMOUNT", Order = 33)]
        [StringLength(50)]
        public string ACRUAL_AMOUNT { get; set; } = "";

        [Column("ACRUAL_AMOUNT_END", Order = 34)]
        [StringLength(50)]
        public string ACRUAL_AMOUNT_END { get; set; } = "";

        [Column("ACCOUNT_STATUS", Order = 35)]
        [StringLength(50)]
        public string ACCOUNT_STATUS { get; set; } = "";

        [Column("DRAMT", Order = 36)]
        [StringLength(50)]
        public string DRAMT { get; set; } = "";

        [Column("CRAMT", Order = 37)]
        [StringLength(50)]
        public string CRAMT { get; set; } = "";

        [Column("EMPLOYEE_NUMBER", Order = 38)]
        [StringLength(50)]
        public string EMPLOYEE_NUMBER { get; set; } = "";

        [Column("EMPLOYEE_NAME", Order = 39)]
        [StringLength(50)]
        public string EMPLOYEE_NAME { get; set; } = "";

        [Column("SPECIAL_RATE", Order = 40)]
        [StringLength(50)]
        public string SPECIAL_RATE { get; set; } = "";

        [Column("AUTO_RENEWAL", Order = 41)]
        [StringLength(50)]
        public string AUTO_RENEWAL { get; set; } = "";

        [Column("CLOSE_DATE", Order = 42)]
        [StringLength(50)]
        public string CLOSE_DATE { get; set; } = "";

        [Column("LOCAL_PROVIN_NAME", Order = 43)]
        [StringLength(50)]
        public string LOCAL_PROVIN_NAME { get; set; } = "";

        [Column("LOCAL_DISTRICT_NAME", Order = 44)]
        [StringLength(50)]
        public string LOCAL_DISTRICT_NAME { get; set; } = "";

        [Column("LOCAL_WARD_NAME", Order = 45)]
        [StringLength(50)]
        public string LOCAL_WARD_NAME { get; set; } = "";

        [Column("TERM_DP_TYPE", Order = 46)]
        [StringLength(50)]
        public string TERM_DP_TYPE { get; set; } = "";

        [Column("TIME_DP_TYPE", Order = 47)]
        [StringLength(50)]
        public string TIME_DP_TYPE { get; set; } = "";

        [Column("STATES_CODE", Order = 48)]
        [StringLength(50)]
        public string STATES_CODE { get; set; } = "";

        [Column("ZIP_CODE", Order = 49)]
        [StringLength(50)]
        public string ZIP_CODE { get; set; } = "";

        [Column("COUNTRY_CODE", Order = 50)]
        [StringLength(50)]
        public string COUNTRY_CODE { get; set; } = "";

        [Column("TAX_CODE_LOCATION", Order = 51)]
        [StringLength(50)]
        public string TAX_CODE_LOCATION { get; set; } = "";

        [Column("MA_CAN_BO_PT", Order = 52)]
        [StringLength(50)]
        public string MA_CAN_BO_PT { get; set; } = "";

        [Column("TEN_CAN_BO_PT", Order = 53)]
        [StringLength(50)]
        public string TEN_CAN_BO_PT { get; set; } = "";

        [Column("PHONG_CAN_BO_PT", Order = 54)]
        [StringLength(50)]
        public string PHONG_CAN_BO_PT { get; set; } = "";

        [Column("NGUOI_NUOC_NGOAI", Order = 55)]
        [StringLength(50)]
        public string NGUOI_NUOC_NGOAI { get; set; } = "";

        [Column("QUOC_TICH", Order = 56)]
        [StringLength(50)]
        public string QUOC_TICH { get; set; } = "";

        [Column("MA_CAN_BO_AGRIBANK", Order = 57)]
        [StringLength(50)]
        public string MA_CAN_BO_AGRIBANK { get; set; } = "";

        [Column("NGUOI_GIOI_THIEU", Order = 58)]
        [StringLength(50)]
        public string NGUOI_GIOI_THIEU { get; set; } = "";

        [Column("TEN_NGUOI_GIOI_THIEU", Order = 59)]
        [StringLength(50)]
        public string TEN_NGUOI_GIOI_THIEU { get; set; } = "";

        [Column("CONTRACT_COUTS_DAY", Order = 60)]
        [StringLength(50)]
        public string CONTRACT_COUTS_DAY { get; set; } = "";

        [Column("SO_KY_AD_LSDB", Order = 61)]
        [StringLength(50)]
        public string SO_KY_AD_LSDB { get; set; } = "";

        [Column("UNTBUSCD", Order = 62)]
        [StringLength(50)]
        public string UNTBUSCD { get; set; } = "";

        [Column("TYGIA", Order = 63)]
        [StringLength(50)]
        public string TYGIA { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 64)]
        public long Id { get; set; }

        [Column("CREATED_DATE", Order = 65)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 66)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 67)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";

        // Temporal columns are shadow properties managed by EF Core automatically
        // ValidFrom/ValidTo removed - managed as shadow properties by ApplicationDbContext
    }
}
