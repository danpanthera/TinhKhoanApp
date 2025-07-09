using System;

namespace TinhKhoanApp.Api.Models
{
    /// <summary>
    /// Kết quả của quá trình Direct Import
    /// </summary>
    public class DirectImportResult
    {
        /// <summary>
        /// Trạng thái thành công hay thất bại
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Tên file được import
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Loại dữ liệu (DP01, LN01, DB01, ...)
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// Tên bảng đích được import
        /// </summary>
        public string TargetTable { get; set; } = string.Empty;

        /// <summary>
        /// Kích thước file (bytes)
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Số records đã được xử lý thành công
        /// </summary>
        public int ProcessedRecords { get; set; }

        /// <summary>
        /// Số records bị lỗi
        /// </summary>
        public int ErrorRecords { get; set; }

        /// <summary>
        /// Batch ID để tracking
        /// </summary>
        public string BatchId { get; set; } = string.Empty;

        /// <summary>
        /// ID của ImportedDataRecord tương ứng
        /// </summary>
        public int ImportedDataRecordId { get; set; }

        /// <summary>
        /// Thời gian bắt đầu import
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc import
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Tổng thời gian import
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Thông báo lỗi (nếu có)
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin chi tiết về kết quả
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Tốc độ import (records/second)
        /// </summary>
        public double RecordsPerSecond
        {
            get
            {
                if (Duration.TotalSeconds > 0)
                    return ProcessedRecords / Duration.TotalSeconds;
                return 0;
            }
        }

        /// <summary>
        /// Tốc độ import (MB/second)
        /// </summary>
        public double MBPerSecond
        {
            get
            {
                if (Duration.TotalSeconds > 0)
                    return (FileSizeBytes / 1024.0 / 1024.0) / Duration.TotalSeconds;
                return 0;
            }
        }
    }
}
