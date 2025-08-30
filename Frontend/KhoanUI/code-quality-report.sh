#!/bin/bash

# Màu sắc cho output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== PHÂN TÍCH VÀ BÁO CÁO VẤN ĐỀ TIỀM ẨN TRONG DỰ ÁN ===${NC}"

# 1. Kiểm tra console.log
echo -e "\n${YELLOW}Kiểm tra console.log trong code...${NC}"
CONSOLE_LOGS=$(grep -r "console.log" --include="*.vue" --include="*.js" --include="*.ts" src)
if [ -n "$CONSOLE_LOGS" ]; then
    echo -e "${RED}Tìm thấy console.log trong code:${NC}"
    echo "$CONSOLE_LOGS"
    echo -e "${YELLOW}Gợi ý: Hãy xóa hoặc comment các console.log trước khi deploy production.${NC}"
else
    echo -e "${GREEN}Không tìm thấy console.log không cần thiết.${NC}"
fi

# 2. Kiểm tra v-for không có key
echo -e "\n${YELLOW}Kiểm tra v-for không có key...${NC}"
VFOR_NO_KEY=$(grep -r "v-for" --include="*.vue" src | grep -v ":key")
if [ -n "$VFOR_NO_KEY" ]; then
    echo -e "${RED}Tìm thấy v-for không có key:${NC}"
    echo "$VFOR_NO_KEY"
    echo -e "${YELLOW}Gợi ý: Luôn sử dụng :key với v-for để cải thiện hiệu suất và tránh lỗi.${NC}"
else
    echo -e "${GREEN}Tất cả v-for đều có key.${NC}"
fi

# 3. Kiểm tra các TODO trong code
echo -e "\n${YELLOW}Kiểm tra TODO trong code...${NC}"
TODOS=$(grep -r "TODO" --include="*.vue" --include="*.js" --include="*.ts" src)
if [ -n "$TODOS" ]; then
    echo -e "${YELLOW}Tìm thấy TODO trong code:${NC}"
    echo "$TODOS"
    echo -e "${YELLOW}Gợi ý: Xem xét hoàn thành các TODO trước khi release.${NC}"
else
    echo -e "${GREEN}Không tìm thấy TODO trong code.${NC}"
fi

# 4. Kiểm tra các file lớn (có thể cần refactor)
echo -e "\n${YELLOW}Kiểm tra file quá lớn...${NC}"
LARGE_FILES=$(find src -name "*.vue" -o -name "*.js" -o -name "*.ts" | xargs wc -l | sort -nr | head -10)
echo -e "${BLUE}Top 10 file lớn nhất:${NC}"
echo "$LARGE_FILES"
echo -e "${YELLOW}Gợi ý: Xem xét chia nhỏ các file lớn để dễ bảo trì.${NC}"

# 5. Kiểm tra các import không sử dụng
echo -e "\n${YELLOW}Kiểm tra các import không sử dụng...${NC}"
echo -e "${YELLOW}Gợi ý: Sử dụng ESLint với rule 'no-unused-vars' để phát hiện.${NC}"

# 6. Kiểm tra các component không sử dụng Composition API
echo -e "\n${YELLOW}Kiểm tra các component không sử dụng Composition API...${NC}"
OPTION_API=$(grep -r "export default {" --include="*.vue" src)
if [ -n "$OPTION_API" ]; then
    echo -e "${YELLOW}Tìm thấy component sử dụng Options API:${NC}"
    echo "$OPTION_API"
    echo -e "${YELLOW}Gợi ý: Xem xét chuyển sang Composition API với <script setup>.${NC}"
else
    echo -e "${GREEN}Tất cả component đều sử dụng Composition API.${NC}"
fi

# 7. Kiểm tra prop validation
echo -e "\n${YELLOW}Kiểm tra prop validation...${NC}"
DEFINE_PROPS=$(grep -r "defineProps" --include="*.vue" src | grep -v "type:")
if [ -n "$DEFINE_PROPS" ]; then
    echo -e "${YELLOW}Tìm thấy defineProps không có type validation:${NC}"
    echo "$DEFINE_PROPS"
    echo -e "${YELLOW}Gợi ý: Thêm type validation cho props để cải thiện an toàn type.${NC}"
else
    echo -e "${GREEN}Tất cả defineProps đều có type validation.${NC}"
fi

# 8. Kiểm tra các file thiếu JSDoc
echo -e "\n${YELLOW}Kiểm tra các function không có JSDoc...${NC}"
echo -e "${YELLOW}Gợi ý: Thêm JSDoc cho các function quan trọng để cải thiện khả năng đọc code.${NC}"

# 9. Kiểm tra inline styles
echo -e "\n${YELLOW}Kiểm tra inline styles...${NC}"
INLINE_STYLES=$(grep -r "style=\"" --include="*.vue" src)
if [ -n "$INLINE_STYLES" ]; then
    echo -e "${YELLOW}Tìm thấy inline styles:${NC}"
    echo "$INLINE_STYLES"
    echo -e "${YELLOW}Gợi ý: Xem xét chuyển sang CSS/SCSS classes.${NC}"
else
    echo -e "${GREEN}Không tìm thấy inline styles.${NC}"
fi

# 10. Kiểm tra kiểu dữ liệu any
echo -e "\n${YELLOW}Kiểm tra kiểu dữ liệu any...${NC}"
ANY_TYPES=$(grep -r ": any" --include="*.vue" --include="*.ts" src)
if [ -n "$ANY_TYPES" ]; then
    echo -e "${YELLOW}Tìm thấy kiểu dữ liệu any:${NC}"
    echo "$ANY_TYPES"
    echo -e "${YELLOW}Gợi ý: Thay thế 'any' bằng kiểu dữ liệu cụ thể.${NC}"
else
    echo -e "${GREEN}Không tìm thấy kiểu dữ liệu any.${NC}"
fi

echo -e "\n${BLUE}=== KẾT THÚC BÁO CÁO ===${NC}"
echo -e "${BLUE}Sử dụng 'npm run quality-check' để chạy kiểm tra toàn diện${NC}"
