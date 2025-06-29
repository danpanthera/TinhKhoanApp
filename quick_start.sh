#!/bin/bash

echo "🚀 KHỞI ĐỘNG NHANH TINHKHOAN APP"
echo "================================="

# Set working directories
BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
FRONTEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite"

# Function to check if port is in use
check_port() {
    lsof -i :$1 > /dev/null 2>&1
}

echo "🔍 Checking backend (port 5055)..."
if check_port 5055; then
    echo "✅ Backend đã chạy"
else
    echo "🚀 Starting backend..."
    cd "$BACKEND_DIR"
    dotnet run --urls=http://localhost:5055 > /dev/null 2>&1 &
    sleep 3
    echo "✅ Backend started"
fi

echo "�� Checking frontend (port 5173)..."
if check_port 5173; then
    echo "✅ Frontend đã chạy"  
else
    echo "🚀 Starting frontend..."
    cd "$FRONTEND_DIR"
    npm run dev > /dev/null 2>&1 &
    sleep 5
    echo "✅ Frontend started"
fi

echo ""
echo "🎉 KHỞI ĐỘNG HOÀN TẤT!"
echo "📱 Frontend: http://localhost:5173"
echo "🔗 Backend API: http://localhost:5055"
echo "================================="
