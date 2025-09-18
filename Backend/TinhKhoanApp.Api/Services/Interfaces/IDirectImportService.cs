using Microsoft.AspNetCore.Http;
using Khoan.Api.Models;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho Direct Import Service - CSV upload và processing
    /// </summary>
    public interface IDirectImportService
    {
        // Generic Import Methods
        Task<DirectImportResult> ImportFromCsvAsync<T>(IFormFile file, string dataType, string? statementDate = null) where T : class;
        Task<DirectImportResult> ImportGenericAsync(IFormFile file, string dataType, string? statementDate = null);

        // DP01 Import Methods
        Task<DirectImportResult> ImportDP01Async(IFormFile file, string? statementDate = null);

        // DPDA Import Methods
        Task<DirectImportResult> ImportDPDAAsync(IFormFile file, string? statementDate = null);

        // EI01 Import Methods
        Task<DirectImportResult> ImportEI01Async(IFormFile file, string? statementDate = null);

        // LN03 Import Methods
        Task<DirectImportResult> ImportLN03EnhancedAsync(IFormFile file, string? statementDate = null);

        // GL01 Import Methods
        Task<DirectImportResult> ImportGL01Async(IFormFile file, string? statementDate = null);

        // GL02 Import Methods
        Task<DirectImportResult> ImportGL02Async(IFormFile file, string? statementDate = null);

        // GL41 Import Methods
        Task<DirectImportResult> ImportGL41Async(IFormFile file, string? statementDate = null);

        // RR01 Import Methods
        Task<DirectImportResult> ImportRR01Async(IFormFile file, string? statementDate = null);

        // Utility Methods
        string ExtractNgayDLFromFileName(string fileName);
        Task<bool> ValidateFileFormatAsync(IFormFile file, string expectedDataType);
        Task<DirectImportResult> GetImportStatusAsync(Guid importId);
    }
}
