#!/bin/bash

# Script tạo 10 sample employees cho test
# Ngày tạo: 12/07/2025
# Tác giả: Assistant

echo "👤 BẮT ĐẦU TẠO 10 SAMPLE EMPLOYEES"
echo "=================================="

BASE_URL="http://localhost:5055/api/employees"

# Hàm tạo employee với full data
create_employee() {
    local code="$1"
    local cbcode="$2"
    local fullname="$3"
    local username="$4"
    local email="$5"
    local phone="$6"
    local unitid="$7"
    local positionid="$8"
    local roleids="$9"

    echo "Đang tạo nhân viên: $fullname ($code)"

    response=$(curl -s -X POST "$BASE_URL" \
        -H "Content-Type: application/json" \
        -d "{
            \"employeeCode\": \"$code\",
            \"cbCode\": \"$cbcode\",
            \"fullName\": \"$fullname\",
            \"username\": \"$username\",
            \"email\": \"$email\",
            \"phoneNumber\": \"$phone\",
            \"isActive\": true,
            \"unitId\": $unitid,
            \"positionId\": $positionid,
            \"roleIds\": $roleids
        }")

    if echo "$response" | jq -e '.Id' > /dev/null 2>&1; then
        id=$(echo "$response" | jq -r '.Id')
        echo "✅ Tạo thành công - ID: $id"
    else
        echo "⚠️  Response: $response"
    fi

    sleep 0.5
}

echo ""
echo "📋 TẠO 10 SAMPLE EMPLOYEES:"

# 1. Giám đốc Lai Châu (Unit 1, Position cần tạo, Role [17])
create_employee "GD001" "123456001" "Nguyễn Văn An - Giám đốc CN Lai Châu" "giamdoc.laichau" "giamdoc@agribank.com.vn" "0987654321" "1" "1" "[17]"

# 2. Trưởng phòng KHDN (Unit 4, Role [1])
create_employee "TP001" "123456002" "Trần Thị Bình - TP KHDN" "tp.khdn" "tp.khdn@agribank.com.vn" "0987654322" "4" "2" "[1]"

# 3. Phó phòng KHDN (Unit 4, Role [3])
create_employee "PP001" "123456003" "Lê Văn Cường - PP KHDN" "pp.khdn" "pp.khdn@agribank.com.vn" "0987654323" "4" "3" "[3]"

# 4. Trưởng phòng KHCN (Unit 5, Role [2])
create_employee "TP002" "123456004" "Phạm Thị Dung - TP KHCN" "tp.khcn" "tp.khcn@agribank.com.vn" "0987654324" "5" "2" "[2]"

# 5. Cán bộ tín dụng (Unit 4, Role [7])
create_employee "CB001" "123456005" "Hoàng Văn Minh - CBTD" "cbtd.01" "cbtd.01@agribank.com.vn" "0987654325" "4" "4" "[7]"

# 6. Trưởng phòng KTNQ (Unit 6, Role [8])
create_employee "TP003" "123456006" "Vũ Thị Hoa - TP KTNQ" "tp.ktnq" "tp.ktnq@agribank.com.vn" "0987654326" "6" "2" "[8]"

# 7. Giao dịch viên (Unit 6, Role [10])
create_employee "GDV001" "123456007" "Đỗ Văn Tùng - GDV" "gdv.01" "gdv.01@agribank.com.vn" "0987654327" "6" "5" "[10]"

# 8. Trưởng phòng KH&QLRR (Unit 8, Role [5])
create_employee "TP004" "123456008" "Ngô Thị Lan - TP KH&QLRR" "tp.khqlrr" "tp.khqlrr@agribank.com.vn" "0987654328" "8" "2" "[5]"

# 9. Giám đốc CNL2 Bình Lư (Unit 10, Role [17])
create_employee "GD002" "123456009" "Bùi Văn Đức - GĐ CNL2 Bình Lư" "gd.binhlu" "gd.binhlu@agribank.com.vn" "0987654329" "10" "1" "[17]"

# 10. Phó Giám đốc CNL2 Phong Thổ (Unit 11, Role [18])
create_employee "PGD001" "123456010" "Đinh Thị Mai - PGĐ Phong Thổ" "pgd.phongtho" "pgd.phongtho@agribank.com.vn" "0987654330" "11" "3" "[18]"

echo ""
echo "🏁 HOÀN THÀNH TẠO 10 SAMPLE EMPLOYEES"
echo "==================================="

echo ""
echo "📊 KIỂM TRA KẾT QUẢ:"
employee_count=$(curl -s "http://localhost:5055/api/employees" | jq length 2>/dev/null || echo "Unable to count")
echo "Tổng số nhân viên đã tạo: $employee_count"

if [ "$employee_count" -ge 10 ]; then
    echo "✅ THÀNH CÔNG: Đã tạo đủ sample employees!"
else
    echo "⚠️  Cần kiểm tra lại: Số lượng employees không đúng"
fi
