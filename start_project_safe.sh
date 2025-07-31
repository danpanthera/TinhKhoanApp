#!/bin/bash

echo "🚀 Safe Project Startup Script"
echo "════════════════════════════════════════"

# Function to kill process on port if exists
kill_port() {
    local port=$1
    local service_name=$2
    
    local pid=$(lsof -ti:$port 2>/dev/null)
    if [ ! -z "$pid" ]; then
        echo "⚡ Killing existing $service_name process (PID: $pid)"
        kill $pid 2>/dev/null
        sleep 2
    fi
}

# Function to start service safely
start_service() {
    local service_name=$1
    local port=$2
    local start_command=$3
    local working_dir=$4
    
    echo "🔄 Starting $service_name..."
    
    # Kill existing process
    kill_port $port "$service_name"
    
    # Start new process
    cd "$working_dir"
    echo "📁 Working directory: $working_dir"
    echo "▶️  Command: $start_command"
    
    # Start in background and capture PID
    nohup bash -c "$start_command" > "${service_name,,}_startup.log" 2>&1 &
    local pid=$!
    
    echo "🆔 Started $service_name with PID: $pid"
    
    # Wait and verify
    sleep 5
    if kill -0 $pid 2>/dev/null; then
        echo "✅ $service_name is running (PID: $pid)"
        return 0
    else
        echo "❌ $service_name failed to start"
        return 1
    fi
}

echo "1️⃣ Starting Backend API..."
start_service "Backend" 5055 "dotnet run" "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"

echo ""
echo "2️⃣ Starting Frontend UI..."
start_service "Frontend" 3000 "npm run dev" "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite"

echo ""
echo "3️⃣ Final verification..."
sleep 3

# Run status check
/Users/nguyendat/Documents/Projects/TinhKhoanApp/check_project_status.sh
