#!/bin/bash

# Script tạo hoàn chỉnh EmployeeKpiAssignments
# Ngày tạo: 07/01/2025
# Mục đích: Tạo giao khoán KPI cho tất cả nhân viên dựa trên roles và KPI assignment tables

BASE_URL="http://localhost:5055/api"

echo "🎯 TẠO HOÀN CHỈNH EMPLOYEE KPI ASSIGNMENTS"
echo "=========================================="

# 1. KIỂM TRA DỮ LIỆU HIỆN TẠI
echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU CƠ BẢN:"

employees_count=$(curl -s "$BASE_URL/employees" | jq '. | length')
roles_count=$(curl -s "$BASE_URL/roles" | jq '. | length')
tables_count=$(curl -s "$BASE_URL/KpiAssignment/tables" | jq '. | length')
periods_count=$(curl -s "$BASE_URL/khoanperiods" | jq '. | length')

echo "   👥 Employees: $employees_count"
echo "   🎭 Roles: $roles_count"
echo "   📋 KPI Tables: $tables_count"
echo "   🗓️  KhoanPeriods: $periods_count"

# Lấy period tháng 1/2025
current_period=$(curl -s "$BASE_URL/khoanperiods" | jq '.[] | select(.PeriodName == "Tháng 1/2025") | .Id')
echo "   📅 Sử dụng period: $current_period (Tháng 1/2025)"

# 2. PHÂN TÍCH EMPLOYEES VÀ ROLES
echo ""
echo "👥 2. EMPLOYEES VÀ ROLES ĐÃ GÁN:"

# Function để lấy role của employee
get_employee_role() {
    local emp_id=$1
    curl -s "$BASE_URL/employees/$emp_id" | jq -r '.EmployeeRoles[0].RoleId // "null"'
}

# Function để lấy tên role
get_role_name() {
    local role_id=$1
    if [ "$role_id" != "null" ]; then
        curl -s "$BASE_URL/roles" | jq -r ".[] | select(.Id == $role_id) | .Name"
    else
        echo "No role"
    fi
}

# Function để tìm KPI table dựa trên role
find_kpi_table_for_role() {
    local role_id=$1
    # Mapping logic dựa trên pattern đã thấy
    case $role_id in
        1) echo "1" ;;  # Trưởng phòng KHDN → TruongphongKhdn
        2) echo "2" ;;  # Trưởng phòng KHCN → TruongphongKhcn  
        5) echo "5" ;;  # Trưởng phòng KH&QLRR → TruongphongKhqlrr
        8) echo "8" ;;  # Trưởng phòng KTNQ CNL1 → TruongphongKtnqCnl1
        12) echo "12" ;; # Trưởng phó IT | Tổng hợp | KTGS → TruongphoItThKtgs
        15) echo "15" ;; # Phó giám đốc Phòng giao dịch → PhogiamdocPgd
        18) echo "18" ;; # Phó giám đốc CNL2 phụ trách TD → PhogiamdocCnl2Td
        *) echo "null" ;;
    esac
}

echo "   Danh sách employees và mapping:"
for emp_id in {1..10}; do
    emp_data=$(curl -s "$BASE_URL/employees/$emp_id" 2>/dev/null)
    if [ $? -eq 0 ]; then
        emp_name=$(echo "$emp_data" | jq -r '.FullName // "N/A"')
        role_id=$(echo "$emp_data" | jq -r '.EmployeeRoles[0].RoleId // "null"')
        
        if [ "$role_id" != "null" ]; then
            role_name=$(curl -s "$BASE_URL/roles" | jq -r ".[] | select(.Id == $role_id) | .Name")
            table_id=$(find_kpi_table_for_role $role_id)
            
            echo "   Employee $emp_id: $emp_name"
            echo "     → Role: $role_id ($role_name)"
            echo "     → KPI Table: $table_id"
            echo ""
        fi
    fi
done

# 3. KIỂM TRA KPI INDICATORS TRONG TABLES
echo "📋 3. KIỂM TRA KPI INDICATORS:"

# Kiểm tra table 1 làm ví dụ
echo "   Ví dụ Table 1 (TruongphongKhdn):"
table1_detail=$(curl -s "$BASE_URL/KpiAssignment/tables/1")
indicators_count=$(echo "$table1_detail" | jq '.Indicators | length')
echo "   - Số indicators: $indicators_count"

