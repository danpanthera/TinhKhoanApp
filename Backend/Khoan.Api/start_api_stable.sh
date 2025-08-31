#!/bin/bash

# 🚀 LN03 API Server - Stable Start Script
# Starts API server with proper configuration for LN03 testing

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}🚀 Starting LN03 API Server for Testing...${NC}"

# Kill any existing dotnet processes
pkill -f "dotnet.*TinhKhoanApp.Api" 2>/dev/null || true
sleep 2

# Start API server in background with proper configuration
cd /opt/Projects/Khoan/Backend/Khoan.Api
nohup dotnet run --project Khoan.Api.csproj --urls=http://localhost:5055 > /opt/Projects/Khoan/Backend/TinhKhoanApp.Api/api_server.log 2>&1 &
API_PID=$!

echo -e "${GREEN}✅ API Server starting with PID: $API_PID${NC}"
echo "API_PID=$API_PID" > api_server.pid

# Wait for API to be ready
echo -e "${YELLOW}⏳ Waiting for API to initialize...${NC}"
sleep 10

# Check if API is responding
if curl -s http://localhost:5055/health >/dev/null 2>&1; then
    echo -e "${GREEN}✅ API Server is ready at: http://localhost:5055${NC}"
    echo -e "${BLUE}📋 Swagger UI: http://localhost:5055/swagger${NC}"
    echo -e "${YELLOW}📊 LN03 Count Test: curl http://localhost:5055/api/LN03/count${NC}"
    echo -e "${YELLOW}📄 Server Logs: tail -f api_server.log${NC}"
else
    echo -e "${RED}❌ API Server failed to start or is not responding${NC}"
    exit 1
fi
