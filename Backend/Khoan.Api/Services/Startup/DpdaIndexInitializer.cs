using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Đảm bảo các chỉ mục phân tích cho DPDA tồn tại (Temporal Table safe) – chạy một lần khi khởi động.
    /// Tạo Nonclustered Indexes an toàn cho các truy vấn phổ biến.
    /// </summary>
    public class DpdaIndexInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DpdaIndexInitializer> _logger;

        public DpdaIndexInitializer(IServiceProvider serviceProvider, ILogger<DpdaIndexInitializer> logger)
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

                // Danh sách chỉ mục cần có cho DPDA
                var statements = new[]
                {
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_NGAY_DL' AND object_id = OBJECT_ID('dbo.DPDA'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DPDA_NGAY_DL ON dbo.DPDA (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_MA_CHI_NHANH' AND object_id = OBJECT_ID('dbo.DPDA'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DPDA_MA_CHI_NHANH ON dbo.DPDA (MA_CHI_NHANH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_MA_KHACH_HANG' AND object_id = OBJECT_ID('dbo.DPDA'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DPDA_MA_KHACH_HANG ON dbo.DPDA (MA_KHACH_HANG); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_SO_TAI_KHOAN' AND object_id = OBJECT_ID('dbo.DPDA'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DPDA_SO_TAI_KHOAN ON dbo.DPDA (SO_TAI_KHOAN); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_LOAI_THE' AND object_id = OBJECT_ID('dbo.DPDA'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_DPDA_LOAI_THE ON dbo.DPDA (LOAI_THE); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_DPDA_Analytics' AND object_id = OBJECT_ID('dbo.DPDA'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_DPDA_Analytics ON dbo.DPDA (NGAY_DL, MA_CHI_NHANH, MA_KHACH_HANG, SO_TAI_KHOAN); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                _logger.LogInformation("✅ DPDA analytics indexes ensured (temporal table safe).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo indexes cho DPDA (có thể đã tồn tại hoặc quyền hạn DB hạn chế)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
