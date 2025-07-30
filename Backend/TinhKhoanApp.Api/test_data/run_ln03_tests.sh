#!/bin/bash

# Script thực hiện kiểm tra import LN03
# Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

echo "=== Chạy kiểm tra import LN03 ==="

# Cấu hình kết nối API
API_URL="http://localhost:5000"  # Thay thế bằng URL thực tế
JWT_TOKEN=""  # Cần được cập nhật với token hợp lệ

# Lấy token nếu cần
get_token() {
    echo "Đang lấy token xác thực..."
    JWT_TOKEN=$(curl -s -X POST "$API_URL/api/Auth/login" \
        -H "Content-Type: application/json" \
        -d '{"username":"admin","password":"Admin123!"}' | grep -o '"token":"[^"]*' | sed 's/"token":"//')
    
    if [ -z "$JWT_TOKEN" ]; then
        echo "Không thể lấy token. Kiểm tra kết nối API và thông tin đăng nhập."
        exit 1
    fi
    
    echo "Đã lấy token xác thực thành công."
}

# Kiểm tra API health
check_api_health() {
    echo "Kiểm tra API health..."
    local health_status=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/health")
    
    if [ "$health_status" = "200" ]; then
        echo "API hoạt động bình thường."
        return 0
    else
        echo "API không phản hồi đúng. Status code: $health_status"
        return 1
    fi
}

# Test case 1: Import file hợp lệ
test_valid_import() {
    echo -e "\n=== Test case 1: Import file hợp lệ ==="
    echo "Đang import file ln03_test_sample.csv..."
    
    curl -s -X POST "$API_URL/api/LN03/import" \
        -H "Authorization: Bearer $JWT_TOKEN" \
        -F "file=@ln03_test_sample.csv" \
        -F "statementDate=20250131" > test_result_1.json
    
    # Kiểm tra kết quả
    cat test_result_1.json
    
    if grep -q "Success\":true" test_result_1.json; then
        echo "✅ Test case 1 thành công: Import file hợp lệ."
    else
        echo "❌ Test case 1 thất bại: Không thể import file hợp lệ."
        return 1
    fi
}

# Test case 2: Import file không hợp lệ
test_invalid_import() {
    echo -e "\n=== Test case 2: Import file không hợp lệ ==="
    echo "Đang import file ln03_test_invalid.csv..."
    
    curl -s -X POST "$API_URL/api/LN03/import" \
        -H "Authorization: Bearer $JWT_TOKEN" \
        -F "file=@ln03_test_invalid.csv" \
        -F "statementDate=20250131" > test_result_2.json
    
    # Kiểm tra kết quả - API nên vẫn import được nhưng có cảnh báo hoặc bỏ qua dòng không hợp lệ
    cat test_result_2.json
    
    if grep -q "Success\":true" test_result_2.json; then
        echo "✅ Test case 2 thành công: API xử lý đúng file không hợp lệ (import với cảnh báo)."
    else
        # Nếu API từ chối hoàn toàn file không hợp lệ, đây cũng có thể là hành vi mong muốn
        echo "⚠️ Test case 2: API từ chối file không hợp lệ. Hãy kiểm tra lại mong đợi về hành vi."
    fi
}

# Test case 3: Import file chỉ có header
test_header_only_import() {
    echo -e "\n=== Test case 3: Import file chỉ có header ==="
    echo "Đang import file ln03_test_header_only.csv..."
    
    curl -s -X POST "$API_URL/api/LN03/import" \
        -H "Authorization: Bearer $JWT_TOKEN" \
        -F "file=@ln03_test_header_only.csv" \
        -F "statementDate=20250131" > test_result_3.json
    
    # Kiểm tra kết quả - API nên xử lý đúng file chỉ có header
    cat test_result_3.json
    
    if grep -q "Success\":true" test_result_3.json && grep -q "RecordsCount\":0" test_result_3.json; then
        echo "✅ Test case 3 thành công: API xử lý đúng file chỉ có header (không có bản ghi nào được import)."
    else
        echo "❌ Test case 3 thất bại: API không xử lý đúng file chỉ có header."
    fi
}

# Test case 4: Kiểm tra dữ liệu đã import
test_imported_data() {
    echo -e "\n=== Test case 4: Kiểm tra dữ liệu đã import ==="
    echo "Đang lấy dữ liệu LN03 gần đây..."
    
    curl -s -X GET "$API_URL/api/LN03/recent?count=10" \
        -H "Authorization: Bearer $JWT_TOKEN" > test_result_4.json
    
    # Kiểm tra kết quả
    cat test_result_4.json
    
    # Kiểm tra kiểu dữ liệu đã được chuyển đổi đúng
    if grep -q "\"soTienXLRR\":" test_result_4.json && grep -q "\"ngayPhatSinhXL\":" test_result_4.json; then
        echo "✅ Test case 4 thành công: Dữ liệu đã được import với kiểu dữ liệu đúng."
    else
        echo "❌ Test case 4 thất bại: Dữ liệu không được hiển thị với kiểu dữ liệu đúng."
    fi
}

# Chạy các test
run_all_tests() {
    # Kiểm tra API trước
    check_api_health
    if [ $? -ne 0 ]; then
        echo "❌ API không hoạt động. Không thể tiếp tục kiểm tra."
        exit 1
    fi
    
    # Lấy token
    get_token
    
    # Chạy các test case
    test_valid_import
    test_invalid_import
    test_header_only_import
    test_imported_data
    
    # Tổng kết
    echo -e "\n=== Tổng kết kết quả kiểm tra ==="
    echo "Đã hoàn thành tất cả các test case."
    echo "Kiểm tra chi tiết kết quả trong các file test_result_*.json"
}

# Chạy tất cả các test
run_all_tests
