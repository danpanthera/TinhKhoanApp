using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Models.Entities
{
    /// <summary>
    /// LN01 Entity - Loan Detail Table
    /// Most complex table với 79 business columns + system columns
    /// CSV Source: 7800_ln01_20241231.csv
    /// </summary>
    [Table("LN01")]
    [Index(nameof(BRCD), Name = "IX_LN01_BRCD")]
    [Index(nameof(CUSTSEQ), Name = "IX_LN01_CUSTSEQ")]
    [Index(nameof(TAI_KHOAN), Name = "IX_LN01_TAI_KHOAN")]
    [Index(nameof(TRANSACTION_DATE), Name = "IX_LN01_TRANSACTION_DATE")]
    [Index(nameof(BRCD), nameof(CUSTSEQ), Name = "IX_LN01_BRCD_CUSTSEQ")]
    public class LN01Entity : ITemporalEntity
    {
        // === SYSTEM COLUMNS (từ ITemporalEntity) ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2(3)")]
        public DateTime UpdatedAt { get; set; }

        // Temporal table support
        [Column(TypeName = "datetime2(3)")]
        public DateTime SysStartTime { get; set; }

        [Column(TypeName = "datetime2(3)")]
        public DateTime SysEndTime { get; set; }

        // === BUSINESS COLUMNS (79 columns theo CSV structure) ===

        /// <summary>
        /// Mã chi nhánh - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string BRCD { get; set; } = string.Empty;

        /// <summary>
        /// Mã khách hàng - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string CUSTSEQ { get; set; } = string.Empty;

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? CUSTNM { get; set; }

        /// <summary>
        /// Số tài khoản - BUSINESS KEY
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string TAI_KHOAN { get; set; } = string.Empty;

        /// <summary>
        /// Đơn vị tiền tệ
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? CCY { get; set; }

        /// <summary>
        /// Dư nợ hiện tại
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DU_NO { get; set; }

        /// <summary>
        /// Disbursement Sequence
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? DSBSSEQ { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? TRANSACTION_DATE { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? DSBSDT { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ giải ngân
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? DISBUR_CCY { get; set; }

        /// <summary>
        /// Số tiền giải ngân
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        /// <summary>
        /// Ngày đến hạn giải ngân
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? DSBSMATDT { get; set; }

        /// <summary>
        /// Base rate code
        /// </summary>
        [Column(TypeName = "nvarchar(20)")]
        public string? BSRTCD { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        [Column(TypeName = "decimal(10,6)")]
        public decimal? INTEREST_RATE { get; set; }

        /// <summary>
        /// Approval sequence
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? APPRSEQ { get; set; }

        /// <summary>
        /// Ngày phê duyệt
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? APPRDT { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ phê duyệt
        /// </summary>
        [Column(TypeName = "nvarchar(10)")]
        public string? APPR_CCY { get; set; }

        /// <summary>
        /// Số tiền phê duyệt
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? APPRAMT { get; set; }

        /// <summary>
        /// Ngày đến hạn phê duyệt
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? APPRMATDT { get; set; }

        /// <summary>
        /// Loại khoản vay
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? LOAN_TYPE { get; set; }

        /// <summary>
        /// Mã nguồn vốn
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? FUND_RESOURCE_CODE { get; set; }

        /// <summary>
        /// Mã mục đích vay
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? FUND_PURPOSE_CODE { get; set; }

        /// <summary>
        /// Số tiền trả nợ
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        /// <summary>
        /// Ngày trả nợ tiếp theo
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NEXT_REPAY_DATE { get; set; }

        /// <summary>
        /// Số tiền trả tiếp theo
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        /// <summary>
        /// Ngày trả lãi tiếp theo
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NEXT_INT_REPAY_DATE { get; set; }

        /// <summary>
        /// Mã cán bộ quản lý
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? OFFICER_ID { get; set; }

        /// <summary>
        /// Tên cán bộ quản lý
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? OFFICER_NAME { get; set; }

        /// <summary>
        /// Số tiền lãi
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? INTEREST_AMOUNT { get; set; }

        /// <summary>
        /// Lãi quá hạn
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }

        /// <summary>
        /// Tổng lãi đã trả
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        /// <summary>
        /// Mã loại khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        /// <summary>
        /// Chi tiết loại khách hàng
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        /// <summary>
        /// Territory code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TRCTCD { get; set; }

        /// <summary>
        /// Territory name
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? TRCTNM { get; set; }

        /// <summary>
        /// Địa chỉ 1
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? ADDR1 { get; set; }

        /// <summary>
        /// Tỉnh/thành phố
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? PROVINCE { get; set; }

        /// <summary>
        /// Tên tỉnh địa phương
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? LCLPROVINNM { get; set; }

        /// <summary>
        /// Quận/huyện
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? DISTRICT { get; set; }

        /// <summary>
        /// Tên quận/huyện địa phương
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? LCLDISTNM { get; set; }

        /// <summary>
        /// Mã xã/phường
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? COMMCD { get; set; }

        /// <summary>
        /// Tên xã/phường địa phương
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? LCLWARDNM { get; set; }

        /// <summary>
        /// Ngày trả nợ cuối cùng
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? LAST_REPAY_DATE { get; set; }

        /// <summary>
        /// Tỷ lệ bảo đảm
        /// </summary>
        [Column(TypeName = "decimal(10,4)")]
        public decimal? SECURED_PERCENT { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [Column(TypeName = "int")]
        public int? NHOM_NO { get; set; }

        /// <summary>
        /// Ngày tính lãi cuối cùng
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? LAST_INT_CHARGE_DATE { get; set; }

        /// <summary>
        /// Miễn lãi
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EXEMPTINT { get; set; }

        /// <summary>
        /// Loại miễn lãi
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? EXEMPTINTTYPE { get; set; }

        /// <summary>
        /// Số tiền miễn lãi
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EXEMPTINTAMT { get; set; }

        /// <summary>
        /// Group number
        /// </summary>
        [Column(TypeName = "int")]
        public int? GRPNO { get; set; }

        /// <summary>
        /// Business code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? BUSCD { get; set; }

        /// <summary>
        /// Business scale type code
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? BSNSSCLTPCD { get; set; }

        /// <summary>
        /// User ID operator
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? USRIDOP { get; set; }

        /// <summary>
        /// Accrual amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        /// <summary>
        /// Accrual amount cuối tháng
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        /// <summary>
        /// Interest calculation method
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? INTCMTH { get; set; }

        /// <summary>
        /// Interest repayment method
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? INTRPYMTH { get; set; }

        /// <summary>
        /// Interest term month
        /// </summary>
        [Column(TypeName = "int")]
        public int? INTTRMMTH { get; set; }

        /// <summary>
        /// Year days
        /// </summary>
        [Column(TypeName = "int")]
        public int? YRDAYS { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string? REMARK { get; set; }

        /// <summary>
        /// Chỉ tiêu
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? CHITIEU { get; set; }

        /// <summary>
        /// CTCV
        /// </summary>
        [Column(TypeName = "nvarchar(255)")]
        public string? CTCV { get; set; }

        /// <summary>
        /// Credit line type
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CREDIT_LINE_YPE { get; set; }

        /// <summary>
        /// Interest lump sum partial type
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        /// <summary>
        /// Interest partial payment type
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        /// <summary>
        /// Interest payment interval
        /// </summary>
        [Column(TypeName = "int")]
        public int? INT_PAYMENT_INTERVAL { get; set; }

        /// <summary>
        /// Ân hạn lãi
        /// </summary>
        [Column(TypeName = "int")]
        public int? AN_HAN_LAI { get; set; }

        /// <summary>
        /// Phương thức giải ngân 1
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        /// <summary>
        /// Tài khoản giải ngân 1
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        /// <summary>
        /// Số tiền giải ngân 1
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        /// <summary>
        /// Phương thức giải ngân 2
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        /// <summary>
        /// Tài khoản giải ngân 2
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        /// <summary>
        /// Số tiền giải ngân 2
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }

        /// <summary>
        /// CMND/Hộ chiếu
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CMT_HC { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? NGAY_SINH { get; set; }

        /// <summary>
        /// Mã cán bộ Agri
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? MA_CB_AGRI { get; set; }

        /// <summary>
        /// Mã ngành kinh tế
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? MA_NGANH_KT { get; set; }

        /// <summary>
        /// Tỷ giá
        /// </summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal? TY_GIA { get; set; }

        /// <summary>
        /// Officer IPCAS
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? OFFICER_IPCAS { get; set; }

        // === METADATA COLUMNS ===

        /// <summary>
        /// Tên file import (7800_ln01_20241231.csv)
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? FileName { get; set; }

        /// <summary>
        /// Import batch ID để tracking
        /// </summary>
        [Column(TypeName = "uniqueidentifier")]
        public Guid? ImportId { get; set; }

        /// <summary>
        /// Additional metadata về import process
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string? ImportMetadata { get; set; }
    }
}
