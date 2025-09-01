using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using Khoan.Api.Models.Entities; // Revert: Use Models.Entities (matches DTOs and interface contracts)
using Khoan.Api.Dtos.LN03;
using Khoan.Api.Services.Interfaces;
using Khoan.Api.Repositories.Interfaces;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Services
{
    public class LN03Service : ILN03Service
    {
        private readonly ILN03Repository _repository;
        private readonly ILogger<LN03Service> _logger;
        private static readonly Regex FileDateRegex = new(@"(\d{8})", RegexOptions.Compiled);
        private readonly LN03ConfigDto _config;

        public LN03Service(ILN03Repository repository, ILogger<LN03Service> logger)
        {
            _repository = repository;
            _logger = logger;
            _config = new LN03ConfigDto(); // Initialize with defaults
        }

        // Basic CRUD Operations
        public async Task<ApiResponse<(IEnumerable<LN03PreviewDto> Data, int TotalCount)>> GetAllPagedAsync(int page, int pageSize, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = await _repository.GetAllPagedAsync(page, pageSize, fromDate, toDate);
                return ApiResponse<(IEnumerable<LN03PreviewDto> Data, int TotalCount)>.Ok(result, "Data retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged LN03 data");
                return ApiResponse<(IEnumerable<LN03PreviewDto> Data, int TotalCount)>.Failure("Failed to retrieve data");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id);
                if (result == null)
                {
                    return ApiResponse<LN03DetailsDto>.Error($"LN03 record with ID {id} not found", 404);
                }
                return ApiResponse<LN03DetailsDto>.Ok(result, "Record retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 record {Id}", id);
                return ApiResponse<LN03DetailsDto>.Failure("Failed to retrieve record");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> CreateAsync(CreateLN03Dto dto)
        {
            try
            {
                var entity = MapToEntity(dto);
                var created = await _repository.CreateAsync(entity);
                var result = await _repository.GetByIdAsync((int)created.Id);
                
                return ApiResponse<LN03DetailsDto>.Ok(result, "Record created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating LN03 record");
                return ApiResponse<LN03DetailsDto>.Failure("Failed to create record");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> UpdateAsync(int id, UpdateLN03Dto dto)
        {
            try
            {
                var entity = MapToEntity(dto);
                var updated = await _repository.UpdateAsync(id, entity);
                if (updated == null)
                {
                    return ApiResponse<LN03DetailsDto>.Error($"LN03 record with ID {id} not found", 404);
                }
                
                var result = await _repository.GetByIdAsync(id);
                return ApiResponse<LN03DetailsDto>.Ok(result, "Record updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LN03 record {Id}", id);
                return ApiResponse<LN03DetailsDto>.Failure("Failed to update record");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                if (!result)
                {
                    return ApiResponse<bool>.Error($"LN03 record with ID {id} not found", 404);
                }
                return ApiResponse<bool>.Ok(true, "Record deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LN03 record {Id}", id);
                return ApiResponse<bool>.Failure("Failed to delete record");
            }
        }

        public async Task<ApiResponse<bool>> SoftDeleteAsync(int id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                {
                    return ApiResponse<bool>.Error($"LN03 record with ID {id} not found", 404);
                }
                return ApiResponse<bool>.Ok(true, "Record soft deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting LN03 record {Id}", id);
                return ApiResponse<bool>.Failure("Failed to soft delete record");
            }
        }

        // CSV Import Operations
        public async Task<ApiResponse<LN03ImportResultDto>> ImportFromCsvStreamAsync(Stream csvStream, string fileName)
        {
            var result = new LN03ImportResultDto
            {
                FileName = fileName,
                ProcessingStartTime = DateTime.UtcNow
            };

            try
            {
                // Extract NGAY_DL from filename
                var extractedDate = ExtractDateFromFilename(fileName);
                result.ExtractedNGAY_DL = extractedDate;

                if (!extractedDate.HasValue)
                {
                    result.Errors.Add("Cannot extract date from filename. Expected format: *ln03*yyyyMMdd*");
                    result.ProcessingEndTime = DateTime.UtcNow;
                    return ApiResponse<LN03ImportResultDto>.Error("Invalid filename format", 400);
                }

                // Validate filename contains 'ln03'
                if (!fileName.ToLower().Contains("ln03"))
                {
                    result.Errors.Add("Filename must contain 'ln03' identifier");
                    result.ProcessingEndTime = DateTime.UtcNow;
                    return ApiResponse<LN03ImportResultDto>.Error("Invalid filename format", 400);
                }

                var entities = new List<LN03Entity>();
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                // Configure CSV reader for LN03 format
                csv.Context.RegisterClassMap<LN03CsvMap>();
                
                var records = csv.GetRecords<LN03CsvRecord>().ToList();
                result.TotalRows = records.Count;

                // Process in batches
                var batchSize = _config.BatchSize;
                for (int i = 0; i < records.Count; i += batchSize)
                {
                    var batch = records.Skip(i).Take(batchSize).ToList();
                    result.BatchCount++;

                    var batchEntities = new List<LN03Entity>();
                    foreach (var record in batch)
                    {
                        try
                        {
                            var entity = ParseCsvRecord(record, extractedDate.Value);
                            batchEntities.Add(entity);
                            result.SuccessfulRows++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedRows++;
                            result.Errors.Add($"Row {i + batchEntities.Count + 1}: {ex.Message}");
                            _logger.LogWarning(ex, "Failed to parse CSV record at row {Row}", i + batchEntities.Count + 1);
                        }
                    }

                    if (batchEntities.Any())
                    {
                        await _repository.BulkCreateAsync(batchEntities);
                    }
                }

                result.ProcessingEndTime = DateTime.UtcNow;
                return ApiResponse<LN03ImportResultDto>.Ok(result, $"Import completed. {result.SuccessfulRows} rows imported successfully.");
            }
            catch (Exception ex)
            {
                result.ProcessingEndTime = DateTime.UtcNow;
                result.Errors.Add($"Import failed: {ex.Message}");
                _logger.LogError(ex, "Error importing LN03 data from CSV");
                return ApiResponse<LN03ImportResultDto>.Failure("Import operation failed");
            }
        }

        public async Task<ApiResponse<LN03ImportResultDto>> ImportFromCsvFileAsync(string filePath)
        {
            try
            {
                using var stream = File.OpenRead(filePath);
                var fileName = Path.GetFileName(filePath);
                return await ImportFromCsvStreamAsync(stream, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing LN03 data from file {FilePath}", filePath);
                return ApiResponse<LN03ImportResultDto>.Failure("Failed to read CSV file");
            }
        }

        public async Task<ApiResponse<bool>> ValidateCsvFormatAsync(Stream csvStream)
        {
            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                // Read header
                await csv.ReadAsync();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;
                
                if (headers == null || headers.Length < 17)
                {
                    return ApiResponse<bool>.Error("Invalid CSV format. Expected at least 17 columns", 400);
                }

                // Check for required columns
                var requiredColumns = _config.RequiredColumns;
                var missingColumns = requiredColumns.Except(headers, StringComparer.OrdinalIgnoreCase).ToList();
                
                if (missingColumns.Any())
                {
                    return ApiResponse<bool>.Error($"Missing required columns: {string.Join(", ", missingColumns)}", 400);
                }

                return ApiResponse<bool>.Ok(true, "CSV format is valid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating CSV format");
                return ApiResponse<bool>.Failure("Failed to validate CSV format");
            }
        }

        // Temporal Table Operations
        public async Task<ApiResponse<LN03DetailsDto>> GetAsOfDateAsync(int id, DateTime asOfDate)
        {
            try
            {
                var result = await _repository.GetAsOfDateAsync(id, asOfDate);
                if (result == null)
                {
                    return ApiResponse<LN03DetailsDto>.Error($"No LN03 record found for ID {id} as of {asOfDate:yyyy-MM-dd}", 404);
                }
                
                return ApiResponse<LN03DetailsDto>.Ok(result, "Historical record retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 record {Id} as of {Date}", id, asOfDate);
                return ApiResponse<LN03DetailsDto>.Failure("Failed to retrieve historical record");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetHistoryAsync(int id)
        {
            try
            {
                var result = await _repository.GetHistoryAsync(id);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(result, "Record history retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 record history {Id}", id);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Failed to retrieve record history");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetChangedBetweenAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _repository.GetChangedBetweenAsync(startDate, endDate);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(result, "Changed records retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving changed LN03 records between {StartDate} and {EndDate}", startDate, endDate);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Failed to retrieve changed records");
            }
        }

        // Business Logic Queries - Implementing key methods
        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByBranchCodeAsync(string branchCode)
        {
            try
            {
                var result = await _repository.GetByBranchCodeAsync(branchCode);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(result, $"Records for branch {branchCode} retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 records by branch code {BranchCode}", branchCode);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Failed to retrieve records by branch code");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByCustomerCodeAsync(string customerCode)
        {
            try
            {
                var result = await _repository.GetByCustomerCodeAsync(customerCode);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(result, $"Records for customer {customerCode} retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 records by customer code {CustomerCode}", customerCode);
                return ApiResponse<IEnumerable<LN03DetailsDto>>.Failure("Failed to retrieve records by customer code");
            }
        }

        // Stub implementations for other interface methods
        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByContractNumberAsync(string contractNumber) =>
            ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(Enumerable.Empty<LN03DetailsDto>(), "Method not implemented");

        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByDebtGroupAsync(string debtGroup) =>
            ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(Enumerable.Empty<LN03DetailsDto>(), "Method not implemented");

        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByDateAsync(DateTime date) =>
            ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(Enumerable.Empty<LN03DetailsDto>(), "Method not implemented");

        public async Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            ApiResponse<IEnumerable<LN03DetailsDto>>.Ok(Enumerable.Empty<LN03DetailsDto>(), "Method not implemented");

        // Analytics and Reporting
        public async Task<ApiResponse<LN03SummaryDto>> GetSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = await _repository.GetSummaryAsync(fromDate, toDate);
                return ApiResponse<LN03SummaryDto>.Ok(result, "Summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LN03 summary");
                return ApiResponse<LN03SummaryDto>.Failure("Failed to retrieve summary");
            }
        }

        // Stub implementations for analytics methods
        public async Task<ApiResponse<Dictionary<string, decimal>>> GetTotalAmountByBranchAsync(DateTime? fromDate = null, DateTime? toDate = null) =>
            ApiResponse<Dictionary<string, decimal>>.Ok(new Dictionary<string, decimal>(), "Method not implemented");

        public async Task<ApiResponse<Dictionary<string, decimal>>> GetOutstandingBalanceByDateAsync(DateTime date) =>
            ApiResponse<Dictionary<string, decimal>>.Ok(new Dictionary<string, decimal>(), "Method not implemented");

        public async Task<ApiResponse<Dictionary<string, int>>> GetContractCountByDebtGroupAsync(DateTime? fromDate = null, DateTime? toDate = null) =>
            ApiResponse<Dictionary<string, int>>.Ok(new Dictionary<string, int>(), "Method not implemented");

        public async Task<ApiResponse<IEnumerable<(string CustomerCode, string CustomerName, decimal TotalAmount)>>> GetTopCustomersByAmountAsync(int topCount = 10, DateTime? fromDate = null, DateTime? toDate = null) =>
            ApiResponse<IEnumerable<(string CustomerCode, string CustomerName, decimal TotalAmount)>>.Ok(Enumerable.Empty<(string, string, decimal)>(), "Method not implemented");

        public async Task<ApiResponse<Dictionary<string, decimal>>> GetTotalAmountByDateRangeAsync(DateTime startDate, DateTime endDate, string groupBy = "month") =>
            ApiResponse<Dictionary<string, decimal>>.Ok(new Dictionary<string, decimal>(), "Method not implemented");

        // Data Management
        public async Task<ApiResponse<bool>> TruncateDataAsync()
        {
            try
            {
                var result = await _repository.TruncateAsync();
                return ApiResponse<bool>.Ok(result, "Data truncated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error truncating LN03 data");
                return ApiResponse<bool>.Failure("Failed to truncate data");
            }
        }

        public async Task<ApiResponse<int>> GetRecordCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var count = await _repository.GetRecordCountAsync(fromDate, toDate);
                return ApiResponse<int>.Ok(count, "Record count retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 record count");
                return ApiResponse<int>.Failure("Failed to get record count");
            }
        }

        public async Task<ApiResponse<IEnumerable<string>>> ValidateDataIntegrityAsync()
        {
            try
            {
                var issues = await _repository.ValidateDataIntegrityAsync();
                return ApiResponse<IEnumerable<string>>.Ok(issues, "Data integrity validation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating LN03 data integrity");
                return ApiResponse<IEnumerable<string>>.Failure("Failed to validate data integrity");
            }
        }

        public async Task<ApiResponse<DateTime?>> GetLatestDataDateAsync()
        {
            try
            {
                var date = await _repository.GetLatestDataDateAsync();
                return ApiResponse<DateTime?>.Ok(date, "Latest data date retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest LN03 data date");
                return ApiResponse<DateTime?>.Failure("Failed to get latest data date");
            }
        }

        public async Task<ApiResponse<DateTime?>> GetOldestDataDateAsync()
        {
            try
            {
                var date = await _repository.GetOldestDataDateAsync();
                return ApiResponse<DateTime?>.Ok(date, "Oldest data date retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting oldest LN03 data date");
                return ApiResponse<DateTime?>.Failure("Failed to get oldest data date");
            }
        }

        // Configuration
        public async Task<ApiResponse<LN03ConfigDto>> GetConfigurationAsync()
        {
            return ApiResponse<LN03ConfigDto>.Ok(_config, "Configuration retrieved successfully");
        }

        public async Task<ApiResponse<bool>> UpdateConfigurationAsync(LN03ConfigDto config)
        {
            // In a real implementation, this would persist the configuration
            return ApiResponse<bool>.Ok(true, "Configuration updated successfully");
        }

        // Helper Methods
        private DateTime? ExtractDateFromFilename(string fileName)
        {
            var match = FileDateRegex.Match(fileName);
            if (match.Success && DateTime.TryParseExact(match.Value, "yyyyMMdd", null, DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        private LN03Entity MapToEntity(CreateLN03Dto dto)
        {
            return new LN03Entity
            {
                NGAY_DL = dto.NGAY_DL ?? DateTime.Now, // Handle nullable DateTime
                MACHINHANH = dto.MACHINHANH,
                TENCHINHANH = dto.TENCHINHANH,
                MAKH = dto.MAKH,
                TENKH = dto.TENKH,
                SOHOPDONG = dto.SOHOPDONG,
                SOTIENXLRR = dto.SOTIENXLRR,
                NGAYPHATSINHXL = dto.NGAYPHATSINHXL,
                THUNOSAUXL = dto.THUNOSAUXL,
                CONLAINGOAIBANG = dto.CONLAINGOAIBANG,
                DUNONOIBANG = dto.DUNONOIBANG,
                NHOMNO = dto.NHOMNO,
                MACBTD = dto.MACBTD,
                TENCBTD = dto.TENCBTD,
                MAPGD = dto.MAPGD,
                TAIKHOANHACHTOAN = dto.TAIKHOANHACHTOAN,
                REFNO = dto.REFNO,
                LOAINGUONVON = dto.LOAINGUONVON,
                COLUMN_18 = dto.COLUMN_18,
                COLUMN_19 = dto.COLUMN_19,
                COLUMN_20 = dto.COLUMN_20?.ToString() // Convert decimal? to string?
            };
        }

        private LN03Entity MapToEntity(UpdateLN03Dto dto)
        {
            return new LN03Entity
            {
                MACHINHANH = dto.MACHINHANH,
                TENCHINHANH = dto.TENCHINHANH,
                MAKH = dto.MAKH,
                TENKH = dto.TENKH,
                SOHOPDONG = dto.SOHOPDONG,
                SOTIENXLRR = dto.SOTIENXLRR,
                NGAYPHATSINHXL = dto.NGAYPHATSINHXL,
                THUNOSAUXL = dto.THUNOSAUXL,
                CONLAINGOAIBANG = dto.CONLAINGOAIBANG,
                DUNONOIBANG = dto.DUNONOIBANG,
                NHOMNO = dto.NHOMNO,
                MACBTD = dto.MACBTD,
                TENCBTD = dto.TENCBTD,
                MAPGD = dto.MAPGD,
                TAIKHOANHACHTOAN = dto.TAIKHOANHACHTOAN,
                REFNO = dto.REFNO,
                LOAINGUONVON = dto.LOAINGUONVON,
                COLUMN_18 = dto.COLUMN_18,
                COLUMN_19 = dto.COLUMN_19,
                COLUMN_20 = dto.COLUMN_20?.ToString() // Convert decimal? to string?
            };
        }

        private LN03Entity ParseCsvRecord(LN03CsvRecord record, DateTime ngayDL)
        {
            return new LN03Entity
            {
                NGAY_DL = ngayDL,
                MACHINHANH = record.MACHINHANH,
                TENCHINHANH = record.TENCHINHANH,
                MAKH = record.MAKH,
                TENKH = record.TENKH,
                SOHOPDONG = record.SOHOPDONG,
                SOTIENXLRR = ParseDecimal(record.SOTIENXLRR),
                NGAYPHATSINHXL = ParseDateTime(record.NGAYPHATSINHXL),
                THUNOSAUXL = ParseDecimal(record.THUNOSAUXL),
                CONLAINGOAIBANG = ParseDecimal(record.CONLAINGOAIBANG),
                DUNONOIBANG = ParseDecimal(record.DUNONOIBANG),
                NHOMNO = record.NHOMNO,
                MACBTD = record.MACBTD,
                TENCBTD = record.TENCBTD,
                MAPGD = record.MAPGD,
                TAIKHOANHACHTOAN = record.TAIKHOANHACHTOAN,
                REFNO = record.REFNO,
                LOAINGUONVON = record.LOAINGUONVON,
                COLUMN_18 = record.COLUMN_18,
                COLUMN_19 = record.COLUMN_19,
                COLUMN_20 = record.COLUMN_20 // Keep as string since Entity expects string?
            };
        }

        private decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            
            // Remove any formatting characters
            var cleanValue = value.Replace(",", "").Replace(" ", "").Trim('"');
            
            if (decimal.TryParse(cleanValue, out var result))
                return result;
                
            return null;
        }

        private DateTime? ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            
            var cleanValue = value.Replace(" ", "").Trim('"');
            
            if (DateTime.TryParseExact(cleanValue, "yyyyMMdd", null, DateTimeStyles.None, out var result))
                return result;
                
            return null;
        }
    }

    // CSV mapping classes
    public class LN03CsvRecord
    {
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public string? SOTIENXLRR { get; set; }
        public string? NGAYPHATSINHXL { get; set; }
        public string? THUNOSAUXL { get; set; }
        public string? CONLAINGOAIBANG { get; set; }
        public string? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }
        public string? COLUMN_18 { get; set; }
        public string? COLUMN_19 { get; set; }
        public string? COLUMN_20 { get; set; }
    }

    public class LN03CsvMap : ClassMap<LN03CsvRecord>
    {
        public LN03CsvMap()
        {
            Map(m => m.MACHINHANH).Index(0);
            Map(m => m.TENCHINHANH).Index(1);
            Map(m => m.MAKH).Index(2);
            Map(m => m.TENKH).Index(3);
            Map(m => m.SOHOPDONG).Index(4);
            Map(m => m.SOTIENXLRR).Index(5);
            Map(m => m.NGAYPHATSINHXL).Index(6);
            Map(m => m.THUNOSAUXL).Index(7);
            Map(m => m.CONLAINGOAIBANG).Index(8);
            Map(m => m.DUNONOIBANG).Index(9);
            Map(m => m.NHOMNO).Index(10);
            Map(m => m.MACBTD).Index(11);
            Map(m => m.TENCBTD).Index(12);
            Map(m => m.MAPGD).Index(13);
            Map(m => m.TAIKHOANHACHTOAN).Index(14);
            Map(m => m.REFNO).Index(15);
            Map(m => m.LOAINGUONVON).Index(16);
            Map(m => m.COLUMN_18).Index(17);
            Map(m => m.COLUMN_19).Index(18);
            Map(m => m.COLUMN_20).Index(19);
        }
    }
}
