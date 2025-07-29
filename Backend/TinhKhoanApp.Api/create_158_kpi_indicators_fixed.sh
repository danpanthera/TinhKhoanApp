#!/bin/bash

# Script để tạo đúng 158 KPI Indicators cho 23 bảng cán bộ
# Theo README_DAT.md: 23 bảng KPI dành cho cán bộ đều có chỉ tiêu cụ thể đi kèm

API_BASE="http://localhost:5055/api"

echo "🎯 Tạo 158 KPI Indicators cho 23 bảng cán bộ"

# Xóa tất cả indicators cũ
echo "🧹 Xóa indicators cũ..."
curl -s -X DELETE "$API_BASE/KpiIndicators/clear" > /dev/null

# Lấy danh sách các bảng KPI cán bộ (ID 1-23)
echo "📋 Lấy danh sách bảng KPI cán bộ..."
TABLES=$(curl -s "$API_BASE/KpiAssignmentTables" | jq -r '.[] | select(.Category == "CANBO" and .Id <= 23) | "\(.Id)|\(.TableName)"')

if [ -z "$TABLES" ]; then
    echo "❌ Không tìm thấy bảng KPI nào!"
    exit 1
fi

# Định nghĩa chỉ tiêu theo từng loại bảng
declare -A INDICATOR_COUNTS=(
    [1]=8   # Trưởng phòng KHDN
    [2]=8   # Trưởng phòng KHCN
    [3]=8   # Phó phòng KHDN
    [4]=8   # Phó phòng KHCN
    [5]=6   # Trưởng phòng KH&QLRR
    [6]=6   # Phó phòng KH&QLRR
    [7]=6   # Trưởng phòng KTNQ CNL1
    [8]=6   # Phó phòng KTNQ CNL1
    [9]=6   # GDV
    [10]=8  # Cán bộ tín dụng
    [11]=5  # Trưởng phòng IT/TH/KTGS
    [12]=4  # CB IT/TH/KTGS
    [13]=9  # Giám đốc PGD
    [14]=9  # Phó giám đốc PGD
    [15]=8  # Phó giám đốc PGD kiêm CBTD
    [16]=11 # Giám đốc CNL2
    [17]=8  # Phó giám đốc CNL2 TD
    [18]=6  # Phó giám đốc CNL2 KT
    [19]=9  # Trưởng phòng KH CNL2
    [20]=8  # Phó phòng KH CNL2
    [21]=6  # Trưởng phòng KTNQ CNL2
    [22]=5  # Phó phòng KTNQ CNL2
    [23]=5  # Nhân viên khác
)

# Định nghĩa tên chỉ tiêu chuẩn
INDICATORS_BASIC=(
    "Tỷ lệ hoàn thành kế hoạch thu nhập (%)"
    "Tỷ lệ hoàn thành kế hoạch tăng trưởng dư nợ tín dụng (%)"
    "Tỷ lệ nợ xấu (%)"
    "Số lượng khách hàng mới phát triển"
    "Tỷ lệ hoàn thành kế hoạch huy động vốn (%)"
)

INDICATORS_EXTENDED=(
    "Hiệu quả hoạt động kinh doanh tổng hợp"
    "Chất lượng dịch vụ khách hàng"
    "Tuân thủ quy định pháp luật và nội bộ"
    "Phát triển sản phẩm dịch vụ mới"
    "Quản lý rủi ro tín dụng"
    "Hiệu quả quản lý chi phí"
)

total_created=0

echo "$TABLES" | while IFS='|' read -r table_id table_name; do
    if [ -n "$table_id" ] && [ -n "$table_name" ]; then
        indicator_count=${INDICATOR_COUNTS[$table_id]}

        if [ -n "$indicator_count" ]; then
            echo "📊 Tạo $indicator_count chỉ tiêu cho bảng ID:$table_id - $table_name..."

            # Tạo các chỉ tiêu cơ bản trước
            for i in $(seq 1 $indicator_count); do
                if [ $i -le 5 ]; then
                    indicator_name="${INDICATORS_BASIC[$((i-1))]}"
                else
                    indicator_name="${INDICATORS_EXTENDED[$((i-6))]}"
                fi

                # Tạo indicator
                response=$(curl -s -X POST "$API_BASE/KpiIndicators" \
                    -H "Content-Type: application/json" \
                    -d "{
                        \"KpiAssignmentTableId\": $table_id,
                        \"IndicatorName\": \"$indicator_name\",
                        \"Description\": \"$indicator_name cho $table_name\",
                        \"Unit\": \"%\",
                        \"TargetValue\": 100,
                        \"Weight\": $((100 / indicator_count)),
                        \"IsActive\": true
                    }")

                if echo "$response" | jq -e '.Id' > /dev/null 2>&1; then
                    ((total_created++))
                else
                    echo "⚠️  Lỗi tạo indicator $i cho bảng $table_id: $response"
                fi
            done

            echo "✅ Đã tạo $indicator_count chỉ tiêu cho ID:$table_id - $table_name"
        else
            echo "⚠️  Không tìm thấy cấu hình cho bảng ID:$table_id"
        fi
    fi
done

# Kiểm tra kết quả cuối cùng
echo ""
echo "📊 KIỂM TRA KẾT QUẢ:"
final_count=$(curl -s "$API_BASE/KpiIndicators" | jq '. | length')
echo "✅ Tổng số chỉ tiêu đã tạo: $final_count"

if [ "$final_count" -eq 158 ]; then
    echo "🎉 THÀNH CÔNG! Đã tạo đúng 158 chỉ tiêu KPI"
else
    echo "⚠️  Cảnh báo: Số lượng chỉ tiêu không đúng (cần 158, có $final_count)"
fi

echo ""
echo "📋 PHÂN BỐ CHỈ TIÊU THEO BẢNG:"
curl -s "$API_BASE/KpiAssignmentTables" | jq -r '.[] | select(.Category == "CANBO" and .Id <= 23) | "ID: \(.Id) - \(.TableName): \(.IndicatorCount) chỉ tiêu"'

echo "🎯 Script hoàn tất!"
