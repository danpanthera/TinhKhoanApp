#!/bin/bash
echo "🔍 TinhKhoanApp Status:"

if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
    echo "✅ Database: Running"
else
    echo "❌ Database: Not Running"
fi

if curl -s --max-time 3 http://localhost:5055/health > /dev/null 2>&1; then
    echo "✅ Backend: Running"
else
    echo "❌ Backend: Not Running"
fi

if curl -s --max-time 3 http://localhost:3000 > /dev/null 2>&1; then
    echo "✅ Frontend: Running"
else
    echo "❌ Frontend: Not Running"
fi
