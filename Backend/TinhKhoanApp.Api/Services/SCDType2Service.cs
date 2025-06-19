using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service for handling SCD Type 2 operations
    /// </summary>
    public class SCDType2Service
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SCDType2Service> _logger;

        public SCDType2Service(ApplicationDbContext context, ILogger<SCDType2Service> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Upsert raw data records using SCD Type 2 logic
        /// </summary>
        public async Task<SCDType2Result> UpsertRawDataRecordsAsync(
            IEnumerable<RawDataRecord> newRecords, 
            string dataSource,
            RawDataImport importInfo)
        {
            var result = new SCDType2Result();
            var errors = new List<string>();
            
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                var recordsProcessed = 0;
                var recordsInserted = 0;
                var recordsUpdated = 0;
                var recordsExpired = 0;
                var recordsDeleted = 0;

                // Step 1: Process records present in the new import
                var newSourceIds = new HashSet<string>();
                
                foreach (var newRecord in newRecords)
                {
                    try
                    {
                        // Create business key (source ID) from the JSON data and import info
                        var sourceId = GenerateSourceId(newRecord, importInfo);
                        newSourceIds.Add(sourceId);
                        
                        // Calculate hash of the record content
                        var newRecordHash = CalculateRecordHash(newRecord);
                        
                        // Find current version of this record
                        var currentRecord = await _context.RawDataRecords_SCD
                            .Where(r => r.SourceId == sourceId && r.IsCurrent)
                            .FirstOrDefaultAsync();

                        if (currentRecord == null)
                        {
                            // New record - insert as version 1
                            var scdRecord = MapToSCDRecord(newRecord, sourceId, newRecordHash, dataSource, importInfo);
                            scdRecord.VersionNumber = 1;
                            scdRecord.ValidFrom = DateTime.UtcNow;
                            scdRecord.IsCurrent = true;
                            scdRecord.IsDeleted = false;
                            
                            _context.RawDataRecords_SCD.Add(scdRecord);
                            recordsInserted++;
                        }
                        else
                        {
                            // Check if record has changed
                            if (currentRecord.RecordHash != newRecordHash)
                            {
                                // Record has changed - expire current version
                                currentRecord.ValidTo = DateTime.UtcNow;
                                currentRecord.IsCurrent = false;
                                
                                // Insert new version
                                var newScdRecord = MapToSCDRecord(newRecord, sourceId, newRecordHash, dataSource, importInfo);
                                newScdRecord.VersionNumber = currentRecord.VersionNumber + 1;
                                newScdRecord.ValidFrom = DateTime.UtcNow;
                                newScdRecord.IsCurrent = true;
                                newScdRecord.IsDeleted = false;
                                
                                _context.RawDataRecords_SCD.Add(newScdRecord);
                                recordsUpdated++;
                                recordsExpired++;
                            }
                            // If hash is the same, no action needed
                        }
                        
                        recordsProcessed++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing record ID {newRecord.Id}: {ex.Message}");
                        _logger.LogError(ex, "Error processing record {RecordId}", newRecord.Id);
                    }
                }

                // Step 2: Find and mark records that are no longer in the new import as deleted
                var branchCode = ExtractBranchCodeFromFileName(importInfo.FileName);
                var deletedRecords = await _context.RawDataRecords_SCD
                    .Where(r => r.DataSource == dataSource && 
                               r.BranchCode == branchCode &&
                               r.DataType == importInfo.DataType &&
                               r.IsCurrent && 
                               !r.IsDeleted &&
                               !newSourceIds.Contains(r.SourceId))
                    .ToListAsync();

                foreach (var deletedRecord in deletedRecords)
                {
                    try
                    {
                        // Mark record as deleted - expire current version
                        deletedRecord.ValidTo = DateTime.UtcNow;
                        deletedRecord.IsCurrent = false;
                        
                        // Create a new version marked as deleted
                        var deletedScdRecord = new RawDataRecord_SCD
                        {
                            SourceId = deletedRecord.SourceId,
                            RawDataImportId = importInfo.Id,
                            JsonData = deletedRecord.JsonData,
                            DataType = deletedRecord.DataType,
                            BranchCode = deletedRecord.BranchCode,
                            StatementDate = deletedRecord.StatementDate,
                            OriginalFileName = importInfo.FileName,
                            ImportDate = DateTime.UtcNow,
                            DataSource = dataSource,
                            VersionNumber = deletedRecord.VersionNumber + 1,
                            ValidFrom = DateTime.UtcNow,
                            IsCurrent = true,
                            IsDeleted = true,
                            RecordHash = deletedRecord.RecordHash,
                            CreatedAt = DateTime.UtcNow,
                            ProcessingStatus = "Deleted",
                            ProcessingNotes = $"Record not present in import {importInfo.FileName}"
                        };
                        
                        _context.RawDataRecords_SCD.Add(deletedScdRecord);
                        recordsDeleted++;
                        recordsExpired++;
                        
                        _logger.LogInformation("Record {SourceId} marked as deleted in import {FileName}", 
                            deletedRecord.SourceId, importInfo.FileName);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error marking record {deletedRecord.SourceId} as deleted: {ex.Message}");
                        _logger.LogError(ex, "Error marking record {SourceId} as deleted", deletedRecord.SourceId);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = errors.Count == 0;
                result.Message = errors.Count == 0 
                    ? "SCD Type 2 upsert completed successfully" 
                    : $"SCD Type 2 upsert completed with {errors.Count} errors";
                result.RecordsProcessed = recordsProcessed;
                result.RecordsInserted = recordsInserted;
                result.RecordsUpdated = recordsUpdated;
                result.RecordsExpired = recordsExpired;
                result.RecordsDeleted = recordsDeleted;
                result.Errors = errors;

                _logger.LogInformation("SCD Type 2 upsert completed: {RecordsProcessed} processed, {RecordsInserted} inserted, {RecordsUpdated} updated, {RecordsDeleted} deleted",
                    recordsProcessed, recordsInserted, recordsUpdated, recordsDeleted);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"SCD Type 2 upsert failed: {ex.Message}";
                result.Errors.Add(ex.Message);
                _logger.LogError(ex, "SCD Type 2 upsert failed");
            }

            return result;
        }

        /// <summary>
        /// Get current version of records
        /// </summary>
        public async Task<IEnumerable<RawDataRecord_SCD>> GetCurrentRecordsAsync(
            string? branchCode = null,
            string? dataType = null,
            DateTime? statementDate = null,
            bool includeDeleted = false)
        {
            var query = _context.RawDataRecords_SCD
                .Where(r => r.IsCurrent);

            if (!includeDeleted)
                query = query.Where(r => !r.IsDeleted);

            if (!string.IsNullOrEmpty(branchCode))
                query = query.Where(r => r.BranchCode == branchCode);

            if (!string.IsNullOrEmpty(dataType))
                query = query.Where(r => r.DataType == dataType);

            if (statementDate.HasValue)
                query = query.Where(r => r.StatementDate.Date == statementDate.Value.Date);

            return await query.OrderBy(r => r.BranchCode)
                .ThenBy(r => r.DataType)
                .ThenBy(r => r.StatementDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get historical versions of a record
        /// </summary>
        public async Task<HistoricalQueryResponse<RawDataRecord_SCD>> GetHistoricalRecordsAsync(
            HistoricalQueryRequest request)
        {
            var query = _context.RawDataRecords_SCD.AsQueryable();

            // Filter by source ID if provided
            if (!string.IsNullOrEmpty(request.SourceId))
                query = query.Where(r => r.SourceId == request.SourceId);

            // Filter by as-of date if provided
            if (request.AsOfDate.HasValue)
            {
                query = query.Where(r => r.ValidFrom <= request.AsOfDate.Value &&
                    (r.ValidTo == null || r.ValidTo > request.AsOfDate.Value));
            }

            // Filter by version number if provided
            if (request.VersionNumber.HasValue)
                query = query.Where(r => r.VersionNumber == request.VersionNumber.Value);

            // If not including history, only get current records
            if (!request.IncludeHistory)
                query = query.Where(r => r.IsCurrent);

            var totalRecords = await query.CountAsync();

            var records = await query
                .OrderBy(r => r.SourceId)
                .ThenBy(r => r.VersionNumber)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new HistoricalQueryResponse<RawDataRecord_SCD>
            {
                Records = records,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize)
            };
        }

        /// <summary>
        /// Get snapshot of data as of a specific date
        /// </summary>
        public async Task<IEnumerable<RawDataRecord_SCD>> GetSnapshotAsync(DateTime asOfDate)
        {
            return await _context.RawDataRecords_SCD
                .Where(r => r.ValidFrom <= asOfDate &&
                    (r.ValidTo == null || r.ValidTo > asOfDate))
                .OrderBy(r => r.BranchCode)
                .ThenBy(r => r.DataType)
                .ThenBy(r => r.StatementDate)
                .ToListAsync();
        }

        /// <summary>
        /// Generate a unique source ID for a raw data record from JSON data
        /// </summary>
        private string GenerateSourceId(RawDataRecord record, RawDataImport importInfo)
        {
            // Parse JSON data to extract key fields
            var jsonData = ParseJsonData(record.JsonData);
            
            // Extract branch code from filename (e.g., "7800_GL01_20250531.csv" -> "7800")
            var branchCode = ExtractBranchCodeFromFileName(importInfo.FileName);
            
            // Create business key from available fields and import info
            // Use a combination of import info and data content hash for uniqueness
            var keyComponents = new List<string>
            {
                importInfo.DataType,
                importInfo.StatementDate.ToString("yyyyMMdd"),
                branchCode,
                record.Id.ToString() // Include record ID to ensure uniqueness
            };
            
            var key = string.Join("|", keyComponents);
            
            // Hash the key to create a shorter, consistent identifier
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").Replace("=", "");
            }
        }

        /// <summary>
        /// Extract branch code from filename (e.g., "7800_GL01_20250531.csv" -> "7800")
        /// </summary>
        private string ExtractBranchCodeFromFileName(string fileName)
        {
            try
            {
                var parts = Path.GetFileNameWithoutExtension(fileName).Split('_');
                return parts.Length > 0 ? parts[0] : "UNKNOWN";
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        /// <summary>
        /// Calculate hash of record content for change detection
        /// </summary>
        private string CalculateRecordHash(RawDataRecord record)
        {
            // Hash the JSON data content
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(record.JsonData));
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Map RawDataRecord to SCD record
        /// </summary>
        private RawDataRecord_SCD MapToSCDRecord(RawDataRecord source, string sourceId, string recordHash, string dataSource, RawDataImport importInfo)
        {
            var branchCode = ExtractBranchCodeFromFileName(importInfo.FileName);
            
            return new RawDataRecord_SCD
            {
                SourceId = sourceId,
                RecordHash = recordHash,
                DataSource = dataSource,
                CreatedAt = DateTime.UtcNow,
                
                // Copy the import info
                RawDataImportId = source.RawDataImportId,
                JsonData = source.JsonData,
                DataType = importInfo.DataType,
                BranchCode = branchCode,
                StatementDate = importInfo.StatementDate,
                OriginalFileName = importInfo.FileName,
                ImportDate = importInfo.ImportDate,
                ProcessingStatus = "Pending",
                ProcessingNotes = source.ProcessingNotes
            };
        }

        /// <summary>
        /// Parse JSON data from record
        /// </summary>
        private Dictionary<string, object> ParseJsonData(string jsonData)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData) 
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }
    }
}
