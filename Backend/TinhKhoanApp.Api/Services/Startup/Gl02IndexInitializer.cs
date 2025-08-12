using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Services.Startup
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
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL02_Analytics' AND object_id = OBJECT_ID('dbo.GL02'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_GL02_Analytics ON dbo.GL02 (NGAY_DL, UNIT, TRCD, CCY); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
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
