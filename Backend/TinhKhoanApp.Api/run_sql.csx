#r "nuget: Microsoft.EntityFrameworkCore.SqlServer, 7.0.0"
#r "nuget: System.Data.SqlClient, 4.8.5"

using System;
using System.IO;
using System.Data.SqlClient;

// Connection string from appsettings.json
string connectionString = "Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;MultipleActiveResultSets=true;";

// Read SQL script
string sql = File.ReadAllText("standardize_branch_names.sql");

// Execute SQL
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (var command = new SqlCommand(sql, connection))
    {
        command.ExecuteNonQuery();
        Console.WriteLine("SQL script executed successfully!");
    }
}
