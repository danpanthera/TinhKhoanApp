#!/bin/bash

# Script gán Employees vào Roles cho hệ thống TinhKhoanApp
# Ngày tạo: 06/07/2025
# Mục đích: Gán vai trò phù hợp cho từng nhân viên dựa trên vị trí và đơn vị

BASE_URL_EMPLOYEES="http://localhost:5055/api/employees"
BASE_URL_ROLES="http://localhost:5055/api/roles"
BASE_URL_UNITS="http://localhost:5055/api/units"

echo "🎯 BẮT ĐẦU GÁN EMPLOYEES VÀO ROLES"
echo "=================================="

# 1. KIỂM TRA BACKEND
echo ""
echo "📊 1. KIỂM TRA BACKEND:"
health_check=$(curl -s "http://localhost:5055/health" | jq -r '.status' 2>/dev/null)
if [ "$health_check" = "Healthy" ]; then
    echo "   ✅ Backend đang hoạt động bình thường"
else
    echo "   ❌ Backend không hoạt động. Vui lòng khởi động backend trước!"
    exit 1
fi

# 2. KIỂM TRA DỮ LIỆU HIỆN TẠI
echo ""
echo "📋 2. KIỂM TRA DỮ LIỆU HIỆN TẠI:"
employees_count=$(curl -s "$BASE_URL_EMPLOYEES" | jq '. | length' 2>/dev/null)
roles_count=$(curl -s "$BASE_URL_ROLES" | jq '. | length' 2>/dev/null)
units_count=$(curl -s "$BASE_URL_UNITS" | jq '. | length' 2>/dev/null)

echo "   👥 Employees: $employees_count"
echo "   🎭 Roles: $roles_count"
echo "   🏢 Units: $units_count"

# 3. PHÂN TÍCH VÀ HIỂN THỊ EMPLOYEES HIỆN TẠI
echo ""
echo "📋 3. DANH SÁCH EMPLOYEES HIỆN TẠI:"
curl -s "$BASE_URL_EMPLOYEES" | jq -r '.[] | "   ID: \(.Id) | \(.FullName) | \(.UnitName) | \(.PositionName)"' 2>/dev/null

# 4. HIỂN THỊ ROLES HIỆN TẠI
echo ""
echo "🎭 4. DANH SÁCH ROLES HIỆN TẠI:"
curl -s "$BASE_URL_ROLES" | jq -r '.[] | "   ID: \(.Id) | \(.Name) | \(.Description)"' 2>/dev/null | head -10

# 5. ĐỀ XUẤT GÁN ROLES CHO EMPLOYEES
echo ""
echo "💡 5. ĐỀ XUẤT GÁN ROLES:"
echo "   Dựa trên PositionName và UnitName của từng Employee:"
echo ""

# Lấy danh sách employees
employees_data=$(curl -s "$BASE_URL_EMPLOYEES" 2>/dev/null)

echo "$employees_data" | jq -r '.[] | "   \(.FullName) (\(.PositionName)) tại \(.UnitName)"' | while read line; do
    echo "   📝 $line"

    # Phân tích và đề xuất role phù hợp
    if [[ "$line" == *"Quản Trị Viên"* ]]; then
        echo "      → Đề xuất Role: Admin/System roles"
    elif [[ "$line" == *"Giám đốc"* ]] && [[ "$line" == *"Hội Sở"* ]]; then
        echo "      → Đề xuất Role: GiamdocCnl1"
    elif [[ "$line" == *"Giám đốc"* ]] && [[ "$line" == *"Chi nhánh"* ]]; then
        echo "      → Đề xuất Role: GiamdocCnl2"
    elif [[ "$line" == *"Phó Giám đốc"* ]]; then
        echo "      → Đề xuất Role: PhogiamdocCnl1/PhogiamdocCnl2"
    elif [[ "$line" == *"Trưởng phòng"* ]] && [[ "$line" == *"Khách hàng Doanh nghiệp"* ]]; then
        echo "      → Đề xuất Role: TruongphongKhdn"
    elif [[ "$line" == *"Trưởng phòng"* ]] && [[ "$line" == *"Khách hàng Cá nhân"* ]]; then
        echo "      → Đề xuất Role: TruongphongKhcn"
    elif [[ "$line" == *"Trưởng phòng"* ]] && [[ "$line" == *"Kế toán"* ]]; then
        echo "      → Đề xuất Role: TruongphongKtnqCnl1/TruongphongKtnqCnl2"
    elif [[ "$line" == *"Trưởng phòng"* ]] && [[ "$line" == *"Kế hoạch"* ]]; then
        echo "      → Đề xuất Role: TruongphongKhqlrr"
    else
        echo "      → Đề xuất Role: CBItThKtgsKhqlrr (cán bộ chung)"
    fi
    echo ""
done

# 6. THỰC HIỆN GÁN ROLES (MẪU)
echo ""
echo "🔧 6. THỰC HIỆN GÁN ROLES MẪU:"
echo "   (Hiện tại chỉ hiển thị logic, chưa thực thi thực tế)"
echo "   Cần API endpoint để gán Employee vào Role"

# 7. KẾT QUẢ
echo ""
echo "📊 7. TỔNG KẾT:"
echo "   ✅ Đã phân tích $employees_count employees"
echo "   ✅ Có $roles_count roles sẵn sàng để gán"
echo "   📋 Cần tạo API endpoint để gán Employee-Role relationship"

echo ""
echo "🏁 HOÀN THÀNH PHÂN TÍCH GÁN ROLES"
echo "================================="

echo ""
echo "📝 GHI CHÚ:"
echo "   - Employees đã có UnitId và PositionId"
echo "   - Roles đã có đầy đủ 23 vai trò"
echo "   - Cần endpoint PUT/POST để gán Employee vào Role"
echo "   - Sau khi gán xong sẽ tạo EmployeeKpiAssignments"
