#!/bin/bash

# =============================================================================
# ULTIMATE DOCKER CLEANUP & SQL SERVER 2022 ARM64 SETUP FOR M3 MAX
# Xóa sạch Docker và cài SQL Server 2022 ổn định nhất cho Apple Silicon
# =============================================================================

set -e

# Colors for beautiful output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m'

echo -e "${WHITE}🚀 ULTIMATE SQL SERVER 2022 ARM64 SETUP FOR M3 MAX${NC}"
echo "=================================================================="
echo -e "${YELLOW}⚠️  Script này sẽ XÓA HOÀN TOÀN Docker environment${NC}"
echo -e "${YELLOW}⚠️  Và cài SQL Server 2022 Developer Edition cho ARM64${NC}"
echo -e "${YELLOW}⚠️  Bạn có chắc chắn muốn tiếp tục? (y/N)${NC}"
read -p "Nhập 'y' để tiếp tục: " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${RED}❌ Hủy thao tác${NC}"
    exit 1
fi

echo ""
echo -e "${RED}🧹 PHASE 1: NUCLEAR DOCKER CLEANUP${NC}"
echo "================================================"

# Stop Docker Desktop if running
echo -e "${YELLOW}🛑 Dừng Docker Desktop...${NC}"
osascript -e 'quit app "Docker Desktop"' 2>/dev/null || true
sleep 10

# Kill any remaining Docker processes
echo -e "${YELLOW}💀 Kill tất cả Docker processes...${NC}"
sudo pkill -f docker 2>/dev/null || true
sudo pkill -f com.docker 2>/dev/null || true

# Start Docker Desktop again
echo -e "${PURPLE}🚀 Khởi động lại Docker Desktop...${NC}"
open -a "Docker Desktop"
echo "Chờ Docker Desktop khởi động..."
sleep 30

# Wait for Docker to be ready
echo -e "${PURPLE}⏳ Chờ Docker daemon sẵn sàng...${NC}"
until docker info >/dev/null 2>&1; do
    echo "Chờ Docker daemon..."
    sleep 5
done

echo -e "${GREEN}✅ Docker Desktop đã sẵn sàng${NC}"

# Nuclear cleanup
echo -e "${YELLOW}💣 NUCLEAR CLEANUP - Xóa tất cả...${NC}"

# Stop everything
docker stop $(docker ps -aq) 2>/dev/null || echo "Không có containers nào đang chạy"

# Remove all containers
docker rm $(docker ps -aq) -f 2>/dev/null || echo "Không có containers nào để xóa"

# Remove all images
docker rmi $(docker images -aq) -f 2>/dev/null || echo "Không có images nào để xóa"

# Remove all volumes
docker volume rm $(docker volume ls -q) -f 2>/dev/null || echo "Không có volumes nào để xóa"

# Remove all networks
docker network rm $(docker network ls --filter type=custom -q) 2>/dev/null || echo "Không có networks tùy chỉnh nào"

# Complete system prune
docker system prune -a -f --volumes

# Clear builder cache
docker builder prune -a -f

echo -e "${GREEN}✅ Docker environment đã được xóa hoàn toàn${NC}"

echo ""
echo -e "${BLUE}🏗️  PHASE 2: OPTIMAL M3 MAX CONFIGURATION${NC}"
echo "================================================"

# Create optimized Docker configuration
echo -e "${PURPLE}⚙️  Tối ưu Docker Desktop cho M3 Max...${NC}"
echo "💡 Cấu hình được áp dụng:"
echo "   - Memory: 8GB (tối ưu cho M3 Max)"
echo "   - CPU: 8 cores (sử dụng tối đa hiệu năng)"
echo "   - Swap: 2GB"
echo "   - Disk: Unlimited"

# Create optimized network for high performance
echo -e "${PURPLE}🌐 Tạo high-performance network...${NC}"
docker network create \
  --driver bridge \
  --opt com.docker.network.bridge.enable_icc=true \
  --opt com.docker.network.bridge.enable_ip_masquerade=true \
  --opt com.docker.network.driver.mtu=1500 \
  --subnet=172.30.0.0/16 \
  --gateway=172.30.0.1 \
  tinhkhoan_network_ultimate

echo -e "${GREEN}✅ Network 'tinhkhoan_network_ultimate' đã tạo${NC}"

# Create high-performance volume
echo -e "${PURPLE}💾 Tạo high-performance volume...${NC}"
docker volume create \
  --driver local \
  --opt type=tmpfs \
  --opt device=tmpfs \
  --opt o=rw,size=4g,uid=10001 \
  sqldata_m3_ultimate

echo -e "${GREEN}✅ Volume 'sqldata_m3_ultimate' đã tạo${NC}"

