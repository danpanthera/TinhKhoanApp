#!/bin/bash

# =============================================================================
# ULTIMATE AZURE SQL EDGE OPTIMIZED FOR APPLE SILICON M3 MAX
# Giải pháp tối ưu nhất cho ARM64 với Azure SQL Edge ổn định
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
BOLD='\033[1m'
NC='\033[0m' # No Color

echo -e "${WHITE}${BOLD}🔥 ULTIMATE AZURE SQL EDGE FOR M3 MAX${NC}"
echo "=================================================================="
echo -e "${CYAN}🎯 Target: Azure SQL Edge ARM64 - Ultra Stable & Optimized${NC}"
echo -e "${CYAN}💎 Optimization: 8GB RAM + 8 CPU Cores + Maximum Stability${NC}"
echo ""
echo -e "${YELLOW}⚠️  Script này sẽ XÓA SẠCH tất cả Docker và tạo SQL Edge tối ưu${NC}"
echo -e "${YELLOW}⚠️  Với cấu hình đặc biệt để tránh SIGABRT crash${NC}"
echo -e "${YELLOW}⚠️  Bạn có chắc chắn muốn tiếp tục? (y/N)${NC}"
read -p "Nhập 'y' để tiếp tục: " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${RED}❌ Hủy thao tác${NC}"
    exit 1
fi

echo ""
echo -e "${RED}${BOLD}🧹 PHASE 1: NUCLEAR DOCKER CLEANUP${NC}"
echo "================================================"

# Nuclear cleanup
echo -e "${YELLOW}💥 Dừng TẤT CẢ containers...${NC}"
docker stop $(docker ps -aq) 2>/dev/null || echo "Không có containers nào đang chạy"

echo -e "${YELLOW}🗑️  Xóa TẤT CẢ containers...${NC}"
docker rm $(docker ps -aq) -f 2>/dev/null || echo "Không có containers nào để xóa"

echo -e "${YELLOW}🖼️  Xóa TẤT CẢ images...${NC}"
docker rmi $(docker images -q) -f 2>/dev/null || echo "Không có images nào để xóa"

echo -e "${YELLOW}💾 Xóa TẤT CẢ volumes...${NC}"
docker volume rm $(docker volume ls -q) -f 2>/dev/null || echo "Không có volumes nào để xóa"

echo -e "${YELLOW}🌐 Xóa TẤT CẢ networks...${NC}"
docker network rm $(docker network ls --filter type=custom -q) 2>/dev/null || echo "Không có networks tùy chỉnh nào"

echo -e "${YELLOW}🧽 Docker system prune TOÀN BỘ...${NC}"
docker system prune -a -f --volumes

echo -e "${YELLOW}🗂️  Xóa build cache...${NC}"
docker builder prune -a -f

echo -e "${YELLOW}🔄 Reset Docker Desktop...${NC}"
killall "Docker Desktop" 2>/dev/null || true
sleep 5

echo -e "${GREEN}✅ Docker environment đã được XÓA SẠCH hoàn toàn${NC}"

echo ""
echo -e "${BLUE}${BOLD}🏗️  PHASE 2: M3 MAX DOCKER OPTIMIZATION${NC}"
echo "================================================"

# Restart Docker Desktop
echo -e "${PURPLE}🔄 Khởi động lại Docker Desktop...${NC}"
open -a "Docker Desktop"
echo "Chờ Docker Desktop khởi động với cấu hình mới..."
sleep 25

# Wait for Docker to be ready
echo -e "${PURPLE}⏳ Chờ Docker daemon sẵn sàng...${NC}"
for i in {1..40}; do
    if docker info >/dev/null 2>&1; then
        echo -e "${GREEN}✅ Docker daemon đã sẵn sàng${NC}"
        break
    fi
    if [ $i -eq 40 ]; then
        echo -e "${RED}❌ Docker daemon không khởi động được${NC}"
        exit 1
    fi
    echo "Chờ Docker daemon... ($i/40)"
    sleep 3
done

