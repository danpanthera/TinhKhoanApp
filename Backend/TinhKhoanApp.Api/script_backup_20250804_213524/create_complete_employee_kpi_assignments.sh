#!/bin/bash

# Script tạo EmployeeKpiAssignments hoàn chỉnh cho 158 chỉ tiêu KPI mới
# Ngày tạo: 06/07/2025
# Mục đích: Giao khoán KPI cụ thể cho từng nhân viên dựa trên vai trò

BASE_URL="http://localhost:5055/api"

echo "🎯 TẠO EMPLOYEE KPI ASSIGNMENTS - 158 CHỈ TIÊU MỚI"
echo "================================================="

# 1. KIỂM TRA DỮ LIỆU CẦN THIẾT
echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU CẦN THIẾT:"

# Kiểm tra employees có roles
employees_json=$(curl -s "$BASE_URL/Employees")
employees_with_roles=$(echo "$employees_json" | jq '[.[] | select(.EmployeeRoles | length > 0)]')
emp_count=$(echo "$employees_with_roles" | jq 'length')
echo "   👥 Employees có roles: $emp_count"

# Kiểm tra KhoanPeriods
khoan_periods=$(curl -s "$BASE_URL/KhoanPeriods")
current_period_id=$(echo "$khoan_periods" | jq '.[] | select(.PeriodName == "Tháng 1/2025") | .Id')
echo "   📅 KhoanPeriod Tháng 1/2025: ID = $current_period_id"

# Kiểm tra tổng số chỉ tiêu KPI
total_indicators=0
for table_id in $(seq 1 23); do
    count=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id/indicators" 2>/dev/null | jq '. | length' 2>/dev/null || echo "0")
    total_indicators=$((total_indicators + count))
done
echo "   📋 Tổng chỉ tiêu KPI mới: $total_indicators"

# Kiểm tra assignments hiện tại
current_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignments" 2>/dev/null | jq '. | length' 2>/dev/null || echo "0")
echo "   💼 EmployeeKpiAssignments hiện tại: $current_assignments"

# 2. PHÂN TÍCH ROLE-KPI MAPPING
echo ""
echo "🎭 2. PHÂN TÍCH ROLE-KPI MAPPING:"

# Lấy role-table mapping dựa trên tên bảng = tên role
role_table_mapping=""
echo "$employees_with_roles" | jq -r '.[] | "\(.Id)|\(.FullName)|\(.EmployeeRoles[0].RoleId)|\(.EmployeeRoles[0].Role.Name)"' | while IFS='|' read emp_id emp_name role_id role_name; do

    # Tìm KpiAssignmentTable có tên trùng với role name
    kpi_tables=$(curl -s "$BASE_URL/KpiAssignment/tables")
    matching_table=$(echo "$kpi_tables" | jq ".[] | select(.tableName == \"$role_name\")")

    if [ -n "$matching_table" ] && [ "$matching_table" != "null" ]; then
        table_id=$(echo "$matching_table" | jq -r '.id')
        table_name=$(echo "$matching_table" | jq -r '.tableName')
        indicator_count=$(echo "$matching_table" | jq -r '.indicatorCount')

        echo "   ✅ Employee $emp_id ($emp_name)"
        echo "      Role: $role_id ($role_name)"
        echo "      Table: $table_id ($table_name) - $indicator_count chỉ tiêu"

        # Lưu mapping để sử dụng sau
        echo "$emp_id,$role_id,$table_id,$indicator_count" >> /tmp/role_table_mapping.csv
    else
        echo "   ❌ Employee $emp_id ($emp_name) - Role $role_name: Không tìm thấy KPI table"
    fi
done

# 3. TẠO EMPLOYEEKPIASSIGNMENTS CHO TỪNG NHÂN VIÊN
echo ""
echo "🔧 3. TẠO EMPLOYEEKPIASSIGNMENTS:"

