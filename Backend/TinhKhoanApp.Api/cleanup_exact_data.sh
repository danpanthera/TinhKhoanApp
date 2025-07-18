#!/bin/bash

# =============================================================================
# CLEANUP EXACT DATA - CHỈ GIỮ LẠI 46 UNITS VÀ 23 ROLES CHUẨN
# =============================================================================

echo "🧹 Bắt đầu cleanup dữ liệu thừa..."

# API Base URL
API_BASE="http://localhost:5055/api"

# =============================================================================
# 1. CLEANUP ROLES - CHỈ GIỮ 23 ROLES CHUẨN
# =============================================================================

echo "📋 Đang cleanup Roles..."

# Lấy danh sách roles hiện tại
echo "Roles hiện tại:"
curl -s "$API_BASE/roles" | jq -r '.[] | "\(.id): \(.name)"'

# Xóa role thừa (ID > 23 hoặc duplicate)
echo "🗑️ Xóa roles thừa..."
ROLES_TO_DELETE=$(curl -s "$API_BASE/roles" | jq -r '.[] | select(.id > 23) | .id')

for role_id in $ROLES_TO_DELETE; do
    echo "Xóa role ID: $role_id"
    curl -X DELETE "$API_BASE/roles/$role_id" -s
done

echo "✅ Roles cleanup hoàn thành. Checking..."
ROLE_COUNT=$(curl -s "$API_BASE/roles" | jq 'length')
echo "Số roles sau cleanup: $ROLE_COUNT/23"

# =============================================================================
# 2. CLEANUP UNITS - CHỈ GIỮ 46 UNITS CHUẨN
# =============================================================================

echo "🏢 Đang cleanup Units..."

# Lấy danh sách units hiện tại
echo "Units hiện tại:"
curl -s "$API_BASE/units" | jq -r '.[] | "\(.id): \(.name)"'

# Xóa units thừa (ID > 46)
echo "🗑️ Xóa units thừa..."
UNITS_TO_DELETE=$(curl -s "$API_BASE/units" | jq -r '.[] | select(.id > 46) | .id')

for unit_id in $UNITS_TO_DELETE; do
    echo "Xóa unit ID: $unit_id"
    curl -X DELETE "$API_BASE/units/$unit_id" -s
done

echo "✅ Units cleanup hoàn thành. Checking..."
UNIT_COUNT=$(curl -s "$API_BASE/units" | jq 'length')
echo "Số units sau cleanup: $UNIT_COUNT/46"

# =============================================================================
# 3. FINAL STATUS CHECK
# =============================================================================

echo ""
echo "📊 TỔNG KẾT CLEANUP:"
echo "==================="
echo "Roles: $ROLE_COUNT/23 $([ $ROLE_COUNT -eq 23 ] && echo '✅' || echo '⚠️')"
echo "Units: $UNIT_COUNT/46 $([ $UNIT_COUNT -eq 46 ] && echo '✅' || echo '⚠️')"

if [ $ROLE_COUNT -eq 23 ] && [ $UNIT_COUNT -eq 46 ]; then
    echo ""
    echo "🎉 CLEANUP THÀNH CÔNG! Dữ liệu đã đúng chuẩn."
else
    echo ""
    echo "⚠️ Cần kiểm tra lại dữ liệu không đúng chuẩn."
fi

echo ""
echo "✅ Cleanup script hoàn thành!"
