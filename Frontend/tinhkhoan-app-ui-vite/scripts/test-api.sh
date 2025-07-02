#!/bin/bash

# Script test API endpoints cho TinhKhoanApp
# Usage: ./scripts/test-api.sh

echo "🧪 TESTING TINHKHOANAPP API ENDPOINTS"
echo "=================================="

# Kiểm tra backend có chạy không
echo "🔍 Checking backend server..."
if curl -s http://localhost:5055/health >/dev/null 2>&1; then
    echo "✅ Backend server is running on http://localhost:5055"
else
    echo "❌ Backend server is not running. Please start backend first."
    exit 1
fi

echo ""
echo "📡 Testing API endpoints..."
echo ""

# Test 1: Nguồn vốn calculation
echo "1️⃣ Testing Nguồn vốn calculation (Unit 7800):"
echo "POST /api/NguonVon/calculate"
curl -X POST "http://localhost:5055/api/NguonVon/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "7800",
    "targetDate": "2025-04-30T00:00:00.000Z",
    "dateType": "month"
  }' | jq . 2>/dev/null || echo "❌ Failed to parse JSON response"

echo ""
echo ""

# Test 2: Nguồn vốn calculation với unit khác
echo "2️⃣ Testing Nguồn vốn calculation (Unit 7801):"
echo "POST /api/NguonVon/calculate"
curl -X POST "http://localhost:5055/api/NguonVon/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "7801",
    "targetDate": "2025-04-30T00:00:00.000Z",
    "dateType": "month"
  }' | jq . 2>/dev/null || echo "❌ Failed to parse JSON response"

echo ""
echo ""

# Test 3: Test với unit không tồn tại
echo "3️⃣ Testing with invalid unit (should return error):"
echo "POST /api/NguonVon/calculate"
curl -X POST "http://localhost:5055/api/NguonVon/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "9999",
    "targetDate": "2025-04-30T00:00:00.000Z",
    "dateType": "month"
  }' | jq . 2>/dev/null || echo "❌ Failed to parse JSON response"

echo ""
echo ""
echo "🎯 API Testing completed!"
echo "=================================="
