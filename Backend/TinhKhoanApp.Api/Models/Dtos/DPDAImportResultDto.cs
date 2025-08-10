using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    /// <summary>
    /// DPDA Import Result DTO - Result information từ CSV import operations
    /// Contains import statistics, success/error counts, validation messages
    /// Used in: ImportFromCsvAsync service method result, CSV import endpoints
    /// Provides detailed feedback về import process cho user interface
    /// </summary>
    public class DPDAImportResultDto
    {
        /// <summary>Tên file CSV được import</summary>
        [Display(Name = "Tên File")]
        public string FileName { get; set; } = "";

        /// <summary>Thời gian thực hiện import</summary>
        [Display(Name = "Ngày Import")]
        public DateTime ImportDate { get; set; }

        /// <summary>Trạng thái import thành công</summary>
        [Display(Name = "Thành Công")]
        public bool IsSuccess { get; set; }

        /// <summary>Tổng số records trong CSV (excluding header)</summary>
        [Display(Name = "Tổng Records")]
        public int TotalRecords { get; set; }

        /// <summary>Số records import thành công</summary>
        [Display(Name = "Records Thành Công")]
        public int SuccessCount { get; set; }

        /// <summary>Số records lỗi</summary>
        [Display(Name = "Records Lỗi")]
        public int ErrorCount { get; set; }

        /// <summary>Danh sách chi tiết lỗi và warnings</summary>
        [Display(Name = "Thông Báo Lỗi")]
        public List<string> ErrorMessages { get; set; } = new List<string>();

        /// <summary>Tỷ lệ thành công (calculated property)</summary>
        [Display(Name = "Tỷ Lệ Thành Công (%)")]
        public double SuccessRate
        {
            get
            {
                return TotalRecords > 0 ? (double)SuccessCount / TotalRecords * 100 : 0;
            }
        }

        /// <summary>Duration của import process (optional)</summary>
        [Display(Name = "Thời Gian Xử Lý")]
        public TimeSpan? ProcessingDuration { get; set; }

        /// <summary>Summary message cho user display</summary>
        [Display(Name = "Tóm Tắt")]
        public string Summary
        {
            get
            {
                return IsSuccess
                    ? $"Import thành công {SuccessCount}/{TotalRecords} records từ {FileName}"
                    : $"Import lỗi: {ErrorCount} lỗi trong {TotalRecords} records từ {FileName}";
            }
        }
    }
}
