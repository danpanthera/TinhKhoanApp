using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// DP01 - Deposit Data Model with exact CSV column structure (63 business columns)
    /// Structure: NGAY_DL -> 63 Business Columns (CSV order) -> System + Temporal Columns
    /// Import policy: Only files containing "dp01" in filename
    /// </summary>
    [Table("DP01")]
    public class DP01
    {
        // System Column - NGAY_DL first (Order 0) - extracted from filename
        [Column("NGAY_DL", Order = 0)]
        public DateTime NGAY_DL { get; set; }

        // 63 Business Columns - Exact CSV order (basic string columns for now)
        [Column("MA_CN", Order = 1)]
        [StringLength(200)]
        public string MA_CN { get; set; } = "";

        [Column("TAI_KHOAN_HACH_TOAN", Order = 2)]
        [StringLength(200)]
        public string TAI_KHOAN_HACH_TOAN { get; set; } = "";

        [Column("MA_KH", Order = 3)]
        [StringLength(200)]
        public string MA_KH { get; set; } = "";

        [Column("TEN_KH", Order = 4)]
        [StringLength(200)]
        public string TEN_KH { get; set; } = "";

        [Column("DP_TYPE_NAME", Order = 5)]
        [StringLength(200)]
        public string DP_TYPE_NAME { get; set; } = "";

        [Column("CCY", Order = 6)]
        [StringLength(200)]
        public string CCY { get; set; } = "";

        [Column("CURRENT_BALANCE", Order = 7)]
        public decimal CURRENT_BALANCE { get; set; }

        [Column("RATE", Order = 8)]
        public decimal RATE { get; set; }

        [Column("SO_TAI_KHOAN", Order = 9)]
        [StringLength(200)]
        public string SO_TAI_KHOAN { get; set; } = "";

        [Column("OPENING_DATE", Order = 10)]
        public DateTime OPENING_DATE { get; set; }

        // ... Simplified for compilation - will expand later
        // Adding remaining columns as string for now

        [Column("MATURITY_DATE", Order = 11)]
        public DateTime MATURITY_DATE { get; set; }

        [Column("ADDRESS", Order = 12)]
        [StringLength(1000)]
        public string ADDRESS { get; set; } = "";

        [Column("NOTENO", Order = 13)]
        [StringLength(200)]
        public string NOTENO { get; set; } = "";

        [Column("MONTH_TERM", Order = 14)]
        [StringLength(200)]
        public string MONTH_TERM { get; set; } = "";

        [Column("TERM_DP_NAME", Order = 15)]
        [StringLength(200)]
        public string TERM_DP_NAME { get; set; } = "";

        [Column("TIME_DP_NAME", Order = 16)]
        [StringLength(200)]
        public string TIME_DP_NAME { get; set; } = "";

        [Column("MA_PGD", Order = 17)]
        [StringLength(200)]
        public string MA_PGD { get; set; } = "";

        [Column("TEN_PGD", Order = 18)]
        [StringLength(200)]
        public string TEN_PGD { get; set; } = "";

        [Column("DP_TYPE_CODE", Order = 19)]
        [StringLength(200)]
        public string DP_TYPE_CODE { get; set; } = "";

        [Column("RENEW_DATE", Order = 20)]
        public DateTime RENEW_DATE { get; set; }

        [Column("CUST_TYPE", Order = 21)]
        [StringLength(200)]
        public string CUST_TYPE { get; set; } = "";

        [Column("CUST_TYPE_NAME", Order = 22)]
        [StringLength(200)]
        public string CUST_TYPE_NAME { get; set; } = "";

        [Column("CUST_TYPE_DETAIL", Order = 23)]
        [StringLength(200)]
        public string CUST_TYPE_DETAIL { get; set; } = "";

        [Column("CUST_DETAIL_NAME", Order = 24)]
        [StringLength(200)]
        public string CUST_DETAIL_NAME { get; set; } = "";

        [Column("PREVIOUS_DP_CAP_DATE", Order = 25)]
        public DateTime PREVIOUS_DP_CAP_DATE { get; set; }

        [Column("NEXT_DP_CAP_DATE", Order = 26)]
        public DateTime NEXT_DP_CAP_DATE { get; set; }

        [Column("ID_NUMBER", Order = 27)]
        [StringLength(200)]
        public string ID_NUMBER { get; set; } = "";

        [Column("ISSUED_BY", Order = 28)]
        [StringLength(200)]
        public string ISSUED_BY { get; set; } = "";

        [Column("ISSUE_DATE", Order = 29)]
        public DateTime ISSUE_DATE { get; set; }

        [Column("SEX_TYPE", Order = 30)]
        [StringLength(200)]
        public string SEX_TYPE { get; set; } = "";

        [Column("BIRTH_DATE", Order = 31)]
        public DateTime BIRTH_DATE { get; set; }

        [Column("TELEPHONE", Order = 32)]
        [StringLength(200)]
        public string TELEPHONE { get; set; } = "";

        // Key properties that ApplicationDbContext expects
        [Column("ACRUAL_AMOUNT", Order = 33)]
        public decimal ACRUAL_AMOUNT { get; set; }

        [Column("ACRUAL_AMOUNT_END", Order = 34)]
        public decimal ACRUAL_AMOUNT_END { get; set; }

        [Column("ACCOUNT_STATUS", Order = 35)]
        [StringLength(200)]
        public string ACCOUNT_STATUS { get; set; } = "";

        [Column("DRAMT", Order = 36)]
        public decimal DRAMT { get; set; }

        [Column("CRAMT", Order = 37)]
        public decimal CRAMT { get; set; }

        [Column("EMPLOYEE_NUMBER", Order = 38)]
        [StringLength(200)]
        public string EMPLOYEE_NUMBER { get; set; } = "";

        [Column("EMPLOYEE_NAME", Order = 39)]
        [StringLength(200)]
        public string EMPLOYEE_NAME { get; set; } = "";

        [Column("SPECIAL_RATE", Order = 40)]
        public decimal SPECIAL_RATE { get; set; }

        [Column("AUTO_RENEWAL", Order = 41)]
        [StringLength(200)]
        public string AUTO_RENEWAL { get; set; } = "";

        [Column("CLOSE_DATE", Order = 42)]
        public DateTime CLOSE_DATE { get; set; }

        // Continue with remaining columns to complete 63 total
        [Column("LOCAL_PROVIN_NAME", Order = 43)]
        [StringLength(200)]
        public string LOCAL_PROVIN_NAME { get; set; } = "";

        [Column("LOCAL_DISTRICT_NAME", Order = 44)]
        [StringLength(200)]
        public string LOCAL_DISTRICT_NAME { get; set; } = "";

        [Column("LOCAL_WARD_NAME", Order = 45)]
        [StringLength(200)]
        public string LOCAL_WARD_NAME { get; set; } = "";

        [Column("TERM_DP_TYPE", Order = 46)]
        [StringLength(200)]
        public string TERM_DP_TYPE { get; set; } = "";

        [Column("TIME_DP_TYPE", Order = 47)]
        [StringLength(200)]
        public string TIME_DP_TYPE { get; set; } = "";

        [Column("STATES_CODE", Order = 48)]
        [StringLength(200)]
        public string STATES_CODE { get; set; } = "";

        [Column("ZIP_CODE", Order = 49)]
        [StringLength(200)]
        public string ZIP_CODE { get; set; } = "";

        [Column("COUNTRY_CODE", Order = 50)]
        [StringLength(200)]
        public string COUNTRY_CODE { get; set; } = "";

        [Column("TAX_CODE_LOCATION", Order = 51)]
        [StringLength(200)]
        public string TAX_CODE_LOCATION { get; set; } = "";

        [Column("MA_CAN_BO_PT", Order = 52)]
        [StringLength(200)]
        public string MA_CAN_BO_PT { get; set; } = "";

        [Column("TEN_CAN_BO_PT", Order = 53)]
        [StringLength(200)]
        public string TEN_CAN_BO_PT { get; set; } = "";

        [Column("PHONG_CAN_BO_PT", Order = 54)]
        [StringLength(200)]
        public string PHONG_CAN_BO_PT { get; set; } = "";

        [Column("NGUOI_NUOC_NGOAI", Order = 55)]
        [StringLength(200)]
        public string NGUOI_NUOC_NGOAI { get; set; } = "";

        [Column("QUOC_TICH", Order = 56)]
        [StringLength(200)]
        public string QUOC_TICH { get; set; } = "";

        [Column("MA_CAN_BO_AGRIBANK", Order = 57)]
        [StringLength(200)]
        public string MA_CAN_BO_AGRIBANK { get; set; } = "";

        [Column("NGUOI_GIOI_THIEU", Order = 58)]
        [StringLength(200)]
        public string NGUOI_GIOI_THIEU { get; set; } = "";

        [Column("TEN_NGUOI_GIOI_THIEU", Order = 59)]
        [StringLength(200)]
        public string TEN_NGUOI_GIOI_THIEU { get; set; } = "";

        [Column("CONTRACT_COUTS_DAY", Order = 60)]
        [StringLength(200)]
        public string CONTRACT_COUTS_DAY { get; set; } = "";

        [Column("SO_KY_AD_LSDB", Order = 61)]
        [StringLength(200)]
        public string SO_KY_AD_LSDB { get; set; } = "";

        [Column("UNTBUSCD", Order = 62)]
        [StringLength(200)]
        public string UNTBUSCD { get; set; } = "";

        [Column("TYGIA", Order = 63)]
        public decimal TYGIA { get; set; }

        // Add CREATED_DATE and UPDATED_DATE properties for ApplicationDbContext compatibility
        [Column("CREATED_DATE", Order = 65)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 66)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        // System/Temporal Columns - Always last
        [Key]
        [Column("Id", Order = 64)]
        public long Id { get; set; }

        [Column("FILE_NAME", Order = 67)]
        [StringLength(500)]
        public string FILE_NAME { get; set; } = "";
    }
}
