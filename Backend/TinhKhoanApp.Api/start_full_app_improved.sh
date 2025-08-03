#!/bin/bash
# Improved TinhKhoanApp Startup - NO HANGING!

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m'

echo -e "${PURPLE}🚀 TinhKhoanApp - NO HANG Startup${NC}"

# Quick service checks
check_database() {
    if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
        echo -e "${GREEN}✅ Database running${NC}"
        return 0
    fi
    return 1
}

check_backend() {
    if curl -s --max-time 3 http://localhost:5055/health > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Backend running${NC}"
        return 0
    fi
    return 1
}

check_frontend() {
    if curl -s --max-time 3 http://localhost:3000 > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Frontend running${NC}"
        return 0
    fi
    return 1
}

start_database_safe() {
    if check_database; then return 0; fi
    echo -e "${YELLOW}🐳 Starting database...${NC}"
    ./start_database.sh > /dev/null 2>&1 &
    sleep 10
    check_database
}

start_backend_safe() {
    if check_backend; then return 0; fi
    echo -e "${YELLOW}⚙️ Starting backend...${NC}"
    pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null
    sleep 2
    ./start_backend_improved.sh > /dev/null 2>&1 &
    sleep 15
    check_backend
}

start_frontend_safe() {
    if check_frontend; then return 0; fi
    echo -e "${YELLOW}🎨 Starting frontend...${NC}"
    cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
    pkill -f "npm.*dev\|vite" 2>/dev/null
    sleep 2
    nohup ./start_frontend.sh > /dev/null 2>&1 &
    sleep 10
    check_frontend
}

# Main execution
echo -e "${YELLOW}Starting services...${NC}"

if start_database_safe; then
    echo -e "${GREEN}✅ Database OK${NC}"
else
    echo -e "${RED}❌ Database failed${NC}"
    exit 1
fi

if start_backend_safe; then
    echo -e "${GREEN}✅ Backend OK${NC}"
else
    echo -e "${YELLOW}⚠️ Backend issue - continuing${NC}"
fi

if start_frontend_safe; then
    echo -e "${GREEN}✅ Frontend OK${NC}"
else
    echo -e "${YELLOW}⚠️ Frontend issue${NC}"
fi

echo -e "${PURPLE}Status Summary:${NC}"
check_database && echo -e "${GREEN}🐳 Database: Running${NC}" || echo -e "${RED}🐳 Database: Failed${NC}"
check_backend && echo -e "${GREEN}⚙️ Backend: Running (http://localhost:5055)${NC}" || echo -e "${RED}⚙️ Backend: Failed${NC}"
check_frontend && echo -e "${GREEN}🎨 Frontend: Running (http://localhost:3000)${NC}" || echo -e "${RED}🎨 Frontend: Failed${NC}"

echo -e "${GREEN}🎉 Startup completed!${NC}"
