using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho Direct Import Service - Import trực tiếp vào bảng riêng biệt
    /// Sử dụng SqlBulkCopy để tăng hiệu năng cao
    /// </summary>
    public interface IDirectImportService
    {
        /// <summary>
        /// Import trực tiếp file CSV vào bảng DP01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportDP01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng LN01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportLN01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng LN03 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportLN03DirectAsync(IFormFile file, string? statementDate = null);



        /// <summary>
        /// Import trực tiếp file CSV vào bảng GL01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportGL01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng GL41 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportGL41DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng DPDA sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportDPDADirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng EI01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportEI01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng RR01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportRR01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import thông minh - tự động detect loại file và import trực tiếp
        /// </summary>
        Task<DirectImportResult> ImportSmartDirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Lấy lịch sử import để hiển thị trong Raw Data view
        /// </summary>
        Task<List<object>> GetImportHistoryAsync();

        /// <summary>
        /// Lấy preview data cho import record
        /// </summary>
        Task<object?> GetImportPreviewAsync(int importId);

        /// <summary>
        /// Xóa import record và dữ liệu liên quan
        /// </summary>
        Task<(bool Success, string ErrorMessage, int RecordsDeleted)> DeleteImportAsync(int importId);

        /// <summary>
        /// Xóa import records theo ngày và data type
        /// </summary>
        Task<(bool Success, string ErrorMessage, int RecordsDeleted)> DeleteImportsByDateAsync(string dataType, string date);

        /// <summary>
        /// Xóa toàn bộ dữ liệu import (import history và dữ liệu trong các bảng)
        /// </summary>
        Task<(bool Success, string ErrorMessage, int RecordsDeleted)> ClearAllDataAsync();

        /// <summary>
        /// 🔧 TEMPORARY: Fix GL41 database structure to match CSV (13 columns)
        /// </summary>
        Task<DirectImportResult> FixGL41DatabaseStructureAsync();

        /// <summary>
        /// Kiểm tra xem dữ liệu có tồn tại cho dataType và date cụ thể
        /// </summary>
        Task<DataCheckResult> CheckDataExistsAsync(string dataType, string date);

        /// <summary>
        /// Xóa toàn bộ dữ liệu của một bảng cụ thể
        /// </summary>
        Task<DirectImportResult> ClearTableDataAsync(string dataType);

        /// <summary>
        /// Lấy số lượng records thực tế từ tất cả database tables
        /// </summary>
        Task<Dictionary<string, int>> GetTableRecordCountsAsync();

        /// <summary>
        /// 🚀 STREAMING IMPORT - Import file lớn bằng streaming để tránh OutOfMemory
        /// Stream trực tiếp từ HTTP request vào database
        /// </summary>
        Task<DirectImportResult> StreamImportAsync(Stream fileStream, string fileName, string dataType);

        /// <summary>
        /// Phát hiện loại dữ liệu từ tên file
        /// </summary>
        string DetectDataTypeFromFileName(string fileName);

        /// <summary>
        /// 🔄 PARALLEL IMPORT - Import với parallel processing cho file cực lớn
        /// Chia file thành chunks và xử lý song song
        /// </summary>
        Task<DirectImportResult> ParallelImportAsync(Stream fileStream, string fileName, string dataType, int chunkSize = 50000);

        /// <summary>
        /// 🆔 Tạo upload session cho chunked upload
        /// </summary>
        Task CreateUploadSessionAsync(UploadSession session);

        /// <summary>
        /// 📤 Upload chunk
        /// </summary>
        Task<ChunkUploadResult> UploadChunkAsync(string sessionId, int chunkIndex, IFormFile chunk);

        /// <summary>
        /// ✅ Finalize chunked upload và process file
        /// </summary>
        Task<DirectImportResult> FinalizeUploadAsync(string sessionId);

        /// <summary>
        /// 📊 Get upload info (for resume functionality)
        /// </summary>
        Task<UploadInfoResponse> GetUploadInfoAsync(string sessionId);

        /// <summary>
        /// 🚫 Cancel upload session
        /// </summary>
        Task CancelUploadAsync(string sessionId);

        /// <summary>
        /// Validate GL02 data - kiểm tra CRTDTM parsing và data integrity
        /// </summary>
        Task<object> ValidateGL02DataAsync();
    }
}
