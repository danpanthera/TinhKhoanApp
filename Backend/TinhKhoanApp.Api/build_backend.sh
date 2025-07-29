#!/bin/bash
# 🔨 TinhKhoan Backend API - Build Script
# Script này chỉ build backend để kiểm tra lỗi

echo "🔨 Bắt đầu build TinhKhoan Backend API..."

# Auto-navigate to correct directory if not already there
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "📂 Thư mục hiện tại: $(pwd)"

# Kiểm tra nếu đang ở đúng thư mục
if [ ! -f "TinhKhoanApp.Api.csproj" ]; then
    echo "❌ Lỗi: Không tìm thấy TinhKhoanApp.Api.csproj!"
    echo "💡 Script nên được chạy trong thư mục Backend/TinhKhoanApp.Api"
    exit 1
fi

# Kiểm tra .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK không được cài đặt"
    exit 1
fi

echo "🧹 Dọn dẹp các build cũ..."
dotnet clean --verbosity minimal

echo "🔨 Building project..."
dotnet build --configuration Debug --verbosity normal

if [ $? -eq 0 ]; then
    echo "✅ Build thành công!"
    echo "💡 Không phát hiện lỗi."
else
    echo "❌ Build thất bại!"
    echo "💡 Vui lòng xem các lỗi phía trên và sửa chúng."
    exit 1
fi
