#!/bin/bash
# 🔄 Khoan App - Backend Restart Script
echo "🔄 Restarting Khoan Backend API..."
cd Backend/KhoanApp.Api
echo "🔨 Building backend..."
dotnet build --configuration Debug --verbosity minimal
if [ $? -eq 0 ]; then
    echo "✅ Build successful! Starting backend..."
    dotnet run --urls=http://localhost:5055
else
    echo "❌ Build failed!"
    exit 1
fi
