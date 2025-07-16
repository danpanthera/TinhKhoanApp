#!/bin/bash

# =======================================================================
# TẠO DỮ LIỆU NHÂN VIÊN MẪU
# =======================================================================

echo "🧑‍💼 TẠO DỮ LIỆU NHÂN VIÊN MẪU..."
echo "======================================================================"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

API_BASE="http://localhost:5055/api"

echo -e "${BLUE}🧑‍💼 Tạo nhân viên mẫu cho các đơn vị...${NC}"

# Danh sách nhân viên mẫu
declare -a employees=(
    "Nguyễn Văn A|NVA001|CB001|nguyenvana|01/01/1980|0987654321|nguyenvana@agribank.com.vn|1|1"
    "Trần Thị B|TTB002|CB002|tranthib|15/03/1985|0987654322|tranthib@agribank.com.vn|2|2"
    "Lê Văn C|LVC003|CB003|levanc|20/05/1982|0987654323|levanc@agribank.com.vn|3|3"
    "Phạm Thị D|PTD004|CB004|phamthid|10/08/1987|0987654324|phamthid@agribank.com.vn|1|4"
    "Hoàng Văn E|HVE005|CB005|hoangvane|25/12/1983|0987654325|hoangvane@agribank.com.vn|2|5"
    "Ngô Thị F|NTF006|CB006|ngothif|30/06/1986|0987654326|ngothif@agribank.com.vn|7|2"
    "Vũ Văn G|VVG007|CB007|vuvang|12/02/1984|0987654327|vuvang@agribank.com.vn|8|3"
    "Đặng Thị H|DTH008|CB008|dangthih|18/09/1988|0987654328|dangthih@agribank.com.vn|9|4"
    "Bùi Văn I|BVI009|CB009|buivani|05/11/1981|0987654329|buivani@agribank.com.vn|10|1"
    "Lý Thị K|LTK010|CB010|lythik|22/04/1989|0987654330|lythik@agribank.com.vn|11|2"
)

SUCCESS_COUNT=0

for employee in "${employees[@]}"; do
    IFS='|' read -r full_name employee_code cb_code username dob phone email position_id unit_id <<< "$employee"

    echo "📝 Tạo nhân viên: $full_name"

    response=$(curl -s -X POST "$API_BASE/employees" \
        -H "Content-Type: application/json" \
        -d "{
            \"FullName\": \"$full_name\",
            \"EmployeeCode\": \"$employee_code\",
            \"CBCode\": \"$cb_code\",
            \"Username\": \"$username\",
            \"DateOfBirth\": \"$dob\",
            \"PhoneNumber\": \"$phone\",
            \"Email\": \"$email\",
            \"PositionId\": $position_id,
            \"UnitId\": $unit_id
        }")

    if echo "$response" | jq -e '.Id' >/dev/null 2>&1; then
        employee_id=$(echo "$response" | jq -r '.Id')
        echo -e "   ${GREEN}✅ Thành công - ID: $employee_id${NC}"
        ((SUCCESS_COUNT++))
    else
        echo -e "   ${RED}❌ Lỗi: $response${NC}"
    fi
done

echo ""
echo -e "${GREEN}🎉 HOÀN THÀNH: Đã tạo $SUCCESS_COUNT/10 nhân viên${NC}"

# Verification
echo ""
echo "🔍 Kiểm tra danh sách employees:"
curl -s "$API_BASE/employees" | jq -r '.[] | "\(.Id): \(.FullName) - \(.EmployeeCode)"'

echo ""
echo "📊 Tổng số employees hiện tại:"
curl -s "$API_BASE/employees" | jq '. | length'

if [ $SUCCESS_COUNT -eq 10 ]; then
    echo -e "${GREEN}✅ HOÀN THÀNH: 10 nhân viên đã được tạo${NC}"
else
    echo -e "${RED}⚠️ CẢNH BÁO: Chỉ tạo được $SUCCESS_COUNT/10 nhân viên${NC}"
fi
