#!/bin/bash

# 🧪 Test script để kiểm tra chức năng import file ZIP có mật khẩu
# Tính Khoán App - Agribank Lai Châu Center

echo "🧪 Bắt đầu kiểm tra chức năng import file ZIP..."

BASE_URL="http://localhost:5001/api/rawdata"

# Kiểm tra server có chạy không
echo "🔍 Kiểm tra server backend..."
if curl -s "$BASE_URL" > /dev/null; then
    echo "✅ Backend server đang hoạt động"
else
    echo "❌ Backend server không phản hồi trên $BASE_URL"
    exit 1
fi

# Tạo file test ZIP (mock)
echo "📦 Tạo file test mock..."
mkdir -p /tmp/test-raw-data
echo "Mock Excel Data for LN01 type 20241221" > /tmp/test-raw-data/LN01_20241221_test.xlsx
echo "Mock CSV Data for DP01 type 20241221" > /tmp/test-raw-data/DP01_20241221_test.csv
echo "Mock TXT Data for GL01 type 20241221" > /tmp/test-raw-data/GL01_20241221_test.txt

# Kiểm tra zip command có sẵn không
if command -v zip > /dev/null; then
    cd /tmp/test-raw-data
    zip -P "test123" ../test-archive.zip *.xlsx *.csv *.txt
    echo "✅ Đã tạo file ZIP có mật khẩu: /tmp/test-archive.zip (password: test123)"
    
    # Kiểm tra endpoint import có hoạt động không
    echo "🔍 Kiểm tra endpoint import LN01..."
    
    # Test import với file ZIP có mật khẩu
    response=$(curl -s -w "%{http_code}" -o /tmp/import_response.json \
        -X POST "$BASE_URL/import/LN01" \
        -F "Files=@/tmp/test-archive.zip" \
        -F "ArchivePassword=test123" \
        -F "Notes=Test import ZIP file with password")
    
    http_code="${response: -3}"
    
    if [ "$http_code" = "200" ]; then
        echo "✅ Import file ZIP với mật khẩu thành công!"
        echo "📋 Response:"
        cat /tmp/import_response.json | head -10
        echo ""
    else
        echo "❌ Import file ZIP thất bại (HTTP $http_code)"
        echo "📋 Response:"
        cat /tmp/import_response.json
        echo ""
    fi
    
    # Test import với mật khẩu sai
    echo "🔍 Kiểm tra với mật khẩu sai..."
    response=$(curl -s -w "%{http_code}" -o /tmp/import_wrong_pass.json \
        -X POST "$BASE_URL/import/LN01" \
        -F "Files=@/tmp/test-archive.zip" \
        -F "ArchivePassword=wrongpassword" \
        -F "Notes=Test with wrong password")
    
    http_code="${response: -3}"
    
    if [ "$http_code" = "400" ] || [ "$http_code" = "500" ]; then
        echo "✅ Xử lý mật khẩu sai đúng như mong đợi (HTTP $http_code)"
    else
        echo "⚠️ Unexpected response for wrong password (HTTP $http_code)"
    fi
    
    # Cleanup
    rm -f /tmp/test-archive.zip /tmp/import_response.json /tmp/import_wrong_pass.json
    rm -rf /tmp/test-raw-data
    
else
    echo "⚠️ Không tìm thấy command 'zip' để tạo file test"
    echo "📝 Thử nghiệm manual: upload file ZIP có mật khẩu qua giao diện web"
fi

echo ""
echo "🌐 Mở giao diện web để kiểm tra manual:"
echo "Frontend: http://localhost:3001/data-import"
echo "📝 Hướng dẫn test manual:"
echo "1. Tạo file Excel/CSV/TXT có tên chứa LN01/DP01/GL01 và ngày (ví dụ: LN01_20241221_test.xlsx)"
echo "2. Nén thành file ZIP với mật khẩu"
echo "3. Upload và nhập mật khẩu trên giao diện"
echo "4. Kiểm tra kết quả import"

echo ""
echo "🏁 Kiểm tra hoàn tất!"
