#!/bin/bash

echo "🚀 ===== KHỞI ĐỘNG LẠI DỰ ÁN TINHKHOANAPP ====="
echo ""

# 1. Kiểm tra và khởi động database container
echo "📊 1. Kiểm tra Azure SQL Edge Container..."
if docker ps | grep -q azure_sql_edge_tinhkhoan; then
    echo "✅ Database container đang chạy"
else
    echo "🔄 Khởi động database container..."
    docker start azure_sql_edge_tinhkhoan
    sleep 5
fi

# 2. Dừng các process cũ
echo ""
echo "🔄 2. Dừng các process cũ..."
pkill -f "dotnet run" 2>/dev/null || echo "   - No dotnet processes to kill"
pkill -f "npm run dev" 2>/dev/null || echo "   - No npm processes to kill"
pkill -f "vite --host" 2>/dev/null || echo "   - No vite processes to kill"


sleep 3

# 3. Khởi động backend
echo ""
echo "🚀 3. Khởi động Backend API (Port 5055)..."
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
nohup dotnet run --urls=http://localhost:5055 > backend.log 2>&1 &
BACKEND_PID=$!

# Đợi backend khởi động
echo "   - Đang đợi backend khởi động..."
for i in {1..15}; do
    if curl -s "http://localhost:5055/health" >/dev/null 2>&1; then
        echo "   ✅ Backend online sau $i giây"
        break
    fi
    sleep 1
done

# 4. Khởi động frontend  
echo ""
echo "🌐 4. Khởi động Frontend (Port 3000)..."
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
nohup npm run dev > frontend.log 2>&1 &
FRONTEND_PID=$!

# Đợi frontend khởi động
echo "   - Đang đợi frontend khởi động..."
for i in {1..20}; do
    if curl -s "http://localhost:3000" >/dev/null 2>&1; then
        echo "   ✅ Frontend online sau $i giây"
        break
    fi
    sleep 1
done

# 5. Kiểm tra tổng quan
echo ""
echo "📋 5. KIỂM TRA TỔNG QUAN:"
echo "   Database: $(docker ps | grep azure_sql_edge_tinhkhoan | awk '{print "✅ Up",$7,$8,$9}' || echo "❌ Down")"
echo "   Backend:  $(curl -s "http://localhost:5055/health" >/dev/null && echo "✅ Online (Port 5055)" || echo "❌ Offline")"
echo "   Frontend: $(curl -s "http://localhost:3000" >/dev/null && echo "✅ Online (Port 3000)" || echo "❌ Offline")"

echo ""
echo "🔧 6. KIỂM TRA API FEATURES:"
DIRECTIMPORT_STATUS=$(curl -s "http://localhost:5055/api/DirectImport/status" | jq -r '.Status' 2>/dev/null)
echo "   DirectImport: $DIRECTIMPORT_STATUS"

IMPORT_COUNT=$(curl -s "http://localhost:5055/api/DataImport/records" | jq '. | length' 2>/dev/null)
echo "   Import Records: $IMPORT_COUNT records"

echo ""
echo "🎉 DỰ ÁN ĐÃ KHỞI ĐỘNG HOÀN TẤT!"
echo ""
echo "📝 THÔNG TIN TRUY CẬP:"
echo "   🌐 Frontend: http://localhost:3000"
echo "   🚀 Backend API: http://localhost:5055"
echo "   📊 API Health: http://localhost:5055/health"
echo "   🧠 Smart Import: http://localhost:5055/api/DirectImport/status"
echo ""
echo "📊 Process IDs:"
echo "   Backend PID: $BACKEND_PID"
echo "   Frontend PID: $FRONTEND_PID"
echo ""
echo "📋 LOG FILES:"
echo "   Backend: /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/backend.log"
echo "   Frontend: /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/frontend.log"
