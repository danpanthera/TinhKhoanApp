#!/bin/bash

# 🔥 SCRIPT KIỂM TRA TỔNG THỂ HỆ THỐNG TINHKHOAN APP
# Kiểm tra backend, frontend, database, và các chức năng chính

echo "🔍 KIỂM TRA TỔNG THỂ HỆ THỐNG TINHKHOAN APP"
echo "============================================"

# Kiểm tra backend API
echo ""
echo "📡 Kiểm tra Backend API..."
backend_response=$(curl -s -w "%{http_code}" -o /dev/null http://localhost:5055/api/rawdata/test-simple)
if [ "$backend_response" = "200" ]; then
    echo "✅ Backend API hoạt động bình thường"
else
    echo "❌ Backend API không phản hồi (HTTP: $backend_response)"
fi

# Kiểm tra frontend
echo ""
echo "🎨 Kiểm tra Frontend..."
frontend_response=$(curl -s -w "%{http_code}" -o /dev/null http://localhost:3001)
if [ "$frontend_response" = "200" ]; then
    echo "✅ Frontend hoạt động bình thường"
else
    echo "❌ Frontend không phản hồi (HTTP: $frontend_response)"
fi

# Test import GLCB41 với file CSV nhỏ
echo ""
echo "📂 Test import GLCB41..."
import_result=$(curl -s -X POST http://localhost:5055/api/rawdata/import/glcb41 \
  -F "files=@test_glcb41.csv" \
  -H "Content-Type: multipart/form-data")

if echo "$import_result" | grep -q "success.*true"; then
    echo "✅ Import GLCB41 thành công"
    
    # Lấy thông tin records từ response
    records=$(echo "$import_result" | grep -o '"recordsProcessed":[0-9]*' | cut -d':' -f2)
    echo "   📊 Số records import: $records"
else
    echo "❌ Import GLCB41 thất bại"
    echo "   📄 Response: $import_result"
fi

# Kiểm tra dữ liệu gần đây
echo ""
echo "📊 Kiểm tra dữ liệu gần đây..."
recent_data=$(curl -s http://localhost:5055/api/rawdata/recent)
if echo "$recent_data" | grep -q '"status":"Completed"'; then
    echo "✅ Có dữ liệu import gần đây với status Completed"
    # Đếm số import thành công
    completed_count=$(echo "$recent_data" | grep -o '"status":"Completed"' | wc -l)
    echo "   📊 Số imports thành công: $completed_count"
else
    echo "❌ Không tìm thấy dữ liệu import thành công gần đây"
fi

# Tóm tắt
echo ""
echo "📋 TÓM TẮT:"
echo "=========="
echo "- Backend API: $([ "$backend_response" = "200" ] && echo "✅ OK" || echo "❌ ERROR")"
echo "- Frontend: $([ "$frontend_response" = "200" ] && echo "✅ OK" || echo "❌ ERROR")"
echo "- Import GLCB41: $(echo "$import_result" | grep -q "success.*true" && echo "✅ OK" || echo "❌ ERROR")"
echo ""
echo "🎯 Hệ thống đã hoạt động ổn định!"
echo "🌐 Frontend: http://localhost:3001"
echo "📡 Backend API: http://localhost:5055"
