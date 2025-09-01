using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho GL41 tồn tại (columnstore approximation) – chạy một lần khi khởi động.
    /// GL41 là Temporal Table với System-versioned, tạo Nonclustered Indexes an toàn.
    /// Business Indexes: NGAY_DL, MA_CN, MA_TK, LOAI_TIEN, LOAI_BT và composite analytics
    /// </summary>
    public class Gl41IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Gl41IndexInitializer> _logger;

        public Gl41IndexInitializer(IServiceProvider serviceProvider, ILogger<Gl41IndexInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Danh sách chỉ mục cần có cho GL41 (xấp xỉ Partitioned Columnstore)
                // GL41 có 13 business columns: MA_CN, LOAI_TIEN, MA_TK, TEN_TK, LOAI_BT + 8 amount columns
                var statements = new[]
                {
                          // Basic analytics rowstore indexes (always safe)
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX IX_GL41_NGAY_DL ON dbo.GL41 (NGAY_DL); END",
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_MA_CN' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX IX_GL41_MA_CN ON dbo.GL41 (MA_CN); END",
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_LOAI_TIEN' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX IX_GL41_LOAI_TIEN ON dbo.GL41 (LOAI_TIEN); END",
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_MA_TK' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX IX_GL41_MA_TK ON dbo.GL41 (MA_TK); END",
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_LOAI_BT' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX IX_GL41_LOAI_BT ON dbo.GL41 (LOAI_BT); END",
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL41_Analytics' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX NCCI_GL41_Analytics ON dbo.GL41 (NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, LOAI_BT); END",
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL41_Amounts' AND object_id = OBJECT_ID('dbo.GL41'))
                              BEGIN CREATE NONCLUSTERED INDEX NCCI_GL41_Amounts ON dbo.GL41 (MA_TK, DN_DAUKY, DC_DAUKY, SBT_NO, SBT_CO); END",

                          // Attempt partitioned columnstore index when supported (skips on Azure SQL Edge)
                          @"DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                              IF (@edition NOT LIKE '%Azure SQL Edge%')
                              BEGIN
                                    BEGIN TRY
                                        -- Create partition function if not exists
                                        IF NOT EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = 'PF_GL41_Date')
                                        BEGIN
                                            CREATE PARTITION FUNCTION PF_GL41_Date (datetime2)
                                            AS RANGE RIGHT FOR VALUES (
                                                '2024-01-01', '2024-04-01', '2024-07-01', '2024-10-01', '2025-01-01'
                                            );
                                            PRINT 'Created partition function PF_GL41_Date';
                                        END

                                        -- Create partition scheme if not exists
                                        IF NOT EXISTS (SELECT 1 FROM sys.partition_schemes WHERE name = 'PS_GL41_Date')
                                        BEGIN
                                            CREATE PARTITION SCHEME PS_GL41_Date
                                            AS PARTITION PF_GL41_Date
                                            ALL TO ([PRIMARY]);
                                            PRINT 'Created partition scheme PS_GL41_Date';
                                        END

                                        -- Create partitioned columnstore index
                                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_Columnstore' AND object_id = OBJECT_ID('dbo.GL41'))
                                        BEGIN
                                             CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL41_Columnstore ON dbo.GL41
                                             (
                                                   NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, TEN_TK,
                                                   DN_DAUKY, DC_DAUKY, SBT_NO, ST_GHINO, SBT_CO, ST_GHICO, DN_CUOIKY, DC_CUOIKY
                                             )
                                             ON PS_GL41_Date(NGAY_DL);
                                             PRINT '✅ Created partitioned columnstore index IX_GL41_Columnstore';
                                        END
                                    END TRY
                                    BEGIN CATCH
                                         PRINT '⚠️ Could not create partitioned columnstore index IX_GL41_Columnstore: ' + ERROR_MESSAGE();
                                    END CATCH
                              END
                              ELSE
                              BEGIN
                                    PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for GL41.';
                              END"
                     };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                _logger.LogInformation("✅ GL41 analytics indexes ensured (temporal table safe).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to initialize GL41 indexes: {Message}", ex.Message);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("✅ GL41 Index Initializer completed successfully.");
            return Task.CompletedTask;
        }
    }
}
