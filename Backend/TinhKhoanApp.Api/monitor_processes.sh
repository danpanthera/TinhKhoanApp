#!/bin/bash
# Process Monitor for TinhKhoanApp

echo "🔍 TinhKhoanApp Process Monitor"
echo "==============================="

# Database
if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
    echo "✅ Database: Running"
else
    echo "❌ Database: Not Running"
fi

# Backend
if curl -s --max-time 3 http://localhost:5055/health > /dev/null 2>&1; then
    echo "✅ Backend: Running (http://localhost:5055)"
else
    echo "❌ Backend: Not Running"
fi

# Frontend
if curl -s --max-time 3 http://localhost:3000 > /dev/null 2>&1; then
    echo "✅ Frontend: Running (http://localhost:3000)"
else
    echo "❌ Frontend: Not Running"
fi

echo "==============================="
echo "Dotnet processes:"
ps aux | grep dotnet | grep -v grep | awk '{print $2, $11, $12, $13}' | head -5

echo "==============================="
echo "Node processes:"
ps aux | grep node | grep -v grep | awk '{print $2, $11, $12, $13}' | head -5
