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
