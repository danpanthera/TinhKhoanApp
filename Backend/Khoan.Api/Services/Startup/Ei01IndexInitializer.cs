using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho EI01 tồn tại (Temporal Table safe) – chạy một lần khi khởi động.
    /// Tạo Nonclustered Indexes an toàn cho các truy vấn phổ biến.
    /// </summary>
    public class Ei01IndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Ei01IndexInitializer> _logger;

        public Ei01IndexInitializer(IServiceProvider serviceProvider, ILogger<Ei01IndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho EI01
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_NGAY_DL' AND object_id = OBJECT_ID('dbo.EI01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_EI01_NGAY_DL ON dbo.EI01 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_MA_CN' AND object_id = OBJECT_ID('dbo.EI01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_EI01_MA_CN ON dbo.EI01 (MA_CN); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_MA_KH' AND object_id = OBJECT_ID('dbo.EI01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_EI01_MA_KH ON dbo.EI01 (MA_KH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_LOAI_KH' AND object_id = OBJECT_ID('dbo.EI01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_EI01_LOAI_KH ON dbo.EI01 (LOAI_KH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_NGAY_DK_EMB' AND object_id = OBJECT_ID('dbo.EI01'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_EI01_NGAY_DK_EMB ON dbo.EI01 (NGAY_DK_EMB); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_EI01_Analytics' AND object_id = OBJECT_ID('dbo.EI01'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_EI01_Analytics ON dbo.EI01 (NGAY_DL, MA_CN, MA_KH, LOAI_KH, TRANG_THAI_EMB); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                // Tạo Columnstore Index cho EI01 (Analytics optimization)
                try
                {
                    _logger.LogInformation("🔧 Creating columnstore index for EI01...");
                    
                    var columnstoreSql = @"
                        DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                        IF (@edition NOT LIKE '%Azure SQL Edge%')
                          BEGIN
                                BEGIN TRY
                                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_Columnstore' AND object_id = OBJECT_ID('dbo.EI01'))
                                      BEGIN
                                              CREATE NONCLUSTERED COLUMNSTORE INDEX IX_EI01_Columnstore ON dbo.EI01
                                              (NGAY_DL, MA_CN, MA_KH, LOAI_KH, NGAY_DK_EMB, TRANG_THAI_EMB, LOAI_THE, HINH_THUC_TT);
                                              PRINT '✅ Created columnstore index IX_EI01_Columnstore';
                                      END
                                END TRY
                                BEGIN CATCH
                                      PRINT '⚠️ Could not create columnstore index IX_EI01_Columnstore: ' + ERROR_MESSAGE();
                                END CATCH
                          END
                        ELSE
                          BEGIN
                                PRINT 'ℹ️ Azure SQL Edge detected – skipping columnstore index creation for EI01.';
                          END
                    ";

                    await db.Database.ExecuteSqlRawAsync(columnstoreSql, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Could not create columnstore index for EI01 (possibly unsupported DB edition)");
                }

                _logger.LogInformation("✅ EI01 analytics indexes ensured (temporal table safe).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho EI01 (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
