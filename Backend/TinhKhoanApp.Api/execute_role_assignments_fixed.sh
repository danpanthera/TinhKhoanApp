#!/bin/bash

# Script thực thi gán Employees vào Roles cho hệ thống TinhKhoanApp
# Ngày cập nhật: 07/01/2025
# Mục đích: Thực hiện gán vai trò cụ thể cho từng nhân viên với payload đúng format

BASE_URL="http://localhost:5055/api"

echo "🎯 THỰC HIỆN GÁN ROLES CHO EMPLOYEES"
echo "==================================="

# 1. KIỂM TRA BACKEND
echo ""
echo "📊 1. KIỂM TRA BACKEND:"
health_check=$(curl -s "$BASE_URL/health" | jq -r '.status' 2>/dev/null || echo "")
if [ "$health_check" = "Healthy" ]; then
    echo "   ✅ Backend đang hoạt động bình thường"
else
    echo "   ❌ Backend không hoạt động. Vui lòng khởi động backend trước!"
    echo "   Debug: health_check = '$health_check'"
    exit 1
fi

# 2. FUNCTION ĐỂ GÁN ROLE CHO EMPLOYEE
assign_role_to_employee() {
    local employee_id=$1
    local role_id=$2
    local employee_name="$3"
    local role_name="$4"

    echo "   🔄 Gán role '$role_name' (ID: $role_id) cho '$employee_name' (ID: $employee_id)"

    # Lấy dữ liệu employee hiện tại
    current_data=$(curl -s "$BASE_URL/employees/$employee_id")
    if [ $? -ne 0 ]; then
        echo "      ❌ Không thể lấy dữ liệu employee $employee_id"
        return 1
    fi

    # Tạo payload với tất cả field cần thiết + RoleIds mới
    payload=$(echo "$current_data" | jq --argjson roleIds "[$role_id]" '{
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

    # Gửi request update
    response=$(curl -s -X PUT "$BASE_URL/employees/$employee_id" \
        -H "Content-Type: application/json" \
        -d "$payload")

    # Kiểm tra kết quả
    if echo "$response" | jq -e '.Roles | length > 0' >/dev/null 2>&1; then
        role_names=$(echo "$response" | jq -r '.Roles | map(.Name) | join(", ")')
        echo "      ✅ Thành công! Roles hiện tại: $role_names"
    else
        echo "      ❌ Lỗi khi gán role!"
        echo "      Response: $response"
    fi

    sleep 1  # Tránh quá tải API
}

# 3. MAPPING ROLES CHO TỪNG EMPLOYEE
echo ""
echo "🎭 3. BẮT ĐẦU GÁN ROLES THEO CHỨC VỤ:"
echo ""

# Mapping dựa trên phân tích trước đó:

# Employee ID 1: Quản Trị Viên Hệ Thống → IT/System Admin role
assign_role_to_employee 1 12 "Quản Trị Viên Hệ Thống" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 2: Nguyễn Văn An - Phó Giám đốc Hội Sở → Deputy Director role
assign_role_to_employee 2 18 "Nguyễn Văn An" "Phó giám đốc CNL2 phụ trách TD"

# Employee ID 3: Trần Thị Bình - Trưởng phòng Ban Giám đốc → Department Head role
assign_role_to_employee 3 12 "Trần Thị Bình" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 4: Lê Văn Cường - Trưởng phòng KHDN → Corporate Customer Head
assign_role_to_employee 4 1 "Lê Văn Cường" "Trưởng phòng KHDN"

# Employee ID 5: Phạm Thị Dung - Trưởng phòng KHCN → Individual Customer Head
assign_role_to_employee 5 2 "Phạm Thị Dung" "Trưởng phòng KHCN"

# Employee ID 6: Hoàng Văn Em - Trưởng phòng KTNQ → Accounting Head
assign_role_to_employee 6 8 "Hoàng Văn Em" "Trưởng phòng KTNQ CNL1"

# Employee ID 7: Ngô Thị Phương - Trưởng phòng Tổng hợp → General Affairs Head
assign_role_to_employee 7 12 "Ngô Thị Phương" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 8: Đinh Văn Giang - Trưởng phòng KH&QLRR → Planning & Risk Head
assign_role_to_employee 8 5 "Đinh Văn Giang" "Trưởng phòng KH&QLRR"

# Employee ID 9: Vừ A Seo - Phó Giám đốc KTGS → Deputy Inspection Head
assign_role_to_employee 9 12 "Vừ A Seo" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 10: Lò Văn Minh - Phó Trưởng phòng Chi nhánh → Deputy Branch Head
assign_role_to_employee 10 15 "Lò Văn Minh" "Phó giám đốc Phòng giao dịch"

# 4. KIỂM TRA KẾT QUẢ TỔNG THỂ
echo ""
echo "📊 4. KIỂM TRA KẾT QUẢ SAU KHI GÁN:"
echo ""

employees_data=$(curl -s "$BASE_URL/employees" | jq '.[] | select(.Roles | length > 0)')
employees_with_roles_count=$(echo "$employees_data" | jq -s '. | length')
total_employees=$(curl -s "$BASE_URL/employees" | jq '. | length')

echo "   👥 Tổng employees: $total_employees"
echo "   ✅ Employees đã có roles: $employees_with_roles_count"
echo ""
echo "   📋 Chi tiết employees có roles:"

echo "$employees_data" | jq -r '"   ID: \(.Id) | \(.FullName) (\(.PositionName)) → \(.Roles | map(.Name) | join(", "))"'

# 5. BACKUP DỮ LIỆU
echo ""
echo "💾 5. BACKUP DỮ LIỆU:"
timestamp=$(date +"%Y%m%d_%H%M%S")
backup_file="employees_with_roles_$timestamp.json"
curl -s "$BASE_URL/employees" > "$backup_file"
echo "   ✅ Đã backup employees vào: $backup_file"

echo ""
echo "🏁 HOÀN THÀNH GÁN ROLES"
echo "======================"

echo ""
echo "📈 THỐNG KÊ:"
echo "   🔢 Tổng employees: $total_employees"
echo "   ✅ Employees có roles: $employees_with_roles_count"
echo "   📊 Tỷ lệ hoàn thành: $(( employees_with_roles_count * 100 / total_employees ))%"

echo ""
echo "🚀 BƯỚC TIẾP THEO:"
echo "   1. ✅ Gán Roles cho Employees (hoàn thành)"
echo "   2. 🔄 Tạo EmployeeKpiAssignments dựa trên roles"
echo "   3. 🔄 Tạo UnitKpiScorings cho đánh giá chi nhánh"
echo "   4. 🔄 Thiết lập đồng bộ tự động"
