using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho GL02 tồn tại (xấp xỉ columnstore) – chạy một lần khi khởi động.
    /// </summary>
    public class Gl02IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Gl02IndexInitializer> _logger;

        public Gl02IndexInitializer(IServiceProvider serviceProvider, ILogger<Gl02IndexInitializer> logger)
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

                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL02'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL02_NGAY_DL ON dbo.GL02 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_UNIT' AND object_id = OBJECT_ID('dbo.GL02'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL02_UNIT ON dbo.GL02 (UNIT); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_TRCD' AND object_id = OBJECT_ID('dbo.GL02'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL02_TRCD ON dbo.GL02 (TRCD); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_CUSTOMER' AND object_id = OBJECT_ID('dbo.GL02'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL02_CUSTOMER ON dbo.GL02 (CUSTOMER); END",
                          // Composite index phân tích thường dùng: (NGAY_DL, TRBRCD, LOCAC, CCY)
                          @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_NgayDl_Trbrcd_Locac_Ccy' AND object_id = OBJECT_ID('dbo.GL02'))
                              BEGIN CREATE NONCLUSTERED INDEX IX_GL02_NgayDl_Trbrcd_Locac_Ccy ON dbo.GL02 (NGAY_DL, TRBRCD, LOCAC, CCY); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL02_Analytics' AND object_id = OBJECT_ID('dbo.GL02'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_GL02_Analytics ON dbo.GL02 (NGAY_DL, UNIT, TRCD, CCY); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Partitioned Columnstore Index cho GL02 (Phân tích tối ưu)
                try
                {
                    _logger.LogInformation("🔧 Creating partitioned columnstore index for GL02...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    -- Create partition function if not exists
                                    IF NOT EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = 'PF_GL02_Date')
                                    BEGIN
                                        CREATE PARTITION FUNCTION PF_GL02_Date (datetime2)
                                        AS RANGE RIGHT FOR VALUES (
                                            '2024-01-01', '2024-04-01', '2024-07-01', '2024-10-01', '2025-01-01'
                                        );
                                        PRINT 'Created partition function PF_GL02_Date';
                                    END

                                    -- Create partition scheme if not exists
                                    IF NOT EXISTS (SELECT 1 FROM sys.partition_schemes WHERE name = 'PS_GL02_Date')
                                    BEGIN
                                        CREATE PARTITION SCHEME PS_GL02_Date
                                        AS PARTITION PF_GL02_Date
                                        ALL TO ([PRIMARY]);
                                        PRINT 'Created partition scheme PS_GL02_Date';
                                    END

                                    -- Create partitioned columnstore index
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_Columnstore' AND object_id = OBJECT_ID('dbo.GL02'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL02_Columnstore ON dbo.GL02
                                              (NGAY_DL, UNIT, TRCD, CCY, CUSTOMER, DR_CR, AMOUNT)
                                              ON PS_GL02_Date(NGAY_DL);
                                              PRINT '✅ Created partitioned columnstore index IX_GL02_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create partitioned columnstore index IX_GL02_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for GL02.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for GL02 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ GL02 analytics indexes ensured.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho GL02");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
