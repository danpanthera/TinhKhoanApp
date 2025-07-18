#!/bin/bash

# 🔧 DOCKER RECOVERY - RECREATE WITH NEW IMAGE
# Giữ nguyên dữ liệu, tạo container mới với image clean
# Ngày: 18/07/2025

echo "🔧 === DOCKER RECOVERY - NEW IMAGE STRATEGY ==="
echo "📅 Ngày: $(date)"
echo ""

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Container names
OLD_CONTAINER="azure_sql_edge_tinhkhoan"
NEW_CONTAINER="azure_sql_edge_tinhkhoan_new"
BACKUP_CONTAINER="azure_sql_edge_backup_$(date +%Y%m%d_%H%M%S)"

echo -e "${BLUE}📊 1. Phân tích tình hình hiện tại...${NC}"
echo "Kiểm tra container cũ:"
docker ps -a --filter name=$OLD_CONTAINER --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""

echo "Kiểm tra disk space:"
df -h | grep -E "Filesystem|/dev/disk3"
echo ""

echo -e "${BLUE}🔄 2. Pull fresh Azure SQL Edge image...${NC}"
echo "Downloading latest Azure SQL Edge ARM64..."
if docker pull mcr.microsoft.com/azure-sql-edge:latest; then
    echo -e "${GREEN}✅ Fresh image downloaded successfully!${NC}"
else
    echo -e "${RED}❌ Failed to pull image. Checking network...${NC}"
    ping -c 3 mcr.microsoft.com && echo "Network OK" || echo "Network issue"
fi
echo ""

echo -e "${BLUE}📋 3. Inspect old container volumes...${NC}"
echo "Getting volume information from old container..."
docker inspect $OLD_CONTAINER --format='{{range .Mounts}}{{.Type}}: {{.Source}} -> {{.Destination}}{{"\n"}}{{end}}' 2>/dev/null || echo "Container inspect failed"
echo ""

echo -e "${BLUE}🛑 4. Stop old container safely...${NC}"
echo "Stopping old container..."
if docker stop $OLD_CONTAINER 2>/dev/null; then
    echo -e "${GREEN}✅ Old container stopped${NC}"
else
    echo -e "${YELLOW}⚠️  Container already stopped or force stop needed${NC}"
    docker kill $OLD_CONTAINER 2>/dev/null && echo "Force killed" || echo "Kill failed"
fi
echo ""

echo -e "${BLUE}🏗️ 5. Create new optimized container...${NC}"
echo "Creating new container with optimizations and volume mapping..."

# Tạo container mới với:
# - Fresh image
# - Memory optimizations
# - Volume mapping cho data persistence
# - Enhanced configuration
docker run -d \
    --name $NEW_CONTAINER \
    --restart=unless-stopped \
    --memory=3g \
    --memory-swap=6g \
    --shm-size=1g \
    --cpus="2.0" \
    -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
    -e "MSSQL_TCP_PORT=1433" \
    -e "MSSQL_MEMORY_LIMIT_MB=2048" \
    -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
    -p 1433:1433 \
    -v tinhkhoan_data:/var/opt/mssql \
    mcr.microsoft.com/azure-sql-edge:latest

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ New container created successfully!${NC}"
else
    echo -e "${RED}❌ Failed to create new container${NC}"
    exit 1
fi
echo ""

echo -e "${BLUE}⏳ 6. Waiting for SQL Server to start...${NC}"
echo "Giving SQL Server time to initialize..."
for i in {1..60}; do
    if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" -W -t 5 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server is ready! (${i}/60)${NC}"
        break
    else
        echo "Waiting for SQL Server... (${i}/60)"
        sleep 5
    fi
done
echo ""

echo -e "${BLUE}🔍 7. Database verification...${NC}"
echo "Checking if TinhKhoanDB exists..."
if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB'" -W -t 10 2>/dev/null | grep -q "TinhKhoanDB"; then
    echo -e "${GREEN}✅ TinhKhoanDB found in new container!${NC}"

    # Count tables
    TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES" -W -h -1 -t 10 2>/dev/null | tr -d ' \r\n')
    echo "Tables found: $TABLE_COUNT"

    # Sample data check
    echo "Checking sample data..."
    sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT TOP 1 'DP01', COUNT(*) FROM DP01; SELECT TOP 1 'Units', COUNT(*) FROM Units; SELECT TOP 1 'Roles', COUNT(*) FROM Roles;" -W -t 10 2>/dev/null || echo "Data check queries failed"

else
    echo -e "${YELLOW}⚠️  TinhKhoanDB not found. Will need migration...${NC}"
fi
echo ""

echo -e "${BLUE}🔄 8. Container replacement...${NC}"
if docker ps --filter name=$NEW_CONTAINER | grep -q $NEW_CONTAINER; then
    echo "Renaming containers..."

    # Rename old container to backup
    docker rename $OLD_CONTAINER $BACKUP_CONTAINER 2>/dev/null && echo "✅ Old container renamed to backup" || echo "⚠️  Rename failed"

    # Rename new container to production name
    docker rename $NEW_CONTAINER $OLD_CONTAINER 2>/dev/null && echo "✅ New container is now production" || echo "⚠️  Production rename failed"

else
    echo -e "${RED}❌ New container not running properly${NC}"
fi
echo ""

echo -e "${BLUE}📊 9. Final status report...${NC}"
echo "Current container status:"
docker ps --filter name=$OLD_CONTAINER --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""

echo "Health check:"
HEALTH=$(docker inspect $OLD_CONTAINER --format='{{.State.Health.Status}}' 2>/dev/null || echo "no_health_check")
echo "Health: $HEALTH"
echo ""

echo "Memory usage:"
docker stats $OLD_CONTAINER --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}" 2>/dev/null || echo "Stats not available"
echo ""

echo -e "${GREEN}🎉 === DOCKER RECOVERY COMPLETED ===${NC}"
echo "New container configuration:"
echo "  - Fresh Azure SQL Edge image"
echo "  - Memory limit: 3GB"
echo "  - Memory swap: 6GB"
echo "  - Shared memory: 1GB"
echo "  - CPU limit: 2.0 cores"
echo "  - SQL Memory: 2048MB"
echo "  - Volume: tinhkhoan_data for persistence"
echo ""

echo -e "${BLUE}🔗 Next steps:${NC}"
echo "  1. Test database: sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB"
echo "  2. Run migrations if needed: cd Backend/TinhKhoanApp.Api && dotnet ef database update"
echo "  3. Start backend: ./start_backend.sh"
echo "  4. Start frontend: ./start_frontend.sh"
echo "  5. Remove backup container: docker rm $BACKUP_CONTAINER (after confirming data is OK)"
echo ""

echo -e "${YELLOW}📝 IMPORTANT:${NC}"
echo "  - If data is missing, run EF migrations to recreate tables"
echo "  - Backup container preserved as: $BACKUP_CONTAINER"
echo "  - Volume 'tinhkhoan_data' contains persistent data"
