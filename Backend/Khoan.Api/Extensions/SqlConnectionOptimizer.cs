using Microsoft.Data.SqlClient;

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Tối ưu hóa kết nối SQL Server và pre-warm connection pool
    /// </summary>
    public static class SqlConnectionOptimizer
    {
        /// <summary>
        /// Pre-warm connection pool để giảm thời gian kết nối lần đầu
        /// </summary>
        /// <param name="connectionString">Connection string của database</param>
        public static void WarmUpConnection(string connectionString)
        {
            // Pre-warm connection pool trong background task
            Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("🔥 Warming up SQL connection pool...");

                    // Tạo nhiều connection để fill pool
                    var warmupTasks = new List<Task>();

                    for (int i = 0; i < 5; i++) // Warm up 5 connections
                    {
                        warmupTasks.Add(WarmUpSingleConnection(connectionString, i + 1));
                    }

                    await Task.WhenAll(warmupTasks);
                    Console.WriteLine("✅ SQL connection pool warmed up successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Warning: Failed to warm up connection pool: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Warm up một connection đơn lẻ
        /// </summary>
        private static async Task WarmUpSingleConnection(string connectionString, int connectionNumber)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("SELECT 1", conn);
                var result = await cmd.ExecuteScalarAsync();

                Console.WriteLine($"🔗 Connection {connectionNumber} warmed up successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Failed to warm up connection {connectionNumber}: {ex.Message}");
            }
        }

        /// <summary>
        /// Test connection performance
        /// </summary>
        /// <param name="connectionString">Connection string để test</param>
        /// <returns>Thời gian kết nối tính bằng milliseconds</returns>
        public static async Task<long> TestConnectionPerformance(string connectionString)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("SELECT GETDATE()", conn);
                await cmd.ExecuteScalarAsync();

                stopwatch.Stop();
                return stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"❌ Connection test failed: {ex.Message}");
                throw;
            }
        }
    }
}
