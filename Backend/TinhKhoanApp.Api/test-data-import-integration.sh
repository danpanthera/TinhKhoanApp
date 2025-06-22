#!/bin/bash

# Script kiểm thử tích hợp cho tính năng import dữ liệu thô
# Chức năng: Kiểm tra việc thêm dữ liệu import mới vào mock data

echo "📋 KIỂM THỬ TÍCH HỢP KHO DỮ LIỆU THÔ"
echo "======================================"
echo ""

# Xóa log file cũ nếu tồn tại
rm -f test-data-import.log

# Tạo file test để import
echo "🔧 Đang tạo file test..."
echo "Test data for import,Value" > test_DP01_20250615.csv
echo "1,100" >> test_DP01_20250615.csv
echo "2,200" >> test_DP01_20250615.csv
echo "3,300" >> test_DP01_20250615.csv

# API Base URL
API_URL="https://localhost:7001/api"

# Kiểm tra server có hoạt động không
echo "🔍 Kiểm tra server API..."
curl -s -k "${API_URL}/health" > /dev/null
if [ $? -ne 0 ]; then
    echo "❌ Server API không hoạt động. Vui lòng kiểm tra lại."
    exit 1
fi
echo "✅ Server API hoạt động."
echo ""

# Lấy danh sách ban đầu
echo "📋 Lấy danh sách dữ liệu ban đầu..."
curl -s -k -X GET "${API_URL}/rawdata" -H "accept: application/json" | tee -a test-data-import.log > initial_list.json
INITIAL_COUNT=$(cat initial_list.json | grep -o '"Id"' | wc -l)
echo "👉 Số lượng dữ liệu ban đầu: $INITIAL_COUNT"
echo ""

# Import file test
echo "📤 Import file test..."
curl -s -k -X POST "${API_URL}/rawdata/import/DP01" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Files=@test_DP01_20250615.csv" \
  -F "Notes=File test tự động import" | tee -a test-data-import.log > import_result.json

IMPORT_SUCCESS=$(cat import_result.json | grep -c "thành công")
if [ $IMPORT_SUCCESS -eq 0 ]; then
    echo "❌ Import thất bại. Chi tiết trong file test-data-import.log"
    exit 1
fi
echo "✅ Import thành công!"
echo ""

# Đợi 1 giây để đảm bảo dữ liệu đã được cập nhật
sleep 1

# Lấy danh sách sau khi import
echo "📋 Lấy danh sách dữ liệu sau khi import..."
curl -s -k -X GET "${API_URL}/rawdata" -H "accept: application/json" | tee -a test-data-import.log > after_import_list.json
AFTER_IMPORT_COUNT=$(cat after_import_list.json | grep -o '"Id"' | wc -l)
echo "👉 Số lượng dữ liệu sau khi import: $AFTER_IMPORT_COUNT"
echo ""

# Kiểm tra file đã import có trong danh sách không
echo "🔍 Kiểm tra file đã import có trong danh sách không..."
FILE_IN_LIST=$(cat after_import_list.json | grep -c "test_DP01_20250615.csv")
if [ $FILE_IN_LIST -eq 0 ]; then
    echo "❌ KHÔNG TÌM THẤY file đã import trong danh sách!"
    echo "⚠️ Vấn đề: Dữ liệu import mới không được thêm vào mock data."
    exit 1
else
    echo "✅ TÌM THẤY file đã import trong danh sách!"
fi
echo ""

# Kiểm tra tổng số lượng record có tăng sau khi import không
if [ $AFTER_IMPORT_COUNT -gt $INITIAL_COUNT ]; then
    echo "✅ THÀNH CÔNG: Số lượng dữ liệu đã tăng sau khi import ($INITIAL_COUNT -> $AFTER_IMPORT_COUNT)"
else
    echo "❌ THẤT BẠI: Số lượng dữ liệu không tăng sau khi import ($INITIAL_COUNT -> $AFTER_IMPORT_COUNT)"
    exit 1
fi
echo ""

# Xóa file test tạm
rm -f test_DP01_20250615.csv initial_list.json after_import_list.json import_result.json

echo "📋 KẾT QUẢ KIỂM THỬ TÍCH HỢP"
echo "============================="
echo "✅ Import dữ liệu mới: THÀNH CÔNG"
echo "✅ Dữ liệu xuất hiện trong danh sách: CÓ"
echo "✅ Số lượng tăng sau khi import: CÓ (từ $INITIAL_COUNT lên $AFTER_IMPORT_COUNT)"
echo ""
echo "🎉 KIỂM THỬ THÀNH CÔNG!"
