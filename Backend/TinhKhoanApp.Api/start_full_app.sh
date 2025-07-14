#!/bin/bash

# 🚀 Script khởi động hoàn chỉnh TinhKhoanApp
# Tác giả: TinhKhoanApp Team
# Ngày tạo: July 14, 2025

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${PURPLE}🚀 TinhKhoanApp - Complete Startup Script${NC}"
echo -e "${PURPLE}=========================================${NC}"

# Đường dẫn projects
BACKEND_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
FRONTEND_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite"

# Hàm kiểm tra và khởi động database
start_database() {
    echo -e "${YELLOW}🐳 Bước 1: Khởi động Database Container...${NC}"

    if [ -f "${BACKEND_PATH}/start_database.sh" ]; then
        cd "${BACKEND_PATH}"
        ./start_database.sh

        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✅ Database đã sẵn sàng!${NC}"
            return 0
        else
            echo -e "${RED}❌ Không thể khởi động database${NC}"
            return 1
        fi
    else
        echo -e "${RED}❌ Không tìm thấy script start_database.sh${NC}"
        return 1
    fi
}

# Hàm kiểm tra và khởi động backend
start_backend() {
    echo -e "${YELLOW}⚙️  Bước 2: Khởi động Backend API...${NC}"

    if [ -f "${BACKEND_PATH}/start_backend.sh" ]; then
        cd "${BACKEND_PATH}"

        # Kill backend processes cũ nếu có
        pkill -f "dotnet.*TinhKhoanApp.Api" 2>/dev/null

        # Khởi động backend trong background
        nohup ./start_backend.sh > backend_startup.log 2>&1 &
        BACKEND_PID=$!

        echo -e "${YELLOW}⏳ Đang đợi Backend khởi động...${NC}"

        # Đợi backend khởi động (tối đa 60 giây)
        for i in {1..30}; do
            if curl -s http://localhost:5055/health > /dev/null 2>&1; then
                echo -e "${GREEN}✅ Backend API đã sẵn sàng tại http://localhost:5055${NC}"
                return 0
            fi
            echo -e "${YELLOW}⏳ Đợi Backend startup... (${i}/30)${NC}"
            sleep 2
        done

        echo -e "${RED}❌ Backend không khởi động được sau 60 giây${NC}"
        echo -e "${YELLOW}📜 Log Backend:${NC}"
        tail -10 backend_startup.log 2>/dev/null
        return 1
    else
        echo -e "${RED}❌ Không tìm thấy script start_backend.sh${NC}"
        return 1
    fi
}

# Hàm kiểm tra và khởi động frontend
start_frontend() {
    echo -e "${YELLOW}🎨 Bước 3: Khởi động Frontend...${NC}"

    if [ -f "${FRONTEND_PATH}/start_frontend.sh" ]; then
        cd "${FRONTEND_PATH}"

        # Kill frontend processes cũ nếu có
        pkill -f "npm.*dev\|vite" 2>/dev/null

        # Khởi động frontend trong background
        nohup ./start_frontend.sh > frontend_startup.log 2>&1 &
        FRONTEND_PID=$!

        echo -e "${YELLOW}⏳ Đang đợi Frontend khởi động...${NC}"

        # Đợi frontend khởi động (tối đa 45 giây)
        for i in {1..25}; do
            if curl -s http://localhost:3000 > /dev/null 2>&1; then
                echo -e "${GREEN}✅ Frontend đã sẵn sàng tại http://localhost:3000${NC}"
                return 0
            fi
            echo -e "${YELLOW}⏳ Đợi Frontend startup... (${i}/25)${NC}"
            sleep 2
        done

        echo -e "${RED}❌ Frontend không khởi động được sau 50 giây${NC}"
        echo -e "${YELLOW}📜 Log Frontend:${NC}"
        tail -10 frontend_startup.log 2>/dev/null
        return 1
    else
        echo -e "${RED}❌ Không tìm thấy script start_frontend.sh${NC}"
        return 1
    fi
}

# Hàm hiển thị kết quả
show_summary() {
    echo -e "${PURPLE}=========================================${NC}"
    echo -e "${GREEN}🎯 TinhKhoanApp Startup Summary${NC}"
    echo -e "${PURPLE}=========================================${NC}"

    # Database status
    if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
        echo -e "${GREEN}🐳 Database: ✅ Running${NC}"
    else
        echo -e "${RED}🐳 Database: ❌ Not Running${NC}"
    fi

    # Backend status
    if curl -s http://localhost:5055/health > /dev/null 2>&1; then
        echo -e "${GREEN}⚙️  Backend API: ✅ Running on http://localhost:5055${NC}"
    else
        echo -e "${RED}⚙️  Backend API: ❌ Not Running${NC}"
    fi

    # Frontend status
    if curl -s http://localhost:3000 > /dev/null 2>&1; then
        echo -e "${GREEN}🎨 Frontend: ✅ Running on http://localhost:3000${NC}"
    else
        echo -e "${RED}🎨 Frontend: ❌ Not Running${NC}"
    fi

    echo -e "${PURPLE}=========================================${NC}"
    echo -e "${BLUE}📋 Quick Access URLs:${NC}"
    echo -e "${BLUE}   • Frontend: http://localhost:3000${NC}"
    echo -e "${BLUE}   • Backend API: http://localhost:5055${NC}"
    echo -e "${BLUE}   • Health Check: http://localhost:5055/health${NC}"
    echo -e "${BLUE}   • Database: localhost:1433 (TinhKhoanDB)${NC}"
    echo -e "${PURPLE}=========================================${NC}"
}

# Main execution
echo -e "${YELLOW}🎬 Bắt đầu khởi động TinhKhoanApp...${NC}"

# Khởi động Database
if start_database; then
    echo -e "${GREEN}✅ Database OK${NC}"
else
    echo -e "${RED}❌ Database failed - Dừng quá trình${NC}"
    exit 1
fi

# Đợi một chút trước khi khởi động backend
sleep 3

# Khởi động Backend
if start_backend; then
    echo -e "${GREEN}✅ Backend OK${NC}"
else
    echo -e "${RED}❌ Backend failed - Tiếp tục với Frontend${NC}"
fi

# Đợi một chút trước khi khởi động frontend
sleep 2

# Khởi động Frontend
if start_frontend; then
    echo -e "${GREEN}✅ Frontend OK${NC}"
else
    echo -e "${RED}❌ Frontend failed${NC}"
fi

# Hiển thị tổng kết
show_summary

echo -e "${GREEN}🎉 Startup process hoàn thành!${NC}"
echo -e "${YELLOW}💡 Tip: Sử dụng ./check_database.sh để monitor database${NC}"