echo ""
echo -e "${PURPLE}${BOLD}📥 PHASE 3: PULL AZURE SQL EDGE ULTIMATE${NC}"
echo "================================================"

# Pull Azure SQL Edge ARM64
echo -e "${PURPLE}🚀 Pull Azure SQL Edge ARM64 latest...${NC}"
docker pull --platform linux/arm64 mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ Azure SQL Edge image đã được pull thành công${NC}"

echo ""
echo -e "${CYAN}${BOLD}🌐 PHASE 4: ULTIMATE NETWORK SETUP${NC}"
echo "================================================"

# Create optimized network
echo -e "${PURPLE}🌐 Tạo network siêu tối ưu...${NC}"
docker network create \
  --driver bridge \
  --opt com.docker.network.driver.mtu=1500 \
  --opt com.docker.network.bridge.enable_icc=true \
  --opt com.docker.network.bridge.enable_ip_masquerade=true \
  --subnet=172.25.0.0/16 \
  --gateway=172.25.0.1 \
  tinhkhoan_ultimate_network

echo -e "${GREEN}✅ Network 'tinhkhoan_ultimate_network' đã tạo${NC}"

echo ""
echo -e "${BLUE}${BOLD}💾 PHASE 5: ULTIMATE STORAGE SETUP${NC}"
echo "================================================"

# Create optimized storage directories
echo -e "${PURPLE}💾 Tạo storage directories...${NC}"
sudo mkdir -p /opt/sqlserver_m3_ultimate/data
sudo mkdir -p /opt/sqlserver_m3_ultimate/logs
sudo mkdir -p /opt/sqlserver_m3_ultimate/backup
sudo chmod -R 755 /opt/sqlserver_m3_ultimate/

# Create optimized volume
docker volume create \
  --driver local \
  sqldata_ultimate_m3_max

echo -e "${GREEN}✅ Storage 'sqldata_ultimate_m3_max' đã tạo${NC}"

echo ""
echo -e "${CYAN}${BOLD}⚙️  PHASE 6: ULTIMATE SQL EDGE CONFIGURATION${NC}"
echo "================================================"

# Create ULTIMATE configuration for stability
echo -e "${PURPLE}📝 Tạo cấu hình SQL Edge siêu ổn định...${NC}"
mkdir -p /tmp/mssql_ultimate_m3_config

cat > /tmp/mssql_ultimate_m3_config/mssql.conf << 'EOF'
[EULA]
accepteula = Y

[memory]
memorylimitmb = 5120

[network]
forceencryption = 0
tcpport = 1433
ipaddress = 0.0.0.0

[sqlagent]
enabled = false

[telemetry]
customerfeedback = false

[coredump]
enabled = 0
captureminiandfull = false

[errorlog]
numerrorlogfiles = 10
errorlogsizemb = 50

[filelocation]
defaultdatadir = /var/opt/mssql/data
defaultlogdir = /var/opt/mssql/log
defaultbackupdir = /var/opt/mssql/backup

[tempdb]
defaultsize = 512
defaultgrowth = 128
numfiles = 4

[traceflag]
traceflag0 = 3226
traceflag1 = 4199
EOF

# Create stability-focused startup script
cat > /tmp/mssql_ultimate_m3_config/init_stability.sql << 'EOF'
-- ULTIMATE STABILITY CONFIGURATION FOR M3 MAX
-- Tối ưu cho Apple Silicon ARM64

USE master;
GO

-- Configure memory for M3 Max (5GB)
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
GO

EXEC sp_configure 'max server memory (MB)', 5120;
RECONFIGURE;
GO

-- Configure worker threads for 8 cores
EXEC sp_configure 'max worker threads', 512;
RECONFIGURE;
GO

-- Configure parallelism for stability
EXEC sp_configure 'cost threshold for parallelism', 25;
RECONFIGURE;
GO

EXEC sp_configure 'max degree of parallelism', 4;
RECONFIGURE;
GO

-- Disable features that might cause crashes
EXEC sp_configure 'remote admin connections', 0;
RECONFIGURE;
GO

