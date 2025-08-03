using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service để xử lý import dữ liệu thô với Direct Import workflow
    /// Sử dụng Direct Import Tables thay vì legacy ImportedDataItems
    /// </summary>
    public interface IRawDataProcessingService
    {
        // Direct import method - only requires metadata from ImportedDataRecord
        Task<ProcessingResult> ProcessDirectImportRecordAsync(int importedDataRecordId, string category);

        Task<List<string>> GetValidCategoriesAsync();
    }

    public class ProcessingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BatchId { get; set; }
        public int ProcessedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? TableName { get; set; }
    }

    public class RawDataProcessingService : IRawDataProcessingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RawDataProcessingService> _logger;

        public RawDataProcessingService(ApplicationDbContext context, ILogger<RawDataProcessingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProcessingResult> ProcessDirectImportRecordAsync(int importedDataRecordId, string category)
        {
            var result = new ProcessingResult();

            try
            {
                _logger.LogInformation("🔄 Processing direct import record. RecordId: {RecordId}, Category: {Category}",
                    importedDataRecordId, category);

                // Validate parameters
                if (string.IsNullOrWhiteSpace(category))
                {
                    result.Message = "Category is required";
                    return result;
                }

                // Get imported data record (metadata only, Direct Import workflow)
                var importedRecord = await _context.ImportedDataRecords
                    .FirstOrDefaultAsync(r => r.Id == importedDataRecordId);

                if (importedRecord == null)
                {
                    result.Message = $"Imported data record with ID {importedDataRecordId} not found";
                    return result;
                }

                // For direct import, data should already be in target tables
                // This method just updates metadata and status
                result.Success = true;
                result.Message = $"Direct import record {importedDataRecordId} processed successfully";
                result.TableName = GetTableNameForCategory(category);
                result.ProcessedRecords = importedRecord.RecordsCount;

                // Update record status if needed
                importedRecord.Notes = $"{importedRecord.Notes} | Direct processed to {result.TableName}";
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Direct import record processed successfully. RecordId: {RecordId}, Table: {Table}",
                    importedDataRecordId, result.TableName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing direct import record. RecordId: {RecordId}, Category: {Category}",
                    importedDataRecordId, category);
                result.Message = $"Processing failed: {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<List<string>> GetValidCategoriesAsync()
        {
            // Return list of supported data types for direct import
            var validCategories = new List<string>
            {
                "DP01",
                "LN01", "LN03",
                "GL01", "GL41",
                "DPDA",
                "EI01",
                "RR01"
            };

            return await Task.FromResult(validCategories);
        }

        private string GetTableNameForCategory(string category)
        {
            return category.ToUpper() switch
            {
                "DP01" => "DP01",
                "LN01" => "LN01",
                "LN03" => "LN03",
                "GL01" => "GL01",
                "GL41" => "GL41",
                "DPDA" => "DPDA",
                "EI01" => "EI01",
                "RR01" => "RR01",
                _ => category
            };
        }

        private string ExtractNgayDLFromFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return DateTime.Now.ToString("yyyyMMdd");

            // Pattern to extract YYYYMMDD from filename
            var match = Regex.Match(fileName, @"(\d{8})");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // Fallback patterns
            var dateMatch = Regex.Match(fileName, @"(\d{4})(\d{2})(\d{2})");
            if (dateMatch.Success)
            {
                return $"{dateMatch.Groups[1].Value}{dateMatch.Groups[2].Value}{dateMatch.Groups[3].Value}";
            }

            // If no date found, use current date
            return DateTime.Now.ToString("yyyyMMdd");
        }
    }
}
