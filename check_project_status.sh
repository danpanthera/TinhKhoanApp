#!/bin/bash

echo "🔍 Checking TinhKhoan Project Status..."
echo "════════════════════════════════════════"

# Function to check if port is in use
check_port() {
    local port=$1
    local service_name=$2
    
    if lsof -ti:$port >/dev/null 2>&1; then
        echo "✅ $service_name: Port $port is in use"
        return 0
    else
        echo "❌ $service_name: Port $port is free (service not running)"
        return 1
    fi
}

# Function to check HTTP response
check_http() {
    local url=$1
    local service_name=$2
    
    local status_code=$(curl -s -o /dev/null -w "%{http_code}" --connect-timeout 5 $url 2>/dev/null)
    
    if [ "$status_code" = "200" ]; then
        echo "✅ $service_name: HTTP 200 OK"
        return 0
    else
        echo "❌ $service_name: HTTP $status_code or unreachable"
        return 1
    fi
}

# Check Backend
echo "�� Backend API (Port 5055):"
check_port 5055 "Backend Process"
check_http "http://localhost:5055/api/Health" "Backend API"

echo ""

# Check Frontend  
echo "🎨 Frontend UI (Port 3000):"
check_port 3000 "Frontend Process"
check_http "http://localhost:3000" "Frontend UI"

echo ""

# Check Database Connection (via backend)
echo "🗄️ Database Connection:"
db_response=$(curl -s --connect-timeout 5 http://localhost:5055/api/Health 2>/dev/null)
if echo "$db_response" | grep -q "healthy"; then
    echo "✅ Database: Connection healthy"
else
    echo "❌ Database: Connection issues"
fi

echo ""
echo "════════════════════════════════════════"

# Summary
backend_ok=0
frontend_ok=0

if lsof -ti:5055 >/dev/null 2>&1 && curl -s -o /dev/null --connect-timeout 5 http://localhost:5055/api/Health >/dev/null 2>&1; then
    backend_ok=1
fi

if lsof -ti:3000 >/dev/null 2>&1 && curl -s -o /dev/null --connect-timeout 5 http://localhost:3000 >/dev/null 2>&1; then
    frontend_ok=1
fi

if [ $backend_ok -eq 1 ] && [ $frontend_ok -eq 1 ]; then
    echo "🎉 PROJECT STATUS: ALL SERVICES RUNNING ✅"
    echo "🌐 Backend:  http://localhost:5055"
    echo "🌐 Frontend: http://localhost:3000"
elif [ $backend_ok -eq 1 ]; then
    echo "⚠️  PROJECT STATUS: BACKEND ONLY ⚡"
    echo "🌐 Backend:  http://localhost:5055"
    echo "❌ Frontend: Not running"
elif [ $frontend_ok -eq 1 ]; then
    echo "⚠️  PROJECT STATUS: FRONTEND ONLY ⚡"
    echo "❌ Backend:  Not running"
    echo "🌐 Frontend: http://localhost:3000"
else
    echo "❌ PROJECT STATUS: ALL SERVICES DOWN ⚠️"
fi

echo "════════════════════════════════════════"
