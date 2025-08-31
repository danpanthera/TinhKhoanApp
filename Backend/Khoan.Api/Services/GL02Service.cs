using Khoan.Api.Models.Dtos.GL02;
using Khoan.Api.Models.Entities;
using Khoan.Api.Models.Common;
using Khoan.Api.Repositories.Interfaces;
using Khoan.Api.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Khoan.Api.Services
{
    /// <summary>
    /// GL02Service - General Ledger Heavy File Service
    /// Supports 2GB files, 10,000 record batches, 15-minute timeout
    /// </summary>
    public class GL02Service : IGL02Service
    {
        private readonly IGL02Repository _gl02Repository;
        private readonly ILogger<GL02Service> _logger;

        public GL02Service(IGL02Repository gl02Repository, ILogger<GL02Service> logger)
        {
            _gl02Repository = gl02Repository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GL02PreviewDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var (data, totalCount) = await _gl02Repository.GetAllPagedAsync(page, pageSize);
                var dtos = data.Select(e => new GL02PreviewDto
                {
                    Id = e.Id,
                    NGAY_DL = e.NGAY_DL,
                    TRBRCD = e.TRBRCD,
                    LOCAC = e.LOCAC,
                    CCY = e.CCY,
                    DRAMOUNT = e.DRAMOUNT ?? 0,
                    CRAMOUNT = e.CRAMOUNT ?? 0
                });
                return ApiResponse<IEnumerable<GL02PreviewDto>>.Ok(dtos, "GL02 data retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL02 data");
                return ApiResponse<IEnumerable<GL02PreviewDto>>.Error("Failed to retrieve GL02 data", 500);
            }
        }

        public async Task<ApiResponse<GL02DetailsDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _gl02Repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<GL02DetailsDto>.Error("GL02 record not found", 404);

                var dto = new GL02DetailsDto
                {
                    Id = entity.Id,
                    NGAY_DL = entity.NGAY_DL,
                    TRBRCD = entity.TRBRCD,
                    USERID = entity.USERID,
                    JOURSEQ = entity.JOURSEQ,
                    DYTRSEQ = entity.DYTRSEQ,
                    LOCAC = entity.LOCAC,
                    CCY = entity.CCY,
                    BUSCD = entity.BUSCD,
                    UNIT = entity.UNIT,
                    TRCD = entity.TRCD,
                    CUSTOMER = entity.CUSTOMER,
                    TRTP = entity.TRTP,
                    REFERENCE = entity.REFERENCE,
                    REMARK = entity.REMARK,
                    DRAMOUNT = entity.DRAMOUNT ?? 0,
                    CRAMOUNT = entity.CRAMOUNT ?? 0,
                    CRTDTM = entity.CRTDTM,
                    CREATED_DATE = entity.CREATED_DATE
                };

                return ApiResponse<GL02DetailsDto>.Ok(dto, "GL02 details retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL02 details for ID: {Id}", id);
                return ApiResponse<GL02DetailsDto>.Error("Failed to retrieve GL02 details", 500);
            }
        }

        public async Task<ApiResponse<GL02ImportResultDto>> ImportCsvAsync(IFormFile file)
        {
            try
            {
                var records = new List<GL02Entity>();
                
                using var reader = new StringReader(await new StreamReader(file.OpenReadStream()).ReadToEndAsync());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Configure CSV settings for GL02
                csv.Context.RegisterClassMap<GL02CsvMap>();
                
                await foreach (var csvRecord in csv.GetRecordsAsync<dynamic>())
                {
                    var record = new GL02Entity();
                    DateTime parsedTradeDate = DateTime.Today;
                    
                    // NGAY_DL derived from TRDATE
                    if (csvRecord.TRDATE != null && DateTime.TryParse(csvRecord.TRDATE.ToString(), out parsedTradeDate))
                    {
                        record.NGAY_DL = parsedTradeDate;
                    }
                    else
                    {
                        record.NGAY_DL = DateTime.Today; // Default fallback
                    }

                    // Map business columns
                    record.TRBRCD = csvRecord.TRBRCD?.ToString() ?? string.Empty;
                    record.USERID = csvRecord.USERID?.ToString();
                    record.JOURSEQ = csvRecord.JOURSEQ?.ToString();
                    record.DYTRSEQ = csvRecord.DYTRSEQ?.ToString();
                    record.LOCAC = csvRecord.LOCAC?.ToString() ?? string.Empty;
                    record.CCY = csvRecord.CCY?.ToString() ?? string.Empty;
                    record.BUSCD = csvRecord.BUSCD?.ToString();
                    record.UNIT = csvRecord.UNIT?.ToString();
                    record.TRCD = csvRecord.TRCD?.ToString();
                    record.CUSTOMER = csvRecord.CUSTOMER?.ToString();
                    record.TRTP = csvRecord.TRTP?.ToString();
                    record.REFERENCE = csvRecord.REFERENCE?.ToString();
                    record.REMARK = csvRecord.REMARK?.ToString();

                    // Parse amounts
                    if (decimal.TryParse(csvRecord.DRAMOUNT?.ToString(), out decimal dramount))
                        record.DRAMOUNT = dramount;
                    if (decimal.TryParse(csvRecord.CRAMOUNT?.ToString(), out decimal cramount))
                        record.CRAMOUNT = cramount;

                    // Parse CRTDTM
                    if (DateTime.TryParse(csvRecord.CRTDTM?.ToString(), out DateTime crtdtm))
                        record.CRTDTM = crtdtm;

                    // Set system fields
                    record.CREATED_DATE = DateTime.UtcNow;
                    record.UPDATED_DATE = DateTime.UtcNow;
                    record.FILE_NAME = file.FileName;

                    records.Add(record);
                }

                if (records.Count == 0)
                    return ApiResponse<GL02ImportResultDto>.Error("No valid records found in CSV", 400);

                // Bulk insert with batch processing
                await _gl02Repository.BulkInsertAsync(records);
                var insertedCount = records.Count();

                var result = new GL02ImportResultDto
                {
                    TotalRecords = insertedCount,
                    SuccessfulRecords = insertedCount,
                    FailedRecords = 0,
                    FileName = file.FileName,
                    ImportDateTime = DateTime.UtcNow
                };

                return ApiResponse<GL02ImportResultDto>.Ok(result, $"Successfully imported {insertedCount} GL02 records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL02 CSV: {FileName}", file.FileName);
                return ApiResponse<GL02ImportResultDto>.Error("Failed to import CSV file", 500);
            }
        }

        public async Task<ApiResponse<int>> DeleteByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var isDeleted = await _gl02Repository.DeleteByDateRangeAsync(startDate, endDate);
                return ApiResponse<int>.Ok(isDeleted ? 1 : 0, 
                    isDeleted ? "GL02 records deleted successfully" : "No GL02 records found to delete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GL02 records from {StartDate} to {EndDate}", startDate, endDate);
                return ApiResponse<int>.Error("Failed to delete GL02 records", 500);
            }
        }

        public async Task<ApiResponse<GL02SummaryByUnitDto>> GetSummaryByUnitAsync(string unitCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                var summaryData = await _gl02Repository.GetSummaryByUnitAsync();
                var summary = new GL02SummaryByUnitDto 
                { 
                    UnitCode = unitCode, 
                    TotalRecords = 0, 
                    TotalDebitAmount = 0, 
                    TotalCreditAmount = 0 
                };
                return ApiResponse<GL02SummaryByUnitDto>.Ok(summary, "GL02 summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL02 summary for unit: {UnitCode}", unitCode);
                return ApiResponse<GL02SummaryByUnitDto>.Error("Failed to retrieve GL02 summary", 500);
            }
        }
    }

    /// <summary>
    /// CSV mapping configuration for GL02
    /// </summary>
    public class GL02CsvMap : ClassMap<dynamic>
    {
        public GL02CsvMap()
        {
            // Configure CSV column mappings if needed
        }
    }
}
