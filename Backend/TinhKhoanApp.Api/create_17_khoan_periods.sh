#!/bin/bash

echo "=== TẠO 17 KỲ KHOÁN CHO NĂM 2025 ==="

API_BASE="http://localhost:5055/api/KhoanPeriods"

# Tạo 12 kỳ tháng
for month in {1..12}; do
    # Định dạng tháng với 2 chữ số
    month_str=$(printf "%02d" $month)

    # Tính ngày cuối tháng
    case $month in
        1|3|5|7|8|10|12) last_day=31 ;;
        4|6|9|11) last_day=30 ;;
        2) last_day=28 ;;  # 2025 không phải năm nhuận
    esac

    echo "Creating monthly period: Tháng $month_str/2025"

    curl -X POST "$API_BASE" \
        -H "Content-Type: application/json" \
        -d "{
            \"Name\": \"Tháng $month_str/2025\",
            \"Type\": 0,
            \"StartDate\": \"2025-$month_str-01T00:00:00\",
            \"EndDate\": \"2025-$month_str-${last_day}T23:59:59\",
            \"Status\": 0
        }" \
        -s > /dev/null

    if [ $? -eq 0 ]; then
        echo "✅ Created: Tháng $month_str/2025"
    else
        echo "❌ Failed: Tháng $month_str/2025"
    fi
done

echo ""
echo "=== TẠO 4 KỲ QUÝ ==="

# Tạo 4 kỳ quý
declare -a quarters=(
    "1:Quý I/2025:2025-01-01T00:00:00:2025-03-31T23:59:59"
    "2:Quý II/2025:2025-04-01T00:00:00:2025-06-30T23:59:59"
    "3:Quý III/2025:2025-07-01T00:00:00:2025-09-30T23:59:59"
    "4:Quý IV/2025:2025-10-01T00:00:00:2025-12-31T23:59:59"
)

for quarter_info in "${quarters[@]}"; do
    IFS=':' read -r quarter_num quarter_name start_date end_date <<< "$quarter_info"

    echo "Creating quarterly period: $quarter_name"

    curl -X POST "$API_BASE" \
        -H "Content-Type: application/json" \
        -d "{
            \"Name\": \"$quarter_name\",
            \"Type\": 1,
            \"StartDate\": \"$start_date\",
            \"EndDate\": \"$end_date\",
            \"Status\": 0
        }" \
        -s > /dev/null

    if [ $? -eq 0 ]; then
        echo "✅ Created: $quarter_name"
    else
        echo "❌ Failed: $quarter_name"
    fi
done

echo ""
echo "=== TẠO 1 KỲ NĂM ==="

# Tạo 1 kỳ năm
echo "Creating annual period: Năm 2025"

curl -X POST "$API_BASE" \
    -H "Content-Type: application/json" \
    -d '{
        "Name": "Năm 2025",
        "Type": 2,
        "StartDate": "2025-01-01T00:00:00",
        "EndDate": "2025-12-31T23:59:59",
        "Status": 0
    }' \
    -s > /dev/null

if [ $? -eq 0 ]; then
    echo "✅ Created: Năm 2025"
else
    echo "❌ Failed: Năm 2025"
fi

echo ""
echo "=== VERIFICATION ==="
echo "Checking total count..."
total_count=$(curl -s "$API_BASE" | jq '. | length')
monthly_count=$(curl -s "$API_BASE" | jq '[.[] | select(.Type == "MONTHLY")] | length')
quarterly_count=$(curl -s "$API_BASE" | jq '[.[] | select(.Type == "QUARTERLY")] | length')
annual_count=$(curl -s "$API_BASE" | jq '[.[] | select(.Type == "ANNUAL")] | length')

echo "📊 RESULTS:"
echo "- Total periods: $total_count"
echo "- Monthly periods: $monthly_count"
echo "- Quarterly periods: $quarterly_count"
echo "- Annual periods: $annual_count"

if [ "$total_count" = "17" ] && [ "$monthly_count" = "12" ] && [ "$quarterly_count" = "4" ] && [ "$annual_count" = "1" ]; then
    echo "✅ SUCCESS: All 17 Khoan Periods created correctly!"
else
    echo "❌ MISMATCH: Expected 17 total (12 monthly + 4 quarterly + 1 annual)"
fi

echo "=== COMPLETED ==="
