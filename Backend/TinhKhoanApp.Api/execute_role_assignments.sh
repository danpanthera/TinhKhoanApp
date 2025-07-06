#!/bin/bash

# Script thực thi gán Employees vào Roles cho hệ thống TinhKhoanApp
# Ngày tạo: 07/01/2025
# Mục đích: Thực hiện gán vai trò cụ thể cho từng nhân viên dựa trên vị trí và đơn vị

BASE_URL="http://localhost:5055/api"

echo "🎯 BẮT ĐẦU GÁN ROLES CHO EMPLOYEES"
echo "================================="

# 1. KIỂM TRA BACKEND
echo ""
echo "📊 1. KIỂM TRA BACKEND:"
health_check=$(curl -s "$BASE_URL/health" | jq -r '.status' 2>/dev/null)
if [ "$health_check" = "Healthy" ]; then
    echo "   ✅ Backend đang hoạt động bình thường"
else
    echo "   ❌ Backend không hoạt động. Vui lòng khởi động backend trước!"
    exit 1
fi

# 2. FUNCTION ĐỂ GÁN ROLE CHO EMPLOYEE
assign_role_to_employee() {
    local employee_id=$1
    local role_id=$2
    local employee_name="$3"
    local role_name="$4"

    echo "   🔄 Gán role '$role_name' (ID: $role_id) cho '$employee_name' (ID: $employee_id)"

    # Tạo payload JSON với RoleIds array
    payload=$(jq -n --argjson roleIds "[$role_id]" '{RoleIds: $roleIds}')

    response=$(curl -s -X PUT "$BASE_URL/employees/$employee_id" \
        -H "Content-Type: application/json" \
        -d "$payload")

    if [ $? -eq 0 ]; then
        echo "      ✅ Thành công!"
    else
        echo "      ❌ Lỗi khi gán role!"
        echo "      Response: $response"
    fi

    sleep 1  # Tránh quá tải API
}

# 3. THỰC HIỆN GÁN ROLES CHO TỪNG EMPLOYEE
echo ""
echo "🎭 3. BẮT ĐẦU GÁN ROLES:"
echo ""

# Employee ID 1: Quản Trị Viên Hệ Thống - gán role IT
assign_role_to_employee 1 12 "Quản Trị Viên Hệ Thống" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 2: Nguyễn Văn An - Phó Giám đốc Hội Sở - gán role CNL1 Deputy
assign_role_to_employee 2 18 "Nguyễn Văn An" "Phó giám đốc CNL2 phụ trách TD"

# Employee ID 3: Trần Thị Bình - Trưởng phòng Ban Giám đốc - gán role leadership
assign_role_to_employee 3 12 "Trần Thị Bình" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 4: Lê Văn Cường - Trưởng phòng KHDN - gán role KHDN
assign_role_to_employee 4 1 "Lê Văn Cường" "Trưởng phòng KHDN"

# Employee ID 5: Phạm Thị Dung - Trưởng phòng KHCN - gán role KHCN
assign_role_to_employee 5 2 "Phạm Thị Dung" "Trưởng phòng KHCN"

# Employee ID 6: Hoàng Văn Em - Trưởng phòng KTNQ - gán role KTNQ CNL1
assign_role_to_employee 6 8 "Hoàng Văn Em" "Trưởng phòng KTNQ CNL1"

# Employee ID 7: Ngô Thị Phương - Trưởng phòng Tổng hợp - gán role Tổng hợp
assign_role_to_employee 7 12 "Ngô Thị Phương" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 8: Đinh Văn Giang - Trưởng phòng KH&QLRR - gán role KH&QLRR
assign_role_to_employee 8 5 "Đinh Văn Giang" "Trưởng phòng KH&QLRR"

# Employee ID 9: Vừ A Seo - Phó Giám đốc KTGS - gán role deputy KTGS
assign_role_to_employee 9 12 "Vừ A Seo" "Trưởng phó IT | Tổng hợp | KTGS"

# Employee ID 10: Lò Văn Minh - Phó Trưởng phòng Chi nhánh Bình Lư - gán role deputy PGD
assign_role_to_employee 10 15 "Lò Văn Minh" "Phó giám đốc Phòng giao dịch"

# 4. KIỂM TRA KẾT QUẢ
echo ""
echo "📊 4. KIỂM TRA KẾT QUẢ SAU KHI GÁN:"
echo ""

employees_with_roles=$(curl -s "$BASE_URL/employees" | jq '.[] | select(.Roles | length > 0)')
employees_count=$(echo "$employees_with_roles" | jq -s '. | length')

echo "   👥 Số employees đã có roles: $employees_count"
echo ""
echo "   📋 Chi tiết:"
echo "$employees_with_roles" | jq -r '"   \(.FullName) (\(.PositionName)) → Roles: \(.Roles | map(.Name) | join(", "))"'

# 5. THỐNG KÊ TỔNG QUAN
echo ""
echo "📈 5. THỐNG KÊ TỔNG QUAN:"
total_employees=$(curl -s "$BASE_URL/employees" | jq '. | length')
total_roles=$(curl -s "$BASE_URL/roles" | jq '. | length')
echo "   🔢 Tổng employees: $total_employees"
echo "   🎭 Tổng roles: $total_roles"
echo "   ✅ Employees đã có roles: $employees_count"

# 6. BACKUP KẾT QUẢ
echo ""
echo "💾 6. BACKUP DỮ LIỆU SAU GÁN ROLES:"
timestamp=$(date +"%Y%m%d_%H%M%S")
backup_file="employees_with_roles_backup_$timestamp.json"
curl -s "$BASE_URL/employees" > "$backup_file"
echo "   ✅ Đã backup vào: $backup_file"

echo ""
echo "🏁 HOÀN THÀNH GÁN ROLES CHO EMPLOYEES"
echo "===================================="

echo ""
echo "📝 BƯỚC TIẾP THEO:"
echo "   1. ✅ Gán Roles cho Employees (vừa hoàn thành)"
echo "   2. 🔄 Tạo EmployeeKpiAssignments dựa trên roles"
echo "   3. 🔄 Tạo UnitKpiScorings cho đánh giá chi nhánh"
echo "   4. 🔄 Thiết lập đồng bộ tự động giữa các module"