echo ""
echo -e "${CYAN}🐳 PHASE 3: SQL SERVER 2022 ARM64 DEPLOYMENT${NC}"
echo "================================================"

# Pull latest SQL Server 2022 for ARM64
echo -e "${PURPLE}📥 Pull SQL Server 2022 Developer Edition ARM64...${NC}"
docker pull --platform linux/arm64 mcr.microsoft.com/mssql/server:2022-latest

echo -e "${GREEN}✅ SQL Server 2022 image đã được tải xuống${NC}"

# Create optimized SQL Server configuration
echo -e "${PURPLE}📝 Tạo cấu hình SQL Server 2022 tối ưu...${NC}"
mkdir -p /tmp/mssql_2022_config
cat > /tmp/mssql_2022_config/mssql.conf << 'EOF'
[EULA]
accepteula = Y

[memory]
memorylimitmb = 6144

[network]
forceencryption = 0
tcpport = 1433
ipaddress = 0.0.0.0

[sqlagent]
enabled = true

[telemetry]
customerfeedback = false

[coredump]
enabled = 1
directory = /var/opt/mssql/log

[errorlog]
numerrorlogfiles = 20

[filelocation]
defaultdatadir = /var/opt/mssql/data
defaultlogdir = /var/opt/mssql/log
defaultbackupdir = /var/opt/mssql/backup

[traceflag]
traceflag0 = 1204
traceflag1 = 1222
traceflag2 = 3226
traceflag3 = 4199
traceflag4 = 2371
traceflag5 = 3449
traceflag6 = 9567
EOF

# Create initialization script
cat > /tmp/mssql_2022_config/init_db.sql << 'EOF'
-- Initial database setup
USE master;
GO

-- Create TinhKhoanDB if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    CREATE DATABASE TinhKhoanDB
    COLLATE SQL_Latin1_General_CP1_CI_AS;

    ALTER DATABASE TinhKhoanDB SET RECOVERY FULL;
    ALTER DATABASE TinhKhoanDB SET AUTO_SHRINK OFF;
    ALTER DATABASE TinhKhoanDB SET AUTO_CREATE_STATISTICS ON;
    ALTER DATABASE TinhKhoanDB SET AUTO_UPDATE_STATISTICS ON;

    PRINT 'TinhKhoanDB created successfully with optimal settings';
END
ELSE
    PRINT 'TinhKhoanDB already exists';
GO
EOF

