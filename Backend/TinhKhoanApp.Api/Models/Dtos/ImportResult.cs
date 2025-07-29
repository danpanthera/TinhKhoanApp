namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// Kết quả import dữ liệu
    /// </summary>
    public class ImportResult
    {
        /// <summary>
        /// Thành công hay không
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng bản ghi đã import
        /// </summary>
        public int RecordsImported { get; set; }

        /// <summary>
        /// Số lượng bản ghi bị lỗi
        /// </summary>
        public int RecordsFailed { get; set; }

        /// <summary>
        /// Tổng số bản ghi trong file nguồn
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Đường dẫn file nguồn
        /// </summary>
        public string SourceFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Tên bảng đích
        /// </summary>
        public string TargetTable { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian import (ms)
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// Chi tiết lỗi
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Mapping cột
        /// </summary>
        public Dictionary<string, string> ColumnMapping { get; set; } = new Dictionary<string, string>();
    }
}
