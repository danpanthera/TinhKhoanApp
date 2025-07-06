#!/bin/bash

# Script tạo EmployeeKpiAssignments dựa trên roles đã gán
# Ngày tạo: 07/01/2025
# Mục đích: Tạo assignments KPI cho từng nhân viên dựa trên vai trò của họ

BASE_URL="http://localhost:5055/api"

echo "🎯 TẠO EMPLOYEE KPI ASSIGNMENTS"
echo "==============================="

# 1. KIỂM TRA DỮ LIỆU HIỆN TẠI
echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU HIỆN TẠI:"

echo "   👥 Employees có roles:"
employees_with_roles=$(curl -s "$BASE_URL/employees" | jq '.[] | select(.EmployeeRoles | length > 0)')
echo "$employees_with_roles" | jq -r '"   - Employee \(.Id): \(.FullName) → Role \(.EmployeeRoles[0].RoleId) (\(.EmployeeRoles[0].Role.Name))"'

echo ""
echo "   🏢 Kiểm tra KhoanPeriods:"
khoan_periods_count=$(curl -s "$BASE_URL/khoanperiods" | jq '. | length')
echo "   - Tổng KhoanPeriods: $khoan_periods_count"

echo ""
echo "   📋 Kiểm tra KpiAssignmentTables:"
kpi_tables_count=$(curl -s "$BASE_URL/kpiassignmenttables" | jq '. | length')
echo "   - Tổng KpiAssignmentTables: $kpi_tables_count"

echo ""
echo "   📋 Kiểm tra KpiDefinitions:"
kpi_definitions_count=$(curl -s "$BASE_URL/kpidefinitions" | jq '. | length')
echo "   - Tổng KpiDefinitions: $kpi_definitions_count"

# 2. KIỂM TRA EMPLOYEEKPIASSIGNMENTS HIỆN TẠI
echo ""
echo "📊 2. KIỂM TRA EMPLOYEEKPIASSIGNMENTS HIỆN TẠI:"
existing_assignments=$(curl -s "$BASE_URL/employeekpiassignments" 2>/dev/null | jq '. | length' 2>/dev/null || echo "0")
echo "   - Hiện có: $existing_assignments assignments"

# 3. PHÂN TÍCH ROLE-KPI MAPPING
echo ""
echo "🎭 3. PHÂN TÍCH ROLE-KPI MAPPING:"
echo "   Dựa trên KpiAssignmentTables và roles đã gán:"

# Lấy danh sách KpiAssignmentTables
echo "   📋 KpiAssignmentTables theo Role:"
curl -s "$BASE_URL/kpiassignmenttables" | jq -r '.[] | "   - Table \(.Id): \(.TableName) | Role: \(.RoleId) | KPIs: \(.NumberOfKpis // "N/A")"' | head -10

# 4. ĐỀ XUẤT TẠO EMPLOYEEKPIASSIGNMENTS
echo ""
echo "💡 4. ĐỀ XUẤT TẠO EMPLOYEEKPIASSIGNMENTS:"
echo "   Logic: Mỗi Employee với Role cụ thể sẽ được gán KPIs từ KpiAssignmentTable tương ứng"
echo ""

# Lấy KhoanPeriod hiện tại (tháng 1/2025)
current_period=$(curl -s "$BASE_URL/khoanperiods" | jq '.[] | select(.PeriodName == "Tháng 1/2025") | .Id' | head -1)
echo "   🗓️  Sử dụng KhoanPeriod hiện tại: $current_period (Tháng 1/2025)"

echo ""
echo "   📝 Mapping dự kiến:"

