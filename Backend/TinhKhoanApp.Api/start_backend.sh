#!/bin/bash

# =============================================================================
# START BACKEND API SCRIPT
# Script để chạy backend API cho TinhKhoanApp
# =============================================================================

echo "🚀 KHỞI ĐỘNG BACKEND API..."

# Kiểm tra thư mục hiện tại
if [ ! -f "TinhKhoanApp.Api.csproj" ]; then
    echo "❌ Không tìm thấy TinhKhoanApp.Api.csproj"
    echo "📍 Hãy chạy script từ thư mục Backend/TinhKhoanApp.Api"
    exit 1
fi

# Kiểm tra .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK chưa được cài đặt"
    exit 1
fi

# Kill backend cũ nếu có
echo "🧹 Dọn dẹp processes cũ..."
pkill -f "dotnet run" 2>/dev/null || true
pkill -f "TinhKhoanApp.Api" 2>/dev/null || true

# Chờ một chút để processes tắt hoàn toàn
sleep 2

# Kiểm tra database connection
echo "🔍 Kiểm tra kết nối database..."
if ! sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT 1" &>/dev/null; then
    echo "⚠️  Cảnh báo: Không thể kết nối database. Backend vẫn sẽ chạy nhưng có thể gặp lỗi."
else
    echo "✅ Database connection OK"
fi

# Clean và restore packages
echo "📦 Restore packages..."
dotnet restore

# Build project
echo "🔨 Build backend..."
if ! dotnet build --no-restore; then
    echo "❌ Build failed!"
    exit 1
fi

echo "✅ Build thành công!"

# Start backend
echo "🚀 Starting backend trên http://localhost:5055..."
echo "📝 Logs sẽ hiển thị bên dưới. Nhấn Ctrl+C để dừng."
echo "==========================================="

# Run với URLs được set sẵn
dotnet run --urls=http://localhost:5055
