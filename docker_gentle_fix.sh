#!/bin/bash

# 🔧 DOCKER GENTLE FIX SCRIPT
# Cố gắng khắc phục Docker mà không mất dữ liệu
# Ngày: 18/07/2025

echo "🔧 === DOCKER GENTLE FIX ==="
echo "📅 Ngày: $(date)"
echo ""

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo -e "${BLUE}📊 1. Current status check...${NC}"
docker ps -a --filter name=$CONTAINER_NAME
echo ""

echo -e "${BLUE}🔄 2. Gentle restart Docker Desktop...${NC}"
echo "Restarting Docker Desktop..."
osascript -e 'quit app "Docker"' 2>/dev/null || echo "Docker app not found in Applications"
sleep 15
open -a Docker
echo "Waiting for Docker to fully start..."
sleep 90
echo ""

echo -e "${BLUE}🔍 3. Check if container survived restart...${NC}"
if docker ps -a --filter name=$CONTAINER_NAME | grep -q $CONTAINER_NAME; then
    echo -e "${GREEN}✅ Container survived restart${NC}"
    
    echo "Trying to start existing container..."
    if docker start $CONTAINER_NAME 2>/dev/null; then
        echo -e "${GREEN}✅ Container started successfully${NC}"
    else
        echo -e "${YELLOW}⚠️  Container start failed, trying gentle approach...${NC}"
        
        # Try to commit current container to preserve data
        echo "Attempting to commit container state..."
        docker commit $CONTAINER_NAME azure_sql_edge_backup:$(date +%Y%m%d) 2>/dev/null && echo "✅ Container state committed" || echo "⚠️  Commit failed"
        
        # Remove problematic container
        docker rm -f $CONTAINER_NAME 2>/dev/null
        
        # Try to run from committed image
        echo "Trying to run from backup image..."
        docker run -d \
            --name $CONTAINER_NAME \
            --restart=unless-stopped \
            --memory=2g \
            -e "ACCEPT_EULA=Y" \
            -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
            -p 1433:1433 \
            azure_sql_edge_backup:$(date +%Y%m%d) 2>/dev/null || {
            
            echo -e "${YELLOW}⚠️  Backup approach failed, trying fresh image...${NC}"
            
            # Pull fresh image
            echo "Pulling fresh Azure SQL Edge image..."
            docker pull mcr.microsoft.com/azure-sql-edge:latest
            
            # Create new container
            docker run -d \
                --name $CONTAINER_NAME \
                --restart=unless-stopped \
                --memory=2g \
                --memory-swap=4g \
                --shm-size=1g \
                -e "ACCEPT_EULA=Y" \
                -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
                -e "MSSQL_TCP_PORT=1433" \
                -p 1433:1433 \
                mcr.microsoft.com/azure-sql-edge:latest
        }
    fi
else
    echo -e "${RED}❌ Container lost during restart${NC}"
    echo "Creating new container..."
    
    # Pull fresh image và tạo mới
    docker pull mcr.microsoft.com/azure-sql-edge:latest
    docker run -d \
        --name $CONTAINER_NAME \
        --restart=unless-stopped \
        --memory=2g \
        --memory-swap=4g \
        --shm-size=1g \
        -e "ACCEPT_EULA=Y" \
        -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
        -e "MSSQL_TCP_PORT=1433" \
        -p 1433:1433 \
        mcr.microsoft.com/azure-sql-edge:latest
fi
echo ""

echo -e "${BLUE}⏳ 4. Waiting for SQL Server...${NC}"
echo "Waiting for SQL Server to be ready..."
for i in {1..30}; do
    if docker exec $CONTAINER_NAME /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "SELECT 1" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ SQL Server ready after $i attempts!${NC}"
        break
    fi
    echo "Health check $i/30..."
    sleep 10
done
echo ""

echo -e "${BLUE}🔍 5. Test connection and database...${NC}"
echo "Testing SQL connection from host..."
if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" -W >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Host connection successful${NC}"
    
    echo "Checking for TinhKhoanDB..."
    if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB'" -W | grep -q TinhKhoanDB; then
        echo -e "${GREEN}✅ TinhKhoanDB exists!${NC}"
        
        echo "Checking tables..."
        TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES" -W -h -1 2>/dev/null | tr -d ' ')
        if [[ "$TABLE_COUNT" =~ ^[0-9]+$ ]] && [ "$TABLE_COUNT" -gt 10 ]; then
            echo -e "${GREEN}✅ TinhKhoanDB has $TABLE_COUNT tables - DATA PRESERVED!${NC}"
        else
            echo -e "${YELLOW}⚠️  TinhKhoanDB exists but has few/no tables ($TABLE_COUNT)${NC}"
        fi
    else
        echo -e "${YELLOW}⚠️  TinhKhoanDB not found, creating...${NC}"
        docker exec $CONTAINER_NAME /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "CREATE DATABASE TinhKhoanDB"
    fi
else
    echo -e "${RED}❌ SQL connection failed${NC}"
fi
echo ""

echo -e "${BLUE}📊 6. Final status...${NC}"
echo "Container status:"
docker ps --filter name=$CONTAINER_NAME --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""

echo "Health status:"
docker inspect $CONTAINER_NAME --format='{{.State.Health.Status}}' 2>/dev/null || echo "No health check defined"
echo ""

echo -e "${GREEN}🎉 === DOCKER GENTLE FIX COMPLETED ===${NC}"
echo "Next steps:"
echo "  1. Check if data preserved: ./verify_data.sh"
echo "  2. If data lost, run EF migrations: cd Backend/TinhKhoanApp.Api && dotnet ef database update"
echo "  3. Start backend & frontend: ./start_backend.sh && ./start_frontend.sh"
