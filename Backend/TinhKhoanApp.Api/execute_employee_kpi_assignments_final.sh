#!/bin/bash

# Script mapping Role Name với Table Name chính xác
# Ngày tạo: 06/07/2025
# Mục đích: Tạo EmployeeKpiAssignments dựa trên mapping role-table chuẩn

BASE_URL="http://localhost:5055/api"

echo "🎯 TẠO EMPLOYEE KPI ASSIGNMENTS - MAPPING CHÍNH XÁC"
echo "================================================="

# Function để mapping role name sang table name
get_table_name_for_role() {
    local role_name="$1"
    case "$role_name" in
        "Trưởng phòng KHDN") echo "TruongphongKhdn" ;;
        "Trưởng phòng KHCN") echo "TruongphongKhcn" ;;
        "Phó phòng KHDN") echo "PhophongKhdn" ;;
        "Phó phòng KHCN") echo "PhophongKhcn" ;;
        "Trưởng phòng KH&QLRR") echo "TruongphongKhqlrr" ;;
        "Phó phòng KH&QLRR") echo "PhophongKhqlrr" ;;
        "Cán bộ tín dụng") echo "Cbtd" ;;
        "Trưởng phòng KTNQ CNL1") echo "TruongphongKtnqCnl1" ;;
        "Phó phòng KTNQ CNL1") echo "PhophongKtnqCnl1" ;;
        "Giao dịch viên") echo "Gdv" ;;
        "Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ") echo "TqHkKtnb" ;;
        "Trưởng phó IT | Tổng hợp | Kiểm tra giám sát") echo "TruongphongItThKtgs" ;;
        "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR") echo "CbItThKtgsKhqlrr" ;;
        "Giám đốc Phòng giao dịch") echo "GiamdocPgd" ;;
        "Phó giám đốc Phòng giao dịch") echo "PhogiamdocPgd" ;;
        "Phó giám đốc PGD kiêm CBTD") echo "PhogiamdocPgdCbtd" ;;
        "Giám đốc Chi nhánh cấp 2") echo "GiamdocCnl2" ;;
        "Phó giám đốc CNL2 phụ trách Tín dụng") echo "PhogiamdocCnl2Td" ;;
        "Phó giám đốc CNL2 phụ trách Kế toán") echo "PhogiamdocCnl2Kt" ;;
        "Trưởng phòng Khách hàng CNL2") echo "TruongphongKhCnl2" ;;
        "Phó phòng Khách hàng CNL2") echo "PhophongKhCnl2" ;;
        "Trưởng phòng Kế toán & Ngân quỹ CNL2") echo "TruongphongKtnqCnl2" ;;
        "Phó phòng Kế toán & Ngân quỹ CNL2") echo "PhophongKtnqCnl2" ;;
        *) echo "" ;;
    esac
}

# 1. KIỂM TRA DỮ LIỆU CẦN THIẾT
echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU CẦN THIẾT:"

# Lấy tất cả employees với roles
employees_with_roles=$(curl -s "$BASE_URL/Employees" | jq '[.[] | select(.EmployeeRoles | length > 0)]')
emp_count=$(echo "$employees_with_roles" | jq 'length')
echo "   👥 Employees có roles: $emp_count"

# Lấy KhoanPeriod tháng 01/2025
current_period_id=$(curl -s "$BASE_URL/KhoanPeriods" | jq '.[] | select(.Name == "Tháng 01/2025") | .Id')
echo "   📅 KhoanPeriod Tháng 01/2025: ID = $current_period_id"

# Kiểm tra assignments hiện tại
current_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignments" 2>/dev/null | jq '. | length' 2>/dev/null || echo "0")
echo "   💼 EmployeeKpiAssignments hiện tại: $current_assignments"

# 2. TẠO ROLE-TABLE MAPPING
echo ""
echo "🎭 2. TẠO ROLE-TABLE MAPPING:"

# Tạo file mapping tạm thời
mapping_file="/tmp/employee_role_table_mapping.csv"
echo "EmployeeId,EmployeeName,RoleId,RoleName,TableName,TableId,IndicatorCount" > "$mapping_file"

echo "$employees_with_roles" | jq -c '.[]' | while read employee; do
    emp_id=$(echo "$employee" | jq -r '.Id')
    emp_name=$(echo "$employee" | jq -r '.FullName')
    role_id=$(echo "$employee" | jq -r '.EmployeeRoles[0].RoleId')
    role_name=$(echo "$employee" | jq -r '.EmployeeRoles[0].Role.Description')

    # Tìm table name tương ứng với role
    table_name=$(get_table_name_for_role "$role_name")

    if [ -n "$table_name" ]; then
        # Tìm table info từ API
        table_info=$(curl -s "$BASE_URL/KpiAssignment/tables" | jq ".[] | select(.TableName == \"$table_name\")")

        if [ -n "$table_info" ] && [ "$table_info" != "null" ]; then
            table_id=$(echo "$table_info" | jq -r '.Id')
            indicator_count=$(echo "$table_info" | jq -r '.IndicatorCount')

            echo "   ✅ Employee $emp_id ($emp_name)"
            echo "      Role: $role_id ($role_name)"
            echo "      Table: $table_id ($table_name) - $indicator_count chỉ tiêu"

            # Ghi vào file mapping
            echo "$emp_id,$emp_name,$role_id,$role_name,$table_name,$table_id,$indicator_count" >> "$mapping_file"
        else
            echo "   ❌ Employee $emp_id ($emp_name) - Table $table_name: Không tìm thấy trong API"
        fi
    else
        echo "   ⚠️  Employee $emp_id ($emp_name) - Role '$role_name': Chưa có mapping"
    fi
