#!/bin/bash

# 🐳 Script khởi động nhanh Azure SQL Edge Container
# Tác giả: TinhKhoanApp Team
# Ngày tạo: July 14, 2025

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CONTAINER_NAME="azure_sql_edge_tinhkhoan"
IMAGE_NAME="mcr.microsoft.com/azure-sql-edge:1.0.5"
DB_PASSWORD="Dientoan@303"

echo -e "${BLUE}🐳 TinhKhoanApp - Database Startup Script${NC}"
echo -e "${BLUE}======================================${NC}"

# Hàm kiểm tra container có tồn tại không
check_container_exists() {
    docker ps -a --filter "name=${CONTAINER_NAME}" --format "{{.Names}}" | grep -w "${CONTAINER_NAME}" > /dev/null
    return $?
}

# Hàm kiểm tra container có đang chạy không
check_container_running() {
    docker ps --filter "name=${CONTAINER_NAME}" --format "{{.Names}}" | grep -w "${CONTAINER_NAME}" > /dev/null
    return $?
}

# Hàm khởi động container
start_container() {
    echo -e "${YELLOW}⚡ Đang khởi động container...${NC}"
    docker start ${CONTAINER_NAME}

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Container đã khởi động thành công!${NC}"
        return 0
    else
        echo -e "${RED}❌ Không thể khởi động container${NC}"
        return 1
    fi
}

# Hàm tạo container mới
create_new_container() {
    echo -e "${YELLOW}📦 Tạo container mới...${NC}"
    docker run -e "ACCEPT_EULA=Y" \
               -e "MSSQL_SA_PASSWORD=${DB_PASSWORD}" \
               -p 1433:1433 \
               --name ${CONTAINER_NAME} \
               --restart unless-stopped \
               -d ${IMAGE_NAME}

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Container mới đã được tạo và khởi động!${NC}"
        return 0
    else
        echo -e "${RED}❌ Không thể tạo container mới${NC}"
        return 1
    fi
}

# Hàm kiểm tra kết nối database
test_connection() {
    echo -e "${YELLOW}🔗 Kiểm tra kết nối database...${NC}"

    # Đợi container khởi động hoàn toàn (tối đa 30 giây)
    for i in {1..30}; do
        # Test TCP connection trước
        if nc -zv localhost 1433 > /dev/null 2>&1; then
            echo -e "${GREEN}✅ Port 1433 accessible!${NC}"

            # Test SQL connection nếu có sqlcmd trên host
            if command -v sqlcmd > /dev/null 2>&1; then
                if sqlcmd -S localhost,1433 -U sa -P "${DB_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1; then
                    echo -e "${GREEN}✅ SQL Server connection verified!${NC}"
                else
                    echo -e "${YELLOW}⚠️  SQL connection test failed, but TCP is working${NC}"
                fi
            fi

            echo -e "${GREEN}✅ Database đã sẵn sàng kết nối!${NC}"
            echo -e "${GREEN}📊 Connection String: Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=true${NC}"
            return 0
        fi
        echo -e "${YELLOW}⏳ Đang đợi database khởi động... (${i}/30)${NC}"
        sleep 2
    done

    echo -e "${RED}❌ Không thể kết nối đến database sau 60 giây${NC}"
    return 1
}

# Hàm dọn dẹp tài nguyên
cleanup_resources() {
    echo -e "${YELLOW}🧹 Dọn dẹp tài nguyên Docker...${NC}"
    docker system prune -f > /dev/null 2>&1
    echo -e "${GREEN}✅ Đã dọn dẹp tài nguyên thừa${NC}"
}

# Hàm hiển thị thông tin container
show_container_info() {
    echo -e "${BLUE}📋 Thông tin Container:${NC}"
    docker ps --filter "name=${CONTAINER_NAME}" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
}

# Main logic
echo -e "${YELLOW}🔍 Kiểm tra tình trạng container...${NC}"

if check_container_exists; then
    if check_container_running; then
        echo -e "${GREEN}✅ Container đang chạy!${NC}"
        show_container_info
        test_connection
    else
        echo -e "${YELLOW}⚠️  Container tồn tại nhưng đã dừng${NC}"
        if start_container; then
            show_container_info
            test_connection
        else
            echo -e "${RED}❌ Không thể khởi động container hiện tại${NC}"
            echo -e "${YELLOW}🔄 Thử xóa và tạo container mới...${NC}"
            docker rm -f ${CONTAINER_NAME} > /dev/null 2>&1
            cleanup_resources
            create_new_container
            if [ $? -eq 0 ]; then
                show_container_info
                test_connection
            fi
        fi
    fi
else
    echo -e "${YELLOW}⚠️  Container không tồn tại${NC}"
    cleanup_resources
    create_new_container
    if [ $? -eq 0 ]; then
        show_container_info
        test_connection
    fi
fi

echo -e "${BLUE}======================================${NC}"
echo -e "${GREEN}🎯 Script hoàn thành!${NC}"
echo -e "${BLUE}📝 Lưu ý: Container được cấu hình restart unless-stopped${NC}"
echo -e "${BLUE}🔧 Backend API có thể kết nối ngay tại: http://localhost:5055${NC}"