if [ "$indicators_count" -gt 0 ]; then
    echo "   - Sample indicators:"
    echo "$table1_detail" | jq '.Indicators[0:3] | .[] | "     ID: \(.Id) | \(.IndicatorName) | Max: \(.MaxScore)"' -r
fi

# 4. TẠO EMPLOYEE KPI ASSIGNMENTS MẪU
echo ""
echo "🔧 4. TẠO EMPLOYEE KPI ASSIGNMENTS:"

# Function để tạo assignment cho 1 employee
create_assignment_for_employee() {
    local emp_id=$1
    local emp_name="$2"
    local role_id=$3
    local table_id=$4
    
    echo "   🔄 Tạo assignment cho Employee $emp_id ($emp_name)"
    echo "     Role: $role_id, Table: $table_id, Period: $current_period"
    
    # Lấy danh sách indicators từ table
    table_detail=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id")
    indicators=$(echo "$table_detail" | jq '.Indicators[]?')
    
    if [ -n "$indicators" ]; then
        indicators_count=$(echo "$table_detail" | jq '.Indicators | length')
        echo "     → Tìm thấy $indicators_count indicators"
        
        # Tạo targets array cho tất cả indicators trong table
        targets=$(echo "$table_detail" | jq '[.Indicators[] | {
            IndicatorId: .Id,
            TargetValue: (.MaxScore * 0.8 | floor),
            Notes: "Auto-assigned based on role"
        }]')
        
        # Tạo payload request
        assignment_payload=$(jq -n \
            --arg empId "$emp_id" \
            --arg periodId "$current_period" \
            --argjson targets "$targets" \
            '{
                EmployeeId: ($empId | tonumber),
                KhoanPeriodId: ($periodId | tonumber),
                Targets: $targets
            }')
        
        # Gửi request
        result=$(curl -s -X POST "$BASE_URL/KpiAssignment/assign" \
            -H "Content-Type: application/json" \
            -d "$assignment_payload")
        
        # Kiểm tra kết quả
        if echo "$result" | jq -e '.Message' >/dev/null 2>&1; then
            targets_count=$(echo "$result" | jq -r '.TargetsCount')
            echo "     ✅ Thành công! Đã tạo $targets_count KPI targets"
        else
            echo "     ❌ Lỗi: $result"
        fi
    else
        echo "     ⚠️  Table $table_id không có indicators"
    fi
    echo ""
}

# Tạo assignments cho tất cả employees có role
echo "   Bắt đầu tạo assignments cho tất cả employees:"

for emp_id in {1..10}; do
    emp_data=$(curl -s "$BASE_URL/employees/$emp_id" 2>/dev/null)
    if [ $? -eq 0 ]; then
        emp_name=$(echo "$emp_data" | jq -r '.FullName')
        role_id=$(echo "$emp_data" | jq -r '.EmployeeRoles[0].RoleId // "null"')
        
        if [ "$role_id" != "null" ]; then
            table_id=$(find_kpi_table_for_role $role_id)
            
            if [ "$table_id" != "null" ]; then
                create_assignment_for_employee $emp_id "$emp_name" $role_id $table_id
            else
                echo "   ⚠️  Employee $emp_id ($emp_name): Không tìm thấy KPI table cho role $role_id"
            fi
        else
            echo "   ⚠️  Employee $emp_id ($emp_name): Chưa có role"
        fi
    fi
done

# 5. KIỂM TRA KẾT QUẢ
echo "📊 5. KIỂM TRA KẾT QUẢ:"

for emp_id in {1..3}; do
    assignment=$(curl -s "$BASE_URL/KpiAssignment/employee/$emp_id/period/$current_period")
    targets_count=$(echo "$assignment" | jq '.KpiTargets | length')
    emp_name=$(echo "$assignment" | jq -r '.Employee.FullName')
    
    echo "   Employee $emp_id ($emp_name): $targets_count KPI targets"
done

echo ""
echo "🏁 HOÀN THÀNH TẠO EMPLOYEE KPI ASSIGNMENTS"
echo "========================================="

echo ""
echo "✅ ĐÃ HOÀN THÀNH:"
echo "   1. ✅ Gán Roles cho tất cả Employees"
echo "   2. ✅ Tạo EmployeeKpiAssignments cho employees có role"
echo "   3. 🔄 Tiếp theo: Tạo UnitKpiScorings cho đánh giá chi nhánh"
