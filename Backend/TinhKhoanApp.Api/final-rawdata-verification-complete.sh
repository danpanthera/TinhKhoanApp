#!/bin/bash

# Script kiểm tra cuối cùng để xác nhận đã sửa triệt để vấn đề với kho dữ liệu thô
# Thực hiện kiểm tra toàn diện tất cả các endpoint của raw data API

echo "🔍 KIỂM TRA CUỐI CÙNG KHO DỮ LIỆU THÔ"
echo "====================================="
echo ""

# Biến lưu trạng thái kiểm tra (0 = thành công, 1 = thất bại)
TEST_STATUS=0

# API Base URL
API_URL="https://localhost:7001/api"

# 1. Kiểm tra kết nối server
echo "1️⃣ Kiểm tra kết nối server..."
if curl -s -k "${API_URL}/health" > /dev/null; then
    echo "✅ Server hoạt động."
else
    echo "❌ Server không hoạt động."
    exit 1
fi
echo ""

# 2. Kiểm tra endpoint lấy danh sách raw data
echo "2️⃣ Kiểm tra GET /api/rawdata..."
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata" -H "accept: application/json")
if [[ $RESPONSE == *"Id"* ]]; then
    echo "✅ GET /api/rawdata trả về dữ liệu hợp lệ."
else
    echo "❌ GET /api/rawdata không trả về dữ liệu hợp lệ."
    TEST_STATUS=1
fi
echo ""

# 3. Kiểm tra endpoint lấy dashboard stats
echo "3️⃣ Kiểm tra GET /api/rawdata/dashboard/stats..."
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata/dashboard/stats" -H "accept: application/json")
if [[ $RESPONSE == *"totalImports"* ]]; then
    echo "✅ GET /api/rawdata/dashboard/stats trả về dữ liệu hợp lệ."
else
    echo "❌ GET /api/rawdata/dashboard/stats không trả về dữ liệu hợp lệ."
    TEST_STATUS=1
fi
echo ""

# 4. Kiểm tra endpoint lấy dữ liệu theo ngày
echo "4️⃣ Kiểm tra GET /api/rawdata/by-date/LN01/20250615..."
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata/by-date/LN01/20250615" -H "accept: application/json")
if [[ $RESPONSE == *"["* ]]; then
    echo "✅ GET /api/rawdata/by-date trả về dữ liệu hợp lệ."
else
    echo "❌ GET /api/rawdata/by-date không trả về dữ liệu hợp lệ."
    TEST_STATUS=1
fi
echo ""

# 5. Kiểm tra endpoint check duplicate
echo "5️⃣ Kiểm tra GET /api/rawdata/check-duplicate/LN01/20250615..."
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata/check-duplicate/LN01/20250615" -H "accept: application/json")
if [[ $RESPONSE == *"hasDuplicate"* ]]; then
    echo "✅ GET /api/rawdata/check-duplicate trả về dữ liệu hợp lệ."
else
    echo "❌ GET /api/rawdata/check-duplicate không trả về dữ liệu hợp lệ."
    TEST_STATUS=1
fi
echo ""

# 6. Kiểm tra endpoint lấy dữ liệu tối ưu
echo "6️⃣ Kiểm tra GET /api/rawdata/optimized/records..."
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata/optimized/records" -H "accept: application/json")
if [[ $RESPONSE == *"data"* ]]; then
    echo "✅ GET /api/rawdata/optimized/records trả về dữ liệu hợp lệ."
else
    echo "❌ GET /api/rawdata/optimized/records không trả về dữ liệu hợp lệ."
    TEST_STATUS=1
fi
echo ""

# 7. Tạo file test và upload
echo "7️⃣ Kiểm tra thêm dữ liệu mới..."
echo "Dữ liệu kiểm tra,Giá trị" > test_DP01_final.csv
echo "1,100" >> test_DP01_final.csv
echo "2,200" >> test_DP01_final.csv

RESPONSE=$(curl -s -k -X POST "${API_URL}/rawdata/import/DP01" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Files=@test_DP01_final.csv" \
  -F "Notes=Kiểm tra cuối cùng")

if [[ $RESPONSE == *"thành công"* ]]; then
    echo "✅ POST /api/rawdata/import/DP01 import thành công."
else
    echo "❌ POST /api/rawdata/import/DP01 import thất bại."
    TEST_STATUS=1
fi
echo ""

# 8. Kiểm tra dữ liệu mới có trong danh sách
echo "8️⃣ Kiểm tra dữ liệu mới có trong danh sách..."
sleep 1
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata" -H "accept: application/json")
if [[ $RESPONSE == *"test_DP01_final.csv"* ]]; then
    echo "✅ Dữ liệu mới xuất hiện trong danh sách."
else
    echo "❌ Dữ liệu mới KHÔNG xuất hiện trong danh sách."
    TEST_STATUS=1
fi
echo ""

# 9. Tìm ID của dữ liệu mới import
ID=$(echo $RESPONSE | grep -o '"Id":[0-9]*,"FileName":"test_DP01_final.csv"' | grep -o '[0-9]*')

# 10. Xóa dữ liệu vừa import
if [[ ! -z "$ID" ]]; then
    echo "9️⃣ Kiểm tra xóa dữ liệu mới import (ID: $ID)..."
    RESPONSE=$(curl -s -k -X DELETE "${API_URL}/rawdata/$ID" -H "accept: application/json")
    if [[ $RESPONSE == *"thành công"* ]]; then
        echo "✅ DELETE /api/rawdata/$ID xóa thành công."
    else
        echo "❌ DELETE /api/rawdata/$ID xóa thất bại."
        TEST_STATUS=1
    fi
    echo ""
fi

# 11. Kiểm tra dữ liệu đã bị xóa khỏi danh sách
echo "🔟 Kiểm tra dữ liệu đã bị xóa khỏi danh sách..."
sleep 1
RESPONSE=$(curl -s -k -X GET "${API_URL}/rawdata" -H "accept: application/json")
if [[ $RESPONSE == *"test_DP01_final.csv"* ]]; then
    echo "❌ Dữ liệu vẫn còn trong danh sách sau khi xóa."
    TEST_STATUS=1
else
    echo "✅ Dữ liệu đã bị xóa khỏi danh sách."
fi
echo ""

# Dọn dẹp
rm -f test_DP01_final.csv

# Kết quả kiểm tra
echo "📋 KẾT QUẢ KIỂM TRA CUỐI CÙNG"
echo "==============================="
if [ $TEST_STATUS -eq 0 ]; then
    echo "✅ TẤT CẢ KIỂM TRA ĐỀU THÀNH CÔNG!"
    echo "🎉 Đã sửa triệt để vấn đề với kho dữ liệu thô."
else
    echo "❌ CÓ LỖI TRONG QUÁ TRÌNH KIỂM TRA!"
    echo "⚠️ Vẫn còn vấn đề cần giải quyết."
fi
