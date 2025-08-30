using Microsoft.Data.SqlClient;
using System;

namespace CheckRunner
{
    class CheckGL41Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=Dientoan@303;TrustServerCertificate=true;";

            try
            {
                Console.WriteLine("=== KIỂM TRA CẤU TRÚC GL41 SAU MIGRATION ===");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Kiểm tra cấu trúc columns
                    Console.WriteLine("\n--- CẤU TRÚC COLUMNS ---");
                    string columnQuery = @"
                        SELECT
                            c.COLUMN_NAME,
                            c.DATA_TYPE,
                            c.IS_NULLABLE,
                            c.ORDINAL_POSITION
                        FROM INFORMATION_SCHEMA.COLUMNS c
                        WHERE c.TABLE_NAME = 'GL41'
                        ORDER BY c.ORDINAL_POSITION";

                    using (var command = new SqlCommand(columnQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["ORDINAL_POSITION"],2}: {reader["COLUMN_NAME"],-20} {reader["DATA_TYPE"],-15} {reader["IS_NULLABLE"]}");
                        }
                    }

                    // Kiểm tra số lượng records
                    Console.WriteLine("\n--- THỐNG KÊ DỮ LIỆU ---");
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM GL41", connection))
                    {
                        var count = command.ExecuteScalar();
                        Console.WriteLine($"Tổng số records: {count}");
                    }

                    // Kiểm tra indexes
                    Console.WriteLine("\n--- INDEXES ---");
                    string indexQuery = @"
                        SELECT
                            i.name as IndexName,
                            i.type_desc as IndexType
                        FROM sys.indexes i
                        WHERE i.object_id = OBJECT_ID('GL41')";

                    using (var command = new SqlCommand(indexQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["IndexName"],-30} {reader["IndexType"]}");
                        }
                    }
                }

                Console.WriteLine("\n=== KIỂM TRA HOÀN THÀNH ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
}
