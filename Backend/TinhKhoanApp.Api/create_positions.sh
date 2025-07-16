#!/bin/bash

# =======================================================================
# TẠO CÁC CHỨC VỤ (POSITIONS) CƠ BẢN
# =======================================================================

echo "👔 TẠO CÁC CHỨC VỤ CƠ BẢN..."
echo "======================================================================"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

API_BASE="http://localhost:5055/api"

echo -e "${BLUE}👔 Tạo các chức vụ cơ bản...${NC}"

# Danh sách chức vụ cơ bản
declare -a positions=(
    "Giám đốc|Giám đốc Chi nhánh|Lãnh đạo cấp cao của chi nhánh"
    "Phó Giám đốc|Phó Giám đốc Chi nhánh|Lãnh đạo cấp cao phụ trách các mảng"
    "Trưởng phòng|Trưởng phòng|Lãnh đạo đơn vị trực thuộc"
    "Phó phòng|Phó phòng|Phụ trách phòng ban"
    "Chuyên viên|Chuyên viên|Nhân viên chuyên môn"
    "Cán bộ|Cán bộ|Nhân viên thực hiện"
    "Giao dịch viên|Giao dịch viên|Nhân viên giao dịch"
    "Thủ quỹ|Thủ quỹ|Nhân viên thủ quỹ"
    "Kế toán|Kế toán|Nhân viên kế toán"
    "Kiểm soát viên|Kiểm soát viên|Nhân viên kiểm soát"
)

SUCCESS_COUNT=0

for position in "${positions[@]}"; do
    IFS='|' read -r name title description <<< "$position"

    echo "📝 Tạo chức vụ: $title"

    response=$(curl -s -X POST "$API_BASE/positions" \
        -H "Content-Type: application/json" \
        -d "{
            \"Name\": \"$name\",
            \"Title\": \"$title\",
            \"Description\": \"$description\"
        }")

    if echo "$response" | jq -e '.Id' >/dev/null 2>&1; then
        position_id=$(echo "$response" | jq -r '.Id')
        echo -e "   ${GREEN}✅ Thành công - ID: $position_id${NC}"
        ((SUCCESS_COUNT++))
    else
        echo -e "   ${RED}❌ Lỗi: $response${NC}"
    fi
done

echo ""
echo -e "${GREEN}🎉 HOÀN THÀNH: Đã tạo $SUCCESS_COUNT/10 chức vụ${NC}"

# Verification
echo ""
echo "🔍 Kiểm tra danh sách positions:"
curl -s "$API_BASE/positions" | jq -r '.[] | "\(.Id): \(.Name) - \(.Description)"'

echo ""
echo "📊 Tổng số positions hiện tại:"
curl -s "$API_BASE/positions" | jq '. | length'

if [ $SUCCESS_COUNT -eq 10 ]; then
    echo -e "${GREEN}✅ HOÀN THÀNH: 10 chức vụ đã được tạo để hỗ trợ CRUD${NC}"
else
    echo -e "${RED}⚠️ CẢNH BÁO: Chỉ tạo được $SUCCESS_COUNT/10 chức vụ${NC}"
fi
