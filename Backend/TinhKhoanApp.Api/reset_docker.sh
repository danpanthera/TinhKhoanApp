#!/bin/bash

# 🧹 Script reset hoàn toàn Docker Environment
# Tác giả: TinhKhoanApp Team
# Ngày tạo: July 14, 2025

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo -e "${RED}🧹 TinhKhoanApp - Docker Reset Script${NC}"
echo -e "${RED}====================================${NC}"
echo -e "${YELLOW}⚠️  WARNING: Script này sẽ xóa hoàn toàn container và data!${NC}"
echo -e "${YELLOW}📋 Bao gồm: Container, Volumes, Networks, Images (nếu chọn)${NC}"

# Confirm với user
read -p "Bạn có chắc chắn muốn reset không? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${GREEN}✅ Đã hủy reset. Container giữ nguyên.${NC}"
    exit 0
fi

echo -e "${YELLOW}🔧 Bắt đầu reset Docker environment...${NC}"

# Stop và remove container
echo -e "${YELLOW}🛑 Dừng và xóa container...${NC}"
docker stop ${CONTAINER_NAME} 2>/dev/null
docker rm -f ${CONTAINER_NAME} 2>/dev/null
echo -e "${GREEN}✅ Container đã được xóa${NC}"

# Remove orphaned containers
echo -e "${YELLOW}🧹 Xóa containers orphaned...${NC}"
docker container prune -f > /dev/null 2>&1
echo -e "${GREEN}✅ Orphaned containers đã được xóa${NC}"

# Clean up volumes (careful - this removes ALL unused volumes)
echo -e "${YELLOW}💾 Dọn dẹp volumes không sử dụng...${NC}"
VOLUME_COUNT=$(docker volume ls -q | wc -l | tr -d ' ')
if [ "$VOLUME_COUNT" -gt 0 ]; then
    docker volume prune -f > /dev/null 2>&1
    echo -e "${GREEN}✅ Đã xóa ${VOLUME_COUNT} volumes không sử dụng${NC}"
else
    echo -e "${GREEN}✅ Không có volume nào cần xóa${NC}"
fi

# Clean up networks
echo -e "${YELLOW}🌐 Dọn dẹp networks không sử dụng...${NC}"
docker network prune -f > /dev/null 2>&1
echo -e "${GREEN}✅ Networks đã được dọn dẹp${NC}"

# System cleanup
echo -e "${YELLOW}🗑️  System cleanup...${NC}"
docker system prune -f > /dev/null 2>&1
echo -e "${GREEN}✅ System cleanup hoàn thành${NC}"

# Option to remove images
echo ""
read -p "Bạn có muốn xóa Azure SQL Edge images không? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${YELLOW}🗑️  Xóa Azure SQL Edge images...${NC}"
    docker rmi $(docker images "mcr.microsoft.com/azure-sql-edge*" -q) 2>/dev/null
    echo -e "${GREEN}✅ Images đã được xóa${NC}"
else
    echo -e "${GREEN}✅ Giữ nguyên images${NC}"
fi

# Display final status
echo -e "${BLUE}====================================${NC}"
echo -e "${GREEN}🎯 Reset hoàn thành!${NC}"
echo -e "${BLUE}====================================${NC}"

# Show remaining docker resources
echo -e "${YELLOW}📊 Docker resources còn lại:${NC}"
echo -e "${BLUE}Images:${NC}"
docker images | head -5
echo -e "${BLUE}Containers:${NC}"
docker ps -a | head -5
echo -e "${BLUE}Volumes:${NC}"
docker volume ls | head -5

echo -e "${BLUE}====================================${NC}"
echo -e "${GREEN}💡 Để tạo lại container, chạy: ./start_database.sh${NC}"
echo -e "${GREEN}💡 Để khởi động toàn bộ app: ./start_full_app.sh${NC}"
