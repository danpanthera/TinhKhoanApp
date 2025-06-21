#!/bin/bash

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${PURPLE}========================================${NC}"
echo -e "${PURPLE}  🚀 KIỂM TRA DASHBOARD TỔNG HỢP${NC}"
echo -e "${PURPLE}========================================${NC}"

# 1. Kiểm tra ảnh nền
echo -e "\n${BLUE}📸 Kiểm tra ảnh nền...${NC}"
bg_count=$(ls -1 public/images/backgrounds/*.{jpg,jpeg,png,webp,gif} 2>/dev/null | wc -l)
echo -e "Số ảnh nền hiện có: ${GREEN}$bg_count${NC}"

if [ $bg_count -ge 9 ]; then
    echo -e "${GREEN}✅ Đủ ảnh nền (≥9 ảnh)${NC}"
else
    echo -e "${YELLOW}⚠️  Chỉ có $bg_count ảnh, khuyến nghị ≥9 ảnh${NC}"
fi

# Hiển thị danh sách ảnh
echo -e "\n${BLUE}📋 Danh sách ảnh nền:${NC}"
ls -1 public/images/backgrounds/*.{jpg,jpeg,png,webp,gif} 2>/dev/null | while read img; do
    filename=$(basename "$img")
    echo -e "  • ${GREEN}$filename${NC}"
done

# 2. Kiểm tra menu đã đổi tên
echo -e "\n${BLUE}📝 Kiểm tra tên menu...${NC}"
if grep -q "Cập nhật số liệu" src/App.vue; then
    echo -e "${GREEN}✅ Menu 'Dashboard tính toán' → 'Cập nhật số liệu'${NC}"
else
    echo -e "${RED}❌ Chưa đổi tên menu 'Dashboard tính toán'${NC}"
fi

if grep -q "DASHBOARD TỔNG HỢP" src/App.vue; then
    echo -e "${GREEN}✅ Menu 'Dashboard KHKD' → 'DASHBOARD TỔNG HỢP'${NC}"
else
    echo -e "${RED}❌ Chưa đổi tên menu 'Dashboard KHKD'${NC}"
fi

# 3. Kiểm tra header dashboard
echo -e "\n${BLUE}📊 Kiểm tra header dashboard...${NC}"
if grep -q "Dashboard Tổng Hợp Chỉ Tiêu Kinh Doanh" src/views/dashboard/BusinessPlanDashboard.vue; then
    echo -e "${GREEN}✅ Header dashboard đã cập nhật${NC}"
else
    echo -e "${RED}❌ Header dashboard chưa cập nhật${NC}"
fi

# 4. Kiểm tra CSS dropdown
echo -e "\n${BLUE}🎨 Kiểm tra CSS dropdown đơn vị...${NC}"
if grep -q "min-width: 200px" src/views/dashboard/BusinessPlanDashboard.vue; then
    echo -e "${GREEN}✅ Dropdown đơn vị đã mở rộng width${NC}"
else
    echo -e "${RED}❌ Dropdown đơn vị chưa mở rộng width${NC}"
fi

# 5. Kiểm tra logic dynamic data
echo -e "\n${BLUE}⚙️  Kiểm tra logic dynamic data...${NC}"
if grep -q "unitMultiplier" src/views/dashboard/BusinessPlanDashboard.vue; then
    echo -e "${GREEN}✅ Logic dynamic data theo unit đã thêm${NC}"
else
    echo -e "${RED}❌ Logic dynamic data chưa thêm${NC}"
fi

# 6. Kiểm tra server
echo -e "\n${BLUE}🔧 Kiểm tra trạng thái server...${NC}"
if lsof -ti:3001 > /dev/null; then
    echo -e "${GREEN}✅ Server đang chạy trên port 3001${NC}"
    
    # Test HTTP request
    if command -v curl >/dev/null 2>&1; then
        if curl -s -o /dev/null -w "%{http_code}" http://localhost:3001 | grep -q "200"; then
            echo -e "${GREEN}✅ Server phản hồi HTTP 200 OK${NC}"
        else
            echo -e "${YELLOW}⚠️  Server có thể chưa sẵn sàng${NC}"
        fi
    fi
else
    echo -e "${RED}❌ Server không chạy trên port 3001${NC}"
    echo -e "${YELLOW}💡 Chạy: npm run dev để khởi động server${NC}"
fi

# 7. Test links dashboard
echo -e "\n${BLUE}🔗 Các link test dashboard:${NC}"
echo -e "  🏠 Trang chủ: ${GREEN}http://localhost:3001/${NC}"
echo -e "  📊 Dashboard Tổng Hợp: ${GREEN}http://localhost:3001/#/dashboard/business-plan${NC}"
echo -e "  🎯 Giao chỉ tiêu: ${GREEN}http://localhost:3001/#/dashboard/target-assignment${NC}"
echo -e "  🧮 Cập nhật số liệu: ${GREEN}http://localhost:3001/#/dashboard/calculation${NC}"

# 8. Tóm tắt kết quả
echo -e "\n${PURPLE}========================================${NC}"
echo -e "${PURPLE}  📋 TÓM TẮT KIỂM TRA${NC}"
echo -e "${PURPLE}========================================${NC}"

total_checks=6
passed_checks=0

# Count passed checks
[ $bg_count -ge 9 ] && ((passed_checks++))
grep -q "Cập nhật số liệu" src/App.vue && ((passed_checks++))
grep -q "DASHBOARD TỔNG HỢP" src/App.vue && ((passed_checks++))
grep -q "Dashboard Tổng Hợp Chỉ Tiêu Kinh Doanh" src/views/dashboard/BusinessPlanDashboard.vue && ((passed_checks++))
grep -q "min-width: 200px" src/views/dashboard/BusinessPlanDashboard.vue && ((passed_checks++))
grep -q "unitMultiplier" src/views/dashboard/BusinessPlanDashboard.vue && ((passed_checks++))

echo -e "Đã hoàn thành: ${GREEN}$passed_checks${NC}/${total_checks} kiểm tra"

if [ $passed_checks -eq $total_checks ]; then
    echo -e "\n${GREEN}🎉 TẤT CẢ KIỂM TRA THÀNH CÔNG!${NC}"
    echo -e "${GREEN}✨ Dashboard đã sẵn sàng sử dụng${NC}"
else
    echo -e "\n${YELLOW}⚠️  Còn $(($total_checks - $passed_checks)) vấn đề cần kiểm tra${NC}"
fi

echo -e "\n${BLUE}🔥 DEMO DASHBOARD:${NC}"
echo -e "   ${GREEN}Mở browser và truy cập: http://localhost:3001/#/dashboard/business-plan${NC}"
echo -e "   ${GREEN}Thử chọn các đơn vị khác nhau để xem data thay đổi${NC}"

echo -e "\n${PURPLE}========================================${NC}"
