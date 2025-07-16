#!/bin/bash

# ====================================
# Khôi phục KPI Definitions - RESTORE KPI CONFIGURATIONS
# ====================================

echo "🔧 KHÔI PHỤC CẤU HÌNH KPI CHO 23 VAI TRÒ..."
echo "============================================="

BASE_URL="http://localhost:5055/api"

echo "📊 Kiểm tra tình trạng hiện tại..."
CURRENT_KPI_COUNT=$(curl -s "${BASE_URL}/KPIDefinitions" | jq '. | length')
echo "Số KPI Definitions hiện tại: $CURRENT_KPI_COUNT"

if [ "$CURRENT_KPI_COUNT" -gt 0 ]; then
    echo "⚠️ Đã có $CURRENT_KPI_COUNT KPI Definitions trong hệ thống"
    echo "🔄 Xóa tất cả KPI Definitions cũ để tạo mới..."

    # Lấy danh sách ID và xóa từng cái
    KPI_IDS=$(curl -s "${BASE_URL}/KPIDefinitions" | jq -r '.[].id')
    for kpi_id in $KPI_IDS; do
        echo "🗑️ Xóa KPI Definition ID: $kpi_id"
        curl -s -X DELETE "${BASE_URL}/KPIDefinitions/$kpi_id" > /dev/null
    done
fi

echo ""
echo "🚀 Gọi API seed KPI Definitions..."

# Gọi API để seed KPI definitions từ code
response=$(curl -s -w "%{http_code}" -X POST "${BASE_URL}/seed/kpi-definitions" -o /tmp/seed_response.txt)
http_code="${response: -3}"

if [ "$http_code" -eq 200 ] || [ "$http_code" -eq 204 ]; then
    echo "✅ Seed KPI Definitions thành công!"
else
    echo "❌ Lỗi khi seed KPI Definitions (HTTP: $http_code)"
    if [ -f /tmp/seed_response.txt ]; then
        echo "Response:"
        cat /tmp/seed_response.txt
    fi
fi

echo ""
echo "📊 Kiểm tra kết quả:"
echo "==================="

# Kiểm tra lại số lượng
NEW_KPI_COUNT=$(curl -s "${BASE_URL}/KPIDefinitions" | jq '. | length')
echo "Tổng số KPI Definitions sau khi restore: $NEW_KPI_COUNT"

if [ "$NEW_KPI_COUNT" -gt 0 ]; then
    echo ""
    echo "📋 Danh sách KPI Definitions theo vai trò:"
    echo "=========================================="

    # Hiển thị thống kê theo roleCode
    curl -s "${BASE_URL}/KPIDefinitions" | jq -r 'group_by(.roleCode) | .[] | "\(.[0].roleCode): \(length) chỉ tiêu"' | head -10

    echo ""
    echo "🎯 Một số chỉ tiêu mẫu:"
    curl -s "${BASE_URL}/KPIDefinitions" | jq -r '.[:5] | .[] | "ID \(.id): \(.name) (\(.roleCode)) - Điểm tối đa: \(.maxScore)"'

    if [ "$NEW_KPI_COUNT" -gt 5 ]; then
        echo "... và $(($NEW_KPI_COUNT - 5)) chỉ tiêu khác"
    fi
else
    echo "❌ Không có KPI Definitions nào được tạo"
fi

echo ""
echo "🎉 HOÀN THÀNH RESTORE KPI CONFIGURATIONS!"
echo "Hệ thống KPI đã sẵn sàng cho 23 vai trò cán bộ chuẩn."
