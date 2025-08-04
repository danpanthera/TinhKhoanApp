#!/bin/bash

# Script populate KpiIndicators vào KpiAssignmentTables từ KpiDefinitions
# Ngày tạo: 06/01/2025
# Mục đích: Tạo quan hệ giữa KpiDefinitions và KpiAssignmentTables qua KpiIndicators

BASE_URL="http://localhost:5055/api"

echo "🎯 POPULATE KPI INDICATORS VÀO ASSIGNMENT TABLES"
echo "================================================"

# 1. KIỂM TRA DỮ LIỆU HIỆN TẠI
echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU HIỆN TẠI:"
kpi_definitions_count=$(curl -s "$BASE_URL/kpidefinitions" | jq '. | length')
kpi_tables_count=$(curl -s "$BASE_URL/KpiAssignment/tables" | jq '. | length')

echo "   📋 KPI Definitions: $kpi_definitions_count"
echo "   📊 KPI Assignment Tables: $kpi_tables_count"

# 2. PHÂN TÍCH MAPPING ROLE → KPI
echo ""
echo "🔍 2. PHÂN TÍCH MAPPING ROLE → KPI:"

# Function để map role name với table ID
get_table_id_for_role() {
    local role_prefix=$1
    case $role_prefix in
        "TruongphongKhdn") echo "1" ;;
        "TruongphongKhcn") echo "2" ;;
        "PhophongKhdn") echo "3" ;;
        "PhophongKhcn") echo "4" ;;
        "TruongphongKhqlrr") echo "5" ;;
        "PhophongKhqlrr") echo "6" ;;
        "Cbtd") echo "7" ;;
        "TruongphongKtnqCnl1") echo "8" ;;
        "PhophongKtnqCnl1") echo "9" ;;
        "Gdv") echo "10" ;;
        "TqHkKtnb") echo "11" ;;
        "TruongphoItThKtgs") echo "12" ;;
        "CbItThKtgsKhqlrr") echo "13" ;;
        "GiamdocPgd") echo "14" ;;
        "PhogiamdocPgd") echo "15" ;;
        "PhogiamdocPgdCbtd") echo "16" ;;
        "GiamdocCnl2") echo "17" ;;
        "PhogiamdocCnl2Td") echo "18" ;;
        "PhogiamdocCnl2Kt") echo "19" ;;
        "TruongphongKhCnl2") echo "20" ;;
        "PhophongKhCnl2") echo "21" ;;
        "TruongphongKtnqCnl2") echo "22" ;;
        "PhophongKtnqCnl2") echo "23" ;;
        *) echo "null" ;;
    esac
}

# 3. TẠO KPI INDICATORS
echo ""
echo "🔧 3. TẠO KPI INDICATORS:"

# Lấy tất cả KPI Definitions
kpi_definitions=$(curl -s "$BASE_URL/kpidefinitions")

# Function để tạo indicator cho 1 KPI
create_kpi_indicator() {
    local kpi_id=$1
    local kpi_code=$2
    local kpi_name="$3"
    local max_score=$4
    local table_id=$5

    echo "   🔄 Tạo indicator: KPI $kpi_id → Table $table_id"
    echo "     KpiCode: $kpi_code"
    echo "     KpiName: $kpi_name"
    echo "     MaxScore: $max_score"

    # Tạo payload cho KpiIndicator
    indicator_payload=$(jq -n \
        --arg tableId "$table_id" \
        --arg kpiId "$kpi_id" \
        --arg indicatorName "$kpi_name" \
        --arg maxScore "$max_score" \
        --arg unit "điểm" \
        '{
            TableId: ($tableId | tonumber),
            KpiDefinitionId: ($kpiId | tonumber),
            IndicatorName: $indicatorName,
            MaxScore: ($maxScore | tonumber),
            Unit: $unit,
            OrderIndex: 1,
            IsActive: true
        }')

    # Note: Cần endpoint để tạo KpiIndicator
    # Hiện tại chỉ hiển thị payload để test
    echo "     Payload: $indicator_payload"
    echo ""
}

# Duyệt qua từng KPI Definition và tạo indicator
echo "$kpi_definitions" | jq -c '.[]' | while read kpi; do
    kpi_id=$(echo "$kpi" | jq -r '.Id')
    kpi_code=$(echo "$kpi" | jq -r '.KpiCode')
    kpi_name=$(echo "$kpi" | jq -r '.KpiName')
    max_score=$(echo "$kpi" | jq -r '.MaxScore')

    # Lấy role prefix từ KpiCode
    role_prefix=$(echo "$kpi_code" | sed 's/_.*$//')

    # Tìm table ID tương ứng
    table_id=$(get_table_id_for_role "$role_prefix")

    if [ "$table_id" != "null" ]; then
        create_kpi_indicator "$kpi_id" "$kpi_code" "$kpi_name" "$max_score" "$table_id"
    else
        echo "   ⚠️  Không tìm thấy table cho role: $role_prefix (KPI: $kpi_code)"
    fi

    # Limit để test - chỉ làm 5 KPI đầu tiên
    if [ "$kpi_id" -gt "80" ]; then
        break
    fi
done

# 4. THỐNG KÊ KẾT QUẢ
echo ""
echo "📊 4. THỐNG KÊ MAPPING:"
echo "   📋 Đã phân tích $kpi_definitions_count KPI Definitions"
echo "   📊 Mapping với $kpi_tables_count Assignment Tables"

# 5. KẾT LUẬN
echo ""
echo "🏁 HOÀN THÀNH PHÂN TÍCH POPULATE KPI INDICATORS"
echo "=============================================="

echo ""
echo "📝 BƯỚC TIẾP THEO:"
echo "   1. 🔄 Cần tạo API endpoint POST /api/KpiAssignment/indicators"
echo "   2. 🔄 Populate tất cả 135 KPI Definitions vào tables"
echo "   3. 🔄 Verify indicators đã được tạo thành công"
echo "   4. 🔄 Test KPI assignment system với indicators"
