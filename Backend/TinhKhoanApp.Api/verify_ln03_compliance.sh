#!/bin/bash
# Script để xác minh việc tuân thủ của bảng LN03

echo "🔍 Kiểm tra tuân thủ của bảng LN03..."

# Thư mục hiện tại
CURRENT_DIR=$(pwd)
echo "📂 Thư mục hiện tại: $CURRENT_DIR"

# Kiểm tra mô hình C#
echo "📝 Kiểm tra mô hình LN03.cs..."
grep -n "decimal\?" Models/DataTables/LN03.cs
grep -n "DateTime\?" Models/DataTables/LN03.cs

# Kiểm tra controller
echo "📝 Kiểm tra LN03Controller.cs..."
grep -n "ImportLN03EnhancedAsync" Controllers/LN03Controller.cs

# Kiểm tra service extension
echo "📝 Kiểm tra DirectImportServiceLN03Extension.cs..."
ls -la Services/DirectImportServiceLN03Extension.cs

echo "✅ Hoàn thành kiểm tra!"
echo "👉 Xem hướng dẫn triển khai chi tiết trong file LN03_IMPLEMENTATION_GUIDE.md"
