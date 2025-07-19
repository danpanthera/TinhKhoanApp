#!/bin/bash
# 🎨 TinhKhoan App - Frontend Startup Script  
# Usage: ./start_frontend.sh (from project root)

echo "🎨 Starting TinhKhoan Frontend UI..."

# Navigate to Frontend directory
cd Frontend/tinhkhoan-app-ui-vite

# Check if Frontend directory exists
if [ ! -d "." ]; then
    echo "❌ Frontend directory not found!"
    exit 1
fi

# Check if the frontend script exists
if [ -f "./start_frontend.sh" ]; then
    echo "📦 Using local frontend startup script..."
    ./start_frontend.sh
else
    echo "📦 Starting frontend with npm run dev..."
    npm run dev
fi

echo "✅ Frontend startup script completed!"
