#!/bin/bash

# Script tối ưu commit để không chờ đợi lâu
# Tác giả: Nguyễn Đạt - Fullstack Developer

echo "🚀 Bắt đầu fast commit..."

# Chuyển đến thư mục dự án
cd "$(dirname "$0")"

# Commit backend với các file đã thay đổi
echo "📦 Commit Backend..."
cd Backend/TinhKhoanApp.Api
if [ -n "$(git status --porcelain)" ]; then
    git add --all
    git commit -m "🔧 Backend: Optimize and clean up - $(date '+%Y-%m-%d %H:%M')" --no-verify
    echo "✅ Backend committed successfully"
else
    echo "ℹ️ Backend: No changes to commit"
fi

# Commit frontend với các file đã thay đổi  
echo "🎨 Commit Frontend..."
cd ../../Frontend/tinhkhoan-app-ui-vite
if [ -n "$(git status --porcelain)" ]; then
    git add --all
    git commit -m "🎨 Frontend: Optimize and clean up - $(date '+%Y-%m-%d %H:%M')" --no-verify
    echo "✅ Frontend committed successfully"
else
    echo "ℹ️ Frontend: No changes to commit"
fi

# Commit root project
echo "📁 Commit Root Project..."
cd ../..
if [ -n "$(git status --porcelain)" ]; then
    git add --all
    git commit -m "⚡ Project: Fast optimization and cleanup - $(date '+%Y-%m-%d %H:%M')" --no-verify
    echo "✅ Root project committed successfully"
else
    echo "ℹ️ Root: No changes to commit"
fi

echo "🎉 Fast commit completed!"
