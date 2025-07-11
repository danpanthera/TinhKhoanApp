using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TinhKhoanApp.Api.Models.RawData;
using TinhKhoanApp.Api.Data;
using Microsoft.Extensions.Logging;
using TemporalImportLog = TinhKhoanApp.Api.Models.Temporal.ImportLog;

namespace TinhKhoanApp.Api.Services
{
    // 🆕 Interface mở rộng cho tất cả các bảng SCD Type 2
    public interface IExtendedRawDataImportService : IRawDataImportService
    {
        // 🆕 Methods for additional tables
        Task<ImportResponseDto> ImportLN03DataAsync(ImportRequestDto request, List<LN03History> data);
        Task<ImportResponseDto> ImportEI01DataAsync(ImportRequestDto request, List<EI01History> data);
        Task<ImportResponseDto> ImportDPDADataAsync(ImportRequestDto request, List<DPDAHistory> data);
        Task<ImportResponseDto> ImportDB01DataAsync(ImportRequestDto request, List<DB01History> data);

        Task<ImportResponseDto> ImportBC57DataAsync(ImportRequestDto request, List<BC57History> data);

        // 🆕 Statistics for all tables
        Task<List<TableSummaryDto>> GetAllTablesSummaryAsync();
    }

    // 🆕 Extended service implementation
    public class ExtendedRawDataImportService : RawDataImportService, IExtendedRawDataImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExtendedRawDataImportService> _logger;

