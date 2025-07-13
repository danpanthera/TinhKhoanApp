#!/bin/bash

# =====================================================
# FRONTEND STARTUP SCRIPT - JULY 13, 2025
# Script chuyên dụng để khởi động Frontend một cách an toàn
# KHÔNG BAO GIỜ SỬ DỤNG SHELL VS CODE ĐỂ CHẠY FRONTEND!
# =====================================================

echo "🎨 KHỞI ĐỘNG FRONTEND UI..."
echo "🧹 Dọn dẹp processes cũ..."

# Tìm và kill các process frontend cũ
pkill -f "vite"
pkill -f "npm run dev"
sleep 2

echo "📦 Kiểm tra dependencies..."
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
fi

echo "🔍 Kiểm tra kết nối backend..."
if curl -s http://localhost:5055/health > /dev/null; then
    echo "✅ Backend connection OK"
else
    echo "⚠️  Backend không phản hồi trên port 5055"
    echo "   Hãy chắc chắn backend đang chạy bằng ./start_backend.sh"
fi

echo "🚀 Starting frontend trên http://localhost:3000..."
echo "📝 Logs sẽ hiển thị bên dưới. Nhấn Ctrl+C để dừng."
echo "==========================================="

# Khởi động frontend với port cố định
npm run dev -- --port 3000 --host 0.0.0.0

echo ""
echo "🛑 Frontend đã dừng"
