#!/bin/bash

# =============================================================================
# OPTIMIZED AZURE SQL EDGE CONTAINER FOR MACOS M3 CHIP
# Tạo container tối ưu tránh crash SIGABRT và đảm bảo ổn định tối đa
# =============================================================================

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m'

CONTAINER_NAME="azure_sql_edge_tinhkhoan"
DB_PASSWORD="Dientoan@303"
VOLUME_NAME="sqldata_tinhkhoan_optimized"

echo -e "${BLUE}🚀 TẠO CONTAINER TỐI ƯU CHO M3 CHIP${NC}"
echo "================================================"

# Step 1: Clean up existing containers and volumes
echo -e "${YELLOW}🧹 Bước 1: Dọn dẹp containers và volumes cũ${NC}"
docker stop $CONTAINER_NAME 2>/dev/null || true
docker rm $CONTAINER_NAME 2>/dev/null || true

# Step 2: Create optimized volume with specific options
echo -e "${YELLOW}💾 Bước 2: Tạo volume tối ưu${NC}"
docker volume rm $VOLUME_NAME 2>/dev/null || true
docker volume create $VOLUME_NAME
echo -e "${GREEN}✅ Volume $VOLUME_NAME created${NC}"

# Step 3: Create container with M3-optimized settings
echo -e "${YELLOW}🐳 Bước 3: Tạo container với cấu hình tối ưu M3${NC}"
docker run -d \
  --name $CONTAINER_NAME \
  --restart unless-stopped \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=$DB_PASSWORD" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_MEMORY_LIMIT_MB=2048" \
  -e "MSSQL_TCP_PORT=1433" \
  -e "MSSQL_ENABLE_HADR=0" \
  -e "MSSQL_AGENT_ENABLED=false" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -p 1433:1433 \
  --memory=3g \
  --memory-swap=6g \
  --cpus="2.0" \
  --cpu-shares=1024 \
  --shm-size=512m \
  --security-opt seccomp=unconfined \
  --cap-add=SYS_PTRACE \
  --ulimit nofile=65536:65536 \
  --ulimit nproc=65536:65536 \
  -v $VOLUME_NAME:/var/opt/mssql/data \
  -v /tmp/mssql_backup:/var/opt/mssql/backup \
  --tmpfs /tmp:rw,noexec,nosuid,size=512m \
  --platform linux/arm64 \
  mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ Container created successfully${NC}"

# Step 4: Wait for container to start
echo -e "${YELLOW}⏳ Bước 4: Chờ container khởi động...${NC}"
sleep 30

# Step 5: Check container status
echo -e "${YELLOW}📊 Bước 5: Kiểm tra trạng thái${NC}"
if docker ps | grep -q $CONTAINER_NAME; then
    echo -e "${GREEN}✅ Container đang chạy${NC}"
    docker stats $CONTAINER_NAME --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"
else
    echo -e "${RED}❌ Container không chạy được. Kiểm tra logs:${NC}"
    docker logs $CONTAINER_NAME --tail 20
    exit 1
fi

# Step 6: Test SQL connectivity with retry logic
echo -e "${YELLOW}🔌 Bước 6: Kiểm tra kết nối SQL (với retry)${NC}"
for i in {1..10}; do
    echo "Thử kết nối lần $i..."
    if sqlcmd -S localhost,1433 -U sa -P "$DB_PASSWORD" -Q "SELECT @@VERSION" -C -l 5 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server đã sẵn sàng!${NC}"
        break
    else
        if [ $i -eq 10 ]; then
            echo -e "${RED}❌ SQL Server không phản hồi sau 10 lần thử${NC}"
            docker logs $CONTAINER_NAME --tail 10
            exit 1
        fi
        echo "Chờ 10 giây trước khi thử lại..."
        sleep 10
    fi
done

echo -e "${GREEN}🎉 CONTAINER TỐI ƯU ĐÃ SẴNG SÀNG!${NC}"
echo ""
echo -e "${BLUE}📋 Thông tin container:${NC}"
echo "- Name: $CONTAINER_NAME"
echo "- Password: $DB_PASSWORD"
echo "- Port: 1433"
echo "- Memory: 3GB"
echo "- CPU: 2 cores"
echo "- Volume: $VOLUME_NAME"
echo ""
echo -e "${YELLOW}🔧 Bước tiếp theo: Chạy script tối ưu SQL Server${NC}"
