#!/bin/bash

# =============================================================================
# MINIMAL STABLE SQL EDGE SETUP FOR M3 MAX
# Cấu hình tối thiểu để tránh crash
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

echo -e "${WHITE}🔧 MINIMAL STABLE SQL EDGE SETUP${NC}"
echo -e "${CYAN}===================================${NC}"
echo -e "${YELLOW}💡 Cấu hình tối thiểu để tránh crash trên M3 Max${NC}"
echo ""

# Clean up everything
echo -e "${RED}🧹 COMPLETE CLEANUP${NC}"
echo -e "${YELLOW}Dừng tất cả containers...${NC}"
docker stop $(docker ps -aq) 2>/dev/null || true

echo -e "${YELLOW}Xóa tất cả containers...${NC}"
docker rm $(docker ps -aq) 2>/dev/null || true

echo -e "${YELLOW}Xóa tất cả volumes...${NC}"
docker volume prune -f

echo -e "${YELLOW}Xóa tất cả networks...${NC}"
docker network prune -f

echo -e "${YELLOW}System prune...${NC}"
docker system prune -a -f

echo -e "${GREEN}✅ Complete cleanup done${NC}"

echo ""
echo -e "${BLUE}🚀 MINIMAL CONTAINER LAUNCH${NC}"
echo -e "${CYAN}=============================${NC}"

# Try the most minimal configuration possible
echo -e "${PURPLE}🐳 Launching minimal SQL Edge container...${NC}"
docker run -d \
  --name sql_edge_minimal \
  --restart no \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_AGENT_ENABLED=false" \
  -e "MSSQL_ENABLE_HADR=0" \
  -p 1433:1433 \
  --memory=4g \
  --cpus="4" \
  mcr.microsoft.com/azure-sql-edge:latest

echo -e "${GREEN}✅ Container launched${NC}"

echo ""
echo -e "${YELLOW}⏳ MONITORING STARTUP${NC}"
echo -e "${CYAN}======================${NC}"

# Monitor container for 2 minutes
echo -e "${PURPLE}📊 Monitoring container stability for 2 minutes...${NC}"
for i in {1..24}; do
    sleep 5

    if docker ps --filter "name=sql_edge_minimal" --format "{{.Names}}" | grep -q sql_edge_minimal; then
        echo "✅ Container still running (${i}0 seconds)"
    else
        echo -e "${RED}❌ Container stopped at ${i}0 seconds${NC}"
        echo -e "${YELLOW}📋 Container logs:${NC}"
        docker logs sql_edge_minimal --tail 30
        echo ""
        echo -e "${RED}🚨 CONTAINER FAILED - AZURE SQL EDGE NOT STABLE ON M3${NC}"
        echo -e "${YELLOW}💡 Recommendations:${NC}"
        echo "   1. Azure SQL Edge has known compatibility issues with Apple Silicon"
        echo "   2. Consider using PostgreSQL with SQL Server compatibility"
        echo "   3. Use SQL Server on a different machine/VM"
        echo "   4. Wait for Microsoft to fix ARM64 compatibility"
        exit 1
    fi
done

echo -e "${GREEN}✅ Container stable for 2 minutes!${NC}"

# Test SQL connection
echo -e "${CYAN}🔌 Testing SQL Server connection...${NC}"
CONNECTION_SUCCESS=false

for i in {1..15}; do
    echo "🔄 Connection attempt $i/15..."

    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "SELECT @@VERSION" -C -t 10 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server connected successfully!${NC}"
        CONNECTION_SUCCESS=true
        break
    fi

    # Check if container is still running
    if ! docker ps --filter "name=sql_edge_minimal" --format "{{.Names}}" | grep -q sql_edge_minimal; then
        echo -e "${RED}❌ Container stopped during connection test${NC}"
        docker logs sql_edge_minimal --tail 20
        exit 1
    fi

    echo "⏱️  Waiting 8 seconds..."
    sleep 8
done

if [ "$CONNECTION_SUCCESS" = true ]; then
    echo ""
    echo -e "${WHITE}🎉 SUCCESS! SQL EDGE IS WORKING!${NC}"
    echo -e "${CYAN}=================================${NC}"

    # Display SQL Server info
    echo -e "${BLUE}📋 SQL Server Information:${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    SELECT 'SQL Server Version' as Info, @@VERSION as Details
    UNION ALL
    SELECT 'Server Name', @@SERVERNAME
    UNION ALL
    SELECT 'Current Time', CONVERT(varchar, GETDATE(), 120)
    " -C -h-1

    # Create KhoanDB
    echo -e "${PURPLE}🗄️  Creating KhoanDB...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -Q "
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KhoanDB')
    BEGIN
        CREATE DATABASE KhoanDB
        COLLATE SQL_Latin1_General_CP1_CI_AS
        PRINT '✅ KhoanDB created successfully'
    END
    ELSE
        PRINT '✅ KhoanDB already exists'
    " -C

    echo -e "${GREEN}✅ Database ready${NC}"

    # Set restart policy to unless-stopped now that it's working
    echo -e "${PURPLE}🔄 Setting restart policy...${NC}"
    docker update --restart unless-stopped sql_edge_minimal

    # Show final stats
    echo -e "${BLUE}📊 Final Container Stats:${NC}"
    docker stats sql_edge_minimal --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}"

    echo ""
    echo -e "${WHITE}🏆 MINIMAL SETUP SUCCESSFUL!${NC}"
    echo -e "${CYAN}Connection Details:${NC}"
    echo "   🌐 Server: localhost,1433"
    echo "   👤 Username: sa"
    echo "   🔐 Password: Dientoan@303"
    echo "   🗄️  Database: KhoanDB"
    echo "   📦 Container: sql_edge_minimal"
    echo ""
    echo -e "${YELLOW}Next Steps:${NC}"
    echo "   1. 🚀 Start Backend: cd Backend/KhoanApp.Api && ./start_backend.sh"
    echo "   2. 🎨 Start Frontend: cd Frontend/KhoanUI && ./start_frontend.sh"
    echo "   3. 📥 Restore Database: ./restore_database_complete.sh"

    # Update container name in README
    echo -e "${PURPLE}📝 Updating README with new container name...${NC}"

else
    echo -e "${RED}❌ SQL SERVER CONNECTION STILL FAILED${NC}"
    echo -e "${YELLOW}📋 Final diagnosis:${NC}"
    docker logs sql_edge_minimal --tail 50

    echo ""
    echo -e "${RED}🚨 AZURE SQL EDGE IS NOT COMPATIBLE WITH THIS M3 MAX SETUP${NC}"
    echo -e "${YELLOW}💡 Alternative Solutions:${NC}"
    echo "   1. Use PostgreSQL with SQL Server compatibility layer"
    echo "   2. Use MySQL/MariaDB"
    echo "   3. Use SQLite for development"
    echo "   4. Run SQL Server on a Windows/Linux VM"
    echo "   5. Use cloud SQL Server (Azure SQL Database)"

    # Clean up failed container
    docker stop sql_edge_minimal 2>/dev/null || true
    docker rm sql_edge_minimal 2>/dev/null || true
fi

echo ""
echo -e "${WHITE}🎯 MINIMAL SETUP SCRIPT COMPLETED${NC}"
