#!/bin/bash

# =============================================================================
# COMPLETE DOCKER CLEANUP AND REBUILD FOR APPLE SILICON M3 MAX
# Xóa hoàn toàn và tạo lại environment tối ưu cho macOS M3 Max
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
NC='\033[0m' # No Color

echo -e "${WHITE}🚀 COMPLETE DOCKER ENVIRONMENT REBUILD FOR M3 MAX${NC}"
echo "=================================================================="
echo -e "${YELLOW}⚠️  Đây sẽ XÓA HOÀN TOÀN tất cả Docker containers, images, volumes${NC}"
echo -e "${YELLOW}⚠️  Bạn có chắc chắn muốn tiếp tục? (y/N)${NC}"
read -p "Nhập 'y' để tiếp tục: " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${RED}❌ Hủy thao tác${NC}"
    exit 1
fi

echo ""
echo -e "${RED}🧹 PHASE 1: COMPLETE DOCKER CLEANUP${NC}"
echo "================================================"

# Stop all running containers
echo -e "${YELLOW}🛑 Dừng tất cả containers...${NC}"
docker stop $(docker ps -aq) 2>/dev/null || echo "Không có containers nào đang chạy"

# Remove all containers
echo -e "${YELLOW}🗑️  Xóa tất cả containers...${NC}"
docker rm $(docker ps -aq) 2>/dev/null || echo "Không có containers nào để xóa"

# Remove all images
echo -e "${YELLOW}🖼️  Xóa tất cả images...${NC}"
docker rmi $(docker images -q) -f 2>/dev/null || echo "Không có images nào để xóa"

# Remove all volumes
echo -e "${YELLOW}💾 Xóa tất cả volumes...${NC}"
docker volume rm $(docker volume ls -q) -f 2>/dev/null || echo "Không có volumes nào để xóa"

# Remove all networks (except default ones)
echo -e "${YELLOW}🌐 Xóa networks tùy chỉnh...${NC}"
docker network rm $(docker network ls --filter type=custom -q) 2>/dev/null || echo "Không có networks tùy chỉnh nào"

# Prune everything
echo -e "${YELLOW}🧽 Docker system prune (toàn bộ)...${NC}"
docker system prune -a -f --volumes

# Clear build cache
echo -e "${YELLOW}🗂️  Xóa build cache...${NC}"
docker builder prune -a -f

echo -e "${GREEN}✅ Docker environment đã được xóa hoàn toàn${NC}"

echo ""
echo -e "${BLUE}🏗️  PHASE 2: OPTIMIZED M3 MAX SETUP${NC}"
echo "================================================"

# Configure Docker Desktop for M3 Max optimization
echo -e "${PURPLE}⚙️  Tối ưu Docker Desktop settings...${NC}"
echo "💡 Khuyến nghị cấu hình Docker Desktop:"
echo "   - Memory: 8GB (tối thiểu 6GB)"
echo "   - CPU: 8 cores"
echo "   - Disk: 100GB+"
echo "   - Use Virtualization framework"
echo "   - Use Rosetta for x86/amd64 emulation"

# Pull optimized SQL Server image for ARM64
echo -e "${PURPLE}📥 Pull Microsoft SQL Server 2022 for ARM64...${NC}"
# Thử pull Azure SQL Edge trước
docker pull --platform linux/arm64 mcr.microsoft.com/azure-sql-edge:latest

# Create optimized network for TinhKhoan app
echo -e "${PURPLE}🌐 Tạo network tối ưu...${NC}"
docker network create \
  --driver bridge \
  --opt com.docker.network.driver.mtu=1500 \
  --subnet=172.20.0.0/16 \
  tinhkhoan_network

echo -e "${GREEN}✅ Network 'tinhkhoan_network' đã tạo${NC}"

# Create optimized volume with specific options
echo -e "${PURPLE}💾 Tạo volume tối ưu cho database...${NC}"
docker volume create \
  --driver local \
  --opt type=none \
  --opt o=bind \
  --opt device=/tmp/sqldata_m3_optimized \
  sqldata_tinhkhoan_m3

# Create the bind mount directory with proper permissions
mkdir -p /tmp/sqldata_m3_optimized
chmod 755 /tmp/sqldata_m3_optimized

echo -e "${GREEN}✅ Volume 'sqldata_tinhkhoan_m3' đã tạo${NC}"

echo ""
echo -e "${CYAN}🐳 PHASE 3: ULTRA-OPTIMIZED CONTAINER CREATION${NC}"
echo "================================================"

# Create ultra-optimized mssql.conf for M3 Max
echo -e "${PURPLE}📝 Tạo cấu hình SQL Server tối ưu cho M3 Max...${NC}"
mkdir -p /tmp/mssql_m3_config
cat > /tmp/mssql_m3_config/mssql.conf << 'EOF'
[EULA]
accepteula = Y

[memory]
memorylimitmb = 4096

[network]
forceencryption = 0
tcpport = 1433

[sqlagent]
enabled = false

[telemetry]
customerfeedback = false

[coredump]
enabled = 1
directory = /var/opt/mssql/log

[errorlog]
numerrorlogfiles = 10

[filelocation]
defaultdatadir = /var/opt/mssql/data
defaultlogdir = /var/opt/mssql/data
defaultbackupdir = /var/opt/mssql/data

[traceflag]
traceflag0 = 1204
traceflag1 = 1222
traceflag2 = 3226
traceflag3 = 4199
traceflag4 = 2371
EOF

# Create startup script for robust initialization
cat > /tmp/mssql_m3_config/start_sql.sh << 'EOF'
#!/bin/bash
set -e

echo "🚀 Starting SQL Server with M3 optimizations..."

