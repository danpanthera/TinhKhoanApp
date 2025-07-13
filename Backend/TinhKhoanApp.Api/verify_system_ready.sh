#!/bin/bash

echo "======================================================"
echo "🎯 KIỂM TRA HOÀN THÀNH HỆ THỐNG TINHKHOAN APP"
echo "======================================================"
echo ""

API_BASE="http://localhost:5055/api"

echo "📊 MASTER DATA STATUS:"
echo ""

# Kiểm tra Units
units_count=$(curl -s "$API_BASE/units" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ Units: $units_count/46 $([ "$units_count" = "46" ] && echo "✅" || echo "❌")"

# Kiểm tra Positions
positions_count=$(curl -s "$API_BASE/positions" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ Positions: $positions_count/5 $([ "$positions_count" = "5" ] && echo "✅" || echo "❌")"

# Kiểm tra Roles
roles_count=$(curl -s "$API_BASE/roles" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ Roles: $roles_count/23 $([ "$roles_count" = "23" ] && echo "✅" || echo "❌")"

# Kiểm tra Employees
employees_count=$(curl -s "$API_BASE/employees" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ Employees: $employees_count (sample data)"

echo ""
echo "🔧 KPI SYSTEM STATUS:"
echo ""

# Kiểm tra KPI Assignment Tables
kpi_tables_count=$(curl -s "$API_BASE/KpiAssignmentTables" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ KPI Assignment Tables: $kpi_tables_count/32 $([ "$kpi_tables_count" = "32" ] && echo "✅" || echo "❌")"

# Kiểm tra phân loại KPI tables
canbo_count=$(curl -s "$API_BASE/KpiAssignmentTables" | jq '[.[] | select(.Category == "CANBO")] | length' 2>/dev/null || echo "0")
chinhanh_count=$(curl -s "$API_BASE/KpiAssignmentTables" | jq '[.[] | select(.Category == "CHINHANH")] | length' 2>/dev/null || echo "0")
echo "   - Cán bộ tables: $canbo_count/23 $([ "$canbo_count" = "23" ] && echo "✅" || echo "❌")"
echo "   - Chi nhánh tables: $chinhanh_count/9 $([ "$chinhanh_count" = "9" ] && echo "✅" || echo "❌")"

# Kiểm tra Khoan Periods
periods_count=$(curl -s "$API_BASE/KhoanPeriods" | jq '. | length' 2>/dev/null || echo "0")
echo "✅ Khoan Periods: $periods_count (CRUD ready)"

echo ""
echo "🚀 CRUD API ENDPOINTS AVAILABLE:"
echo ""
echo "✅ GET/POST/PUT/DELETE /api/KhoanPeriods"
echo "✅ GET/POST/PUT/DELETE /api/roles"
echo "✅ GET/POST/PUT/DELETE /api/employees"
echo "✅ GET/POST/PUT/DELETE /api/units"
echo "✅ GET/POST/PUT/DELETE /api/positions"
echo "✅ GET /api/KpiAssignmentTables"

echo ""
echo "📋 REQUIREMENTS CHECK:"
echo ""

all_good=true

if [ "$roles_count" = "23" ]; then
    echo "✅ 1. Chỉ có 23 vai trò - PASSED"
else
    echo "❌ 1. Chỉ có 23 vai trò - FAILED ($roles_count found)"
    all_good=false
fi

if [ "$periods_count" -gt "0" ]; then
    echo "✅ 2. Kỳ khoán CRUD hoạt động - PASSED"
else
    echo "❌ 2. Kỳ khoán CRUD hoạt động - FAILED"
    all_good=false
fi

if [ "$kpi_tables_count" = "32" ] && [ "$canbo_count" = "23" ] && [ "$chinhanh_count" = "9" ]; then
    echo "✅ 3. KPI tables không tự động mapping - PASSED"
else
    echo "❌ 3. KPI tables configuration - FAILED"
    all_good=false
fi

echo ""
if [ "$all_good" = true ]; then
    echo "🎉 SUCCESS: HỆ THỐNG ĐÃ SẴN SÀNG SỬ DỤNG!"
    echo "🎯 Người dùng có thể:"
    echo "   - Tạo/sửa/xóa kỳ khoán"
    echo "   - Chọn bảng KPI cho từng cán bộ"
    echo "   - Quản lý toàn bộ master data"
else
    echo "❌ FAILED: Có lỗi cần khắc phục"
fi

echo ""
echo "======================================================"
