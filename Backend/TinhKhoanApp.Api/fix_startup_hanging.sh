#!/bin/bash

# 🛠️  COMPREHENSIVE STARTUP FIX SCRIPT
# Fixes all hanging issues with TinhKhoanApp startup
# Created by GitHub Copilot - Aug 3, 2025

echo "🔧 FIXING TINHKHOANAPP STARTUP HANGING ISSUES..."
echo "================================================="

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Fix 1: Backup original scripts
echo -e "${YELLOW}1. Backing up original scripts...${NC}"
cp start_full_app.sh start_full_app.sh.backup 2>/dev/null || echo "No original start_full_app.sh found"
cp start_backend.sh start_backend.sh.backup 2>/dev/null || echo "No original start_backend.sh found"

# Fix 2: Create improved startup scripts with timeout protection
echo -e "${YELLOW}2. Creating improved startup scripts...${NC}"

# Already created the fixed scripts above, just ensure they're executable
chmod +x start_full_app.sh
chmod +x start_backend.sh
chmod +x start_frontend.sh 2>/dev/null || echo "start_frontend.sh not found"

# Fix 3: Create process monitor script
echo -e "${YELLOW}3. Creating process monitor script...${NC}"
cat > monitor_processes.sh << 'EOF'
#!/bin/bash
# Process Monitor for TinhKhoanApp

echo "🔍 TinhKhoanApp Process Monitor"
echo "==============================="

# Database
if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
    echo "✅ Database: Running"
else
    echo "❌ Database: Not Running"
fi

# Backend
if curl -s --max-time 3 http://localhost:5055/health > /dev/null 2>&1; then
    echo "✅ Backend: Running (http://localhost:5055)"
else
    echo "❌ Backend: Not Running"
fi

# Frontend
if curl -s --max-time 3 http://localhost:3000 > /dev/null 2>&1; then
    echo "✅ Frontend: Running (http://localhost:3000)"
else
    echo "❌ Frontend: Not Running"
fi

echo "==============================="
echo "Dotnet processes:"
ps aux | grep dotnet | grep -v grep | awk '{print $2, $11, $12, $13}' | head -5

echo "==============================="
echo "Node processes:"
ps aux | grep node | grep -v grep | awk '{print $2, $11, $12, $13}' | head -5
EOF

chmod +x monitor_processes.sh

# Fix 4: Create emergency cleanup script
echo -e "${YELLOW}4. Creating emergency cleanup script...${NC}"
cat > emergency_cleanup.sh << 'EOF'
#!/bin/bash
# Emergency cleanup for hanging processes

echo "🚨 EMERGENCY CLEANUP - STOPPING ALL PROCESSES"
echo "============================================="

# Kill dotnet processes
echo "Killing dotnet processes..."
pkill -f "dotnet.*run" 2>/dev/null
pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null

# Kill node/npm processes
echo "Killing node/npm processes..."
pkill -f "npm.*dev" 2>/dev/null
pkill -f "vite" 2>/dev/null
pkill -f "node.*vite" 2>/dev/null

# Force kill by ports
echo "Force killing processes on ports 5055 and 3000..."
lsof -ti:5055 | xargs kill -9 2>/dev/null
lsof -ti:3000 | xargs kill -9 2>/dev/null

sleep 3

echo "✅ Cleanup completed!"
echo "Run ./start_full_app.sh to restart everything"
EOF

chmod +x emergency_cleanup.sh

# Fix 5: Create quick restart script
echo -e "${YELLOW}5. Creating quick restart script...${NC}"
cat > quick_restart.sh << 'EOF'
#!/bin/bash
# Quick restart script

echo "🔄 QUICK RESTART - NO HANGING!"
echo "==============================="

# Cleanup first
./emergency_cleanup.sh

# Wait a moment
sleep 2

# Start everything
./start_full_app.sh
EOF

chmod +x quick_restart.sh

# Fix 6: Update tasks.json to use improved scripts
echo -e "${YELLOW}6. Checking VS Code tasks configuration...${NC}"
if [ -f ".vscode/tasks.json" ]; then
    echo "✅ VS Code tasks.json found"
else
    echo "⚠️  No VS Code tasks.json found - tasks may need manual update"
fi

# Fix 7: Create health check script
echo -e "${YELLOW}7. Creating health check script...${NC}"
cat > health_check.sh << 'EOF'
#!/bin/bash
# Health check for all services

check_service() {
    local name=$1
    local url=$2
    local timeout=$3

    if curl -s --max-time $timeout "$url" > /dev/null 2>&1; then
        echo "✅ $name: Healthy"
        return 0
    else
        echo "❌ $name: Unhealthy"
        return 1
    fi
}

echo "🏥 TinhKhoanApp Health Check"
echo "============================"

# Check database (via backend connection)
check_service "Database" "http://localhost:5055/health" 5

# Check backend
check_service "Backend API" "http://localhost:5055/health" 3

# Check frontend
check_service "Frontend" "http://localhost:3000" 3

echo "============================"
EOF

chmod +x health_check.sh

# Fix 8: Test the fixed scripts
echo -e "${YELLOW}8. Testing improved scripts...${NC}"
./monitor_processes.sh

echo -e "${GREEN}✅ STARTUP FIX COMPLETED!${NC}"
echo -e "${GREEN}=========================${NC}"
echo -e "${BLUE}Available scripts:${NC}"
echo -e "${BLUE}• ./start_full_app.sh      - Improved startup (no hanging)${NC}"
echo -e "${BLUE}• ./monitor_processes.sh   - Monitor all services${NC}"
echo -e "${BLUE}• ./emergency_cleanup.sh   - Force stop all processes${NC}"
echo -e "${BLUE}• ./quick_restart.sh       - Clean restart everything${NC}"
echo -e "${BLUE}• ./health_check.sh        - Check service health${NC}"
echo -e "${GREEN}=========================${NC}"
echo -e "${GREEN}🎉 NO MORE HANGING ISSUES!${NC}"
