#!/bin/bash
# =============================================================================
# VS CODE TASKS FIX SCRIPT - TinhKhoanApp
# =============================================================================
# Giải pháp cho vấn đề VS Code tasks bị treo

echo "🔧 FIXING VS CODE TASKS ISSUES..."

# 1. Kill tất cả processes có thể conflict
echo "1️⃣ Stopping all running processes..."
pkill -f "dotnet.*" 2>/dev/null || true
pkill -f "npm.*" 2>/dev/null || true
pkill -f "node.*" 2>/dev/null || true

# 2. Clear VS Code workspace state (nếu có)
echo "2️⃣ Clearing VS Code cache..."
if [ -d ".vscode/.cache" ]; then
    rm -rf .vscode/.cache
fi

# 3. Reset terminal state
echo "3️⃣ Resetting terminal state..."
exec $SHELL

echo "✅ VS Code tasks should work now!"
echo ""
echo "🚀 ALTERNATIVE: Use direct scripts instead:"
echo "   ./start-backend.sh    - Start backend API"
echo "   ./start-frontend.sh   - Start frontend dev server"
echo ""
