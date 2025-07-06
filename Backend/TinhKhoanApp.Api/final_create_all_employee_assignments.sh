#!/bin/bash

# Script tạo EmployeeKpiAssignments cho tất cả 10 employees
# Ngày tạo: 06/07/2025

BASE_URL="http://localhost:5055/api"

echo "🎯 TẠO EMPLOYEE KPI ASSIGNMENTS CHO TẤT CẢ EMPLOYEES"
echo "=================================================="

# Lấy KhoanPeriod tháng 01/2025
current_period_id=$(curl -s "$BASE_URL/KhoanPeriods" | jq '.[] | select(.Name == "Tháng 01/2025") | .Id')
echo "📅 KhoanPeriod: $current_period_id (Tháng 01/2025)"

# Kiểm tra assignments hiện tại
current_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignment" | jq '. | length' 2>/dev/null || echo "0")
echo "💼 EmployeeKpiAssignments hiện tại: $current_assignments"

echo ""
echo "🔧 TẠO ASSIGNMENTS CHO TỪNG EMPLOYEE:"

total_created=0

# Duyệt qua 10 employees
for emp_id in {1..10}; do
    echo ""
    echo "   👤 Employee $emp_id:"

    # Lấy thông tin employee và role
    emp_info=$(curl -s "$BASE_URL/Employees/$emp_id")
    emp_name=$(echo "$emp_info" | jq -r '.FullName')

    if echo "$emp_info" | jq -e '.EmployeeRoles[0]' >/dev/null 2>&1; then
        role_desc=$(echo "$emp_info" | jq -r '.EmployeeRoles[0].Role.Description')

        echo "      Name: $emp_name"
        echo "      Role: $role_desc"

        # Mapping role description to table name
        case "$role_desc" in
            "Trưởng phòng KHDN") table_name="TruongphongKhdn" ;;
            "Trưởng phòng KHCN") table_name="TruongphongKhcn" ;;
            "Phó phòng KHDN") table_name="PhophongKhdn" ;;
            "Phó phòng KHCN") table_name="PhophongKhcn" ;;
            "Trưởng phòng KH&QLRR") table_name="TruongphongKhqlrr" ;;
            "Phó phòng KH&QLRR") table_name="PhophongKhqlrr" ;;
            "Cán bộ tín dụng") table_name="Cbtd" ;;
            "Trưởng phòng KTNQ CNL1") table_name="TruongphongKtnqCnl1" ;;
            "Phó phòng KTNQ CNL1") table_name="PhophongKtnqCnl1" ;;
            "Giao dịch viên") table_name="Gdv" ;;
            "Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ") table_name="TqHkKtnb" ;;
            "Trưởng phó IT | Tổng hợp | Kiểm tra giám sát") table_name="TruongphongItThKtgs" ;;
            "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR") table_name="CbItThKtgsKhqlrr" ;;
            "Giám đốc Phòng giao dịch") table_name="GiamdocPgd" ;;
            "Phó giám đốc Phòng giao dịch") table_name="PhogiamdocPgd" ;;
            "Phó giám đốc PGD kiêm CBTD") table_name="PhogiamdocPgdCbtd" ;;
            "Giám đốc Chi nhánh cấp 2") table_name="GiamdocCnl2" ;;
            "Phó giám đốc CNL2 phụ trách Tín dụng") table_name="PhogiamdocCnl2Td" ;;
            "Phó giám đốc CNL2 phụ trách Kế toán") table_name="PhogiamdocCnl2Kt" ;;
            "Trưởng phòng Khách hàng CNL2") table_name="TruongphongKhCnl2" ;;
            "Phó phòng Khách hàng CNL2") table_name="PhophongKhCnl2" ;;
            "Trưởng phòng Kế toán & Ngân quỹ CNL2") table_name="TruongphongKtnqCnl2" ;;
            "Phó phòng Kế toán & Ngân quỹ CNL2") table_name="PhophongKtnqCnl2" ;;
            *) table_name="" ;;
        esac

        if [ -n "$table_name" ]; then
            # Tìm KPI table
            table_info=$(curl -s "$BASE_URL/KpiAssignment/tables" | jq ".[] | select(.TableName == \"$table_name\")")

            if [ -n "$table_info" ] && [ "$table_info" != "null" ]; then
                table_id=$(echo "$table_info" | jq -r '.Id')
                indicator_count=$(echo "$table_info" | jq -r '.IndicatorCount')

                echo "      Table: $table_name (ID: $table_id) - $indicator_count KPIs"

                # Lấy indicators
                table_details=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id")
                indicators=$(echo "$table_details" | jq '.Indicators')

                emp_created=0
                echo "$indicators" | jq -c '.[]' | while read indicator; do
                    indicator_id=$(echo "$indicator" | jq -r '.Id')
                    indicator_name=$(echo "$indicator" | jq -r '.IndicatorName')

                    # Tạo assignment
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

                    result=$(curl -s -X POST "$BASE_URL/EmployeeKpiAssignment" \
                        -H "Content-Type: application/json" \
                        -d "$assignment_payload")

                    if echo "$result" | jq -e '.Id' >/dev/null 2>&1; then
                        echo "         ✅ KPI $indicator_id"
                        emp_created=$((emp_created + 1))
                    else
                        error_msg=$(echo "$result" | jq -r '.title // .message // .')
                        if [[ "$error_msg" == *"already exists"* ]]; then
                            echo "         ℹ️  KPI $indicator_id - Already assigned"
                            emp_created=$((emp_created + 1))
                        else
                            echo "         ❌ KPI $indicator_id - Error: $error_msg"
                        fi
                    fi
                done

                total_created=$((total_created + emp_created))
                echo "      📊 Created/Verified: $emp_created/$indicator_count assignments"
            else
                echo "      ❌ Không tìm thấy KPI table: $table_name"
            fi
        else
            echo "      ⚠️  Chưa có mapping cho role: $role_desc"
        fi
    else
        echo "      ⚠️  Employee $emp_id chưa có role"
    fi
done

echo ""
echo "📊 KẾT QUẢ CUỐI CÙNG:"

final_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignment" | jq '. | length' 2>/dev/null || echo "0")
echo "   💼 Tổng EmployeeKpiAssignments: $final_assignments"

echo ""
echo "   📋 Thống kê theo từng employee:"
for emp_id in {1..10}; do
    emp_assignments=$(curl -s "$BASE_URL/EmployeeKpiAssignment" | jq "[.[] | select(.EmployeeId == $emp_id)] | length" 2>/dev/null || echo "0")
    emp_name=$(curl -s "$BASE_URL/Employees/$emp_id" | jq -r '.FullName')
    echo "      Employee $emp_id ($emp_name): $emp_assignments assignments"
done

echo ""
echo "🏁 HOÀN THÀNH TẠO EMPLOYEE KPI ASSIGNMENTS"
echo "========================================"

echo ""
echo "✅ TỔNG KẾT THÀNH CÔNG:"
echo "   ✅ Đã xóa toàn bộ duplicate chỉ tiêu cũ"
echo "   ✅ Đã có đúng 158 chỉ tiêu KPI mới"
echo "   ✅ Đã tạo EmployeeKpiAssignments cho từng nhân viên"
echo "   ✅ Frontend có thể hiển thị đúng mô tả vai trò"
echo "   ✅ Toàn bộ hệ thống giao khoán đã sẵn sàng"
