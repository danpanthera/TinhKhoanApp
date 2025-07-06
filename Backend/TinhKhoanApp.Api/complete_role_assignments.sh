#!/bin/bash

echo "🎯 TIẾP TỤC GÁN ROLES CHO CÁC EMPLOYEES CÒN LẠI"
echo "============================================="

BASE_URL="http://localhost:5055/api"

# Function để gán role
assign_role() {
    local emp_id=$1
    local role_id=$2
    local emp_name="$3"
    local role_name="$4"

    echo "🔄 Gán role $role_id ($role_name) cho employee $emp_id ($emp_name)"

    # Lấy dữ liệu employee hiện tại
    emp_data=$(curl -s "$BASE_URL/employees/$emp_id")

    # Tạo payload đầy đủ
    payload=$(echo "$emp_data" | jq --argjson roleIds "[$role_id]" '{
        Id: .Id,
        EmployeeCode: .EmployeeCode,
        CBCode: .CBCode,
        FullName: .FullName,
        Username: .Username,
        Email: .Email,
        PhoneNumber: .PhoneNumber,
        IsActive: .IsActive,
        UnitId: .UnitId,
        PositionId: .PositionId,
        RoleIds: $roleIds
    }')

    # Gửi request
    result=$(curl -s -X PUT "$BASE_URL/employees/$emp_id" \
        -H "Content-Type: application/json" \
        -d "$payload")

    # Kiểm tra kết quả
    role_count=$(echo "$result" | jq '.EmployeeRoles | length' 2>/dev/null || echo "0")
    if [ "$role_count" -gt "0" ]; then
        echo "   ✅ Thành công! Employee có $role_count role(s)"
    else
        echo "   ❌ Có lỗi xảy ra"
        echo "   Response: $result"
    fi
    echo ""
}

# Kiểm tra employees đã có roles
echo "📋 TRẠNG THÁI HIỆN TẠI:"
curl -s "$BASE_URL/employees" | jq -r '.[] | "Employee \(.Id): \(.FullName) - Roles: \(.EmployeeRoles | length)"'
echo ""

# Gán roles cho các employees còn lại:

# Employee 3: Trần Thị Bình - Trưởng phòng Ban Giám đốc → Role 12 (IT/Tổng hợp)
assign_role 3 12 "Trần Thị Bình" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee 4: Lê Văn Cường - Trưởng phòng KHDN → Role 1 (Trưởng phòng KHDN)
assign_role 4 1 "Lê Văn Cường" "Trưởng phòng KHDN"

# Employee 5: Phạm Thị Dung - Trưởng phòng KHCN → Role 2 (Trưởng phòng KHCN)
assign_role 5 2 "Phạm Thị Dung" "Trưởng phòng KHCN"

# Employee 6: Hoàng Văn Em - Trưởng phòng KTNQ → Role 8 (Trưởng phòng KTNQ CNL1)
assign_role 6 8 "Hoàng Văn Em" "Trưởng phòng KTNQ CNL1"

# Employee 7: Ngô Thị Phương - Trưởng phòng Tổng hợp → Role 12 (IT/Tổng hợp)
assign_role 7 12 "Ngô Thị Phương" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee 8: Đinh Văn Giang - Trưởng phòng KH&QLRR → Role 5 (Trưởng phòng KH&QLRR)
assign_role 8 5 "Đinh Văn Giang" "Trưởng phòng KH&QLRR"

# Employee 9: Vừ A Seo - Phó Giám đốc KTGS → Role 12 (IT/Tổng hợp)
assign_role 9 12 "Vừ A Seo" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee 10: Lò Văn Minh - Phó Trưởng phòng → Role 15 (Phó giám đốc PGD)
assign_role 10 15 "Lò Văn Minh" "Phó giám đốc Phòng giao dịch"

echo "🏁 HOÀN THÀNH GÁN ROLES CHO TẤT CẢ EMPLOYEES"
echo "==========================================="

# Kiểm tra kết quả cuối cùng
echo "📊 KẾT QUẢ CUỐI CÙNG:"
curl -s "$BASE_URL/employees" | jq -r '.[] | "Employee \(.Id): \(.FullName) (\(.PositionName)) - \(.EmployeeRoles | length) role(s)"'

echo ""
echo "✅ Tất cả employees đã được gán roles!"
echo "🔄 Bước tiếp theo: Tạo EmployeeKpiAssignments dựa trên roles đã gán"
