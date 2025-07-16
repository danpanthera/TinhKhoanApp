#!/bin/bash

# =======================================================================
# KHÔI PHỤC 23 VAI TRÒ THEO README_DAT.md
# =======================================================================

echo "🎭 KHÔI PHỤC 23 VAI TRÒ THEO README_DAT..."
echo "======================================================================"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

API_BASE="http://localhost:5055/api"

# Xóa tất cả roles hiện tại
echo -e "${YELLOW}🧹 Xóa tất cả roles hiện tại...${NC}"
curl -s -X DELETE "$API_BASE/roles/clear-all" | jq '.'

echo ""
echo -e "${BLUE}🎭 Tạo 23 vai trò theo README_DAT...${NC}"

# Danh sách 23 vai trò theo README_DAT
declare -a roles=(
    "TruongphongKhdn|Trưởng phòng KHDN|Trưởng phòng Khách hàng Doanh nghiệp"
    "TruongphongKhcn|Trưởng phòng KHCN|Trưởng phòng Khách hàng Cá nhân"
    "PhophongKhdn|Phó phòng KHDN|Phó phòng Khách hàng Doanh nghiệp"
    "PhophongKhcn|Phó phòng KHCN|Phó phòng Khách hàng Cá nhân"
    "TruongphongKhqlrr|Trưởng phòng KH&QLRR|Trưởng phòng Kế hoạch & Quản lý rủi ro"
    "PhophongKhqlrr|Phó phòng KH&QLRR|Phó phòng Kế hoạch & Quản lý rủi ro"
    "Cbtd|Cán bộ tín dụng|Cán bộ tín dụng"
    "TruongphongKtnqCnl1|Trưởng phòng KTNQ CNL1|Trưởng phòng Kế toán & Ngân quỹ CNL1"
    "PhophongKtnqCnl1|Phó phòng KTNQ CNL1|Phó phòng Kế toán & Ngân quỹ CNL1"
    "Gdv|GDV|Giao dịch viên"
    "TqHkKtnb|Thủ quỹ | Hậu kiểm | KTNB|Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ"
    "TruongphoItThKtgs|Trưởng phó IT | Tổng hợp | KTGS|Trưởng phó IT | Tổng hợp | Kiểm tra giám sát"
    "CBItThKtgsKhqlrr|Cán bộ IT | Tổng hợp | KTGS | KH&QLRR|Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"
    "GiamdocPgd|Giám đốc Phòng giao dịch|Giám đốc Phòng giao dịch"
    "PhogiamdocPgd|Phó giám đốc Phòng giao dịch|Phó giám đốc Phòng giao dịch"
    "PhogiamdocPgdCbtd|Phó giám đốc PGD kiêm CBTD|Phó giám đốc Phòng giao dịch kiêm CBTD"
    "GiamdocCnl2|Giám đốc CNL2|Giám đốc Chi nhánh cấp 2"
    "PhogiamdocCnl2Td|Phó giám đốc CNL2 phụ trách TD|Phó giám đốc CNL2 phụ trách Tín dụng"
    "PhogiamdocCnl2Kt|Phó giám đốc CNL2 phụ trách KT|Phó giám đốc CNL2 phụ trách Kế toán"
    "TruongphongKhCnl2|Trưởng phòng KH CNL2|Trưởng phòng Khách hàng CNL2"
    "PhophongKhCnl2|Phó phòng KH CNL2|Phó phòng Khách hàng CNL2"
    "TruongphongKtnqCnl2|Trưởng phòng KTNQ CNL2|Trưởng phòng Kế toán & Ngân quỹ CNL2"
    "PhophongKtnqCnl2|Phó phòng KTNQ CNL2|Phó phòng Kế toán & Ngân quỹ CNL2"
)

SUCCESS_COUNT=0

for role in "${roles[@]}"; do
    IFS='|' read -r name display_name description <<< "$role"

    echo "📝 Tạo role: $display_name"

    response=$(curl -s -X POST "$API_BASE/roles" \
        -H "Content-Type: application/json" \
        -d "{
            \"Name\": \"$name\",
            \"Description\": \"$description\"
        }")

    if echo "$response" | jq -e '.Id' >/dev/null 2>&1; then
        role_id=$(echo "$response" | jq -r '.Id')
        echo -e "   ${GREEN}✅ Thành công - ID: $role_id${NC}"
        ((SUCCESS_COUNT++))
    else
        echo -e "   ${RED}❌ Lỗi: $response${NC}"
    fi
done

echo ""
echo -e "${GREEN}🎉 HOÀN THÀNH: Đã tạo $SUCCESS_COUNT/23 vai trò${NC}"

# Verification
echo ""
echo "🔍 Kiểm tra danh sách roles:"
curl -s "$API_BASE/roles" | jq -r '.[] | "\(.Id): \(.Name) - \(.Description)"' | head -10

echo ""
echo "📊 Tổng số roles hiện tại:"
curl -s "$API_BASE/roles" | jq '. | length'

if [ $SUCCESS_COUNT -eq 23 ]; then
    echo -e "${GREEN}✅ HOÀN THÀNH: 23 vai trò đã được tạo theo README_DAT${NC}"
else
    echo -e "${RED}⚠️ CẢNH BÁO: Chỉ tạo được $SUCCESS_COUNT/23 vai trò${NC}"
fi
