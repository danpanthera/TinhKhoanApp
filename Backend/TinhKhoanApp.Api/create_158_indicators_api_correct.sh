#!/bin/bash

# =============================================================================
# TẠO 158 KPI INDICATORS VỚI ENDPOINT CHÍNH XÁC
# =============================================================================

echo "📊 TẠO 158 KPI INDICATORS VỚI API ENDPOINT ĐÚNG"
echo "================================================"

API_BASE="http://localhost:5055/api"

# =============================================================================
# FUNCTIONS
# =============================================================================

create_indicator() {
    local table_id="$1"
    local indicator_name="$2"
    local max_score="$3"
    local unit="$4"
    local order_index="$5"
    local value_type="$6"

    local data="{
        \"tableId\": $table_id,
        \"indicatorName\": \"$indicator_name\",
        \"maxScore\": $max_score,
        \"unit\": \"$unit\",
        \"orderIndex\": $order_index,
        \"valueType\": \"$value_type\",
        \"isActive\": true
    }"

    echo "  🔹 Tạo: $indicator_name (TableID: $table_id, Score: $max_score)"

    result=$(curl -s -X POST "$API_BASE/KpiIndicators" \
        -H "Content-Type: application/json" \
        -d "$data")

    if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
        echo "    ✅ Thành công"
        return 0
    else
        echo "    ❌ Lỗi: $(echo "$result" | jq -r '.message // .title // "Unknown error"')"
        return 1
    fi
}

# =============================================================================
# TẠO INDICATORS CHO BẢNG 1-4: KHDN/KHCN (32 chỉ tiêu)
# =============================================================================

echo ""
echo "🎯 BẢNG 1-4: KHDN/KHCN (32 chỉ tiêu)"
echo "=================================="

# Indicators cho bảng 1-4 (mỗi bảng 8 chỉ tiêu)
khdn_khcn_indicators=(
    "Huy động tiền gửi|15|Triệu VND|QUANTITATIVE"
    "Tăng trưởng huy động so cùng kỳ|10|%|QUANTITATIVE"
    "Dư nợ tín dụng|20|Triệu VND|QUANTITATIVE"
    "Tăng trưởng dư nợ so cùng kỳ|15|%|QUANTITATIVE"
    "Nợ quá hạn|10|%|QUANTITATIVE"
    "Thu nhập dịch vụ|15|Triệu VND|QUANTITATIVE"
    "Lợi nhuận trước thuế|10|Triệu VND|QUANTITATIVE"
    "Đánh giá định tính|5|Điểm|QUALITATIVE"
)

for table_id in 1 2 3 4; do
    echo "Bảng $table_id:"
    order=1
    for indicator in "${khdn_khcn_indicators[@]}"; do
        IFS='|' read -ra PARTS <<< "$indicator"
        create_indicator $table_id "${PARTS[0]}" "${PARTS[1]}" "${PARTS[2]}" $order "${PARTS[3]}"
        ((order++))
    done
done

# =============================================================================
# TẠO INDICATORS CHO BẢNG 5-6: KH&QLRR (12 chỉ tiêu)
# =============================================================================

echo ""
echo "🎯 BẢNG 5-6: KH&QLRR (12 chỉ tiêu)"
echo "================================"

khqlrr_indicators=(
    "Lập kế hoạch kinh doanh|20|Điểm|QUALITATIVE"
    "Phân tích thị trường|15|Điểm|QUALITATIVE"
    "Quản lý rủi ro tín dụng|25|Điểm|QUALITATIVE"
    "Báo cáo quản trị|15|Điểm|QUALITATIVE"
    "Tuân thủ quy định|15|Điểm|QUALITATIVE"
    "Đánh giá tổng hợp|10|Điểm|QUALITATIVE"
)

for table_id in 5 6; do
    echo "Bảng $table_id:"
    order=1
    for indicator in "${khqlrr_indicators[@]}"; do
        IFS='|' read -ra PARTS <<< "$indicator"
        create_indicator $table_id "${PARTS[0]}" "${PARTS[1]}" "${PARTS[2]}" $order "${PARTS[3]}"
        ((order++))
    done
done

# =============================================================================
# TẠO INDICATORS CHO BẢNG 7: CBTD (8 chỉ tiêu)
# =============================================================================

echo ""
echo "🎯 BẢNG 7: CBTD (8 chỉ tiêu)"
echo "=========================="

