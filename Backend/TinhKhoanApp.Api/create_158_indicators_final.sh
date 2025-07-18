#!/bin/bash

# Script để tạo đúng 158 KPI Indicators cho 23 bảng cán bộ
# Theo README_DAT.md: 23 bảng KPI dành cho cán bộ đều có chỉ tiêu cụ thể đi kèm

API_BASE="http://localhost:5055/api"

echo "🎯 Tạo 158 KPI Indicators cho 23 bảng cán bộ"

# Xóa tất cả indicators cũ
echo "🧹 Xóa indicators cũ..."
curl -s -X DELETE "$API_BASE/KpiIndicators/clear" > /dev/null

# Định nghĩa chỉ tiêu theo từng ID bảng
create_indicators_for_table() {
    local table_id=$1
    local table_name="$2"
    local count=$3

    echo "📊 Tạo $count chỉ tiêu cho bảng ID:$table_id - $table_name..."

    # Định nghĩa tên chỉ tiêu chuẩn
    local indicators=(
        "Tỷ lệ hoàn thành kế hoạch thu nhập (%)"
        "Tỷ lệ hoàn thành kế hoạch tăng trưởng dư nợ tín dụng (%)"
        "Tỷ lệ nợ xấu (%)"
        "Số lượng khách hàng mới phát triển"
        "Tỷ lệ hoàn thành kế hoạch huy động vốn (%)"
        "Hiệu quả hoạt động kinh doanh tổng hợp"
        "Chất lượng dịch vụ khách hàng"
        "Tuân thủ quy định pháp luật và nội bộ"
        "Phát triển sản phẩm dịch vụ mới"
        "Quản lý rủi ro tín dụng"
        "Hiệu quả quản lý chi phí"
    )

    local created=0
    for i in $(seq 1 $count); do
        local indicator_name="${indicators[$((i-1))]}"
        if [ -z "$indicator_name" ]; then
            indicator_name="Chỉ tiêu $i cho $table_name"
        fi

        # Tạo indicator với đúng cấu trúc API cần
        local response=$(curl -s -X POST "$API_BASE/KpiIndicators" \
            -H "Content-Type: application/json" \
            -d "{
                \"TableId\": $table_id,
                \"IndicatorName\": \"$indicator_name\",
                \"MaxScore\": 100,
                \"Unit\": \"%\",
                \"OrderIndex\": $i,
                \"ValueType\": 0,
                \"IsActive\": true
            }")

        # Kiểm tra response
        if echo "$response" | jq -e '.Id' > /dev/null 2>&1; then
            ((created++))
        else
            echo "⚠️  Lỗi tạo indicator $i cho bảng $table_id: $response"
        fi
    done

    echo "✅ Đã tạo $created/$count chỉ tiêu cho ID:$table_id - $table_name"
    return $created
}

# Tạo indicators cho từng bảng theo đúng phân bố README
total_created=0

# ID 1: Trưởng phòng KHDN - 8 chỉ tiêu
create_indicators_for_table 1 "Trưởng phòng KHDN" 8
total_created=$((total_created + $?))

# ID 2: Trưởng phòng KHCN - 8 chỉ tiêu
create_indicators_for_table 2 "Trưởng phòng KHCN" 8
total_created=$((total_created + $?))

# ID 3: Phó phòng KHDN - 8 chỉ tiêu
create_indicators_for_table 3 "Phó phòng KHDN" 8
total_created=$((total_created + $?))

# ID 4: Phó phòng KHCN - 8 chỉ tiêu
create_indicators_for_table 4 "Phó phòng KHCN" 8
total_created=$((total_created + $?))

# ID 5: Trưởng phòng KH&QLRR - 6 chỉ tiêu
create_indicators_for_table 5 "Trưởng phòng KH&QLRR" 6
total_created=$((total_created + $?))

# ID 6: Phó phòng KH&QLRR - 6 chỉ tiêu
create_indicators_for_table 6 "Phó phòng KH&QLRR" 6
total_created=$((total_created + $?))

# ID 7: Cán bộ tín dụng - 6 chỉ tiêu
create_indicators_for_table 7 "Cán bộ tín dụng" 6
total_created=$((total_created + $?))

