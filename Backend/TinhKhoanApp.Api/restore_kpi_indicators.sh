#!/bin/bash

# ====================================
# KHÔI PHỤC KPI INDICATORS - RESTORE KPI INDICATORS FROM DEFINITIONS
# ====================================

echo "🔧 KHÔI PHỤC KPI INDICATORS TỪ DEFINITIONS..."
echo "============================================="

BASE_URL="http://localhost:5055/api"

echo "📊 Kiểm tra tình trạng hiện tại..."
CURRENT_INDICATORS=$(curl -s "${BASE_URL}/KpiIndicators" | jq '. | length')
CURRENT_TABLES=$(curl -s "${BASE_URL}/KpiAssignmentTables" | jq '. | length')
CURRENT_DEFINITIONS=$(curl -s "${BASE_URL}/KPIDefinitions" | jq '. | length')

echo "Số KPI Indicators hiện tại: $CURRENT_INDICATORS"
echo "Số KPI Assignment Tables: $CURRENT_TABLES"
echo "Số KPI Definitions: $CURRENT_DEFINITIONS"

if [ "$CURRENT_DEFINITIONS" -eq 0 ]; then
    echo "❌ Chưa có KPI Definitions. Vui lòng chạy restore KPI Definitions trước!"
    exit 1
fi

if [ "$CURRENT_TABLES" -eq 0 ]; then
    echo "❌ Chưa có KPI Assignment Tables. Vui lòng chạy seed KPI tables trước!"
    exit 1
fi

echo ""
echo "🚀 Gọi API để tạo indicators từ definitions..."

# Gọi API SeedKpi để tạo indicators
response=$(curl -s -w "%{http_code}" -X POST "${BASE_URL}/SeedKpi/create-indicators-from-definitions" -o /tmp/indicators_response.txt)
http_code="${response: -3}"

if [ "$http_code" -eq 200 ] || [ "$http_code" -eq 204 ]; then
    echo "✅ Tạo indicators từ definitions thành công!"
    if [ -f /tmp/indicators_response.txt ]; then
        cat /tmp/indicators_response.txt | jq .
    fi
else
    echo "❌ Lỗi khi tạo indicators (HTTP: $http_code)"
    if [ -f /tmp/indicators_response.txt ]; then
        echo "Response:"
        cat /tmp/indicators_response.txt
    fi
    echo ""
    echo "🔄 Thử phương pháp thủ công..."

    # Lấy danh sách tables và tạo indicators cho từng table
    echo "📋 Lấy danh sách KPI tables..."
    curl -s "${BASE_URL}/KpiAssignmentTables" | jq -r '.[] | "\(.Id):\(.TableName):\(.Category)"' > /tmp/kpi_tables.txt

    while IFS=':' read -r table_id table_name category; do
        echo "🔄 Xử lý bảng: $table_name (ID: $table_id)"

        # Tìm roleCode từ table name (bỏ suffix _KPI_Assignment)
        role_code=$(echo "$table_name" | sed 's/_KPI_Assignment$//')

        # Lấy definitions cho role này
        definitions=$(curl -s "${BASE_URL}/KPIDefinitions" | jq --arg role "$role_code" '[.[] | select(.roleCode == $role)]')
        definition_count=$(echo "$definitions" | jq '. | length')

        if [ "$definition_count" -gt 0 ]; then
            echo "   📋 Tìm thấy $definition_count definitions cho role: $role_code"

            # Tạo indicators cho từng definition
            echo "$definitions" | jq -c '.[]' | while read -r def; do
                name=$(echo "$def" | jq -r '.name')
                max_score=$(echo "$def" | jq -r '.maxScore')
                unit=$(echo "$def" | jq -r '.unit // "Điểm"')
                order_index=$(echo "$def" | jq -r '.orderIndex // 1')

                indicator_data=$(cat <<EOF
{
    "tableId": $table_id,
    "indicatorName": "$name",
    "maxScore": $max_score,
    "unit": "$unit",
    "orderIndex": $order_index,
    "valueType": 0,
    "isActive": true
}
EOF
)

                echo "      📝 Tạo indicator: $name ($max_score điểm)"
                response=$(curl -s -w "%{http_code}" -X POST "${BASE_URL}/KpiIndicators" \
                    -H "Content-Type: application/json" \
                    -d "$indicator_data" -o /dev/null)

                if [[ "${response: -3}" =~ ^(200|201)$ ]]; then
                    echo "         ✅ Thành công"
                else
                    echo "         ❌ Thất bại (${response: -3})"
                fi
            done
        else
            echo "   ⚠️ Không tìm thấy definitions cho role: $role_code"
        fi

    done < /tmp/kpi_tables.txt
fi

echo ""
echo "📊 Kiểm tra kết quả:"
echo "==================="

# Kiểm tra lại số lượng
NEW_INDICATORS=$(curl -s "${BASE_URL}/KpiIndicators" | jq '. | length')
echo "Tổng số KPI Indicators sau khi restore: $NEW_INDICATORS"

if [ "$NEW_INDICATORS" -gt 0 ]; then
    echo ""
    echo "📋 Thống kê indicators theo bảng:"
    echo "=================================="

    # Lấy summary từ API
    curl -s "${BASE_URL}/KpiIndicators/summary" | jq -r '
        "📊 Tổng quan:",
        "- Tổng indicators: \(.TotalIndicators)",
        "- Tổng tables: \(.TotalTables)",
        "",
        "📋 Phân loại:",
        (.CategoryBreakdown[] | "- \(.Category): \(.TotalIndicators) indicators trên \(.TotalTables) tables")
    '

    echo ""
    echo "🎯 Một số indicators mẫu:"
    curl -s "${BASE_URL}/KpiIndicators" | jq -r '.[:5] | .[] | "ID \(.Id): \(.IndicatorName) - \(.MaxScore) \(.Unit) (Table: \(.TableId))"'

    if [ "$NEW_INDICATORS" -gt 5 ]; then
        echo "... và $(($NEW_INDICATORS - 5)) indicators khác"
    fi
else
    echo "❌ Không có KPI Indicators nào được tạo"
fi

echo ""
echo "🎉 HOÀN THÀNH RESTORE KPI INDICATORS!"
echo "Hệ thống KPI đã có đầy đủ indicators cho tất cả các bảng."
