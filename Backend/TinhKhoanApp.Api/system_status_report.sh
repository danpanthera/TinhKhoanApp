#!/bin/bash

# 🎯 TinhKhoanApp Full Stack Status Report
# Script kiểm tra tổng quan toàn bộ hệ thống

PROJECT_ROOT="/Users/nguyendat/Documents/Projects/TinhKhoanApp"
BACKEND_DIR="${PROJECT_ROOT}/Backend/TinhKhoanApp.Api"
FRONTEND_DIR="${PROJECT_ROOT}/Frontend/tinhkhoan-app-ui-vite"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}🎯 TinhKhoanApp Full Stack Status Report${NC}"
echo -e "${CYAN}=======================================${NC}"
echo "📅 Thời gian: $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# 1. DOCKER STATUS
echo -e "${BLUE}🐳 1. DOCKER CONTAINER STATUS:${NC}"
echo "--------------------------------"

if docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | grep -q "azure_sql_edge_tinhkhoan"; then
    echo -e "✅ ${GREEN}Container: azure_sql_edge_tinhkhoan${NC}"
    docker ps --format "   📊 {{.Status}} | {{.Ports}}" --filter "name=azure_sql_edge_tinhkhoan"

    # Docker stats
    echo "   💾 Memory Usage:"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "   📈 {{.MemUsage}} ({{.MemPerc}}) | CPU: {{.CPUPerc}}"
else
    echo -e "❌ ${RED}Container: azure_sql_edge_tinhkhoan NOT RUNNING${NC}"
fi

echo ""

# 2. DATABASE STATUS
echo -e "${BLUE}🗄️  2. DATABASE STATUS:${NC}"
echo "----------------------"

if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" &>/dev/null; then
    echo -e "✅ ${GREEN}SQL Server: Connected${NC}"

    # Database info
    DB_INFO=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "
        SELECT
            (SELECT COUNT(*) FROM sys.tables WHERE name NOT LIKE '%History%') as MainTables,
            (SELECT COUNT(*) FROM sys.tables WHERE name LIKE '%History%') as HistoryTables
    " -h -1 2>/dev/null | tr -d '\r ' | grep -E '^[0-9]+$')

    if [ ! -z "$DB_INFO" ]; then
        MAIN_TABLES=$(echo "$DB_INFO" | head -n 1)
        HISTORY_TABLES=$(echo "$DB_INFO" | tail -n 1)
        echo "   📋 Main Tables: ${MAIN_TABLES}"
        echo "   📚 History Tables: ${HISTORY_TABLES}"
    fi    # Check core data tables
    echo "   🗂️  Core Data Tables:"
    for table in DP01 DPDA EI01 GL01 GL41 LN01 LN03 RR01; do
        # Simple count query
        RESULT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM ${table}" -h -1 2>/dev/null | tr -d '\r\n ' | sed 's/.*affected)//' | grep -E '^[0-9]+$' | head -1)

        if [ -z "$RESULT" ]; then
            # Fallback: just get the number before "(X rows affected)"
            RESULT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM ${table}" -h -1 2>/dev/null | grep -oE '[0-9]+' | head -1)
        fi

        if [ -z "$RESULT" ]; then
            echo -e "      ❌ ${table}: Table not accessible"
        elif [ "$RESULT" -gt 0 ] 2>/dev/null; then
            echo -e "      ✅ ${table}: ${RESULT} records"
        else
            echo -e "      ⚠️  ${table}: Empty (0 records)"
        fi
    done
else
    echo -e "❌ ${RED}SQL Server: Not accessible${NC}"
fi

echo ""

# 3. BACKEND API STATUS
echo -e "${BLUE}🔧 3. BACKEND API STATUS:${NC}"
echo "-------------------------"

