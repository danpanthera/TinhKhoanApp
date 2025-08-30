#!/bin/bash

# 🚀 KhoanApp - Complete Fullstack Startup Script
# Theo quy tắc trong README_DAT.md dòng 110-125
# Khởi động: Database -> Backend (5055) -> Frontend (3000)

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${PURPLE}🚀 KhoanApp - Fullstack Startup${NC}"
echo -e "${PURPLE}===================================${NC}"

# Đường dẫn projects
BACKEND_PATH="/opt/Projects/Khoan/Backend/KhoanApp.Api"
FRONTEND_PATH="/opt/Projects/Khoan/Frontend/KhoanUI"

# Hàm cleanup khi script bị interrupt
cleanup() {
    echo -e "${YELLOW}🛑 Cleaning up processes...${NC}"
    pkill -f "dotnet.*TinhKhoan" 2>/dev/null
    pkill -f "npm.*dev" 2>/dev/null
    exit 0
}

# Trap SIGINT (Ctrl+C)
trap cleanup SIGINT

echo -e "${BLUE}📍 Backend Path: ${BACKEND_PATH}${NC}"
echo -e "${BLUE}📍 Frontend Path: ${FRONTEND_PATH}${NC}"

# Bước 1: Khởi động Database
echo -e "${YELLOW}🐳 Bước 1: Khởi động Database...${NC}"
cd "${BACKEND_PATH}"
if [ -f "./start_database.sh" ]; then
    ./start_database.sh
    echo -e "${GREEN}✅ Database started${NC}"
else
    echo -e "${RED}❌ start_database.sh not found${NC}"
fi

# Bước 2: Khởi động Backend
echo -e "${YELLOW}⚙️  Bước 2: Khởi động Backend API (Port 5055)...${NC}"
cd "${BACKEND_PATH}"
if [ -f "./start_backend.sh" ]; then
    ./start_backend.sh &
    BACKEND_PID=$!
    echo -e "${GREEN}✅ Backend started with PID: ${BACKEND_PID}${NC}"

    # Wait for backend to be ready
    echo -e "${YELLOW}⏳ Waiting for backend to be ready...${NC}"
    sleep 10

    # Test backend
    if curl -s http://localhost:5055/health >/dev/null 2>&1; then
        echo -e "${GREEN}✅ Backend health check passed${NC}"
    else
        echo -e "${YELLOW}⚠️  Backend starting (health check pending)${NC}"
    fi
else
    echo -e "${RED}❌ start_backend.sh not found${NC}"
fi

# Bước 3: Khởi động Frontend
echo -e "${YELLOW}🎨 Bước 3: Khởi động Frontend (Port 3000)...${NC}"
cd "${FRONTEND_PATH}"
if [ -f "./start_frontend.sh" ]; then
    ./start_frontend.sh &
    FRONTEND_PID=$!
    echo -e "${GREEN}✅ Frontend started with PID: ${FRONTEND_PID}${NC}"

    # Wait for frontend to be ready
    echo -e "${YELLOW}⏳ Waiting for frontend to be ready...${NC}"
    sleep 15

    # Test frontend
    if curl -s http://localhost:3000 >/dev/null 2>&1; then
        echo -e "${GREEN}✅ Frontend ready at http://localhost:3000${NC}"
    else
        echo -e "${YELLOW}⚠️  Frontend starting (still loading)${NC}"
    fi
else
    echo -e "${RED}❌ start_frontend.sh not found${NC}"
fi

# Summary
echo -e "${PURPLE}🎉 KhoanApp Fullstack Started!${NC}"
echo -e "${PURPLE}=================================${NC}"
echo -e "${GREEN}🗄️  Database: TinhKhoanDB (sa/Dientoan@303)${NC}"
echo -e "${GREEN}⚙️  Backend API: http://localhost:5055${NC}"
echo -e "${GREEN}🎨 Frontend: http://localhost:3000${NC}"
echo -e "${BLUE}📝 Logs:${NC}"
echo -e "${BLUE}  - Backend: ${BACKEND_PATH}/backend.log${NC}"
echo -e "${BLUE}  - Frontend: ${FRONTEND_PATH}/frontend.log${NC}"
echo -e "${YELLOW}💡 Press Ctrl+C to stop all services${NC}"

# Keep script running để maintain processes
echo -e "${YELLOW}🔄 Services running. Press Ctrl+C to stop...${NC}"
echo -e "${BLUE}💡 Monitoring services health every 30 seconds...${NC}"

while true; do
    sleep 30  # 🔧 Tăng từ 10s lên 30s để không check quá thường xuyên

    # Check backend health bằng cách test API endpoint thay vì process name
    if ! curl -s http://localhost:5055/health >/dev/null 2>&1; then
        echo -e "${RED}⚠️  Backend API not responding, checking process...${NC}"

        # Double check với process pattern rộng hơn
        if ! pgrep -f "dotnet.*5055" >/dev/null && ! pgrep -f "KhoanApp.Api" >/dev/null; then
            echo -e "${RED}🔄 Restarting backend...${NC}"
            cd "${BACKEND_PATH}"
            ./start_backend.sh &
            sleep 15  # 🔧 Chờ backend khởi động
        else
            echo -e "${YELLOW}🔍 Backend process exists but API not responding (may be starting up)${NC}"
        fi
    else
        echo -e "${GREEN}✅ Backend health check OK${NC}"
    fi

    # Check frontend health bằng cách test port 3000
    if ! curl -s http://localhost:3000 >/dev/null 2>&1; then
        echo -e "${RED}⚠️  Frontend not responding, checking process...${NC}"

        if ! pgrep -f "npm.*dev" >/dev/null && ! pgrep -f "vite.*3000" >/dev/null; then
            echo -e "${RED}🔄 Restarting frontend...${NC}"
            cd "${FRONTEND_PATH}"
            ./start_frontend.sh &
            sleep 10  # 🔧 Chờ frontend khởi động
        else
            echo -e "${YELLOW}🔍 Frontend process exists but not responding (may be starting up)${NC}"
        fi
    else
        echo -e "${GREEN}✅ Frontend health check OK${NC}"
    fi
done
