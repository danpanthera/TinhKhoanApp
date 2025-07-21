#!/bin/bash
# 🚀 TinhKhoan App - Backend Startup Script (Universal)
# Usage: ./start_backend.sh (from anywhere)

echo "🚀 Starting TinhKhoan Backend API..."

# Get the absolute path of the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"
BACKEND_DIR="$PROJECT_ROOT/Backend/TinhKhoanApp.Api"

echo "📁 Project root: $PROJECT_ROOT"
echo "📁 Backend directory: $BACKEND_DIR"

# Check if Backend directory exists
if [ ! -d "$BACKEND_DIR" ]; then
    echo "❌ Backend directory not found at: $BACKEND_DIR"
    exit 1
fi

# Navigate to Backend directory
cd "$BACKEND_DIR"

# Kill any existing dotnet processes on port 5055
echo "🔄 Stopping any existing backend processes..."
pkill -f "dotnet.*TinhKhoanApp.Api" || true
sleep 2

# Start the backend
echo "🚀 Starting .NET backend on port 5055..."
dotnet run --urls=http://0.0.0.0:5055 &

# Wait a moment for the process to start
sleep 3

echo "✅ Backend startup completed!"
echo "🌐 Backend API: http://localhost:5055"
echo "🔍 Health check: http://localhost:5055/health"
