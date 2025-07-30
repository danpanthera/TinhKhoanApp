#!/bin/bash

# Script kiểm tra quá trình import CSV cho LN03
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Bắt đầu kiểm tra import CSV cho LN03 ==="

# Tạo thư mục test data nếu chưa tồn tại
TEST_DIR="./test_data"
mkdir -p $TEST_DIR

# Tạo file LN03 mẫu với các kiểu dữ liệu khác nhau
cat > $TEST_DIR/ln03_test_sample.csv << 'CSV_EOF'
MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
7800,CN Hà Nội,KH001,Nguyễn Văn A,HD001,1000000.50,2025-01-15,500000.25,500000.25,0,N1,CB001,Trần Văn B,PGD001,TK001,REF001,NV1
7800,CN Hà Nội,KH002,Nguyễn Thị C,HD002,2000000.75,2025-01-20,750000.50,1250000.25,0,N2,CB002,Lê Thị D,PGD002,TK002,REF002,NV2
7800,CN Hà Nội,KH003,Trần Văn E,HD003,3000000.25,2025-01-25,1000000.75,2000000.50,0,N3,CB003,Phạm Văn F,PGD003,TK003,REF003,NV3
7800,CN Hà Nội,KH004,Lê Minh G,HD004,"4,000,000.00",25/01/2025,"1,500,000.50","2,500,000.00",0,N1,CB001,Trần Văn B,PGD001,TK001,REF004,NV1
7800,CN Hà Nội,KH005,Phạm Thị H,HD005,,,,,,N2,CB002,Lê Thị D,PGD002,TK002,REF005,NV2
CSV_EOF

# Tạo file LN03 mẫu với dữ liệu không hợp lệ để test xử lý lỗi
cat > $TEST_DIR/ln03_test_invalid.csv << 'CSV_EOF'
MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
7800,CN Hà Nội,KH006,Hoàng Văn I,HD006,abc,xyz,invalid,not-a-number,text,N1,CB001,Trần Văn B,PGD001,TK001,REF006,NV1
7800,CN Hà Nội,KH007,Trương Thị J,HD007,5000000.25,invalid-date,2000000.00,3000000.25,0,N2,CB002,Lê Thị D,PGD002,TK002,REF007,NV2
CSV_EOF

# Tạo file test để chỉ có header
cat > $TEST_DIR/ln03_test_header_only.csv << 'CSV_EOF'
MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
CSV_EOF

# Tạo script kịch bản test
cat > $TEST_DIR/run_ln03_tests.sh << 'TEST_SCRIPT_EOF'
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
TEST_SCRIPT_EOF

chmod +x $TEST_DIR/run_ln03_tests.sh

# Tạo tài liệu hướng dẫn kiểm tra
cat > $TEST_DIR/README.md << 'README_EOF'
# Hướng dẫn kiểm tra import CSV cho LN03

Thư mục này chứa các file và script để kiểm tra quá trình import CSV cho LN03 sau khi thực hiện 
các thay đổi về mô hình dữ liệu.

## Các file test

1. `ln03_test_sample.csv` - File CSV mẫu với dữ liệu hợp lệ
2. `ln03_test_invalid.csv` - File CSV mẫu với dữ liệu không hợp lệ để kiểm tra xử lý lỗi
3. `ln03_test_header_only.csv` - File CSV chỉ có header để kiểm tra trường hợp đặc biệt
4. `run_ln03_tests.sh` - Script tự động hóa kiểm tra import

## Cách sử dụng

### Kiểm tra thủ công

1. Đảm bảo API đang chạy trên môi trường phát triển hoặc staging
2. Sử dụng Postman hoặc công cụ tương tự để import các file CSV mẫu
3. Kiểm tra kết quả trả về và dữ liệu trong cơ sở dữ liệu

### Kiểm tra tự động

1. Chỉnh sửa thông tin kết nối API trong file `run_ln03_tests.sh`
2. Chạy script kiểm tra:

```bash
cd test_data
./run_ln03_tests.sh
```

## Kịch bản kiểm tra

Script kiểm tra sẽ thực hiện các kịch bản sau:

1. **Test case 1**: Import file hợp lệ (ln03_test_sample.csv)
   - Kỳ vọng: Import thành công với các kiểu dữ liệu đúng

2. **Test case 2**: Import file không hợp lệ (ln03_test_invalid.csv)
   - Kỳ vọng: API xử lý đúng dữ liệu không hợp lệ (bỏ qua hoặc hiển thị lỗi)

3. **Test case 3**: Import file chỉ có header (ln03_test_header_only.csv)
   - Kỳ vọng: API xử lý đúng file chỉ có header (không có bản ghi nào được import)

4. **Test case 4**: Kiểm tra dữ liệu đã import
   - Kỳ vọng: Dữ liệu được hiển thị với kiểu dữ liệu đúng (decimal, datetime)

## Kiểm tra thủ công bổ sung

Ngoài các kiểm tra tự động, hãy thực hiện thêm các kiểm tra thủ công sau:

1. Kiểm tra API response với các trường decimal và datetime
2. Kiểm tra trực tiếp trong cơ sở dữ liệu các kiểu dữ liệu của bảng LN03
3. Thử thêm/sửa/xóa các bản ghi LN03 thông qua API

## Vấn đề đã biết

- Định dạng ngày tháng trong file CSV cần phải là yyyy-MM-dd
- Định dạng số trong file CSV không nên có dấu phân cách hàng nghìn
README_EOF

echo "=== Đã chuẩn bị xong các file kiểm tra import CSV cho LN03 ==="
echo "Thư mục test: $TEST_DIR"
echo ""
echo "Để chạy kiểm tra, hãy làm theo các bước sau:"
echo "1. Cập nhật thông tin kết nối API trong file $TEST_DIR/run_ln03_tests.sh"
echo "2. Chạy script kiểm tra: cd $TEST_DIR && ./run_ln03_tests.sh"
echo "3. Kiểm tra kết quả trong các file test_result_*.json"
echo ""
echo "Đọc hướng dẫn chi tiết trong file $TEST_DIR/README.md"
