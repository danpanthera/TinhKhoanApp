#!/bin/bash
# final-rawdata-static-fix-verification.sh
# Script kiểm thử sau khi sửa static context

echo "===== KIỂM TRA BACKEND SAU KHI SỬA STATIC CONTEXT ====="

# Lấy danh sách raw data hiện tại
echo -e "\n1. Lấy danh sách raw data hiện tại..."
curl -s http://localhost:5000/api/rawdata | jq '.|length'

# Xóa một item (ID 1)
echo -e "\n2. Xóa item ID 1..."
curl -s -X DELETE http://localhost:5000/api/rawdata/1 | jq '.message'

# Kiểm tra lại danh sách sau khi xóa
echo -e "\n3. Kiểm tra danh sách sau khi xóa..."
curl -s http://localhost:5000/api/rawdata | jq '.|length'

# Import file mới (mô phỏng bằng lệnh curl)
echo -e "\n4. Import file mới (mô phỏng)..."
curl -s -X POST \
  -F "Files=@test.xlsx" \
  -F "Notes=Test import after static fix" \
  http://localhost:5000/api/rawdata/import/LN01 | jq '.message'

# Kiểm tra lại danh sách sau khi import
echo -e "\n5. Kiểm tra danh sách sau khi import..."
curl -s http://localhost:5000/api/rawdata | jq '.|length'

echo -e "\n===== KIỂM TRA HOÀN TẤT ====="
