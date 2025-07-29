#!/bin/bash

# 🚨 DOCKER RECOVERY & REBUILD SCRIPT
# Khắc phục Docker image corruption và rebuild container
# Ngày: 18/07/2025

echo "🚨 === DOCKER RECOVERY & REBUILD ==="
echo "📅 Ngày: $(date)"
echo ""

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo -e "${RED}⚠️  CẢNH BÁO: Docker image đã bị corrupt!${NC}"
echo -e "${RED}    Cần rebuild container với fresh image${NC}"
echo -e "${YELLOW}    Dữ liệu database có thể bị mất!${NC}"
echo ""
read -p "Nhấn Enter để tiếp tục hoặc Ctrl+C để dừng..."
echo ""

echo -e "${BLUE}📋 1. Backup container config & data...${NC}"
# Backup container inspect
if docker inspect $CONTAINER_NAME > container_backup_$(date +%Y%m%d_%H%M%S).json 2>/dev/null; then
    echo "✅ Container config backup thành công"
else
    echo "⚠️  Container config backup thất bại"
fi

# Backup volumes nếu có
echo "Checking for volumes..."
docker volume ls | grep -i sql > volumes_backup_$(date +%Y%m%d_%H%M%S).txt
echo ""

echo -e "${BLUE}��️  2. Dọn dẹp hoàn toàn Docker...${NC}"
# Stop all containers
echo "Stopping all containers..."
docker stop $(docker ps -aq) 2>/dev/null || echo "No containers to stop"

# Remove corrupted container
echo "Removing corrupted container..."
docker rm -f $CONTAINER_NAME 2>/dev/null && echo "✅ Container removed" || echo "⚠️  Container removal failed"

# Prune everything aggressively
echo "Aggressive cleanup..."
docker system prune -af --volumes 2>/dev/null && echo "✅ System pruned" || echo "⚠️  System prune failed"

# Remove corrupted images manually
echo "Removing all images..."
docker rmi -f $(docker images -aq) 2>/dev/null || echo "⚠️  Image removal had issues"
echo ""

echo -e "${BLUE}🔄 3. Reset Docker Desktop...${NC}"
echo "Killing Docker Desktop processes..."
killall -9 Docker 2>/dev/null || echo "Docker already stopped"
killall -9 com.docker.hyperkit 2>/dev/null || echo "Hyperkit already stopped"

echo "Waiting for processes to stop..."
sleep 10

echo "Restarting Docker Desktop..."
open -a Docker
echo "Waiting for Docker to start..."
sleep 60
echo ""

echo -e "${BLUE}📥 4. Pull fresh Azure SQL Edge image...${NC}"
echo "Pulling latest Azure SQL Edge image..."
docker pull mcr.microsoft.com/azure-sql-edge:latest && echo "✅ Image pulled successfully" || {
    echo -e "${RED}❌ Image pull failed!${NC}"
    echo "Trying alternative approach..."
    sleep 30
    docker pull mcr.microsoft.com/azure-sql-edge:latest
}
echo ""

echo -e "${BLUE}🚀 5. Create optimized container...${NC}"
echo "Creating new optimized container..."
docker run -d \
    --name $CONTAINER_NAME \
    --restart=unless-stopped \
    --memory=2g \
    --memory-swap=4g \
    --shm-size=1g \
    --cpus="1.5" \
    -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
    -e "MSSQL_TCP_PORT=1433" \
    -e "MSSQL_MEMORY_LIMIT_MB=1536" \
    -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
    -p 1433:1433 \
    mcr.microsoft.com/azure-sql-edge:latest \
    && echo "✅ Container created successfully" || echo "❌ Container creation failed"
echo ""

echo -e "${BLUE}⏳ 6. Waiting for SQL Server to be ready...${NC}"
echo "Waiting for SQL Server startup..."
for i in {1..60}; do
    if docker exec $CONTAINER_NAME /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "SELECT 1" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server is ready after $i attempts!${NC}"
        break
    fi
    echo "Health check $i/60: starting..."
    sleep 10
done
echo ""

echo -e "${BLUE}🗄️  7. Recreate TinhKhoanDB...${NC}"
echo "Creating TinhKhoanDB database..."
docker exec $CONTAINER_NAME /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "CREATE DATABASE TinhKhoanDB" \
    && echo "✅ TinhKhoanDB created" || echo "⚠️  Database creation failed"
echo ""

echo -e "${BLUE}🔍 8. Final verification...${NC}"
echo "Testing connection from host..."
if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" -W >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Host connection successful!${NC}"
    
    echo "Testing TinhKhoanDB..."
    if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT 1" -W >/dev/null 2>&1; then
        echo -e "${GREEN}✅ TinhKhoanDB accessible!${NC}"
    else
        echo -e "${YELLOW}⚠️  TinhKhoanDB needs setup${NC}"
    fi
else
    echo -e "${RED}❌ Connection still failed${NC}"
fi
echo ""

echo -e "${BLUE}📊 9. Status summary...${NC}"
echo "Container status:"
docker ps --filter name=$CONTAINER_NAME --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""

echo "Resource usage:"
docker stats $CONTAINER_NAME --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}" 2>/dev/null || echo "Stats not available yet"
echo ""

echo -e "${GREEN}🎉 === DOCKER REBUILD COMPLETED ===${NC}"
echo -e "${YELLOW}⚠️  LƯU Ý: Dữ liệu cũ đã bị mất!${NC}"
echo "Cần thực hiện:"
echo "  1. Chạy EF migrations: cd Backend/TinhKhoanApp.Api && dotnet ef database update"
echo "  2. Import dữ liệu từ CSV files"
echo "  3. Recreate users, roles, units, etc."
echo ""
echo "🔗 Recommended next steps:"
echo "  ./start_backend.sh"
echo "  ./start_frontend.sh"
