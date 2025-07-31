#!/bin/bash

echo "🛑 Safe Project Shutdown Script"
echo "════════════════════════════════════════"

# Function to safely kill process on port
safe_kill() {
    local port=$1
    local service_name=$2
    
    local pids=$(lsof -ti:$port 2>/dev/null)
    
    if [ -z "$pids" ]; then
        echo "✅ $service_name: No process found on port $port"
        return 0
    fi
    
    echo "🔍 $service_name: Found processes on port $port: $pids"
    
    for pid in $pids; do
        echo "⚡ Terminating $service_name process (PID: $pid)..."
        
        # Try graceful shutdown first
        kill $pid 2>/dev/null
        sleep 3
        
        # Check if still running
        if kill -0 $pid 2>/dev/null; then
            echo "💀 Force killing $service_name process (PID: $pid)..."
            kill -9 $pid 2>/dev/null
            sleep 1
        fi
        
        # Verify
        if kill -0 $pid 2>/dev/null; then
            echo "❌ Failed to kill $service_name process (PID: $pid)"
        else
            echo "✅ Successfully killed $service_name process (PID: $pid)"
        fi
    done
}

echo "1️⃣ Stopping Backend API (Port 5055)..."
safe_kill 5055 "Backend"

echo ""
echo "2️⃣ Stopping Frontend UI (Port 3000)..."
safe_kill 3000 "Frontend"

echo ""
echo "3️⃣ Final verification..."
sleep 2

# Check final status
backend_check=$(lsof -ti:5055 2>/dev/null)
frontend_check=$(lsof -ti:3000 2>/dev/null)

if [ -z "$backend_check" ] && [ -z "$frontend_check" ]; then
    echo "🎉 All services stopped successfully ✅"
elif [ -z "$backend_check" ]; then
    echo "⚠️  Backend stopped, but Frontend still running"
elif [ -z "$frontend_check" ]; then
    echo "⚠️  Frontend stopped, but Backend still running"
else
    echo "❌ Some services are still running"
fi

echo "════════════════════════════════════════"
