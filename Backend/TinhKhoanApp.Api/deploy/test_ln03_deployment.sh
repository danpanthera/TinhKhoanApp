#!/bin/bash

# Script kiểm tra triển khai LN03
# Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

echo "=== Kiểm tra triển khai LN03 ==="

# Cấu hình API endpoint
API_BASE_URL="https://api.tinhkhoanapp.com"  # Thay thế bằng URL thực tế
API_TOKEN="YOUR_API_TOKEN"  # Thay thế bằng token thực tế

# Kiểm tra API hoạt động
echo "Kiểm tra kết nối API..."
curl -s -o /dev/null -w "%{http_code}" $API_BASE_URL/api/health

if [ $? -ne 0 ]; then
    echo "Không thể kết nối đến API. Kiểm tra lại kết nối mạng và trạng thái dịch vụ."
    exit 1
fi

# Kiểm tra endpoint LN03
echo "Kiểm tra endpoint LN03..."
curl -s -H "Authorization: Bearer $API_TOKEN" $API_BASE_URL/api/LN03/recent?count=1

if [ $? -ne 0 ]; then
    echo "Endpoint LN03 không phản hồi đúng. Kiểm tra logs để biết thêm chi tiết."
    exit 1
fi

# Thử upload file LN03 mẫu
echo "Thử upload file LN03 mẫu..."
curl -s -X POST -H "Authorization: Bearer $API_TOKEN" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@./test_data/ln03_sample.csv" \
     $API_BASE_URL/api/LN03/import

if [ $? -ne 0 ]; then
    echo "Không thể upload file LN03 mẫu. Kiểm tra logs để biết thêm chi tiết."
    exit 1
fi

echo "=== Kiểm tra hoàn tất thành công! ==="
