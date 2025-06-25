#!/bin/bash

# This script will reset the database and apply the minimal schema to fix migration issues
# It uses sqlcmd to execute SQL commands (requires mssql-tools to be installed)

echo "🗑️ Resetting database to fix migration issues..."

# Get the password from appsettings.json
PASSWORD=$(grep -o '"Password=.*"' /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/appsettings.json | cut -d'"' -f2 | cut -d';' -f1 | cut -d'=' -f2)

echo "🔑 Using password from appsettings.json"

# Execute SQL to drop and recreate database
sqlcmd -S localhost,1433 -U sa -P "$PASSWORD" -Q "
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    ALTER DATABASE [TinhKhoanDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TinhKhoanDB];
    PRINT '✅ Dropped existing database';
END
"

echo "💾 Creating new database with temporal tables from minimal_schema.sql..."

# Execute the minimal_schema.sql file
sqlcmd -S localhost,1433 -U sa -P "$PASSWORD" -i /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/minimal_schema.sql

echo "✅ Database reset completed!"
echo "🚀 Now restart the backend API to apply the changes"
