#!/bin/bash

# =============================================================================
# ULTIMATE AZURE SQL EDGE OPTIMIZED FOR APPLE SILICON M3 MAX (FIXED)
# Giải pháp tối ưu nhất cho ARM64 với Azure SQL Edge ổn định - Fixed volumes
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

echo -e "${WHITE}${BOLD}🔥 ULTIMATE AZURE SQL EDGE FOR M3 MAX (FIXED)${NC}"
echo "=================================================================="
echo -e "${CYAN}🎯 Target: Azure SQL Edge ARM64 - Ultra Stable & Optimized${NC}"
echo -e "${CYAN}💎 Optimization: 6GB RAM + 6 CPU Cores + Maximum Stability${NC}"
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

# Stop existing container if running
echo -e "${YELLOW}💥 Dừng container hiện tại...${NC}"
docker stop azure_sql_edge_tinhkhoan 2>/dev/null || echo "Container không chạy"
docker rm azure_sql_edge_tinhkhoan 2>/dev/null || echo "Container không tồn tại"

echo -e "${YELLOW}🗑️  Dọn dẹp containers, images, volumes...${NC}"
docker container prune -f
docker image prune -a -f
docker volume prune -f
docker network prune -f

echo -e "${YELLOW}🧽 System cleanup...${NC}"
docker system prune -a -f --volumes

echo -e "${GREEN}✅ Docker environment đã được dọn dẹp${NC}"

echo ""
echo -e "${BLUE}${BOLD}🏗️  PHASE 2: M3 MAX DOCKER CHECK${NC}"
echo "================================================"

# Verify Docker is running
echo -e "${PURPLE}🔍 Kiểm tra Docker daemon...${NC}"
if ! docker info >/dev/null 2>&1; then
    echo -e "${YELLOW}⏳ Chờ Docker daemon khởi động...${NC}"
    for i in {1..30}; do
        if docker info >/dev/null 2>&1; then
            echo -e "${GREEN}✅ Docker daemon đã sẵn sàng${NC}"
            break
        fi
        if [ $i -eq 30 ]; then
            echo -e "${RED}❌ Docker daemon không sẵn sàng${NC}"
            exit 1
        fi
        echo "Chờ Docker... ($i/30)"
        sleep 2
    done
else
    echo -e "${GREEN}✅ Docker daemon đã sẵn sàng${NC}"
fi

echo ""
echo -e "${PURPLE}${BOLD}📥 PHASE 3: PULL AZURE SQL EDGE${NC}"
echo "================================================"

# Pull Azure SQL Edge
echo -e "${PURPLE}🚀 Pull Azure SQL Edge ARM64...${NC}"
docker pull --platform linux/arm64 mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ Azure SQL Edge image đã được pull thành công${NC}"

echo ""
echo -e "${CYAN}${BOLD}🌐 PHASE 4: OPTIMIZED NETWORK${NC}"
echo "================================================"

# Create optimized network
echo -e "${PURPLE}🌐 Tạo network tối ưu...${NC}"
docker network create \
  --driver bridge \
  --opt com.docker.network.driver.mtu=1500 \
  --subnet=172.25.0.0/16 \
  --gateway=172.25.0.1 \
  tinhkhoan_ultimate_network 2>/dev/null || echo "Network đã tồn tại"

echo -e "${GREEN}✅ Network đã sẵn sàng${NC}"

echo ""
echo -e "${BLUE}${BOLD}💾 PHASE 5: OPTIMIZED STORAGE${NC}"
echo "================================================"

# Create Docker volumes (no bind mounts to avoid permission issues)
echo -e "${PURPLE}💾 Tạo Docker volumes...${NC}"
docker volume create sqldata_ultimate_m3_max 2>/dev/null || echo "Data volume đã tồn tại"
docker volume create sqllogs_ultimate_m3_max 2>/dev/null || echo "Logs volume đã tồn tại"
docker volume create sqlbackup_ultimate_m3_max 2>/dev/null || echo "Backup volume đã tồn tại"

echo -e "${GREEN}✅ Storage volumes đã sẵn sàng${NC}"

echo ""
echo -e "${CYAN}${BOLD}⚙️  PHASE 6: ULTIMATE SQL EDGE CONFIGURATION${NC}"
echo "================================================"

# Create configuration directory in tmp
echo -e "${PURPLE}📝 Tạo cấu hình SQL Edge siêu ổn định...${NC}"
mkdir -p /tmp/mssql_ultimate_m3_config

cat > /tmp/mssql_ultimate_m3_config/mssql.conf << 'EOF'
[EULA]
accepteula = Y

[memory]
memorylimitmb = 4096

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
numerrorlogfiles = 5
errorlogsizemb = 50

[filelocation]
defaultdatadir = /var/opt/mssql/data
defaultlogdir = /var/opt/mssql/log
defaultbackupdir = /var/opt/mssql/backup

[tempdb]
defaultsize = 256
defaultgrowth = 64
numfiles = 4

[traceflag]
traceflag0 = 3226
traceflag1 = 4199
EOF

echo -e "${GREEN}✅ Cấu hình SQL Edge siêu ổn định đã tạo${NC}"

