#!/bin/bash
# 🚀 TinhKhoan Backend API - Local Start Script
# Usage: ./start_backend.sh (from Backend/TinhKhoanApp.Api directory OR anywhere)

echo "🚀 Starting TinhKhoan Backend API..."

# Auto-navigate to correct directory if not already there
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "📂 Current directory: $(pwd)"

# Kiểm tra nếu đang ở đúng thư mục
if [ ! -f "TinhKhoanApp.Api.csproj" ]; then
    echo "❌ Error: Cannot find TinhKhoanApp.Api.csproj!"
    echo "💡 Script should be in Backend/TinhKhoanApp.Api directory"
    exit 1
fi

# Kiểm tra .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not installed"
    exit 1
fi

echo "🔨 Building project..."
dotnet build --configuration Debug --verbosity minimal

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "🌐 Starting backend on http://localhost:5055"
    echo "🛑 Press Ctrl+C to stop the server"
    echo ""
    dotnet run --urls=http://localhost:5055
else
    echo "❌ Build failed!"
    exit 1
fi
