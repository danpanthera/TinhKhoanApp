#!/bin/bash

# 📊 Script monitoring Azure SQL Edge Container
# Tác giả: TinhKhoanApp Team
# Ngày tạo: July 14, 2025

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo -e "${BLUE}📊 TinhKhoanApp - Database Health Monitor${NC}"
echo -e "${BLUE}=====================================${NC}"

# Kiểm tra Docker daemon
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker daemon không chạy!${NC}"
    echo -e "${YELLOW}💡 Hãy khởi động Docker Desktop trước${NC}"
    exit 1
fi

# Kiểm tra container status
echo -e "${YELLOW}🔍 Kiểm tra container status...${NC}"
CONTAINER_STATUS=$(docker ps -a --filter "name=${CONTAINER_NAME}" --format "{{.Status}}")

if [ -z "$CONTAINER_STATUS" ]; then
    echo -e "${RED}❌ Container không tồn tại!${NC}"
    echo -e "${YELLOW}💡 Chạy ./start_database.sh để tạo container${NC}"
    exit 1
fi

echo -e "${BLUE}📋 Container Status: ${CONTAINER_STATUS}${NC}"

# Kiểm tra nếu container đang chạy
if docker ps --filter "name=${CONTAINER_NAME}" --format "{{.Names}}" | grep -w "${CONTAINER_NAME}" > /dev/null; then
    echo -e "${GREEN}✅ Container đang chạy${NC}"    # Kiểm tra port 1433
    echo -e "${YELLOW}🔗 Kiểm tra port 1433...${NC}"
    if nc -zv localhost 1433 > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Port 1433 đã được bind và accessible${NC}"
    else
        echo -e "${RED}❌ Port 1433 không accessible${NC}"
    fi

    # Kiểm tra database connection với sqlcmd (nếu có)
    echo -e "${YELLOW}🗄️  Kiểm tra database connection...${NC}"
    if nc -zv localhost 1433 > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Database TCP connection OK${NC}"

        # Test SQL connection nếu có sqlcmd trên host
        if command -v sqlcmd > /dev/null 2>&1; then
            if sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -C -Q "SELECT @@VERSION" > /dev/null 2>&1; then
                echo -e "${GREEN}✅ SQL Server connection verified!${NC}"

                # Kiểm tra TinhKhoanDB
                echo -e "${YELLOW}🎯 Kiểm tra TinhKhoanDB...${NC}"
                if sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -C -d TinhKhoanDB -Q "SELECT COUNT(*) as TableCount FROM sys.tables" > /dev/null 2>&1; then
                    TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM sys.tables" -h -1 | tr -d ' ' | head -1)
                    echo -e "${GREEN}✅ TinhKhoanDB accessible với ${TABLE_COUNT} tables${NC}"
                else
                    echo -e "${YELLOW}⚠️  TinhKhoanDB chưa tồn tại hoặc chưa accessible${NC}"
                fi
            else
                echo -e "${YELLOW}⚠️  SQL connection test failed${NC}"
            fi
        else
            echo -e "${YELLOW}💡 Sqlcmd not available on host - TCP test only${NC}"
        fi

        echo -e "${GREEN}📊 Ready for Backend API connection!${NC}"
    else
        echo -e "${RED}❌ Database connection failed${NC}"
    fi

    # Hiển thị container logs (10 dòng cuối)
    echo -e "${YELLOW}📜 Container logs (10 dòng cuối):${NC}"
    docker logs ${CONTAINER_NAME} --tail 10 2>/dev/null | head -10

else
    echo -e "${RED}❌ Container đã dừng${NC}"
    echo -e "${YELLOW}💡 Chạy: docker start ${CONTAINER_NAME}${NC}"
    echo -e "${YELLOW}📜 Container logs gần nhất:${NC}"
    docker logs ${CONTAINER_NAME} --tail 5 2>/dev/null
fi

# Hiển thị resource usage
echo -e "${YELLOW}📊 Resource usage:${NC}"
docker stats ${CONTAINER_NAME} --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}" 2>/dev/null

# Hiển thị Docker system info
echo -e "${YELLOW}💾 Docker system usage:${NC}"
docker system df

echo -e "${BLUE}======================================${NC}"
echo -e "${GREEN}🎯 Health check hoàn thành!${NC}"
