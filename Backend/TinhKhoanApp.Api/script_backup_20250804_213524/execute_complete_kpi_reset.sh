#!/bin/bash
set -e

echo "🔄 COMPLETE RESET & RECREATE EMPLOYEE KPI SYSTEM"
echo "================================================"
echo "Xóa toàn bộ và tạo lại 23 bảng KPI cán bộ với đầy đủ chỉ tiêu theo danh sách chuẩn"
echo ""

# Check if backend is running
echo "🏥 Kiểm tra trạng thái backend..."
if ! curl -s http://localhost:5055/health > /dev/null; then
    echo "❌ Backend không hoạt động! Vui lòng khởi động backend trước."
    exit 1
fi
echo "✅ Backend đang hoạt động"
echo ""

# Step 1: Create backup
echo "📋 BƯỚC 1: Tạo backup dữ liệu hiện tại..."
./reset_delete_employee_kpi_tables.sh 2>&1 | tee reset_log_$(date +%Y%m%d_%H%M%S).log
echo ""

# Step 2: Recreate 23 tables
echo "🏗️ BƯỚC 2: Tạo lại 23 bảng KPI cán bộ..."
./recreate_23_employee_kpi_tables.sh
echo ""

# Step 3: Populate indicators
echo "📝 BƯỚC 3: Populate toàn bộ chỉ tiêu KPI..."
./populate_all_kpi_indicators_new.sh
echo ""

# Final verification
echo "🔍 BƯỚC 4: Xác nhận kết quả cuối cùng..."
API_BASE="http://localhost:5055/api"

EMPLOYEE_TABLES=$(curl -s "$API_BASE/KpiAssignment/tables" | jq '[.[] | select(.Category == "CANBO")] | length')
TOTAL_INDICATORS=$(curl -s "$API_BASE/KpiAssignment/indicators" | jq 'length')
BRANCH_TABLES=$(curl -s "$API_BASE/KpiAssignment/tables" | jq '[.[] | select(.Category == "CHINHANH")] | length')

echo ""
echo "📊 KẾT QUẢ CUỐI CÙNG:"
echo "==================="
echo "✅ Bảng KPI cán bộ: $EMPLOYEE_TABLES/23"
echo "✅ Bảng KPI chi nhánh: $BRANCH_TABLES/9"
echo "✅ Tổng chỉ tiêu KPI: $TOTAL_INDICATORS"
echo "✅ Tổng bảng KPI: $(curl -s "$API_BASE/KpiAssignment/tables" | jq 'length')/32"

if [ "$EMPLOYEE_TABLES" = "23" ] && [ "$BRANCH_TABLES" = "9" ]; then
    echo ""
    echo "🎉 THÀNH CÔNG! Đã hoàn thành reset và tạo lại toàn bộ hệ thống KPI!"
    echo "🎯 Sẵn sàng cho bước tiếp theo: Tạo EmployeeKpiAssignments"
else
    echo ""
    echo "⚠️ CÓ VẤN ĐỀ! Vui lòng kiểm tra lại:"
    echo "   - Bảng KPI cán bộ: $EMPLOYEE_TABLES/23"
    echo "   - Bảng KPI chi nhánh: $BRANCH_TABLES/9"
fi

echo ""
echo "📋 Các script đã sử dụng:"
echo "  1. reset_delete_employee_kpi_tables.sh - Xóa dữ liệu cũ"
echo "  2. recreate_23_employee_kpi_tables.sh - Tạo lại 23 bảng"
echo "  3. populate_all_kpi_indicators_new.sh - Populate chỉ tiêu"
