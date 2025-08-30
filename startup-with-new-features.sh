#!/bin/bash

# 🚀 Startup script for KhoanApp with all new features
# Preview, Delete, Number Formatting

echo "🚀 ===== STARTING TINHKHOANAPP WITH NEW FEATURES ====="

# Check Docker
echo "1. 🐳 Checking Docker containers..."
CONTAINER_STATUS=$(docker ps --format "table {{.Names}}\t{{.Status}}" | grep azure_sql_edge_tinhkhoan)
if [ -z "$CONTAINER_STATUS" ]; then
    echo "⚠️ Starting Azure SQL Edge container..."
    docker start azure_sql_edge_tinhkhoan
    sleep 5
else
    echo "✅ Azure SQL Edge container is running"
fi

# Start Backend
echo "2. 🔧 Starting Backend API..."
cd /opt/Projects/Khoan/Backend/KhoanApp.Api

# Kill existing backend process if running
BACKEND_PID=$(lsof -ti:5055)
if [ ! -z "$BACKEND_PID" ]; then
    echo "⚠️ Killing existing backend process..."
    kill $BACKEND_PID
    sleep 2
fi

# Start backend in background
echo "🚀 Starting backend on port 5055..."
dotnet run --urls=http://localhost:5055 > backend.log 2>&1 &
BACKEND_PID=$!
echo "✅ Backend started with PID: $BACKEND_PID"

# Wait for backend to be ready
echo "⏳ Waiting for backend to be ready..."
for i in {1..30}; do
    if curl -s http://localhost:5055/health > /dev/null 2>&1; then
        echo "✅ Backend is ready!"
        break
    fi
    sleep 1
    echo -n "."
done

# Start Frontend
echo "3. 🎨 Starting Frontend..."
cd /opt/Projects/Khoan/Frontend/KhoanUI

# Kill existing frontend process if running
FRONTEND_PID=$(lsof -ti:3000)
if [ ! -z "$FRONTEND_PID" ]; then
    echo "⚠️ Killing existing frontend process..."
    kill $FRONTEND_PID
    sleep 2
fi

# Start frontend in background
echo "🚀 Starting frontend on port 3000..."
npm run dev > frontend.log 2>&1 &
FRONTEND_PID=$!
echo "✅ Frontend started with PID: $FRONTEND_PID"

# Wait for frontend to be ready
echo "⏳ Waiting for frontend to be ready..."
for i in {1..30}; do
    if curl -s http://localhost:3000 > /dev/null 2>&1; then
        echo "✅ Frontend is ready!"
        break
    fi
    sleep 1
    echo -n "."
done

# Test new features
echo "4. 🧪 Testing new features..."

# Test API endpoints
echo "4.1 📊 Testing import records..."
RECORDS_COUNT=$(curl -s "http://localhost:5055/api/DataImport/records" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ Found $RECORDS_COUNT import records"

# Test preview endpoint
if [ "$RECORDS_COUNT" -gt 0 ]; then
    FIRST_RECORD_ID=$(curl -s "http://localhost:5055/api/DataImport/records" | jq -r '.[0].Id // .[0].id' 2>/dev/null)
    if [ "$FIRST_RECORD_ID" != "null" ] && [ "$FIRST_RECORD_ID" != "0" ]; then
        echo "4.2 🔍 Testing preview endpoint..."
        PREVIEW_RESPONSE=$(curl -s "http://localhost:5055/api/DataImport/preview/$FIRST_RECORD_ID")
        PREVIEW_SUCCESS=$(echo "$PREVIEW_RESPONSE" | jq -r '.ImportId // "null"' 2>/dev/null)
        if [ "$PREVIEW_SUCCESS" != "null" ]; then
            echo "✅ Preview API working"
        else
            echo "❌ Preview API failed"
        fi
    fi
fi

# Display status
echo ""
echo "🎯 ===== STARTUP COMPLETE ====="
echo "🔧 Backend API: http://localhost:5055"
echo "🔧 Backend Health: http://localhost:5055/health"
echo "🔧 Backend Swagger: http://localhost:5055/swagger"
echo "🎨 Frontend App: http://localhost:3000"
echo "🧪 Number Format Test: http://localhost:3000/test-number-formatting.html"
echo ""
echo "🆕 NEW FEATURES AVAILABLE:"
echo "1. 💰 Format số Triệu VND: #,###.00 tr.VND"
echo "2. 🔍 Preview dữ liệu chi tiết từ database"
echo "3. 🗑️ Xóa bản ghi với xác nhận"
echo ""
echo "📋 USAGE:"
echo "- Go to 'Kho dữ liệu thô' page"
echo "- Click 👁️ to preview data"
echo "- Click 🗑️ to delete records"
echo "- Use formatMillionVND() for Triệu VND formatting"
echo ""
echo "🔄 TO STOP SERVICES:"
echo "- Backend: kill $BACKEND_PID"
echo "- Frontend: kill $FRONTEND_PID"
echo "- Or use: pkill -f 'dotnet run' && pkill -f 'npm run dev'"
echo ""
echo "🏆 ALL FEATURES READY FOR PRODUCTION TESTING!"

# Save PIDs for later use
echo "BACKEND_PID=$BACKEND_PID" > /tmp/tinhkhoan_pids.txt
echo "FRONTEND_PID=$FRONTEND_PID" >> /tmp/tinhkhoan_pids.txt

echo "📝 Process IDs saved to /tmp/tinhkhoan_pids.txt"
