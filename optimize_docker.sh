#!/bin/bash

# =============================================================================
# DOCKER OPTIMIZATION SCRIPT FOR TINHKHOAN APP
# Tối ưu Docker Container cho hiệu suất và độ ổn định tối đa
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

echo -e "${BLUE}🐳 BẮT ĐẦU QUÁ TRÌNH TỐI ƯU DOCKER CONTAINER${NC}"
echo "=============================================="

# Check if container exists and is running
if ! docker ps | grep -q "$CONTAINER_NAME"; then
    echo -e "${RED}❌ Container $CONTAINER_NAME không đang chạy!${NC}"
    exit 1
fi

echo -e "${YELLOW}📊 Trạng thái hiện tại:${NC}"
docker stats $CONTAINER_NAME --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"

# 1. CPU & I/O Optimization
echo ""
echo -e "${PURPLE}⚙️ BƯỚC 1: Tối ưu CPU & I/O${NC}"
echo "Đang áp dụng giới hạn CPU (2 cores) và tăng I/O priority..."
docker update --cpus="2" --cpu-shares=1024 --blkio-weight=800 $CONTAINER_NAME
echo -e "${GREEN}✅ CPU giới hạn 2 cores, CPU shares=1024, I/O weight=800${NC}"

# 2. Log Management Optimization
echo ""
echo -e "${PURPLE}📝 BƯỚC 2: Tối ưu Log Management${NC}"
echo "Đang cấu hình log rotation (50MB x 5 files)..."
docker update --log-driver json-file --log-opt max-size=50m --log-opt max-file=5 $CONTAINER_NAME
echo -e "${GREEN}✅ Log rotation configured: 50MB x 5 files${NC}"

# 3. SQL Server Performance Tuning
echo ""
echo -e "${PURPLE}🔧 BƯỚC 3: Tối ưu SQL Server Configuration${NC}"
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

# 4. Database Statistics Update
echo ""
echo -e "${PURPLE}📊 BƯỚC 4: Cập nhật Database Statistics${NC}"
echo "Đang cập nhật statistics cho tối ưu query performance..."

sqlcmd -S localhost,1433 -U sa -P "$DB_PASSWORD" -C -Q "
USE TinhKhoanDB;
UPDATE STATISTICS DP01;
UPDATE STATISTICS EI01;
UPDATE STATISTICS GL01;
UPDATE STATISTICS GL41;
UPDATE STATISTICS LN01;
UPDATE STATISTICS LN03;
UPDATE STATISTICS RR01;
UPDATE STATISTICS DPDA;
PRINT 'Statistics updated for all data tables';
" -h -1

echo -e "${GREEN}✅ Database statistics updated${NC}"

# 5. Container Health Check
echo ""
echo -e "${PURPLE}🏥 BƯỚC 5: Kiểm tra sức khỏe Container${NC}"
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
echo -e "${BLUE}🚀 Container is now optimized for maximum performance and stability!${NC}"
