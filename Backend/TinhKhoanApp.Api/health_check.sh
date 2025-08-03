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
