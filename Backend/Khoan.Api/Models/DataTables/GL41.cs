namespace Khoan.Api.Models.DataTables
{
    /// <summary>
    /// GL41 DataTable model - 100% CSV Business Columns Compliance
    /// CSV Source: 7800_gl41_yyyymmdd.csv
    /// Business Columns: MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY
    /// </summary>
    public class GL41
    {
        /// <summary>
        /// Mã chi nhánh - Business Column 1
        /// </summary>
        public string? MA_CN { get; set; }

        /// <summary>
        /// Loại tiền - Business Column 2
        /// </summary>
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Mã tài khoản - Business Column 3
        /// </summary>
        public string? MA_TK { get; set; }

        /// <summary>
        /// Tên tài khoản - Business Column 4
        /// </summary>
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Loại bút toán - Business Column 5
        /// </summary>
        public string? LOAI_BT { get; set; }

        /// <summary>
        /// Dư nợ đầu kỳ - Business Column 6
        /// </summary>
        public decimal? DN_DAUKY { get; set; }

        /// <summary>
        /// Dư có đầu kỳ - Business Column 7
        /// </summary>
        public decimal? DC_DAUKY { get; set; }

        /// <summary>
        /// Số bút toán nợ - Business Column 8
        /// </summary>
        public decimal? SBT_NO { get; set; }

        /// <summary>
        /// Số tiền ghi nợ - Business Column 9
        /// </summary>
        public decimal? ST_GHINO { get; set; }

        /// <summary>
        /// Số bút toán có - Business Column 10
        /// </summary>
        public decimal? SBT_CO { get; set; }

        /// <summary>
        /// Số tiền ghi có - Business Column 11
        /// </summary>
        public decimal? ST_GHICO { get; set; }

        /// <summary>
        /// Dư nợ cuối kỳ - Business Column 12
        /// </summary>
        public decimal? DN_CUOIKY { get; set; }

        /// <summary>
        /// Dư có cuối kỳ - Business Column 13
        /// </summary>
        public decimal? DC_CUOIKY { get; set; }

        /// <summary>
        /// Ngày dữ liệu - System Column
        /// </summary>
        public DateTime? NGAY_DL { get; set; }

        /// <summary>
        /// Tên file nguồn - System Column
        /// </summary>
        public string? FILE_NAME { get; set; }
    }
}
