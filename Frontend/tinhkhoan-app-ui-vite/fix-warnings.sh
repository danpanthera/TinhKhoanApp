#!/bin/bash

# Script tìm và khắc phục các warning phổ biến trong dự án Vue.js
# Tác giả: Copilot
# Ngày tạo: 30/07/2025

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔍 Bắt đầu phân tích và khắc phục warnings...${NC}"

# Chuyển đến thư mục frontend
cd "$(dirname "$0")"

# Tìm và xử lý console.log trong code
echo -e "${BLUE}🔍 Đang tìm console.log trong code sản xuất...${NC}"
CONSOLE_LOGS=$(grep -r --include="*.vue" --include="*.js" --include="*.ts" "console\.log" src | grep -v "// console\.log" | grep -v "/\* console\.log" | wc -l | tr -d ' ')

if [ "$CONSOLE_LOGS" -gt 0 ]; then
  echo -e "${YELLOW}⚠️ Phát hiện $CONSOLE_LOGS câu lệnh console.log chưa được comment:${NC}"
  grep -r --include="*.vue" --include="*.js" --include="*.ts" -n "console\.log" src | grep -v "// console\.log" | grep -v "/\* console\.log" | head -10

  echo -e "${GREEN}💡 Gợi ý: Comment hoặc xóa console.log trong code sản xuất${NC}"

  read -p "Bạn có muốn comment tất cả console.log không? (y/n) " -n 1 -r
  echo
  if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${BLUE}🔧 Đang comment console.log...${NC}"

    # Thay thế console.log bằng // console.log trong các file
    find src -type f \( -name "*.js" -o -name "*.ts" -o -name "*.vue" \) -exec sed -i '' 's/console\.log/\/\/ console.log/g' {} \;

    echo -e "${GREEN}✅ Đã comment tất cả console.log!${NC}"
  fi
fi

# Tìm và xử lý các component không đặt tên theo quy ước
echo -e "${BLUE}🔍 Kiểm tra tên component...${NC}"
COMPONENT_FILES=$(find src/components -type f -name "*.vue" | wc -l | tr -d ' ')
NON_PASCAL_CASE=$(find src/components -type f -name "*.vue" | grep -v -E '^src/components/[A-Z][A-Za-z0-9]*\.vue$' | wc -l | tr -d ' ')

if [ "$NON_PASCAL_CASE" -gt 0 ]; then
  echo -e "${YELLOW}⚠️ Phát hiện $NON_PASCAL_CASE/$COMPONENT_FILES component không đặt tên theo PascalCase:${NC}"
  find src/components -type f -name "*.vue" | grep -v -E '^src/components/[A-Z][A-Za-z0-9]*\.vue$' | head -10
  echo -e "${GREEN}💡 Gợi ý: Đổi tên các component theo quy ước PascalCase${NC}"
fi

# Tìm v-for không có :key
echo -e "${BLUE}🔍 Kiểm tra v-for không có :key...${NC}"
VFOR_WITHOUT_KEY=$(grep -r --include="*.vue" -A 1 "v-for=" src | grep -v ":key=" | grep -B 1 "v-for=" | grep "v-for=" | wc -l | tr -d ' ')

if [ "$VFOR_WITHOUT_KEY" -gt 0 ]; then
  echo -e "${YELLOW}⚠️ Phát hiện $VFOR_WITHOUT_KEY trường hợp v-for không có :key:${NC}"
  grep -r --include="*.vue" -n -A 1 "v-for=" src | grep -v ":key=" | grep -B 1 "v-for=" | grep "v-for=" | head -10
  echo -e "${GREEN}💡 Gợi ý: Thêm :key cho tất cả v-for, ví dụ: v-for=\"item in items\" :key=\"item.id\"${NC}"
fi

# Tìm các template không có thẻ root duy nhất
echo -e "${BLUE}🔍 Kiểm tra cấu trúc template...${NC}"
MULTIPLE_ROOT=$(grep -r --include="*.vue" -A 5 "<template>" src | grep -v "<template><div" | grep -v "<template><section" | grep -v "<template><form" | grep -v "<template>$" | wc -l | tr -d ' ')

if [ "$MULTIPLE_ROOT" -gt 0 ]; then
  echo -e "${YELLOW}⚠️ Có thể có $MULTIPLE_ROOT component với nhiều thẻ root:${NC}"
  echo -e "${GREEN}💡 Gợi ý: Đảm bảo mỗi template chỉ có một thẻ root${NC}"
fi

# Kiểm tra lỗi unused imports
echo -e "${BLUE}🔍 Kiểm tra unused imports...${NC}"
echo -e "${GREEN}💡 Gợi ý: Chạy lệnh 'npm run lint' để tìm và sửa unused imports${NC}"

# Kiểm tra và báo cáo kết quả
echo -e "${BLUE}📊 Tổng kết:${NC}"
echo -e "  ${YELLOW}⚠️ Console.log chưa comment: $CONSOLE_LOGS${NC}"
echo -e "  ${YELLOW}⚠️ Component không theo PascalCase: $NON_PASCAL_CASE${NC}"
echo -e "  ${YELLOW}⚠️ v-for không có :key: $VFOR_WITHOUT_KEY${NC}"
echo -e "  ${YELLOW}⚠️ Template có thể có nhiều thẻ root: $MULTIPLE_ROOT${NC}"

if [ $((CONSOLE_LOGS + NON_PASCAL_CASE + VFOR_WITHOUT_KEY + MULTIPLE_ROOT)) -eq 0 ]; then
  echo -e "${GREEN}✅ Tuyệt vời! Không phát hiện warning phổ biến nào.${NC}"
else
  echo -e "${YELLOW}⚠️ Đã phát hiện một số warning. Hãy xem xét sửa chúng để cải thiện chất lượng code.${NC}"
fi

echo -e "${GREEN}✅ Phân tích hoàn tất!${NC}"
