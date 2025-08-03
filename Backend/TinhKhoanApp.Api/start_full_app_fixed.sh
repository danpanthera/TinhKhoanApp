#!/bin/bash

# 🚀 IMPROVED TinhKhoanApp Startup Script - NO MORE HANGING!
# Fixed by GitHub Copilot - Aug 3, 2025

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m'

echo -e "${PURPLE}🚀 TinhKhoanApp - IMPROVED Startup Script${NC}"
echo -e "${PURPLE}===========================================${NC}"

# Paths
BACKEND_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
FRONTEND_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite"

# Enhanced service checking functions
check_database() {
    if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
        echo -e "${GREEN}✅ Database already running${NC}"
        return 0
    else
        echo -e "${YELLOW}🐳 Database not running - starting...${NC}"
        return 1
    fi
}

check_backend() {
    if curl -s --max-time 5 http://localhost:5055/health > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Backend already running${NC}"
        return 0
    else
        echo -e "${YELLOW}⚙️  Backend not running - starting...${NC}"
        return 1
    fi
}

check_frontend() {
    if curl -s --max-time 5 http://localhost:3000 > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Frontend already running${NC}"
        return 0
    else
        echo -e "${YELLOW}🎨 Frontend not running - starting...${NC}"
        return 1
    fi
}

# Improved database startup with timeout
start_database_safe() {
    if check_database; then
        return 0
    fi

    echo -e "${YELLOW}🐳 Starting Database Container...${NC}"
    cd "${BACKEND_PATH}"

    # Start database with timeout
    timeout 30s ./start_database.sh > /dev/null 2>&1 &
    DB_PID=$!

    # Wait for database with timeout
    for i in {1..15}; do
        if check_database; then
            echo -e "${GREEN}✅ Database started successfully!${NC}"
            return 0
        fi
        echo -e "${YELLOW}⏳ Waiting for database... (${i}/15)${NC}"
        sleep 2
    done

    echo -e "${RED}❌ Database failed to start in 30 seconds${NC}"
    return 1
}

# Improved backend startup with proper process management
start_backend_safe() {
    if check_backend; then
        return 0
    fi

    echo -e "${YELLOW}⚙️  Starting Backend API...${NC}"
    cd "${BACKEND_PATH}"

    # Clean up old processes
    pkill -f "dotnet.*TinhKhoanApp.Api" 2>/dev/null
    sleep 2

    # Start backend with proper nohup and output redirection
    nohup ./start_backend.sh </dev/null >/dev/null 2>&1 &
    disown

    # Wait for backend with improved checking
    for i in {1..20}; do
        if check_backend; then
            echo -e "${GREEN}✅ Backend started successfully!${NC}"
            return 0
        fi
        echo -e "${YELLOW}⏳ Waiting for backend... (${i}/20)${NC}"
        sleep 3
    done

    echo -e "${RED}❌ Backend failed to start in 60 seconds${NC}"
    echo -e "${YELLOW}📜 Checking backend log:${NC}"
    tail -5 backend.log 2>/dev/null || echo "No log file found"
    return 1
}

# Improved frontend startup
start_frontend_safe() {
    if check_frontend; then
        return 0
    fi

    echo -e "${YELLOW}🎨 Starting Frontend...${NC}"
    cd "${FRONTEND_PATH}"

    # Clean up old processes
    pkill -f "npm.*dev\|vite" 2>/dev/null
    sleep 2

    # Start frontend with proper process management
    nohup ./start_frontend.sh </dev/null >/dev/null 2>&1 &
    disown

    # Wait for frontend
    for i in {1..15}; do
        if check_frontend; then
            echo -e "${GREEN}✅ Frontend started successfully!${NC}"
            return 0
        fi
        echo -e "${YELLOW}⏳ Waiting for frontend... (${i}/15)${NC}"
        sleep 3
    done

    echo -e "${RED}❌ Frontend failed to start in 45 seconds${NC}"
    echo -e "${YELLOW}📜 Checking frontend log:${NC}"
    tail -5 frontend_startup.log 2>/dev/null || echo "No log file found"
    return 1
}

# Enhanced status summary
show_final_status() {
    echo -e "${PURPLE}===========================================${NC}"
    echo -e "${GREEN}🎯 TinhKhoanApp Status Summary${NC}"
    echo -e "${PURPLE}===========================================${NC}"

    # Check all services
    DB_STATUS="❌ Not Running"
    BACKEND_STATUS="❌ Not Running"
    FRONTEND_STATUS="❌ Not Running"

    if check_database; then
        DB_STATUS="✅ Running"
    fi

    if check_backend; then
        BACKEND_STATUS="✅ Running"
    fi

    if check_frontend; then
        FRONTEND_STATUS="✅ Running"
    fi

    echo -e "${BLUE}🐳 Database: ${DB_STATUS}${NC}"
    echo -e "${BLUE}⚙️  Backend API: ${BACKEND_STATUS} (http://localhost:5055)${NC}"
    echo -e "${BLUE}🎨 Frontend: ${FRONTEND_STATUS} (http://localhost:3000)${NC}"

    echo -e "${PURPLE}===========================================${NC}"
    echo -e "${BLUE}📋 Quick Access:${NC}"
    echo -e "${BLUE}   • App: http://localhost:3000${NC}"
    echo -e "${BLUE}   • API: http://localhost:5055${NC}"
    echo -e "${BLUE}   • Health: http://localhost:5055/health${NC}"
    echo -e "${PURPLE}===========================================${NC}"
}

# Main execution with error handling
main() {
    echo -e "${YELLOW}🎬 Starting TinhKhoanApp services...${NC}"

    # Step 1: Database
    if ! start_database_safe; then
        echo -e "${RED}❌ Critical: Database failed - stopping${NC}"
        show_final_status
        exit 1
    fi

    sleep 2

    # Step 2: Backend
    if ! start_backend_safe; then
        echo -e "${YELLOW}⚠️  Backend failed but continuing...${NC}"
    fi

    sleep 2

    # Step 3: Frontend
    if ! start_frontend_safe; then
        echo -e "${YELLOW}⚠️  Frontend failed but continuing...${NC}"
    fi

    # Final status
    show_final_status

    # Success message
    if check_database && check_backend && check_frontend; then
        echo -e "${GREEN}🎉 All services started successfully!${NC}"
        echo -e "${GREEN}💡 TinhKhoanApp is ready to use!${NC}"
    else
        echo -e "${YELLOW}⚠️  Some services may need manual attention${NC}"
        echo -e "${YELLOW}💡 Check logs and restart individual services if needed${NC}"
    fi
}

# Execute main function
main "$@"
