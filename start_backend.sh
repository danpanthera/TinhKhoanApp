#!/bin/bash
# 🚀 TinhKhoan App - Backend Startup Script
# Usage: ./start_backend.sh (from project root)

echo "🚀 Starting TinhKhoan Backend API..."

# Navigate to Backend directory
cd Backend/TinhKhoanApp.Api

# Check if Backend directory exists
if [ ! -d "." ]; then
    echo "❌ Backend directory not found!"
    exit 1
fi

# Check if the backend script exists
if [ -f "./start_backend.sh" ]; then
    echo "📦 Using local backend startup script..."
    ./start_backend.sh
else
    echo "📦 Starting backend with dotnet run..."
    dotnet run --urls=http://localhost:5055
fi

echo "✅ Backend startup script completed!"
