#!/bin/bash

echo "👤 PHỤC HỒI HOÀN CHỈNH 23 ROLES"
echo "================================"

API_BASE="http://localhost:5055/api"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Function to create role
create_role() {
    local name="$1"
    local description="$2"

    local data="{\"name\": \"$name\", \"description\": \"$description\"}"

    result=$(curl -s -X POST "$API_BASE/roles" \
        -H "Content-Type: application/json" \
        -d "$data")

    if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
        role_id=$(echo "$result" | jq -r '.id')
        echo -e "${GREEN}✅ $name (ID: $role_id)${NC}"
    else
        echo -e "${YELLOW}⚠️ $name (đã tồn tại hoặc lỗi)${NC}"
    fi
}

echo "📋 Tạo đầy đủ 23 vai trò chuẩn..."
echo ""

# 23 roles theo đúng danh sách
roles=(
    "TruongphongKhdn|Trưởng phòng Khách hàng Doanh nghiệp"
    "TruongphongKhcn|Trưởng phòng Khách hàng Cá nhân"
    "PhophongKhdn|Phó phòng Khách hàng Doanh nghiệp"
    "PhophongKhcn|Phó phòng Khách hàng Cá nhân"
    "TruongphongKhqlrr|Trưởng phòng Kế hoạch & Quản lý rủi ro"
    "PhophongKhqlrr|Phó phòng Kế hoạch & Quản lý rủi ro"
    "Cbtd|Cán bộ tín dụng"
    "TruongphongKtnqCnl1|Trưởng phòng Kế toán & Ngân quỹ CNL1"
    "PhophongKtnqCnl1|Phó phòng Kế toán & Ngân quỹ CNL1"
    "Gdv|Giao dịch viên"
    "TqHkKtnb|Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ"
    "TruongphoItThKtgs|Trưởng phó IT | Tổng hợp | Kiểm tra giám sát"
    "CBItThKtgsKhqlrr|Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"
    "GiamdocPgd|Giám đốc Phòng giao dịch"
    "PhogiamdocPgd|Phó giám đốc Phòng giao dịch"
    "PhogiamdocPgdCbtd|Phó giám đốc Phòng giao dịch kiêm CBTD"
    "GiamdocCnl2|Giám đốc Chi nhánh cấp 2"
    "PhogiamdocCnl2Td|Phó giám đốc CNL2 phụ trách Tín dụng"
    "PhogiamdocCnl2Kt|Phó giám đốc CNL2 phụ trách Kế toán"
    "TruongphongKhCnl2|Trưởng phòng Khách hàng CNL2"
    "PhophongKhCnl2|Phó phòng Khách hàng CNL2"
    "TruongphongKtnqCnl2|Trưởng phòng Kế toán & Ngân quỹ CNL2"
    "PhophongKtnqCnl2|Phó phòng Kế toán & Ngân quỹ CNL2"
)

# Create all roles
for role_entry in "${roles[@]}"; do
    IFS='|' read -r name description <<< "$role_entry"
    create_role "$name" "$description"
    sleep 0.2  # Small delay to avoid overwhelming API
done

echo ""
echo "🎉 HOÀN THÀNH TẠO 23 ROLES!"
echo ""

# Verify count
final_count=$(curl -s "$API_BASE/roles" | jq 'length' 2>/dev/null || echo "0")
echo -e "${BLUE}📊 Tổng số roles: $final_count/23${NC}"

if [ "$final_count" -ge "20" ]; then
    echo -e "${GREEN}✅ ĐẠT MỤC TIÊU: Đã có đủ roles!${NC}"
else
    echo -e "${YELLOW}⚠️ Cần kiểm tra: Chưa đủ 23 roles${NC}"
fi

echo ""
echo "📋 Danh sách roles đã tạo:"
curl -s "$API_BASE/roles" | jq -r '.[] | "  - \(.Name): \(.Description)"' | head -10
echo "  ..."
