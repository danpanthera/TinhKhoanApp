#!/bin/bash

# ================================================================
# UNIVERSAL FRONTEND STARTUP SCRIPT
# Auto-detects và khởi động frontend từ bất kỳ thư mục nào
# ================================================================

set -e

echo "🎨 Starting Khoan Frontend..."

# Detect script directory để tìm project root
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"
FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"

echo "📁 Project root: $PROJECT_ROOT"
echo "📁 Frontend directory: $FRONTEND_DIR"

# Validate frontend directory
if [ ! -d "$FRONTEND_DIR" ]; then
    echo "❌ Frontend directory not found: $FRONTEND_DIR"
    exit 1
fi

if [ ! -f "$FRONTEND_DIR/package.json" ]; then
    echo "❌ package.json not found in frontend directory"
    exit 1
fi

# Clean up any existing frontend processes
echo "🔄 Stopping any existing frontend processes..."
pkill -f "vite.*3000" || true
pkill -f "node.*vite" || true
sleep 1

# Navigate to frontend directory và start
echo "🚀 Starting frontend development server..."
cd "$FRONTEND_DIR"

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
fi

# Start frontend server
echo "✅ Frontend startup completed!"
echo "🌐 Frontend URL: http://localhost:3000"
echo "🛑 Press Ctrl+C to stop the server"

# Start the development server
npm run dev -- --host 0.0.0.0 --port 3000
