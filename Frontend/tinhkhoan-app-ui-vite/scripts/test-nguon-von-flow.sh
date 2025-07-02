#!/bin/bash

# ==============================================================================
# TEST SCRIPT: Nguồn vốn calculation flow
# ==============================================================================

echo "🔍 TESTING NGUỒN VỐN CALCULATION FLOW"
echo "===================================="

API_BASE="http://localhost:5055/api/NguonVon/calculate"
DATE="2024-12-01T00:00:00Z"

echo ""
echo "1️⃣ Testing ALL units (Toàn tỉnh)..."
echo "-----------------------------------"
curl -s -X POST $API_BASE \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "ALL",
    "targetDate": "'$DATE'",
    "dateType": "month"
  }' | jq '.data | {unitCode, unitName, totalBalance, recordCount}'

echo ""
echo "2️⃣ Testing Hội sở (7800)..."
echo "----------------------------"
curl -s -X POST $API_BASE \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "7800",
    "targetDate": "'$DATE'",
    "dateType": "month"
  }' | jq '.data | {unitCode, unitName, totalBalance, recordCount}'

echo ""
echo "3️⃣ Testing Chi nhánh Bình Lư (7801)..."
echo "---------------------------------------"
curl -s -X POST $API_BASE \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "7801",
    "targetDate": "'$DATE'",
    "dateType": "month"
  }' | jq '.data | {unitCode, unitName, totalBalance, recordCount}'

echo ""
echo "4️⃣ Testing invalid unit code..."
echo "------------------------------"
curl -s -X POST $API_BASE \
  -H "Content-Type: application/json" \
  -d '{
    "unitCode": "9999",
    "targetDate": "'$DATE'",
    "dateType": "month"
  }' | jq '.success'

echo ""
echo "✅ NGUỒN VỐN FLOW TEST COMPLETED!"
echo "================================="
echo ""
echo "📋 NEXT STEPS:"
echo "1. Mở browser: http://localhost:4173"
echo "2. Chọn 'Toàn tỉnh (Tổng hợp)' trong dropdown"
echo "3. Nhấn nút 'Nguồn vốn' để test UI"
echo "4. Kiểm tra kết quả hiển thị"
echo ""
