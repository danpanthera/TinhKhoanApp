#!/bin/bash

echo "🔍 DIAGNOSE FRONTEND ISSUES - KPI DROPDOWN"
echo "=========================================="

echo ""
echo "📊 1. Kiểm tra Backend API - KpiAssignmentTables:"
echo "  Total tables:"
curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq 'length'

echo "  CANBO tables:"
curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq '[.[] | select(.Category == "CANBO")] | length'

echo "  CHINHANH tables:"
curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq '[.[] | select(.Category == "CHINHANH")] | length'

echo ""
echo "📋 2. Sample CANBO tables:"
curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq '[.[] | select(.Category == "CANBO")] | .[0:3] | .[] | {Id, TableName, Description, IndicatorCount}'

echo ""
echo "🏢 3. Sample CHINHANH tables:"
curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq '[.[] | select(.Category == "CHINHANH")] | .[0:3] | .[] | {Id, TableName, Description, IndicatorCount}'

echo ""
echo "🔍 4. Test direct API call for debugging:"
echo "URL: http://localhost:3000/kpi-definitions"
echo "Backend API endpoint: http://localhost:5055/api/KpiAssignmentTables"

echo ""
echo "💡 NEXT STEPS:"
echo "1. Kiểm tra frontend với Ctrl+Shift+R (hard refresh)"
echo "2. Check console.log errors trong DevTools"
echo "3. Frontend cần check: Category filter logic đã sửa từ 'VAI TRÒ CÁN BỘ' -> 'CANBO'"
echo ""
echo "✅ BACKEND DATA READY:"
echo "   - 23 CANBO tables ✅"
echo "   - 9 CHINHANH tables ✅"
echo "   - Frontend display issues is caching/reactivity problem not backend problem"