        public ExtendedRawDataImportService(ApplicationDbContext context, ILogger<ExtendedRawDataImportService> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        // =======================================
        // 🆕 Import Methods for Additional Tables
        // =======================================

        public async Task<ImportResponseDto> ImportLN03DataAsync(ImportRequestDto request, List<LN03History> data)
        {
            return await ImportDataAsync(request, data, "LN03");
        }

        public async Task<ImportResponseDto> ImportEI01DataAsync(ImportRequestDto request, List<EI01History> data)
        {
            return await ImportDataAsync(request, data, "EI01");
        }

        public async Task<ImportResponseDto> ImportDPDADataAsync(ImportRequestDto request, List<DPDAHistory> data)
        {
            return await ImportDataAsync(request, data, "DPDA");
        }

        public async Task<ImportResponseDto> ImportDB01DataAsync(ImportRequestDto request, List<DB01History> data)
        {
            return await ImportDataAsync(request, data, "DB01");
        }



        public async Task<ImportResponseDto> ImportBC57DataAsync(ImportRequestDto request, List<BC57History> data)
        {
            return await ImportDataAsync(request, data, "BC57");
        }

        // 🔧 Generic Import Method
        // =======================================
        private async Task<ImportResponseDto> ImportDataAsync<T>(
            ImportRequestDto request,
            List<T> data,
            string tableName) where T : class, IExtendedHistoryModel
        {
            var response = new ImportResponseDto
            {
                BatchId = request.BatchId ?? Guid.NewGuid().ToString(),
                Statistics = new ImportStatisticsDto { TableName = tableName }
            };

            var importDate = request.ImportDate ?? DateTime.UtcNow;
            var startTime = DateTime.UtcNow;

            // Tạo import log
            var importLog = new TemporalImportLog
            {
                BatchId = response.BatchId,
                TableName = tableName,
                ImportDate = importDate,
                Status = "PROCESSING",
                TotalRecords = data.Count,
                StartTime = startTime,
                CreatedBy = request.CreatedBy ?? "System"
            };

            _context.ImportLogs.Add(importLog);
            await _context.SaveChangesAsync();

            try
            {
                _logger.LogInformation($"Starting import for {tableName}, BatchId: {response.BatchId}");

                // Setup records with metadata
                SetupRecordsMetadata(data, response.BatchId, importDate);

                // Add records to appropriate DbSet
                await AddRecordsToContext(data, tableName);

                // Save changes
                await _context.SaveChangesAsync();

                // Update import log
                var endTime = DateTime.UtcNow;
                importLog.Status = "COMPLETED";
                importLog.ProcessedRecords = data.Count;
                importLog.NewRecords = data.Count; // Simplified - all records are new
                importLog.EndTime = endTime;

                await _context.SaveChangesAsync();

                response.Success = true;

                _logger.LogInformation($"Completed import for {tableName}, BatchId: {response.BatchId}, Records: {data.Count}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing {tableName}, BatchId: {response.BatchId}");

                // Update import log with error
                importLog.Status = "FAILED";
                importLog.ErrorMessage = ex.Message;
                importLog.EndTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.Success = false;
                return response;
            }
        }

        // =======================================
        // � Helper Methods
        // =======================================

        private void SetupRecordsMetadata<T>(List<T> data, string batchId, DateTime importDate)
        {
            foreach (var record in data)
            {
                // Use reflection to set common properties
                var type = typeof(T);

                var importIdProp = type.GetProperty("ImportId");
                importIdProp?.SetValue(record, batchId);

                var statementDateProp = type.GetProperty("StatementDate");
                statementDateProp?.SetValue(record, importDate.Date);

                var processedDateProp = type.GetProperty("ProcessedDate");
                processedDateProp?.SetValue(record, importDate);

                var effectiveDateProp = type.GetProperty("EffectiveDate");
                effectiveDateProp?.SetValue(record, importDate);

                var isCurrentProp = type.GetProperty("IsCurrent");
                isCurrentProp?.SetValue(record, true);

                var rowVersionProp = type.GetProperty("RowVersion");
                rowVersionProp?.SetValue(record, 1);

                var dataHashProp = type.GetProperty("DataHash");
                dataHashProp?.SetValue(record, CalculateSimpleHash(record));

                var businessKeyProp = type.GetProperty("BusinessKey");
                if (businessKeyProp?.GetValue(record) == null)
                {
                    businessKeyProp?.SetValue(record, Guid.NewGuid().ToString());
                }
            }
        }

        private async Task AddRecordsToContext<T>(List<T> data, string tableName) where T : class
        {
            switch (tableName)
            {
                case "LN03":
                    _context.LN03History.AddRange(data.Cast<LN03History>());
                    break;
                case "EI01":
                    _context.EI01History.AddRange(data.Cast<EI01History>());
                    break;
                case "DPDA":
                    _context.DPDAHistory.AddRange(data.Cast<DPDAHistory>());
                    break;
                case "DB01":
                    _context.DB01History.AddRange(data.Cast<DB01History>());
                    break;

                case "BC57":
                    _context.BC57History.AddRange(data.Cast<BC57History>());
                    break;
                default:
                    throw new ArgumentException($"Unknown table name: {tableName}");
            }
        }

        private string CalculateSimpleHash<T>(T record)
        {
            try
            {
                var json = JsonSerializer.Serialize(record);
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
                    return Convert.ToBase64String(hashBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error calculating hash for record");
                return Guid.NewGuid().ToString("N"); // Fallback hash
            }
        }

        // =======================================
        // 📊 Statistics for All Tables
        // =======================================

        public async Task<List<TableSummaryDto>> GetAllTablesSummaryAsync()
        {
            var summaries = new List<TableSummaryDto>();

            try
            {
                // Helper function to get table summary
                async Task<TableSummaryDto> GetTableSummary<T>(IQueryable<T> query, string tableName, string description)
                    where T : class, IExtendedHistoryModel
                {
                    var records = await query.ToListAsync();
                    return new TableSummaryDto
                    {
                        TableName = tableName,
                        Description = description,
                        TotalRecords = records.Count,
                        UniqueRecords = records.Select(x => x.BusinessKey).Distinct().Count(),
                        LastProcessedDate = records.Any() ? records.Max(x => x.ProcessedDate) : (DateTime?)null
                    };
                }

                // Add all table summaries with current records only
                summaries.Add(await GetTableSummary(
                    _context.LN03History.Where(x => x.IsCurrent),
                    "LN03",
                    "Dữ liệu Nợ XLRR"));

                summaries.Add(await GetTableSummary(
                    _context.EI01History.Where(x => x.IsCurrent),
                    "EI01",
                    "Dữ liệu Mobile Banking"));

                summaries.Add(await GetTableSummary(
                    _context.DPDAHistory.Where(x => x.IsCurrent),
                    "DPDA",
                    "Dữ liệu Phát hành thẻ"));

                summaries.Add(await GetTableSummary(
                    _context.DB01History.Where(x => x.IsCurrent),
                    "DB01",
                    "Sao kê TSDB và Không TSDB"));

                summaries.Add(await GetTableSummary(
                    _context.BC57History.Where(x => x.IsCurrent),
                    "BC57",
                    "Sao kê Lãi dự thu"));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table summaries");
            }

            return summaries.OrderBy(x => x.TableName).ToList();
        }
    }

    // =======================================
    // 🆕 DTOs for Extended Services
    // =======================================
    public class TableSummaryDto
    {
        public string TableName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TotalRecords { get; set; }
        public int UniqueRecords { get; set; }
        public DateTime? LatestStatementDate { get; set; }
        public DateTime? LastProcessedDate { get; set; }
    }
}
