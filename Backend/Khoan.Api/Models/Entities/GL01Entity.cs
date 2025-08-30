using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// GL01 Entity - General Ledger Detail Table (UNIFIED STRUCTURE - HEAVY FILE OPTIMIZED)
    /// Cấu trúc: NGAY_DL (từ TR_TIME) -> 27 business columns -> 4 System Columns (NON-TEMPORAL)
    /// CSV Source: 7800_gl01_2024120120241231.csv
    /// 27 business columns: STS,NGAY_GD,NGUOI_TAO,DYSEQ,TR_TYPE,DT_SEQ,TAI_KHOAN,TEN_TK,SO_TIEN_GD,POST_BR,LOAI_TIEN,DR_CR,MA_KH,TEN_KH,CCA_USRID,TR_EX_RT,REMARK,BUS_CODE,UNIT_BUS_CODE,TR_CODE,TR_NAME,REFERENCE,VALUE_DATE,DEPT_CODE,TR_TIME,COMFIRM,TRDT_TIME
    /// Heavy File Support: Partitioned Columnstore, BulkInsert BatchSize 10,000, MaxFileSize 2GB
    /// Date Format: DATE/NGAY columns as datetime2 (dd/MM/yyyy)
    /// Decimal Format: AMT/AMOUNT/BALANCE/SO_TIEN as decimal(18,2) with #,###.00 format
    /// String Columns: nvarchar(200), REMARK as nvarchar(1000)
    /// </summary>
    [Table("GL01")]
    [Index(nameof(NGAY_GD), Name = "IX_GL01_NGAY_GD")]
    [Index(nameof(TAI_KHOAN), Name = "IX_GL01_TAI_KHOAN")]
    [Index(nameof(MA_KH), Name = "IX_GL01_MA_KH")]
    [Index(nameof(DYSEQ), Name = "IX_GL01_DYSEQ")]
    [Index(nameof(DR_CR), Name = "IX_GL01_DR_CR")]
    [Index(nameof(NGAY_DL), Name = "IX_GL01_NGAY_DL")]
    public class GL01Entity
    {
        // === PRIMARY KEY ===
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // === NGAY_DL - System Column FIRST (lấy từ TR_TIME column 25) ===
        /// <summary>
        /// Ngày dữ liệu - lấy từ TR_TIME column (column 25) -> datetime2 (dd/MM/yyyy)
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime NGAY_DL { get; set; }

        // === 27 BUSINESS COLUMNS - Exact CSV structure ===

        /// <summary>
        /// Status - Column 1
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? STS { get; set; }

        /// <summary>
        /// Ngày giao dịch - Column 2 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? NGAY_GD { get; set; }

        /// <summary>
        /// Người tạo - Column 3
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? NGUOI_TAO { get; set; }

        /// <summary>
        /// Daily sequence - Column 4
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? DYSEQ { get; set; }

        /// <summary>
        /// Transaction type - Column 5
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TR_TYPE { get; set; }

        /// <summary>
        /// Detail sequence - Column 6
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? DT_SEQ { get; set; }

        /// <summary>
        /// Tài khoản - Column 7
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TAI_KHOAN { get; set; }

        /// <summary>
        /// Tên tài khoản - Column 8
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Số tiền giao dịch - Column 9 (SO_TIEN format -> decimal(18,2) #,###.00)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SO_TIEN_GD { get; set; }

        /// <summary>
        /// Post branch - Column 10
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? POST_BR { get; set; }

        /// <summary>
        /// Loại tiền - Column 11
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Debit/Credit - Column 12
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? DR_CR { get; set; }

        /// <summary>
        /// Mã khách hàng - Column 13
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? MA_KH { get; set; }

        /// <summary>
        /// Tên khách hàng - Column 14
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TEN_KH { get; set; }

        /// <summary>
        /// CCA User ID - Column 15
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? CCA_USRID { get; set; }

        /// <summary>
        /// Transaction exchange rate - Column 16
        /// </summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal? TR_EX_RT { get; set; }

        /// <summary>
        /// Remark - Column 17 (REMARK -> nvarchar(1000))
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string? REMARK { get; set; }

        /// <summary>
        /// Business code - Column 18
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? BUS_CODE { get; set; }

        /// <summary>
        /// Unit business code - Column 19
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? UNIT_BUS_CODE { get; set; }

        /// <summary>
        /// Transaction code - Column 20
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TR_CODE { get; set; }

        /// <summary>
        /// Transaction name - Column 21
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? TR_NAME { get; set; }

        /// <summary>
        /// Reference - Column 22
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? REFERENCE { get; set; }

        /// <summary>
        /// Value date - Column 23 (DATE format -> datetime2 dd/MM/yyyy)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// Department code - Column 24
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? DEPT_CODE { get; set; }

        /// <summary>
        /// Transaction time - Column 25 (TR_TIME được dùng làm NGAY_DL)
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? TR_TIME { get; set; }

        /// <summary>
        /// Confirm - Column 26
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? COMFIRM { get; set; }

        /// <summary>
        /// Transaction date time - Column 27
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? TRDT_TIME { get; set; }

        // === 4 SYSTEM COLUMNS (NO TEMPORAL for heavy file performance) ===
        /// <summary>
        /// Created date - System column
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Updated date - System column
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Source file name - System column
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? FileName { get; set; }

        /// <summary>
        /// Import batch ID - System column for heavy file tracking
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? ImportBatchId { get; set; }
    }
}
