#!/bin/bash

# Script kiểm tra type an toàn cho dự án
# Thực hiện kiểm tra type và báo cáo lỗi

echo "🔍 Bắt đầu kiểm tra type an toàn..."

# Chuyển đến thư mục frontend
cd "$(dirname "$0")"

# Kiểm tra file tsconfig.json
if [ ! -f "tsconfig.json" ]; then
  echo "❌ Không tìm thấy file tsconfig.json!"
  exit 1
fi

# Kiểm tra type với vue-tsc
echo "🔍 Đang chạy vue-tsc để kiểm tra type..."
npx vue-tsc --noEmit

if [ $? -eq 0 ]; then
  echo "✅ Kiểm tra type hoàn tất: Không có lỗi type!"
else
  echo "⚠️ Phát hiện lỗi type. Vui lòng sửa các lỗi trên."

  # Phân tích các lỗi phổ biến
  echo "🔍 Đang phân tích các lỗi phổ biến..."

  MISSING_TYPE_COUNT=$(npx vue-tsc --noEmit 2>&1 | grep -c "has no type specified")
  ANY_TYPE_COUNT=$(npx vue-tsc --noEmit 2>&1 | grep -c "any")
  NULL_ERROR_COUNT=$(npx vue-tsc --noEmit 2>&1 | grep -c "null")

  echo "📊 Thống kê lỗi:"
  echo "  - Biến thiếu type: $MISSING_TYPE_COUNT"
  echo "  - Sử dụng type 'any': $ANY_TYPE_COUNT"
  echo "  - Lỗi null/undefined: $NULL_ERROR_COUNT"

  echo "💡 Gợi ý sửa lỗi:"
  echo "  1. Thêm chú thích type cho biến: const myVar: string = 'value'"
  echo "  2. Tránh sử dụng type 'any', thay bằng type cụ thể"
  echo "  3. Kiểm tra null/undefined trước khi sử dụng: if (myVar !== null) {...}"
fi

# Kiểm tra các file có vấn đề với TS
echo "🔍 Kiểm tra các file .js nên chuyển sang .ts..."
JS_FILES_WITH_TYPES=$(grep -r --include="*.js" "type\|interface\|enum" src 2>/dev/null | wc -l | tr -d ' ')

if [ "$JS_FILES_WITH_TYPES" -gt 0 ]; then
  echo "⚠️ Phát hiện $JS_FILES_WITH_TYPES file JS có chứa định nghĩa type. Nên chuyển sang .ts"
  grep -r --include="*.js" -l "type\|interface\|enum" src 2>/dev/null | head -5

  if [ "$JS_FILES_WITH_TYPES" -gt 5 ]; then
    echo "   ... và $(($JS_FILES_WITH_TYPES - 5)) file khác"
  fi
fi

echo "🔍 Kiểm tra hoàn tất!"
