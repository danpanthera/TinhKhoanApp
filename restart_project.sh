#!/bin/bash

# =====================================================
# PROJECT FULL RESTART SCRIPT - JULY 13, 2025
# Script khởi động lại toàn bộ dự án an toàn
# =====================================================

echo "🚀 KHỞI ĐỘNG LẠI TOÀN BỘ DỰ ÁN TINHKHOAN"
echo "=========================================="

# Dừng tất cả processes cũ
echo "🛑 Dừng tất cả processes cũ..."
pkill -f "dotnet"
pkill -f "vite" 
pkill -f "npm run dev"
sleep 3

# Kiểm tra Docker container
echo "🐳 Kiểm tra Azure SQL Edge container..."
if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "✅ Database container đang chạy"
else
    echo "⚠️  Khởi động database container..."
    docker start azure_sql_edge_tinhkhoan
    sleep 5
fi

# Khởi động Backend
echo ""
echo "🔧 KHỞI ĐỘNG BACKEND..."
echo "========================"
cd Backend/KhoanApp.Api
./start_backend.sh &
BACKEND_PID=$!

# Đợi backend khởi động
echo "⏳ Đợi backend khởi động..."
sleep 10

# Kiểm tra backend health
if curl -s http://localhost:5055/health > /dev/null; then
    echo "✅ Backend đã sẵn sàng trên http://localhost:5055"
else
    echo "❌ Backend chưa sẵn sàng"
fi

# Khởi động Frontend
echo ""
echo "🎨 KHỞI ĐỘNG FRONTEND..."
echo "========================"
cd ../../Frontend/KhoanUI
./start_frontend.sh &
FRONTEND_PID=$!

# Đợi frontend khởi động
echo "⏳ Đợi frontend khởi động..."
sleep 5

# Kiểm tra frontend
if curl -s http://localhost:3000 > /dev/null; then
    echo "✅ Frontend đã sẵn sàng trên http://localhost:3000"
else
    echo "❌ Frontend chưa sẵn sàng"
fi

echo ""
echo "🎉 DỰ ÁN ĐÃ KHỞI ĐỘNG HOÀN TẤT!"
echo "================================"
echo "📋 THÔNG TIN:"
echo "   - Backend API: http://localhost:5055"
echo "   - Frontend UI: http://localhost:3000"
echo "   - Swagger API: http://localhost:5055/swagger"
echo ""
echo "💡 QUẢN LÝ PROCESSES:"
echo "   - Backend PID: $BACKEND_PID"
echo "   - Frontend PID: $FRONTEND_PID"
echo "   - Nhấn Ctrl+C để dừng script này"
echo ""
echo "📊 Kiểm tra trạng thái hệ thống..."
cd ../../Backend/KhoanApp.Api
./verify_system_ready.sh

# Giữ script chạy
echo ""
echo "🔄 Monitoring processes... (Ctrl+C to stop)"
wait
