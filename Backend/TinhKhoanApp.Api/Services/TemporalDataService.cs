using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Temporal;
using System.Text.Json;

namespace TinhKhoanApp.Api.Services
{
    public interface ITemporalDataService
    {
        Task<int> ImportDailyDataAsync(TemporalImportRequest request);
        Task<List<Models.Temporal.RawDataImport>> GetDataAsOfAsync(DateTime asOfDate, string? employeeCode = null, string? branchCode = null, string? kpiCode = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<TemporalDataComparison>> CompareDataBetweenDatesAsync(DateTime date1, DateTime date2, string? employeeCode = null, string? branchCode = null, string? kpiCode = null);
        Task<List<Models.Temporal.RawDataImport>> GetRecordHistoryAsync(string employeeCode, string kpiCode, string branchCode, DateTime? importDate = null);
        Task<List<DailyChangeSummary>> GetDailyChangeSummaryAsync(DateTime reportDate, string? branchCode = null);
        Task<List<PerformanceAnalytics>> GetPerformanceAnalyticsAsync(DateTime startDate, DateTime endDate, string? branchCode = null, string? employeeCode = null);
        Task<(int RowsArchived, int RowsDeleted)> ArchiveOldDataAsync(int archiveMonthsOld = 12, int deleteMonthsOld = 24);
        Task MaintainIndexesAsync();
        Task<object> GetHealthCheckAsync();
    }

    public class TemporalDataService : ITemporalDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TemporalDataService> _logger;