echo ""
echo -e "${WHITE}${BOLD}🚀 PHASE 7: LAUNCH ULTIMATE STABLE CONTAINER${NC}"
echo "================================================"

# Launch ULTIMATE stable SQL Edge container (simplified for M3 Max)
echo -e "${PURPLE}🚀 Khởi động SQL Edge ULTIMATE container...${NC}"
docker run -d \
  --name azure_sql_edge_tinhkhoan \
  --platform linux/arm64 \
  --network tinhkhoan_ultimate_network \
  --restart unless-stopped \
  --memory=5g \
  --cpus="4" \
  --shm-size=1g \
  --ulimit nofile=65536:65536 \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_AGENT_ENABLED=false" \
  -e "MSSQL_ENABLE_HADR=0" \
  -e "MSSQL_MEMORY_LIMIT_MB=4096" \
  -e "MSSQL_TCP_PORT=1433" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -p 1433:1433 \
  -v sqldata_ultimate_m3_max:/var/opt/mssql/data \
  -v sqllogs_ultimate_m3_max:/var/opt/mssql/log \
  -v sqlbackup_ultimate_m3_max:/var/opt/mssql/backup \
  -v /tmp/mssql_ultimate_m3_config:/var/opt/mssql/config:ro \
  mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ SQL Edge ULTIMATE container đã được khởi động${NC}"

echo ""
echo -e "${YELLOW}${BOLD}⏳ PHASE 8: ULTIMATE STABILITY CHECK${NC}"
echo "================================================"

# Quick stability check
echo -e "${PURPLE}🔍 Kiểm tra container stability (90 giây)...${NC}"
sleep 15

# Check container status
echo -e "${CYAN}📊 Container status:${NC}"
docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Extended SQL connection test
echo -e "${CYAN}🔌 SQL connection test (timeout 5 phút):${NC}"
CONNECTION_SUCCESS=false

for i in {1..30}; do
    echo "🔍 Connection test $i/30..."

    # Check if container is still running
    if ! docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -q azure_sql_edge_tinhkhoan; then
        echo -e "${RED}❌ Container đã dừng hoạt động${NC}"
        echo -e "${YELLOW}📋 Container logs:${NC}"
        docker logs azure_sql_edge_tinhkhoan --tail 20
        break
    fi

    # Try SQL connection
    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION" -C -t 15 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Edge ULTIMATE đã sẵn sàng và ổn định!${NC}"
        CONNECTION_SUCCESS=true
        break
    fi

    echo "⏳ Chờ 10 giây..."
    sleep 10
done

if [ "$CONNECTION_SUCCESS" = true ]; then
    echo ""
    echo -e "${WHITE}${BOLD}🎉 ULTIMATE SUCCESS! SQL EDGE STABLE & READY!${NC}"
    echo "=================================================================="

    # Display SQL Edge information
    echo -e "${BLUE}📋 SQL Edge ULTIMATE Info:${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    SELECT
        'SQL Server Version' as Info, LEFT(@@VERSION, 50) as Value
    UNION ALL
    SELECT
        'Server Name', @@SERVERNAME
    UNION ALL
    SELECT
        'Edition', CAST(SERVERPROPERTY('Edition') as VARCHAR(50))
    " -C

    # Create TinhKhoanDB with optimized settings
    echo -e "${PURPLE}🗄️  Tạo TinhKhoanDB với settings tối ưu...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
    BEGIN
        CREATE DATABASE TinhKhoanDB
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
    echo -e "${BLUE}📊 Performance Stats:${NC}"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}"

    # Show volumes
    echo -e "${BLUE}💾 Storage Volumes:${NC}"
    docker volume ls --filter name=sql

    echo ""
    echo -e "${WHITE}${BOLD}🔥 ULTIMATE STABLE ENVIRONMENT READY!${NC}"
    echo "=================================================================="
    echo -e "${CYAN}💎 Azure SQL Edge - ULTIMATE Stability for M3 Max${NC}"
    echo -e "${CYAN}🎯 Optimized: ARM64 + 5GB RAM + 4 CPU + Anti-Crash${NC}"
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
    echo -e "${GREEN}✅ ULTIMATE STABLE SETUP COMPLETED SUCCESSFULLY!${NC}"

else
    echo -e "${RED}❌ SQL EDGE KHÔNG ỔN ĐỊNH${NC}"
    echo -e "${YELLOW}📋 Container logs để debug:${NC}"
    docker logs azure_sql_edge_tinhkhoan --tail 50

    echo -e "${CYAN}💡 Troubleshooting:${NC}"
    echo "   1. Kiểm tra Docker Desktop memory >= 8GB"
    echo "   2. Restart Docker Desktop"
    echo "   3. Thử chạy lại script"

    # Try to keep container running for debugging
    echo -e "${YELLOW}🔍 Container vẫn chạy để debug. Kiểm tra logs:${NC}"
    echo "   docker logs azure_sql_edge_tinhkhoan"
    echo "   docker exec -it azure_sql_edge_tinhkhoan /bin/bash"
fi

echo ""
echo -e "${WHITE}${BOLD}🔥 ULTIMATE SCRIPT HOÀN THÀNH!${NC}"
