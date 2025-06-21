#!/bin/bash

# Alternative approach to run SQL - Use sqlite3 command if available
# First, check if we're using SQLite

CONNECTION_STRING=$(grep -o 'Filename=.*\.db' appsettings.json || true)

if [ -n "$CONNECTION_STRING" ]; then
    # Extract the SQLite DB path
    DB_PATH=$(echo $CONNECTION_STRING | sed 's/Filename=//')
    echo "Using SQLite database at: $DB_PATH"
    
    # Run SQL script using sqlite3
    sqlite3 $DB_PATH < standardize_branch_names.sql
    
    echo "SQL script executed with sqlite3."
else
    # Use the dotnet-script approach which should work for any provider
    echo "Creating a C# script to run SQL..."
    
    cat > run_sql.csx << EOL
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
EOL

    # Run the C# script with dotnet-script (if installed)
    if command -v dotnet-script &> /dev/null; then
        dotnet-script run_sql.csx
    else
        echo "Neither sqlite3 nor dotnet-script is available."
        echo "Please install one of these tools to run the SQL script,"
        echo "or manually execute the standardize_branch_names.sql script."
    fi
fi

echo "Attempted to execute SQL script to standardize branch names."
