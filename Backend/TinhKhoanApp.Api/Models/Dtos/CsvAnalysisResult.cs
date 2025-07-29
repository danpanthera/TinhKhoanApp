namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// Kết quả phân tích cấu trúc CSV
    /// </summary>
    public class CsvAnalysisResult
    {
        /// <summary>
        /// Số lượng cột
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// Tên các cột
        /// </summary>
        public List<string> ColumnNames { get; set; } = new List<string>();

        /// <summary>
        /// Kiểu dữ liệu ước tính của các cột
        /// </summary>
        public Dictionary<string, string> ColumnTypes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Độ dài tối đa của mỗi cột
        /// </summary>
        public Dictionary<string, int> MaxLengths { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Số lượng dòng phân tích
        /// </summary>
        public int AnalyzedRows { get; set; }

        /// <summary>
        /// Số lượng dòng ước tính trong file
        /// </summary>
        public int EstimatedTotalRows { get; set; }

        /// <summary>
        /// Kích thước file (bytes)
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Ký tự phân cách được sử dụng
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// Có hàng tiêu đề không
        /// </summary>
        public bool HasHeaderRow { get; set; }

        /// <summary>
        /// Đường dẫn file
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName => Path.GetFileName(FilePath);

        /// <summary>
        /// Thời gian phân tích (ms)
        /// </summary>
        public long AnalysisTimeMs { get; set; }

        /// <summary>
        /// Các vấn đề tìm thấy trong quá trình phân tích
        /// </summary>
        public List<string> Issues { get; set; } = new List<string>();

        /// <summary>
        /// Mẫu dữ liệu
        /// </summary>
        public List<Dictionary<string, string>> SampleData { get; set; } = new List<Dictionary<string, string>>();
    }
}
