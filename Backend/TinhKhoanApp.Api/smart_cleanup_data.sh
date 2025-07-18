#!/bin/bash

# =============================================================================
# SMART CLEANUP - XÓA DỮ LIỆU THỪA VÀ DUPLICATES
# =============================================================================

echo "🧹 Bắt đầu Smart Cleanup..."

API_BASE="http://localhost:5055/api"

# =============================================================================
# 1. XÓA ROLES THỪA (ID >= 54)
# =============================================================================

echo "📋 Cleanup Roles duplicates..."

# Lấy danh sách roles có ID >= 54 (roles thừa)
ROLES_TO_DELETE=$(curl -s "$API_BASE/roles" | jq -r '.[] | select(.Id >= 54) | .Id')

echo "🗑️ Xóa roles với ID >= 54:"
for role_id in $ROLES_TO_DELETE; do
    echo "  Xóa role ID: $role_id"
    response=$(curl -X DELETE "$API_BASE/roles/$role_id" -s -w "%{http_code}")
    echo "    Response: $response"
done

# =============================================================================
# 2. XÓA UNITS THỪA (ID > 46)
# =============================================================================

echo "🏢 Cleanup Units thừa..."

# Lấy danh sách units có ID > 46
UNITS_TO_DELETE=$(curl -s "$API_BASE/units" | jq -r '.[] | select(.Id > 46) | .Id')

echo "🗑️ Xóa units với ID > 46:"
for unit_id in $UNITS_TO_DELETE; do
    echo "  Xóa unit ID: $unit_id"
    response=$(curl -X DELETE "$API_BASE/units/$unit_id" -s -w "%{http_code}")
    echo "    Response: $response"
done

# =============================================================================
# 3. VERIFICATION
# =============================================================================

echo ""
echo "🔍 VERIFICATION RESULTS:"
echo "========================"

# Check roles
ROLE_COUNT=$(curl -s "$API_BASE/roles" | jq 'length')
echo "Roles: $ROLE_COUNT/23 $([ $ROLE_COUNT -eq 23 ] && echo '✅' || echo '⚠️')"

# Check units
UNIT_COUNT=$(curl -s "$API_BASE/units" | jq 'length')
echo "Units: $UNIT_COUNT/46 $([ $UNIT_COUNT -eq 46 ] && echo '✅' || echo '⚠️')"

# Hiển thị ranges
echo ""
echo "📊 Current ID Ranges:"
echo "Roles: $(curl -s "$API_BASE/roles" | jq -r '[.[].Id] | "min: \(min), max: \(max)"')"
echo "Units: $(curl -s "$API_BASE/units" | jq -r '[.[].Id] | "min: \(min), max: \(max)"')"

echo ""
if [ $ROLE_COUNT -eq 23 ] && [ $UNIT_COUNT -eq 46 ]; then
    echo "🎉 CLEANUP THÀNH CÔNG! Dữ liệu chuẩn: 23 roles + 46 units"
else
    echo "⚠️ Cần kiểm tra thêm. Dữ liệu chưa đúng chuẩn."
fi

echo "✅ Smart cleanup hoàn thành!"
