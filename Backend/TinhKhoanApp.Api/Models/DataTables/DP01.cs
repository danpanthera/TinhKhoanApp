using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DP01 - Dữ liệu tiền gửi
    /// Cấu trúc theo file: 7808_dp01_20241231.csv
    /// 62 cột business data + temporal columns
    /// </summary>
    [Table("DP01")]
    public class DP01
    {
        [Key]
        public int Id { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // === 62 CỘT BUSINESS DATA THEO CSV GỐC ===

        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("TAI_KHOAN_HACH_TOAN")]
        [StringLength(50)]
        public string? TAI_KHOAN_HACH_TOAN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("DP_TYPE_NAME")]
        [StringLength(255)]
        public string? DP_TYPE_NAME { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? CCY { get; set; }

        [Column("CURRENT_BALANCE")]
        public decimal? CURRENT_BALANCE { get; set; }

        [Column("RATE")]
        public decimal? RATE { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(50)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("OPENING_DATE")]
        [StringLength(20)]
        public string? OPENING_DATE { get; set; }

        [Column("MATURITY_DATE")]
        [StringLength(20)]
        public string? MATURITY_DATE { get; set; }

        [Column("ADDRESS")]
        [StringLength(500)]
        public string? ADDRESS { get; set; }

        [Column("NOTENO")]
        [StringLength(50)]
        public string? NOTENO { get; set; }

        [Column("MONTH_TERM")]
        public int? MONTH_TERM { get; set; }

        [Column("TERM_DP_NAME")]
        [StringLength(255)]
        public string? TERM_DP_NAME { get; set; }

        [Column("TIME_DP_NAME")]
        [StringLength(255)]
        public string? TIME_DP_NAME { get; set; }

        [Column("MA_PGD")]
        [StringLength(50)]
        public string? MA_PGD { get; set; }

        [Column("TEN_PGD")]
        [StringLength(255)]
        public string? TEN_PGD { get; set; }

        [Column("DP_TYPE_CODE")]
        [StringLength(50)]
        public string? DP_TYPE_CODE { get; set; }

        [Column("RENEW_DATE")]
        [StringLength(20)]
        public string? RENEW_DATE { get; set; }

        [Column("CUST_TYPE")]
        [StringLength(50)]
        public string? CUST_TYPE { get; set; }

        [Column("CUST_TYPE_NAME")]
        [StringLength(255)]
        public string? CUST_TYPE_NAME { get; set; }

        [Column("CUST_TYPE_DETAIL")]
        [StringLength(50)]
        public string? CUST_TYPE_DETAIL { get; set; }

        [Column("CUST_DETAIL_NAME")]
        [StringLength(255)]
        public string? CUST_DETAIL_NAME { get; set; }

        [Column("PREVIOUS_DP_CAP_DATE")]
        [StringLength(20)]
        public string? PREVIOUS_DP_CAP_DATE { get; set; }

        [Column("NEXT_DP_CAP_DATE")]
        [StringLength(20)]
        public string? NEXT_DP_CAP_DATE { get; set; }

        [Column("ID_NUMBER")]
        [StringLength(50)]
        public string? ID_NUMBER { get; set; }

        [Column("ISSUED_BY")]
        [StringLength(255)]
        public string? ISSUED_BY { get; set; }

        [Column("ISSUE_DATE")]
        [StringLength(20)]
        public string? ISSUE_DATE { get; set; }

        [Column("SEX_TYPE")]
        [StringLength(20)]
        public string? SEX_TYPE { get; set; }

        [Column("BIRTH_DATE")]
        [StringLength(20)]
        public string? BIRTH_DATE { get; set; }

        [Column("TELEPHONE")]
        [StringLength(50)]
        public string? TELEPHONE { get; set; }

        [Column("ACRUAL_AMOUNT")]
        public decimal? ACRUAL_AMOUNT { get; set; }

        [Column("ACRUAL_AMOUNT_END")]
        public decimal? ACRUAL_AMOUNT_END { get; set; }

        [Column("ACCOUNT_STATUS")]
        [StringLength(50)]
        public string? ACCOUNT_STATUS { get; set; }

        [Column("DRAMT")]
        public decimal? DRAMT { get; set; }

        [Column("CRAMT")]
        public decimal? CRAMT { get; set; }

        [Column("EMPLOYEE_NUMBER")]
        [StringLength(50)]
        public string? EMPLOYEE_NUMBER { get; set; }

        [Column("EMPLOYEE_NAME")]
        [StringLength(255)]
        public string? EMPLOYEE_NAME { get; set; }

        [Column("SPECIAL_RATE")]
        public decimal? SPECIAL_RATE { get; set; }

        [Column("AUTO_RENEWAL")]
        public int? AUTO_RENEWAL { get; set; }

        [Column("CLOSE_DATE")]
        [StringLength(20)]
        public string? CLOSE_DATE { get; set; }

        [Column("LOCAL_PROVIN_NAME")]
        [StringLength(255)]
        public string? LOCAL_PROVIN_NAME { get; set; }

        [Column("LOCAL_DISTRICT_NAME")]
        [StringLength(255)]
        public string? LOCAL_DISTRICT_NAME { get; set; }

        [Column("LOCAL_WARD_NAME")]
        [StringLength(255)]
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
        [StringLength(255)]
        public string? TAX_CODE_LOCATION { get; set; }

        [Column("MA_CAN_BO_PT")]
        [StringLength(50)]
        public string? MA_CAN_BO_PT { get; set; }

        [Column("TEN_CAN_BO_PT")]
        [StringLength(255)]
        public string? TEN_CAN_BO_PT { get; set; }

        [Column("PHONG_CAN_BO_PT")]
        [StringLength(255)]
        public string? PHONG_CAN_BO_PT { get; set; }

        [Column("NGUOI_NUOC_NGOAI")]
        [StringLength(10)]
        public string? NGUOI_NUOC_NGOAI { get; set; }

        [Column("QUOC_TICH")]
        [StringLength(100)]
        public string? QUOC_TICH { get; set; }

        [Column("MA_CAN_BO_AGRIBANK")]
        [StringLength(50)]
        public string? MA_CAN_BO_AGRIBANK { get; set; }

        [Column("NGUOI_GIOI_THIEU")]
        [StringLength(50)]
        public string? NGUOI_GIOI_THIEU { get; set; }

        [Column("TEN_NGUOI_GIOI_THIEU")]
        [StringLength(255)]
        public string? TEN_NGUOI_GIOI_THIEU { get; set; }

        [Column("CONTRACT_COUTS_DAY")]
        public int? CONTRACT_COUTS_DAY { get; set; }

        [Column("SO_KY_AD_LSDB")]
        public int? SO_KY_AD_LSDB { get; set; }

        [Column("UNTBUSCD")]
        [StringLength(50)]
        public string? UNTBUSCD { get; set; }

        [Column("TYGIA")]
        public decimal? TYGIA { get; set; }
    }
}
