#!/bin/bash

# =======================================================================
# XÓA 46 VAI TRÒ THỪA, CHỈ GIỮ LẠI 23 VAI TRÒ CHUẨN THEO README_DAT
# =======================================================================

echo "🧹 XÓA VAI TRÒ THỪA, CHỈ GIỮ 23 VAI TRÒ CHUẨN..."
echo "======================================================================"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

API_BASE="http://localhost:5055/api"

echo -e "${YELLOW}📊 Kiểm tra tình trạng hiện tại...${NC}"
CURRENT_COUNT=$(curl -s "$API_BASE/roles" | jq '. | length')
echo "Số roles hiện tại: $CURRENT_COUNT"

echo ""
echo -e "${BLUE}🎯 Danh sách 23 vai trò chuẩn cần giữ lại:${NC}"

# Danh sách 23 vai trò chuẩn theo README_DAT
declare -a standard_roles=(
    "TruongphongKhdn"
    "TruongphongKhcn"
    "PhophongKhdn"
    "PhophongKhcn"
    "TruongphongKhqlrr"
    "PhophongKhqlrr"
    "Cbtd"
    "TruongphongKtnqCnl1"
    "PhophongKtnqCnl1"
    "Gdv"
    "TqHkKtnb"
    "TruongphoItThKtgs"
    "CBItThKtgsKhqlrr"
    "GiamdocPgd"
    "PhogiamdocPgd"
    "PhogiamdocPgdCbtd"
    "GiamdocCnl2"
    "PhogiamdocCnl2Td"
    "PhogiamdocCnl2Kt"
    "TruongphongKhCnl2"
    "PhophongKhCnl2"
    "TruongphongKtnqCnl2"
    "PhophongKtnqCnl2"
)

# Hiển thị danh sách
for i in "${!standard_roles[@]}"; do
    echo "$((i+1)). ${standard_roles[$i]}"
done

echo ""
echo -e "${YELLOW}🔍 Lấy danh sách tất cả roles hiện tại...${NC}"

# Lấy tất cả roles
ALL_ROLES=$(curl -s "$API_BASE/roles")
echo "$ALL_ROLES" > /tmp/all_roles.json

echo ""
echo -e "${BLUE}🗑️ Xóa các roles không thuộc danh sách chuẩn...${NC}"

DELETED_COUNT=0
KEPT_COUNT=0

# Duyệt qua tất cả roles và xóa những role không thuộc danh sách chuẩn
while IFS= read -r role; do
    role_id=$(echo "$role" | jq -r '.Id')
    role_name=$(echo "$role" | jq -r '.Name')

    # Kiểm tra xem role này có trong danh sách chuẩn không
    is_standard=false
    for standard_role in "${standard_roles[@]}"; do
        if [ "$role_name" == "$standard_role" ]; then
            is_standard=true
            break
        fi
    done

    if [ "$is_standard" = false ]; then
        echo "🗑️ Xóa role thừa: ID=$role_id, Name=$role_name"
        response=$(curl -s -X DELETE "$API_BASE/roles/$role_id")
        if echo "$response" | grep -q "error\|Error" 2>/dev/null; then
            echo -e "   ${RED}❌ Lỗi xóa: $response${NC}"
        else
            echo -e "   ${GREEN}✅ Đã xóa${NC}"
            ((DELETED_COUNT++))
        fi
    else
        echo "✅ Giữ lại role chuẩn: ID=$role_id, Name=$role_name"
        ((KEPT_COUNT++))
    fi
done < <(echo "$ALL_ROLES" | jq -c '.[]')

echo ""
echo -e "${GREEN}📊 KẾT QUẢ:${NC}"
echo "- Roles đã xóa: $DELETED_COUNT"
echo "- Roles được giữ: $KEPT_COUNT"

# Verification cuối cùng
echo ""
echo "🔍 Kiểm tra lại danh sách roles sau khi dọn dẹp:"
FINAL_COUNT=$(curl -s "$API_BASE/roles" | jq '. | length')
echo "Tổng số roles cuối cùng: $FINAL_COUNT"

if [ "$FINAL_COUNT" -eq 23 ]; then
    echo -e "${GREEN}🎉 HOÀN THÀNH: Chỉ còn đúng 23 vai trò chuẩn!${NC}"
else
    echo -e "${YELLOW}⚠️ CẢNH BÁO: Còn $FINAL_COUNT roles (mong muốn: 23)${NC}"
fi

echo ""
echo "📋 Danh sách 23 roles cuối cùng:"
curl -s "$API_BASE/roles" | jq -r '.[] | "\(.Id): \(.Name)"' | sort
