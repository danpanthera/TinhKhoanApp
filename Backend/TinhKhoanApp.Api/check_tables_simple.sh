#!/bin/bash

echo "🔍 KIỂM TRA 8 BẢNG CORE DATA TABLES"
echo "=================================="

# Gọi API để thực thi SQL query đơn giản
curl -s -X POST "http://localhost:5055/api/TestData/execute-sql" \
-H "Content-Type: application/json" \
-d '{
  "query": "SELECT name FROM sys.tables WHERE name IN ('"'"'DP01'"'"', '"'"'LN01'"'"', '"'"'LN03'"'"', '"'"'GL01'"'"', '"'"'GL41'"'"', '"'"'DPDA'"'"', '"'"'EI01'"'"', '"'"'RR01'"'"') ORDER BY name"
}' | jq '.'

echo ""
echo "✅ Expected: 8 bảng - DP01(63), LN01(79), LN03(17), GL01(27), GL41(13), DPDA(13), EI01(24), RR01(25)"
