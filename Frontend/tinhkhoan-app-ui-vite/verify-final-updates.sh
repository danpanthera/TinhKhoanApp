#!/bin/bash

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${PURPLE}========================================${NC}"
echo -e "${PURPLE}  🔧 KIỂM TRA CẬP NHẬT CUỐI CÙNG${NC}"
echo -e "${PURPLE}========================================${NC}"

# 1. Kiểm tra bản quyền đã sửa
echo -e "\n${BLUE}📝 Kiểm tra bản quyền trong SoftwareInfoView...${NC}"
if grep -q "© 2025 Agribank Lai Châu" src/views/SoftwareInfoView.vue; then
    echo -e "${GREEN}✅ Bản quyền đã cập nhật: '© 2025 Agribank Lai Châu'${NC}"
else
    echo -e "${RED}❌ Bản quyền chưa được cập nhật${NC}"
fi

# 2. Kiểm tra cấu hình API URL
echo -e "\n${BLUE}🔗 Kiểm tra cấu hình API URL...${NC}"
if grep -q "http://localhost:5000/api" .env; then
    echo -e "${GREEN}✅ API URL đã sửa về port 5000${NC}"
else
    echo -e "${RED}❌ API URL chưa đúng${NC}"
fi

# 3. Kiểm tra backend server
echo -e "\n${BLUE}🖥️ Kiểm tra backend server...${NC}"
if lsof -ti:5000 | grep -q .; then
    echo -e "${GREEN}✅ Backend server đang chạy trên port 5000${NC}"
    
    # Test API endpoint
    if command -v curl >/dev/null 2>&1; then
        echo -e "${BLUE}🔍 Test API health check...${NC}"
        if curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/api/health 2>/dev/null | grep -q "200\|404"; then
            echo -e "${GREEN}✅ API server phản hồi${NC}"
        else
            echo -e "${YELLOW}⚠️  API health check không phản hồi (có thể endpoint không tồn tại)${NC}"
        fi
    fi
else
    echo -e "${RED}❌ Backend server không chạy trên port 5000${NC}"
    echo -e "${YELLOW}💡 Chạy: cd Backend/TinhKhoanApp.Api && dotnet run${NC}"
fi

# 4. Kiểm tra frontend server
echo -e "\n${BLUE}🌐 Kiểm tra frontend server...${NC}"
if lsof -ti:3001 > /dev/null; then
    echo -e "${GREEN}✅ Frontend server đang chạy trên port 3001${NC}"
else
    echo -e "${RED}❌ Frontend server không chạy trên port 3001${NC}"
fi

# 5. Kiểm tra error handling trong rawDataService
echo -e "\n${BLUE}⚠️  Kiểm tra error handling...${NC}"
if grep -q "fallbackData" src/services/rawDataService.js; then
    echo -e "${GREEN}✅ Đã thêm fallback data cho demo${NC}"
else
    echo -e "${RED}❌ Chưa thêm fallback data${NC}"
fi

if grep -q "getMockData" src/services/rawDataService.js; then
    echo -e "${GREEN}✅ Đã thêm mock data function${NC}"
else
    echo -e "${RED}❌ Chưa thêm mock data function${NC}"
fi

# 6. Kiểm tra improved error messages
echo -e "\n${BLUE}💬 Kiểm tra thông báo lỗi cải thiện...${NC}"
if grep -q "HƯỚNG DẪN KHẮC PHỤC" src/views/DataImportView.vue; then
    echo -e "${GREEN}✅ Đã thêm hướng dẫn khắc phục lỗi${NC}"
else
    echo -e "${RED}❌ Chưa thêm hướng dẫn khắc phục${NC}"
fi

# 7. Test URLs
echo -e "\n${BLUE}🔗 Các URL test:${NC}"
echo -e "  🏠 Trang chủ: ${GREEN}http://localhost:3001/${NC}"
echo -e "  📊 Dashboard: ${GREEN}http://localhost:3001/#/dashboard/business-plan${NC}"
echo -e "  📂 Kho dữ liệu thô: ${GREEN}http://localhost:3001/#/data-import${NC}"
echo -e "  ℹ️  Thông tin phần mềm: ${GREEN}http://localhost:3001/#/about/software-info${NC}"
echo -e "  🔧 Backend API: ${GREEN}http://localhost:5000/api${NC}"

# 8. Kiểm tra toàn bộ
echo -e "\n${PURPLE}========================================${NC}"
echo -e "${PURPLE}  📋 TÓM TẮT KIỂM TRA${NC}"
echo -e "${PURPLE}========================================${NC}"

total_checks=6
passed_checks=0

# Count passed checks
grep -q "© 2025 Agribank Lai Châu" src/views/SoftwareInfoView.vue && ((passed_checks++))
grep -q "http://localhost:5000/api" .env && ((passed_checks++))
lsof -ti:5000 | grep -q . && ((passed_checks++))
lsof -ti:3001 > /dev/null && ((passed_checks++))
grep -q "fallbackData" src/services/rawDataService.js && ((passed_checks++))
grep -q "HƯỚNG DẪN KHẮC PHỤC" src/views/DataImportView.vue && ((passed_checks++))

echo -e "Đã hoàn thành: ${GREEN}$passed_checks${NC}/${total_checks} kiểm tra"

if [ $passed_checks -eq $total_checks ]; then
    echo -e "\n${GREEN}🎉 TẤT CẢ CẬP NHẬT THÀNH CÔNG!${NC}"
    echo -e "${GREEN}✨ Hệ thống đã sẵn sàng${NC}"
    
    echo -e "\n${BLUE}🧪 HƯỚNG DẪN TEST:${NC}"
    echo -e "1. ${GREEN}Mở http://localhost:3001/#/about/software-info${NC}"
    echo -e "   👀 Kiểm tra bản quyền '© 2025 Agribank Lai Châu'"
    echo -e "2. ${GREEN}Mở http://localhost:3001/#/data-import${NC}"
    echo -e "   👀 Trang sẽ load thành công thay vì báo lỗi"
    echo -e "   👀 Có mock data hiển thị nếu API chưa có"
    echo -e "3. ${GREEN}Test error handling${NC}"
    echo -e "   👀 Stop backend server và reload trang data-import"
    echo -e "   👀 Sẽ hiện thông báo lỗi chi tiết + hướng dẫn khắc phục"
else
    echo -e "\n${YELLOW}⚠️  Còn $(($total_checks - $passed_checks)) vấn đề cần kiểm tra${NC}"
fi

echo -e "\n${PURPLE}========================================${NC}"
