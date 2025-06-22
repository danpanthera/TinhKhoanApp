#!/bin/bash

# Script kiểm tra tích hợp Raw Data Import/Preview/Delete trên port mới
echo "🧪 Bắt đầu kiểm tra tích hợp Raw Data..."

# Kiểm tra backend có hoạt động không
echo "🔍 Kiểm tra backend API trên port 5001..."
BACKEND_STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5001/api/rawdata)

if [ "$BACKEND_STATUS" = "200" ]; then
  echo "✅ Backend API đang hoạt động (status code: $BACKEND_STATUS)"
else
  echo "❌ Backend API không hoạt động (status code: $BACKEND_STATUS). Vui lòng kiểm tra lại."
  exit 1
fi

# Kiểm tra frontend có hoạt động không
echo "🔍 Kiểm tra frontend trên port 3001..."
FRONTEND_STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:3001)

if [ "$FRONTEND_STATUS" = "200" ]; then
  echo "✅ Frontend đang hoạt động (status code: $FRONTEND_STATUS)"
else
  echo "❌ Frontend không hoạt động (status code: $FRONTEND_STATUS). Vui lòng kiểm tra lại."
  exit 1
fi

# Kiểm tra API preview dữ liệu thô
echo "🔍 Kiểm tra API preview dữ liệu thô..."
PREVIEW_RESPONSE=$(curl -s http://localhost:5001/api/rawdata/1/preview)
PREVIEW_ROWS_COUNT=$(echo $PREVIEW_RESPONSE | grep -o '"previewRows"' | wc -l)

if [ "$PREVIEW_ROWS_COUNT" -gt 0 ]; then
  echo "✅ API preview dữ liệu thô hoạt động tốt"
else
  echo "❌ API preview dữ liệu thô không trả về kết quả mong đợi"
  echo "Response: $PREVIEW_RESPONSE"
fi

# Kiểm tra API xóa dữ liệu thô
echo "🔍 Kiểm tra API xóa dữ liệu thô..."
DELETE_RESPONSE=$(curl -s -X DELETE http://localhost:5001/api/rawdata/999)
DELETE_SUCCESS=$(echo $DELETE_RESPONSE | grep -o '"success"' | wc -l)

if [ "$DELETE_SUCCESS" -gt 0 ]; then
  echo "✅ API xóa dữ liệu thô hoạt động tốt"
else
  echo "❌ API xóa dữ liệu thô không trả về kết quả mong đợi"
  echo "Response: $DELETE_RESPONSE"
fi

echo "🏁 Kiểm tra hoàn tất!"
echo "📝 Kết luận: Backend và Frontend đã được khởi động lại thành công trên port mới"
echo "Backend: http://localhost:5001"
echo "Frontend: http://localhost:3001"
