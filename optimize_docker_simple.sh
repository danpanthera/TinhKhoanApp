#!/bin/bash

# =============================================================================
# DOCKER OPTIMIZATION SCRIPT - SIMPLE VERSION FOR DOCKER DESKTOP
# Tối ưu Docker Container cho macOS Docker Desktop
# =============================================================================

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

CONTAINER_NAME="azure_sql_edge_tinhkhoan"
DB_PASSWORD="Dientoan@303"

echo -e "${BLUE}🐳 BẮT ĐẦU QUÁ TRÌNH TỐI ƯU DOCKER CONTAINER (SIMPLE)${NC}"
echo "=============================================="

# Check if container exists and is running
if ! docker ps | grep -q "$CONTAINER_NAME"; then
    echo -e "${RED}❌ Container $CONTAINER_NAME không đang chạy!${NC}"
    exit 1
fi

echo -e "${YELLOW}📊 Trạng thái hiện tại:${NC}"
docker stats $CONTAINER_NAME --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"

# 1. Basic CPU limit (works on Docker Desktop)
echo ""
echo -e "${PURPLE}⚙️ BƯỚC 1: Tối ưu CPU cơ bản${NC}"
echo "Đang áp dụng giới hạn CPU (2 cores)..."
docker update --cpus="2" $CONTAINER_NAME
echo -e "${GREEN}✅ CPU giới hạn 2 cores${NC}"

# 2. SQL Server Performance Tuning
echo ""
echo -e "${PURPLE}🔧 BƯỚC 2: Tối ưu SQL Server Configuration${NC}"
echo "Đang áp dụng cấu hình SQL Server cho ARM64 architecture..."

sqlcmd -S localhost,1433 -U sa -P "$DB_PASSWORD" -C -Q "
-- Performance tuning for ARM64
EXEC sp_configure 'cost threshold for parallelism', 50;
EXEC sp_configure 'max degree of parallelism', 2;
EXEC sp_configure 'optimize for ad hoc workloads', 1;
EXEC sp_configure 'max server memory (MB)', 3072;
RECONFIGURE WITH OVERRIDE;
PRINT 'SQL Server configuration optimized';
" -h -1

echo -e "${GREEN}✅ SQL Server performance tuning applied${NC}"

# 3. Create TinhKhoanDB if not exists
echo ""
echo -e "${PURPLE}🗄️ BƯỚC 3: Tạo Database TinhKhoanDB${NC}"
sqlcmd -S localhost,1433 -U sa -P "$DB_PASSWORD" -C -Q "
IF NOT EXISTS(SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    CREATE DATABASE TinhKhoanDB;
    PRINT 'TinhKhoanDB created successfully';
END
ELSE
    PRINT 'TinhKhoanDB already exists';
" -h -1

echo -e "${GREEN}✅ TinhKhoanDB ready${NC}"

# 4. Container Health Check
echo ""
echo -e "${PURPLE}🏥 BƯỚC 4: Kiểm tra sức khỏe Container${NC}"
echo "Đang kiểm tra connectivity và performance..."

# Test database connectivity
if sqlcmd -S localhost,1433 -U sa -P "$DB_PASSWORD" -Q "SELECT @@VERSION" -C -h -1 >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Database connectivity OK${NC}"
else
    echo -e "${RED}❌ Database connectivity failed${NC}"
    exit 1
fi

# Check container resource usage
echo "📊 Resource usage after optimization:"
docker stats $CONTAINER_NAME --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"

echo ""
echo -e "${GREEN}🎉 DOCKER OPTIMIZATION COMPLETED SUCCESSFULLY!${NC}"
echo ""
echo -e "${BLUE}🚀 Container is now optimized and ready for development!${NC}"
echo ""
echo -e "${YELLOW}📋 Next steps:${NC}"
echo "  • Chạy migrations: cd Backend/TinhKhoanApp.Api && dotnet ef database update"
echo "  • Start backend: ./start_backend.sh"
echo "  • Start frontend: ./start_frontend.sh"