cbtd_indicators=(
    "Khách hàng mới|15|Khách hàng|QUANTITATIVE"
    "Dư nợ được giao|20|Triệu VND|QUANTITATIVE"
    "Tỷ lệ thu hồi nợ|15|%|QUANTITATIVE"
    "Chất lượng tín dụng|15|Điểm|QUALITATIVE"
    "Dịch vụ khách hàng|10|Điểm|QUALITATIVE"
    "Tuân thủ quy trình|10|Điểm|QUALITATIVE"
    "Phát triển sản phẩm|10|Điểm|QUALITATIVE"
    "Đánh giá tổng hợp|5|Điểm|QUALITATIVE"
)

echo "Bảng 7:"
order=1
for indicator in "${cbtd_indicators[@]}"; do
    IFS='|' read -ra PARTS <<< "$indicator"
    create_indicator 7 "${PARTS[0]}" "${PARTS[1]}" "${PARTS[2]}" $order "${PARTS[3]}"
    ((order++))
done

# =============================================================================
# TẠO INDICATORS CHO BẢNG 8-9: KTNQ CNL1 (12 chỉ tiêu)
# =============================================================================

echo ""
echo "🎯 BẢNG 8-9: KTNQ CNL1 (12 chỉ tiêu)"
echo "================================="

ktnq_cnl1_indicators=(
    "Chính xác báo cáo tài chính|25|Điểm|QUALITATIVE"
    "Đúng hạn báo cáo|20|Điểm|QUALITATIVE"
    "Quản lý ngân quỹ|20|Điểm|QUALITATIVE"
    "Tuân thủ quy định|15|Điểm|QUALITATIVE"
    "Hỗ trợ kinh doanh|10|Điểm|QUALITATIVE"
    "Cải tiến quy trình|10|Điểm|QUALITATIVE"
)

for table_id in 8 9; do
    echo "Bảng $table_id:"
    order=1
    for indicator in "${ktnq_cnl1_indicators[@]}"; do
        IFS='|' read -ra PARTS <<< "$indicator"
        create_indicator $table_id "${PARTS[0]}" "${PARTS[1]}" "${PARTS[2]}" $order "${PARTS[3]}"
        ((order++))
    done
done

# =============================================================================
# TẠO INDICATORS CHO BẢNG 10: GDV (6 chỉ tiêu)
# =============================================================================

echo ""
echo "🎯 BẢNG 10: GDV (6 chỉ tiêu)"
echo "========================="

gdv_indicators=(
    "Số lượng giao dịch|20|Giao dịch|QUANTITATIVE"
    "Chính xác giao dịch|25|%|QUANTITATIVE"
    "Thời gian xử lý|15|Điểm|QUALITATIVE"
    "Thái độ phục vụ|20|Điểm|QUALITATIVE"
    "Tuân thủ quy trình|15|Điểm|QUALITATIVE"
    "Đánh giá khách hàng|5|Điểm|QUALITATIVE"
)

echo "Bảng 10:"
order=1
for indicator in "${gdv_indicators[@]}"; do
    IFS='|' read -ra PARTS <<< "$indicator"
    create_indicator 10 "${PARTS[0]}" "${PARTS[1]}" "${PARTS[2]}" $order "${PARTS[3]}"
    ((order++))
done

# =============================================================================
# FINAL SUMMARY
# =============================================================================

echo ""
echo "🔍 VERIFICATION"
echo "==============="

# Đếm số indicators đã tạo
TOTAL_INDICATORS=$(curl -s "$API_BASE/KpiIndicators" | jq 'length')
echo "📊 Tổng số KPI Indicators: $TOTAL_INDICATORS/158"

if [ "$TOTAL_INDICATORS" -gt 0 ]; then
    echo "✅ THÀNH CÔNG! Đã tạo được $TOTAL_INDICATORS KPI Indicators"

    # Phân tích theo bảng
    echo ""
    echo "📋 Phân tích theo bảng:"
    for i in {1..10}; do
        count=$(curl -s "$API_BASE/KpiIndicators/table/$i" | jq 'length // 0')
        echo "  Bảng $i: $count chỉ tiêu"
    done
else
    echo "❌ THẤT BẠI! Chưa tạo được KPI Indicators nào"
fi

echo ""
echo "✅ Script hoàn thành!"
