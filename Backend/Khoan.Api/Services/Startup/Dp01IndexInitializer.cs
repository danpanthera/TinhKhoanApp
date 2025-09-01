using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho DP01 tồn tại (Temporal Table safe) – chạy một lần khi khởi động.
    /// Tạo Nonclustered Indexes an toàn cho các truy vấn phổ biến trên bảng DP01 (63 business columns).
    /// </summary>
    public class Dp01IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Dp01IndexInitializer> _logger;

        public Dp01IndexInitializer(IServiceProvider serviceProvider, ILogger<Dp01IndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho DP01 (Temporal Table với 63 business columns)
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_NGAY_DL' AND object_id = OBJECT_ID('dbo.DP01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DP01_NGAY_DL ON dbo.DP01 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_MA_CN' AND object_id = OBJECT_ID('dbo.DP01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DP01_MA_CN ON dbo.DP01 (MA_CN); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_MA_KH' AND object_id = OBJECT_ID('dbo.DP01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DP01_MA_KH ON dbo.DP01 (MA_KH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_SO_TAI_KHOAN' AND object_id = OBJECT_ID('dbo.DP01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DP01_SO_TAI_KHOAN ON dbo.DP01 (SO_TAI_KHOAN); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_CCY' AND object_id = OBJECT_ID('dbo.DP01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DP01_CCY ON dbo.DP01 (CCY); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_DP01_Analytics' AND object_id = OBJECT_ID('dbo.DP01'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_DP01_Analytics ON dbo.DP01 (NGAY_DL, MA_CN, MA_KH, SO_TAI_KHOAN, CCY); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Columnstore Index cho DP01 (Analytics optimization)
                try
                {
                    _logger.LogInformation("🔧 Creating columnstore index for DP01...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_Columnstore' AND object_id = OBJECT_ID('dbo.DP01'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DP01_Columnstore ON dbo.DP01
                                              (NGAY_DL, MA_CN, MA_KH, SO_TAI_KHOAN, CCY, CURRENT_BALANCE, DP_TYPE_CODE, CUST_TYPE);
                                              PRINT '✅ Created columnstore index IX_DP01_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create columnstore index IX_DP01_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for DP01.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for DP01 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ DP01 analytics indexes ensured (temporal table safe, 63 business columns).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho DP01 (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
