#!/bin/bash
# 🚀 TinhKhoan App - Backend Startup Script (Root)
# Usage: ./start_backend.sh (from project root)

echo "🚀 Starting TinhKhoan Backend API..."

# Navigate to Backend directory
cd Backend/TinhKhoanApp.Api

# Check if Backend directory exists
if [ ! -d "." ]; then
    echo "❌ Backend directory not found!"
    exit 1
fi

# Make sure local script is executable
chmod +x start_backend.sh

# Run the local backend script
echo "📦 Running local backend startup script..."
./start_backend.sh

echo "✅ Backend startup script completed!"
