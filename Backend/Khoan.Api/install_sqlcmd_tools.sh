#!/bin/bash

# 🛠️ INSTALL SQL COMMAND TOOLS IN SQL SERVER 2022
echo "🛠️ Installing SQL Command Tools..."

# Install sqlcmd tools inside the container
docker exec sqlserver2022 /bin/bash -c "
apt-get update &&
apt-get install -y curl gnupg2 &&
curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - &&
curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/msprod.list &&
apt-get update &&
ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev
"

echo "🔧 Setting up PATH for sqlcmd..."
docker exec sqlserver2022 /bin/bash -c "echo 'export PATH=\"\$PATH:/opt/mssql-tools18/bin\"' >> ~/.bashrc"

echo "✅ SQL Command Tools installed successfully!"

# Test sqlcmd
echo "🧪 Testing sqlcmd connection..."
docker exec sqlserver2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dientoan@303" -Q "SELECT @@VERSION AS [SQL_Server_Version]" -C

# Create KhoanDB database
echo "📊 Creating KhoanDB database..."
docker exec sqlserver2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dientoan@303" -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KhoanDB')
BEGIN
    CREATE DATABASE [KhoanDB];
    ALTER DATABASE [KhoanDB] SET RECOVERY SIMPLE;
    ALTER DATABASE [KhoanDB] SET AUTO_CLOSE OFF;
    ALTER DATABASE [KhoanDB] SET AUTO_SHRINK OFF;
    PRINT 'KhoanDB database created successfully';
END
ELSE
    PRINT 'KhoanDB database already exists';
" -C

echo "🎉 SQL Server 2022 fully ready with tools and database!"
