namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu GL01 - General Ledger Data (27 business columns)
    /// STRUCTURE: NGAY_DL -> 27 Business Columns (CSV order) -> System Columns
    /// IMPORTANT: Use exact business column names, NO Vietnamese transformation
    /// </summary>
    public class GL01PreviewDto
    {
        /// <summary>
        /// Ngày dữ liệu (từ TR_TIME) - Partitioned Column
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// STS - Status
        /// </summary>
        public string? STS { get; set; }

        /// <summary>
        /// NGAY_GD - Transaction Date
        /// </summary>
        public DateTime? NGAY_GD { get; set; }

        /// <summary>
        /// NGUOI_TAO - Creator
        /// </summary>
        public string? NGUOI_TAO { get; set; }

        /// <summary>
        /// DYSEQ - Daily Sequence
        /// </summary>
        public string? DYSEQ { get; set; }

        /// <summary>
        /// TR_TYPE - Transaction Type
        /// </summary>
        public string? TR_TYPE { get; set; }

        /// <summary>
        /// DT_SEQ - Detail Sequence
        /// </summary>
        public string? DT_SEQ { get; set; }

        /// <summary>
        /// TAI_KHOAN - Account Number
        /// </summary>
        public string? TAI_KHOAN { get; set; }

        /// <summary>
        /// TEN_TK - Account Name
        /// </summary>
        public string? TEN_TK { get; set; }

        /// <summary>
        /// SO_TIEN_GD - Transaction Amount (decimal format #,###.00)
        /// </summary>
        public decimal? SO_TIEN_GD { get; set; }

        /// <summary>
        /// POST_BR - Posting Branch
        /// </summary>
        public string? POST_BR { get; set; }

        /// <summary>
        /// LOAI_TIEN - Currency Type
        /// </summary>
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// DR_CR - Debit/Credit Flag
        /// </summary>
        public string? DR_CR { get; set; }

        /// <summary>
        /// MA_KH - Customer Code
        /// </summary>
        public string? MA_KH { get; set; }

        /// <summary>
        /// TEN_KH - Customer Name
        /// </summary>
        public string? TEN_KH { get; set; }

        /// <summary>
        /// CCA_USRID - User ID
        /// </summary>
        public string? CCA_USRID { get; set; }

        /// <summary>
        /// TR_EX_RT - Exchange Rate
        /// </summary>
        public string? TR_EX_RT { get; set; }

        /// <summary>
        /// REMARK - Remarks (1000 chars)
        /// </summary>
        public string? REMARK { get; set; }

        /// <summary>
        /// BUS_CODE - Business Code
        /// </summary>
        public string? BUS_CODE { get; set; }

        /// <summary>
        /// UNIT_BUS_CODE - Unit Business Code
        /// </summary>
        public string? UNIT_BUS_CODE { get; set; }

        /// <summary>
        /// TR_CODE - Transaction Code
        /// </summary>
        public string? TR_CODE { get; set; }

        /// <summary>
        /// TR_NAME - Transaction Name
        /// </summary>
        public string? TR_NAME { get; set; }

        /// <summary>
        /// REFERENCE - Reference
        /// </summary>
        public string? REFERENCE { get; set; }

        /// <summary>
        /// VALUE_DATE - Value Date
        /// </summary>
        public DateTime? VALUE_DATE { get; set; }

        /// <summary>
        /// DEPT_CODE - Department Code
        /// </summary>
        public string? DEPT_CODE { get; set; }

        /// <summary>
        /// TR_TIME - Transaction Time (source for NGAY_DL)
        /// </summary>
        public string? TR_TIME { get; set; }

        /// <summary>
        /// COMFIRM - Confirmation
        /// </summary>
        public string? COMFIRM { get; set; }

        /// <summary>
        /// TRDT_TIME - Transaction Date Time
        /// </summary>
        public string? TRDT_TIME { get; set; }

        /// <summary>
        /// System Column: Primary Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// System Column: Created Date
        /// </summary>
        public DateTime CREATED_DATE { get; set; }

        /// <summary>
        /// System Column: Updated Date
        /// </summary>
        public DateTime UPDATED_DATE { get; set; }

        /// <summary>
        /// System Column: File Name
        /// </summary>
        public string? FILE_NAME { get; set; }
    }

    /// <summary>
    /// GL01 Detail DTO - Inherits from Preview with additional system info
    /// </summary>
    public class GL01DetailDto : GL01PreviewDto
    {
        // All fields inherited from GL01PreviewDto
        // Additional computed fields can be added here if needed
    }

    /// <summary>
    /// GL01 Summary DTO cho analytics và reports
    /// </summary>
    public class GL01SummaryDto
    {
        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Tổng số records
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Tổng số tiền giao dịch
        /// </summary>
        public decimal? TotalSO_TIEN_GD { get; set; }

        /// <summary>
        /// Số files được import
        /// </summary>
        public string? FileSources { get; set; }

        /// <summary>
        /// Thời điểm import mới nhất
        /// </summary>
        public DateTime LatestImport { get; set; }

        /// <summary>
        /// Số lượng DR transactions
        /// </summary>
        public int DebitCount { get; set; }

        /// <summary>
        /// Số lượng CR transactions
        /// </summary>
        public int CreditCount { get; set; }
    }
}
