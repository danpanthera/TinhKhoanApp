#!/bin/bash

echo "🧹 XÓA DUPLICATE ROLES - CHỈ GIỮ 23 ROLES CHUẨN"
echo "================================================"

API_BASE="http://localhost:5055/api"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

# Danh sách 23 roles chuẩn duy nhất
STANDARD_ROLES=(
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

echo "🔍 Lấy danh sách tất cả roles..."
all_roles=$(curl -s "$API_BASE/roles" | jq -r '.[] | "\(.Id):\(.Name)"')

echo "📊 Phân tích duplicate roles..."

# Track first occurrence of each role name
declare -A first_occurrence
kept_count=0
deleted_count=0

while IFS=':' read -r id name; do
    if [[ " ${STANDARD_ROLES[@]} " =~ " ${name} " ]]; then
        # This is a standard role
        if [[ -z "${first_occurrence[$name]}" ]]; then
            # First occurrence - keep it
            first_occurrence[$name]="$id"
            echo -e "${GREEN}✅ Giữ lại: $name (ID: $id)${NC}"
            ((kept_count++))
        else
            # Duplicate - delete it
            echo -e "${YELLOW}🗑️ Xóa duplicate: $name (ID: $id)${NC}"
            result=$(curl -s -X DELETE "$API_BASE/roles/$id")
            if [[ $? -eq 0 ]]; then
                echo -e "${GREEN}   ✅ Đã xóa thành công${NC}"
                ((deleted_count++))
            else
                echo -e "${RED}   ❌ Lỗi khi xóa: $result${NC}"
            fi
        fi
    else
        # Not a standard role - delete it
        echo -e "${RED}🗑️ Xóa role không chuẩn: $name (ID: $id)${NC}"
        result=$(curl -s -X DELETE "$API_BASE/roles/$id")
        if [[ $? -eq 0 ]]; then
            echo -e "${GREEN}   ✅ Đã xóa thành công${NC}"
            ((deleted_count++))
        else
            echo -e "${RED}   ❌ Lỗi khi xóa: $result${NC}"
        fi
    fi
    sleep 0.1
done <<< "$all_roles"

echo ""
echo "🎉 HOÀN THÀNH CLEANUP!"
echo ""

# Verify final count
final_count=$(curl -s "$API_BASE/roles" | jq 'length')
echo -e "${BLUE}📊 Kết quả:${NC}"
echo -e "${GREEN}   ✅ Roles giữ lại: $kept_count${NC}"
echo -e "${YELLOW}   🗑️ Roles đã xóa: $deleted_count${NC}"
echo -e "${BLUE}   📊 Tổng roles cuối cùng: $final_count${NC}"

if [ "$final_count" -eq 23 ]; then
    echo -e "${GREEN}🎯 THÀNH CÔNG: Đúng 23 roles chuẩn!${NC}"
else
    echo -e "${YELLOW}⚠️ Cần kiểm tra: Có $final_count roles (mục tiêu: 23)${NC}"
fi

echo ""
echo "📋 23 roles chuẩn còn lại:"
curl -s "$API_BASE/roles" | jq -r '.[] | "  - \(.Name): \(.Description)"' | head -23
