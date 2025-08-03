#!/bin/bash
echo "🚨 EMERGENCY STOP"
pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null
pkill -f "npm.*dev" 2>/dev/null
pkill -f "vite" 2>/dev/null
lsof -ti:5055 | xargs kill -9 2>/dev/null || true
lsof -ti:3000 | xargs kill -9 2>/dev/null || true
echo "✅ All processes stopped"
