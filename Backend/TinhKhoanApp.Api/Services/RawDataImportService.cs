using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using TinhKhoanApp.Api.Models.RawData;
using TinhKhoanApp.Api.Data;
using Microsoft.Extensions.Logging;

namespace TinhKhoanApp.Api.Services
{
    public class ImportCounts
    {
        public int NewRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int DeletedRecords { get; set; }
    }

    public interface IRawDataImportService
    {
        Task<ImportResponseDto> ImportLN01DataAsync(ImportRequestDto request, List<LN01History> data);
        Task<ImportResponseDto> ImportGL01DataAsync(ImportRequestDto request, List<GL01History> data);
        Task<ImportStatisticsDto> GetImportStatisticsAsync(string tableName);
        Task<List<ImportLog>> GetRecentImportsAsync(int limit = 20);
        Task<List<LN01History>> GetLN01HistoryAsync(string sourceId);
        Task<List<GL01History>> GetGL01HistoryAsync(string sourceId);
        Task<List<LN01History>> GetLN01SnapshotAsync(DateTime snapshotDate);
        Task<List<GL01History>> GetGL01SnapshotAsync(DateTime snapshotDate);
        Task<bool> CleanupOldDataAsync(string tableName, int retainMonths = 12);
    }

    public class RawDataImportService : IRawDataImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RawDataImportService> _logger;

