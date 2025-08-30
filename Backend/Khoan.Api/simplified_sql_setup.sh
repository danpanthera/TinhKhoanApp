#!/bin/bash

# =============================================================================
# SIMPLIFIED AZURE SQL EDGE SETUP FOR M3 MAX
# Cấu hình đơn giản và ổn định nhất
# =============================================================================

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m'

echo -e "${WHITE}🚀 SIMPLIFIED AZURE SQL EDGE SETUP FOR M3 MAX${NC}"
echo -e "${CYAN}================================================${NC}"
echo -e "${YELLOW}💡 Cấu hình đơn giản và ổn định nhất${NC}"
echo ""

# Stop and remove existing container
echo -e "${YELLOW}🛑 Dừng và xóa container hiện tại...${NC}"
docker stop azure_sql_edge_ultimate_v2 2>/dev/null || true
docker rm azure_sql_edge_ultimate_v2 2>/dev/null || true

# Remove problematic volumes
echo -e "${YELLOW}💾 Xóa volumes có vấn đề...${NC}"
docker volume rm sqldata_ultimate_v2 sqllogs_ultimate_v2 sqlconfig_ultimate_v2 2>/dev/null || true

# Remove problematic network
echo -e "${YELLOW}🌐 Xóa network có vấn đề...${NC}"
docker network rm tinhkhoan_ultimate_network 2>/dev/null || true

echo -e "${GREEN}✅ Cleanup completed${NC}"

echo ""
echo -e "${BLUE}🐳 SIMPLIFIED CONTAINER SETUP${NC}"
echo -e "${CYAN}================================${NC}"

# Create simple volume
echo -e "${PURPLE}💾 Tạo volume đơn giản...${NC}"
docker volume create sqldata_simple

echo -e "${GREEN}✅ Simple volume created${NC}"

# Launch simplified container
echo -e "${PURPLE}🚀 Khởi động container đơn giản...${NC}"
docker run -d \
  --name azure_sql_edge_tinhkhoan \
  --restart unless-stopped \
  --memory=6g \
  --cpus="6" \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  -v sqldata_simple:/var/opt/mssql \
  mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ Simplified container started${NC}"

echo ""
echo -e "${YELLOW}⏳ HEALTH CHECK${NC}"
echo -e "${CYAN}=================${NC}"

# Wait for container to start
echo -e "${PURPLE}⏳ Chờ container khởi động...${NC}"
sleep 30

# Check container status
echo -e "${CYAN}📊 Container status:${NC}"
if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -q azure_sql_edge_tinhkhoan; then
    echo -e "${GREEN}✅ Container is running${NC}"
    docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
else
    echo -e "${RED}❌ Container is not running${NC}"
    echo -e "${YELLOW}📋 Checking logs...${NC}"
    docker logs azure_sql_edge_tinhkhoan --tail 20
    exit 1
fi

# Test SQL connection
echo -e "${CYAN}🔌 Testing SQL Server connection:${NC}"
CONNECTION_SUCCESS=false

for i in {1..20}; do
    echo "🔄 Connection attempt $i/20..."

    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION" -C -t 15 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server is ready!${NC}"
        CONNECTION_SUCCESS=true
        break
    fi

    # Check if container is still running
    if ! docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -q azure_sql_edge_tinhkhoan; then
        echo -e "${RED}❌ Container stopped during connection test${NC}"
        docker logs azure_sql_edge_tinhkhoan --tail 20
        exit 1
    fi

    echo "⏱️  Waiting 10 seconds..."
    sleep 10
done

if [ "$CONNECTION_SUCCESS" = true ]; then
    echo ""
    echo -e "${WHITE}🎉 SETUP COMPLETED SUCCESSFULLY!${NC}"
    echo -e "${CYAN}====================================${NC}"

    # Display SQL Server info
    echo -e "${BLUE}📋 SQL Server Information:${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION" -C

    # Create KhoanDB
    echo -e "${PURPLE}🗄️  Creating KhoanDB...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KhoanDB')
    BEGIN
        CREATE DATABASE KhoanDB
        COLLATE SQL_Latin1_General_CP1_CI_AS
        PRINT '✅ KhoanDB created'
    END
    ELSE
        PRINT '✅ KhoanDB already exists'
    " -C

    echo -e "${GREEN}✅ Database ready${NC}"

    # Show performance stats
    echo -e "${BLUE}📊 Performance Stats:${NC}"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}"

    echo ""
    echo -e "${WHITE}🏆 ENVIRONMENT READY FOR DEVELOPMENT!${NC}"
    echo -e "${CYAN}Connection Details:${NC}"
    echo "   🌐 Server: localhost,1433"
    echo "   👤 Username: sa"
    echo "   🔐 Password: Dientoan@303"
    echo "   🗄️  Database: KhoanDB"
    echo ""
    echo -e "${YELLOW}Next Steps:${NC}"
    echo "   1. 🚀 Start Backend: cd Backend/KhoanApp.Api && ./start_backend.sh"
    echo "   2. 🎨 Start Frontend: cd Frontend/KhoanUI && ./start_frontend.sh"
    echo "   3. 📥 Restore Database: ./restore_database_complete.sh"

else
    echo -e "${RED}❌ SQL SERVER CONNECTION FAILED${NC}"
    echo -e "${YELLOW}📋 Final container logs:${NC}"
    docker logs azure_sql_edge_tinhkhoan --tail 30

    echo -e "${PURPLE}💡 Troubleshooting:${NC}"
    echo "   1. Check Docker Desktop is running"
    echo "   2. Restart Docker Desktop"
    echo "   3. Check available memory (need at least 6GB free)"
    echo "   4. Try running script again"
fi

echo ""
echo -e "${WHITE}🎯 SCRIPT COMPLETED${NC}"
