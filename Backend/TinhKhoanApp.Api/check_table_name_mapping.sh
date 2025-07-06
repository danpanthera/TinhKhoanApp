#!/bin/bash

echo "🔍 SO SÁNH TÊN BẢNG KPI: Script vs Database"
echo "============================================="

API_BASE="http://localhost:5055/api"

# Lấy danh sách tên bảng từ database
echo "📋 Tên bảng trong Database:"
curl -s "$API_BASE/KpiAssignment/tables" | jq -r '.[] | .TableName' | grep -v "^KPI_" | sort > /tmp/db_tables.txt
cat /tmp/db_tables.txt | nl

echo ""
echo "📋 Tên bảng trong Script populate:"

# Trích xuất tên bảng từ script populate
grep -o 'create_kpi_indicator "[^"]*"' /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/populate_all_kpi_indicators_new.sh | \
    sed 's/create_kpi_indicator "//g' | sed 's/".*//g' | sort | uniq > /tmp/script_tables.txt
cat /tmp/script_tables.txt | nl

echo ""
echo "❌ Tên bảng KHÔNG khớp:"
echo "   (Có trong Database nhưng KHÔNG có trong Script)"
comm -23 /tmp/db_tables.txt /tmp/script_tables.txt | while read table; do
    echo "   - $table"
done

echo ""
echo "⚠️  Tên bảng SAI trong Script:"
echo "   (Có trong Script nhưng KHÔNG có trong Database)"
comm -13 /tmp/db_tables.txt /tmp/script_tables.txt | while read table; do
    echo "   - $table"
done

echo ""
echo "✅ Tên bảng ĐÚNG (khớp):"
comm -12 /tmp/db_tables.txt /tmp/script_tables.txt | wc -l | awk '{print "   Có " $1 " bảng khớp"}'

# Cleanup
rm -f /tmp/db_tables.txt /tmp/script_tables.txt
