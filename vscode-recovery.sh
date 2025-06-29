#!/bin/bash

# 🚨 SCRIPT KHÔI PHỤC VS CODE SAU SỰ CỐ PROJECTSYSTEMSERVERFAULT
# Sử dụng khi VS Code C# extension bị crash

echo "🚨 VS Code Recovery Script - Xử lý ProjectSystemServerFault"
echo "=================================================="

# 1. Kill tất cả processes liên quan
echo "🔧 Step 1: Killing related processes..."
pkill -f "Microsoft.CodeAnalysis" 2>/dev/null || true
pkill -f "dotnet.*ServiceHub" 2>/dev/null || true  
pkill -f "dotnet.*LanguageServer" 2>/dev/null || true
pkill -f "OmniSharp" 2>/dev/null || true
echo "✅ Processes killed"

# 2. Dọn dẹp cache
echo "🔧 Step 2: Cleaning cache..."
rm -rf "/Users/nguyendat/Library/Application Support/Code/logs/*" 2>/dev/null || true
rm -rf "/Users/nguyendat/Library/Application Support/Code/CachedExtensions" 2>/dev/null || true
rm -rf "/Users/nguyendat/.nuget/packages/.tools" 2>/dev/null || true
echo "✅ Cache cleaned"

# 3. Backup workspace
echo "🔧 Step 3: Backup workspace..."
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp
./fast-commit.sh
echo "✅ Workspace backed up"

# 4. Check memory usage
echo "🔧 Step 4: Memory check..."
free_mem=$(vm_stat | grep "Pages free" | awk '{print $3}' | sed 's/\.//')
echo "📊 Free memory: $free_mem pages"

# 5. Restart backend safely
echo "🔧 Step 5: Restart backend..."
lsof -ti:5055 | xargs kill -9 2>/dev/null || true
cd Backend/TinhKhoanApp.Api
nohup dotnet run --urls=http://localhost:5055 > /tmp/backend.log 2>&1 &
echo "✅ Backend restarted (check /tmp/backend.log)"

echo "=================================================="
echo "🎉 Recovery completed!"
echo "📝 Recommendations:"
echo "   1. Restart VS Code completely"
echo "   2. Open only Backend folder (not full workspace)"
echo "   3. Disable C# DevKit temporarily if issues persist"
echo "   4. Monitor memory usage: Activity Monitor"
echo "=================================================="
