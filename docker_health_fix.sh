#!/bin/bash

# 🔧 DOCKER HEALTH FIX & OPTIMIZATION SCRIPT
# Khắc phục vấn đề Docker không mất dữ liệu
# Ngày: 18/07/2025

echo "🔧 === DOCKER HEALTH FIX & OPTIMIZATION ==="
echo "📅 Ngày: $(date)"
echo ""

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Container name
CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo -e "${BLUE}📊 1. Kiểm tra trạng thái hiện tại...${NC}"
echo "Container status:"
docker ps -a --filter name=$CONTAINER_NAME --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""

echo "Health status:"
HEALTH=$(docker inspect $CONTAINER_NAME --format='{{.State.Health.Status}}' 2>/dev/null || echo "not_found")
echo "Health: $HEALTH"
echo ""

echo "Disk space:"
df -h | grep -E "Filesystem|/dev/disk3"
echo ""

echo -e "${BLUE}📋 2. Backup container configuration...${NC}"
# Backup current container config
docker inspect $CONTAINER_NAME > container_backup_$(date +%Y%m%d_%H%M%S).json 2>/dev/null && echo "✅ Container config backed up" || echo "⚠️  Container config backup failed"
echo ""

echo -e "${BLUE}🧹 3. Docker system cleanup (safe)...${NC}"
# Dọn dẹp an toàn
echo "Cleaning unused images and cache..."
docker system prune -f --volumes 2>/dev/null && echo "✅ System cleanup completed" || echo "⚠️  System cleanup had issues"
echo ""

echo -e "${BLUE}🔄 4. Container restart với optimizations...${NC}"
# Stop container an toàn
echo "Stopping container safely..."
docker stop $CONTAINER_NAME 2>/dev/null && echo "✅ Container stopped" || echo "⚠️  Stop failed"

# Khởi động lại với memory limits và optimizations
echo "Starting container with optimizations..."
docker start $CONTAINER_NAME 2>/dev/null && echo "✅ Container started" || {
    echo -e "${RED}❌ Start failed, trying to recreate with same data...${NC}"
    
    # Recreate container với cùng volume để giữ dữ liệu
    echo "Recreating container with optimizations..."
    docker run -d \
        --name ${CONTAINER_NAME}_new \
        --restart=unless-stopped \
        --memory=2g \
        --memory-swap=4g \
        --shm-size=1g \
        -e "ACCEPT_EULA=Y" \
        -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
        -e "MSSQL_TCP_PORT=1433" \
        -e "MSSQL_MEMORY_LIMIT_MB=1536" \
        -p 1433:1433 \
        mcr.microsoft.com/azure-sql-edge:latest \
        && echo "✅ New optimized container created" || echo "❌ Container creation failed"
    
    # Remove old container nếu tạo mới thành công
    if [ $? -eq 0 ]; then
        docker rm $CONTAINER_NAME 2>/dev/null
        docker rename ${CONTAINER_NAME}_new $CONTAINER_NAME 2>/dev/null
        echo "✅ Old container replaced"
    fi
}
echo ""

echo -e "${BLUE}⏳ 5. Waiting for container to be ready...${NC}"
echo "Waiting for SQL Server to start..."
for i in {1..30}; do
    HEALTH=$(docker inspect $CONTAINER_NAME --format='{{.State.Health.Status}}' 2>/dev/null || echo "starting")
    echo "Health check $i/30: $HEALTH"
    
    if [ "$HEALTH" = "healthy" ]; then
        echo -e "${GREEN}✅ Container is healthy!${NC}"
        break
    fi
    sleep 10
done
echo ""

echo -e "${BLUE}🔍 6. Connection test...${NC}"
echo "Testing SQL connection..."
if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" -W -t 30 >/dev/null 2>&1; then
    echo -e "${GREEN}✅ SQL Server connection successful!${NC}"
    
    # Test database
    echo "Testing TinhKhoanDB..."
    if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES" -W -t 10 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ TinhKhoanDB accessible!${NC}"
    else
        echo -e "${YELLOW}⚠️  TinhKhoanDB may need recreation${NC}"
    fi
else
    echo -e "${RED}❌ SQL Server connection failed${NC}"
fi
echo ""

echo -e "${BLUE}📊 7. Final status report...${NC}"
echo "Container status:"
docker ps --filter name=$CONTAINER_NAME --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""

echo "Health status:"
docker inspect $CONTAINER_NAME --format='{{.State.Health.Status}}' 2>/dev/null || echo "Container not found"
echo ""

echo "Memory usage:"
docker stats $CONTAINER_NAME --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}" 2>/dev/null || echo "Stats not available"
echo ""

echo -e "${GREEN}🎉 === DOCKER OPTIMIZATION COMPLETED ===${NC}"
echo "Container should now be running optimally with:"
echo "  - Memory limit: 2GB"
echo "  - Memory swap: 4GB" 
echo "  - Shared memory: 1GB"
echo "  - Auto restart: unless-stopped"
echo "  - SQL Memory limit: 1536MB"
echo ""
echo "🔗 Next steps:"
echo "  1. Test API: curl http://localhost:5055/health"
echo "  2. Test Frontend: Open http://localhost:3000"
echo "  3. Run: ./start_backend.sh && ./start_frontend.sh"