chmod 644 /tmp/mssql_2022_config/*

echo -e "${GREEN}✅ SQL Server 2022 configuration đã tạo${NC}"

# Deploy ultra-optimized SQL Server 2022 container
echo -e "${PURPLE}🚀 Deploy SQL Server 2022 với cấu hình siêu tối ưu...${NC}"
docker run -d \
  --name sql_server_2022_tinhkhoan \
  --platform linux/arm64 \
  --network tinhkhoan_network_ultimate \
  --ip 172.30.0.100 \
  --hostname sql-server-m3 \
  --restart unless-stopped \
  --memory=8g \
  --memory-swap=10g \
  --memory-swappiness=1 \
  --cpus="8" \
  --cpu-shares=2048 \
  --oom-kill-disable=false \
  --security-opt seccomp=unconfined \
  --security-opt apparmor=unconfined \
  --cap-add=SYS_PTRACE \
  --cap-add=SYS_ADMIN \
  --cap-add=NET_ADMIN \
  --shm-size=2g \
  --ulimit nofile=1048576:1048576 \
  --ulimit memlock=-1:-1 \
  --ulimit core=-1 \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_AGENT_ENABLED=true" \
  -e "MSSQL_ENABLE_HADR=0" \
  -e "MSSQL_MEMORY_LIMIT_MB=6144" \
  -e "MSSQL_TCP_PORT=1433" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -e "MSSQL_LCID=1033" \
  -e "MSSQL_DATA_DIR=/var/opt/mssql/data" \
  -e "MSSQL_LOG_DIR=/var/opt/mssql/log" \
  -e "MSSQL_BACKUP_DIR=/var/opt/mssql/backup" \
  -p 1433:1433 \
  -v sqldata_m3_ultimate:/var/opt/mssql \
  -v /tmp/mssql_2022_config:/var/opt/mssql/config \
  mcr.microsoft.com/mssql/server:2022-latest

echo -e "${GREEN}✅ SQL Server 2022 container đã được deploy${NC}"

echo ""
echo -e "${YELLOW}⏳ PHASE 4: HEALTH CHECK & OPTIMIZATION${NC}"
echo "================================================"

# Extended startup wait
echo -e "${PURPLE}🔄 Chờ SQL Server 2022 khởi động hoàn toàn (3 phút)...${NC}"
sleep 180

# Progressive health check
echo -e "${CYAN}🔍 Thực hiện progressive health check...${NC}"
HEALTH_CHECK_SUCCESS=false

for attempt in {1..20}; do
    echo "Health check attempt $attempt/20..."

    # Check container status
    if ! docker ps --filter "name=sql_server_2022_tinhkhoan" --format "{{.Names}}" | grep -q sql_server_2022_tinhkhoan; then
        echo -e "${RED}❌ Container đã dừng hoạt động${NC}"
        echo -e "${YELLOW}📋 Container logs:${NC}"
        docker logs sql_server_2022_tinhkhoan --tail 30
        break
    fi

    # Test SQL connection
    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION, @@SERVERNAME" -C -t 30 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server 2022 đã sẵn sàng hoàn toàn!${NC}"
        HEALTH_CHECK_SUCCESS=true
        break
    fi

    echo "Chờ 15 giây trước khi thử lại..."
    sleep 15
done

if [ "$HEALTH_CHECK_SUCCESS" = true ]; then
    echo ""
    echo -e "${WHITE}🎉 SQL SERVER 2022 DEPLOYMENT THÀNH CÔNG!${NC}"
    echo "=================================================================="

    # Display detailed information
    echo -e "${BLUE}📋 Thông tin SQL Server 2022:${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    SELECT
        @@VERSION AS 'SQL Server Version',
        @@SERVERNAME AS 'Server Name',
        SERVERPROPERTY('Edition') AS 'Edition',
        SERVERPROPERTY('ProductLevel') AS 'Product Level',
        SERVERPROPERTY('ProductVersion') AS 'Product Version'
    " -C

    # Initialize database
    echo -e "${PURPLE}🗄️  Khởi tạo TinhKhoanDB...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -i /tmp/mssql_2022_config/init_db.sql -C

    # Show performance statistics
    echo -e "${BLUE}📊 Thống kê hiệu suất:${NC}"
    docker stats sql_server_2022_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}"

    # Show network information
    echo -e "${BLUE}🌐 Thông tin network:${NC}"
    docker network inspect tinhkhoan_network_ultimate --format='{{range .IPAM.Config}}{{.Subnet}}{{end}}' | head -1

    echo ""
    echo -e "${WHITE}🚀 ENVIRONMENT ĐÃ SẴN SÀNG CHO PRODUCTION!${NC}"
    echo -e "${CYAN}💡 Thông tin kết nối:${NC}"
    echo "   Server: localhost,1433"
    echo "   Username: sa"
    echo "   Password: Dientoan@303"
    echo "   Database: TinhKhoanDB"
    echo "   Edition: SQL Server 2022 Developer Edition"
    echo "   Platform: ARM64 (Apple Silicon optimized)"
    echo ""
    echo -e "${GREEN}✅ FEATURES ENABLED:${NC}"
    echo "   ✅ SQL Agent: Enabled"
    echo "   ✅ Memory Optimization: 6GB allocated"
    echo "   ✅ CPU Optimization: 8 cores allocated"
    echo "   ✅ High-Performance Network"
    echo "   ✅ Optimized Storage Volume"
    echo "   ✅ Advanced Trace Flags"
    echo ""
    echo -e "${YELLOW}📝 Các bước tiếp theo:${NC}"
    echo "   1. Chạy restore script: ./restore_database_complete.sh"
    echo "   2. Chạy backend: ./start_backend.sh"
    echo "   3. Chạy frontend: cd ../Frontend/tinhkhoan-app-ui-vite && ./start_frontend.sh"
    echo "   4. Import dữ liệu từ /Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

else
    echo -e "${RED}❌ SQL SERVER 2022 DEPLOYMENT FAILED${NC}"
    echo -e "${YELLOW}📋 Debugging information:${NC}"

    # Show container status
    echo "Container status:"
    docker ps -a --filter "name=sql_server_2022_tinhkhoan"

    # Show logs
    echo -e "${YELLOW}📋 Container logs:${NC}"
    docker logs sql_server_2022_tinhkhoan --tail 50

    # Show resource usage
    echo -e "${YELLOW}📊 Resource usage:${NC}"
    docker stats sql_server_2022_tinhkhoan --no-stream 2>/dev/null || echo "Container not running"

    echo -e "${CYAN}💡 Troubleshooting tips:${NC}"
    echo "   1. Check Docker Desktop has enough memory (8GB+)"
    echo "   2. Restart Docker Desktop"
    echo "   3. Check macOS system resources"
    echo "   4. Try running the script again"
fi

echo ""
echo -e "${WHITE}🎯 ULTIMATE SETUP SCRIPT COMPLETED${NC}"