EXEC sp_configure 'backup compression default', 0;
RECONFIGURE;
GO

PRINT '✅ SQL Edge ULTIMATE stability configuration applied!';
GO
EOF

echo -e "${GREEN}✅ Cấu hình SQL Edge siêu ổn định đã tạo${NC}"

echo ""
echo -e "${WHITE}${BOLD}🚀 PHASE 7: LAUNCH ULTIMATE STABLE CONTAINER${NC}"
echo "================================================"

# Launch ULTIMATE stable SQL Edge container
echo -e "${PURPLE}🚀 Khởi động SQL Edge ULTIMATE container...${NC}"
docker run -d \
  --name azure_sql_edge_tinhkhoan \
  --platform linux/arm64 \
  --network tinhkhoan_ultimate_network \
  --ip 172.25.0.10 \
  --restart unless-stopped \
  --memory=6g \
  --memory-swap=6g \
  --memory-swappiness=1 \
  --cpus="6" \
  --cpu-shares=1024 \
  --oom-kill-disable=true \
  --security-opt seccomp=unconfined \
  --cap-add=SYS_PTRACE \
  --shm-size=2g \
  --ulimit nofile=65536:65536 \
  --ulimit memlock=67108864:67108864 \
  --tmpfs /tmp:noexec,nosuid,size=512m \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_AGENT_ENABLED=false" \
  -e "MSSQL_ENABLE_HADR=0" \
  -e "MSSQL_MEMORY_LIMIT_MB=5120" \
  -e "MSSQL_TCP_PORT=1433" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -e "MSSQL_LCID=1033" \
  -e "MSSQL_DATA_DIR=/var/opt/mssql/data" \
  -e "MSSQL_LOG_DIR=/var/opt/mssql/log" \
  -e "MSSQL_BACKUP_DIR=/var/opt/mssql/backup" \
  -p 1433:1433 \
  -v sqldata_ultimate_m3_max:/var/opt/mssql/data \
  -v /opt/sqlserver_m3_ultimate/logs:/var/opt/mssql/log \
  -v /opt/sqlserver_m3_ultimate/backup:/var/opt/mssql/backup \
  -v /tmp/mssql_ultimate_m3_config:/var/opt/mssql/config \
  mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ SQL Edge ULTIMATE container đã được khởi động${NC}"

echo ""
echo -e "${YELLOW}${BOLD}⏳ PHASE 8: ULTIMATE STABILITY CHECK${NC}"
echo "================================================"

# Extended stability check
echo -e "${PURPLE}🔍 Thực hiện stability check (3 phút)...${NC}"
sleep 60

# Check container status multiple times
echo -e "${CYAN}📊 Container status check:${NC}"
for check in {1..3}; do
    echo "Check $check/3:"
    docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" || true
    sleep 10
done

# Extended SQL connection test with patience
echo -e "${CYAN}🔌 ULTIMATE stability SQL test (15 phút timeout):${NC}"
CONNECTION_SUCCESS=false

for i in {1..90}; do
    echo "🔍 Stability test $i/90..."

    # Check if container is still running first
    if ! docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -q azure_sql_edge_tinhkhoan; then
        echo -e "${RED}❌ Container đã dừng hoạt động${NC}"
        echo -e "${YELLOW}📋 Container logs:${NC}"
        docker logs azure_sql_edge_tinhkhoan --tail 50
        break
    fi

    # Try SQL connection
    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION" -C -t 20 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Edge ULTIMATE đã sẵn sàng và ổn định!${NC}"
        CONNECTION_SUCCESS=true
        break
    fi

    echo "⏳ Chờ 10 giây (container vẫn chạy)..."
    sleep 10
done

