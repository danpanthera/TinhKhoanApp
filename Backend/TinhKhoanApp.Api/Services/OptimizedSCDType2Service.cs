using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Services;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace TinhKhoanApp.Api.Services
{
    public interface IOptimizedSCDType2Service
    {
        Task<SCDType2Result> ProcessSCDImportOptimizedAsync(
            List<Dictionary<string, object>> newRecords,
            string dataSource,
            ImportInfo importInfo);
        
        Task<BulkOperationResult> BulkInsertRecordsAsync(List<RawDataRecord_SCD> records);
        Task<BulkOperationResult> BulkUpdateRecordsAsync(List<RawDataRecord_SCD> records);
        Task<Dictionary<string, CurrentRecordInfo>> LoadCurrentRecordsToMemoryAsync(string dataSource);
    }

    public class OptimizedSCDType2Service : IOptimizedSCDType2Service
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<OptimizedSCDType2Service> _logger;
        private const int BATCH_SIZE = 1000;
        private const int MAX_DEGREE_OF_PARALLELISM = 4;

        public OptimizedSCDType2Service(
            ApplicationDbContext context,
            ICacheService cache,
            ILogger<OptimizedSCDType2Service> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<SCDType2Result> ProcessSCDImportOptimizedAsync(
            List<Dictionary<string, object>> newRecords,
            string dataSource,
            ImportInfo importInfo)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new SCDType2Result();

            try
            {
                _logger.LogInformation("Starting optimized SCD import for {RecordCount} records from {DataSource}", 
                    newRecords.Count, dataSource);

                // 1. Disable auto-detect changes for performance
                _context.ChangeTracker.AutoDetectChangesEnabled = false;

                // 2. Load current records to memory for comparison
                var currentRecordsDict = await LoadCurrentRecordsToMemoryAsync(dataSource);
                _logger.LogDebug("Loaded {Count} current records to memory", currentRecordsDict.Count);

                // 3. Process records in parallel batches
                var recordsToInsert = new ConcurrentBag<RawDataRecord_SCD>();
                var recordsToExpire = new ConcurrentBag<int>();
                var processedNewIds = new ConcurrentBag<string>();

                var actionBlock = new ActionBlock<List<Dictionary<string, object>>>(
                    async batch => await ProcessBatchAsync(
                        batch, currentRecordsDict, dataSource, importInfo, 
                        recordsToInsert, recordsToExpire, processedNewIds),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                        BoundedCapacity = BATCH_SIZE * 2
                    });

                // 4. Send batches for processing
                foreach (var batch in newRecords.Chunk(BATCH_SIZE))
                {
                    await actionBlock.SendAsync(batch.ToList());
                }

                actionBlock.Complete();
                await actionBlock.Completion;

                // 5. Bulk operations
                if (recordsToInsert.Any())
                {
                    var insertResult = await BulkInsertRecordsAsync(recordsToInsert.ToList());
                    result.RecordsInserted = insertResult.SuccessCount;
                    _logger.LogDebug("Bulk inserted {Count} records", insertResult.SuccessCount);
                }

                if (recordsToExpire.Any())
                {
                    await BulkUpdateExpiredRecordsAsync(recordsToExpire.ToList());
                    result.RecordsUpdated = recordsToExpire.Count;
                    _logger.LogDebug("Bulk updated {Count} expired records", recordsToExpire.Count);
                }

                // 6. Handle deleted records (records that exist in current but not in new)
                await HandleDeletedRecordsAsync(
                    currentRecordsDict, processedNewIds.ToHashSet(), dataSource, result);

                // 7. Invalidate cache
                await _cache.RemoveByPatternAsync($"scd:{dataSource}");

                stopwatch.Stop();
                result.ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "SCD import completed: {Inserted} inserted, {Updated} updated, {Deleted} deleted in {ElapsedMs}ms",
                    result.RecordsInserted, result.RecordsUpdated, result.RecordsDeleted, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error during SCD import in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public async Task<Dictionary<string, CurrentRecordInfo>> LoadCurrentRecordsToMemoryAsync(string dataSource)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Try from cache first
                var cacheKey = $"scd:current:{dataSource}";
                var cached = await _cache.GetAsync<Dictionary<string, CurrentRecordInfo>>(cacheKey);
                if (cached != null)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("Loaded {Count} current records from cache in {ElapsedMs}ms", 
                        cached.Count, stopwatch.ElapsedMilliseconds);
                    return cached;
                }

                // Load from database with projection to minimize memory usage
                var currentRecords = await _context.RawDataRecords_SCD
                    .AsNoTracking()
                    .Where(r => r.DataSource == dataSource && r.IsCurrent && !r.IsDeleted)
                    .Select(r => new CurrentRecordInfo
                    {
                        SourceId = r.SourceId,
                        RecordHash = r.RecordHash,
                        VersionNumber = r.VersionNumber,
                        Id = r.Id
                    })
                    .ToDictionaryAsync(r => r.SourceId);

                // Cache for 5 minutes
                await _cache.SetAsync(cacheKey, currentRecords, TimeSpan.FromMinutes(5));

                stopwatch.Stop();
                _logger.LogDebug("Loaded {Count} current records from database in {ElapsedMs}ms", 
                    currentRecords.Count, stopwatch.ElapsedMilliseconds);

                return currentRecords;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error loading current records in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task ProcessBatchAsync(
            List<Dictionary<string, object>> batch,
            Dictionary<string, CurrentRecordInfo> currentRecordsDict,
            string dataSource,
            ImportInfo importInfo,
            ConcurrentBag<RawDataRecord_SCD> recordsToInsert,
            ConcurrentBag<int> recordsToExpire,
            ConcurrentBag<string> processedNewIds)
        {
            await Task.Run(() =>
            {
                foreach (var newRecord in batch)
                {
                    try
                    {
                        var sourceId = GenerateSourceId(newRecord, dataSource);
                        var newRecordHash = GenerateRecordHash(newRecord);

                        // Track processed record
                        processedNewIds.Add(sourceId);

                        if (!currentRecordsDict.TryGetValue(sourceId, out var currentRecord))
                        {
                            // New record - prepare for bulk insert
                            var newSCDRecord = CreateSCDRecord(newRecord, sourceId, newRecordHash, dataSource, importInfo, 1);
                            recordsToInsert.Add(newSCDRecord);
                        }
                        else if (currentRecord.RecordHash != newRecordHash)
                        {
                            // Changed record - expire old and create new
                            recordsToExpire.Add(currentRecord.Id);
                            
                            var updatedSCDRecord = CreateSCDRecord(newRecord, sourceId, newRecordHash, dataSource, importInfo,
                                currentRecord.VersionNumber + 1);
                            recordsToInsert.Add(updatedSCDRecord);
                        }
                        // If hash matches, no action needed (record unchanged)
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing record in batch");
                    }
                }
            });
        }

        public async Task<BulkOperationResult> BulkInsertRecordsAsync(List<RawDataRecord_SCD> records)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new BulkOperationResult { TotalProcessed = records.Count };

            try
            {
                // Use bulk insert strategy
                const int chunkSize = 500; // Smaller chunks for better memory management
                
                foreach (var chunk in records.Chunk(chunkSize))
                {
                    try
                    {
                        _context.RawDataRecords_SCD.AddRange(chunk);
                        await _context.SaveChangesAsync();
                        
                        result.SuccessCount += chunk.Length;
                        _context.ChangeTracker.Clear(); // Clear tracking to free memory
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount += chunk.Length;
                        result.Errors.Add($"Chunk error: {ex.Message}");
                        _logger.LogError(ex, "Error inserting chunk of {Count} records", chunk.Length);
                    }
                }

                stopwatch.Stop();
                result.ProcessingTime = stopwatch.Elapsed;

                _logger.LogDebug("Bulk insert completed: {Success}/{Total} records in {ElapsedMs}ms",
                    result.SuccessCount, result.TotalProcessed, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ProcessingTime = stopwatch.Elapsed;
                _logger.LogError(ex, "Error during bulk insert in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<BulkOperationResult> BulkUpdateRecordsAsync(List<RawDataRecord_SCD> records)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new BulkOperationResult { TotalProcessed = records.Count };

            try
            {
                const int chunkSize = 500;
                
                foreach (var chunk in records.Chunk(chunkSize))
                {
                    try
                    {
                        _context.RawDataRecords_SCD.UpdateRange(chunk);
                        await _context.SaveChangesAsync();
                        
                        result.SuccessCount += chunk.Length;
                        _context.ChangeTracker.Clear();
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount += chunk.Length;
                        result.Errors.Add($"Update chunk error: {ex.Message}");
                        _logger.LogError(ex, "Error updating chunk of {Count} records", chunk.Length);
                    }
                }

                stopwatch.Stop();
                result.ProcessingTime = stopwatch.Elapsed;

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ProcessingTime = stopwatch.Elapsed;
                _logger.LogError(ex, "Error during bulk update in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task BulkUpdateExpiredRecordsAsync(List<int> recordIds)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var now = DateTime.UtcNow;
                
                // Update in chunks to avoid parameter limits
                const int chunkSize = 1000;
                
                foreach (var chunk in recordIds.Chunk(chunkSize))
                {
                    var records = await _context.RawDataRecords_SCD
                        .Where(r => chunk.Contains(r.Id))
                        .ToListAsync();

                    foreach (var record in records)
                    {
                        record.IsCurrent = false;
                        record.ValidTo = now;
                        record.ModifiedAt = now;
                    }

                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                }

                stopwatch.Stop();
                _logger.LogDebug("Bulk expired {Count} records in {ElapsedMs}ms", 
                    recordIds.Count, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error bulk expiring records in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task HandleDeletedRecordsAsync(
            Dictionary<string, CurrentRecordInfo> currentRecordsDict,
            HashSet<string> processedNewIds,
            string dataSource,
            SCDType2Result result)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var deletedSourceIds = currentRecordsDict.Keys
                    .Where(sourceId => !processedNewIds.Contains(sourceId))
                    .ToList();

                if (deletedSourceIds.Any())
                {
                    var deletedRecordIds = currentRecordsDict
                        .Where(kvp => deletedSourceIds.Contains(kvp.Key))
                        .Select(kvp => kvp.Value.Id)
                        .ToList();

                    // Mark as deleted and expire
                    const int chunkSize = 1000;
                    
                    foreach (var chunk in deletedRecordIds.Chunk(chunkSize))
                    {
                        var records = await _context.RawDataRecords_SCD
                            .Where(r => chunk.Contains(r.Id))
                            .ToListAsync();

                        var now = DateTime.UtcNow;
                        foreach (var record in records)
                        {
                            record.IsDeleted = true;
                            record.IsCurrent = false;
                            record.ValidTo = now;
                            record.ModifiedAt = now;
                        }

                        await _context.SaveChangesAsync();
                        _context.ChangeTracker.Clear();
                    }

                    result.RecordsDeleted = deletedSourceIds.Count;
                    
                    _logger.LogDebug("Marked {Count} records as deleted", deletedSourceIds.Count);
                }

                stopwatch.Stop();
                _logger.LogDebug("Handled deleted records in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error handling deleted records in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private string GenerateSourceId(Dictionary<string, object> record, string dataSource)
        {
            // Generate a unique source ID based on key fields
            var keyFields = new List<string>();
            
            // Add standard key fields
            if (record.ContainsKey("AccountNumber"))
                keyFields.Add(record["AccountNumber"]?.ToString() ?? "");
            if (record.ContainsKey("BranchCode"))
                keyFields.Add(record["BranchCode"]?.ToString() ?? "");
            if (record.ContainsKey("CustomerCode"))
                keyFields.Add(record["CustomerCode"]?.ToString() ?? "");

            var keyString = string.Join("|", keyFields);
            return $"{dataSource}:{keyString}";
        }

        private string GenerateRecordHash(Dictionary<string, object> record)
        {
            // Create deterministic hash of record content
            var json = JsonSerializer.Serialize(record, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hashBytes);
        }

        private RawDataRecord_SCD CreateSCDRecord(
            Dictionary<string, object> record,
            string sourceId,
            string recordHash,
            string dataSource,
            ImportInfo importInfo,
            int versionNumber)
        {
            var now = DateTime.UtcNow;
            
            return new RawDataRecord_SCD
            {
                // Id sẽ được tự động generate bởi database (auto-increment)
                SourceId = sourceId,
                DataSource = dataSource,
                DataType = importInfo.DataType,
                BranchCode = record.ContainsKey("BranchCode") ? record["BranchCode"]?.ToString() : null,
                StatementDate = importInfo.StatementDate,
                JsonData = JsonSerializer.Serialize(record),
                RecordHash = recordHash,
                VersionNumber = versionNumber,
                IsCurrent = true,
                IsDeleted = false,
                ValidFrom = now,
                ValidTo = null,
                CreatedAt = now,
                CreatedBy = importInfo.ImportedBy,
                ModifiedAt = null,
                ModifiedBy = null
            };
        }
    }

    public class CurrentRecordInfo
    {
        public string SourceId { get; set; } = string.Empty;
        public string RecordHash { get; set; } = string.Empty;
        public int VersionNumber { get; set; }
        public int Id { get; set; }
    }

    public class BulkOperationResult
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public TimeSpan ProcessingTime { get; set; }
    }

    public class ImportInfo
    {
        public string DataType { get; set; } = string.Empty;
        public DateTime StatementDate { get; set; }
        public string ImportedBy { get; set; } = string.Empty;
    }
}
