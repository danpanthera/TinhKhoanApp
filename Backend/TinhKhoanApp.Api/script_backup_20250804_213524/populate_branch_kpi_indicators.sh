#!/bin/bash

# 🏢 Script tạo chỉ tiêu KPI cho 9 bảng chi nhánh
# Dựa trên mẫu bảng "Giám đốc CNL2" (ID=17)

echo "🏢 Đang tạo chỉ tiêu KPI cho 9 bảng chi nhánh..."

# Mapping table IDs cho 9 chi nhánh (24-32)
declare -A BRANCH_TABLES=(
    [24]="HoiSo"
    [25]="BinhLu"
    [26]="PhongTho"
    [27]="SinHo"
    [28]="BumTo"
    [29]="ThanUyen"
    [30]="DoanKet"
    [31]="TanUyen"
    [32]="NamHang"
)

# Template chỉ tiêu dựa trên "Giám đốc CNL2"
INDICATORS=(
    '{"IndicatorName": "Tổng dư nợ BQ", "MaxScore": 30.0, "Unit": "Triệu VND", "OrderIndex": 1, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Tỷ lệ nợ xấu", "MaxScore": 15.0, "Unit": "%", "OrderIndex": 2, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Phát triển Khách hàng", "MaxScore": 10.0, "Unit": "Khách hàng", "OrderIndex": 3, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Thu nợ đã XLRR", "MaxScore": 10.0, "Unit": "Triệu VND", "OrderIndex": 4, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Thực hiện nhiệm vụ theo chương trình công tác", "MaxScore": 10.0, "Unit": "%", "OrderIndex": 5, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", "MaxScore": 10.0, "Unit": "%", "OrderIndex": 6, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Tổng nguồn vốn huy động BQ", "MaxScore": 10.0, "Unit": "Triệu VND", "OrderIndex": 7, "ValueType": "NUMBER", "IsActive": true}'
    '{"IndicatorName": "Hoàn thành chỉ tiêu giao khoán SPDV", "MaxScore": 5.0, "Unit": "%", "OrderIndex": 8, "ValueType": "NUMBER", "IsActive": true}'
)

API_BASE="http://localhost:5055/api"
SUCCESS_COUNT=0
TOTAL_COUNT=0

# Tạo chỉ tiêu cho từng bảng chi nhánh
for TABLE_ID in "${!BRANCH_TABLES[@]}"; do
    BRANCH_NAME="${BRANCH_TABLES[$TABLE_ID]}"
    echo "📊 Đang xử lý bảng: $BRANCH_NAME (ID: $TABLE_ID)"

    # Xóa chỉ tiêu cũ (nếu có)
    echo "  🗑️  Xóa chỉ tiêu cũ..."
    curl -s -X DELETE "$API_BASE/KpiAssignment/tables/$TABLE_ID/indicators" > /dev/null

    # Thêm từng chỉ tiêu
    for i in "${!INDICATORS[@]}"; do
        INDICATOR="${INDICATORS[$i]}"
        TOTAL_COUNT=$((TOTAL_COUNT + 1))

        echo "    ➕ Thêm chỉ tiêu $(($i + 1))/8..."

        RESPONSE=$(curl -s -X POST "$API_BASE/KpiAssignment/indicators" \
            -H "Content-Type: application/json" \
            -d "{
                \"KpiAssignmentTableId\": $TABLE_ID,
                $(echo "$INDICATOR" | sed 's/^{//' | sed 's/}$//')
            }")

        if [[ $? -eq 0 ]]; then
            SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
            echo "    ✅ Thành công"
        else
            echo "    ❌ Thất bại: $RESPONSE"
        fi
    done

    echo "  ✅ Hoàn thành bảng $BRANCH_NAME"
    echo ""
done

# Báo cáo kết quả
echo "📊 TỔNG KẾT:"
echo "  ✅ Thành công: $SUCCESS_COUNT/$TOTAL_COUNT chỉ tiêu"
echo "  📋 Đã cấu hình cho 9 bảng chi nhánh"
echo "  💯 Mỗi bảng có 8 chỉ tiêu (tổng điểm: 100)"

# Kiểm tra kết quả
echo ""
echo "🔍 Kiểm tra kết quả:"
for TABLE_ID in "${!BRANCH_TABLES[@]}"; do
    BRANCH_NAME="${BRANCH_TABLES[$TABLE_ID]}"
    COUNT=$(curl -s "$API_BASE/KpiAssignment/tables/$TABLE_ID" | jq '.Indicators | length' 2>/dev/null || echo "0")
    echo "  📊 $BRANCH_NAME: $COUNT chỉ tiêu"
done

echo ""
echo "🎉 Hoàn thành cấu hình KPI cho 9 chi nhánh!"
