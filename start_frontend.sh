#!/bin/bash
# 🎨 TinhKhoan App - Frontend Startup Script (Root)
# Usage: ./start_frontend.sh (from project root)

echo "🎨 Starting TinhKhoan Frontend UI..."

# Navigate to Frontend directory
cd Frontend/tinhkhoan-app-ui-vite

# Check if Frontend directory exists
if [ ! -d "." ]; then
    echo "❌ Frontend directory not found!"
    exit 1
fi

# Make sure local script is executable
chmod +x start_frontend.sh

# Run the local frontend script
echo "📦 Running local frontend startup script..."
./start_frontend.sh

echo "✅ Frontend startup script completed!"
