using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho RR01 tồn tại (Temporal Table safe) – chạy một lần khi khởi động.
    /// Tạo Nonclustered Indexes an toàn cho các truy vấn phổ biến.
    /// </summary>
    public class Rr01IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Rr01IndexInitializer> _logger;

        public Rr01IndexInitializer(IServiceProvider serviceProvider, ILogger<Rr01IndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho RR01
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_NGAY_DL' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_RR01_NGAY_DL ON dbo.RR01 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_BRCD' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_RR01_BRCD ON dbo.RR01 (BRCD); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_MA_KH' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_RR01_MA_KH ON dbo.RR01 (MA_KH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_SO_LDS' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_RR01_SO_LDS ON dbo.RR01 (SO_LDS); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_NGAY_XLRR' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_RR01_NGAY_XLRR ON dbo.RR01 (NGAY_XLRR); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_LOAI_KH' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_RR01_LOAI_KH ON dbo.RR01 (LOAI_KH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_RR01_Analytics' AND object_id = OBJECT_ID('dbo.RR01'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_RR01_Analytics ON dbo.RR01 (NGAY_DL, BRCD, MA_KH, LOAI_KH, CCY); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Columnstore Index cho RR01 (Analytics optimization)
                try
                {
                    _logger.LogInformation("🔧 Creating columnstore index for RR01...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_Columnstore' AND object_id = OBJECT_ID('dbo.RR01'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_RR01_Columnstore ON dbo.RR01
                                              (NGAY_DL, BRCD, MA_KH, SO_LDS, NGAY_XLRR, LOAI_KH, CCY, SO_TIEN_RR, XEPLOAI_TTDB);
                                              PRINT '✅ Created columnstore index IX_RR01_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create columnstore index IX_RR01_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for RR01.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for RR01 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ RR01 analytics indexes ensured (temporal table safe).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho RR01 (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