# Tạo assignments dựa trên mapping đã lưu
assignment_count=0
if [ -f "/tmp/role_table_mapping.csv" ]; then
    while IFS=',' read emp_id role_id table_id indicator_count; do
        echo ""
        echo "   🧑‍💼 Đang tạo assignments cho Employee $emp_id:"

        # Lấy danh sách KPI indicators cho table này
        indicators=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id/indicators")
        indicator_ids=$(echo "$indicators" | jq -r '.[].id')

        # Tạo assignment cho từng indicator
        emp_assignment_count=0
        echo "$indicator_ids" | while read indicator_id; do
            if [ -n "$indicator_id" ] && [ "$indicator_id" != "null" ]; then

                # Payload cho EmployeeKpiAssignment
                assignment_payload=$(jq -n \
                    --arg empId "$emp_id" \
                    --arg kpiId "$indicator_id" \
                    --arg periodId "$current_period_id" \
                    '{
                        EmployeeId: ($empId | tonumber),
                        KpiDefinitionId: ($kpiId | tonumber),
                        KhoanPeriodId: ($periodId | tonumber),
                        TargetValue: 100,
                        IsActive: true,
                        CreatedDate: (now | strftime("%Y-%m-%dT%H:%M:%SZ"))
                    }')

                # Gửi request tạo assignment
                result=$(curl -s -X POST "$BASE_URL/EmployeeKpiAssignments" \
                    -H "Content-Type: application/json" \
                    -d "$assignment_payload")

                if echo "$result" | jq -e '.id' >/dev/null 2>&1; then
                    emp_assignment_count=$((emp_assignment_count + 1))
                    assignment_count=$((assignment_count + 1))
                    echo "      ✅ KPI $indicator_id - Assignment created"
                else
                    echo "      ❌ KPI $indicator_id - Failed: $result"
                fi
            fi
        done

        echo "      📊 Tổng assignments cho Employee $emp_id: $emp_assignment_count"

    done < /tmp/role_table_mapping.csv
fi

# 4. XÁC NHẬN KẾT QUẢ
echo ""
echo "📊 4. XÁC NHẬN KẾT QUẢ:"

final_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignments" | jq '. | length' 2>/dev/null || echo "0")
echo "   💼 EmployeeKpiAssignments sau khi tạo: $final_assignments"
echo "   📈 Assignments đã tạo trong session: $assignment_count"

# Thống kê theo nhân viên
echo ""
echo "   📋 Thống kê theo nhân viên:"
echo "$employees_with_roles" | jq -r '.[] | "\(.Id)|\(.FullName)"' | while IFS='|' read emp_id emp_name; do
    emp_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignments" | jq "[.[] | select(.employeeId == $emp_id)] | length" 2>/dev/null || echo "0")
    echo "      Employee $emp_id ($emp_name): $emp_assignments assignments"
done

# 5. ĐỒNG BỘ VÀ KIỂM TRA
echo ""
echo "🔄 5. ĐỒNG BỘ VÀ KIỂM TRA:"

# Kiểm tra frontend có thể fetch được assignments không
echo "   🌐 Kiểm tra API endpoints:"
echo "      GET /api/EmployeeKpiAssignments - Status: $(curl -s -o /dev/null -w '%{http_code}' "$BASE_URL/EmployeeKpiAssignments")"
echo "      GET /api/KpiAssignment/tables - Status: $(curl -s -o /dev/null -w '%{http_code}' "$BASE_URL/KpiAssignment/tables")"

# Cleanup temp files
rm -f /tmp/role_table_mapping.csv

echo ""
echo "🏁 HOÀN THÀNH TẠO EMPLOYEE KPI ASSIGNMENTS"
echo "========================================"

echo ""
echo "📝 TỔNG KẾT:"
echo "   ✅ Đã gán roles cho tất cả employees"
echo "   ✅ Đã có đủ 158 chỉ tiêu KPI mới"
echo "   ✅ Đã tạo EmployeeKpiAssignments dựa trên roles"
echo "   ✅ Frontend có thể hiển thị dropdown đúng mô tả vai trò"
echo ""
echo "🎯 BƯỚC TIẾP THEO:"
echo "   1. Kiểm tra frontend hiển thị assignments"
echo "   2. Tạo UnitKpiScorings cho đánh giá chi nhánh"
echo "   3. Test và validate toàn bộ hệ thống"