# Duyệt qua từng employee có role và đề xuất assignment
echo "$employees_with_roles" | jq -r '.Id' | while read emp_id; do
    emp_data=$(echo "$employees_with_roles" | jq ". | select(.Id == $emp_id)")
    emp_name=$(echo "$emp_data" | jq -r '.FullName')
    role_id=$(echo "$emp_data" | jq -r '.EmployeeRoles[0].RoleId')
    role_name=$(echo "$emp_data" | jq -r '.EmployeeRoles[0].Role.Name')

    # Tìm KpiAssignmentTable cho role này
    kpi_table=$(curl -s "$BASE_URL/kpiassignmenttables" | jq ".[] | select(.RoleId == $role_id)" | head -1)

    if [ -n "$kpi_table" ]; then
        table_id=$(echo "$kpi_table" | jq -r '.Id')
        table_name=$(echo "$kpi_table" | jq -r '.TableName')
        num_kpis=$(echo "$kpi_table" | jq -r '.NumberOfKpis // "N/A"')

        echo "   → Employee $emp_id ($emp_name)"
        echo "     Role: $role_id ($role_name)"
        echo "     KpiTable: $table_id ($table_name) - $num_kpis KPIs"
        echo "     Assignment: EmployeeId=$emp_id, KpiAssignmentTableId=$table_id, KhoanPeriodId=$current_period"
        echo ""
    else
        echo "   → Employee $emp_id ($emp_name) - Role $role_id: ⚠️  Không tìm thấy KpiAssignmentTable"
        echo ""
    fi
done

# 5. TẠO EMPLOYEEKPIASSIGNMENTS MẪU
echo ""
echo "🔧 5. TẠO EMPLOYEEKPIASSIGNMENTS MẪU:"
echo "   (Sẽ tạo assignments cho từng employee dựa trên role và KpiAssignmentTable)"

# Tạo assignment cho employee đầu tiên để test
first_employee=$(echo "$employees_with_roles" | jq -r '.Id' | head -1)
if [ -n "$first_employee" ]; then
    emp_data=$(echo "$employees_with_roles" | jq ". | select(.Id == $first_employee)")
    role_id=$(echo "$emp_data" | jq -r '.EmployeeRoles[0].RoleId')

    # Tìm KpiAssignmentTable cho role này
    kpi_table_id=$(curl -s "$BASE_URL/kpiassignmenttables" | jq -r ".[] | select(.RoleId == $role_id) | .Id" | head -1)

    if [ -n "$kpi_table_id" ] && [ "$kpi_table_id" != "null" ]; then
        echo "   🧪 Test tạo assignment cho Employee $first_employee:"

        assignment_payload=$(jq -n \
            --arg empId "$first_employee" \
            --arg tableId "$kpi_table_id" \
            --arg periodId "$current_period" \
            '{
                EmployeeId: ($empId | tonumber),
                KpiAssignmentTableId: ($tableId | tonumber),
                KhoanPeriodId: ($periodId | tonumber),
                IsActive: true,
                CreatedDate: (now | strftime("%Y-%m-%dT%H:%M:%SZ"))
            }')

        echo "   Payload: $assignment_payload"

        # Gửi request tạo assignment
        result=$(curl -s -X POST "$BASE_URL/employeekpiassignments" \
            -H "Content-Type: application/json" \
            -d "$assignment_payload")

        echo "   Result: $result"

        # Kiểm tra kết quả
        new_count=$(curl -s "$BASE_URL/employeekpiassignments" | jq '. | length' 2>/dev/null || echo "0")
        echo "   ✅ EmployeeKpiAssignments sau khi tạo: $new_count"
    else
        echo "   ⚠️  Không tìm thấy KpiAssignmentTable cho role $role_id"
    fi
fi

echo ""
echo "🏁 HOÀN THÀNH PHÂN TÍCH VÀ TEST"
echo "==============================="

echo ""
echo "📝 BƯỚC TIẾP THEO:"
echo "   1. ✅ Đã gán Roles cho tất cả Employees"
echo "   2. 🔄 Tạo EmployeeKpiAssignments cho tất cả employees (đang thực hiện)"
echo "   3. 🔄 Tạo UnitKpiScorings cho đánh giá chi nhánh"
echo "   4. 🔄 Thiết lập đồng bộ tự động"
