#!/bin/bash

# Script thực thi populate KpiIndicators vào KpiAssignmentTables
# Ngày tạo: 06/01/2025
# Mục đích: Tạo hoàn chỉnh quan hệ KpiDefinitions → KpiIndicators → KpiAssignmentTables

BASE_URL="http://localhost:5055/api"

echo "🚀 THỰC THI POPULATE KPI INDICATORS"
echo "==================================="

# 1. KIỂM TRA DỮ LIỆU HIỆN TẠI
echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU HIỆN TẠI:"
kpi_definitions_count=$(curl -s "$BASE_URL/kpidefinitions" | jq '. | length')
kpi_tables_count=$(curl -s "$BASE_URL/KpiAssignment/tables" | jq '. | length')

echo "   📋 KPI Definitions: $kpi_definitions_count"
echo "   📊 KPI Assignment Tables: $kpi_tables_count"

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
        "TruongphoItThKtgs" | "TruongphongItThKtgs") echo "12" ;;
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

# 2. KIỂM TRA SỐ INDICATORS HIỆN TẠI
echo ""
echo "🔍 2. KIỂM TRA INDICATORS HIỆN TẠI:"
table1_detail=$(curl -s "$BASE_URL/KpiAssignment/tables/1")
current_indicators=$(echo "$table1_detail" | jq '.Indicators | length')
echo "   Table 1 hiện có: $current_indicators indicators"

# 3. TẠO KPI INDICATORS
echo ""
echo "🔧 3. BẮT ĐẦU TẠO KPI INDICATORS:"

# Counters
success_count=0
error_count=0
total_processed=0

# Function để tạo indicator cho 1 KPI
create_kpi_indicator() {
    local kpi_id=$1
    local kpi_code=$2
    local kpi_name="$3"
    local max_score=$4
    local table_id=$5

    echo "   🔄 Tạo: KPI $kpi_id → Table $table_id ($kpi_code)"

    # Tạo payload cho KpiIndicator
    indicator_payload=$(jq -n \
        --arg tableId "$table_id" \
        --arg indicatorName "$kpi_name" \
        --arg maxScore "$max_score" \
        --arg unit "điểm" \
        '{
            TableId: ($tableId | tonumber),
            IndicatorName: $indicatorName,
            MaxScore: ($maxScore | tonumber),
            Unit: $unit,
            ValueTypeString: "NUMBER"
        }')

    # Gửi request tạo indicator
    response=$(curl -s -X POST "$BASE_URL/KpiAssignment/indicators" \
        -H "Content-Type: application/json" \
        -d "$indicator_payload")

    # Kiểm tra kết quả
    if echo "$response" | jq -e '.success' >/dev/null 2>&1; then
        echo "     ✅ Thành công!"
        success_count=$((success_count + 1))
    else
        echo "     ❌ Lỗi: $(echo "$response" | jq -r '.message // .title // "Unknown error"')"
        error_count=$((error_count + 1))
    fi

    total_processed=$((total_processed + 1))
}

# Lấy tất cả KPI Definitions và process
echo "   Bắt đầu process tất cả KPI Definitions..."

kpi_definitions=$(curl -s "$BASE_URL/kpidefinitions")
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
        echo "   ⚠️  Bỏ qua KPI $kpi_id: Không tìm thấy table cho role '$role_prefix'"
        error_count=$((error_count + 1))
        total_processed=$((total_processed + 1))
    fi

    # Pause để tránh quá tải API
    sleep 0.5
done

# 4. KIỂM TRA KẾT QUẢ
echo ""
echo "📊 4. KIỂM TRA KẾT QUẢ SAU KHI TẠO:"

# Kiểm tra vài table đầu tiên
for table_id in {1..5}; do
    table_detail=$(curl -s "$BASE_URL/KpiAssignment/tables/$table_id")
    table_name=$(echo "$table_detail" | jq -r '.TableName')
    indicators_count=$(echo "$table_detail" | jq '.Indicators | length')
    echo "   Table $table_id ($table_name): $indicators_count indicators"
done

# 5. THỐNG KÊ TỔNG KẾT
echo ""
echo "🏁 HOÀN THÀNH POPULATE KPI INDICATORS"
echo "===================================="

echo ""
echo "📈 THỐNG KÊ:"
echo "   📋 Tổng KPI Definitions: $kpi_definitions_count"
echo "   ✅ Thành công: $success_count"
echo "   ❌ Lỗi: $error_count"
echo "   📊 Tổng processed: $total_processed"

if [ $success_count -gt 0 ]; then
    echo ""
    echo "✅ ĐÃ HOÀN THÀNH:"
    echo "   1. ✅ Populate KpiIndicators vào Assignment Tables"
    echo "   2. 🔄 Sẵn sàng cho Employee KPI Assignments"
    echo "   3. 🔄 Sẵn sàng cho Unit KPI Scorings"
fi

echo ""
echo "🔄 BƯỚC TIẾP THEO:"
echo "   1. ✅ Populate KpiIndicators (hoàn thành)"
echo "   2. 🔄 Tạo UnitKpiScorings cho đánh giá chi nhánh"
echo "   3. 🔄 Thiết lập auto-sync mechanisms"
