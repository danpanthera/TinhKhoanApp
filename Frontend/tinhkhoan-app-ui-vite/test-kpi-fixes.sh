#!/bin/bash

echo "🧪 Testing Unit KPI Assignment Fixes"
echo "=================================="

echo ""
echo "📊 1. Testing API Endpoints:"
echo "----------------------------"

echo "🔍 Testing HoiSo table (should have 11 indicators):"
HOISO_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignment/table/HoiSo" | jq '.indicators["$values"] | length')
echo "   HoiSo indicators: $HOISO_COUNT"

echo "🔍 Testing GiamdocCnl2 table (should have 11 indicators):"
CNL2_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignment/table/GiamdocCnl2" | jq '.indicators["$values"] | length')
echo "   GiamdocCnl2 indicators: $CNL2_COUNT"

echo ""
echo "📋 2. API Response Sample (HoiSo):"
echo "-----------------------------------"
echo "Table info:"
curl -s "http://localhost:5055/api/KpiAssignment/table/HoiSo" | jq '{id: .id, tableName: .tableName, tableType: .tableType, indicatorCount: (.indicators["$values"] | length)}'

echo ""
echo "First 3 indicators:"
curl -s "http://localhost:5055/api/KpiAssignment/table/HoiSo" | jq '.indicators["$values"][0:3] | .[] | {id: .id, indicatorName: .indicatorName, maxScore: .maxScore, unit: .unit}'

echo ""
echo "📋 3. API Response Sample (GiamdocCnl2):"
echo "----------------------------------------"
echo "Table info:"
curl -s "http://localhost:5055/api/KpiAssignment/table/GiamdocCnl2" | jq '{id: .id, tableName: .tableName, tableType: .tableType, indicatorCount: (.indicators["$values"] | length)}'

echo ""
echo "First 3 indicators:"
curl -s "http://localhost:5055/api/KpiAssignment/table/GiamdocCnl2" | jq '.indicators["$values"][0:3] | .[] | {id: .id, indicatorName: .indicatorName, maxScore: .maxScore, unit: .unit}'

echo ""
echo "✅ 4. Verification Results:"
echo "---------------------------"
if [ "$HOISO_COUNT" -eq 11 ] && [ "$CNL2_COUNT" -eq 11 ]; then
    echo "   ✅ Both tables have correct number of indicators (11 each)"
    echo "   ✅ API endpoints working correctly"
    echo ""
    echo "🎯 Expected Frontend Behavior:"
    echo "   - When selecting CNL1 branch → Load HoiSo table (11 indicators)"  
    echo "   - When selecting CNL2 branch → Load GiamdocCnl2 table (11 indicators)"
    echo "   - When clicking refresh → Load KPI based on currently selected branch"
    echo ""
    echo "🔧 Frontend Changes Made:"
    echo "   - Fixed getKpiIndicatorsByTableType() to properly handle \$values"
    echo "   - Updated loadKpiIndicators() to load based on selected branch"
    echo "   - Enhanced onBranchChange() with better logging"
    echo "   - Added debugging console logs"
else
    echo "   ❌ Issue detected:"
    echo "      HoiSo count: $HOISO_COUNT (expected: 11)"
    echo "      GiamdocCnl2 count: $CNL2_COUNT (expected: 11)"
fi

echo ""
echo "🚀 Next Steps:"
echo "--------------"
echo "1. Open browser console at http://localhost:3000/unit-kpi-assignment"
echo "2. Select different branches and watch console logs"
echo "3. Click 'Làm mới dữ liệu' and verify KPI count"
echo "4. Click 'Tạo giao khoán mới' and check if KPI pre-loaded"
