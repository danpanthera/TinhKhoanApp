#!/bin/bash
set -e

echo "🔄 RESET KPI EMPLOYEE TABLES - Xóa và tạo lại 23 bảng KPI cán bộ"
echo "=================================================="

API_BASE="http://localhost:5055/api"

# 1. Tạo backup dữ liệu hiện tại
echo "📋 1. Backup dữ liệu hiện tại..."
curl -s "$API_BASE/KpiAssignment/tables" > backup_kpi_tables_$(date +%Y%m%d_%H%M%S).json
curl -s "$API_BASE/KpiDefinition" > backup_kpi_definitions_$(date +%Y%m%d_%H%M%S).json

# 2. Lấy danh sách 23 bảng KPI cán bộ hiện tại
echo "📊 2. Lấy danh sách 23 bảng KPI cán bộ..."
EMPLOYEE_TABLES=$(curl -s "$API_BASE/KpiAssignment/tables" | jq -r '.[] | select(.Category == "CANBO") | .Id')

echo "Tìm thấy các bảng KPI cán bộ:"
for table_id in $EMPLOYEE_TABLES; do
    TABLE_INFO=$(curl -s "$API_BASE/KpiAssignment/tables" | jq -r ".[] | select(.Id == $table_id) | \"\(.Id): \(.TableName) - \(.Description)\"")
    echo "  $TABLE_INFO"
done

# 3. Xóa toàn bộ KpiIndicators thuộc bảng KPI cán bộ
echo "🗑️ 3. Xóa KpiIndicators thuộc các bảng KPI cán bộ..."
for table_id in $EMPLOYEE_TABLES; do
    TABLE_NAME=$(curl -s "$API_BASE/KpiAssignment/tables" | jq -r ".[] | select(.Id == $table_id) | .TableName")
    echo "  Xóa indicators của bảng: $TABLE_NAME"

    # Get indicators của bảng này
    INDICATORS=$(curl -s "$API_BASE/KpiAssignment/indicators/$TABLE_NAME" | jq -r '.[].Id // empty')

    # Xóa từng indicator
    for indicator_id in $INDICATORS; do
        echo "    Xóa indicator ID: $indicator_id"
        curl -s -X DELETE "$API_BASE/KpiAssignment/indicators/$indicator_id" || echo "    Lỗi xóa indicator $indicator_id"
    done
done

# 4. Xóa toàn bộ KpiDefinitions
echo "🗑️ 4. Xóa toàn bộ KpiDefinitions..."
KPI_DEFINITIONS=$(curl -s "$API_BASE/KpiDefinition" | jq -r '.[].Id // empty')
for def_id in $KPI_DEFINITIONS; do
    echo "  Xóa KpiDefinition ID: $def_id"
    curl -s -X DELETE "$API_BASE/KpiDefinition/$def_id" || echo "  Lỗi xóa definition $def_id"
done

# 5. Xóa 23 bảng KPI cán bộ
echo "🗑️ 5. Xóa 23 bảng KPI cán bộ..."
for table_id in $EMPLOYEE_TABLES; do
    TABLE_NAME=$(curl -s "$API_BASE/KpiAssignment/tables" | jq -r ".[] | select(.Id == $table_id) | .TableName")
    echo "  Xóa bảng: $TABLE_NAME (ID: $table_id)"
    curl -s -X DELETE "$API_BASE/KpiAssignment/tables/$table_id" || echo "  Lỗi xóa bảng $table_id"
done

echo "✅ Hoàn thành xóa dữ liệu cũ!"
echo ""
echo "📊 Kiểm tra trạng thái sau khi xóa:"
echo "KpiAssignmentTables còn lại: $(curl -s "$API_BASE/KpiAssignment/tables" | jq 'length') bảng"
echo "KpiDefinitions còn lại: $(curl -s "$API_BASE/KpiDefinition" | jq 'length') definitions"

echo ""
echo "🎯 Sẵn sàng tạo lại 23 bảng KPI cán bộ theo danh sách mới!"
