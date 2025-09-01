#!/bin/bash

# Script khởi động backend stable
set -e

echo "🚀 Starting TinhKhoan Backend API..."

# Kiểm tra và kill process cũ trên port 5055
echo "🔍 Checking for existing processes on port 5055..."
if lsof -ti:5055 >/dev/null 2>&1; then
    echo "⚠️ Killing existing processes on port 5055..."
    lsof -ti:5055 | xargs kill -9 2>/dev/null || true
    sleep 2
fi

# Chuyển đến thư mục backend
cd /opt/Projects/Khoan/Backend/Khoan.Api

# Build project
echo "🔨 Building backend..."
dotnet build Khoan.Api.csproj --configuration Debug --verbosity minimal

# Khởi động backend với nohup để tránh interrupt
echo "🚀 Starting backend on port 5055..."
nohup dotnet run --urls=http://localhost:5055 > backend_stable.log 2>&1 &
backend_pid=$!

echo "✅ Backend started with PID: $backend_pid"
echo "📄 Log file: backend_stable.log"

# Đợi backend khởi động
echo "⏳ Waiting for backend to start..."
sleep 5

# Kiểm tra xem backend có chạy không
if netstat -an | grep -q ":5055.*LISTEN"; then
    echo "✅ Backend is running on http://localhost:5055"
    echo "🎯 Backend PID: $backend_pid"
    
    # Lưu PID vào file để dễ quản lý sau này
    echo $backend_pid > backend.pid
    echo "💾 PID saved to backend.pid"
else
    echo "❌ Backend failed to start"
    exit 1
fi
