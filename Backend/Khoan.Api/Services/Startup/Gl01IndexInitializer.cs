using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho GL01 tồn tại (xấp xỉ columnstore) – chạy một lần khi khởi động.
    /// Không dùng Temporal; chỉ tạo Nonclustered Indexes an toàn nếu chưa có.
    /// </summary>
    public class Gl01IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Gl01IndexInitializer> _logger;

        public Gl01IndexInitializer(IServiceProvider serviceProvider, ILogger<Gl01IndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho GL01 (xấp xỉ Partitioned Columnstore)
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL01_NGAY_DL ON dbo.GL01 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_DEPT_CODE' AND object_id = OBJECT_ID('dbo.GL01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL01_DEPT_CODE ON dbo.GL01 (DEPT_CODE); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_TAI_KHOAN' AND object_id = OBJECT_ID('dbo.GL01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL01_TAI_KHOAN ON dbo.GL01 (TAI_KHOAN); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_TR_CODE' AND object_id = OBJECT_ID('dbo.GL01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL01_TR_CODE ON dbo.GL01 (TR_CODE); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_MA_KH' AND object_id = OBJECT_ID('dbo.GL01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_GL01_MA_KH ON dbo.GL01 (MA_KH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL01_Analytics' AND object_id = OBJECT_ID('dbo.GL01'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_GL01_Analytics ON dbo.GL01 (NGAY_DL, DEPT_CODE, TAI_KHOAN, DR_CR, LOAI_TIEN); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Partitioned Columnstore Index cho GL01 (Phân tích tối ưu)
                try
                {
                    _logger.LogInformation("🔧 Creating partitioned columnstore index for GL01...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    -- Create partition function if not exists
                                    IF NOT EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = 'PF_GL01_Date')
                                    BEGIN
                                        CREATE PARTITION FUNCTION PF_GL01_Date (datetime2)
                                        AS RANGE RIGHT FOR VALUES (
                                            '2024-01-01', '2024-04-01', '2024-07-01', '2024-10-01', '2025-01-01'
                                        );
                                        PRINT 'Created partition function PF_GL01_Date';
                                    END

                                    -- Create partition scheme if not exists
                                    IF NOT EXISTS (SELECT 1 FROM sys.partition_schemes WHERE name = 'PS_GL01_Date')
                                    BEGIN
                                        CREATE PARTITION SCHEME PS_GL01_Date
                                        AS PARTITION PF_GL01_Date
                                        ALL TO ([PRIMARY]);
                                        PRINT 'Created partition scheme PS_GL01_Date';
                                    END

                                    -- Create partitioned columnstore index
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_Columnstore' AND object_id = OBJECT_ID('dbo.GL01'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL01_Columnstore ON dbo.GL01
                                              (NGAY_DL, DEPT_CODE, TAI_KHOAN, DR_CR, LOAI_TIEN, SO_TIEN, MA_KH)
                                              ON PS_GL01_Date(NGAY_DL);
                                              PRINT '✅ Created partitioned columnstore index IX_GL01_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create partitioned columnstore index IX_GL01_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for GL01.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for GL01 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ GL01 analytics indexes ensured.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho GL01 (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