        public TemporalDataService(ApplicationDbContext context, ILogger<TemporalDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> ImportDailyDataAsync(TemporalImportRequest request)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var batchId = $"API_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
                
                // Chuyển đổi dữ liệu sang format JSON phù hợp với cấu trúc quarterly
                var jsonData = JsonSerializer.Serialize(request.Data.Select(d => new
                {
                    Unit = d.BranchCode,
                    Indicator = d.KpiName ?? d.KpiCode,
                    Q1 = d.Value.ToString(),
                    Q2 = (d.Value * 1.1m).ToString(), // Tạo mock data cho Q2-Q4
                    Q3 = (d.Value * 1.2m).ToString(),
                    Q4 = (d.Value * 1.3m).ToString(),
                    Note = d.Note ?? d.Unit ?? "API Import"
                }));

                var parameters = new[]
                {
                    new SqlParameter("@ImportDate", SqlDbType.Date) { Value = request.ImportDate },
                    new SqlParameter("@ImportBatchId", SqlDbType.NVarChar, 50) { Value = batchId },
                    new SqlParameter("@DataType", SqlDbType.NVarChar, 20) { Value = request.DataType },
                    new SqlParameter("@FileName", SqlDbType.NVarChar, 200) { Value = (object?)request.FileName ?? DBNull.Value },
                    new SqlParameter("@ImportedBy", SqlDbType.NVarChar, 100) { Value = request.ImportedBy },
                    new SqlParameter("@JsonData", SqlDbType.NVarChar, -1) { Value = jsonData }
                };

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC sp_ImportDailyRawData @ImportDate, @ImportBatchId, @DataType, @FileName, @ImportedBy, @JsonData";
                    command.Parameters.AddRange(parameters);

                    await _context.Database.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var processedCount = reader.GetInt32("ProcessedRecords");
                            var processingTime = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                            
                            _logger.LogInformation("Imported {Count} records in {ProcessingTime}ms for batch {BatchId}", 
                                processedCount, processingTime, batchId);

                            return processedCount;
                        }
                    }
                }

                return request.Data.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing daily data for date {ImportDate}", request.ImportDate);
                throw;
            }
        }

        public async Task<List<Models.Temporal.RawDataImport>> GetDataAsOfAsync(DateTime asOfDate, string? employeeCode = null, string? branchCode = null, string? kpiCode = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@AsOfDate", SqlDbType.DateTime2) { Value = asOfDate }
            };

            var results = new List<Models.Temporal.RawDataImport>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_GetDataAsOf @AsOfDate";
                command.Parameters.AddRange(parameters);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Models.Temporal.RawDataImport
                        {
                            ImportDate = reader.GetDateTime("ImportDate"),
                            EmployeeCode = reader.IsDBNull("EmployeeCode") ? string.Empty : reader.GetString("EmployeeCode"),
                            KpiCode = reader.IsDBNull("KpiCode") ? string.Empty : reader.GetString("KpiCode"),
                            KpiName = reader.IsDBNull("KpiName") ? string.Empty : reader.GetString("KpiName"),
                            BranchCode = reader.IsDBNull("BranchCode") ? string.Empty : reader.GetString("BranchCode"),
                            DepartmentCode = reader.IsDBNull("DepartmentCode") ? string.Empty : reader.GetString("DepartmentCode"),
                            KpiValue = reader.IsDBNull("Value") ? 0 : reader.GetDecimal("Value"),
                            Unit = reader.IsDBNull("Unit") ? null : reader.GetString("Unit"),
                            DataType = reader.IsDBNull("DataType") ? string.Empty : reader.GetString("DataType"),
                            ImportBatchId = Guid.TryParse(reader.GetString("ImportBatchId"), out var batchId) ? batchId : Guid.NewGuid(),
                            FileName = reader.IsDBNull("FileName") ? string.Empty : reader.GetString("FileName"),
                            CreatedBy = reader.IsDBNull("ImportedBy") ? "SYSTEM" : reader.GetString("ImportedBy"),
                            CreatedDate = reader.GetDateTime("ImportedAt"),
                            ValidFrom = reader.GetDateTime("ValidFrom"),
                            ValidTo = reader.GetDateTime("ValidTo")
                        });
                    }
                }
            }

            return results;
        }

        public async Task<List<TemporalDataComparison>> CompareDataBetweenDatesAsync(DateTime date1, DateTime date2, string? employeeCode = null, string? branchCode = null, string? kpiCode = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Date1", SqlDbType.DateTime2) { Value = date1 },
                new SqlParameter("@Date2", SqlDbType.DateTime2) { Value = date2 },
                new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 50) { Value = employeeCode ?? (object)DBNull.Value },
                new SqlParameter("@BranchCode", SqlDbType.NVarChar, 20) { Value = branchCode ?? (object)DBNull.Value },
                new SqlParameter("@KpiCode", SqlDbType.NVarChar, 50) { Value = kpiCode ?? (object)DBNull.Value }
            };

            var results = new List<TemporalDataComparison>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_CompareDataBetweenDates @Date1, @Date2, @EmployeeCode, @BranchCode, @KpiCode";
                command.Parameters.AddRange(parameters);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new TemporalDataComparison
                        {
                            ImportDate = reader.GetDateTime("ImportDate"),
                            EmployeeCode = reader.GetString("EmployeeCode"),
                            KpiCode = reader.GetString("KpiCode"),
                            KpiName = reader.GetString("KpiName"),
                            BranchCode = reader.GetString("BranchCode"),
                            Value1 = reader.IsDBNull("Value1") ? null : reader.GetDecimal("Value1"),
                            Value2 = reader.IsDBNull("Value2") ? null : reader.GetDecimal("Value2"),
                            ValueChange = reader.IsDBNull("ValueChange") ? null : reader.GetDecimal("ValueChange"),
                            PercentageChange = reader.IsDBNull("PercentageChange") ? null : reader.GetDecimal("PercentageChange"),
                            Unit = reader.GetString("Unit"),
                            ChangeType = reader.GetString("ChangeType")
                        });
                    }
                }
            }

            return results;
        }

        public async Task<List<Models.Temporal.RawDataImport>> GetRecordHistoryAsync(string employeeCode, string kpiCode, string branchCode, DateTime? importDate = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 50) { Value = employeeCode },
                new SqlParameter("@KpiCode", SqlDbType.NVarChar, 50) { Value = kpiCode },
                new SqlParameter("@BranchCode", SqlDbType.NVarChar, 20) { Value = branchCode },
                new SqlParameter("@ImportDate", SqlDbType.Date) { Value = importDate ?? (object)DBNull.Value }
            };

            var results = new List<Models.Temporal.RawDataImport>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_GetRecordHistory @EmployeeCode, @KpiCode, @BranchCode, @ImportDate";
                command.Parameters.AddRange(parameters);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Models.Temporal.RawDataImport
                        {
                            ImportDate = reader.GetDateTime("ImportDate"),
                            EmployeeCode = reader.IsDBNull("EmployeeCode") ? string.Empty : reader.GetString("EmployeeCode"),
                            KpiCode = reader.IsDBNull("KpiCode") ? string.Empty : reader.GetString("KpiCode"),
                            KpiName = reader.IsDBNull("KpiName") ? string.Empty : reader.GetString("KpiName"),
                            BranchCode = reader.IsDBNull("BranchCode") ? string.Empty : reader.GetString("BranchCode"),
                            DepartmentCode = reader.IsDBNull("DepartmentCode") ? string.Empty : reader.GetString("DepartmentCode"),
                            // Fix: Safe casting cho Value field - có thể là string hoặc decimal
                            KpiValue = reader.IsDBNull("Value") ? 0 : 
                                      reader.GetFieldType(reader.GetOrdinal("Value")) == typeof(string) ? 
                                      decimal.TryParse(reader.GetString("Value"), out var val) ? val : 0 :
                                      reader.GetDecimal("Value"),
                            Unit = reader.IsDBNull("Unit") ? null : reader.GetString("Unit"),
                            DataType = reader.IsDBNull("DataType") ? string.Empty : reader.GetString("DataType"),
                            ImportBatchId = reader.IsDBNull("ImportBatchId") ? Guid.NewGuid() : 
                                           Guid.TryParse(reader.GetString("ImportBatchId"), out var batchId) ? batchId : Guid.NewGuid(),
                            FileName = reader.IsDBNull("FileName") ? string.Empty : reader.GetString("FileName"),
                            CreatedBy = reader.IsDBNull("ImportedBy") ? "SYSTEM" : reader.GetString("ImportedBy"),
                            CreatedDate = reader.GetDateTime("ImportedAt"),
                            ValidFrom = reader.GetDateTime("ValidFrom"),
                            ValidTo = reader.GetDateTime("ValidTo")
                        });
                    }
                }
            }

            return results;
        }

        public async Task<List<DailyChangeSummary>> GetDailyChangeSummaryAsync(DateTime reportDate, string? branchCode = null)
        {
            // Fix: Ensure date is within SQL Server range
            var safeReportDate = reportDate.Date;
            if (safeReportDate < new DateTime(1753, 1, 1))
                safeReportDate = new DateTime(1753, 1, 1);
            if (safeReportDate > new DateTime(9999, 12, 31))
                safeReportDate = new DateTime(9999, 12, 31);

            var parameters = new[]
            {
                new SqlParameter("@ReportDate", SqlDbType.Date) { Value = safeReportDate },
                new SqlParameter("@BranchCode", SqlDbType.NVarChar, 20) { Value = branchCode ?? (object)DBNull.Value }
            };

            var results = new List<DailyChangeSummary>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_GetDailyChangeSummary @ReportDate, @BranchCode";
                command.Parameters.AddRange(parameters);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new DailyChangeSummary
                        {
                            ReportDate = reader.GetDateTime("ReportDate"),
                            BranchCode = reader.GetString("BranchCode"),
                            NewRecords = reader.GetInt32("NewRecords"),
                            DeletedRecords = reader.GetInt32("DeletedRecords"),
                            ModifiedRecords = reader.GetInt32("ModifiedRecords"),
                            UnchangedRecords = reader.GetInt32("UnchangedRecords"),
                            TotalComparisons = reader.GetInt32("TotalComparisons")
                        });
                    }
                }
            }

            return results;
        }

        public async Task<List<PerformanceAnalytics>> GetPerformanceAnalyticsAsync(DateTime startDate, DateTime endDate, string? branchCode = null, string? employeeCode = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate },
                new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate },
                new SqlParameter("@BranchCode", SqlDbType.NVarChar, 20) { Value = branchCode ?? (object)DBNull.Value },
                new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 50) { Value = employeeCode ?? (object)DBNull.Value }
            };

            var results = new List<PerformanceAnalytics>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_GetPerformanceAnalytics @StartDate, @EndDate, @BranchCode, @EmployeeCode";
                command.Parameters.AddRange(parameters);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new PerformanceAnalytics
                        {
                            ImportDate = reader.GetDateTime("ImportDate"),
                            BranchCode = reader.GetString("BranchCode"),
                            EmployeeCode = reader.GetString("EmployeeCode"),
                            KpiCode = reader.GetString("KpiCode"),
                            KpiName = reader.GetString("KpiName"),
                            Value = reader.GetDecimal("Value"),
                            Unit = reader.GetString("Unit"),
                            MovingAverage7Days = reader.GetDecimal("MovingAverage7Days"),
                            DayOverDayChange = reader.IsDBNull("DayOverDayChange") ? 0m : reader.GetDecimal("DayOverDayChange"),
                            // Fix: Safe conversion từ any numeric type sang decimal
                            PercentileRank = reader.IsDBNull("PercentileRank") ? 0m : 
                                           reader.GetFieldType(reader.GetOrdinal("PercentileRank")) == typeof(double) ?
                                           (decimal)reader.GetDouble("PercentileRank") :
                                           reader.GetDecimal("PercentileRank"),
                            ValidFrom = reader.GetDateTime("ValidFrom"),
                            ValidTo = reader.GetDateTime("ValidTo")
                        });
                    }
                }
            }

            return results;
        }

        public async Task<(int RowsArchived, int RowsDeleted)> ArchiveOldDataAsync(int archiveMonthsOld = 12, int deleteMonthsOld = 24)
        {
            var parameters = new[]
            {
                new SqlParameter("@ArchiveMonthsOld", SqlDbType.Int) { Value = archiveMonthsOld },
                new SqlParameter("@DeleteMonthsOld", SqlDbType.Int) { Value = deleteMonthsOld }
            };

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_ArchiveOldTemporalData @ArchiveMonthsOld, @DeleteMonthsOld";
                command.Parameters.AddRange(parameters);
                command.CommandTimeout = 3600; // 1 hour timeout for archiving

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return (reader.GetInt32("RowsArchived"), reader.GetInt32("RowsDeleted"));
                    }
                }
            }

            return (0, 0);
        }

        public async Task MaintainIndexesAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_MaintainTemporalIndexes");
            _logger.LogInformation("Temporal table index maintenance completed");
        }

        public async Task<object> GetHealthCheckAsync()
        {
            var healthData = new List<object>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_TemporalHealthCheck";

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Process multiple result sets
                    do
                    {
                        var resultSet = new List<Dictionary<string, object>>();
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
                            }
                            resultSet.Add(row);
                        }
                        if (resultSet.Any())
                        {
                            healthData.Add(resultSet);
                        }
                    } while (await reader.NextResultAsync());
                }
            }

            return healthData;
        }
    }
}
