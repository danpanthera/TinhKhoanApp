#!/bin/bash

# ====================================
# Tạo Kỳ Khoán Mẫu - CREATE KHOAN PERIODS
# ====================================

echo "🗓️ TẠO CÁC KỲ KHOÁN MẪU..."
echo "======================================"

BASE_URL="http://localhost:5055/api"

# Tạo kỳ khoán tháng cho năm 2024
declare -a MONTHS=(
    "01" "02" "03" "04" "05" "06"
    "07" "08" "09" "10" "11" "12"
)

echo "📅 Tạo 12 kỳ khoán tháng cho năm 2024..."
for month in "${MONTHS[@]}"; do
    # Tính ngày bắt đầu và kết thúc cho từng tháng
    if [ "$month" = "02" ]; then
        end_day="29"  # 2024 là năm nhuận
    elif [[ "$month" = "04" || "$month" = "06" || "$month" = "09" || "$month" = "11" ]]; then
        end_day="30"
    else
        end_day="31"
    fi

    PERIOD_DATA=$(cat <<EOF
{
    "name": "Tháng ${month}/2024",
    "type": 0,
    "startDate": "2024-${month}-01T00:00:00Z",
    "endDate": "2024-${month}-${end_day}T23:59:59Z",
    "status": 1
}
EOF
)

    echo "📝 Tạo kỳ khoán: Tháng ${month}/2024"
    curl -s -X POST "${BASE_URL}/KhoanPeriods" \
        -H "Content-Type: application/json" \
        -d "$PERIOD_DATA" | jq -r '.id // "Lỗi"' > /dev/null

    if [ $? -eq 0 ]; then
        echo "   ✅ Thành công"
    else
        echo "   ❌ Thất bại"
    fi
done

echo ""
echo "📅 Tạo 4 kỳ khoán quý cho năm 2024..."

# Tạo kỳ khoán theo quý
declare -a QUARTERS=(
    "I:01-01:03-31"
    "II:04-01:06-30"
    "III:07-01:09-30"
    "IV:10-01:12-31"
)

for quarter_info in "${QUARTERS[@]}"; do
    IFS=':' read -ra QUARTER_PARTS <<< "$quarter_info"
    quarter_name="${QUARTER_PARTS[0]}"
    start_date="${QUARTER_PARTS[1]}"
    end_date="${QUARTER_PARTS[2]}"

    PERIOD_DATA=$(cat <<EOF
{
    "name": "Quý ${quarter_name}/2024",
    "type": 1,
    "startDate": "2024-${start_date}T00:00:00Z",
    "endDate": "2024-${end_date}T23:59:59Z",
    "status": 1
}
EOF
)

    echo "📝 Tạo kỳ khoán: Quý ${quarter_name}/2024"
    curl -s -X POST "${BASE_URL}/KhoanPeriods" \
        -H "Content-Type: application/json" \
        -d "$PERIOD_DATA" | jq -r '.id // "Lỗi"' > /dev/null

    if [ $? -eq 0 ]; then
        echo "   ✅ Thành công"
    else
        echo "   ❌ Thất bại"
    fi
done

echo ""
echo "📅 Tạo kỳ khoán năm 2024..."

PERIOD_DATA=$(cat <<EOF
{
    "name": "Năm 2024",
    "type": 2,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z",
    "status": 1
}
EOF
)

echo "📝 Tạo kỳ khoán: Năm 2024"
curl -s -X POST "${BASE_URL}/KhoanPeriods" \
    -H "Content-Type: application/json" \
    -d "$PERIOD_DATA" | jq -r '.id // "Lỗi"' > /dev/null

if [ $? -eq 0 ]; then
    echo "   ✅ Thành công"
else
    echo "   ❌ Thất bại"
fi

echo ""
echo "📅 Tạo một số kỳ khoán cho năm 2025..."

# Tạo 3 tháng đầu năm 2025
for month in "01" "02" "03"; do
    if [ "$month" = "02" ]; then
        end_day="28"  # 2025 không phải năm nhuận
    else
        end_day="31"
    fi

    PERIOD_DATA=$(cat <<EOF
{
    "name": "Tháng ${month}/2025",
    "type": 0,
    "startDate": "2025-${month}-01T00:00:00Z",
    "endDate": "2025-${month}-${end_day}T23:59:59Z",
    "status": 0
}
EOF
)

    echo "📝 Tạo kỳ khoán: Tháng ${month}/2025 (Trạng thái DRAFT)"
    curl -s -X POST "${BASE_URL}/KhoanPeriods" \
        -H "Content-Type: application/json" \
        -d "$PERIOD_DATA" | jq -r '.id // "Lỗi"' > /dev/null

    if [ $? -eq 0 ]; then
        echo "   ✅ Thành công"
    else
        echo "   ❌ Thất bại"
    fi
done

echo ""
echo "📊 Kiểm tra danh sách kỳ khoán đã tạo:"
echo "======================================"

curl -s "${BASE_URL}/KhoanPeriods" | jq -r '.[] | "\(.id): \(.name) (\(.type | if . == 0 then "THÁNG" elif . == 1 then "QUÝ" else "NĂM" end)) - \(.status | if . == 0 then "DRAFT" elif . == 1 then "OPEN" elif . == 2 then "PROCESSING" elif . == 3 then "PENDING APPROVAL" elif . == 4 then "CLOSED" else "ARCHIVED" end)"'

TOTAL_PERIODS=$(curl -s "${BASE_URL}/KhoanPeriods" | jq '. | length')
echo ""
echo "🎉 HOÀN THÀNH: Đã tạo $TOTAL_PERIODS kỳ khoán!"
echo "   - 12 kỳ tháng năm 2024 (trạng thái OPEN)"
echo "   - 4 kỳ quý năm 2024 (trạng thái OPEN)"
echo "   - 1 kỳ năm 2024 (trạng thái OPEN)"
echo "   - 3 kỳ tháng đầu năm 2025 (trạng thái DRAFT)"
