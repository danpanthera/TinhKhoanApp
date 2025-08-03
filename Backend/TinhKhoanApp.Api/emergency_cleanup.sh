#!/bin/bash
# Emergency cleanup for hanging processes

echo "🚨 EMERGENCY CLEANUP - STOPPING ALL PROCESSES"
echo "============================================="

# Kill dotnet processes
echo "Killing dotnet processes..."
pkill -f "dotnet.*run" 2>/dev/null
pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null

# Kill node/npm processes
echo "Killing node/npm processes..."
pkill -f "npm.*dev" 2>/dev/null
pkill -f "vite" 2>/dev/null
pkill -f "node.*vite" 2>/dev/null

# Force kill by ports
echo "Force killing processes on ports 5055 and 3000..."
lsof -ti:5055 | xargs kill -9 2>/dev/null
lsof -ti:3000 | xargs kill -9 2>/dev/null

sleep 3

echo "✅ Cleanup completed!"
echo "Run ./start_full_app.sh to restart everything"