# Set memory and CPU optimizations
export MSSQL_MEMORY_LIMIT_MB=4096
export MSSQL_CPU_LIMIT=8

# Start SQL Server in background
/opt/mssql/bin/sqlservr &
SQL_PID=$!

# Wait for SQL Server to be ready
echo "⏳ Waiting for SQL Server to start..."
for i in {1..60}; do
    if /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT @@VERSION" >/dev/null 2>&1; then
        echo "✅ SQL Server is ready!"
        break
    fi
    if [ $i -eq 60 ]; then
        echo "❌ SQL Server failed to start within 60 seconds"
        exit 1
    fi
    sleep 2
done

# Keep the container running
wait $SQL_PID
EOF

chmod +x /tmp/mssql_m3_config/start_sql.sh

echo -e "${GREEN}✅ Cấu hình SQL Server tối ưu đã tạo${NC}"

# Launch ultra-optimized container
echo -e "${PURPLE}🚀 Khởi động container siêu tối ưu...${NC}"
docker run -d \
  --name azure_sql_edge_tinhkhoan \
  --platform linux/arm64 \
  --network tinhkhoan_network \
  --ip 172.20.0.10 \
  --restart unless-stopped \
  --memory=6g \
  --memory-swap=8g \
  --memory-swappiness=10 \
  --cpus="6" \
  --cpu-shares=1024 \
  --oom-kill-disable=false \
  --security-opt seccomp=unconfined \
  --security-opt apparmor=unconfined \
  --cap-add=SYS_PTRACE \
  --cap-add=NET_ADMIN \
  --shm-size=2g \
  --ulimit nofile=65536:65536 \
  --ulimit memlock=-1:-1 \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_AGENT_ENABLED=false" \
  -e "MSSQL_ENABLE_HADR=0" \
  -e "MSSQL_MEMORY_LIMIT_MB=4096" \
  -e "MSSQL_TCP_PORT=1433" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -p 1433:1433 \
  -v sqldata_tinhkhoan_m3:/var/opt/mssql/data \
  -v /tmp/mssql_m3_config:/var/opt/mssql \
  mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ Container đã được tạo với cấu hình siêu tối ưu${NC}"

echo ""
echo -e "${YELLOW}⏳ PHASE 4: HEALTH CHECK & VALIDATION${NC}"
echo "================================================"

# Extended health check with retries
echo -e "${PURPLE}🔍 Thực hiện health check mở rộng...${NC}"
sleep 30

# Check container status
echo -e "${CYAN}📊 Kiểm tra trạng thái container:${NC}"
docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Test SQL connection with extended timeout
echo -e "${CYAN}🔌 Kiểm tra kết nối SQL Server (timeout 5 phút):${NC}"
CONNECTION_SUCCESS=false

for i in {1..30}; do
    echo "Thử kết nối lần $i/30..."

    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION" -C -t 30 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server đã sẵn sàng!${NC}"
        CONNECTION_SUCCESS=true
        break
    fi

    # Check if container is still running
    if ! docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -q azure_sql_edge_tinhkhoan; then
        echo -e "${RED}❌ Container đã dừng hoạt động${NC}"
        echo -e "${YELLOW}📋 Container logs:${NC}"
        docker logs azure_sql_edge_tinhkhoan --tail 20
        break
    fi

    echo "Chờ 10 giây..."
    sleep 10
done

if [ "$CONNECTION_SUCCESS" = true ]; then
    echo ""
    echo -e "${GREEN}🎉 SETUP THÀNH CÔNG! SQL SERVER ĐÃ SẴN SÀNG${NC}"
    echo "================================================"

    # Display SQL Server information
    echo -e "${BLUE}📋 Thông tin SQL Server:${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION, @@SERVERNAME, DB_NAME()" -C

    # Show container performance stats
    echo -e "${BLUE}📊 Thống kê hiệu suất:${NC}"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}"

    # Create TinhKhoanDB database
    echo -e "${PURPLE}🗄️  Tạo database TinhKhoanDB...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
    BEGIN
        CREATE DATABASE TinhKhoanDB
        COLLATE SQL_Latin1_General_CP1_CI_AS
    END
    " -C

    echo -e "${GREEN}✅ Database TinhKhoanDB đã sẵn sàng${NC}"

    echo ""
    echo -e "${WHITE}🚀 ENVIRONMENT ĐÃ SẴNG SÀNG CHO DEVELOPMENT!${NC}"
    echo -e "${CYAN}💡 Thông tin kết nối:${NC}"
    echo "   Server: localhost,1433"
    echo "   Username: sa"
    echo "   Password: Dientoan@303"
    echo "   Database: TinhKhoanDB"
    echo ""
    echo -e "${YELLOW}📝 Các bước tiếp theo:${NC}"
    echo "   1. Chạy backend: cd Backend/KhoanApp.Api && ./start_backend.sh"
    echo "   2. Chạy frontend: cd Frontend/KhoanUI && ./start_frontend.sh"
    echo "   3. Import dữ liệu từ /Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

else
    echo -e "${RED}❌ SQL SERVER KHÔNG KHỞI ĐỘNG ĐƯỢC${NC}"
    echo -e "${YELLOW}📋 Container logs để debug:${NC}"
    docker logs azure_sql_edge_tinhkhoan --tail 30

    echo -e "${CYAN}💡 Gợi ý khắc phục:${NC}"
    echo "   1. Kiểm tra Docker Desktop settings"
    echo "   2. Tăng memory allocation cho Docker"
    echo "   3. Restart Docker Desktop"
    echo "   4. Chạy lại script này"
fi

echo ""
echo -e "${WHITE}🎯 SCRIPT HOÀN THÀNH${NC}"
