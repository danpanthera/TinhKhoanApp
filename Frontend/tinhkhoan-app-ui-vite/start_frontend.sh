#!/bin/bash
# 🎨 TinhKhoan Frontend UI - Local Start Script  
# Usage: ./start_frontend.sh (from Frontend/tinhkhoan-app-ui-vite directory OR anywhere)

echo "🎨 Starting TinhKhoan Frontend UI..."

# Auto-navigate to correct directory if not already there
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "📂 Current directory: $(pwd)"

# Kiểm tra nếu đang ở đúng thư mục
if [ ! -f "package.json" ]; then
    echo "❌ Error: Cannot find package.json!"
    echo "💡 Script should be in Frontend/tinhkhoan-app-ui-vite directory"
    exit 1
fi

# Kiểm tra Node.js
if ! command -v node &> /dev/null; then
    echo "❌ Node.js not installed"
    exit 1
fi

echo "🧹 Cleaning up old processes..."
# Tìm và kill các process frontend cũ
pkill -f "vite" 2>/dev/null || true
pkill -f "npm run dev" 2>/dev/null || true
sleep 2

echo "📦 Checking dependencies..."
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
fi

echo "🌐 Starting frontend on http://localhost:3000"
echo "🛑 Press Ctrl+C to stop the server"
echo ""
npm run dev