# ID 8: Trưởng phòng KTNQ CNL1 - 6 chỉ tiêu
create_indicators_for_table 8 "Trưởng phòng KTNQ CNL1" 6
total_created=$((total_created + $?))

# ID 9: Phó phòng KTNQ CNL1 - 6 chỉ tiêu
create_indicators_for_table 9 "Phó phòng KTNQ CNL1" 6
total_created=$((total_created + $?))

# ID 10: GDV - 8 chỉ tiêu
create_indicators_for_table 10 "GDV" 8
total_created=$((total_created + $?))

# ID 11: Thủ quỹ | Hậu kiểm | KTNB - 5 chỉ tiêu
create_indicators_for_table 11 "Thủ quỹ | Hậu kiểm | KTNB" 5
total_created=$((total_created + $?))

# ID 12: Trưởng phó IT | Tổng hợp | KTGS - 4 chỉ tiêu
create_indicators_for_table 12 "Trưởng phó IT | Tổng hợp | KTGS" 4
total_created=$((total_created + $?))

# ID 13: Cán bộ IT | Tổng hợp | KTGS | KH&QLRR - 9 chỉ tiêu
create_indicators_for_table 13 "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR" 9
total_created=$((total_created + $?))

# ID 14: Giám đốc Phòng giao dịch - 9 chỉ tiêu
create_indicators_for_table 14 "Giám đốc Phòng giao dịch" 9
total_created=$((total_created + $?))

# ID 15: Phó giám đốc Phòng giao dịch - 8 chỉ tiêu
create_indicators_for_table 15 "Phó giám đốc Phòng giao dịch" 8
total_created=$((total_created + $?))

# ID 16: Phó giám đốc PGD kiêm CBTD - 11 chỉ tiêu
create_indicators_for_table 16 "Phó giám đốc PGD kiêm CBTD" 11
total_created=$((total_created + $?))

# ID 17: Giám đốc CNL2 - 8 chỉ tiêu
create_indicators_for_table 17 "Giám đốc CNL2" 8
total_created=$((total_created + $?))

# ID 18: Phó giám đốc CNL2 phụ trách TD - 6 chỉ tiêu
create_indicators_for_table 18 "Phó giám đốc CNL2 phụ trách TD" 6
total_created=$((total_created + $?))

# ID 19: Phó giám đốc CNL2 phụ trách KT - 9 chỉ tiêu
create_indicators_for_table 19 "Phó giám đốc CNL2 phụ trách KT" 9
total_created=$((total_created + $?))

# ID 20: Trưởng phòng KH CNL2 - 8 chỉ tiêu
create_indicators_for_table 20 "Trưởng phòng KH CNL2" 8
total_created=$((total_created + $?))

# ID 21: Phó phòng KH CNL2 - 6 chỉ tiêu
create_indicators_for_table 21 "Phó phòng KH CNL2" 6
total_created=$((total_created + $?))

# ID 22: Trưởng phòng KTNQ CNL2 - 5 chỉ tiêu
create_indicators_for_table 22 "Trưởng phòng KTNQ CNL2" 5
total_created=$((total_created + $?))

# ID 23: Phó phòng KTNQ CNL2 - 5 chỉ tiêu
create_indicators_for_table 23 "Phó phòng KTNQ CNL2" 5
total_created=$((total_created + $?))

# Kiểm tra kết quả cuối cùng
echo ""
echo "📊 KIỂM TRA KẾT QUẢ:"
final_count=$(curl -s "$API_BASE/KpiIndicators" | jq '. | length')
echo "✅ Tổng số chỉ tiêu đã tạo thành công: $total_created"
echo "✅ Tổng số chỉ tiêu hiện có trong hệ thống: $final_count"

if [ "$final_count" -eq 158 ]; then
    echo "🎉 THÀNH CÔNG! Đã tạo đúng 158 chỉ tiêu KPI"
else
    echo "⚠️  Kết quả: Số lượng chỉ tiêu không đúng (cần 158, có $final_count)"
fi

echo ""
echo "📋 PHÂN BỐ CHỈ TIÊU THEO BẢNG:"
curl -s "$API_BASE/KpiAssignmentTables" | jq -r '.[] | select(.Category == "CANBO" and .Id <= 23) | "ID: \(.Id) - \(.TableName): \(.IndicatorCount) chỉ tiêu"'

echo "🎯 Script hoàn tất!"