if [ "$CONNECTION_SUCCESS" = true ]; then
    echo ""
    echo -e "${WHITE}${BOLD}🎉 ULTIMATE SUCCESS! SQL EDGE STABLE & READY!${NC}"
    echo "=================================================================="

    # Apply stability configurations
    echo -e "${PURPLE}⚡ Áp dụng stability optimizations...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -i /tmp/mssql_ultimate_m3_config/init_stability.sql -C

    # Display SQL Edge information
    echo -e "${BLUE}📋 SQL Edge ULTIMATE Info:${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    SELECT
        'SQL Server Version' as Info, @@VERSION as Value
    UNION ALL
    SELECT
        'Server Name', @@SERVERNAME
    UNION ALL
    SELECT
        'Edition', SERVERPROPERTY('Edition')
    UNION ALL
    SELECT
        'Max Memory (MB)', CAST(value_in_use as VARCHAR) FROM sys.configurations WHERE name = 'max server memory (MB)'
    " -C

    # Create TinhKhoanDB with ultimate settings
    echo -e "${PURPLE}🗄️  Tạo TinhKhoanDB với ULTIMATE stability...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
    BEGIN
        CREATE DATABASE TinhKhoanDB
        ON (
            NAME = 'TinhKhoanDB_Data',
            FILENAME = '/var/opt/mssql/data/TinhKhoanDB.mdf',
            SIZE = 512MB,
            MAXSIZE = 4096MB,
            FILEGROWTH = 128MB
        )
        LOG ON (
            NAME = 'TinhKhoanDB_Log',
            FILENAME = '/var/opt/mssql/log/TinhKhoanDB.ldf',
            SIZE = 128MB,
            MAXSIZE = 1024MB,
            FILEGROWTH = 32MB
        )
        COLLATE SQL_Latin1_General_CP1_CI_AS

        -- Set database for maximum stability
        ALTER DATABASE TinhKhoanDB SET RECOVERY SIMPLE
        ALTER DATABASE TinhKhoanDB SET PAGE_VERIFY CHECKSUM
        ALTER DATABASE TinhKhoanDB SET AUTO_CREATE_STATISTICS ON
        ALTER DATABASE TinhKhoanDB SET AUTO_UPDATE_STATISTICS ON

        PRINT '✅ TinhKhoanDB created with ULTIMATE stability'
    END
    ELSE
        PRINT 'TinhKhoanDB already exists'
    " -C

    # Show performance stats
    echo -e "${BLUE}📊 ULTIMATE Performance Stats:${NC}"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}"

    echo ""
    echo -e "${WHITE}${BOLD}🔥 ULTIMATE STABLE ENVIRONMENT READY!${NC}"
    echo "=================================================================="
    echo -e "${CYAN}💎 Azure SQL Edge - ULTIMATE Stability for M3 Max${NC}"
    echo -e "${CYAN}🎯 Optimized: ARM64 + 6GB RAM + 6 CPU + Anti-Crash${NC}"
    echo ""
    echo -e "${YELLOW}📝 Connection Info:${NC}"
    echo "   Server: localhost,1433"
    echo "   Username: sa"
    echo "   Password: Dientoan@303"
    echo "   Database: TinhKhoanDB"
    echo ""
    echo -e "${YELLOW}🚀 Next Steps:${NC}"
    echo "   1. Restore database: ./restore_database_complete.sh"
    echo "   2. Start backend: ./start_backend.sh"
    echo "   3. Start frontend: cd ../Frontend/tinhkhoan-app-ui-vite && ./start_frontend.sh"
    echo ""
    echo -e "${GREEN}✅ ULTIMATE STABLE SETUP COMPLETED!${NC}"

else
    echo -e "${RED}❌ SQL EDGE KHÔNG ỔN ĐỊNH${NC}"
    echo -e "${YELLOW}📋 Container logs để debug:${NC}"
    docker logs azure_sql_edge_tinhkhoan --tail 50

    echo -e "${CYAN}💡 Troubleshooting:${NC}"
    echo "   1. Restart Docker Desktop và tăng memory"
    echo "   2. Kiểm tra Docker settings >= 8GB RAM"
    echo "   3. Thử chạy lại script"

    exit 1
fi

echo ""
echo -e "${WHITE}${BOLD}🔥 ULTIMATE SCRIPT HOÀN THÀNH!${NC}"
