#!/bin/bash

echo "🔄 ===== PHỤC HỒI TOÀN BỘ DỮ LIỆU TINHKHOANAPP ===== 🔄"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# API Base URL
API_BASE="http://localhost:5055/api"

# Function to check if API is available
check_api() {
    echo -e "${BLUE}🔍 Kiểm tra API backend...${NC}"
    if curl -s "$API_BASE/../health" > /dev/null; then
        echo -e "${GREEN}✅ API backend đã sẵn sàng${NC}"
        return 0
    else
        echo -e "${RED}❌ API backend không hoạt động${NC}"
        return 1
    fi
}

# Function to restore units
restore_units() {
    echo -e "${BLUE}🏢 Kiểm tra và phục hồi Units...${NC}"

    current_count=$(curl -s "$API_BASE/units" | jq 'length' 2>/dev/null || echo "0")
    echo "📊 Hiện có: $current_count units"

    if [ "$current_count" -ge "40" ]; then
        echo -e "${GREEN}✅ Units đã đủ ($current_count units)${NC}"
        return 0
    fi

    echo "📋 Cần tạo thêm units..."

    # Create basic units (sample)
    units_to_create=(
        '{"name": "Hội Sở", "code": "HoiSo", "level": "CNL1"}'
        '{"name": "Chi nhánh Bình Lư", "code": "BinhLu", "level": "CNL2"}'
        '{"name": "Chi nhánh Phong Thổ", "code": "PhongTho", "level": "CNL2"}'
        '{"name": "Chi nhánh Sìn Hồ", "code": "SinHo", "level": "CNL2"}'
        '{"name": "Chi nhánh Bum Tở", "code": "BumTo", "level": "CNL2"}'
    )

    for unit_data in "${units_to_create[@]}"; do
        result=$(curl -s -X POST "$API_BASE/units" \
            -H "Content-Type: application/json" \
            -d "$unit_data")

        if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
            name=$(echo "$unit_data" | jq -r '.name')
            echo -e "${GREEN}✅ Tạo thành công: $name${NC}"
        else
            name=$(echo "$unit_data" | jq -r '.name')
            echo -e "${YELLOW}⚠️ Đã tồn tại hoặc lỗi: $name${NC}"
        fi
    done
}

# Function to restore roles
restore_roles() {
    echo -e "${BLUE}👤 Kiểm tra và phục hồi Roles...${NC}"

    current_count=$(curl -s "$API_BASE/roles" | jq 'length' 2>/dev/null || echo "0")
    echo "📊 Hiện có: $current_count roles"

    if [ "$current_count" -ge "20" ]; then
        echo -e "${GREEN}✅ Roles đã đủ ($current_count roles)${NC}"
        return 0
    fi

    echo "📋 Cần tạo thêm roles..."

    # Create basic roles
    roles_to_create=(
        '{"name": "TruongphongKhdn", "description": "Trưởng phòng Khách hàng Doanh nghiệp"}'
        '{"name": "TruongphongKhcn", "description": "Trưởng phòng Khách hàng Cá nhân"}'
        '{"name": "PhophongKhdn", "description": "Phó phòng Khách hàng Doanh nghiệp"}'
        '{"name": "PhophongKhcn", "description": "Phó phòng Khách hàng Cá nhân"}'
        '{"name": "Cbtd", "description": "Cán bộ tín dụng"}'
        '{"name": "Gdv", "description": "Giao dịch viên"}'
        '{"name": "GiamdocCnl2", "description": "Giám đốc Chi nhánh cấp 2"}'
    )

    for role_data in "${roles_to_create[@]}"; do
        result=$(curl -s -X POST "$API_BASE/roles" \
            -H "Content-Type: application/json" \
            -d "$role_data")

        if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
            name=$(echo "$role_data" | jq -r '.name')
            echo -e "${GREEN}✅ Tạo thành công: $name${NC}"
        else
            name=$(echo "$role_data" | jq -r '.name')
            echo -e "${YELLOW}⚠️ Đã tồn tại hoặc lỗi: $name${NC}"
        fi
    done
}

# Function to restore employees
restore_employees() {
    echo -e "${BLUE}👨‍💼 Kiểm tra và phục hồi Employees...${NC}"

    current_count=$(curl -s "$API_BASE/employees" | jq 'length' 2>/dev/null || echo "0")
    echo "📊 Hiện có: $current_count employees"

    if [ "$current_count" -ge "5" ]; then
        echo -e "${GREEN}✅ Employees đã đủ ($current_count employees)${NC}"
        return 0
    fi

    echo "📋 Cần tạo thêm employees..."

    # Create basic employees
    employees_to_create=(
        '{"employeeCode": "EMP001", "name": "Nguyễn Văn A", "email": "nguyenvana@agribank.com", "unitId": 1}'
        '{"employeeCode": "EMP002", "name": "Trần Thị B", "email": "tranthib@agribank.com", "unitId": 1}'
        '{"employeeCode": "EMP003", "name": "Lê Văn C", "email": "levanc@agribank.com", "unitId": 1}'
    )

    for emp_data in "${employees_to_create[@]}"; do
        result=$(curl -s -X POST "$API_BASE/employees" \
            -H "Content-Type: application/json" \
            -d "$emp_data")

        if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
            name=$(echo "$emp_data" | jq -r '.name')
            echo -e "${GREEN}✅ Tạo thành công: $name${NC}"
        else
            name=$(echo "$emp_data" | jq -r '.name')
            echo -e "${YELLOW}⚠️ Đã tồn tại hoặc lỗi: $name${NC}"
        fi
    done
}

# Function to check data tables
check_data_tables() {
    echo -e "${BLUE}📊 Kiểm tra Data Tables...${NC}"

    tables=("DP01" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01" "DPDA")

    for table in "${tables[@]}"; do
        count=$(curl -s "$API_BASE/DirectImport/table-counts" | jq -r ".\"$table\" // 0")
        if [ "$count" -gt "0" ]; then
            echo -e "${GREEN}✅ $table: $count records${NC}"
        else
            echo -e "${YELLOW}⚠️ $table: Chưa có dữ liệu${NC}"
        fi
    done
}

# Main execution
main() {
    echo "🕐 Bắt đầu lúc: $(date)"
    echo ""

    # Check API first
    if ! check_api; then
        echo -e "${RED}❌ Không thể kết nối API. Vui lòng khởi động backend trước.${NC}"
        exit 1
    fi

    # Restore data step by step
    restore_units
    echo ""

    restore_roles
    echo ""

    restore_employees
    echo ""

    check_data_tables
    echo ""

    # Final summary
    echo -e "${GREEN}🎉 ===== HOÀN THÀNH PHỤC HỒI DỮ LIỆU ===== 🎉${NC}"
    echo ""
    echo "📊 Tổng kết:"

    units_count=$(curl -s "$API_BASE/units" | jq 'length' 2>/dev/null || echo "0")
    roles_count=$(curl -s "$API_BASE/roles" | jq 'length' 2>/dev/null || echo "0")
    employees_count=$(curl -s "$API_BASE/employees" | jq 'length' 2>/dev/null || echo "0")

    echo "🏢 Units: $units_count"
    echo "👤 Roles: $roles_count"
    echo "👨‍💼 Employees: $employees_count"

    echo ""
    echo -e "${BLUE}📋 Bước tiếp theo:${NC}"
    echo "1. Import dữ liệu CSV cho 8 bảng data"
    echo "2. Tạo KPI assignments"
    echo "3. Thiết lập temporal tables + columnstore"
    echo ""
    echo "🕐 Hoàn thành lúc: $(date)"
}

# Run main function
main
