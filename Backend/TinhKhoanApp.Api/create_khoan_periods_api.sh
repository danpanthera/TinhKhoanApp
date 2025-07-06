#!/bin/bash

# Script tạo KhoanPeriods qua API cho hệ thống TinhKhoanApp
# Ngày tạo: 06/07/2025
# Mục đích: Tạo các kỳ khoán cho năm 2025

BASE_URL="http://localhost:5055/api/khoanperiods"

echo "🎯 BẮT ĐẦU TẠO KHOAN PERIODS QUA API"
echo "====================================="

# 1. KIỂM TRA BACKEND
echo ""
echo "📊 1. KIỂM TRA BACKEND:"
health_check=$(curl -s "http://localhost:5055/health" | jq -r '.status' 2>/dev/null)
if [ "$health_check" = "Healthy" ]; then
    echo "   ✅ Backend đang hoạt động bình thường"
else
    echo "   ❌ Backend không hoạt động. Vui lòng khởi động backend trước!"
    exit 1
fi

# 2. KIỂM TRA KHOAN PERIODS HIỆN TẠI
echo ""
echo "📋 2. KIỂM TRA KHOAN PERIODS HIỆN TẠI:"
existing_count=$(curl -s "$BASE_URL" | jq '. | length' 2>/dev/null)
echo "   📅 KhoanPeriods hiện có: $existing_count kỳ"

# 3. TẠO 12 KỲ KHOÁN THÁNG CHO NĂM 2025
echo ""
echo "📅 3. TẠO 12 KỲ KHOÁN THÁNG:"

months=("01" "02" "03" "04" "05" "06" "07" "08" "09" "10" "11" "12")
month_days=("31" "28" "31" "30" "31" "30" "31" "31" "30" "31" "30" "31")
monthly_created=0

for i in "${!months[@]}"; do
    month="${months[$i]}"
    days="${month_days[$i]}"

    payload=$(cat <<EOF
{
    "name": "Tháng $month/2025",
    "type": 0,
    "startDate": "2025-$month-01T00:00:00",
    "endDate": "2025-$month-${days}T23:59:59",
    "status": 1
}
EOF
)

    echo "   📝 Tạo kỳ khoán: Tháng $month/2025"
    response=$(curl -s -X POST "$BASE_URL" \
        -H "Content-Type: application/json" \
        -d "$payload")

    if echo "$response" | jq -e '.id' >/dev/null 2>&1; then
        echo "      ✅ Thành công"
        ((monthly_created++))
    else
        echo "      ❌ Lỗi: $response"
    fi
done

echo "   ✅ Đã tạo $monthly_created kỳ khoán tháng"

# 4. TẠO 4 KỲ KHOÁN QUÝ CHO NĂM 2025
echo ""
echo "🗓️ 4. TẠO 4 KỲ KHOÁN QUÝ:"

quarters=(
    "I|2025-01-01|2025-03-31"
    "II|2025-04-01|2025-06-30"
    "III|2025-07-01|2025-09-30"
    "IV|2025-10-01|2025-12-31"
)
quarterly_created=0

for quarter_info in "${quarters[@]}"; do
    IFS='|' read -r quarter start_date end_date <<< "$quarter_info"

    payload=$(cat <<EOF
{
    "name": "Quý $quarter/2025",
    "type": 1,
    "startDate": "${start_date}T00:00:00",
    "endDate": "${end_date}T23:59:59",
    "status": 1
}
EOF
)

    echo "   📝 Tạo kỳ khoán: Quý $quarter/2025"
    response=$(curl -s -X POST "$BASE_URL" \
        -H "Content-Type: application/json" \
        -d "$payload")

    if echo "$response" | jq -e '.id' >/dev/null 2>&1; then
        echo "      ✅ Thành công"
        ((quarterly_created++))
    else
        echo "      ❌ Lỗi: $response"
    fi
done

echo "   ✅ Đã tạo $quarterly_created kỳ khoán quý"

# 5. TẠO 1 KỲ KHOÁN NĂM 2025
echo ""
echo "📆 5. TẠO KỲ KHOÁN NĂM:"

payload=$(cat <<EOF
{
    "name": "Năm 2025",
    "type": 2,
    "startDate": "2025-01-01T00:00:00",
    "endDate": "2025-12-31T23:59:59",
    "status": 1
}
EOF
)

echo "   📝 Tạo kỳ khoán: Năm 2025"
response=$(curl -s -X POST "$BASE_URL" \
    -H "Content-Type: application/json" \
    -d "$payload")

yearly_created=0
if echo "$response" | jq -e '.id' >/dev/null 2>&1; then
    echo "      ✅ Thành công"
    yearly_created=1
else
    echo "      ❌ Lỗi: $response"
fi

echo "   ✅ Đã tạo $yearly_created kỳ khoán năm"

# 6. KIỂM TRA KẾT QUẢ
echo ""
echo "📊 6. KIỂM TRA KẾT QUẢ:"
total_periods=$(curl -s "$BASE_URL" | jq '. | length' 2>/dev/null)
echo "   📅 Tổng kỳ khoán: $total_periods"
echo "       - Theo tháng: $monthly_created kỳ"
echo "       - Theo quý: $quarterly_created kỳ"
echo "       - Theo năm: $yearly_created kỳ"

# 7. HIỂN THỊ DANH SÁCH
echo ""
echo "📋 7. DANH SÁCH KỲ KHOÁN ĐÃ TẠO:"
curl -s "$BASE_URL" | jq -r '.[] | "   ID: \(.id) | \(.name) | Type: \(.type) | \(.startDate) đến \(.endDate)"' 2>/dev/null

# 8. KẾT LUẬN
echo ""
if [ "$total_periods" = "17" ]; then
    echo "✅ THÀNH CÔNG: Đã tạo đầy đủ $total_periods kỳ khoán!"
    echo "   🎯 Sẵn sàng cho bước tiếp theo: Gán Employees vào Units và Roles"
else
    echo "⚠️ CẢNH BÁO: Số lượng kỳ khoán không đúng ($total_periods/17)"
fi

echo ""
echo "🏁 HOÀN THÀNH TẠO KHOAN PERIODS"
echo "==============================="
