#!/bin/bash

# Khoan App - Fullstack Startup Script
# Khởi động theo thứ tự: Database -> Backend -> Frontend

set -e

echo "🚀 Starting Khoan Fullstack Application..."
echo "================================================"

# Màu sắc cho output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to check if service is running
check_service() {
    local url=$1
    local service_name=$2
    
    for i in {1..30}; do
        if curl -s "$url" > /dev/null 2>&1; then
            echo -e "${GREEN}✅ $service_name is ready${NC}"
            return 0
        fi
        echo -e "${YELLOW}⏳ Waiting for $service_name... (attempt $i/30)${NC}"
        sleep 2
    done
    
    echo -e "${RED}❌ $service_name failed to start${NC}"
    return 1
}

# 1. Kiểm tra Database Container
echo "1️⃣ Checking Database Container..."
if ! docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo -e "${RED}❌ Database container not running. Please start it first:${NC}"
    echo "   docker start azure_sql_edge_tinhkhoan"
    exit 1
fi
echo -e "${GREEN}✅ Database container is running${NC}"

# 2. Khởi động Backend API
echo ""
echo "2️⃣ Starting Backend API..."
cd Backend/KhoanApp.Api

# Kill existing backend processes
pkill -f "dotnet run.*5055" 2>/dev/null || true
sleep 2

# Start backend in background
nohup dotnet run --urls=http://0.0.0.0:5055 > backend.log 2>&1 &
BACKEND_PID=$!
echo "Backend PID: $BACKEND_PID"

# Wait for backend to be ready
check_service "http://localhost:5055/api/Health" "Backend API"

# 3. Khởi động Frontend
echo ""
echo "3️⃣ Starting Frontend..."
cd ../../Frontend/KhoanUI

# Kill existing frontend processes
pkill -f "npm run dev" 2>/dev/null || true
sleep 2

# Start frontend in background
nohup npm run dev > frontend.log 2>&1 &
FRONTEND_PID=$!
echo "Frontend PID: $FRONTEND_PID"

# Wait for frontend to be ready
check_service "http://localhost:3000" "Frontend"

# 4. Kiểm tra APIs
echo ""
echo "4️⃣ Verifying APIs..."
KPI_TABLES=$(curl -s http://localhost:5055/api/KpiAssignmentTables | jq length 2>/dev/null || echo "0")
KPI_INDICATORS=$(curl -s http://localhost:5055/api/KpiIndicators | jq length 2>/dev/null || echo "0")

echo -e "${GREEN}📊 KPI Tables: $KPI_TABLES${NC}"
echo -e "${GREEN}📈 KPI Indicators: $KPI_INDICATORS${NC}"

# 5. Final Status
echo ""
echo "🎉 Fullstack Application Started Successfully!"
echo "================================================"
echo -e "${GREEN}🌐 Frontend:${NC} http://localhost:3000"
echo -e "${GREEN}🔗 Backend API:${NC} http://localhost:5055"
echo -e "${GREEN}🗄️ Database:${NC} localhost:1433"
echo ""
echo -e "${YELLOW}📝 Logs:${NC}"
echo "   Backend: Backend/KhoanApp.Api/backend.log"
echo "   Frontend: Frontend/KhoanUI/frontend.log"
echo ""
echo -e "${YELLOW}🛑 To stop:${NC}"
echo "   kill $BACKEND_PID $FRONTEND_PID"
echo "   or use: pkill -f 'dotnet run'; pkill -f 'npm run dev'"
echo ""
echo -e "${GREEN}✅ Ready for testing B2 KPI Definitions at: http://localhost:3000/kpi-definitions${NC}"
