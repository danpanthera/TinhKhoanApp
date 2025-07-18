#!/bin/bash

echo "👨‍💼 TẠO EMPLOYEES VÀ EMPLOYEE ROLES"
echo "===================================="

API_BASE="http://localhost:5055/api"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

# Function to create employee
create_employee() {
    local code="$1"
    local name="$2"
    local email="$3"
    local unit_id="$4"
    local cb_code="CB$(printf "%03d" $((10#$(echo $code | sed 's/EMP//') + 100)))"
    local username=$(echo "$name" | sed 's/[[:space:]]//g' | iconv -t ascii//TRANSLIT 2>/dev/null || echo "user$code")

    local data="{
        \"employeeCode\": \"$code\",
        \"cbCode\": \"$cb_code\",
        \"fullName\": \"$name\",
        \"username\": \"$username\",
        \"passwordHash\": \"hashed_password_123\",
        \"email\": \"$email\",
        \"unitId\": $unit_id,
        \"positionId\": 1
    }"

    result=$(curl -s -X POST "$API_BASE/employees" \
        -H "Content-Type: application/json" \
        -d "$data")

    # Check if result contains id
    if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
        emp_id=$(echo "$result" | jq -r '.id')
        echo -e "${GREEN}✅ $name (ID: $emp_id, CB: $cb_code)${NC}"
        echo "$emp_id"
    else
        # Check if employee already exists by code
        existing=$(curl -s "$API_BASE/employees" | jq -r ".[] | select(.employeeCode == \"$code\") | .id" 2>/dev/null)
        if [ ! -z "$existing" ] && [ "$existing" != "null" ]; then
            echo -e "${YELLOW}⚠️ $name (đã tồn tại - ID: $existing)${NC}"
            echo "$existing"
        else
            echo -e "${RED}❌ $name (lỗi: $result)${NC}"
            echo "0"
        fi
    fi
}

echo "🔍 Kiểm tra units và roles..."
units_count=$(curl -s "$API_BASE/units" | jq 'length' 2>/dev/null || echo "0")
roles_count=$(curl -s "$API_BASE/roles" | jq 'length' 2>/dev/null || echo "0")
echo -e "${BLUE}� Units hiện có: $units_count, Roles hiện có: $roles_count${NC}"

if [ "$units_count" -lt 6 ] || [ "$roles_count" -lt 10 ]; then
    echo -e "${YELLOW}⚠️ Cần có ít nhất 6 units và 10 roles trước khi tạo employees${NC}"
fi

echo ""
echo "📋 Tạo employees mẫu..."

echo ""

# Tạo employees cho các units khác nhau
employees=(
    "EMP001|Nguyễn Văn An|nguyenvanan@agribank.com|1|1"  # CNL1 - TruongphongKhdn
    "EMP002|Trần Thị Bình|tranthibinh@agribank.com|2|2"  # HoiSo - TruongphongKhcn
    "EMP003|Lê Văn Cường|levancuong@agribank.com|1|5"    # CNL1 - Cbtd
    "EMP004|Phạm Thị Dung|phamthidung@agribank.com|3|6"  # BinhLu - Gdv
    "EMP005|Hoàng Văn Em|hoangvanem@agribank.com|4|7"    # PhongTho - GiamdocCnl2
    "EMP006|Vũ Thị Phương|vuthiphuong@agribank.com|2|3"  # HoiSo - PhophongKhdn
    "EMP007|Đặng Văn Giang|dangvangiang@agribank.com|5|4" # SinHo - PhophongKhcn
    "EMP008|Bùi Thị Hoa|buithihoa@agribank.com|1|8"      # CNL1 - TruongphongKtnqCnl1
    "EMP009|Ngô Văn Inh|ngovaninh@agribank.com|6|9"      # BumTo - PhophongKtnqCnl1
    "EMP010|Lý Thị Kim|lythikim@agribank.com|2|10"       # HoiSo - Gdv
    "EMP011|Hoàng Minh Tuấn|hoangminhtuan@agribank.com|1|11" # CNL1 - PhophongKtnqCnl1
    "EMP012|Võ Thị Lan|vothilan@agribank.com|3|12"       # BinhLu - TruongteamGd
    "EMP013|Đỗ Văn Mạnh|dovanmanh@agribank.com|2|13"     # HoiSo - PhongdoiCnl1
    "EMP014|Lê Thị Nga|lethinga@agribank.com|4|14"       # PhongTho - Nvkd
    "EMP015|Trịnh Văn Oai|trinhvanoai@agribank.com|5|15" # SinHo - Nvtn
)

created_employees=()

for emp_entry in "${employees[@]}"; do
    IFS='|' read -r code name email unit_id role_id <<< "$emp_entry"
    emp_id=$(create_employee "$code" "$name" "$email" "$unit_id")
    if [ "$emp_id" != "0" ]; then
        created_employees+=("$emp_id|$role_id")
    fi
    sleep 0.3
done

echo ""
echo "📋 Gán roles cho employees..."
echo ""

# Assign roles to employees
success_count=0
for emp_role in "${created_employees[@]}"; do
    IFS='|' read -r emp_id role_id <<< "$emp_role"

    # Get employee and role names for display
    emp_data=$(curl -s "$API_BASE/employees/$emp_id" 2>/dev/null)
    role_data=$(curl -s "$API_BASE/roles/$role_id" 2>/dev/null)

    emp_name=$(echo "$emp_data" | jq -r '.name // .Name // "Unknown"' 2>/dev/null)
    role_name=$(echo "$role_data" | jq -r '.name // .Name // "Unknown"' 2>/dev/null)

    # Try different role assignment endpoints
    assign_data="{\"employeeId\": $emp_id, \"roleId\": $role_id}"

    # Try endpoint 1: /employees/{id}/roles
    result1=$(curl -s -X POST "$API_BASE/employees/$emp_id/roles" \
        -H "Content-Type: application/json" \
        -d "$assign_data" 2>/dev/null)

    # Try endpoint 2: /employeeroles
    assign_data2="{\"EmployeeId\": $emp_id, \"RoleId\": $role_id}"
    result2=$(curl -s -X POST "$API_BASE/employeeroles" \
        -H "Content-Type: application/json" \
        -d "$assign_data2" 2>/dev/null)

    if echo "$result1$result2" | grep -q "success\|id\|Id"; then
        echo -e "${GREEN}✅ $emp_name → $role_name${NC}"
        ((success_count++))
    else
        echo -e "${YELLOW}⚠️ $emp_name → $role_name (lỗi hoặc đã có)${NC}"
    fi

    sleep 0.2
done

echo ""
echo "🎉 HOÀN THÀNH TẠO EMPLOYEES!"
echo ""

# Verify counts
emp_count=$(curl -s "$API_BASE/employees" | jq 'length' 2>/dev/null || echo "0")
echo -e "${BLUE}📊 Tổng số employees: $emp_count${NC}"
echo -e "${BLUE}📊 Role assignments thành công: $success_count/${#created_employees[@]}${NC}"

echo ""
echo "📋 Top 15 employees đã tạo:"
curl -s "$API_BASE/employees" | jq -r '.[] | "  - \(.name // .Name) (\(.employeeCode // .EmployeeCode)) - Unit: \(.unitId // .UnitId)"' 2>/dev/null | head -15
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
