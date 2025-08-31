using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.GL41;
using Khoan.Api.Models.Entities;
using Khoan.Api.Repositories;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    /// <summary>
    /// Service implementation for GL41 with partitioned columnstore optimization
    /// Handles direct CSV import with filename-based NGAY_DL extraction
    /// Proper decimal conversion for AMOUNT/BALANCE columns (#,###.00 format)
    /// 13 business columns + 4 system columns = 17 total columns
    /// </summary>
    public class GL41Service : IGL41Service
    {
        private readonly IGL41Repository _gl41Repository;
        private readonly ILogger<GL41Service> _logger;

        public GL41Service(IGL41Repository gl41Repository, ILogger<GL41Service> logger)
        {
            _gl41Repository = gl41Repository;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated GL41 data with preview information
        /// </summary>
        public async Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var (entities, totalCount) = await _gl41Repository.GetAllPagedAsync(page, pageSize);
                var dtos = entities.Select(e => new GL41PreviewDto
                {
                    Id = e.Id,
                    NGAY_DL = e.NGAY_DL,
                    MA_CN = e.MA_CN,
                    LOAI_TIEN = e.LOAI_TIEN,
                    MA_TK = e.MA_TK,
                    TEN_TK = e.TEN_TK,
                    DN_DAUKY = e.DN_DAUKY,
                    DC_CUOIKY = e.DC_CUOIKY
                });

                return ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(dtos, "GL41 data retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data");
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Error("Error retrieving GL41 data");
            }
        }

        /// <summary>
        /// Get detailed GL41 record by ID
        /// </summary>
        public async Task<ApiResponse<GL41DetailsDto?>> GetByIdAsync(long id)
        {
            try
            {
                var entity = await _gl41Repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    return ApiResponse<GL41DetailsDto?>.Error("GL41 record not found", 404);
                }

                var dto = new GL41DetailsDto
                {
                    Id = entity.Id,
                    NGAY_DL = entity.NGAY_DL,
                    MA_CN = entity.MA_CN,
                    LOAI_TIEN = entity.LOAI_TIEN,
                    MA_TK = entity.MA_TK,
                    TEN_TK = entity.TEN_TK,
                    LOAI_BT = entity.LOAI_BT,
                    DN_DAUKY = entity.DN_DAUKY,
                    DC_DAUKY = entity.DC_DAUKY,
                    SBT_NO = entity.SBT_NO,
                    ST_GHINO = entity.ST_GHINO,
                    SBT_CO = entity.SBT_CO,
                    ST_GHICO = entity.ST_GHICO,
                    DN_CUOIKY = entity.DN_CUOIKY,
                    DC_CUOIKY = entity.DC_CUOIKY,
                    FILE_NAME = entity.FILE_NAME,
                    CREATED_DATE = entity.CREATED_DATE
                };

                return ApiResponse<GL41DetailsDto?>.Ok(dto, "GL41 details retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 details for ID {Id}", id);
                return ApiResponse<GL41DetailsDto?>.Error("Error retrieving GL41 details");
            }
        }

        /// <summary>
        /// Import GL41 CSV file with direct database insertion
        /// Only processes files containing "gl41" in filename
        /// NGAY_DL extracted from filename (yyyyMMdd format)
        /// </summary>
        public async Task<ApiResponse<GL41ImportResultDto>> ImportCsvAsync(IFormFile file, string? fileName = null)
        {
            var startTime = DateTime.UtcNow;
            var actualFileName = fileName ?? file.FileName;
            
            try
            {
                // Validate file contains "gl41"
                if (!actualFileName.ToLower().Contains("gl41"))
                {
                    return ApiResponse<GL41ImportResultDto>.Error("File must contain 'gl41' in filename");
                }

                // Extract NGAY_DL from filename (expect format: *gl41_yyyyMMdd* or *yyyyMMdd*gl41*)
                var extractedDate = ExtractDateFromFilename(actualFileName);
                if (!extractedDate.HasValue)
                {
                    return ApiResponse<GL41ImportResultDto>.Error("Cannot extract date from filename. Expected format: gl41_yyyyMMdd");
                }

                var records = new List<GL41Entity>();
                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    BadDataFound = null // Ignore bad data
                });

                // Configure CSV mapping for GL41 columns
                csv.Context.RegisterClassMap<GL41CsvMap>();
                
                await foreach (var csvRecord in csv.GetRecordsAsync<dynamic>())
                {
                    var record = new GL41Entity();
                    
                    // Set NGAY_DL from extracted filename date
                    record.NGAY_DL = extractedDate.Value;

                    // Map 13 business columns
                    record.MA_CN = GetStringValue(csvRecord.MA_CN);
                    record.LOAI_TIEN = GetStringValue(csvRecord.LOAI_TIEN);
                    record.MA_TK = GetStringValue(csvRecord.MA_TK);
                    record.TEN_TK = GetStringValue(csvRecord.TEN_TK);
                    record.LOAI_BT = GetStringValue(csvRecord.LOAI_BT);

                    // Convert AMOUNT/BALANCE columns - handle #,###.00 format
                    record.DN_DAUKY = ParseDecimal(GetStringValue(csvRecord.DN_DAUKY));
                    record.DC_DAUKY = ParseDecimal(GetStringValue(csvRecord.DC_DAUKY));
                    record.SBT_NO = ParseDecimal(GetStringValue(csvRecord.SBT_NO));
                    record.ST_GHINO = ParseDecimal(GetStringValue(csvRecord.ST_GHINO));
                    record.SBT_CO = ParseDecimal(GetStringValue(csvRecord.SBT_CO));
                    record.ST_GHICO = ParseDecimal(GetStringValue(csvRecord.ST_GHICO));
                    record.DN_CUOIKY = ParseDecimal(GetStringValue(csvRecord.DN_CUOIKY));
                    record.DC_CUOIKY = ParseDecimal(GetStringValue(csvRecord.DC_CUOIKY));

                    // Set system columns
                    record.FILE_NAME = actualFileName;
                    record.CREATED_DATE = DateTime.UtcNow;

                    records.Add(record);

                    // Process in batches of 10,000 for performance
                    if (records.Count >= 10000)
                    {
                        await _gl41Repository.BulkInsertAsync(records);
                        records.Clear();
                    }
                }

                // Process remaining records
                if (records.Any())
                {
                    await _gl41Repository.BulkInsertAsync(records);
                }

                var endTime = DateTime.UtcNow;
                var result = new GL41ImportResultDto
                {
                    FileName = actualFileName,
                    TotalRecords = records.Count,
                    SuccessfulRecords = records.Count,
                    FailedRecords = 0,
                    ErrorMessages = new List<string>(),
                    ImportDateTime = endTime,
                    ProcessingDuration = endTime - startTime
                };

                return ApiResponse<GL41ImportResultDto>.Ok(result, "GL41 CSV import completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL41 CSV file {FileName}", actualFileName);
                var endTime = DateTime.UtcNow;
                var errorResult = new GL41ImportResultDto
                {
                    FileName = actualFileName,
                    TotalRecords = 0,
                    SuccessfulRecords = 0,
                    FailedRecords = 1,
                    ErrorMessages = new List<string> { ex.Message },
                    ImportDateTime = endTime,
                    ProcessingDuration = endTime - startTime
                };

                return ApiResponse<GL41ImportResultDto>.Ok(errorResult, "GL41 CSV import completed with errors");
            }
        }

        /// <summary>
        /// Delete GL41 records by date range
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var deletedCount = await _gl41Repository.DeleteByDateRangeAsync(fromDate, toDate);
                _logger.LogInformation("Deleted {Count} GL41 records from {FromDate} to {ToDate}", deletedCount, fromDate, toDate);
                
                return ApiResponse<bool>.Ok(deletedCount > 0, $"Deleted {deletedCount} GL41 records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GL41 records");
                return ApiResponse<bool>.Error("Error deleting GL41 records");
            }
        }

        /// <summary>
        /// Get GL41 summary analytics by unit
        /// </summary>
        public async Task<ApiResponse<IEnumerable<GL41SummaryByUnitDto>>> GetSummaryByUnitAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var summaries = await _gl41Repository.GetSummaryByUnitAsync(fromDate, toDate);
                var dtos = summaries.Select(s => new GL41SummaryByUnitDto
                {
                    UnitCode = (string)s.GetType().GetProperty("UnitCode")?.GetValue(s) ?? "",
                    TotalRecords = (int)s.GetType().GetProperty("TotalRecords")?.GetValue(s),
                    TotalDebitBalance = (decimal)s.GetType().GetProperty("TotalDebitBalance")?.GetValue(s),
                    TotalCreditBalance = (decimal)s.GetType().GetProperty("TotalCreditBalance")?.GetValue(s),
                    ReportDate = toDate,
                    Currencies = (List<string>)s.GetType().GetProperty("Currencies")?.GetValue(s) ?? new List<string>()
                });

                return ApiResponse<IEnumerable<GL41SummaryByUnitDto>>.Ok(dtos, "GL41 summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 summary");
                return ApiResponse<IEnumerable<GL41SummaryByUnitDto>>.Error("Error retrieving GL41 summary");
            }
        }

        /// <summary>
        /// Get GL41 analytics configuration
        /// </summary>
        public async Task<ApiResponse<GL41AnalyticsConfigDto>> GetAnalyticsConfigAsync()
        {
            var config = new GL41AnalyticsConfigDto
            {
                MaxFileSizeBytes = 2L * 1024 * 1024 * 1024, // 2GB
                MaxBatchSize = 10000,
                TimeoutMinutes = 15,
                AllowedFilePattern = "*gl41*",
                SupportedColumns = 13,
                RequiredColumns = new[] { "MA_CN", "LOAI_TIEN", "MA_TK" }
            };

            return ApiResponse<GL41AnalyticsConfigDto>.Ok(config, "GL41 analytics configuration retrieved");
        }

        /// <summary>
        /// Get GL41 records by unit code
        /// </summary>
        public async Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            try
            {
                var entities = await _gl41Repository.GetByUnitCodeAsync(unitCode, maxResults);
                var dtos = entities.Select(e => new GL41PreviewDto
                {
                    Id = e.Id,
                    NGAY_DL = e.NGAY_DL,
                    MA_CN = e.MA_CN,
                    LOAI_TIEN = e.LOAI_TIEN,
                    MA_TK = e.MA_TK,
                    TEN_TK = e.TEN_TK,
                    DN_DAUKY = e.DN_DAUKY,
                    DC_CUOIKY = e.DC_CUOIKY
                });

                return ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(dtos, $"GL41 data for unit {unitCode} retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data for unit {UnitCode}", unitCode);
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Error("Error retrieving GL41 data by unit");
            }
        }

        /// <summary>
        /// Get GL41 records by account code
        /// </summary>
        public async Task<ApiResponse<IEnumerable<GL41PreviewDto>>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            try
            {
                var entities = await _gl41Repository.GetByAccountCodeAsync(accountCode, maxResults);
                var dtos = entities.Select(e => new GL41PreviewDto
                {
                    Id = e.Id,
                    NGAY_DL = e.NGAY_DL,
                    MA_CN = e.MA_CN,
                    LOAI_TIEN = e.LOAI_TIEN,
                    MA_TK = e.MA_TK,
                    TEN_TK = e.TEN_TK,
                    DN_DAUKY = e.DN_DAUKY,
                    DC_CUOIKY = e.DC_CUOIKY
                });

                return ApiResponse<IEnumerable<GL41PreviewDto>>.Ok(dtos, $"GL41 data for account {accountCode} retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data for account {AccountCode}", accountCode);
                return ApiResponse<IEnumerable<GL41PreviewDto>>.Error("Error retrieving GL41 data by account");
            }
        }

        /// <summary>
        /// Get GL41 balance summary for specific unit and date
        /// </summary>
        public async Task<ApiResponse<decimal>> GetBalanceSummaryAsync(string unitCode, DateTime? date = null)
        {
            try
            {
                var closingBalance = await _gl41Repository.GetTotalClosingBalanceByUnitAsync(unitCode, date);
                return ApiResponse<decimal>.Ok(closingBalance, $"Balance summary for unit {unitCode} retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving balance summary for unit {UnitCode}", unitCode);
                return ApiResponse<decimal>.Error("Error retrieving balance summary");
            }
        }

        /// <summary>
        /// Check if GL41 data exists for specific date
        /// </summary>
        public async Task<ApiResponse<bool>> HasDataForDateAsync(DateTime date)
        {
            try
            {
                var hasData = await _gl41Repository.HasDataForDateAsync(date);
                return ApiResponse<bool>.Ok(hasData, $"GL41 data check for {date:dd/MM/yyyy} completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking GL41 data for date {Date}", date);
                return ApiResponse<bool>.Error("Error checking GL41 data existence");
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Extract date from GL41 filename (expect format: *gl41_yyyyMMdd* or *yyyyMMdd*gl41*)
        /// </summary>
        private DateTime? ExtractDateFromFilename(string fileName)
        {
            try
            {
                // Remove file extension
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                
                // Look for 8-digit date pattern
                var datePattern = @"(\d{8})";
                var match = System.Text.RegularExpressions.Regex.Match(nameWithoutExtension, datePattern);
                
                if (match.Success && DateTime.TryParseExact(match.Value, "yyyyMMdd", null, DateTimeStyles.None, out var date))
                {
                    return date;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get string value from dynamic CSV record
        /// </summary>
        private string GetStringValue(dynamic value)
        {
            return value?.ToString()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Parse decimal value handling #,###.00 format
        /// </summary>
        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // Remove commas and spaces, handle decimal separators
            value = value.Replace(",", "").Replace(" ", "").Trim();
            
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// CSV mapping configuration for GL41
    /// </summary>
    public class GL41CsvMap : ClassMap<dynamic>
    {
        public GL41CsvMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
        }
    }
}
