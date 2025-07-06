#!/bin/bash

# Test script tạo EmployeeKpiAssignments cho 1 employee
# Employee ID = 1 (Quản Trị Viên Hệ Thống)

BASE_URL="http://localhost:5055/api"

echo "🧪 TEST TẠO ASSIGNMENT CHO 1 EMPLOYEE"
echo "===================================="

# Employee 1 info
emp_id=1
role_desc="Trưởng phó IT | Tổng hợp | Kiểm tra giám sát"
table_name="TruongphongItThKtgs"
table_id=11

echo "👤 Employee: $emp_id"
echo "🎭 Role: $role_desc"
echo "📋 Table: $table_name (ID: $table_id)"

# Lấy KhoanPeriod
current_period_id=$(curl -s "$BASE_URL/KhoanPeriods" | jq '.[] | select(.Name == "Tháng 01/2025") | .Id')
echo "📅 KhoanPeriod: $current_period_id"

# Lấy indicators
table_details=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id")
indicators=$(echo "$table_details" | jq '.Indicators')
indicator_count=$(echo "$indicators" | jq 'length')

echo "📊 Indicators: $indicator_count"

# Hiển thị danh sách indicators
echo ""
echo "📋 Danh sách indicators:"
echo "$indicators" | jq -r '.[] | "   \(.Id): \(.IndicatorName)"'

# Tạo assignments
echo ""
echo "🔧 Tạo assignments:"
created_count=0

echo "$indicators" | jq -c '.[]' | while read indicator; do
    indicator_id=$(echo "$indicator" | jq -r '.Id')
    indicator_name=$(echo "$indicator" | jq -r '.IndicatorName')

    # Payload
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

    echo "   📝 Tạo assignment cho KPI $indicator_id..."

    # POST request
    result=$(curl -s -X POST "$BASE_URL/EmployeeKpiAssignment" \
        -H "Content-Type: application/json" \
        -d "$assignment_payload")

    if echo "$result" | jq -e '.Id' >/dev/null 2>&1; then
        assignment_id=$(echo "$result" | jq -r '.Id')
        echo "      ✅ Assignment ID: $assignment_id"
        created_count=$((created_count + 1))
    else
        echo "      ❌ Error: $(echo "$result" | jq -r '.title // .message // .')"
    fi
done

echo ""
echo "📊 Kết quả:"
echo "   Assignments đã tạo: $created_count/$indicator_count"

# Kiểm tra assignments của employee này
echo ""
echo "🔍 Kiểm tra assignments cho Employee $emp_id:"
emp_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignment" | jq "[.[] | select(.EmployeeId == $emp_id)]")
echo "$emp_assignments" | jq -r '.[] | "   Assignment \(.Id): KPI \(.KpiDefinitionId) - Target: \(.TargetValue)"'

echo ""
echo "✅ TEST HOÀN THÀNH"