if curl -s http://localhost:5055/health &>/dev/null; then
    echo -e "✅ ${GREEN}Backend API: Running (http://localhost:5055)${NC}"

    # Get health details
    HEALTH_STATUS=$(curl -s http://localhost:5055/health | jq -r '.status' 2>/dev/null)
    if [ "$HEALTH_STATUS" = "Healthy" ]; then
        echo "   💚 Health Status: ${HEALTH_STATUS}"

        # Check specific endpoints
        for endpoint in "api/units" "api/roles" "api/employees"; do
            if curl -s "http://localhost:5055/${endpoint}" &>/dev/null; then
                echo -e "   ✅ /${endpoint}: Accessible"
            else
                echo -e "   ❌ /${endpoint}: Not accessible"
            fi
        done
    else
        echo "   ⚠️  Health Status: ${HEALTH_STATUS}"
    fi
else
    echo -e "❌ ${RED}Backend API: Not running${NC}"
fi

echo ""

# 4. FRONTEND STATUS
echo -e "${BLUE}🎨 4. FRONTEND STATUS:${NC}"
echo "---------------------"

if curl -s http://localhost:3000 &>/dev/null; then
    echo -e "✅ ${GREEN}Frontend: Running (http://localhost:3000)${NC}"

    # Check if it's the right app
    if curl -s http://localhost:3000 | grep -q "TinhKhoanApp"; then
        echo "   🎯 App: TinhKhoanApp detected"
    else
        echo "   ⚠️  App: Unknown application"
    fi

    # Check Vite dev server
    if curl -s http://localhost:3000/@vite/client &>/dev/null; then
        echo "   ⚡ Vite: Dev server active"
    else
        echo "   📦 Vite: Production mode"
    fi
else
    echo -e "❌ ${RED}Frontend: Not running${NC}"
fi

echo ""

# 5. PROCESS STATUS
echo -e "${BLUE}⚙️  5. PROCESS STATUS:${NC}"
echo "--------------------"

# Backend processes
BACKEND_PROCESSES=$(pgrep -f "dotnet.*TinhKhoanApp.Api" | wc -l | tr -d ' ')
if [ "$BACKEND_PROCESSES" -gt 0 ]; then
    echo -e "✅ ${GREEN}Backend Processes: ${BACKEND_PROCESSES} running${NC}"
    pgrep -f "dotnet.*TinhKhoanApp.Api" | head -3 | while read pid; do
        echo "   🔧 PID: $pid"
    done
else
    echo -e "❌ ${RED}Backend Processes: None running${NC}"
fi

# Frontend processes
FRONTEND_PROCESSES=$(pgrep -f "vite.*dev\|node.*vite" | wc -l | tr -d ' ')
if [ "$FRONTEND_PROCESSES" -gt 0 ]; then
    echo -e "✅ ${GREEN}Frontend Processes: ${FRONTEND_PROCESSES} running${NC}"
    pgrep -f "vite.*dev\|node.*vite" | head -3 | while read pid; do
        echo "   🎨 PID: $pid"
    done
else
    echo -e "❌ ${RED}Frontend Processes: None running${NC}"
fi

echo ""

# 6. RESOURCE USAGE
echo -e "${BLUE}📊 6. RESOURCE USAGE:${NC}"
echo "--------------------"

# Docker resources
echo "🐳 Docker:"
docker system df --format "   {{.Type}}: {{.Size}} ({{.Reclaimable}} reclaimable)"

# Disk space
echo "💽 Disk Space:"
df -h / | tail -1 | awk '{print "   Root: " $3 " used / " $2 " total (" $5 " used)"}'

echo ""

# 7. QUICK ACTIONS
echo -e "${BLUE}🎯 7. QUICK ACTIONS:${NC}"
echo "------------------"
echo "🔄 Restart Docker:     cd ${BACKEND_DIR} && ./docker_stability_monitor.sh"
echo "🚀 Start Full Stack:   cd ${BACKEND_DIR} && ./fullstack_auto_start.sh"
echo "🐛 Troubleshoot:       cd ${BACKEND_DIR} && ./docker_troubleshoot_fix.sh"
echo "📊 Check Status:       cd ${BACKEND_DIR} && ./system_status_report.sh"
echo ""
echo "🌐 URLs:"
echo "   Backend API:  http://localhost:5055"
echo "   Frontend:     http://localhost:3000"
echo "   API Health:   http://localhost:5055/health"
echo ""

# 8. SUMMARY
echo -e "${PURPLE}📋 SUMMARY:${NC}"
echo "----------"

ALL_SERVICES_OK=true

# Check each service
if ! docker ps --format "table {{.Names}}" | grep -q "azure_sql_edge_tinhkhoan"; then
    ALL_SERVICES_OK=false
fi

if ! curl -s http://localhost:5055/health &>/dev/null; then
    ALL_SERVICES_OK=false
fi

if ! curl -s http://localhost:3000 &>/dev/null; then
    ALL_SERVICES_OK=false
fi

if [ "$ALL_SERVICES_OK" = true ]; then
    echo -e "🎉 ${GREEN}All services are running perfectly!${NC}"
    echo -e "✅ ${GREEN}TinhKhoanApp is ready for development${NC}"
else
    echo -e "⚠️  ${YELLOW}Some services need attention${NC}"
    echo -e "🔧 ${YELLOW}Run troubleshoot script to fix issues${NC}"
fi

echo ""
echo -e "${CYAN}Report generated at: $(date '+%Y-%m-%d %H:%M:%S')${NC}"
