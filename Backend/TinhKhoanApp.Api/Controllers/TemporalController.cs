using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Models.Temporal;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller chuyên dụng cho các thao tác SQL Server Temporal Tables
    /// Thay thế hoàn toàn cho SCD Type 2 controllers
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalController : ControllerBase
    {
        private readonly ITemporalTableService _temporalService;
        private readonly ILogger<TemporalController> _logger;

        public TemporalController(
            ITemporalTableService temporalService,
            ILogger<TemporalController> logger)
        {
            _temporalService = temporalService;
            _logger = logger;
        }

        /// <summary>
        /// 🕒 Query temporal data with advanced filtering and pagination
        /// </summary>
        [HttpPost("query")]
        public async Task<ActionResult<TemporalQueryResult<RawDataImport>>> QueryTemporalData(
            [FromBody] TemporalQueryRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Executing temporal query: {Request}", request);
                
                var result = await _temporalService.GetTemporalDataAsync<RawDataImport>(request);
                
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = $"Retrieved {result.TotalCount} records successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error executing temporal query");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 🕒 Query temporal data with GET method (for frontend compatibility)
        /// </summary>
        [HttpGet("query/{tableName?}")]
        public async Task<ActionResult<TemporalQueryResult<RawDataImport>>> QueryTemporalDataGet(
            string? tableName = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "desc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? departmentCode = null,
            [FromQuery] string? employeeCode = null,
            [FromQuery] string? kpiCode = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var request = new TemporalQueryRequest
                {
                    Page = page,
                    PageSize = Math.Min(pageSize, 100), // Max 100 records per page
                    OrderBy = sortBy ?? "ImportDate",
                    FromDate = fromDate,
                    ToDate = toDate,
                    Filter = searchTerm
                };

                _logger.LogInformation("🔍 Executing temporal GET query for {TableName} with filters", tableName ?? "RawDataImport");
                
                var result = await _temporalService.GetTemporalDataAsync<RawDataImport>(request);
                
                return Ok(new
                {
                    success = true,
                    records = result.Data,
                    totalCount = result.TotalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                    message = $"Retrieved {result.Data?.Count() ?? 0} records successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error executing temporal GET query");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 📚 Get complete history of an entity
        /// </summary>
        [HttpGet("history/{entityId}")]
        public async Task<ActionResult<TemporalHistoryResult<RawDataImport>>> GetEntityHistory(
            string entityId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                _logger.LogInformation("📚 Getting history for entity: {EntityId}", entityId);
                
                var result = await _temporalService.GetHistoryDataAsync<RawDataImport>(entityId, fromDate, toDate);
                
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = $"Retrieved {result.TotalVersions} versions successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting entity history for {EntityId}", entityId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// ⏰ Get entity state as of specific date
        /// </summary>
        [HttpGet("as-of/{entityId}")]
        public async Task<ActionResult<RawDataImport>> GetAsOfDate(
            string entityId,
            [FromQuery] DateTime asOfDate)
        {
            try
            {
                _logger.LogInformation("⏰ Getting entity {EntityId} as of {AsOfDate}", entityId, asOfDate);
                
                var result = await _temporalService.GetAsOfDateAsync<RawDataImport>(entityId, asOfDate);
                
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"No data found for entity {entityId} as of {asOfDate}"
                    });
                }
                
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Entity retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting entity as of date");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 🛠️ Enable temporal table for a specific table
        /// </summary>
        [HttpPost("enable/{tableName}")]
        public async Task<ActionResult> EnableTemporalTable(string tableName)
        {
            try
            {
                _logger.LogInformation("🛠️ Enabling temporal table: {TableName}", tableName);
                
                var success = await _temporalService.EnableTemporalTableAsync(tableName);
                
                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Temporal table enabled successfully for {tableName}"
                    });
                }
                
                return BadRequest(new
                {
                    success = false,
                    message = $"Failed to enable temporal table for {tableName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enabling temporal table {TableName}", tableName);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 📈 Create columnstore index for performance optimization
        /// </summary>
        [HttpPost("index/{tableName}")]
        public async Task<ActionResult> CreateColumnstoreIndex(
            string tableName,
            [FromBody] CreateIndexRequest request)
        {
            try
            {
                _logger.LogInformation("📈 Creating columnstore index for: {TableName}", tableName);
                
                var success = await _temporalService.CreateColumnstoreIndexAsync(
                    tableName, 
                    request.IndexName, 
                    request.Columns);
                
                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Columnstore index created successfully for {tableName}"
                    });
                }
                
                return BadRequest(new
                {
                    success = false,
                    message = $"Failed to create columnstore index for {tableName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating columnstore index for {TableName}", tableName);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 📊 Get temporal table statistics
        /// </summary>
        [HttpGet("statistics/{tableName}")]
        public async Task<ActionResult<TemporalStatistics>> GetTemporalStatistics(string tableName)
        {
            try
            {
                _logger.LogInformation("📊 Getting statistics for: {TableName}", tableName);
                
                var stats = await _temporalService.GetTemporalStatisticsAsync(tableName);
                
                return Ok(new
                {
                    success = true,
                    data = stats,
                    message = "Statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting statistics for {TableName}", tableName);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 🔍 Compare data between two dates (replaces SCD Type 2 comparison)
        /// </summary>
        [HttpPost("compare")]
        public async Task<ActionResult> CompareTemporalData([FromBody] CompareTemporalDataRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Comparing temporal data between {Date1} and {Date2}", 
                    request.Date1, request.Date2);

                var query1 = new TemporalQueryRequest 
                { 
                    AsOfDate = request.Date1,
                    Filter = request.Filter,
                    PageSize = int.MaxValue // Get all records for comparison
                };
                
                var query2 = new TemporalQueryRequest 
                { 
                    AsOfDate = request.Date2,
                    Filter = request.Filter,
                    PageSize = int.MaxValue
                };

                var result1 = await _temporalService.GetTemporalDataAsync<RawDataImport>(query1);
                var result2 = await _temporalService.GetTemporalDataAsync<RawDataImport>(query2);

                // TODO: Implement comparison logic here
                var comparison = new
                {
                    Date1Records = result1.TotalCount,
                    Date2Records = result2.TotalCount,
                    Difference = result2.TotalCount - result1.TotalCount,
                    // Add more comparison metrics as needed
                };

                return Ok(new
                {
                    success = true,
                    data = comparison,
                    message = "Data comparison completed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error comparing temporal data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateIndexRequest
    {
        public string IndexName { get; set; } = string.Empty;
        public string[] Columns { get; set; } = Array.Empty<string>();
    }

    public class CompareTemporalDataRequest
    {
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
        public string? Filter { get; set; }
    }
}
