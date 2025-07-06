#!/bin/bash

echo "📊 KIỂM TRA TỔNG SỐ CHỈ TIÊU KPI - Sau khi populate"
echo "=================================================="

API_BASE="http://localhost:5055/api"

echo "📋 Số lượng chỉ tiêu theo từng bảng KPI:"
echo ""

total_indicators=0
employee_tables=0

for i in {1..23}; do
    table_info=$(curl -s "$API_BASE/KpiAssignment/tables/$i")
    table_name=$(echo "$table_info" | jq -r '.TableName // "N/A"')
    description=$(echo "$table_info" | jq -r '.Description // "N/A"')
    indicator_count=$(echo "$table_info" | jq '.Indicators | length // 0')

    if [ "$table_name" != "N/A" ] && [ "$indicator_count" -gt 0 ]; then
        printf "  %2d. %-25s | %2s chỉ tiêu | %s\n" "$i" "$table_name" "$indicator_count" "$description"
        total_indicators=$((total_indicators + indicator_count))
        employee_tables=$((employee_tables + 1))

        # Hiển thị tên các chỉ tiêu mới (OrderIndex >= 17)
        new_indicators=$(echo "$table_info" | jq -r '.Indicators[] | select(.OrderIndex >= 17) | "     - " + .IndicatorName + " (" + (.MaxScore | tostring) + " " + .Unit + ")"')
        if [ ! -z "$new_indicators" ]; then
            echo "$new_indicators"
        fi
        echo ""
    fi
done

echo "======================================================="
echo "📊 TỔNG KẾT:"
echo "   Số bảng KPI cán bộ có chỉ tiêu: $employee_tables/23"
echo "   Tổng số chỉ tiêu KPI: $total_indicators"
echo ""

# Tính số chỉ tiêu mới (theo OrderIndex >= 17)
echo "📈 Chỉ tiêu mới được thêm (theo danh sách anh cung cấp):"
new_total=0
for i in {1..23}; do
    table_info=$(curl -s "$API_BASE/KpiAssignment/tables/$i")
    new_count=$(echo "$table_info" | jq '[.Indicators[] | select(.OrderIndex >= 17)] | length // 0')
    new_total=$((new_total + new_count))
done

echo "   Số chỉ tiêu mới: $new_total"
echo ""

if [ $new_total -ge 158 ]; then
    echo "✅ ĐÃ ĐẠT MỤC TIÊU: Có đủ 158+ chỉ tiêu cho 23 bảng KPI cán bộ!"
else
    echo "⚠️  CHƯA ĐẦY ĐỦ: Cần thêm $((158 - new_total)) chỉ tiêu nữa để đạt 158 chỉ tiêu."
fi
