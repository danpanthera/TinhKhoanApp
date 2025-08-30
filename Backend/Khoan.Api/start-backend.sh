#!/bin/bash
# Script khởi động backend đơn giản thay thế VS Code tasks

echo "🔨 Building Backend..."
dotnet build --verbosity minimal

if [ $? -eq 0 ]; then
    echo "✅ Build thành công!"
    echo "🚀 Starting Backend API on port 5055..."
    dotnet run --urls=http://localhost:5055
else
    echo "❌ Build failed!"
    exit 1
fi
