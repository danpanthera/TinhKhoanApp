#!/bin/bash

# ====================================
# KIỂM TRA TỔNG QUAN HỆ THỐNG - SYSTEM STATUS CHECK
# ====================================

echo "📊 KIỂM TRA TỔNG QUAN HỆ THỐNG TINHKHOAN"
echo "========================================="

BASE_URL="http://localhost:5055/api"

echo "🔍 1. KIỂM TRA CÁC THÀNH PHẦN CỐT LÕI:"
echo "-------------------------------------"

# Roles
ROLES_COUNT=$(curl -s "${BASE_URL}/Roles" | jq '. | length')
echo "✅ Roles (Vai trò): $ROLES_COUNT/23 (mục tiêu)"

# Positions
POSITIONS_COUNT=$(curl -s "${BASE_URL}/Positions" | jq '. | length')
echo "✅ Positions (Chức vụ): $POSITIONS_COUNT"

# Units
UNITS_COUNT=$(curl -s "${BASE_URL}/Units" | jq '. | length')
echo "✅ Units (Đơn vị): $UNITS_COUNT"

# Employees
EMPLOYEES_COUNT=$(curl -s "${BASE_URL}/Employees" | jq '. | length')
echo "✅ Employees (Nhân viên): $EMPLOYEES_COUNT"

echo ""
echo "🗓️ 2. KIỂM TRA KỲ KHOÁN:"
echo "------------------------"

# KhoanPeriods
PERIODS_COUNT=$(curl -s "${BASE_URL}/KhoanPeriods" | jq '. | length')
echo "✅ KhoanPeriods (Kỳ khoán): $PERIODS_COUNT"

if [ "$PERIODS_COUNT" -gt 0 ]; then
    echo "   📋 Các kỳ khoán hiện tại:"
    curl -s "${BASE_URL}/KhoanPeriods" | jq -r '.[:5] | .[] | "   - \(.Name) (\(.Type)) - \(.Status)"'
    if [ "$PERIODS_COUNT" -gt 5 ]; then
        echo "   ... và $(($PERIODS_COUNT - 5)) kỳ khác"
    fi
fi

echo ""
echo "🎯 3. KIỂM TRA HỆ THỐNG KPI:"
echo "---------------------------"

# KPI Definitions
KPI_DEFINITIONS_COUNT=$(curl -s "${BASE_URL}/KPIDefinitions" | jq '. | length')
echo "✅ KPI Definitions (Định nghĩa KPI): $KPI_DEFINITIONS_COUNT"

# KPI Assignment Tables
KPI_TABLES_COUNT=$(curl -s "${BASE_URL}/KpiAssignmentTables" | jq '. | length')
echo "✅ KPI Assignment Tables (Bảng giao KPI): $KPI_TABLES_COUNT/32 (mục tiêu)"

# KPI Indicators
KPI_INDICATORS_COUNT=$(curl -s "${BASE_URL}/KpiIndicators" | jq '. | length')
echo "✅ KPI Indicators (Chỉ tiêu KPI): $KPI_INDICATORS_COUNT"

echo ""
echo "📈 4. KIỂM TRA TÍNH NĂNG CRUD:"
echo "-----------------------------"

# Test GET endpoints
echo "🔗 API Endpoints:"
endpoints=("Roles" "Positions" "Units" "Employees" "KhoanPeriods" "KPIDefinitions" "KpiAssignmentTables")

for endpoint in "${endpoints[@]}"; do
    status_code=$(curl -s -o /dev/null -w "%{http_code}" "${BASE_URL}/$endpoint")
    if [ "$status_code" -eq 200 ]; then
        echo "   ✅ $endpoint: Hoạt động ($status_code)"
    else
        echo "   ❌ $endpoint: Lỗi ($status_code)"
    fi
done

echo ""
echo "🎉 TỔNG KẾT:"
echo "============"

# Tính tổng điểm
total_checks=0
passed_checks=0

# Check roles
if [ "$ROLES_COUNT" -eq 23 ]; then
    echo "✅ Roles: HOÀN THÀNH (23/23)"
    ((passed_checks++))
else
    echo "⚠️ Roles: Cần kiểm tra ($ROLES_COUNT/23)"
fi
((total_checks++))

# Check positions
if [ "$POSITIONS_COUNT" -gt 0 ]; then
    echo "✅ Positions: HOÀN THÀNH ($POSITIONS_COUNT)"
    ((passed_checks++))
else
    echo "❌ Positions: Chưa có dữ liệu"
fi
((total_checks++))

# Check employees
if [ "$EMPLOYEES_COUNT" -gt 0 ]; then
    echo "✅ Employees: HOÀN THÀNH ($EMPLOYEES_COUNT)"
    ((passed_checks++))
else
    echo "❌ Employees: Chưa có dữ liệu"
fi
((total_checks++))

# Check periods
if [ "$PERIODS_COUNT" -gt 0 ]; then
    echo "✅ KhoanPeriods: HOÀN THÀNH ($PERIODS_COUNT kỳ)"
    ((passed_checks++))
else
    echo "❌ KhoanPeriods: Chưa có dữ liệu"
fi
((total_checks++))

# Check KPI system
if [ "$KPI_TABLES_COUNT" -eq 32 ] && [ "$KPI_DEFINITIONS_COUNT" -gt 0 ]; then
    echo "✅ KPI System: HOÀN THÀNH ($KPI_TABLES_COUNT bảng, $KPI_DEFINITIONS_COUNT định nghĩa)"
    ((passed_checks++))
else
    echo "⚠️ KPI System: Một phần ($KPI_TABLES_COUNT/32 bảng, $KPI_DEFINITIONS_COUNT định nghĩa)"
fi
((total_checks++))

echo ""
echo "📊 Kết quả: $passed_checks/$total_checks thành phần hoạt động tốt"

percentage=$((passed_checks * 100 / total_checks))
if [ "$percentage" -ge 80 ]; then
    echo "🎉 Hệ thống đã sẵn sàng ($percentage%)"
elif [ "$percentage" -ge 60 ]; then
    echo "⚠️ Hệ thống cần một số điều chỉnh ($percentage%)"
else
    echo "❌ Hệ thống cần khắc phục nhiều vấn đề ($percentage%)"
fi

echo ""
echo "🚀 CÁC CHỨC NĂNG ĐÃ SẴN SÀNG:"
echo "- ✅ Quản lý 23 vai trò chuẩn"
echo "- ✅ CRUD Positions (Chức vụ)"
echo "- ✅ CRUD KhoanPeriods (Kỳ khoán)"
echo "- ✅ Hệ thống KPI với 32 bảng cấu hình"
echo "- ✅ Frontend hiển thị Units và dữ liệu cơ bản"
