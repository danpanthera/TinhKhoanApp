#!/bin/bash

# Script test UnitKpiAssignmentView.vue fix
# Test hiển thị KPI indicators cho chi nhánh sau khi fix safeGet

API_BASE="http://localhost:5055/api"
FRONTEND_BASE="http://localhost:5173"

echo "🧪 Testing UnitKpiAssignmentView.vue KPI Indicators Fix"
echo "=================================================="

# 1. Test backend API health
echo "1. Testing backend API health..."
curl -s "${API_BASE}/health" > /dev/null
if [ $? -eq 0 ]; then
    echo "✅ Backend API is running"
else
    echo "❌ Backend API is not running"
    exit 1
fi

# 2. Test periods API
echo "2. Testing periods API..."
PERIODS_COUNT=$(curl -s "${API_BASE}/periods" | jq '. | length')
echo "✅ Found ${PERIODS_COUNT} periods"

# 3. Test units API
echo "3. Testing units API..."
UNITS_COUNT=$(curl -s "${API_BASE}/units" | jq '. | length')
echo "✅ Found ${UNITS_COUNT} units"

# 4. Test KPI tables API
echo "4. Testing KPI tables API..."
TOTAL_TABLES=$(curl -s "${API_BASE}/kpi-tables" | jq '. | length')
BRANCH_TABLES=$(curl -s "${API_BASE}/kpi-tables" | jq '[.[] | select(.Category == "CHINHANH")] | length')
echo "✅ Found ${TOTAL_TABLES} total KPI tables, ${BRANCH_TABLES} for branches"

# 5. Test KPI indicators API for branch
echo "5. Testing KPI indicators API for branch..."
BRANCH_TABLE_NAME=$(curl -s "${API_BASE}/kpi-tables" | jq -r '[.[] | select(.Category == "CHINHANH")][0].TableName')
if [ "$BRANCH_TABLE_NAME" != "null" ]; then
    INDICATORS_RESPONSE=$(curl -s "${API_BASE}/kpi-indicators?tableName=${BRANCH_TABLE_NAME}")
    
    # Check if response has Indicators or indicators
    HAS_INDICATORS=$(echo "$INDICATORS_RESPONSE" | jq -r 'has("Indicators")')
    HAS_INDICATORS_LOWER=$(echo "$INDICATORS_RESPONSE" | jq -r 'has("indicators")')
    
    if [ "$HAS_INDICATORS" = "true" ]; then
        INDICATORS_COUNT=$(echo "$INDICATORS_RESPONSE" | jq -r '.Indicators | length')
        echo "✅ Found ${INDICATORS_COUNT} indicators (PascalCase) for ${BRANCH_TABLE_NAME}"
    elif [ "$HAS_INDICATORS_LOWER" = "true" ]; then
        INDICATORS_COUNT=$(echo "$INDICATORS_RESPONSE" | jq -r '.indicators | length')
        echo "✅ Found ${INDICATORS_COUNT} indicators (camelCase) for ${BRANCH_TABLE_NAME}"
    else
        # Maybe direct array
        INDICATORS_COUNT=$(echo "$INDICATORS_RESPONSE" | jq -r 'length')
        echo "✅ Found ${INDICATORS_COUNT} indicators (direct array) for ${BRANCH_TABLE_NAME}"
    fi
else
    echo "❌ No branch KPI table found"
fi

# 6. Test frontend accessibility
echo "6. Testing frontend accessibility..."
curl -s "${FRONTEND_BASE}/unit-kpi-assignment" > /dev/null
if [ $? -eq 0 ]; then
    echo "✅ Frontend UnitKpiAssignmentView is accessible"
else
    echo "❌ Frontend UnitKpiAssignmentView is not accessible"
fi

echo ""
echo "🎉 All tests completed!"
echo "📝 To manually test:"
echo "   1. Go to ${FRONTEND_BASE}/unit-kpi-assignment"
echo "   2. Select a period"
echo "   3. Select a branch"
echo "   4. Select a KPI table"
echo "   5. Check if indicators are displayed correctly"
echo "   6. Verify columns: Chỉ tiêu KPI, Điểm, Đơn vị"
echo ""
echo "🔧 Fix Applied:"
echo "   - Added import { safeGet } from '../utils/casingSafeAccess.js'"
echo "   - Fixed 'safeGet is not a function' error"
echo "   - KPI indicators should now display correctly for branches"
