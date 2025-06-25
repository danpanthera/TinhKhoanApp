#!/bin/bash

echo "🔄 Fixing database schema and migration history..."

# Get the password from appsettings.json
PASSWORD=$(grep -o '"Password=.*"' /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/appsettings.json | cut -d'"' -f2 | cut -d';' -f1 | cut -d'=' -f2)

echo "🗑️ Dropping and recreating database..."

# Drop and recreate database
cat << 'EOF' | docker exec -i sql_server_tinhkhoan /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$PASSWORD" -C
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    ALTER DATABASE [TinhKhoanDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TinhKhoanDB];
    PRINT '✅ Dropped existing database';
END

CREATE DATABASE [TinhKhoanDB];
PRINT '✅ Created new database';
EOF

echo "📊 Applying complete schema..."

# Apply the complete schema from the first migration
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
dotnet ef database update 20250621132903_DashboardTablesCreated

echo "🔄 Applying remaining migrations..."

# Apply all remaining migrations
dotnet ef database update

echo "👤 Setting up basic data..."

# Run basic tables setup
cat setup_basic_tables.sql | docker exec -i sql_server_tinhkhoan /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$PASSWORD" -d TinhKhoanDB -C

echo "✅ Database initialization completed!"
echo "🚀 Ready to restart the application"
