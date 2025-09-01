using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho LN03 tồn tại (Temporal Table safe) – chạy một lần khi khởi động.
    /// Tạo Nonclustered Indexes an toàn cho các truy vấn phổ biến.
    /// </summary>
    public class Ln03IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Ln03IndexInitializer> _logger;

        public Ln03IndexInitializer(IServiceProvider serviceProvider, ILogger<Ln03IndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho LN03 (20 business columns)
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_NGAY_DL' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_NGAY_DL ON dbo.LN03 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_MACHINHANH' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_MACHINHANH ON dbo.LN03 (MACHINHANH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_MAKH' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_MAKH ON dbo.LN03 (MAKH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_SOHOPDONG' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_SOHOPDONG ON dbo.LN03 (SOHOPDONG); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_MACBTD' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_MACBTD ON dbo.LN03 (MACBTD); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_NGAYPHATSINHXL' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_NGAYPHATSINHXL ON dbo.LN03 (NGAYPHATSINHXL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_LN03_Analytics' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_LN03_Analytics ON dbo.LN03 (NGAY_DL, MACHINHANH, MAKH, MACBTD, NHOMNO); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Columnstore Index cho LN03 (Analytics optimization)
                try
                {
                    _logger.LogInformation("🔧 Creating columnstore index for LN03...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_Columnstore' AND object_id = OBJECT_ID('dbo.LN03'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN03_Columnstore ON dbo.LN03
                                              (NGAY_DL, MACHINHANH, MAKH, SOHOPDONG, MACBTD, NGAYPHATSINHXL, NHOMNO, DUNOGOCDAUKY, DUNOGOICUOIKY);
                                              PRINT '✅ Created columnstore index IX_LN03_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create columnstore index IX_LN03_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for LN03.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for LN03 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ LN03 analytics indexes ensured (temporal table safe).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho LN03 (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
