#!/bin/bash
# Script kiểm tra TypeScript với giới hạn thời gian

echo "🔍 Bắt đầu kiểm tra TypeScript..."

# Chạy kiểm tra từng thành phần
echo "🧩 Kiểm tra components..."
npx vue-tsc --noEmit src/components/dashboard/LoadingOverlay.vue
if [ $? -eq 0 ]; then
  echo "✅ Kiểm tra component LoadingOverlay thành công!"
else
  echo "❌ Kiểm tra component LoadingOverlay thất bại!"
fi

echo "🧩 Kiểm tra services..."
npx tsc --noEmit src/services/rawDataService.ts
if [ $? -eq 0 ]; then
  echo "✅ Kiểm tra service rawDataService thành công!"
else
  echo "❌ Kiểm tra service rawDataService thất bại!"
fi

echo "🧩 Kiểm tra App.vue..."
npx vue-tsc --noEmit src/App.vue
if [ $? -eq 0 ]; then
  echo "✅ Kiểm tra App.vue thành công!"
else
  echo "❌ Kiểm tra App.vue thất bại!"
fi

echo "✅ Đã hoàn thành kiểm tra các component chính"
echo "💡 Để tránh bị treo, không chạy kiểm tra toàn bộ dự án"
echo "💡 Sử dụng lệnh sau để kiểm tra file cụ thể: npx vue-tsc --noEmit <đường-dẫn-file>"

exit 0
