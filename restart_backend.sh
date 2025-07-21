#!/bin/bash
# 🔄 TinhKhoan App - Backend Restart Script
echo "🔄 Restarting TinhKhoan Backend API..."
cd Backend/TinhKhoanApp.Api
echo "🔨 Building backend..."
dotnet build --configuration Debug --verbosity minimal
if [ $? -eq 0 ]; then
    echo "✅ Build successful! Starting backend..."
    dotnet run --urls=http://localhost:5055
else
    echo "❌ Build failed!"
    exit 1
fi
