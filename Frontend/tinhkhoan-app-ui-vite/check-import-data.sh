#!/bin/bash

# Script kiểm tra dữ liệu sau khi import để debug vấn đề
echo "🔍 Kiểm tra dữ liệu import trong database..."

# Kiểm tra backend có chạy không
echo "1. Kiểm tra backend health..."
curl -s http://localhost:5055/health | head -10

echo -e "\n2. Kiểm tra ImportedDataRecords..."
curl -s "http://localhost:5055/api/ImportedData/records" | head -200

echo -e "\n3. Kiểm tra bảng DP01_New..."
curl -s "http://localhost:5055/api/ImportedData/data/DP01" | head -200

echo -e "\n4. Kiểm tra stats các bảng dữ liệu thô..."
curl -s "http://localhost:5055/api/ImportedData/stats" | head -200

echo -e "\n✅ Kiểm tra hoàn tất!"