        public RawDataImportService(ApplicationDbContext context, ILogger<RawDataImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImportResponseDto> ImportLN01DataAsync(ImportRequestDto request, List<LN01History> data)
        {
            return await ImportDataInternalAsync(request, data, _context.LN01History);
        }

        public async Task<ImportResponseDto> ImportGL01DataAsync(ImportRequestDto request, List<GL01History> data)
        {
            return await ImportDataInternalAsync(request, data, _context.GL01History);
        }

        private async Task<ImportResponseDto> ImportDataInternalAsync<T>(
            ImportRequestDto request, 
            List<T> data, 
            DbSet<T> dbSet) where T : BaseHistoryModel
        {
            var response = new ImportResponseDto
            {
                BatchId = request.BatchId ?? Guid.NewGuid().ToString(),
                Statistics = new ImportStatisticsDto { TableName = request.TableName }
            };

            var importDate = request.ImportDate ?? DateTime.Now;
            var startTime = DateTime.Now;

            // Tạo import log
            var importLog = new ImportLog
            {
                BatchId = response.BatchId,
                TableName = request.TableName,
                ImportDate = importDate,
                Status = "PROCESSING",
                TotalRecords = data.Count,
                StartTime = startTime,
                CreatedBy = request.CreatedBy
            };

            _context.ImportLogs.Add(importLog);
            await _context.SaveChangesAsync();

            try
            {
                _logger.LogInformation($"Starting import for {request.TableName}, BatchId: {response.BatchId}");

                // Tính hash cho tất cả records
                foreach (var record in data)
                {
                    record.RecordHash = CalculateRecordHash(record);
                    record.ValidFrom = importDate;
                    record.CreatedDate = importDate;
                    record.ModifiedDate = importDate;
                }

                // Lấy tất cả current records
                var currentRecords = await dbSet
                    .Where(x => x.IsCurrent)
                    .ToListAsync();

                var stats = new ImportCounts { NewRecords = 0, UpdatedRecords = 0, DeletedRecords = 0 };
                
                // Process new and updated records
                foreach (var newRecord in data)
                {
                    var existingRecord = currentRecords.FirstOrDefault(x => x.SourceID == newRecord.SourceID);
                    
                    if (existingRecord == null)
                    {
                        // New record
                        newRecord.VersionNumber = 1;
                        dbSet.Add(newRecord);
                        stats.NewRecords++;
                    }
                    else if (existingRecord.RecordHash != newRecord.RecordHash)
                    {
                        // Updated record - expire old version
                        existingRecord.ValidTo = importDate;
                        existingRecord.IsCurrent = false;
                        existingRecord.ModifiedDate = importDate;
                        
                        // Add new version
                        newRecord.VersionNumber = existingRecord.VersionNumber + 1;
                        dbSet.Add(newRecord);
                        stats.UpdatedRecords++;
                    }
                    // Unchanged records - do nothing
                }

                // Process deleted records
                var deletedRecords = currentRecords.Where(existing => 
                    !data.Any(newRecord => newRecord.SourceID == existing.SourceID)).ToList();
                
                foreach (var deletedRecord in deletedRecords)
                {
                    deletedRecord.ValidTo = importDate;
                    deletedRecord.IsCurrent = false;
                    deletedRecord.ModifiedDate = importDate;
                    stats.DeletedRecords++;
                }

                await _context.SaveChangesAsync();

                // Update import log
                var endTime = DateTime.Now;
                importLog.Status = "SUCCESS";
                importLog.EndTime = endTime;
                importLog.Duration = (int)(endTime - startTime).TotalSeconds;
                importLog.ProcessedRecords = data.Count;
                importLog.NewRecords = stats.NewRecords;
                importLog.UpdatedRecords = stats.UpdatedRecords;
                importLog.DeletedRecords = stats.DeletedRecords;

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Import completed successfully";
                response.Statistics = await GetImportStatisticsAsync(request.TableName);

                _logger.LogInformation($"Import completed for {request.TableName}, BatchId: {response.BatchId}. " +
                    $"New: {stats.NewRecords}, Updated: {stats.UpdatedRecords}, Deleted: {stats.DeletedRecords}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Import failed for {request.TableName}, BatchId: {response.BatchId}");

                // Update import log with error
                importLog.Status = "FAILED";
                importLog.EndTime = DateTime.Now;
                importLog.Duration = (int)(DateTime.Now - startTime).TotalSeconds;
                importLog.ErrorMessage = ex.Message;
                await _context.SaveChangesAsync();

                response.Success = false;
                response.Message = "Import failed";
                response.Errors.Add(ex.Message);

                return response;
            }
        }

        public async Task<ImportStatisticsDto> GetImportStatisticsAsync(string tableName)
        {
            var logs = await _context.ImportLogs
                .Where(x => x.TableName == tableName)
                .ToListAsync();

            if (!logs.Any())
            {
                return new ImportStatisticsDto { TableName = tableName };
            }

            var avgDuration = logs
                .Where(x => x.Duration.HasValue)
                .Select(x => (double)x.Duration!.Value)
                .DefaultIfEmpty(0)
                .Average();

            var stats = new ImportStatisticsDto
            {
                TableName = tableName,
                TotalImports = logs.Count,
                SuccessfulImports = logs.Count(x => x.Status == "SUCCESS"),
                FailedImports = logs.Count(x => x.Status == "FAILED"),
                ProcessingImports = logs.Count(x => x.Status == "PROCESSING"),
                LastImportDate = logs.Any() ? logs.Max(x => x.ImportDate) : DateTime.MinValue,
                TotalRecordsProcessed = logs.Sum(x => x.TotalRecords),
                TotalNewRecords = logs.Sum(x => x.NewRecords),
                TotalUpdatedRecords = logs.Sum(x => x.UpdatedRecords),
                TotalDeletedRecords = logs.Sum(x => x.DeletedRecords),
                AvgDurationSeconds = avgDuration,
                SuccessRate = logs.Count > 0 ? logs.Count(x => x.Status == "SUCCESS") * 100.0 / logs.Count : 0
            };

            return stats;
        }

        public async Task<List<ImportLog>> GetRecentImportsAsync(int limit = 20)
        {
            return await _context.ImportLogs
                .OrderByDescending(x => x.ImportDate)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<LN01History>> GetLN01HistoryAsync(string sourceId)
        {
            return await _context.LN01History
                .Where(x => x.SourceID == sourceId)
                .OrderByDescending(x => x.ValidFrom)
                .ToListAsync();
        }

        public async Task<List<GL01History>> GetGL01HistoryAsync(string sourceId)
        {
            return await _context.GL01History
                .Where(x => x.SourceID == sourceId)
                .OrderByDescending(x => x.ValidFrom)
                .ToListAsync();
        }

        public async Task<List<LN01History>> GetLN01SnapshotAsync(DateTime snapshotDate)
        {
            return await _context.LN01History
                .Where(x => snapshotDate >= x.ValidFrom && snapshotDate < x.ValidTo)
                .ToListAsync();
        }

        public async Task<List<GL01History>> GetGL01SnapshotAsync(DateTime snapshotDate)
        {
            return await _context.GL01History
                .Where(x => snapshotDate >= x.ValidFrom && snapshotDate < x.ValidTo)
                .ToListAsync();
        }

        public async Task<bool> CleanupOldDataAsync(string tableName, int retainMonths = 12)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddMonths(-retainMonths);
                
                // Only delete non-current records older than cutoff
                var deletedCount = await _context.Database.ExecuteSqlAsync(
                    $"DELETE FROM {tableName}_History WHERE IsCurrent = 0 AND ValidTo < {cutoffDate}");

                _logger.LogInformation($"Cleaned up {deletedCount} old records from {tableName}_History");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cleanup old data for {tableName}");
                return false;
            }
        }

        private string CalculateRecordHash<T>(T record) where T : BaseHistoryModel
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != nameof(BaseHistoryModel.HistoryID) &&
                           p.Name != nameof(BaseHistoryModel.ValidFrom) &&
                           p.Name != nameof(BaseHistoryModel.ValidTo) &&
                           p.Name != nameof(BaseHistoryModel.IsCurrent) &&
                           p.Name != nameof(BaseHistoryModel.VersionNumber) &&
                           p.Name != nameof(BaseHistoryModel.RecordHash) &&
                           p.Name != nameof(BaseHistoryModel.CreatedDate) &&
                           p.Name != nameof(BaseHistoryModel.ModifiedDate))
                .OrderBy(p => p.Name);

            var sb = new StringBuilder();
            foreach (var property in properties)
            {
                var value = property.GetValue(record)?.ToString() ?? string.Empty;
                sb.Append($"{property.Name}:{value}|");
            }

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
