using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho Direct Import Service - Import trực tiếp vào bảng riêng biệt
    /// Bỏ hoàn toàn ImportedDataItems để tăng hiệu năng
    /// </summary>
    public interface IDirectImportService
    {
        /// <summary>
        /// Import trực tiếp file CSV vào bảng DP01_New sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportDP01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng LN01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportLN01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng LN02 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportLN02DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng LN03 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportLN03DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng DB01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportDB01DirectAsync(IFormFile file, string? statementDate = null);

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
        /// Import trực tiếp file CSV vào bảng KH03 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportKH03DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file CSV vào bảng RR01 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportRR01DirectAsync(IFormFile file, string? statementDate = null);

        /// <summary>
        /// Import trực tiếp file Excel vào bảng 7800_DT_KHKD1 sử dụng SqlBulkCopy
        /// </summary>
        Task<DirectImportResult> ImportDT_KHKD1DirectAsync(IFormFile file, string? statementDate = null);

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
    }
}
