#!/bin/bash
# 🎨 TinhKhoan App - Frontend Startup Script (Universal)
# Usage: ./start_frontend.sh (from anywhere)

echo "🎨 Starting TinhKhoan Frontend UI..."

# Get the absolute path of the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"
FRONTEND_DIR="$PROJECT_ROOT/Frontend/tinhkhoan-app-ui-vite"

echo "📁 Project root: $PROJECT_ROOT"
echo "📁 Frontend directory: $FRONTEND_DIR"

# Check if Frontend directory exists
if [ ! -d "$FRONTEND_DIR" ]; then
    echo "❌ Frontend directory not found at: $FRONTEND_DIR"
    exit 1
fi

# Navigate to Frontend directory
cd "$FRONTEND_DIR"

# Kill any existing node processes on port 3000
echo "🔄 Stopping any existing frontend processes..."
pkill -f "vite.*3000" || true
sleep 2

# Check if package.json exists
if [ ! -f "package.json" ]; then
    echo "❌ package.json not found in frontend directory"
    exit 1
fi

# Start the frontend
echo "🎨 Starting Vite dev server on port 3000..."
npm run dev &

# Wait a moment for the process to start
sleep 5

echo "✅ Frontend startup completed!"
echo "🌐 Frontend UI: http://localhost:3000"
