#!/bin/bash

echo "🔄 POPULATE KPI INDICATORS VÀO ASSIGNMENT TABLES"
echo "================================================="

# Function để call API tạo indicators cho từng table
populate_indicators() {
    local table_id=$1
    local table_name="$2"
    local indicator_count=$3

    echo "📋 Processing Table ID: $table_id - $table_name ($indicator_count indicators)"

    # Tạo sample indicators cho table này với schema đúng
    for i in $(seq 1 $indicator_count); do
        curl -s -X POST "http://localhost:5055/api/KpiIndicators" \
            -H "Content-Type: application/json" \
            -d "{
                \"TableId\": $table_id,
                \"IndicatorName\": \"Chỉ tiêu ${i} - ${table_name}\",
                \"MaxScore\": 10.0,
                \"Unit\": \"Triệu đồng\",
                \"OrderIndex\": $i,
                \"ValueType\": 1,
                \"IsActive\": true
            }" > /dev/null
    done

    echo "   ✅ Đã tạo $indicator_count indicators"
}

# Populate indicators theo đúng phân bố đã định
echo ""
echo "🎯 POPULATE INDICATORS CHO CÁC BẢNG KPI:"

# CANBO tables with specific indicator counts
populate_indicators 3 "TruongphongKhdn" 8
populate_indicators 4 "TruongphongKhcn" 8
populate_indicators 5 "PhophongKhdn" 8
populate_indicators 6 "PhophongKhcn" 8
populate_indicators 7 "TruongphongKhqlrr" 6
populate_indicators 8 "PhophongKhqlrr" 6
populate_indicators 9 "Cbtd" 8
populate_indicators 10 "TruongphongKtnqCnl1" 6
populate_indicators 11 "PhophongKtnqCnl1" 6
populate_indicators 12 "Gdv" 6
populate_indicators 13 "TqHkKtnb" 5
populate_indicators 14 "TruongphoItThKtgs" 5
populate_indicators 15 "CBItThKtgsKhqlrr" 4
populate_indicators 16 "GiamdocPgd" 9
populate_indicators 17 "PhogiamdocPgd" 9
populate_indicators 18 "PhogiamdocPgdCbtd" 8
populate_indicators 19 "GiamdocCnl2" 11
populate_indicators 20 "PhogiamdocCnl2Td" 8
populate_indicators 21 "PhogiamdocCnl2Kt" 6
populate_indicators 22 "TruongphongKhCnl2" 9
populate_indicators 23 "PhophongKhCnl2" 8
populate_indicators 24 "TruongphongKtnqCnl2" 6
populate_indicators 25 "PhophongKtnqCnl2" 5

# CHINHANH tables - mỗi chi nhánh 10 indicators
for table_id in {26..34}; do
    populate_indicators $table_id "ChiNhanh_${table_id}" 10
done

echo ""
echo "✅ HOÀN THÀNH! Đã populate indicators cho tất cả bảng KPI"

# Verify kết quả
echo ""
echo "🔍 VERIFICATION:"
curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq '.[] | {Id, TableName, Category, IndicatorCount}' | head -20
