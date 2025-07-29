using System.Data;
using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Interface for DirectImportService
    /// </summary>
    public interface IDirectImportService
    {
        /// <summary>
        /// Import data từ file CSV vào database
        /// </summary>
        Task<ImportResult> ImportDataFromCsvAsync(string filePath, string tableName, bool hasHeaderRow = true, char delimiter = ',', int batchSize = 1000, bool ignoreErrors = false);

        /// <summary>
        /// Đọc dữ liệu từ file CSV
        /// </summary>
        Task<DataTable> ReadCsvFileAsync(string filePath, bool hasHeaderRow = true, char delimiter = ',', int maxRows = 0);

        /// <summary>
        /// Lấy thông tin các cột của bảng
        /// </summary>
        Task<Dictionary<string, string>> GetTableColumnsAsync(string tableName);

        /// <summary>
        /// Lấy số lượng bản ghi của các bảng
        /// </summary>
        Task<Dictionary<string, int>> GetTableRecordCountsAsync();

        /// <summary>
        /// Lấy danh sách các bảng trong database
        /// </summary>
        Task<List<string>> GetAllTablesAsync();

        /// <summary>
        /// Kiểm tra sự tồn tại của một bảng
        /// </summary>
        Task<bool> TableExistsAsync(string tableName);

        /// <summary>
        /// Phân tích cấu trúc file CSV
        /// </summary>
        Task<CsvAnalysisResult> AnalyzeCsvStructureAsync(string filePath, bool hasHeaderRow = true, char delimiter = ',', int sampleSize = 1000);

        /// <summary>
        /// Lấy bản ghi mới nhất từ bảng
        /// </summary>
        Task<dynamic> GetLatestRecordAsync(string tableName);

        /// <summary>
        /// Xuất dữ liệu bảng ra file CSV
        /// </summary>
        Task<string> ExportTableToCsvAsync(string tableName, string outputPath, int maxRows = 0);
    }
}
