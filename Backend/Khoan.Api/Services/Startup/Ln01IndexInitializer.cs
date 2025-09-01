using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho LN01 tồn tại (Temporal Table safe) – chạy một lần khi khởi động.
    /// Tạo Nonclustered Indexes an toàn cho các truy vấn phổ biến.
    /// </summary>
    public class Ln01IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Ln01IndexInitializer> _logger;

        public Ln01IndexInitializer(IServiceProvider serviceProvider, ILogger<Ln01IndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho LN01
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_NGAY_DL' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN01_NGAY_DL ON dbo.LN01 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_BRCD' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN01_BRCD ON dbo.LN01 (BRCD); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_CUSTSEQ' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN01_CUSTSEQ ON dbo.LN01 (CUSTSEQ); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_TAI_KHOAN' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN01_TAI_KHOAN ON dbo.LN01 (TAI_KHOAN); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_APPRDT' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN01_APPRDT ON dbo.LN01 (APPRDT); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_APPRMATDT' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN01_APPRMATDT ON dbo.LN01 (APPRMATDT); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_LN01_Analytics' AND object_id = OBJECT_ID('dbo.LN01'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_LN01_Analytics ON dbo.LN01 (NGAY_DL, BRCD, CUSTSEQ, CCY, LOAN_TYPE); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Columnstore Index cho LN01 (Analytics optimization)
                try
                {
                    _logger.LogInformation("🔧 Creating columnstore index for LN01...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_Columnstore' AND object_id = OBJECT_ID('dbo.LN01'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN01_Columnstore ON dbo.LN01
                                              (NGAY_DL, BRCD, CUSTSEQ, TAI_KHOAN, CCY, LOAN_TYPE, APPRDT, APPRMATDT, APPR_AMT, OUTSTANDING);
                                              PRINT '✅ Created columnstore index IX_LN01_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create columnstore index IX_LN01_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for LN01.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for LN01 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ LN01 analytics indexes ensured (temporal table safe).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho LN01 (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