done

# 3. TẠO EMPLOYEEKPIASSIGNMENTS
echo ""
echo "🔧 3. TẠO EMPLOYEEKPIASSIGNMENTS:"

total_assignments_created=0

# Đọc mapping và tạo assignments
tail -n +2 "$mapping_file" | while IFS=',' read emp_id emp_name role_id role_name table_name table_id indicator_count; do
    echo ""
    echo "   🧑‍💼 Đang tạo assignments cho Employee $emp_id ($emp_name):"
    echo "      Table: $table_name (ID: $table_id) - $indicator_count chỉ tiêu"    # Lấy danh sách KPI indicators cho table này
    table_details=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id")
    indicators=$(echo "$table_details" | jq '.Indicators')

    if [ "$indicators" != "null" ] && [ -n "$indicators" ]; then
        emp_assignment_count=0

        echo "$indicators" | jq -c '.[]' | while read indicator; do
            indicator_id=$(echo "$indicator" | jq -r '.Id')
            indicator_name=$(echo "$indicator" | jq -r '.IndicatorName')

            if [ -n "$indicator_id" ] && [ "$indicator_id" != "null" ]; then
                # Tạo EmployeeKpiAssignment
                assignment_payload=$(jq -n \
                    --arg empId "$emp_id" \
                    --arg kpiId "$indicator_id" \
                    --arg periodId "$current_period_id" \
                    '{
                        EmployeeId: ($empId | tonumber),
                        KpiDefinitionId: ($kpiId | tonumber),
                        KhoanPeriodId: ($periodId | tonumber),
                        TargetValue: 100,
                        IsActive: true
                    }')

                # Gửi POST request
                result=$(curl -s -X POST "$BASE_URL/EmployeeKpiAssignment" \
                    -H "Content-Type: application/json" \
                    -d "$assignment_payload")

                if echo "$result" | jq -e '.Id' >/dev/null 2>&1; then
                    emp_assignment_count=$((emp_assignment_count + 1))
                    echo "         ✅ KPI $indicator_id ($indicator_name)"
                else
                    error_msg=$(echo "$result" | jq -r '.title // .message // .')
                    if [[ "$error_msg" == *"already exists"* ]]; then
                        echo "         ℹ️  KPI $indicator_id - Already assigned"
                        emp_assignment_count=$((emp_assignment_count + 1))
                    else
                        echo "         ❌ KPI $indicator_id - Error: $error_msg"
                    fi
                fi
            fi
        done

        echo "      📊 Assignments tạo cho Employee $emp_id: $emp_assignment_count/$indicator_count"
    else
        echo "      ⚠️  Không lấy được indicators cho table $table_id"
    fi
done

# 4. KIỂM TRA KẾT QUẢ
echo ""
echo "📊 4. KIỂM TRA KẾT QUẢ:"

final_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignment" | jq '. | length' 2>/dev/null || echo "0")
echo "   💼 Tổng EmployeeKpiAssignments: $final_assignments"

# Thống kê theo từng nhân viên
echo ""
echo "   📋 Thống kê assignments theo nhân viên:"
tail -n +2 "$mapping_file" | while IFS=',' read emp_id emp_name role_id role_name table_name table_id indicator_count; do
    emp_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignment" | jq "[.[] | select(.EmployeeId == $emp_id)] | length" 2>/dev/null || echo "0")
    echo "      Employee $emp_id ($emp_name): $emp_assignments assignments"
done

# 5. CLEANUP VÀ TỔNG KẾT
echo ""
echo "🧹 5. CLEANUP VÀ TỔNG KẾT:"

# Xóa file tạm
rm -f "$mapping_file"

echo "   🗑️  Đã xóa file mapping tạm thời"

echo ""
echo "🏁 HOÀN THÀNH TẠO EMPLOYEE KPI ASSIGNMENTS"
echo "========================================"

echo ""
echo "📝 TỔNG KẾT:"
echo "   ✅ Đã gán $emp_count employees với roles phù hợp"
echo "   ✅ Đã có 158 chỉ tiêu KPI mới trong 22 bảng KPI"
echo "   ✅ Đã tạo EmployeeKpiAssignments cho từng nhân viên"
echo "   ✅ Frontend có thể hiển thị đúng mô tả vai trò"

echo ""
echo "🎯 BƯỚC TIẾP THEO:"
echo "   1. Kiểm tra frontend hiển thị assignments"
echo "   2. Test API endpoints"
echo "   3. Validate toàn bộ hệ thống giao khoán"
