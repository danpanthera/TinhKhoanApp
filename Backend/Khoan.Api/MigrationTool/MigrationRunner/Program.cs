using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace MigrationRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=Dientoan@303;TrustServerCertificate=true;";
            string sqlFile = "RebuildGL41PartitionedColumnstore.sql";

            try
            {
                Console.WriteLine("=== CHẠY MIGRATION GL41 QUA .NET ===");

                if (!File.Exists(sqlFile))
                {
                    Console.WriteLine($"File {sqlFile} không tồn tại!");
                    return;
                }

                string sqlScript = File.ReadAllText(sqlFile);

                // Chia script thành các batch (tách bởi GO)
                string[] batches = sqlScript.Split(new string[] { "\nGO\n", "\nGO\r\n", "\rGO\r", "\ngo\n", "\ngo\r\n", "\rgo\r" },
                    StringSplitOptions.RemoveEmptyEntries);

                using (var connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Đang kết nối database...");
                    connection.Open();
                    Console.WriteLine("Kết nối thành công!");

                    foreach (var batch in batches)
                    {
                        var trimmedBatch = batch.Trim();
                        if (string.IsNullOrWhiteSpace(trimmedBatch) || trimmedBatch.StartsWith("--"))
                            continue;

                        Console.WriteLine($"Đang chạy batch: {trimmedBatch.Substring(0, Math.Min(50, trimmedBatch.Length))}...");

                        try
                        {
                            using (var command = new SqlCommand(trimmedBatch, connection))
                            {
                                command.CommandTimeout = 300; // 5 minutes
                                var result = command.ExecuteNonQuery();
                                Console.WriteLine($"Batch thành công. Rows affected: {result}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi chạy batch: {ex.Message}");
                            // Tiếp tục với batch tiếp theo
                        }
                    }
                }

                // Kiểm tra kết quả
                Console.WriteLine("\n=== KIỂM TRA KẾT QUẢ ===");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL41'", connection))
                    {
                        var columnCount = command.ExecuteScalar();
                        Console.WriteLine($"Bảng GL41 có {columnCount} cột");
                    }

                    using (var command = new SqlCommand("SELECT COUNT(*) FROM GL41", connection))
                    {
                        var rowCount = command.ExecuteScalar();
                        Console.WriteLine($"Bảng GL41 có {rowCount} dòng dữ liệu");
                    }
                }

                Console.WriteLine("=== MIGRATION HOÀN THÀNH ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
}
