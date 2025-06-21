using Microsoft.EntityFrameworkCore;
using System.Text;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Temporal;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service chuyên dụng cho SQL Server Temporal Tables với Columnstore Indexes
    /// Thay thế hoàn toàn SCD Type 2 bằng công nghệ SQL Server native
    /// </summary>
    public interface ITemporalTableService
    {
        Task<TemporalQueryResult<T>> GetTemporalDataAsync<T>(TemporalQueryRequest request) where T : class;
        Task<TemporalHistoryResult<T>> GetHistoryDataAsync<T>(string entityId, DateTime? fromDate = null, DateTime? toDate = null) where T : class;
        Task<T?> GetAsOfDateAsync<T>(string entityId, DateTime asOfDate) where T : class;
        Task<bool> EnableTemporalTableAsync(string tableName);
        Task<bool> CreateColumnstoreIndexAsync(string tableName, string indexName, string[] columns);
        Task<TemporalStatistics> GetTemporalStatisticsAsync(string tableName);
    }

    public class TemporalTableService : ITemporalTableService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TemporalTableService> _logger;

        public TemporalTableService(
            ApplicationDbContext context,
            ILogger<TemporalTableService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 📊 Lấy dữ liệu từ Temporal Table với hiệu suất cao
        /// </summary>
        public async Task<TemporalQueryResult<T>> GetTemporalDataAsync<T>(TemporalQueryRequest request) where T : class
        {
            try
            {
                _logger.LogInformation("🕒 Executing temporal query for {EntityType}", typeof(T).Name);

                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                var sql = BuildTemporalQuery<T>(request);
                _logger.LogDebug("📝 Temporal SQL: {SQL}", sql);

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                
                // Thêm parameters
                AddTemporalParameters(command, request);

                var results = new List<T>();
                using var reader = await command.ExecuteReaderAsync();
                
                // TODO: Map results to T (cần implement mapper)
                // results = MapReaderToEntity<T>(reader);

                return new TemporalQueryResult<T>
                {
                    Data = results,
                    TotalCount = results.Count,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    QueryExecutionTime = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi thực hiện temporal query cho {EntityType}", typeof(T).Name);
                throw;
            }
        }

        /// <summary>
        /// 🔍 Lấy lịch sử thay đổi của một entity
        /// </summary>
        public async Task<TemporalHistoryResult<T>> GetHistoryDataAsync<T>(string entityId, DateTime? fromDate = null, DateTime? toDate = null) where T : class
        {
            try
            {
                var tableName = GetTableName<T>();
                var historyTableName = $"{tableName}_History";

                var sql = $@"
                    SELECT * FROM {tableName} 
                    FOR SYSTEM_TIME ALL
                    WHERE Id = @EntityId";

                if (fromDate.HasValue)
                    sql += " AND SysStartTime >= @FromDate";
                if (toDate.HasValue)
                    sql += " AND SysEndTime <= @ToDate";

                sql += " ORDER BY SysStartTime DESC";

                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                
                var entityIdParam = command.CreateParameter();
                entityIdParam.ParameterName = "@EntityId";
                entityIdParam.Value = entityId;
                command.Parameters.Add(entityIdParam);

                if (fromDate.HasValue)
                {
                    var fromDateParam = command.CreateParameter();
                    fromDateParam.ParameterName = "@FromDate";
                    fromDateParam.Value = fromDate.Value;
                    command.Parameters.Add(fromDateParam);
                }

                if (toDate.HasValue)
                {
                    var toDateParam = command.CreateParameter();
                    toDateParam.ParameterName = "@ToDate";
                    toDateParam.Value = toDate.Value;
                    command.Parameters.Add(toDateParam);
                }

                var results = new List<T>();
                using var reader = await command.ExecuteReaderAsync();
                
                // TODO: Map results
                
                return new TemporalHistoryResult<T>
                {
                    EntityId = entityId,
                    Versions = results,
                    TotalVersions = results.Count,
                    EarliestChange = fromDate,
                    LatestChange = toDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy history cho entity {EntityId}", entityId);
                throw;
            }
        }

        /// <summary>
        /// ⏰ Lấy trạng thái của entity tại một thời điểm cụ thể
        /// </summary>
        public async Task<T?> GetAsOfDateAsync<T>(string entityId, DateTime asOfDate) where T : class
        {
            try
            {
                var tableName = GetTableName<T>();

                var sql = $@"
                    SELECT TOP 1 * FROM {tableName} 
                    FOR SYSTEM_TIME AS OF @AsOfDate
                    WHERE Id = @EntityId";

                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                
                var entityIdParam = command.CreateParameter();
                entityIdParam.ParameterName = "@EntityId";
                entityIdParam.Value = entityId;
                command.Parameters.Add(entityIdParam);

                var asOfDateParam = command.CreateParameter();
                asOfDateParam.ParameterName = "@AsOfDate";
                asOfDateParam.Value = asOfDate;
                command.Parameters.Add(asOfDateParam);

                using var reader = await command.ExecuteReaderAsync();
                
                // TODO: Map single result
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy entity {EntityId} tại thời điểm {AsOfDate}", entityId, asOfDate);
                throw;
            }
        }

        /// <summary>
        /// 🛠️ Bật tính năng Temporal Table cho một bảng
        /// </summary>
        public async Task<bool> EnableTemporalTableAsync(string tableName)
        {
            try
            {
                var sql = $@"
                    -- Thêm columns temporal nếu chưa có
                    IF NOT EXISTS (SELECT * FROM sys.columns 
                                  WHERE object_id = OBJECT_ID('{tableName}') 
                                  AND name = 'SysStartTime')
                    BEGIN
                        ALTER TABLE {tableName} 
                        ADD SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN 
                            CONSTRAINT DF_{tableName}_SysStartTime DEFAULT SYSUTCDATETIME(),
                            SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN 
                            CONSTRAINT DF_{tableName}_SysEndTime DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
                            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
                    END

                    -- Bật System Versioning
                    IF NOT EXISTS (SELECT * FROM sys.tables 
                                  WHERE name = '{tableName}' 
                                  AND temporal_type = 2)
                    BEGIN
                        ALTER TABLE {tableName} 
                        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.{tableName}_History));
                    END";

                await _context.Database.ExecuteSqlRawAsync(sql);
                _logger.LogInformation("✅ Đã bật Temporal Table cho {TableName}", tableName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi bật Temporal Table cho {TableName}", tableName);
                return false;
            }
        }

        /// <summary>
        /// 📈 Tạo Columnstore Index để tăng hiệu suất query
        /// </summary>
        public async Task<bool> CreateColumnstoreIndexAsync(string tableName, string indexName, string[] columns)
        {
            try
            {
                var columnsList = string.Join(", ", columns);
                var sql = $@"
                    -- Kiểm tra index đã tồn tại chưa
                    IF NOT EXISTS (SELECT * FROM sys.indexes 
                                  WHERE name = '{indexName}' 
                                  AND object_id = OBJECT_ID('{tableName}'))
                    BEGIN
                        CREATE NONCLUSTERED COLUMNSTORE INDEX {indexName}
                        ON {tableName} ({columnsList})
                        WITH (COMPRESSION_DELAY = 0);
                    END

                    -- Tạo columnstore index cho history table nếu có
                    IF EXISTS (SELECT * FROM sys.tables WHERE name = '{tableName}_History')
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.indexes 
                                      WHERE name = '{indexName}_History' 
                                      AND object_id = OBJECT_ID('{tableName}_History'))
                        BEGIN
                            CREATE CLUSTERED COLUMNSTORE INDEX {indexName}_History
                            ON {tableName}_History
                            WITH (COMPRESSION_DELAY = 0);
                        END
                    END";

                await _context.Database.ExecuteSqlRawAsync(sql);
                _logger.LogInformation("✅ Đã tạo Columnstore Index {IndexName} cho {TableName}", indexName, tableName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo Columnstore Index {IndexName} cho {TableName}", indexName, tableName);
                return false;
            }
        }

        /// <summary>
        /// 📊 Lấy thống kê về Temporal Table
        /// </summary>
        public async Task<TemporalStatistics> GetTemporalStatisticsAsync(string tableName)
        {
            try
            {
                var sql = $@"
                    SELECT 
                        (SELECT COUNT(*) FROM {tableName}) as CurrentRecords,
                        (SELECT COUNT(*) FROM {tableName}_History) as HistoryRecords,
                        (SELECT MIN(SysStartTime) FROM {tableName}_History) as EarliestChange,
                        (SELECT MAX(SysEndTime) FROM {tableName}_History WHERE SysEndTime < '9999-12-31') as LatestChange,
                        (SELECT COUNT(DISTINCT Id) FROM {tableName}_History) as UniqueEntities";

                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new TemporalStatistics
                    {
                        TableName = tableName,
                        CurrentRecords = reader["CurrentRecords"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CurrentRecords"]),
                        HistoryRecords = reader["HistoryRecords"] == DBNull.Value ? 0 : Convert.ToInt32(reader["HistoryRecords"]),
                        EarliestChange = reader["EarliestChange"] == DBNull.Value ? null : Convert.ToDateTime(reader["EarliestChange"]),
                        LatestChange = reader["LatestChange"] == DBNull.Value ? null : Convert.ToDateTime(reader["LatestChange"]),
                        UniqueEntities = reader["UniqueEntities"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UniqueEntities"]),
                        GeneratedAt = DateTime.UtcNow
                    };
                }

                return new TemporalStatistics { TableName = tableName };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy thống kê temporal cho {TableName}", tableName);
                throw;
            }
        }

        // Helper methods
        private string BuildTemporalQuery<T>(TemporalQueryRequest request)
        {
            var tableName = GetTableName<T>();
            var sql = new StringBuilder();

            sql.Append($"SELECT * FROM {tableName}");

            if (request.AsOfDate.HasValue)
            {
                sql.Append($" FOR SYSTEM_TIME AS OF @AsOfDate");
            }
            else if (request.FromDate.HasValue || request.ToDate.HasValue)
            {
                if (request.FromDate.HasValue && request.ToDate.HasValue)
                {
                    sql.Append($" FOR SYSTEM_TIME BETWEEN @FromDate AND @ToDate");
                }
                else if (request.FromDate.HasValue)
                {
                    sql.Append($" FOR SYSTEM_TIME FROM @FromDate TO '9999-12-31'");
                }
                else
                {
                    sql.Append($" FOR SYSTEM_TIME FROM '1900-01-01' TO @ToDate");
                }
            }

            sql.Append(" WHERE 1=1");

            if (!string.IsNullOrEmpty(request.Filter))
            {
                sql.Append($" AND ({request.Filter})");
            }

            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                sql.Append($" ORDER BY {request.OrderBy}");
            }

            if (request.PageSize > 0)
            {
                sql.Append($" OFFSET {(request.Page - 1) * request.PageSize} ROWS");
                sql.Append($" FETCH NEXT {request.PageSize} ROWS ONLY");
            }

            return sql.ToString();
        }

        private void AddTemporalParameters(System.Data.Common.DbCommand command, TemporalQueryRequest request)
        {
            if (request.AsOfDate.HasValue)
            {
                var param = command.CreateParameter();
                param.ParameterName = "@AsOfDate";
                param.Value = request.AsOfDate.Value;
                command.Parameters.Add(param);
            }

            if (request.FromDate.HasValue)
            {
                var param = command.CreateParameter();
                param.ParameterName = "@FromDate";
                param.Value = request.FromDate.Value;
                command.Parameters.Add(param);
            }

            if (request.ToDate.HasValue)
            {
                var param = command.CreateParameter();
                param.ParameterName = "@ToDate";
                param.Value = request.ToDate.Value;
                command.Parameters.Add(param);
            }
        }

        private string GetTableName<T>()
        {
            // TODO: Implement proper table name resolution
            return typeof(T).Name;
        }
    }

    // Support classes
    public class TemporalQueryRequest
    {
        public DateTime? AsOfDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Filter { get; set; }
        public string? OrderBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class TemporalQueryResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime QueryExecutionTime { get; set; }
    }

    public class TemporalHistoryResult<T>
    {
        public string EntityId { get; set; } = string.Empty;
        public IEnumerable<T> Versions { get; set; } = new List<T>();
        public int TotalVersions { get; set; }
        public DateTime? EarliestChange { get; set; }
        public DateTime? LatestChange { get; set; }
    }

    public class TemporalStatistics
    {
        public string TableName { get; set; } = string.Empty;
        public int CurrentRecords { get; set; }
        public int HistoryRecords { get; set; }
        public DateTime? EarliestChange { get; set; }
        public DateTime? LatestChange { get; set; }
        public int UniqueEntities { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
