#!/bin/bash

# 🔄 RECREATE AZURE SQL EDGE CONTAINER
# Script tạo lại container với cấu hình tối ưu sau khi reinstall Docker

set -e

CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo "🔄 RECREATING AZURE SQL EDGE CONTAINER"
echo "====================================="

# 1. Stop và remove container cũ (nếu có)
echo "🛑 1. Cleaning up old container..."
if docker ps -a --filter name=$CONTAINER_NAME --format '{{.Names}}' | grep -q $CONTAINER_NAME; then
    docker stop $CONTAINER_NAME 2>/dev/null || true
    docker rm $CONTAINER_NAME 2>/dev/null || true
    echo "✅ Old container removed"
else
    echo "ℹ️ No existing container found"
fi

# 2. Pull fresh Azure SQL Edge image
echo ""
echo "📦 2. Pulling fresh Azure SQL Edge image..."
docker pull mcr.microsoft.com/azure-sql-edge:latest
echo "✅ Fresh image pulled"

# 3. Create optimized container
echo ""
echo "🚀 3. Creating optimized container..."
docker run -e "ACCEPT_EULA=Y" \
           -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
           -e "MSSQL_PID=Developer" \
           -e "MSSQL_AGENT_ENABLED=true" \
           -p 1433:1433 \
           --name $CONTAINER_NAME \
           --restart=unless-stopped \
           --memory=4g \
           --memory-swap=8g \
           --cpus=2 \
           --shm-size=256m \
           --tmpfs /tmp:rw,noexec,nosuid,size=256m \
           -d mcr.microsoft.com/azure-sql-edge:latest

echo "✅ Container created with optimized settings:"
echo "   - Memory: 4GB limit, 8GB swap"
echo "   - CPU: 2 cores"
echo "   - Shared memory: 256MB"
echo "   - Temp filesystem: 256MB"
echo "   - Auto restart: unless-stopped"

# 4. Wait for startup
echo ""
echo "⏳ 4. Waiting for SQL Server startup..."
sleep 30

# Health check loop
for i in {1..12}; do
    if docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "SELECT @@VERSION" > /dev/null 2>&1; then
        echo "✅ SQL Server is ready!"
        break
    else
        echo "⏳ Attempt $i/12: Waiting for SQL Server..."
        sleep 10
    fi

    if [ $i -eq 12 ]; then
        echo "❌ SQL Server startup timeout"
        exit 1
    fi
done

# 5. Create database
echo ""
echo "🗄️ 5. Creating TinhKhoanDB database..."
docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TinhKhoanDB')
BEGIN
    CREATE DATABASE TinhKhoanDB;
    PRINT 'Database TinhKhoanDB created successfully';
END
ELSE
BEGIN
    PRINT 'Database TinhKhoanDB already exists';
END
"

# 6. Enable features
echo ""
echo "🔧 6. Enabling advanced features..."
docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'Agent XPs', 1;
RECONFIGURE;
"

# 7. Final health check
echo ""
echo "🔍 7. Final health check..."
docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "
SELECT
    @@VERSION as SQLVersion,
    DB_NAME() as CurrentDatabase,
    GETDATE() as CurrentTime
"

# 8. Container status
echo ""
echo "📊 8. Container status:"
docker ps --filter name=$CONTAINER_NAME --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

echo ""
echo "🎉 CONTAINER RECREATION COMPLETED!"
echo "=================================="
echo "Container Name: $CONTAINER_NAME"
echo "Database: TinhKhoanDB"
echo "Connection: localhost:1433"
echo "Username: sa"
echo "Password: YourStrong@Password123"
echo ""
echo "📝 NEXT STEPS:"
echo "1. Run backend migrations: cd Backend/TinhKhoanApp.Api && dotnet ef database update"
echo "2. Start backend: cd Backend/TinhKhoanApp.Api && ./start_backend.sh"
echo "3. Start frontend: cd Frontend/tinhkhoan-app-ui-vite && ./start_frontend.sh"
echo ""
echo "✅ Ready for development!"
