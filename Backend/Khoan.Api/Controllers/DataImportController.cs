using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Provides import history and simple preview/cleanup endpoints to support the B9 UI.
    /// Note: DirectImport writes directly to domain tables; this controller surfaces metadata in ImportedDataRecords
    /// and offers lightweight preview by looking up rows in the corresponding table by NGAY_DL (StatementDate).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DataImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataImportController> _logger;

        public DataImportController(ApplicationDbContext context, ILogger<DataImportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns recent import records. The frontend expects a plain array.
        /// </summary>
        [HttpGet("records")]
        public async Task<IActionResult> GetRecords([FromQuery] int take = 200)
        {
            var records = await _context.ImportedDataRecords
                .AsNoTracking()
                .OrderByDescending(r => r.ImportDate)
                .Take(Math.Clamp(take, 1, 1000))
                .ToListAsync();

            return Ok(records);
        }

        /// <summary>
        /// Simple preview by import id: returns first N rows for the table/type and StatementDate.
        /// </summary>
        [HttpGet("preview/{id:int}")]
        public async Task<IActionResult> Preview(int id, [FromQuery] int take = 50)
        {
            var rec = await _context.ImportedDataRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (rec == null)
                return NotFound(new { message = "Import record not found" });

            var dataType = (rec.Category ?? rec.FileType ?? string.Empty).ToUpperInvariant();
            var date = rec.StatementDate?.Date;
            int total = 0;
            List<Dictionary<string, object?>> rows = new();

            try
            {
                switch (dataType)
                {
                    case "DP01":
                        {
                            var q = _context.DP01.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "DPDA":
                        {
                            var q = _context.DPDA.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "EI01":
                        {
                            var q = _context.EI01.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "GL01":
                        {
                            var q = _context.GL01.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "GL02":
                        {
                            var q = _context.GL02.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "GL41":
                        {
                            var q = _context.GL41.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "LN01":
                        {
                            var q = _context.LN01.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "LN03":
                        {
                            var q = _context.LN03.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    case "RR01":
                        {
                            var q = _context.RR01.AsNoTracking();
                            if (date.HasValue) q = q.Where(x => x.NGAY_DL >= date.Value && x.NGAY_DL < date.Value.AddDays(1));
                            total = await q.CountAsync();
                            var list = await q.Take(take).ToListAsync();
                            rows = list.Select(x => ToDict(x)).ToList();
                            break;
                        }
                    default:
                        return BadRequest(new { message = $"Unsupported data type: {dataType}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building preview for {DataType} on {Date}", dataType, date);
                return StatusCode(500, new { message = "Error building preview", error = ex.Message });
            }

            // Build columns from first row
            var columns = rows.Count > 0 ? rows[0].Keys.ToList() : new List<string>();

            // Convert any DateTime values to ISO strings to bypass custom converter issues
            var safeRows = rows.Select(r => r.ToDictionary(kv => kv.Key, kv => NormalizeForJson(kv.Value))).ToList();

            var payload = new Dictionary<string, object?>
            {
                ["id"] = rec.Id,
                ["fileName"] = rec.FileName,
                ["dataType"] = dataType,
                // Use string serialization for DateTimes to bypass custom converter issues
                ["importDate"] = NormalizeForJson(rec.ImportDate),
                ["statementDate"] = NormalizeForJson(rec.StatementDate),
                ["totalRecords"] = total,
                ["columns"] = columns,
                ["previewRows"] = safeRows
            };

            // Pre-serialize with default options (no custom DateTime converters)
            var json = System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false
            });

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        /// <summary>
        /// Ultra-light preview that always returns a minimal JSON object without DateTime types.
        /// Useful as a temporary workaround for DateTime converter issues.
        /// </summary>
        [HttpGet("preview-lite/{id:int}")]
        public async Task<IActionResult> PreviewLite(int id)
        {
            var rec = await _context.ImportedDataRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (rec == null)
                return new ContentResult { Content = "{\"error\":\"not_found\"}", ContentType = "application/json", StatusCode = 404 };

            var dataType = (rec.Category ?? rec.FileType ?? string.Empty).ToUpperInvariant();
            var payload = new { id = rec.Id, fileName = rec.FileName, dataType, totalRecords = 0, columns = Array.Empty<string>(), previewRows = Array.Empty<object>() };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            return new ContentResult { Content = json, ContentType = "application/json", StatusCode = 200 };
        }

        /// <summary>
        /// Delete a single import record (metadata only). Returns number of data rows deleted (0 by default).
        /// </summary>
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteImport(int id)
        {
            var rec = await _context.ImportedDataRecords.FirstOrDefaultAsync(x => x.Id == id);
            if (rec == null)
                return NotFound(new { message = "Import record not found" });

            _context.ImportedDataRecords.Remove(rec);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted import metadata", recordsDeleted = 0 });
        }

        /// <summary>
        /// Delete data rows by dataType and statement date (yyyy-MM-dd or yyyyMMdd). Also cleans metadata records.
        /// </summary>
        [HttpDelete("by-date/{dataType}/{dateStr}")]
        public async Task<IActionResult> DeleteByDate(string dataType, string dateStr)
        {
            dataType = (dataType ?? string.Empty).ToUpperInvariant();
            if (!TryParseDate(dateStr, out var date))
                return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd or yyyyMMdd" });

            int affected = 0;
            try
            {
                switch (dataType)
                {
                    case "DP01":
                        affected = await _context.DP01.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "DPDA":
                        affected = await _context.DPDA.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "EI01":
                        affected = await _context.EI01.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "GL01":
                        affected = await _context.GL01.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "GL02":
                        affected = await _context.GL02.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "GL41":
                        affected = await _context.GL41.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "LN01":
                        affected = await _context.LN01.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "LN03":
                        affected = await _context.LN03.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    case "RR01":
                        affected = await _context.RR01.Where(x => x.NGAY_DL >= date && x.NGAY_DL < date.AddDays(1)).ExecuteDeleteAsync();
                        break;
                    default:
                        return BadRequest(new { message = $"Unsupported data type: {dataType}" });
                }

                // Clean metadata records for same date/type
                var metas = _context.ImportedDataRecords.Where(x => (x.Category ?? x.FileType) == dataType && x.StatementDate.HasValue && x.StatementDate.Value.Date == date.Date);
                _context.ImportedDataRecords.RemoveRange(metas);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting data for {DataType} on {Date}", dataType, date);
                return StatusCode(500, new { message = "Error deleting data", error = ex.Message });
            }

            return Ok(new { message = $"Deleted data for {dataType} on {date:yyyy-MM-dd}", recordsDeleted = affected });
        }

        /// <summary>
        /// Clear all data from a specific table type
        /// </summary>
        [HttpDelete("clear-table/{dataType}")]
        public async Task<IActionResult> ClearTable(string dataType)
        {
            dataType = (dataType ?? string.Empty).ToUpperInvariant();
            int affected = 0;
            
            try
            {
                switch (dataType)
                {
                    case "DP01":
                        affected = await _context.DP01.ExecuteDeleteAsync();
                        break;
                    case "DPDA":
                        affected = await _context.DPDA.ExecuteDeleteAsync();
                        break;
                    case "EI01":
                        affected = await _context.EI01.ExecuteDeleteAsync();
                        break;
                    case "GL01":
                        affected = await _context.GL01.ExecuteDeleteAsync();
                        break;
                    case "GL02":
                        affected = await _context.GL02.ExecuteDeleteAsync();
                        break;
                    case "GL41":
                        affected = await _context.GL41.ExecuteDeleteAsync();
                        break;
                    case "LN01":
                        affected = await _context.LN01.ExecuteDeleteAsync();
                        break;
                    case "LN03":
                        affected = await _context.LN03.ExecuteDeleteAsync();
                        break;
                    case "RR01":
                        affected = await _context.RR01.ExecuteDeleteAsync();
                        break;
                    default:
                        return BadRequest(new { message = $"Unsupported data type: {dataType}" });
                }

                // Clean metadata records for this table type
                var metas = _context.ImportedDataRecords.Where(x => (x.Category ?? x.FileType) == dataType);
                _context.ImportedDataRecords.RemoveRange(metas);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all data for {DataType}", dataType);
                return StatusCode(500, new { message = "Error clearing table data", error = ex.Message });
            }

            return Ok(new { 
                success = true,
                message = $"Successfully cleared all data from {dataType} table", 
                recordsDeleted = affected 
            });
        }

        /// <summary>
        /// Clear all data from all tables
        /// </summary>
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllData()
        {
            int totalAffected = 0;
            var results = new Dictionary<string, int>();
            
            try
            {
                // Clear all data tables
                var tableTypes = new[] { "DP01", "DPDA", "EI01", "GL01", "GL02", "GL41", "LN01", "LN03", "RR01" };
                
                foreach (var table in tableTypes)
                {
                    int affected = 0;
                    switch (table)
                    {
                        case "DP01":
                            affected = await _context.DP01.ExecuteDeleteAsync();
                            break;
                        case "DPDA":
                            affected = await _context.DPDA.ExecuteDeleteAsync();
                            break;
                        case "EI01":
                            affected = await _context.EI01.ExecuteDeleteAsync();
                            break;
                        case "GL01":
                            affected = await _context.GL01.ExecuteDeleteAsync();
                            break;
                        case "GL02":
                            affected = await _context.GL02.ExecuteDeleteAsync();
                            break;
                        case "GL41":
                            affected = await _context.GL41.ExecuteDeleteAsync();
                            break;
                        case "LN01":
                            affected = await _context.LN01.ExecuteDeleteAsync();
                            break;
                        case "LN03":
                            affected = await _context.LN03.ExecuteDeleteAsync();
                            break;
                        case "RR01":
                            affected = await _context.RR01.ExecuteDeleteAsync();
                            break;
                    }
                    results[table] = affected;
                    totalAffected += affected;
                }

                // Clear all metadata records
                var allMetas = _context.ImportedDataRecords.ToList();
                _context.ImportedDataRecords.RemoveRange(allMetas);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully cleared all data from all tables. Total records deleted: {Total}", totalAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all data from all tables");
                return StatusCode(500, new { message = "Error clearing all data", error = ex.Message });
            }

            return Ok(new { 
                success = true,
                message = "Successfully cleared all data from all tables", 
                recordsDeleted = totalAffected,
                tableResults = results
            });
        }

        private static bool TryParseDate(string input, out DateTime date)
        {
            if (DateTime.TryParse(input, out date)) return true;
            if (DateTime.TryParseExact(input, new[] { "yyyy-MM-dd", "yyyyMMdd" }, null, System.Globalization.DateTimeStyles.None, out date)) return true;
            return false;
        }

        private static Dictionary<string, object?> ToDict(object obj)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var p in obj.GetType().GetProperties())
            {
                try
                {
                    var val = p.GetValue(obj);
                    dict[p.Name] = SanitizeValue(val);
                }
                catch
                {
                    // ignore
                }
            }
            return dict;
        }

        private static object? SanitizeValue(object? value)
        {
            if (value is DateTime dt)
            {
                // Avoid serializing DateTime.MinValue with timezone conversion
                if (dt.Year <= 1)
                    return null;
                // Force UTC kind to keep converter safe
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }
            if (value is DateTimeOffset dto)
            {
                if (dto.Year <= 1)
                    return null;
                return dto;
            }
            return value;
        }

        private static object? NormalizeForJson(object? value)
        {
            if (value is DateTime dt)
            {
                if (dt.Year <= 1) return null;
                return dt.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            if (value is DateTimeOffset dto)
            {
                if (dto.Year <= 1) return null;
                return dto.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            return value;
        }
    }
}
