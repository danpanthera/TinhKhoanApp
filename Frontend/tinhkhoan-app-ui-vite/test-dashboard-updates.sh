#!/bin/bash

# 🔍 Script kiểm tra cập nhật Dashboard KHKD
# Kiểm tra 6 chỉ tiêu mới, logic hiệu suất tổng thể, và tiêu đề

echo "🏦 AGRIBANK LAI CHÂU - KIỂM TRA CẬP NHẬT DASHBOARD"
echo "=================================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✅ $2${NC}"
    else
        echo -e "${RED}❌ $2${NC}"
    fi
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite

echo "1. KIỂM TRA 6 CHỈ TIÊU KHKD MỚI"
echo "==============================="

dashboard_file="src/views/dashboard/BusinessPlanDashboard.vue"

# Kiểm tra 6 chỉ tiêu mới
indicators=(
    "Huy động vốn"
    "Dư nợ"
    "Tỷ lệ nợ xấu" 
    "Thu nợ đã XLRR"
    "Thu dịch vụ"
    "Lợi nhuận khoán tài chính"
)

for indicator in "${indicators[@]}"; do
    if grep -q "$indicator" "$dashboard_file"; then
        print_status 0 "Chỉ tiêu: $indicator"
    else
        print_status 1 "Chỉ tiêu: $indicator"
    fi
done

echo ""
echo "2. KIỂM TRA LOGIC HIỆU SUẤT TỔNG THỂ"
echo "==================================="

# Kiểm tra computed property achievedCount
if grep -q "achievedCount = computed" "$dashboard_file"; then
    print_status 0 "Logic achievedCount computed đã được cập nhật"
else
    print_status 1 "Logic achievedCount computed chưa được cập nhật"
fi

# Kiểm tra logic tính cho Tỷ lệ nợ xấu
if grep -q "TyLeNoXau.*actualValue <= indicator.planValue" "$dashboard_file"; then
    print_status 0 "Logic đặc biệt cho Tỷ lệ nợ xấu"
else
    print_status 1 "Logic đặc biệt cho Tỷ lệ nợ xấu"
fi

echo ""
echo "3. KIỂM TRA TIÊU ĐỀ DASHBOARD"
echo "============================"

# Kiểm tra tiêu đề mới
if grep -q "DASHBOARD CHỈ TIÊU KHKD" "$dashboard_file"; then
    print_status 0 "Tiêu đề đã được cập nhật thành 'DASHBOARD CHỈ TIÊU KHKD'"
else
    print_status 1 "Tiêu đề chưa được cập nhật"
fi

# Kiểm tra CSS font trắng
if grep -q "color: #FFFFFF" "$dashboard_file"; then
    print_status 0 "Font màu trắng cho tiêu đề"
else
    print_status 1 "Font màu trắng cho tiêu đề"
fi

echo ""
echo "4. KIỂM TRA GIAO DIỆN HIỆU SUẤT TỔNG THỂ"
echo "======================================="

# Kiểm tra CSS cải thiện
if grep -q "padding: 40px" "$dashboard_file"; then
    print_status 0 "Padding được tăng lên (40px)"
else
    print_status 1 "Padding chưa được tăng"
fi

if grep -q "width: 250px" "$dashboard_file"; then
    print_status 0 "Gauge size được tăng lên (250x125px)"
else
    print_status 1 "Gauge size chưa được tăng"
fi

if grep -q "font-size: 42px" "$dashboard_file"; then
    print_status 0 "Font size gauge value được tăng (42px)"
else
    print_status 1 "Font size gauge value chưa được tăng"
fi

echo ""
echo "5. KIỂM TRA SERVER HOẠT ĐỘNG"
echo "============================"

# Kiểm tra frontend server
if curl -s http://localhost:3000 > /dev/null; then
    print_status 0 "Frontend server đang chạy (port 3000)"
else
    print_status 1 "Frontend server không chạy"
fi

# Kiểm tra backend server
if curl -s http://localhost:5055 > /dev/null; then
    print_status 0 "Backend server đang chạy (port 5055)"
else
    print_status 1 "Backend server không chạy"
fi

echo ""
echo "6. TEST DASHBOARD TRÊN BROWSER"
echo "=============================="

print_info "Mở dashboard trong browser để kiểm tra visual..."

# Mở browser nếu có thể
if command -v open >/dev/null 2>&1; then
    open "http://localhost:3000/#/dashboard/business-plan" 2>/dev/null
    print_status 0 "Đã mở dashboard trong browser"
elif command -v xdg-open >/dev/null 2>&1; then
    xdg-open "http://localhost:3000/#/dashboard/business-plan" 2>/dev/null
    print_status 0 "Đã mở dashboard trong browser"
else
    print_info "Vui lòng mở http://localhost:3000/#/dashboard/business-plan trong browser"
fi

echo ""
echo "7. CHECKLIST CẬP NHẬT"
echo "===================="

print_info "Các điểm cần kiểm tra trong browser:"
echo "   📊 6 chỉ tiêu KHKD hiển thị đúng tên"
echo "   🎯 Hiệu suất tổng thể tính đúng số chỉ tiêu đạt"
echo "   📏 Phần hiệu suất tổng thể to hơn, dễ nhìn"
echo "   🎨 Tiêu đề 'DASHBOARD CHỈ TIÊU KHKD' màu trắng đẹp"
echo "   ⚡ Gauge chart lớn hơn và rõ ràng"

echo ""
echo "=================================================="
echo "🎯 KIỂM TRA HOÀN TẤT"
echo ""
print_info "Nếu tất cả items đều ✅, dashboard đã sẵn sàng!"
print_info "Nếu có items ❌, cần kiểm tra lại code và server."
echo ""
print_info "URL test: http://localhost:3000/#/dashboard/business-plan"
echo "=================================================="
