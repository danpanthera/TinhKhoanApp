#!/bin/bash

# Màu sắc cho output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== KIỂM TRA VÀ CHUẨN BỊ CHO PRODUCTION ===${NC}"

cd "$(dirname "$0")"

# 1. Kiểm tra console.log và tự động comment out
echo -e "\n${YELLOW}Đang kiểm tra và xử lý console.log...${NC}"
CONSOLE_LOG_FILES=$(grep -l "console.log" --include="*.vue" --include="*.js" --include="*.ts" src)

if [ -n "$CONSOLE_LOG_FILES" ]; then
    echo -e "${YELLOW}Đang comment out console.log trong các file:${NC}"
    for file in $CONSOLE_LOG_FILES; do
        echo "  - $file"
        # Comment out console.log lines
        sed -i '' 's/console\.log\(.*\)/\/\/ console.log\1 \/\/ Commented out for production/g' "$file"
    done
    echo -e "${GREEN}Đã comment out tất cả console.log.${NC}"
else
    echo -e "${GREEN}Không tìm thấy console.log không cần thiết.${NC}"
fi

# 2. Kiểm tra v-for không có key
echo -e "\n${YELLOW}Đang kiểm tra v-for không có key...${NC}"
VFOR_NO_KEY=$(grep -r "v-for" --include="*.vue" src | grep -v ":key")
if [ -n "$VFOR_NO_KEY" ]; then
    echo -e "${RED}Tìm thấy v-for không có key:${NC}"
    echo "$VFOR_NO_KEY"
    echo -e "${RED}Vui lòng thêm :key cho tất cả v-for trước khi build production.${NC}"
    exit 1
else
    echo -e "${GREEN}Tất cả v-for đều có key.${NC}"
fi

# 3. Kiểm tra các TODO còn sót lại
echo -e "\n${YELLOW}Đang kiểm tra TODO trong code...${NC}"
TODOS=$(grep -r "TODO" --include="*.vue" --include="*.js" --include="*.ts" src)
if [ -n "$TODOS" ]; then
    echo -e "${YELLOW}Tìm thấy TODO trong code:${NC}"
    echo "$TODOS"
    echo -e "${YELLOW}Cân nhắc xử lý các TODO trước khi triển khai production.${NC}"
fi

# 4. Chạy TypeScript check
echo -e "\n${YELLOW}Đang kiểm tra TypeScript...${NC}"
npx vue-tsc --noEmit
if [ $? -eq 0 ]; then
    echo -e "${GREEN}TypeScript check passed.${NC}"
else
    echo -e "${RED}TypeScript check failed. Vui lòng sửa các lỗi trước khi build production.${NC}"
    exit 1
fi

# 5. Chạy ESLint
echo -e "\n${YELLOW}Đang kiểm tra ESLint...${NC}"
npx eslint --ext .js,.vue,.ts src
if [ $? -eq 0 ]; then
    echo -e "${GREEN}ESLint check passed.${NC}"
else
    echo -e "${RED}ESLint check failed. Vui lòng sửa các lỗi trước khi build production.${NC}"
    exit 1
fi

# 6. Tối ưu images (nếu có imagemin)
if command -v npx &> /dev/null && npm list -g imagemin &> /dev/null; then
    echo -e "\n${YELLOW}Đang tối ưu images...${NC}"
    npx imagemin public/images/* --out-dir=public/images
    echo -e "${GREEN}Đã tối ưu images.${NC}"
fi

# 7. Kiểm tra build size
echo -e "\n${YELLOW}Đang build và phân tích kích thước...${NC}"
npm run build
if [ $? -eq 0 ]; then
    echo -e "${GREEN}Build thành công.${NC}"

    # Phân tích kích thước build (đơn giản)
    echo -e "\n${YELLOW}Kích thước build:${NC}"
    du -sh dist
    echo -e "\n${YELLOW}Chi tiết các file:${NC}"
    find dist -type f -name "*.js" -o -name "*.css" | xargs du -sh | sort -hr
else
    echo -e "${RED}Build thất bại.${NC}"
    exit 1
fi

echo -e "\n${GREEN}Quá trình chuẩn bị cho production đã hoàn tất.${NC}"
echo -e "${BLUE}Sử dụng 'npm run preview' để kiểm tra build trước khi triển khai.${NC}"
