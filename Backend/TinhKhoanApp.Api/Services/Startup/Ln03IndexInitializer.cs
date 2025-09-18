using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Services.Startup
{
    /// <summary>
    /// Ensure LN03 analytics indexes exist (nonclustered indexes as columnstore approximation)
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

                var statements = new[]
                {
                    // Basic indexes
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_NGAY_DL' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_NGAY_DL ON dbo.LN03 (NGAY_DL); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_MACHINHANH' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_MACHINHANH ON dbo.LN03 (MACHINHANH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_MAKH' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_MAKH ON dbo.LN03 (MAKH); END",
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_MACBTD' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX IX_LN03_MACBTD ON dbo.LN03 (MACBTD); END",
                    // Analytics composite
                    @"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_LN03_Analytics' AND object_id = OBJECT_ID('dbo.LN03'))
                       BEGIN CREATE NONCLUSTERED INDEX NCCI_LN03_Analytics ON dbo.LN03 (NGAY_DL, MACHINHANH, MAKH, MACBTD); END"
                };

                foreach (var sql in statements)
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }

                _logger.LogInformation("✅ LN03 analytics indexes ensured.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to initialize LN03 indexes: {Message}", ex.Message);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔄 LN03 Index Initializer stopped.");
            return Task.CompletedTask;
        }
    }
}
