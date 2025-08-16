#!/bin/bash

# 🛡️ TinhKhoan Project Guardian - Keeps backend/frontend running continuously
# This script monitors and restarts services if they die

PROJECT_NAME="TinhKhoanApp"
BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
FRONTEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite"
GUARDIAN_LOG="guardian.log"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo -e "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$GUARDIAN_LOG"
}

check_backend() {
    if curl -s --max-time 3 http://localhost:5055/health > /dev/null 2>&1; then
        return 0  # Backend is healthy
    else
        return 1  # Backend is not responding
    fi
}

check_frontend() {
    if curl -s --max-time 3 http://localhost:3000 > /dev/null 2>&1; then
        return 0  # Frontend is healthy
    else
        return 1  # Frontend is not responding
    fi
}

start_backend() {
    log "${YELLOW}🔄 Starting Backend...${NC}"
    cd "$BACKEND_DIR"
    ./start_backend.sh &
    sleep 10  # Give it time to start
}

start_frontend() {
    log "${YELLOW}🔄 Starting Frontend...${NC}"
    cd "$FRONTEND_DIR"
    ./start_frontend.sh &
    sleep 8  # Give it time to start
}

# Initial cleanup
> "$GUARDIAN_LOG"
log "${BLUE}🛡️ TinhKhoan Guardian Started${NC}"
log "${BLUE}📍 Monitoring Backend: http://localhost:5055${NC}"
log "${BLUE}📍 Monitoring Frontend: http://localhost:3000${NC}"

# Start services initially
if ! check_backend; then
    log "${RED}❌ Backend not running, starting...${NC}"
    start_backend
fi

if ! check_frontend; then
    log "${RED}❌ Frontend not running, starting...${NC}"
    start_frontend
fi

# Monitoring loop
while true; do
    # Check backend
    if ! check_backend; then
        log "${RED}⚠️ Backend down! Restarting...${NC}"
        start_backend
    else
        log "${GREEN}✅ Backend healthy${NC}"
    fi

    # Check frontend
    if ! check_frontend; then
        log "${RED}⚠️ Frontend down! Restarting...${NC}"
        start_frontend
    else
        log "${GREEN}✅ Frontend healthy${NC}"
    fi

    # Wait before next check
    sleep 30
done
